# DECISIONS.md

## Assignment 11 — Data and Saves

**2026-11-18**
- Used AI assistance to understand the assignment requirements and review the existing game systems.
- Checked the player health, enemy behaviour, level transitions and ScriptableObject data.
- Tested the game after making changes and fixed issues found during testing.

**2026-11-19**
- Used AI assistance to improve JSON saving and loading with `JsonUtility`.
- Added safe handling for missing or corrupted save files, using default values.
- Connected audio settings and reset progress to the save system.
- Tested saving settings, resetting progress and saving data when the application is paused or closed.
- Verified the game starts correctly with a corrupted save file.