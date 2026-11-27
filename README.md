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

> This project does not inject code into CS2, modify the game, read game memory, render an in-game overlay, or bypass anti-cheat. The launcher only reads the running process's executable path to locate the installation directory. AIM is an offline practice planner, not an aim-assistance feature.

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

- `Launcher/` — .NET console launcher.
- `Client/` — Windows Forms desktop client and profile manager.
- `Assets/` — images, icons, and other project resources.
- `Documentation/` — design and development notes.

## Build

Requires the .NET 8 SDK on Windows.

From the repository root, build both projects:

```powershell
dotnet build .\Client\VHVLOSE.Client.csproj -c Release
dotnet build .\Launcher\VHVLOSE.Launcher.csproj -c Release
dotnet run --project .\Launcher\VHVLOSE.Launcher.csproj -c Release
```

Keep the repository folder structure intact. Run the launcher while CS2 is running so it can identify the correct installation folder and pass it to the client.

Windows may restrict writing under `C:\Program Files`. If saving a profile is denied, the client will show a permissions message; do not move or overwrite CS2 files.

See [Documentation/UI.md](Documentation/UI.md) and [Documentation/ROADMAP.md](Documentation/ROADMAP.md).
