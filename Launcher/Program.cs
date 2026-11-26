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
            if (Process.GetProcessesByName(TargetProcessName).Length > 0)
            {
                Console.WriteLine("[+] Counter-Strike 2 detected");
                Console.WriteLine("[+] VHVLOSE companion client is ready");
                Console.WriteLine();

                string? clientPath = FindClientExecutable();
                if (clientPath is not null)
                {
                    Process.Start(new ProcessStartInfo(clientPath) { UseShellExecute = true });
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      VHVLOSE CLIENT OPENED");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("      VHVLOSE CLIENT READY");
                    Console.WriteLine("[!] Client executable not found. Build the Client project first.");
                }

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("This companion prototype does not inject into or modify the game.");
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
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
