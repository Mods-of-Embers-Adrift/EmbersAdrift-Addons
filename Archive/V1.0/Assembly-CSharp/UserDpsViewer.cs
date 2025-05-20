using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E07 RID: 3591
public class UserDpsViewer : MonoBehaviour
{
	// Token: 0x06006A5B RID: 27227 RVA: 0x000874EA File Offset: 0x000856EA
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
			if (this._visible && (this._windowRect.x < 0f || this._windowRect.x > (float)Screen.width || this._windowRect.y < 0f || this._windowRect.y > (float)Screen.height))
			{
				this._windowRect = new Rect(20f, 20f, 350f, 480f);
			}
		}
	}

	// Token: 0x06006A5D RID: 27229 RVA: 0x0021ABC8 File Offset: 0x00218DC8
	private void OnGUI()
	{
		if (!this._visible)
		{
			return;
		}
		GUIStyle style = new GUIStyle(GUI.skin.window)
		{
			fontSize = 14,
			padding = new RectOffset(10, 10, 20, 10)
		};
		this._windowRect = GUI.Window(911, this._windowRect, new GUI.WindowFunction(this.DrawDpsWindow), "DPS Overlay", style);
	}

	// Token: 0x06006A5E RID: 27230 RVA: 0x0021AC34 File Offset: 0x00218E34
	private void DrawDpsWindow(int windowId)
	{
		GUIStyle style = new GUIStyle(GUI.skin.label)
		{
			fontSize = 13,
			fontStyle = FontStyle.Normal
		};
		if (GUILayout.Button("Reset DPS", new GUILayoutOption[]
		{
			GUILayout.Height(28f)
		}))
		{
			UserDpsTracker.Reset();
		}
		GUILayout.Space(8f);
		GUILayout.Label("Name - DPS", new GUIStyle(GUI.skin.label)
		{
			fontStyle = FontStyle.Bold
		}, Array.Empty<GUILayoutOption>());
		Dictionary<string, float> currentDps = UserDpsTracker.CurrentDps;
		if (currentDps == null || currentDps.Count == 0)
		{
			GUILayout.Label("No DPS data available.", style, Array.Empty<GUILayoutOption>());
			GUI.DragWindow();
			return;
		}
		foreach (KeyValuePair<string, float> keyValuePair in currentDps)
		{
			GUILayout.Label(string.Format("{0,-20} {1,6:F1} DPS", keyValuePair.Key, keyValuePair.Value), style, Array.Empty<GUILayoutOption>());
		}
		GUI.DragWindow();
	}

	// Token: 0x04005CB5 RID: 23733
	private bool _visible = true;

	// Token: 0x04005CB6 RID: 23734
	private Rect _windowRect = new Rect(20f, 20f, 350f, 480f);
}
