using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200058F RID: 1423
	public class MatchGroundAngle : GameEntityComponent
	{
		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x06002C74 RID: 11380 RVA: 0x0005ED5F File Offset: 0x0005CF5F
		// (set) Token: 0x06002C75 RID: 11381 RVA: 0x0005ED67 File Offset: 0x0005CF67
		public LayerMask layers
		{
			get
			{
				return this.m_layerMask;
			}
			set
			{
				this.m_layerMask = value;
			}
		}

		// Token: 0x06002C76 RID: 11382 RVA: 0x0005ED70 File Offset: 0x0005CF70
		private void Awake()
		{
			this.m_transform = base.gameObject.transform;
		}

		// Token: 0x06002C77 RID: 11383 RVA: 0x0005ED83 File Offset: 0x0005CF83
		private void Start()
		{
			if (base.GameEntity)
			{
				this.m_groundSampler = base.GameEntity.GroundSampler;
			}
		}

		// Token: 0x06002C78 RID: 11384 RVA: 0x00149D70 File Offset: 0x00147F70
		private void Update()
		{
			if (!this.m_transform || !this.m_groundSampler)
			{
				return;
			}
			this.m_groundSampler.SampleGround(ref this.m_layerMask);
			if (!this.m_terrainOnly || (this.m_groundSampler.LastHit.collider != null && this.m_groundSampler.LastHit.collider is TerrainCollider))
			{
				this.AlignWithNormal(this.m_groundSampler.LastHit.normal);
				return;
			}
			this.ResetRotation();
		}

		// Token: 0x06002C79 RID: 11385 RVA: 0x00149E08 File Offset: 0x00148008
		private void AlignWithNormal(Vector3 normal)
		{
			Quaternion rotation = this.m_transform.rotation;
			Quaternion b = Quaternion.FromToRotation(this.m_transform.up, normal) * rotation;
			this.m_transform.rotation = Quaternion.Slerp(rotation, b, Time.deltaTime * this.m_speed);
		}

		// Token: 0x06002C7A RID: 11386 RVA: 0x0005EDA3 File Offset: 0x0005CFA3
		public void ResetRotation()
		{
			this.m_transform.localRotation = Quaternion.identity;
		}

		// Token: 0x04002C30 RID: 11312
		[SerializeField]
		private float m_speed = 10f;

		// Token: 0x04002C31 RID: 11313
		[SerializeField]
		private bool m_terrainOnly;

		// Token: 0x04002C32 RID: 11314
		[SerializeField]
		private LayerMask m_layerMask = 1;

		// Token: 0x04002C33 RID: 11315
		private Transform m_transform;

		// Token: 0x04002C34 RID: 11316
		private GroundSampler m_groundSampler;
	}
}
