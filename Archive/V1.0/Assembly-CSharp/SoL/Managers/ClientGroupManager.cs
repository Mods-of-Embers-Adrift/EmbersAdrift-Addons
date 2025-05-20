using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Grouping;
using SoL.Game.Loot;
using SoL.Game.Messages;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x020004E9 RID: 1257
	public class ClientGroupManager : MonoBehaviour
	{
		// Token: 0x1400003D RID: 61
		// (add) Token: 0x060022E1 RID: 8929 RVA: 0x00127FE0 File Offset: 0x001261E0
		// (remove) Token: 0x060022E2 RID: 8930 RVA: 0x00128014 File Offset: 0x00126214
		public static event Action AutoLootRollChanged;

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x060022E3 RID: 8931 RVA: 0x000591CF File Offset: 0x000573CF
		// (set) Token: 0x060022E4 RID: 8932 RVA: 0x000591D6 File Offset: 0x000573D6
		public static LootRollChoice AutoLootRoll
		{
			get
			{
				return ClientGroupManager.m_autoLootRoll;
			}
			set
			{
				if (ClientGroupManager.m_autoLootRoll == value)
				{
					return;
				}
				ClientGroupManager.m_autoLootRoll = value;
				Action autoLootRollChanged = ClientGroupManager.AutoLootRollChanged;
				if (autoLootRollChanged == null)
				{
					return;
				}
				autoLootRollChanged();
			}
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x000591F6 File Offset: 0x000573F6
		public static bool IsMyGroup(UniqueId groupId)
		{
			return !groupId.IsEmpty && ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.GroupId == groupId;
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x060022E6 RID: 8934 RVA: 0x00128048 File Offset: 0x00126248
		// (remove) Token: 0x060022E7 RID: 8935 RVA: 0x00128080 File Offset: 0x00126280
		public event Action RefreshGroup;

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x060022E8 RID: 8936 RVA: 0x001280B8 File Offset: 0x001262B8
		// (remove) Token: 0x060022E9 RID: 8937 RVA: 0x001280F0 File Offset: 0x001262F0
		public event Action RefreshLeader;

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x060022EA RID: 8938 RVA: 0x00128128 File Offset: 0x00126328
		// (remove) Token: 0x060022EB RID: 8939 RVA: 0x00128160 File Offset: 0x00126360
		public event Action GroupIdChanged;

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x060022EC RID: 8940 RVA: 0x00128198 File Offset: 0x00126398
		// (remove) Token: 0x060022ED RID: 8941 RVA: 0x001281D0 File Offset: 0x001263D0
		public event Action GroupMemberStatusUpdate;

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x060022EE RID: 8942 RVA: 0x0005922B File Offset: 0x0005742B
		// (set) Token: 0x060022EF RID: 8943 RVA: 0x00128208 File Offset: 0x00126408
		public UniqueId GroupId
		{
			get
			{
				return this.m_groupId;
			}
			set
			{
				if (value == this.m_groupId)
				{
					return;
				}
				this.m_groupId = value;
				if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.NetworkEntity == null || LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler == null)
				{
					if (this.m_delayedGroupIdSet != null)
					{
						base.StopCoroutine(this.m_delayedGroupIdSet);
					}
					this.m_delayedGroupIdSet = this.DelayedGroupIdSet();
					base.StartCoroutine(this.m_delayedGroupIdSet);
				}
				else
				{
					this.SetServerGroupId();
				}
				Action groupIdChanged = this.GroupIdChanged;
				if (groupIdChanged != null)
				{
					groupIdChanged();
				}
				ClientGroupManager.AutoLootRoll = LootRollChoice.Unanswered;
			}
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x060022F0 RID: 8944 RVA: 0x00059233 File Offset: 0x00057433
		// (set) Token: 0x060022F1 RID: 8945 RVA: 0x0005923B File Offset: 0x0005743B
		public GroupMember Leader
		{
			get
			{
				return this.m_leader;
			}
			set
			{
				if (value == this.m_leader)
				{
					return;
				}
				this.m_leader = value;
				Action refreshLeader = this.RefreshLeader;
				if (refreshLeader == null)
				{
					return;
				}
				refreshLeader();
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060022F2 RID: 8946 RVA: 0x0005925E File Offset: 0x0005745E
		public bool GroupIsFull
		{
			get
			{
				return this.m_members.Count >= 6;
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060022F3 RID: 8947 RVA: 0x00059271 File Offset: 0x00057471
		public bool IsGrouped
		{
			get
			{
				return this.m_members.Count > 1;
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x060022F4 RID: 8948 RVA: 0x00059281 File Offset: 0x00057481
		public bool IsLeader
		{
			get
			{
				return this.IsGrouped && this.Leader != null && SessionData.SelectedCharacter != null && this.Leader.Name == SessionData.SelectedCharacter.Name;
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060022F5 RID: 8949 RVA: 0x000592B8 File Offset: 0x000574B8
		public int GroupMemberCount
		{
			get
			{
				return this.m_members.Count;
			}
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x001282B0 File Offset: 0x001264B0
		private void Awake()
		{
			NetworkEntity.Initialized += this.OnEntityInitialized;
			NetworkEntity.Destroyed += this.OnEntityDestroyed;
			ClientGameManager.SocialManager.NewGroupInviteReceived += this.OnInviteReceived;
			ClientGameManager.SocialManager.NewGroupInviteSent += this.OnInviteSent;
			ClientGameManager.SocialManager.ExpiredGroupInvite += this.OnReceivedInviteExpired;
			ClientGameManager.SocialManager.ExpiredSentGroupInvite += this.OnSentInviteExpired;
			ClientGameManager.SocialManager.NewRaidInviteReceived += this.OnRaidInviteReceived;
			ClientGameManager.SocialManager.NewRaidInviteSent += this.OnRaidInviteSent;
			ClientGameManager.SocialManager.ExpiredRaidInvite += this.OnReceivedRaidInviteExpired;
			ClientGameManager.SocialManager.ExpiredSentRaidInvite += this.OnSentRaidInviteExpired;
			ClientGameManager.SocialManager.PlayerStatusesChanged += this.OnPlayerStatusUpdate;
			this.m_invitedCallback = new Action<bool, object>(this.InvitedCallback);
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x001283B8 File Offset: 0x001265B8
		private void OnDestroy()
		{
			NetworkEntity.Initialized -= this.OnEntityInitialized;
			NetworkEntity.Destroyed -= this.OnEntityDestroyed;
			ClientGameManager.SocialManager.NewGroupInviteReceived -= this.OnInviteReceived;
			ClientGameManager.SocialManager.NewGroupInviteSent -= this.OnInviteSent;
			ClientGameManager.SocialManager.ExpiredGroupInvite -= this.OnReceivedInviteExpired;
			ClientGameManager.SocialManager.ExpiredSentGroupInvite -= this.OnSentInviteExpired;
			ClientGameManager.SocialManager.NewRaidInviteReceived -= this.OnRaidInviteReceived;
			ClientGameManager.SocialManager.NewRaidInviteSent -= this.OnRaidInviteSent;
			ClientGameManager.SocialManager.ExpiredRaidInvite -= this.OnReceivedRaidInviteExpired;
			ClientGameManager.SocialManager.ExpiredSentRaidInvite -= this.OnSentRaidInviteExpired;
			ClientGameManager.SocialManager.PlayerStatusesChanged -= this.OnPlayerStatusUpdate;
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x000592C5 File Offset: 0x000574C5
		private IEnumerator DelayedGroupIdSet()
		{
			while (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.NetworkEntity == null || LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler == null)
			{
				yield return null;
			}
			this.SetServerGroupId();
			this.m_delayedGroupIdSet = null;
			yield break;
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x001284B0 File Offset: 0x001266B0
		private void SetServerGroupId()
		{
			Debug.Log("Setting GroupId to " + this.GroupId.Value);
			LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.SetGroupId(this.GroupId);
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x001284F4 File Offset: 0x001266F4
		private void OnEntityInitialized(NetworkEntity obj)
		{
			if (obj && obj.GameEntity && obj.GameEntity.Type == GameEntityType.Player && obj.GameEntity.CharacterData && !string.IsNullOrEmpty(obj.GameEntity.CharacterData.Name.Value))
			{
				string value = obj.GameEntity.CharacterData.Name.Value;
				this.m_entities.AddOrReplace(value, obj);
				GroupMember groupMember;
				if (this.GetMember(value, obj.GameEntity.CharacterData.CharacterId, out groupMember))
				{
					groupMember.Entity = obj.GameEntity;
				}
			}
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x001285A4 File Offset: 0x001267A4
		private void OnEntityDestroyed(NetworkEntity obj)
		{
			if (obj && obj.GameEntity && obj.GameEntity.Type == GameEntityType.Player && obj.GameEntity.CharacterData && !string.IsNullOrEmpty(obj.GameEntity.CharacterData.Name.Value))
			{
				string value = obj.GameEntity.CharacterData.Name.Value;
				this.m_entities.Remove(value);
				GroupMember groupMember;
				if (this.GetMember(value, obj.GameEntity.CharacterData.CharacterId, out groupMember))
				{
					groupMember.Entity = null;
				}
			}
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x00128650 File Offset: 0x00126850
		public void StatusUpdate(UniqueId groupId, string leaderName, GroupMemberZoneStatus[] memberStatus)
		{
			ClientGameManager.SocialManager.DeleteGroupInvite();
			ClientGameManager.SocialManager.DeleteSentGroupInvite();
			this.GroupId = groupId;
			for (int i = 0; i < this.m_members.Count; i++)
			{
				this.m_members[i].MarkedForRemoval = true;
			}
			for (int j = 0; j < memberStatus.Length; j++)
			{
				GroupMember groupMember;
				if (this.GetMember(memberStatus[j].CharacterName, new UniqueId(memberStatus[j].CharacterId), out groupMember))
				{
					groupMember.UpdateStatus(memberStatus[j]);
				}
				else
				{
					groupMember = new GroupMember(memberStatus[j]);
					this.m_members.Add(groupMember.Name, groupMember);
					string content = groupMember.IsSelf ? "You have joined the group." : (TextMeshProExtensions.CreatePlayerLink(groupMember.Name) + " has joined the group.");
					MessageManager.ChatQueue.AddToQueue(MessageType.Party | MessageType.PreFormatted, content);
				}
				if (groupMember.Name == leaderName)
				{
					this.Leader = groupMember;
				}
			}
			for (int k = 0; k < this.m_members.Count; k++)
			{
				if (this.m_members[k].MarkedForRemoval)
				{
					this.m_members.Remove(this.m_members[k].Name);
					k--;
				}
			}
			Action refreshGroup = this.RefreshGroup;
			if (refreshGroup == null)
			{
				return;
			}
			refreshGroup();
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x001287B8 File Offset: 0x001269B8
		private void OnPlayerStatusUpdate()
		{
			bool flag = false;
			for (int i = 0; i < this.m_members.Count; i++)
			{
				PlayerStatus status;
				if (ClientGameManager.SocialManager.TryGetLatestStatus(this.m_members[i].Name, out status))
				{
					this.m_members[i].UpdateStatus(status);
					flag = true;
				}
			}
			if (flag)
			{
				Action groupMemberStatusUpdate = this.GroupMemberStatusUpdate;
				if (groupMemberStatusUpdate == null)
				{
					return;
				}
				groupMemberStatusUpdate();
			}
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x000592D4 File Offset: 0x000574D4
		private bool GetMember(string characterName, UniqueId characterId, out GroupMember member)
		{
			if (!this.m_members.TryGetValue(characterName, out member))
			{
				return false;
			}
			if (member.CharacterId != characterId)
			{
				this.m_members.Remove(characterName);
				member = null;
				return false;
			}
			return true;
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x00128824 File Offset: 0x00126A24
		public void SetLeader(string leaderName)
		{
			GroupMember leader;
			if (this.m_members.TryGetValue(leaderName, out leader))
			{
				this.Leader = leader;
				Action refreshGroup = this.RefreshGroup;
				if (refreshGroup == null)
				{
					return;
				}
				refreshGroup();
			}
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x00059309 File Offset: 0x00057509
		public void PromoteToLeader(GroupMember member)
		{
			if (!this.IsLeader)
			{
				return;
			}
			this.SendCommand(CommandType.promote, member.Name);
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x00059322 File Offset: 0x00057522
		public void KickFromGroup(GroupMember member)
		{
			if (!this.IsLeader)
			{
				return;
			}
			this.SendCommand(CommandType.kick, member.Name);
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x00128858 File Offset: 0x00126A58
		public void InviteNewMember(string playerName)
		{
			if ((this.IsGrouped && !this.IsLeader) || this.GroupIsFull)
			{
				return;
			}
			Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
			dictionary.Add("Message", new Mail
			{
				Type = MailType.GroupInvite,
				Sender = LocalPlayer.GameEntity.CharacterData.Name,
				Recipient = playerName
			});
			SolServerConnectionManager.CurrentConnection.Send(CommandClass.mail.NewCommand(CommandType.send, dictionary));
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x001288D0 File Offset: 0x00126AD0
		private void SendCommand(CommandType cmdType, string playerName)
		{
			SolServerCommand solServerCommand = new SolServerCommand(CommandClass.group, cmdType);
			solServerCommand.Args.Add("Receiver", playerName);
			solServerCommand.Args.Add("Message", playerName);
			solServerCommand.Send();
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x0005933B File Offset: 0x0005753B
		public bool TryGetEntityForName(string entityName, out NetworkEntity entity)
		{
			return this.m_entities.TryGetValue(entityName, out entity);
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x0005934A File Offset: 0x0005754A
		public IEnumerable<GroupMember> GetAllGroupMembers()
		{
			int num;
			for (int i = 0; i < this.m_members.Count; i = num + 1)
			{
				yield return this.m_members[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x0005935A File Offset: 0x0005755A
		public void ResetGroup()
		{
			this.m_members.Clear();
			this.Leader = null;
			this.GroupId = UniqueId.Empty;
			Action refreshGroup = this.RefreshGroup;
			if (refreshGroup == null)
			{
				return;
			}
			refreshGroup();
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x00128910 File Offset: 0x00126B10
		public void InvitedToGroup(string inviter)
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Group Invite",
				Text = "You have been invited to group with " + inviter + ".",
				ConfirmationText = "Accept",
				CancelText = "Decline",
				ShowCloseButton = false,
				Callback = this.m_invitedCallback
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x00128988 File Offset: 0x00126B88
		private void InvitedCallback(bool answer, object obj)
		{
			Mail pendingIncomingGroupInvite = ClientGameManager.SocialManager.PendingIncomingGroupInvite;
			if (pendingIncomingGroupInvite != null)
			{
				Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
				dictionary.Add("mailid", pendingIncomingGroupInvite._id);
				dictionary.Add("type", pendingIncomingGroupInvite.Type);
				SolServerConnectionManager.CurrentConnection.Send(CommandClass.mail.NewCommand(answer ? CommandType.accept : CommandType.decline, dictionary));
			}
			this.CloseGroupInviteDialog();
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x00059389 File Offset: 0x00057589
		private void OnInviteReceived()
		{
			this.InvitedToGroup(ClientGameManager.SocialManager.PendingIncomingGroupInvite.Sender);
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x000593A0 File Offset: 0x000575A0
		private void OnInviteSent()
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You invited " + TextMeshProExtensions.CreatePlayerLink(ClientGameManager.SocialManager.PendingOutgoingGroupInvite.Recipient) + " to group.");
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x000593D5 File Offset: 0x000575D5
		private void OnReceivedInviteExpired(Mail invite)
		{
			ClientGameManager.GroupManager.CloseGroupInviteDialog();
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Your group invite from " + TextMeshProExtensions.CreatePlayerLink(invite.Sender) + " has expired.");
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x0005940B File Offset: 0x0005760B
		private void OnSentInviteExpired(Mail invite)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Your group invite to " + TextMeshProExtensions.CreatePlayerLink(invite.Recipient) + " has expired.");
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x00059437 File Offset: 0x00057637
		public void CloseGroupInviteDialog()
		{
			if (ClientGameManager.SocialManager.IsGroupInvitePending)
			{
				if (ClientGameManager.UIManager.ConfirmationDialog.Visible)
				{
					ClientGameManager.UIManager.ConfirmationDialog.Hide(false);
				}
				ClientGameManager.SocialManager.DeleteGroupInvite();
			}
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x001289F4 File Offset: 0x00126BF4
		public void InvitedToRaid(string inviter)
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Raid Invite",
				Text = "You and your group have been invited to raid with " + inviter + ".",
				ConfirmationText = "Accept",
				CancelText = "Decline",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.RaidInvitedCallback)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x0600230F RID: 8975 RVA: 0x00128A74 File Offset: 0x00126C74
		private void RaidInvitedCallback(bool answer, object obj)
		{
			Mail pendingIncomingRaidInvite = ClientGameManager.SocialManager.PendingIncomingRaidInvite;
			if (pendingIncomingRaidInvite != null)
			{
				Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
				dictionary.Add("mailid", pendingIncomingRaidInvite._id);
				dictionary.Add("type", pendingIncomingRaidInvite.Type);
				SolServerConnectionManager.CurrentConnection.Send(CommandClass.mail.NewCommand(answer ? CommandType.accept : CommandType.decline, dictionary));
			}
			this.CloseRaidInviteDialog();
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x00059470 File Offset: 0x00057670
		private void OnRaidInviteReceived()
		{
			this.InvitedToRaid(ClientGameManager.SocialManager.PendingIncomingRaidInvite.Sender);
		}

		// Token: 0x06002311 RID: 8977 RVA: 0x00059487 File Offset: 0x00057687
		private void OnRaidInviteSent()
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "You invited " + TextMeshProExtensions.CreatePlayerLink(ClientGameManager.SocialManager.PendingOutgoingRaidInvite.Recipient) + "'s group to a raid.");
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x000594BC File Offset: 0x000576BC
		private void OnReceivedRaidInviteExpired(Mail invite)
		{
			ClientGameManager.GroupManager.CloseRaidInviteDialog();
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Your raid invite from " + TextMeshProExtensions.CreatePlayerLink(invite.Sender) + " has expired.");
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000594F2 File Offset: 0x000576F2
		private void OnSentRaidInviteExpired(Mail invite)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Social, "Your raid invite to " + TextMeshProExtensions.CreatePlayerLink(invite.Recipient) + "'s group has expired.");
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x0005951E File Offset: 0x0005771E
		public void CloseRaidInviteDialog()
		{
			if (ClientGameManager.SocialManager.IsRaidInvitePending)
			{
				if (ClientGameManager.UIManager.ConfirmationDialog.Visible)
				{
					ClientGameManager.UIManager.ConfirmationDialog.Hide(false);
				}
				ClientGameManager.SocialManager.DeleteRaidInvite();
			}
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x00128AE0 File Offset: 0x00126CE0
		public bool IsMemberOfMyGroup(GameEntity entity)
		{
			return this.IsGrouped && entity && entity.CharacterData && !entity.CharacterData.GroupId.IsEmpty && entity.CharacterData.GroupId.Value == this.GroupId;
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x00128B44 File Offset: 0x00126D44
		public bool TryGetGroupMember(string playerName, out GroupMember groupMember)
		{
			groupMember = null;
			if (this.IsGrouped)
			{
				for (int i = 0; i < this.m_members.Count; i++)
				{
					if (this.m_members[i].Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase))
					{
						groupMember = this.m_members[i];
						break;
					}
				}
			}
			return groupMember != null;
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x00128BA4 File Offset: 0x00126DA4
		public void RenameGroupMember(string oldName, string newName)
		{
			if (this.IsGrouped)
			{
				for (int i = 0; i < this.m_members.Count; i++)
				{
					if (this.m_members[i].Name.Equals(oldName, StringComparison.InvariantCultureIgnoreCase))
					{
						this.m_members[i].Name = newName;
					}
				}
			}
		}

		// Token: 0x040026BF RID: 9919
		private static LootRollChoice m_autoLootRoll;

		// Token: 0x040026C4 RID: 9924
		public const int kMaxGroupMembers = 6;

		// Token: 0x040026C5 RID: 9925
		private const MessageType kMessageType = MessageType.Social;

		// Token: 0x040026C6 RID: 9926
		private readonly DictionaryList<string, GroupMember> m_members = new DictionaryList<string, GroupMember>(false);

		// Token: 0x040026C7 RID: 9927
		private readonly Dictionary<string, NetworkEntity> m_entities = new Dictionary<string, NetworkEntity>();

		// Token: 0x040026C8 RID: 9928
		private IEnumerator m_delayedGroupIdSet;

		// Token: 0x040026C9 RID: 9929
		private UniqueId m_groupId = UniqueId.Empty;

		// Token: 0x040026CA RID: 9930
		private GroupMember m_leader;

		// Token: 0x040026CB RID: 9931
		private Action<bool, object> m_invitedCallback;
	}
}
