using HarmonyLib;
using SoL.Game.AuctionHouse;
using SoL.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

[HarmonyPatch]
public static class Patch_ContextMenuUI_Init
{
    static MethodBase TargetMethod()
    {
        return typeof(ContextMenuUI).GetMethod(
            "Init",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(List<ContextMenuAction>), typeof(string) },
            null
        );
    }

    static void Prefix(ContextMenuUI __instance, List<ContextMenuAction> actions, string title)
    {
        if (!string.IsNullOrEmpty(title) && title.Contains("Auction"))
        {
            actions.Add(new ContextMenuAction
            {
                Text = "Pin",
                Enabled = true,
                Callback = () => { /* No-op for now */ }
            });
            actions.Add(new ContextMenuAction
            {
                Text = "Unpin",
                Enabled = true,
                Callback = () => { /* No-op for now */ }
            });
        }
    }
}

[HarmonyPatch(typeof(AuctionHouseForSaleListItem), "InitItem")]
public static class Patch_AuctionHouseForSaleListItem_InitItem
{
    static void Postfix(AuctionHouseForSaleListItem __instance)
    {
        // No-op for now
    }
}