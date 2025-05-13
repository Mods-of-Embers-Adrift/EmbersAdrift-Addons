using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E07 RID: 3591
public class UserDpsViewer : MonoBehaviour
{
	// Token: 0x06006A5B RID: 27227
	public static void Launch()
	{
		if (GameObject.Find("UserDpsViewer") == null)
		{
			GameObject gameObject = new GameObject("UserDpsViewer");
			gameObject.AddComponent<UserDpsViewer>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	// Token: 0x06006A5C RID: 27228
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2))
		{
			this._visible = !this._visible;
			Debug.Log("[UserDpsViewer] Overlay toggled: " + (this._visible ? "ON" : "OFF"));
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			this._collapsed = !this._collapsed;
			Debug.Log("[UserDpsViewer] View toggled: " + (this._collapsed ? "Collapsed" : "Expanded"));
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			this._filterText = "";
			Debug.Log("[UserDpsViewer] Filter cleared.");
		}
		this.ClampWindowToScreen();
	}

	// Token: 0x06006A5D RID: 27229
	private void OnGUI()
	{
		if (!this._visible)
		{
			return;
		}
		GUIStyle windowStyle = new GUIStyle(GUI.skin.window)
		{
			fontSize = 14,
			padding = new RectOffset(10, 10, 20, 10)
		};
		this._windowRect = GUI.Window(911, this._windowRect, new GUI.WindowFunction(this.DrawWindow), "DPS Overlay", windowStyle);
	}

	// Token: 0x06006AAF RID: 27311
	private void DrawWindow(int windowId)
	{
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 13
		};
		GUIStyle headerStyle = new GUIStyle(labelStyle)
		{
			fontStyle = FontStyle.Bold
		};
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Reset DPS", new GUILayoutOption[]
		{
			GUILayout.Height(24f)
		}))
		{
			UserDpsTracker.Reset();
		}
		if (GUILayout.Button(this._collapsed ? "Expand View" : "Collapse View", new GUILayoutOption[]
		{
			GUILayout.Height(24f)
		}))
		{
			this._collapsed = !this._collapsed;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Filter:", new GUILayoutOption[]
		{
			GUILayout.Width(40f)
		});
		this._filterText = GUILayout.TextField(this._filterText, new GUILayoutOption[]
		{
			GUILayout.Width(200f)
		});
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		if (this._collapsed)
		{
			GUILayout.Label("Name                     DPS", headerStyle, Array.Empty<GUILayoutOption>());
		}
		else
		{
			GUILayout.Label("Name                     DPS     Total Dmg   Duration (s)", headerStyle, Array.Empty<GUILayoutOption>());
		}
		Dictionary<string, float> currentDps = UserDpsTracker.CurrentDps;
		IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> entries = UserDpsTracker.Entries;
		if (currentDps == null || currentDps.Count == 0 || entries == null)
		{
			GUILayout.Label("No DPS data available.", labelStyle, Array.Empty<GUILayoutOption>());
			return;
		}
		this._scrollPos = GUILayout.BeginScrollView(this._scrollPos, new GUILayoutOption[]
		{
			GUILayout.Height(400f)
		});
		bool alternate = false;
		foreach (KeyValuePair<string, float> kvp in this.SortedByDps(currentDps))
		{
			UserDpsTracker.DpsEntry entry;
			if (entries.TryGetValue(kvp.Key, out entry) && (string.IsNullOrEmpty(this._filterText) || kvp.Key.ToLower().Contains(this._filterText.ToLower())))
			{
				GUI.backgroundColor = (alternate ? new Color(0.9f, 0.9f, 0.9f, 0.3f) : Color.clear);
				GUILayout.BeginHorizontal("box", Array.Empty<GUILayoutOption>());
				if (this._collapsed)
				{
					GUILayout.Label(string.Format("{0,-18}  {1,6:F1}", kvp.Key, entry.DPS), labelStyle, Array.Empty<GUILayoutOption>());
				}
				else
				{
					GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
					{
						kvp.Key,
						entry.DPS,
						entry.TotalDamage,
						entry.Duration
					}), labelStyle, Array.Empty<GUILayoutOption>());
				}
				GUILayout.EndHorizontal();
				alternate = !alternate;
			}
		}
		GUILayout.EndScrollView();
		GUILayout.Space(10f);
		GUILayout.Label("Shortcuts", headerStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F2 - Toggle Overlay", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F3 - Collapse/Expand View", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F4 - Clear Filter", labelStyle, Array.Empty<GUILayoutOption>());
		GUI.DragWindow();
	}

	// Token: 0x06006AB0 RID: 27312
	private IEnumerable<KeyValuePair<string, float>> SortedByDps(Dictionary<string, float> dpsDict)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>(dpsDict);
		list.Sort((KeyValuePair<string, float> a, KeyValuePair<string, float> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	// Token: 0x06006AB1 RID: 27313
	private void ClampWindowToScreen()
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;
		this._windowRect.x = Mathf.Clamp(this._windowRect.x, 0f, screenWidth - this._windowRect.width);
		this._windowRect.y = Mathf.Clamp(this._windowRect.y, 0f, screenHeight - this._windowRect.height);
	}

	// Token: 0x06006ADB RID: 27355
	private void Awake()
	{
		this._windowRect.x = PlayerPrefs.GetFloat("DpsViewerPosX", 20f);
		this._windowRect.y = PlayerPrefs.GetFloat("DpsViewerPosY", 20f);
		this._visible = (PlayerPrefs.GetInt("DpsViewerVisible", 1) == 1);
		this._collapsed = (PlayerPrefs.GetInt("DpsViewerCollapsed", 0) == 1);
	}

	// Token: 0x06006ADC RID: 27356
	private void OnDestroy()
	{
		PlayerPrefs.SetFloat("DpsViewerPosX", this._windowRect.x);
		PlayerPrefs.SetFloat("DpsViewerPosY", this._windowRect.y);
		PlayerPrefs.SetInt("DpsViewerVisible", this._visible ? 1 : 0);
		PlayerPrefs.SetInt("DpsViewerCollapsed", this._collapsed ? 1 : 0);
		PlayerPrefs.Save();
	}

	// Token: 0x04005CB5 RID: 23733
	private bool _visible = true;

	// Token: 0x04005CB6 RID: 23734
	private Rect _windowRect = new Rect(20f, 20f, 450f, 500f);

	// Token: 0x04005D06 RID: 23814
	private Vector2 _scrollPos;

	// Token: 0x04005D19 RID: 23833
	private bool _collapsed;

	// Token: 0x04005D1C RID: 23836
	private string _filterText = "";

	// Token: 0x04005D1D RID: 23837
	private const string PrefKeyX = "DpsViewerPosX";

	// Token: 0x04005D1E RID: 23838
	private const string PrefKeyY = "DpsViewerPosY";

	// Token: 0x04005D1F RID: 23839
	private const string PrefKeyVisible = "DpsViewerVisible";

	// Token: 0x04005D20 RID: 23840
	private const string PrefKeyCollapsed = "DpsViewerCollapsed";
}
