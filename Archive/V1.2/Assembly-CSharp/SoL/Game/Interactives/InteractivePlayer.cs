using System;
using Cysharp.Text;
using SoL.Game.Dueling;
using SoL.Game.Grouping;
using SoL.Managers;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B9F RID: 2975
	public class InteractivePlayer : BaseInteractive, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06005C07 RID: 23559 RVA: 0x0007DBD4 File Offset: 0x0007BDD4
		private void InitializeTrade()
		{
			ClientGameManager.TradeManager.Client_RequestTrade(base.GameEntity.NetworkEntity);
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x0007DBEB File Offset: 0x0007BDEB
		private void InviteToGroup()
		{
			ClientGameManager.GroupManager.InviteNewMember(base.GameEntity.CharacterData.Name);
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x0007DC0C File Offset: 0x0007BE0C
		private void AddFriend()
		{
			ClientGameManager.SocialManager.Friend(base.GameEntity.CharacterData.Name);
		}

		// Token: 0x06005C0A RID: 23562 RVA: 0x001F05E0 File Offset: 0x001EE7E0
		private bool CanTrade()
		{
			return base.GameEntity && base.GameEntity != LocalPlayer.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.CharacterFlags.Value.CanTrade() && this.CanInteract(LocalPlayer.GameEntity);
		}

		// Token: 0x06005C0B RID: 23563 RVA: 0x001F0648 File Offset: 0x001EE848
		private bool CanInspect()
		{
			return base.GameEntity && base.GameEntity != LocalPlayer.GameEntity && base.GameEntity.CharacterData && !base.GameEntity.CharacterData.BlockInspections;
		}

		// Token: 0x06005C0C RID: 23564 RVA: 0x001F069C File Offset: 0x001EE89C
		public string FillActionsGetTitle()
		{
			if (!base.GameEntity || base.GameEntity == LocalPlayer.GameEntity)
			{
				return null;
			}
			bool enabled = this.CanTrade();
			string text = "Request Trade";
			bool flag = true;
			if (SessionData.IsTrial)
			{
				enabled = false;
				text = ZString.Format<string, string>("{0} {1}", text, "(Purchase Required)");
				flag = false;
			}
			else if (base.GameEntity.CharacterData && base.GameEntity.CharacterData.IsTrial)
			{
				enabled = false;
				text = "Trial players cannot trade.";
				flag = false;
			}
			Func<bool> interactiveCheck = null;
			if (flag)
			{
				interactiveCheck = new Func<bool>(this.CanTrade);
			}
			ContextMenuUI.AddContextAction(text, enabled, new Action(this.InitializeTrade), null, interactiveCheck);
			ContextMenuUI.AddContextAction("Inspect", this.CanInspect(), delegate()
			{
				if (base.GameEntity.NetworkEntity && LocalPlayer.GameEntity && LocalPlayer.GameEntity.NetworkEntity && LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler)
				{
					LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.InspectRequest(base.GameEntity.NetworkEntity);
				}
			}, null, new Func<bool>(this.CanInspect));
			string value = base.GameEntity.CharacterData.Name.Value;
			InteractivePlayer.FillActionsForEntityName(value, base.GameEntity);
			return value;
		}

		// Token: 0x06005C0D RID: 23565 RVA: 0x001F0798 File Offset: 0x001EE998
		public static void FillActionsForEntityName(string characterName, GameEntity entity = null)
		{
			ContextMenuUI.AddContextAction("Who", true, delegate()
			{
				SolServerCommand solServerCommand = CommandClass.chat.NewCommand(CommandType.who);
				solServerCommand.Args.Add("Message", characterName);
				solServerCommand.Send();
			}, null, null);
			ContextMenuUI.AddContextAction("Send Tell", true, delegate()
			{
				UIManager.ActiveChatInput.StartWhisper(characterName);
			}, null, null);
			GroupMember groupMember;
			if (ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader && ClientGameManager.GroupManager.TryGetGroupMember(characterName, out groupMember))
			{
				if (groupMember.Status == GroupMemberStatus.Online)
				{
					ContextMenuUI.AddContextAction("Promote to Leader", true, delegate()
					{
						ClientGameManager.GroupManager.PromoteToLeader(groupMember);
					}, null, null);
				}
				ContextMenuUI.AddContextAction("Kick from Group", true, delegate()
				{
					ClientGameManager.GroupManager.KickFromGroup(groupMember);
				}, null, null);
			}
			else if ((!ClientGameManager.GroupManager.IsGrouped || ClientGameManager.GroupManager.IsLeader) && !ClientGameManager.GroupManager.GroupIsFull)
			{
				ContextMenuUI.AddContextAction("Invite to Group", true, delegate()
				{
					ClientGameManager.GroupManager.InviteNewMember(characterName);
				}, null, null);
			}
			if (ClientGameManager.SocialManager.IsFriend(characterName))
			{
				ContextMenuUI.AddContextAction("Remove Friend", true, delegate()
				{
					ClientGameManager.SocialManager.Unfriend(characterName);
				}, null, null);
				ContextMenuUI.AddContextAction("Remove Friend & Block", true, delegate()
				{
					ClientGameManager.SocialManager.Block(characterName);
				}, null, null);
			}
			else if (ClientGameManager.SocialManager.IsBlocked(characterName))
			{
				ContextMenuUI.AddContextAction("Unblock", true, delegate()
				{
					ClientGameManager.SocialManager.Unblock(characterName);
				}, null, null);
			}
			else
			{
				ContextMenuUI.AddContextAction(ClientGameManager.SocialManager.HasIncomingFriendRequestFrom(characterName) ? "Accept Friend Request" : "Send Friend Request", true, delegate()
				{
					ClientGameManager.SocialManager.Friend(characterName);
				}, null, null);
				ContextMenuUI.AddContextAction("Block", true, delegate()
				{
					ClientGameManager.SocialManager.Block(characterName);
				}, null, null);
			}
			if (ClientGameManager.SocialManager.CanInviteGuildMembers && (entity == null || entity.CharacterData == null || string.IsNullOrEmpty(entity.CharacterData.GuildName.Value)))
			{
				ContextMenuUI.AddContextAction("Invite to Guild", true, delegate()
				{
					ClientGameManager.SocialManager.InviteToGuild(characterName);
				}, null, null);
			}
			if (entity && entity.NetworkEntity && DuelExtensions.IsWithinDuelDistance(LocalPlayer.GameEntity, entity))
			{
				ContextMenuUI.AddContextAction("Duel", true, delegate()
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.DuelRequest(entity.NetworkEntity);
				}, null, null);
			}
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x001F0A04 File Offset: 0x001EEC04
		private bool AllowContextMenu()
		{
			return !Options.GameOptions.DisablePlayerWorldSpaceContextMenus.Value && (!Options.GameOptions.DisablePlayerWorldSpaceContextMenusWeaponsDrawn.Value || !LocalPlayer.GameEntity || !LocalPlayer.GameEntity.CharacterData || LocalPlayer.GameEntity.Vitals.Stance != Stance.Combat);
		}

		// Token: 0x170015B2 RID: 5554
		// (get) Token: 0x06005C0F RID: 23567 RVA: 0x0007DC2D File Offset: 0x0007BE2D
		GameObject IInteractiveBase.gameObject
		{
			get
			{
				if (!this || !base.gameObject)
				{
					return null;
				}
				return base.gameObject;
			}
		}

		// Token: 0x06005C10 RID: 23568 RVA: 0x0007DC4C File Offset: 0x0007BE4C
		string IContextMenu.FillActionsGetTitle()
		{
			if (!this.AllowContextMenu())
			{
				return null;
			}
			return this.FillActionsGetTitle();
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001F0A60 File Offset: 0x001EEC60
		private ITooltipParameter GetTooltipParameter()
		{
			if (base.GameEntity != null && base.GameEntity.NetworkEntity != null)
			{
				return new ObjectTextTooltipParameter(this, base.GameEntity.CharacterData.Name.Value, false);
			}
			return null;
		}

		// Token: 0x170015B3 RID: 5555
		// (get) Token: 0x06005C12 RID: 23570 RVA: 0x0007DC5E File Offset: 0x0007BE5E
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015B4 RID: 5556
		// (get) Token: 0x06005C13 RID: 23571 RVA: 0x0007DC6C File Offset: 0x0007BE6C
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015B5 RID: 5557
		// (get) Token: 0x06005C14 RID: 23572 RVA: 0x0007DC74 File Offset: 0x0007BE74
		protected override CursorType ActiveCursorType
		{
			get
			{
				if (!this.AllowContextMenu())
				{
					return CursorType.MainCursor;
				}
				return CursorType.TextCursor;
			}
		}

		// Token: 0x170015B6 RID: 5558
		// (get) Token: 0x06005C15 RID: 23573 RVA: 0x0007DC82 File Offset: 0x0007BE82
		protected override CursorType InactiveCursorType
		{
			get
			{
				if (!this.AllowContextMenu())
				{
					return CursorType.MainCursor;
				}
				return CursorType.TextCursorInactive;
			}
		}

		// Token: 0x04005000 RID: 20480
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
