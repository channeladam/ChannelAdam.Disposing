//-----------------------------------------------------------------------
// <copyright file="AsyncDisposable.cs">
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
    using System.Threading.Tasks;
    using ChannelAdam.Disposing.Abstractions.Internal;

    /// <summary>
    /// Abstract class for correctly implementing an Async Disposable pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// NOTE: <see cref="IAsyncDisposable"/> is NOT a replacement for <see cref="IDisposable"/> - rather it complements it.
    /// </para>
    /// <para>
    /// NOTE: unlike <see cref="Disposable"/>, this class does NOT provide a default, do nothing, implementation of the abstract methods
    ///   <c>DisposeManagedResources()</c>, <c>DisposeManagedResourcesAsync()</c>, <c>DisposeUnmanagedResources()</c>,
    ///   <c>DisposeUnmanagedResourcesAsync()</c> and <c>SetResourcesToNull()</c>
    ///   because it is important that the implementor intentionally implements BOTH the synchronous and asynchronous approaches in this Async Dispose Pattern.
    /// </para>
    /// <para>
    /// Instructions:
    /// - It is IMPORTANT to implement BOTH the synchronous and asynchronous approaches to disposing the same resources
    ///     in case only one of <c>Dispose()</c> or <c>DisposeAsync()</c> is called - such as from <see cref="AsyncDisposableWithDestructor"/>.
    /// - Inherit from this class and override the methods:
    ///     <c>DisposeManagedResources()</c>, <c>DisposeManagedResourcesAsync()</c>, <c>DisposeUnmanagedResources()</c>,
    ///     <c>DisposeUnmanagedResourcesAsync()</c> and <c>SetResourcesToNull()</c>.
    /// - Use <c>SafeDispose()</c> and <c>SafeDisposeAsync()</c> to dispose of resources from within the overridden methods.
    /// </para>
    /// <para>
    /// Inspiration for an Async Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    /// Inspiration for a Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    /// </para>
    /// </remarks>
    public abstract class AsyncDisposable : CoreDisposable, IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Performs the deterministic, application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Implementation of <c>IDisposable.Dispose</c> method.
        /// </remarks>
        public void Dispose()
        {
            // Call the Dispose method and indicate we are calling it deterministically from our application code.
            // (Garbage collection calls finalisers that destroys managed objects non-deterministically.)
            base.Dispose(disposing:true);

            // This object is being cleaned up by the Dispose method.
            // Calling GC.SupressFinalize() takes this object off the finalization queue and prevents
            // finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Implementation of <c>IAsyncDisposable.DisposeAsync</c> method.
        /// </remarks>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(disposing:true).ConfigureAwait(false);

            // This object is being cleaned up by the Dispose method.
            // Calling GC.SupressFinalize() takes this object off the finalization queue and prevents
            // finalization code for this object from executing a second time.
#pragma warning disable CA1816 // Calling GC.SuppressFinalize outside of IDisposable.Dispose()
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Calling GC.SuppressFinalize outside of IDisposable.Dispose()
        }

        #region Protected Abstract Methods

        /// <summary>
        /// Dispose of managed resources here.
        /// </summary>
        /// <remarks>
        /// This releases them faster than if they were reclaimed non-deterministically from a finaliser.
        /// Call <c>SafeDisposeAsync()</c> on every managed resource that needs to be disposed.
        /// </remarks>
        protected abstract Task DisposeManagedResourcesAsync();

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        /// <remarks>
        /// Consider reusing/calling <c>DisposeUnmanagedResources()</c> from here if there are no async mechanisms to release
        /// the unmanaged resources.
        /// </remarks>
        protected abstract Task DisposeUnmanagedResourcesAsync();

        #endregion Protected Abstract Methods

        /// <summary>
        /// Safely disposes of the given object - asynchronously or synchronously.
        /// </summary>
        /// <remarks>
        /// Calls either <c>DisposeAsync()</c> or <c>Dispose()</c> on the given object depending on the interface implemented, if it is not null.
        /// </remarks>
        /// <param name="objectToDispose">The object to dispose.</param>
        protected static async Task SafeDisposeAsync(object objectToDispose)
        {
            if (objectToDispose is null) return;

            // Prefer to dispose with IAsyncDisposable, but fallback to IDisposable.
            if (objectToDispose is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else if (objectToDispose is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Orchestration of asynchronous dispose functionality.
        /// </summary>
        /// <param name="disposing">Should be <c>true</c> when called from IAsyncDisposable.DisposeAsync().</param>
        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (base.IsDisposed)
            {
                return;
            }

            // Set IsDisposed at the start to prevent multiple calls where a caller potentially does not await and calls again.
            base.IsDisposed = true;

            this.OnDisposing(disposing);

            // Dispose of managed resources only if the DisposeAsync() method was called with disposing == true.
            if (disposing)
            {
                await this.DisposeManagedResourcesAsync().ConfigureAwait(false);
            }

            // Always destroy unmanaged resources.
            // The implementer is responsible for ensuring that they do not interact with managed objects that may have been reclaimed by the Garbage Collector.
            await this.DisposeUnmanagedResourcesAsync().ConfigureAwait(false);

            // Set fields to null to make them unreachable.
            this.SetResourcesToNull();

            this.OnDisposed();
        }
    }
}