using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[Guid("2E941141-7F97-4756-BA1D-9DECDE894A3D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16)]
partial interface IApplicationActivationManager
{
    [PreserveSig]
    int ActivateApplication(string appUserModelId, string arguments, int options, out int processId);

    [PreserveSig]
    int ActivateForFile(string appUserModelId, nint itemArray, string verb, out int processId);

    [PreserveSig]
    int ActivateForProtocol(string appUserModelId, nint itemArray, out int processId);
}

[GeneratedComClass]
sealed partial class ApplicationActivationManager : IApplicationActivationManager
{
    static readonly Type Type = Type.GetTypeFromCLSID(new("45BA127D-10A8-46EA-8AB7-56EA9078943C"));

    readonly IApplicationActivationManager _ = (IApplicationActivationManager)Activator.CreateInstance(Type);

    public int ActivateApplication(string appUserModelId, string arguments, int options, out int processId) => _.ActivateApplication(appUserModelId, arguments, options, out processId);

    public int ActivateForFile(string appUserModelId, nint itemArray, string verb, out int processId) => _.ActivateForFile(appUserModelId, itemArray, verb, out processId);

    public int ActivateForProtocol(string appUserModelId, nint itemArray, out int processId) => _.ActivateForProtocol(appUserModelId, itemArray, out processId);
}