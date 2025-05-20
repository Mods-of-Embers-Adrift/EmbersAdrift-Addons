using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001D8 RID: 472
	public class BGSceneLoader : MonoBehaviour
	{
		// Token: 0x060010B9 RID: 4281 RVA: 0x0004DEB5 File Offset: 0x0004C0B5
		private void Start()
		{
			if (this.IsMainMenu)
			{
				BGSceneLoader.Inited = true;
			}
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x0004DEC5 File Offset: 0x0004C0C5
		public void LoadScene(string scene)
		{
			SceneManager.LoadScene(scene);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000E0B24 File Offset: 0x000DED24
		private void OnGUI()
		{
			if (!BGSceneLoader.Inited || "BGCollidersMainMenu".Equals(SceneManager.GetActiveScene().name))
			{
				return;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("To Main Menu", Array.Empty<GUILayoutOption>()))
			{
				SceneManager.LoadScene("BGCollidersMainMenu");
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x04000E07 RID: 3591
		public static bool Inited;

		// Token: 0x04000E08 RID: 3592
		public bool IsMainMenu;
	}
}
