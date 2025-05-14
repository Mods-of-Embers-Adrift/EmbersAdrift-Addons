using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD1 RID: 2769
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/Mastery")]
	public class MasteryArchetype : BaseArchetype, IStatModifier
	{
		// Token: 0x170013C0 RID: 5056
		// (get) Token: 0x06005576 RID: 21878 RVA: 0x0007910A File Offset: 0x0007730A
		public AutoAttackAbility AutoAttackOverride
		{
			get
			{
				return this.m_autoAttackOverride;
			}
		}

		// Token: 0x170013C1 RID: 5057
		// (get) Token: 0x06005577 RID: 21879 RVA: 0x00079112 File Offset: 0x00077312
		public bool InvertListing
		{
			get
			{
				return this.m_invertListing;
			}
		}

		// Token: 0x06005578 RID: 21880 RVA: 0x001DEBAC File Offset: 0x001DCDAC
		public List<AbilityArchetype> GetAbilitiesSortedByLevel()
		{
			if (this.m_sortedAbilities == null)
			{
				this.m_sortedAbilities = new List<AbilityArchetype>(this.m_abilities.Length * 2);
			}
			else
			{
				this.m_sortedAbilities.Clear();
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null)
			{
				for (int i = 0; i < LocalPlayer.GameEntity.CollectionController.Abilities.Count; i++)
				{
					ArchetypeInstance index = LocalPlayer.GameEntity.CollectionController.Abilities.GetIndex(i);
					if (index != null && index.Ability && index.Ability.Mastery == this)
					{
						this.m_sortedAbilities.Add(index.Ability);
					}
				}
			}
			else
			{
				for (int j = 0; j < this.m_abilities.Length; j++)
				{
					this.m_sortedAbilities.Add(this.m_abilities[j]);
				}
			}
			this.m_sortedAbilities.Sort(delegate(AbilityArchetype a, AbilityArchetype b)
			{
				int num = a.MinimumLevel.CompareTo(b.MinimumLevel);
				if (num != 0)
				{
					return num;
				}
				return a.DisplayName.CompareTo(b.DisplayName);
			});
			return this.m_sortedAbilities;
		}

		// Token: 0x170013C2 RID: 5058
		// (get) Token: 0x06005579 RID: 21881 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HasSpecializations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170013C3 RID: 5059
		// (get) Token: 0x0600557A RID: 21882 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual StatModifierScaling[] Stats
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170013C4 RID: 5060
		// (get) Token: 0x0600557B RID: 21883 RVA: 0x0007911A File Offset: 0x0007731A
		VitalScalingValue[] IStatModifier.Vitals
		{
			get
			{
				return this.m_vitalModifiers;
			}
		}

		// Token: 0x170013C5 RID: 5061
		// (get) Token: 0x0600557C RID: 21884 RVA: 0x00079122 File Offset: 0x00077322
		StatModifierScaling[] IStatModifier.Stats
		{
			get
			{
				return this.Stats;
			}
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x0007912A File Offset: 0x0007732A
		private IEnumerable GetAutoAttacks()
		{
			return SolOdinUtilities.GetDropdownItems<AutoAttackAbility>();
		}

		// Token: 0x170013C6 RID: 5062
		// (get) Token: 0x0600557E RID: 21886 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_dynamicallyLoaded
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170013C7 RID: 5063
		// (get) Token: 0x0600557F RID: 21887 RVA: 0x00079131 File Offset: 0x00077331
		public bool DynamicallyLoaded
		{
			get
			{
				return this.m_dynamicallyLoaded;
			}
		}

		// Token: 0x170013C8 RID: 5064
		// (get) Token: 0x06005580 RID: 21888 RVA: 0x00079139 File Offset: 0x00077339
		private bool m_showStats
		{
			get
			{
				return this.m_type.ContributeStats();
			}
		}

		// Token: 0x170013C9 RID: 5065
		// (get) Token: 0x06005581 RID: 21889 RVA: 0x00079146 File Offset: 0x00077346
		private bool m_showClassification
		{
			get
			{
				return this.m_type != MasteryType.Trade && this.m_type != MasteryType.Harvesting;
			}
		}

		// Token: 0x170013CA RID: 5066
		// (get) Token: 0x06005582 RID: 21890 RVA: 0x0007915F File Offset: 0x0007735F
		public MasteryType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x170013CB RID: 5067
		// (get) Token: 0x06005583 RID: 21891 RVA: 0x0004479C File Offset: 0x0004299C
		public override ArchetypeIconType IconShape
		{
			get
			{
				return ArchetypeIconType.Circle;
			}
		}

		// Token: 0x170013CC RID: 5068
		// (get) Token: 0x06005584 RID: 21892 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ChangeFrameColor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170013CD RID: 5069
		// (get) Token: 0x06005585 RID: 21893 RVA: 0x00079167 File Offset: 0x00077367
		public override Color FrameColor
		{
			get
			{
				return this.m_type.GetMasteryColor();
			}
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x00079174 File Offset: 0x00077374
		public float PointsPerSuccess(float currentLevel)
		{
			return this.GetSingleSuccess(currentLevel) * this.m_modifier;
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x00079184 File Offset: 0x00077384
		private float GetSingleSuccess(float current)
		{
			return 1f / this.GetSuccessesPerIncrease(current);
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x00079193 File Offset: 0x00077393
		private float GetSuccessesPerIncrease(float current)
		{
			return Mathf.Max(this.GetValue(current), 1f);
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x0006108D File Offset: 0x0005F28D
		private float GetValue(float current)
		{
			return 0f;
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x001DECD0 File Offset: 0x001DCED0
		public float GetCurrentLevel(GameEntity entity)
		{
			float result = 0f;
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.TryGetInstance(ContainerType.Masteries, out containerInstance) && containerInstance.TryGetInstanceForArchetypeId(base.Id, out archetypeInstance))
			{
				result = archetypeInstance.GetAssociatedLevel(entity);
			}
			return result;
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x001DED10 File Offset: 0x001DCF10
		public bool IsAtCorrectCraftingStation(GameEntity entity)
		{
			if ((this.m_type == MasteryType.Harvesting || this.m_type == MasteryType.Trade) && (entity.CollectionController.RefinementStation || this.m_showWithoutCraftingStation))
			{
				InteractiveRefinementStation refinementStation = entity.CollectionController.RefinementStation;
				return ((refinementStation != null) ? refinementStation.Profile : null) == null || this.m_stationCategory.HasBitFlag(entity.CollectionController.RefinementStation.Profile.Category);
			}
			return false;
		}

		// Token: 0x170013CE RID: 5070
		// (get) Token: 0x0600558C RID: 21900 RVA: 0x001DED8C File Offset: 0x001DCF8C
		private static Dictionary<MasteryType, ArchetypeInstance> MasteryTypeLevelDict
		{
			get
			{
				if (MasteryArchetype.m_masteryTypeLevelDict == null)
				{
					MasteryArchetype.m_masteryTypeLevelDict = new Dictionary<MasteryType, ArchetypeInstance>(default(MasteryTypeComparer));
				}
				return MasteryArchetype.m_masteryTypeLevelDict;
			}
		}

		// Token: 0x0600558D RID: 21901 RVA: 0x001DEDC0 File Offset: 0x001DCFC0
		public static void RefreshHighestLevelMastery(GameEntity entity)
		{
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Type != GameEntityType.Player || entity.CollectionController == null || entity.CollectionController.Masteries == null)
			{
				return;
			}
			if (MasteryArchetype.MasteryTypeLevelDict.Count > 0)
			{
				MasteryArchetype.MasteryTypeLevelDict.Clear();
			}
			for (int i = 0; i < entity.CollectionController.Masteries.Count; i++)
			{
				ArchetypeInstance instanceForListIndex = entity.CollectionController.Masteries.GetInstanceForListIndex(i);
				MasteryArchetype.UpdateMasteryTypeLevel(entity, instanceForListIndex);
			}
			if (entity.CharacterData)
			{
				entity.CharacterData.SetHighestMasteryLevels(MasteryArchetype.MasteryTypeLevelDict);
			}
			MasteryArchetype.MasteryTypeLevelDict.Clear();
		}

		// Token: 0x0600558E RID: 21902 RVA: 0x001DEE74 File Offset: 0x001DD074
		public static void RefreshHighestLevelMastery(GameEntity entity, List<ArchetypeInstance> instances)
		{
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Type != GameEntityType.Player || instances == null || instances.Count <= 0)
			{
				return;
			}
			MasteryArchetype.MasteryTypeLevelDict.Clear();
			for (int i = 0; i < instances.Count; i++)
			{
				MasteryArchetype.UpdateMasteryTypeLevel(entity, instances[i]);
			}
			if (entity.CharacterData)
			{
				entity.CharacterData.SetHighestMasteryLevels(MasteryArchetype.MasteryTypeLevelDict);
			}
		}

		// Token: 0x0600558F RID: 21903 RVA: 0x001DEEF0 File Offset: 0x001DD0F0
		private static void UpdateMasteryTypeLevel(GameEntity entity, ArchetypeInstance instance)
		{
			MasteryArchetype masteryArchetype;
			if (instance == null || !instance.Archetype || !instance.Archetype.TryGetAsType(out masteryArchetype))
			{
				return;
			}
			ArchetypeInstance archetypeInstance;
			if (MasteryArchetype.MasteryTypeLevelDict.TryGetValue(masteryArchetype.Type, out archetypeInstance))
			{
				if (archetypeInstance != null && archetypeInstance.GetAssociatedLevel(entity) < instance.GetAssociatedLevel(entity))
				{
					MasteryArchetype.MasteryTypeLevelDict[masteryArchetype.Type] = instance;
					return;
				}
			}
			else
			{
				MasteryArchetype.MasteryTypeLevelDict.Add(masteryArchetype.Type, instance);
			}
		}

		// Token: 0x06005590 RID: 21904 RVA: 0x000791A6 File Offset: 0x000773A6
		public static int GetRequiredMasteryTotalToLearn(int masterySphereCount)
		{
			return (masterySphereCount - 1) * masterySphereCount * 10;
		}

		// Token: 0x06005591 RID: 21905 RVA: 0x001DEF68 File Offset: 0x001DD168
		public static int GetMaximumForMasteryLevel(MasterySphere masterySphere, int masteryLevel)
		{
			int maximumForSphere = masterySphere.GetMaximumForSphere();
			for (int i = 1; i <= maximumForSphere; i++)
			{
				int requiredMasteryTotalToLearn = MasteryArchetype.GetRequiredMasteryTotalToLearn(i);
				if (masteryLevel < requiredMasteryTotalToLearn)
				{
					return i;
				}
			}
			return maximumForSphere;
		}

		// Token: 0x06005592 RID: 21906 RVA: 0x001DEF98 File Offset: 0x001DD198
		public static void AddDynamicMasteries(ContainerInstance masteryContainerInstance)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (MasteryArchetype.m_dynamicMasteries == null)
			{
				MasteryArchetype.m_dynamicMasteries = new List<MasteryArchetype>(10);
				using (IEnumerator<BaseArchetype> enumerator = InternalGameDatabase.Archetypes.GetAllItems().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MasteryArchetype masteryArchetype;
						if (enumerator.Current.TryGetAsType(out masteryArchetype) && masteryArchetype.m_dynamicallyLoaded)
						{
							MasteryArchetype.m_dynamicMasteries.Add(masteryArchetype);
						}
					}
				}
			}
			for (int i = 0; i < MasteryArchetype.m_dynamicMasteries.Count; i++)
			{
				ArchetypeInstance archetypeInstance;
				if (!masteryContainerInstance.TryGetInstanceForArchetypeId(MasteryArchetype.m_dynamicMasteries[i].Id, out archetypeInstance))
				{
					ArchetypeInstance archetypeInstance2 = MasteryArchetype.m_dynamicMasteries[i].CreateNewInstance();
					archetypeInstance2.MasteryData.BaseLevel = 1f;
					masteryContainerInstance.Add(archetypeInstance2, true);
				}
			}
		}

		// Token: 0x170013CF RID: 5071
		// (get) Token: 0x06005593 RID: 21907 RVA: 0x00062532 File Offset: 0x00060732
		public override ArchetypeCategory Category
		{
			get
			{
				return ArchetypeCategory.Mastery;
			}
		}

		// Token: 0x06005594 RID: 21908 RVA: 0x000791B0 File Offset: 0x000773B0
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.MasteryData = new MasteryInstanceData();
		}

		// Token: 0x06005595 RID: 21909 RVA: 0x001DF074 File Offset: 0x001DD274
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			base.FillTooltipBlocks(tooltip, instance, entity);
			int num = Mathf.FloorToInt(this.GetCurrentLevel(entity));
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			dataBlock.AppendLine("Sphere", this.m_type.GetMasterySphere().ToString());
			dataBlock.AppendLine("Type", this.m_type.ToString().Color(this.m_type.GetMasteryColor()));
			if (this.m_classification != MasteryClassification.None)
			{
				dataBlock.AppendLine("Classification", this.m_classification.ToString());
			}
			dataBlock.AppendLine("Level", num.ToString());
			int? level = null;
			if (num > 0 && instance != null)
			{
				level = new int?(num);
			}
			IStatModifierExtensions.FillTooltipBlock(dataBlock, level, this.m_vitalModifiers);
		}

		// Token: 0x04004BDD RID: 19421
		protected const string kListElementLabelName = "IndexName";

		// Token: 0x04004BDE RID: 19422
		protected const string kModifiersGroupName = "Modifiers";

		// Token: 0x04004BDF RID: 19423
		protected const string kPassiveModifiersGroupName = "Modifiers/Passive";

		// Token: 0x04004BE0 RID: 19424
		protected const string kCraftingGroupName = "Crafting";

		// Token: 0x04004BE1 RID: 19425
		public const int kMasteryLevelPoolCost = 1;

		// Token: 0x04004BE2 RID: 19426
		[SerializeField]
		private MasteryType m_type;

		// Token: 0x04004BE3 RID: 19427
		[SerializeField]
		private float m_modifier = 1f;

		// Token: 0x04004BE4 RID: 19428
		[SerializeField]
		private DummyClass m_dumDum;

		// Token: 0x04004BE5 RID: 19429
		[SerializeField]
		protected VitalScalingValue[] m_vitalModifiers;

		// Token: 0x04004BE6 RID: 19430
		[SerializeField]
		private MasteryClassification m_classification;

		// Token: 0x04004BE7 RID: 19431
		[Tooltip("NOT YET IMPLEMENTED PROPERLY, ARCHETYPE COLLECTION HAS OVERLAP DUE TO STATIC ID")]
		[SerializeField]
		private AutoAttackAbility m_autoAttackOverride;

		// Token: 0x04004BE8 RID: 19432
		private const string kAbilityRefGroup = "Ability References";

		// Token: 0x04004BE9 RID: 19433
		[SerializeField]
		private AbilityArchetype[] m_abilities;

		// Token: 0x04004BEA RID: 19434
		[Tooltip("What stations should show this mastery. Crafting stations without profiles will always show all harvesting/trade masteries.")]
		[SerializeField]
		private CraftingStationCategory m_stationCategory;

		// Token: 0x04004BEB RID: 19435
		[Tooltip("Whether the Crafting UI should show this mastery if the player is not at a crafting station.")]
		[SerializeField]
		private bool m_showWithoutCraftingStation = true;

		// Token: 0x04004BEC RID: 19436
		[Tooltip("Whether the Crafting UI should place the tab for this mastery starting from the bottom of the window instead of the top.")]
		[SerializeField]
		private bool m_invertListing;

		// Token: 0x04004BED RID: 19437
		[NonSerialized]
		private List<AbilityArchetype> m_sortedAbilities;

		// Token: 0x04004BEE RID: 19438
		private static Dictionary<MasteryType, ArchetypeInstance> m_masteryTypeLevelDict;

		// Token: 0x04004BEF RID: 19439
		private static List<MasteryArchetype> m_dynamicMasteries;
	}
}
