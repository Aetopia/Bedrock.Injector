namespace Bedrock.Injector;

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Minecraft.Bedrock;
using static Unmanaged;

static class Injector
{
    readonly static nint lpStartAddress;

    static readonly SecurityIdentifier Identifier = new("S-1-15-2-1");

    static Injector()
    {
        nint hModule = default;
        try
        {
            hModule = LoadLibraryEx("Kernel32.dll", default, LOAD_LIBRARY_SEARCH_SYSTEM32);
            lpStartAddress = GetProcAddress(hModule, "LoadLibraryW");
        }
        finally { FreeLibrary(hModule); }
    }

    internal static void Launch(IEnumerable<string> paths)
    {
        using var process = Game.Launch();
        foreach (var path in paths)
        {
            FileInfo info = new(path);
            var security = info.GetAccessControl();
            security.AddAccessRule(new(Identifier, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
            info.SetAccessControl(security);

            nint hThread = default, lpBaseAddress = default, lpBuffer = default;
            try
            {
                var dwSize = sizeof(char) * (info.FullName.Length + 1);
                lpBaseAddress = VirtualAllocEx(process.Handle, default, dwSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
                if (lpBaseAddress == default) throw new Win32Exception(Marshal.GetLastWin32Error());

                if (!WriteProcessMemory(process.Handle, lpBaseAddress, lpBuffer = Marshal.StringToHGlobalUni(info.FullName), dwSize, default))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                hThread = CreateRemoteThread(process.Handle, default, default, lpStartAddress, lpBaseAddress, default, default);
                if (hThread == default) throw new Win32Exception(Marshal.GetLastWin32Error());
                WaitForSingleObject(hThread, Timeout.Infinite);
            }
            finally
            {
                Marshal.FreeHGlobal(lpBuffer);
                VirtualFreeEx(process.Handle, lpBaseAddress, default, MEM_RELEASE);
                CloseHandle(hThread);
            }
        }
    }
}