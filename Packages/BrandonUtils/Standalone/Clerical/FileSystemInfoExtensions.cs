using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Clerical {
    [PublicAPI]
    public static class FileSystemInfoExtensions {
        #region Uri

        [ContractAnnotation("null => null")]
        [ContractAnnotation("notnull => notnull")]
        [CanBeNull]
        public static Uri ToUri([CanBeNull] this FileSystemInfo fileSystemInfo) {
            return fileSystemInfo switch {
                null              => null,
                DirectoryInfo dir => new Uri(dir.FullName.AppendIfMissing(Path.DirectorySeparatorChar.ToString())),
                FileInfo file     => new Uri(file.FullName),
                _                 => throw new ArgumentException($"Must be a {nameof(FileInfo)} or {nameof(DirectoryInfo)}, but was {fileSystemInfo.GetType().Prettify()}", nameof(fileSystemInfo))
            };
        }

        [ContractAnnotation("null => null")]
        [ContractAnnotation("notnull => notnull")]
        [CanBeNull]
        public static Uri ToUri([CanBeNull] this IHasFileSystemInfo hasFileSystemInfo) {
            return hasFileSystemInfo?.FileSystemInfo.ToUri();
        }

        #endregion

        #region Parent-Child Relationships

        #region IsParentOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        [Pure]
        public static bool IsParentOf([NotNull] this DirectoryInfo parent, [NotNull] FileSystemInfo child) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            if (child == null) {
                throw new ArgumentNullException(nameof(child));
            }

            return parent.ToUri().IsBaseOf(child.ToUri());
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf([NotNull] this DirectoryInfo parent, [NotNull] IHasFileSystemInfo child) => parent.IsParentOf(child.FileSystemInfo);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf([NotNull] this IHasDirectoryInfo parent, [NotNull] FileSystemInfo child) => parent.Directory!.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsParentOf([NotNull] this IHasDirectoryInfo parent, [NotNull] IHasFileSystemInfo child) => parent.IsParentOf(child.FileSystemInfo);

        #endregion

        #region MustBeParentOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf([CanBeNull] this DirectoryInfo parent, [CanBeNull] FileSystemInfo child) {
            CheckNull(parent, child);

            if (parent.IsParentOf(child) == false) {
                throw NotMyChildException(parent, child);
            }
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf([CanBeNull] this DirectoryInfo parent, [CanBeNull] IHasFileSystemInfo child) => MustBeParentOf(parent, child!.FileSystemInfo);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf([CanBeNull] this IHasDirectoryInfo parent, [CanBeNull] FileSystemInfo child) => MustBeParentOf(parent!.Directory!, child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeParentOf([CanBeNull] this IHasDirectoryInfo parent, [CanBeNull] IHasFileSystemInfo child) => MustBeParentOf(parent!.Directory!, child!.FileSystemInfo);

        #endregion

        #region IsChildOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf([NotNull] this FileSystemInfo child, [NotNull] DirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf([NotNull] this FileSystemInfo child, [NotNull] IHasDirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf([NotNull] this IHasFileSystemInfo child, [NotNull] DirectoryInfo parent) => parent.IsParentOf(child);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static bool IsChildOf([NotNull] this IHasFileSystemInfo child, [NotNull] IHasDirectoryInfo parent) => parent.IsParentOf(child);

        #endregion

        #region MustBeChildOf

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf([CanBeNull] this FileSystemInfo child, [CanBeNull] DirectoryInfo parent) {
            CheckNull(parent, child);

            if (child.IsChildOf(parent) == false) {
                throw NotMyMomException(child, parent);
            }
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf([CanBeNull] this FileSystemInfo child, [CanBeNull] IHasDirectoryInfo parent) => child.MustBeChildOf(parent!.Directory!);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf([CanBeNull] this IHasFileSystemInfo child, [CanBeNull] DirectoryInfo parent) => child!.FileSystemInfo.MustBeChildOf(parent);

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        public static void MustBeChildOf([CanBeNull] this IHasFileSystemInfo child, [CanBeNull] IHasDirectoryInfo parent) => child.MustBeChildOf(parent!.Directory!);

        #endregion

        private static InvalidOperationException NotMyChildException(DirectoryInfo parent, FileSystemInfo child) {
            return new InvalidOperationException(
                $"The {nameof(parent)} {parent.GetType().Prettify()} does not contain the {nameof(child)} {child.GetType().Prettify()}! {FamilyString(parent, child)}"
            );
        }

        private static InvalidOperationException NotMyMomException(FileSystemInfo child, DirectoryInfo parent) {
            return new InvalidOperationException(
                $"The {nameof(child)} {child.GetType().Prettify()} isn't contained by the {nameof(parent)} {parent.GetType().Prettify()}! {FamilyString(parent, child)}"
            );
        }

        [ContractAnnotation("parent:null => stop")]
        [ContractAnnotation("child:null => stop")]
        private static void CheckNull(DirectoryInfo parent, FileSystemInfo child) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent), $"{nameof(parent)} was null, so it cannot possibly contain the {nameof(child)} {child.Prettify()}!");
            }

            if (child == null) {
                throw new ArgumentNullException(nameof(child), $"{nameof(child)} was null, so it cannot possibly be contained by the {nameof(parent)} {parent.Prettify()}!");
            }
        }

        private static string FamilyString(DirectoryInfo parent, FileSystemInfo child) {
            return new Dictionary<string, FileSystemInfo>() {
                [nameof(parent)] = parent,
                [nameof(child)]  = child
            }.Prettify();
        }

        #endregion

        #region Existential

        public static void MustNotExist([NotNull] this FileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.Refresh();
            if (fileSystemInfo.Exists) {
                throw ClericalErrors.ItemAlreadyExistsException(fileSystemInfo);
            }
        }

        public static void MustNotExist([NotNull] this IHasFileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.FileSystemInfo.Refresh();
            if (fileSystemInfo.FileSystemInfo.Exists) {
                throw ClericalErrors.ItemAlreadyExistsException(fileSystemInfo);
            }
        }

        public static void MustExist([NotNull] this FileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.Refresh();
            if (fileSystemInfo.Exists == false) {
                throw ClericalErrors.ItemDoesNotExistException(fileSystemInfo);
            }
        }

        public static void MustExist([NotNull] this IHasFileSystemInfo fileSystemInfo) {
            if (fileSystemInfo == null) {
                throw new ArgumentNullException(nameof(fileSystemInfo));
            }

            fileSystemInfo.FileSystemInfo.Refresh();
            if (fileSystemInfo.FileSystemInfo.Exists == false) {
                throw ClericalErrors.ItemDoesNotExistException(fileSystemInfo);
            }
        }

        #endregion
    }
}