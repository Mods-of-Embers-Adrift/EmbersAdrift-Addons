using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SoL.Game.GM
{
	// Token: 0x02000E06 RID: 3590
	internal class DpsMeterOverlay : MonoBehaviour
	{
		// Token: 0x06006A54 RID: 27220 RVA: 0x00087495 File Offset: 0x00085695
		private void Start()
		{
			this.TryInitController();
			Debug.Log("[DPS Overlay] Initialized and listening for F2.");
		}

		// Token: 0x06006A55 RID: 27221 RVA: 0x0021AA60 File Offset: 0x00218C60
		private void OnGUI()
		{
			if (!DpsMeterOverlay._active || this._controller == null)
			{
				return;
			}
			this._dpsInfoList = this._controller.DpsInfoList;
			if (this._dpsInfoList == null)
			{
				return;
			}
			GUI.Box(new Rect(10f, 10f, 300f, 400f), "DPS Overlay");
			this._scroll = GUILayout.BeginScrollView(this._scroll, new GUILayoutOption[]
			{
				GUILayout.Width(290f),
				GUILayout.Height(370f)
			});
			foreach (DpsMeterInfo obj in this._dpsInfoList)
			{
				string privateField = this.GetPrivateField<string>(obj, "m_name");
				float[] privateField2 = this.GetPrivateField<float[]>(obj, "m_values");
				if (!string.IsNullOrEmpty(privateField) && privateField2 != null && privateField2.Length != 0)
				{
					GUILayout.Label(string.Format("{0}: {1:F1} DPS", privateField, privateField2[0]), Array.Empty<GUILayoutOption>());
				}
			}
			GUILayout.EndScrollView();
		}

		// Token: 0x06006A56 RID: 27222 RVA: 0x0021AB7C File Offset: 0x00218D7C
		private T GetPrivateField<T>(object obj, string fieldName)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (!(field != null))
			{
				return default(T);
			}
			return (T)((object)field.GetValue(obj));
		}

		// Token: 0x06006A58 RID: 27224 RVA: 0x000874A7 File Offset: 0x000856A7
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F2) && !DpsMeterOverlay._active)
			{
				Debug.Log("[DPS Overlay] F2 pressed — activating overlay.");
				DpsMeterOverlay._active = true;
				this.TryInitController();
			}
		}

		// Token: 0x06006A59 RID: 27225 RVA: 0x0021ABB8 File Offset: 0x00218DB8
		private void TryInitController()
		{
			if (this._controller == null)
			{
				this._controller = UnityEngine.Object.FindObjectOfType<DpsMeterController>();
				if (this._controller == null)
				{
					GameObject gameObject = new GameObject("UserDpsController");
					this._controller = gameObject.AddComponent<DpsMeterController>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				if (this._controller != null)
				{
					Debug.Log("[DPS Overlay] DpsMeterController ready.");
					DpsMeterOverlay._active = true;
				}
			}
		}

		// Token: 0x06006A5A RID: 27226 RVA: 0x000874D2 File Offset: 0x000856D2
		public static void Launch()
		{
			GameObject gameObject = new GameObject("DpsMeterOverlay");
			gameObject.AddComponent<DpsMeterOverlay>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		// Token: 0x04005CB1 RID: 23729
		private DpsMeterController _controller;

		// Token: 0x04005CB2 RID: 23730
		private List<DpsMeterInfo> _dpsInfoList;

		// Token: 0x04005CB3 RID: 23731
		private Vector2 _scroll;

		// Token: 0x04005CB4 RID: 23732
		private static bool _active;
	}
}
