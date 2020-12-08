using ChannelAdam.TestFramework.Xunit.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace BehaviourTests
{
    public class DisposableUnitTests : MoqTestFixture
    {
        public DisposableUnitTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ShouldPerformLifecycleOfOverridableMethods()
        {
            // ARRANGE
            var disposable = new MyDisposable(base.Logger);

            LogAssert.IsFalse("IsDisposed", disposable.IsDisposed);
            LogAssert.AreEqual("OnDisposing Call Position", 0, disposable.OnDisposingCallPosition);
            LogAssert.AreEqual("DisposeManagedResources Call Position", 0, disposable.DisposeManagedResourcesCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResources Call Position", 0, disposable.DisposeUnmanagedResourcesCallPosition);
            LogAssert.AreEqual("SetResourcesToNull Call Position", 0, disposable.SetResourcesToNullCallPosition);
            LogAssert.AreEqual("OnDisposed Call Position", 0, disposable.OnDisposedCallPosition);

            // ACT
            disposable.Dispose();

            // ASSERT
            LogAssert.IsTrue("IsDisposed", disposable.IsDisposed);
            LogAssert.AreEqual("OnDisposing Call Position", 1, disposable.OnDisposingCallPosition);
            LogAssert.AreEqual("DisposeManagedResources Call Position", 2, disposable.DisposeManagedResourcesCallPosition);
            LogAssert.AreEqual("DisposeUnmanagedResources Call Position", 3, disposable.DisposeUnmanagedResourcesCallPosition);
            LogAssert.AreEqual("SetResourcesToNull Call Position", 4, disposable.SetResourcesToNullCallPosition);
            LogAssert.AreEqual("OnDisposed Call Position", 5, disposable.OnDisposedCallPosition);
        }

        [Fact]
        public void ShouldReturnWithoutAnError_WhenSafeDisposeIsCalled_AndTheObjectIsNull()
        {
            // ARRANGE

            // ACT
            MyDisposable.SafeDispose(null);

            // ASSERT
            // Great if no exception was thrown.
        }

        [Fact]
        public void ShouldDisposeOfAnObject_WhenSafeDisposeIsCalled_AndTheObjectIsIDisposable()
        {
            // ARRANGE
            var disposable = new MyDisposable(base.Logger);

            // ACT
            MyDisposable.SafeDispose(disposable);

            // ASSERT
            LogAssert.IsTrue("Is Disposed", disposable.IsDisposed);
        }

        [Fact]
        public void ShouldNotDisposeAnIAsyncDisposableObject_WhenSafeDisposeIsCalled()
        {
            // ARRANGE
            var disposable = new MyAsyncOnlyDisposable(base.Logger);

            // ACT
            MyDisposable.SafeDispose(disposable);

            // ASSERT
            LogAssert.IsFalse("Is Disposed", disposable.IsDisposed);
        }
    }
}
