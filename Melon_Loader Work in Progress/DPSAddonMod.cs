using MelonLoader;
using UnityEngine;
using HarmonyLib;

[assembly: MelonInfo(typeof(DPSAddon.DpsAddonMod), "DPSAddon", "1.0.0", "MrJambix")]
[assembly: MelonGame("Stormhaven Studios", "Embers Adrift")]

namespace DPSAddon
{
    public class DpsAddonMod : MelonMod
    {
        private HarmonyLib.Harmony _harmony;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("[DPSAddon] Initialized");
            _harmony = new HarmonyLib.Harmony("DPSAddon.Patches");
            _harmony.PatchAll();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"[DPSAddon] Scene Loaded: {sceneName}");

            if (GameObject.Find("DpsCanvasUI") == null)
            {
                MelonLogger.Msg("[DPSAddon] Creating DpsCanvasUI GameObject");
                GameObject obj = new GameObject("DpsCanvasUI");
                GameObject.DontDestroyOnLoad(obj);
                obj.AddComponent<DpsCanvasUI>();
            }
        }
    }
}
