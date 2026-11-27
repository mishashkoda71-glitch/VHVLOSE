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

        if (!LicenseKeyService.IsActivated())
        {
            using var activation = new KeyActivationForm();
            if (activation.ShowDialog() != DialogResult.OK)
                return;
        }

        if (!LicenseConsent.IsCurrentAccepted())
        {
            using var license = new LicenseForm();
            if (license.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                LicenseConsent.RecordAcceptance();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Не удалось сохранить принятие лицензии. Проверьте права доступа к папке профиля Windows.",
                    "VHVLOSE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        ClientSettings.ConfigureGameRoot(gameRoot);
        Application.Run(new MainForm());
    }
}
