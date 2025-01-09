using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16)]
partial interface IPackageDebugSettings
{
    [PreserveSig]
    int EnableDebugging(string packageFullName, string debuggerCommandLine, string environment);

    [PreserveSig]
    int DisableDebugging(string packageFullName);

    [PreserveSig]
    int Suspend(string packageFullName);

    [PreserveSig]
    int Resume(string packageFullName);

    [PreserveSig]
    int TerminateAllProcesses(string packageFullName);

    [PreserveSig]
    int SetTargetSessionId(ulong sessionId);

    [PreserveSig]
    int EnumerateBackgroundTasks(string packageFullName, nint taskCount, nint taskIds, nint taskNames);

    [PreserveSig]
    int ActivateBackgroundTask(nint taskId);

    [PreserveSig]
    int StartServicing(string packageFullName);

    [PreserveSig]
    int StopServicing(string packageFullName);

    [PreserveSig]
    int StartSessionRedirection(string packageFullName, ulong sessionId);

    [PreserveSig]
    int StopSessionRedirection(string packageFullName);

    [PreserveSig]
    int GetPackageExecutionState(string packageFullName, nint packageExecutionState);

    [PreserveSig]
    int RegisterForPackageStateChanges(string packageFullName, nint pPackageExecutionStateChangeNotification, nint pdwCookie);

    [PreserveSig]
    int UnregisterForPackageStateChanges(uint dwCookie);
}

[GeneratedComClass]
sealed partial class PackageDebugSettings : IPackageDebugSettings
{
    static readonly Type Type = Type.GetTypeFromCLSID(new("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D"));

    readonly IPackageDebugSettings _ = (IPackageDebugSettings)Activator.CreateInstance(Type);

    public int ActivateBackgroundTask(nint taskId) => _.ActivateBackgroundTask(taskId);

    public int DisableDebugging(string packageFullName) => _.DisableDebugging(packageFullName);

    public int EnableDebugging(string packageFullName, string debuggerCommandLine, string environment) => _.EnableDebugging(packageFullName, debuggerCommandLine, environment);

    public int EnumerateBackgroundTasks(string packageFullName, nint taskCount, nint taskIds, nint taskNames) => _.EnumerateBackgroundTasks(packageFullName, taskCount, taskIds, taskNames);

    public int GetPackageExecutionState(string packageFullName, nint packageExecutionState) => _.GetPackageExecutionState(packageFullName, packageExecutionState);

    public int RegisterForPackageStateChanges(string packageFullName, nint pPackageExecutionStateChangeNotification, nint pdwCookie) => _.RegisterForPackageStateChanges(packageFullName, pPackageExecutionStateChangeNotification, pdwCookie);

    public int Resume(string packageFullName) => _.Resume(packageFullName);

    public int SetTargetSessionId(ulong sessionId) => _.SetTargetSessionId(sessionId);

    public int StartServicing(string packageFullName) => _.StartServicing(packageFullName);

    public int StartSessionRedirection(string packageFullName, ulong sessionId) => _.StartSessionRedirection(packageFullName, sessionId);

    public int StopServicing(string packageFullName) => _.StartServicing(packageFullName);

    public int StopSessionRedirection(string packageFullName) => _.StopSessionRedirection(packageFullName);

    public int Suspend(string packageFullName) => _.Suspend(packageFullName);

    public int TerminateAllProcesses(string packageFullName) => _.TerminateAllProcesses(packageFullName);

    public int UnregisterForPackageStateChanges(uint dwCookie) => _.UnregisterForPackageStateChanges(dwCookie);
}