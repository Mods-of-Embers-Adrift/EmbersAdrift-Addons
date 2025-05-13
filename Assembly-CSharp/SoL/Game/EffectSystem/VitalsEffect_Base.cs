using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C1A RID: 3098
	[Serializable]
	public abstract class VitalsEffect_Base
	{
		// Token: 0x06005F89 RID: 24457 RVA: 0x001FA5AC File Offset: 0x001F87AC
		public virtual string GetTitleText()
		{
			string result = string.Empty;
			EffectResourceType resourceType = this.m_resourceType;
			if (resourceType <= EffectResourceType.Stamina)
			{
				if (resourceType != EffectResourceType.Health)
				{
					if (resourceType == EffectResourceType.Stamina)
					{
						result = (this.m_modifyWounds ? "Modify Stamina Wounds" : "Modify Stamina");
					}
				}
				else if (this.m_modifyWounds)
				{
					result = "Modify Health Wounds";
				}
				else if (this.m_valueIsHealthFraction)
				{
					result = "Modify Health FRACTION";
				}
				else
				{
					result = "Modify Health";
				}
			}
			else if (resourceType != EffectResourceType.Armor)
			{
				if (resourceType == EffectResourceType.Threat)
				{
					result = "Modify Threat";
				}
			}
			else
			{
				result = "Modify Armor Class";
			}
			return result;
		}

		// Token: 0x170016C8 RID: 5832
		// (get) Token: 0x06005F8A RID: 24458 RVA: 0x00080506 File Offset: 0x0007E706
		protected virtual EffectResourceType[] m_validResourceTypes
		{
			get
			{
				return new EffectResourceType[]
				{
					EffectResourceType.Health,
					EffectResourceType.Stamina,
					EffectResourceType.Armor,
					EffectResourceType.Threat
				};
			}
		}

		// Token: 0x170016C9 RID: 5833
		// (get) Token: 0x06005F8B RID: 24459 RVA: 0x00080519 File Offset: 0x0007E719
		public EffectResourceType ResourceType
		{
			get
			{
				return this.m_resourceType;
			}
		}

		// Token: 0x170016CA RID: 5834
		// (get) Token: 0x06005F8C RID: 24460 RVA: 0x00080521 File Offset: 0x0007E721
		public bool ValueIsHealthFraction
		{
			get
			{
				return this.m_resourceType == EffectResourceType.Health && this.m_valueIsHealthFraction && !this.m_modifyWounds;
			}
		}

		// Token: 0x170016CB RID: 5835
		// (get) Token: 0x06005F8D RID: 24461 RVA: 0x0008053E File Offset: 0x0007E73E
		public bool ModifyWounds
		{
			get
			{
				return this.m_modifyWounds && this.m_resourceType.HasWounds();
			}
		}

		// Token: 0x170016CC RID: 5836
		// (get) Token: 0x06005F8E RID: 24462 RVA: 0x00080555 File Offset: 0x0007E755
		public bool ApplyHealthFractionBonus
		{
			get
			{
				return this.AllowHealthFractionBonus && this.m_applyHealthFractionBonus;
			}
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x001FA630 File Offset: 0x001F8830
		public int GetSignMultiplier(bool isPositive)
		{
			if (this.m_modifyWounds)
			{
				if (!isPositive)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				EffectResourceType resourceType = this.m_resourceType;
				if (resourceType != EffectResourceType.Armor)
				{
					if (resourceType == EffectResourceType.Threat)
					{
						if (!isPositive)
						{
							return 1;
						}
						return -1;
					}
					else
					{
						if (!isPositive)
						{
							return -1;
						}
						return 1;
					}
				}
				else
				{
					if (!isPositive)
					{
						return 1;
					}
					return -1;
				}
			}
		}

		// Token: 0x170016CD RID: 5837
		// (get) Token: 0x06005F90 RID: 24464
		protected abstract bool AllowHealthFractionBonus { get; }

		// Token: 0x170016CE RID: 5838
		// (get) Token: 0x06005F91 RID: 24465 RVA: 0x00080567 File Offset: 0x0007E767
		private bool m_showCustomCurve
		{
			get
			{
				return this.ApplyHealthFractionBonus && this.m_useCustomCurve;
			}
		}

		// Token: 0x170016CF RID: 5839
		// (get) Token: 0x06005F92 RID: 24466 RVA: 0x00080579 File Offset: 0x0007E779
		private bool m_showValueIsHealthFraction
		{
			get
			{
				return this.m_resourceType == EffectResourceType.Health;
			}
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x001FA674 File Offset: 0x001F8874
		public float? GetHealthFractionBonus(float healthFraction)
		{
			if (this.ApplyHealthFractionBonus)
			{
				return new float?(this.m_useCustomCurve ? this.m_customHealthFractionBonusCurve.Evaluate(healthFraction) : GlobalSettings.Values.Combat.GetHealthFractionBonus(healthFraction));
			}
			return null;
		}

		// Token: 0x04005279 RID: 21113
		protected const string kVitalsBaseGroup = "VitalsBase";

		// Token: 0x0400527A RID: 21114
		[SerializeField]
		protected EffectResourceType m_resourceType;

		// Token: 0x0400527B RID: 21115
		[SerializeField]
		protected bool m_modifyWounds;

		// Token: 0x0400527C RID: 21116
		[Tooltip("If selected then the value will be the health FRACTION, does NOTHING if ModifyWounds=true")]
		[SerializeField]
		private bool m_valueIsHealthFraction;

		// Token: 0x0400527D RID: 21117
		[SerializeField]
		private bool m_applyHealthFractionBonus;

		// Token: 0x0400527E RID: 21118
		[SerializeField]
		private bool m_useCustomCurve;

		// Token: 0x0400527F RID: 21119
		[Tooltip("X-Axis is health fraction.  Y-Axis is bonus %")]
		[SerializeField]
		private AnimationCurve m_customHealthFractionBonusCurve;
	}
}
