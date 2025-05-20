using System;
using SoL.Game;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003DF RID: 991
	public static class GroupHandler
	{
		// Token: 0x06001A82 RID: 6786 RVA: 0x00109268 File Offset: 0x00107468
		public static void Handle(SolServerCommand cmd)
		{
			string content;
			if (!cmd.State)
			{
				if (cmd.TryGetArgValue("err", out content))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
				return;
			}
			CommandType command = cmd.Command;
			if (command != CommandType.notification)
			{
				switch (command)
				{
				case CommandType.invited:
				case CommandType.selfjoined:
				case CommandType.joined:
				case CommandType.selfdisbaned:
				case CommandType.leave:
				case CommandType.kick:
				case CommandType.promote:
					break;
				case CommandType.disbanded:
					GroupHandler.ResetGroupAndRaid();
					MessageManager.ChatQueue.AddToQueue(MessageType.Party, "The party has been disbanded.");
					return;
				case CommandType.left:
				{
					string text;
					if (cmd.TryGetArgValue("Player", out text))
					{
						if (text == "You")
						{
							content = "You have left the party.";
							GroupHandler.ResetGroupAndRaid();
						}
						else
						{
							content = TextMeshProExtensions.CreatePlayerLink(text) + " has left the party.";
						}
						MessageManager.ChatQueue.AddToQueue(MessageType.Party | MessageType.PreFormatted, content);
						return;
					}
					break;
				}
				case CommandType.kicked:
				{
					string text;
					if (cmd.TryGetArgValue("Player", out text))
					{
						if (text == "You")
						{
							content = "You have been removed from the party.";
							GroupHandler.ResetGroupAndRaid();
						}
						else
						{
							content = TextMeshProExtensions.CreatePlayerLink(text) + " has been removed from the party.";
						}
						MessageManager.ChatQueue.AddToQueue(MessageType.Party | MessageType.PreFormatted, content);
						return;
					}
					break;
				}
				case CommandType.promoted:
				{
					string text;
					if (SessionData.SelectedCharacter != null && cmd.TryGetArgValue("Player", out text))
					{
						content = ((text == SessionData.SelectedCharacter.Name) ? "You have been promoted to leader of the party." : (TextMeshProExtensions.CreatePlayerLink(text) + " has been promoted to leader of the party."));
						MessageManager.ChatQueue.AddToQueue(MessageType.Party | MessageType.PreFormatted, content);
						ClientGameManager.GroupManager.SetLeader(text);
						if (UIManager.RaidWindowUI)
						{
							UIManager.RaidWindowUI.RefreshLeaveRaidButton();
							return;
						}
					}
					break;
				}
				case CommandType.statusupdate:
				{
					string value;
					string leaderName;
					if (cmd.TryGetArgValue("groupid", out value) && cmd.TryGetArgValue("leader", out leaderName))
					{
						GroupMemberZoneStatus[] array = cmd.DeserializeKey("members");
						ClientGameManager.GroupManager.StatusUpdate(new UniqueId(value), leaderName, array);
						if (array != null)
						{
							foreach (GroupMemberZoneStatus groupMemberZoneStatus in array)
							{
								PresenceFlags presence = PresenceFlags.Online;
								PresenceFlags presenceFlags;
								if (ClientGameManager.SocialManager.TryGetLatestPresence(groupMemberZoneStatus.CharacterName, out presenceFlags))
								{
									presence = presenceFlags;
								}
								ClientGameManager.SocialManager.EnqueuePlayerStatusUpdates(new PlayerStatus[]
								{
									new PlayerStatus
									{
										Character = groupMemberZoneStatus.CharacterName,
										Presence = (int)presence,
										ZoneId = groupMemberZoneStatus.ZoneId,
										SubZoneId = groupMemberZoneStatus.SubZoneId,
										Role = groupMemberZoneStatus.Role,
										Level = groupMemberZoneStatus.Level,
										EmberRingIndex = groupMemberZoneStatus.EmberRingIndex,
										LastOnline = new DateTime?(GameTimeReplicator.GetServerCorrectedDateTimeUtc())
									}
								});
							}
						}
						string value2;
						string value3;
						RaidGroup[] groups;
						PlayerStatus[] statuses;
						if (cmd.TryGetArgValue("raidid", out value2) && cmd.TryGetArgValue("leadgroup", out value3) && cmd.TryDeserializeKey("raidgroups", out groups) && cmd.TryDeserializeKey("statuses", out statuses))
						{
							ClientGameManager.SocialManager.UpdateRaid(new UniqueId(value2), new UniqueId(value3), groups, statuses);
							return;
						}
						if (!ClientGameManager.SocialManager.RaidId.IsEmpty)
						{
							ClientGameManager.SocialManager.UpdateRaid(UniqueId.Empty, UniqueId.Empty, null, null);
							return;
						}
					}
					else
					{
						if (ClientGameManager.GroupManager.IsGrouped)
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Party, "The server has disbanded your party.");
						}
						GroupHandler.ResetGroupAndRaid();
					}
					break;
				}
				default:
					return;
				}
			}
			else if (cmd.TryGetArgValue("Message", out content))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				return;
			}
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x00054970 File Offset: 0x00052B70
		private static void ResetGroupAndRaid()
		{
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.ResetRaid();
			}
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.ResetGroup();
			}
		}

		// Token: 0x04002168 RID: 8552
		private const string kPlayerKey = "Player";

		// Token: 0x04002169 RID: 8553
		private const string kYouKey = "You";

		// Token: 0x0400216A RID: 8554
		private const string kGroupIdKey = "groupid";

		// Token: 0x0400216B RID: 8555
		private const string kLeaderKey = "leader";

		// Token: 0x0400216C RID: 8556
		private const string kInviteeKey = "invitee";

		// Token: 0x0400216D RID: 8557
		private const string kMembersKey = "members";

		// Token: 0x0400216E RID: 8558
		private const string kRaidIdKey = "raidid";

		// Token: 0x0400216F RID: 8559
		private const string kRaidGroupsKey = "raidgroups";

		// Token: 0x04002170 RID: 8560
		private const string kLeadGroupKey = "leadgroup";

		// Token: 0x04002171 RID: 8561
		private const string kStatuses = "statuses";

		// Token: 0x04002172 RID: 8562
		public const MessageType kMessageType = MessageType.Party | MessageType.PreFormatted;
	}
}
