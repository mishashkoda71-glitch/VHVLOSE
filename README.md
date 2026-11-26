# VHVLOSE

VHVLOSE is a Windows companion launcher and standalone client UI prototype for Counter-Strike 2.

## Current scope

- Waits for the Counter-Strike 2 process (`cs2.exe`) to start.
- Detects the running game's installation directory and passes it to the client.
- Opens the standalone VHVLOSE desktop client.
- Provides a dark desktop UI with HOME, AIM, VISUALS, MISC, and CONFIG sections.
- Creates, lists, selects, and saves named VHVLOSE profile files in the game's folder.

> This project does not inject code into CS2, modify the game, read game memory, render an in-game overlay, or bypass anti-cheat. The launcher only reads the running process's executable path to locate the installation directory.

## Config storage

Profiles are stored separately from CS2 files:

```text
<CS2 installation folder>/
└── VHVLOSE/
    └── configs/
        ├── Default.vhv
        └── Custom.vhv
```

Each `.vhv` file currently stores profile metadata. The actual client preference fields will be added in later milestones.

## Project layout

- `Launcher/` — .NET console launcher.
- `Client/` — Windows Forms desktop client UI and profile manager.
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
