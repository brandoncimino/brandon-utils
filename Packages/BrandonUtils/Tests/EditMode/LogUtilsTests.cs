using NUnit.Framework;

using Packages.BrandonUtils.Runtime.Logging;

namespace Packages.BrandonUtils.Tests.EditMode {
    public class LogUtilsTests {
        [Test]
        public void CanLogInEditMode() {
            LogUtils.Log("I have logged this whilst in Edit Mode");
        }
    }
}
