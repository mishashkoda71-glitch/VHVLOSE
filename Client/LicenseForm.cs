using System.Drawing;
using System.Text.Json;

namespace VHVLOSE.Client;

internal sealed class LicenseForm : Form
{
    private static readonly Color Back = Color.FromArgb(14, 15, 22);
    private static readonly Color Surface = Color.FromArgb(24, 26, 37);
    private static readonly Color Muted = Color.FromArgb(157, 160, 178);
    private static readonly Color Accent = Color.FromArgb(153, 104, 255);
    private readonly CheckBox acceptCheck;
    private readonly Button acceptButton;

    public LicenseForm()
    {
        Text = "VHVLOSE — License agreement";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(900, 650);
        BackColor = Back;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        var brandPanel = new Panel { Dock = DockStyle.Left, Width = 245, BackColor = Color.FromArgb(22, 20, 34), Padding = new Padding(25) };
        var brand = new Label { Text = "VHV\nLOSE", AutoSize = true, ForeColor = Accent, Font = new Font("Segoe UI", 27, FontStyle.Bold), Location = new Point(25, 35) };
        var eyebrow = new Label { Text = "DESKTOP COMPANION", AutoSize = true, ForeColor = Color.FromArgb(195, 180, 226), Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(28, 125) };
        var brandLine = new Panel { BackColor = Accent, Location = new Point(28, 160), Size = new Size(48, 3) };
        var sideCopy = new Label { Text = "A quick read before you\nstep inside.\n\nPlease review the terms\ncarefully. Your choice is\nrecorded on this device.", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 10), Location = new Point(28, 190) };
        var version = new Label { Text = "LICENSE  •  VERSION 1.0", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(28, 590) };
        brandPanel.Controls.AddRange([brand, eyebrow, brandLine, sideCopy, version]);

        var heading = new Label { Text = "License agreement", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 22, FontStyle.Bold), Location = new Point(282, 28) };
        var subtitle = new Label { Text = "Please read the terms before using VHVLOSE.", AutoSize = true, ForeColor = Muted, Location = new Point(285, 72) };

        var document = new RichTextBox
        {
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Surface,
            ForeColor = Color.FromArgb(225, 226, 235),
            Font = new Font("Segoe UI", 10),
            Location = new Point(282, 112),
            Size = new Size(580, 390),
            ScrollBars = RichTextBoxScrollBars.Vertical,
            DetectUrls = false,
            WordWrap = true,
            Text =
@"1. ACCEPTANCE

By selecting “I have read and accept”, you confirm that you have had the opportunity to review these terms. If you do not agree, select “Decline” and close the application.

2. WHAT VHVLOSE IS

VHVLOSE is a standalone desktop companion prototype. Its current features include a launcher, local preferences and profile management, and an offline practice-session planner. It is not an official Valve or Counter-Strike product and is not affiliated with or endorsed by Valve.

3. PERMITTED USE AND GAME INTEGRATION

The current project does not inject code into Counter-Strike 2, modify game files, read or change game memory, provide in-game overlays or aim assistance, or bypass anti-cheat systems. Do not use the application to violate the rules or terms of services you use.

4. DISCLAIMER

The application is provided “as is” and “as available”. To the extent permitted by applicable law, the developers make no warranty that the application will be uninterrupted, error-free, or suitable for a particular purpose. To the extent permitted by law, the developers are not liable for indirect or consequential loss arising from use of the application. Nothing in these terms excludes liability or consumer rights that cannot legally be excluded.

5. SUPPORT

Before contacting the VHVLOSE support team through Telegram or another channel, please read this agreement and the project instructions. Support may refer you back to the relevant documentation and may decline to provide individual assistance where the answer is already clearly covered by the published terms or instructions. This does not affect any mandatory legal rights.

6. UPDATES AND CHANGES

The application and these terms may change as the project develops. If the license version changes, you will be asked to review and accept the updated terms before continuing.

7. YOUR CHOICE

You may decline these terms and close the application at any time. Acceptance is stored locally on this Windows account so that this screen does not need to appear on every launch. You can contact the project maintainers if you have questions about these terms."
        };

        var divider = new Panel { BackColor = Color.FromArgb(49, 51, 67), Location = new Point(282, 520), Size = new Size(580, 1) };
        acceptCheck = new CheckBox
        {
            Text = "I have read and accept the license agreement",
            AutoSize = true,
            ForeColor = Color.FromArgb(222, 223, 233),
            Location = new Point(284, 540),
            Cursor = Cursors.Hand
        };
        acceptButton = new Button
        {
            Text = "Accept and continue",
            Location = new Point(675, 580),
            Size = new Size(187, 42),
            BackColor = Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Enabled = false,
            Cursor = Cursors.Hand
        };
        acceptButton.FlatAppearance.BorderSize = 0;
        acceptButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 128, 255);
        acceptButton.Click += (_, _) => { if (acceptCheck.Checked) DialogResult = DialogResult.OK; };

        var declineButton = new Button
        {
            Text = "Decline",
            Location = new Point(555, 580),
            Size = new Size(108, 42),
            BackColor = Surface,
            ForeColor = Muted,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        declineButton.FlatAppearance.BorderColor = Color.FromArgb(64, 66, 82);
        declineButton.Click += (_, _) => DialogResult = DialogResult.Cancel;
        acceptCheck.CheckedChanged += (_, _) => acceptButton.Enabled = acceptCheck.Checked;

        Controls.AddRange([brandPanel, heading, subtitle, document, divider, acceptCheck, declineButton, acceptButton]);
        AcceptButton = acceptButton;
        CancelButton = declineButton;
    }
}

internal static class LicenseConsent
{
    private const string CurrentVersion = "1.0";
    private static string ConsentPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VHVLOSE", "license-consent.json");

    public static bool IsCurrentAccepted()
    {
        try
        {
            if (!File.Exists(ConsentPath)) return false;
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(ConsentPath));
            return document.RootElement.TryGetProperty("licenseVersion", out var version) &&
                   version.GetString() == CurrentVersion &&
                   document.RootElement.TryGetProperty("accepted", out var accepted) &&
                   accepted.GetBoolean();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException)
        {
            return false;
        }
    }

    public static void RecordAcceptance()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConsentPath)!);
        string json = JsonSerializer.Serialize(new
        {
            licenseVersion = CurrentVersion,
            accepted = true,
            acceptedAtUtc = DateTimeOffset.UtcNow
        }, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConsentPath, json);
    }
}
