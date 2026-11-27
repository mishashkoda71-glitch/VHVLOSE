# VHVLOSE Client UI

The desktop client uses Windows Forms and .NET 8.

## Pages
- HOME — welcome, launcher information, and current profile.
- AIM — offline practice planner with editable session length and target count. It does not assist aiming in CS2.
- VISUALS — client accent color (purple, blue, or red) and compact-mode preference.
- MISC — desktop-client notification preference and version information.
- CONFIG — named profile list, profile selection, and save controls.

## Profile storage

The launcher reads the CS2 executable path from the running process and passes the installation root to the client. The profile manager uses:

`<CS2 installation folder>\\VHVLOSE\\configs\\<ProfileName>.vhv`

Each profile stores desktop-client preferences as JSON: profile name, format version, accent preset, compact-mode preference, notification preference, and offline practice-plan values. Profiles are separate from CS2's own configuration files.

If Windows denies write access (common for installations under Program Files), the UI reports the permission issue. The app does not change CS2 files or permissions.

## Safety boundary

This is a standalone companion UI. It does not attach to, inject into, read, or modify Counter-Strike 2. The AIM section is an offline session planner only.
