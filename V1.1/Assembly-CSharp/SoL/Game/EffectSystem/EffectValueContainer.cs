using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C1E RID: 3102
	public class EffectValueContainer
	{
		// Token: 0x06005FB0 RID: 24496 RVA: 0x001FAF28 File Offset: 0x001F9128
		public EffectValueContainer(StatusEffectType type)
		{
			this.m_type = type;
			this.m_hasSubTypes = this.m_type.HasSubTypes();
			this.m_hasDamageTypes = this.m_type.HasDamageTypes();
			if (this.m_hasSubTypes)
			{
				StatusEffectSubType[] subType = this.m_type.GetSubType();
				if (this.m_hasDamageTypes)
				{
					this.m_subTypesWithDamage = new Dictionary<StatusEffectSubType, DamageTypeValueContainer>(subType.Length, default(StatusEffectSubTypeComparer));
					for (int i = 0; i < subType.Length; i++)
					{
						this.m_subTypesWithDamage.Add(subType[i], new DamageTypeValueContainer(subType[i]));
					}
					return;
				}
				this.m_subTypes = new Dictionary<StatusEffectSubType, int>(subType.Length, default(StatusEffectSubTypeComparer));
				for (int j = 0; j < subType.Length; j++)
				{
					this.m_subTypes.Add(subType[j], 0);
				}
			}
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x00080645 File Offset: 0x0007E845
		public int GetValue()
		{
			return this.m_baseValue;
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x0008064D File Offset: 0x0007E84D
		public int GetValue(StatusEffectSubType subType)
		{
			if (this.m_hasDamageTypes)
			{
				throw new ArgumentException("Asking for StatusEffectSubType value when we should be including the DamageType!");
			}
			if (!this.m_hasSubTypes)
			{
				throw new ArgumentException("Asking for StatusEffectSubType value when we have no sub types!");
			}
			return this.m_baseValue + this.m_subTypes[subType];
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x001FB000 File Offset: 0x001F9200
		public int GetValue(StatusEffectSubType subType, DamageType damageType)
		{
			if (!this.m_hasDamageTypes)
			{
				throw new ArgumentException("Asking for StatusEffectSubType WITH DamageType but there is no damage type!");
			}
			int num = 0;
			DamageTypeValueContainer damageTypeValueContainer;
			if (this.m_subTypesWithDamage.TryGetValue(subType, out damageTypeValueContainer))
			{
				num = damageTypeValueContainer.GetValue(damageType);
			}
			return this.m_baseValue + num;
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x00080688 File Offset: 0x0007E888
		public void ModifyValue(int delta)
		{
			this.m_baseValue += delta;
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x001FB044 File Offset: 0x001F9244
		public void ModifyValue(StatusEffectSubType subType, int delta)
		{
			if (this.m_hasDamageTypes)
			{
				this.m_subTypesWithDamage[subType].ModifyValue(delta);
				return;
			}
			if (this.m_hasSubTypes)
			{
				Dictionary<StatusEffectSubType, int> subTypes = this.m_subTypes;
				subTypes[subType] += delta;
				return;
			}
			throw new ArgumentException("subType");
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x00080698 File Offset: 0x0007E898
		public void ModifyValue(StatusEffectSubType subType, DamageType damageType, int delta)
		{
			if (!this.m_hasDamageTypes)
			{
				throw new ArgumentException("Asking for StatusEffectSubType WITH DamageType but there is no damage type!");
			}
			this.m_subTypesWithDamage[subType].ModifyValue(damageType, delta);
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x001FB098 File Offset: 0x001F9298
		public void ResetValues()
		{
			this.m_baseValue = 0;
			if (this.m_hasSubTypes)
			{
				StatusEffectSubType[] subType = this.m_type.GetSubType();
				if (this.m_hasDamageTypes)
				{
					for (int i = 0; i < subType.Length; i++)
					{
						this.m_subTypesWithDamage[subType[i]].ResetValues();
					}
					return;
				}
				for (int j = 0; j < subType.Length; j++)
				{
					this.m_subTypes[subType[j]] = 0;
				}
			}
		}

		// Token: 0x04005290 RID: 21136
		private readonly StatusEffectType m_type;

		// Token: 0x04005291 RID: 21137
		private int m_baseValue;

		// Token: 0x04005292 RID: 21138
		private readonly bool m_hasSubTypes;

		// Token: 0x04005293 RID: 21139
		private readonly bool m_hasDamageTypes;

		// Token: 0x04005294 RID: 21140
		private readonly Dictionary<StatusEffectSubType, int> m_subTypes;

		// Token: 0x04005295 RID: 21141
		private readonly Dictionary<StatusEffectSubType, DamageTypeValueContainer> m_subTypesWithDamage;
	}
}
