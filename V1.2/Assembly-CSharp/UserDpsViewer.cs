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
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			this._collapsed = !this._collapsed;
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			this._filterText = "";
		}
	}

	// Token: 0x06006A5D RID: 27229
	private void OnGUI()
	{
		if (!this._visible)
		{
			return;
		}
		GUIStyle winStyle = new GUIStyle(GUI.skin.window)
		{
			fontSize = 14,
			padding = new RectOffset(10, 10, 20, 10)
		};
		this._windowRect = GUI.Window(911, this._windowRect, new GUI.WindowFunction(this.DrawMainWindow), "DPS Overlay", winStyle);
		if (this._activeSessionIndex != null)
		{
			int? activeSessionIndex = this._activeSessionIndex;
			IReadOnlyList<UserDpsTracker.FightSession> pastFights = UserDpsTracker.PastFights;
			int? num = (pastFights != null) ? new int?(pastFights.Count) : null;
			if (activeSessionIndex.GetValueOrDefault() < num.GetValueOrDefault() & (activeSessionIndex != null & num != null))
			{
				this._popupRect = GUI.Window(912, this._popupRect, new GUI.WindowFunction(this.DrawSessionPopup), "Fight Session #" + (this._activeSessionIndex + 1), winStyle);
			}
		}
	}

	// Token: 0x06006A5E RID: 27230
	public UserDpsViewer()
	{
		this._visible = true;
		this._filterText = "";
		this._popupRect = new Rect(680f, 20f, 520f, 500f);
		base..ctor();
	}

	// Token: 0x06006A60 RID: 27232
	private IEnumerable<KeyValuePair<string, float>> SortedByDps(Dictionary<string, float> dpsDict)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>(dpsDict);
		list.Sort((KeyValuePair<string, float> a, KeyValuePair<string, float> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	// Token: 0x06006A62 RID: 27234
	private void Awake()
	{
		this._windowRect.x = PlayerPrefs.GetFloat("DpsViewerPosX", 20f);
		this._windowRect.y = PlayerPrefs.GetFloat("DpsViewerPosY", 20f);
		this._visible = (PlayerPrefs.GetInt("DpsViewerVisible", 1) == 1);
		this._collapsed = (PlayerPrefs.GetInt("DpsViewerCollapsed", 0) == 1);
	}

	// Token: 0x06006A63 RID: 27235
	private void OnDestroy()
	{
		PlayerPrefs.SetFloat("DpsViewerPosX", this._windowRect.x);
		PlayerPrefs.SetFloat("DpsViewerPosY", this._windowRect.y);
		PlayerPrefs.SetInt("DpsViewerVisible", this._visible ? 1 : 0);
		PlayerPrefs.SetInt("DpsViewerCollapsed", this._collapsed ? 1 : 0);
		PlayerPrefs.Save();
	}

	// Token: 0x06006A87 RID: 27271
	private void DrawMainWindow(int windowId)
	{
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
		GUILayout.Label(this._collapsed ? "Name                     DPS" : "Name                     DPS     Total Dmg   Duration (s)", Array.Empty<GUILayoutOption>());
		Dictionary<string, float> dpsData = UserDpsTracker.CurrentDps;
		IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> entries = UserDpsTracker.Entries;
		if (dpsData == null || entries == null || dpsData.Count == 0)
		{
			GUILayout.Label("No DPS data available.", Array.Empty<GUILayoutOption>());
		}
		else
		{
			this._scrollPos = GUILayout.BeginScrollView(this._scrollPos, new GUILayoutOption[]
			{
				GUILayout.Height(360f)
			});
			bool alt = false;
			foreach (KeyValuePair<string, float> kvp in this.SortedByDps(dpsData))
			{
				UserDpsTracker.DpsEntry entry;
				if (entries.TryGetValue(kvp.Key, out entry) && (string.IsNullOrEmpty(this._filterText) || kvp.Key.ToLower().Contains(this._filterText.ToLower())))
				{
					GUI.backgroundColor = (alt ? new Color(0.9f, 0.9f, 0.9f, 0.3f) : Color.clear);
					GUILayout.BeginHorizontal("box", Array.Empty<GUILayoutOption>());
					if (this._collapsed)
					{
						GUILayout.Label(string.Format("{0,-18}  {1,6:F1}", kvp.Key, entry.DPS), Array.Empty<GUILayoutOption>());
					}
					else
					{
						GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
						{
							kvp.Key,
							entry.DPS,
							entry.TotalDamage,
							entry.Duration
						}), Array.Empty<GUILayoutOption>());
					}
					GUILayout.EndHorizontal();
					alt = !alt;
				}
			}
			GUILayout.EndScrollView();
		}
		GUILayout.Label("Fight History:", Array.Empty<GUILayoutOption>());
		IReadOnlyList<UserDpsTracker.FightSession> sessions = UserDpsTracker.PastFights;
		if (sessions != null)
		{
			for (int i = 0; i < sessions.Count; i++)
			{
				if (GUILayout.Button("View: " + sessions[i].ToString(), Array.Empty<GUILayoutOption>()))
				{
					this._activeSessionIndex = new int?(i);
				}
			}
		}
		GUILayout.Label("Shortcuts:", Array.Empty<GUILayoutOption>());
		GUILayout.Label("F2 - Toggle Overlay", Array.Empty<GUILayoutOption>());
		GUILayout.Label("F3 - Collapse/Expand View", Array.Empty<GUILayoutOption>());
		GUILayout.Label("F4 - Clear Filter", Array.Empty<GUILayoutOption>());
		GUI.DragWindow();
	}

	// Token: 0x06006A88 RID: 27272
	private void DrawSessionPopup(int windowId)
	{
		if (this._activeSessionIndex == null)
		{
			return;
		}
		IReadOnlyList<UserDpsTracker.FightSession> pastFights = UserDpsTracker.PastFights;
		UserDpsTracker.FightSession session = (pastFights != null) ? pastFights[this._activeSessionIndex.Value] : null;
		if (session == null)
		{
			return;
		}
		if (GUILayout.Button("Close", Array.Empty<GUILayoutOption>()))
		{
			this._activeSessionIndex = null;
			return;
		}
		this._popupScroll = GUILayout.BeginScrollView(this._popupScroll, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Name                     DPS     Total Dmg   Duration (s)", Array.Empty<GUILayoutOption>());
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in session.Entries)
		{
			UserDpsTracker.DpsEntry e = kvp.Value;
			GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
			{
				kvp.Key,
				e.DPS,
				e.TotalDamage,
				e.Duration
			}), Array.Empty<GUILayoutOption>());
		}
		GUILayout.EndScrollView();
		GUI.DragWindow();
	}

	// Token: 0x04005CB5 RID: 23733
	private bool _visible;

	// Token: 0x04005CB6 RID: 23734
	private Rect _windowRect = new Rect(20f, 20f, 640f, 600f);

	// Token: 0x04005CB7 RID: 23735
	private Vector2 _scrollPos;

	// Token: 0x04005CB8 RID: 23736
	private bool _collapsed;

	// Token: 0x04005CB9 RID: 23737
	private string _filterText;

	// Token: 0x04005CD3 RID: 23763
	private int? _activeSessionIndex;

	// Token: 0x04005CD4 RID: 23764
	private Rect _popupRect;

	// Token: 0x04005CD5 RID: 23765
	private Vector2 _popupScroll;
}
