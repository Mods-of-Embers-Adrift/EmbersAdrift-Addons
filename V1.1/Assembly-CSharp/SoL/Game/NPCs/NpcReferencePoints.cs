using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000811 RID: 2065
	public class NpcReferencePoints : GameEntityComponent
	{
		// Token: 0x17000DC1 RID: 3521
		// (get) Token: 0x06003BD0 RID: 15312 RVA: 0x0006876D File Offset: 0x0006696D
		public GameObject Overhead
		{
			get
			{
				return this.m_referencePoints.Overhead;
			}
		}

		// Token: 0x17000DC2 RID: 3522
		// (get) Token: 0x06003BD1 RID: 15313 RVA: 0x0006877A File Offset: 0x0006697A
		public float DamageTargetToSourceOffset
		{
			get
			{
				return this.m_damageTargetToSourceOffset;
			}
		}

		// Token: 0x06003BD2 RID: 15314 RVA: 0x00068782 File Offset: 0x00066982
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.NpcReferencePoints = this;
			}
		}

		// Token: 0x06003BD3 RID: 15315 RVA: 0x0017CE48 File Offset: 0x0017B048
		private void Start()
		{
			if (!GameManager.IsServer && base.GameEntity != null && base.GameEntity.CharacterData != null && base.GameEntity.CharacterData.ReferencePoints == null)
			{
				base.GameEntity.CharacterData.ReferencePoints = new HumanoidReferencePoints?(this.m_referencePoints);
				if (this.m_referencePoints.Overhead)
				{
					base.GameEntity.OverheadReference = this.m_referencePoints.Overhead.transform;
				}
			}
		}

		// Token: 0x06003BD4 RID: 15316 RVA: 0x0017CEE4 File Offset: 0x0017B0E4
		public GameObject GetDamageTargetForFx(GameEntity source)
		{
			if (!source || this.m_additionalDamageTargets == null || this.m_additionalDamageTargets.Length == 0)
			{
				return this.m_referencePoints.DamageTarget;
			}
			GameObject gameObject = null;
			float num = float.MaxValue;
			Vector3 a = (source.CharacterData && source.CharacterData.ReferencePoints != null && source.CharacterData.ReferencePoints.Value.DamageTarget) ? source.CharacterData.ReferencePoints.Value.DamageTarget.transform.position : source.gameObject.transform.position;
			if (this.m_referencePoints.DamageTarget)
			{
				gameObject = this.m_referencePoints.DamageTarget;
				num = (a - gameObject.transform.position).sqrMagnitude;
			}
			for (int i = 0; i < this.m_additionalDamageTargets.Length; i++)
			{
				if (this.m_additionalDamageTargets[i])
				{
					float sqrMagnitude = (a - this.m_additionalDamageTargets[i].transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						gameObject = this.m_additionalDamageTargets[i];
						num = sqrMagnitude;
					}
				}
			}
			return gameObject;
		}

		// Token: 0x04003A7F RID: 14975
		[SerializeField]
		private HumanoidReferencePoints m_referencePoints;

		// Token: 0x04003A80 RID: 14976
		[SerializeField]
		private float m_damageTargetToSourceOffset = 0.2f;

		// Token: 0x04003A81 RID: 14977
		[SerializeField]
		private GameObject[] m_additionalDamageTargets;
	}
}
