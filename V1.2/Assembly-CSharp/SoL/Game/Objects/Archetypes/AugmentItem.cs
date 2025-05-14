using System;
using Cysharp.Text;
using SoL.Game.Audio;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AAA RID: 2730
	public abstract class AugmentItem : StackableUtilityItem
	{
		// Token: 0x17001361 RID: 4961
		// (get) Token: 0x06005459 RID: 21593
		protected abstract bool RefreshStats { get; }

		// Token: 0x0600545A RID: 21594
		protected abstract string GetExpirationLabel();

		// Token: 0x0600545B RID: 21595
		protected abstract string GetExpirationSuffixLabel();

		// Token: 0x0600545C RID: 21596
		protected abstract bool IsValidAugment();

		// Token: 0x17001362 RID: 4962
		// (get) Token: 0x0600545D RID: 21597 RVA: 0x000786B0 File Offset: 0x000768B0
		protected override AudioClipCollection ClipCollection
		{
			get
			{
				return GlobalSettings.Values.Audio.RepairModeClipCollection;
			}
		}

		// Token: 0x17001363 RID: 4963
		// (get) Token: 0x0600545E RID: 21598 RVA: 0x000786C1 File Offset: 0x000768C1
		public int LevelReq
		{
			get
			{
				if (this.m_levelRequirement == null)
				{
					return 0;
				}
				return this.m_levelRequirement.Level;
			}
		}

		// Token: 0x17001364 RID: 4964
		// (get) Token: 0x0600545F RID: 21599 RVA: 0x000786D8 File Offset: 0x000768D8
		public int ExpirationAmount
		{
			get
			{
				return this.m_expirationAmount;
			}
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x000786E0 File Offset: 0x000768E0
		protected override bool IsValidItem(ArchetypeInstance targetInstance)
		{
			return targetInstance != null && targetInstance.Archetype != null && targetInstance.ItemData != null;
		}

		// Token: 0x06005461 RID: 21601 RVA: 0x0004479C File Offset: 0x0004299C
		public override CursorType GetCursorType()
		{
			return CursorType.MainCursor;
		}

		// Token: 0x06005462 RID: 21602 RVA: 0x001DA604 File Offset: 0x001D8804
		protected override void ClientRequestExecuteUtilityInternal(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance)
		{
			if (targetItemInstance.ItemData.HasAugment)
			{
				if (!(targetItemInstance.ItemData.Augment.ArchetypeId == base.Id))
				{
					DialogOptions opts = new DialogOptions
					{
						Title = "Replace Augment",
						Text = "An augment is already present on this item. Are you sure you want to replace it?",
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = delegate(bool answer, object result)
						{
							if (answer)
							{
								this.SendExecuteUtilityRequest(entity, sourceItemInstance, targetItemInstance);
							}
						},
						Instance = targetItemInstance
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
					return;
				}
				if (targetItemInstance.ItemData.Augment.StackCount >= 5)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Augments can only stack to " + 5.ToString() + "!");
					return;
				}
				base.SendExecuteUtilityRequest(entity, sourceItemInstance, targetItemInstance);
				return;
			}
			else
			{
				if (!targetItemInstance.ItemData.ItemFlags.HasBitFlag(ItemFlags.NoTrade))
				{
					DialogOptions opts2 = new DialogOptions
					{
						Title = "No Trade on Augment",
						Text = "Applying an augment will prevent this item from being traded in the future. Are you sure you want to do this?",
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = delegate(bool answer, object result)
						{
							if (answer)
							{
								this.SendExecuteUtilityRequest(entity, sourceItemInstance, targetItemInstance);
							}
						},
						Instance = targetItemInstance
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts2);
					return;
				}
				base.SendExecuteUtilityRequest(entity, sourceItemInstance, targetItemInstance);
				return;
			}
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x001DA7C8 File Offset: 0x001D89C8
		protected override bool ExecuteUtilityInternal(GameEntity sourceEntity, ArchetypeInstance targetItemInstance)
		{
			bool flag = false;
			if (this.IsValidAugment())
			{
				if (targetItemInstance.ItemData.HasAugment && targetItemInstance.ItemData.Augment.ArchetypeId == base.Id)
				{
					if (targetItemInstance.ItemData.Augment.StackCount >= 5)
					{
						return false;
					}
					ItemAugment augment = targetItemInstance.ItemData.Augment;
					augment.StackCount += 1;
				}
				else
				{
					ItemAugment fromPool = StaticPool<ItemAugment>.GetFromPool();
					fromPool.ArchetypeId = base.Id;
					fromPool.Count = 0;
					fromPool.StackCount = 1;
					targetItemInstance.ItemData.Augment = fromPool;
				}
				targetItemInstance.ItemData.TriggerAugmentChanged();
				flag = true;
			}
			if (flag)
			{
				targetItemInstance.ItemData.ItemFlags |= ItemFlags.NoTrade;
				if (this.RefreshStats)
				{
					sourceEntity.Vitals.RefreshAugmentStats(true);
				}
			}
			return flag;
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x001DA8A0 File Offset: 0x001D8AA0
		public override void AugmentUsed(GameEntity sourceEntity, ArchetypeInstance itemInstance, int amount)
		{
			base.AugmentUsed(sourceEntity, itemInstance, amount);
			if (!GameManager.IsServer || sourceEntity == null || itemInstance == null || itemInstance.ItemData == null || itemInstance.ItemData.Augment == null)
			{
				return;
			}
			ItemAugment augment = itemInstance.ItemData.Augment;
			augment.Count += amount;
			AugmentUpdateInfo update = default(AugmentUpdateInfo);
			if (augment.Count >= this.m_expirationAmount)
			{
				augment.Count = 0;
				ItemAugment itemAugment = augment;
				itemAugment.StackCount -= 1;
				if (augment.StackCount <= 0)
				{
					StaticPool<ItemAugment>.ReturnToPool(augment);
					itemInstance.ItemData.Augment = null;
					update.Expired = true;
				}
			}
			update.Count = augment.Count;
			update.StackCount = augment.StackCount;
			itemInstance.ItemData.TriggerAugmentChanged();
			sourceEntity.NetworkEntity.PlayerRpcHandler.Server_UpdateClientAugment(itemInstance.ContainerInstance.ContainerType, itemInstance.InstanceId, update);
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x000786FE File Offset: 0x000768FE
		public bool MeetsLevelRequirement(GameEntity entity)
		{
			return this.m_levelRequirement != null && this.m_levelRequirement.MeetsAllRequirements(entity);
		}

		// Token: 0x06005466 RID: 21606
		protected abstract void AddAugmentStatsToTooltip(ArchetypeTooltip tooltip);

		// Token: 0x06005467 RID: 21607
		protected abstract string GetRemainingText(ArchetypeInstance instance);

		// Token: 0x06005468 RID: 21608 RVA: 0x001DA990 File Offset: 0x001D8B90
		public void FillTooltipBlocksForAugment(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (instance != null && instance.Archetype != this && instance.ItemData != null && instance.ItemData.Augment != null && instance.ItemData.Augment.ArchetypeId == base.Id)
			{
				tooltip.DataBlock.AppendLine(ZString.Format<string, string, byte>("<color={0}>{1}</color> (x{2})", UIManager.AugmentColor.ToHex(), this.DisplayName, instance.ItemData.Augment.StackCount), this.GetRemainingText(instance));
			}
			this.AddAugmentStatsToTooltip(tooltip);
		}

		// Token: 0x04004B11 RID: 19217
		[SerializeField]
		protected int m_expirationAmount = 100;
	}
}
