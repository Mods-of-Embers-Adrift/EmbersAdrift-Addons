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
	// Token: 0x02000A1F RID: 2591
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Mastery Specialization Bundle")]
	public class MasterySpecializationBundle : BaseArchetype, IMerchantInventory
	{
		// Token: 0x17001185 RID: 4485
		// (get) Token: 0x06004FB0 RID: 20400 RVA: 0x0007573B File Offset: 0x0007393B
		public SpecializedRole Specialization
		{
			get
			{
				return this.m_role;
			}
		}

		// Token: 0x17001186 RID: 4486
		// (get) Token: 0x06004FB1 RID: 20401 RVA: 0x00075743 File Offset: 0x00073943
		public override Sprite Icon
		{
			get
			{
				if (!(this.m_role == null))
				{
					return this.m_role.Icon;
				}
				return base.Icon;
			}
		}

		// Token: 0x17001187 RID: 4487
		// (get) Token: 0x06004FB2 RID: 20402 RVA: 0x00075765 File Offset: 0x00073965
		public override string DisplayName
		{
			get
			{
				if (!(this.m_role == null))
				{
					return this.m_role.DisplayName;
				}
				return base.DisplayName;
			}
		}

		// Token: 0x17001188 RID: 4488
		// (get) Token: 0x06004FB3 RID: 20403 RVA: 0x00075787 File Offset: 0x00073987
		public override string Description
		{
			get
			{
				if (!(this.m_role == null))
				{
					return this.m_role.Description;
				}
				return base.Description;
			}
		}

		// Token: 0x17001189 RID: 4489
		// (get) Token: 0x06004FB4 RID: 20404 RVA: 0x0004479C File Offset: 0x0004299C
		public override ArchetypeIconType IconShape
		{
			get
			{
				return ArchetypeIconType.Circle;
			}
		}

		// Token: 0x1700118A RID: 4490
		// (get) Token: 0x06004FB5 RID: 20405 RVA: 0x000757A9 File Offset: 0x000739A9
		public override bool ChangeFrameColor
		{
			get
			{
				return this.m_role != null;
			}
		}

		// Token: 0x1700118B RID: 4491
		// (get) Token: 0x06004FB6 RID: 20406 RVA: 0x000757B7 File Offset: 0x000739B7
		public override Color FrameColor
		{
			get
			{
				if (!(this.m_role == null))
				{
					return this.m_role.GeneralRole.Type.GetMasteryColor();
				}
				return base.FrameColor;
			}
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x00065EF0 File Offset: 0x000640F0
		private IEnumerable GetSpecializations()
		{
			return SolOdinUtilities.GetDropdownItems<SpecializedRole>();
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x001CAFE4 File Offset: 0x001C91E4
		private bool CanLearn(GameEntity entity, out string errorMessage)
		{
			errorMessage = null;
			ArchetypeInstance archetypeInstance;
			if (!entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_role.GeneralRole.Id, out archetypeInstance))
			{
				errorMessage = string.Concat(new string[]
				{
					"You must know ",
					this.m_role.GeneralRole.DisplayName,
					" to learn ",
					this.m_role.DisplayName,
					"!"
				});
				return false;
			}
			if (archetypeInstance.MasteryData.Specialization != null)
			{
				errorMessage = ((archetypeInstance.MasteryData.Specialization.Value == this.m_role.Id) ? ("You already know " + this.m_role.DisplayName + "!") : ("You have already specialized in " + this.m_role.GeneralRole.DisplayName + "!"));
				return false;
			}
			if (archetypeInstance.GetAssociatedLevel(entity) < 6f)
			{
				errorMessage = string.Concat(new string[]
				{
					"You must be at least level ",
					Mathf.FloorToInt(6f).ToString(),
					" in ",
					this.m_role.GeneralRole.DisplayName,
					" to learn this!"
				});
				return false;
			}
			return true;
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x001CB138 File Offset: 0x001C9338
		private bool AddToPlayer(GameEntity entity, ItemAddContext context)
		{
			string text;
			if (!GameManager.IsServer || !this.CanLearn(entity, out text))
			{
				return false;
			}
			if (MasterySpecializationBundle.m_instanceCache == null)
			{
				MasterySpecializationBundle.m_instanceCache = new List<ArchetypeInstance>();
			}
			ArchetypeInstance archetypeInstance;
			if (!entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_role.GeneralRole.Id, out archetypeInstance) || archetypeInstance.MasteryData == null || archetypeInstance.MasteryData.Specialization != null || archetypeInstance.GetAssociatedLevel(entity) < 6f)
			{
				return false;
			}
			archetypeInstance.MasteryData.Specialization = new UniqueId?(this.m_role.Id);
			entity.NetworkEntity.PlayerRpcHandler.TrainSpecializationResponse(OpCodes.Ok, archetypeInstance.InstanceId, this.m_role.Id, archetypeInstance.MasteryData.SpecializationLevel);
			if (this.m_abilities != null)
			{
				for (int i = 0; i < this.m_abilities.Length; i++)
				{
					ArchetypeInstance archetypeInstance2;
					if (!(this.m_abilities[i] == null) && !(this.m_abilities[i].Mastery != this.m_role.GeneralRole) && !(this.m_abilities[i].Specialization != this.m_role) && !entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_abilities[i].Id, out archetypeInstance2))
					{
						ArchetypeInstance archetypeInstance3 = this.m_abilities[i].CreateNewInstance();
						entity.CollectionController.Abilities.Add(archetypeInstance3, true);
						MasterySpecializationBundle.m_instanceCache.Add(archetypeInstance3);
					}
				}
			}
			if (MasterySpecializationBundle.m_instanceCache.Count > 0)
			{
				ArchetypeAddRemoveTransaction archetypeAddRemoveTransaction = new ArchetypeAddRemoveTransaction
				{
					Op = OpCodes.Ok,
					AddedTransactions = new ArchetypeAddedTransaction[MasterySpecializationBundle.m_instanceCache.Count]
				};
				for (int j = 0; j < MasterySpecializationBundle.m_instanceCache.Count; j++)
				{
					archetypeAddRemoveTransaction.AddedTransactions[j] = new ArchetypeAddedTransaction
					{
						Op = OpCodes.Ok,
						Instance = MasterySpecializationBundle.m_instanceCache[j],
						TargetContainer = MasterySpecializationBundle.m_instanceCache[j].ContainerInstance.Id,
						Context = context
					};
				}
				entity.NetworkEntity.PlayerRpcHandler.AddRemoveItems(archetypeAddRemoveTransaction);
				MasterySpecializationBundle.m_instanceCache.Clear();
			}
			MasteryArchetype.RefreshHighestLevelMastery(entity);
			entity.NetworkEntity.PlayerRpcHandler.RemoteRefreshHighestLevelMastery();
			entity.NetworkEntity.PlayerRpcHandler.SendChatNotification("You have learned " + this.m_role.DisplayName + "!");
			return true;
		}

		// Token: 0x1700118C RID: 4492
		// (get) Token: 0x06004FBA RID: 20410 RVA: 0x0004BC2B File Offset: 0x00049E2B
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x000757E3 File Offset: 0x000739E3
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return (ulong)this.m_cost;
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x06004FBD RID: 20413 RVA: 0x000757EC File Offset: 0x000739EC
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			return this.CanLearn(entity, out errorMessage);
		}

		// Token: 0x06004FBE RID: 20414 RVA: 0x000757F6 File Offset: 0x000739F6
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			instance = null;
			return this.AddToPlayer(entity, context);
		}

		// Token: 0x06004FBF RID: 20415 RVA: 0x001CB3CC File Offset: 0x001C95CC
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (this.m_role == null)
			{
				return;
			}
			this.m_role.FillTooltipBlocks(tooltip, instance, entity);
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
			string str;
			if (!this.CanLearn(LocalPlayer.GameEntity, out str))
			{
				tooltip.RequirementsBlock.AppendLine("<color=\"red\">" + str + "</color>", 0);
			}
			if (ArchetypeTooltip.CurrentArchetypeParameter != null && ArchetypeTooltip.CurrentArchetypeParameter.Value.AtMerchant && LocalPlayer.GameEntity.CollectionController.Inventory.Currency < (ulong)this.m_cost)
			{
				tooltip.RequirementsBlock.AppendLine("<color=\"red\">Not enough funds!</color>", 0);
			}
		}

		// Token: 0x04004800 RID: 18432
		private static List<ArchetypeInstance> m_instanceCache;

		// Token: 0x04004801 RID: 18433
		[SerializeField]
		private SpecializedRole m_role;

		// Token: 0x04004802 RID: 18434
		[SerializeField]
		private AbilityArchetype[] m_abilities;

		// Token: 0x04004803 RID: 18435
		[SerializeField]
		private uint m_cost = 1000U;
	}
}
