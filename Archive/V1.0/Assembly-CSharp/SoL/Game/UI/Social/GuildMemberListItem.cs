using System;
using System.Text;
using Cysharp.Text;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000901 RID: 2305
	public class GuildMemberListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06004398 RID: 17304 RVA: 0x0006DA43 File Offset: 0x0006BC43
		private void Start()
		{
			if (this.m_icon)
			{
				this.m_defaultRoleIconColor = this.m_icon.color;
			}
		}

		// Token: 0x06004399 RID: 17305 RVA: 0x0006DA63 File Offset: 0x0006BC63
		public void Init(GuildMemberList parent, int index, GuildMember member)
		{
			this.m_parent = parent;
			this.m_index = index;
			this.m_member = member;
			this.Refresh();
		}

		// Token: 0x0600439A RID: 17306 RVA: 0x0006DA80 File Offset: 0x0006BC80
		public void Refresh()
		{
			this.m_name.ZStringSetText(this.m_member.CharacterId);
			this.RefreshStatus();
		}

		// Token: 0x0600439B RID: 17307 RVA: 0x00196A40 File Offset: 0x00194C40
		private void RefreshStatus()
		{
			if (this.m_member != null)
			{
				SocialManager socialManager = ClientGameManager.SocialManager;
				if (((socialManager != null) ? socialManager.Guild : null) != null)
				{
					StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
					StringBuilder stringBuilder = fromPool;
					string characterId = this.m_member.CharacterId;
					string str = "\n";
					GuildRank rankById = ClientGameManager.SocialManager.Guild.GetRankById(this.m_member.RankId);
					stringBuilder.Append(characterId + str + (((rankById != null) ? rankById.Name : null) ?? "Unknown Rank"));
					int num = 0;
					SubZoneId subZoneId = SubZoneId.None;
					DateTime? dateTime = null;
					PlayerStatus playerStatus;
					if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData != null && LocalPlayer.GameEntity.CharacterData.Name == this.m_member.CharacterId)
					{
						ZoneRecord zoneRecord = LocalZoneManager.ZoneRecord;
						num = ((zoneRecord != null) ? zoneRecord.ZoneId : 0);
						SpawnVolumeOverride spawnVolumeOverride;
						if (LocalZoneManager.TryGetSpawnVolumeOverride(LocalPlayer.GameEntity.gameObject.transform.position, out spawnVolumeOverride))
						{
							subZoneId = spawnVolumeOverride.SubZoneId;
						}
						UniqueId id = UniqueId.Empty;
						if (!LocalPlayer.GameEntity.CharacterData.SpecializedRoleId.IsEmpty)
						{
							id = LocalPlayer.GameEntity.CharacterData.SpecializedRoleId;
						}
						else if (!LocalPlayer.GameEntity.CharacterData.BaseRoleId.IsEmpty)
						{
							id = LocalPlayer.GameEntity.CharacterData.BaseRoleId;
						}
						int adventuringLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel;
						this.m_level.SetTextFormat("Lvl {0}", adventuringLevel);
						BaseArchetype baseArchetype;
						if (!id.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<BaseArchetype>(id, out baseArchetype))
						{
							fromPool.AppendLine("\nLvl " + adventuringLevel.ToString() + " " + baseArchetype.DisplayName);
							this.m_icon.sprite = baseArchetype.Icon;
							this.m_icon.color = baseArchetype.IconTint;
							this.m_iconTooltip.Text = baseArchetype.DisplayName;
						}
						else
						{
							fromPool.AppendLine("\nLvl " + adventuringLevel.ToString() + " Unknown Role");
							this.m_icon.sprite = this.m_unknownRoleIcon;
							this.m_icon.color = this.m_defaultRoleIconColor;
							this.m_iconTooltip.Text = "Unknown Role";
						}
					}
					else if (ClientGameManager.SocialManager != null && ClientGameManager.SocialManager.TryGetLatestStatus(this.m_member.CharacterId, out playerStatus) && playerStatus != null)
					{
						num = playerStatus.ZoneId;
						subZoneId = playerStatus.SubZoneIdEnum;
						dateTime = playerStatus.LastOnline;
						this.m_level.SetTextFormat("Lvl {0}", playerStatus.Level);
						BaseArchetype roleFromPacked = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus.RolePacked);
						if (roleFromPacked != null)
						{
							fromPool.AppendLine("\nLvl " + playerStatus.Level.ToString() + " " + roleFromPacked.DisplayName);
							this.m_icon.sprite = roleFromPacked.Icon;
							this.m_icon.color = roleFromPacked.IconTint;
							this.m_iconTooltip.Text = roleFromPacked.DisplayName;
						}
						else
						{
							fromPool.AppendLine("\nLvl " + playerStatus.Level.ToString() + " Unknown Role");
							this.m_icon.sprite = this.m_unknownRoleIcon;
							this.m_icon.color = this.m_defaultRoleIconColor;
							this.m_iconTooltip.Text = "Unknown Role";
						}
					}
					else
					{
						this.m_level.ZStringSetText("Lvl ??");
						fromPool.AppendLine("\nLvl ?? Unknown Role");
						this.m_icon.sprite = this.m_unknownRoleIcon;
						this.m_icon.color = this.m_defaultRoleIconColor;
						this.m_iconTooltip.Text = "Unknown Role";
					}
					ZoneRecord zoneRecord2 = SessionData.GetZoneRecord((ZoneId)num);
					if (num == 0)
					{
						this.m_zone.ZStringSetText("Unknown");
					}
					else if (num == -1)
					{
						this.m_zone.ZStringSetText("Offline");
					}
					else if (num == -2)
					{
						this.m_zone.ZStringSetText("Anonymous");
					}
					else if (zoneRecord2 != null)
					{
						this.m_zone.ZStringSetText(LocalZoneManager.GetFormattedZoneName(zoneRecord2.DisplayName, subZoneId));
					}
					else
					{
						this.m_zone.ZStringSetText("Unknown");
					}
					if (!string.IsNullOrWhiteSpace(this.m_member.PublicNote))
					{
						fromPool.Append("\n\nNote:\n" + this.m_member.PublicNote);
					}
					if (ClientGameManager.SocialManager.OwnGuildRank != null && ClientGameManager.SocialManager.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.ViewOfficerNote) && !string.IsNullOrWhiteSpace(this.m_member.OfficerNote))
					{
						fromPool.Append("\n\nOfficer's Note:\n" + this.m_member.OfficerNote);
					}
					fromPool.Append("\n\nJoined: " + this.m_member.Created.ToLocalTime().ToString("g"));
					if (num <= 0 && dateTime != null)
					{
						fromPool.Append("\nLast Online: " + dateTime.Value.ToLocalTime().ToString("g"));
					}
					this.m_tooltipStr = fromPool.ToString_ReturnToPool();
					return;
				}
			}
		}

		// Token: 0x0600439C RID: 17308 RVA: 0x0006DA9E File Offset: 0x0006BC9E
		private void SendTell()
		{
			UIManager.ActiveChatInput.StartWhisper(this.m_member.CharacterId);
		}

		// Token: 0x0600439D RID: 17309 RVA: 0x0006DAB5 File Offset: 0x0006BCB5
		private void InviteToGroup()
		{
			ClientGameManager.GroupManager.InviteNewMember(this.m_member.CharacterId);
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x0006DACC File Offset: 0x0006BCCC
		private void PromoteGuildMember()
		{
			ClientGameManager.SocialManager.PromoteGuildMember(this.m_member.CharacterId);
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x0006DAE3 File Offset: 0x0006BCE3
		private void DemoteGuildMember()
		{
			ClientGameManager.SocialManager.DemoteGuildMember(this.m_member.CharacterId);
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x0006DAFA File Offset: 0x0006BCFA
		private void KickGuildMember()
		{
			ClientGameManager.SocialManager.KickGuildMember(this.m_member.CharacterId);
		}

		// Token: 0x060043A1 RID: 17313 RVA: 0x00196F9C File Offset: 0x0019519C
		private void EditPublicNote()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Edit Public Note",
				Text = this.m_member.PublicNote,
				ConfirmationText = "Ok",
				CancelText = "Cancel",
				ShowCloseButton = false,
				CharacterLimit = GlobalSettings.Values.Social.GuildPublicNoteCharacterLimit,
				Callback = new Action<bool, object>(this.OnEditPublicNoteConfirmed)
			};
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x060043A2 RID: 17314 RVA: 0x0006DB11 File Offset: 0x0006BD11
		private void OnEditPublicNoteConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.EditPublicNote(this.m_member.CharacterId, (string)obj);
			}
		}

		// Token: 0x060043A3 RID: 17315 RVA: 0x0019702C File Offset: 0x0019522C
		private void EditOfficerNote()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Edit Officer Note",
				Text = this.m_member.OfficerNote,
				ConfirmationText = "Ok",
				CancelText = "Cancel",
				ShowCloseButton = false,
				CharacterLimit = GlobalSettings.Values.Social.GuildOfficerNoteCharacterLimit,
				Callback = new Action<bool, object>(this.OnEditOfficerNoteConfirmed)
			};
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x060043A4 RID: 17316 RVA: 0x0006DB31 File Offset: 0x0006BD31
		private void OnEditOfficerNoteConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.EditOfficerNote(this.m_member.CharacterId, (string)obj);
			}
		}

		// Token: 0x060043A5 RID: 17317 RVA: 0x001970BC File Offset: 0x001952BC
		private void TransferGuild()
		{
			this.m_transferPlayer = this.m_member.CharacterId;
			DialogOptions opts = new DialogOptions
			{
				Title = "Transfer Guild",
				Text = "Are you sure you want to transfer the guild to \"" + this.m_transferPlayer + "\"? They will have complete control of the guild and this transfer may not be recinded.",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnTransferGuildConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060043A6 RID: 17318 RVA: 0x0006DB51 File Offset: 0x0006BD51
		private void OnTransferGuildConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.TransferGuild(this.m_transferPlayer);
			}
			this.m_transferPlayer = null;
		}

		// Token: 0x060043A7 RID: 17319 RVA: 0x0006DB6D File Offset: 0x0006BD6D
		private void Friend()
		{
			ClientGameManager.SocialManager.Friend(this.m_member.CharacterId);
		}

		// Token: 0x060043A8 RID: 17320 RVA: 0x0006DB84 File Offset: 0x0006BD84
		private void Block()
		{
			ClientGameManager.SocialManager.Block(this.m_member.CharacterId);
		}

		// Token: 0x060043A9 RID: 17321 RVA: 0x00197150 File Offset: 0x00195350
		string IContextMenu.FillActionsGetTitle()
		{
			GroupMember groupMember;
			bool flag = !ClientGameManager.GroupManager.IsGrouped || (ClientGameManager.GroupManager.IsLeader && !ClientGameManager.GroupManager.GroupIsFull && !ClientGameManager.GroupManager.TryGetGroupMember(this.m_member.CharacterId, out groupMember));
			bool valueOrDefault = ClientGameManager.SocialManager.IsOnline(this.m_member.CharacterId).GetValueOrDefault();
			this.m_member.CharacterId.Equals(LocalPlayer.GameEntity.CharacterData.Name, StringComparison.CurrentCultureIgnoreCase);
			GuildRank ownGuildRank = ClientGameManager.SocialManager.OwnGuildRank;
			GuildRank rankById = ClientGameManager.SocialManager.Guild.GetRankById(this.m_member.RankId);
			if ((ownGuildRank == null || rankById == null) && (ownGuildRank == null || !ownGuildRank.IsGuildMaster))
			{
				Debug.LogError("Failed to retrieve guild rank data in order to populate guild member context menu.");
				ContextMenuUI.ClearContextActions();
				return "Error";
			}
			GuildRank guildRank = null;
			GuildRank guildRank2 = null;
			if (rankById != null)
			{
				foreach (GuildRank guildRank3 in ClientGameManager.SocialManager.Guild.Ranks)
				{
					if (guildRank3.Sort == rankById.Sort + 1)
					{
						guildRank = guildRank3;
					}
					if (guildRank3.Sort == rankById.Sort - 1)
					{
						guildRank2 = guildRank3;
					}
				}
			}
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Send Tell", valueOrDefault, new Action(this.SendTell), null, null);
			ContextMenuUI.AddContextAction("Invite to Group", valueOrDefault && flag, new Action(this.InviteToGroup), null, null);
			if (rankById != null)
			{
				if (ownGuildRank.Permissions.HasBitFlag(GuildPermissions.Promote))
				{
					ContextMenuUI.AddContextAction("Promote", ownGuildRank.Sort - 1 > rankById.Sort && guildRank != null, new Action(this.PromoteGuildMember), null, null);
				}
				if (ownGuildRank.Permissions.HasBitFlag(GuildPermissions.Demote))
				{
					ContextMenuUI.AddContextAction("Demote", ownGuildRank.Sort > rankById.Sort && guildRank2 != null, new Action(this.DemoteGuildMember), null, null);
				}
			}
			if (ownGuildRank.Permissions.HasBitFlag(GuildPermissions.KickMember))
			{
				ContextMenuUI.AddContextAction("Kick from Guild", ownGuildRank.IsGuildMaster || ownGuildRank.Sort > rankById.Sort, new Action(this.KickGuildMember), null, null);
			}
			if (ownGuildRank.Permissions == (GuildPermissions)(-1))
			{
				ContextMenuUI.AddContextAction("Transfer Guild", true, new Action(this.TransferGuild), null, null);
			}
			ContextMenuUI.AddContextAction(ClientGameManager.SocialManager.HasIncomingFriendRequestFrom(this.m_member.CharacterId) ? "Accept Friend Request" : "Send Friend Request", true, new Action(this.Friend), null, null);
			ContextMenuUI.AddContextAction("Block", true, new Action(this.Block), null, null);
			return this.m_member.CharacterId;
		}

		// Token: 0x17000F4B RID: 3915
		// (get) Token: 0x060043AA RID: 17322 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060043AB RID: 17323 RVA: 0x0006DB9B File Offset: 0x0006BD9B
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_tooltipStr, false);
		}

		// Token: 0x17000F4C RID: 3916
		// (get) Token: 0x060043AC RID: 17324 RVA: 0x0006DBAF File Offset: 0x0006BDAF
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F4D RID: 3917
		// (get) Token: 0x060043AD RID: 17325 RVA: 0x0006DBBD File Offset: 0x0006BDBD
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060043AF RID: 17327 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004026 RID: 16422
		[SerializeField]
		private Image m_icon;

		// Token: 0x04004027 RID: 16423
		[SerializeField]
		private TextMeshProUGUI m_level;

		// Token: 0x04004028 RID: 16424
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04004029 RID: 16425
		[SerializeField]
		private TextMeshProUGUI m_zone;

		// Token: 0x0400402A RID: 16426
		[SerializeField]
		private TextTooltipTrigger m_iconTooltip;

		// Token: 0x0400402B RID: 16427
		[SerializeField]
		private Sprite m_unknownRoleIcon;

		// Token: 0x0400402C RID: 16428
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400402D RID: 16429
		private const string kUnknownRole = "Unknown Role";

		// Token: 0x0400402E RID: 16430
		private GuildMemberList m_parent;

		// Token: 0x0400402F RID: 16431
		private int m_index = -1;

		// Token: 0x04004030 RID: 16432
		private GuildMember m_member;

		// Token: 0x04004031 RID: 16433
		private string m_tooltipStr;

		// Token: 0x04004032 RID: 16434
		private Color m_defaultRoleIconColor;

		// Token: 0x04004033 RID: 16435
		private string m_transferPlayer;
	}
}
