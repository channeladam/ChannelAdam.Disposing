//-----------------------------------------------------------------------
// <copyright file="DisposingEventArgs.cs">
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
    /// Event arguments for a disposing event.
    /// </summary>
    public class DisposingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposingEventArgs" /> class.
        /// </summary>
        /// <param name="isDisposing">If set to <c>true</c> the sender is disposing.</param>
        public DisposingEventArgs(bool isDisposing)
        {
            this.IsDisposing = isDisposing;
        }

        /// <summary>
        /// Gets a value indicating whether the sender is disposing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposing { get; }
    }
}