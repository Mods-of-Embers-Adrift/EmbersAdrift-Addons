using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000377 RID: 887
	public class ArchetypeTooltip : BaseTooltip
	{
		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001856 RID: 6230 RVA: 0x0005317C File Offset: 0x0005137C
		private bool m_showNestedOffset
		{
			get
			{
				return this.m_nestedTooltip != null;
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001857 RID: 6231 RVA: 0x0005318A File Offset: 0x0005138A
		// (set) Token: 0x06001858 RID: 6232 RVA: 0x00053192 File Offset: 0x00051392
		public TooltipTextBlock DataBlock { get; private set; }

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06001859 RID: 6233 RVA: 0x0005319B File Offset: 0x0005139B
		// (set) Token: 0x0600185A RID: 6234 RVA: 0x000531A3 File Offset: 0x000513A3
		public TooltipTextBlock StatsBlock { get; private set; }

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x0600185B RID: 6235 RVA: 0x000531AC File Offset: 0x000513AC
		// (set) Token: 0x0600185C RID: 6236 RVA: 0x000531B4 File Offset: 0x000513B4
		public TooltipTextBlock MaterialBlock { get; private set; }

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x0600185D RID: 6237 RVA: 0x000531BD File Offset: 0x000513BD
		// (set) Token: 0x0600185E RID: 6238 RVA: 0x000531C5 File Offset: 0x000513C5
		public TooltipTextBlock ReagentBlock { get; private set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x0600185F RID: 6239 RVA: 0x000531CE File Offset: 0x000513CE
		// (set) Token: 0x06001860 RID: 6240 RVA: 0x000531D6 File Offset: 0x000513D6
		public TooltipTextBlock CombatBlock { get; private set; }

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06001861 RID: 6241 RVA: 0x000531DF File Offset: 0x000513DF
		// (set) Token: 0x06001862 RID: 6242 RVA: 0x000531E7 File Offset: 0x000513E7
		public TooltipTextBlock EffectsBlock { get; private set; }

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x000531F0 File Offset: 0x000513F0
		// (set) Token: 0x06001864 RID: 6244 RVA: 0x000531F8 File Offset: 0x000513F8
		public TooltipTextBlock SubEffectsBlock { get; private set; }

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06001865 RID: 6245 RVA: 0x00053201 File Offset: 0x00051401
		// (set) Token: 0x06001866 RID: 6246 RVA: 0x00053209 File Offset: 0x00051409
		public TooltipTextBlock RequirementsBlock { get; private set; }

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06001867 RID: 6247 RVA: 0x00053212 File Offset: 0x00051412
		// (set) Token: 0x06001868 RID: 6248 RVA: 0x0005321A File Offset: 0x0005141A
		public TooltipTextBlock DescriptionBlock { get; private set; }

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06001869 RID: 6249 RVA: 0x00053223 File Offset: 0x00051423
		// (set) Token: 0x0600186A RID: 6250 RVA: 0x0005322A File Offset: 0x0005142A
		public static bool IsAlchemyOnlyTooltip { get; private set; }

		// Token: 0x0600186B RID: 6251 RVA: 0x00053232 File Offset: 0x00051432
		public void AddLineToLeftSubHeader(string txt)
		{
			ArchetypeTooltip.SubHeader leftSubHeader = this.m_leftSubHeader;
			if (leftSubHeader == null)
			{
				return;
			}
			leftSubHeader.AddLineToSubHeader(txt);
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x00053245 File Offset: 0x00051445
		public void AddLineToRightSubHeader(string txt)
		{
			this.m_rightSubHeader.AddLineToSubHeader(txt);
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x0600186D RID: 6253 RVA: 0x00053253 File Offset: 0x00051453
		public static ArchetypeTooltipParameter? CurrentArchetypeParameter
		{
			get
			{
				return ArchetypeTooltip.m_currentArchetypeParameter;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0005325A File Offset: 0x0005145A
		// (set) Token: 0x0600186F RID: 6255 RVA: 0x00053262 File Offset: 0x00051462
		public bool IsApplied { get; private set; }

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001870 RID: 6256 RVA: 0x0005326B File Offset: 0x0005146B
		// (set) Token: 0x06001871 RID: 6257 RVA: 0x00053273 File Offset: 0x00051473
		public bool ShowChance { get; private set; }

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06001872 RID: 6258 RVA: 0x0005327C File Offset: 0x0005147C
		public bool ShowTargeting
		{
			get
			{
				return !this.IsApplied;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001873 RID: 6259 RVA: 0x0004479C File Offset: 0x0004299C
		public bool ShowInstant
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001874 RID: 6260 RVA: 0x0004479C File Offset: 0x0004299C
		public bool ShowConditionals
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001875 RID: 6261 RVA: 0x00053287 File Offset: 0x00051487
		// (set) Token: 0x06001876 RID: 6262 RVA: 0x0005328F File Offset: 0x0005148F
		protected override DynamicUIWindow.PivotCorner CurrentPivotCorner
		{
			get
			{
				return this.m_pivotCorner;
			}
			set
			{
				if (value == this.m_pivotCorner)
				{
					return;
				}
				this.m_pivotCorner = value;
				this.UpdateNestedTooltipTransform();
			}
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x001040DC File Offset: 0x001022DC
		protected override void Start()
		{
			base.Start();
			for (int i = 0; i < this.m_blocks.Length; i++)
			{
				if (this.m_blocks[i])
				{
					switch (i)
					{
					case 0:
						this.DataBlock = this.m_blocks[i];
						break;
					case 1:
						this.StatsBlock = this.m_blocks[i];
						break;
					case 2:
						this.MaterialBlock = this.m_blocks[i];
						break;
					case 3:
						this.ReagentBlock = this.m_blocks[i];
						break;
					case 4:
						this.CombatBlock = this.m_blocks[i];
						break;
					case 5:
						this.EffectsBlock = this.m_blocks[i];
						break;
					case 6:
						this.SubEffectsBlock = this.m_blocks[i];
						break;
					case 7:
						this.RequirementsBlock = this.m_blocks[i];
						break;
					case 8:
						this.DescriptionBlock = this.m_blocks[i];
						break;
					}
				}
			}
			if (this.m_currencyBlock != null)
			{
				this.m_currencyBlock.InitCoinDisplay(false, true, true, false);
			}
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x001041F8 File Offset: 0x001023F8
		protected override void SetData()
		{
			ArchetypeTooltip.IsAlchemyOnlyTooltip = false;
			BaseTooltip.GetTooltipParameter parameterGetter = this.m_parameterGetter;
			ITooltipParameter tooltipParameter = (parameterGetter != null) ? parameterGetter() : null;
			ArchetypeTooltip.m_currentArchetypeParameter = null;
			if (tooltipParameter == null)
			{
				return;
			}
			if (tooltipParameter is ArchetypeTooltipParameter)
			{
				this.IsApplied = false;
				this.ShowChance = true;
				this.SetArchtypeData(tooltipParameter);
				return;
			}
			if (tooltipParameter is EffectSyncDataTooltipParameter)
			{
				this.IsApplied = true;
				this.ShowChance = false;
				this.SetEffectSyncData(tooltipParameter);
			}
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0004475B File Offset: 0x0004295B
		protected void SetDurability(ArchetypeInstance instance)
		{
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x00104268 File Offset: 0x00102468
		private void SetArchtypeData(ITooltipParameter param)
		{
			ArchetypeTooltipParameter archetypeTooltipParameter = (ArchetypeTooltipParameter)param;
			ArchetypeTooltip.m_currentArchetypeParameter = new ArchetypeTooltipParameter?(archetypeTooltipParameter);
			ArchetypeInstance instance = archetypeTooltipParameter.Instance;
			BaseArchetype baseArchetype = (instance == null) ? archetypeTooltipParameter.Archetype : instance.Archetype;
			if (baseArchetype == null)
			{
				return;
			}
			this.ResetSubHeaders();
			this.ResetBlocks();
			this.SetDurability(instance);
			AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None;
			string text = baseArchetype.GetModifiedDisplayName(instance);
			string text2 = baseArchetype.Description;
			AbilityArchetype abilityArchetype;
			if (instance != null && instance.Archetype && instance.Archetype.TryGetAsType(out abilityArchetype))
			{
				if (GameManager.AllowAlchemy)
				{
					if (archetypeTooltipParameter.AlchemyPowerLevel != null)
					{
						alchemyPowerLevel = archetypeTooltipParameter.AlchemyPowerLevel.Value;
						ArchetypeTooltip.IsAlchemyOnlyTooltip = true;
					}
					else if (instance.Index != -1 && AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.II) && AlchemyExtensions.AlchemyPowerLevelAvailable(LocalPlayer.GameEntity, instance, AlchemyPowerLevel.II))
					{
						alchemyPowerLevel = AlchemyPowerLevel.II;
					}
					else if (instance.Index != -1 && AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.I) && AlchemyExtensions.AlchemyPowerLevelAvailable(LocalPlayer.GameEntity, instance, AlchemyPowerLevel.I))
					{
						alchemyPowerLevel = AlchemyPowerLevel.I;
					}
					text = abilityArchetype.GetAlchemyDisplayName(alchemyPowerLevel);
					text2 = abilityArchetype.GetAlchemyDescription(alchemyPowerLevel);
					if (alchemyPowerLevel != AlchemyPowerLevel.None)
					{
						this.AddLineToLeftSubHeader(ZString.Format<string, string>("<color={0}>{1}</color>", UIManager.EmberColor.ToHex(), alchemyPowerLevel.GetAlchemyPowerLevelDescription()));
					}
				}
				string tierDisplayLevel = abilityArchetype.GetTierDisplayLevel(instance);
				if (!string.IsNullOrEmpty(tierDisplayLevel))
				{
					text = ZString.Format<string, string>("{0} {1}", text, tierDisplayLevel);
				}
			}
			if (baseArchetype.ItemCategory)
			{
				if (baseArchetype.ItemCategory.ColorFlag.HasBitFlag(ItemCategory.ColorFlags.DisplayName))
				{
					text = ZString.Format<string, string>("<color={0}>{1}</color>", baseArchetype.ItemCategory.Color.ToHex(), text);
				}
				if (!string.IsNullOrEmpty(baseArchetype.ItemCategory.Description))
				{
					string description = baseArchetype.ItemCategory.Description;
					text = (baseArchetype.ItemCategory.ColorFlag.HasBitFlag(ItemCategory.ColorFlags.Description) ? ZString.Format<string, string, string>("{0}\n<size=16px><color={1}><i>{2}</i></color></size>", text, baseArchetype.ItemCategory.Color.ToHex(), description) : ZString.Format<string, string>("{0}\n<size=16px><i>{1}</i></size>", text, description));
				}
			}
			this.m_title.ZStringSetText(text);
			this.m_archetypeIcon.SetIcon(baseArchetype, new Color?(baseArchetype.GetInstanceColor(instance)));
			if (!string.IsNullOrEmpty(baseArchetype.SubHeaderText))
			{
				this.m_leftSubHeader.AddLineToSubHeader(ZString.Format<string>("<i>{0}</i>", baseArchetype.SubHeaderText));
			}
			this.m_leftSubHeader.AddLineToSubHeader(archetypeTooltipParameter.SubHeadingText);
			this.SetTitles();
			if (!string.IsNullOrEmpty(text2))
			{
				this.DescriptionBlock.Sb.Append("<i><color=#00ffff>" + text2 + "</color></i>");
			}
			if (!string.IsNullOrEmpty(archetypeTooltipParameter.AdditionalText))
			{
				this.DataBlock.AppendLine(archetypeTooltipParameter.AdditionalText, 0);
			}
			if (baseArchetype is AbilityArchetype)
			{
				baseArchetype.FillAbilityTooltipBlocks(this, instance, LocalPlayer.GameEntity, alchemyPowerLevel);
			}
			else
			{
				baseArchetype.FillTooltipBlocks(this, instance, LocalPlayer.GameEntity);
			}
			if (archetypeTooltipParameter.AtMerchant && ClientGameManager.UIManager != null && ClientGameManager.UIManager.MerchantUI != null && ClientGameManager.UIManager.MerchantUI.Interactive != null && ClientGameManager.UIManager.MerchantUI.Interactive.ItemFlagsToSet != ItemFlags.None)
			{
				this.DataBlock.AppendLine("Flags:", ClientGameManager.UIManager.MerchantUI.Interactive.ItemFlagsToSet.GetColoredItemFlags());
			}
			if (UIManager.TooltipShowMore && baseArchetype.ItemCategory)
			{
				if (baseArchetype.ItemCategory.OverrideProgressionDescription)
				{
					TooltipTextBlock descriptionBlock = this.DescriptionBlock;
					if (descriptionBlock.Sb.Length > 0)
					{
						descriptionBlock.AppendLine("", 0);
					}
					string text3 = baseArchetype.ItemCategory.Description.Italicize().Underline().Bold();
					text3 = ZString.Format<string, string>("{0}: <i>{1}</i>", text3, baseArchetype.ItemCategory.ProgressionDescription);
					if (baseArchetype.ItemCategory.ColorFlag.HasBitFlag(ItemCategory.ColorFlags.Description))
					{
						text3 = text3.Color(baseArchetype.ItemCategory.Color);
					}
					descriptionBlock.Append(text3, 0);
				}
				else if (GlobalSettings.Values && GlobalSettings.Values.UI != null)
				{
					bool flag = instance != null && instance.ItemData != null && instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.Crafted) && GlobalSettings.Values.UI.CraftedCategoryOrder.Contains(baseArchetype.ItemCategory);
					List<ItemCategory> list = flag ? GlobalSettings.Values.UI.CraftedCategoryOrder : GlobalSettings.Values.UI.CategoryOrder;
					if (list != null && list.Contains(baseArchetype.ItemCategory))
					{
						string txt = ZString.Format<string>(" <size=70%>{0}</size> ", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>");
						TooltipTextBlock descriptionBlock2 = this.DescriptionBlock;
						if (descriptionBlock2.Sb.Length > 0)
						{
							descriptionBlock2.AppendLine("", 0);
						}
						string txt2 = ZString.Format<string>("{0} Item Power Progression:", flag ? "Crafted" : "General");
						descriptionBlock2.AppendLine(txt2, 0);
						for (int i = 0; i < list.Count; i++)
						{
							string text4 = list[i].Description.Italicize();
							if (baseArchetype.ItemCategory == list[i])
							{
								text4 = text4.Underline().Bold();
							}
							if (list[i].ColorFlag.HasBitFlag(ItemCategory.ColorFlags.Description))
							{
								text4 = text4.Color(list[i].Color);
							}
							descriptionBlock2.Append(text4, 0);
							if (i < list.Count - 1)
							{
								descriptionBlock2.Append(txt, 0);
							}
						}
					}
				}
			}
			this.ToggleBlocks();
			this.SetSubHeaders();
			bool active = false;
			if (LocalPlayer.GameEntity.CollectionController.InteractiveStation)
			{
				ContainerType containerType = LocalPlayer.GameEntity.CollectionController.InteractiveStation.ContainerType;
				ItemArchetype itemArchetype;
				ulong value;
				if (containerType != ContainerType.MerchantOutgoing)
				{
					if (containerType == ContainerType.BlacksmithOutgoing)
					{
						if (instance != null && instance.IsItem && instance.ItemData.Durability != null)
						{
							this.m_currencyBlockLabel.SetText("Repair Cost:");
							uint repairCost = instance.GetRepairCost();
							this.m_currencyBlock.UpdateCoin((ulong)repairCost);
							active = true;
						}
					}
				}
				else if (instance != null && baseArchetype.TryGetAsType(out itemArchetype) && itemArchetype.TryGetSalePrice(instance, out value))
				{
					this.m_currencyBlockLabel.SetText("Sale Price:");
					this.m_currencyBlock.UpdateCoin(value);
					active = true;
				}
			}
			else if (GlobalSettings.Values.Player.BagBuybackEnabled && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag) && instance != null && instance.ContainerInstance != null && instance.ContainerInstance.ContainerType == ContainerType.Inventory)
			{
				ulong bagBuybackCost = GlobalSettings.Values.Player.GetBagBuybackCost(LocalPlayer.GameEntity, instance);
				this.m_currencyBlockLabel.SetText("Recovery Cost:");
				this.m_currencyBlock.UpdateCoin(bagBuybackCost);
				active = true;
			}
			this.m_currencyBlockParent.SetActive(active);
			if (this.m_nestedTooltip != null)
			{
				bool flag2 = false;
				IEquipable equipable;
				if (baseArchetype.TryGetAsType(out equipable) && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Inventory != null && equipable.MeetsRoleRequirements(LocalPlayer.GameEntity) && (instance == null || instance.ContainerInstance == null || instance.ContainerInstance.ContainerType != ContainerType.Equipment))
				{
					bool flag3 = equipable.Type.IsWeaponSlot();
					bool flag4 = !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
					foreach (EquipmentSlot equipmentSlot in equipable.Type.GetCachedCompatibleSlots())
					{
						ArchetypeInstance archetypeInstance;
						if ((!flag3 || (flag4 ? (equipmentSlot == EquipmentSlot.PrimaryWeapon_MainHand || equipmentSlot == EquipmentSlot.PrimaryWeapon_OffHand) : (equipmentSlot == EquipmentSlot.SecondaryWeapon_MainHand || equipmentSlot == EquipmentSlot.SecondaryWeapon_OffHand))) && LocalPlayer.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex((int)equipmentSlot, out archetypeInstance) && archetypeInstance.InstanceUI != null)
						{
							ITooltip instanceUI = archetypeInstance.InstanceUI;
							this.m_nestedTooltip.ActivateTooltip(instanceUI.GetTooltipParameter, true);
							this.UpdateNestedTooltipTransform();
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2 && this.m_nestedTooltip.Visible)
				{
					this.m_nestedTooltip.Hide(true);
				}
			}
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x00104AFC File Offset: 0x00102CFC
		private void SetEffectSyncData(ITooltipParameter param)
		{
			EffectSyncDataTooltipParameter effectSyncDataTooltipParameter = (EffectSyncDataTooltipParameter)param;
			bool combatEffect = effectSyncDataTooltipParameter.SyncData.CombatEffect != null;
			BaseArchetype archetype = effectSyncDataTooltipParameter.Archetype;
			if (!combatEffect || archetype == null)
			{
				return;
			}
			float timeRemaining = effectSyncDataTooltipParameter.SyncData.GetTimeRemaining();
			this.ResetSubHeaders();
			this.ResetBlocks();
			this.SetTitles();
			this.m_title.text = archetype.DisplayName;
			this.m_archetypeIcon.SetIcon(archetype, null);
			if (!string.IsNullOrEmpty(archetype.SubHeaderText))
			{
				this.m_leftSubHeader.AddLineToSubHeader(archetype.SubHeaderText);
			}
			string left = ZString.Format<string>("Applied by <i>{0}</i>", effectSyncDataTooltipParameter.SyncData.ApplicatorName);
			if (effectSyncDataTooltipParameter.SyncData.SourceIsPlayer)
			{
				this.AddLineToLeftSubHeader(ZString.Format<byte>("Level {0}", effectSyncDataTooltipParameter.SyncData.Level));
			}
			if (effectSyncDataTooltipParameter.SyncData.AlchemyPowerLevel != AlchemyPowerLevel.None)
			{
				this.AddLineToLeftSubHeader(ZString.Format<string, string>("<color={0}>{1}</color>", UIManager.EmberColor.ToHex(), effectSyncDataTooltipParameter.SyncData.AlchemyPowerLevel.GetAlchemyPowerLevelDescription()));
			}
			TooltipTextBlock dataBlock = this.DataBlock;
			AuraAbility auraAbility;
			string arg = archetype.TryGetAsType(out auraAbility) ? "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>" : timeRemaining.GetFormattedTime(true);
			dataBlock.AppendLine(left, ZString.Format<string>("<b>{0}</b> Remaining", arg));
			if (effectSyncDataTooltipParameter.SyncData.CombatEffect != null)
			{
				TargetingParams targetingParams = effectSyncDataTooltipParameter.SyncData.EffectSource.GetTargetingParams((float)Mathf.Abs((int)effectSyncDataTooltipParameter.SyncData.Level), AlchemyPowerLevel.None);
				CombatEffectExtensions.FillTooltipEffectsBlock(effectSyncDataTooltipParameter.SyncData.ArchetypeId, effectSyncDataTooltipParameter.SyncData.CombatEffect, targetingParams, effectSyncDataTooltipParameter.SyncData.ReagentItem, this, null, false, effectSyncDataTooltipParameter.SyncData.StackCount);
				ExpirationParams expiration = effectSyncDataTooltipParameter.SyncData.CombatEffect.Expiration;
				if (expiration != null && expiration.HasTriggerCount)
				{
					int triggerCount = expiration.TriggerCount;
					int arg2 = (effectSyncDataTooltipParameter.SyncData.TriggerCount != null) ? (triggerCount - (int)effectSyncDataTooltipParameter.SyncData.TriggerCount.Value) : triggerCount;
					dataBlock.AppendLine(string.Empty, ZString.Format<int, string>("{0}x {1} Remaining", arg2, effectSyncDataTooltipParameter.SyncData.CombatEffect.TriggerParams.GetTriggerDescription()));
				}
			}
			if (effectSyncDataTooltipParameter.SyncData.Diminished)
			{
				dataBlock.AppendLine("DIMINISHED".Color(UIManager.RequirementsNotMetColor), 0);
			}
			this.SetDurability(null);
			this.m_currencyBlock.gameObject.transform.parent.gameObject.SetActive(false);
			this.ToggleBlocks();
			this.SetSubHeaders();
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x00104D88 File Offset: 0x00102F88
		private void SetTitles()
		{
			this.DataBlock.Title = string.Empty;
			this.StatsBlock.Title = "Stats";
			this.MaterialBlock.Title = "Material";
			this.ReagentBlock.Title = "Reagent";
			this.CombatBlock.Title = "Combat";
			this.EffectsBlock.Title = "Effects";
			this.SubEffectsBlock.Title = "Triggered";
			this.RequirementsBlock.Title = "Requirements";
			this.DescriptionBlock.Title = string.Empty;
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x00104E28 File Offset: 0x00103028
		private void ResetBlocks()
		{
			for (int i = 0; i < this.m_blocks.Length; i++)
			{
				if (this.m_blocks[i])
				{
					this.m_blocks[i].ResetBlock();
				}
			}
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x00104E64 File Offset: 0x00103064
		protected void ToggleBlocks()
		{
			for (int i = 0; i < this.m_blocks.Length; i++)
			{
				if (this.m_blocks[i])
				{
					this.m_blocks[i].ToggleBlock();
				}
			}
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x000532A8 File Offset: 0x000514A8
		private void ResetSubHeaders()
		{
			ArchetypeTooltip.SubHeader leftSubHeader = this.m_leftSubHeader;
			if (leftSubHeader != null)
			{
				leftSubHeader.Reset();
			}
			ArchetypeTooltip.SubHeader rightSubHeader = this.m_rightSubHeader;
			if (rightSubHeader == null)
			{
				return;
			}
			rightSubHeader.Reset();
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000532CB File Offset: 0x000514CB
		private void SetSubHeaders()
		{
			ArchetypeTooltip.SubHeader leftSubHeader = this.m_leftSubHeader;
			if (leftSubHeader != null)
			{
				leftSubHeader.SetText();
			}
			ArchetypeTooltip.SubHeader rightSubHeader = this.m_rightSubHeader;
			if (rightSubHeader == null)
			{
				return;
			}
			rightSubHeader.SetText();
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x000532EE File Offset: 0x000514EE
		public override void DeactivateTooltip()
		{
			base.DeactivateTooltip();
			if (this.m_nestedTooltip != null && this.m_nestedTooltip.Visible)
			{
				this.m_nestedTooltip.Hide(true);
			}
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x00104EA0 File Offset: 0x001030A0
		private void UpdateNestedTooltipTransform()
		{
			if (this.m_nestedTooltip)
			{
				DynamicUIWindow.PivotCorner pivotCorner = this.m_pivotCorner;
				if (pivotCorner <= DynamicUIWindow.PivotCorner.UpperLeft)
				{
					this.m_nestedTooltip.RectTransform.SetAnchors(Vector2.one, Vector2.one);
					this.m_nestedTooltip.RectTransform.SetPivot(Vector2.up);
					this.m_nestedTooltip.RectTransform.anchoredPosition = new Vector2((float)this.m_nestedOffset, 0f);
					return;
				}
				if (pivotCorner - DynamicUIWindow.PivotCorner.LowerRight > 1)
				{
					return;
				}
				this.m_nestedTooltip.RectTransform.SetAnchors(Vector2.up, Vector2.up);
				this.m_nestedTooltip.RectTransform.SetPivot(Vector2.one);
				this.m_nestedTooltip.RectTransform.anchoredPosition = new Vector2((float)(-(float)this.m_nestedOffset), 0f);
			}
		}

		// Token: 0x04001F97 RID: 8087
		[SerializeField]
		protected TextMeshProUGUI m_title;

		// Token: 0x04001F98 RID: 8088
		[SerializeField]
		protected ArchetypeIconUI m_archetypeIcon;

		// Token: 0x04001F99 RID: 8089
		[SerializeField]
		private ArchetypeTooltip.SubHeader m_leftSubHeader;

		// Token: 0x04001F9A RID: 8090
		[SerializeField]
		private ArchetypeTooltip.SubHeader m_rightSubHeader;

		// Token: 0x04001F9B RID: 8091
		[SerializeField]
		private TooltipTextBlock[] m_blocks;

		// Token: 0x04001F9C RID: 8092
		[SerializeField]
		private DurabilityPanelUI m_durability;

		// Token: 0x04001F9D RID: 8093
		private const string kCurrencyGroup = "Currency";

		// Token: 0x04001F9E RID: 8094
		[SerializeField]
		private CurrencyDisplayPanelUI m_currencyBlock;

		// Token: 0x04001F9F RID: 8095
		[SerializeField]
		private TextMeshProUGUI m_currencyBlockLabel;

		// Token: 0x04001FA0 RID: 8096
		[SerializeField]
		private GameObject m_currencyBlockParent;

		// Token: 0x04001FA1 RID: 8097
		[SerializeField]
		private ArchetypeTooltip m_nestedTooltip;

		// Token: 0x04001FA2 RID: 8098
		[SerializeField]
		private int m_nestedOffset = 2;

		// Token: 0x04001FAD RID: 8109
		private static ArchetypeTooltipParameter? m_currentArchetypeParameter;

		// Token: 0x04001FB0 RID: 8112
		private DynamicUIWindow.PivotCorner m_pivotCorner;

		// Token: 0x04001FB1 RID: 8113
		public const string kSubEffectsBlockTitle = "Triggered";

		// Token: 0x02000378 RID: 888
		[Serializable]
		public class ArchetypeIconReference
		{
			// Token: 0x04001FB2 RID: 8114
			public Image Image;

			// Token: 0x04001FB3 RID: 8115
			public WindowComponentStylizer Stylizer;
		}

		// Token: 0x02000379 RID: 889
		[Serializable]
		private class SubHeader
		{
			// Token: 0x06001885 RID: 6277 RVA: 0x0005332C File Offset: 0x0005152C
			public void AddLineToSubHeader(string txt)
			{
				if (this.m_text && !string.IsNullOrEmpty(txt))
				{
					this.m_sb.AppendLine(txt);
				}
			}

			// Token: 0x06001886 RID: 6278 RVA: 0x00053350 File Offset: 0x00051550
			public void SetText()
			{
				this.m_text.ZStringSetText(this.m_sb.ToString());
			}

			// Token: 0x06001887 RID: 6279 RVA: 0x00053368 File Offset: 0x00051568
			public void Reset()
			{
				this.m_sb.Clear();
			}

			// Token: 0x04001FB4 RID: 8116
			private readonly StringBuilder m_sb = new StringBuilder();

			// Token: 0x04001FB5 RID: 8117
			[SerializeField]
			private TextMeshProUGUI m_text;
		}
	}
}
