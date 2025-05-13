using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using SoL.Networking.SolServer;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000510 RID: 1296
	public class SocialManager : MonoBehaviour
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x0600253A RID: 9530 RVA: 0x0005A950 File Offset: 0x00058B50
		public bool IsGroupInvitePending
		{
			get
			{
				return this.m_pendingIncomingGroupInvite != null;
			}
		}

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x0600253B RID: 9531 RVA: 0x0005A95B File Offset: 0x00058B5B
		public Mail PendingIncomingGroupInvite
		{
			get
			{
				return this.m_pendingIncomingGroupInvite;
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x0600253C RID: 9532 RVA: 0x0005A963 File Offset: 0x00058B63
		public Mail PendingOutgoingGroupInvite
		{
			get
			{
				return this.m_pendingOutgoingGroupInvite;
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x0600253D RID: 9533 RVA: 0x0005A96B File Offset: 0x00058B6B
		public bool IsRaidInvitePending
		{
			get
			{
				return this.m_pendingIncomingRaidInvite != null;
			}
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x0600253E RID: 9534 RVA: 0x0005A976 File Offset: 0x00058B76
		public Mail PendingIncomingRaidInvite
		{
			get
			{
				return this.m_pendingIncomingRaidInvite;
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x0600253F RID: 9535 RVA: 0x0005A97E File Offset: 0x00058B7E
		public Mail PendingOutgoingRaidInvite
		{
			get
			{
				return this.m_pendingOutgoingRaidInvite;
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06002540 RID: 9536 RVA: 0x0005A986 File Offset: 0x00058B86
		public Dictionary<string, Mail> PendingIncomingFriendRequests
		{
			get
			{
				return this.m_pendingIncomingFriendRequests;
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06002541 RID: 9537 RVA: 0x0005A98E File Offset: 0x00058B8E
		public Dictionary<string, Mail> PendingOutgoingFriendRequests
		{
			get
			{
				return this.m_pendingOutgoingFriendRequests;
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06002542 RID: 9538 RVA: 0x0005A996 File Offset: 0x00058B96
		public Dictionary<string, Mail> PendingIncomingGuildInvites
		{
			get
			{
				return this.m_pendingIncomingGuildInvites;
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06002543 RID: 9539 RVA: 0x0005A99E File Offset: 0x00058B9E
		public Dictionary<string, Mail> PendingOutgoingGuildInvites
		{
			get
			{
				return this.m_pendingOutgoingGuildInvites;
			}
		}

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06002544 RID: 9540 RVA: 0x0005A9A6 File Offset: 0x00058BA6
		public Dictionary<string, Mail> PostInbox
		{
			get
			{
				return this.m_postInbox;
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06002545 RID: 9541 RVA: 0x0005A9AE File Offset: 0x00058BAE
		public Dictionary<string, Mail> PostOutbox
		{
			get
			{
				return this.m_postOutbox;
			}
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06002546 RID: 9542 RVA: 0x0005A9B6 File Offset: 0x00058BB6
		public UniqueId RaidId
		{
			get
			{
				return this.m_raidId;
			}
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06002547 RID: 9543 RVA: 0x0005A9BE File Offset: 0x00058BBE
		public UniqueId RaidLeadGroupId
		{
			get
			{
				return this.m_raidLeadGroupId;
			}
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06002548 RID: 9544 RVA: 0x0005A9C6 File Offset: 0x00058BC6
		public RaidGroup[] RaidGroups
		{
			get
			{
				return this.m_raidGroups;
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06002549 RID: 9545 RVA: 0x0005A9CE File Offset: 0x00058BCE
		public Dictionary<string, Relation> Friends
		{
			get
			{
				return this.m_friends;
			}
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x0600254A RID: 9546 RVA: 0x0005A9D6 File Offset: 0x00058BD6
		public Dictionary<string, Relation> BlockList
		{
			get
			{
				return this.m_blockList;
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x0600254B RID: 9547 RVA: 0x0005A9DE File Offset: 0x00058BDE
		public Guild Guild
		{
			get
			{
				return this.m_guild;
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x0600254C RID: 9548 RVA: 0x0005A9E6 File Offset: 0x00058BE6
		public GuildRank OwnGuildRank
		{
			get
			{
				return this.m_ownGuildRank;
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x0600254D RID: 9549 RVA: 0x0005A9EE File Offset: 0x00058BEE
		public Dictionary<string, GuildMember> GuildRoster
		{
			get
			{
				return this.m_guildRoster;
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x0600254E RID: 9550 RVA: 0x0005A9F6 File Offset: 0x00058BF6
		public LookingFor OwnLfgLfmEntry
		{
			get
			{
				return this.m_ownLfgLfmEntry;
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x0600254F RID: 9551 RVA: 0x0005A9FE File Offset: 0x00058BFE
		public Dictionary<string, LookingFor> LookingList
		{
			get
			{
				return this.m_lookingList;
			}
		}

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06002550 RID: 9552 RVA: 0x0013012C File Offset: 0x0012E32C
		// (remove) Token: 0x06002551 RID: 9553 RVA: 0x00130164 File Offset: 0x0012E364
		public event Action<CharacterIdentification, int> NameCheckResponded;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06002552 RID: 9554 RVA: 0x0013019C File Offset: 0x0012E39C
		// (remove) Token: 0x06002553 RID: 9555 RVA: 0x001301D4 File Offset: 0x0012E3D4
		public event Action NewGroupInviteReceived;

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06002554 RID: 9556 RVA: 0x0013020C File Offset: 0x0012E40C
		// (remove) Token: 0x06002555 RID: 9557 RVA: 0x00130244 File Offset: 0x0012E444
		public event Action NewRaidInviteReceived;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06002556 RID: 9558 RVA: 0x0013027C File Offset: 0x0012E47C
		// (remove) Token: 0x06002557 RID: 9559 RVA: 0x001302B4 File Offset: 0x0012E4B4
		public event Action<Mail> NewFriendRequestReceived;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06002558 RID: 9560 RVA: 0x001302EC File Offset: 0x0012E4EC
		// (remove) Token: 0x06002559 RID: 9561 RVA: 0x00130324 File Offset: 0x0012E524
		public event Action<Mail> NewGuildInviteReceived;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x0600255A RID: 9562 RVA: 0x0013035C File Offset: 0x0012E55C
		// (remove) Token: 0x0600255B RID: 9563 RVA: 0x00130394 File Offset: 0x0012E594
		public event Action NewGroupInviteSent;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x0600255C RID: 9564 RVA: 0x001303CC File Offset: 0x0012E5CC
		// (remove) Token: 0x0600255D RID: 9565 RVA: 0x00130404 File Offset: 0x0012E604
		public event Action NewRaidInviteSent;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x0600255E RID: 9566 RVA: 0x0013043C File Offset: 0x0012E63C
		// (remove) Token: 0x0600255F RID: 9567 RVA: 0x00130474 File Offset: 0x0012E674
		public event Action NewFriendRequestSent;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06002560 RID: 9568 RVA: 0x001304AC File Offset: 0x0012E6AC
		// (remove) Token: 0x06002561 RID: 9569 RVA: 0x001304E4 File Offset: 0x0012E6E4
		public event Action NewGuildInviteSent;

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06002562 RID: 9570 RVA: 0x0013051C File Offset: 0x0012E71C
		// (remove) Token: 0x06002563 RID: 9571 RVA: 0x00130554 File Offset: 0x0012E754
		public event Action<Mail> ExpiredGroupInvite;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06002564 RID: 9572 RVA: 0x0013058C File Offset: 0x0012E78C
		// (remove) Token: 0x06002565 RID: 9573 RVA: 0x001305C4 File Offset: 0x0012E7C4
		public event Action<Mail> ExpiredRaidInvite;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x06002566 RID: 9574 RVA: 0x001305FC File Offset: 0x0012E7FC
		// (remove) Token: 0x06002567 RID: 9575 RVA: 0x00130634 File Offset: 0x0012E834
		public event Action<Mail> ExpiredFriendRequest;

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x06002568 RID: 9576 RVA: 0x0013066C File Offset: 0x0012E86C
		// (remove) Token: 0x06002569 RID: 9577 RVA: 0x001306A4 File Offset: 0x0012E8A4
		public event Action<Mail> ExpiredGuildInvite;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x0600256A RID: 9578 RVA: 0x001306DC File Offset: 0x0012E8DC
		// (remove) Token: 0x0600256B RID: 9579 RVA: 0x00130714 File Offset: 0x0012E914
		public event Action<Mail> ExpiredPost;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x0600256C RID: 9580 RVA: 0x0013074C File Offset: 0x0012E94C
		// (remove) Token: 0x0600256D RID: 9581 RVA: 0x00130784 File Offset: 0x0012E984
		public event Action<Mail> ExpiredSentGroupInvite;

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x0600256E RID: 9582 RVA: 0x001307BC File Offset: 0x0012E9BC
		// (remove) Token: 0x0600256F RID: 9583 RVA: 0x001307F4 File Offset: 0x0012E9F4
		public event Action<Mail> ExpiredSentRaidInvite;

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x06002570 RID: 9584 RVA: 0x0013082C File Offset: 0x0012EA2C
		// (remove) Token: 0x06002571 RID: 9585 RVA: 0x00130864 File Offset: 0x0012EA64
		public event Action<Mail> ExpiredSentFriendRequest;

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x06002572 RID: 9586 RVA: 0x0013089C File Offset: 0x0012EA9C
		// (remove) Token: 0x06002573 RID: 9587 RVA: 0x001308D4 File Offset: 0x0012EAD4
		public event Action<Mail> ExpiredSentGuildInvite;

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x06002574 RID: 9588 RVA: 0x0013090C File Offset: 0x0012EB0C
		// (remove) Token: 0x06002575 RID: 9589 RVA: 0x00130944 File Offset: 0x0012EB44
		public event Action<Mail> ExpiredSentPost;

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x06002576 RID: 9590 RVA: 0x0013097C File Offset: 0x0012EB7C
		// (remove) Token: 0x06002577 RID: 9591 RVA: 0x001309B4 File Offset: 0x0012EBB4
		public event Action UnreadMailUpdated;

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x06002578 RID: 9592 RVA: 0x001309EC File Offset: 0x0012EBEC
		// (remove) Token: 0x06002579 RID: 9593 RVA: 0x00130A24 File Offset: 0x0012EC24
		public event Action RaidUpdated;

		// Token: 0x14000060 RID: 96
		// (add) Token: 0x0600257A RID: 9594 RVA: 0x00130A5C File Offset: 0x0012EC5C
		// (remove) Token: 0x0600257B RID: 9595 RVA: 0x00130A94 File Offset: 0x0012EC94
		public event Action FriendRequestsUpdated;

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x0600257C RID: 9596 RVA: 0x00130ACC File Offset: 0x0012ECCC
		// (remove) Token: 0x0600257D RID: 9597 RVA: 0x00130B04 File Offset: 0x0012ED04
		public event Action FriendsListUpdated;

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x0600257E RID: 9598 RVA: 0x00130B3C File Offset: 0x0012ED3C
		// (remove) Token: 0x0600257F RID: 9599 RVA: 0x00130B74 File Offset: 0x0012ED74
		public event Action BlockListUpdated;

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x06002580 RID: 9600 RVA: 0x00130BAC File Offset: 0x0012EDAC
		// (remove) Token: 0x06002581 RID: 9601 RVA: 0x00130BE4 File Offset: 0x0012EDE4
		public event Action GuildInvitesUpdated;

		// Token: 0x14000064 RID: 100
		// (add) Token: 0x06002582 RID: 9602 RVA: 0x00130C1C File Offset: 0x0012EE1C
		// (remove) Token: 0x06002583 RID: 9603 RVA: 0x00130C54 File Offset: 0x0012EE54
		public event Action GuildUpdated;

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x06002584 RID: 9604 RVA: 0x00130C8C File Offset: 0x0012EE8C
		// (remove) Token: 0x06002585 RID: 9605 RVA: 0x00130CC4 File Offset: 0x0012EEC4
		public event Action GuildRosterUpdated;

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x06002586 RID: 9606 RVA: 0x00130CFC File Offset: 0x0012EEFC
		// (remove) Token: 0x06002587 RID: 9607 RVA: 0x00130D34 File Offset: 0x0012EF34
		public event Action<GuildMember> GuildMemberJoined;

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06002588 RID: 9608 RVA: 0x00130D6C File Offset: 0x0012EF6C
		// (remove) Token: 0x06002589 RID: 9609 RVA: 0x00130DA4 File Offset: 0x0012EFA4
		public event Action<GuildMember> GuildMemberPromoted;

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x0600258A RID: 9610 RVA: 0x00130DDC File Offset: 0x0012EFDC
		// (remove) Token: 0x0600258B RID: 9611 RVA: 0x00130E14 File Offset: 0x0012F014
		public event Action<GuildMember> GuildMemberDemoted;

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x0600258C RID: 9612 RVA: 0x00130E4C File Offset: 0x0012F04C
		// (remove) Token: 0x0600258D RID: 9613 RVA: 0x00130E84 File Offset: 0x0012F084
		public event Action<string> GuildMemberLeft;

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x0600258E RID: 9614 RVA: 0x00130EBC File Offset: 0x0012F0BC
		// (remove) Token: 0x0600258F RID: 9615 RVA: 0x00130EF4 File Offset: 0x0012F0F4
		public event Action PostInboxUpdated;

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06002590 RID: 9616 RVA: 0x00130F2C File Offset: 0x0012F12C
		// (remove) Token: 0x06002591 RID: 9617 RVA: 0x00130F64 File Offset: 0x0012F164
		public event Action PostOutboxUpdated;

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x06002592 RID: 9618 RVA: 0x00130F9C File Offset: 0x0012F19C
		// (remove) Token: 0x06002593 RID: 9619 RVA: 0x00130FD4 File Offset: 0x0012F1D4
		public event Action PlayerStatusesChanged;

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x06002594 RID: 9620 RVA: 0x0013100C File Offset: 0x0012F20C
		// (remove) Token: 0x06002595 RID: 9621 RVA: 0x00131044 File Offset: 0x0012F244
		public event Action PlayerOnline;

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x06002596 RID: 9622 RVA: 0x0013107C File Offset: 0x0012F27C
		// (remove) Token: 0x06002597 RID: 9623 RVA: 0x001310B4 File Offset: 0x0012F2B4
		public event Action PlayerOffline;

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x06002598 RID: 9624 RVA: 0x001310EC File Offset: 0x0012F2EC
		// (remove) Token: 0x06002599 RID: 9625 RVA: 0x00131124 File Offset: 0x0012F324
		public event Action LookingUpdated;

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x0600259A RID: 9626 RVA: 0x0013115C File Offset: 0x0012F35C
		// (remove) Token: 0x0600259B RID: 9627 RVA: 0x00131194 File Offset: 0x0012F394
		public event Action LookingListUpdated;

		// Token: 0x0600259C RID: 9628 RVA: 0x001311CC File Offset: 0x0012F3CC
		public void Reset()
		{
			this.m_nextMailExpiration = null;
			this.m_mailQueue.Clear();
			this.m_relQueue.Clear();
			this.m_guildRosterQueue.Clear();
			this.m_guildRosterRemovalQueue.Clear();
			this.m_playerUpdateQueue.Clear();
			this.m_lookingQueue.Clear();
			this.m_pendingIncomingGroupInvite = null;
			this.m_pendingOutgoingGroupInvite = null;
			this.m_pendingIncomingFriendRequests.Clear();
			this.m_pendingOutgoingFriendRequests.Clear();
			this.m_pendingIncomingGuildInvites.Clear();
			this.m_pendingOutgoingGuildInvites.Clear();
			this.m_postInbox.Clear();
			this.m_postOutbox.Clear();
			this.m_friends.Clear();
			this.m_blockList.Clear();
			this.m_latestRelationStatuses.Clear();
			this.m_guild = null;
			this.m_ownGuildRank = null;
			this.m_guildUpdated = false;
			this.m_guildRoster.Clear();
			this.m_ownLfgLfmEntry = null;
			this.m_lookingList.Clear();
			this.UnreadMailNotificationDismissed = false;
			Action friendRequestsUpdated = this.FriendRequestsUpdated;
			if (friendRequestsUpdated != null)
			{
				friendRequestsUpdated();
			}
			Action friendsListUpdated = this.FriendsListUpdated;
			if (friendsListUpdated != null)
			{
				friendsListUpdated();
			}
			Action blockListUpdated = this.BlockListUpdated;
			if (blockListUpdated != null)
			{
				blockListUpdated();
			}
			Action playerStatusesChanged = this.PlayerStatusesChanged;
			if (playerStatusesChanged == null)
			{
				return;
			}
			playerStatusesChanged();
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x0005AA06 File Offset: 0x00058C06
		private void Start()
		{
			this.GuildMemberJoined += this.OnGuildMemberJoined;
			this.GuildMemberPromoted += this.OnGuildMemberPromoted;
			this.GuildMemberDemoted += this.OnGuildMemberDemoted;
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x0005AA3E File Offset: 0x00058C3E
		private void OnDestroy()
		{
			this.GuildMemberJoined -= this.OnGuildMemberJoined;
			this.GuildMemberPromoted -= this.OnGuildMemberPromoted;
			this.GuildMemberDemoted -= this.OnGuildMemberDemoted;
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x00131314 File Offset: 0x0012F514
		private void Update()
		{
			if (LocalPlayer.GameEntity)
			{
				if (this.m_mailQueue.Count > 0)
				{
					this.ProcessNewMail();
				}
				if (this.m_relQueue.Count > 0)
				{
					this.ProcessNewRelations();
				}
				if (this.m_guildUpdated || this.m_guildRosterQueue.Count > 0 || this.m_guildRosterRemovalQueue.Count > 0)
				{
					this.ProcessGuildUpdates();
				}
				if (this.m_playerUpdateQueue.Count > 0)
				{
					this.ProcessNewPlayerStatusUpdates();
				}
				if (this.m_lookingQueueNeedsProcessing)
				{
					this.ProcessNewLookingEntries();
				}
			}
			if (this.m_nextMailExpiration != null)
			{
				DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
				if (this.m_nextMailExpiration <= serverCorrectedDateTimeUtc)
				{
					this.ExpireMail(serverCorrectedDateTimeUtc);
				}
			}
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x001313E0 File Offset: 0x0012F5E0
		public void UpdateRelationName(string oldName, string newName)
		{
			Mail mail = null;
			Relation relation = null;
			string key = oldName.ToLower();
			string key2 = newName.ToLower();
			if (this.m_pendingIncomingFriendRequests.TryGetValue(key, out mail))
			{
				if (mail.Sender == oldName)
				{
					mail.Sender = newName;
				}
				if (mail.Recipient == oldName)
				{
					mail.Recipient = newName;
				}
				this.m_pendingIncomingFriendRequests.Add(key2, mail);
				this.m_pendingIncomingFriendRequests.Remove(key);
			}
			if (this.m_pendingOutgoingFriendRequests.TryGetValue(key, out mail))
			{
				if (mail.Sender == oldName)
				{
					mail.Sender = newName;
				}
				if (mail.Recipient == oldName)
				{
					mail.Recipient = newName;
				}
				this.m_pendingOutgoingFriendRequests.Add(key2, mail);
				this.m_pendingOutgoingFriendRequests.Remove(key);
			}
			if (this.m_pendingIncomingGuildInvites.TryGetValue(key, out mail))
			{
				if (mail.Sender == oldName)
				{
					mail.Sender = newName;
				}
				if (mail.Recipient == oldName)
				{
					mail.Recipient = newName;
				}
				this.m_pendingIncomingGuildInvites.Add(key2, mail);
				this.m_pendingIncomingGuildInvites.Remove(key);
			}
			if (this.m_pendingOutgoingGuildInvites.TryGetValue(key, out mail))
			{
				if (mail.Sender == oldName)
				{
					mail.Sender = newName;
				}
				if (mail.Recipient == oldName)
				{
					mail.Recipient = newName;
				}
				this.m_pendingOutgoingGuildInvites.Add(key2, mail);
				this.m_pendingOutgoingGuildInvites.Remove(key);
			}
			if (this.m_friends.TryGetValue(key, out relation))
			{
				relation.OtherName = newName;
				this.m_friends.Add(key2, relation);
				this.m_friends.Remove(key);
			}
			if (this.m_blockList.TryGetValue(key, out relation))
			{
				relation.OtherName = newName;
				this.m_blockList.Add(key2, relation);
				this.m_blockList.Remove(key);
			}
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(key, out playerStatus))
			{
				playerStatus.Character = newName;
				this.m_latestRelationStatuses.Add(key2, playerStatus);
				this.m_latestRelationStatuses.Remove(key);
			}
			GuildMember guildMember;
			if (this.m_guildRoster.TryGetValue(key, out guildMember))
			{
				guildMember.CharacterId = newName;
				this.m_guildRoster.Add(key2, guildMember);
				this.m_guildRoster.Remove(key);
			}
			foreach (LookingFor lookingFor in this.m_lookingList.Values)
			{
				if (lookingFor.ContactName == oldName)
				{
					lookingFor.ContactName = newName;
				}
			}
			Action friendRequestsUpdated = this.FriendRequestsUpdated;
			if (friendRequestsUpdated != null)
			{
				friendRequestsUpdated();
			}
			Action guildInvitesUpdated = this.GuildInvitesUpdated;
			if (guildInvitesUpdated != null)
			{
				guildInvitesUpdated();
			}
			Action friendsListUpdated = this.FriendsListUpdated;
			if (friendsListUpdated != null)
			{
				friendsListUpdated();
			}
			Action blockListUpdated = this.BlockListUpdated;
			if (blockListUpdated != null)
			{
				blockListUpdated();
			}
			Action playerStatusesChanged = this.PlayerStatusesChanged;
			if (playerStatusesChanged != null)
			{
				playerStatusesChanged();
			}
			Action guildRosterUpdated = this.GuildRosterUpdated;
			if (guildRosterUpdated != null)
			{
				guildRosterUpdated();
			}
			Action lookingListUpdated = this.LookingListUpdated;
			if (lookingListUpdated == null)
			{
				return;
			}
			lookingListUpdated();
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x0005AA76 File Offset: 0x00058C76
		public bool TryGetPlayerIdentById(UniqueId id, out CharacterIdentification ident)
		{
			return this.m_nameCheckCache.TryGetValue(id, out ident);
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x001316EC File Offset: 0x0012F8EC
		public void NameCheck(string playerName)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.namecheck);
			solServerCommand.Args.Add("name", playerName);
			solServerCommand.Send();
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x0005AA85 File Offset: 0x00058C85
		public void NameCheckResponse(CharacterIdentification ident, int attachmentsRemaining)
		{
			if (ident != null)
			{
				this.m_nameCheckCache.AddOrReplace(ident._id, ident);
			}
			Action<CharacterIdentification, int> nameCheckResponded = this.NameCheckResponded;
			if (nameCheckResponded == null)
			{
				return;
			}
			nameCheckResponded(ident, attachmentsRemaining);
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x060025A4 RID: 9636 RVA: 0x0005AAAE File Offset: 0x00058CAE
		public bool IsInGroup
		{
			get
			{
				return ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped;
			}
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x0013171C File Offset: 0x0012F91C
		public void SendGroupInvite(string playerName)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.send);
			solServerCommand.Args.Add("Message", new Mail
			{
				Type = MailType.GroupInvite,
				Sender = LocalPlayer.GameEntity.CharacterData.Name.Value,
				Recipient = playerName
			});
			solServerCommand.Send();
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x00131778 File Offset: 0x0012F978
		public void AcceptGroupInvite()
		{
			if (this.IsGroupInvitePending)
			{
				SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.accept);
				solServerCommand.Args.Add("mailid", this.m_pendingIncomingGroupInvite._id);
				solServerCommand.Args.Add("type", MailType.GroupInvite);
				solServerCommand.Send();
				this.DeleteGroupInvite();
			}
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x001317D8 File Offset: 0x0012F9D8
		public void DeclineGroupInvite()
		{
			if (this.IsGroupInvitePending)
			{
				SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.decline);
				solServerCommand.Args.Add("mailid", this.m_pendingIncomingGroupInvite._id);
				solServerCommand.Args.Add("type", MailType.GroupInvite);
				solServerCommand.Send();
				this.DeleteGroupInvite();
			}
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x00131838 File Offset: 0x0012FA38
		public void SendRaidInvite(string playerName)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.send);
			solServerCommand.Args.Add("Message", new Mail
			{
				Type = MailType.RaidInvite,
				Sender = LocalPlayer.GameEntity.CharacterData.Name.Value,
				Recipient = playerName
			});
			solServerCommand.Send();
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x00131894 File Offset: 0x0012FA94
		public void AcceptRaidInvite()
		{
			if (this.IsRaidInvitePending)
			{
				SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.accept);
				solServerCommand.Args.Add("mailid", this.m_pendingIncomingRaidInvite._id);
				solServerCommand.Args.Add("type", MailType.RaidInvite);
				solServerCommand.Send();
				this.DeleteRaidInvite();
			}
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x001318F4 File Offset: 0x0012FAF4
		public void DeclineRaidInvite()
		{
			if (this.IsRaidInvitePending)
			{
				SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.decline);
				solServerCommand.Args.Add("mailid", this.m_pendingIncomingRaidInvite._id);
				solServerCommand.Args.Add("type", MailType.RaidInvite);
				solServerCommand.Send();
				this.DeleteRaidInvite();
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x060025AB RID: 9643 RVA: 0x0005AAC8 File Offset: 0x00058CC8
		public bool IsInRaid
		{
			get
			{
				return !this.m_raidId.IsEmpty;
			}
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x00131954 File Offset: 0x0012FB54
		public bool IsRaidMember(string playerName)
		{
			if (this.IsInGroup)
			{
				using (IEnumerator<GroupMember> enumerator = ClientGameManager.GroupManager.GetAllGroupMembers().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
						{
							return true;
						}
					}
				}
			}
			if (this.m_raidGroups != null)
			{
				RaidGroup[] raidGroups = this.m_raidGroups;
				for (int i = 0; i < raidGroups.Length; i++)
				{
					using (List<string>.Enumerator enumerator2 = raidGroups[i].Members.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x00131A24 File Offset: 0x0012FC24
		public bool IsRaidLeader(string playerName)
		{
			if (ClientGroupManager.IsMyGroup(this.m_raidLeadGroupId) && ClientGameManager.GroupManager && ClientGameManager.GroupManager.Leader != null && !string.IsNullOrWhiteSpace(ClientGameManager.GroupManager.Leader.Name))
			{
				return ClientGameManager.GroupManager.Leader.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase);
			}
			if (this.m_raidGroups != null)
			{
				foreach (RaidGroup raidGroup in this.m_raidGroups)
				{
					if (raidGroup != null && raidGroup.GroupId == this.m_raidLeadGroupId)
					{
						return raidGroup.Leader.Equals(playerName, StringComparison.CurrentCultureIgnoreCase);
					}
				}
			}
			return false;
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x00131AD0 File Offset: 0x0012FCD0
		public string GetRaidLeader()
		{
			if (ClientGroupManager.IsMyGroup(this.m_raidLeadGroupId))
			{
				return ClientGameManager.GroupManager.Leader.Name;
			}
			if (this.m_raidGroups != null)
			{
				foreach (RaidGroup raidGroup in this.m_raidGroups)
				{
					if (raidGroup.GroupId == this.m_raidLeadGroupId)
					{
						return raidGroup.Leader;
					}
				}
			}
			return null;
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x0005AAD8 File Offset: 0x00058CD8
		public void ResetRaid()
		{
			this.UpdateRaid(UniqueId.Empty, UniqueId.Empty, null, null);
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x00131B3C File Offset: 0x0012FD3C
		public void UpdateRaid(UniqueId raidId, UniqueId leadGroupId, RaidGroup[] groups, PlayerStatus[] statuses)
		{
			UniqueId raidId2 = this.m_raidId;
			this.m_raidId = raidId;
			this.m_raidLeadGroupId = leadGroupId;
			this.m_raidGroups = groups;
			this.EnqueuePlayerStatusUpdates(statuses);
			this.ProcessNewPlayerStatusUpdates();
			Action raidUpdated = this.RaidUpdated;
			if (raidUpdated != null)
			{
				raidUpdated();
			}
			if (raidId2 != this.m_raidId)
			{
				if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.NetworkEntity == null || LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler == null)
				{
					if (this.m_delayedRaidIdSet != null)
					{
						base.StopCoroutine(this.m_delayedRaidIdSet);
					}
					this.m_delayedRaidIdSet = this.DelayedRaidIdSet();
					base.StartCoroutine(this.m_delayedRaidIdSet);
					return;
				}
				this.SetServerRaidId();
			}
		}

		// Token: 0x060025B1 RID: 9649 RVA: 0x0005AAEC File Offset: 0x00058CEC
		private IEnumerator DelayedRaidIdSet()
		{
			while (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.NetworkEntity == null || LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler == null)
			{
				yield return null;
			}
			this.SetServerRaidId();
			yield break;
		}

		// Token: 0x060025B2 RID: 9650 RVA: 0x00131BFC File Offset: 0x0012FDFC
		private void SetServerRaidId()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.NetworkEntity && LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler)
			{
				Debug.Log("Setting RaidId to " + this.RaidId.Value);
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.SetRaidId(this.RaidId);
			}
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x00131C74 File Offset: 0x0012FE74
		public bool? IsOnline(string playerName)
		{
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				return new bool?((playerStatus.PresenceFlags & PresenceFlags.Invisible) == PresenceFlags.Invalid && (playerStatus.ZoneId > 0 || (playerStatus.PresenceFlags & PresenceFlags.Anonymous) > PresenceFlags.Invalid));
			}
			return null;
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x0005AAFB File Offset: 0x00058CFB
		public bool IsAfk()
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot determine local player status, game not initialized.");
			}
			return (LocalPlayer.GameEntity.CharacterData.PresenceFlags & (PresenceFlags.AwayUserSet | PresenceFlags.AwayAutomatic)) > PresenceFlags.Invalid;
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x00131CC8 File Offset: 0x0012FEC8
		public bool? IsAfk(string playerName)
		{
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				return new bool?((playerStatus.PresenceFlags & (PresenceFlags.AwayUserSet | PresenceFlags.AwayAutomatic)) > PresenceFlags.Invalid);
			}
			return null;
		}

		// Token: 0x060025B6 RID: 9654 RVA: 0x0005AB29 File Offset: 0x00058D29
		public bool IsDnD()
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot determine local player status, game not initialized.");
			}
			return (LocalPlayer.GameEntity.CharacterData.PresenceFlags & PresenceFlags.DoNotDisturb) > PresenceFlags.Invalid;
		}

		// Token: 0x060025B7 RID: 9655 RVA: 0x00131D00 File Offset: 0x0012FF00
		public bool? IsDnD(string playerName)
		{
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				return new bool?((playerStatus.PresenceFlags & PresenceFlags.DoNotDisturb) > PresenceFlags.Invalid);
			}
			return null;
		}

		// Token: 0x060025B8 RID: 9656 RVA: 0x0005AB57 File Offset: 0x00058D57
		public bool IsAnonymous()
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot determine local player status, game not initialized.");
			}
			return (LocalPlayer.GameEntity.CharacterData.PresenceFlags & PresenceFlags.Anonymous) > PresenceFlags.Invalid;
		}

		// Token: 0x060025B9 RID: 9657 RVA: 0x00131D38 File Offset: 0x0012FF38
		public bool? IsAnonymous(string playerName)
		{
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				return new bool?((playerStatus.PresenceFlags & PresenceFlags.Anonymous) > PresenceFlags.Invalid);
			}
			return null;
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x0005AB86 File Offset: 0x00058D86
		public bool IsInvisible()
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot determine local player status, game not initialized.");
			}
			return (LocalPlayer.GameEntity.CharacterData.PresenceFlags & PresenceFlags.Invisible) > PresenceFlags.Invalid;
		}

		// Token: 0x060025BB RID: 9659 RVA: 0x00131D70 File Offset: 0x0012FF70
		public bool? IsInvisible(string playerName)
		{
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				return new bool?((playerStatus.PresenceFlags & PresenceFlags.Invisible) > PresenceFlags.Invalid);
			}
			return null;
		}

		// Token: 0x060025BC RID: 9660 RVA: 0x0005ABB5 File Offset: 0x00058DB5
		public bool TryGetLatestStatus(string playerName, out PlayerStatus status)
		{
			return this.m_latestRelationStatuses.TryGetValue(playerName, out status);
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x00131DA8 File Offset: 0x0012FFA8
		public bool TryGetLatestZone(string playerName, out int zoneId, out byte subZoneId)
		{
			zoneId = 0;
			subZoneId = 0;
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				zoneId = playerStatus.ZoneId;
				subZoneId = playerStatus.SubZoneId;
				return true;
			}
			return false;
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x00131DE0 File Offset: 0x0012FFE0
		public bool TryGetLatestPresence(string playerName, out PresenceFlags presenceFlags)
		{
			presenceFlags = PresenceFlags.Invalid;
			PlayerStatus playerStatus;
			if (this.m_latestRelationStatuses.TryGetValue(playerName, out playerStatus))
			{
				presenceFlags = playerStatus.PresenceFlags;
				return true;
			}
			return false;
		}

		// Token: 0x060025BF RID: 9663 RVA: 0x0005ABC4 File Offset: 0x00058DC4
		public bool IsFriend(string playerName)
		{
			return this.m_friends.ContainsKey(playerName.ToLower());
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x00131E0C File Offset: 0x0013000C
		public bool IsMemberOfMyGuild(string playerName)
		{
			GuildMember guildMember;
			return this.IsInGuild && this.m_guildRoster.TryGetValue(playerName.ToLower(), out guildMember);
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x0005ABD7 File Offset: 0x00058DD7
		public bool HasIncomingFriendRequestFrom(string playerName)
		{
			return this.m_pendingIncomingFriendRequests.ContainsKey(playerName.ToLower());
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x00131E38 File Offset: 0x00130038
		public void Friend(string playerName)
		{
			string key = playerName.ToLower();
			if (this.m_friends.ContainsKey(key))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are already friends with this person.");
				return;
			}
			if (this.m_pendingIncomingFriendRequests.ContainsKey(key))
			{
				this.AcceptFriendRequest(this.m_pendingIncomingFriendRequests[key]._id);
				return;
			}
			if (this.m_pendingOutgoingFriendRequests.ContainsKey(key))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You already have a friend request pending.");
				return;
			}
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to request friends.");
				return;
			}
			this.SendFriendRequest(playerName);
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x00131EE0 File Offset: 0x001300E0
		public void Unfriend(string playerName)
		{
			string key = playerName.ToLower();
			if (this.m_friends.ContainsKey(key))
			{
				this.DeleteRelation(this.m_friends[key]._id);
				return;
			}
			if (this.m_pendingOutgoingFriendRequests.ContainsKey(key))
			{
				this.RejectFriendRequest(this.m_pendingOutgoingFriendRequests[key]._id);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not currently friends with this person.");
		}

		// Token: 0x060025C4 RID: 9668 RVA: 0x00131F58 File Offset: 0x00130158
		private void SendFriendRequest(string playerName)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.send);
			solServerCommand.Args.AddOrReplace("Message", new Mail
			{
				Sender = LocalPlayer.GameEntity.CharacterData.Name.Value,
				Recipient = playerName,
				Type = MailType.FriendRequest
			});
			solServerCommand.Send();
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x00131FB4 File Offset: 0x001301B4
		public void AcceptFriendRequest(string mailId)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.accept);
			solServerCommand.Args.Add("mailid", mailId);
			solServerCommand.Args.Add("type", MailType.FriendRequest);
			solServerCommand.Send();
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x00131FFC File Offset: 0x001301FC
		public void RejectFriendRequest(string mailId)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.decline);
			solServerCommand.Args.Add("mailid", mailId);
			solServerCommand.Args.Add("type", MailType.FriendRequest);
			solServerCommand.Send();
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x0005ABEA File Offset: 0x00058DEA
		public bool IsBlocked(string playerName)
		{
			return this.m_blockList.ContainsKey(playerName.ToLower());
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x00132044 File Offset: 0x00130244
		public void Block(string playerName)
		{
			string text = playerName.ToLower();
			if (!this.m_blockList.ContainsKey(text))
			{
				if (this.m_friends.ContainsKey(text))
				{
					this.DeleteRelation(this.m_friends[text]._id);
				}
				if (this.m_pendingIncomingFriendRequests.ContainsKey(text))
				{
					this.RejectFriendRequest(this.m_pendingIncomingFriendRequests[text]._id);
				}
				if (this.m_pendingOutgoingFriendRequests.ContainsKey(text))
				{
					this.RejectFriendRequest(this.m_pendingOutgoingFriendRequests[text]._id);
				}
				if (this.m_pendingIncomingGroupInvite != null && this.m_pendingIncomingGroupInvite.Sender.Equals(text, StringComparison.CurrentCultureIgnoreCase))
				{
					this.DeclineGroupInvite();
				}
				SolServerCommand solServerCommand = CommandClass.relations.NewCommand(CommandType.add);
				solServerCommand.Args.Add("target", playerName);
				solServerCommand.Args.Add("type", RelationType.Blocked);
				solServerCommand.Send();
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are already blocking this person.");
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x00132148 File Offset: 0x00130348
		public void Unblock(string playerName)
		{
			string key = playerName.ToLower();
			if (this.m_blockList.ContainsKey(key))
			{
				this.DeleteRelation(this.m_blockList[key]._id);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not currently blocking this person.");
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x00132198 File Offset: 0x00130398
		public void DeleteRelation(string relationId)
		{
			SolServerCommand solServerCommand = CommandClass.relations.NewCommand(CommandType.delete);
			solServerCommand.Args.Add("relid", relationId);
			solServerCommand.Send();
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x060025CB RID: 9675 RVA: 0x0005ABFD File Offset: 0x00058DFD
		public bool IsInGuild
		{
			get
			{
				return this.Guild != null;
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x060025CC RID: 9676 RVA: 0x0005AC08 File Offset: 0x00058E08
		public bool CanViewGuildChat
		{
			get
			{
				return this.IsInGuild && this.OwnGuildRank != null && this.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.ChatListen);
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x060025CD RID: 9677 RVA: 0x0005AC31 File Offset: 0x00058E31
		public bool CanSendGuildChat
		{
			get
			{
				return this.IsInGuild && this.OwnGuildRank != null && this.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.ChatSpeak);
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x060025CE RID: 9678 RVA: 0x0005AC5A File Offset: 0x00058E5A
		public bool CanViewOfficerChat
		{
			get
			{
				return this.IsInGuild && this.OwnGuildRank != null && this.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.OfficerChatListen);
			}
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x060025CF RID: 9679 RVA: 0x0005AC83 File Offset: 0x00058E83
		public bool CanSendOfficerChat
		{
			get
			{
				return this.IsInGuild && this.OwnGuildRank != null && this.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.OfficerChatSpeak);
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x060025D0 RID: 9680 RVA: 0x0005ACAC File Offset: 0x00058EAC
		public bool CanInviteGuildMembers
		{
			get
			{
				return this.IsInGuild && this.OwnGuildRank != null && this.OwnGuildRank.Permissions.HasBitFlag(GuildPermissions.InviteMember);
			}
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x001321C8 File Offset: 0x001303C8
		public void CreateNewGuild(string guildName)
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot create new guild, game not initialized.");
			}
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild != null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot create a new guild if you are already in one. Leave your current guild first.");
				return;
			}
			if (string.IsNullOrWhiteSpace(guildName))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "That is not a valid name for a guild.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.create);
			solServerCommand.Args.Add("name", guildName);
			solServerCommand.Args.Add("master", LocalPlayer.GameEntity.CharacterData.Name.Value);
			solServerCommand.Send();
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x0013228C File Offset: 0x0013048C
		public void InviteToGuild(string playerName)
		{
			if (LocalPlayer.GameEntity == null)
			{
				throw new InvalidOperationException("Cannot invite player to guild, game not initialized.");
			}
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not currently in a guild.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.InviteMember) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to invite new players to the guild.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.send);
			solServerCommand.Args.Add("Message", new Mail
			{
				Sender = LocalPlayer.GameEntity.CharacterData.Name.Value,
				Recipient = playerName,
				Type = MailType.GuildInvite
			});
			solServerCommand.Send();
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x00132368 File Offset: 0x00130568
		public void AcceptGuildInvite(string mailId)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.accept);
			solServerCommand.Args.Add("mailid", mailId);
			solServerCommand.Args.Add("type", MailType.GuildInvite);
			solServerCommand.Send();
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x001323B0 File Offset: 0x001305B0
		public void RejectGuildInvite(string mailId)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.decline);
			solServerCommand.Args.Add("mailid", mailId);
			solServerCommand.Args.Add("type", MailType.GuildInvite);
			solServerCommand.Send();
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x001323F8 File Offset: 0x001305F8
		public void PromoteGuildMember(string playerName)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot promote members of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.Promote) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to promote guild members.");
				return;
			}
			GuildRank rankForGuildMember = this.GetRankForGuildMember(playerName);
			if (rankForGuildMember == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Could not find the specified player.");
				return;
			}
			if (this.m_ownGuildRank.Sort - 1 > rankForGuildMember.Sort)
			{
				SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.promote);
				solServerCommand.Args.Add("character", playerName);
				solServerCommand.Send();
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to promote that member.");
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x001324DC File Offset: 0x001306DC
		public void DemoteGuildMember(string playerName)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot demote members of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.Demote) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to demote guild members.");
				return;
			}
			GuildRank rankForGuildMember = this.GetRankForGuildMember(playerName);
			if (rankForGuildMember == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Could not find the specified player.");
				return;
			}
			if (this.m_ownGuildRank.Sort > rankForGuildMember.Sort)
			{
				SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.demote);
				solServerCommand.Args.Add("character", playerName);
				solServerCommand.Send();
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to demote that member.");
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x001325C0 File Offset: 0x001307C0
		public void KickGuildMember(string playerName)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot kick members of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.KickMember) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to kick guild members.");
				return;
			}
			GuildRank rankForGuildMember = this.GetRankForGuildMember(playerName);
			if (rankForGuildMember == null && !this.m_ownGuildRank.IsGuildMaster)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Could not find the specified player.");
				return;
			}
			if (this.m_ownGuildRank.IsGuildMaster || this.m_ownGuildRank.Sort > rankForGuildMember.Sort)
			{
				SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.kick);
				solServerCommand.Args.Add("character", playerName);
				solServerCommand.Send();
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to kick that member.");
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x001326C0 File Offset: 0x001308C0
		public GuildRank GetRankForGuildMember(string playerName)
		{
			string key = playerName.ToLower();
			GuildMember guildMember;
			if (this.m_guildRoster.TryGetValue(key, out guildMember))
			{
				foreach (GuildRank guildRank in this.m_guild.Ranks)
				{
					if (guildRank._id == guildMember.RankId)
					{
						return guildRank;
					}
				}
			}
			return null;
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x00132744 File Offset: 0x00130944
		public void UpdateGuildDescription(string description)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit the description of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditDescription) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit the guild description.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.editdescription);
			solServerCommand.Args.Add("Message", description);
			solServerCommand.Send();
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x001327DC File Offset: 0x001309DC
		public void UpdateGuildMotd(string motd)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit the message of the day of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditMotd) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit the guild's message of the day.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.editmotd);
			solServerCommand.Args.Add("Message", motd);
			solServerCommand.Send();
		}

		// Token: 0x060025DB RID: 9691 RVA: 0x00132874 File Offset: 0x00130A74
		public void AddRank(string name, int sort)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit ranks of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditRanks) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit guild ranks.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.addrank);
			solServerCommand.Args.Add("name", name);
			solServerCommand.Args.Add("sort", sort);
			solServerCommand.Send();
		}

		// Token: 0x060025DC RID: 9692 RVA: 0x00132920 File Offset: 0x00130B20
		public void UpdateGuildRank(string rankId, GuildRank rank)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit ranks of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditRanks) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit guild ranks.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.editrank);
			solServerCommand.Args.Add("rankid", rankId);
			solServerCommand.Args.Add("rank", rank);
			solServerCommand.Send();
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x001329C8 File Offset: 0x00130BC8
		public void RemoveGuildRank(string rankId)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit ranks of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditRanks) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit guild ranks.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.removerank);
			solServerCommand.Args.Add("rankid", rankId);
			solServerCommand.Send();
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x00132A60 File Offset: 0x00130C60
		public void EditPublicNote(string player, string note)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit notes of a guild you yourself are not in.");
				return;
			}
			if (!player.Equals(LocalPlayer.GameEntity.CharacterData.Name, StringComparison.CurrentCultureIgnoreCase) && (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditPublicNote) == GuildPermissions.None))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit public notes.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.editpublicnote);
			solServerCommand.Args.Add("character", player);
			solServerCommand.Args.Add("Message", note);
			solServerCommand.Send();
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x00132B28 File Offset: 0x00130D28
		public void EditOfficerNote(string player, string note)
		{
			if (SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You must purchase the game to participate in a guild.");
				return;
			}
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You cannot edit notes of a guild you yourself are not in.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.EditOfficerNote) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to edit officer notes.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.editofficernote);
			solServerCommand.Args.Add("character", player);
			solServerCommand.Args.Add("Message", note);
			solServerCommand.Send();
		}

		// Token: 0x060025E0 RID: 9696 RVA: 0x00132BD4 File Offset: 0x00130DD4
		public void LeaveGuild()
		{
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not currently in a guild.");
				return;
			}
			CommandClass.guild.NewCommand(CommandType.leave).Send();
		}

		// Token: 0x060025E1 RID: 9697 RVA: 0x00132C10 File Offset: 0x00130E10
		public void TransferGuild(string playerName)
		{
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not in a guild.");
				return;
			}
			if (this.m_ownGuildRank == null || this.m_ownGuildRank.Permissions != (GuildPermissions)(-1))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to transfer the guild.");
				return;
			}
			SolServerCommand solServerCommand = CommandClass.guild.NewCommand(CommandType.transfer);
			solServerCommand.Args.Add("character", playerName);
			solServerCommand.Send();
		}

		// Token: 0x060025E2 RID: 9698 RVA: 0x00132C88 File Offset: 0x00130E88
		public void DisbandGuild()
		{
			if (this.m_guild == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You are not currently in a guild.");
				return;
			}
			if (this.m_ownGuildRank == null || (this.m_ownGuildRank.Permissions & GuildPermissions.Disband) == GuildPermissions.None)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You do not have permission to disband the guild.");
				return;
			}
			CommandClass.guild.NewCommand(CommandType.disband).Send();
		}

		// Token: 0x060025E3 RID: 9699 RVA: 0x00132CF0 File Offset: 0x00130EF0
		public void CheckLooking(bool force = false)
		{
			DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
			if (force || serverCorrectedDateTimeUtc - this.m_lastLookingListCheck > this.m_lfglfmCheckInterval)
			{
				SolServerCommand solServerCommand = CommandClass.lookingfor.NewCommand(CommandType.listall);
				solServerCommand.Args.Add("lastchecked", this.m_lastLookingListCheck.ToUnixTime());
				solServerCommand.Send();
				this.m_lastLookingListCheck = serverCorrectedDateTimeUtc;
			}
		}

		// Token: 0x060025E4 RID: 9700 RVA: 0x00132D58 File Offset: 0x00130F58
		public void StartOrUpdateLooking(LookingFor entry = null)
		{
			if (ClientGameManager.GroupManager == null)
			{
				throw new InvalidOperationException("Cannot start or update LFG/LFM, game not initialized.");
			}
			bool isGrouped = ClientGameManager.GroupManager.IsGrouped;
			if (isGrouped && !ClientGameManager.GroupManager.IsLeader)
			{
				return;
			}
			LookingFor ownLfgLfmEntry = this.m_ownLfgLfmEntry;
			this.m_ownLfgLfmEntry = (entry ?? this.m_ownLfgLfmEntry);
			if (this.m_ownLfgLfmEntry == null)
			{
				return;
			}
			this.m_ownLfgLfmEntry.Note = ((!string.IsNullOrEmpty(this.m_ownLfgLfmEntry.Note) && SessionData.IsSubscriber) ? string.Format("<noparse>{0}</noparse>", this.m_ownLfgLfmEntry.Note.Replace("<noparse>", "").Replace("</noparse>", "")) : null);
			if (this.m_ownLfgLfmEntry != ownLfgLfmEntry)
			{
				Action lookingUpdated = this.LookingUpdated;
				if (lookingUpdated != null)
				{
					lookingUpdated();
				}
			}
			SolServerCommand solServerCommand = CommandClass.lookingfor.NewCommand(isGrouped ? CommandType.startlfm : CommandType.startlfg);
			solServerCommand.Args.Add("minlevel", this.m_ownLfgLfmEntry.MinLevel);
			solServerCommand.Args.Add("maxlevel", this.m_ownLfgLfmEntry.MaxLevel);
			solServerCommand.Args.Add("tags", this.m_ownLfgLfmEntry.Tags);
			solServerCommand.Args.Add("note", this.m_ownLfgLfmEntry.Note);
			solServerCommand.Send();
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x00132EC0 File Offset: 0x001310C0
		public void StopLooking(bool updateServer = true)
		{
			if (ClientGameManager.GroupManager == null)
			{
				throw new InvalidOperationException("Cannot stop LFG/LFM, game not initialized.");
			}
			if (this.m_ownLfgLfmEntry != null)
			{
				if (updateServer)
				{
					CommandClass.lookingfor.NewCommand(ClientGameManager.GroupManager.IsGrouped ? CommandType.stoplfm : CommandType.stoplfg).Send();
				}
				this.m_ownLfgLfmEntry = null;
				Action lookingUpdated = this.LookingUpdated;
				if (lookingUpdated == null)
				{
					return;
				}
				lookingUpdated();
			}
		}

		// Token: 0x060025E6 RID: 9702 RVA: 0x0005ACD2 File Offset: 0x00058ED2
		public bool IsLookingForGroup()
		{
			if (ClientGameManager.GroupManager == null)
			{
				throw new InvalidOperationException("Cannot determine LFG/LFM, game not initialized.");
			}
			return this.m_ownLfgLfmEntry != null && this.m_ownLfgLfmEntry.Type == LookingType.Lfg && !ClientGameManager.GroupManager.IsGrouped;
		}

		// Token: 0x060025E7 RID: 9703 RVA: 0x0005AD11 File Offset: 0x00058F11
		public bool IsGroupLookingForMember()
		{
			if (ClientGameManager.GroupManager == null)
			{
				throw new InvalidOperationException("Cannot determine LFG/LFM, game not initialized.");
			}
			return this.m_ownLfgLfmEntry != null && this.m_ownLfgLfmEntry.Type == LookingType.Lfm && ClientGameManager.GroupManager.IsGrouped;
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x00132F28 File Offset: 0x00131128
		public bool IsLeaderOfGroupLookingForMember()
		{
			if (ClientGameManager.GroupManager == null)
			{
				throw new InvalidOperationException("Cannot determine LFG/LFM, game not initialized.");
			}
			return this.m_ownLfgLfmEntry != null && this.m_ownLfgLfmEntry.Type == LookingType.Lfm && ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader;
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x0005AD4D File Offset: 0x00058F4D
		public bool TryGetInboxItemById(string id, out Mail mail)
		{
			return this.m_postInbox.TryGetValue(id, out mail);
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x0005AD5C File Offset: 0x00058F5C
		public bool TryGetOutboxItemById(string id, out Mail mail)
		{
			return this.m_postOutbox.TryGetValue(id, out mail);
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x00132F7C File Offset: 0x0013117C
		public void DeletePost(string mailId)
		{
			SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.decline);
			solServerCommand.Args.Add("mailid", mailId);
			solServerCommand.Args.Add("type", MailType.Post);
			solServerCommand.Send();
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x00132FC4 File Offset: 0x001311C4
		public void MarkAsRead(string mailId, bool notify = true)
		{
			if (this.m_postInbox.ContainsKey(mailId))
			{
				this.m_postInbox[mailId].Read = true;
				if (notify)
				{
					Action unreadMailUpdated = this.UnreadMailUpdated;
					if (unreadMailUpdated != null)
					{
						unreadMailUpdated();
					}
				}
				SolServerCommand solServerCommand = CommandClass.mail.NewCommand(CommandType.read);
				solServerCommand.Args.Add("mailid", mailId);
				solServerCommand.Args.Add("type", MailType.Post);
				solServerCommand.Send();
			}
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x0005AD6B File Offset: 0x00058F6B
		public bool IsUnread(string mailId)
		{
			return this.m_postInbox.ContainsKey(mailId) && !this.m_postInbox[mailId].Read;
		}

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x060025EE RID: 9710 RVA: 0x00133040 File Offset: 0x00131240
		public bool HasUnreadMail
		{
			get
			{
				using (Dictionary<string, Mail>.ValueCollection.Enumerator enumerator = this.m_postInbox.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Read)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x001330A0 File Offset: 0x001312A0
		public int GetUnreadMailCount()
		{
			int num = 0;
			using (Dictionary<string, Mail>.ValueCollection.Enumerator enumerator = this.m_postInbox.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Read)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x060025F0 RID: 9712 RVA: 0x0005AD91 File Offset: 0x00058F91
		// (set) Token: 0x060025F1 RID: 9713 RVA: 0x0005AD99 File Offset: 0x00058F99
		public bool UnreadMailNotificationDismissed { get; set; }

		// Token: 0x060025F2 RID: 9714 RVA: 0x0005ADA2 File Offset: 0x00058FA2
		public void DeleteGroupInvite()
		{
			if (this.m_pendingIncomingGroupInvite != null)
			{
				this.m_pendingIncomingGroupInvite = null;
				this.FindNextExpiration();
			}
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x0005ADB9 File Offset: 0x00058FB9
		public void DeleteRaidInvite()
		{
			if (this.m_pendingIncomingRaidInvite != null)
			{
				this.m_pendingIncomingRaidInvite = null;
				this.FindNextExpiration();
			}
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x0005ADD0 File Offset: 0x00058FD0
		public void DeleteSentGroupInvite()
		{
			if (this.m_pendingOutgoingGroupInvite != null)
			{
				this.m_pendingOutgoingGroupInvite = null;
				this.FindNextExpiration();
			}
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x00133100 File Offset: 0x00131300
		public void DeleteMail(MailType type, string id)
		{
			int num = 0;
			switch (type)
			{
			case MailType.Post:
				num = this.m_postInbox.RemoveAll((Mail x) => x._id == id);
				if (num > 0)
				{
					Action postInboxUpdated = this.PostInboxUpdated;
					if (postInboxUpdated != null)
					{
						postInboxUpdated();
					}
				}
				else
				{
					num = this.m_postOutbox.RemoveAll((Mail x) => x._id == id);
					if (num > 0)
					{
						Action postOutboxUpdated = this.PostOutboxUpdated;
						if (postOutboxUpdated != null)
						{
							postOutboxUpdated();
						}
					}
				}
				break;
			case MailType.FriendRequest:
				num = this.m_pendingIncomingFriendRequests.RemoveAll((Mail x) => x._id == id);
				num += this.m_pendingOutgoingFriendRequests.RemoveAll((Mail x) => x._id == id);
				if (num > 0)
				{
					Action friendRequestsUpdated = this.FriendRequestsUpdated;
					if (friendRequestsUpdated != null)
					{
						friendRequestsUpdated();
					}
				}
				break;
			case MailType.GroupInvite:
				if (this.m_pendingIncomingGroupInvite != null && this.m_pendingIncomingGroupInvite._id == id)
				{
					this.m_pendingIncomingGroupInvite = null;
					num = 1;
				}
				else if (this.m_pendingOutgoingGroupInvite != null && this.m_pendingOutgoingGroupInvite._id == id)
				{
					this.m_pendingOutgoingGroupInvite = null;
					num = 1;
				}
				break;
			case MailType.GuildInvite:
				num = this.m_pendingIncomingGuildInvites.RemoveAll((Mail x) => x._id == id);
				if (num == 0)
				{
					num = this.m_pendingOutgoingGuildInvites.RemoveAll((Mail x) => x._id == id);
				}
				if (num > 0)
				{
					Action guildInvitesUpdated = this.GuildInvitesUpdated;
					if (guildInvitesUpdated != null)
					{
						guildInvitesUpdated();
					}
				}
				break;
			case MailType.RaidInvite:
				if (this.m_pendingIncomingRaidInvite != null && this.m_pendingIncomingRaidInvite._id == id)
				{
					this.m_pendingIncomingRaidInvite = null;
					num = 1;
				}
				else if (this.m_pendingOutgoingRaidInvite != null && this.m_pendingOutgoingRaidInvite._id == id)
				{
					this.m_pendingOutgoingRaidInvite = null;
					num = 1;
				}
				break;
			}
			if (num > 0)
			{
				this.FindNextExpiration();
			}
		}

		// Token: 0x060025F6 RID: 9718 RVA: 0x00133304 File Offset: 0x00131504
		public void DeleteRelation(RelationType type, string id)
		{
			bool flag = false;
			bool flag2 = false;
			if (type != RelationType.Friend)
			{
				if (type == RelationType.Blocked)
				{
					flag2 = (this.m_blockList.RemoveAll((Relation x) => x._id == id) > 0);
				}
			}
			else
			{
				flag = (this.m_friends.RemoveAll((Relation x) => x._id == id) > 0);
			}
			if (flag)
			{
				Action friendsListUpdated = this.FriendsListUpdated;
				if (friendsListUpdated != null)
				{
					friendsListUpdated();
				}
			}
			if (flag2)
			{
				Action blockListUpdated = this.BlockListUpdated;
				if (blockListUpdated == null)
				{
					return;
				}
				blockListUpdated();
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x060025F7 RID: 9719 RVA: 0x0005ADE7 File Offset: 0x00058FE7
		// (set) Token: 0x060025F8 RID: 9720 RVA: 0x0005ADEE File Offset: 0x00058FEE
		public static Mail PendingOutgoingMail { get; set; }

		// Token: 0x060025F9 RID: 9721 RVA: 0x0013338C File Offset: 0x0013158C
		public void EnqueueMail(params Mail[] mail)
		{
			foreach (Mail item in mail)
			{
				this.m_mailQueue.Enqueue(item);
			}
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x001333BC File Offset: 0x001315BC
		private void ProcessNewMail()
		{
			string b = LocalPlayer.GameEntity.CharacterData.Name;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			Mail mail;
			while (this.m_mailQueue.TryDequeue(out mail))
			{
				if (mail != null && (mail.Expires == null || !(mail.Expires <= GameTimeReplicator.GetServerCorrectedDateTimeUtc())))
				{
					bool flag12 = false;
					switch (mail.Type)
					{
					case MailType.Post:
						if (mail.Sender != b)
						{
							bool flag13 = this.m_postInbox.ContainsKey(mail._id);
							this.m_postInbox.AddOrReplace(mail._id, mail);
							if (!flag13)
							{
								flag5 = true;
							}
							if (!flag13 || !this.m_postInbox[mail._id].Read)
							{
								flag11 = true;
								this.UnreadMailNotificationDismissed = false;
							}
						}
						else
						{
							bool flag14 = this.m_postOutbox.ContainsKey(mail._id);
							this.m_postOutbox.AddOrReplace(mail._id, mail);
							if (!flag14)
							{
								flag10 = true;
							}
						}
						break;
					case MailType.FriendRequest:
						if (mail.Sender != b)
						{
							string key = mail.Sender.ToLower();
							if (!this.m_pendingIncomingFriendRequests.ContainsKey(key))
							{
								this.m_pendingIncomingFriendRequests.Add(key, mail);
								flag3 = true;
								flag12 = true;
								Action<Mail> newFriendRequestReceived = this.NewFriendRequestReceived;
								if (newFriendRequestReceived != null)
								{
									newFriendRequestReceived(mail);
								}
							}
						}
						else
						{
							string key2 = mail.Recipient.ToLower();
							if (!this.m_pendingOutgoingFriendRequests.ContainsKey(key2))
							{
								this.m_pendingOutgoingFriendRequests.Add(key2, mail);
								flag8 = true;
								flag12 = true;
							}
						}
						break;
					case MailType.GroupInvite:
						if (mail.Sender != b)
						{
							this.m_pendingIncomingGroupInvite = mail;
							flag = true;
						}
						else
						{
							this.m_pendingOutgoingGroupInvite = mail;
							flag6 = true;
						}
						flag12 = true;
						break;
					case MailType.GuildInvite:
						if (this.m_guild == null && mail.Recipient == b)
						{
							string key3 = mail.Sender.ToLower();
							if (!this.m_pendingIncomingGuildInvites.ContainsKey(key3))
							{
								this.m_pendingIncomingGuildInvites.Add(key3, mail);
								flag4 = true;
								flag12 = true;
								Action<Mail> newGuildInviteReceived = this.NewGuildInviteReceived;
								if (newGuildInviteReceived != null)
								{
									newGuildInviteReceived(mail);
								}
							}
						}
						else if (this.m_guild != null && mail.Sender == this.m_guild._id)
						{
							string key4 = mail.Recipient.ToLower();
							if (!this.m_pendingOutgoingGuildInvites.ContainsKey(key4))
							{
								this.m_pendingOutgoingGuildInvites.Add(key4, mail);
								flag9 = true;
								flag12 = true;
							}
						}
						break;
					case MailType.RaidInvite:
						if (mail.Sender != b)
						{
							this.m_pendingIncomingRaidInvite = mail;
							flag2 = true;
						}
						else
						{
							this.m_pendingOutgoingRaidInvite = mail;
							flag7 = true;
						}
						flag12 = true;
						break;
					}
					if (flag12 && mail.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > mail.Expires))
					{
						this.m_nextMailExpiration = mail.Expires;
					}
				}
			}
			if (flag)
			{
				Action newGroupInviteReceived = this.NewGroupInviteReceived;
				if (newGroupInviteReceived != null)
				{
					newGroupInviteReceived();
				}
			}
			if (flag2)
			{
				Action newRaidInviteReceived = this.NewRaidInviteReceived;
				if (newRaidInviteReceived != null)
				{
					newRaidInviteReceived();
				}
			}
			if (flag3)
			{
				Action friendRequestsUpdated = this.FriendRequestsUpdated;
				if (friendRequestsUpdated != null)
				{
					friendRequestsUpdated();
				}
			}
			if (flag4)
			{
				Action guildInvitesUpdated = this.GuildInvitesUpdated;
				if (guildInvitesUpdated != null)
				{
					guildInvitesUpdated();
				}
			}
			if (flag5)
			{
				Action postInboxUpdated = this.PostInboxUpdated;
				if (postInboxUpdated != null)
				{
					postInboxUpdated();
				}
			}
			if (flag6)
			{
				Action newGroupInviteSent = this.NewGroupInviteSent;
				if (newGroupInviteSent != null)
				{
					newGroupInviteSent();
				}
			}
			if (flag7)
			{
				Action newRaidInviteSent = this.NewRaidInviteSent;
				if (newRaidInviteSent != null)
				{
					newRaidInviteSent();
				}
			}
			if (flag8)
			{
				Action newFriendRequestSent = this.NewFriendRequestSent;
				if (newFriendRequestSent != null)
				{
					newFriendRequestSent();
				}
			}
			if (flag9)
			{
				Action newGuildInviteSent = this.NewGuildInviteSent;
				if (newGuildInviteSent != null)
				{
					newGuildInviteSent();
				}
			}
			if (flag10)
			{
				Action postOutboxUpdated = this.PostOutboxUpdated;
				if (postOutboxUpdated != null)
				{
					postOutboxUpdated();
				}
			}
			if (flag11)
			{
				Action unreadMailUpdated = this.UnreadMailUpdated;
				if (unreadMailUpdated == null)
				{
					return;
				}
				unreadMailUpdated();
			}
		}

		// Token: 0x060025FB RID: 9723 RVA: 0x00133824 File Offset: 0x00131A24
		private void ExpireMail(DateTime now)
		{
			string playerName = LocalPlayer.GameEntity.CharacterData.Name;
			int num = 0;
			Predicate<Mail> predicate = delegate(Mail mail)
			{
				if (mail != null && mail.Expires != null && mail.Expires <= now)
				{
					if (mail.Sender == playerName)
					{
						Debug.Log(mail.Type.ToStringWithSpaces() + " to " + mail.Recipient + " has expired.");
					}
					else
					{
						Debug.Log(mail.Type.ToStringWithSpaces() + " from " + mail.Sender + " has expired.");
					}
					switch (mail.Type)
					{
					case MailType.Post:
						if (mail.Sender == playerName)
						{
							Action<Mail> expiredSentPost = this.ExpiredSentPost;
							if (expiredSentPost != null)
							{
								expiredSentPost(mail);
							}
							Action postOutboxUpdated = this.PostOutboxUpdated;
							if (postOutboxUpdated != null)
							{
								postOutboxUpdated();
							}
						}
						else
						{
							Action<Mail> expiredPost = this.ExpiredPost;
							if (expiredPost != null)
							{
								expiredPost(mail);
							}
							Action postInboxUpdated = this.PostInboxUpdated;
							if (postInboxUpdated != null)
							{
								postInboxUpdated();
							}
						}
						break;
					case MailType.FriendRequest:
						if (mail.Sender == playerName)
						{
							Action<Mail> expiredSentFriendRequest = this.ExpiredSentFriendRequest;
							if (expiredSentFriendRequest != null)
							{
								expiredSentFriendRequest(mail);
							}
						}
						else
						{
							Action<Mail> expiredFriendRequest = this.ExpiredFriendRequest;
							if (expiredFriendRequest != null)
							{
								expiredFriendRequest(mail);
							}
							Action friendRequestsUpdated = this.FriendRequestsUpdated;
							if (friendRequestsUpdated != null)
							{
								friendRequestsUpdated();
							}
						}
						break;
					case MailType.GroupInvite:
						if (mail.Sender == playerName)
						{
							Action<Mail> expiredSentGroupInvite = this.ExpiredSentGroupInvite;
							if (expiredSentGroupInvite != null)
							{
								expiredSentGroupInvite(mail);
							}
						}
						else
						{
							Action<Mail> expiredGroupInvite = this.ExpiredGroupInvite;
							if (expiredGroupInvite != null)
							{
								expiredGroupInvite(mail);
							}
						}
						break;
					case MailType.GuildInvite:
						if (mail.Sender == playerName)
						{
							Action<Mail> expiredSentGuildInvite = this.ExpiredSentGuildInvite;
							if (expiredSentGuildInvite != null)
							{
								expiredSentGuildInvite(mail);
							}
						}
						else
						{
							Action<Mail> expiredGuildInvite = this.ExpiredGuildInvite;
							if (expiredGuildInvite != null)
							{
								expiredGuildInvite(mail);
							}
						}
						break;
					case MailType.RaidInvite:
						if (mail.Sender == playerName)
						{
							Action<Mail> expiredSentRaidInvite = this.ExpiredSentRaidInvite;
							if (expiredSentRaidInvite != null)
							{
								expiredSentRaidInvite(mail);
							}
						}
						else
						{
							Action<Mail> expiredRaidInvite = this.ExpiredRaidInvite;
							if (expiredRaidInvite != null)
							{
								expiredRaidInvite(mail);
							}
						}
						break;
					}
					return true;
				}
				return false;
			};
			num += this.m_pendingIncomingFriendRequests.RemoveAll(predicate);
			num += this.m_pendingOutgoingFriendRequests.RemoveAll(predicate);
			num += this.m_pendingIncomingGuildInvites.RemoveAll(predicate);
			num += this.m_pendingOutgoingGuildInvites.RemoveAll(predicate);
			num += this.m_postInbox.RemoveAll(predicate);
			num += this.m_postOutbox.RemoveAll(predicate);
			if (predicate(this.m_pendingIncomingGroupInvite))
			{
				this.m_pendingIncomingGroupInvite = null;
				num++;
			}
			if (predicate(this.m_pendingOutgoingGroupInvite))
			{
				this.m_pendingOutgoingGroupInvite = null;
				num++;
			}
			if (predicate(this.m_pendingIncomingRaidInvite))
			{
				this.m_pendingIncomingRaidInvite = null;
				num++;
			}
			if (predicate(this.m_pendingOutgoingRaidInvite))
			{
				this.m_pendingOutgoingRaidInvite = null;
				num++;
			}
			if (num > 0)
			{
				this.FindNextExpiration();
			}
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x00133938 File Offset: 0x00131B38
		private void FindNextExpiration()
		{
			this.m_nextMailExpiration = null;
			Mail pendingIncomingGroupInvite = this.m_pendingIncomingGroupInvite;
			if (pendingIncomingGroupInvite != null && pendingIncomingGroupInvite.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > this.m_pendingIncomingGroupInvite.Expires))
			{
				this.m_nextMailExpiration = this.m_pendingIncomingGroupInvite.Expires;
			}
			Mail pendingOutgoingGroupInvite = this.m_pendingOutgoingGroupInvite;
			if (pendingOutgoingGroupInvite != null && pendingOutgoingGroupInvite.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > this.m_pendingOutgoingGroupInvite.Expires))
			{
				this.m_nextMailExpiration = this.m_pendingOutgoingGroupInvite.Expires;
			}
			Mail pendingIncomingRaidInvite = this.m_pendingIncomingRaidInvite;
			if (pendingIncomingRaidInvite != null && pendingIncomingRaidInvite.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > this.m_pendingIncomingRaidInvite.Expires))
			{
				this.m_nextMailExpiration = this.m_pendingIncomingRaidInvite.Expires;
			}
			Mail pendingOutgoingRaidInvite = this.m_pendingOutgoingRaidInvite;
			if (pendingOutgoingRaidInvite != null && pendingOutgoingRaidInvite.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > this.m_pendingOutgoingRaidInvite.Expires))
			{
				this.m_nextMailExpiration = this.m_pendingOutgoingRaidInvite.Expires;
			}
			foreach (Mail mail in this.m_pendingIncomingFriendRequests.Values)
			{
				if (mail.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > mail.Expires))
				{
					this.m_nextMailExpiration = mail.Expires;
				}
			}
			foreach (Mail mail2 in this.m_pendingIncomingGuildInvites.Values)
			{
				if (mail2.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > mail2.Expires))
				{
					this.m_nextMailExpiration = mail2.Expires;
				}
			}
			foreach (Mail mail3 in this.m_postInbox.Values)
			{
				if (mail3.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > mail3.Expires))
				{
					this.m_nextMailExpiration = mail3.Expires;
				}
			}
			foreach (Mail mail4 in this.m_postOutbox.Values)
			{
				if (mail4.Expires != null && (this.m_nextMailExpiration == null || this.m_nextMailExpiration > mail4.Expires))
				{
					this.m_nextMailExpiration = mail4.Expires;
				}
			}
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x00133DBC File Offset: 0x00131FBC
		public void EnqueueRelations(Relation[] relations)
		{
			foreach (Relation item in relations)
			{
				this.m_relQueue.Enqueue(item);
			}
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x00133DEC File Offset: 0x00131FEC
		private void ProcessNewRelations()
		{
			LocalPlayer.GameEntity.CharacterData.Name;
			bool flag = false;
			bool flag2 = false;
			Relation relation;
			while (this.m_relQueue.TryDequeue(out relation))
			{
				string key = relation.OtherName.ToLower();
				RelationType type = relation.Type;
				if (type != RelationType.Friend)
				{
					if (type == RelationType.Blocked)
					{
						if (!this.m_blockList.ContainsKey(key))
						{
							this.m_blockList.Add(key, relation);
							flag2 = true;
						}
					}
				}
				else if (!this.m_friends.ContainsKey(key))
				{
					this.m_friends.Add(key, relation);
					if (this.m_pendingIncomingFriendRequests.Remove(key) || this.m_pendingOutgoingFriendRequests.Remove(key))
					{
						Action friendRequestsUpdated = this.FriendRequestsUpdated;
						if (friendRequestsUpdated != null)
						{
							friendRequestsUpdated();
						}
					}
					flag = true;
				}
			}
			if (flag)
			{
				Action friendsListUpdated = this.FriendsListUpdated;
				if (friendsListUpdated != null)
				{
					friendsListUpdated();
				}
			}
			if (flag2)
			{
				Action blockListUpdated = this.BlockListUpdated;
				if (blockListUpdated == null)
				{
					return;
				}
				blockListUpdated();
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x00133EDC File Offset: 0x001320DC
		public void UpdateGuild(Guild guild)
		{
			if (this.m_guild != null && guild != null && this.m_guild._id != guild._id)
			{
				Debug.LogWarning("Guild update data ID mismatch! This should not happen!");
			}
			if ((this.m_guild != null && guild != null && this.m_guild.Motd != guild.Motd && !string.IsNullOrWhiteSpace(guild.Motd)) || (this.m_guild == null && guild != null && !string.IsNullOrWhiteSpace(guild.Motd)))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Guild, "Message of the Day: " + guild.Motd);
			}
			this.m_guild = guild;
			if (this.m_guild == null)
			{
				this.m_guildRosterQueue.Clear();
				this.m_guildRoster.Clear();
			}
			else if (this.m_ownGuildRank != null)
			{
				foreach (GuildRank guildRank in this.m_guild.Ranks)
				{
					if (guildRank._id == this.m_ownGuildRank._id)
					{
						this.m_ownGuildRank = guildRank;
					}
				}
			}
			this.m_guildUpdated = true;
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x00134018 File Offset: 0x00132218
		public void EnqueueGuildRosterUpdates(GuildMember[] entries)
		{
			if (entries != null)
			{
				foreach (GuildMember item in entries)
				{
					this.m_guildRosterQueue.Enqueue(item);
				}
			}
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x00134048 File Offset: 0x00132248
		public void EnqueueGuildRosterRemovals(string[] entries)
		{
			if (entries != null)
			{
				foreach (string item in entries)
				{
					this.m_guildRosterRemovalQueue.Enqueue(item);
				}
			}
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x00134078 File Offset: 0x00132278
		private void ProcessGuildUpdates()
		{
			string b = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData) ? LocalPlayer.GameEntity.CharacterData.Name.Value : string.Empty;
			bool flag = false;
			bool flag2 = this.m_guildRoster.Count == 0;
			GuildMember guildMember;
			while (this.m_guildRosterQueue.TryDequeue(out guildMember))
			{
				string key = guildMember.CharacterId.ToLower();
				if (this.m_guildRoster.ContainsKey(key))
				{
					goto IL_104;
				}
				this.m_guildRoster.Add(key, guildMember);
				if (!flag2)
				{
					Action<GuildMember> guildMemberJoined = this.GuildMemberJoined;
					if (guildMemberJoined != null)
					{
						guildMemberJoined(guildMember);
					}
				}
				if (guildMember.CharacterId == b)
				{
					using (List<GuildRank>.Enumerator enumerator = this.m_guild.Ranks.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GuildRank guildRank = enumerator.Current;
							if (guildRank._id == guildMember.RankId)
							{
								this.m_ownGuildRank = guildRank;
							}
						}
						goto IL_1F0;
					}
					goto IL_104;
				}
				IL_1F0:
				flag = true;
				continue;
				IL_104:
				GuildMember guildMember2 = this.m_guildRoster[key];
				this.m_guildRoster[key] = guildMember;
				GuildRank guildRank2 = null;
				GuildRank guildRank3 = null;
				foreach (GuildRank guildRank4 in this.m_guild.Ranks)
				{
					if (guildMember2.RankId == guildRank4._id)
					{
						guildRank2 = guildRank4;
					}
					if (guildMember.RankId == guildRank4._id)
					{
						guildRank3 = guildRank4;
						if (guildMember.CharacterId == b)
						{
							this.m_ownGuildRank = guildRank4;
						}
					}
				}
				if (guildRank2 == null || guildRank3 == null)
				{
					goto IL_1F0;
				}
				if (guildRank2.Sort < guildRank3.Sort)
				{
					Action<GuildMember> guildMemberPromoted = this.GuildMemberPromoted;
					if (guildMemberPromoted != null)
					{
						guildMemberPromoted(guildMember);
					}
				}
				if (guildRank2.Sort <= guildRank3.Sort)
				{
					goto IL_1F0;
				}
				Action<GuildMember> guildMemberDemoted = this.GuildMemberDemoted;
				if (guildMemberDemoted == null)
				{
					goto IL_1F0;
				}
				guildMemberDemoted(guildMember);
				goto IL_1F0;
			}
			string text;
			while (this.m_guildRosterRemovalQueue.TryDequeue(out text))
			{
				string key2 = text.ToLower();
				if (this.m_guildRoster.ContainsKey(key2))
				{
					this.m_guildRoster.Remove(key2);
					Action<string> guildMemberLeft = this.GuildMemberLeft;
					if (guildMemberLeft != null)
					{
						guildMemberLeft(text);
					}
					flag = true;
				}
			}
			if (this.m_guildUpdated)
			{
				this.m_guildUpdated = false;
				SynchronizedVariable<string> guildName = LocalPlayer.GameEntity.CharacterData.GuildName;
				Guild guild = this.m_guild;
				guildName.Value = ((guild != null) ? guild.Name : null);
				Action guildUpdated = this.GuildUpdated;
				if (guildUpdated != null)
				{
					guildUpdated();
				}
				if (this.m_guild == null)
				{
					Action guildRosterUpdated = this.GuildRosterUpdated;
					if (guildRosterUpdated != null)
					{
						guildRosterUpdated();
					}
				}
			}
			if (flag)
			{
				Action guildRosterUpdated2 = this.GuildRosterUpdated;
				if (guildRosterUpdated2 == null)
				{
					return;
				}
				guildRosterUpdated2();
			}
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x0005ADF6 File Offset: 0x00058FF6
		private void OnGuildMemberJoined(GuildMember member)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, member.CharacterId + " has joined the guild!");
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x00134358 File Offset: 0x00132558
		private void OnGuildMemberPromoted(GuildMember member)
		{
			if (LocalPlayer.GameEntity != null && member.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You have been promoted to " + this.m_guild.GetRankById(member.RankId).Name + "!");
			}
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x001343C8 File Offset: 0x001325C8
		private void OnGuildMemberDemoted(GuildMember member)
		{
			if (LocalPlayer.GameEntity != null && member.CharacterId == LocalPlayer.GameEntity.CharacterData.Name)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You have been demoted to " + this.m_guild.GetRankById(member.RankId).Name + "!");
			}
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x00134438 File Offset: 0x00132638
		public void EnqueuePlayerStatusUpdates(params PlayerStatus[] updates)
		{
			if (updates != null)
			{
				foreach (PlayerStatus playerStatus in updates)
				{
					if (playerStatus != null)
					{
						this.m_playerUpdateQueue.Enqueue(playerStatus);
					}
				}
			}
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x0013446C File Offset: 0x0013266C
		private void ProcessNewPlayerStatusUpdates()
		{
			bool flag = false;
			PlayerStatus playerStatus;
			while (this.m_playerUpdateQueue.TryDequeue(out playerStatus))
			{
				PlayerStatus playerStatus2;
				if (this.m_latestRelationStatuses.TryGetValue(playerStatus.Character, out playerStatus2))
				{
					if (playerStatus2.Presence != playerStatus.Presence || playerStatus2.ZoneId != playerStatus.ZoneId || playerStatus2.SubZoneId != playerStatus.SubZoneId || playerStatus2.Role != playerStatus.Role || playerStatus2.Level != playerStatus.Level || playerStatus2.EmberRingIndex != playerStatus.EmberRingIndex)
					{
						if ((playerStatus2.PresenceFlags & (PresenceFlags.Anonymous | PresenceFlags.Invisible)) == PresenceFlags.Invalid && (playerStatus.PresenceFlags & (PresenceFlags.Anonymous | PresenceFlags.Invisible)) == PresenceFlags.Invalid)
						{
							if (playerStatus2.ZoneId <= 0 && playerStatus.ZoneId > 0)
							{
								this.StatusChanged(playerStatus.Character, true);
								Action playerOnline = this.PlayerOnline;
								if (playerOnline != null)
								{
									playerOnline();
								}
							}
							else if (playerStatus2.ZoneId > 0 && playerStatus.ZoneId == -1)
							{
								this.StatusChanged(playerStatus.Character, false);
								Action playerOffline = this.PlayerOffline;
								if (playerOffline != null)
								{
									playerOffline();
								}
							}
						}
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				this.m_latestRelationStatuses.AddOrReplace(playerStatus.Character, playerStatus);
			}
			if (flag)
			{
				Action playerStatusesChanged = this.PlayerStatusesChanged;
				if (playerStatusesChanged == null)
				{
					return;
				}
				playerStatusesChanged();
			}
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x001345A4 File Offset: 0x001327A4
		private void StatusChanged(string characterName, bool comingOnline)
		{
			bool flag = this.IsFriend(characterName);
			bool flag2 = this.IsMemberOfMyGuild(characterName);
			GroupMember groupMember;
			bool flag3 = ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.TryGetGroupMember(characterName, out groupMember);
			string text = ZString.Format<string, string>("<link=\"{0}\">{1}</link>", "playerName", characterName);
			if (flag)
			{
				text = ZString.Format<string, string>("<color={0}><i>{1}</i></color>", WhoResultExtensions.kSameColor.ToHex(), text);
			}
			if (flag3)
			{
				text = ZString.Format<string, string, string, string>("<color={0}><link=\"{1}:Group Member\">{2}</link></color> {3}", NameplateControllerUI.kGroupColor.ToHex(), "text", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>", text);
			}
			string content = comingOnline ? ZString.Format<string>("{0} is now online.", text) : ZString.Format<string>("{0} has gone offline.", text);
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, content);
			if (flag && Options.AudioOptions.AudioOnFriendStatusUpdate)
			{
				ClientGameManager.UIManager.PlayClip(this.GetOnlineOfflineClip(comingOnline), new float?(1f), new float?(GlobalSettings.Values.Audio.FriendVolume));
				return;
			}
			if (flag2 && Options.AudioOptions.AudioOnGuildStatusUpdate)
			{
				ClientGameManager.UIManager.PlayClip(this.GetOnlineOfflineClip(comingOnline), new float?(0.8f), new float?(GlobalSettings.Values.Audio.FriendVolume));
			}
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x0005AE18 File Offset: 0x00059018
		private AudioClip GetOnlineOfflineClip(bool comingOnline)
		{
			if (!comingOnline)
			{
				return GlobalSettings.Values.Audio.FriendOfflineClip;
			}
			return GlobalSettings.Values.Audio.FriendOnlineClip;
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x001346E0 File Offset: 0x001328E0
		private void FriendStatusChanged(string message, AudioClip clip)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, message);
			if (Options.AudioOptions.AudioOnFriendStatusUpdate.Value)
			{
				ClientGameManager.UIManager.PlayClip(clip, new float?(1f), new float?(GlobalSettings.Values.Audio.FriendVolume));
			}
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x00134734 File Offset: 0x00132934
		public void EnqueueLookingEntries(LookingFor[] entries)
		{
			if (entries != null)
			{
				foreach (LookingFor item in entries)
				{
					this.m_lookingQueue.Enqueue(item);
				}
				this.m_lookingQueueNeedsProcessing = true;
			}
			this.m_lastLookingListCheck = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x00134778 File Offset: 0x00132978
		private void ProcessNewLookingEntries()
		{
			this.m_lookingQueueNeedsProcessing = false;
			LookingFor ownLfgLfmEntry = this.m_ownLfgLfmEntry;
			this.m_ownLfgLfmEntry = null;
			this.m_lookingList.Clear();
			LookingFor lookingFor;
			while (this.m_lookingQueue.TryDequeue(out lookingFor))
			{
				this.m_lookingList.AddOrReplace(lookingFor.Key, lookingFor);
				if (lookingFor.Key == LocalPlayer.GameEntity.CharacterData.CharacterId || lookingFor.Key == ClientGameManager.GroupManager.GroupId)
				{
					this.m_ownLfgLfmEntry = lookingFor;
				}
			}
			if (this.m_ownLfgLfmEntry != ownLfgLfmEntry)
			{
				Action lookingUpdated = this.LookingUpdated;
				if (lookingUpdated != null)
				{
					lookingUpdated();
				}
			}
			Action lookingListUpdated = this.LookingListUpdated;
			if (lookingListUpdated == null)
			{
				return;
			}
			lookingListUpdated();
		}

		// Token: 0x040027D4 RID: 10196
		public const MessageType kMessageType = MessageType.Social;

		// Token: 0x040027D5 RID: 10197
		public const MessageType kGuildMessageType = MessageType.Guild;

		// Token: 0x040027D6 RID: 10198
		public const string kMessageOfTheDayPrefix = "Message of the Day:";

		// Token: 0x040027D7 RID: 10199
		public const string kFriendFreeTrialMessage = "You must purchase the game to request friends.";

		// Token: 0x040027D8 RID: 10200
		public const string kGuildFreeTrialMessage = "You must purchase the game to participate in a guild.";

		// Token: 0x040027D9 RID: 10201
		public const string kSendTell = "Send Tell";

		// Token: 0x040027DA RID: 10202
		public const string kSendFriendRequest = "Send Friend Request";

		// Token: 0x040027DB RID: 10203
		public const string kAcceptFriendRequest = "Accept Friend Request";

		// Token: 0x040027DC RID: 10204
		public const string kInviteToGroup = "Invite to Group";

		// Token: 0x040027DD RID: 10205
		public const string kRequestTrade = "Request Trade";

		// Token: 0x040027DE RID: 10206
		public const string kRemoveFriend = "Remove Friend";

		// Token: 0x040027DF RID: 10207
		public const string kBlock = "Block";

		// Token: 0x040027E0 RID: 10208
		public const string kUnblock = "Unblock";

		// Token: 0x040027E1 RID: 10209
		public const string kRemoveAndBlock = "Remove Friend & Block";

		// Token: 0x040027E2 RID: 10210
		public const string kPromoteToLeader = "Promote to Leader";

		// Token: 0x040027E3 RID: 10211
		public const string kKickFromGroup = "Kick from Group";

		// Token: 0x040027E4 RID: 10212
		public const string kInviteToGuild = "Invite to Guild";

		// Token: 0x040027E5 RID: 10213
		public const string kPromote = "Promote";

		// Token: 0x040027E6 RID: 10214
		public const string kDemote = "Demote";

		// Token: 0x040027E7 RID: 10215
		public const string kEditPublicNote = "Edit Public Note";

		// Token: 0x040027E8 RID: 10216
		public const string kEditOfficerNote = "Edit Officer Note";

		// Token: 0x040027E9 RID: 10217
		public const string kKickFromGuild = "Kick from Guild";

		// Token: 0x040027EA RID: 10218
		public const string kTransferGuild = "Transfer Guild";

		// Token: 0x040027EB RID: 10219
		private const string kReadMailPrefKeyTemplate = "SocialServer_ReadMail_{0}";

		// Token: 0x040027EC RID: 10220
		private const string kArg_LastChecked = "lastchecked";

		// Token: 0x040027ED RID: 10221
		private const string kArg_MinLevel = "minlevel";

		// Token: 0x040027EE RID: 10222
		private const string kArg_MaxLevel = "maxlevel";

		// Token: 0x040027EF RID: 10223
		private const string kArg_Tags = "tags";

		// Token: 0x040027F0 RID: 10224
		private const string kArg_Note = "note";

		// Token: 0x040027F1 RID: 10225
		private readonly TimeSpan m_lfglfmCheckInterval = TimeSpan.FromSeconds(30.0);

		// Token: 0x040027F2 RID: 10226
		private Queue<Mail> m_mailQueue = new Queue<Mail>();

		// Token: 0x040027F3 RID: 10227
		private Queue<Relation> m_relQueue = new Queue<Relation>();

		// Token: 0x040027F4 RID: 10228
		private Queue<GuildMember> m_guildRosterQueue = new Queue<GuildMember>();

		// Token: 0x040027F5 RID: 10229
		private Queue<string> m_guildRosterRemovalQueue = new Queue<string>();

		// Token: 0x040027F6 RID: 10230
		private Queue<PlayerStatus> m_playerUpdateQueue = new Queue<PlayerStatus>();

		// Token: 0x040027F7 RID: 10231
		private Queue<LookingFor> m_lookingQueue = new Queue<LookingFor>();

		// Token: 0x040027F8 RID: 10232
		private Mail m_pendingIncomingGroupInvite;

		// Token: 0x040027F9 RID: 10233
		private Mail m_pendingOutgoingGroupInvite;

		// Token: 0x040027FA RID: 10234
		private Mail m_pendingIncomingRaidInvite;

		// Token: 0x040027FB RID: 10235
		private Mail m_pendingOutgoingRaidInvite;

		// Token: 0x040027FC RID: 10236
		private Dictionary<string, Mail> m_pendingIncomingFriendRequests = new Dictionary<string, Mail>();

		// Token: 0x040027FD RID: 10237
		private Dictionary<string, Mail> m_pendingOutgoingFriendRequests = new Dictionary<string, Mail>();

		// Token: 0x040027FE RID: 10238
		private Dictionary<string, Mail> m_pendingIncomingGuildInvites = new Dictionary<string, Mail>();

		// Token: 0x040027FF RID: 10239
		private Dictionary<string, Mail> m_pendingOutgoingGuildInvites = new Dictionary<string, Mail>();

		// Token: 0x04002800 RID: 10240
		private Dictionary<string, Mail> m_postInbox = new Dictionary<string, Mail>();

		// Token: 0x04002801 RID: 10241
		private Dictionary<string, Mail> m_postOutbox = new Dictionary<string, Mail>();

		// Token: 0x04002802 RID: 10242
		private UniqueId m_raidId = UniqueId.Empty;

		// Token: 0x04002803 RID: 10243
		private UniqueId m_raidLeadGroupId = UniqueId.Empty;

		// Token: 0x04002804 RID: 10244
		private RaidGroup[] m_raidGroups;

		// Token: 0x04002805 RID: 10245
		private Dictionary<string, Relation> m_friends = new Dictionary<string, Relation>();

		// Token: 0x04002806 RID: 10246
		private Dictionary<string, Relation> m_blockList = new Dictionary<string, Relation>();

		// Token: 0x04002807 RID: 10247
		private Dictionary<string, PlayerStatus> m_latestRelationStatuses = new Dictionary<string, PlayerStatus>();

		// Token: 0x04002808 RID: 10248
		private Guild m_guild;

		// Token: 0x04002809 RID: 10249
		private GuildRank m_ownGuildRank;

		// Token: 0x0400280A RID: 10250
		private bool m_guildUpdated;

		// Token: 0x0400280B RID: 10251
		private Dictionary<string, GuildMember> m_guildRoster = new Dictionary<string, GuildMember>();

		// Token: 0x0400280C RID: 10252
		private LookingFor m_ownLfgLfmEntry;

		// Token: 0x0400280D RID: 10253
		private Dictionary<string, LookingFor> m_lookingList = new Dictionary<string, LookingFor>();

		// Token: 0x0400280E RID: 10254
		private DateTime m_lastLookingListCheck = DateTimeExtensions.UnixEpoch;

		// Token: 0x0400280F RID: 10255
		private Dictionary<UniqueId, CharacterIdentification> m_nameCheckCache = new Dictionary<UniqueId, CharacterIdentification>();

		// Token: 0x04002810 RID: 10256
		private bool m_lookingQueueNeedsProcessing;

		// Token: 0x04002811 RID: 10257
		private DateTime? m_nextMailExpiration;

		// Token: 0x04002838 RID: 10296
		private IEnumerator m_delayedRaidIdSet;
	}
}
