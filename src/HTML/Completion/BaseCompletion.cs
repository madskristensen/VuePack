using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VuePack
{
    abstract class BaseCompletion : IHtmlCompletionListProvider
    {
        private static ImageSource _icon = GetImage(Images.VueFile, 16);

        public abstract string CompletionType { get; }

        public abstract IList<HtmlCompletion> GetEntries(HtmlCompletionContext context);

        public static BitmapSource GetImage(ImageMoniker moniker, int size)
        {
            ImageAttributes imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                LogicalHeight = size,
                LogicalWidth = size,
                StructSize = Marshal.SizeOf(typeof(ImageAttributes))
            };

            var imageService = (IVsImageService2)Package.GetGlobalService(typeof(SVsImageService));
            var image = imageService.GetImage(moniker, imageAttributes);

            object data;
            image.get_Data(out data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }

        protected HtmlCompletion CreateItem(string name, string description, ICompletionSession session)
        {
            string desc = description + Environment.NewLine + Environment.NewLine + Vsix.Name;
            return new HtmlCompletion(name, name, desc, _icon, null, session);
        }
    }
}
