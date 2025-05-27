using MelonLoader;
using UnityEngine;

namespace RevealAllMapFogAndPOI
{
    public class RevealAllMapFogAndPOI : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                MelonLogger.Msg("F10 pressed - revealing all map fog and showing all POIs...");
                RevealAllFogAndShowAllPOI();
            }
        }

        private void RevealAllFogAndShowAllPOI()
        {
            // Find the MapParent object in the scene hierarchy
            var mapParent = GameObject.Find("UIManager/RegularCanvas/GameUIPanel/MapUI_New(Clone)/WindowContent/MapParent");
            if (mapParent == null)
            {
                MelonLogger.Error("Could not find MapParent in the UI hierarchy.");
                return;
            }

            int fogRemoved = 0;
            int poiRevealed = 0;

            // Iterate over all children (maps: Map_<AreaName>(Clone))
            foreach (Transform mapArea in mapParent.transform)
            {
                // Remove/deactivate Fog GameObject if it exists
                var fog = mapArea.Find("Fog");
                if (fog != null && fog.gameObject.activeSelf)
                {
                    GameObject.Destroy(fog.gameObject); // Alternatively: fog.gameObject.SetActive(false);
                    fogRemoved++;
                    MelonLogger.Msg($"Removed fog for: {mapArea.name}");
                }

                // Reveal POI GameObject if it exists
                var poi = mapArea.Find("POI");
                if (poi != null && !poi.gameObject.activeSelf)
                {
                    poi.gameObject.SetActive(true);
                    poiRevealed++;
                    MelonLogger.Msg($"Revealed POI for: {mapArea.name}");
                }
            }

            MelonLogger.Msg($"Process complete! Fog removed for {fogRemoved} maps; POI revealed for {poiRevealed} maps.");
        }
    }
}