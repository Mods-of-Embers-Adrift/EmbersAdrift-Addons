using UnityEngine;
using MelonLoader;
using System.Linq;
using System.Collections.Generic;

namespace DPSAddon
{
    public class DPSCanvasUI : MonoBehaviour
    {
        #region Constants and Fields
        // Window-related constants
        private const string WINDOW_PREFS_KEY = "DPSWindowPos";
        private const string WINDOW_SIZE_PREFS_KEY = "DPSWindowSize";
        private const string ALLOW_DRAG_PREFS_KEY = "DPSAllowDrag";
        private const string ALLOW_RESIZE_PREFS_KEY = "DPSAllowResize";
        private const string DEFAULT_WINDOW_POS = "20,20";
        private const string DEFAULT_WINDOW_SIZE = "300,400";

        // UI state fields
        private Rect _windowRect = new Rect(20, 20, 300, 400);
        private bool _visible = true;
        private bool _showMinimizedButton = false;
        private Rect _minimizedButtonRect = new Rect(10, 10, 120, 30);
        private Vector2 _scrollPosition;
        private bool _showHealing;

        // Window control fields
        private bool _isResizing;
        private Vector2 _resizeStart;
        private Rect _resizeRect;
        private bool _allowResize = true;
        private bool _allowDrag = true;

        // Style fields
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _checkboxStyle;
        private GUIStyle _resizeStyle;

        #endregion

        #region Classes and Enums
        private enum Tab { Current, History, Settings }
        private Tab _currentTab = Tab.Current;
        #endregion

        #region Initialization and Lifecycle Methods
        private void Awake()
        {
            MelonLogger.Msg("[DPSCanvasUI] Initialized");
            LoadWindowPosition();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    ResetPosition();
                }
                else
                {
                    ToggleVisibility();
                }
            }
        }

        private void OnGUI()
        {
            InitStyles();

            // Draw minimized button if UI is hidden
            if (_showMinimizedButton)
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
                if (GUI.Button(_minimizedButtonRect, "DPS/HPS Addon", _buttonStyle))
                {
                    ToggleVisibility();
                }
            }

            // Draw main window if visible
            if (_visible)
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
                _windowRect = GUILayout.Window(1001, _windowRect, DrawWindow, "DPS Meter");
            }
        }
        #endregion

        #region Window Management
        private void LoadWindowPosition()
        {
            string[] pos = PlayerPrefs.GetString(WINDOW_PREFS_KEY, DEFAULT_WINDOW_POS).Split(',');
            string[] size = PlayerPrefs.GetString(WINDOW_SIZE_PREFS_KEY, DEFAULT_WINDOW_SIZE).Split(',');

            if (float.TryParse(pos[0], out float x) && float.TryParse(pos[1], out float y) &&
                float.TryParse(size[0], out float width) && float.TryParse(size[1], out float height))
            {
                _windowRect = new Rect(x, y, width, height);
            }

            // Load window control preferences
            _allowDrag = PlayerPrefs.GetInt(ALLOW_DRAG_PREFS_KEY, 1) == 1;
            _allowResize = PlayerPrefs.GetInt(ALLOW_RESIZE_PREFS_KEY, 1) == 1;
        }

        private void SaveWindowPosition()
        {
            PlayerPrefs.SetString(WINDOW_PREFS_KEY, $"{_windowRect.x},{_windowRect.y}");
            PlayerPrefs.SetString(WINDOW_SIZE_PREFS_KEY, $"{_windowRect.width},{_windowRect.height}");

            // Save window control preferences
            PlayerPrefs.SetInt(ALLOW_DRAG_PREFS_KEY, _allowDrag ? 1 : 0);
            PlayerPrefs.SetInt(ALLOW_RESIZE_PREFS_KEY, _allowResize ? 1 : 0);

            PlayerPrefs.Save();
        }

        private void ResetPosition()
        {
            string[] defaultPos = DEFAULT_WINDOW_POS.Split(',');
            string[] defaultSize = DEFAULT_WINDOW_SIZE.Split(',');

            if (float.TryParse(defaultPos[0], out float x) &&
                float.TryParse(defaultPos[1], out float y) &&
                float.TryParse(defaultSize[0], out float width) &&
                float.TryParse(defaultSize[1], out float height))
            {
                _windowRect = new Rect(x, y, width, height);
                SaveWindowPosition();
            }

            MelonLogger.Msg("[DPSCanvasUI] Position reset to default");
        }

        private void ToggleVisibility()
        {
            _visible = !_visible;
            _showMinimizedButton = !_visible;
            MelonLogger.Msg($"[DPSCanvasUI] Visibility toggled: {_visible}");
        }
        #endregion

        #region Style Initialization
        private void InitStyles()
        {
            if (_labelStyle != null) return;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            _tabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                padding = new RectOffset(10, 10, 5, 5)
            };

            _checkboxStyle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            _resizeStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = CreateResizeTexture() }
            };
        }

        private Texture2D CreateResizeTexture()
        {
            var tex = new Texture2D(16, 16);
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    tex.SetPixel(i, j, i > 12 || j > 12 ? Color.white : Color.clear);
            tex.Apply();
            return tex;
        }
        #endregion

        #region Drawing Methods
        private void DrawWindow(int windowID)
        {
            DrawTabs();

            switch (_currentTab)
            {
                case Tab.Current:
                    DrawCurrentTab();
                    break;
                case Tab.History:
                    DrawHistoryTab();
                    break;
                case Tab.Settings:
                    DrawSettingsTab();
                    break;
            }

            // Only show resize handle if allowed
            if (_allowResize)
            {
                _resizeRect = new Rect(_windowRect.width - 16, _windowRect.height - 16, 16, 16);
                if (GUI.RepeatButton(_resizeRect, "", _resizeStyle))
                {
                    _isResizing = true;
                    if (!_resizeStart.x.Equals(Event.current.mousePosition.x))
                        _resizeStart = Event.current.mousePosition;
                }

                if (_isResizing)
                {
                    _windowRect.width = Mathf.Max(300, _resizeStart.x + (Event.current.mousePosition.x - _resizeStart.x));
                    _windowRect.height = Mathf.Max(400, _resizeStart.y + (Event.current.mousePosition.y - _resizeStart.y));

                    if (Event.current.type == EventType.MouseUp)
                    {
                        _isResizing = false;
                        SaveWindowPosition();
                    }
                }
            }

            // Only allow dragging if enabled
            if (_allowDrag)
            {
                GUI.DragWindow();
            }
        }

        private void DrawTabs()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_currentTab == Tab.Current, "Current", _tabStyle)) _currentTab = Tab.Current;
            if (GUILayout.Toggle(_currentTab == Tab.History, "History", _tabStyle)) _currentTab = Tab.History;
            if (GUILayout.Toggle(_currentTab == Tab.Settings, "Settings", _tabStyle)) _currentTab = Tab.Settings;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        private void DrawCurrentTab()
        {
            // Toggle buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(!_showHealing, "Damage", _buttonStyle)) _showHealing = false;
            if (GUILayout.Toggle(_showHealing, "Healing", _buttonStyle)) _showHealing = true;

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset", _buttonStyle)) DPSTracker.Reset();
            if (GUILayout.Button("Close", _buttonStyle)) ToggleVisibility();
            GUILayout.EndHorizontal();

            DrawMeters();
        }

        private void DrawHistoryTab()
        {
            GUILayout.Label("Combat History", _headerStyle);
            // Add history implementation here if needed
        }

        private void DrawSettingsTab()
        {
            GUILayout.Label("Settings", _headerStyle);

            GUILayout.Space(10);
            GUILayout.Label("Window Controls:", _headerStyle);

            _allowDrag = GUILayout.Toggle(_allowDrag, "Allow Window Dragging", _checkboxStyle);
            _allowResize = GUILayout.Toggle(_allowResize, "Allow Window Resizing", _checkboxStyle);

            GUILayout.Space(10);
            GUILayout.Label("Keybinds:", _labelStyle);
            GUILayout.Label("Insert - Toggle UI", _labelStyle);
            GUILayout.Label("Shift + Insert - Reset Position", _labelStyle);

            GUILayout.Space(10);
            if (GUILayout.Button("Reset Position", _buttonStyle))
            {
                ResetPosition();
            }

            if (GUILayout.Button("Save Window Position", _buttonStyle))
            {
                SaveWindowPosition();
            }
        }

        private void DrawMeters()
        {
            GUILayout.Space(5);

            // Headers
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", _headerStyle, GUILayout.Width(100));
            GUILayout.Label(_showHealing ? "HPS" : "DPS", _headerStyle, GUILayout.Width(60));
            GUILayout.Label("Total", _headerStyle, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            var entries = _showHealing ? DPSTracker.HealingEntries : DPSTracker.Entries;
            if (entries != null && entries.Any())
            {
                foreach (var entry in entries.OrderByDescending(e => _showHealing ? e.Value.HPS : e.Value.DPS))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entry.Key, _labelStyle, GUILayout.Width(100));
                    GUILayout.Label((_showHealing ? entry.Value.HPS : entry.Value.DPS).ToString("F1"),
                        _labelStyle, GUILayout.Width(60));
                    GUILayout.Label((_showHealing ? entry.Value.TotalHealing : entry.Value.TotalDamage).ToString("F0"),
                        _labelStyle, GUILayout.Width(60));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No data available", _labelStyle);
            }

            GUILayout.EndScrollView();

            // Current fight duration
            if (entries != null && entries.Any())
            {
                var duration = entries.Values.Max(e => e.Duration);
                GUILayout.Label($"Fight Duration: {duration:F1}s", _labelStyle);
            }
        }

        #endregion
    }
}