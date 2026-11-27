# VHVLOSE

VHVLOSE is a Windows companion launcher and standalone desktop client prototype for Counter-Strike 2.

## Current scope

- Waits for the Counter-Strike 2 process (`cs2.exe`) to start.
- Detects the running game's installation directory and passes it to the client.
- Opens the standalone VHVLOSE desktop client.
- Provides HOME, AIM, VISUALS, MISC, and CONFIG sections.
- Includes an offline practice-session planner (session length and target count).
- Supports client-only appearance preferences (purple, blue, or red accent; compact-mode preference).
- Supports a client notification preference.
- Creates, lists, selects, and saves named VHVLOSE profile files.
- Requires a product key on first launch and stores activation in the current Windows user's LocalAppData.

> This project does not inject code into CS2, modify the game, read game memory, render an in-game overlay, or bypass anti-cheat. The launcher only reads the running process's executable path to locate the installation directory. AIM is an offline practice planner, not an aim-assistance feature.

## Product key activation

The current development build accepts the temporary test key:

```text
VHV-TEST-2026
```

Enter it in the activation window on first launch. Activation is stored at `%LOCALAPPDATA%\\VHVLOSE\\activation.json` for the current Windows account.

**Important:** this is a development-only test key, not a secure production licensing system. It is intentionally visible in source code and can be shared or bypassed by anyone who inspects the public repository. Before distributing paid or private keys through Telegram, replace the demo check with asymmetric signed keys: keep the private signing key offline and embed only its public verification key in the client. Never publish the private signing key or put it in this repository. The test key should be disabled in a production release.

## Setup

On 64-bit Windows, extract the complete repository ZIP and run `Setup.ps1` from the repository root in PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\\Setup.ps1
```

The setup script:
- Checks for the .NET 8 Windows Desktop Runtime.
- If it is missing, downloads Microsoft's official `dotnet-install.ps1` and installs the .NET 8 Desktop Runtime into the current user's LocalAppData (no administrator elevation).
- Uses the already-installed .NET SDK to publish the client and launcher.
- Installs the app under `%LOCALAPPDATA%\\Programs\\VHVLOSE`.
- Creates a VHVLOSE shortcut on the current user's desktop.

The script does not install software system-wide, change Windows account settings, or modify CS2 files. It requires an internet connection for a missing runtime and for package restore. If the .NET SDK is absent, setup stops and explains that it is needed to build this source package.

The launcher and client are published into separate `Launcher` and `Client` folders. The shortcut uses a small command wrapper so a private per-user .NET runtime can be found when needed.

## Config storage

Profiles are stored separately from CS2 files:

```text
<CS2 installation folder>/
└── VHVLOSE/
    └── configs/
        ├── Default.vhv
        └── Custom.vhv
```

Each `.vhv` file stores VHVLOSE desktop-client preferences as JSON, including the profile name, format version, accent preset, compact-mode preference, notification preference, and offline practice-plan values. These settings do not change CS2 settings.

## Project layout

- `Setup.ps1` — per-user setup with .NET 8 Desktop Runtime detection.
- `Launcher/` — .NET console launcher.
- `Client/` — Windows Forms desktop client, key activation, and profile manager.
- `Documentation/` — design and development notes.

## Manual build

Requires a compatible .NET SDK on Windows.

From the repository root:

```powershell
dotnet build .\\Client\\VHVLOSE.Client.csproj -c Release
dotnet build .\\Launcher\\VHVLOSE.Launcher.csproj -c Release
```

If Windows denies write access to the CS2 installation folder (common for installations under `C:\\Program Files`), the client reports the permission issue; it does not change folder permissions.

See [Documentation/UI.md](Documentation/UI.md) and [Documentation/ROADMAP.md](Documentation/ROADMAP.md).


## Telegram order bot (new prototype)

The `Bot/` folder contains a Cloudflare Worker + D1 implementation for the agreed manual-payment order flow. Buyers choose 1–5 devices, receive an order number, amount, and configured bank transfer details, then submit the sender name shown by their bank. Only after the sender name is submitted does the order appear in the admin-only `/check` list. The admin verifies the incoming transfer in their banking app and confirms or cancels the order from Telegram.

See [Bot/README.md](Bot/README.md) for deployment and secret configuration. The bot is source code only at this stage: it is not deployed or connected to a Telegram bot yet. Its generated license keys are not yet integrated with the Windows client's activation verifier, so do not sell or distribute them as working desktop activations until that integration is completed. Never commit a Telegram token, admin ID, webhook secret, or bank-card details.
