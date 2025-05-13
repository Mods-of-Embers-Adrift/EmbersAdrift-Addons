using System;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE2 RID: 2786
	[CreateAssetMenu(menuName = "SoL/Profiles/Weapon")]
	public class ScriptableWeaponProfile : BaseArchetype, IWeaponProfile
	{
		// Token: 0x060055E2 RID: 21986 RVA: 0x00079491 File Offset: 0x00077691
		public StatType GetDamageType()
		{
			return this.m_profile.GetDamageType();
		}

		// Token: 0x170013E7 RID: 5095
		// (get) Token: 0x060055E3 RID: 21987 RVA: 0x0007949E File Offset: 0x0007769E
		public WeaponTypes WeaponType
		{
			get
			{
				return this.m_profile.WeaponType;
			}
		}

		// Token: 0x170013E8 RID: 5096
		// (get) Token: 0x060055E4 RID: 21988 RVA: 0x000794AB File Offset: 0x000776AB
		public DamageType DamageType
		{
			get
			{
				return this.m_profile.DamageType;
			}
		}

		// Token: 0x170013E9 RID: 5097
		// (get) Token: 0x060055E5 RID: 21989 RVA: 0x000794B8 File Offset: 0x000776B8
		public MinMaxFloatRange Distance
		{
			get
			{
				return this.m_profile.Distance;
			}
		}

		// Token: 0x170013EA RID: 5098
		// (get) Token: 0x060055E6 RID: 21990 RVA: 0x000794C5 File Offset: 0x000776C5
		public int Angle
		{
			get
			{
				return this.m_profile.Angle;
			}
		}

		// Token: 0x170013EB RID: 5099
		// (get) Token: 0x060055E7 RID: 21991 RVA: 0x000794D2 File Offset: 0x000776D2
		public float AoeRadius
		{
			get
			{
				return this.m_profile.AoeRadius;
			}
		}

		// Token: 0x170013EC RID: 5100
		// (get) Token: 0x060055E8 RID: 21992 RVA: 0x000794DF File Offset: 0x000776DF
		public float AoeAngle
		{
			get
			{
				return this.m_profile.AoeAngle;
			}
		}

		// Token: 0x170013ED RID: 5101
		// (get) Token: 0x060055E9 RID: 21993 RVA: 0x000794EC File Offset: 0x000776EC
		public float OffHandDamageMultiplier
		{
			get
			{
				return this.m_profile.OffHandDamageMultiplier;
			}
		}

		// Token: 0x060055EA RID: 21994 RVA: 0x000794F9 File Offset: 0x000776F9
		public void CopyValuesFrom(IWeaponProfile other)
		{
			this.m_profile.CopyValuesFrom(other);
		}

		// Token: 0x04004C30 RID: 19504
		[SerializeField]
		private WeaponProfile m_profile = new WeaponProfile();
	}
}
