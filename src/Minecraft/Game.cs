using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Management.Core;

namespace Minecraft;

static class Game
{
    static readonly App App = new("Microsoft.MinecraftUWP_8wekyb3d8bbwe!App");

    const string Path = @"games\com.mojang\minecraftpe\resource_init_lock";

    internal static int Launch()
    {
        var path = ApplicationDataManager.CreateForPackageFamily(App.PackageFamilyName).LocalFolder.Path;
        using ManualResetEventSlim @event = new(App.Running && !File.Exists(System.IO.Path.Combine(path, Path)));

        using FileSystemWatcher watcher = new(path) { NotifyFilter = NotifyFilters.FileName, IncludeSubdirectories = true, EnableRaisingEvents = true };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(Path, StringComparison.OrdinalIgnoreCase)) @event.Set(); };

        using var process = Process.GetProcessById(App.Launch()); process.EnableRaisingEvents = true;
        var _ = false; process.Exited += (_, _) => { _ = true; @event.Set(); };
        @event.Wait(); return _ ? throw new OperationCanceledException() : process.Id;
    }
}