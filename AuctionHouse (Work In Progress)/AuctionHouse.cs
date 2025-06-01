using MelonLoader;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// Game namespaces
using SoL.Game;
using SoL.Game.AuctionHouse;

public class AuctionHouseCheckboxMod : MelonMod
{
    internal static MelonPreferences_Category prefsCategory;
    internal static MelonPreferences_Entry<bool> pinnedCheckboxValue;

    public override void OnInitializeMelon()
    {
        prefsCategory = MelonPreferences.CreateCategory("AuctionHouseCheckboxMod", "Auction House Mod Settings");
        pinnedCheckboxValue = prefsCategory.CreateEntry("PinnedCheckbox", false, "Pinned Checkbox State");
    }
}

[HarmonyPatch(typeof(AuctionHouseUI), "UIWindowOnShowCalled")]
public static class AuctionHouseUIPatch
{
    static void Postfix(AuctionHouseUI __instance)
    {
        var ahGO = __instance.gameObject;
        Transform filterPanel = null;

        // Try likely panel names; adjust if you find the exact filter panel
        foreach (var childName in new[] { "WindowContent", "TopPanelContent" })
        {
            var child = ahGO.transform.Find(childName);
            if (child != null && child.GetComponentsInChildren<Toggle>(true).Length > 0)
            {
                filterPanel = child;
                break;
            }
        }
        if (filterPanel == null)
        {
            MelonLogger.Warning("Auction House filter panel not found!");
            return;
        }

        // Find the "Can Use" toggle to clone (or another filter checkbox)
        var canUseToggle = filterPanel.GetComponentsInChildren<Toggle>(true)
            .FirstOrDefault(t =>
            {
                var label = t.GetComponentInChildren<TextMeshProUGUI>();
                return (t.gameObject.name.Contains("CanUse") ||
                        (label != null && label.text.Trim() == "Can Use"));
            });
        if (canUseToggle == null)
        {
            MelonLogger.Warning("Can Use toggle not found!");
            return;
        }

        // Prevent duplicate
        if (filterPanel.Find("PinnedToggle") != null)
            return;

        // Clone the toggle
        var newToggleObj = Object.Instantiate(canUseToggle.gameObject, canUseToggle.transform.parent);
        newToggleObj.name = "PinnedToggle";
        var newToggle = newToggleObj.GetComponent<Toggle>();
        var labelObj = newToggleObj.GetComponentInChildren<TextMeshProUGUI>();
        if (labelObj != null)
            labelObj.text = "Pinned";

        // Place right after "Can Use" (below in vertical layout)
        int canUseIndex = canUseToggle.transform.GetSiblingIndex();
        newToggleObj.transform.SetSiblingIndex(canUseIndex + 1);

        // (Optional) If visually overlapping, try to adjust position (for non-layout-group UIs)
        var rt = newToggleObj.GetComponent<RectTransform>();
        if (rt != null && canUseToggle.GetComponent<RectTransform>() != null)
        {
            // Only adjust if necessary. If a VerticalLayoutGroup is present, you can skip this.
            var parentLayout = canUseToggle.transform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
            if (parentLayout == null)
            {
                // Space below original
                rt.anchoredPosition = canUseToggle.GetComponent<RectTransform>().anchoredPosition -
                                     new Vector2(0, rt.sizeDelta.y + 5f);
            }
        }

        // Set persistent value
        newToggle.isOn = AuctionHouseCheckboxMod.pinnedCheckboxValue.Value;
        newToggle.onValueChanged.RemoveAllListeners();
        newToggle.onValueChanged.AddListener((value) =>
        {
            AuctionHouseCheckboxMod.pinnedCheckboxValue.Value = value;
            MelonLogger.Msg($"Pinned Checkbox toggled: {value}");
            // Add custom logic here (for filtering, etc.)
        });

        MelonLogger.Msg("Pinned Checkbox injected!");
    }
}