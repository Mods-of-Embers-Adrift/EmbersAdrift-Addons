using MelonLoader;
using UnityEngine;
using SoL.Managers;

namespace QuestTrackerMod
{
    public class QuestTrackerMod : MelonMod
    {
        private GameObject trackerCanvas;
        private QuestTrackerUI trackerUI;
        private bool questManagerSubscribed = false;
        private bool initialized = false;

        public override void OnUpdate()
        {
            // Only initialize UI once Player_Spawn(Clone) is present in the scene
            if (!initialized && GameObject.Find("Player_Spawn(Clone)") != null)
            {
                MelonLogger.Msg("[QuestTracker] Player_Spawn(Clone) detected, initializing QuestTracker UI...");
                trackerCanvas = new GameObject("QuestTrackerCanvas");
                Object.DontDestroyOnLoad(trackerCanvas);
                trackerUI = trackerCanvas.AddComponent<QuestTrackerUI>();
                trackerUI.Init();
                initialized = true;
            }

            // Attach to QuestManager.BBTasksUpdated as soon as it's available
            if (initialized && !questManagerSubscribed)
            {
                QuestManager questManager = GameManager.QuestManager;
                if (questManager != null)
                {
                    questManager.BBTasksUpdated += trackerUI.UpdateQuestDisplay;
                    questManagerSubscribed = true;
                    MelonLogger.Msg("[QuestTracker] Successfully subscribed to BBTasksUpdated!");
                }
            }

            // Toggle with F9
            if (Input.GetKeyDown(KeyCode.F9))
            {
                MelonLogger.Msg("[QuestTracker] F9 pressed, toggling UI.");
                if (trackerUI != null)
                    trackerUI.Toggle();
                else
                    MelonLogger.Warning("[QuestTracker] trackerUI is null in OnUpdate!");
            }
        }

        public override void OnDeinitializeMelon()
        {
            MelonLogger.Msg("[QuestTracker] Cleaning up mod...");
            QuestManager questManager = GameManager.QuestManager;
            if (questManager != null && questManagerSubscribed && trackerUI != null)
            {
                questManager.BBTasksUpdated -= trackerUI.UpdateQuestDisplay;
                MelonLogger.Msg("[QuestTracker] Unsubscribed from BBTasksUpdated.");
            }
            if (trackerCanvas != null)
            {
                GameObject.Destroy(trackerCanvas);
                MelonLogger.Msg("[QuestTracker] Destroyed trackerCanvas.");
            }
            questManagerSubscribed = false;
            initialized = false;
        }
    }
}