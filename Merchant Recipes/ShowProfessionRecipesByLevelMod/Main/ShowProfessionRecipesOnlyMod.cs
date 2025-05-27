using MelonLoader;
using HarmonyLib;
using System.Collections.Generic;
using SoL.UI;
using SoL.Game.UI.Merchants;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Objects;
using SoL.Game;
using SoL;
using System.Linq;

public class ShowProfessionRecipesByLevelMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        var harmony = new HarmonyLib.Harmony("ShowProfessionRecipesByLevelMod");
        harmony.Patch(
            AccessTools.Method(typeof(MerchantForSaleList), nameof(MerchantForSaleList.UpdateItems)),
            prefix: new HarmonyMethod(typeof(ShowProfessionRecipesByLevelMod), nameof(UpdateItems_Prefix))
        );
    }

    public static bool UpdateItems_Prefix(MerchantForSaleList __instance, UniqueId[] forSaleItemIds, string nameFilter)
    {
        MelonLogger.Msg("[ProfessionRecipeLevel] UpdateItems called!");

        var windowField = AccessTools.Field(typeof(MerchantForSaleList), "m_window");
        var window = windowField.GetValue(__instance) as UIWindow;
        if (window == null || !window.Visible || forSaleItemIds == null)
        {
            MelonLogger.Msg("[ProfessionRecipeLevel] Window not visible or forSaleItemIds null.");
            return false;
        }

        var saleItemsField = AccessTools.Field(typeof(MerchantForSaleList), "m_saleItems");
        var saleItems = saleItemsField.GetValue(__instance) as List<IMerchantInventory>;
        if (saleItems == null)
        {
            saleItems = new List<IMerchantInventory>(forSaleItemIds.Length);
            saleItemsField.SetValue(__instance, saleItems);
        }
        else
        {
            saleItems.Clear();
        }

        bool noFilter = string.IsNullOrEmpty(nameFilter);

        // Get the player's masteries (professions) and their associated ability levels
        var masteries = LocalPlayer.GameEntity?.CollectionController?.Masteries;
        var abilities = LocalPlayer.GameEntity?.CollectionController?.Abilities;
        var masteryIds = new HashSet<UniqueId>();
        var abilityLevels = new Dictionary<UniqueId, int>();
        if (masteries != null && abilities != null)
        {
            for (int i = 0; i < masteries.Count; i++)
            {
                var masteryInstance = masteries.GetIndex(i);
                if (masteryInstance?.Archetype != null)
                    masteryIds.Add(masteryInstance.Archetype.Id);
            }
            for (int i = 0; i < abilities.Count; i++)
            {
                var abilityInstance = abilities.GetIndex(i);
                if (abilityInstance?.Archetype != null)
                    abilityLevels[abilityInstance.Archetype.Id] = abilityInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity);
            }
        }

        foreach (var id in forSaleItemIds)
        {
            if (InternalGameDatabase.Archetypes.TryGetAsType<IMerchantInventory>(id, out var merchantInventory)
                && (noFilter || merchantInventory.Archetype.MatchesTextFilter(nameFilter)))
            {
                // Only filter recipe items
                if (merchantInventory.Archetype is RecipeItem recipeItem && recipeItem.Recipe != null)
                {
                    var recipe = recipeItem.Recipe;
                    var recipeMastery = recipe.Mastery;
                    var recipeAbility = recipe.Ability;
                    bool show = false;
                    string reason = "";

                    if (recipeMastery != null && recipeAbility != null)
                    {
                        // Must match profession
                        if (masteryIds.Contains(recipeMastery.Id))
                        {
                            // Must have enough crafting level
                            int playerLevel = 0;
                            abilityLevels.TryGetValue(recipeAbility.Id, out playerLevel);
                            if (playerLevel >= recipe.MinimumAbilityLevel)
                            {
                                show = true;
                            }
                            else
                            {
                                reason = $"Level too low (you: {playerLevel}, required: {recipe.MinimumAbilityLevel})";
                            }
                        }
                        else
                        {
                            reason = $"Wrong profession (recipe: {recipeMastery.DisplayName})";
                        }
                    }
                    else
                    {
                        reason = "Recipe missing mastery or ability";
                    }

                    if (show)
                    {
                        saleItems.Add(merchantInventory);
                        MelonLogger.Msg($"[ProfessionRecipeLevel] Added: {recipeItem.DisplayName} (Profession: {recipeMastery.DisplayName}, LevelReq: {recipe.MinimumAbilityLevel})");
                    }
                    else
                    {
                        MelonLogger.Msg($"[ProfessionRecipeLevel] Skipped: {recipeItem.DisplayName} ({reason})");
                    }
                    continue;
                }
                // Non-recipe items are always shown
                saleItems.Add(merchantInventory);
            }
        }

        AccessTools.Method(typeof(MerchantForSaleList), "ResetItems")
            .Invoke(__instance, new object[] { saleItems.Count, false, false });

        MelonLogger.Msg($"[ProfessionRecipeLevel] Final sale list count: {saleItems.Count}");
        return false;
    }
}