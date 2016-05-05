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
    class DirectivesCache : IVsTextViewCreationListener
    {
        private static bool _hasRun, _isProcessing;
        private static ConcurrentDictionary<string, string[]> _elements = new ConcurrentDictionary<string, string[]>();
        private static ConcurrentDictionary<string, string[]> _attributes = new ConcurrentDictionary<string, string[]>();

        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public static Regex ElementRegex { get; } = new Regex("Vue\\.(elementDirective|component)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);

        public static Regex AttributeRegex { get; } = new Regex("Vue\\.(directive)\\(('|\")(?<name>[^'\"]+)\\2", RegexOptions.Compiled);

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (_hasRun || _isProcessing)
                return;

            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            ITextDocument doc;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out doc))
            {
                if (Path.IsPathRooted(doc.FilePath) && File.Exists(doc.FilePath))
                {
                    _isProcessing = true;
                    var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
                    var item = dte.Solution?.FindProjectItem(doc.FilePath);

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        EnsureInitialized(item);
                        _hasRun = _isProcessing = false;
                    });
                }
            }
        }

        public static void Clear()
        {
            _hasRun = false;
            _elements.Clear();
            _attributes.Clear();
        }

        private void EnsureInitialized(ProjectItem item)
        {
            if (item == null || item.ContainingProject == null)
                return;

            try
            {
                string folder = item.ContainingProject.GetRootFolder();

                var vueFiles = GetFiles(folder, "*.vue");
                var jsFiles = GetFiles(folder, "*.js");
                var allFiles = vueFiles
                                .Union(jsFiles)
                                .Where(f => !f.Contains(".min.") && !f.EndsWith(".intellisense.js") && !f.EndsWith("-vsdoc.js"));

                ProcessFile(allFiles.ToArray());
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
                _elements[file] = elementMatches.Select(m => m.Groups["name"].Value).ToArray();

                // Attributes
                var attributeMatches = AttributeRegex.Matches(content).Cast<Match>();
                _attributes[file] = attributeMatches.Select(m => m.Groups["name"].Value).ToArray();
            }
        }

        public static List<string> GetValues(DirectiveType type)
        {
            var names = new List<string>();
            var cache = type == DirectiveType.Element ? _elements : _attributes;

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

    public enum DirectiveType
    {
        Element,
        Attribute
    }
}
