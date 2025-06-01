# EffectPanelRowFixMod

Automatically fixes the persistent "2 rows" bug on the buff/status bar (EffectIconPanelUI).  
When buffs and debuffs are cleared, the bar now collapses back to a single row without needing to restart the client or press a reset button.
---

## Features

- Monitors all `EffectIconPanelUI` instances in the game.
- Dynamically sets the proper row count on the `GridLayoutGroup` so the bar always resizes as buffs/debuffs are added or removed.
- No code changes to the game or manual resets required.
---
## How It Works
- Each frame, the mod finds all `EffectIconPanelUI` scripts in the scene.
- It checks the number of active buff/debuff icons under each UI panel.
- The mod adjusts the panel's `GridLayoutGroup` constraint and count, so it always collapses to 1 row unless more are needed.

---

## Configuration

- By default, the mod assumes a maximum of 10 icons per row.
- To change this, edit the `MaxIconsPerRow` constant in the source code and rebuild.


---
## Credits

- Mod & fix by [MrJambix]

