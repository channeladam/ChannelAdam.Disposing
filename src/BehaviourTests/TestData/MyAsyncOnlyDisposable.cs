using System;
using System.Threading.Tasks;
using ChannelAdam.Disposing.Abstractions.Internal;
using ChannelAdam.Logging.Abstractions;

namespace BehaviourTests
{
    public class MyAsyncOnlyDisposable : CoreDisposable, IAsyncDisposable
    {
        private readonly ISimpleLogger logger;

        private int callPosition;
        public int DisposeManagedResourcesCallPosition { get; set; }
        public int DisposeUnmanagedResourcesCallPosition { get; set; }
        public int SetResourcesToNullCallPosition { get; set; }
        public int DisposeAsyncCallPosition { get; set; }

        public MyAsyncOnlyDisposable(ISimpleLogger logger) : base()
        {
            this.logger = logger;
            this.callPosition = 1;
        }

        public ValueTask DisposeAsync()
        {
            this.logger.Log("In DisposeAsync()");
            this.DisposeAsyncCallPosition = this.callPosition++;

            return ValueTask.CompletedTask;
        }

        protected override void DisposeManagedResources()
        {
            this.logger.Log("In DisposeManagedResources()");
            this.DisposeManagedResourcesCallPosition = this.callPosition++;
        }

        protected override void DisposeUnmanagedResources()
        {
            this.logger.Log("In DisposeUnmanagedResources()");
            this.DisposeUnmanagedResourcesCallPosition = this.callPosition++;
        }

        protected override void SetResourcesToNull()
        {
            this.logger.Log("In SetResourcesToNull()");
            this.SetResourcesToNullCallPosition = this.callPosition++;
        }
    }
}