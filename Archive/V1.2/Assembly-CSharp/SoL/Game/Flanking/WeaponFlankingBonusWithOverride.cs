using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BFD RID: 3069
	[Serializable]
	public class WeaponFlankingBonusWithOverride : WeaponFlankingBonus
	{
		// Token: 0x1700165D RID: 5725
		// (get) Token: 0x06005E96 RID: 24214 RVA: 0x0007F940 File Offset: 0x0007DB40
		protected override bool m_hideInternal
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x1700165E RID: 5726
		// (get) Token: 0x06005E97 RID: 24215 RVA: 0x0007F94E File Offset: 0x0007DB4E
		internal override WeaponFlankingBonus.WeaponFlankingBonusData Front
		{
			get
			{
				if (!(this.m_override != null))
				{
					return base.Front;
				}
				return this.m_override.Bonus.Front;
			}
		}

		// Token: 0x1700165F RID: 5727
		// (get) Token: 0x06005E98 RID: 24216 RVA: 0x0007F975 File Offset: 0x0007DB75
		internal override WeaponFlankingBonus.WeaponFlankingBonusData Sides
		{
			get
			{
				if (!(this.m_override != null))
				{
					return base.Sides;
				}
				return this.m_override.Bonus.Sides;
			}
		}

		// Token: 0x17001660 RID: 5728
		// (get) Token: 0x06005E99 RID: 24217 RVA: 0x0007F99C File Offset: 0x0007DB9C
		internal override WeaponFlankingBonus.WeaponFlankingBonusData Rear
		{
			get
			{
				if (!(this.m_override != null))
				{
					return base.Rear;
				}
				return this.m_override.Bonus.Rear;
			}
		}

		// Token: 0x17001661 RID: 5729
		// (get) Token: 0x06005E9A RID: 24218 RVA: 0x0007F9C3 File Offset: 0x0007DBC3
		private IEnumerable GetProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<WeaponFlankingBonusProfile>();
			}
		}

		// Token: 0x040051C9 RID: 20937
		[SerializeField]
		private WeaponFlankingBonusProfile m_override;
	}
}
