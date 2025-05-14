# EmbersAdrift Addons
This repository contains experimental modifications for *Embers Adrift* designed to enhance user experience, primarily through UI improvements and utility tools like DPS tracking.

In no way shape or form is this addon/mod supproted by the game developers. Use caution when using it and do not use it to destroy other players motivation, or idea of gaming. By this I mean do not shame other players or talk shit to them for there performance metrics.
---
## 📂 Installation Instructions
**Replace the Assembly-CSharp.dll:**
1. Navigate to:
   ```
   SteamLibrary\steamapps\common\Embers Adrift\Embers Adrift_Data\Managed
   ```
2. **Backup your existing `Assembly-CSharp.dll`** by copying it to a safe location.  
3. Replace the original file with the modified version provided in this repository.

> ⚠️ Always back up the original file in case you want to revert changes.
---
## 🕹️ In-Game Usage

- Press **F2** to toggle the UI visibility.
- Press **F3** to collapse/expand the DPS view.
- Press **F4** to clear the player name filter.

---

## 🔧 Features In Progress

### ✅ DPS Tracker
# ✔️ **Hook into `InitializeCombatText`**
# ✔️ **Combat State Awareness**  
- DPS tracking stops after 8 seconds of inactivity.
# ✔️ **Fight History Viewer**  
- Displays a list of previous combat encounters.
- Click on any session to open a detailed popup.
# ✔️ **Auto-Fight Segmentation**  
- Automatically starts a new session when new damage is detected after timeout.
# ✔️ **Fixed Fight Duration**  
- Duration is locked in once combat ends.
# ✔️ **Resizable UI Windows**  
- Main window and popups are freely movable and scalable.
# ✔️ **Reset Button**  
- Clears current and historical data.

# 🛠️ Coming Soon:
- Color-coded roles (Player/Group/Enemy)
- Sorting: DPS ascending/descending toggle
- Filter toggles (Show self/group only)
- Tooltip stats on hover

---

### 🗡️ Attack Swing Tracker / Timer
- Implement visual timer for attack swings

### 📊 Stat Display
- Determine and display underlying stat formulas and calculations

### 🔄 Positional Awareness
- Notifications or cues when positional bonuses apply  
> "Yell at me if I'm not in the right spot!"

### 🛒 Merchant Enhancements
- Show item prices relative to your inventory and compare

---

## 🎨 Visual/UI Improvements

- **Resizable Window**: Allow resizing via a drag handle. ✔️
- **Move to Unity UI**: Replace `OnGUI()` with `UnityEngine.UI` Canvas for modern styling and better interaction.
- **Custom Colors**: Team/class-based color options or DPS gradients.
- **Font Tuning**: Customizable font sizes, bold headers, user-defined styles.

# **Column Layout**:
- DPS  
- Total Damage  
- Time Active

# **Alternating Row Colors**: Enhance readability ✔️

---

## 🔢 Sorting & Filtering

- Sort by DPS (ascending/descending)
- Filter by Player Name ✔️
- Toggle: Show Self Only / Group Only
- Minimum DPS Threshold: Auto-hide entries below set value

---

## ⏱️ Time Management

- Time Elapsed Display
- DPS Interval Selector:
- Last 5 / 10 / 30 seconds
- Full fight
- Combat State Awareness ✔️

---

## 🕹️ Controls & Interactions

- Pause/Unpause DPS tracking
- Reset Button ✔️
- Force Refresh Button
- Lock Window Position
- Click-to-Focus on Player (future: inspect, target)
- Tooltips: Hover for total damage and fight duration

---

## VERSION 1.2
- ✅ Fight segmentation by combat inactivity
- ✅ Popup browser for previous encounters
- ✅ Accurate locked duration tracking
- ✅ UI drag-and-drop support
- ✅ Cleaner scrollable layout
  
![V1 2](https://github.com/user-attachments/assets/0617f260-43e0-4b77-ac42-4976c24c1053)

## VERSION 1.1
![V1 1 Embers](https://github.com/user-attachments/assets/bd574788-5746-40d4-90c4-145d5307f23e)

## VERSION 1.0
![EarlyStagesDPS](https://github.com/user-attachments/assets/3a5b2534-0929-4d4d-a6de-efbcbf65031d)



##
