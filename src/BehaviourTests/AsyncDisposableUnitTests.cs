using System.Threading.Tasks;
using ChannelAdam.TestFramework.Xunit.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace BehaviourTests
{
    public class AsyncDisposableUnitTests : MoqTestFixture
    {
        public AsyncDisposableUnitTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ShouldPerformLifecycleOfOverridableDisposeMethods()
        {
            // ARRANGE
            var disposable = new MyAsyncDisposable(base.Logger);
            AssertInitialState(disposable);

            // ACT
            disposable.Dispose();

            // ASSERT
            // Assert lifecycle of Dispose methods were invoked
            LogAssert.IsTrue("IsDisposed", disposable.IsDisposed);
            LogAssert.AreEqual("OnDisposing Call Position", 1, disposable.OnDisposingCallPosition);
            LogAssert.AreEqual("DisposeManagedResources Call Position", 2, disposable.DisposeManagedResourcesCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResources Call Position", 3, disposable.DisposeUnmanagedResourcesCallPosition);
            LogAssert.AreEqual("SetResourcesToNull Call Position", 4, disposable.SetResourcesToNullCallPosition);
            LogAssert.AreEqual("OnDisposed Call Position", 5, disposable.OnDisposedCallPosition);

            // Assert async methods were NOT invoked
            LogAssert.AreEqual("DisposeManagedResourcesAsync Call Position", 0, disposable.DisposeManagedResourcesAsyncCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResourcesAsync Call Position", 0, disposable.DisposeUnmanagedResourcesAsyncCallPosition);
        }

        [Fact]
        public async Task ShouldPerformLifecycleOfOverridableAsyncDisposeMethods()
        {
            // ARRANGE
            var disposable = new MyAsyncDisposable(base.Logger);
            AssertInitialState(disposable);

            // ACT
            await disposable.DisposeAsync().ConfigureAwait(false);

            // ASSERT
            // Assert lifecycle of Async Dispose methods were invoked
            LogAssert.IsTrue("IsDisposed", disposable.IsDisposed);
            LogAssert.AreEqual("OnDisposing Call Position", 1, disposable.OnDisposingCallPosition);
            LogAssert.AreEqual("DisposeManagedResourcesAsync Call Position", 2, disposable.DisposeManagedResourcesAsyncCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResourcesAsync Call Position", 3, disposable.DisposeUnmanagedResourcesAsyncCallPosition);
            LogAssert.AreEqual("SetResourcesToNull Call Position", 4, disposable.SetResourcesToNullCallPosition);
            LogAssert.AreEqual("OnDisposed Call Position", 5, disposable.OnDisposedCallPosition);

            // Assert the normal Dispose methods were NOT invoked
            LogAssert.AreEqual("DisposeManagedResources Call Position", 0, disposable.DisposeManagedResourcesCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResources Call Position", 0, disposable.DisposeUnmanagedResourcesCallPosition);
        }

        private void AssertInitialState(MyAsyncDisposable disposable)
        {
            LogAssert.IsFalse("IsDisposed", disposable.IsDisposed);
            LogAssert.AreEqual("OnDisposing Call Position", 0, disposable.OnDisposingCallPosition);
            LogAssert.AreEqual("DisposeManagedResources Call Position", 0, disposable.DisposeManagedResourcesCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResources Call Position", 0, disposable.DisposeUnmanagedResourcesCallPosition);
            LogAssert.AreEqual("SetResourcesToNull Call Position", 0, disposable.SetResourcesToNullCallPosition);
            LogAssert.AreEqual("OnDisposed Call Position", 0, disposable.OnDisposedCallPosition);
            LogAssert.AreEqual("DisposeManagedResourcesAsync Call Position", 0, disposable.DisposeManagedResourcesAsyncCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResourcesAsync Call Position", 0, disposable.DisposeUnmanagedResourcesAsyncCallPosition);
        }

        [Fact]
        public async Task ShouldReturnWithoutAnError_WhenSafeDisposeAsyncIsCalled_AndTheObjectIsNull()
        {
            // ARRANGE

            // ACT
            await MyAsyncDisposable.SafeDisposeAsync(null).ConfigureAwait(false);

            // ASSERT
        }

       [Fact]
        public async Task ShouldDisposeAnIAsyncDisposableObject_WhenSafeDisposeAsyncIsCalled()
        {
            // ARRANGE
            var disposable = new MyAsyncDisposable(base.Logger);

            // ACT
            await MyAsyncDisposable.SafeDisposeAsync(disposable).ConfigureAwait(false);

            // ASSERT
            LogAssert.IsTrue("Is Disposed", disposable.IsDisposed);
        }

        [Fact]
        public async Task ShouldDisposeOfAnObject_WhenSafeDisposeAsyncIsCalled_AndTheObjectIsIDisposable()
        {
            // ARRANGE
            var disposable = new MyDisposable(base.Logger);

            // ACT
            await MyAsyncDisposable.SafeDisposeAsync(disposable).ConfigureAwait(false);

            // ASSERT
            LogAssert.IsTrue("Is Disposed", disposable.IsDisposed);
        }
    }
}
