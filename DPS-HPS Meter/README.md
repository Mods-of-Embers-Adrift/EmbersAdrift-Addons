# 🔥 DPSAddon for Embers Adrift (MelonLoader)

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

## Version 1.0.3
### Major Updates:

- Fixed HPS tracking for self-healing now works even when chat messages include Unity rich text tags (color, italics, etc.).
- Improved Combat message parsing is more robust and reliable for all healing events.
- Refactored Internal healing recording logic is now minimal and consistent with damage logic.
- Added Utility function to strip Unity rich text tags from combat log messages.
- Improved logging for easier debugging and traceability.
- Removed multi-tracking system with checkboxes until further notice or better integration.

---

## Version 1.0.2

### Major Updates:
### Combat Text & Data:
- Fixed combat text parsing for damage messages  
- Implemented proper damage number extraction  
- Added player name resolution  
- Fixed data flow from combat text to tracker  
- Confirmed working DPS tracking system  

### UI System Overhaul:
- Implemented multi-tracking system with checkboxes  
- Added window position saving  
- Added window resizing functionality  
- Added minimized mode with top-left button  
- Added proper window state management  
- Improved UI visibility controls  

### Keybind Implementation:
- Changed toggle key from F2 to Insert  
- Added Shift+Insert for position reset  
- Added keybind information to Settings tab  
- Improved keybind responsiveness  
- Added proper keybind state handling  

### Window Management:
- Added window position saving/loading  
- Added window size persistence  
- Added default position reset  
- Added proper window dragging  
- Added resize handle  
- Added multi-window support  
- Added window state persistence  

### Quality of Life:
- Added minimize button functionality  
- Added proper window restoration  
- Added settings tab with controls  
- Added multi-tracking toggles  
- Added window position reset  
- Added proper cleanup handling  
- Improved error handling  

### Visual Improvements:
- Added proper styling system  
- Improved text readability  
- Added consistent UI theme  
- Added proper spacing  
- Added proper alignment  
- Added proper scrolling  
- Added proper headers  

### Bug Fixes:
- Fixed instance persistence issues  
- Fixed window state management  
- Fixed position saving issues  
- Fixed multi-tracking bugs  
- Fixed style initialization  
- Fixed window cleanup  
- Fixed data persistence  

### Performance:
- Improved window handling  
- Better state management  
- Improved data flow  
- Better cleanup handling  
- Optimized UI updates  
- Better memory management  

---

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

---

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
