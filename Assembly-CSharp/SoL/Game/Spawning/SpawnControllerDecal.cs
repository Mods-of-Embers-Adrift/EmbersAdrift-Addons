using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.Spawning
{
	// Token: 0x020006C8 RID: 1736
	[ExecuteInEditMode]
	public class SpawnControllerDecal : MonoBehaviour
	{
		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x060034D5 RID: 13525 RVA: 0x000642D2 File Offset: 0x000624D2
		// (set) Token: 0x060034D6 RID: 13526 RVA: 0x000642DA File Offset: 0x000624DA
		public SpawnController Controller
		{
			get
			{
				return this.m_controller;
			}
			set
			{
				this.m_controller = value;
			}
		}

		// Token: 0x060034D7 RID: 13527 RVA: 0x000642E3 File Offset: 0x000624E3
		private void Awake()
		{
			if (Application.isPlaying && GameManager.IsServer && GameManager.IsOnline)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x0400330A RID: 13066
		[SerializeField]
		private SpawnController m_controller;

		// Token: 0x0400330B RID: 13067
		[SerializeField]
		private DecalProjector m_decalProjector;

		// Token: 0x0400330C RID: 13068
		[SerializeField]
		private SpawnControllerDecal.MaterialPair[] m_materialPairs;

		// Token: 0x020006C9 RID: 1737
		[Serializable]
		private class MaterialPair
		{
			// Token: 0x17000B95 RID: 2965
			// (get) Token: 0x060034D9 RID: 13529 RVA: 0x00064306 File Offset: 0x00062506
			internal SpawnController.DecalColorType DecalColor
			{
				get
				{
					return this.m_colorType;
				}
			}

			// Token: 0x060034DA RID: 13530 RVA: 0x0006430E File Offset: 0x0006250E
			public Material GetMaterial(SpawnController.AreaShape shape)
			{
				if (shape != SpawnController.AreaShape.Cube)
				{
					return this.m_sphereMaterial;
				}
				return this.m_cubeMaterial;
			}

			// Token: 0x0400330D RID: 13069
			[SerializeField]
			private SpawnController.DecalColorType m_colorType = SpawnController.DecalColorType.Color1;

			// Token: 0x0400330E RID: 13070
			[SerializeField]
			private Material m_cubeMaterial;

			// Token: 0x0400330F RID: 13071
			[SerializeField]
			private Material m_sphereMaterial;
		}
	}
}
