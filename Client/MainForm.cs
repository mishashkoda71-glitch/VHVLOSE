using System.Drawing;

namespace VHVLOSE.Client;

internal sealed class MainForm : Form
{
    private static readonly Color BackgroundColor = Color.FromArgb(13, 14, 20);
    private static readonly Color PanelColor = Color.FromArgb(22, 24, 34);
    private static readonly Color AccentColor = Color.FromArgb(151, 93, 255);
    private static readonly Color MutedColor = Color.FromArgb(135, 137, 153);

    private readonly Panel content = new() { Dock = DockStyle.Fill, BackColor = BackgroundColor };
    private readonly List<Button> navigationButtons = [];
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

        var sidebar = new Panel { Dock = DockStyle.Left, Width = 190, BackColor = PanelColor };
        var brand = new Label
        {
            Text = "VHVLOSE",
            ForeColor = AccentColor,
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
            Text = "STANDALONE CLIENT\nUI PREVIEW • 0.1.0",
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
            button.BackColor = selected ? AccentColor : PanelColor;
            button.Font = new Font("Segoe UI", 9, selected ? FontStyle.Bold : FontStyle.Regular);
        }

        if (page == "HOME")
        {
            AddCard("WELCOME TO VHVLOSE", "Your desktop companion is running.", 28, 92);
            AddCard("GAME STATUS", "The launcher can detect CS2. This client does not inject into, read, or modify the game.", 28, 210);
            AddCard("BUILD", "Early UI preview • Version 0.1.0", 28, 328);
            return;
        }

        string description = page switch
        {
            "AIM" => "This page is a visual placeholder for offline UI demonstrations. It does not control aiming or gameplay.",
            "VISUALS" => "Theme and interface preview settings only. No game overlay is attached.",
            "MISC" => "General desktop-client preferences will be added here.",
            "CONFIG" => "Local interface profiles and settings export are planned for a later milestone.",
            _ => "This section is under construction."
        };
        AddCard(page + " SETTINGS", description, 28, 92);
        AddCard("PROTOTYPE STATUS", "UI preview only — no controls on this page affect CS2.", 28, 210);
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
            ForeColor = AccentColor,
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
