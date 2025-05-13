using System;
using SoL.Game.Randomization;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007F4 RID: 2036
	public class AlternateTargetPoints : GameEntityComponent
	{
		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x06003B32 RID: 15154 RVA: 0x00068178 File Offset: 0x00066378
		public GameObject[] Points
		{
			get
			{
				return this.m_points;
			}
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x0017ABE4 File Offset: 0x00178DE4
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.AlternateTargetPoints = this;
				NpcScaleAdjuster npcScaleAdjuster = base.gameObject.AddComponent<NpcScaleAdjuster>();
				if (npcScaleAdjuster)
				{
					npcScaleAdjuster.SetVarsExternal(true, true);
				}
			}
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x0017AC28 File Offset: 0x00178E28
		private void OnDrawGizmosSelected()
		{
			if (this.m_points == null)
			{
				return;
			}
			foreach (GameObject gameObject in this.m_points)
			{
				if (gameObject && gameObject.transform)
				{
					Gizmos.DrawSphere(gameObject.transform.position, 0.25f);
				}
			}
		}

		// Token: 0x040039A5 RID: 14757
		[SerializeField]
		private GameObject[] m_points;
	}
}
