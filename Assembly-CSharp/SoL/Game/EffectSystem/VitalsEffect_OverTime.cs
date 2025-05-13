using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C1C RID: 3100
	[Serializable]
	public class VitalsEffect_OverTime : VitalsEffect_Base
	{
		// Token: 0x06005FAB RID: 24491 RVA: 0x00080617 File Offset: 0x0007E817
		public override string GetTitleText()
		{
			return base.GetTitleText() + " Over Time";
		}

		// Token: 0x170016DF RID: 5855
		// (get) Token: 0x06005FAC RID: 24492 RVA: 0x00080629 File Offset: 0x0007E829
		protected override EffectResourceType[] m_validResourceTypes
		{
			get
			{
				return new EffectResourceType[]
				{
					EffectResourceType.Health,
					EffectResourceType.Stamina
				};
			}
		}

		// Token: 0x170016E0 RID: 5856
		// (get) Token: 0x06005FAD RID: 24493 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool AllowHealthFractionBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170016E1 RID: 5857
		// (get) Token: 0x06005FAE RID: 24494 RVA: 0x00080635 File Offset: 0x0007E835
		public int Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x0400528C RID: 21132
		[SerializeField]
		private int m_value;
	}
}
