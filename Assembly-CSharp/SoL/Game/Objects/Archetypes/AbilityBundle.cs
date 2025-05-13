using System;
using System.Collections;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A1B RID: 2587
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Ability Bundle")]
	public class AbilityBundle : BaseArchetype, IMerchantInventory
	{
		// Token: 0x17001176 RID: 4470
		// (get) Token: 0x06004F89 RID: 20361 RVA: 0x000755E0 File Offset: 0x000737E0
		public override Sprite Icon
		{
			get
			{
				return this.m_ability.Icon;
			}
		}

		// Token: 0x17001177 RID: 4471
		// (get) Token: 0x06004F8A RID: 20362 RVA: 0x000755ED File Offset: 0x000737ED
		public override string DisplayName
		{
			get
			{
				return this.m_ability.DisplayName;
			}
		}

		// Token: 0x17001178 RID: 4472
		// (get) Token: 0x06004F8B RID: 20363 RVA: 0x000755FA File Offset: 0x000737FA
		public override string Description
		{
			get
			{
				return this.m_ability.Description;
			}
		}

		// Token: 0x06004F8C RID: 20364 RVA: 0x00075607 File Offset: 0x00073807
		private IEnumerable GetAbilities()
		{
			return SolOdinUtilities.GetDropdownItems<AbilityArchetype>();
		}

		// Token: 0x06004F8D RID: 20365 RVA: 0x001CA730 File Offset: 0x001C8930
		private bool CanLearn(GameEntity entity, out string errorMessage)
		{
			errorMessage = null;
			if (entity == null)
			{
				return false;
			}
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_ability.Id, out archetypeInstance))
			{
				errorMessage = "You already know this ability!";
				return false;
			}
			ArchetypeInstance archetypeInstance2;
			if (!entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_ability.Mastery.Id, out archetypeInstance2))
			{
				errorMessage = "You do not know the required role!";
				return false;
			}
			if (this.m_ability.Specialization != null && (archetypeInstance2.MasteryData.Specialization == null || archetypeInstance2.MasteryData.Specialization.Value != this.m_ability.Specialization.Id))
			{
				errorMessage = "You are not specialized in the proper specialization!";
				return false;
			}
			if (archetypeInstance2.GetAssociatedLevel(entity) < (float)this.m_ability.LevelRange.Min)
			{
				errorMessage = this.m_ability.Mastery.DisplayName + " level not high enough!";
				return false;
			}
			return true;
		}

		// Token: 0x06004F8E RID: 20366 RVA: 0x001CA838 File Offset: 0x001C8A38
		private bool AddToPlayer(GameEntity entity, ItemAddContext context, out ArchetypeInstance instance)
		{
			instance = null;
			string text;
			if (!GameManager.IsServer || !this.CanLearn(entity, out text))
			{
				return false;
			}
			instance = this.m_ability.CreateNewInstance();
			entity.CollectionController.Abilities.Add(instance, true);
			ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
			{
				Op = OpCodes.Ok,
				Context = context,
				Instance = instance,
				TargetContainer = instance.ContainerInstance.Id
			};
			entity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
			return true;
		}

		// Token: 0x17001179 RID: 4473
		// (get) Token: 0x06004F8F RID: 20367 RVA: 0x0004BC2B File Offset: 0x00049E2B
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x0007560E File Offset: 0x0007380E
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return (ulong)this.m_cost;
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x00075617 File Offset: 0x00073817
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			return this.CanLearn(entity, out errorMessage);
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x00075621 File Offset: 0x00073821
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			return this.AddToPlayer(entity, context, out instance);
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x001CA8C8 File Offset: 0x001C8AC8
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (this.m_ability == null)
			{
				return;
			}
			this.m_ability.FillTooltipBlocks(tooltip, instance, entity);
			if (ArchetypeTooltip.CurrentArchetypeParameter != null && ArchetypeTooltip.CurrentArchetypeParameter.Value.AtMerchant && LocalPlayer.GameEntity.CollectionController.Inventory.Currency < (ulong)this.m_cost)
			{
				tooltip.RequirementsBlock.AppendLine("<color=\"red\">Not enough funds!</color>", 0);
			}
		}

		// Token: 0x040047F8 RID: 18424
		[SerializeField]
		private AbilityArchetype m_ability;

		// Token: 0x040047F9 RID: 18425
		[SerializeField]
		private uint m_cost = 1000U;
	}
}
