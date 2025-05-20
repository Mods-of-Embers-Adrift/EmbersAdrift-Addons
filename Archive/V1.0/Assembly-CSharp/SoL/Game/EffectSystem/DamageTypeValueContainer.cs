using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C1F RID: 3103
	public class DamageTypeValueContainer
	{
		// Token: 0x06005FB8 RID: 24504 RVA: 0x001FB108 File Offset: 0x001F9308
		public DamageTypeValueContainer(StatusEffectSubType subType)
		{
			this.m_subType = subType;
			DamageType[] damageTypes = subType.GetDamageTypes();
			this.m_damageTypes = new Dictionary<DamageType, int>(damageTypes.Length, default(DamageTypeComparer));
			for (int i = 0; i < damageTypes.Length; i++)
			{
				this.m_damageTypes.Add(damageTypes[i], 0);
			}
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x000806C0 File Offset: 0x0007E8C0
		public int GetValue(DamageType damageType)
		{
			return this.m_baseValue + this.m_damageTypes[damageType];
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x000806D5 File Offset: 0x0007E8D5
		public void ModifyValue(int delta)
		{
			this.m_baseValue += delta;
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x001FB164 File Offset: 0x001F9364
		public void ModifyValue(DamageType damageType, int delta)
		{
			Dictionary<DamageType, int> damageTypes = this.m_damageTypes;
			damageTypes[damageType] += delta;
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x001FB18C File Offset: 0x001F938C
		public void ResetValues()
		{
			this.m_baseValue = 0;
			DamageType[] damageTypes = this.m_subType.GetDamageTypes();
			for (int i = 0; i < damageTypes.Length; i++)
			{
				this.m_damageTypes[damageTypes[i]] = 0;
			}
		}

		// Token: 0x04005296 RID: 21142
		private readonly StatusEffectSubType m_subType;

		// Token: 0x04005297 RID: 21143
		private int m_baseValue;

		// Token: 0x04005298 RID: 21144
		private readonly Dictionary<DamageType, int> m_damageTypes;
	}
}
