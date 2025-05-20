using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000590 RID: 1424
	public class MatchGroundHeight : GameEntityComponent
	{
		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x0005EDD4 File Offset: 0x0005CFD4
		// (set) Token: 0x06002C7D RID: 11389 RVA: 0x0005EDDC File Offset: 0x0005CFDC
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

		// Token: 0x06002C7E RID: 11390 RVA: 0x0005EDE5 File Offset: 0x0005CFE5
		private void Awake()
		{
			this.m_transform = base.gameObject.transform;
			this.m_defaultLocalPosition = this.m_transform.localPosition;
		}

		// Token: 0x06002C7F RID: 11391 RVA: 0x00149E58 File Offset: 0x00148058
		private void Update()
		{
			if (!this.m_transform || !this.m_parent)
			{
				return;
			}
			if (Time.time - this.m_timeOfLastSample >= 1f)
			{
				this.SampleGround();
				if (!this.m_validHit)
				{
					this.ResetHeight();
				}
				else if (!this.m_terrainOnly || (this.m_lastValidHit.collider && this.m_lastValidHit.collider is TerrainCollider))
				{
					this.SetHeight(this.m_lastValidHit.point);
				}
				else
				{
					this.ResetHeight();
				}
				this.m_timeOfLastSample = Time.time;
			}
			Vector3 vector = this.m_transform.localPosition;
			vector = Vector3.MoveTowards(vector, this.m_targetLocalPosition, Time.deltaTime * this.m_speed);
			this.m_transform.localPosition = vector;
		}

		// Token: 0x06002C80 RID: 11392 RVA: 0x00149F2C File Offset: 0x0014812C
		private void SetHeight(Vector3 hitPoint)
		{
			float y = this.m_parent.position.y;
			float num = hitPoint.y - y;
			Vector3 defaultLocalPosition = this.m_defaultLocalPosition;
			defaultLocalPosition.y += num;
			this.m_targetLocalPosition = defaultLocalPosition;
		}

		// Token: 0x06002C81 RID: 11393 RVA: 0x0005EE09 File Offset: 0x0005D009
		internal void ResetHeight()
		{
			this.m_targetLocalPosition = this.m_defaultLocalPosition;
		}

		// Token: 0x06002C82 RID: 11394 RVA: 0x00149F70 File Offset: 0x00148170
		private void SampleGround()
		{
			Vector3 position = this.m_transform.position;
			position.y = this.m_parent.position.y + this.m_sampleHeight;
			RaycastHit lastValidHit;
			if (Physics.Raycast(position, Vector3.down, out lastValidHit, 2f + this.m_sampleHeight, this.layers, QueryTriggerInteraction.Ignore))
			{
				this.m_lastValidHit = lastValidHit;
				this.m_validHit = true;
				return;
			}
			this.m_validHit = false;
		}

		// Token: 0x04002C35 RID: 11317
		private const float kMaxSampleRate = 1f;

		// Token: 0x04002C36 RID: 11318
		private const float kMaxSampleDistance = 2f;

		// Token: 0x04002C37 RID: 11319
		[SerializeField]
		private Transform m_parent;

		// Token: 0x04002C38 RID: 11320
		[SerializeField]
		private float m_speed = 10f;

		// Token: 0x04002C39 RID: 11321
		[SerializeField]
		private bool m_terrainOnly;

		// Token: 0x04002C3A RID: 11322
		[SerializeField]
		private LayerMask m_layerMask = 1;

		// Token: 0x04002C3B RID: 11323
		[SerializeField]
		private float m_sampleHeight = 1.5f;

		// Token: 0x04002C3C RID: 11324
		private Vector3 m_defaultLocalPosition;

		// Token: 0x04002C3D RID: 11325
		private Vector3 m_targetLocalPosition;

		// Token: 0x04002C3E RID: 11326
		private Transform m_transform;

		// Token: 0x04002C3F RID: 11327
		private RaycastHit m_lastValidHit;

		// Token: 0x04002C40 RID: 11328
		private bool m_validHit;

		// Token: 0x04002C41 RID: 11329
		private float m_timeOfLastSample;
	}
}
