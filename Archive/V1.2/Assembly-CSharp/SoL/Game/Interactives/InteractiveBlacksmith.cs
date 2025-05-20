using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Networking;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B80 RID: 2944
	public class InteractiveBlacksmith : BaseNetworkInteractiveStation
	{
		// Token: 0x17001534 RID: 5428
		// (get) Token: 0x06005AB7 RID: 23223 RVA: 0x0007CEA4 File Offset: 0x0007B0A4
		public override string CurrencyRemovalMessage
		{
			get
			{
				return "for repairs.";
			}
		}

		// Token: 0x17001535 RID: 5429
		// (get) Token: 0x06005AB8 RID: 23224 RVA: 0x0007CEAB File Offset: 0x0007B0AB
		protected override string m_tooltipText
		{
			get
			{
				return "Blacksmith";
			}
		}

		// Token: 0x17001536 RID: 5430
		// (get) Token: 0x06005AB9 RID: 23225 RVA: 0x0007014A File Offset: 0x0006E34A
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.BlacksmithOutgoing;
			}
		}

		// Token: 0x17001537 RID: 5431
		// (get) Token: 0x06005ABA RID: 23226 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x001ED6EC File Offset: 0x001EB8EC
		public void ItemRepairRequest(GameEntity interactionSource, UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			if (interactionSource == null)
			{
				return;
			}
			OpCodes op = OpCodes.Error;
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (interactionSource.CollectionController != null && interactionSource.CollectionController.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance) && archetypeInstance.IsItem && archetypeInstance.ItemData.Durability != null)
			{
				uint repairCost = archetypeInstance.GetRepairCost();
				if (repairCost > 0U && base.TryRemoveCurrency(interactionSource, (ulong)repairCost))
				{
					archetypeInstance.ItemData.Durability.RepairItem();
					if (sourceContainerType == ContainerType.Equipment && interactionSource.Vitals != null)
					{
						interactionSource.Vitals.RecalculateTotalArmorClass();
					}
					op = OpCodes.Ok;
					InteractiveBlacksmith.LogRepair(interactionSource, (ulong)repairCost, "Item");
				}
			}
			interactionSource.NetworkEntity.PlayerRpcHandler.BlacksmithItemRepairResponse(op, itemInstanceId, sourceContainerType);
		}

		// Token: 0x06005ABC RID: 23228 RVA: 0x001ED7A8 File Offset: 0x001EB9A8
		public void ContainerRepairRequest(GameEntity interactionSource, ContainerType sourceContainerType)
		{
			if (interactionSource == null)
			{
				return;
			}
			OpCodes op = OpCodes.Error;
			ContainerInstance containerInstance;
			if (sourceContainerType.CanRepairFrom() && interactionSource.CollectionController != null && interactionSource.CollectionController.TryGetInstance(sourceContainerType, out containerInstance))
			{
				ulong num = 0UL;
				for (int i = 0; i < containerInstance.Count; i++)
				{
					ArchetypeInstance instanceForListIndex = containerInstance.GetInstanceForListIndex(i);
					if (instanceForListIndex != null)
					{
						num += (ulong)instanceForListIndex.GetRepairCost();
					}
				}
				if (num > 0UL && base.TryRemoveCurrency(interactionSource, num))
				{
					bool flag = false;
					for (int j = 0; j < containerInstance.Count; j++)
					{
						ArchetypeInstance instanceForListIndex2 = containerInstance.GetInstanceForListIndex(j);
						if (instanceForListIndex2 != null && instanceForListIndex2.IsItem && instanceForListIndex2.ItemData.Durability != null && instanceForListIndex2.ItemData.Durability.Absorbed > 0)
						{
							instanceForListIndex2.ItemData.Durability.RepairItem();
							flag = true;
						}
					}
					if (flag && sourceContainerType == ContainerType.Equipment && interactionSource.Vitals != null)
					{
						interactionSource.Vitals.RecalculateTotalArmorClass();
					}
					op = OpCodes.Ok;
					InteractiveBlacksmith.LogRepair(interactionSource, num, sourceContainerType.ToString());
				}
			}
			interactionSource.NetworkEntity.PlayerRpcHandler.BlacksmithContainerRepairResponse(op, sourceContainerType);
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x001ED8DC File Offset: 0x001EBADC
		private static void LogRepair(GameEntity entity, ulong repairCost, string context)
		{
			if (!entity)
			{
				return;
			}
			try
			{
				InteractiveBlacksmith.m_logArguments[0] = "BlacksmithRepair";
				InteractiveBlacksmith.m_logArguments[1] = entity.User.Id;
				InteractiveBlacksmith.m_logArguments[2] = entity.CollectionController.Record.Id;
				InteractiveBlacksmith.m_logArguments[3] = entity.CollectionController.Record.Name;
				InteractiveBlacksmith.m_logArguments[4] = repairCost;
				InteractiveBlacksmith.m_logArguments[5] = context;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName} has spent {@Currency} to repair {@Context}", InteractiveBlacksmith.m_logArguments);
			}
			catch
			{
			}
		}

		// Token: 0x04004F8F RID: 20367
		private static readonly object[] m_logArguments = new object[6];
	}
}
