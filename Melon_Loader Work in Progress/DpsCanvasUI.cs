// =======================
// DpsCanvasUI.cs
// =======================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MelonLoader;

namespace DPSAddon
{
    public class DpsCanvasUI : MonoBehaviour
    {
        private Canvas _canvas;
        private RectTransform _panel;
        private RectTransform _contentRect;
        private Dictionary<string, GameObject> _entryObjects = new Dictionary<string, GameObject>();
        private bool _showHealing = false;

        private void Awake()
        {
            MelonLogger.Msg("[DpsCanvasUI] Awake called");
            CreateCanvas();
            CreatePanel();
            CreateHeader();
            CreateContentArea();
            InvokeRepeating("UpdateDpsList", 1f, 1f);
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("DpsCanvas");
            canvasObj.transform.SetParent(this.transform);

            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        private void CreatePanel()
        {
            GameObject panelObj = new GameObject("DpsPanel");
            panelObj.transform.SetParent(_canvas.transform);

            _panel = panelObj.AddComponent<RectTransform>();
            _panel.sizeDelta = new Vector2(300f, 400f);
            _panel.anchorMin = new Vector2(0.5f, 0.5f);
            _panel.anchorMax = new Vector2(0.5f, 0.5f);
            _panel.pivot = new Vector2(0.5f, 0.5f);
            _panel.anchoredPosition = Vector2.zero;

            Image img = panelObj.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.75f);

            panelObj.AddComponent<Draggable>();
        }

        private void CreateHeader()
        {
            GameObject headerObj = new GameObject("Header");
            headerObj.transform.SetParent(_panel);
            RectTransform rect = headerObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300f, 30f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            Image bg = headerObj.AddComponent<Image>();
            bg.color = new Color(0.4f, 0f, 0.6f, 1f);

            GameObject textObj = new GameObject("HeaderText");
            textObj.transform.SetParent(headerObj.transform);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            RectTransform tmpRect = tmp.GetComponent<RectTransform>();
            tmpRect.anchorMin = new Vector2(0f, 0f);
            tmpRect.anchorMax = new Vector2(1f, 1f);
            tmpRect.offsetMin = tmpRect.offsetMax = Vector2.zero;

            tmp.text = "DPS Meter";
            tmp.fontSize = 22;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.yellow;

            GameObject cogBtn = new GameObject("SettingsButton");
            cogBtn.transform.SetParent(headerObj.transform);
            RectTransform cogRT = cogBtn.AddComponent<RectTransform>();
            cogRT.sizeDelta = new Vector2(25f, 25f);
            cogRT.anchorMin = new Vector2(1f, 0.5f);
            cogRT.anchorMax = new Vector2(1f, 0.5f);
            cogRT.pivot = new Vector2(1f, 0.5f);
            cogRT.anchoredPosition = new Vector2(-5f, 0f);

            Button cog = cogBtn.AddComponent<Button>();
            TextMeshProUGUI cogText = cogBtn.AddComponent<TextMeshProUGUI>();
            cogText.text = "⚙";
            cogText.fontSize = 18;
            cogText.alignment = TextAlignmentOptions.Center;
            cogText.color = Color.white;
            cog.onClick.AddListener(() => MelonLogger.Msg("[DpsCanvasUI] Settings button clicked"));
        }

        private void CreateContentArea()
        {
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(_panel);
            _contentRect = contentObj.AddComponent<RectTransform>();
            _contentRect.anchorMin = new Vector2(0f, 0f);
            _contentRect.anchorMax = new Vector2(1f, 1f);
            _contentRect.offsetMin = new Vector2(10f, 10f);
            _contentRect.offsetMax = new Vector2(-10f, -40f);

            VerticalLayoutGroup layout = contentObj.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.spacing = 3f;
        }

        private void UpdateDpsList()
        {
            foreach (var obj in _entryObjects.Values)
                Destroy(obj);
            _entryObjects.Clear();

            var entries = _showHealing ? DPSTracker.GetTopHealingEntries() : DPSTracker.GetTopDpsEntries();
            float maxValue = 1f;
            foreach (var entry in entries)
            {
                float value = _showHealing ? entry.Hps : entry.Dps;
                if (value > maxValue) maxValue = value;
            }

            int i = 1;
            foreach (var entry in entries)
            {
                GameObject row = new GameObject($"Row_{i}");
                row.transform.SetParent(_contentRect);
                RectTransform rowRT = row.AddComponent<RectTransform>();
                rowRT.sizeDelta = new Vector2(0, 22);

                HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandHeight = true;
                layout.spacing = 4f;

                GameObject barBg = new GameObject("BarBg");
                barBg.transform.SetParent(row.transform);
                Image bg = barBg.AddComponent<Image>();
                bg.color = Color.gray;
                RectTransform bgRT = barBg.GetComponent<RectTransform>();
                bgRT.sizeDelta = new Vector2(150, 16);

                GameObject barFg = new GameObject("BarFg");
                barFg.transform.SetParent(barBg.transform);
                Image fg = barFg.AddComponent<Image>();
                fg.color = Color.red;
                RectTransform fgRT = barFg.GetComponent<RectTransform>();
                float pct = (_showHealing ? entry.Hps : entry.Dps) / maxValue;
                fgRT.anchorMin = new Vector2(0f, 0f);
                fgRT.anchorMax = new Vector2(pct, 1f);
                fgRT.offsetMin = fgRT.offsetMax = Vector2.zero;

                GameObject txtObj = new GameObject("Label");
                txtObj.transform.SetParent(row.transform);
                TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
                txt.fontSize = 14;
                txt.color = Color.white;
                txt.alignment = TextAlignmentOptions.MidlineLeft;
                txt.text = $"{i}. {entry.Name} - {(_showHealing ? "HPS" : "DPS")}: {(_showHealing ? entry.Hps : entry.Dps):F1}";

                _entryObjects[entry.Name] = row;
                i++;
            }
        }
    }
}
