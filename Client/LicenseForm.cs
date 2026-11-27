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
        Text = "VHVLOSE — Лицензионное соглашение";
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
        var eyebrow = new Label { Text = "ДЕСКТОПНЫЙ КЛИЕНТ", AutoSize = true, ForeColor = Color.FromArgb(195, 180, 226), Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(28, 125) };
        var brandLine = new Panel { BackColor = Accent, Location = new Point(28, 160), Size = new Size(48, 3) };
        var sideCopy = new Label { Text = "Перед началом работы\nознакомьтесь с условиями.\n\nВаш выбор сохраняется\nна этом устройстве.", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 10), Location = new Point(28, 190) };
        var version = new Label { Text = "ЛИЦЕНЗИЯ  •  ВЕРСИЯ 1.0", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(28, 590) };
        brandPanel.Controls.AddRange([brand, eyebrow, brandLine, sideCopy, version]);

        var heading = new Label { Text = "Лицензионное соглашение", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 22, FontStyle.Bold), Location = new Point(282, 28) };
        var subtitle = new Label { Text = "Пожалуйста, ознакомьтесь с условиями перед использованием VHVLOSE.", AutoSize = true, ForeColor = Muted, Location = new Point(285, 72) };

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
@"1. ПРИНЯТИЕ УСЛОВИЙ

By selecting “I have read and accept”, you confirm that you have had the opportunity to review these terms. If you do not agree, select “Отклонить” and close the application.

2. ЧТО ТАКОЕ VHVLOSE

VHVLOSE — самостоятельное настольное приложение-компаньон. В текущую версию входят лаунчер, локальные настройки, управление профилями и планировщик офлайн-тренировок. Программа не является официальным продуктом Valve или Counter-Strike и не связана с Valve.

3. ИСПОЛЬЗОВАНИЕ И ВЗАИМОДЕЙСТВИЕ С ИГРОЙ

Текущая версия проекта не внедряет код в Counter-Strike 2, не изменяет файлы игры, не читает и не меняет память игры, не создаёт внутриигровые оверлеи и помощь в прицеливании и не обходит античит. Не используйте приложение для нарушения правил игр и сервисов.

4. ОТКАЗ ОТ ГАРАНТИЙ И ОТВЕТСТВЕННОСТИ

Приложение предоставляется «как есть» и «по мере доступности». В пределах, разрешённых применимым законодательством, разработчики не гарантируют бесперебойную или безошибочную работу и пригодность приложения для конкретной цели. В пределах, разрешённых законом, разработчики не отвечают за косвенные или последующие убытки, связанные с использованием приложения. Ничто в этом соглашении не ограничивает права потребителя и ответственность, которые нельзя ограничить по закону.

5. ТЕХНИЧЕСКАЯ ПОДДЕРЖКА

Перед обращением в поддержку VHVLOSE через Telegram или другой канал внимательно прочитайте это соглашение и инструкции проекта. Если ответ уже содержится в опубликованных условиях или документации, поддержка может направить вас к соответствующему разделу и не предоставлять отдельную консультацию по этому вопросу. Это не ограничивает обязательные права пользователя, предусмотренные законом.

6. ОБНОВЛЕНИЯ И ИЗМЕНЕНИЯ

По мере развития проекта приложение и условия могут обновляться. При изменении версии лицензии вам будет предложено ознакомиться с новой редакцией и принять её до продолжения работы.

7. ВАШ ВЫБОР

Вы можете отказаться от условий и закрыть приложение в любой момент. Согласие сохраняется локально для этой учётной записи Windows, поэтому при каждом запуске повторно принимать ту же редакцию не потребуется. По вопросам об условиях можно обратиться к разработчикам проекта."
        };

        var divider = new Panel { BackColor = Color.FromArgb(49, 51, 67), Location = new Point(282, 520), Size = new Size(580, 1) };
        acceptCheck = new CheckBox
        {
            Text = "Я прочитал(а) и принимаю лицензионное соглашение",
            AutoSize = true,
            ForeColor = Color.FromArgb(222, 223, 233),
            Location = new Point(284, 540),
            Cursor = Cursors.Hand
        };
        acceptButton = new Button
        {
            Text = "Принять и продолжить",
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
