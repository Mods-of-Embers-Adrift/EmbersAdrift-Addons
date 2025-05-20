# 🌋 Embers Adrift Addons

This repository hosts **experimental UI and utility modifications** for *Embers Adrift* aimed at enhancing player experience. These tools help identify gameplay issues—such as class imbalances—and improve quality of life without fostering toxicity.

> Our mission: Help players **engage deeper** with the game through visibility, utility, and transparency.

---

## 1️⃣ Installation Instructions

**How to install the modified `Assembly-CSharp.dll`:**

1. Navigate to the game folder:
   ```
   SteamLibrary\steamapps\common\Embers Adrift\Embers Adrift_Data\Managed
   ```
2. **Backup your original `Assembly-CSharp.dll`.**
3. Replace it with the provided modified version from this repository.

> ⚠️ Backup is **mandatory** to restore the original state if needed.

---

## 🔄 Transition Notice: Moving to MelonLoader & Lua

We are shifting from direct `Assembly-CSharp.dll` replacement to a **MelonLoader-based mod** architecture.

- ✅ **No game file modification** – All mods loaded externally
- ✅ **Supports Lua scripting** via [LuaLoader](https://github.com/NeptuneQ/LuaLoader)
- ✅ **Extensible UI overlays and logic** without C# knowledge
- ✅ **Community-friendly** – Easy to extend and safer to distribute

> 🧠 This shift makes the addon more accessible, safer to use, and easier to extend by the community.

### 🔒 Why is this a safer route?

1. No violation of TOS clause (iii) – *no reverse engineering or file modification*
2. All enhancements are sandboxed – **original game files remain untouched**


```
EmbersAdrift_Addons/
├── Embers Adrift Game Client            # Original game files (untouched)
│
├── MelonLoader/ or External Injector      # Injects mods without modifying game files
│
├── Addon Loader/                          # Runtime-loaded C# addon container
│   ├── UI Layer/                          # Custom overlay windows (DPS, Threat, HPS)
│   ├── Harmony Patch Layer/               # Read-only method hooks for combat events
│   └── File Logger/                       # Optional local combat log or diagnostics
```
---
## 🧪 What We're Working On (MelonLoader Mods)

Here’s what we’re planning to build using MelonLoader for *Embers Adrift*:

### ✅ Core Mods
- `DPSMeter.dll` – Tracks real-time player/group DPS
- `ThreatMeter.dll` – Measures generated threat in combat
- `HPSMeter.dll` – Tracks healing done by all sources
- `SwingTimer.dll` – Melee swing animations per class
- `FightHistory.dll` – View logs from past encounters

### 📊 Visual Overlays
- `CombatHud.dll` – Compact stat display for buffs/debuffs
- `StatusBars.dll` – Health, stamina, energy bars outside native UI
- `EnemyCastBar.dll` – Displays enemy casting status
- `CooldownMonitor.dll` – Overlay for ability cooldowns

### 🧠 Intelligence Tools
- `SkillBreakdown.dll` – Shows which skills contributed to DPS/healing
- `AuraTracker.dll` – Alerts when key buffs are down
- `CombatLogRecorder.dll` – Local combat log with timestamps

### 🛠️ Utilities
- `PositionAlert.dll` – Notifies if missing positional bonus
- `LootValueScanner.dll` – Flags under/overpriced vendor items

### 💡 Lua-Based Addons (via LuaLoader)
- Create `.lua` scripts for:
- UI layouts
- Custom alert sounds
- Hotkeyed DPS reset or mode swap
- Group composition breakdown

> Got mod ideas? Open an issue or join our discussions tab!

---

## 📝 Changelogs

## 📌 Version 1.4 – Threat & DPS Enhancements

### ✅ Changes & Additions
- `UserThreatTracker` + `UserDpsTracker` for threat, DPS, and HPS
- Hooked into `CombatTextManager` to capture combat events
- Toggle UI between DPS and HPS (per-player breakdowns)
- Redesigned layout for clarity (aligned UI controls)
- Auto-hide Swing Timer after combat
- Cleaned threat list to show only valid player entries

### 🔧 Fixes & Tweaks
- Smaller UI footprint
- Ignored invalid NPC threats
- Only valid combat entries are tracked

### 🔜 Next Steps
- Implement class-based threat multipliers
- Add skill contribution breakdown (per-crit damage/heal)

![V1 4](https://github.com/user-attachments/assets/4452017d-7ae5-4ec8-a989-ff7103c0e098)

---

## 8️⃣ Contributing

If you'd like to collaborate, feel free to fork the repo, make changes, and submit a pull request. For ideas, check the **Features In Progress** section above!
