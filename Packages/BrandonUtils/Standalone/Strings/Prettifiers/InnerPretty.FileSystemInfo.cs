using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Clerical;
using BrandonUtils.Standalone.Collections;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyFileSystemInfo([NotNull] FileSystemInfo item, [CanBeNull] PrettificationSettings settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            return settings.PreferredLineStyle == LineStyle.Single
                       ? PrettifySingleItem(item, settings)
                       : PrettifyWithChildren(item, settings).JoinLines();
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<string> SummarizeChildren([NotNull] DirectoryInfo directoryInfo) {
            var childCount = directoryInfo.EnumerateFileSystemInfos().Count();
            return new[] { $"{StringUtils.Ellipsis}({childCount} children)" };
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<string> EnumerateChildren([NotNull] DirectoryInfo dir, [CanBeNull] PrettificationSettings settings, int depthLimit, int currentDepth) {
            return dir.EnumerateFileSystemInfos()
                      .SelectMany(it => PrettifyWithChildren(it, settings, depthLimit, currentDepth));
        }

        [NotNull]
        private static string GetIcon([NotNull] FileSystemInfo item) {
            return item switch {
                FileInfo _      => BPath.FileIcon,
                DirectoryInfo _ => BPath.ClosedFolderIcon,
                _               => throw new ArgumentException(nameof(item))
            };
        }

        private static string PrettifySingleItem([NotNull] FileSystemInfo item, [NotNull] PrettificationSettings settings, int currentDepth = 0) {
            var itemName = currentDepth == 0 ? item.ToUri().ToString() : item.Name;
            return itemName.Prefix(GetIcon(item), " ");
        }

        [NotNull]
        private static IEnumerable<string> PrettifyWithChildren(FileSystemInfo item, [CanBeNull] PrettificationSettings settings, [NonNegativeValue] int depthLimit = 2, int currentDepth = 0) {
            /*
             * 📂 Folder
             *   | File
             *   | File
             *  📂 Folder
             *   …(5 children)
             */
            IEnumerable<string> lines;
            var                 itemName = currentDepth == 0 ? item.ToUri().ToString() : item.Name;

            if (item is DirectoryInfo dir) {
                var directoryName = itemName.Prefix(BPath.ClosedFolderIcon, " ");
                var children      = currentDepth >= depthLimit ? SummarizeChildren(dir) : EnumerateChildren(dir, settings, depthLimit, currentDepth);
                lines = children.Prepend(directoryName);
            }
            else {
                lines = new[] { $"| {itemName}" };
            }

            return lines.SplitLines().Indent();
        }
    }
}