//-----------------------------------------------------------------------
// <copyright file="AsyncDisposableWithDestructor.cs">
//     Copyright (c) 2014-2020 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.Disposing.Abstractions
{
    using System;

    /// <summary>
    /// Abstract class that implements an Async Disposable pattern <see cref="AsyncDisposable"/> with a Destructor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// NOTE: <see cref="IAsyncDisposable"/> is NOT a replacement for <see cref="IDisposable"/> - rather it complements it.
    /// NOTE: The synchronous <c>Dispose()</c> method is called from this destructor as there is no async version of a destructor.
    /// </para>
    /// <para>
    /// Instructions:
    /// - It is important to implement BOTH the synchronous and asynchronous approaches to disposing the same resources.
    /// - Inherit from this class and override the methods:
    ///     <c>DisposeManagedResources()</c>, <c>DisposeManagedResourcesAsync()</c>, <c>DisposeUnmanagedResources()</c>,
    ///     <c>DisposeUnmanagedResourcesAsync()</c> and <c>SetResourcesToNull()</c>
    /// - Use <c>SafeDispose()</c> and <c>SafeDisposeAsync()</c> to dispose of resources from within the overridden methods.
    /// </para>
    /// <para>
    /// Inspiration for an Async Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    /// Inspiration for a Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    /// </para>
    /// </remarks>
    public abstract class AsyncDisposableWithDestructor : AsyncDisposable
    {
        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="AsyncDisposableWithDestructor"/> class.
        /// </summary>
        /// <remarks>
        /// This destructor will be called by the GC only if the Dispose method does not get called.
        /// Do not provide destructors in types derived from this class - derived types should instead override the Dispose method.
        /// </remarks>
        ~AsyncDisposableWithDestructor()
        {
            try
            {
                // We have to use the synchronous version of Dispose() because there is no async version of a Destructor.
                // Pass false to indicate that this is a finaliser, calling it non-deterministically.
                // The Garbage Collector calls finalisers that destroys managed objects non-deterministically.
                this.Dispose(disposing:false);
            }
            catch (Exception e)
            {
                // Suppress exceptions thrown from a destructor/finalizer so they do not go back into the Garbage Collector!
                // But, allow the client code to handle the exception - e.g. log it
                this.OnDestructorException(e);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the behaviour to perform when an exception occurs during the destructor/finalize.
        /// </summary>
        /// <value>
        /// The exception behaviour.
        /// </value>
        public virtual Action<Exception>? DestructorExceptionBehaviour
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when there is an exception in the destructor/finalize.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnDestructorException(Exception exception)
        {
            try
            {
                this.DestructorExceptionBehaviour?.Invoke(exception);
            }
            catch (Exception ex)
            {
                // Failsafe - Suppress this so they do not go back into the Garbage Collector!
                Console.Error.WriteLine("Exception occurred during destructor: " + ex.ToString());
            }
        }

        #endregion
    }
}
