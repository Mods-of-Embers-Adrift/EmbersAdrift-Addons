# 🔥 DPSAddon v1.0 for Embers Adrift (MelonLoader)

Welcome to **DPSAddon v1.0**, a fully MelonLoader-based DPS/HPS tracking addon for **Embers Adrift**.  
This addon gives you a real-time overview of combat performance in a fully customizable and non-intrusive UI.

---

## 📦 Features

- 🎯 **Tracks DPS and HPS** per player with total and average stats
- 🌈 **Color-coded performance bars** with dynamic gradients
- 🔍 **Search and filter** entries by name
- ⬆️ **Sort entries** by name or stat (DPS/HPS)
- 📌 **Pin entries** to keep top performers highlighted
- 🔁 **Right-click context menu** with copy, pin, and reset options
- 💾 **Persistent window state** using PlayerPrefs (position, visibility)
- 🪟 **Draggable and resizable** UI panel with edge-boundary prevention
- ⌨️ **Insert key toggles visibility** and resets position on long press
- 🧰 **Optional side button toggle** when main UI is closed

---

## 🆕 Changelog – Version 1.0

- ✅ Converted project to full **MelonLoader** support (no assembly patching required)
- ✅ Implemented **CombatTextManager hook** via Harmony
- ✅ Rebuilt the **IMGUI interface** for modern feel and control
- ✅ Added **Search bar**, **Sort toggles**, and **CheckBoxes**
- ✅ Added **Window state persistence** and **restore defaults**
- ✅ Right-click menu: Copy Stats / Pin Entry / Reset Entry
- ✅ UI dynamically scales and respects screen boundaries
- ✅ Insert key hotkey: toggle UI visibility & reset layout
- ✅ Polished color system for performance bars

---

## ❗ Known Issues

- ❌ **Data not feeding into the tracker** – The `CombatTextManager` hook is not currently passing valid data to the `DpsEventListener`, causing the UI to appear blank.  
  This is being investigated and will be resolved in the next release.

---

## 🔍 Troubleshooting

If you don’t see the UI:
- Press `Insert` to bring it up or reset layout
- Ensure MelonLoader is installed correctly
- Check for conflicting mods or hook issues

---

### 📘 License

Open-source and free to modify or improve. Contributions welcome!