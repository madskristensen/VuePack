using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [ContentType("htmlx")]
    class AttributeDirectiveCompletion : BaseCompletion
    {
        private static Regex _regex = new Regex("Vue\\.(directive)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);

        public override string CompletionType
        {
            get { return CompletionTypes.Attributes; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string text = context.Document.TextBuffer.CurrentSnapshot.GetText();
            var list = new List<HtmlCompletion>();

            foreach (Match match in _regex.Matches(text))
            {
                var item = CreateItem(match.Groups["name"].Value, "Custom directive", context.Session);
                list.Add(item);
            }

            return list;
        }
    }
}
