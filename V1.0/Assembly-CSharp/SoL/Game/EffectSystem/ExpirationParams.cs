using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C23 RID: 3107
	[Serializable]
	public class ExpirationParams
	{
		// Token: 0x170016EF RID: 5871
		// (get) Token: 0x06005FD8 RID: 24536 RVA: 0x00080804 File Offset: 0x0007EA04
		private bool m_hasTriggerCount
		{
			get
			{
				return this.m_conditions.HasBitFlag(ExpirationConditionFlags.TriggerCount);
			}
		}

		// Token: 0x170016F0 RID: 5872
		// (get) Token: 0x06005FD9 RID: 24537 RVA: 0x00080812 File Offset: 0x0007EA12
		private bool m_hasCurable
		{
			get
			{
				return this.m_conditions.HasBitFlag(ExpirationConditionFlags.Curable);
			}
		}

		// Token: 0x170016F1 RID: 5873
		// (get) Token: 0x06005FDA RID: 24538 RVA: 0x00080820 File Offset: 0x0007EA20
		private bool m_canDismiss
		{
			get
			{
				return this.m_conditions.HasBitFlag(ExpirationConditionFlags.CanDismiss);
			}
		}

		// Token: 0x170016F2 RID: 5874
		// (get) Token: 0x06005FDB RID: 24539 RVA: 0x0008082E File Offset: 0x0007EA2E
		public bool HasTrigger
		{
			get
			{
				return this.m_hasTriggerCount;
			}
		}

		// Token: 0x170016F3 RID: 5875
		// (get) Token: 0x06005FDC RID: 24540 RVA: 0x00080836 File Offset: 0x0007EA36
		public bool CanDismiss
		{
			get
			{
				return this.m_canDismiss;
			}
		}

		// Token: 0x170016F4 RID: 5876
		// (get) Token: 0x06005FDD RID: 24541 RVA: 0x0008083E File Offset: 0x0007EA3E
		public int MaxDuration
		{
			get
			{
				return this.m_maxDuration;
			}
		}

		// Token: 0x170016F5 RID: 5877
		// (get) Token: 0x06005FDE RID: 24542 RVA: 0x00080804 File Offset: 0x0007EA04
		public bool HasTriggerCount
		{
			get
			{
				return this.m_conditions.HasBitFlag(ExpirationConditionFlags.TriggerCount);
			}
		}

		// Token: 0x170016F6 RID: 5878
		// (get) Token: 0x06005FDF RID: 24543 RVA: 0x00080846 File Offset: 0x0007EA46
		public int TriggerCount
		{
			get
			{
				return this.m_triggers;
			}
		}

		// Token: 0x06005FE0 RID: 24544 RVA: 0x001FB5C4 File Offset: 0x001F97C4
		public bool Expire(float timeElapsed, int triggerCount)
		{
			bool flag = timeElapsed >= (float)this.m_maxDuration;
			if (!flag && this.m_hasTriggerCount)
			{
				flag = (triggerCount >= this.m_triggers);
			}
			return flag;
		}

		// Token: 0x06005FE1 RID: 24545 RVA: 0x0008084E File Offset: 0x0007EA4E
		public bool ExpireFromTriggers(int triggerCount, int triggerCountMod)
		{
			return this.m_hasTriggerCount && triggerCount >= this.m_triggers + triggerCountMod;
		}

		// Token: 0x06005FE2 RID: 24546 RVA: 0x00080868 File Offset: 0x0007EA68
		public bool HasRemainingTriggers(int triggerCount, int triggerCountMod)
		{
			return !this.ExpireFromTriggers(triggerCount, triggerCountMod);
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x001FB5F8 File Offset: 0x001F97F8
		public bool HasRemainingTriggers(int triggerCount, ReagentItem reagentItem)
		{
			int triggerCountMod = reagentItem ? reagentItem.GetTriggerCountMod() : 0;
			return this.HasRemainingTriggers(triggerCount, triggerCountMod);
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x00080875 File Offset: 0x0007EA75
		private void ValidateNumbers()
		{
			this.m_maxDuration = Mathf.Clamp(this.m_maxDuration, 1, int.MaxValue);
			this.m_triggers = Mathf.Clamp(this.m_triggers, 1, int.MaxValue);
		}

		// Token: 0x040052AF RID: 21167
		[SerializeField]
		private int m_maxDuration = 1;

		// Token: 0x040052B0 RID: 21168
		[SerializeField]
		private ExpirationConditionFlags m_conditions;

		// Token: 0x040052B1 RID: 21169
		[SerializeField]
		private int m_triggers = 1;
	}
}
