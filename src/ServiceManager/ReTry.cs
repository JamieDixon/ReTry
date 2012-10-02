// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReTry.cs" company="Jamie Dixon Ltd">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReTry.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using global::ReTry.Service.Exceptions;

    /// <summary>
    ///  Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
    /// </summary>
    public class ReTry : IReTry
    {
        /// <summary>
        /// default attempts.
        /// </summary>
        private const int DefaultAttempts = 1;

        /// <summary>
        /// The execute service.
        /// </summary>
        /// <typeparam name="TSuccess">Success type.</typeparam>
        /// <typeparam name="TFailure">Failure type.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>The ReTry.IReTry.</returns>
        public IReTry<TSuccess, TFailure> ExecuteService<TSuccess, TFailure>(Func<TSuccess> action, TimeSpan timeout, int attempts = DefaultAttempts)
        {
            var serviceManager = new ReTry<TSuccess, TFailure>(action, timeout, attempts);
            return serviceManager;
        }

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>IRetry instance.</returns>
        public IReTry<TSuccess, TFailure> ExecuteService<TSuccess, TFailure>(Func<TSuccess> action, int attempts = DefaultAttempts)
        {
            return this.ExecuteService<TSuccess, TFailure>(action, TimeSpan.Zero, attempts);
        }

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>IRetry instance.</returns>
        public IReTry<TResult, TResult> ExecuteService<TResult>(Func<TResult> action, TimeSpan timeout, int attempts = DefaultAttempts)
        {
            return this.ExecuteService<TResult, TResult>(action, timeout, attempts);
        }

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>IRetry instance.</returns>
        public IReTry<TResult, TResult> ExecuteService<TResult>(Func<TResult> action, int attempts = DefaultAttempts)
        {
            return this.ExecuteService<TResult, TResult>(action, TimeSpan.Zero, attempts);
        }

        /// <summary>
        /// Clones the current instance of ReTry.
        /// </summary>
        /// <returns>
        /// An object reprisenting a clone of the current ReTry.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    /// <summary>
    /// The re try.
    /// </summary>
    /// <typeparam name="TSuccess">
    /// The success type.
    /// </typeparam>
    /// <typeparam name="TFailure">
    /// The failure type.
    /// </typeparam>
    public class ReTry<TSuccess, TFailure> : ReTry, IReTry<TSuccess, TFailure>
    {
        /// <summary>
        /// A collection of Funcs to execute if the service fails.
        /// </summary>
        private readonly IDictionary<Type, Func<Exception, TFailure>> failedFuncs = new Dictionary<Type, Func<Exception, TFailure>>();

        /// <summary>
        /// Gets or sets the millisecond timeout.
        /// </summary>
        private readonly TimeSpan timeOut;

        /// <summary>
        /// The number of times to re-try the service if it fails.
        /// </summary>
        private readonly int attemptsAllowed;

        /// <summary>
        /// The number of times the service call has been attempted.
        /// </summary>
        private int count;

        /// <summary>
        /// The exception that was thrown.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReTry{TSuccess,TFailure}"/> class.
        /// </summary>
        /// <param name="serviceFunc">The service func.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="attempts">The attempts.</param>
        internal ReTry(Func<TSuccess> serviceFunc, TimeSpan timeout, int attempts)
        {
            this.ServiceFunc = serviceFunc;
            this.attemptsAllowed = attempts;
            this.timeOut = timeout;
        }

        /// <summary>
        /// Gets or sets a value indicating whether failed.
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        /// Gets or sets the service func.
        /// </summary>
        private Func<TSuccess> ServiceFunc { get; set; }

        /// <summary>
        /// Gets or sets the result implimentation.
        /// </summary>
        private dynamic ResultImplimentation { get; set; }

        /// <summary>
        /// The if service fails.
        /// </summary>
        /// <typeparam name="TExceptionType">
        /// The exception type to be handle.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The ReTry.IReTry`1[TResult -&gt; TResult].
        /// </returns>
        public IReTry<TSuccess, TFailure> IfServiceFailsThen<TExceptionType>(Func<TExceptionType, TFailure> action) where TExceptionType : Exception
        {
            var serviceManager = (ReTry<TSuccess, TFailure>)this.Clone();

            // Convert the func to a func type we can store.
            var f = new Func<Exception, TFailure>(x => action(x as TExceptionType));

            serviceManager.failedFuncs.Add(typeof(TExceptionType), f);

            return serviceManager;
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <returns>
        /// The TResult.
        /// </returns>
        public ReTryResult<TSuccess, TFailure> Result()
        {
            this.count++;

            TFailure failureResult = default(TFailure);

            try
            {
                this.ResultImplimentation = this.ServiceFunc();
            }
            catch (Exception ex)
            {
                if (this.count < this.attemptsAllowed)
                {
                    if (this.timeOut != TimeSpan.Zero)
                    {
                        // Wait half a second then re-try.
                        Thread.Sleep(this.timeOut);
                    }

                    this.Result();
                }
                else
                {
                    this.exception = ex;

                    foreach (var possibleMethod in this.failedFuncs)
                    {
                        if (possibleMethod.Key.IsInstanceOfType(this.exception))
                        {
                            this.Failed = true;
                            failureResult = possibleMethod.Value(this.exception);
                            break;
                        }
                    }

                    if (!this.Failed)
                    {
                        throw new UnspecifiedExceptionHandler(this.exception);
                    }
                }
            }

            return new ReTryResult<TSuccess, TFailure>(this.ResultImplimentation, failureResult, this.exception);
        }
    }   
}
