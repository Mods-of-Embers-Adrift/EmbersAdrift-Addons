using System;
using JBooth.MicroSplat;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200026B RID: 619
	public class CustomMicroSplatBlendableObject : MicroSplatBlendableObject
	{
		// Token: 0x0600139E RID: 5022 RVA: 0x0004FCEC File Offset: 0x0004DEEC
		protected override void OnEnable()
		{
			this.AssignTerrainInternal();
			base.OnEnable();
			if (!this.msObject && Application.isPlaying)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x000F72B8 File Offset: 0x000F54B8
		private void AssignTerrainInternal()
		{
			if (this.msObject)
			{
				return;
			}
			RaycastHit[] hits = Hits.Hits25;
			int num = Physics.RaycastNonAlloc(base.gameObject.transform.position + Vector3.up * 5f, Vector3.down, hits, 20f);
			for (int i = 0; i < num; i++)
			{
				TerrainCollider terrainCollider = hits[i].collider as TerrainCollider;
				if (terrainCollider)
				{
					MicroSplatTerrain component = terrainCollider.gameObject.GetComponent<MicroSplatTerrain>();
					if (component != null)
					{
						this.msObject = component;
						return;
					}
				}
			}
		}
	}
}
