using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008DF RID: 2271
	public class TeleportToGroupButton : MonoBehaviour
	{
		// Token: 0x06004266 RID: 16998 RVA: 0x0006CD16 File Offset: 0x0006AF16
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.OnTeleportToGroupClicked));
			MapUI.HighlightedDiscoveriesChanged += this.MapUIOnHighlightedDiscoveriesChanged;
		}

		// Token: 0x06004267 RID: 16999 RVA: 0x0006CD45 File Offset: 0x0006AF45
		private void Start()
		{
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.RefreshGroup += this.RefreshGroupEvent;
				ClientGameManager.GroupManager.GroupMemberStatusUpdate += this.RefreshGroupEvent;
			}
		}

		// Token: 0x06004268 RID: 17000 RVA: 0x00192834 File Offset: 0x00190A34
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.OnTeleportToGroupClicked));
			MapUI.HighlightedDiscoveriesChanged -= this.MapUIOnHighlightedDiscoveriesChanged;
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.RefreshGroup -= this.RefreshGroupEvent;
				ClientGameManager.GroupManager.GroupMemberStatusUpdate -= this.RefreshGroupEvent;
			}
		}

		// Token: 0x06004269 RID: 17001 RVA: 0x001928A8 File Offset: 0x00190AA8
		private void Refresh()
		{
			this.RefreshGroupMemberData();
			this.m_canTeleportToList.Clear();
			this.m_tooltipLines.Clear();
			bool flag = false;
			bool flag2 = false;
			foreach (KeyValuePair<int, TeleportToGroupButton.TeleportToGroupData> keyValuePair in this.m_teleportToGroupDataDict)
			{
				keyValuePair.Value.UpdateStatus();
				flag = (flag || keyValuePair.Value.CanTeleportToGroup);
				flag2 = (flag2 || keyValuePair.Value.ShowButton);
				if (keyValuePair.Value.ShowButton)
				{
					if (keyValuePair.Value.CanTeleportToGroup)
					{
						this.m_tooltipLines.Add(ZString.Format<string>("{0}: READY", keyValuePair.Value.ContextMenuTooltipDisplayName));
					}
					else
					{
						this.m_tooltipLines.Add(ZString.Format<string, string>("{0}: {1}", keyValuePair.Value.ContextMenuTooltipDisplayName, keyValuePair.Value.Reason));
					}
				}
				if (keyValuePair.Value.CanTeleportToGroup)
				{
					this.m_canTeleportToList.Add(keyValuePair.Value);
				}
			}
			this.m_button.interactable = flag;
			this.m_button.gameObject.SetActive(flag2);
			this.m_partyLabel.enabled = !flag2;
			if (this.m_tooltip)
			{
				this.m_tooltip.Text = ((this.m_tooltipLines.Count > 0) ? string.Join("\n\n", this.m_tooltipLines) : null);
			}
		}

		// Token: 0x0600426A RID: 17002 RVA: 0x00192A40 File Offset: 0x00190C40
		private void OnTeleportToGroupClicked()
		{
			if (!this.m_button.interactable)
			{
				return;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.InCombat)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
				return;
			}
			this.Refresh();
			if (this.m_canTeleportToList.Count <= 0)
			{
				return;
			}
			if (this.m_canTeleportToList.Count == 1)
			{
				TeleportConfirmationOptions teleportConfirmationOptions = this.GetTeleportConfirmationOptions(this.m_canTeleportToList[0]);
				ClientGameManager.UIManager.TeleportConfirmationDialog.Init(teleportConfirmationOptions);
				return;
			}
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.ContextMenu)
			{
				ContextMenuUI.ClearContextActions();
				using (List<TeleportToGroupButton.TeleportToGroupData>.Enumerator enumerator = this.m_canTeleportToList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TeleportToGroupButton.TeleportToGroupData teleportTo = enumerator.Current;
						ContextMenuUI.AddContextAction(teleportTo.ContextMenuTooltipDisplayName, teleportTo.CanTeleportToGroup, delegate()
						{
							TeleportConfirmationOptions teleportConfirmationOptions2 = this.GetTeleportConfirmationOptions(teleportTo);
							ClientGameManager.UIManager.TeleportConfirmationDialog.Init(teleportConfirmationOptions2);
						}, null, null);
					}
				}
				ClientGameManager.UIManager.ContextMenu.Init(ZString.Format<string>("{0} to", "Ley Link"));
			}
		}

		// Token: 0x0600426B RID: 17003 RVA: 0x00192B8C File Offset: 0x00190D8C
		private TeleportConfirmationOptions GetTeleportConfirmationOptions(TeleportToGroupButton.TeleportToGroupData teleportTo)
		{
			string contextMenuTooltipDisplayName = teleportTo.ContextMenuTooltipDisplayName;
			UniqueId sourceId = teleportTo.SourceMonolithProfile.Id;
			UniqueId targetId = teleportTo.TargetLeyLinkProfile.Id;
			ZoneId targetZoneId = teleportTo.TargetZoneId;
			return new TeleportConfirmationOptions
			{
				Title = "Teleport to Group",
				Text = MapDiscovery.GetTeleportConfirmationText(contextMenuTooltipDisplayName),
				CancelText = "Cancel",
				Callback = delegate(bool answer, object useTravelEssence)
				{
					if (answer)
					{
						this.ConfirmTeleportRequest(sourceId, targetId, targetZoneId, (bool)useTravelEssence);
					}
				},
				AutoCancel = new Func<bool>(this.AutoCancel),
				EssenceCost = teleportTo.EssenceCost
			};
		}

		// Token: 0x0600426C RID: 17004 RVA: 0x00192C3C File Offset: 0x00190E3C
		private void ConfirmTeleportRequest(UniqueId sourceMonolithId, UniqueId targetEmberRingId, ZoneId targetZoneId, bool useTravelEssence)
		{
			if (LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
			{
				this.Refresh();
				foreach (TeleportToGroupButton.TeleportToGroupData teleportToGroupData in this.m_canTeleportToList)
				{
					if (teleportToGroupData.CanTeleportToGroup && teleportToGroupData.TargetZoneId == targetZoneId && teleportToGroupData.SourceMonolithProfile.Id == sourceMonolithId && teleportToGroupData.TargetLeyLinkProfile.Id == targetEmberRingId)
					{
						LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZoneToGroup(teleportToGroupData.SourceMonolithProfile.Id, (int)teleportToGroupData.TargetZoneId, teleportToGroupData.Index, useTravelEssence);
						break;
					}
				}
			}
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x0006CD7F File Offset: 0x0006AF7F
		private bool AutoCancel()
		{
			return !LocalPlayer.GameEntity || LocalPlayer.GameEntity.InCombat || !this.m_button.interactable;
		}

		// Token: 0x0600426E RID: 17006 RVA: 0x00192D14 File Offset: 0x00190F14
		private void RefreshGroupMemberData()
		{
			foreach (KeyValuePair<int, TeleportToGroupButton.TeleportToGroupData> keyValuePair in this.m_teleportToGroupDataDict)
			{
				StaticPool<TeleportToGroupButton.TeleportToGroupData>.ReturnToPool(keyValuePair.Value);
			}
			this.m_teleportToGroupDataDict.Clear();
			if (!GlobalSettings.Values.Ashen.AllowGroupTeleportsFromMonolith || !ClientGameManager.GroupManager || !ClientGameManager.GroupManager.IsGrouped || ClientGameManager.GroupManager.GroupMemberCount < GlobalSettings.Values.Ashen.GroupTeleportMemberThreshold)
			{
				return;
			}
			if (!LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.CurrentEmberStone == null)
			{
				return;
			}
			int groupFromMonolithEssenceCost = GlobalSettings.Values.Ashen.GroupFromMonolithEssenceCost;
			bool hasEnoughEssence = LocalPlayer.GameEntity.CollectionController.GetAvailableEmberEssenceForTravel() >= groupFromMonolithEssenceCost;
			MonolithProfile monolithProfile;
			MonolithProfile sourceMonolithProfile = (ClientGameManager.UIManager && ClientGameManager.UIManager.MapUI && ClientGameManager.UIManager.MapUI.TryGetHighlightedMonolithDiscoveryProfile(out monolithProfile)) ? monolithProfile : null;
			foreach (GroupMember groupMember in ClientGameManager.GroupManager.GetAllGroupMembers())
			{
				if (groupMember != null && groupMember.EmberRingIndex > 0 && !groupMember.IsSelf && groupMember.InDifferentZone)
				{
					byte emberRingIndex = groupMember.EmberRingIndex;
					TeleportToGroupButton.TeleportToGroupData fromPool;
					if (this.m_teleportToGroupDataDict.TryGetValue((int)groupMember.EmberRingIndex, out fromPool))
					{
						fromPool.AddToMemberCount(groupMember);
					}
					else
					{
						fromPool = StaticPool<TeleportToGroupButton.TeleportToGroupData>.GetFromPool();
						fromPool.Init(groupMember, sourceMonolithProfile, groupFromMonolithEssenceCost, hasEnoughEssence);
						this.m_teleportToGroupDataDict.Add((int)emberRingIndex, fromPool);
					}
				}
			}
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x0006CDA9 File Offset: 0x0006AFA9
		private void MapUIOnHighlightedDiscoveriesChanged()
		{
			this.Refresh();
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x0006CDA9 File Offset: 0x0006AFA9
		private void RefreshGroupEvent()
		{
			this.Refresh();
		}

		// Token: 0x04003F72 RID: 16242
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003F73 RID: 16243
		[SerializeField]
		private TextMeshProUGUI m_partyLabel;

		// Token: 0x04003F74 RID: 16244
		[SerializeField]
		private TextTooltipTrigger m_tooltip;

		// Token: 0x04003F75 RID: 16245
		private readonly Dictionary<int, TeleportToGroupButton.TeleportToGroupData> m_teleportToGroupDataDict = new Dictionary<int, TeleportToGroupButton.TeleportToGroupData>();

		// Token: 0x04003F76 RID: 16246
		private readonly List<TeleportToGroupButton.TeleportToGroupData> m_canTeleportToList = new List<TeleportToGroupButton.TeleportToGroupData>(2);

		// Token: 0x04003F77 RID: 16247
		private readonly List<string> m_tooltipLines = new List<string>(32);

		// Token: 0x020008E0 RID: 2272
		private class TeleportToGroupData : IPoolable
		{
			// Token: 0x17000F16 RID: 3862
			// (get) Token: 0x06004272 RID: 17010 RVA: 0x0006CDDD File Offset: 0x0006AFDD
			// (set) Token: 0x06004273 RID: 17011 RVA: 0x0006CDE5 File Offset: 0x0006AFE5
			public int EssenceCost { get; private set; }

			// Token: 0x17000F17 RID: 3863
			// (get) Token: 0x06004274 RID: 17012 RVA: 0x0006CDEE File Offset: 0x0006AFEE
			// (set) Token: 0x06004275 RID: 17013 RVA: 0x0006CDF6 File Offset: 0x0006AFF6
			public byte Index { get; private set; }

			// Token: 0x17000F18 RID: 3864
			// (get) Token: 0x06004276 RID: 17014 RVA: 0x0006CDFF File Offset: 0x0006AFFF
			// (set) Token: 0x06004277 RID: 17015 RVA: 0x0006CE07 File Offset: 0x0006B007
			public ZoneId TargetZoneId { get; private set; }

			// Token: 0x17000F19 RID: 3865
			// (get) Token: 0x06004278 RID: 17016 RVA: 0x0006CE10 File Offset: 0x0006B010
			// (set) Token: 0x06004279 RID: 17017 RVA: 0x0006CE18 File Offset: 0x0006B018
			public SubZoneId TargetSubZoneId { get; private set; }

			// Token: 0x17000F1A RID: 3866
			// (get) Token: 0x0600427A RID: 17018 RVA: 0x0006CE21 File Offset: 0x0006B021
			// (set) Token: 0x0600427B RID: 17019 RVA: 0x0006CE29 File Offset: 0x0006B029
			public MonolithProfile SourceMonolithProfile { get; private set; }

			// Token: 0x17000F1B RID: 3867
			// (get) Token: 0x0600427C RID: 17020 RVA: 0x0006CE32 File Offset: 0x0006B032
			// (set) Token: 0x0600427D RID: 17021 RVA: 0x0006CE3A File Offset: 0x0006B03A
			public LeyLinkProfile TargetLeyLinkProfile { get; private set; }

			// Token: 0x17000F1C RID: 3868
			// (get) Token: 0x0600427E RID: 17022 RVA: 0x0006CE43 File Offset: 0x0006B043
			// (set) Token: 0x0600427F RID: 17023 RVA: 0x0006CE4B File Offset: 0x0006B04B
			public string ContextMenuTooltipDisplayName { get; private set; }

			// Token: 0x17000F1D RID: 3869
			// (get) Token: 0x06004280 RID: 17024 RVA: 0x0006CE54 File Offset: 0x0006B054
			// (set) Token: 0x06004281 RID: 17025 RVA: 0x0006CE5C File Offset: 0x0006B05C
			public bool CanTeleportToGroup { get; private set; }

			// Token: 0x17000F1E RID: 3870
			// (get) Token: 0x06004282 RID: 17026 RVA: 0x0006CE65 File Offset: 0x0006B065
			// (set) Token: 0x06004283 RID: 17027 RVA: 0x0006CE6D File Offset: 0x0006B06D
			public bool ShowButton { get; private set; }

			// Token: 0x17000F1F RID: 3871
			// (get) Token: 0x06004284 RID: 17028 RVA: 0x0006CE76 File Offset: 0x0006B076
			// (set) Token: 0x06004285 RID: 17029 RVA: 0x0006CE7E File Offset: 0x0006B07E
			public string Reason { get; private set; }

			// Token: 0x06004286 RID: 17030 RVA: 0x00192EF4 File Offset: 0x001910F4
			public void Init(GroupMember member, MonolithProfile sourceMonolithProfile, int essenceCost, bool hasEnoughEssence)
			{
				this.Reset();
				this.SourceMonolithProfile = sourceMonolithProfile;
				this.EssenceCost = essenceCost;
				this.m_hasEnoughEssence = hasEnoughEssence;
				if (member != null)
				{
					this.m_memberCount = 1;
					this.TargetZoneId = (ZoneId)member.ZoneId;
					this.TargetSubZoneId = (SubZoneId)member.SubZoneId;
					this.Index = member.EmberRingIndex;
					this.TargetLeyLinkProfile = GlobalSettings.Values.Ashen.GetLeyLinkProfileByIndex(this.Index);
					if (this.TargetLeyLinkProfile)
					{
						this.m_levelRequirement = this.TargetLeyLinkProfile.LevelRequirement;
						List<UniqueId> list;
						this.m_hasDiscoveredProfile = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Discoveries != null && LocalPlayer.GameEntity.CollectionController.Record.Discoveries.TryGetValue(this.TargetZoneId, out list) && list.Contains(this.TargetLeyLinkProfile.Id));
						string formattedZoneName = LocalZoneManager.GetFormattedZoneName(this.TargetZoneId, this.TargetSubZoneId);
						if (string.IsNullOrEmpty(this.TargetLeyLinkProfile.DisplayName))
						{
							this.ContextMenuTooltipDisplayName = ZString.Format<string, string>("{0} {1}", formattedZoneName, "Ley Link");
							return;
						}
						this.ContextMenuTooltipDisplayName = ZString.Format<string, string, string>("{0} {1} in {2}", this.TargetLeyLinkProfile.DisplayName, "Ley Link", formattedZoneName);
					}
				}
			}

			// Token: 0x06004287 RID: 17031 RVA: 0x00193064 File Offset: 0x00191264
			private void Reset()
			{
				this.EssenceCost = 0;
				this.Index = 0;
				this.TargetZoneId = ZoneId.None;
				this.TargetSubZoneId = SubZoneId.None;
				this.SourceMonolithProfile = null;
				this.TargetLeyLinkProfile = null;
				this.ContextMenuTooltipDisplayName = null;
				this.m_hasEnoughEssence = false;
				this.m_hasDiscoveredProfile = false;
				this.m_memberCount = 0;
				this.m_levelRequirement = 0;
				this.CanTeleportToGroup = false;
				this.ShowButton = false;
				this.Reason = null;
			}

			// Token: 0x06004288 RID: 17032 RVA: 0x0006CE87 File Offset: 0x0006B087
			public void AddToMemberCount(GroupMember member)
			{
				if (member != null)
				{
					this.m_memberCount++;
				}
			}

			// Token: 0x06004289 RID: 17033 RVA: 0x0006CE9A File Offset: 0x0006B09A
			private bool ShowButtonInternal()
			{
				return this.m_memberCount > 0;
			}

			// Token: 0x0600428A RID: 17034 RVA: 0x001930D4 File Offset: 0x001912D4
			private bool CanTeleportToGroupInternal()
			{
				return this.m_hasEnoughEssence && this.m_hasDiscoveredProfile && this.TargetLeyLinkProfile && this.SourceMonolithProfile && this.SourceMonolithProfile.IsAvailable() && this.m_memberCount >= GlobalSettings.Values.Ashen.GroupTeleportMemberThreshold && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.AdventuringLevel >= this.m_levelRequirement;
			}

			// Token: 0x0600428B RID: 17035 RVA: 0x00193168 File Offset: 0x00191368
			public void UpdateStatus()
			{
				this.CanTeleportToGroup = this.CanTeleportToGroupInternal();
				this.ShowButton = this.ShowButtonInternal();
				this.Reason = null;
				if (this.ShowButton && !this.CanTeleportToGroup)
				{
					if (!this.m_hasDiscoveredProfile)
					{
						this.Reason = "Undiscovered!";
						return;
					}
					if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.AdventuringLevel < this.m_levelRequirement)
					{
						this.Reason = ZString.Format<int>("Must be Adventuring level {0}!", this.m_levelRequirement);
						return;
					}
					if (this.m_memberCount < GlobalSettings.Values.Ashen.GroupTeleportMemberThreshold)
					{
						this.Reason = ZString.Format<int, int, string>("{0}/{1} group members required at the same {2}!", this.m_memberCount, GlobalSettings.Values.Ashen.GroupTeleportMemberThreshold, "Ley Link");
						return;
					}
					if (!this.SourceMonolithProfile)
					{
						this.Reason = "Not near a Monolith!";
						return;
					}
					if (!this.SourceMonolithProfile.IsAvailable())
					{
						this.Reason = "Monolith not available!";
						return;
					}
					if (!this.m_hasEnoughEssence)
					{
						this.Reason = ZString.Format<int>("Not enough Ember Essence! {0} required.", this.EssenceCost);
					}
				}
			}

			// Token: 0x17000F20 RID: 3872
			// (get) Token: 0x0600428C RID: 17036 RVA: 0x0006CEA5 File Offset: 0x0006B0A5
			// (set) Token: 0x0600428D RID: 17037 RVA: 0x0006CEAD File Offset: 0x0006B0AD
			bool IPoolable.InPool
			{
				get
				{
					return this.m_inPool;
				}
				set
				{
					this.m_inPool = value;
				}
			}

			// Token: 0x0600428E RID: 17038 RVA: 0x0006CEB6 File Offset: 0x0006B0B6
			void IPoolable.Reset()
			{
				this.Reset();
			}

			// Token: 0x04003F7F RID: 16255
			private bool m_hasEnoughEssence;

			// Token: 0x04003F80 RID: 16256
			private bool m_hasDiscoveredProfile;

			// Token: 0x04003F81 RID: 16257
			private int m_memberCount;

			// Token: 0x04003F82 RID: 16258
			private int m_levelRequirement;

			// Token: 0x04003F86 RID: 16262
			private bool m_inPool;
		}
	}
}
