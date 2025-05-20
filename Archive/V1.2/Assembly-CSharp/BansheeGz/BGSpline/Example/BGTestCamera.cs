using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001D9 RID: 473
	public class BGTestCamera : MonoBehaviour
	{
		// Token: 0x060010BD RID: 4285 RVA: 0x000E0B7C File Offset: 0x000DED7C
		private void Update()
		{
			if (Input.GetKey(KeyCode.A))
			{
				base.transform.RotateAround(Vector3.zero, Vector3.up, 100f * Time.deltaTime);
				return;
			}
			if (Input.GetKey(KeyCode.D))
			{
				base.transform.RotateAround(Vector3.zero, Vector3.up, -100f * Time.deltaTime);
			}
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x000E0BDC File Offset: 0x000DEDDC
		private void OnGUI()
		{
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label)
				{
					fontSize = 24
				};
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Use A and D to rotate camera", this.style, Array.Empty<GUILayoutOption>());
			if (BGTestMainMenu.Inited && GUILayout.Button("To Main Menu", Array.Empty<GUILayoutOption>()))
			{
				SceneManager.LoadScene("BGCurveMainMenu");
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x04000E09 RID: 3593
		private const int Speed = 100;

		// Token: 0x04000E0A RID: 3594
		private GUIStyle style;
	}
}
