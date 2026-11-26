# VHVLOSE Client UI

The desktop client uses Windows Forms and .NET 8.

## Pages
- HOME — welcome and launcher status information.
- AIM — placeholder page for offline UI demonstrations.
- VISUALS — theme/visual preferences mock-up.
- MISC — general preferences.
- CONFIG — named profile list, profile selection, and save controls.

## Profile storage

The launcher reads the CS2 executable path from the running process and passes the installation root to the client. The profile manager uses:

`<CS2 installation folder>\\VHVLOSE\\configs\\<ProfileName>.vhv`

Profiles are separate from CS2's own configuration files. At this stage, a profile contains its name and a format version; actual standalone client preferences will be added later.

If Windows denies write access (common for installations under Program Files), the UI reports the permission issue. The app does not change CS2 files or permissions.

## Safety boundary

This is a standalone companion UI. It does not attach to, inject into, read, or modify Counter-Strike 2.
