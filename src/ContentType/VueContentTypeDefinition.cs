using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.IcedCoffeeScript
{
    public class VueContentTypeDefinition
    {
        public const string VueContentType = "Vue";

        [Export(typeof(ContentTypeDefinition))]
        [Name(VueContentType)]
        [BaseDefinition("htmlx")]
        public ContentTypeDefinition IVueContentTypeDefinitionContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(VueContentType)]
        [FileExtension(".vue")]
        public FileExtensionToContentTypeDefinition VueFileExtension { get; set; }
    }
}
