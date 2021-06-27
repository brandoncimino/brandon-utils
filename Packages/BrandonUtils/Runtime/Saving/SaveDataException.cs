using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;

namespace BrandonUtils.Saving {
    /// <summary>
    /// A special <see cref="BrandonException"/> for exceptions caused by <see cref="SaveData{t}"/>.
    /// </summary>
    public class SaveDataException : BrandonException {
        public SaveDataException(ISaveData saveData) : base(GetMessage(saveData)) { }

        public SaveDataException(ISaveData saveData, string message) : base(GetMessage(saveData, message)) { }

        public SaveDataException(
            ISaveData saveData,
            Exception innerException
        ) : base(
            GetMessage(saveData),
            innerException
        ) { }

        public SaveDataException(Exception innerException) : base(GetMessage(), innerException) { }

        public SaveDataException(
            string message,
            Exception innerException
        ) : base(
            GetMessage(message: message),
            innerException
        ) { }

        public SaveDataException(string message) : base(GetMessage(message: message)) { }

        public SaveDataException(
            ISaveData saveData,
            string message,
            Exception innerException
        ) : base(
            GetMessage(saveData, message),
            innerException
        ) { }

        /// <summary>
        /// Formats the <see cref="BrandonException.Message"/>
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetMessage(
            ISaveData saveData = null,
            string message = "Something went wrong with save data management!"
        ) {
            var lines = new List<string>() {
                message
            };

            if (saveData != null) {
                lines.Add($"{nameof(saveData.Nickname)}: {saveData.Nickname}");
                lines.Add($"{saveData.GetType()}:\n{saveData}");
            }

            return lines.JoinString("\n");
        }
    }
}