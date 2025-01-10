using System.Linq;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.System;
using Minecraft.Unmanaged;
using System.Threading.Tasks;

namespace Minecraft;
using static Native;

sealed class App(string value)
{
    static readonly PackageDebugSettings PackageDebugSettings = new();

    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    readonly AppInfo AppInfo = AppInfo.GetFromAppUserModelId(value);

    internal string PackageFamilyName => AppInfo.PackageFamilyName;

    internal bool Running
    {
        get
        {
            var _ = AppDiagnosticInfo.RequestInfoForAppAsync(AppInfo.AppUserModelId);
            if (_.Status is AsyncStatus.Started) using (ManualResetEventSlim @event = new()) { _.Completed += (_, _) => @event.Set(); @event.Wait(); }
            try { return _.Status is AsyncStatus.Error ? throw _.ErrorCode : _.GetResults().Any(_ => _.GetResourceGroups().SelectMany(_ => _.GetProcessDiagnosticInfos()).Any()); }
            finally { _.Close(); }
        }
    }

    internal int Launch()
    {
        PackageDebugSettings.EnableDebugging(AppInfo.Package.Id.FullName, default, default);
        ApplicationActivationManager.ActivateApplication(AppInfo.AppUserModelId, default, AO_NOERRORUI, out var processId);
        return processId;
    }
}