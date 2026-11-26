# VHVLOSE

VHVLOSE is a Windows companion launcher and standalone client UI prototype for Counter-Strike 2.

## Current scope

- Waits for the Counter-Strike 2 process (`cs2.exe`) to start.
- Displays clear launcher status in the console.
- Opens the standalone VHVLOSE desktop client when its executable is available.
- Keeps the client UI and configuration separate from the launcher.

> This project does not inject code into CS2, modify the game, read game memory, or bypass anti-cheat. The launcher status indicates that the companion client is ready.

## Project layout

- `Launcher/` — .NET console launcher.
- `Client/` — Windows Forms desktop client UI.
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

Keep the repository folder structure intact. When CS2 is detected, the launcher looks for the Release client executable and opens it. If it is not found, build the Client project first.

See [Documentation/UI.md](Documentation/UI.md) and [Documentation/ROADMAP.md](Documentation/ROADMAP.md).
