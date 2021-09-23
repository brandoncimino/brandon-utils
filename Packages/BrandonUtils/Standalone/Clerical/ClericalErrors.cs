using System.IO;

using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Standalone.Clerical {
    public static class ClericalErrors {
        private static IOException _itemAlreadyExistsException(object item) {
            return new IOException($"The {item.GetType().Prettify()} {item.Prettify()} already exists!");
        }

        public static IOException ItemAlreadyExistsException(FileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }

        public static IOException ItemAlreadyExistsException(IHasFileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }

        private static IOException _itemDoesNotExistException(object item) {
            return new IOException($"The {item.GetType().Prettify()} {item.Prettify()} does not exist!");
        }

        public static IOException ItemDoesNotExistException(FileSystemInfo item) {
            return _itemDoesNotExistException(item);
        }

        public static IOException ItemDoesNotExistException(IHasFileSystemInfo item) {
            return _itemAlreadyExistsException(item);
        }
    }
}