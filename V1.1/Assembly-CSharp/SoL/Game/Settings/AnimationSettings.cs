using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.Dueling;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000718 RID: 1816
	[Serializable]
	public class AnimationSettings
	{
		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x0600367B RID: 13947 RVA: 0x0006557E File Offset: 0x0006377E
		private IEnumerable GetAnimationSetPairs
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSetPair>();
			}
		}

		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x0600367C RID: 13948 RVA: 0x00065585 File Offset: 0x00063785
		private IEnumerable GetAnimationPoses
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationPose>();
			}
		}

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x0600367D RID: 13949 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetAnimationSets
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
			}
		}

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x0600367E RID: 13950 RVA: 0x0006558C File Offset: 0x0006378C
		private IEnumerable GetHumanoidWeaps
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<HumanoidWeaponAnimationSets>();
			}
		}

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x0600367F RID: 13951 RVA: 0x00065593 File Offset: 0x00063793
		public Emote DuelRollEmote
		{
			get
			{
				return this.m_duelRollEmote;
			}
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06003680 RID: 13952 RVA: 0x0006559B File Offset: 0x0006379B
		public Emote DuelLossEmote
		{
			get
			{
				return this.m_duelLossEmote;
			}
		}

		// Token: 0x06003681 RID: 13953 RVA: 0x00169B94 File Offset: 0x00167D94
		public bool TryGetDuelWinEmote(DuelRoll duelRoll, out Emote winEmote)
		{
			winEmote = null;
			if (this.m_duelWinEmotes != null)
			{
				int num = new System.Random(duelRoll.RollCount + (int)(duelRoll.SourceId * duelRoll.OpponentId)).Next(this.m_duelWinEmotes.Length);
				winEmote = this.m_duelWinEmotes[num];
			}
			return winEmote != null;
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x06003682 RID: 13954 RVA: 0x000655A3 File Offset: 0x000637A3
		public GameObject BookPrefab
		{
			get
			{
				return this.m_bookPrefab;
			}
		}

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x06003683 RID: 13955 RVA: 0x000655AB File Offset: 0x000637AB
		public GameObject WaterTrailPrefab
		{
			get
			{
				return this.m_waterTrailPrefab;
			}
		}

		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x06003684 RID: 13956 RVA: 0x000655B3 File Offset: 0x000637B3
		public Emote[] DefaultEmotes
		{
			get
			{
				return this.m_defaultEmotes;
			}
		}

		// Token: 0x06003685 RID: 13957 RVA: 0x00169BE8 File Offset: 0x00167DE8
		public bool TryGetDefaultEmote(UniqueId emoteId, out Emote emote)
		{
			if (this.m_defaultEmoteDict == null)
			{
				this.m_defaultEmoteDict = new Dictionary<UniqueId, Emote>(default(UniqueIdComparer));
				for (int i = 0; i < this.m_defaultEmotes.Length; i++)
				{
					if (this.m_defaultEmotes[i] != null)
					{
						this.m_defaultEmoteDict.Add(this.m_defaultEmotes[i].Id, this.m_defaultEmotes[i]);
					}
				}
			}
			return this.m_defaultEmoteDict.TryGetValue(emoteId, out emote);
		}

		// Token: 0x0400346E RID: 13422
		public AnimancerAnimationSetPair IdleSetPair;

		// Token: 0x0400346F RID: 13423
		public AnimancerAnimationSet FallbackCombatSet;

		// Token: 0x04003470 RID: 13424
		public AnimancerAnimationPose TorchPose;

		// Token: 0x04003471 RID: 13425
		public float MinTimeBetweenHitAnims = 5f;

		// Token: 0x04003472 RID: 13426
		public float MovementLerpRate = 3f;

		// Token: 0x04003473 RID: 13427
		public const float kUpperRotationZeroThreshold = 1f;

		// Token: 0x04003474 RID: 13428
		public const float kLowerRotationZeroThreshold = -1f;

		// Token: 0x04003475 RID: 13429
		public float RotationLerpRate = 3f;

		// Token: 0x04003476 RID: 13430
		public float RotationLerpRateToZeroMultiplier = 2f;

		// Token: 0x04003477 RID: 13431
		public MinMaxFloatRange IdleTickRate = new MinMaxFloatRange(15f, 45f);

		// Token: 0x04003478 RID: 13432
		public HumanoidWeaponAnimationSets HumanoidWeaponAnimationSets;

		// Token: 0x04003479 RID: 13433
		[SerializeField]
		private Emote m_duelRollEmote;

		// Token: 0x0400347A RID: 13434
		[SerializeField]
		private Emote m_duelLossEmote;

		// Token: 0x0400347B RID: 13435
		[SerializeField]
		private Emote[] m_duelWinEmotes;

		// Token: 0x0400347C RID: 13436
		[SerializeField]
		private GameObject m_bookPrefab;

		// Token: 0x0400347D RID: 13437
		[SerializeField]
		private GameObject m_waterTrailPrefab;

		// Token: 0x0400347E RID: 13438
		[SerializeField]
		private Emote[] m_defaultEmotes;

		// Token: 0x0400347F RID: 13439
		private Dictionary<UniqueId, Emote> m_defaultEmoteDict;

		// Token: 0x04003480 RID: 13440
		[SerializeField]
		private AnimancerAnimationSet[] m_referenceLocoSets;
	}
}
