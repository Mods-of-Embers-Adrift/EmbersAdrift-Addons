using System;
using System.Collections.Generic;
using System.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000905 RID: 2309
	public class GuildUI : MonoBehaviour
	{
		// Token: 0x17000F53 RID: 3923
		// (get) Token: 0x060043CE RID: 17358 RVA: 0x0006DD09 File Offset: 0x0006BF09
		public GameObject TabContent
		{
			get
			{
				return this.m_tabContent;
			}
		}

		// Token: 0x17000F54 RID: 3924
		// (get) Token: 0x060043CF RID: 17359 RVA: 0x0006DD11 File Offset: 0x0006BF11
		public bool IsShown
		{
			get
			{
				return this.m_isShown;
			}
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x001978C0 File Offset: 0x00195AC0
		private void Start()
		{
			this.PopulateSortDropdown();
			this.PlayerPrefsKey = this.m_socialUI.PlayerPrefsKey + "_Guild";
			this.m_rosterSortDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_SortFactor", 0));
			this.m_sortDirection = (PlayerPrefs.GetInt(this.PlayerPrefsKey + "_SortDirection", 0) != 0);
			this.m_rosterSortDirectionButton.image.sprite = (this.m_sortDirection ? this.m_sortDescendingIcon : this.m_sortAscendingIcon);
			this.m_rosterShowOfflineToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_ShowOFfline", 1) != 0);
			this.m_newGuildButton.onClick.AddListener(new UnityAction(this.OnNewGuildClicked));
			this.m_rosterSortDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnSortFactorChanged));
			this.m_rosterSortDirectionButton.onClick.AddListener(new UnityAction(this.OnSortDirectionClicked));
			this.m_rosterShowOfflineToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowOfflineChanged));
			this.m_leaveGuildButton.onClick.AddListener(new UnityAction(this.OnLeaveGuildClicked));
			this.m_guildConfigButton.onClick.AddListener(new UnityAction(this.OnGuildConfigClicked));
			this.m_inviteButton.onClick.AddListener(new UnityAction(this.OnInviteClicked));
			this.m_descriptionButton.onClick.AddListener(new UnityAction(this.OnDescriptionEditClicked));
			this.m_motdButton.onClick.AddListener(new UnityAction(this.OnMotdEditClicked));
			this.m_addRankButton.onClick.AddListener(new UnityAction(this.OnAddRankClicked));
			this.m_disbandButton.onClick.AddListener(new UnityAction(this.OnDisbandClicked));
			this.m_rankNameButton.onClick.AddListener(new UnityAction(this.OnRankNameEditClicked));
			this.m_rankEditCloseButton.onClick.AddListener(new UnityAction(this.OnRankEditCloseClicked));
			GuildUI.PermissionToggle[] permissionsToggles = this.m_permissionsToggles;
			for (int i = 0; i < permissionsToggles.Length; i++)
			{
				permissionsToggles[i].Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnRankPermissionsChanged));
			}
			this.m_rankList.RankEditOpenRequested += this.OnRankEditOpenRequested;
			ClientGameManager.SocialManager.GuildInvitesUpdated += this.RefreshVisuals;
			ClientGameManager.SocialManager.GuildUpdated += this.RefreshVisuals;
			ClientGameManager.SocialManager.GuildRosterUpdated += this.RefreshVisuals;
			ClientGameManager.SocialManager.PlayerStatusesChanged += this.UpdateRosterWhenReady;
			this.RefreshVisuals();
		}

		// Token: 0x060043D1 RID: 17361 RVA: 0x00197B94 File Offset: 0x00195D94
		private void OnDestroy()
		{
			this.m_newGuildButton.onClick.RemoveListener(new UnityAction(this.OnNewGuildClicked));
			this.m_rosterSortDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnSortFactorChanged));
			this.m_rosterSortDirectionButton.onClick.RemoveListener(new UnityAction(this.OnSortDirectionClicked));
			this.m_rosterShowOfflineToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnShowOfflineChanged));
			this.m_leaveGuildButton.onClick.RemoveListener(new UnityAction(this.OnLeaveGuildClicked));
			this.m_guildConfigButton.onClick.RemoveListener(new UnityAction(this.OnGuildConfigClicked));
			this.m_inviteButton.onClick.RemoveListener(new UnityAction(this.OnInviteClicked));
			this.m_descriptionButton.onClick.RemoveListener(new UnityAction(this.OnDescriptionEditClicked));
			this.m_motdButton.onClick.RemoveListener(new UnityAction(this.OnMotdEditClicked));
			this.m_addRankButton.onClick.RemoveListener(new UnityAction(this.OnAddRankClicked));
			this.m_disbandButton.onClick.RemoveListener(new UnityAction(this.OnDisbandClicked));
			this.m_rankNameButton.onClick.RemoveListener(new UnityAction(this.OnRankNameEditClicked));
			this.m_rankEditCloseButton.onClick.RemoveListener(new UnityAction(this.OnRankEditCloseClicked));
			GuildUI.PermissionToggle[] permissionsToggles = this.m_permissionsToggles;
			for (int i = 0; i < permissionsToggles.Length; i++)
			{
				permissionsToggles[i].Toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnRankPermissionsChanged));
			}
			this.m_rankList.RankEditOpenRequested -= this.OnRankEditOpenRequested;
			ClientGameManager.SocialManager.GuildInvitesUpdated -= this.RefreshVisuals;
			ClientGameManager.SocialManager.GuildUpdated -= this.RefreshVisuals;
			ClientGameManager.SocialManager.GuildRosterUpdated -= this.RefreshVisuals;
			ClientGameManager.SocialManager.PlayerStatusesChanged -= this.UpdateRosterWhenReady;
			this.m_invitesList.Initialized -= this.UpdateInviteList;
			this.m_memberList.Initialized -= this.UpdateRoster;
			this.m_rankList.Initialized -= this.UpdateRankList;
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x0006DD19 File Offset: 0x0006BF19
		public void OnShow()
		{
			this.m_isShown = true;
			this.RefreshVisuals();
		}

		// Token: 0x060043D3 RID: 17363 RVA: 0x0006DD28 File Offset: 0x0006BF28
		public void OnHide()
		{
			this.m_isShown = false;
			this.m_newGuildNameField.Deactivate();
			this.m_inviteTextInput.Deactivate();
		}

		// Token: 0x060043D4 RID: 17364 RVA: 0x00197DF4 File Offset: 0x00195FF4
		private void PopulateSortDropdown()
		{
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			foreach (object obj in Enum.GetValues(typeof(GuildUI.RosterSortFactor)))
			{
				GuildUI.RosterSortFactor rosterSortFactor = (GuildUI.RosterSortFactor)obj;
				fromPool.Add(rosterSortFactor.ToStringWithSpaces());
			}
			this.m_rosterSortDropdown.AddOptions(fromPool);
			StaticListPool<string>.ReturnToPool(fromPool);
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x00197E78 File Offset: 0x00196078
		private void RefreshVisuals()
		{
			Guild guild = (ClientGameManager.SocialManager != null) ? ClientGameManager.SocialManager.Guild : null;
			GuildRank guildRank = (ClientGameManager.SocialManager != null) ? ClientGameManager.SocialManager.OwnGuildRank : null;
			this.m_newGuildPanel.SetActive(guild == null);
			this.m_currentGuildPanel.SetActive(guild != null);
			if (guild != null)
			{
				this.m_guildName.text = guild.Name;
				this.m_rankName.text = (((guildRank != null) ? guildRank.Name : null) ?? "Unknown");
				StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
				fromPool.AppendLine("Guild Ranks:");
				List<GuildRank> fromPool2 = StaticListPool<GuildRank>.GetFromPool();
				fromPool2.AddRange(guild.Ranks);
				fromPool2.Sort((GuildRank x, GuildRank y) => -x.Sort.CompareTo(y.Sort));
				foreach (GuildRank guildRank2 in fromPool2)
				{
					if (guildRank2._id == (((guildRank != null) ? guildRank._id : null) ?? string.Empty))
					{
						fromPool.AppendLine(guildRank2.Name + " <-- Your rank");
					}
					else
					{
						fromPool.AppendLine(guildRank2.Name);
					}
				}
				StaticListPool<GuildRank>.ReturnToPool(fromPool2);
				fromPool.AppendLine("\nYour Permissions:");
				if (guildRank == null || guildRank.Permissions == GuildPermissions.None)
				{
					fromPool.AppendLine("None");
				}
				else
				{
					foreach (object obj in Enum.GetValues(typeof(GuildPermissions)))
					{
						GuildPermissions guildPermissions = (GuildPermissions)obj;
						if (guildPermissions != GuildPermissions.None && guildRank.Permissions.HasBitFlag(guildPermissions))
						{
							fromPool.AppendLine(guildPermissions.ToStringWithSpaces());
						}
					}
				}
				this.m_rankPermissionsTooltip.Text = fromPool.ToString_ReturnToPool();
				this.m_guildConfigButton.gameObject.SetActive(guildRank != null && (guildRank.Permissions & (GuildPermissions.EditDescription | GuildPermissions.EditMotd | GuildPermissions.EditRanks)) > GuildPermissions.None);
				this.UpdateRosterWhenReady();
				if (!this.m_guildConfigButton.gameObject.activeInHierarchy)
				{
					this.m_guildAdminPanel.SetActive(false);
				}
				if (this.m_guildAdminPanel.activeInHierarchy)
				{
					this.m_descriptionPreview.text = guild.Description;
					this.m_motdPreview.text = guild.Motd;
					this.m_descriptionButton.interactable = (guildRank != null && guildRank.Permissions.HasBitFlag(GuildPermissions.EditDescription));
					this.m_motdButton.interactable = (guildRank != null && guildRank.Permissions.HasBitFlag(GuildPermissions.EditMotd));
					if (this.m_rankList.IsInitialized)
					{
						this.UpdateRankList();
					}
					else
					{
						this.m_rankList.Initialized += this.UpdateRankList;
					}
					this.m_rankEditPanel.SetActive(this.m_editingRankId != null);
					if (this.m_editingRankId != null)
					{
						GuildRank guildRank3 = null;
						foreach (GuildRank guildRank4 in ClientGameManager.SocialManager.Guild.Ranks)
						{
							if (guildRank4._id == this.m_editingRankId)
							{
								guildRank3 = guildRank4;
							}
						}
						if (guildRank3 != null)
						{
							this.m_rankNamePreview.text = guildRank3.Name;
							this.m_ignorePermissionsChanges = true;
							foreach (GuildUI.PermissionToggle permissionToggle in this.m_permissionsToggles)
							{
								permissionToggle.Toggle.isOn = guildRank3.Permissions.HasBitFlag(permissionToggle.Permission);
								permissionToggle.Toggle.interactable = !guildRank3.IsGuildMaster;
							}
							this.m_ignorePermissionsChanges = false;
						}
					}
				}
				this.m_leaveGuildButton.interactable = (guildRank == null || !guildRank.IsGuildMaster);
				this.m_leaveButtonTooltipRegion.gameObject.SetActive(guildRank != null && guildRank.IsGuildMaster);
				this.m_inviteButton.interactable = (guildRank != null && guildRank.Permissions.HasBitFlag(GuildPermissions.InviteMember));
				this.m_inviteButtonTooltipRegion.gameObject.SetActive(guildRank == null || !guildRank.Permissions.HasBitFlag(GuildPermissions.InviteMember));
				return;
			}
			if (this.m_invitesList.IsInitialized)
			{
				this.UpdateInviteList();
				return;
			}
			this.m_invitesList.Initialized += this.UpdateInviteList;
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x00198310 File Offset: 0x00196510
		private void UpdateInviteList()
		{
			List<Mail> fromPool = StaticListPool<Mail>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.PendingIncomingGuildInvites.Values);
			fromPool.Sort((Mail x, Mail y) => x.Created.CompareTo(y.Created));
			this.m_invitesList.UpdateItems(fromPool);
			this.m_invitesList.Reindex();
			StaticListPool<Mail>.ReturnToPool(fromPool);
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x0019837C File Offset: 0x0019657C
		private void UpdateRoster()
		{
			List<GuildMember> fromPool = StaticListPool<GuildMember>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.GuildRoster.Values);
			if (!this.m_rosterShowOfflineToggle.isOn)
			{
				fromPool.RemoveAll(delegate(GuildMember x)
				{
					PlayerStatus playerStatus;
					return ClientGameManager.SocialManager.TryGetLatestStatus(x.CharacterId, out playerStatus) && (playerStatus.ZoneId == -1 || playerStatus.PresenceFlags.HasBitFlag(PresenceFlags.Invisible));
				});
			}
			fromPool.Sort(delegate(GuildMember x, GuildMember y)
			{
				int num = x.CharacterId.CompareTo(y.CharacterId);
				switch (this.m_rosterSortDropdown.value)
				{
				case 1:
				{
					GuildRank rankById = ClientGameManager.SocialManager.Guild.GetRankById(x.RankId);
					GuildRank rankById2 = ClientGameManager.SocialManager.Guild.GetRankById(y.RankId);
					num = ((rankById != null) ? rankById.Sort : 0).CompareTo((rankById2 != null) ? rankById2.Sort : 0);
					break;
				}
				case 2:
				{
					BaseArchetype baseArchetype = null;
					BaseArchetype baseArchetype2 = null;
					PlayerStatus playerStatus;
					if (ClientGameManager.SocialManager.TryGetLatestStatus(x.CharacterId, out playerStatus))
					{
						baseArchetype = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus.RolePacked);
					}
					PlayerStatus playerStatus2;
					if (ClientGameManager.SocialManager.TryGetLatestStatus(y.CharacterId, out playerStatus2))
					{
						baseArchetype2 = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus2.RolePacked);
					}
					if (x.CharacterId == LocalPlayer.GameEntity.CharacterData.Name || y.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
					{
						BaseArchetype baseArchetype3 = null;
						if (LocalPlayer.GameEntity.CharacterData.SpecializedRoleId.IsEmpty)
						{
							InternalGameDatabase.Archetypes.TryGetAsType<BaseArchetype>(LocalPlayer.GameEntity.CharacterData.BaseRoleId, out baseArchetype3);
						}
						else
						{
							InternalGameDatabase.Archetypes.TryGetAsType<BaseArchetype>(LocalPlayer.GameEntity.CharacterData.SpecializedRoleId, out baseArchetype3);
						}
						if (x.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
						{
							baseArchetype = baseArchetype3;
						}
						if (y.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
						{
							baseArchetype2 = baseArchetype3;
						}
					}
					num = baseArchetype.DisplayName.CompareTo(baseArchetype2.DisplayName);
					break;
				}
				case 3:
				{
					PlayerStatus playerStatus3 = null;
					PlayerStatus playerStatus4 = null;
					ClientGameManager.SocialManager.TryGetLatestStatus(x.CharacterId, out playerStatus3);
					ClientGameManager.SocialManager.TryGetLatestStatus(y.CharacterId, out playerStatus4);
					int num2 = (int)((playerStatus3 != null) ? playerStatus3.Level : 50);
					int value = (int)((playerStatus4 != null) ? playerStatus4.Level : 50);
					if (x.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
					{
						num2 = LocalPlayer.GameEntity.CharacterData.AdventuringLevel;
					}
					if (y.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
					{
						value = LocalPlayer.GameEntity.CharacterData.AdventuringLevel;
					}
					num = num2.CompareTo(value);
					break;
				}
				case 4:
				{
					int num3 = 0;
					int num4 = 0;
					byte subZoneId = 0;
					byte subZoneId2 = 0;
					if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData != null)
					{
						int zoneId = LocalZoneManager.ZoneRecord.ZoneId;
						byte b = 0;
						SpawnVolumeOverride spawnVolumeOverride;
						if (LocalZoneManager.TryGetSpawnVolumeOverride(LocalPlayer.GameEntity.gameObject.transform.position, out spawnVolumeOverride))
						{
							b = (byte)spawnVolumeOverride.SubZoneId;
						}
						if (LocalPlayer.GameEntity.CharacterData.Name == x.CharacterId)
						{
							num3 = zoneId;
							subZoneId = b;
						}
						else
						{
							ClientGameManager.SocialManager.TryGetLatestZone(x.CharacterId, out num3, out subZoneId);
						}
						if (LocalPlayer.GameEntity.CharacterData.Name == y.CharacterId)
						{
							num4 = zoneId;
							subZoneId2 = b;
						}
						else
						{
							ClientGameManager.SocialManager.TryGetLatestZone(y.CharacterId, out num4, out subZoneId2);
						}
					}
					ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)num3);
					ZoneRecord zoneRecord2 = SessionData.GetZoneRecord((ZoneId)num4);
					string text = string.Empty;
					string text2 = string.Empty;
					if (zoneRecord != null)
					{
						text = LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, (SubZoneId)subZoneId);
					}
					else if (num3 == 0)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					else if (num3 == -1)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzy";
					}
					else if (num3 == -2)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzx";
					}
					else
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					if (zoneRecord2 != null)
					{
						text2 = LocalZoneManager.GetFormattedZoneName(zoneRecord2.DisplayName, (SubZoneId)subZoneId2);
					}
					else if (num4 == 0)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					else if (num4 == -1)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzy";
					}
					else if (num4 == -2)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzx";
					}
					else
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					num = text.ToLower().CompareTo(text2.ToLower());
					break;
				}
				case 5:
				{
					DateTime dateTime = x.Created;
					num = dateTime.CompareTo(y.Created);
					break;
				}
				case 6:
				{
					PlayerStatus playerStatus5 = null;
					PlayerStatus playerStatus6 = null;
					ClientGameManager.SocialManager.TryGetLatestStatus(x.CharacterId, out playerStatus5);
					ClientGameManager.SocialManager.TryGetLatestStatus(y.CharacterId, out playerStatus6);
					DateTime dateTime = ((playerStatus5 != null) ? playerStatus5.LastOnline : null) ?? DateTime.MinValue;
					num = dateTime.CompareTo(((playerStatus6 != null) ? playerStatus6.LastOnline : null) ?? DateTime.MinValue);
					break;
				}
				}
				if (this.m_sortDirection)
				{
					num *= -1;
				}
				return num;
			});
			this.m_memberList.UpdateItems(fromPool);
			this.m_memberList.Reindex();
			StaticListPool<GuildMember>.ReturnToPool(fromPool);
		}

		// Token: 0x060043D8 RID: 17368 RVA: 0x0006DD47 File Offset: 0x0006BF47
		private void UpdateRosterWhenReady()
		{
			if (this.m_memberList.IsInitialized)
			{
				this.UpdateRoster();
				return;
			}
			this.m_memberList.Initialized += this.UpdateRoster;
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x00198408 File Offset: 0x00196608
		private void UpdateRankList()
		{
			List<GuildRank> fromPool = StaticListPool<GuildRank>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.Guild.Ranks);
			fromPool.Sort((GuildRank x, GuildRank y) => -x.Sort.CompareTo(y.Sort));
			this.m_rankList.UpdateItems(fromPool);
			this.m_rankList.Reindex();
			StaticListPool<GuildRank>.ReturnToPool(fromPool);
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x0006DD74 File Offset: 0x0006BF74
		private void OnSortFactorChanged(int index)
		{
			this.UpdateRosterWhenReady();
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_SortFactor", index);
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x00198474 File Offset: 0x00196674
		private void OnSortDirectionClicked()
		{
			this.m_rosterSortDirectionButton.image.sprite = (this.m_sortDirection ? this.m_sortAscendingIcon : this.m_sortDescendingIcon);
			this.m_sortDirection = !this.m_sortDirection;
			this.UpdateRosterWhenReady();
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_SortDirection", this.m_sortDirection ? 1 : 0);
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x0006DD92 File Offset: 0x0006BF92
		private void OnShowOfflineChanged(bool value)
		{
			this.UpdateRosterWhenReady();
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_ShowOFfline", value ? 1 : 0);
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x001984E0 File Offset: 0x001966E0
		private void OnNewGuildClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Create Guild",
				Text = "Are you sure you want to create a guild called \"" + this.m_newGuildNameField.text + "\"? Guild names cannot be changed.",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnNewGuildConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x0006DDB6 File Offset: 0x0006BFB6
		private void OnNewGuildConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.CreateNewGuild(this.m_newGuildNameField.text);
			}
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x00198568 File Offset: 0x00196768
		private void OnLeaveGuildClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Leave Guild",
				Text = "Are you sure you want to leave the guild?",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnLeaveGuildConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060043E0 RID: 17376 RVA: 0x0006DDD0 File Offset: 0x0006BFD0
		private void OnLeaveGuildConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.LeaveGuild();
			}
		}

		// Token: 0x060043E1 RID: 17377 RVA: 0x0006DDDF File Offset: 0x0006BFDF
		private void OnInviteClicked()
		{
			ClientGameManager.SocialManager.InviteToGuild(this.m_inviteTextInput.text);
			this.m_inviteTextInput.text = string.Empty;
		}

		// Token: 0x060043E2 RID: 17378 RVA: 0x001985DC File Offset: 0x001967DC
		private void OnDescriptionEditClicked()
		{
			DialogOptions dialogOptions = default(DialogOptions);
			dialogOptions.Title = "Guild Description (visible on invites)";
			SocialManager socialManager = ClientGameManager.SocialManager;
			string text;
			if (socialManager == null)
			{
				text = null;
			}
			else
			{
				Guild guild = socialManager.Guild;
				text = ((guild != null) ? guild.Description : null);
			}
			dialogOptions.Text = text;
			dialogOptions.ConfirmationText = "Ok";
			dialogOptions.CancelText = "Cancel";
			dialogOptions.ShowCloseButton = false;
			dialogOptions.CharacterLimit = GlobalSettings.Values.Social.GuildDescriptionCharacterLimit;
			dialogOptions.Callback = new Action<bool, object>(this.OnDescriptionEditConfirmed);
			DialogOptions opts = dialogOptions;
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x0006DE06 File Offset: 0x0006C006
		private void OnDescriptionEditConfirmed(bool answer, object result)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.UpdateGuildDescription((string)result);
			}
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x0019867C File Offset: 0x0019687C
		private void OnMotdEditClicked()
		{
			DialogOptions dialogOptions = default(DialogOptions);
			dialogOptions.Title = "Guild Message of the Day";
			SocialManager socialManager = ClientGameManager.SocialManager;
			string text;
			if (socialManager == null)
			{
				text = null;
			}
			else
			{
				Guild guild = socialManager.Guild;
				text = ((guild != null) ? guild.Motd : null);
			}
			dialogOptions.Text = text;
			dialogOptions.ConfirmationText = "Ok";
			dialogOptions.CancelText = "Cancel";
			dialogOptions.ShowCloseButton = false;
			dialogOptions.CharacterLimit = GlobalSettings.Values.Social.GuildMotdCharacterLimit;
			dialogOptions.Callback = new Action<bool, object>(this.OnMotdEditConfirmed);
			DialogOptions opts = dialogOptions;
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x0006DE1B File Offset: 0x0006C01B
		private void OnMotdEditConfirmed(bool answer, object result)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.UpdateGuildMotd((string)result);
			}
		}

		// Token: 0x060043E6 RID: 17382 RVA: 0x0006DE30 File Offset: 0x0006C030
		private void OnGuildConfigClicked()
		{
			this.m_guildAdminPanel.SetActive(!this.m_guildAdminPanel.activeInHierarchy);
			this.RefreshVisuals();
		}

		// Token: 0x060043E7 RID: 17383 RVA: 0x0006DE51 File Offset: 0x0006C051
		private void OnAddRankClicked()
		{
			ClientGameManager.SocialManager.AddRank("New Rank", 0);
		}

		// Token: 0x060043E8 RID: 17384 RVA: 0x0019871C File Offset: 0x0019691C
		private void OnDisbandClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Disband Guild",
				Text = "Are you sure you want to disband the guild?",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnDisbandConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060043E9 RID: 17385 RVA: 0x0006DE63 File Offset: 0x0006C063
		private void OnDisbandConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.DisbandGuild();
			}
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x00198790 File Offset: 0x00196990
		private void OnRankNameEditClicked()
		{
			GuildRank guildRank = null;
			foreach (GuildRank guildRank2 in ClientGameManager.SocialManager.Guild.Ranks)
			{
				if (guildRank2._id == this.m_editingRankId)
				{
					guildRank = guildRank2;
				}
			}
			if (guildRank != null)
			{
				DialogOptions opts = new DialogOptions
				{
					Title = "New Rank Name",
					Text = guildRank.Name,
					ConfirmationText = "Ok",
					CancelText = "Cancel",
					ShowCloseButton = false,
					Callback = new Action<bool, object>(this.OnRankNameEditConfirmed)
				};
				ClientGameManager.UIManager.TextEntryDialog.Init(opts);
			}
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x00198868 File Offset: 0x00196A68
		private void OnRankNameEditConfirmed(bool answer, object result)
		{
			if (answer)
			{
				GuildRank guildRank = null;
				foreach (GuildRank guildRank2 in ClientGameManager.SocialManager.Guild.Ranks)
				{
					if (guildRank2._id == this.m_editingRankId)
					{
						guildRank = guildRank2;
					}
				}
				if (guildRank != null)
				{
					guildRank.Name = (string)result;
					ClientGameManager.SocialManager.UpdateGuildRank(this.m_editingRankId, guildRank);
				}
			}
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x001988F8 File Offset: 0x00196AF8
		private void OnRankPermissionsChanged(bool value)
		{
			if (!this.m_ignorePermissionsChanges && this.m_editingRankId != null)
			{
				GuildRank guildRank = null;
				foreach (GuildRank guildRank2 in ClientGameManager.SocialManager.Guild.Ranks)
				{
					if (guildRank2._id == this.m_editingRankId)
					{
						guildRank = guildRank2;
					}
				}
				if (guildRank != null)
				{
					guildRank.Permissions = GuildPermissions.None;
					foreach (GuildUI.PermissionToggle permissionToggle in this.m_permissionsToggles)
					{
						if (permissionToggle.Toggle.isOn)
						{
							guildRank.Permissions |= permissionToggle.Permission;
						}
					}
					ClientGameManager.SocialManager.UpdateGuildRank(this.m_editingRankId, guildRank);
				}
			}
		}

		// Token: 0x060043ED RID: 17389 RVA: 0x0006DE72 File Offset: 0x0006C072
		private void OnRankEditOpenRequested(string rankId)
		{
			this.m_editingRankId = rankId;
			this.RefreshVisuals();
		}

		// Token: 0x060043EE RID: 17390 RVA: 0x0006DE81 File Offset: 0x0006C081
		private void OnRankEditCloseClicked()
		{
			this.m_editingRankId = null;
			this.RefreshVisuals();
		}

		// Token: 0x04004042 RID: 16450
		[SerializeField]
		private SocialUI m_socialUI;

		// Token: 0x04004043 RID: 16451
		[SerializeField]
		private GameObject m_tabContent;

		// Token: 0x04004044 RID: 16452
		[SerializeField]
		private GameObject m_newGuildPanel;

		// Token: 0x04004045 RID: 16453
		[SerializeField]
		private SolTMP_InputField m_newGuildNameField;

		// Token: 0x04004046 RID: 16454
		[SerializeField]
		private SolButton m_newGuildButton;

		// Token: 0x04004047 RID: 16455
		[SerializeField]
		private GuildInvitesList m_invitesList;

		// Token: 0x04004048 RID: 16456
		[SerializeField]
		private GameObject m_currentGuildPanel;

		// Token: 0x04004049 RID: 16457
		[SerializeField]
		private TextMeshProUGUI m_guildName;

		// Token: 0x0400404A RID: 16458
		[SerializeField]
		private TextMeshProUGUI m_rankName;

		// Token: 0x0400404B RID: 16459
		[SerializeField]
		private TextTooltipTrigger m_rankPermissionsTooltip;

		// Token: 0x0400404C RID: 16460
		[SerializeField]
		private TMP_Dropdown m_rosterSortDropdown;

		// Token: 0x0400404D RID: 16461
		[SerializeField]
		private SolButton m_rosterSortDirectionButton;

		// Token: 0x0400404E RID: 16462
		[SerializeField]
		private Sprite m_sortAscendingIcon;

		// Token: 0x0400404F RID: 16463
		[SerializeField]
		private Sprite m_sortDescendingIcon;

		// Token: 0x04004050 RID: 16464
		[SerializeField]
		private SolToggle m_rosterShowOfflineToggle;

		// Token: 0x04004051 RID: 16465
		[SerializeField]
		private GuildMemberList m_memberList;

		// Token: 0x04004052 RID: 16466
		[SerializeField]
		private SolButton m_guildConfigButton;

		// Token: 0x04004053 RID: 16467
		[SerializeField]
		private SolButton m_leaveGuildButton;

		// Token: 0x04004054 RID: 16468
		[SerializeField]
		private SolButton m_inviteButton;

		// Token: 0x04004055 RID: 16469
		[SerializeField]
		private TextTooltipTrigger m_inviteButtonTooltipRegion;

		// Token: 0x04004056 RID: 16470
		[SerializeField]
		private SolTMP_InputField m_inviteTextInput;

		// Token: 0x04004057 RID: 16471
		[SerializeField]
		private TextTooltipTrigger m_leaveButtonTooltipRegion;

		// Token: 0x04004058 RID: 16472
		[SerializeField]
		private GameObject m_guildAdminPanel;

		// Token: 0x04004059 RID: 16473
		[SerializeField]
		private TextMeshProUGUI m_descriptionPreview;

		// Token: 0x0400405A RID: 16474
		[SerializeField]
		private SolButton m_descriptionButton;

		// Token: 0x0400405B RID: 16475
		[SerializeField]
		private TextMeshProUGUI m_motdPreview;

		// Token: 0x0400405C RID: 16476
		[SerializeField]
		private SolButton m_motdButton;

		// Token: 0x0400405D RID: 16477
		[SerializeField]
		private GuildRankList m_rankList;

		// Token: 0x0400405E RID: 16478
		[SerializeField]
		private SolButton m_addRankButton;

		// Token: 0x0400405F RID: 16479
		[SerializeField]
		private SolButton m_disbandButton;

		// Token: 0x04004060 RID: 16480
		[SerializeField]
		private GameObject m_rankEditPanel;

		// Token: 0x04004061 RID: 16481
		[SerializeField]
		private TextMeshProUGUI m_rankNamePreview;

		// Token: 0x04004062 RID: 16482
		[SerializeField]
		private SolButton m_rankNameButton;

		// Token: 0x04004063 RID: 16483
		[SerializeField]
		private GuildUI.PermissionToggle[] m_permissionsToggles;

		// Token: 0x04004064 RID: 16484
		[SerializeField]
		private SolButton m_rankEditCloseButton;

		// Token: 0x04004065 RID: 16485
		[NonSerialized]
		public string PlayerPrefsKey;

		// Token: 0x04004066 RID: 16486
		private bool m_sortDirection;

		// Token: 0x04004067 RID: 16487
		private string m_editingRankId;

		// Token: 0x04004068 RID: 16488
		private bool m_ignorePermissionsChanges;

		// Token: 0x04004069 RID: 16489
		private bool m_isShown;

		// Token: 0x02000906 RID: 2310
		private enum RosterSortFactor
		{
			// Token: 0x0400406B RID: 16491
			CharacterName,
			// Token: 0x0400406C RID: 16492
			Rank,
			// Token: 0x0400406D RID: 16493
			Role,
			// Token: 0x0400406E RID: 16494
			Level,
			// Token: 0x0400406F RID: 16495
			Zone,
			// Token: 0x04004070 RID: 16496
			JoinDate,
			// Token: 0x04004071 RID: 16497
			LastOnline
		}

		// Token: 0x02000907 RID: 2311
		[Serializable]
		private class PermissionToggle
		{
			// Token: 0x04004072 RID: 16498
			public SolToggle Toggle;

			// Token: 0x04004073 RID: 16499
			public GuildPermissions Permission;
		}
	}
}
