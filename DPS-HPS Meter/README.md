# 🔥 DPSAddon v1.1 for Embers Adrift (MelonLoader)

Welcome to **DPSAddon**, a fully MelonLoader-based DPS/HPS tracking addon for **Embers Adrift**.  
This addon gives you a real-time overview of combat performance in a fully customizable and non-intrusive UI.

---

## 📦 Features in progress

- Tracks DPS and HPS per player with total and average stats  
- Color-coded performance bars with dynamic gradients  
- Search and filter entries by name  
- Sort entries by name or stat (DPS/HPS)  
- Pin entries to keep top performers highlighted  
- Right-click context menu with copy, pin, and reset options  
- Persistent window state using PlayerPrefs (position, visibility)  
- Draggable and resizable UI panel with edge-boundary prevention  
- Insert key toggles visibility and resets position on long press  
- Optional side button toggle when main UI is closed  

---

## 🆕 Changelog

### Version 1.0.1

- Fixed combat text parsing to handle formatted messages like `<color="red"><i>Your</i></color> Hidden Strike HITS`  
- Successfully parsing and recording damage numbers (confirmed with logs)  
- Added handling for all attack types including Auto Attack, Hidden Strike, and Offhand attacks  
- Confirmed full data flow from Combat Text → Parser → DPSTracker → UI  
- Added scene persistence and prevented DPSTracker instance from being destroyed during transitions  
- Implemented static GameObject tracking and improved instance recovery after scene loads  
- Improved debug output and internal logging for damage events  
- Fixed tracker initialization logic and ensured proper instance tracking  
- Intercepted raw combat messages and applied parsing for damage values  
- Implemented basic UI framework and ensured core functionality across scenes   

### Version 1.0

- Converted project to full MelonLoader support (no assembly patching required)  
- Implemented CombatTextManager hook via Harmony  
- Rebuilt the IMGUI interface for modern feel and control  
- Added Search bar, Sort toggles, and CheckBoxes  
- Added Window state persistence and restore defaults  
- Right-click menu: Copy Stats / Pin Entry / Reset Entry  
- UI dynamically scales and respects screen boundaries  
- Insert key hotkey: toggle UI visibility & reset layout  
- Polished color system for performance bars  

---

## ❗ Known Issues

None at this time.

---

## 🔍 Troubleshooting

If you don’t see the UI:  
- Press `WIP` to bring it up or reset layout  
- Ensure MelonLoader is installed correctly  
- Check for conflicting mods or hook issues  

---

### 📘 License

Open-source and free to modify or improve. Contributions welcome!
