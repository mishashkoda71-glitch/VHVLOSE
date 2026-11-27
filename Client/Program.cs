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

        ApplicationConfiguration.Initialize();

        if (!LicenseConsent.IsCurrentAccepted())
        {
            using var license = new LicenseForm();
            if (license.ShowDialog() != DialogResult.OK)
                return;

            LicenseConsent.RecordAcceptance();
        }

        ClientSettings.ConfigureGameRoot(gameRoot);
        Application.Run(new MainForm());
    }
}
