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
- [ ] Add editable client preference fields to profiles.
- [ ] Add import/export for client-only settings.

## Phase 3 — Quality
- [ ] Add automated Windows build checks.
- [ ] Add version information and release notes.
- [x] Document Windows setup and profile storage.
- [ ] Test the launcher and client on Windows.

## Boundaries
The project remains a standalone companion/prototype. It will not inject code into CS2, manipulate game memory, render an in-game overlay, evade anti-cheat, or provide unfair gameplay automation.
