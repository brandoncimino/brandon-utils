using System;

namespace BrandonUtils.Strings {
    /// <summary>
    /// A class for handling noun conjugations consistently.
    /// </summary>
    [Serializable]
    public class Noun {
        public string Singular;
        public string Plural;

        public Noun(string singular, string plural = null) {
            this.Singular = singular;
            this.Plural   = plural ?? Singular + "s";
        }

        public string Get(int quantity = 1) {
            return quantity == 1 ? Singular : Plural;
        }
    }
}