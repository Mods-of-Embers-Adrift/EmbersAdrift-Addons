using MelonLoader;
using HarmonyLib;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using System.Reflection;

[assembly: MelonInfo(typeof(ShowVendorItemPrice.ShowVendorItemPriceMod), "ShowVendorItemPrice", "1.0.0", "MrJambix")]
[assembly: MelonGame("Stormhaven Studios", "Embers Adrift")]

namespace ShowVendorItemPrice
{
    public class ShowVendorItemPriceMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("ShowVendorItemPriceMod loaded!");
        }

        [HarmonyPatch(typeof(ArchetypeTooltip), "SetArchtypeData")]
        class Patch_ArchetypeTooltip_SetArchtypeData
        {
            static void Postfix(ArchetypeTooltip __instance, ITooltipParameter param)
            {
                if (param is ArchetypeTooltipParameter archetypeTooltipParameter)
                {
                    var instance = archetypeTooltipParameter.Instance;
                    var baseArchetype = instance == null ? archetypeTooltipParameter.Archetype : instance.Archetype;
                    if (baseArchetype is ItemArchetype itemArchetype && instance != null)
                    {
                        if (itemArchetype.TryGetSalePrice(instance, out ulong value) && value > 0)
                        {
                            // Use reflection to access private fields
                            var currencyBlockLabel = Traverse.Create(__instance).Field("m_currencyBlockLabel").GetValue<TMPro.TextMeshProUGUI>();
                            var currencyBlock = Traverse.Create(__instance).Field("m_currencyBlock").GetValue<object>();
                            var currencyBlockParent = Traverse.Create(__instance).Field("m_currencyBlockParent").GetValue<UnityEngine.GameObject>();

                            // Set text
                            currencyBlockLabel?.SetText("Sale Price:");

                            // Call UpdateCoin on m_currencyBlock using reflection
                            var updateCoinMethod = currencyBlock?.GetType().GetMethod("UpdateCoin");
                            updateCoinMethod?.Invoke(currencyBlock, new object[] { value });

                            // Set active
                            currencyBlockParent?.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}