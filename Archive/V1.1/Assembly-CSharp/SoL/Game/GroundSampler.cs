using System;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000581 RID: 1409
	public class GroundSampler : GameEntityComponent
	{
		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06002BF6 RID: 11254 RVA: 0x00147504 File Offset: 0x00145704
		// (remove) Token: 0x06002BF7 RID: 11255 RVA: 0x0014753C File Offset: 0x0014573C
		public event Action<Collider> WaterColliderChanged;

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x0005E82A File Offset: 0x0005CA2A
		// (set) Token: 0x06002BF9 RID: 11257 RVA: 0x0005E832 File Offset: 0x0005CA32
		public RaycastHit LastHit { get; private set; }

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x06002BFA RID: 11258 RVA: 0x0005E83B File Offset: 0x0005CA3B
		// (set) Token: 0x06002BFB RID: 11259 RVA: 0x0005E843 File Offset: 0x0005CA43
		public Vector3 SourcePos { get; private set; }

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06002BFC RID: 11260 RVA: 0x0005E84C File Offset: 0x0005CA4C
		// (set) Token: 0x06002BFD RID: 11261 RVA: 0x0005E854 File Offset: 0x0005CA54
		public bool IsUnderWater { get; private set; }

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06002BFE RID: 11262 RVA: 0x0005E85D File Offset: 0x0005CA5D
		// (set) Token: 0x06002BFF RID: 11263 RVA: 0x0005E865 File Offset: 0x0005CA65
		private Collider WaterCollider
		{
			get
			{
				return this.m_waterCollider;
			}
			set
			{
				if (this.m_waterCollider == value)
				{
					return;
				}
				this.m_waterCollider = value;
				Action<Collider> waterColliderChanged = this.WaterColliderChanged;
				if (waterColliderChanged == null)
				{
					return;
				}
				waterColliderChanged(this.m_waterCollider);
			}
		}

		// Token: 0x06002C00 RID: 11264 RVA: 0x0005E893 File Offset: 0x0005CA93
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.GroundSampler = this;
			}
		}

		// Token: 0x06002C01 RID: 11265 RVA: 0x0005E8AE File Offset: 0x0005CAAE
		public void MarkAsSelf()
		{
			this.m_sampleRate = 0.5f;
		}

		// Token: 0x06002C02 RID: 11266 RVA: 0x0005E8BB File Offset: 0x0005CABB
		public void TimeLimitedSampleGround()
		{
			if (Time.time - this.m_timeOfLastSample >= this.m_sampleRate)
			{
				this.SampleGround(ref GlobalSettings.Values.Ik.GroundLayers);
			}
		}

		// Token: 0x06002C03 RID: 11267 RVA: 0x00147574 File Offset: 0x00145774
		public void SampleGround(ref LayerMask layerMask)
		{
			if (Time.frameCount == this.m_frameOfLastSample)
			{
				return;
			}
			Vector3 vector = base.gameObject.transform.position + Vector3.up * 0.2f;
			RaycastHit lastHit;
			if (Physics.Raycast(vector, Vector3.down, out lastHit, 2f, layerMask, QueryTriggerInteraction.Ignore))
			{
				this.LastHit = lastHit;
				this.SourcePos = vector;
			}
			if (base.GameEntity && base.GameEntity.Type == GameEntityType.Player)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, Vector3.up, out raycastHit, 2f, LayerMap.CameraCollidePlayerIgnore.LayerMask, QueryTriggerInteraction.Ignore) && raycastHit.collider.gameObject.CompareTag("Water"))
				{
					this.IsUnderWater = true;
					this.WaterCollider = raycastHit.collider;
				}
				else
				{
					this.IsUnderWater = false;
					this.WaterCollider = null;
				}
			}
			this.m_timeOfLastSample = Time.time;
			this.m_frameOfLastSample = Time.frameCount;
		}

		// Token: 0x04002BC4 RID: 11204
		private const float kMaxSampleRate = 1f;

		// Token: 0x04002BC5 RID: 11205
		private const float kSelfMaxSampleRate = 0.5f;

		// Token: 0x04002BC6 RID: 11206
		private const float kMaxSampleDistance = 2f;

		// Token: 0x04002BC7 RID: 11207
		private float m_sampleRate = 1f;

		// Token: 0x04002BC8 RID: 11208
		private float m_timeOfLastSample;

		// Token: 0x04002BC9 RID: 11209
		private int m_frameOfLastSample;

		// Token: 0x04002BCD RID: 11213
		private Collider m_waterCollider;
	}
}
