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
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>New instance of IReTry</returns>
        IReTry<TSuccess, TFailure> ExecuteService<TSuccess, TFailure>(Func<TSuccess> action, TimeSpan timeout, int attempts = 1);

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>New instance of IReTry</returns>
        IReTry<TSuccess, TFailure> ExecuteService<TSuccess, TFailure>(Func<TSuccess> action, int attempts = 1);

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>New instance of IReTry</returns>
        IReTry<TResult, TResult> ExecuteService<TResult>(Func<TResult> action, TimeSpan timeout, int attempts = 1);

        /// <summary>
        /// Executes the service.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns>New instance of IReTry</returns>
        IReTry<TResult, TResult> ExecuteService<TResult>(Func<TResult> action, int attempts = 1);
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
        /// Gets the result.
        /// </summary>
        ReTryResult<TSuccess, TFailure> Result { get; }

        /// <summary>
        /// The if service fails then.
        /// </summary>
        /// <typeparam name="TExceptionType">Exception type to handle.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The ReTry.Service.IReTry`2[TSuccess -&gt; TSuccess, TFailure -&gt; TFailure].
        /// </returns>
        IReTry<TSuccess, TFailure> IfServiceFailsThen<TExceptionType>(Func<TExceptionType, TFailure> action) where TExceptionType : Exception;

        /// <summary>
        /// The if service fails then.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TExceptionType">Exception type to handle.</typeparam>
        /// <returns>
        /// The ReTry.Service.IReTry`2[TSuccess -&gt; TSuccess, TFailure -&gt; TFailure].
        /// </returns>
        IReTry<TSuccess, TFailure> IfServiceFailsThen<TExceptionType>(Action<TExceptionType> action) where TExceptionType : Exception;
    }
}
