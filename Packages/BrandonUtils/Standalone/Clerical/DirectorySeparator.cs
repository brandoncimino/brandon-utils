namespace BrandonUtils.Standalone.Clerical {
    public enum DirectorySeparator {
        /// <summary>
        /// Aka "Unix".
        /// </summary>
        Universal = '/',
        Windows = '\\',
    }

    public static class DirectorySeparatorExtensions {
        public static char ToChar(this DirectorySeparator separator) {
            return (char)separator;
        }
    }
}