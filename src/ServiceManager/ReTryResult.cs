// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReTryResult.cs" company="Jamie Dixon Ltd">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   The result of a ReTry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReTry.Service
{
    using System;

    /// <summary>
    /// The result of a ReTry.
    /// </summary>
    /// <typeparam name="TSuccess">
    /// Succes type.
    /// </typeparam>
    /// <typeparam name="TFailure">
    /// Failure type.
    /// </typeparam>
    public class ReTryResult<TSuccess, TFailure>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReTryResult{TSuccess,TFailure}"/> class.
        /// </summary>
        /// <param name="success">
        /// The success.
        /// </param>
        /// <param name="failure">
        /// The failure.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public ReTryResult(TSuccess success, TFailure failure, Exception exception)
        {
            this.Result = success;
            this.FailureResult = failure;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public TSuccess Result { get; private set; }

        /// <summary>
        /// Gets the failure result.
        /// </summary>
        public TFailure FailureResult { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an exception was thrown.
        /// </summary>
        public bool Failed
        {
            get
            {
                return this.Exception != null;
            }
        }
    }
}
