using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace VisibilityConstraintsSample
{
    internal sealed class MyButton
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("497de1d3-ed31-4519-a864-bbcd992fa57d");

        private readonly AsyncPackage _package;

        private MyButton(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(Execute, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MenuItemQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            // Make the button invisible by default
            button.Visible = false;

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            ProjectItem item = dte.SelectedItems.Item(1)?.ProjectItem;

            if (item != null)
            {
                string fileExtension = Path.GetExtension(item.Name).ToLowerInvariant();
                string[] supportedFiles = new[] { ".cs", ".vb" };

                // Show the button only if a supported file is selected
                button.Visible = supportedFiles.Contains(fileExtension);
            }
        }

        public static MyButton Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new MyButton(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", GetType().FullName);
            string title = "MyButton";

            VsShellUtilities.ShowMessageBox(
                _package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
