using System;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE5 RID: 2789
	[Serializable]
	public class WeaponProfile : IWeaponProfile
	{
		// Token: 0x170013EE RID: 5102
		// (get) Token: 0x060055F7 RID: 22007 RVA: 0x00079557 File Offset: 0x00077757
		private StatType[] m_validDamageTypes
		{
			get
			{
				return StatTypeExtensions.ValidDamageTypes;
			}
		}

		// Token: 0x170013EF RID: 5103
		// (get) Token: 0x060055F8 RID: 22008 RVA: 0x0007955E File Offset: 0x0007775E
		private bool m_showCustomDamageTypeOption
		{
			get
			{
				return this.m_weaponType == WeaponTypes.None;
			}
		}

		// Token: 0x170013F0 RID: 5104
		// (get) Token: 0x060055F9 RID: 22009 RVA: 0x00079569 File Offset: 0x00077769
		private bool m_showCustomDamageType
		{
			get
			{
				return this.m_showCustomDamageTypeOption && this.m_useCustomDamageType;
			}
		}

		// Token: 0x170013F1 RID: 5105
		// (get) Token: 0x060055FA RID: 22010 RVA: 0x0007957B File Offset: 0x0007777B
		private bool m_showOffHandDamageMultiplier
		{
			get
			{
				return this.m_weaponType.IsOneHandedMelee();
			}
		}

		// Token: 0x060055FB RID: 22011 RVA: 0x00079588 File Offset: 0x00077788
		public StatType GetDamageType()
		{
			if (!this.m_showCustomDamageType)
			{
				return this.m_weaponType.GetWeaponDamageType();
			}
			return this.m_customDamageType;
		}

		// Token: 0x170013F2 RID: 5106
		// (get) Token: 0x060055FC RID: 22012 RVA: 0x000795A4 File Offset: 0x000777A4
		public WeaponTypes WeaponType
		{
			get
			{
				return this.m_weaponType;
			}
		}

		// Token: 0x170013F3 RID: 5107
		// (get) Token: 0x060055FD RID: 22013 RVA: 0x000795AC File Offset: 0x000777AC
		public DamageType DamageType
		{
			get
			{
				return this.m_damageType;
			}
		}

		// Token: 0x170013F4 RID: 5108
		// (get) Token: 0x060055FE RID: 22014 RVA: 0x000795B4 File Offset: 0x000777B4
		public MinMaxFloatRange Distance
		{
			get
			{
				return this.m_distance;
			}
		}

		// Token: 0x170013F5 RID: 5109
		// (get) Token: 0x060055FF RID: 22015 RVA: 0x000795BC File Offset: 0x000777BC
		public int Angle
		{
			get
			{
				return this.m_angle;
			}
		}

		// Token: 0x170013F6 RID: 5110
		// (get) Token: 0x06005600 RID: 22016 RVA: 0x000795C4 File Offset: 0x000777C4
		public float AoeRadius
		{
			get
			{
				return this.m_aoeRadius;
			}
		}

		// Token: 0x170013F7 RID: 5111
		// (get) Token: 0x06005601 RID: 22017 RVA: 0x000795CC File Offset: 0x000777CC
		public float AoeAngle
		{
			get
			{
				return this.m_aoeAngle;
			}
		}

		// Token: 0x170013F8 RID: 5112
		// (get) Token: 0x06005602 RID: 22018 RVA: 0x000795D4 File Offset: 0x000777D4
		public float OffHandDamageMultiplier
		{
			get
			{
				if (!this.m_showOffHandDamageMultiplier)
				{
					return 1f;
				}
				return this.m_offHandDamageMultiplier;
			}
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x001E0214 File Offset: 0x001DE414
		public void CopyValuesFrom(IWeaponProfile other)
		{
			this.m_weaponType = other.WeaponType;
			this.m_damageType = other.DamageType;
			this.m_distance = new MinMaxFloatRange(other.Distance.Min, other.Distance.Max);
			this.m_angle = other.Angle;
			this.m_aoeRadius = other.AoeRadius;
			this.m_aoeAngle = other.AoeAngle;
			this.m_offHandDamageMultiplier = other.OffHandDamageMultiplier;
		}

		// Token: 0x06005604 RID: 22020 RVA: 0x000795EA File Offset: 0x000777EA
		public bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.Angle || assignerName - ComponentEffectAssignerName.MinimumDistance <= 1;
		}

		// Token: 0x06005605 RID: 22021 RVA: 0x001E0290 File Offset: 0x001DE490
		public bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.Angle)
			{
				this.m_angle = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_angle);
				return true;
			}
			if (assignerName == ComponentEffectAssignerName.MinimumDistance)
			{
				this.m_distance = new MinMaxFloatRange(ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_distance.Min), this.m_distance.Max);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.MaximumDistance)
			{
				return false;
			}
			this.m_distance = new MinMaxFloatRange(this.m_distance.Min, ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_distance.Max));
			return true;
		}

		// Token: 0x04004C35 RID: 19509
		public const string kWeaponProfileGroupName = "Weapon Profile";

		// Token: 0x04004C36 RID: 19510
		[SerializeField]
		private WeaponTypes m_weaponType;

		// Token: 0x04004C37 RID: 19511
		[SerializeField]
		private bool m_useCustomDamageType;

		// Token: 0x04004C38 RID: 19512
		[SerializeField]
		private StatType m_customDamageType;

		// Token: 0x04004C39 RID: 19513
		[SerializeField]
		private DamageType m_damageType;

		// Token: 0x04004C3A RID: 19514
		[SerializeField]
		private MinMaxFloatRange m_distance = new MinMaxFloatRange(0f, 5f);

		// Token: 0x04004C3B RID: 19515
		[SerializeField]
		private int m_angle = 40;

		// Token: 0x04004C3C RID: 19516
		[SerializeField]
		private float m_aoeRadius = 3f;

		// Token: 0x04004C3D RID: 19517
		[SerializeField]
		private float m_aoeAngle = 10f;

		// Token: 0x04004C3E RID: 19518
		[SerializeField]
		private float m_offHandDamageMultiplier = 1f;
	}
}
