using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VuePack
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid("68fc93ea-cc77-4459-a612-15d2a3ba4c86")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class WebLinterPackage : Package
    {
        private SolutionEvents _events;

        protected override void Initialize()
        {
            var dte = GetService(typeof(DTE)) as DTE2;

            _events = dte.Events.SolutionEvents;
            _events.AfterClosing += delegate { HtmlCreationListener.Clear(); };

            base.Initialize();
        }
    }
}
