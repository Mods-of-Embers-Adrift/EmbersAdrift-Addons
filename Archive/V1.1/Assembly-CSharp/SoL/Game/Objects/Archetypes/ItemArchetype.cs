using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Audio;
using SoL.Game.Crafting;
using SoL.Game.Player;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A82 RID: 2690
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Item")]
	public class ItemArchetype : BaseArchetype, IItem, IMerchantInventory
	{
		// Token: 0x170012DD RID: 4829
		// (get) Token: 0x06005321 RID: 21281 RVA: 0x000776FF File Offset: 0x000758FF
		private bool m_isRawComponent
		{
			get
			{
				return this is RawComponent;
			}
		}

		// Token: 0x170012DE RID: 4830
		// (get) Token: 0x06005322 RID: 21282 RVA: 0x0007770A File Offset: 0x0007590A
		public ComponentEffect[] ComponentEffects
		{
			get
			{
				return this.m_componentEffects;
			}
		}

		// Token: 0x170012DF RID: 4831
		// (get) Token: 0x06005323 RID: 21283 RVA: 0x00077712 File Offset: 0x00075912
		public bool HasComponentEffects
		{
			get
			{
				return this.m_componentEffects != null && this.m_componentEffects.Length != 0;
			}
		}

		// Token: 0x170012E0 RID: 4832
		// (get) Token: 0x06005324 RID: 21284 RVA: 0x00077728 File Offset: 0x00075928
		public bool HasNumericComponentEffects
		{
			get
			{
				return this.HasComponentEffects && this.HasNumericEffects();
			}
		}

		// Token: 0x170012E1 RID: 4833
		// (get) Token: 0x06005325 RID: 21285 RVA: 0x0007773A File Offset: 0x0007593A
		public ItemArchetype DeconstructItem
		{
			get
			{
				return this.m_deconstructItem;
			}
		}

		// Token: 0x170012E2 RID: 4834
		// (get) Token: 0x06005326 RID: 21286 RVA: 0x00077742 File Offset: 0x00075942
		public MaterialCategory MaterialCategory
		{
			get
			{
				return this.m_materialCategory;
			}
		}

		// Token: 0x170012E3 RID: 4835
		// (get) Token: 0x06005327 RID: 21287 RVA: 0x0007774A File Offset: 0x0007594A
		public bool RestrictDropCount
		{
			get
			{
				return this.m_restrictDropCount;
			}
		}

		// Token: 0x170012E4 RID: 4836
		// (get) Token: 0x06005328 RID: 21288 RVA: 0x00077752 File Offset: 0x00075952
		public int SlidingTimeFrame
		{
			get
			{
				return this.m_slidingTimeframe;
			}
		}

		// Token: 0x170012E5 RID: 4837
		// (get) Token: 0x06005329 RID: 21289 RVA: 0x0007775A File Offset: 0x0007595A
		public int MaxDropCount
		{
			get
			{
				return this.m_maxDropCount;
			}
		}

		// Token: 0x170012E6 RID: 4838
		// (get) Token: 0x0600532A RID: 21290 RVA: 0x00076A4A File Offset: 0x00074C4A
		private IEnumerable GetItemCategories
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ItemCategory>();
			}
		}

		// Token: 0x170012E7 RID: 4839
		// (get) Token: 0x0600532B RID: 21291 RVA: 0x00077762 File Offset: 0x00075962
		public int? MinimumMaterialLevel
		{
			get
			{
				if (!(this is RawComponent))
				{
					return this.m_minimumMaterialLevel;
				}
				return new int?(((RawComponent)this).MinimumRawMaterialLevel);
			}
		}

		// Token: 0x170012E8 RID: 4840
		// (get) Token: 0x0600532C RID: 21292 RVA: 0x00077783 File Offset: 0x00075983
		public int? MaximumMaterialLevel
		{
			get
			{
				if (!(this is RawComponent))
				{
					return this.m_maximumMaterialLevel;
				}
				return new int?(((RawComponent)this).MaximumRawMaterialLevel);
			}
		}

		// Token: 0x170012E9 RID: 4841
		// (get) Token: 0x0600532D RID: 21293 RVA: 0x000777A4 File Offset: 0x000759A4
		public override string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_customName))
				{
					return this.m_customName;
				}
				return base.DisplayName;
			}
		}

		// Token: 0x170012EA RID: 4842
		// (get) Token: 0x0600532E RID: 21294 RVA: 0x000777C0 File Offset: 0x000759C0
		public override string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_customDescription))
				{
					return this.m_customDescription;
				}
				return base.Description;
			}
		}

		// Token: 0x170012EB RID: 4843
		// (get) Token: 0x0600532F RID: 21295 RVA: 0x000777DC File Offset: 0x000759DC
		public override Sprite Icon
		{
			get
			{
				if (!(this.m_customIcon == null))
				{
					return this.m_customIcon;
				}
				return base.Icon;
			}
		}

		// Token: 0x170012EC RID: 4844
		// (get) Token: 0x06005330 RID: 21296 RVA: 0x000777F9 File Offset: 0x000759F9
		public override Color IconTint
		{
			get
			{
				if (this.m_customIconTint != null)
				{
					return this.m_customIconTint.Value;
				}
				return this.m_iconTint;
			}
		}

		// Token: 0x170012ED RID: 4845
		// (get) Token: 0x06005331 RID: 21297 RVA: 0x0007781A File Offset: 0x00075A1A
		public float Weight
		{
			get
			{
				return this.m_weight;
			}
		}

		// Token: 0x170012EE RID: 4846
		// (get) Token: 0x06005332 RID: 21298 RVA: 0x00077822 File Offset: 0x00075A22
		public override bool LootRoll
		{
			get
			{
				return this.m_lootRoll;
			}
		}

		// Token: 0x170012EF RID: 4847
		// (get) Token: 0x06005333 RID: 21299 RVA: 0x0007782A File Offset: 0x00075A2A
		public override bool HasDynamicValues
		{
			get
			{
				return this.HasComponentEffects;
			}
		}

		// Token: 0x170012F0 RID: 4848
		// (get) Token: 0x06005334 RID: 21300 RVA: 0x00077832 File Offset: 0x00075A32
		public bool HasNumericDynamicValues
		{
			get
			{
				return this.HasNumericComponentEffects;
			}
		}

		// Token: 0x170012F1 RID: 4849
		// (get) Token: 0x06005335 RID: 21301 RVA: 0x0007783A File Offset: 0x00075A3A
		public override ArchetypeCategory Category
		{
			get
			{
				if (!this.m_isLootable)
				{
					return ArchetypeCategory.None;
				}
				return ArchetypeCategory.Item;
			}
		}

		// Token: 0x170012F2 RID: 4850
		// (get) Token: 0x06005336 RID: 21302 RVA: 0x001D7090 File Offset: 0x001D5290
		public override ItemCategory ItemCategory
		{
			get
			{
				if (this.m_itemCategory)
				{
					return this.m_itemCategory;
				}
				if (GlobalSettings.Values && GlobalSettings.Values.UI != null)
				{
					return GlobalSettings.Values.UI.FallbackItemCategory;
				}
				return base.ItemCategory;
			}
		}

		// Token: 0x06005337 RID: 21303 RVA: 0x00077847 File Offset: 0x00075A47
		private bool ItemCategoryChangeFrameColor()
		{
			return this.ItemCategory && this.ItemCategory.ColorFlag.HasBitFlag(ItemCategory.ColorFlags.IconBorder);
		}

		// Token: 0x170012F3 RID: 4851
		// (get) Token: 0x06005338 RID: 21304 RVA: 0x00077869 File Offset: 0x00075A69
		public override bool ChangeFrameColor
		{
			get
			{
				return this.ItemCategoryChangeFrameColor() || base.ChangeFrameColor;
			}
		}

		// Token: 0x170012F4 RID: 4852
		// (get) Token: 0x06005339 RID: 21305 RVA: 0x0007787B File Offset: 0x00075A7B
		public override Color FrameColor
		{
			get
			{
				if (!this.ItemCategoryChangeFrameColor())
				{
					return base.FrameColor;
				}
				return this.ItemCategory.Color;
			}
		}

		// Token: 0x170012F5 RID: 4853
		// (get) Token: 0x0600533A RID: 21306 RVA: 0x00077897 File Offset: 0x00075A97
		public override AudioClipCollection DragDropAudio
		{
			get
			{
				return this.m_dragDropAudioOverride;
			}
		}

		// Token: 0x170012F6 RID: 4854
		// (get) Token: 0x0600533B RID: 21307 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showDragDropAudio
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool CanUse()
		{
			return false;
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnUse()
		{
		}

		// Token: 0x170012F7 RID: 4855
		// (get) Token: 0x0600533E RID: 21310 RVA: 0x0004BC2B File Offset: 0x00049E2B
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600533F RID: 21311 RVA: 0x0007789F File Offset: 0x00075A9F
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return (ulong)(this.m_baseWorth * 2U);
		}

		// Token: 0x06005340 RID: 21312 RVA: 0x001D70E0 File Offset: 0x001D52E0
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			uint num = this.m_eventCurrencyCost;
			uint num2;
			if (num > 0U && entity && entity.Subscriber && GlobalSettings.Values && GlobalSettings.Values.Subscribers != null && GlobalSettings.Values.Subscribers.TryGetSubscriberCost(num, out num2))
			{
				num = num2;
			}
			return (ulong)num;
		}

		// Token: 0x06005341 RID: 21313 RVA: 0x00056DED File Offset: 0x00054FED
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			errorMessage = "NONE";
			return true;
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x000778AA File Offset: 0x00075AAA
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			return entity.CollectionController.TryAddItemToPlayer(this, ItemAddContext.Merchant, out instance, (int)quantity, -1, itemFlags, markAsSoulbound);
		}

		// Token: 0x06005343 RID: 21315 RVA: 0x001D7138 File Offset: 0x001D5338
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.ItemData = new ItemInstanceData();
			instance.ItemData.ItemFlags = this.m_flagsToSet;
			if (this.ArchetypeHasCount())
			{
				instance.ItemData.Count = new int?(1);
				return;
			}
			if (this.ArchetypeHasCharges())
			{
				instance.ItemData.Charges = new int?(1);
			}
		}

		// Token: 0x06005344 RID: 21316 RVA: 0x001D719C File Offset: 0x001D539C
		public virtual bool TryGetSalePrice(ArchetypeInstance instance, out ulong value)
		{
			value = (ulong)this.m_baseWorth;
			if (instance == null || !instance.IsItem || !instance.ItemData.ItemFlags.CanBeSold() || instance.ItemData.AssociatedMasteryId != null)
			{
				return false;
			}
			if (instance.Archetype.ArchetypeHasCount() && instance.ItemData.Count != null)
			{
				int? count = instance.ItemData.Count;
				int num = 0;
				value = (ulong)((count.GetValueOrDefault() <= num & count != null) ? 0 : (instance.ItemData.Count.Value * (int)this.m_baseWorth));
			}
			return true;
		}

		// Token: 0x06005345 RID: 21317 RVA: 0x001D724C File Offset: 0x001D544C
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			bool flag = instance != null && instance.ItemData != null;
			if (flag && instance.ItemData.ItemFlags != ItemFlags.None)
			{
				tooltip.AddLineToRightSubHeader(instance.ItemData.ItemFlags.GetColoredItemFlags());
				Color color;
				if (!string.IsNullOrEmpty(instance.ItemData.PlayerName) && instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.Crafted) && ItemFlags.Crafted.TryGetItemFlagColor(out color))
				{
					tooltip.DataBlock.AppendLine("Crafted by".Color(color), "<b><i>" + instance.ItemData.PlayerName + "</b></i>");
				}
			}
			if (this.m_materialCategory != null)
			{
				tooltip.MaterialBlock.AppendLine("Category", this.m_materialCategory.DisplayName);
			}
			if (this.MinimumMaterialLevel != null && this.MaximumMaterialLevel != null)
			{
				string right = (this.MinimumMaterialLevel.Value == this.MaximumMaterialLevel.Value) ? ZString.Format<int>("<b>{0}</b>", this.MaximumMaterialLevel.Value) : ZString.Format<int, int>("<b>{0}-{1}</b>", this.MinimumMaterialLevel.Value, this.MaximumMaterialLevel.Value);
				tooltip.MaterialBlock.AppendLine("Level", right);
			}
			if (flag)
			{
				ItemComponentTree itemComponentTree = instance.ItemData.ItemComponentTree;
				if (itemComponentTree != null && itemComponentTree.QualityModifier != null)
				{
					tooltip.MaterialBlock.AppendLine("Quality", ZString.Format<int?>("<b>{0}%</b>", instance.ItemData.ItemComponentTree.QualityModifier));
				}
			}
			MasteryArchetype masteryArchetype;
			if (flag && instance.ItemData.AssociatedMasteryId != null && InternalGameDatabase.Archetypes.TryGetAsType<MasteryArchetype>(instance.ItemData.AssociatedMasteryId.Value, out masteryArchetype))
			{
				tooltip.DataBlock.AppendLine("<i>Associated Mastery</i>", "<i>" + masteryArchetype.DisplayName + "</i>");
			}
			if (flag)
			{
				if (instance.Archetype.ArchetypeHasCount() && instance.ItemData.Count != null)
				{
					tooltip.DataBlock.AppendLine("Count", ZString.Format<int>("<b>{0}</b>", instance.ItemData.Count.Value));
				}
				else if (instance.Archetype.ArchetypeHasCharges() && instance.ItemData.Charges != null)
				{
					tooltip.DataBlock.AppendLine("Charges", ZString.Format<int>("<b>{0}</b>", instance.ItemData.Charges.Value));
				}
			}
			this.AddArmorCostToTooltip(tooltip);
			this.AddWeaponDataToTooltip(tooltip, false);
			IArmorClass armorClass = this as IArmorClass;
			if (armorClass != null)
			{
				armorClass.AppendArmorClassToTooltipBlock(tooltip.DataBlock, instance);
			}
			IDurability durability = this as IDurability;
			if (durability != null)
			{
				durability.AppendDurabilityToTooltipBlock(tooltip.DataBlock, instance);
			}
			AugmentItem augmentItem;
			if (flag && instance.ItemData.Augment != null && instance.ItemData.Augment.UtilityItem != null && instance.ItemData.Augment.UtilityItem.Archetype != null && instance.ItemData.Augment.UtilityItem.Archetype.TryGetAsType(out augmentItem))
			{
				augmentItem.FillTooltipBlocksForAugment(tooltip, instance, entity);
			}
			else
			{
				AugmentItem augmentItem2 = this as AugmentItem;
				if (augmentItem2 != null)
				{
					augmentItem2.FillTooltipBlocksForAugment(tooltip, instance, entity);
				}
			}
			this.AddFlankingBonusToTooltip(tooltip);
		}

		// Token: 0x06005346 RID: 21318 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AddLevelRequirements(ArchetypeTooltip tooltip, GameEntity entity)
		{
		}

		// Token: 0x06005347 RID: 21319 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AddArmorCostToTooltip(ArchetypeTooltip tooltip)
		{
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AddFlankingBonusToTooltip(ArchetypeTooltip tooltip)
		{
		}

		// Token: 0x170012F8 RID: 4856
		// (get) Token: 0x0600534A RID: 21322 RVA: 0x001D75D0 File Offset: 0x001D57D0
		public IEnumerable<ComponentEffectAssignerName> ComponentEffectAssignerNames
		{
			get
			{
				IEnumerable<ComponentEffectAssignerName> allAssignerNames = ComponentEffectAssigners.GetAllAssignerNames();
				List<ComponentEffectAssignerName> list = new List<ComponentEffectAssignerName>();
				foreach (ComponentEffectAssignerName componentEffectAssignerName in allAssignerNames)
				{
					if (this.IsAssignerHandled(componentEffectAssignerName))
					{
						list.Add(componentEffectAssignerName);
					}
				}
				return list;
			}
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x001D762C File Offset: 0x001D582C
		public bool CanBeMadeFrom(ItemArchetype item)
		{
			if (item == this)
			{
				return true;
			}
			List<Recipe> list = this.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list)
			{
				if (recipe.Components != null)
				{
					foreach (RecipeComponent recipeComponent in recipe.Components)
					{
						if (recipeComponent != null)
						{
							foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
							{
								if (componentMaterial != null && componentMaterial.Archetype.CanBeMadeFrom(item))
								{
									return true;
								}
							}
						}
					}
				}
			}
			StaticListPool<Recipe>.ReturnToPool(list);
			return false;
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x001D76F8 File Offset: 0x001D58F8
		public bool IsValidComponentParentage(UniqueId[] parentage, int index = 0)
		{
			if (parentage == null || parentage.Length <= index || parentage[index] != base.Id)
			{
				return false;
			}
			index++;
			if (index == parentage.Length)
			{
				return true;
			}
			List<Recipe> list = this.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list)
			{
				if (recipe.Components != null)
				{
					foreach (RecipeComponent recipeComponent in recipe.Components)
					{
						if (recipeComponent != null && recipeComponent.AcceptableMaterials != null)
						{
							foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
							{
								if (componentMaterial.Archetype.Id == parentage[index])
								{
									return componentMaterial.Archetype.IsValidComponentParentage(parentage, index);
								}
							}
						}
					}
				}
			}
			StaticListPool<Recipe>.ReturnToPool(list);
			return false;
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x001D7808 File Offset: 0x001D5A08
		public List<Recipe> FindRecipesThatNeedThisItem()
		{
			List<Recipe> list = new List<Recipe>();
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				Recipe recipe = baseArchetype as Recipe;
				if (recipe != null)
				{
					bool flag = false;
					if (recipe.Components != null)
					{
						foreach (RecipeComponent recipeComponent in recipe.Components)
						{
							if (recipeComponent != null)
							{
								foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
								{
									if (componentMaterial != null && componentMaterial.Archetype.Id == base.Id)
									{
										list.Add(recipe);
										flag = true;
										break;
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600534E RID: 21326 RVA: 0x001D78E8 File Offset: 0x001D5AE8
		public void GetAllItemsInTree(List<ItemArchetype> items)
		{
			if (items.Contains(this))
			{
				return;
			}
			List<Recipe> list = this.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list)
			{
				recipe.GetAllItemsInTree(items);
			}
			StaticListPool<Recipe>.ReturnToPool(list);
		}

		// Token: 0x0600534F RID: 21327 RVA: 0x001D794C File Offset: 0x001D5B4C
		public void GetAllItemsInTreeReverse(List<ItemArchetype> items)
		{
			items.Add(this);
			foreach (Recipe recipe in this.FindRecipesThatNeedThisItem())
			{
				List<ItemArchetype> allPossibleOutputItems = recipe.GetAllPossibleOutputItems();
				foreach (ItemArchetype itemArchetype in allPossibleOutputItems)
				{
					if (!(itemArchetype == this))
					{
						itemArchetype.GetAllItemsInTreeReverse(items);
					}
				}
				StaticListPool<ItemArchetype>.ReturnToPool(allPossibleOutputItems);
			}
		}

		// Token: 0x170012F9 RID: 4857
		// (get) Token: 0x06005350 RID: 21328 RVA: 0x00076A82 File Offset: 0x00074C82
		private IEnumerable GetComponentEffectsProfile
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ComponentEffectsProfile>();
			}
		}

		// Token: 0x170012FA RID: 4858
		// (get) Token: 0x06005351 RID: 21329 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioCollections
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
			}
		}

		// Token: 0x170012FB RID: 4859
		// (get) Token: 0x06005352 RID: 21330 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x06005353 RID: 21331 RVA: 0x001D79F0 File Offset: 0x001D5BF0
		public List<Recipe> FindRecipesThatProduceThisItem()
		{
			List<Recipe> fromPool = StaticListPool<Recipe>.GetFromPool();
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				Recipe recipe = baseArchetype as Recipe;
				if (recipe != null)
				{
					List<ItemArchetype> allPossibleOutputItems = recipe.GetAllPossibleOutputItems();
					if (recipe != null && allPossibleOutputItems.Contains(this))
					{
						fromPool.Add(recipe);
					}
					StaticListPool<ItemArchetype>.ReturnToPool(allPossibleOutputItems);
				}
			}
			return fromPool;
		}

		// Token: 0x06005354 RID: 21332 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void PrepareDynamicArchetype()
		{
		}

		// Token: 0x06005355 RID: 21333 RVA: 0x000778C1 File Offset: 0x00075AC1
		public virtual bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.Weight <= 1;
		}

		// Token: 0x06005356 RID: 21334 RVA: 0x000778CC File Offset: 0x00075ACC
		public virtual bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.Weight)
			{
				this.m_weight = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_weight);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.BaseWorth)
			{
				return false;
			}
			this.m_baseWorth = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_baseWorth);
			return true;
		}

		// Token: 0x06005357 RID: 21335 RVA: 0x00077909 File Offset: 0x00075B09
		public bool WasMadeFromAll(ArchetypeInstance instance, ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && instance.ItemData.ItemComponentTree.ContainsAll(archetypes, componentFilters));
		}

		// Token: 0x06005358 RID: 21336 RVA: 0x00077938 File Offset: 0x00075B38
		public bool WasMadeFromAny(ArchetypeInstance instance, ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && instance.ItemData.ItemComponentTree.ContainsAny(archetypes, componentFilters));
		}

		// Token: 0x06005359 RID: 21337 RVA: 0x00077967 File Offset: 0x00075B67
		public bool WasMadeFromOnly(ArchetypeInstance instance, ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && instance.ItemData.ItemComponentTree.ContainsOnly(archetypes, componentFilters));
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x00077996 File Offset: 0x00075B96
		public bool ComponentWasMadeFromAll(ArchetypeInstance instance, UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && parentage != null && instance.ItemData.ItemComponentTree.ComponentContainsAll(parentage, archetypes));
		}

		// Token: 0x0600535B RID: 21339 RVA: 0x000779C8 File Offset: 0x00075BC8
		public bool ComponentWasMadeFromAny(ArchetypeInstance instance, UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && parentage != null && instance.ItemData.ItemComponentTree.ComponentContainsAny(parentage, archetypes));
		}

		// Token: 0x0600535C RID: 21340 RVA: 0x000779FA File Offset: 0x00075BFA
		public bool ComponentWasMadeFromOnly(ArchetypeInstance instance, UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			return archetypes == null || archetypes.Length == 0 || (instance != null && instance.ItemData.ItemComponentTree != null && parentage != null && instance.ItemData.ItemComponentTree.ComponentContainsOnly(parentage, archetypes));
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x001D7A70 File Offset: 0x001D5C70
		public bool IsAttributeInUtilizedComponents(ArchetypeInstance instance, ItemAttributes.Names attributeName, ComponentParentage[] componentFilters = null)
		{
			if (instance == null)
			{
				return false;
			}
			List<ItemArchetype> archetypeLeafList = instance.ItemData.ItemComponentTree.GetArchetypeLeafList<RawComponent>(componentFilters);
			if (archetypeLeafList == null)
			{
				return false;
			}
			bool flag = false;
			foreach (ItemArchetype itemArchetype in archetypeLeafList)
			{
				flag = (flag || ((RawComponent)itemArchetype).IsAttributeActive(attributeName));
			}
			StaticListPool<ItemArchetype>.ReturnToPool(archetypeLeafList);
			return flag;
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x001D7AF0 File Offset: 0x001D5CF0
		public float[] GetAttributeValuesFromUtilizedComponents(ArchetypeInstance instance, ItemAttributes.Names attributeName, ComponentParentage[] componentFilters = null)
		{
			if (instance == null)
			{
				return null;
			}
			ItemInstanceData itemData = instance.ItemData;
			List<ItemArchetype> list;
			if (itemData == null)
			{
				list = null;
			}
			else
			{
				ItemComponentTree itemComponentTree = itemData.ItemComponentTree;
				list = ((itemComponentTree != null) ? itemComponentTree.GetArchetypeLeafList<RawComponent>(componentFilters) : null);
			}
			List<ItemArchetype> list2 = list;
			if (list2 == null)
			{
				return null;
			}
			int num = 0;
			int num2 = 1000;
			foreach (ItemArchetype itemArchetype in list2)
			{
				RawComponent rawComponent = itemArchetype as RawComponent;
				if (rawComponent != null)
				{
					num2 = Math.Min(num2, rawComponent.GlobalAttributeClamp);
					if (rawComponent.IsAttributeActive(attributeName))
					{
						num++;
					}
				}
			}
			if (num == 0)
			{
				StaticListPool<ItemArchetype>.ReturnToPool(list2);
				return null;
			}
			float[] array = new float[num];
			int num3 = 0;
			foreach (ItemArchetype itemArchetype2 in list2)
			{
				RawComponent rawComponent2 = itemArchetype2 as RawComponent;
				if (rawComponent2 != null && rawComponent2.IsAttributeActive(attributeName))
				{
					int attribute = rawComponent2.GetAttribute(attributeName);
					if (attribute != 0)
					{
						array[num3++] = (float)Math.Min(attribute, num2);
					}
				}
			}
			StaticListPool<ItemArchetype>.ReturnToPool(list2);
			return array;
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x001D7C18 File Offset: 0x001D5E18
		public float[] GetQualifiedAttributeValuesFromUtilizedComponents(ArchetypeInstance instance, ItemAttributes.Names attributeName, ComponentParentage[] componentFilters = null)
		{
			if (instance == null)
			{
				return null;
			}
			ItemInstanceData itemData = instance.ItemData;
			List<RawComponentWithQuality> list;
			if (itemData == null)
			{
				list = null;
			}
			else
			{
				ItemComponentTree itemComponentTree = itemData.ItemComponentTree;
				list = ((itemComponentTree != null) ? itemComponentTree.GetRawMaterialsListWithQualities(componentFilters) : null);
			}
			List<RawComponentWithQuality> list2 = list;
			if (list2 == null)
			{
				return null;
			}
			int num = 0;
			foreach (RawComponentWithQuality rawComponentWithQuality in list2)
			{
				if (rawComponentWithQuality != null)
				{
					RawComponentWithQuality rawComponentWithQuality2 = rawComponentWithQuality;
					if (rawComponentWithQuality2.RawComponent.IsAttributeActive(attributeName))
					{
						num++;
					}
				}
			}
			if (num == 0)
			{
				StaticPoolableListPool<RawComponentWithQuality>.ReturnToPool(list2);
				return null;
			}
			float[] array = new float[num];
			int num2 = 0;
			foreach (RawComponentWithQuality rawComponentWithQuality3 in list2)
			{
				if (rawComponentWithQuality3 != null)
				{
					RawComponentWithQuality rawComponentWithQuality4 = rawComponentWithQuality3;
					if (rawComponentWithQuality4.RawComponent.IsAttributeActive(attributeName))
					{
						float num3 = (float)rawComponentWithQuality4.RawComponent.GetAttribute(attributeName) * ((float)rawComponentWithQuality4.Quality / 100f);
						if (num3 != 0f)
						{
							array[num2++] = num3;
						}
					}
				}
			}
			StaticPoolableListPool<RawComponentWithQuality>.ReturnToPool(list2);
			return array;
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x001D7D44 File Offset: 0x001D5F44
		public override BaseArchetype BuildDynamic(ArchetypeInstance instance)
		{
			if (instance.ArchetypeId != base.Id)
			{
				return null;
			}
			ItemArchetype itemArchetype = this;
			if ((instance.ItemData.ItemFlags & ItemFlags.Crafted) == ItemFlags.Crafted)
			{
				itemArchetype = UnityEngine.Object.Instantiate<ItemArchetype>(this);
				itemArchetype.DeriveMaterialLevel(instance);
				itemArchetype.ApplyComponentEffects(instance);
			}
			return itemArchetype;
		}

		// Token: 0x06005361 RID: 21345 RVA: 0x001D7D90 File Offset: 0x001D5F90
		public void ApplyInstanceComponentEffects(ArchetypeInstance instance)
		{
			if (this.m_componentEffects != null)
			{
				foreach (ComponentEffect componentEffect in this.m_componentEffects)
				{
					if (componentEffect.OperationType.AffectsInstanceOnly())
					{
						componentEffect.AffectItem(this, instance, null, null);
					}
				}
			}
		}

		// Token: 0x06005362 RID: 21346 RVA: 0x001D7DD8 File Offset: 0x001D5FD8
		private void ApplyComponentEffects(ArchetypeInstance instance)
		{
			if (this.m_componentEffects != null)
			{
				this.PrepareDynamicArchetype();
				ComponentEffect[] componentEffects = this.m_componentEffects;
				for (int i = 0; i < componentEffects.Length; i++)
				{
					componentEffects[i].AffectItem(this, instance, null, null);
				}
			}
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x001D7E14 File Offset: 0x001D6014
		private void DeriveMaterialLevel(ArchetypeInstance instance)
		{
			ItemInstanceData itemData = instance.ItemData;
			List<ItemArchetype> list;
			if (itemData == null)
			{
				list = null;
			}
			else
			{
				ItemComponentTree itemComponentTree = itemData.ItemComponentTree;
				list = ((itemComponentTree != null) ? itemComponentTree.GetArchetypeLeafList<ItemArchetype>(null) : null);
			}
			List<ItemArchetype> list2 = list;
			if (list2 != null)
			{
				foreach (ItemArchetype itemArchetype in list2)
				{
					this.m_minimumMaterialLevel = new int?(Math.Max(itemArchetype.MinimumMaterialLevel.GetValueOrDefault(), this.m_minimumMaterialLevel.GetValueOrDefault()));
					this.m_maximumMaterialLevel = new int?(Math.Max(itemArchetype.MaximumMaterialLevel.GetValueOrDefault(), this.m_maximumMaterialLevel.GetValueOrDefault()));
				}
				StaticListPool<ItemArchetype>.ReturnToPool(list2);
			}
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x001D7EDC File Offset: 0x001D60DC
		private bool HasNumericEffects()
		{
			bool flag = false;
			foreach (ComponentEffect componentEffect in this.m_componentEffects)
			{
				flag = (flag || (componentEffect.OperationType == OperationType.Numeric && componentEffect.Operations.Length != 0) || (componentEffect.OperationType == OperationType.Profile && componentEffect.ComponentEffectsProfile.HasNumericComponentEffects));
			}
			return flag;
		}

		// Token: 0x06005365 RID: 21349 RVA: 0x00077A2C File Offset: 0x00075C2C
		public void SetCustomName(string newName)
		{
			this.m_customName = newName;
		}

		// Token: 0x06005366 RID: 21350 RVA: 0x00077A35 File Offset: 0x00075C35
		public void SetCustomPrefix(string prefix)
		{
			this.m_customName = prefix + " " + (this.m_customName ?? base.DisplayName);
		}

		// Token: 0x06005367 RID: 21351 RVA: 0x00077A58 File Offset: 0x00075C58
		public void SetCustomSuffix(string suffix)
		{
			this.m_customName = (this.m_customName ?? base.DisplayName) + " " + suffix;
		}

		// Token: 0x06005368 RID: 21352 RVA: 0x00077A7B File Offset: 0x00075C7B
		public void SetCustomDescription(string newDescription)
		{
			this.m_customDescription = newDescription;
		}

		// Token: 0x06005369 RID: 21353 RVA: 0x00077A84 File Offset: 0x00075C84
		public void SetCustomIcon(Sprite icon)
		{
			this.m_customIcon = icon;
		}

		// Token: 0x0600536A RID: 21354 RVA: 0x00077A8D File Offset: 0x00075C8D
		public void SetCustomTint(Color tint)
		{
			this.m_customIconTint = new Color?(tint);
		}

		// Token: 0x0600536B RID: 21355 RVA: 0x00077A9B File Offset: 0x00075C9B
		public void SetItemCategory(ItemCategory itemCategory)
		{
			this.m_itemCategory = itemCategory;
		}

		// Token: 0x0600536C RID: 21356 RVA: 0x00077AA4 File Offset: 0x00075CA4
		public override GameObject GetInstanceUIPrefabReference()
		{
			return ClientGameManager.UIManager.ItemInstanceUIPrefab;
		}

		// Token: 0x04004A5E RID: 19038
		[SerializeField]
		private bool m_isLootable = true;

		// Token: 0x04004A5F RID: 19039
		[SerializeField]
		private bool m_lootRoll = true;

		// Token: 0x04004A60 RID: 19040
		[SerializeField]
		private float m_weight = 1f;

		// Token: 0x04004A61 RID: 19041
		[SerializeField]
		private uint m_baseWorth;

		// Token: 0x04004A62 RID: 19042
		[SerializeField]
		private uint m_eventCurrencyCost;

		// Token: 0x04004A63 RID: 19043
		[SerializeField]
		private AudioClipCollection m_dragDropAudioOverride;

		// Token: 0x04004A64 RID: 19044
		[SerializeField]
		private Color m_iconTint = Color.white;

		// Token: 0x04004A65 RID: 19045
		[SerializeField]
		private ComponentEffect[] m_componentEffects;

		// Token: 0x04004A66 RID: 19046
		[SerializeField]
		private ItemArchetype m_deconstructItem;

		// Token: 0x04004A67 RID: 19047
		[SerializeField]
		private MaterialCategory m_materialCategory;

		// Token: 0x04004A68 RID: 19048
		private const string kDropRestrictions = "Drop Restrictions";

		// Token: 0x04004A69 RID: 19049
		[SerializeField]
		private bool m_restrictDropCount;

		// Token: 0x04004A6A RID: 19050
		[SerializeField]
		private int m_slidingTimeframe = 10;

		// Token: 0x04004A6B RID: 19051
		[SerializeField]
		private int m_maxDropCount = 1;

		// Token: 0x04004A6C RID: 19052
		[SerializeField]
		private ItemFlags m_flagsToSet;

		// Token: 0x04004A6D RID: 19053
		[SerializeField]
		private ItemCategory m_itemCategory;

		// Token: 0x04004A6E RID: 19054
		private int? m_minimumMaterialLevel;

		// Token: 0x04004A6F RID: 19055
		private int? m_maximumMaterialLevel;

		// Token: 0x04004A70 RID: 19056
		private string m_customName;

		// Token: 0x04004A71 RID: 19057
		private string m_customDescription;

		// Token: 0x04004A72 RID: 19058
		private Sprite m_customIcon;

		// Token: 0x04004A73 RID: 19059
		private Color? m_customIconTint;
	}
}
