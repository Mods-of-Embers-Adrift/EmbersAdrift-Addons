using System;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x0200075A RID: 1882
	[CreateAssetMenu(menuName = "SoL/Scene Management/Startup Scene Config")]
	public class StartupSceneComposition : SceneComposition
	{
		// Token: 0x17000CC9 RID: 3273
		// (get) Token: 0x06003800 RID: 14336 RVA: 0x000662D1 File Offset: 0x000644D1
		public SceneReference ManagerScene
		{
			get
			{
				return this.m_managerScene;
			}
		}

		// Token: 0x040036E2 RID: 14050
		[SerializeField]
		private SceneReference m_managerScene;
	}
}
