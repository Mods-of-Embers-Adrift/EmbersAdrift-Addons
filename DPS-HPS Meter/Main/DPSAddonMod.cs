using MelonLoader;
using UnityEngine;
using HarmonyLib;

[assembly: MelonInfo(typeof(DPSAddon.DpsAddonMod), "DPSAddon", "1.0.0", "MrJambix")]
[assembly: MelonGame("Stormhaven Studios", "Embers Adrift")]

namespace DPSAddon
{
    public class DpsAddonMod : MelonMod
    {
        public const string LOG_PREFIX = "[DPSAddon]";
        private HarmonyLib.Harmony _harmony;
        private GameObject _uiObject;
        private bool _initialized;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"{LOG_PREFIX} Initialization started");

            try
            {
                // Launch the DPS Tracker first
                MelonLogger.Msg($"{LOG_PREFIX} Launching DPSTracker...");
                DPSTracker.Launch();

                // Test the tracker
                MelonLogger.Msg($"{LOG_PREFIX} Testing tracker...");
                DPSTracker.TestRecord();

                // Create UI
                MelonLogger.Msg($"{LOG_PREFIX} Creating UI...");
                CreateUI();

                // Hook all Harmony patches
                _harmony = new HarmonyLib.Harmony("DPSAddon.Patches");
                _harmony.PatchAll();
                MelonLogger.Msg($"{LOG_PREFIX} Harmony patches applied");

                // Fallback: Try manual delegate injection
                MelonLogger.Msg($"{LOG_PREFIX} Injecting manual hook via ManualInjector...");
                ManualInjector.Inject();

                _initialized = true;
                MelonLogger.Msg($"{LOG_PREFIX} Initialization completed");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"{LOG_PREFIX} Initialization failed: {ex.Message}");
                MelonLogger.Error($"{LOG_PREFIX} Stack trace: {ex.StackTrace}");
            }
        }

        private void CreateUI()
        {
            try
            {
                if (_uiObject != null)
                {
                    MelonLogger.Msg($"{LOG_PREFIX} UI already exists");
                    return;
                }

                _uiObject = new GameObject("DPSCanvasUI");
                var uiComponent = _uiObject.AddComponent<DPSCanvasUI>();
                UnityEngine.Object.DontDestroyOnLoad(_uiObject);

                MelonLogger.Msg($"{LOG_PREFIX} UI created successfully");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"{LOG_PREFIX} Failed to create UI: {ex.Message}");
            }
        }

        public override void OnUpdate()
        {
            if (!_initialized) return;

            // Add a test key to verify the tracker is working
            if (Input.GetKeyDown(KeyCode.F5))
            {
                MelonLogger.Msg($"{LOG_PREFIX} Test key pressed");
                DPSTracker.TestRecord();
            }

            // Check if UI needs to be recreated
            if (_uiObject == null)
            {
                MelonLogger.Msg($"{LOG_PREFIX} UI missing - recreating");
                CreateUI();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"{LOG_PREFIX} Scene loaded: {sceneName}");

            // Ensure tracker exists after scene load
            if (DPSTracker.Entries == null)
            {
                MelonLogger.Msg($"{LOG_PREFIX} Relaunching tracker after scene load");
                DPSTracker.Launch();
            }

            // Ensure UI exists after scene load
            if (_uiObject == null)
            {
                MelonLogger.Msg($"{LOG_PREFIX} Recreating UI after scene load");
                CreateUI();
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"{LOG_PREFIX} Scene unloaded: {sceneName}");
        }

        public override void OnDeinitializeMelon()
        {
            try
            {
                _initialized = false;

                if (_harmony != null)
                {
                    _harmony.UnpatchSelf();
                    _harmony = null;
                }

                if (_uiObject != null)
                {
                    UnityEngine.Object.Destroy(_uiObject);
                    _uiObject = null;
                }

                ManualInjector.Cleanup();

                MelonLogger.Msg($"{LOG_PREFIX} Cleanup completed");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"{LOG_PREFIX} Cleanup failed: {ex.Message}");
            }
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg($"{LOG_PREFIX} Application quitting");
            OnDeinitializeMelon();
        }
    }
}
