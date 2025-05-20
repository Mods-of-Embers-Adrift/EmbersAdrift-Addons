using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BAD RID: 2989
	public class InteractiveZonePointTeleporter : MonoBehaviour, ITooltip, IInteractiveBase, ICursor, IContextMenu
	{
		// Token: 0x170015E9 RID: 5609
		// (get) Token: 0x06005CC4 RID: 23748 RVA: 0x0007E445 File Offset: 0x0007C645
		public bool IsValid
		{
			get
			{
				return this.m_zonePoint != null && this.m_discoveryTrigger != null && this.m_discoveryTrigger.Profile != null;
			}
		}

		// Token: 0x170015EA RID: 5610
		// (get) Token: 0x06005CC5 RID: 23749 RVA: 0x0007E476 File Offset: 0x0007C676
		public ZonePoint ZonePoint
		{
			get
			{
				return this.m_zonePoint;
			}
		}

		// Token: 0x170015EB RID: 5611
		// (get) Token: 0x06005CC6 RID: 23750 RVA: 0x0007E47E File Offset: 0x0007C67E
		public UniqueId DiscoveryId
		{
			get
			{
				if (!(this.m_discoveryTrigger != null))
				{
					return UniqueId.Empty;
				}
				return this.m_discoveryTrigger.DiscoveryId;
			}
		}

		// Token: 0x06005CC7 RID: 23751 RVA: 0x0007E49F File Offset: 0x0007C69F
		private void Awake()
		{
			if (this.IsValid)
			{
				LocalZoneManager.RegisterTeleporter(this);
			}
		}

		// Token: 0x06005CC8 RID: 23752 RVA: 0x0007E4AF File Offset: 0x0007C6AF
		private void OnDestroy()
		{
			if (this.IsValid)
			{
				LocalZoneManager.DeregisterTeleporter(this);
			}
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x0007E4BF File Offset: 0x0007C6BF
		private bool CanInteract(GameEntity entity)
		{
			return entity != null && this.m_interactionDistance.IsWithinRange(base.gameObject, entity);
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x001F1D78 File Offset: 0x001EFF78
		private bool AutoCancel()
		{
			return !LocalPlayer.GameEntity || LocalPlayer.GameEntity.InCombat || !this.m_interactionDistance.IsWithinRange(base.gameObject, LocalPlayer.GameEntity) || (this.m_sourceProfile && !this.m_sourceProfile.IsAvailable()) || (this.m_targetProfile && !this.m_targetProfile.IsAvailable());
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x001F1DF0 File Offset: 0x001EFFF0
		private void ZoneRequestWithConfirmation(ZoneId targetZoneId, MonolithProfile sourceProfile, MonolithProfile targetProfile, int cost, string zoneDisplayName)
		{
			this.m_sourceProfile = sourceProfile;
			this.m_targetProfile = targetProfile;
			if (!GameManager.IsServer && this.CanInteract(LocalPlayer.GameEntity))
			{
				if (LocalPlayer.GameEntity.InCombat)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
					return;
				}
				TeleportConfirmationOptions opts = new TeleportConfirmationOptions
				{
					Title = "Teleport",
					Text = MapDiscovery.GetTeleportConfirmationText(zoneDisplayName),
					CancelText = "Cancel",
					Callback = delegate(bool answer, object useTravelEssenceObj)
					{
						if (answer)
						{
							this.ZoneRequest(targetZoneId, sourceProfile, targetProfile, (bool)useTravelEssenceObj);
						}
					},
					AutoCancel = new Func<bool>(this.AutoCancel),
					EssenceCost = cost
				};
				ClientGameManager.UIManager.TeleportConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x06005CCC RID: 23756 RVA: 0x001F1EE0 File Offset: 0x001F00E0
		private void ZoneRequest(ZoneId targetZoneId, MonolithProfile sourceProfile, MonolithProfile targetProfile, bool useTravelEssence)
		{
			if (!GameManager.IsServer && this.CanInteract(LocalPlayer.GameEntity))
			{
				if (LocalPlayer.GameEntity.InCombat)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
					return;
				}
				if ((sourceProfile && !sourceProfile.IsAvailable()) || (targetProfile && !targetProfile.IsAvailable()))
				{
					return;
				}
				LoginApiManager.PerformZoneCheck(targetZoneId, delegate(bool response)
				{
					this.ZoneCheckResponse(response, targetZoneId, targetProfile, useTravelEssence);
				});
			}
		}

		// Token: 0x06005CCD RID: 23757 RVA: 0x001F1F84 File Offset: 0x001F0184
		private void ZoneCheckResponse(bool result, ZoneId targetZoneId, DiscoveryProfile targetDiscovery, bool useTravelEssence)
		{
			if (result)
			{
				if (LocalPlayer.GameEntity.InCombat)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
					return;
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZoneToDiscovery(this.DiscoveryId, (int)targetZoneId, targetDiscovery.Id, useTravelEssence);
			}
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x001F1FD0 File Offset: 0x001F01D0
		string IContextMenu.FillActionsGetTitle()
		{
			MonolithProfile sourceProfile;
			if (GameManager.IsServer || !this.CanInteract(LocalPlayer.GameEntity) || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null || LocalPlayer.GameEntity.CollectionController.Record.Discoveries == null || this.m_discoveryTrigger == null || this.m_discoveryTrigger.Profile == null || !this.m_discoveryTrigger.Profile.TryGetAsType(out sourceProfile))
			{
				return null;
			}
			ContextMenuUI.ClearContextActions();
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.MapUI)
			{
				ContextMenuUI.AddContextAction("Open Map", true, delegate()
				{
					if (!ClientGameManager.UIManager.MapUI.Visible)
					{
						ClientGameManager.UIManager.MapUI.ToggleWindow();
					}
				}, null, null);
			}
			ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)LocalZoneManager.ZoneRecord.ZoneId);
			using (Dictionary<ZoneId, List<UniqueId>>.Enumerator enumerator = LocalPlayer.GameEntity.CollectionController.Record.Discoveries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<ZoneId, List<UniqueId>> kvp = enumerator.Current;
					if (kvp.Key.IsAvailable())
					{
						zoneRecord = SessionData.GetZoneRecord(kvp.Key);
						string zoneDisplayName = zoneRecord.DisplayName;
						foreach (UniqueId uniqueId in kvp.Value)
						{
							MonolithProfile targetProfile;
							if (uniqueId != this.m_discoveryTrigger.Profile.Id && InternalGameDatabase.Archetypes.TryGetAsType<MonolithProfile>(uniqueId, out targetProfile))
							{
								MonolithFlags flags = sourceProfile.MonolithFlag | targetProfile.MonolithFlag;
								int cost = GlobalSettings.Values.Ashen.GetMonolithCost(flags);
								bool flag = LocalPlayer.GameEntity.CollectionController.GetAvailableEmberEssenceForTravel() >= cost;
								string text = ZString.Format<int, string>("[{0} EE] to {1}", cost, zoneDisplayName);
								bool enabled = flag && sourceProfile.IsAvailable() && targetProfile.IsAvailable();
								ContextMenuUI.AddContextAction(text, enabled, delegate()
								{
									this.ZoneRequestWithConfirmation(kvp.Key, sourceProfile, targetProfile, cost, zoneDisplayName);
								}, null, null);
							}
						}
					}
				}
			}
			return "Ember Monolith";
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x0007E4DE File Offset: 0x0007C6DE
		private string GetDisplayText(ZoneRecord zoneRecord, DiscoveryProfile discoveryProfile)
		{
			return "To " + zoneRecord.DisplayName;
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x0007E4F0 File Offset: 0x0007C6F0
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Teleport to another Ember Monolith", false);
		}

		// Token: 0x170015EC RID: 5612
		// (get) Token: 0x06005CD1 RID: 23761 RVA: 0x0007E503 File Offset: 0x0007C703
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015ED RID: 5613
		// (get) Token: 0x06005CD2 RID: 23762 RVA: 0x0007E511 File Offset: 0x0007C711
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015EE RID: 5614
		// (get) Token: 0x06005CD3 RID: 23763 RVA: 0x0007E519 File Offset: 0x0007C719
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x170015EF RID: 5615
		// (get) Token: 0x06005CD4 RID: 23764 RVA: 0x00070E66 File Offset: 0x0006F066
		private CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170015F0 RID: 5616
		// (get) Token: 0x06005CD5 RID: 23765 RVA: 0x0004479C File Offset: 0x0004299C
		private CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x170015F1 RID: 5617
		// (get) Token: 0x06005CD6 RID: 23766 RVA: 0x0007E521 File Offset: 0x0007C721
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity))
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x06005CD8 RID: 23768 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400503C RID: 20540
		[SerializeField]
		private ZonePoint m_zonePoint;

		// Token: 0x0400503D RID: 20541
		[SerializeField]
		private DiscoveryTrigger m_discoveryTrigger;

		// Token: 0x0400503E RID: 20542
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400503F RID: 20543
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04005040 RID: 20544
		private MonolithProfile m_sourceProfile;

		// Token: 0x04005041 RID: 20545
		private MonolithProfile m_targetProfile;
	}
}
