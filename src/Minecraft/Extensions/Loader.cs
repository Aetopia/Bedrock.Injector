using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Minecraft.Unmanaged;

namespace Minecraft.Extensions;
using static Native;

public static class Loader
{
    static readonly nint lpStartAddress;

    static readonly SecurityIdentifier Identifier = new("S-1-15-2-1");

    static Loader()
    {
        nint hModule = default;
        try
        {
            hModule = LoadLibraryEx("Kernel32.dll", default, LOAD_LIBRARY_SEARCH_SYSTEM32);
            lpStartAddress = GetProcAddress(hModule, "LoadLibraryW");
        }
        finally { FreeLibrary(hModule); }
    }

    static void Throw() => throw new Win32Exception(Marshal.GetLastWin32Error());

    static string Get(string path)
    {
        FileInfo info = new(path); var security = info.GetAccessControl();
        security.AddAccessRule(new(Identifier, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
        info.SetAccessControl(security);
        return info.FullName;
    }

    static void Launch(nint hProcess, string path)
    {
        nint lpBaseAddress = default, lpBuffer = default, hThread = default;
        try
        {
            var dwSize = sizeof(char) * ((path = Get(path)).Length + 1);

            if ((lpBaseAddress = VirtualAllocEx(hProcess, default, dwSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE)) == default)
                Throw();

            if (!WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer = Marshal.StringToHGlobalUni(path), dwSize, default))
              Throw();

            if ((hThread = CreateRemoteThread(hProcess, default, default, lpStartAddress, lpBaseAddress, default, default)) == default)
               Throw();

            WaitForSingleObject(hThread, Timeout.Infinite);
        }
        finally
        {
            Marshal.FreeHGlobal(lpBuffer);
            VirtualFreeEx(hProcess, lpBaseAddress, default, MEM_RELEASE);
            CloseHandle( hThread);
        }
    }

    static void Launch(IEnumerable<string> paths)
    {
        var dwProcessId = Game.Launch(); if (!paths.Any()) return;
        nint hProcess = default;
        try
        {
            hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, dwProcessId);
            foreach (var path in paths) Launch(hProcess, path);
        }
        finally { CloseHandle(hProcess); }
    }

    internal static async Task LaunchAsync(IEnumerable<string> paths) => await Task.Run(() => Launch(paths));
}