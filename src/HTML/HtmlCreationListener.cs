using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("htmlx")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    class HtmlCreationListener : IVsTextViewCreationListener
    {
        private static bool _hasRun, _isProcessing;

        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public static ConcurrentDictionary<string, string[]> Elements => new ConcurrentDictionary<string, string[]>();
        public static ConcurrentDictionary<string, string[]> Attributes => new ConcurrentDictionary<string, string[]>();

        public static Regex ElementRegex => new Regex("Vue\\.(elementDirective|component)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);
        public static Regex AttributeRegex => new Regex("Vue\\.(directive)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (_hasRun)
                return;

            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            ITextDocument doc;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out doc))
            {
                if (Path.IsPathRooted(doc.FilePath) && File.Exists(doc.FilePath))
                {
                    var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
                    var item = dte.Solution?.FindProjectItem(doc.FilePath);

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        EnsureInitialized(item);
                    });
                }
            }
        }

        public static void Clear()
        {
            _hasRun = false;
            Elements.Clear();
            Attributes.Clear();
        }

        private void EnsureInitialized(ProjectItem item)
        {
            if (_hasRun || _isProcessing || item == null || item.ContainingProject == null)
                return;

            try
            {
                string folder = item.ContainingProject.GetRootFolder();

                var vueFiles = GetFiles(folder, "*.vue");
                var jsFiles = GetFiles(folder, "*.js");
                var allFiles = vueFiles.Union(jsFiles).Where(f => !f.Contains(".min."));

                ProcessFile(allFiles.ToArray());

                _hasRun = _isProcessing = true;
            }
            catch (Exception)
            {
                // TODO: Add logging
            }
        }

        public static void ProcessFile(params string[] files)
        {
            foreach (string file in files)
            {
                string content = File.ReadAllText(file);

                // Elements
                var elementMatches = ElementRegex.Matches(content).Cast<Match>();
                Elements[file] = elementMatches.Select(m => m.Groups["name"].Value).ToArray();

                // Attributes
                var attributeMatches = AttributeRegex.Matches(content).Cast<Match>();
                Attributes[file] = attributeMatches.Select(m => m.Groups["name"].Value).ToArray();
            }
        }

        public static List<string> GetValues(ConcurrentDictionary<string, string[]> cache)
        {
            var names = new List<string>();

            foreach (var file in cache.Keys)
                foreach (var attr in cache[file])
                {
                    if (!names.Contains(attr))
                        names.Add(attr);
                }

            return names;
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
