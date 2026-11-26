namespace VHVLOSE.Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        string? gameRoot = null;
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], "--game-root", StringComparison.OrdinalIgnoreCase))
            {
                gameRoot = args[i + 1];
                break;
            }
        }

        ClientSettings.ConfigureGameRoot(gameRoot);
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
