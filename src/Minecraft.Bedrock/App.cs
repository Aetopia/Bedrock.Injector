namespace Minecraft.Bedrock;

using System.Linq;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.System;

sealed class App(string value)
{
    const int AO_NOERRORUI = 0x00000002;

    readonly AppInfo AppInfo = AppInfo.GetFromAppUserModelId(value);

    readonly static ApplicationActivationManager ApplicationActivationManager = new();

    readonly static PackageDebugSettings PackageDebugSettings = new();

    internal string PackageFamilyName => AppInfo.PackageFamilyName;

    internal bool Running
    {
        get
        {
            var operation = AppDiagnosticInfo.RequestInfoForAppAsync(AppInfo.AppUserModelId);
            try
            {
                if (operation.Status is AsyncStatus.Started)
                {
                    using ManualResetEventSlim @event = new();
                    operation.Completed += (_, _) => @event.Set();
                    @event.Wait();
                }
                return operation.Status is AsyncStatus.Error
                ? throw operation.ErrorCode
                : operation.GetResults().SelectMany(_ => _.GetResourceGroups().SelectMany(_ => _.GetProcessDiagnosticInfos())).Any();
            }
            finally { operation.Close(); }
        }
    }

    internal int Launch()
    {
        PackageDebugSettings.EnableDebugging(AppInfo.Package.Id.FullName, default, default);
        ApplicationActivationManager.ActivateApplication(AppInfo.AppUserModelId, default, AO_NOERRORUI, out var processId);
        return processId;
    }
}