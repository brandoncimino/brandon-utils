using BrandonUtils.Logging;

using NUnit.Framework;

namespace BrandonUtils.Tests.EditMode {
    public class LogUtilsTests {
        [Test]
        public void CanLogInEditMode() {
            LogUtils.Log("I have logged this whilst in Edit Mode");
        }
    }
}
