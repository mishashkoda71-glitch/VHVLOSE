using System.Drawing;

namespace VHVLOSE.Client;

internal sealed class MainForm : Form
{
    private static readonly Color Background = Color.FromArgb(13, 14, 21);
    private static readonly Color Surface = Color.FromArgb(22, 24, 35);
    private static readonly Color SurfaceRaised = Color.FromArgb(29, 31, 44);
    private static readonly Color Muted = Color.FromArgb(145, 148, 166);
    private Color accent = Color.FromArgb(153, 104, 255);

    private readonly Panel content = new() { Dock = DockStyle.Fill, BackColor = Background, Padding = new Padding(30, 24, 30, 24) };
    private readonly Panel sidebar = new() { Dock = DockStyle.Left, Width = 205, BackColor = Surface };
    private readonly Panel topbar = new() { Dock = DockStyle.Top, Height = 66, BackColor = Color.FromArgb(17, 18, 27) };
    private readonly Label pageTitle = new() { AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 19, FontStyle.Bold), Location = new Point(0, 0) };
    private readonly Label pageSubtitle = new() { AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 9), Location = new Point(2, 39) };
    private readonly Label brand = new() { AutoSize = true, Text = "VHVLOSE", Font = new Font("Segoe UI", 21, FontStyle.Bold), Location = new Point(22, 23) };
    private readonly List<Button> navButtons = [];
    private ClientSettings settings = new();

    public MainForm()
    {
        Text = "VHVLOSE";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 620);
        Size = new Size(1120, 740);
        BackColor = Background;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9.5f);
        DoubleBuffered = true;

        settings = ClientSettings.Load();
        ApplyAccent();
        BuildSidebar();
        BuildTopbar();

        content.Controls.Add(pageTitle);
        content.Controls.Add(pageSubtitle);
        Controls.Add(content);
        Controls.Add(topbar);
        Controls.Add(sidebar);
        ShowPage("HOME");
    }

    private void BuildSidebar()
    {
        brand.ForeColor = accent;
        sidebar.Controls.Add(brand);
        var sub = new Label { Text = "DESKTOP COMPANION", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), Location = new Point(25, 58) };
        sidebar.Controls.Add(sub);
        var line = new Panel { BackColor = Color.FromArgb(48, 49, 66), Location = new Point(20, 91), Size = new Size(165, 1) };
        sidebar.Controls.Add(line);

        string[] pages = ["HOME", "AIM", "VISUALS", "MISC", "CONFIG"];
        string[] labels = ["⌂   Overview", "◎   Practice", "◈   Appearance", "⚙   Preferences", "▤   Profiles"];
        for (int i = 0; i < pages.Length; i++)
        {
            string page = pages[i];
            var button = new Button
            {
                Text = labels[i], Tag = page, Location = new Point(13, 116 + i * 51),
                Size = new Size(179, 42), FlatStyle = FlatStyle.Flat,
                BackColor = Surface, ForeColor = Color.FromArgb(205, 207, 220),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(13, 0, 0, 0),
                Font = new Font("Segoe UI", 9.5f), Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = SurfaceRaised;
            button.Click += (_, _) => ShowPage(page);
            navButtons.Add(button);
            sidebar.Controls.Add(button);
        }

        var bottomLine = new Panel { BackColor = Color.FromArgb(48, 49, 66), Location = new Point(20, 0), Size = new Size(165, 1), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
        var footer = new Label { Text = "VHVLOSE  •  BUILD 0.3", AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(22, 0), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
        sidebar.Controls.Add(bottomLine);
        sidebar.Controls.Add(footer);
        sidebar.Resize += (_, _) =>
        {
            bottomLine.Location = new Point(20, sidebar.ClientSize.Height - 58);
            footer.Location = new Point(22, sidebar.ClientSize.Height - 40);
        };
    }

    private void BuildTopbar()
    {
        var statusDot = new Panel { BackColor = Color.FromArgb(73, 201, 139), Size = new Size(8, 8), Location = new Point(0, 0) };
        var status = new Label { Text = "CLIENT READY", AutoSize = true, ForeColor = Color.FromArgb(165, 218, 190), Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(14, -4) };
        var statusWrap = new Panel { Size = new Size(120, 24), Location = new Point(0, 21), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        statusDot.Location = new Point(0, 4);
        statusWrap.Controls.Add(statusDot);
        statusWrap.Controls.Add(status);
        topbar.Controls.Add(statusWrap);
        topbar.Resize += (_, _) => statusWrap.Left = topbar.ClientSize.Width - statusWrap.Width - 28;
    }

    private void ShowPage(string page)
    {
        content.SuspendLayout();
        content.Controls.Clear();
        pageTitle.Text = page switch
        {
            "HOME" => "Overview",
            "AIM" => "Practice planner",
            "VISUALS" => "Appearance",
            "MISC" => "Preferences",
            "CONFIG" => "Profiles",
            _ => page
        };
        pageSubtitle.Text = page switch
        {
            "HOME" => "Your VHVLOSE workspace at a glance.",
            "AIM" => "Plan an offline training session.",
            "VISUALS" => "Personalize the desktop client.",
            "MISC" => "General application preferences.",
            "CONFIG" => "Save and load local client profiles.",
            _ => ""
        };
        content.Controls.Add(pageTitle);
        content.Controls.Add(pageSubtitle);

        foreach (var button in navButtons)
        {
            bool selected = string.Equals(button.Tag?.ToString(), page, StringComparison.Ordinal);
            button.BackColor = selected ? Color.FromArgb(48, 37, 72) : Surface;
            button.ForeColor = selected ? Color.White : Color.FromArgb(190, 192, 207);
            button.Font = new Font("Segoe UI", 9.5f, selected ? FontStyle.Bold : FontStyle.Regular);
            button.FlatAppearance.BorderColor = selected ? accent : Surface;
            button.FlatAppearance.BorderSize = selected ? 1 : 0;
        }

        switch (page)
        {
            case "HOME":
                AddHero("WELCOME BACK", "VHVLOSE is ready when you are.", "A clean desktop companion for local preferences and offline practice planning.", 0, 92, 3);
                AddCard("GAME CONNECTION", "The launcher detects the CS2 process and locates its installation folder. It does not modify game files or gameplay.", 0, 250, 2);
                AddCard("ACTIVE PROFILE", settings.ProfileName, 0, 370, 2);
                break;
            case "AIM": BuildPracticePage(); break;
            case "VISUALS": BuildVisualsPage(); break;
            case "MISC": BuildMiscPage(); break;
            case "CONFIG": BuildConfigPage(); break;
        }
        content.ResumeLayout();
    }

    private void BuildPracticePage()
    {
        AddCard("OFFLINE SESSION", "Set up a simple practice plan. These values are stored locally and do not control CS2.", 0, 92, 3, 104);
        AddSettingLabel("SESSION LENGTH", 22, 228);
        var minutes = MakeNumber(settings.PracticeSessionMinutes, 1, 120, 22, 256);
        minutes.ValueChanged += (_, _) => settings.PracticeSessionMinutes = (int)minutes.Value;
        content.Controls.Add(minutes);
        AddSettingLabel("TARGET COUNT", 250, 228);
        var targets = MakeNumber(settings.PracticeTargetCount, 5, 500, 250, 256);
        targets.Increment = 5;
        targets.ValueChanged += (_, _) => settings.PracticeTargetCount = (int)targets.Value;
        content.Controls.Add(targets);
        AddCard("WHAT THIS DOES", "This is a session planner only. No aim assistance, automation, injection, or interaction with game memory is provided.", 0, 340, 3, 108);
        AddSaveHint();
    }

    private NumericUpDown MakeNumber(int value, int min, int max, int x, int y) => new()
    {
        Minimum = min, Maximum = max, Value = Math.Clamp(value, min, max),
        Location = new Point(x, y), Size = new Size(190, 38),
        BackColor = SurfaceRaised, ForeColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 11)
    };

    private void BuildVisualsPage()
    {
        AddCard("COLOR THEME", "Choose the accent color used throughout the VHVLOSE interface.", 0, 92, 3, 104);
        AddSettingLabel("ACCENT PRESET", 22, 228);
        var picker = new ComboBox
        {
            Location = new Point(22, 256), Size = new Size(230, 38),
            DropDownStyle = ComboBoxStyle.DropDownList, BackColor = SurfaceRaised,
            ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10)
        };
        picker.Items.AddRange(["Purple", "Blue", "Red"]);
        picker.SelectedItem = settings.AccentPreset;
        picker.SelectedIndexChanged += (_, _) =>
        {
            settings.AccentPreset = picker.SelectedItem?.ToString() ?? "Purple";
            ApplyAccent();
            ShowPage("VISUALS");
        };
        content.Controls.Add(picker);
        AddCard("LAYOUT", "Compact spacing preference is saved with your local profile.", 0, 330, 3, 90);
        var compact = new CheckBox { Text = "Use compact interface spacing", Checked = settings.CompactMode, AutoSize = true, ForeColor = Color.White, Location = new Point(22, 445), Cursor = Cursors.Hand };
        compact.CheckedChanged += (_, _) => settings.CompactMode = compact.Checked;
        content.Controls.Add(compact);
        AddSaveHint();
    }

    private void BuildMiscPage()
    {
        AddCard("NOTIFICATIONS", "Choose whether VHVLOSE may show its own desktop notifications.", 0, 92, 3, 104);
        var notifications = new CheckBox { Text = "Show client notifications", Checked = settings.ShowNotifications, AutoSize = true, ForeColor = Color.White, Location = new Point(22, 228), Cursor = Cursors.Hand };
        notifications.CheckedChanged += (_, _) => settings.ShowNotifications = notifications.Checked;
        content.Controls.Add(notifications);
        AddCard("ABOUT VHVLOSE", "Version 0.3  •  Windows desktop companion  •  Local profile storage", 0, 300, 3, 94);
        AddSaveHint();
    }

    private void BuildConfigPage()
    {
        string? directory = ClientSettings.GetConfigDirectory();
        AddCard("PROFILE MANAGER", "Keep separate sets of VHVLOSE desktop preferences in .vhv files.", 0, 92, 3, 104);
        AddSettingLabel("SAVED PROFILES", 22, 226);
        var list = new ComboBox { Location = new Point(22, 254), Size = new Size(290, 38), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = SurfaceRaised, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        foreach (string profile in ClientSettings.ListProfiles()) list.Items.Add(profile);
        if (list.Items.Count == 0) list.Items.Add("Default");
        list.SelectedItem = list.Items.Contains(settings.ProfileName) ? settings.ProfileName : list.Items[0];

        AddSettingLabel("PROFILE NAME", 22, 318);
        var name = new TextBox { Text = list.SelectedItem?.ToString() ?? "Default", Location = new Point(22, 346), Size = new Size(290, 36), BackColor = SurfaceRaised, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
        var status = new Label { Text = directory is null ? "Launch VHVLOSE through its launcher while CS2 is running to detect the game folder." : "Profiles are saved in the detected CS2 installation folder.", Location = new Point(22, 454), Size = new Size(680, 42), ForeColor = Muted };
        var load = MakeActionButton("LOAD PROFILE", 340, 252, SurfaceRaised, 130);
        load.FlatAppearance.BorderColor = accent;
        load.Click += (_, _) =>
        {
            if (list.SelectedItem is string selected)
            {
                settings = ClientSettings.Load(selected);
                name.Text = settings.ProfileName;
                ApplyAccent();
                status.ForeColor = Color.LightGreen;
                status.Text = $"Loaded profile: {selected}.";
            }
        };
        var save = MakeActionButton("SAVE PROFILE", 22, 400, accent, 150);
        save.Click += (_, _) =>
        {
            string profile = name.Text.Trim();
            if (profile.Length == 0 || profile.Length > 32 || profile.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Enter a profile name from 1 to 32 valid characters.";
                return;
            }
            try
            {
                settings.ProfileName = profile;
                string path = settings.Save();
                if (!list.Items.Contains(profile)) list.Items.Add(profile);
                list.SelectedItem = profile;
                status.ForeColor = Color.LightGreen;
                status.Text = $"Saved successfully: {path}";
            }
            catch (UnauthorizedAccessException)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Windows denied access to the selected folder.";
            }
            catch (Exception)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Could not save. Start the client from the launcher while CS2 is running.";
            }
        };
        content.Controls.AddRange([list, name, load, save, status]);
        AddCard("STORAGE LOCATION", directory ?? "Game folder not detected yet", 0, 520, 3, 80);
    }

    private void AddHero(string eyebrow, string title, string body, int x, int y, int span)
    {
        var panel = MakePanel(x, y, span, 136);
        var tag = new Label { Text = eyebrow, AutoSize = true, ForeColor = accent, Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(22, 17) };
        var headline = new Label { Text = title, AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 17, FontStyle.Bold), Location = new Point(22, 42) };
        var detail = new Label { Text = body, AutoSize = false, ForeColor = Muted, Font = new Font("Segoe UI", 9.5f), Location = new Point(22, 78), Size = new Size(panel.Width - 44, 42), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        var accentBar = new Panel { BackColor = accent, Size = new Size(4, 88), Location = new Point(panel.Width - 5, 24), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        panel.Controls.AddRange([tag, headline, detail, accentBar]);
        content.Controls.Add(panel);
    }

    private void AddCard(string heading, string body, int x, int y, int span, int height = 94)
    {
        var panel = MakePanel(x, y, span, height);
        var title = new Label { Text = heading, AutoSize = true, ForeColor = accent, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(20, 16) };
        var detail = new Label { Text = body, AutoSize = false, ForeColor = Color.FromArgb(218, 219, 229), Font = new Font("Segoe UI", 9.5f), Location = new Point(20, 44), Size = new Size(panel.Width - 40, height - 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        panel.Controls.AddRange([title, detail]);
        content.Controls.Add(panel);
    }

    private Panel MakePanel(int x, int y, int span, int height)
    {
        int width = Math.Max(420, content.ClientSize.Width - x - 60);
        var panel = new Panel { Location = new Point(x, y), Size = new Size(width, height), BackColor = Surface, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        return panel;
    }

    private void AddSettingLabel(string text, int x, int y)
    {
        content.Controls.Add(new Label { Text = text, AutoSize = true, ForeColor = Muted, Font = new Font("Segoe UI", 8, FontStyle.Bold), Location = new Point(x, y) });
    }

    private Button MakeActionButton(string text, int x, int y, Color color, int width)
    {
        var button = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 39), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, .12f);
        return button;
    }

    private void AddSaveHint()
    {
        var hint = new Label { Text = "Changes are kept in memory for this session. Save a profile in the PROFILES section to keep them.", AutoSize = true, ForeColor = Muted, Location = new Point(2, 540), Font = new Font("Segoe UI", 8) };
        content.Controls.Add(hint);
    }

    private void ApplyAccent()
    {
        accent = settings.AccentPreset switch
        {
            "Blue" => Color.FromArgb(70, 145, 255),
            "Red" => Color.FromArgb(238, 78, 100),
            _ => Color.FromArgb(153, 104, 255)
        };
        brand.ForeColor = accent;
    }
}
