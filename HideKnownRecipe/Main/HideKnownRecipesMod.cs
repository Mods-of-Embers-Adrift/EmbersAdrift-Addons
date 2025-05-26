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

[assembly: MelonInfo(typeof(HideKnownRecipesMod), "Hide Known Recipes", "1.0.0", "MrJambix")]

public class HideKnownRecipesMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        var harmony = new HarmonyLib.Harmony("HideKnownRecipesMod");
        harmony.Patch(
            AccessTools.Method(typeof(MerchantForSaleList), nameof(MerchantForSaleList.UpdateItems)),
            prefix: new HarmonyMethod(typeof(HideKnownRecipesMod), nameof(UpdateItems_Prefix))
        );
    }

    public static bool UpdateItems_Prefix(MerchantForSaleList __instance, UniqueId[] forSaleItemIds, string nameFilter)
    {
        MelonLogger.Msg("[MerchantUI] UpdateItems called!");

        var windowField = AccessTools.Field(typeof(MerchantForSaleList), "m_window");
        var window = windowField.GetValue(__instance) as UIWindow;
        if (window == null || !window.Visible || forSaleItemIds == null)
        {
            MelonLogger.Msg("[MerchantUI] Window not visible or forSaleItemIds null.");
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

        var recipesContainer = LocalPlayer.GameEntity?.CollectionController?.Recipes;
        if (recipesContainer == null)
        {
            MelonLogger.Warning("[MerchantUI] Could not get player's known recipes container!");
        }
        else
        {
            var knownIds = new List<string>();
            foreach (var l in recipesContainer.Learnables)
                knownIds.Add($"{l.Id} ({l.DisplayName})");
            MelonLogger.Msg($"[MerchantUI] Player known recipes: {string.Join(", ", knownIds)}");
        }

        foreach (var id in forSaleItemIds)
        {
            if (InternalGameDatabase.Archetypes.TryGetAsType<IMerchantInventory>(id, out var merchantInventory)
                && (noFilter || merchantInventory.Archetype.MatchesTextFilter(nameFilter)))
            {
                if (merchantInventory.Archetype is RecipeItem recipeItem && recipeItem.Recipe != null)
                {
                    bool known = IsRecipeAlreadyKnown(recipeItem.Recipe);
                    MelonLogger.Msg($"[MerchantUI] RecipeItem: {recipeItem.DisplayName} - Underlying Recipe: {recipeItem.Recipe.DisplayName} ({recipeItem.Recipe.Id}) Known: {known}");
                    if (known)
                    {
                        MelonLogger.Msg($"[MerchantUI] -> SKIP (already known)");
                        continue;
                    }
                }
                saleItems.Add(merchantInventory);
                MelonLogger.Msg($"[MerchantUI] Added: {merchantInventory.Archetype.DisplayName}");
            }
        }

        AccessTools.Method(typeof(MerchantForSaleList), "ResetItems")
            .Invoke(__instance, new object[] { saleItems.Count, false, false });

        MelonLogger.Msg($"[MerchantUI] Final sale list count: {saleItems.Count}");
        return false;
    }

    static bool IsRecipeAlreadyKnown(Recipe recipe)
    {
        try
        {
            var gameEntity = LocalPlayer.GameEntity;
            var collection = gameEntity?.CollectionController;
            var recipesContainer = collection?.Recipes;
            if (recipesContainer == null)
            {
                MelonLogger.Warning("[MerchantUI] Recipes container is null in IsRecipeAlreadyKnown!");
                return false;
            }

            LearnableArchetype learnable;
            bool result = recipesContainer.TryGetLearnableForId(recipe.Id, out learnable);
            MelonLogger.Msg($"[MerchantUI] TryGetLearnableForId({recipe.Id}) = {result}");
            return result;
        }
        catch (System.Exception ex)
        {
            MelonLogger.Error($"[MerchantUI] Exception in IsRecipeAlreadyKnown: {ex}");
            return false;
        }
    }
}