using System.Collections.Generic;
using System.Linq;

using Packages.BrandonUtils.Runtime.Collections;

namespace Packages.BrandonUtils.Runtime {
    public class Author {
        public List<string> NameParts;
        public Hyperlink    Website;

        public string FullName => NameParts.Where(n => !string.IsNullOrWhiteSpace(n)).JoinString(" ");

        public string Citation(Hyperlink.MarkupLanguage markupLanguage = Hyperlink.MarkupLanguage.HTML) {
            return $"{FullName} ({Website.ToString(markupLanguage)})";
        }
    }
}