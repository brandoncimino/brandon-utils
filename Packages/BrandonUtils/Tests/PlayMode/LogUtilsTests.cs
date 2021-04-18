using NUnit.Framework;

using Packages.BrandonUtils.Runtime.Logging;

namespace Packages.BrandonUtils.Tests.PlayMode {
    public class LogUtilsTests {
        [Test]
        public void CanLogInPlayMode() {
            LogUtils.Log("I have logged this whilst in a Play Mode test");
        }
    }
}
