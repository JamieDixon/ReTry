// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReTry.cs" company="Jamie Dixon">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReTry.Service
{
    using System;

    /// <summary>
    ///   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
    /// </summary>
    public interface IReTry : ICloneable 
    {
        /// <summary>
        /// The execute service implimentation.
        /// </summary>
        /// <typeparam name="TSuccess">
        /// The success return type.
        /// </typeparam>
        /// <typeparam name="TFailure">
        /// The failure return type.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="attempts">
        /// The attempts.
        /// </param>
        /// <param name="timeoutMilliseconds">
        /// The number of milliseconds between execute attempts.
        /// </param>
        /// <returns>
        /// The IReTry`1[TResult -&gt; TResult].
        /// </returns>
        IReTry<TSuccess, TFailure> ExecuteService<TSuccess, TFailure>(Func<TSuccess> action, int attempts, int timeoutMilliseconds);
    }

    /// <summary>
    /// The ReTry interface.
    /// </summary>
    /// <typeparam name="TSuccess">
    /// Success return type.
    /// </typeparam>
    /// <typeparam name="TFailure">
    /// Failure return type.
    /// </typeparam>
    public interface IReTry<TSuccess, TFailure> : IReTry
    {
        /// <summary>
        /// The if service fails then.
        /// </summary>
        /// <typeparam name="TExceptionType">
        /// Exception type to handle.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The ReTry.Service.IReTry`2[TSuccess -&gt; TSuccess, TFailure -&gt; TFailure].
        /// </returns>
        IReTry<TSuccess, TFailure> IfServiceFailsThen<TExceptionType>(Func<Exception, TFailure> action);

        /// <summary>
        /// The result.
        /// </summary>
        /// <returns>
        /// The ReTry.Service.ReTryResult.
        /// </returns>
        ReTryResult<TSuccess, TFailure> Result();
    }
}
