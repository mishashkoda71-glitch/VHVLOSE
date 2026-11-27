# VHVLOSE per-user setup
# Installs only for the current Windows user. It does not require administrator rights.
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

function Write-Step([string]$Message) {
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

try {
    if (-not [Environment]::Is64BitOperatingSystem) {
        throw "VHVLOSE currently supports 64-bit Windows only."
    }

    $repoRoot = $PSScriptRoot
    $clientProject = Join-Path $repoRoot "Client\VHVLOSE.Client.csproj"
    $launcherProject = Join-Path $repoRoot "Launcher\VHVLOSE.Launcher.csproj"
    if (-not (Test-Path $clientProject) -or -not (Test-Path $launcherProject)) {
        throw "Project files were not found. Extract the complete ZIP and run Setup.ps1 from the VHVLOSE-main root folder."
    }

    $installRoot = Join-Path $env:LOCALAPPDATA "Programs\VHVLOSE"
    $privateDotnet = Join-Path $env:LOCALAPPDATA "VHVLOSE\dotnet"
    $desktopRuntimeFound = $false

    Write-Step "Checking .NET 8 Desktop Runtime"
    $dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnetCommand) {
        $runtimeList = & dotnet --list-runtimes 2>$null | Out-String
        if ($runtimeList -match "Microsoft\.WindowsDesktop\.App 8\.") {
            $desktopRuntimeFound = $true
            Write-Host ".NET 8 Desktop Runtime is already installed." -ForegroundColor Green
        }
    }

    if (-not $desktopRuntimeFound) {
        $privateRuntimeFolder = Join-Path $privateDotnet "shared\Microsoft.WindowsDesktop.App"
        if (Test-Path $privateRuntimeFolder) {
            $privateRuntime = Get-ChildItem $privateRuntimeFolder -Directory -ErrorAction SilentlyContinue |
                Where-Object { $_.Name -like "8.*" } | Select-Object -First 1
            $desktopRuntimeFound = [bool]$privateRuntime
        }
    }

    if (-not $desktopRuntimeFound) {
        Write-Step "Installing .NET 8 Desktop Runtime for this Windows account"
        New-Item -ItemType Directory -Force -Path $privateDotnet | Out-Null
        $installerScript = Join-Path $env:TEMP "vhvlose-dotnet-install.ps1"
        Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $installerScript -UseBasicParsing
        & $installerScript -Channel "8.0" -Runtime "windowsdesktop" -Architecture "x64" -InstallDir $privateDotnet -NoPath
        if ($LASTEXITCODE -ne 0) {
            throw ".NET 8 Desktop Runtime installation failed (exit code $LASTEXITCODE)."
        }
        $env:DOTNET_ROOT = $privateDotnet
        $env:PATH = "$privateDotnet;$env:PATH"
        $privateRuntimeFolder = Join-Path $privateDotnet "shared\Microsoft.WindowsDesktop.App"
        if (-not (Test-Path $privateRuntimeFolder)) {
            throw ".NET installation finished, but the Desktop Runtime folder was not found."
        }
        Write-Host ".NET 8 Desktop Runtime installed for the current user." -ForegroundColor Green
    }

    Write-Step "Checking .NET SDK"
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "The .NET SDK is required to build this source package. Install a compatible .NET SDK, then run Setup.ps1 again."
    }
    & dotnet --version
    if ($LASTEXITCODE -ne 0) {
        throw ".NET SDK could not be started."
    }

    Write-Step "Publishing VHVLOSE Client"
    New-Item -ItemType Directory -Force -Path $installRoot | Out-Null
    $clientOutput = Join-Path $installRoot "Client"
    & dotnet publish $clientProject -c Release -r win-x64 --self-contained false -o $clientOutput
    if ($LASTEXITCODE -ne 0) { throw "Client publish failed." }

    Write-Step "Publishing VHVLOSE Launcher"
    $launcherOutput = Join-Path $installRoot "Launcher"
    & dotnet publish $launcherProject -c Release -r win-x64 --self-contained false -o $launcherOutput
    if ($LASTEXITCODE -ne 0) { throw "Launcher publish failed." }

    $launcherExe = Join-Path $launcherOutput "VHVLOSE.Launcher.exe"
    if (-not (Test-Path $launcherExe)) { throw "Launcher executable was not produced." }

    Write-Step "Creating a shortcut for this Windows account"
    $startScript = Join-Path $installRoot "Start-VHVLOSE.cmd"
    @(
        "@echo off",
        "setlocal",
        'if exist "%LOCALAPPDATA%\VHVLOSE\dotnet\dotnet.exe" (',
        '  set "DOTNET_ROOT=%LOCALAPPDATA%\VHVLOSE\dotnet"',
        '  set "PATH=%LOCALAPPDATA%\VHVLOSE\dotnet;%PATH%"',
        ")",
        'start "" "%~dp0Launcher\VHVLOSE.Launcher.exe"'
    ) | Set-Content -Path $startScript -Encoding Ascii

    $desktop = [Environment]::GetFolderPath("Desktop")
    $shortcutPath = Join-Path $desktop "VHVLOSE.lnk"
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $startScript
    $shortcut.WorkingDirectory = $installRoot
    $shortcut.Description = "VHVLOSE standalone desktop companion"
    $shortcut.Save()

    Write-Host ""
    Write-Host "VHVLOSE installed successfully for the current Windows account." -ForegroundColor Green
    Write-Host "Install folder: $installRoot"
    Write-Host "Shortcut: $shortcutPath"
    Write-Host "No CS2 files or system-wide Windows settings were changed."
}
catch {
    Write-Host ""
    Write-Host "SETUP FAILED: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}