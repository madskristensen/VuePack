using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    public class VueContentTypeDefinition
    {
        public const string VueContentType = "Vue";
        private const string VueFileExtension = ".vue";

        [Export(typeof(ContentTypeDefinition))]
        [Name(VueContentType)]
        [BaseDefinition("htmlx")]
        public ContentTypeDefinition IVueContentTypeDefinitionContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(VueContentType)]
        [FileExtension(VueFileExtension)]
        public FileExtensionToContentTypeDefinition VueFileExtensionDefinition { get; set; }
    }
}
