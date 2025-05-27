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

public class ShowProfessionRecipesByLevelAndHideKnownMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        var harmony = new HarmonyLib.Harmony("ShowProfessionRecipesByLevelAndHideKnownMod");
        harmony.Patch(
            AccessTools.Method(typeof(MerchantForSaleList), nameof(MerchantForSaleList.UpdateItems)),
            prefix: new HarmonyMethod(typeof(ShowProfessionRecipesByLevelAndHideKnownMod), nameof(UpdateItems_Prefix))
        );
    }

    public static bool UpdateItems_Prefix(MerchantForSaleList __instance, UniqueId[] forSaleItemIds, string nameFilter)
    {
        MelonLogger.Msg("[ProfRecipesLevelHideKnown] UpdateItems called!");

        var windowField = AccessTools.Field(typeof(MerchantForSaleList), "m_window");
        var window = windowField.GetValue(__instance) as UIWindow;
        if (window == null || !window.Visible || forSaleItemIds == null)
        {
            MelonLogger.Msg("[ProfRecipesLevelHideKnown] Window not visible or forSaleItemIds null.");
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

        // Get the player's masteries, abilities, and known recipes
        var entity = LocalPlayer.GameEntity;
        var masteries = entity?.CollectionController?.Masteries;
        var abilities = entity?.CollectionController?.Abilities;
        var recipesContainer = entity?.CollectionController?.Recipes;
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
                    abilityLevels[abilityInstance.Archetype.Id] = abilityInstance.GetAssociatedLevelInteger(entity);
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

                            // Must not already know the recipe
                            bool alreadyKnown = false;
                            if (recipesContainer != null)
                            {
                                LearnableArchetype learnable;
                                alreadyKnown = recipesContainer.TryGetLearnableForId(recipe.Id, out learnable);
                            }

                            if (alreadyKnown)
                            {
                                reason = "Already known";
                            }
                            else if (playerLevel >= recipe.MinimumAbilityLevel)
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
                        MelonLogger.Msg($"[ProfRecipesLevelHideKnown] Added: {recipeItem.DisplayName} (Profession: {recipeMastery.DisplayName}, LevelReq: {recipe.MinimumAbilityLevel})");
                    }
                    else
                    {
                        MelonLogger.Msg($"[ProfRecipesLevelHideKnown] Skipped: {recipeItem.DisplayName} ({reason})");
                    }
                    continue;
                }
                // Non-recipe items are always shown
                saleItems.Add(merchantInventory);
            }
        }

        AccessTools.Method(typeof(MerchantForSaleList), "ResetItems")
            .Invoke(__instance, new object[] { saleItems.Count, false, false });

        MelonLogger.Msg($"[ProfRecipesLevelHideKnown] Final sale list count: {saleItems.Count}");
        return false;
    }
}