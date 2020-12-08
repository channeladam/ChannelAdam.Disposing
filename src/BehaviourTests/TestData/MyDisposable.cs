using ChannelAdam.Disposing.Abstractions;
using ChannelAdam.Logging.Abstractions;

namespace BehaviourTests
{
    public class MyDisposable : Disposable
    {
        private readonly ISimpleLogger logger;
        private int callPosition;

        public int DisposeManagedResourcesCallPosition { get; set; }
        public int DisposeUnmanagedResourcesCallPosition { get; set; }
        public int SetResourcesToNullCallPosition { get; set; }
        public int OnDisposingCallPosition { get; set; }
        public int OnDisposedCallPosition { get; set; }

        public MyDisposable(ISimpleLogger logger)
        {
            this.logger = logger;
            this.callPosition = 1;
        }

        public static new void SafeDispose(object obj)
        {
            Disposable.SafeDispose(obj);
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
    }
}