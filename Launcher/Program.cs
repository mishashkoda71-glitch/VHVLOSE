using System.Diagnostics;

namespace VHVLOSE.Launcher;

internal static class Program
{
    private const string TargetProcessName = "cs2";

    private static async Task Main()
    {
        Console.Title = "VHVLOSE Launcher";
        PrintBanner();
        Console.WriteLine("[+] Launcher started");
        Console.WriteLine("[+] Waiting for Counter-Strike 2...");
        Console.WriteLine();

        while (true)
        {
            Process? game = Process.GetProcessesByName(TargetProcessName).FirstOrDefault();
            if (game is not null)
            {
                Console.WriteLine("[+] Counter-Strike 2 detected");
                string? gameRoot = FindGameRoot(game);
                string? clientPath = FindClientExecutable();

                if (clientPath is not null && gameRoot is not null)
                {
                    var startInfo = new ProcessStartInfo(clientPath) { UseShellExecute = true };
                    startInfo.ArgumentList.Add("--game-root");
                    startInfo.ArgumentList.Add(gameRoot);
                    Process.Start(startInfo);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] VHVLOSE companion client opened");
                    Console.WriteLine($"[+] Config directory: {Path.Combine(gameRoot, "VHVLOSE", "configs")}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[!] Could not locate the client or CS2 installation folder.");
                    Console.WriteLine("[!] Build the Client project and launch again.");
                }

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("This companion prototype does not inject into or modify the game.");
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    private static string? FindGameRoot(Process game)
    {
        try
        {
            string? executable = game.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(executable))
                return null;

            var directory = new DirectoryInfo(Path.GetDirectoryName(executable)!);
            // CS2 is normally installed under: <game root>\\game\\bin\\win64\\cs2.exe
            return directory.Parent?.Parent?.Parent?.FullName;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                                   or InvalidOperationException
                                   or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string? FindClientExecutable()
    {
        string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        string[] configurations = ["Release", "Debug"];

        foreach (string configuration in configurations)
        {
            string candidate = Path.Combine(root, "Client", "bin", configuration,
                "net8.0-windows", "VHVLOSE.Client.exe");
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    private static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("========================================");
        Console.WriteLine("                 VHVLOSE                ");
        Console.WriteLine("             CUSTOM COMPANION           ");
        Console.WriteLine("========================================");
        Console.ResetColor();
        Console.WriteLine();
    }
}
