// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManager.cs" company="Jamie Dixon Ltd">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Service_Manager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///  Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        /// <summary>
        /// A collection of Funcs to execute if the service fails.
        /// </summary>
        private readonly List<Func<Exception, dynamic>> failedFuncs = new List<Func<Exception, dynamic>>();

        /// <summary>
        /// The number of times the service call has been attempted.
        /// </summary>
        private int count;

        /// <summary>
        /// The number of times to re-try the service if it fails.
        /// </summary>
        private int attemptsAllowed;

        /// <summary>
        /// The exception that was thrown.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// Gets or sets the millisecond timeout.
        /// </summary>
        private int millisecondTimeout;

        /// <summary>
        /// Gets or sets a value indicating whether failed.
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        /// Gets or sets the service func.
        /// </summary>
        private Func<dynamic> ServiceFunc { get; set; }

        /// <summary>
        /// Gets or sets the result implimentation.
        /// </summary>
        private dynamic ResultImplimentation { get; set; }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <returns>
        /// The TResult.
        /// </returns>
        public TResult Result<TResult>()
        {
            var result = this.Execute<TResult>();

            return result;
        }

        /// <summary>
        /// The execute service.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="attempts">
        /// The attempts.
        /// </param>
        /// <param name="timeoutMilliseconds">
        /// The timeout Milliseconds.
        /// </param>
        /// <returns>
        /// The ServiceManager.IServiceManager.
        /// </returns>
        public IServiceManager ExecuteService<TResult>(Func<TResult> action, int attempts, int timeoutMilliseconds = 500)
        {
            var serviceManager  = (ServiceManager)this.Clone();
            serviceManager.ServiceFunc = (dynamic)action;
            serviceManager.attemptsAllowed = attempts;
            serviceManager.millisecondTimeout = timeoutMilliseconds;
            return serviceManager;
        }

        /// <summary>
        /// The if service fails.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The ServiceManager.IServiceManager`1[TResult -&gt; TResult].
        /// </returns>
        public IServiceManager IfServiceFailsThen<TResult>(Func<Exception, TResult> action)
        {
            var serviceManager = (ServiceManager)this.Clone();
            serviceManager.failedFuncs.Add((dynamic)action);
            return serviceManager;
        }


        /// <summary>
        /// Clones the current instance of ServiceManager.
        /// </summary>
        /// <returns>
        /// An object reprisenting a clone of the current ServiceManager.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <returns>
        /// The TResult.
        /// </returns>
        private TResult Execute<TResult>()
        {
            return this
                .ExecuteServiceImplimentation(this.ServiceFunc, this.attemptsAllowed)
                .ResultImplimentation;
        }

        /// <summary>
        /// The execute failed funcs.
        /// </summary>
        private void ExecuteFailedFuncs()
        {
            foreach (var func in this.failedFuncs)
            {
                this.ResultImplimentation = func(this.exception);
            }
        }

        /// <summary>
        /// The execute service implimentation.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="attempts">
        /// The attempts.
        /// </param>
        /// <returns>
        /// The curent IServiceManager.
        /// </returns>
        private ServiceManager ExecuteServiceImplimentation<TResult>(Func<TResult> action, int attempts)
        {
            this.count++;

            try
            {
                this.ResultImplimentation = action();
            }
            catch (Exception ex)
            {
                if (this.count <= attempts)
                {
                    // Wait half a second then re-try.
                    Thread.Sleep(this.millisecondTimeout);

                    this.ExecuteServiceImplimentation(action, attempts);
                }
                else
                {
                    this.exception = ex;
                    this.ExecuteFailedFuncs();
                }
            }

            return this;
        }
    }
}
