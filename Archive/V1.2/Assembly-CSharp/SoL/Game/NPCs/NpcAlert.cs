using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007FF RID: 2047
	public class NpcAlert : IPoolable
	{
		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x06003B59 RID: 15193 RVA: 0x0006824C File Offset: 0x0006644C
		public float Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x06003B5A RID: 15194 RVA: 0x00068254 File Offset: 0x00066454
		public DateTime Timestamp
		{
			get
			{
				return this.m_timestamp;
			}
		}

		// Token: 0x17000D97 RID: 3479
		// (get) Token: 0x06003B5B RID: 15195 RVA: 0x0006825C File Offset: 0x0006645C
		public Vector3 Position
		{
			get
			{
				return this.m_position;
			}
		}

		// Token: 0x17000D98 RID: 3480
		// (get) Token: 0x06003B5C RID: 15196 RVA: 0x00068264 File Offset: 0x00066464
		public GameEntity Entity
		{
			get
			{
				return this.m_entity;
			}
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x0006826C File Offset: 0x0006646C
		public NpcAlert()
		{
		}

		// Token: 0x06003B5E RID: 15198 RVA: 0x00068295 File Offset: 0x00066495
		public NpcAlert(GameEntity entity, float value)
		{
			this.Initialize(entity, value);
		}

		// Token: 0x06003B5F RID: 15199 RVA: 0x0017B084 File Offset: 0x00179284
		public void Initialize(GameEntity entity, float value)
		{
			this.m_entity = entity;
			this.m_id = UniqueId.GenerateFromGuid();
			this.m_position = entity.gameObject.transform.position;
			this.m_originalValue = value;
			this.m_value = value;
			this.m_timestamp = DateTime.UtcNow;
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x000682C6 File Offset: 0x000664C6
		public void UpdateAlert(GameEntity entity, float value)
		{
			this.m_position = entity.gameObject.transform.position;
			this.m_originalValue = value;
			this.m_value = value;
			this.m_timestamp = DateTime.UtcNow;
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x0017B0D4 File Offset: 0x001792D4
		public void UpdateValue()
		{
			if (this.m_lastFrameUpdated == Time.frameCount)
			{
				return;
			}
			float num = 1f - Mathf.Clamp01((float)(DateTime.UtcNow - this.m_timestamp).TotalSeconds / 10f);
			this.m_value = num * this.m_originalValue;
			this.m_lastFrameUpdated = Time.frameCount;
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0017B134 File Offset: 0x00179334
		public void Reset()
		{
			this.m_entity = null;
			this.m_id = UniqueId.Empty;
			this.m_position = Vector3.zero;
			this.m_originalValue = 0f;
			this.m_value = 0f;
			this.m_timestamp = DateTime.MinValue;
			this.m_lastFrameUpdated = 0;
		}

		// Token: 0x17000D99 RID: 3481
		// (get) Token: 0x06003B63 RID: 15203 RVA: 0x000682F7 File Offset: 0x000664F7
		// (set) Token: 0x06003B64 RID: 15204 RVA: 0x000682FF File Offset: 0x000664FF
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x040039BF RID: 14783
		private const float kAlertDecayTime = 10f;

		// Token: 0x040039C0 RID: 14784
		private bool m_inPool;

		// Token: 0x040039C1 RID: 14785
		private GameEntity m_entity;

		// Token: 0x040039C2 RID: 14786
		private UniqueId m_id = UniqueId.Empty;

		// Token: 0x040039C3 RID: 14787
		private Vector3 m_position = Vector3.zero;

		// Token: 0x040039C4 RID: 14788
		private float m_originalValue;

		// Token: 0x040039C5 RID: 14789
		private float m_value;

		// Token: 0x040039C6 RID: 14790
		private DateTime m_timestamp = DateTime.MinValue;

		// Token: 0x040039C7 RID: 14791
		private int m_lastFrameUpdated;
	}
}
