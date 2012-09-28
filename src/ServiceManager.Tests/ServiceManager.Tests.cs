// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManager.Tests.cs" company="Jamie Dixon Ltd.">
//   Copyright (c) Jamie Dixon Ltd. All rights reserved.
// </copyright>
// <summary>
//   Defines the Given_ServiceManager_Instance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceManager.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::ReTry.Service;
    using global::ReTry.Service.Exceptions;

    [TestClass]
    public class Given_ServiceManager_Instance
    {
        [TestClass]
        public class When_ExecuteService_Invoked
        {
            [TestMethod]
            public void Passed_Method_Is_Called()
            {
                // Arrange
                var serviceManager = new ReTry();

                // Act
                var result = serviceManager
                    .ExecuteService<string, string>(() => "Hello World", 5)
                    .Result();

                // Assert
                Assert.AreEqual("Hello World", result.Result);
            }

            [TestMethod]
            public void New_Instance_Of_ServiceManager_Returned()
            {
                // Arrange
                var serviceManager = new ReTry();

                var newServiceManager = serviceManager.ExecuteService<string, string>(() => "Hello World", 5);

                Assert.AreNotEqual(serviceManager, newServiceManager);
            }
        }

        [TestClass]
        public class When_IfServiceFailsThen_Invoked
        {
            [TestMethod]
            public void Passed_Method_Is_Called_When_Exception_On_ExecuteService()
            {
                // Arrange
                var serviceManager = new ReTry();

                // Act
                var result = serviceManager
                    .ExecuteService<string, string>(() => { throw new DirectoryNotFoundException("Boom"); })
                    .IfServiceFailsThen<DirectoryNotFoundException>(exception => "Hello World")
                    .Result();

                // Assert
                Assert.AreEqual("Hello World", result.FailureResult);
            }

            [TestMethod]
            public void Less_Specific_Exception_Caught_When_More_Specific_Exception_Occurs()
            {
                // Arrange
                var serviceManager = new ReTry();

                // Act
                var result = serviceManager
                    .ExecuteService<string, string>(() => { throw new DirectoryNotFoundException("Boom"); })
                    .IfServiceFailsThen<Exception>(exception => "Hello World")
                    .Result();

                // Assert
                Assert.AreEqual("Hello World", result.FailureResult);
            }

            [ExpectedException(typeof(UnspecifiedExceptionHandler))]
            [TestMethod]
            public void Exception_Is_Not_Specified_For_Handling_Returns_UnspecifiedExceptionHandler_Exception()
            {
                // Arrange
                var serviceManager = new ReTry();

                try
                {
                    var result = serviceManager
                    .ExecuteService<string, string>(() => { throw new Exception("Boom"); })
                    .Result();
                    Assert.Fail("We should not get here");
                }
                catch (Exception ex)
                {
                    Assert.AreEqual("No handler was found for the exception that was thrown. Make sure you've specified a handler through the IfServiceFailsThen method for this exception.", ex.Message);
                }
            }
        }
    }
}
