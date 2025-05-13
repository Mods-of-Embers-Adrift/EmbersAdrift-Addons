using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000673 RID: 1651
	[Serializable]
	public class CallForHelpProfileWithOverride : CallForHelpSettings
	{
		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x0600333D RID: 13117 RVA: 0x000634F0 File Offset: 0x000616F0
		protected override bool m_hasOverride
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x0600333E RID: 13118 RVA: 0x000634FE File Offset: 0x000616FE
		private IEnumerable GetProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<CallForHelpScriptableProfile>();
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x0600333F RID: 13119 RVA: 0x00063505 File Offset: 0x00061705
		public override CallForHelpData InitialThreat
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.InitialThreat;
				}
				return this.m_override.InitialThreat;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06003340 RID: 13120 RVA: 0x00063521 File Offset: 0x00061721
		public override CallForHelpData Death
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.Death;
				}
				return this.m_override.Death;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06003341 RID: 13121 RVA: 0x0006353D File Offset: 0x0006173D
		public override CallForHelpPeriodicData Periodic
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.Periodic;
				}
				return this.m_override.Periodic;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06003342 RID: 13122 RVA: 0x00063559 File Offset: 0x00061759
		public override CallForHelpPeriodicData Fleeing
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.Fleeing;
				}
				return this.m_override.Fleeing;
			}
		}

		// Token: 0x0400316A RID: 12650
		[SerializeField]
		private CallForHelpScriptableProfile m_override;
	}
}
