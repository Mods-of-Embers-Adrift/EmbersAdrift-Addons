using System;
using System.Collections;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A83 RID: 2691
	[Serializable]
	public class ItemBundle
	{
		// Token: 0x170012FC RID: 4860
		// (get) Token: 0x0600536E RID: 21358 RVA: 0x00077AEB File Offset: 0x00075CEB
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

		// Token: 0x170012FD RID: 4861
		// (get) Token: 0x0600536F RID: 21359 RVA: 0x00077B0C File Offset: 0x00075D0C
		private bool m_showItemCount
		{
			get
			{
				return this.m_item != null && this.m_item.ArchetypeHasCount();
			}
		}

		// Token: 0x170012FE RID: 4862
		// (get) Token: 0x06005370 RID: 21360 RVA: 0x00077B29 File Offset: 0x00075D29
		private bool m_showItemCharges
		{
			get
			{
				return this.m_item != null && this.m_item.ArchetypeHasCharges();
			}
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x001D7F34 File Offset: 0x001D6134
		private string GetDescriptionLineAdditionalDetails()
		{
			if (this.m_showItemCount)
			{
				return " [CNT " + this.m_itemCount.ToString() + "]";
			}
			if (this.m_soulbound)
			{
				if (!this.m_showItemCharges)
				{
					return " [SOULBOUND]";
				}
				return " [SOULBOUND, CHR " + this.m_itemCharges.ToString() + "]";
			}
			else
			{
				if (this.m_showItemCharges)
				{
					return " [CHR " + this.m_itemCharges.ToString() + "]";
				}
				return string.Empty;
			}
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x00077B46 File Offset: 0x00075D46
		public string GetDescriptionLine()
		{
			if (!(this.m_item != null))
			{
				return string.Empty;
			}
			return this.m_item.DisplayName + this.GetDescriptionLineAdditionalDetails();
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x001D7FC0 File Offset: 0x001D61C0
		public ArchetypeInstance InitializeItem()
		{
			ArchetypeInstance archetypeInstance = this.m_item.CreateNewInstance();
			archetypeInstance.ItemData.ItemFlags = this.m_itemFlags;
			if (this.m_showItemCount)
			{
				archetypeInstance.ItemData.Count = new int?(this.m_itemCount);
			}
			else if (this.m_showItemCharges)
			{
				archetypeInstance.ItemData.Charges = new int?(this.m_itemCharges);
			}
			return archetypeInstance;
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x001D802C File Offset: 0x001D622C
		public void AddItemToPlayer(GameEntity entity, ItemAddContext context, MasteryArchetype associatedMastery = null)
		{
			ContainerInstance containerInstance;
			if (!GameManager.IsServer || this.m_item == null || entity == null || !entity.CollectionController.TryGetInstance(this.m_containerType, out containerInstance))
			{
				return;
			}
			ArchetypeInstance archetypeInstance = this.InitializeItem();
			if (associatedMastery != null)
			{
				archetypeInstance.ItemData.AssociatedMasteryId = new UniqueId?(associatedMastery.Id);
			}
			ArchetypeInstance archetypeInstance2;
			if (this.m_showItemCount && containerInstance.TryGetInstanceForArchetypeId(this.m_item.Id, out archetypeInstance2) && archetypeInstance2.CanMergeWith(archetypeInstance))
			{
				int num = (archetypeInstance.ItemData.Count != null) ? archetypeInstance.ItemData.Count.Value : 1;
				int num2 = (archetypeInstance2.ItemData.Count != null) ? archetypeInstance2.ItemData.Count.Value : 1;
				int value = num + num2;
				archetypeInstance2.ItemData.Count = new int?(value);
				ItemCountUpdatedTransaction transaction = new ItemCountUpdatedTransaction
				{
					Container = archetypeInstance2.ContainerInstance.Id,
					InstanceId = archetypeInstance2.InstanceId,
					NewCount = archetypeInstance2.ItemData.Count.Value
				};
				entity.NetworkEntity.PlayerRpcHandler.UpdateItemCount(transaction);
				StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
				return;
			}
			if (containerInstance.HasRoom())
			{
				if (this.m_soulbound)
				{
					archetypeInstance.MarkAsSoulbound(entity.CollectionController.Record);
				}
				archetypeInstance.Index = containerInstance.GetFirstAvailableIndex();
				containerInstance.Add(archetypeInstance, true);
				ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
				{
					Op = OpCodes.Ok,
					Instance = archetypeInstance,
					TargetContainer = containerInstance.Id,
					Context = context
				};
				entity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
				return;
			}
			StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x00077B72 File Offset: 0x00075D72
		private IEnumerable GetItems()
		{
			return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
		}

		// Token: 0x04004A74 RID: 19060
		[SerializeField]
		private ItemArchetype m_item;

		// Token: 0x04004A75 RID: 19061
		[SerializeField]
		private bool m_soulbound;

		// Token: 0x04004A76 RID: 19062
		[SerializeField]
		private ItemFlags m_itemFlags;

		// Token: 0x04004A77 RID: 19063
		[SerializeField]
		private ContainerType m_containerType = ContainerType.Inventory;

		// Token: 0x04004A78 RID: 19064
		[SerializeField]
		private int m_itemCount = 1;

		// Token: 0x04004A79 RID: 19065
		[SerializeField]
		private int m_itemCharges = 1;
	}
}
