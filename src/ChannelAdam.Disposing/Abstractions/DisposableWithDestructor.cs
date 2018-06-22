//-----------------------------------------------------------------------
// <copyright file="DisposableWithDestructor.cs">
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
    /// Abstract class that implements the Dispose pattern <see cref="Disposable"/> and implements a Destructor.
    /// </summary>
    /// <remarks>
    /// The Dispose Pattern - <see cref="http://msdn.microsoft.com/en-us/library/b1yfkh5e.aspx"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e(v=vs.100).aspx"/>
    /// </remarks>
    public abstract class DisposableWithDestructor : Disposable
    {
        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableWithDestructor"/> class.
        /// </summary>
        /// <remarks>
        /// This destructor will be called by the GC only if the Dispose method does not get called.
        /// Do not provide destructors in types derived from this class - derived types should instead override the Dispose method.
        /// </remarks>
        ~DisposableWithDestructor()
        {
            try
            {
                this.Dispose(false);
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
        public virtual Action<Exception> DestructorExceptionBehaviour
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
