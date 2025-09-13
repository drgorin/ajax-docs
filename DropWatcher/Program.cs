using System.Runtime.InteropServices;

internal class Program
{
    private static readonly string WatchPath = @"C:\\dropme";

    private static void Main(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("This sample is intended to run on Windows.");
        }

        try
        {
            if (!Directory.Exists(WatchPath))
            {
                Directory.CreateDirectory(WatchPath);
                Console.WriteLine($"Created directory: {WatchPath}");
            }

            using var watcher = new FileSystemWatcher(WatchPath)
            {
                IncludeSubdirectories = false,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
            };

            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Watching for files dropped into {WatchPath}");
            Console.WriteLine("Press Ctrl+C to exit.");

            // Keep the process alive
            using var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex}");
        }
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Created: {e.FullPath}");
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed: {e.OldFullPath} -> {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.Error.WriteLine($"Watcher error: {e.GetException()}");
    }
}

