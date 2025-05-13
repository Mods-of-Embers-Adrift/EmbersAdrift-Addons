# EmbersAdrift Addons

This repository contains experimental modifications for *Embers Adrift* designed to enhance user experience, primarily through UI improvements and utility tools like DPS tracking.

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

---

## 🔧 Features In Progress

### ✅ DPS Tracker
- ~~Hook into `InitializeCombatText`~~ (✔️ Done)
- Prevent DPS updates when out of combat stance
- Color-coded breakdown:
  - Player vs. Party Members vs. Enemies
  - Optional: Split mobs into a lower section
- Improve UI appearance and readability

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

- **Resizable Window**: Allow resizing via a drag handle.
- **Move to Unity UI**: Replace `OnGUI()` with `UnityEngine.UI` Canvas for modern styling and better interaction.
- **Custom Colors**: Team/class-based color options or DPS gradients.
- **Font Tuning**: Customizable font sizes, bold headers, user-defined styles.
- **Column Layout**:
  - DPS  
  - Total Damage  
  - Time Active  
- **Alternating Row Colors**: Enhance readability.

---

## 🔢 Sorting & Filtering

- Sort by DPS (ascending/descending)
- Filter by Player Name
- Toggle: Show Self Only / Group Only
- Minimum DPS Threshold: Auto-hide entries below set value

---

## ⏱️ Time Management

- Time Elapsed Display
- DPS Interval Selector:
  - Last 5 / 10 / 30 seconds
  - Full fight
- Combat State Awareness:
  - Track only while in combat

---

## 🕹️ Controls & Interactions

- Pause/Unpause DPS tracking
- Reset Button (already implemented)
- Force Refresh Button
- Lock Window Position
- Click-to-Focus on Player (future: inspect, target)
- Tooltips: Hover for total damage and fight duration
---

## VERSION 1.1
![V1 1 Embers](https://github.com/user-attachments/assets/bd574788-5746-40d4-90c4-145d5307f23e)

## VERSION 1.0
![EarlyStagesDPS](https://github.com/user-attachments/assets/3a5b2534-0929-4d4d-a6de-efbcbf65031d)



##
