// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnspecifiedExceptionHandler.cs" company="Jamie Dixon Ltd">
//   Copyright (c) Jamie Dixon. All rights reserved.
// </copyright>
// <summary>
//   An exception that is thrown when no handler is specified for a given exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReTry.Service.Exceptions
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UnspecifiedExceptionHandler : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnspecifiedExceptionHandler"/> class.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public UnspecifiedExceptionHandler(Exception exception) : base("No handler was found for the exception that was thrown. Make sure you've specified a handler through the IfServiceFailsThen method for this exception.", exception)
        {
        }
    }
}
