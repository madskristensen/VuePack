using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Language.Intellisense;

namespace VuePack
{
    abstract class BaseCompletion : IHtmlCompletionListProvider
    {
        private static ImageSource _icon = GetIcon();

        public abstract string CompletionType { get; }

        public abstract IList<HtmlCompletion> GetEntries(HtmlCompletionContext context);

        protected static ImageSource GetIcon()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            string path = Path.Combine(folder, "HTML\\Completion\\icon.png");

            var uri = new Uri(path);
            return BitmapFrame.Create(uri);
        }

        protected HtmlCompletion CreateItem(string name, string description, ICompletionSession session)
        {
            string desc = description + Environment.NewLine + Environment.NewLine + Vsix.Name;
            return new HtmlCompletion(name, name, desc, _icon, null, session);
        }
    }
}
