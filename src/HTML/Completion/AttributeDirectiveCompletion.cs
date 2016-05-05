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
        public override string CompletionType
        {
            get { return CompletionTypes.Attributes; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string text = context.Document.TextBuffer.CurrentSnapshot.GetText();
            var names = new List<string>();
            var list = new List<HtmlCompletion>();

            foreach (var file in HtmlCreationListener.Attributes.Keys)
                foreach (var attr in HtmlCreationListener.Attributes[file])
                {
                    if (!names.Contains(attr))
                        names.Add(attr);
                }

            foreach (Match match in HtmlCreationListener.EttributeRegex.Matches(text))
            {
                var name = match.Groups["name"].Value;
                if (!names.Contains(name))
                    names.Add(name);
            }

            foreach (string name in names)
            {
                var item = CreateItem(name, "Custom directive", context.Session);
                list.Add(item);
            }

            return list;
        }
    }
}
