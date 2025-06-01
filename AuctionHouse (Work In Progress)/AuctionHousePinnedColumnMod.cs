using MelonLoader;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Reflection;
using SoL.Game.AuctionHouse;

public class AuctionHousePinnedColumnMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("AuctionHousePinnedColumnMod loaded!");
    }
}

[HarmonyPatch(typeof(AuctionHouseUI), "UIWindowOnShowCalled")]
public static class AuctionHouseUI_HeaderPinPatch
{
    static void Postfix(AuctionHouseUI __instance)
    {
        var ahGO = __instance.gameObject;
        var headerRow = ahGO.transform.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(t => t.name.ToLower().Contains("header") && t.GetComponentsInChildren<TextMeshProUGUI>(true).Any(x => x.text.Trim() == "Seller"));
        if (headerRow == null)
        {
            MelonLogger.Warning("Auction House header row not found!");
            return;
        }

        var headerTexts = headerRow.GetComponentsInChildren<TextMeshProUGUI>(true);
        var sellerHeader = headerTexts.FirstOrDefault(x => x.text.Trim() == "Seller");
        var costHeader = headerTexts.FirstOrDefault(x => x.text.Trim() == "Cost");

        if (sellerHeader == null || costHeader == null)
        {
            MelonLogger.Warning("Seller or Cost header not found!");
            return;
        }

        if (headerTexts.Any(x => x.text.Trim() == "Pinned"))
            return;

        var pinnedHeaderObj = UnityEngine.Object.Instantiate(sellerHeader.gameObject, sellerHeader.transform.parent);
        pinnedHeaderObj.name = "PinnedHeader";
        var pinnedHeaderText = pinnedHeaderObj.GetComponent<TextMeshProUGUI>();
        pinnedHeaderText.text = "Pinned";
        int sellerIndex = sellerHeader.transform.GetSiblingIndex();
        pinnedHeaderObj.transform.SetSiblingIndex(sellerIndex + 1);

        MelonLogger.Msg("Pinned column header injected!");
    }
}

[HarmonyPatch(typeof(AuctionHouseForSaleListItem), "InitItem")]
public static class AuctionHouseUI_RowPinPatch
{
    static void Postfix(AuctionHouseForSaleListItem __instance)
    {
        // Use reflection to get the private m_auction field
        var auctionField = typeof(AuctionHouseForSaleListItem).GetField("m_auction", BindingFlags.Instance | BindingFlags.NonPublic);
        var auction = auctionField?.GetValue(__instance);
        string sellerName = null;
        if (auction != null)
        {
            var sellerNameProp = auction.GetType().GetProperty("SellerName", BindingFlags.Public | BindingFlags.Instance);
            sellerName = sellerNameProp?.GetValue(auction) as string;
        }

        var sellerLabel = __instance.transform.GetComponentsInChildren<TextMeshProUGUI>(true)
            .FirstOrDefault(x => x.text == sellerName);

        if (sellerLabel == null)
        {
            sellerLabel = __instance.transform.GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(x => x.text.Trim().Length > 0);
        }
        if (sellerLabel == null)
        {
            MelonLogger.Warning("Seller label not found in row!");
            return;
        }

        var existing = __instance.transform.Find("PinnedCell");
        if (existing != null) return;

        var pinnedCellObj = UnityEngine.Object.Instantiate(sellerLabel.gameObject, sellerLabel.transform.parent);
        pinnedCellObj.name = "PinnedCell";
        var pinnedCellText = pinnedCellObj.GetComponent<TextMeshProUGUI>();
        pinnedCellText.text = ""; // No text, just a checkbox

        int sellerIndex = sellerLabel.transform.GetSiblingIndex();
        pinnedCellObj.transform.SetSiblingIndex(sellerIndex + 1);

        var oldToggle = pinnedCellObj.GetComponentInChildren<Toggle>(true);
        if (oldToggle != null)
        {
            GameObject.DestroyImmediate(oldToggle.gameObject);
        }
        var checkboxGO = new GameObject("PinnedCheckbox", typeof(RectTransform), typeof(Toggle), typeof(Image));
        var checkboxRT = checkboxGO.GetComponent<RectTransform>();
        checkboxGO.transform.SetParent(pinnedCellObj.transform, false);
        checkboxRT.anchorMin = new Vector2(0.5f, 0.5f);
        checkboxRT.anchorMax = new Vector2(0.5f, 0.5f);
        checkboxRT.sizeDelta = new Vector2(18, 18);

        var img = checkboxGO.GetComponent<Image>();
        img.color = Color.black;
        var toggle = checkboxGO.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle.onValueChanged.AddListener((val) =>
        {
            MelonLogger.Msg($"Pinned checkbox for {sellerName ?? __instance.name} set to: {val}");
        });

        MelonLogger.Msg("Pinned checkbox injected for row: " + (sellerName ?? __instance.name));
    }
}