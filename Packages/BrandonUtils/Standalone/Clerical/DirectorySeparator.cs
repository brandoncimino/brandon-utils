﻿using System.IO;

using BrandonUtils.Standalone.Enums;

namespace BrandonUtils.Standalone.Clerical {
    public enum DirectorySeparator {
        /// <summary>
        /// Aka "Unix".
        /// </summary>
        Universal,
        Windows,
        PlatformDependent
    }

    public static class DirectorySeparatorExtensions {
        public static char ToChar(this DirectorySeparator separator) {
            return separator switch {
                DirectorySeparator.Universal         => '/',
                DirectorySeparator.Windows           => '\\',
                DirectorySeparator.PlatformDependent => Path.DirectorySeparatorChar,
                _                                    => throw BEnum.InvalidEnumArgumentException(nameof(separator), separator)
            };
        }
    }
}