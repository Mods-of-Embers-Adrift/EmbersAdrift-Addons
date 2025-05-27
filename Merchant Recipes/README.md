# Recipe Vendor Filtering Mods for Embers Adrift

This repo contains three separate MelonLoader mods for filtering recipes in the merchant (vendor) UI of Embers Adrift:

---

## 1. HideKnownRecipesMod

- **Purpose:** Hides recipes in vendor shops that your character already knows.
- **Effect:** You will only see recipes you have not learned yet when browsing vendors.

---

## 2. ShowProfessionRecipesByLevelMod

- **Purpose:** Only shows recipes for sale that match your character’s current profession (Mastery) and crafting level.
- **Effect:** You will only see recipes in vendor shops that you are eligible to learn and use, based on your profession and crafting level. Recipes for other professions or higher levels are hidden.

---

## 3. ShowProfessionRecipesByLevelAndHideKnownMod (Combined Mod)

- **Purpose:** Combines the above two features.
- **Effect:** Vendors will only display recipes that:
    - Match your character’s current profession (Mastery)
    - Are at or below your current crafting level for that profession
    - Are not already known by your character

---

## Disclaimer & Usage

> - If you want both effects (profession/level filtering and hiding known recipes), only use the combined mod:  
>   **ShowProfessionRecipesByLevelAndHideKnownMod**
>
> **Do NOT run `HideKnownRecipesMod` and `ShowProfessionRecipesByLevelMod` at the same time!**
>
> Due to the way Harmony patches work, running both at once will cause them to overwrite each other and only one mod's filtering will be applied.
>
> - If you only want one specific effect, use just that individual mod.

---

## Credits

- Mods by MrJambix
