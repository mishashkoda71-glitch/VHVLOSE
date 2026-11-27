using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VHVLOSE.Client;

internal static class LicenseKeyService
{
    // Public demo key for installation testing. Never distribute it as a paid/public key.
    public const string TestKey = "VHV-TEST-2026";
    private static readonly string ActivationPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VHVLOSE", "activation.json");

    public static bool IsActivated()
    {
        try
        {
            if (!File.Exists(ActivationPath)) return false;
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(ActivationPath));
            return doc.RootElement.TryGetProperty("keyHash", out var hash) &&
                   hash.GetString() == Hash(TestKey);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException)
        {
            return false;
        }
    }

    public static bool TryActivate(string key, out string message)
    {
        string normalized = new(key.Trim().Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (!string.Equals(normalized, TestKey, StringComparison.OrdinalIgnoreCase))
        {
            message = "Ключ не найден или недействителен. Проверьте его и попробуйте ещё раз.";
            return false;
        }

        try
        {
            string directory = Path.GetDirectoryName(ActivationPath)!;
            Directory.CreateDirectory(directory);
            string json = JsonSerializer.Serialize(new
            {
                keyHash = Hash(TestKey),
                activatedAtUtc = DateTimeOffset.UtcNow,
                licenseType = "TEST"
            }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ActivationPath, json, Encoding.UTF8);
            message = "Тестовый ключ активирован. Добро пожаловать в VHVLOSE!";
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            message = "Не удалось сохранить активацию. Проверьте права доступа к папке профиля Windows.";
            return false;
        }
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

internal sealed class KeyActivationForm : Form
{
    private readonly TextBox keyInput;
    private readonly Button activateButton;

    public KeyActivationForm()
    {
        Text = "VHVLOSE — Активация";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(500, 310);
        BackColor = Color.FromArgb(14, 15, 22);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        var title = new Label
        {
            Text = "VHVLOSE",
            Font = new Font("Segoe UI", 27, FontStyle.Bold),
            ForeColor = Color.FromArgb(153, 104, 255),
            AutoSize = true,
            Location = new Point(34, 25)
        };
        var heading = new Label
        {
            Text = "Активация продукта",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(38, 88)
        };
        var description = new Label
        {
            Text = "Введите ключ доступа, полученный в официальном канале VHVLOSE.",
            ForeColor = Color.FromArgb(157, 160, 178),
            AutoSize = false,
            Size = new Size(420, 42),
            Location = new Point(40, 128)
        };
        keyInput = new TextBox
        {
            Location = new Point(40, 181),
            Size = new Size(420, 34),
            Font = new Font("Consolas", 12),
            CharacterCasing = CharacterCasing.Upper,
            BackColor = Color.FromArgb(24, 26, 37),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            PlaceholderText = "VHV-XXXX-XXXX-XXXX"
        };
        activateButton = new Button
        {
            Text = "Активировать",
            Location = new Point(300, 235),
            Size = new Size(160, 42),
            BackColor = Color.FromArgb(153, 104, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        activateButton.FlatAppearance.BorderSize = 0;
        activateButton.Click += (_, _) => Activate();
        keyInput.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Activate(); }
        };
        Controls.AddRange([title, heading, description, keyInput, activateButton]);
        AcceptButton = activateButton;
    }

    private void Activate()
    {
        if (!LicenseKeyService.TryActivate(keyInput.Text, out string message))
        {
            MessageBox.Show(this, message, "VHVLOSE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(this, message, "VHVLOSE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
