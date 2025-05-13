using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000672 RID: 1650
	[Serializable]
	public class CallForHelpSettings : ICallForHelpSettings
	{
		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x0600332E RID: 13102 RVA: 0x000633F4 File Offset: 0x000615F4
		private bool m_hasInitialThreat
		{
			get
			{
				return this.m_flags.HasBitFlag(CallForHelpFlags.OnInitialThreat);
			}
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x0600332F RID: 13103 RVA: 0x00063402 File Offset: 0x00061602
		private bool m_hasDeath
		{
			get
			{
				return this.m_flags.HasBitFlag(CallForHelpFlags.OnDeath);
			}
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06003330 RID: 13104 RVA: 0x00063410 File Offset: 0x00061610
		private bool m_hasPeriodic
		{
			get
			{
				return this.m_flags.HasBitFlag(CallForHelpFlags.Periodically);
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06003331 RID: 13105 RVA: 0x0006341E File Offset: 0x0006161E
		private bool m_hasFleeing
		{
			get
			{
				return this.m_flags.HasBitFlag(CallForHelpFlags.WhileFleeing);
			}
		}

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06003332 RID: 13106 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hasOverride
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06003333 RID: 13107 RVA: 0x0006342C File Offset: 0x0006162C
		private bool m_showInitialThreat
		{
			get
			{
				return !this.m_hasOverride && this.m_hasInitialThreat;
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06003334 RID: 13108 RVA: 0x0006343E File Offset: 0x0006163E
		private bool m_showDeath
		{
			get
			{
				return !this.m_hasOverride && this.m_hasDeath;
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x06003335 RID: 13109 RVA: 0x00063450 File Offset: 0x00061650
		private bool m_showPeriodic
		{
			get
			{
				return !this.m_hasOverride && this.m_hasPeriodic;
			}
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06003336 RID: 13110 RVA: 0x00063462 File Offset: 0x00061662
		private bool m_showFleeing
		{
			get
			{
				return !this.m_hasOverride && this.m_hasFleeing;
			}
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06003337 RID: 13111 RVA: 0x00063474 File Offset: 0x00061674
		public virtual CallForHelpData InitialThreat
		{
			get
			{
				if (!this.m_hasInitialThreat)
				{
					return null;
				}
				return this.m_initialThreat;
			}
		}

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06003338 RID: 13112 RVA: 0x00063486 File Offset: 0x00061686
		public virtual CallForHelpData Death
		{
			get
			{
				if (!this.m_hasDeath)
				{
					return null;
				}
				return this.m_death;
			}
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06003339 RID: 13113 RVA: 0x00063498 File Offset: 0x00061698
		public virtual CallForHelpPeriodicData Periodic
		{
			get
			{
				if (!this.m_hasPeriodic)
				{
					return null;
				}
				return this.m_periodic;
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x0600333A RID: 13114 RVA: 0x000634AA File Offset: 0x000616AA
		public virtual CallForHelpPeriodicData Fleeing
		{
			get
			{
				if (!this.m_hasFleeing)
				{
					return null;
				}
				return this.m_fleeing;
			}
		}

		// Token: 0x0600333B RID: 13115 RVA: 0x000634BC File Offset: 0x000616BC
		public void NormalizeProbabilities()
		{
			CallForHelpData initialThreat = this.m_initialThreat;
			if (initialThreat != null)
			{
				initialThreat.NormalizeProbabilities();
			}
			CallForHelpData death = this.m_death;
			if (death != null)
			{
				death.NormalizeProbabilities();
			}
			CallForHelpPeriodicData periodic = this.m_periodic;
			if (periodic == null)
			{
				return;
			}
			periodic.NormalizeProbabilities();
		}

		// Token: 0x04003164 RID: 12644
		protected const string kGroupName = "Call For Help";

		// Token: 0x04003165 RID: 12645
		[Tooltip("Select which call for helps are active.\nInitial is triggered when target count goes from 0 to greater than 0.\nPeriodic is triggered periodically.\nWhileFleeing is triggered when the creature is fleeing (from fear or other means). This also overrides Periodic while fleeing.")]
		[SerializeField]
		private CallForHelpFlags m_flags;

		// Token: 0x04003166 RID: 12646
		[SerializeField]
		private CallForHelpData m_initialThreat;

		// Token: 0x04003167 RID: 12647
		[SerializeField]
		private CallForHelpPeriodicData m_periodic;

		// Token: 0x04003168 RID: 12648
		[SerializeField]
		private CallForHelpData m_death;

		// Token: 0x04003169 RID: 12649
		[SerializeField]
		private CallForHelpPeriodicData m_fleeing;
	}
}
