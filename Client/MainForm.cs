using System.Drawing;

namespace VHVLOSE.Client;

internal sealed class MainForm : Form
{
    private static readonly Color BackgroundColor = Color.FromArgb(13, 14, 20);
    private static readonly Color PanelColor = Color.FromArgb(22, 24, 34);
    private static readonly Color MutedColor = Color.FromArgb(135, 137, 153);

    private Color accentColor = Color.FromArgb(151, 93, 255);
    private readonly Panel content = new() { Dock = DockStyle.Fill, BackColor = BackgroundColor };
    private readonly List<Button> navigationButtons = [];
    private ClientSettings currentSettings = new();

    private readonly Label pageTitle = new()
    {
        AutoSize = true,
        ForeColor = Color.White,
        Font = new Font("Segoe UI", 19, FontStyle.Bold),
        Location = new Point(28, 24),
        Text = "HOME"
    };

    public MainForm()
    {
        Text = "VHVLOSE";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(820, 560);
        Size = new Size(1000, 680);
        BackColor = BackgroundColor;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        currentSettings = ClientSettings.Load();
        ApplyAccent();

        var sidebar = new Panel { Dock = DockStyle.Left, Width = 190, BackColor = PanelColor };
        var brand = new Label
        {
            Text = "VHVLOSE",
            ForeColor = accentColor,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(22, 24)
        };
        sidebar.Controls.Add(brand);

        string[] pages = ["HOME", "AIM", "VISUALS", "MISC", "CONFIG"];
        for (int i = 0; i < pages.Length; i++)
        {
            string page = pages[i];
            var button = new Button
            {
                Text = page,
                Tag = page,
                Location = new Point(14, 92 + i * 52),
                Size = new Size(162, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = PanelColor,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 34, 54);
            button.Click += (_, _) => ShowPage(page);
            navigationButtons.Add(button);
            sidebar.Controls.Add(button);
        }

        var footer = new Label
        {
            Text = "DESKTOP CLIENT • 0.2.0",
            ForeColor = MutedColor,
            AutoSize = true,
            Location = new Point(22, 0),
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        };
        sidebar.Controls.Add(footer);
        sidebar.Resize += (_, _) => footer.Location = new Point(22, sidebar.ClientSize.Height - footer.Height - 22);

        content.Controls.Add(pageTitle);
        Controls.Add(content);
        Controls.Add(sidebar);
        ShowPage("HOME");
    }

    private void ShowPage(string page)
    {
        content.Controls.Clear();
        pageTitle.Text = page;
        content.Controls.Add(pageTitle);

        foreach (var button in navigationButtons)
        {
            bool selected = string.Equals(button.Tag?.ToString(), page, StringComparison.Ordinal);
            button.BackColor = selected ? accentColor : PanelColor;
            button.Font = new Font("Segoe UI", 9, selected ? FontStyle.Bold : FontStyle.Regular);
        }

        switch (page)
        {
            case "HOME":
                AddCard("WELCOME TO VHVLOSE", "A standalone desktop companion with local preferences and profile management.", 28, 92);
                AddCard("GAME STATUS", "The launcher can detect CS2 and pass its installation folder to this client. Game files and gameplay are not modified.", 28, 210);
                AddCard("CURRENT PROFILE", currentSettings.ProfileName, 28, 328);
                break;

            case "AIM":
                BuildPracticePage();
                break;

            case "VISUALS":
                BuildVisualsPage();
                break;

            case "MISC":
                BuildMiscPage();
                break;

            case "CONFIG":
                BuildConfigPage();
                break;
        }
    }

    private void BuildPracticePage()
    {
        AddCard("OFFLINE PRACTICE PLANNER", "Plan a standalone aim-training session. These values are saved locally and do not control CS2.", 28, 92);

        AddSettingLabel("SESSION LENGTH (MINUTES)", 48, 218);
        var minutes = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 120,
            Value = currentSettings.PracticeSessionMinutes,
            Location = new Point(48, 248),
            Size = new Size(180, 34),
            BackColor = PanelColor,
            ForeColor = Color.White
        };
        minutes.ValueChanged += (_, _) => currentSettings.PracticeSessionMinutes = (int)minutes.Value;
        content.Controls.Add(minutes);

        AddSettingLabel("TARGETS PER SESSION", 48, 310);
        var targets = new NumericUpDown
        {
            Minimum = 5,
            Maximum = 500,
            Increment = 5,
            Value = currentSettings.PracticeTargetCount,
            Location = new Point(48, 340),
            Size = new Size(180, 34),
            BackColor = PanelColor,
            ForeColor = Color.White
        };
        targets.ValueChanged += (_, _) => currentSettings.PracticeTargetCount = (int)targets.Value;
        content.Controls.Add(targets);

        AddCard("SCOPE", "This page is a practice-session planner only. It does not provide aim assistance or interact with a game.", 28, 414);
    }

    private void BuildVisualsPage()
    {
        AddCard("CLIENT APPEARANCE", "Customize the VHVLOSE desktop interface. Changes apply to this client only.", 28, 92);
        AddSettingLabel("ACCENT COLOR", 48, 218);

        var accentPicker = new ComboBox
        {
            Location = new Point(48, 248),
            Size = new Size(220, 34),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = PanelColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        accentPicker.Items.AddRange(["Purple", "Blue", "Red"]);
        accentPicker.SelectedItem = currentSettings.AccentPreset;
        accentPicker.SelectedIndexChanged += (_, _) =>
        {
            currentSettings.AccentPreset = accentPicker.SelectedItem?.ToString() ?? "Purple";
            ApplyAccent();
            ShowPage("VISUALS");
        };
        content.Controls.Add(accentPicker);

        var compact = new CheckBox
        {
            Text = "Compact interface spacing",
            Checked = currentSettings.CompactMode,
            AutoSize = true,
            ForeColor = Color.White,
            Location = new Point(48, 310)
        };
        compact.CheckedChanged += (_, _) => currentSettings.CompactMode = compact.Checked;
        content.Controls.Add(compact);
    }

    private void BuildMiscPage()
    {
        AddCard("GENERAL PREFERENCES", "Desktop-client options. No settings on this page alter game behavior.", 28, 92);

        var notifications = new CheckBox
        {
            Text = "Show client notifications",
            Checked = currentSettings.ShowNotifications,
            AutoSize = true,
            ForeColor = Color.White,
            Location = new Point(48, 220)
        };
        notifications.CheckedChanged += (_, _) => currentSettings.ShowNotifications = notifications.Checked;
        content.Controls.Add(notifications);

        AddCard("ABOUT", "VHVLOSE 0.2.0 • Windows desktop companion • Local profile storage", 28, 286);
    }

    private void BuildConfigPage()
    {
        string? configDirectory = ClientSettings.GetConfigDirectory();
        AddCard("CONFIGURATION PROFILES", "Save and load VHVLOSE desktop preferences as local .vhv profiles.", 28, 92);

        AddSettingLabel("SAVED PROFILES", 48, 210);
        var profileList = new ComboBox
        {
            Location = new Point(48, 238),
            Size = new Size(340, 34),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = PanelColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        foreach (string profile in ClientSettings.ListProfiles())
            profileList.Items.Add(profile);
        if (profileList.Items.Count == 0)
            profileList.Items.Add("Default");
        profileList.SelectedItem = profileList.Items.Contains(currentSettings.ProfileName)
            ? currentSettings.ProfileName
            : profileList.Items[0];

        AddSettingLabel("PROFILE NAME", 48, 290);
        var profileInput = new TextBox
        {
            Text = profileList.SelectedItem?.ToString() ?? "Default",
            Location = new Point(48, 318),
            Size = new Size(340, 34),
            BackColor = PanelColor,
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 11)
        };

        var status = new Label
        {
            Text = configDirectory is null
                ? "Start the launcher while CS2 is running to detect the installation folder."
                : "Profiles are stored in the CS2 installation folder.",
            ForeColor = MutedColor,
            AutoSize = false,
            Location = new Point(48, 414),
            Size = new Size(650, 42)
        };

        var loadButton = CreateButton("LOAD", new Point(402, 236), new Size(110, 38), PanelColor);
        loadButton.FlatAppearance.BorderColor = accentColor;
        loadButton.Click += (_, _) =>
        {
            if (profileList.SelectedItem is string selected)
            {
                currentSettings = ClientSettings.Load(selected);
                profileInput.Text = currentSettings.ProfileName;
                ApplyAccent();
                status.ForeColor = Color.LightGreen;
                status.Text = $"Loaded profile: {selected}. Open another page to see its settings.";
            }
        };

        var saveButton = CreateButton("SAVE PROFILE", new Point(48, 366), new Size(150, 40), accentColor);
        saveButton.Click += (_, _) =>
        {
            string name = profileInput.Text.Trim();
            if (name.Length == 0 || name.Length > 32 ||
                name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Use a profile name of 1–32 characters without filename symbols.";
                return;
            }

            try
            {
                currentSettings.ProfileName = name;
                string savedPath = currentSettings.Save();
                if (!profileList.Items.Contains(name))
                    profileList.Items.Add(name);
                profileList.SelectedItem = name;
                status.ForeColor = Color.LightGreen;
                status.Text = $"Saved: {savedPath}";
            }
            catch (UnauthorizedAccessException)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Windows denied access. The CS2 folder may require administrator permission.";
            }
            catch (Exception)
            {
                status.ForeColor = Color.Orange;
                status.Text = "Could not save. Start the client from the launcher while CS2 is running.";
            }
        };

        content.Controls.Add(profileList);
        content.Controls.Add(profileInput);
        content.Controls.Add(loadButton);
        content.Controls.Add(saveButton);
        content.Controls.Add(status);
        AddCard("CONFIG FOLDER", configDirectory ?? "CS2 folder not detected", 28, 470);
    }

    private void AddSettingLabel(string text, int x, int y)
    {
        content.Controls.Add(new Label
        {
            Text = text,
            ForeColor = MutedColor,
            AutoSize = true,
            Location = new Point(x, y),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        });
    }

    private Button CreateButton(string text, Point location, Size size, Color background)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = background,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void ApplyAccent()
    {
        accentColor = currentSettings.AccentPreset switch
        {
            "Blue" => Color.FromArgb(65, 145, 255),
            "Red" => Color.FromArgb(235, 75, 95),
            _ => Color.FromArgb(151, 93, 255)
        };
    }

    private void AddCard(string heading, string body, int x, int y)
    {
        var card = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(Math.Max(400, content.ClientSize.Width - 56), 94),
            BackColor = PanelColor,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        var title = new Label
        {
            Text = heading,
            AutoSize = true,
            ForeColor = accentColor,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(18, 14)
        };
        var detail = new Label
        {
            Text = body,
            AutoSize = false,
            ForeColor = Color.Gainsboro,
            Location = new Point(18, 43),
            Size = new Size(card.Width - 36, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        card.Controls.Add(title);
        card.Controls.Add(detail);
        content.Controls.Add(card);
    }
}
