using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000E07 RID: 3591
public class UserDpsViewer : MonoBehaviour
{
	// Token: 0x06006A5D RID: 27229 RVA: 0x00087504 File Offset: 0x00085704
	public static void Launch()
	{
		if (GameObject.Find("UserDpsViewer") == null)
		{
			GameObject gameObject = new GameObject("UserDpsViewer");
			UserDpsViewer.Instance = gameObject.AddComponent<UserDpsViewer>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	// Token: 0x06006A5E RID: 27230 RVA: 0x0021AED0 File Offset: 0x002190D0
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
				this._swingElapsed = 0f;
				this._swingActive = true;
				return;
			}
		}
		else if (this._swingActive)
		{
			this._swingElapsed += Time.deltaTime;
			if (this._swingElapsed >= this._swingDuration)
			{
				this._swingElapsed = this._swingDuration;
				this._swingActive = false;
				this._preSwingElapsed = 0f;
				this._preSwingActive = true;
			}
		}
	}

	// Token: 0x06006A5F RID: 27231
	private void OnGUI()
	{
		this.InitStyles();
		if (!this._visible)
		{
			return;
		}
		GUIStyle style = new GUIStyle(GUI.skin.window)
		{
			fontSize = 14,
			padding = new RectOffset(10, 10, 20, 10),
			normal = 
			{
				textColor = Color.white
			}
		};
		GUI.backgroundColor = new Color(0.2f, 0.2f, 0.3f, 1f);
		this._hubRect = GUI.Window(1000, this._hubRect, new GUI.WindowFunction(this.DrawHubWindow), "<b>Addon Overlay</b>", style);
		if (this._showDpsWindow)
		{
			this._dpsRect.width = 380f;
			this._dpsRect.height = 360f;
			this._dpsRect = GUI.Window(1001, this._dpsRect, new GUI.WindowFunction(this.DrawDpsWindow), "Live DPS/HPS", style);
		}
		if (this._showHistoryWindow)
		{
			this._historyRect = GUI.Window(1002, this._historyRect, new GUI.WindowFunction(this.DrawFightHistoryWindow), "Fight History", style);
		}
		if (this._showSwingTimer)
		{
			GUI.Window(1004, new Rect(1020f, 650f, 300f, 100f), delegate(int id)
			{
				GUILayout.Label("Swing Timer", this._headerStyle, Array.Empty<GUILayoutOption>());
				if (this._swingActive)
				{
					float num = Mathf.Clamp01(this._swingElapsed / this._swingDuration);
					Rect rect = GUILayoutUtility.GetRect(280f, 20f);
					this._backgroundTex = Texture2D.whiteTexture;
					GUI.color = Color.gray;
					GUI.DrawTexture(rect, this._backgroundTex);
					GUI.color = Color.green;
					GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * num, rect.height), this._backgroundTex);
					GUI.color = Color.white;
					GUILayout.Label(string.Format("Swing in progress: {0:F1}s", this._swingDuration - this._swingElapsed), this._labelStyle, Array.Empty<GUILayoutOption>());
				}
				else if (this._preSwingActive)
				{
					float num2 = Mathf.Clamp01(this._preSwingElapsed / this._preSwingDuration);
					GUILayout.Label(string.Format("Next swing in: {0:F1}s", this._preSwingDuration - this._preSwingElapsed), this._labelStyle, Array.Empty<GUILayoutOption>());
					Rect rect2 = GUILayoutUtility.GetRect(280f, 20f);
					this._backgroundTex = Texture2D.whiteTexture;
					GUI.color = Color.gray;
					GUI.DrawTexture(rect2, this._backgroundTex);
					GUI.color = Color.yellow;
					GUI.DrawTexture(new Rect(rect2.x, rect2.y, rect2.width * num2, rect2.height), this._backgroundTex);
					GUI.color = Color.white;
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
			}, "Swing Timer", style);
		}
		if (this._showThreatWindow)
		{
			this._threatRect.width = 380f;
			this._threatRect.height = 360f;
			this._threatRect = GUI.Window(1005, this._threatRect, new GUI.WindowFunction(this.DrawThreatWindow), "Threat Meter", style);
		}
	}

	// Token: 0x06006A60 RID: 27232 RVA: 0x0021B100 File Offset: 0x00219300
	public UserDpsViewer()
	{
		this._visible = true;
		this._hubRect = new Rect(20f, 20f, 400f, 300f);
		this._dpsRect = new Rect(440f, 20f, 640f, 600f);
		this._historyRect = new Rect(1100f, 20f, 520f, 600f);
	}

	// Token: 0x06006A61 RID: 27233 RVA: 0x0021B198 File Offset: 0x00219398
	private void DrawHubWindow(int id)
	{
		GUILayout.Label("Welcome to your addon hub!", this._headerStyle, Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Show Live DPS", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showDpsWindow = !this._showDpsWindow;
		}
		if (GUILayout.Button("Swing Timer", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showSwingTimer = !this._showSwingTimer;
		}
		if (GUILayout.Button("Threat Meter", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showThreatWindow = !this._showThreatWindow;
		}
		GUILayout.Space(20f);
		GUILayout.Label("Shortcuts:", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F2 - Toggle Hub UI", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label("Created by: <b>MrJambix</b>", this._labelStyle, Array.Empty<GUILayoutOption>());
		GUI.DragWindow();
	}

	// Token: 0x06006A62 RID: 27234 RVA: 0x0021B280 File Offset: 0x00219480
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

	// Token: 0x06006A63 RID: 27235 RVA: 0x00087532 File Offset: 0x00085732
	public void StartSwingTimer(float duration)
	{
		this._swingDuration = duration;
		this._swingElapsed = 0f;
		this._swingActive = true;
	}

	// Token: 0x06006A64 RID: 27236 RVA: 0x0021B354 File Offset: 0x00219554
	private void DrawFightHistoryWindow(int id)
	{
		IReadOnlyList<UserDpsTracker.FightSession> pastFights = UserDpsTracker.PastFights;
		this._historyScroll = GUILayout.BeginScrollView(this._historyScroll, new GUILayoutOption[]
		{
			GUILayout.Height(400f)
		});
		if (pastFights != null)
		{
			for (int i = 0; i < pastFights.Count; i++)
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
			foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> keyValuePair in pastFights[this._activeSessionIndex.Value].Entries)
			{
				GUILayout.Label(string.Format("{0,-18}  {1,6:F1}    {2,10:F0}    {3,8:F1}", new object[]
				{
					keyValuePair.Key,
					keyValuePair.Value.DPS,
					keyValuePair.Value.TotalDamage,
					keyValuePair.Value.Duration
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

	// Token: 0x06006A65 RID: 27237 RVA: 0x0008754D File Offset: 0x0008574D
	private IEnumerable<KeyValuePair<string, float>> SortedByDps(Dictionary<string, float> dpsDict)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>(dpsDict);
		list.Sort((KeyValuePair<string, float> a, KeyValuePair<string, float> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	// Token: 0x06006A66 RID: 27238
	private void DrawDpsWindow(int id)
	{
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Toggle(!this._showHps, "DPS", this._buttonStyle, new GUILayoutOption[]
		{
			GUILayout.Width(60f)
		}))
		{
			this._showHps = false;
		}
		if (GUILayout.Toggle(this._showHps, "HPS", this._buttonStyle, new GUILayoutOption[]
		{
			GUILayout.Width(60f)
		}))
		{
			this._showHps = true;
		}
		GUILayout.Space(20f);
		if (GUILayout.Button("Close", this._buttonStyle, new GUILayoutOption[]
		{
			GUILayout.Width(60f)
		}))
		{
			this._showDpsWindow = false;
		}
		if (GUILayout.Button("History", this._buttonStyle, new GUILayoutOption[]
		{
			GUILayout.Width(70f)
		}))
		{
			this._showHistoryWindow = !this._showHistoryWindow;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("<b>Name</b>", this._labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(140f)
		});
		GUILayout.Label(this._showHps ? "<b>HPS</b>" : "<b>DPS</b>", this._labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(60f)
		});
		GUILayout.Label("<b>Total</b>", this._labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(80f)
		});
		GUILayout.Label("<b>Duration</b>", this._labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(80f)
		});
		GUILayout.EndHorizontal();
		Dictionary<string, float> currentStats = this._showHps ? UserDpsTracker.CurrentHps : UserDpsTracker.CurrentDps;
		IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> entries = this._showHps ? UserDpsTracker.HealingEntries : UserDpsTracker.Entries;
		if (currentStats != null && entries != null)
		{
			float maxStat = (currentStats.Values.Count > 0) ? currentStats.Values.Max() : 1f;
			this._threatScroll = GUILayout.BeginScrollView(this._threatScroll, new GUILayoutOption[]
			{
				GUILayout.Height(260f)
			});
			foreach (KeyValuePair<string, float> kvp in this.SortedByDps(currentStats))
			{
				UserDpsTracker.DpsEntry entry;
				if (entries.TryGetValue(kvp.Key, out entry))
				{
					float value = this._showHps ? entry.HPS : entry.DPS;
					float barFill = Mathf.Clamp01(value / maxStat);
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label(kvp.Key, this._labelStyle, new GUILayoutOption[]
					{
						GUILayout.Width(140f)
					});
					GUILayout.Label(value.ToString("F1"), this._labelStyle, new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					});
					GUILayout.Label((this._showHps ? entry.TotalHealing : entry.TotalDamage).ToString("F0"), this._labelStyle, new GUILayoutOption[]
					{
						GUILayout.Width(80f)
					});
					GUILayout.Label(entry.Duration.ToString("F1"), this._labelStyle, new GUILayoutOption[]
					{
						GUILayout.Width(80f)
					});
					GUILayout.EndHorizontal();
					Rect barRect = GUILayoutUtility.GetRect(360f, 10f);
					GUI.color = Color.black;
					GUI.DrawTexture(barRect, Texture2D.whiteTexture);
					GUI.color = (this._showHps ? Color.cyan : Color.green);
					GUI.DrawTexture(new Rect(barRect.x, barRect.y, barRect.width * barFill, barRect.height), Texture2D.whiteTexture);
					GUI.color = Color.white;
					GUILayout.Space(4f);
				}
			}
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label("No data available.", this._labelStyle, Array.Empty<GUILayoutOption>());
		}
		GUI.DragWindow();
	}

	// Token: 0x17001952 RID: 6482
	// (get) Token: 0x06006A67 RID: 27239 RVA: 0x0008757A File Offset: 0x0008577A
	// (set) Token: 0x06006A68 RID: 27240 RVA: 0x00087581 File Offset: 0x00085781
	public static UserDpsViewer Instance { get; private set; }

	// Token: 0x06006A69 RID: 27241 RVA: 0x00087589 File Offset: 0x00085789
	public void StartPreSwingTimer(float delay)
	{
		this._preSwingDuration = delay;
		this._preSwingElapsed = 0f;
		this._preSwingActive = true;
		this._showSwingTimer = true;
	}

	// Token: 0x06006A6A RID: 27242 RVA: 0x0021B668 File Offset: 0x00219868
	private void DrawThreatWindow(int id)
	{
		IReadOnlyDictionary<string, float> currentThreat = UserThreatTracker.CurrentThreat;
		GUILayout.Label("<b>Name                     Threat %</b>", this._labelStyle, Array.Empty<GUILayoutOption>());
		if (currentThreat != null && currentThreat.Count > 0)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (KeyValuePair<string, float> keyValuePair in currentThreat)
			{
				if (this.IsPlayerName(keyValuePair.Key))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (dictionary.Count > 0)
			{
				float num = 0f;
				foreach (float num2 in dictionary.Values)
				{
					if (num2 > num)
					{
						num = num2;
					}
				}
				GUILayout.BeginScrollView(Vector2.zero, new GUILayoutOption[]
				{
					GUILayout.Height(400f)
				});
				foreach (KeyValuePair<string, float> keyValuePair2 in this.SortedByDps(dictionary))
				{
					float num3 = (num > 0f) ? (keyValuePair2.Value / num * 100f) : 0f;
					GUILayout.Label(string.Format("{0,-18}  {1,8:F1}%", keyValuePair2.Key, num3), this._labelStyle, Array.Empty<GUILayoutOption>());
				}
				GUILayout.EndScrollView();
			}
			else
			{
				GUILayout.Label("No player threat data available.", this._labelStyle, Array.Empty<GUILayoutOption>());
			}
		}
		else
		{
			GUILayout.Label("No threat data available.", this._labelStyle, Array.Empty<GUILayoutOption>());
		}
		if (GUILayout.Button("Close", this._buttonStyle, Array.Empty<GUILayoutOption>()))
		{
			this._showThreatWindow = false;
		}
		GUI.DragWindow();
	}

	// Token: 0x06006A6B RID: 27243 RVA: 0x000875AB File Offset: 0x000857AB
	private IEnumerable<KeyValuePair<string, float>> SortedByDps(IReadOnlyDictionary<string, float> dpsDict)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>(dpsDict);
		list.Sort((KeyValuePair<string, float> a, KeyValuePair<string, float> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	// Token: 0x06006A6E RID: 27246 RVA: 0x000875D8 File Offset: 0x000857D8
	private bool IsPlayerName(string name)
	{
		return !string.IsNullOrEmpty(name) && char.IsUpper(name[0]) && !name.Contains("NPC") && name.Length >= 3;
	}

	// Token: 0x04005CB6 RID: 23734
	private bool _visible;

	// Token: 0x04005CB7 RID: 23735
	private Rect _hubRect;

	// Token: 0x04005CB8 RID: 23736
	private bool _showSwingTimer;

	// Token: 0x04005CB9 RID: 23737
	private GUIStyle _buttonStyle;

	// Token: 0x04005CBA RID: 23738
	private GUIStyle _labelStyle;

	// Token: 0x04005CBB RID: 23739
	private GUIStyle _headerStyle;

	// Token: 0x04005CBC RID: 23740
	private float _swingElapsed;

	// Token: 0x04005CBD RID: 23741
	private float _swingDuration;

	// Token: 0x04005CBE RID: 23742
	private bool _showDpsWindow;

	// Token: 0x04005CBF RID: 23743
	private bool _showHistoryWindow;

	// Token: 0x04005CC0 RID: 23744
	private int? _activeSessionIndex;

	// Token: 0x04005CC1 RID: 23745
	private Vector2 _popupScroll;

	// Token: 0x04005CC2 RID: 23746
	private Vector2 _historyScroll;

	// Token: 0x04005CC3 RID: 23747
	private Rect _dpsRect;

	// Token: 0x04005CC4 RID: 23748
	private Rect _historyRect;

	// Token: 0x04005CC5 RID: 23749
	private bool _swingActive;

	// Token: 0x04005CC6 RID: 23750
	private Texture2D _backgroundTex;

	// Token: 0x04005CC8 RID: 23752
	private float _preSwingDuration;

	// Token: 0x04005CC9 RID: 23753
	private float _preSwingElapsed;

	// Token: 0x04005CCA RID: 23754
	private bool _preSwingActive;

	// Token: 0x04005CCB RID: 23755
	private bool _showThreatWindow;

	// Token: 0x04005CCC RID: 23756
	private Rect _threatRect = new Rect(440f, 640f, 640f, 600f);

	// Token: 0x04005CCD RID: 23757
	private Vector2 _threatScroll;

	// Token: 0x04005D44 RID: 23876
	private bool _showHps;
}
