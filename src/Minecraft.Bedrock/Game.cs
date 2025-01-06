namespace Minecraft.Bedrock;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Windows.Management.Core;

static class Game
{
    readonly static App App = new("Microsoft.MinecraftUWP_8wekyb3d8bbwe!App");

    const string Path = @"games\com.mojang\minecraftpe\resource_init_lock";

    internal static Process Launch()
    {
        var path = ApplicationDataManager.CreateForPackageFamily(App.PackageFamilyName).LocalFolder.Path;
        using ManualResetEventSlim @event = new(App.Running && !File.Exists(System.IO.Path.Combine(path, Path)));

        using FileSystemWatcher watcher = new(path) { NotifyFilter = NotifyFilters.FileName, IncludeSubdirectories = true, EnableRaisingEvents = true };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(Path, StringComparison.OrdinalIgnoreCase)) @event.Set(); };

        var process = Process.GetProcessById(App.Launch());
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => throw new OperationCanceledException();
        @event.Wait(); return process;
    }
}