using System.Drawing;

namespace VHVLOSE.Client;

internal sealed class MainForm : Form
{
    private static readonly Color BackgroundColor = Color.FromArgb(13, 14, 20);
    private static readonly Color PanelColor = Color.FromArgb(22, 24, 34);
    private static readonly Color AccentColor = Color.FromArgb(151, 93, 255);
    private readonly Panel content = new() { Dock = DockStyle.Fill, BackColor = BackgroundColor };
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
                BackColor = page == "HOME" ? AccentColor : PanelColor,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (_, _) => ShowPage(page);
            sidebar.Controls.Add(button);
        }

        var footer = new Label
        {
            Text = "STANDALONE CLIENT\nUI PREVIEW",
            ForeColor = Color.FromArgb(135, 137, 153),
            AutoSize = true,
            Location = new Point(22, 590)
        };
        sidebar.Controls.Add(footer);

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

        if (page == "HOME")
        {
            AddCard("WELCOME TO VHVLOSE", "Your desktop companion is running.", 28, 92);
            AddCard("GAME STATUS", "Launcher detection is handled separately. No game process is modified.", 28, 210);
            AddCard("BUILD", "Early UI preview • Version 0.1.0", 28, 328);
            return;
        }

        string description = page switch
        {
            "AIM" => "Reserved for harmless interface settings and offline demonstrations.",
            "VISUALS" => "Visual theme and overlay mock-up settings only; no game overlay is attached.",
            "MISC" => "General client preferences.",
            "CONFIG" => "Configuration profiles will be added in a later milestone.",
            _ => "This section is under construction."
        };
        AddCard(page + " SETTINGS", description, 28, 92);
        AddCard("STATUS", "UI prototype — controls are placeholders and do not affect CS2.", 28, 210);
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
