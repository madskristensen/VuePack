using System;
using Microsoft.VisualStudio.Imaging.Interop;

namespace VuePack
{
    public static class Images
    {
        private static Guid _guid = new Guid("{af265e71-5e4f-43f6-9d14-8b198b7e74f2}");

        public static ImageMoniker VueFile { get { return new ImageMoniker { Guid = _guid, Id = 0 }; } }
    }
}
