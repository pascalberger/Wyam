using System;
using System.CommandLine;
using Wyam.Common.IO;
using Wyam.Common.Tracing;
using Wyam.Configuration.Preprocessing;

namespace Wyam.Commands
{
    internal class PreviewCommand : Command
    {
        private int _port = 5080;
        private bool _forceExtension = false;
        private DirectoryPath _path = null;
        private DirectoryPath _virtualDirectory = null;

        public override string Description => "Runs the preview server without generating anything.";

        protected override void ParseOptions(ArgumentSyntax syntax)
        {
            syntax.DefineOption("p|port", ref _port, "Start the preview web server on the specified port (default is " + _port + ").");
            syntax.DefineOption("force-ext", ref _forceExtension, "Force the use of extensions in the preview web server (by default, extensionless URLs may be used).");
            syntax.DefineOption("virtual-dir", ref _virtualDirectory, DirectoryPath.FromString, "Serve files in the preview web server under the specified virtual directory.");
        }

        protected override void ParseParameters(ArgumentSyntax syntax)
        {
            syntax.DefineParameter("path", ref _path, DirectoryPath.FromString, "The path that the server should preview (defaults to \"output\").");
        }

        protected override ExitCode RunCommand(Preprocessor preprocessor)
        {
            _path = new DirectoryPath(Environment.CurrentDirectory).Combine(_path ?? "output");
            using (PreviewServer.Start(_path, _port, _forceExtension, _virtualDirectory, null))
            {
                Trace.Information("Hit any key to exit");
                Console.ReadKey();
                Trace.Information("Shutting down");
            }
            return ExitCode.Normal;
        }
    }
}