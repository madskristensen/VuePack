using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(VueContentTypeDefinition.VueContentType)]
    [ContentType("javascript")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    class VueCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private ITextDocument _document;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            // Both "Web Compiler" and "Bundler & Minifier" extensions add this property on their
            // generated output files. Generated output should be ignored from linting
            bool generated;
            if (textView.Properties.TryGetProperty("generated", out generated) && generated)
                return;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                _document.FileActionOccurred += DocumentSaved;
            }
        }

        private void DocumentSaved(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                Task.Run(() =>
                {
                    DirectivesCache.ProcessFile(e.FilePath);
                });
            }
        }

        private static List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            if (path.Contains("node_modules"))
                return files;

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch { }

            return files;
        }
    }
}
