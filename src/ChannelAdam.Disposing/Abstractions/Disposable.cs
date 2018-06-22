//-----------------------------------------------------------------------
// <copyright file="Disposable.cs">
//     Copyright (c) 2014-2018 Adam Craven. All rights reserved.
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
    /// Abstract class for implementing the Dispose pattern.
    /// </summary>
    /// <remarks>
    /// Instructions: Inherit from this class and override the DisposeManagedResources() and DisposeUnmanagedResources() methods.
    /// The Dispose Pattern - <see cref="http://msdn.microsoft.com/en-us/library/b1yfkh5e.aspx."/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e(v=vs.100).aspx"/>
    /// </remarks>
    public abstract class Disposable : IDisposable
    {
        private volatile bool isDisposed;   // volatile because the Garbage Collector calls finalizers in a different thread

        #region Events

        /// <summary>
        /// Occurs when the object is about to be disposed.
        /// </summary>
        public event EventHandler<DisposingEventArgs> Disposing;

        /// <summary>
        /// Occurs when the object has been disposed.
        /// </summary>
        public event EventHandler Disposed;

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
        }

        #endregion Properties

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object is being cleaned up by the Dispose method.
            // Calling GC.SupressFinalize() takes this object off the finalization queue and prevents
            // finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        #region Protected Virtual Methods

        /// <summary>
        /// Release managed resources here.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            // override this method
        }

        /// <summary>
        /// Release unmanaged resources here.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
            // override this method
        }

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

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.OnDisposing(disposing);

            this.DisposeUnmanagedResources();

            if (disposing)
            {
                this.DisposeManagedResources();
            }

            this.isDisposed = true;

            this.OnDisposed();
        }
    }
}