using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C2F RID: 3119
	[Serializable]
	public abstract class StatModifierBase
	{
		// Token: 0x17001716 RID: 5910
		// (get) Token: 0x06006032 RID: 24626 RVA: 0x00080BC7 File Offset: 0x0007EDC7
		public StatType StatType
		{
			get
			{
				return this.m_statType;
			}
		}

		// Token: 0x17001717 RID: 5911
		// (get) Token: 0x06006033 RID: 24627 RVA: 0x00080BCF File Offset: 0x0007EDCF
		public StatusEffectType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17001718 RID: 5912
		// (get) Token: 0x06006034 RID: 24628 RVA: 0x00080BD7 File Offset: 0x0007EDD7
		public StatusEffectSubType SubType
		{
			get
			{
				return this.m_subType;
			}
		}

		// Token: 0x17001719 RID: 5913
		// (get) Token: 0x06006035 RID: 24629 RVA: 0x001FC0E8 File Offset: 0x001FA2E8
		public DamageType? DamageType
		{
			get
			{
				if (!this.m_type.HasDamageTypes())
				{
					return null;
				}
				return new DamageType?(this.m_damageType);
			}
		}

		// Token: 0x06006036 RID: 24630 RVA: 0x00044765 File Offset: 0x00042965
		public StatModifierBase()
		{
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x00080BDF File Offset: 0x0007EDDF
		public StatModifierBase(StatType statType)
		{
			this.m_statType = statType;
		}

		// Token: 0x06006038 RID: 24632 RVA: 0x00080BEE File Offset: 0x0007EDEE
		public StatModifierBase(StatusEffectType type, StatusEffectSubType subType, bool specifyDamageType, DamageType damageType)
		{
			this.m_type = type;
			this.m_subType = subType;
			this.m_specifyDamageType = specifyDamageType;
			this.m_damageType = damageType;
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x001FC118 File Offset: 0x001FA318
		protected void ModifyValueInternal(Dictionary<StatType, int> effects, int value, bool adding)
		{
			StatType statType;
			if (adding)
			{
				statType = this.m_statType;
				effects[statType] += value;
				return;
			}
			statType = this.m_statType;
			effects[statType] -= value;
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x00080C13 File Offset: 0x0007EE13
		protected string GetTypeString()
		{
			return this.m_statType.GetTooltipDisplay();
		}

		// Token: 0x1700171A RID: 5914
		// (get) Token: 0x0600603B RID: 24635 RVA: 0x00080C20 File Offset: 0x0007EE20
		public string IndexName
		{
			get
			{
				return this.m_statType.GetIndexName();
			}
		}

		// Token: 0x0600603C RID: 24636
		protected abstract string GetValueString();

		// Token: 0x1700171B RID: 5915
		// (get) Token: 0x0600603D RID: 24637 RVA: 0x00080C2D File Offset: 0x0007EE2D
		private bool m_showSubType
		{
			get
			{
				return this.m_type.HasSubTypes();
			}
		}

		// Token: 0x1700171C RID: 5916
		// (get) Token: 0x0600603E RID: 24638 RVA: 0x00080C3A File Offset: 0x0007EE3A
		private bool m_showDamageTypeBool
		{
			get
			{
				return this.m_type.HasDamageTypes();
			}
		}

		// Token: 0x1700171D RID: 5917
		// (get) Token: 0x0600603F RID: 24639 RVA: 0x00080C47 File Offset: 0x0007EE47
		private bool m_showDamageType
		{
			get
			{
				return this.m_showDamageTypeBool && this.m_specifyDamageType;
			}
		}

		// Token: 0x1700171E RID: 5918
		// (get) Token: 0x06006040 RID: 24640 RVA: 0x00080C59 File Offset: 0x0007EE59
		private StatusEffectSubType[] m_validSubTypes
		{
			get
			{
				if (!this.m_type.HasSubTypes())
				{
					return null;
				}
				return this.m_type.GetSubType();
			}
		}

		// Token: 0x1700171F RID: 5919
		// (get) Token: 0x06006041 RID: 24641 RVA: 0x00080C75 File Offset: 0x0007EE75
		private DamageType[] m_validDamageTypes
		{
			get
			{
				if (!this.m_type.HasDamageTypes())
				{
					return null;
				}
				return this.m_subType.GetDamageTypes();
			}
		}

		// Token: 0x06006042 RID: 24642 RVA: 0x001FC15C File Offset: 0x001FA35C
		private void ValidateTypes()
		{
			if (this.m_type.HasSubTypes())
			{
				bool flag = false;
				StatusEffectSubType[] validSubTypes = this.m_validSubTypes;
				for (int i = 0; i < validSubTypes.Length; i++)
				{
					if (validSubTypes[i] == this.m_subType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.m_subType = validSubTypes[0];
				}
			}
			if (this.m_type.HasDamageTypes())
			{
				bool flag2 = false;
				DamageType[] validDamageTypes = this.m_validDamageTypes;
				for (int j = 0; j < validDamageTypes.Length; j++)
				{
					if (validDamageTypes[j] == this.m_damageType)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					this.m_damageType = validDamageTypes[0];
				}
			}
		}

		// Token: 0x040052E9 RID: 21225
		[SerializeField]
		protected StatType m_statType;

		// Token: 0x040052EA RID: 21226
		[HideInInspector]
		[SerializeField]
		protected StatusEffectType m_type;

		// Token: 0x040052EB RID: 21227
		[HideInInspector]
		[SerializeField]
		protected StatusEffectSubType m_subType;

		// Token: 0x040052EC RID: 21228
		[HideInInspector]
		[SerializeField]
		protected bool m_specifyDamageType;

		// Token: 0x040052ED RID: 21229
		[HideInInspector]
		[SerializeField]
		protected DamageType m_damageType;
	}
}
