# Reveal All Map Fog & POI
## Overview
**Reveal All Map Fog & POI** allows you to instantly uncover all areas of the map and reveal all Points of Interest (POIs) for every region.
> ⚠️ **This mod only affects your client**. It does not unlock discoveries or affect the server-side game state for your account.
---

## Features

- Instantly removes all "fog of war" overlays from every map area.
- Reveals all POIs (towns, dungeons, landmarks, etc.) for every map section. (W.I.P)
- One-key activation: Press **F10** in-game to activate.

---
## Usage
1. Enter the game and open the map UI.
2. **Press `F10`** to remove all fog and reveal all POIs for every map area currently loaded.
3. Check your map: all regions should be fully visible and all points of interest marked.

You can repeat this at any time; new map areas added to the UI will be revealed the next time you press **F10**.
---

## How It Works

- The mod searches the in-game UI hierarchy for every loaded map area.
- For each map, it destroys the `Fog` GameObject (removing the fog overlay) and activates the `POI` GameObject (revealing all points of interest).
- This is a client-side visual change only and does **not** affect your character's actual in-game discoveries or progression.

---

## Disclaimer

- Use at your own risk.
- This mod is provided as-is, with no guarantee of compatibility with future game updates.
- Not affiliated with Stormhaven Studios or the official Ember Adrift team.

---

## Contributing

Pull requests are welcome! If you find bugs or want to add features (like per-map toggles or hotkey customization), feel free to open an issue or PR.

- Mod by MrJambix

*Happy adventuring!*