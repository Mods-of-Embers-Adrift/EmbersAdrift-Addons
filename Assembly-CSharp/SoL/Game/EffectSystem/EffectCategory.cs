using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C58 RID: 3160
	[Serializable]
	public class EffectCategory
	{
		// Token: 0x1700174F RID: 5967
		// (get) Token: 0x06006122 RID: 24866 RVA: 0x00081710 File Offset: 0x0007F910
		public EffectCategoryType CategoryType
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x17001750 RID: 5968
		// (get) Token: 0x06006123 RID: 24867 RVA: 0x00081718 File Offset: 0x0007F918
		public EffectCategoryFlags CategoryFlags
		{
			get
			{
				return this.m_categoryFlags;
			}
		}

		// Token: 0x17001751 RID: 5969
		// (get) Token: 0x06006124 RID: 24868 RVA: 0x00081720 File Offset: 0x0007F920
		public EffectVariantType VariantType
		{
			get
			{
				return this.m_variantType;
			}
		}

		// Token: 0x17001752 RID: 5970
		// (get) Token: 0x06006125 RID: 24869 RVA: 0x00081728 File Offset: 0x0007F928
		public int StackLimit
		{
			get
			{
				if (!this.m_allowStacking)
				{
					return 1;
				}
				return this.m_stackLimit;
			}
		}

		// Token: 0x06006126 RID: 24870 RVA: 0x001FF270 File Offset: 0x001FD470
		public bool Migrate(out string msg)
		{
			EffectCategoryFlags categoryFlags = this.m_categoryFlags;
			EffectVariantType variantType = this.m_variantType;
			msg = string.Empty;
			switch (this.m_category)
			{
			case EffectCategoryType.None:
				return false;
			case EffectCategoryType.OutgoingDamage:
				this.m_categoryFlags = EffectCategoryFlags.OutgoingDamage;
				break;
			case EffectCategoryType.OutgoingHit:
				this.m_categoryFlags = EffectCategoryFlags.OutgoingHit;
				break;
			case EffectCategoryType.OutgoingPenetration:
				this.m_categoryFlags = EffectCategoryFlags.OutgoingPenetration;
				break;
			case EffectCategoryType.IncomingDamageResist:
				this.m_categoryFlags = EffectCategoryFlags.IncomingDamageResist;
				break;
			case EffectCategoryType.IncomingActiveDefense:
				msg = "Incoming Active Defense";
				this.m_categoryFlags = EffectCategoryFlags.IncomingActiveDefense;
				break;
			case EffectCategoryType.IncomingStatusEffectResist:
				this.m_categoryFlags = EffectCategoryFlags.IncomingStatusEffectResist;
				break;
			case EffectCategoryType.HealthRegen:
				this.m_categoryFlags = EffectCategoryFlags.HealthRegen;
				break;
			case EffectCategoryType.Resilience:
				this.m_categoryFlags = EffectCategoryFlags.Resilience;
				break;
			case EffectCategoryType.SafeFall:
				this.m_categoryFlags = EffectCategoryFlags.SafeFall;
				break;
			case EffectCategoryType.Haste:
				this.m_categoryFlags = EffectCategoryFlags.Haste;
				break;
			case EffectCategoryType.Movement:
				this.m_categoryFlags = EffectCategoryFlags.Movement;
				break;
			case EffectCategoryType.StaminaRegen:
				this.m_categoryFlags = EffectCategoryFlags.StaminaRegen;
				break;
			case EffectCategoryType.HealthOverTime:
				this.m_categoryFlags = EffectCategoryFlags.HealthOverTime;
				break;
			case EffectCategoryType.StaminaOverTime:
				this.m_categoryFlags = EffectCategoryFlags.StaminaOverTime;
				break;
			case EffectCategoryType.OpenWound:
				this.m_categoryFlags = EffectCategoryFlags.OpenWound;
				break;
			case EffectCategoryType.DreadBellow:
				this.m_variantType = EffectVariantType.Song;
				this.m_categoryFlags = EffectCategoryFlags.OutgoingHit;
				break;
			case EffectCategoryType.FocusingBattleChant:
				this.m_variantType = EffectVariantType.Song;
				this.m_categoryFlags = EffectCategoryFlags.OutgoingHit;
				break;
			case EffectCategoryType.Flatworm:
				this.m_categoryFlags = EffectCategoryFlags.Flatworm;
				break;
			case EffectCategoryType.Serration:
				this.m_categoryFlags = EffectCategoryFlags.Serration;
				break;
			case EffectCategoryType.Supplies:
				this.m_categoryFlags = EffectCategoryFlags.Supplies;
				break;
			case EffectCategoryType.ViperidToxins:
				this.m_categoryFlags = EffectCategoryFlags.ViperidToxins;
				break;
			}
			return this.m_categoryFlags != categoryFlags || this.m_variantType != variantType;
		}

		// Token: 0x04005441 RID: 21569
		[SerializeField]
		private EffectCategoryType m_category;

		// Token: 0x04005442 RID: 21570
		[SerializeField]
		private EffectCategoryFlags m_categoryFlags;

		// Token: 0x04005443 RID: 21571
		[SerializeField]
		private EffectVariantType m_variantType;

		// Token: 0x04005444 RID: 21572
		[SerializeField]
		private bool m_allowStacking;

		// Token: 0x04005445 RID: 21573
		[SerializeField]
		private int m_stackLimit = 1;
	}
}
