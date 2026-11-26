# VHVLOSE

VHVLOSE is a Windows companion launcher and client UI prototype for Counter-Strike 2.

## Current scope

- Waits for the Counter-Strike 2 process (`cs2.exe`) to start.
- Displays clear launcher status in the console.
- Starts a standalone VHVLOSE client-ready state when the game is detected.
- Keeps the client UI and configuration separate from the launcher.

> This starter project does not inject code into CS2, modify the game, read game memory, or bypass anti-cheat. The status message means the companion client is ready.

## Project layout

- `Launcher/` — .NET console launcher.
- `Client/` — reserved for a standalone client UI prototype.
- `Assets/` — images, icons, and other project resources.
- `Documentation/` — design and development notes.

## Build

Requires the .NET 8 SDK on Windows.

```powershell
dotnet build .\Launcher\VHVLOSE.Launcher.csproj
dotnet run --project .\Launcher\VHVLOSE.Launcher.csproj
```

The launcher waits until `cs2.exe` is detected, then prints `VHVLOSE CLIENT READY`.
