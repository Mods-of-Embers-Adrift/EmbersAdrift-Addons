using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EffectPanelRowFixMod : MelonMod
{
    private const int MaxIconsPerRow = 10; // Adjust to desired value for your UI

    public override void OnUpdate()
    {
        // Find all active EffectIconPanelUI objects in the scene
        var panels = GameObject.FindObjectsOfType<MonoBehaviour>()
            .Where(mb => mb.GetType().Name == "EffectIconPanelUI");

        foreach (var panel in panels)
        {
            var grid = ((MonoBehaviour)panel).GetComponent<GridLayoutGroup>();
            if (grid == null) continue;

            // Count only direct child icons of this panel that are active
            int iconCount = ((MonoBehaviour)panel).GetComponentsInChildren<UnityEngine.UI.Image>(true)
                .Count(img => img.gameObject.activeSelf && img.transform.parent == ((MonoBehaviour)panel).transform);

            // Always at least 1 row; collapse to 1 when under MaxIconsPerRow
            int rowCount = (iconCount > MaxIconsPerRow) ? 2 : 1;

            // Only update if values are not already correct
            if (grid.constraint != GridLayoutGroup.Constraint.FixedRowCount || grid.constraintCount != rowCount)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grid.constraintCount = rowCount;
            }
        }
    }
}