using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Flanking;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ACA RID: 2762
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/Base Role")]
	public class BaseRole : CombatMasteryArchetype
	{
		// Token: 0x170013AC RID: 5036
		// (get) Token: 0x06005549 RID: 21833 RVA: 0x00078F19 File Offset: 0x00077119
		private static Color CombatRoleIconColor
		{
			get
			{
				return new Color(0.7921569f, 0.77254903f, 0.76862746f);
			}
		}

		// Token: 0x170013AD RID: 5037
		// (get) Token: 0x0600554A RID: 21834 RVA: 0x00078F2F File Offset: 0x0007712F
		public override Color IconTint
		{
			get
			{
				if (base.Type != MasteryType.Combat)
				{
					return base.IconTint;
				}
				if (!this.m_overrideIconTint)
				{
					return BaseRole.CombatRoleIconColor;
				}
				return this.m_iconTintOverride;
			}
		}

		// Token: 0x170013AE RID: 5038
		// (get) Token: 0x0600554B RID: 21835 RVA: 0x00078F55 File Offset: 0x00077155
		public RoleFlankingBonus FlankingBonus
		{
			get
			{
				return this.m_flankingBonus;
			}
		}

		// Token: 0x170013AF RID: 5039
		// (get) Token: 0x0600554C RID: 21836 RVA: 0x00078F5D File Offset: 0x0007715D
		public override bool HasSpecializations
		{
			get
			{
				return this.m_specializations != null && this.m_specializations.Length != 0;
			}
		}

		// Token: 0x170013B0 RID: 5040
		// (get) Token: 0x0600554D RID: 21837 RVA: 0x00078F73 File Offset: 0x00077173
		public SpecializedRole[] Specializations
		{
			get
			{
				return this.m_specializations;
			}
		}

		// Token: 0x170013B1 RID: 5041
		// (get) Token: 0x0600554E RID: 21838 RVA: 0x00078F7B File Offset: 0x0007717B
		public string CreationDescription
		{
			get
			{
				return this.m_creationDescription;
			}
		}

		// Token: 0x170013B2 RID: 5042
		// (get) Token: 0x0600554F RID: 21839 RVA: 0x00078F83 File Offset: 0x00077183
		public ScriptableCombatEffect OffhandAutoAttackCombatEffect
		{
			get
			{
				BaseRole.OffHandAutoAttackData offHandAutoAttack = this.m_offHandAutoAttack;
				if (offHandAutoAttack == null)
				{
					return null;
				}
				return offHandAutoAttack.CombatEffect;
			}
		}

		// Token: 0x06005550 RID: 21840 RVA: 0x00078F96 File Offset: 0x00077196
		public bool ShouldPerformOffhandAutoAttack(GameEntity entity, float level)
		{
			return this.m_offHandAutoAttack != null && this.m_offHandAutoAttack.ShouldPerform(entity, level);
		}

		// Token: 0x06005551 RID: 21841 RVA: 0x001DE544 File Offset: 0x001DC744
		public override bool MeetsHandheldRequirements(GameEntity entity)
		{
			HandheldFlagConfig handheldFlagConfig = entity.GetHandheldFlagConfig();
			return this.MeetsHandheldRequirements(handheldFlagConfig);
		}

		// Token: 0x06005552 RID: 21842 RVA: 0x001DE560 File Offset: 0x001DC760
		public override bool MeetsHandheldRequirements(HandheldFlagConfig config)
		{
			for (int i = 0; i < this.m_compatibleWeaponConfigs.Length; i++)
			{
				if (this.m_compatibleWeaponConfigs[i].WeaponConfig.MeetsRequirements(config))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005553 RID: 21843 RVA: 0x001DE598 File Offset: 0x001DC798
		public override bool HasCompatibleWeapons(GameEntity entity, out string compatibleWeaponString)
		{
			compatibleWeaponString = string.Empty;
			HandheldFlagConfig? handheldFlagConfig = null;
			if (entity)
			{
				handheldFlagConfig = new HandheldFlagConfig?(entity.HandHeldItemCache.GetHandheldFlagConfig());
			}
			bool flag = false;
			TooltipExtensions.ToCombine.Clear();
			for (int i = 0; i < this.m_compatibleWeaponConfigs.Length; i++)
			{
				bool flag2 = handheldFlagConfig != null && this.m_compatibleWeaponConfigs[i].WeaponConfig.MeetsRequirements(handheldFlagConfig.Value);
				flag = (flag || flag2);
				string item = flag2 ? ZString.Format<string, string>("<color={0}>{1}</color>", UIManager.RequirementsMetColor.ToHex(), this.m_compatibleWeaponConfigs[i].Description) : this.m_compatibleWeaponConfigs[i].Description;
				TooltipExtensions.ToCombine.Add(item);
			}
			Color color = flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			compatibleWeaponString = ZString.Format<string, string, string, string>("{0}<color={1}>You must have one of these configs equipped to use this ability:</color>\n{2}{3}", "<i><size=80%>", color.ToHex(), string.Join(", ", TooltipExtensions.ToCombine), "</size></i>");
			return flag;
		}

		// Token: 0x06005554 RID: 21844 RVA: 0x001DE698 File Offset: 0x001DC898
		public void AddStartingEquipment(CharacterRecord record)
		{
			float baseLevel = (GameManager.IsServer && ServerGameManager.GameServerConfig != null) ? ((float)ServerGameManager.GameServerConfig.StartingLevel) : 1f;
			ArchetypeInstance archetypeInstance = this.CreateNewInstance();
			archetypeInstance.MasteryData.BaseLevel = baseLevel;
			record.Storage[ContainerType.Masteries].Instances.Add(archetypeInstance);
			for (int i = 0; i < this.m_creationAbilities.Length; i++)
			{
				if (!(this.m_creationAbilities[i].Mastery != this))
				{
					ArchetypeInstance archetypeInstance2 = this.m_creationAbilities[i].CreateNewInstance();
					archetypeInstance2.Index = i;
					archetypeInstance2.AbilityData.MemorizationTimestamp = new DateTime?(DateTime.MinValue);
					record.Storage[ContainerType.Abilities].Instances.Add(archetypeInstance2);
				}
			}
			for (int j = 0; j < this.m_creationItems.Length; j++)
			{
				this.m_creationItems[j].AddToRecord(record, new UniqueId?(base.Id));
			}
		}

		// Token: 0x06005555 RID: 21845 RVA: 0x00078FAF File Offset: 0x000771AF
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			if (base.Type == MasteryType.Combat)
			{
				BaseRole.OffHandAutoAttackData offHandAutoAttack = this.m_offHandAutoAttack;
				if (offHandAutoAttack != null)
				{
					offHandAutoAttack.FillTooltipBlock(tooltip, instance, entity);
				}
				RoleFlankingBonus flankingBonus = this.m_flankingBonus;
				if (flankingBonus == null)
				{
					return;
				}
				flankingBonus.FillTooltip(tooltip);
			}
		}

		// Token: 0x04004BBF RID: 19391
		private const string kSpecReferences = "Specialization References";

		// Token: 0x04004BC0 RID: 19392
		private const string kOffhandAutoAttack = "Offhand Auto Attack";

		// Token: 0x04004BC1 RID: 19393
		[SerializeField]
		private bool m_overrideIconTint;

		// Token: 0x04004BC2 RID: 19394
		[SerializeField]
		private Color m_iconTintOverride = BaseRole.CombatRoleIconColor;

		// Token: 0x04004BC3 RID: 19395
		[SerializeField]
		private SpecializedRole[] m_specializations;

		// Token: 0x04004BC4 RID: 19396
		[SerializeField]
		private RoleFlankingBonus m_flankingBonus;

		// Token: 0x04004BC5 RID: 19397
		[SerializeField]
		private BaseRole.CompatibleWeaponConfig[] m_compatibleWeaponConfigs;

		// Token: 0x04004BC6 RID: 19398
		[SerializeField]
		private BaseRole.OffHandAutoAttackData m_offHandAutoAttack;

		// Token: 0x04004BC7 RID: 19399
		private const string kCreationGroupName = "Creation";

		// Token: 0x04004BC8 RID: 19400
		[TextArea(4, 10)]
		[SerializeField]
		private string m_creationDescription = string.Empty;

		// Token: 0x04004BC9 RID: 19401
		[SerializeField]
		private AbilityArchetype[] m_creationAbilities;

		// Token: 0x04004BCA RID: 19402
		[SerializeField]
		private BaseRole.CreationItem[] m_creationItems;

		// Token: 0x02000ACB RID: 2763
		[Serializable]
		private class CompatibleWeaponConfig
		{
			// Token: 0x170013B3 RID: 5043
			// (get) Token: 0x06005557 RID: 21847 RVA: 0x00079006 File Offset: 0x00077206
			public HandheldItemRequirement WeaponConfig
			{
				get
				{
					return this.m_weaponConfig;
				}
			}

			// Token: 0x170013B4 RID: 5044
			// (get) Token: 0x06005558 RID: 21848 RVA: 0x0007900E File Offset: 0x0007720E
			public string Description
			{
				get
				{
					return this.m_description;
				}
			}

			// Token: 0x04004BCB RID: 19403
			[SerializeField]
			private HandheldItemRequirement m_weaponConfig;

			// Token: 0x04004BCC RID: 19404
			[SerializeField]
			private string m_description;
		}

		// Token: 0x02000ACC RID: 2764
		[Serializable]
		private class OffHandAutoAttackData
		{
			// Token: 0x170013B5 RID: 5045
			// (get) Token: 0x0600555A RID: 21850 RVA: 0x00079016 File Offset: 0x00077216
			public ScriptableOffHandAutoAttack CombatEffect
			{
				get
				{
					return this.m_combatEffect;
				}
			}

			// Token: 0x170013B6 RID: 5046
			// (get) Token: 0x0600555B RID: 21851 RVA: 0x0007901E File Offset: 0x0007721E
			private IEnumerable GetScriptableCombatEffects
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<ScriptableOffHandAutoAttack>();
				}
			}

			// Token: 0x0600555C RID: 21852 RVA: 0x001DE78C File Offset: 0x001DC98C
			public bool ShouldPerform(GameEntity entity, float level)
			{
				return this.m_hasOffhandAutoAttack && this.m_combatEffect && this.m_weaponConfig.MeetsRequirements(entity) && UnityEngine.Random.Range(0f, 1f) <= this.m_chance.Evaluate(level);
			}

			// Token: 0x0600555D RID: 21853 RVA: 0x001DE7E0 File Offset: 0x001DC9E0
			public void FillTooltipBlock(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
			{
				if (this.m_hasOffhandAutoAttack)
				{
					TooltipTextBlock effectsBlock = tooltip.EffectsBlock;
					bool flag;
					if (instance != null && entity)
					{
						int associatedLevelInteger = instance.GetAssociatedLevelInteger(entity);
						string asPercentage = this.m_chance.Evaluate((float)associatedLevelInteger).GetAsPercentage();
						effectsBlock.AppendLine(ZString.Format<string>("{0}% chance to perform an off hand attack.", asPercentage), 0);
						flag = UIManager.TooltipShowMore;
					}
					else
					{
						effectsBlock.AppendLine("Off hand attack chance:", 0);
						flag = true;
					}
					if (flag)
					{
						string asPercentage2 = this.m_chance.Evaluate(1f).GetAsPercentage();
						string asPercentage3 = this.m_chance.Evaluate(50f).GetAsPercentage();
						effectsBlock.AppendLine(ZString.Format<string, int>("  {0}% at level {1}", asPercentage2, 1), 0);
						effectsBlock.AppendLine(ZString.Format<string, int>("  {0}% at level {1}", asPercentage3, 50), 0);
					}
				}
			}

			// Token: 0x04004BCD RID: 19405
			[SerializeField]
			private bool m_hasOffhandAutoAttack;

			// Token: 0x04004BCE RID: 19406
			[SerializeField]
			private ScriptableOffHandAutoAttack m_combatEffect;

			// Token: 0x04004BCF RID: 19407
			[SerializeField]
			private AnimationCurve m_chance = AnimationCurve.Linear(0f, 0.05f, 50f, 0.6f);

			// Token: 0x04004BD0 RID: 19408
			[SerializeField]
			private HandheldItemRequirement m_weaponConfig;
		}

		// Token: 0x02000ACD RID: 2765
		[Serializable]
		private class CreationItem
		{
			// Token: 0x170013B7 RID: 5047
			// (get) Token: 0x0600555F RID: 21855 RVA: 0x0007904C File Offset: 0x0007724C
			public string IndexName
			{
				get
				{
					if (!(this.m_item == null))
					{
						return this.m_item.DisplayName;
					}
					return "NONE";
				}
			}

			// Token: 0x170013B8 RID: 5048
			// (get) Token: 0x06005560 RID: 21856 RVA: 0x0007906D File Offset: 0x0007726D
			private bool m_showTargetSlot
			{
				get
				{
					return this.m_targetContainerType == ContainerType.Equipment;
				}
			}

			// Token: 0x170013B9 RID: 5049
			// (get) Token: 0x06005561 RID: 21857 RVA: 0x00079078 File Offset: 0x00077278
			private bool m_showCount
			{
				get
				{
					return this.m_itemHasCount || this.m_itemHasCharges;
				}
			}

			// Token: 0x170013BA RID: 5050
			// (get) Token: 0x06005562 RID: 21858 RVA: 0x0007908A File Offset: 0x0007728A
			private bool m_itemHasCount
			{
				get
				{
					return this.m_item != null && this.m_item.ArchetypeHasCount();
				}
			}

			// Token: 0x170013BB RID: 5051
			// (get) Token: 0x06005563 RID: 21859 RVA: 0x000790A7 File Offset: 0x000772A7
			private bool m_itemHasCharges
			{
				get
				{
					return this.m_item != null && this.m_item.ArchetypeHasCharges();
				}
			}

			// Token: 0x06005564 RID: 21860 RVA: 0x000790C4 File Offset: 0x000772C4
			private string GetTitle()
			{
				if (!this.m_itemHasCharges)
				{
					return "Count";
				}
				return "Charges";
			}

			// Token: 0x06005565 RID: 21861 RVA: 0x00077B72 File Offset: 0x00075D72
			private IEnumerable GetItems()
			{
				return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
			}

			// Token: 0x06005566 RID: 21862 RVA: 0x001DE8AC File Offset: 0x001DCAAC
			public void AddToRecord(CharacterRecord record, UniqueId? roleId)
			{
				if (record == null || this.m_item == null || this.m_targetContainerType == ContainerType.None)
				{
					return;
				}
				ArchetypeInstance archetypeInstance = this.m_item.CreateNewInstance();
				archetypeInstance.ItemData.ItemFlags = (this.m_itemFlags | ItemFlags.NoSale);
				archetypeInstance.ItemData.AssociatedMasteryId = roleId;
				if (this.m_item.ArchetypeHasCount())
				{
					archetypeInstance.ItemData.Count = new int?(this.m_quantity);
				}
				else if (this.m_item.ArchetypeHasCharges())
				{
					archetypeInstance.ItemData.Charges = new int?(this.m_quantity);
				}
				if (this.m_soulbound)
				{
					archetypeInstance.ItemData.MarkAsSoulbound(record);
				}
				ContainerRecord containerRecord = record.Storage[this.m_targetContainerType];
				bool flag = false;
				if (this.m_targetContainerType == ContainerType.Equipment)
				{
					archetypeInstance.Index = (int)this.m_targetSlot;
					flag = true;
				}
				else
				{
					int maxCapacity = this.m_targetContainerType.GetMaxCapacity();
					for (int i = 0; i < maxCapacity; i++)
					{
						bool flag2 = false;
						using (List<ArchetypeInstance>.Enumerator enumerator = containerRecord.Instances.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.Index == i)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							archetypeInstance.Index = i;
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					containerRecord.Instances.Add(archetypeInstance);
				}
			}

			// Token: 0x04004BD1 RID: 19409
			[SerializeField]
			private ItemArchetype m_item;

			// Token: 0x04004BD2 RID: 19410
			[SerializeField]
			private bool m_soulbound;

			// Token: 0x04004BD3 RID: 19411
			[SerializeField]
			private ItemFlags m_itemFlags;

			// Token: 0x04004BD4 RID: 19412
			[SerializeField]
			private ContainerType m_targetContainerType;

			// Token: 0x04004BD5 RID: 19413
			[SerializeField]
			private int m_quantity = 1;

			// Token: 0x04004BD6 RID: 19414
			[SerializeField]
			private EquipmentSlot m_targetSlot;
		}
	}
}
