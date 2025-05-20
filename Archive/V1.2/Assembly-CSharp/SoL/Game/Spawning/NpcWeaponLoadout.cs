using System;
using System.Collections;
using SoL.Game.Animation;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000687 RID: 1671
	[Serializable]
	public class NpcWeaponLoadout
	{
		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x06003399 RID: 13209 RVA: 0x000637B7 File Offset: 0x000619B7
		public EquipableItem MainHand
		{
			get
			{
				return this.m_mainHand;
			}
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x0600339A RID: 13210 RVA: 0x000637BF File Offset: 0x000619BF
		public EquipableItem OffHand
		{
			get
			{
				return this.m_offHand;
			}
		}

		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x0600339B RID: 13211 RVA: 0x000637C7 File Offset: 0x000619C7
		public LightItem LightItem
		{
			get
			{
				return this.m_light;
			}
		}

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x0600339C RID: 13212 RVA: 0x000637CF File Offset: 0x000619CF
		public RangedAmmoItem RangedAmmo
		{
			get
			{
				return this.m_rangedAmmo;
			}
		}

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x0600339D RID: 13213 RVA: 0x000637D7 File Offset: 0x000619D7
		public AnimancerAnimationSet AnimationSet
		{
			get
			{
				return this.m_combatStance;
			}
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x000636B9 File Offset: 0x000618B9
		private IEnumerable GetEquipable()
		{
			return SolOdinUtilities.GetDropdownItems<EquipableItem>();
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x000636C0 File Offset: 0x000618C0
		private IEnumerable GetRangedAmmo()
		{
			return SolOdinUtilities.GetDropdownItems<RangedAmmoItem>();
		}

		// Token: 0x060033A0 RID: 13216 RVA: 0x000637DF File Offset: 0x000619DF
		private IEnumerable GetLightItem()
		{
			return SolOdinUtilities.GetDropdownItems<LightItem>();
		}

		// Token: 0x060033A1 RID: 13217 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetAnimSets()
		{
			return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
		}

		// Token: 0x0400319D RID: 12701
		[SerializeField]
		private EquipableItem m_mainHand;

		// Token: 0x0400319E RID: 12702
		[SerializeField]
		private EquipableItem m_offHand;

		// Token: 0x0400319F RID: 12703
		[SerializeField]
		private RangedAmmoItem m_rangedAmmo;

		// Token: 0x040031A0 RID: 12704
		[SerializeField]
		private LightItem m_light;

		// Token: 0x040031A1 RID: 12705
		[SerializeField]
		private AnimancerAnimationSet m_combatStance;
	}
}
