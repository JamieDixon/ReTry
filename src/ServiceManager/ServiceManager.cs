// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManager.cs" company="Jamie Dixon Ltd">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Service_Manager
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        /// <summary>
        /// A collection of Funcs to execute if the service fails.
        /// </summary>
        private readonly List<Func<dynamic>> failedFuncs = new List<Func<dynamic>>();

        /// <summary>
        /// The number of times the service call has been attempted.
        /// </summary>
        private int count;

        /// <summary>
        /// The number of times to re-try the service if it fails.
        /// </summary>
        private int attemptsAllowed;

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
            return this.Execute<TResult>();
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
        /// <returns>
        /// The BlinkBox.Client.TescoTV.Services.ServiceManager.IServiceManager.
        /// </returns>
        public IServiceManager ExecuteService<TResult>(Func<TResult> action, int attempts)
        {
            this.ServiceFunc = (dynamic)action;
            this.count = 0;
            this.attemptsAllowed = attempts;
            this.Failed = false;

            return this;
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
        /// The BlinkBox.Client.TescoTV.Services.ServiceManager.IServiceManager`1[TResult -&gt; TResult].
        /// </returns>
        public IServiceManager IfServiceFailsThen<TResult>(Func<TResult> action)
        {
            this.failedFuncs.Add((dynamic)action);
            return this;
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
        public TResult Execute<TResult>()
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
                this.ResultImplimentation = func();
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
            catch (Exception)
            {
                if (this.count <= attempts)
                {
                    this.ExecuteServiceImplimentation(action, attempts);
                }
                else
                {
                    this.ExecuteFailedFuncs();
                }
            }

            return this;
        }
    }
}
