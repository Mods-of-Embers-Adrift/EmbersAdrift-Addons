using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoL.Game.Quests;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using SoL.Game.Objects;
using SoL.Game;

namespace QuestTrackerMod
{
    public class QuestTrackerUI : MonoBehaviour
    {
        private VerticalLayoutGroup layoutGroup;
        private readonly List<GameObject> questEntries = new List<GameObject>();
        private GameObject panel;
        private bool isVisible = false;

        // Resize feature
        private Toggle resizeCheckbox;
        private bool resizeEnabled = false;
        private Vector2 minPanelSize = new Vector2(140, 60);
        private Vector2 maxPanelSize = new Vector2(800, 600);
        private Vector2 currentPanelSize = new Vector2(140, 60);
        private bool resizingNow = false;
        private Vector2 dragStartMouse;
        private Vector2 dragStartSize;
        private GameObject resizeHandle;

        public void Init()
        {
            MelonLogger.Msg("[QuestTracker] Initializing UI canvas...");
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            panel = new GameObject("QuestTrackerPanel", typeof(Image));
            panel.transform.SetParent(transform, false);
            Image img = panel.GetComponent<Image>();
            img.color = new Color(0.08f, 0.08f, 0.12f, 0.85f);

            var shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
            shadow.effectDistance = new Vector2(2, -2);

            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.85f, 0.60f);
            rect.anchorMax = new Vector2(1f, 0.70f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.sizeDelta = currentPanelSize;

            // --- Main Vertical Layout ---
            layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.spacing = 0.5f;
            layoutGroup.padding = new RectOffset(2, 2, 2, 2);

            // --- Small Resize Checkbox (bottom-left) ---
            GameObject checkboxObj = new GameObject("ResizeCheckbox", typeof(Toggle));
            checkboxObj.transform.SetParent(panel.transform, false);
            resizeCheckbox = checkboxObj.GetComponent<Toggle>();
            RectTransform checkboxRect = checkboxObj.GetComponent<RectTransform>();
            checkboxRect.anchorMin = new Vector2(0, 0);
            checkboxRect.anchorMax = new Vector2(0, 0);
            checkboxRect.pivot = new Vector2(0, 0);
            checkboxRect.anchoredPosition = new Vector2(4, 4); // 4px from bottom-left
            checkboxRect.sizeDelta = new Vector2(16, 16); // Small size

            // Checkbox visuals: Small box with X or no X (no label)
            // Box
            GameObject cbBg = new GameObject("CB_BG");
            cbBg.transform.SetParent(checkboxObj.transform, false);
            Image cbBgImg = cbBg.AddComponent<Image>();
            cbBgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform cbBgRect = cbBg.GetComponent<RectTransform>();
            cbBgRect.anchorMin = Vector2.zero;
            cbBgRect.anchorMax = Vector2.one;
            cbBgRect.pivot = new Vector2(0.5f, 0.5f);
            cbBgRect.anchoredPosition = Vector2.zero;
            cbBgRect.sizeDelta = Vector2.zero;

            // X mark
            GameObject cbCheckGO = new GameObject("CB_Checkmark");
            cbCheckGO.transform.SetParent(cbBg.transform, false);
            TextMeshProUGUI cbCheckText = cbCheckGO.AddComponent<TextMeshProUGUI>();
            cbCheckText.text = "X";
            cbCheckText.fontSize = 12;
            cbCheckText.alignment = TextAlignmentOptions.Center;
            cbCheckText.color = Color.green;
            cbCheckText.enableWordWrapping = false;
            cbCheckText.gameObject.SetActive(false);
            RectTransform cbCheckRect = cbCheckGO.GetComponent<RectTransform>();
            cbCheckRect.anchorMin = Vector2.zero;
            cbCheckRect.anchorMax = Vector2.one;
            cbCheckRect.offsetMin = Vector2.zero;
            cbCheckRect.offsetMax = Vector2.zero;

            resizeCheckbox.targetGraphic = cbBgImg;
            // Instead of using Unity's built-in graphic, we toggle our TMP X mark:
            resizeCheckbox.graphic = null;

            resizeCheckbox.onValueChanged.AddListener((enabled) =>
            {
                cbCheckText.gameObject.SetActive(enabled);
                resizeEnabled = enabled;
                resizeHandle.SetActive(enabled);
            });

            // --- Resize Handle (bottom-left) ---
            resizeHandle = new GameObject("ResizeHandle", typeof(Image));
            resizeHandle.transform.SetParent(panel.transform, false);
            Image handleImage = resizeHandle.GetComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.35f);
            RectTransform handleRect = resizeHandle.GetComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0, 0);
            handleRect.anchorMax = new Vector2(0, 0);
            handleRect.pivot = new Vector2(0, 0);
            handleRect.sizeDelta = new Vector2(18, 18);
            handleRect.anchoredPosition = new Vector2(0, 0);
            resizeHandle.SetActive(false);

            panel.SetActive(isVisible);

            MelonLogger.Msg("[QuestTracker] UI initialization done.");
        }

        void Update()
        {
            if (resizeEnabled && resizeHandle.activeSelf)
            {
                RectTransform handleRect = resizeHandle.GetComponent<RectTransform>();
                Vector2 mousePos = Input.mousePosition;
                RectTransform panelRect = panel.GetComponent<RectTransform>();

                // Convert mouse to panel local space
                Vector2 localMouse;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelRect,
                    mousePos,
                    null,
                    out localMouse
                );

                // Begin resize on mouse down (bottom-left)
                if (!resizingNow && RectTransformUtility.RectangleContainsScreenPoint(handleRect, mousePos))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        resizingNow = true;
                        dragStartMouse = mousePos;
                        dragStartSize = panelRect.sizeDelta;
                    }
                }

                // Drag
                if (resizingNow)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 delta = mousePos - dragStartMouse;
                        Vector2 newSize = dragStartSize - new Vector2(delta.x, -delta.y); // Invert X for left anchor
                        newSize.x = Mathf.Clamp(newSize.x, minPanelSize.x, maxPanelSize.x);
                        newSize.y = Mathf.Clamp(newSize.y, minPanelSize.y, maxPanelSize.y);
                        panelRect.sizeDelta = newSize;
                        currentPanelSize = newSize;
                    }
                    else
                    {
                        resizingNow = false;
                    }
                }
            }
        }

        public void Toggle()
        {
            isVisible = !isVisible;
            MelonLogger.Msg($"[QuestTracker] Toggling tracker UI: {(isVisible ? "Visible" : "Hidden")}");
            if (panel != null)
                panel.SetActive(isVisible);
            if (isVisible)
                UpdateQuestDisplay();
        }

        public void UpdateQuestDisplay()
        {
            MelonLogger.Msg("[QuestTracker] Updating quest display...");
            if (panel == null || !isVisible)
                return;

            foreach (GameObject entry in questEntries)
                Destroy(entry);
            questEntries.Clear();

            var progression = LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null
                && LocalPlayer.GameEntity.CollectionController.Record != null
                ? LocalPlayer.GameEntity.CollectionController.Record.Progression
                : null;

            var bbTasks = progression != null ? progression.BBTasks : null;
            if (bbTasks == null)
                return;

            foreach (var kv in bbTasks)
            {
                BBTask bbtask;
                if (InternalGameDatabase.BBTasks.TryGetItem(kv.Key, out bbtask) && bbtask.Enabled)
                {
                    string boardName = bbtask.BulletinBoard?.Title ?? "";
                    string zone = null;

                    // Try to get zone property
                    if (bbtask.BulletinBoard != null)
                    {
                        var zoneProp = bbtask.BulletinBoard.GetType().GetProperty("Zone");
                        if (zoneProp != null)
                            zone = zoneProp.GetValue(bbtask.BulletinBoard) as string;
                    }
                    // Always extract from boardName if property is empty or whitespace
                    if (string.IsNullOrWhiteSpace(zone) && boardName.Contains("[") && boardName.Contains("]"))
                    {
                        var match = Regex.Match(boardName, @"\[(.*?)\]");
                        if (match.Success)
                            zone = match.Groups[1].Value;
                    }
                    // At this point, 'zone' will always be present
                    string questTitle = $"{boardName} [{zone}]";

                    // Commission name (quest name)
                    string commissionName = bbtask.Title ?? "Commission";

                    // Description
                    string description = "";
                    if (bbtask.GetType().GetProperty("Description") != null)
                        description = (string)bbtask.GetType().GetProperty("Description")?.GetValue(bbtask);
                    if (string.IsNullOrWhiteSpace(description) && bbtask.Objectives != null && bbtask.Objectives.Count > 0)
                        description = bbtask.Objectives[0].Description;

                    // --- QUEST TITLE ---
                    GameObject titleGO = new GameObject("QuestTitle");
                    titleGO.transform.SetParent(panel.transform, false);
                    TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
                    titleText.fontSize = 4;
                    titleText.fontStyle = FontStyles.Bold;
                    titleText.color = new Color(1f, 0.85f, 0.3f); // gold
                    titleText.text = questTitle;
                    titleText.enableWordWrapping = false;
                    titleText.margin = new Vector4(0, 3, 0, 0.5f);
                    questEntries.Add(titleGO);

                    // --- COMMISSION NAME ---
                    GameObject commGO = new GameObject("CommissionLabel");
                    commGO.transform.SetParent(panel.transform, false);
                    TextMeshProUGUI commText = commGO.AddComponent<TextMeshProUGUI>();
                    commText.fontSize = 3;
                    commText.fontStyle = FontStyles.Bold;
                    commText.color = new Color(0.85f, 0.95f, 1f);
                    commText.text = $"Commission: {commissionName}:";
                    commText.enableWordWrapping = false;
                    commText.margin = new Vector4(0, 0, 0, 0.5f);
                    questEntries.Add(commGO);

                    // --- DESCRIPTION ---
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        GameObject descGO = new GameObject("DescriptionLabel");
                        descGO.transform.SetParent(panel.transform, false);
                        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
                        descText.fontSize = 3;
                        descText.fontStyle = FontStyles.Normal;
                        descText.color = new Color(0.95f, 0.95f, 0.95f);
                        descText.text = description;
                        descText.enableWordWrapping = false;
                        descText.margin = new Vector4(0, 0, 0, 0.5f);
                        questEntries.Add(descGO);
                    }

                    // --- OBJECTIVES ---
                    var progData = kv.Value;
                    if (bbtask.Objectives != null && progData.Objectives != null && bbtask.Objectives.Count > 0)
                    {
                        foreach (var obj in bbtask.Objectives)
                        {
                            ObjectiveProgressionData objProg = null;
                            foreach (var prog in progData.Objectives)
                                if (prog.ObjectiveId == obj.Id)
                                {
                                    objProg = prog;
                                    break;
                                }
                            int done = objProg != null ? objProg.IterationsCompleted : 0;
                            int total = obj.IterationsRequired > 0 ? obj.IterationsRequired : 1;
                            bool complete = (objProg != null && done >= total);

                            // Checkbox style with progress
                            string checkbox = complete ? "[x]" : "[ ]";
                            string progressTxt = total > 1 ? $" ({done}/{total})" : "";
                            string line = $"{checkbox} {obj.Description}{progressTxt}";
                            GameObject objGO = new GameObject("ObjectiveLabel");
                            objGO.transform.SetParent(panel.transform, false);
                            TextMeshProUGUI objText = objGO.AddComponent<TextMeshProUGUI>();
                            objText.fontSize = 3;
                            objText.color = complete ? new Color(0.7f, 1f, 0.7f) : Color.white;
                            objText.text = line;
                            objText.enableWordWrapping = false;
                            objText.margin = new Vector4(0, 0, 0, 1f);
                            questEntries.Add(objGO);
                        }
                    }
                }
            }
        }
    }
}