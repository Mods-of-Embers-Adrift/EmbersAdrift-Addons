using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class EffectPanelRowFixMod : MelonMod
{
    private const int MaxIconsPerRow = 10;
    private List<MonoBehaviour> cachedPanels = new List<MonoBehaviour>();
    private Dictionary<MonoBehaviour, int> lastIconCounts = new Dictionary<MonoBehaviour, int>();

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        // Find and cache the EffectIconPanelUI objects when a scene loads
        cachedPanels = GameObject.FindObjectsOfType<MonoBehaviour>()
            .Where(mb => mb.GetType().Name == "EffectIconPanelUI")
            .ToList();
        lastIconCounts.Clear();
    }

    public override void OnUpdate()
    {
        foreach (var panel in cachedPanels)
        {
            var grid = panel.GetComponent<GridLayoutGroup>();
            if (grid == null) continue;

            int iconCount = panel.GetComponentsInChildren<UnityEngine.UI.Image>(true)
                .Count(img => img.gameObject.activeSelf && img.transform.parent == panel.transform);

            if (!lastIconCounts.TryGetValue(panel, out int lastCount) || lastCount != iconCount)
            {
                int rowCount = (iconCount > MaxIconsPerRow) ? 2 : 1;
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grid.constraintCount = rowCount;
                lastIconCounts[panel] = iconCount;
            }
        }
    }
}