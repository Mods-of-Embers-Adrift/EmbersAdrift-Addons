using System;
using UnityEngine;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BFF RID: 3071
	[CreateAssetMenu(menuName = "SoL/Profiles/Weapon Flanking Bonus")]
	public class WeaponFlankingBonusProfile : ScriptableObject
	{
		// Token: 0x17001662 RID: 5730
		// (get) Token: 0x06005E9D RID: 24221 RVA: 0x0007FA01 File Offset: 0x0007DC01
		public WeaponFlankingBonus Bonus
		{
			get
			{
				return this.m_bonus;
			}
		}

		// Token: 0x040051CA RID: 20938
		[SerializeField]
		private WeaponFlankingBonus m_bonus;
	}
}
