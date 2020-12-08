//-----------------------------------------------------------------------
// <copyright file="CoreDisposable.cs">
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

namespace ChannelAdam.Disposing.Abstractions.Internal
{
    using System;

    /// <summary>
    /// Common base class for implementing a Disposable pattern.
    /// </summary>
    public abstract class CoreDisposable
    {
        private volatile bool isDisposed;   // volatile because the Garbage Collector calls finalizers in a different thread

        #region Events

        /// <summary>
        /// Occurs when the object is about to be disposed.
        /// </summary>
        public event EventHandler<DisposingEventArgs>? Disposing;

        /// <summary>
        /// Occurs when the object has been disposed.
        /// </summary>
        public event EventHandler? Disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return this.isDisposed; }
            internal set { this.isDisposed = value; }
        }

        #endregion Properties

        #region Protected Abstract Methods

        /// <summary>
        /// Dispose of managed resources here.
        /// </summary>
        /// <remarks>
        /// This releases them faster than if they were reclaimed non-deterministically from a finaliser.
        /// Call <c>SafeDispose()</c> on every managed resource that needs to be disposed.
        /// </remarks>
        protected abstract void DisposeManagedResources();

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        /// <remarks>
        /// The implementer is responsible for ensuring that they do not interact with managed objects that may have been reclaimed by the Garbage Collector.
        /// <remarks>
        protected abstract void DisposeUnmanagedResources();

        /// <summary>
        /// Set the disposed resources to null to make them unreachable, and to prevent double disposal attempts.
        /// </summary>
        /// <remarks>
        /// This executes after all the Dispose*anagedResources*() methods.
        /// </remarks>
        protected abstract void SetResourcesToNull();

        #endregion Protected Abstract Methods

        #region Protected Virtual Methods

        /// <summary>
        /// Called when the object is about to be disposed.
        /// </summary>
        /// <param name="isDisposing">If set to <c>true</c> then the object is being disposed from a call to Dispose(); <c>false</c> if it is from a finalizer / destructor.</param>
        protected virtual void OnDisposing(bool isDisposing)
        {
            this.Disposing?.Invoke(this, new DisposingEventArgs(isDisposing));
        }

        /// <summary>
        /// Called when the object has been disposed.
        /// </summary>
        protected virtual void OnDisposed()
        {
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion Protected Virtual Methods

        /// <summary>
        /// Safely disposes of the given object.
        /// </summary>
        /// <remarks>
        /// Calls Dispose() on the given object if it is not null and implements IDisposable.
        /// </remarks>
        /// <param name="objectToDispose">The object to dispose.</param>
        protected static void SafeDispose(object objectToDispose)
        {
            if (objectToDispose is null) return;

            if (objectToDispose is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Orchestration of synchronous dispose functionality.
        /// </summary>
        /// <param name="disposing">Should be <c>true</c> when called from IDisposable.Dispose(); otherwise <c>false</c> when called from a finaliser.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            // Event
            this.OnDisposing(disposing);

            // Dispose of managed resources only if the Dispose() method was called (disposing == true).
            // If we are called from a finaliser (disposing == false), then the Garbage Collector is non-deterministically
            // destroying the managed resources, and we should not attempt to dispose of managed resources.
            if (disposing)
            {
                this.DisposeManagedResources();
            }

            // Always destroy unmanaged resources.
            // The implementer is responsible for ensuring that they do not interact with managed objects that may have been reclaimed by the Garbage Collector.
            this.DisposeUnmanagedResources();

            // Set fields to null to make them unreachable.
            this.SetResourcesToNull();

            this.IsDisposed = true;

            // Event
            this.OnDisposed();
        }
    }
}