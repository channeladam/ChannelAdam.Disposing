//-----------------------------------------------------------------------
// <copyright file="Disposable.cs">
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
    using ChannelAdam.Disposing.Abstractions.Internal;

    /// <summary>
    /// Abstract class for correctly implementing a Disposable pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provides a default, do nothing, implementation of the abstract methods
    ///   <c>DisposeManagedResources()</c>, <c>DisposeUnmanagedResources()</c>, and <c>SetResourcesToNull()</c>
    ///   so that the implementor does not need to implement every one of them - which is a reasonable assumption for this Dispose Pattern.
    /// </para>
    /// <para>
    /// Instructions:
    /// - Inherit from this class and override the methods:
    ///     <c>DisposeManagedResources()</c>, <c>DisposeUnmanagedResources()</c>, and <c>SetResourcesToNull()</c>.
    /// - Use <c>SafeDispose()</c> to dispose of resources from within the overridden methods.
    /// </para>
    /// <para>
    /// Inspiration for a Dispose Pattern - see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    /// See https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/b1yfkh5e(v=vs.100)
    /// </para>
    /// </remarks>
    public abstract class Disposable : CoreDisposable, IDisposable
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

        #region Protected Virtual Methods

        /// <summary>
        /// Dispose of managed resources here.
        /// </summary>
        /// <remarks>
        /// This releases them faster than if they were reclaimed non-deterministically from a finaliser.
        /// Call <c>SafeDispose()</c> on every managed resource that needs to be disposed.
        /// </remarks>
        protected override void DisposeManagedResources()
        {
            // override this method
        }

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        /// <remarks>
        /// The implementer is responsible for ensuring that they do not interact with managed objects that may have been reclaimed by the Garbage Collector.
        /// <remarks>
        protected override void DisposeUnmanagedResources()
        {
            // override this method
        }

        /// <summary>
        /// Set the disposed resources to null to make them unreachable, and to prevent double disposal attempts.
        /// </summary>
        /// <remarks>
        /// This executes after all the Dispose*anagedResources*() methods.
        /// </remarks>
        protected override void SetResourcesToNull()
        {
            // override this method
        }

        #endregion Protected Virtual Methods
    }
}