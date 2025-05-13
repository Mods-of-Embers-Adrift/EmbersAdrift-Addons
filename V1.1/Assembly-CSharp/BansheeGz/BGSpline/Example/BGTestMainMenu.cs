using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001F6 RID: 502
	public class BGTestMainMenu : MonoBehaviour
	{
		// Token: 0x0600115D RID: 4445 RVA: 0x0004E667 File Offset: 0x0004C867
		private void Start()
		{
			BGTestMainMenu.Inited = true;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0004DEC5 File Offset: 0x0004C0C5
		public void LoadScene(string scene)
		{
			SceneManager.LoadScene(scene);
		}

		// Token: 0x04000EC3 RID: 3779
		public static bool Inited;
	}
}
