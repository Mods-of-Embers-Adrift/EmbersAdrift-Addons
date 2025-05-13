using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Subscription;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000747 RID: 1863
	[Serializable]
	public class SubscriberSettings
	{
		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06003790 RID: 14224 RVA: 0x00066027 File Offset: 0x00064227
		public Color SubscriberColor
		{
			get
			{
				return this.m_subscriberColor;
			}
		}

		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x06003791 RID: 14225 RVA: 0x0006602F File Offset: 0x0006422F
		public ConsumableItemEmberRingEnhancment EmberRingEnhancer
		{
			get
			{
				return this.m_emberRingEnhancment;
			}
		}

		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x06003792 RID: 14226 RVA: 0x00066037 File Offset: 0x00064237
		public DeployPortableStationAbility DeployPortableCraftingStationAbility
		{
			get
			{
				return this.m_deployPortableCraftingStationAbility;
			}
		}

		// Token: 0x17000C8D RID: 3213
		// (get) Token: 0x06003793 RID: 14227 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColors
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x17000C8E RID: 3214
		// (get) Token: 0x06003794 RID: 14228 RVA: 0x0006603F File Offset: 0x0006423F
		public float SubscriberBankBorderValueMultiplier
		{
			get
			{
				return this.m_subscriberBankBorderValueMultiplier;
			}
		}

		// Token: 0x17000C8F RID: 3215
		// (get) Token: 0x06003795 RID: 14229 RVA: 0x00066047 File Offset: 0x00064247
		public float RegularBankBorderValueMultiplier
		{
			get
			{
				return this.m_regularBankBorderValueMultiplier;
			}
		}

		// Token: 0x17000C90 RID: 3216
		// (get) Token: 0x06003796 RID: 14230 RVA: 0x0006604F File Offset: 0x0006424F
		public int EventCostDiscount
		{
			get
			{
				return 100 - this.m_eventCost;
			}
		}

		// Token: 0x06003797 RID: 14231 RVA: 0x0006605A File Offset: 0x0006425A
		public bool TryGetSubscriberCost(uint cost, out uint modifiedCost)
		{
			modifiedCost = cost;
			if (this.m_eventCost < 100)
			{
				modifiedCost = (uint)Mathf.Clamp(Mathf.FloorToInt((float)((ulong)cost * (ulong)((long)this.m_eventCost)) / 100f), 1, int.MaxValue);
				return true;
			}
			return false;
		}

		// Token: 0x17000C91 RID: 3217
		// (get) Token: 0x06003798 RID: 14232 RVA: 0x0006608F File Offset: 0x0006428F
		public ScriptableSubscriptionPerks DefaultSubscriberPerks
		{
			get
			{
				return this.m_defaultSubscriberPerks;
			}
		}

		// Token: 0x04003675 RID: 13941
		public const string kGenericSubscriberOnly = "This is reserved for subscribers.";

		// Token: 0x04003676 RID: 13942
		public const string kEmoteCannotUse = "This emote is reserved for subscribers. <link=\"activateSub\"><u>Activate your subscription.</u></link>";

		// Token: 0x04003677 RID: 13943
		public const string kSubscriberPortraitTooltip = "This portrait is reserved for subscribers.";

		// Token: 0x04003678 RID: 13944
		public const string kSubscriberBanksSlotTooltip = "This slot is reserved for subscribers.";

		// Token: 0x04003679 RID: 13945
		public const string kSubscriberFollowFeature = "The follow feature is reserved for subscribers.";

		// Token: 0x0400367A RID: 13946
		[SerializeField]
		private Color m_subscriberColor = Color.white;

		// Token: 0x0400367B RID: 13947
		[SerializeField]
		private ConsumableItemEmberRingEnhancment m_emberRingEnhancment;

		// Token: 0x0400367C RID: 13948
		[SerializeField]
		private DeployPortableStationAbility m_deployPortableCraftingStationAbility;

		// Token: 0x0400367D RID: 13949
		[Range(0f, 1f)]
		[SerializeField]
		private float m_subscriberBankBorderValueMultiplier = 0.5f;

		// Token: 0x0400367E RID: 13950
		[Range(0f, 1f)]
		[SerializeField]
		private float m_regularBankBorderValueMultiplier = 1f;

		// Token: 0x0400367F RID: 13951
		private const int kMaxEventCost = 100;

		// Token: 0x04003680 RID: 13952
		[Range(0f, 100f)]
		[SerializeField]
		private int m_eventCost = 100;

		// Token: 0x04003681 RID: 13953
		[SerializeField]
		private ScriptableSubscriptionPerks m_defaultSubscriberPerks;
	}
}
