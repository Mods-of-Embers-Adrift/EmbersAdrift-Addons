using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A1E RID: 2590
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Mastery Ability Bundle")]
	public class MasteryAbilityBundle : BaseArchetype, IMerchantInventory
	{
		// Token: 0x1700117B RID: 4475
		// (get) Token: 0x06004F9C RID: 20380 RVA: 0x00075640 File Offset: 0x00073840
		public MasteryArchetype Mastery
		{
			get
			{
				return this.m_mastery;
			}
		}

		// Token: 0x1700117C RID: 4476
		// (get) Token: 0x06004F9D RID: 20381 RVA: 0x00075648 File Offset: 0x00073848
		public AbilityArchetype[] Abilities
		{
			get
			{
				return this.m_abilities;
			}
		}

		// Token: 0x1700117D RID: 4477
		// (get) Token: 0x06004F9E RID: 20382 RVA: 0x00075650 File Offset: 0x00073850
		public override Color IconTint
		{
			get
			{
				if (!this.m_mastery)
				{
					return base.IconTint;
				}
				return this.m_mastery.IconTint;
			}
		}

		// Token: 0x1700117E RID: 4478
		// (get) Token: 0x06004F9F RID: 20383 RVA: 0x00075671 File Offset: 0x00073871
		public override Sprite Icon
		{
			get
			{
				if (!this.m_mastery)
				{
					return null;
				}
				return this.m_mastery.Icon;
			}
		}

		// Token: 0x1700117F RID: 4479
		// (get) Token: 0x06004FA0 RID: 20384 RVA: 0x0007568D File Offset: 0x0007388D
		public override string DisplayName
		{
			get
			{
				if (!this.m_mastery)
				{
					return null;
				}
				return this.m_mastery.DisplayName;
			}
		}

		// Token: 0x17001180 RID: 4480
		// (get) Token: 0x06004FA1 RID: 20385 RVA: 0x000756A9 File Offset: 0x000738A9
		public override string Description
		{
			get
			{
				if (!this.m_mastery)
				{
					return null;
				}
				return this.m_mastery.Description;
			}
		}

		// Token: 0x17001181 RID: 4481
		// (get) Token: 0x06004FA2 RID: 20386 RVA: 0x0004479C File Offset: 0x0004299C
		public override ArchetypeIconType IconShape
		{
			get
			{
				return ArchetypeIconType.Circle;
			}
		}

		// Token: 0x17001182 RID: 4482
		// (get) Token: 0x06004FA3 RID: 20387 RVA: 0x000756C5 File Offset: 0x000738C5
		public override bool ChangeFrameColor
		{
			get
			{
				return this.m_mastery != null;
			}
		}

		// Token: 0x17001183 RID: 4483
		// (get) Token: 0x06004FA4 RID: 20388 RVA: 0x000756D3 File Offset: 0x000738D3
		public override Color FrameColor
		{
			get
			{
				if (!(this.m_mastery == null))
				{
					return this.m_mastery.Type.GetMasteryColor();
				}
				return base.FrameColor;
			}
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x000636F5 File Offset: 0x000618F5
		private IEnumerable GetMasteries()
		{
			return SolOdinUtilities.GetDropdownItems<MasteryArchetype>();
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x000756FA File Offset: 0x000738FA
		private bool CanLearn(GameEntity entity, out string errorMessage)
		{
			return this.m_mastery.CanBeLearnedBy(entity, out errorMessage);
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x001CAA24 File Offset: 0x001C8C24
		private bool AddToPlayer(GameEntity entity, ItemAddContext context, out ArchetypeInstance masteryInstance)
		{
			masteryInstance = null;
			string text;
			if (!GameManager.IsServer || !this.CanLearn(entity, out text))
			{
				return false;
			}
			if (MasteryAbilityBundle.m_instanceCache == null)
			{
				MasteryAbilityBundle.m_instanceCache = new List<ArchetypeInstance>();
			}
			ArchetypeInstance archetypeInstance = this.m_mastery.CreateNewInstance();
			archetypeInstance.MasteryData.BaseLevel = ((GameManager.IsServer && ServerGameManager.GameServerConfig != null) ? ((float)ServerGameManager.GameServerConfig.StartingLevel) : 1f);
			entity.CollectionController.Masteries.Add(archetypeInstance, true);
			MasteryAbilityBundle.m_instanceCache.Add(archetypeInstance);
			BaseRole baseRole;
			if (this.m_mastery.TryGetAsType(out baseRole) && baseRole.Type == MasteryType.Combat)
			{
				entity.CharacterData.BaseRoleId = baseRole.Id;
				entity.CharacterData.SpecializedRoleId = UniqueId.Empty;
			}
			masteryInstance = archetypeInstance;
			if (this.m_abilities != null)
			{
				for (int i = 0; i < this.m_abilities.Length; i++)
				{
					ArchetypeInstance archetypeInstance2;
					if (!(this.m_abilities[i] == null) && !(this.m_abilities[i].Mastery != this.m_mastery) && !entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_abilities[i].Id, out archetypeInstance2))
					{
						archetypeInstance = this.m_abilities[i].CreateNewInstance();
						entity.CollectionController.Abilities.Add(archetypeInstance, true);
						MasteryAbilityBundle.m_instanceCache.Add(archetypeInstance);
					}
				}
			}
			if (this.m_learnableBundles != null)
			{
				for (int j = 0; j < this.m_learnableBundles.Length; j++)
				{
					if (this.m_learnableBundles[j] != null)
					{
						this.m_learnableBundles[j].TeachToPlayer(entity);
					}
				}
			}
			if (this.m_itemBundles != null)
			{
				for (int k = 0; k < this.m_itemBundles.Length; k++)
				{
					this.m_itemBundles[k].AddItemToPlayer(entity, context, this.m_mastery);
				}
			}
			if (MasteryAbilityBundle.m_instanceCache.Count > 0)
			{
				ArchetypeAddRemoveTransaction archetypeAddRemoveTransaction = new ArchetypeAddRemoveTransaction
				{
					Op = OpCodes.Ok,
					AddedTransactions = new ArchetypeAddedTransaction[MasteryAbilityBundle.m_instanceCache.Count]
				};
				for (int l = 0; l < MasteryAbilityBundle.m_instanceCache.Count; l++)
				{
					archetypeAddRemoveTransaction.AddedTransactions[l] = new ArchetypeAddedTransaction
					{
						Op = OpCodes.Ok,
						Instance = MasteryAbilityBundle.m_instanceCache[l],
						TargetContainer = MasteryAbilityBundle.m_instanceCache[l].ContainerInstance.Id,
						Context = context
					};
				}
				entity.NetworkEntity.PlayerRpcHandler.AddRemoveItems(archetypeAddRemoveTransaction);
				entity.NetworkEntity.PlayerRpcHandler.SendChatNotification("You have learned " + this.m_mastery.DisplayName + "!");
				MasteryAbilityBundle.m_instanceCache.Clear();
			}
			MasteryArchetype.RefreshHighestLevelMastery(entity);
			entity.NetworkEntity.PlayerRpcHandler.RemoteRefreshHighestLevelMastery();
			return true;
		}

		// Token: 0x06004FA8 RID: 20392 RVA: 0x001CACF8 File Offset: 0x001C8EF8
		private ulong GetPriceForEntity(GameEntity entity)
		{
			ulong num = 0UL;
			if (entity != null && entity.CollectionController != null && entity.CollectionController.Masteries != null)
			{
				MasterySphere masterySphere = this.m_mastery.Type.GetMasterySphere();
				using (IEnumerator<ArchetypeInstance> enumerator = entity.CollectionController.Masteries.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MasteryArchetype masteryArchetype;
						if (enumerator.Current.Archetype.TryGetAsType(out masteryArchetype) && masteryArchetype.Type.GetMasterySphere() == masterySphere)
						{
							num += 1UL;
						}
					}
					goto IL_8D;
				}
				goto IL_85;
				IL_8D:
				if (num >= 2UL)
				{
					return (ulong)this.m_cost * (num - 1UL);
				}
				return 0UL;
			}
			IL_85:
			return (ulong)this.m_cost;
		}

		// Token: 0x17001184 RID: 4484
		// (get) Token: 0x06004FA9 RID: 20393 RVA: 0x0004BC2B File Offset: 0x00049E2B
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06004FAA RID: 20394 RVA: 0x00075709 File Offset: 0x00073909
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return this.GetPriceForEntity(entity);
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x00075712 File Offset: 0x00073912
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			return this.CanLearn(entity, out errorMessage);
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x0007571C File Offset: 0x0007391C
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			return this.AddToPlayer(entity, context, out instance);
		}

		// Token: 0x06004FAE RID: 20398 RVA: 0x001CADB8 File Offset: 0x001C8FB8
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (this.m_mastery == null)
			{
				return;
			}
			this.m_mastery.FillTooltipBlocks(tooltip, instance, entity);
			if (this.m_abilities != null && this.m_abilities.Length != 0)
			{
				TooltipTextBlock dataBlock = tooltip.DataBlock;
				dataBlock.AppendLine("", 0);
				dataBlock.AppendLine("Included Abilities:", 0);
				for (int i = 0; i < this.m_abilities.Length; i++)
				{
					dataBlock.AppendLine(string.Concat(new string[]
					{
						"<sprite=\"SolIcons\" name=\"Circle\" tint=1> ",
						this.m_abilities[i].DisplayName,
						" [LVL ",
						this.m_abilities[i].MinimumLevel.ToString(),
						"]"
					}), 0);
					dataBlock.AppendLine(this.m_abilities[i].Description.Italicize(), 0);
				}
			}
			if (this.m_itemBundles != null && this.m_itemBundles.Length != 0)
			{
				TooltipTextBlock dataBlock2 = tooltip.DataBlock;
				dataBlock2.AppendLine("", 0);
				dataBlock2.AppendLine("Included Items:", 0);
				for (int j = 0; j < this.m_itemBundles.Length; j++)
				{
					dataBlock2.AppendLine("<sprite=\"SolIcons\" name=\"Circle\" tint=1> " + this.m_itemBundles[j].GetDescriptionLine(), 0);
				}
			}
			string str;
			if (!this.m_mastery.CanBeLearnedBy(LocalPlayer.GameEntity, out str))
			{
				tooltip.RequirementsBlock.AppendLine("<color=\"red\">" + str + "</color>", 0);
			}
			if (ArchetypeTooltip.CurrentArchetypeParameter != null && ArchetypeTooltip.CurrentArchetypeParameter.Value.AtMerchant)
			{
				ulong priceForEntity = this.GetPriceForEntity(entity);
				if (LocalPlayer.GameEntity.CollectionController.Inventory.Currency < priceForEntity)
				{
					tooltip.RequirementsBlock.AppendLine("<color=\"red\">Not enough funds!</color>", 0);
				}
			}
		}

		// Token: 0x040047FA RID: 18426
		private static List<ArchetypeInstance> m_instanceCache;

		// Token: 0x040047FB RID: 18427
		[SerializeField]
		private MasteryArchetype m_mastery;

		// Token: 0x040047FC RID: 18428
		[SerializeField]
		private AbilityArchetype[] m_abilities;

		// Token: 0x040047FD RID: 18429
		[SerializeField]
		private LearnableBundle[] m_learnableBundles;

		// Token: 0x040047FE RID: 18430
		[SerializeField]
		private ItemBundle[] m_itemBundles;

		// Token: 0x040047FF RID: 18431
		[SerializeField]
		private uint m_cost = 1000U;
	}
}
