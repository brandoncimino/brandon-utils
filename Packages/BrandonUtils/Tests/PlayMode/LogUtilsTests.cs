using BrandonUtils.Logging;

using NUnit.Framework;

namespace BrandonUtils.Tests.PlayMode {
    public class LogUtilsTests {
        [Test]
        public void CanLogInPlayMode() {
            LogUtils.Log("I have logged this whilst in a Play Mode test");
        }
    }
}
