using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD0 RID: 3024
	[CreateAssetMenu(menuName = "SoL/Profiles/Hunting Log Settings")]
	public class HuntingLogSettingsProfile : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x1700160A RID: 5642
		// (get) Token: 0x06005D6E RID: 23918 RVA: 0x0007ECA2 File Offset: 0x0007CEA2
		public HuntingLogTier[] Tiers
		{
			get
			{
				return this.m_tiers;
			}
		}

		// Token: 0x1700160B RID: 5643
		// (get) Token: 0x06005D6F RID: 23919 RVA: 0x0007ECAA File Offset: 0x0007CEAA
		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		// Token: 0x1700160C RID: 5644
		// (get) Token: 0x06005D70 RID: 23920 RVA: 0x0007ECB2 File Offset: 0x0007CEB2
		public bool Disabled
		{
			get
			{
				return !this.m_enabled;
			}
		}

		// Token: 0x1700160D RID: 5645
		// (get) Token: 0x06005D71 RID: 23921 RVA: 0x0007ECBD File Offset: 0x0007CEBD
		public int Version
		{
			get
			{
				return this.m_version;
			}
		}

		// Token: 0x1700160E RID: 5646
		// (get) Token: 0x06005D72 RID: 23922 RVA: 0x0007ECC5 File Offset: 0x0007CEC5
		public int TierCount
		{
			get
			{
				if (this.m_tiers == null)
				{
					return 0;
				}
				return this.m_tiers.Length;
			}
		}

		// Token: 0x1700160F RID: 5647
		// (get) Token: 0x06005D73 RID: 23923 RVA: 0x001F3D8C File Offset: 0x001F1F8C
		public int MaxPerkCount
		{
			get
			{
				if (this.m_maxPerkCount == null && this.m_tiers != null && this.m_tiers.Length != 0)
				{
					this.m_maxPerkCount = new int?(this.m_tiers[this.m_tiers.Length - 1].Count);
				}
				if (this.m_maxPerkCount == null)
				{
					return int.MaxValue;
				}
				return this.m_maxPerkCount.Value;
			}
		}

		// Token: 0x06005D74 RID: 23924 RVA: 0x001F3DF8 File Offset: 0x001F1FF8
		public int GetPerkMultiplier(HuntingLogPerkType perkType)
		{
			if (this.m_perkMultiplierLookup == null && this.m_perkMultipliers != null)
			{
				this.m_perkMultiplierLookup = new Dictionary<HuntingLogPerkType, int>(default(HuntingLogPerkTypeComparer));
				for (int i = 0; i < this.m_perkMultipliers.Length; i++)
				{
					this.m_perkMultiplierLookup.AddOrReplace(this.m_perkMultipliers[i].Type, this.m_perkMultipliers[i].Multiplier);
				}
			}
			int result;
			if (this.m_perkMultiplierLookup == null || !this.m_perkMultiplierLookup.TryGetValue(perkType, out result))
			{
				return 1;
			}
			return result;
		}

		// Token: 0x06005D75 RID: 23925 RVA: 0x0007ECD9 File Offset: 0x0007CED9
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_maxPerkCount = null;
			this.m_perkMultiplierLookup = null;
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x0004475B File Offset: 0x0004295B
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
		}

		// Token: 0x040050CF RID: 20687
		[SerializeField]
		private bool m_enabled = true;

		// Token: 0x040050D0 RID: 20688
		[SerializeField]
		private int m_version = 1;

		// Token: 0x040050D1 RID: 20689
		[SerializeField]
		private HuntingLogTier[] m_tiers;

		// Token: 0x040050D2 RID: 20690
		[SerializeField]
		private HuntingLogSettingsProfile.StatMultiplier[] m_perkMultipliers;

		// Token: 0x040050D3 RID: 20691
		[NonSerialized]
		private int? m_maxPerkCount;

		// Token: 0x040050D4 RID: 20692
		[NonSerialized]
		private Dictionary<HuntingLogPerkType, int> m_perkMultiplierLookup;

		// Token: 0x02000BD1 RID: 3025
		[Serializable]
		private class StatMultiplier
		{
			// Token: 0x17001610 RID: 5648
			// (get) Token: 0x06005D78 RID: 23928 RVA: 0x0007ED04 File Offset: 0x0007CF04
			public HuntingLogPerkType Type
			{
				get
				{
					return this.m_perkType;
				}
			}

			// Token: 0x17001611 RID: 5649
			// (get) Token: 0x06005D79 RID: 23929 RVA: 0x0007ED0C File Offset: 0x0007CF0C
			public int Multiplier
			{
				get
				{
					return this.m_multiplier;
				}
			}

			// Token: 0x17001612 RID: 5650
			// (get) Token: 0x06005D7A RID: 23930 RVA: 0x0007EB72 File Offset: 0x0007CD72
			private HuntingLogPerkType[] m_validStatPerks
			{
				get
				{
					return HuntingLogExtensions.StatPerks;
				}
			}

			// Token: 0x040050D5 RID: 20693
			[SerializeField]
			private HuntingLogPerkType m_perkType = HuntingLogPerkType.Dmg;

			// Token: 0x040050D6 RID: 20694
			[SerializeField]
			private int m_multiplier = 1;
		}
	}
}
