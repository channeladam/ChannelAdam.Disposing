using System.Threading.Tasks;
using ChannelAdam.Disposing.Abstractions;
using ChannelAdam.Logging.Abstractions;

namespace BehaviourTests
{
    public class MyAsyncDisposable : AsyncDisposable
    {
        private readonly ISimpleLogger logger;
        private int callPosition;

        public int DisposeManagedResourcesCallPosition { get; set; }
        public int DisposeUnmanagedResourcesCallPosition { get; set; }
        public int SetResourcesToNullCallPosition { get; set; }
        public int OnDisposingCallPosition { get; set; }
        public int OnDisposedCallPosition { get; set; }
        public int DisposeManagedResourcesAsyncCallPosition { get; set; }
        public int DisposeUnmanagedResourcesAsyncCallPosition { get; set; }

        public MyAsyncDisposable(ISimpleLogger logger)
        {
            this.logger = logger;
            this.callPosition = 1;
        }

        public static new async Task SafeDisposeAsync(object? obj)
        {
            await AsyncDisposable.SafeDisposeAsync(obj).ConfigureAwait(false);
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

        protected override void OnDisposing(bool isDisposing)
        {
            this.logger.Log("In OnDisposing()");
            this.OnDisposingCallPosition = this.callPosition++;
        }

        protected override void OnDisposed()
        {
            this.logger.Log("In OnDisposed()");
            this.OnDisposedCallPosition = this.callPosition++;
        }

        protected override Task DisposeManagedResourcesAsync()
        {
            this.logger.Log("In DisposeManagedResourcesAsync()");
            this.DisposeManagedResourcesAsyncCallPosition = this.callPosition++;

            return Task.CompletedTask;
        }

        protected override Task DisposeUnmanagedResourcesAsync()
        {
            this.logger.Log("In DisposeUnmanagedResourcesAsync()");
            this.DisposeUnmanagedResourcesAsyncCallPosition = this.callPosition++;

            return Task.CompletedTask;
        }
    }
}