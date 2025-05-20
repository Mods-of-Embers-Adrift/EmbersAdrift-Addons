using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C0D RID: 3085
	public class DiminishingReturnCalculator
	{
		// Token: 0x06005EF7 RID: 24311 RVA: 0x0007FE7F File Offset: 0x0007E07F
		public DiminishingReturnCalculator(DiminishingReturnType type)
		{
			this.m_type = type;
			this.m_diminishingTime = this.m_type.GetDiminishingTime();
		}

		// Token: 0x06005EF8 RID: 24312 RVA: 0x001F8444 File Offset: 0x001F6644
		public float GetMultiplier(UniqueId instanceId)
		{
			if (DateTime.UtcNow >= this.m_expiration)
			{
				this.m_level = 0;
				this.m_currentApplicationInstance = UniqueId.Empty;
				this.m_expiration = DateTime.MinValue;
			}
			int level = this.m_level;
			if (this.m_currentApplicationInstance != instanceId)
			{
				if (this.m_level < 3)
				{
					this.m_expiration = DateTime.UtcNow.AddSeconds((double)this.m_diminishingTime);
				}
				this.m_level = Mathf.Clamp(this.m_level + 1, 0, 3);
				this.m_currentApplicationInstance = instanceId;
			}
			switch (level)
			{
			case 0:
				return 1f;
			case 1:
				return 0.75f;
			case 2:
				return 0.5f;
			case 3:
				return 0f;
			default:
				return 0f;
			}
		}

		// Token: 0x0400521B RID: 21019
		private readonly DiminishingReturnType m_type;

		// Token: 0x0400521C RID: 21020
		private readonly float m_diminishingTime;

		// Token: 0x0400521D RID: 21021
		private UniqueId m_currentApplicationInstance = UniqueId.Empty;

		// Token: 0x0400521E RID: 21022
		private DateTime m_expiration = DateTime.MinValue;

		// Token: 0x0400521F RID: 21023
		private int m_level;
	}
}
