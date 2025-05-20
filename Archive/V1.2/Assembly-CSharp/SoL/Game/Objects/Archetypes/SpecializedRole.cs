using System;
using System.Collections;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ADB RID: 2779
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/Specialized Role")]
	public class SpecializedRole : BaseArchetype, IStatModifier
	{
		// Token: 0x170013D3 RID: 5075
		// (get) Token: 0x060055AA RID: 21930 RVA: 0x0004479C File Offset: 0x0004299C
		public override ArchetypeIconType IconShape
		{
			get
			{
				return ArchetypeIconType.Circle;
			}
		}

		// Token: 0x170013D4 RID: 5076
		// (get) Token: 0x060055AB RID: 21931 RVA: 0x000792C8 File Offset: 0x000774C8
		public override Color IconTint
		{
			get
			{
				if (!this.m_baseRole)
				{
					return base.IconTint;
				}
				return this.m_baseRole.IconTint;
			}
		}

		// Token: 0x170013D5 RID: 5077
		// (get) Token: 0x060055AC RID: 21932 RVA: 0x000792E9 File Offset: 0x000774E9
		public bool DisallowContextMenuLearning
		{
			get
			{
				return this.m_disallowContextMenuLearning;
			}
		}

		// Token: 0x170013D6 RID: 5078
		// (get) Token: 0x060055AD RID: 21933 RVA: 0x000792F1 File Offset: 0x000774F1
		public VitalScalingValue[] VitalModifiers
		{
			get
			{
				return this.m_vitalModifiers;
			}
		}

		// Token: 0x170013D7 RID: 5079
		// (get) Token: 0x060055AE RID: 21934 RVA: 0x000792F9 File Offset: 0x000774F9
		public AbilityArchetype[] Abilities
		{
			get
			{
				return this.m_abilities;
			}
		}

		// Token: 0x170013D8 RID: 5080
		// (get) Token: 0x060055AF RID: 21935 RVA: 0x00079301 File Offset: 0x00077501
		public string CreationDescription
		{
			get
			{
				return this.m_creationDescription;
			}
		}

		// Token: 0x170013D9 RID: 5081
		// (get) Token: 0x060055B0 RID: 21936 RVA: 0x000792F1 File Offset: 0x000774F1
		VitalScalingValue[] IStatModifier.Vitals
		{
			get
			{
				return this.m_vitalModifiers;
			}
		}

		// Token: 0x170013DA RID: 5082
		// (get) Token: 0x060055B1 RID: 21937 RVA: 0x00079309 File Offset: 0x00077509
		StatModifierScaling[] IStatModifier.Stats
		{
			get
			{
				return this.m_statModifiers;
			}
		}

		// Token: 0x170013DB RID: 5083
		// (get) Token: 0x060055B2 RID: 21938 RVA: 0x00079311 File Offset: 0x00077511
		public BaseRole GeneralRole
		{
			get
			{
				return this.m_baseRole;
			}
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x001DF410 File Offset: 0x001DD610
		public bool EntityHasCompatibleWeapons(IHandHeldItems handHeldItems)
		{
			if (handHeldItems == null)
			{
				throw new ArgumentNullException("handHeldItems");
			}
			HandheldFlagConfig handheldFlagConfig = handHeldItems.GetHandheldFlagConfig();
			return this.m_baseRole.MeetsHandheldRequirements(handheldFlagConfig);
		}

		// Token: 0x060055B4 RID: 21940 RVA: 0x00065EE9 File Offset: 0x000640E9
		private IEnumerable GetBaseRoles()
		{
			return SolOdinUtilities.GetDropdownItems<BaseRole>();
		}

		// Token: 0x060055B5 RID: 21941 RVA: 0x001DF440 File Offset: 0x001DD640
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			base.FillTooltipBlocks(tooltip, instance, entity);
			bool flag = false;
			int? level = null;
			int? num = null;
			ArchetypeInstance archetypeInstance = null;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_baseRole.Id, out archetypeInstance) && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null)
			{
				level = new int?(Mathf.FloorToInt(archetypeInstance.MasteryData.BaseLevel));
				flag = true;
				if (archetypeInstance.MasteryData.Specialization.Value == base.Id)
				{
					num = new int?(Mathf.FloorToInt(archetypeInstance.MasteryData.SpecializationLevel));
				}
			}
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			if (flag)
			{
				if (num != null)
				{
					if (num.Value == level.Value)
					{
						dataBlock.AppendLine("Level", num.Value.ToString());
					}
					else
					{
						dataBlock.AppendLine("Base Role Level", level.Value.ToString());
						dataBlock.AppendLine("Specialization Level", num.Value.ToString());
					}
					dataBlock.AppendLine("", 0);
				}
			}
			else
			{
				dataBlock.AppendLine("Unlocks at level " + Mathf.FloorToInt(6f).ToString(), 0);
			}
			IStatModifierExtensions.FillTooltipBlock(dataBlock, level, this.m_vitalModifiers);
			IStatModifierExtensions.FillTooltipBlock(dataBlock, level, this.m_statModifiers);
			if (level != null && num != null && level.Value != num.Value)
			{
				dataBlock.AppendLine("", 0);
				dataBlock.AppendLine("<size=80%><i>Base Role Level</i> determines stat distribution while <i>Specialization Level</i> determines Ability power.</size>", 0);
			}
			if (archetypeInstance != null && archetypeInstance.MasteryData != null && (archetypeInstance.MasteryData.Specialization == null || archetypeInstance.MasteryData.Specialization.Value != base.Id))
			{
				dataBlock = tooltip.DataBlock;
				dataBlock.AppendLine("", 0);
				dataBlock.AppendLine("Included Abilities:", 0);
				AbilityArchetype[] abilitiesSortedByLevel = this.GetAbilitiesSortedByLevel();
				for (int i = 0; i < abilitiesSortedByLevel.Length; i++)
				{
					dataBlock.AppendLine(string.Concat(new string[]
					{
						"<sprite=\"SolIcons\" name=\"Circle\" tint=1> ",
						abilitiesSortedByLevel[i].DisplayName,
						" [LVL ",
						abilitiesSortedByLevel[i].MinimumLevel.ToString(),
						"]"
					}), 0);
					dataBlock.AppendLine(abilitiesSortedByLevel[i].Description.Italicize(), 0);
				}
			}
		}

		// Token: 0x060055B6 RID: 21942 RVA: 0x001DF734 File Offset: 0x001DD934
		private AbilityArchetype[] GetAbilitiesSortedByLevel()
		{
			if (this.m_sortedAbilities == null)
			{
				this.m_sortedAbilities = new AbilityArchetype[this.m_abilities.Length];
				for (int i = 0; i < this.m_abilities.Length; i++)
				{
					this.m_sortedAbilities[i] = this.m_abilities[i];
				}
				Array.Sort<AbilityArchetype>(this.m_sortedAbilities, (AbilityArchetype a, AbilityArchetype b) => a.MinimumLevel.CompareTo(b.MinimumLevel));
			}
			return this.m_sortedAbilities;
		}

		// Token: 0x04004C07 RID: 19463
		private const string kListElementLabelName = "IndexName";

		// Token: 0x04004C08 RID: 19464
		private const string kModifiersGroupName = "Modifiers";

		// Token: 0x04004C09 RID: 19465
		private const string kPassiveModifiersGroupName = "Modifiers/Passive";

		// Token: 0x04004C0A RID: 19466
		private const string kActiveModifiersGroupName = "Modifiers/Active (combat stance)";

		// Token: 0x04004C0B RID: 19467
		[SerializeField]
		private bool m_disallowContextMenuLearning;

		// Token: 0x04004C0C RID: 19468
		[FormerlySerializedAs("m_generalRole")]
		[SerializeField]
		private BaseRole m_baseRole;

		// Token: 0x04004C0D RID: 19469
		[SerializeField]
		private DummyClass m_dumDum;

		// Token: 0x04004C0E RID: 19470
		[SerializeField]
		private VitalScalingValue[] m_vitalModifiers;

		// Token: 0x04004C0F RID: 19471
		[SerializeField]
		private StatModifierScaling[] m_statModifiers;

		// Token: 0x04004C10 RID: 19472
		[SerializeField]
		private AbilityArchetype[] m_abilities;

		// Token: 0x04004C11 RID: 19473
		[TextArea(4, 10)]
		[SerializeField]
		private string m_creationDescription = string.Empty;

		// Token: 0x04004C12 RID: 19474
		[NonSerialized]
		private AbilityArchetype[] m_sortedAbilities;
	}
}
