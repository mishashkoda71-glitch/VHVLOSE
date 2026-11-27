# VHVLOSE Roadmap

## Phase 1 — Launcher foundation
- [x] Create .NET console launcher.
- [x] Detect the CS2 process by process name.
- [x] Read the CS2 executable location and resolve the installation root.
- [x] Pass the game installation root to the client.
- [ ] Add graceful shutdown and clearer error handling.

## Phase 2 — Standalone client UI
- [x] Create a VHVLOSE-themed desktop window.
- [x] Add navigation: HOME, AIM, VISUALS, MISC, CONFIG.
- [x] Add active navigation state and hover feedback.
- [x] Create the VHVLOSE/configs folder under the CS2 installation root when saving.
- [x] Save, list, and select named .vhv profile files.
- [x] Add editable client-only preference fields to profiles.
- [ ] Add import/export for client-only settings.
- [x] Add an offline practice-session planner.
- [x] Add client accent-color and compact-mode preferences.

## Phase 3 — Quality
- [ ] Add automated Windows build checks.
- [ ] Add version information and release notes.
- [x] Document Windows setup and profile storage.
- [ ] Build and test the launcher and client on Windows 11.
- [ ] Verify profile save/load and all navigation pages on the target laptop.

## Boundaries
The project remains a standalone companion. It will not inject code into CS2, manipulate game memory, render an in-game overlay, evade anti-cheat, or provide unfair gameplay automation. The AIM page is an offline practice planner only.
