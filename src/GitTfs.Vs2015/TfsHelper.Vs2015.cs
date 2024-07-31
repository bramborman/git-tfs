using System.Diagnostics;
using System.Reflection;

using GitTfs.Core.TfsInterop;
using GitTfs.VsCommon;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;

using StructureMap;

namespace GitTfs.Vs2015
{
    public class TfsHelper : TfsHelperBase
    {
        private string vsInstallDir;

        private string TfsVersionString => "14.0";

        public TfsHelper(TfsApiBridge bridge, IContainer container)
            : base(bridge, container)
        {
            _assemblySearchPaths.Add(Path.Combine(GetVsInstallDir(), "PrivateAssemblies"));
        }

        protected override string GetDialogAssemblyPath()
        {
#if NETFRAMEWORK
            var tfsExtensionsFolder = TryGetUserRegStringStartingWithName(@"Software\Microsoft\VisualStudio\14.0\ExtensionManager\EnabledExtensions", "Microsoft.VisualStudio.TeamFoundation.TeamExplorer.Extensions");
            return Path.Combine(tfsExtensionsFolder, DialogAssemblyName + ".dll");
#else
            Trace.TraceWarning("Checkin dialog is not supported with dotnet core version of git-tfs");
            return string.Empty;
#endif
        }

        private string GetVsInstallDir()
        {
            if (vsInstallDir == null)
            {
                vsInstallDir = TryGetRegString(@"Software\WOW6432Node\Microsoft\VisualStudio\" + TfsVersionString, "InstallDir")
                    ?? TryGetRegString(@"Software\Microsoft\VisualStudio\" + TfsVersionString, "InstallDir")
                    ?? TryGetUserRegString(@"Software\WOW6432Node\Microsoft\WDExpress\" + TfsVersionString + "_Config", "InstallDir")
                    ?? TryGetUserRegString(@"Software\Microsoft\WDExpress\" + TfsVersionString + "_Config", "InstallDir");
            }
            return vsInstallDir;
        }

#pragma warning disable 618
        protected override TfsTeamProjectCollection GetTfsCredential(Uri uri) => HasCredentials ?
                new TfsTeamProjectCollection(uri, GetCredential(), new UICredentialsProvider()) :
                new TfsTeamProjectCollection(uri, new UICredentialsProvider());
#pragma warning restore 618
    }
}
