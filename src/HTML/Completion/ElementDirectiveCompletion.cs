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
        public override string CompletionType
        {
            get { return CompletionTypes.Children; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string text = context.Document.TextBuffer.CurrentSnapshot.GetText();
            var names = new List<string>();
            var list = new List<HtmlCompletion>();

            foreach (var file in HtmlCreationListener.Elements.Keys)
                foreach (var elm in HtmlCreationListener.Elements[file])
                {
                    if (!names.Contains(elm))
                        names.Add(elm);
                }

            foreach (Match match in HtmlCreationListener.ElementRegex.Matches(text))
            {
                var name = match.Groups["name"].Value;
                if (!names.Contains(name))
                    names.Add(name);
            }

            foreach (string name in names)
            {
                var item = CreateItem(name, "Custom component/directive", context.Session);
                list.Add(item);
            }

            return list;
        }
    }
}
