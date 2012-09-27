// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceManager.cs" company="Jamie Dixon">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Service_Manager
{
    using System;

    /// <summary>
    ///   Allows a service to be called and manages faiures allowing for re-tries and failover functionality.
    /// </summary>
    public interface IServiceManager : ICloneable 
    {
        /// <summary>
        /// The result of the service call.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result.
        /// </typeparam>
        /// <returns>
        /// The TResult.
        /// </returns>
        TResult Result<TResult>();

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
        /// <param name="timeoutMilliseconds">
        ///  The number of milliseconds between execute attempts.
        /// </param>
        /// <returns>
        /// The IServiceManager`1[TResult -&gt; TResult].
        /// </returns>
        IServiceManager ExecuteService<TResult>(Func<TResult> action, int attempts, int timeoutMilliseconds);

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
        /// IServiceManager`1[TResult -&gt; TResult].
        /// </returns>
        IServiceManager IfServiceFailsThen<TResult>(Func<Exception, TResult> action);
    }
}
