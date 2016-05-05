using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [HtmlCompletionProvider(CompletionTypes.Children, "*")]
    [ContentType("htmlx")]
    class ElementDirectiveCompletion : BaseCompletion
    {
        private static Regex _regex = new Regex("Vue\\.(elementDirective|component)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);

        public override string CompletionType
        {
            get { return CompletionTypes.Children; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string text = context.Document.TextBuffer.CurrentSnapshot.GetText();
            var list = new List<HtmlCompletion>();

            foreach (Match match in _regex.Matches(text))
            {
                var item = CreateItem(match.Groups["name"].Value, "Custom component/directive", context.Session);
                list.Add(item);
            }

            return list;
        }
    }
}
