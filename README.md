# 🌋 Embers Adrift Addons

This repository hosts **experimental UI and utility modifications** for *Embers Adrift* aimed at enhancing player experience. These tools help identify gameplay issues—such as class imbalances—and improve quality of life without fostering toxicity.

> Our mission: Help players **engage deeper** with the game through visibility, utility, and transparency.

---

## 🔄 Transition Notice: Moving to MelonLoader & Lua

We are looking into possibly **migrating from direct DLL replacement** to a **MelonLoader-based mod** architecture.

- ✅ **MelonLoader Integration**: Full support for modular mod loading without needing to replace `Assembly-CSharp.dll`
- ✅ **Lua Scripting Support** (via [LuaLoader](https://github.com/NeptuneQ/LuaLoader)): Allow players to script UI behavior and extensions using Lua without needing C# knowledge
- 🔧 We’ll continue updating core logic (DPS, threat, UI) in C# while exposing functionality to Lua
- 🧩 Future releases will include:
  - A Lua scripting API for custom behaviors
  - Plug-and-play `.lua` scripts
  - Backward compatibility with saved layouts and logs

> 🧠 This shift makes the addon more accessible, safer to use, and easier to extend by the community.

---

## 📝 Changelogs

## 📌 Version 1.4 – Threat & DPS Enhancements
# ✅ Changes & Additions
- Added `UserThreatTracker` and `UserDpsTracker` for tracking threat, DPS, and HPS.
- Integrated combat event hooks via `CombatTextManager` to log player damage and healing.
- Introduced UI toggle between DPS and HPS views, including per-player stat bars.
- Redesigned DPS window layout for better readability; buttons now aligned in a single top row.
- Threat Meter now filters out NPCs and non-player names for cleaner output.
- Swing Timer now auto-hides after combat ends.
# 🔧 Fixes & Tweaks
- Resized DPS and Threat UI windows for more compact display.
- Prevented combat events from affecting threat logs if source is invalid or NPC.
- Ensured only valid player data is tracked and shown.
# 🔜 Known Issues & Next Steps
- **Threat Meter:** Needs to account for class-based multipliers (e.g. tanks generate more threat, healers generate less).
- **Skill Breakdown:** Add ability to display which skills contributed to damage/healing, including per-crit breakdowns.
---

### 📌 Version 1.3
- ✅ **Integrated Swing Timer**  
  - Displays a real-time swing bar animation with timing.
- 🔄 **Reworked UI Filters**  
  - Temporarily removed the **Player Filter** (under revision).
- 🧪 **In Progress Features (partially implemented):**
  - Enemy DPS tab structure started (visual placeholder only).
  - HPS tab layout planned but not yet populated.
  - Removed Threat Meter logic under initial design/testing phase.
- 🧼 UI Polish & Performance:
  - Optimized memory use and draw calls.
  - Visual cleanup across draggable elements.
    
  ![V1 3](https://github.com/user-attachments/assets/0761bcec-2200-4847-a864-f02a81a8422b)

---

### 📌 Version 1.2
- ✅ Fight segmentation by combat inactivity
- ✅ Popup viewer for past fights
- ✅ Locked post-combat duration
- ✅ Draggable/resizable window support
- ✅ Scrollable list layout  
![V1.2](https://github.com/user-attachments/assets/0617f260-43e0-4b77-ac42-4976c24c1053)

---

### 📌 Version 1.1
- Basic DPS chart implementation
- Cleaned up OnGUI clutter  
![V1.1](https://github.com/user-attachments/assets/bd574788-5746-40d4-90c4-145d5307f23e)

---

### 📌 Version 1.0
- First working prototype
- Simple overlay with DPS totals  
![EarlyStagesDPS](https://github.com/user-attachments/assets/3a5b2534-0929-4d4d-a6de-efbcbf65031d)

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

## 2️⃣ In-Game Usage

- Press **F2** to toggle visibility of the custom overlay UI.

---

## 3️⃣ Features & Modules

### 3.1 ✅ DPS Tracker

- ✔️ Hooks into `InitializeCombatText`
- ✔️ Combat-aware (ends after 8s of no action)
- ✔️ Fight history viewer w/ session popup
- ✔️ Automatic session segmentation
- ✔️ Fixed duration post-combat
- ✔️ Resizable and movable windows
- ✔️ Reset button for clearing data

#### 🧪 Coming Soon:
- Color-coded entries (e.g., Player/Group/Enemy)
- Sorting toggle (ascending/descending)
- Self/group filter toggles
- Hover tooltips with advanced stats

---

### 3.2 🗡️ Attack Swing Tracker (WIP)

- Visual representation of attack swing timers  
- Supports rhythmic/melee-heavy classes

---

### 3.3 📊 Stat Display (Planned)

- Surface hidden calculations like crit chance, haste multipliers, etc.

---

### 3.4 🔄 Positional Awareness (Planned)

> “Yell at me if I'm not in the right spot!”

- Pop-up or alert when positional bonus is missed

---

### 3.5 🛒 Merchant Enhancements (Planned)

- Compare item prices vs. inventory value
- Flag expensive/unfavorable items

---

## 4️⃣ UI & Visual Improvements

### 4.1 Layout

- ✔️ Resizable and movable UI
- Canvas-based (moving away from `OnGUI`)
- Font size and style customization
- Alternating row colors for readability

### 4.2 Column Support

- DPS  
- Total Damage  
- Time Active  

---

## 5️⃣ Sorting & Filtering

- ✔️ Filter by player name
- DPS sorting (ASC/DESC)
- Self-only / Group-only toggle
- Minimum DPS threshold setting

---

## 6️⃣ Time Management Features

- Time Elapsed Display
- Interval selectors:
  - Last 5, 10, 30 seconds
  - Full fight
- ✔️ Combat-state detection built-in

---

## 7️⃣ UI Controls & Interaction

- ✔️ Reset session/fight logs
- Pause/unpause DPS capture
- Force refresh button (planned)
- Lock UI window position
- Tooltips with damage and duration details
- Click-to-focus on player (planned)

---

## 8️⃣ Contributing

If you'd like to collaborate, feel free to fork the repo, make changes, and submit a pull request. For ideas, check the **Features In Progress** section above!
