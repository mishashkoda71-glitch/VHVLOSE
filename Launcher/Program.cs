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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("      VHVLOSE CLIENT READY");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("This prototype does not inject into or modify the game.");
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
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
