using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

static partial class Unmanaged
{
    internal const int LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

    internal const int MEM_RELEASE = 0x00008000;

    internal const int PROCESS_ALL_ACCESS = 0X1FFFFF;

    internal const int MEM_COMMIT = 0x00001000;

    internal const int MEM_RESERVE = 0x00002000;

    internal const int PAGE_EXECUTE_READWRITE = 0x40;

    [LibraryImport("Kernel32", SetLastError = true)]
    internal static partial nint OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [LibraryImport("Kernel32", SetLastError = true)]
    internal static partial int WaitForSingleObject(nint hHandle, int dwMilliseconds);

    [LibraryImport("Kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(nint hObject);

    [LibraryImport("Kernel32", SetLastError = true)]
    internal static partial nint VirtualAllocEx(nint hProcess, nint lpAddress, int dwSize, int flAllocationType, int flProtect);

    [LibraryImport("Kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, nint lpBuffer, int nSize, nint lpNumberOfBytesWritten);

    [LibraryImport("Kernel32", SetLastError = true)]
    internal static partial nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, int dwStackSize, nint lpStartAddress, nint lpParameter, int dwCreationFlags, nint lpThreadId);

    [LibraryImport("Kernel32", EntryPoint = "LoadLibraryExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint LoadLibraryEx(string lpLibFileName, nint hFile, int dwFlags);

    [LibraryImport("Kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool FreeLibrary(nint hLibModule);

    [LibraryImport("Kernel32", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);

    [LibraryImport("Kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualFreeEx(nint hProcess, nint lpAddress, int dwSize, int dwFreeType);

    [LibraryImport("Shell32", EntryPoint = "ShellMessageBoxW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int ShellMessageBox(nint hAppInst = default, nint hWnd = default, string lpcText = default, string lpcTitle = "Bedrock Injector", int fuStyle = 0x00000010);
}