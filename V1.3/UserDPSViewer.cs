using System;
using System.Collections.Generic;
using System.Linq;
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
			UserDpsViewer.Instance = gameObject.AddComponent<UserDpsViewer>();
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
		if (this._preSwingActive)
		{
			this._preSwingElapsed += Time.deltaTime;
			if (this._preSwingElapsed >= this._preSwingDuration)
			{
				this._preSwingElapsed = this._preSwingDuration;
				this._preSwingActive = false;
			}
		}
		if (this._swingActive)
		{
			this._swingElapsed += Time.deltaTime;
			if (this._swingElapsed >= this._swingDuration)
			{
				this._swingElapsed = this._swingDuration;
				this._swingActive = false;
			}
		}
	}

	// Token: 0x06006A5D RID: 27229
	private void OnGUI()
	{
		this.InitStyles();
		if (!this._visible)
		{
			return;
		}
		GUIStyle windowStyle = new GUIStyle(GUI.skin.window)
		{
			fontSize = 14,
			padding = new RectOffset(10, 10, 20, 10),
			normal = 
			{
				textColor = Color.white
			}
		};
		GUI.backgroundColor = new Color(0.2f, 0.2f, 0.3f, 1f);
		this._hubRect = GUI.Window(1000, this._hubRect, new GUI.WindowFunction(this.DrawHubWindow), "<b>Addon Overlay</b>", windowStyle);
		if (this._showDpsWindow)
		{
			this._dpsRect = GUI.Window(1001, this._dpsRect, new GUI.WindowFunction(this.DrawDpsWindow), "Live DPS", windowStyle);
		}
		if (this._showHistoryWindow)
		{
			this._historyRect = GUI.Window(1002, this._historyRect, new GUI.WindowFunction(this.DrawFightHistoryWindow), "Fight History", windowStyle);
		}
		if (this._showThreatMeter)
		{
			GUI.Window(1003, new Rect(600f, 650f, 400f, 300f), delegate(int id)
			{
				GUILayout.Label("Threat Meter", this._headerStyle, Array.Empty<GUILayoutOption>());
				Dictionary<string, float> threatData = UserDpsTracker.CurrentThreat;
				if (threatData != null && threatData.Count > 0)
				{
					float maxThreat = threatData.Values.Max();
					using (IEnumerator<KeyValuePair<string, float>> enumerator = this.SortedByDps(threatData).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, float> pair = enumerator.Current;
							float percent = pair.Value / maxThreat;
							GUILayout.Label(string.Format("{0}: {1:F1} ({2:F1}%)", pair.Key, pair.Value, percent * 100f), this._labelStyle, Array.Empty<GUILayoutOption>());
							Rect lastRect = GUILayoutUtility.GetLastRect();
							if (this._backgroundTex == null)
							{
								this._backgroundTex = Texture2D.whiteTexture;
							}
							Color color = GUI.color;
							GUI.color = Color.gray;
							GUI.DrawTexture(lastRect, this._backgroundTex);
							GUI.color = Color.green;
							GUI.DrawTexture(new Rect(lastRect.x, lastRect.y, lastRect.width * percent, 6f), this._backgroundTex);
							GUI.color = color;
							GUILayout.Space(10f);
						}
						goto IL_145;
					}
				}
				GUILayout.Label("No threat data.", this._labelStyle, Array.Empty<GUILayoutOption>());
				IL_145:
				if (GUILayout.Button("Close", this._buttonStyle, Array.Empty<GUILayoutOption>()))
				{
					this._showThreatMeter = false;
				}
				GUI.DragWindow();
			}, "Threat Meter", windowStyle);
		}
		if (this._showSwingTimer)
		{
			GUI.Window(1004, new Rect(1020f, 650f, 300f, 100f), delegate(int id)
			{
				GUILayout.Label("Swing Timer", this._headerStyle, Array.Empty<GUILayoutOption>());
				if (this._swingActive)
				{
					float percent = Mathf.Clamp01(this._swingElapsed / this._swingDuration);
					Rect rect = GUILayoutUtility.GetRect(280f, 20f);
					if (this._backgroundTex == null)
					{
						this._backgroundTex = Texture2D.whiteTexture;
					}
					Color color = GUI.color;
					GUI.color = Color.gray;
					GUI.DrawTexture(rect, this._backgroundTex);
					GUI.color = Color.green;
					GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * percent, rect.height), this._backgroundTex);
					GUI.color = color;
				}
				else if (this._preSwingActive)
				{
					float remaining = Mathf.Max(0f, this._preSwingDuration - this._preSwingElapsed);
					GUILayout.Label(string.Format("Next swing in: {0:F1}s", remaining), this._labelStyle, Array.Empty<GUILayoutOption>());
				}
				else
				{
					GUILayout.Label("Waiting for swing...", this._labelStyle, Array.Empty<GUILayoutOption>());
				}
				if (GUILayout.Button("Close", this._buttonStyle, Array.Empty<GUILayoutOption>()))
				{
					this._showSwingTimer = false;
				}
				GUI.DragWindow();
			}, "Swing Timer", windowStyle);
		}
	}

	// Token: 0x06006A7C RID: 27260
	private void DrawHubWindow(int id)
	{
		GUILayout.Label("Welcome to your addon hub!", this._headerStyle, Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Show Live DPS", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showDpsWindow = !this._showDpsWindow;
		}
		if (GUILayout.Button("Threat Meter", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showThreatMeter = !this._showThreatMeter;
		}
		if (GUILayout.Button("Swing Timer", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showSwingTimer = !this._showSwingTimer;
		}
		GUILayout.Space(20f);
		GUILayout.Label("Shortcuts:", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F2 - Toggle Hub UI", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label("Created by: <b>MrJambix</b>", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUI.DragWindow();
	}

	// Token: 0x06006A8C RID: 27276
	private void InitStyles()
	{
		if (this._buttonStyle == null)
		{
			this._buttonStyle = new GUIStyle(GUI.skin.button)
			{
				fontSize = 14,
				normal = 
				{
					textColor = Color.white
				},
				hover = 
				{
					textColor = Color.yellow
				},
				fontStyle = FontStyle.Bold
			};
			this._labelStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				normal = 
				{
					textColor = Color.cyan
				},
				richText = true
			};
			this._headerStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 16,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				normal = 
				{
					textColor = Color.green
				},
				richText = true
			};
		}
	}

	// Token: 0x06006B4C RID: 27468
	public void StartSwingTimer(float duration)
	{
		this._swingDuration = duration;
		this._swingElapsed = 0f;
		this._swingActive = true;
	}

	// Token: 0x06006B59 RID: 27481
	private void DrawFightHistoryWindow(int id)
	{
		IReadOnlyList<UserDpsTracker.FightSession> fights = UserDpsTracker.PastFights;
		this._historyScroll = GUILayout.BeginScrollView(this._historyScroll, new GUILayoutOption[]
		{
			GUILayout.Height(400f)
		});
		if (fights != null)
		{
			for (int i = 0; i < fights.Count; i++)
			{
				if (GUILayout.Button(string.Format("View: Fight Session #{0}", i + 1), this._buttonStyle, Array.Empty<GUILayoutOption>()))
				{
					this._activeSessionIndex = new int?(i);
				}
			}
		}
		GUILayout.EndScrollView();
		if (this._activeSessionIndex != null)
		{
			GUILayout.Label("--- Fight Session Details ---", this._headerStyle, Array.Empty<GUILayoutOption>());
			this._popupScroll = GUILayout.BeginScrollView(this._popupScroll, new GUILayoutOption[]
			{
				GUILayout.Height(300f)
			});
			foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> entry in fights[this._activeSessionIndex.Value].Entries)
			{
				GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
				{
					entry.Key,
					entry.Value.DPS,
					entry.Value.TotalDamage,
					entry.Value.Duration
				}), this._labelStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndScrollView();
		}
		if (GUILayout.Button("Close", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showHistoryWindow = false;
			this._activeSessionIndex = null;
		}
		GUI.DragWindow();
	}

	// Token: 0x06006B5A RID: 27482
	private IEnumerable<KeyValuePair<string, float>> SortedByDps(Dictionary<string, float> dpsDict)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>(dpsDict);
		list.Sort((KeyValuePair<string, float> a, KeyValuePair<string, float> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	// Token: 0x06006B74 RID: 27508
	private void DrawDpsWindow(int id)
	{
		Dictionary<string, float> dpsData = UserDpsTracker.CurrentDps;
		IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> entries = UserDpsTracker.Entries;
		GUILayout.Label("<b>Name                     DPS     Total Dmg   Duration (s)</b>", this._labelStyle, Array.Empty<GUILayoutOption>());
		if (dpsData != null && entries != null)
		{
			GUILayout.BeginScrollView(Vector2.zero, new GUILayoutOption[]
			{
				GUILayout.Height(400f)
			});
			foreach (KeyValuePair<string, float> pair in this.SortedByDps(dpsData))
			{
				UserDpsTracker.DpsEntry entry;
				if (entries.TryGetValue(pair.Key, out entry))
				{
					GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
					{
						pair.Key,
						entry.DPS,
						entry.TotalDamage,
						entry.Duration
					}), this._labelStyle, Array.Empty<GUILayoutOption>());
				}
			}
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label("No DPS data available.", this._labelStyle, Array.Empty<GUILayoutOption>());
		}
		if (GUILayout.Button("Close", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showDpsWindow = false;
		}
		if (GUILayout.Button("Fight History", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showHistoryWindow = !this._showHistoryWindow;
		}
		GUI.DragWindow();
	}

	// Token: 0x1700195E RID: 6494
	// (get) Token: 0x06006B7F RID: 27519
	// (set) Token: 0x06006B80 RID: 27520
	public static UserDpsViewer Instance { get; private set; }

	// Token: 0x06006BEA RID: 27626
	public void StartPreSwingTimer(float delay)
	{
		this._preSwingDuration = delay;
		this._preSwingElapsed = 0f;
		this._preSwingActive = true;
	}

	// Token: 0x04005CB5 RID: 23733
	private bool _visible = true;

	// Token: 0x04005CCF RID: 23759
	private Rect _hubRect = new Rect(20f, 20f, 400f, 300f);

	// Token: 0x04005CDB RID: 23771
	private bool _showSwingTimer;

	// Token: 0x04005CE2 RID: 23778
	private GUIStyle _buttonStyle;

	// Token: 0x04005CE3 RID: 23779
	private GUIStyle _labelStyle;

	// Token: 0x04005CE4 RID: 23780
	private GUIStyle _headerStyle;

	// Token: 0x04005DAC RID: 23980
	private float _swingElapsed;

	// Token: 0x04005DAD RID: 23981
	private float _swingDuration;

	// Token: 0x04005DC2 RID: 24002
	private bool _showDpsWindow;

	// Token: 0x04005DC3 RID: 24003
	private bool _showHistoryWindow;

	// Token: 0x04005DC4 RID: 24004
	private bool _showThreatMeter;

	// Token: 0x04005DC6 RID: 24006
	private int? _activeSessionIndex;

	// Token: 0x04005DC7 RID: 24007
	private Vector2 _popupScroll;

	// Token: 0x04005DC8 RID: 24008
	private Vector2 _historyScroll;

	// Token: 0x04005DCA RID: 24010
	private Rect _dpsRect = new Rect(440f, 20f, 640f, 600f);

	// Token: 0x04005DCB RID: 24011
	private Rect _historyRect = new Rect(1100f, 20f, 520f, 600f);

	// Token: 0x04005DCF RID: 24015
	private bool _swingActive;

	// Token: 0x04005DD2 RID: 24018
	private Texture2D _backgroundTex;

	// Token: 0x04005E42 RID: 24130
	private float _preSwingDuration;

	// Token: 0x04005E43 RID: 24131
	private float _preSwingElapsed;

	// Token: 0x04005E44 RID: 24132
	private bool _preSwingActive;
}
