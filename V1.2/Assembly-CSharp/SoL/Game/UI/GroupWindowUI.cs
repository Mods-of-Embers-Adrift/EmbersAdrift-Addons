using System;
using System.Collections.Generic;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x0200088B RID: 2187
	public class GroupWindowUI : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x06003FA9 RID: 16297 RVA: 0x0006B10C File Offset: 0x0006930C
		private void Awake()
		{
			UIManager.GroupWindowUI = this;
			this.SubscribeToEvents();
			this.GroupMembersNearbyOnChanged(0);
			this.GroupElevatedLevelOnChanged(0);
		}

		// Token: 0x06003FAA RID: 16298 RVA: 0x00189358 File Offset: 0x00187558
		private void Start()
		{
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				if (this.m_nameplates[i] != null)
				{
					this.m_nameplates[i].ParentUI = this;
				}
			}
			this.RefreshLabels();
		}

		// Token: 0x06003FAB RID: 16299 RVA: 0x0006B128 File Offset: 0x00069328
		private void OnDestroy()
		{
			this.UnsubscribeToEvents();
		}

		// Token: 0x06003FAC RID: 16300 RVA: 0x0018939C File Offset: 0x0018759C
		private void SubscribeToEvents()
		{
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.RefreshGroup += this.RefreshGroup;
				ClientGameManager.GroupManager.RefreshLeader += this.RefreshLeader;
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.RaidUpdated += this.RefreshLeader;
			}
			LocalZoneManager.ZoneRecordUpdated += this.RefreshGroup;
			this.m_leaveGroupButton.onClick.AddListener(new UnityAction(this.OnLeaveGroupButtonClicked));
		}

		// Token: 0x06003FAD RID: 16301 RVA: 0x00189430 File Offset: 0x00187630
		private void UnsubscribeToEvents()
		{
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.RefreshGroup -= this.RefreshGroup;
				ClientGameManager.GroupManager.RefreshLeader -= this.RefreshLeader;
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.RaidUpdated -= this.RefreshLeader;
			}
			LocalZoneManager.ZoneRecordUpdated -= this.RefreshGroup;
			this.m_leaveGroupButton.onClick.RemoveListener(new UnityAction(this.OnLeaveGroupButtonClicked));
		}

		// Token: 0x06003FAE RID: 16302 RVA: 0x001894C4 File Offset: 0x001876C4
		private void OnLeaveGroupButtonClicked()
		{
			if (ClientGameManager.GroupManager.IsGrouped)
			{
				SolServerCommand solServerCommand = new SolServerCommand(CommandClass.group, CommandType.leave);
				solServerCommand.Args.Add("Receiver", SessionData.SelectedCharacter.Name);
				solServerCommand.Args.Add("Message", SessionData.SelectedCharacter.Name);
				solServerCommand.Send();
			}
		}

		// Token: 0x06003FAF RID: 16303 RVA: 0x00189524 File Offset: 0x00187724
		public void InitLocalPlayer()
		{
			((PlayerCharacterData)LocalPlayer.GameEntity.CharacterData).SyncedNearbyGroupInfo.Changed += this.NearbyGroupInfoOnChanged;
			this.GroupMembersNearbyOnChanged((byte)LocalPlayer.GameEntity.CharacterData.GroupMembersNearby);
			this.GroupElevatedLevelOnChanged((byte)LocalPlayer.GameEntity.CharacterData.GroupedLevel);
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.DefensiveTargetChanged += this.TargetControllerOnDefensiveTargetChanged;
			}
		}

		// Token: 0x06003FB0 RID: 16304 RVA: 0x001895BC File Offset: 0x001877BC
		public void UnsetLocalPlayer()
		{
			if (LocalPlayer.NetworkEntity != null)
			{
				((PlayerCharacterData)LocalPlayer.GameEntity.CharacterData).SyncedNearbyGroupInfo.Changed -= this.NearbyGroupInfoOnChanged;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.DefensiveTargetChanged -= this.TargetControllerOnDefensiveTargetChanged;
			}
		}

		// Token: 0x06003FB1 RID: 16305 RVA: 0x00189634 File Offset: 0x00187834
		private void RefreshGroup()
		{
			int num = 0;
			foreach (GroupMember groupMember in ClientGameManager.GroupManager.GetAllGroupMembers())
			{
				if (!groupMember.IsSelf)
				{
					if (num < this.m_nameplates.Length)
					{
						this.m_nameplates[num].AssignMember(groupMember);
					}
					num++;
				}
			}
			for (int i = num; i < this.m_nameplates.Length; i++)
			{
				this.m_nameplates[i].AssignMember(null);
			}
			this.ResizeGroupWindowSize();
			this.RefreshLeader();
			this.RefreshLabels();
			this.RefreshSelected();
			if (ClientGameManager.GroupManager.IsGrouped)
			{
				if (!this.m_draggable.Visible)
				{
					this.m_draggable.Show(false);
					return;
				}
			}
			else if (this.m_draggable.Visible)
			{
				this.m_draggable.Hide(false);
			}
		}

		// Token: 0x06003FB2 RID: 16306 RVA: 0x00189720 File Offset: 0x00187920
		private void RefreshLeader()
		{
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				bool isLeader = false;
				bool isRaidLeader = false;
				if (this.m_nameplates[i].Member != null)
				{
					isLeader = (ClientGameManager.GroupManager && ClientGameManager.GroupManager.Leader == this.m_nameplates[i].Member);
					isRaidLeader = (ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsRaidLeader(this.m_nameplates[i].Member.Name));
				}
				this.m_nameplates[i].SetLeader(isLeader, isRaidLeader);
			}
			if (ClientGameManager.UIManager.SelfNameplate)
			{
				string playerName = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData) ? LocalPlayer.GameEntity.CharacterData.Name.Value : "UNKNOWN";
				bool isLeader2 = ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsLeader;
				bool isRaidLeader2 = ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsRaidLeader(playerName);
				ClientGameManager.UIManager.SelfNameplate.ToggleLeaderIcon(isLeader2, isRaidLeader2);
			}
		}

		// Token: 0x06003FB3 RID: 16307 RVA: 0x00189854 File Offset: 0x00187A54
		string IContextMenu.FillActionsGetTitle()
		{
			this.m_draggable.FillActionsGetTitle();
			if (ClientGameManager.GroupManager.IsGrouped)
			{
				ContextMenuUI.AddContextAction("Leave Group", true, delegate()
				{
					SolServerCommand solServerCommand = new SolServerCommand(CommandClass.group, CommandType.leave);
					solServerCommand.Args.Add("Receiver", SessionData.SelectedCharacter.Name);
					solServerCommand.Args.Add("Message", SessionData.SelectedCharacter.Name);
					solServerCommand.Send();
				}, null, null);
				return "Group";
			}
			return null;
		}

		// Token: 0x17000EBB RID: 3771
		// (get) Token: 0x06003FB4 RID: 16308 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003FB5 RID: 16309 RVA: 0x0006B130 File Offset: 0x00069330
		private void NearbyGroupInfoOnChanged(NearbyGroupInfo obj)
		{
			this.GroupMembersNearbyOnChanged(obj.GroupMembersNearby);
			this.GroupElevatedLevelOnChanged(obj.GroupedLevel);
		}

		// Token: 0x06003FB6 RID: 16310 RVA: 0x0006B14A File Offset: 0x0006934A
		private void GroupMembersNearbyOnChanged(byte obj)
		{
			this.m_nearbyUi.Value = (int)obj;
		}

		// Token: 0x06003FB7 RID: 16311 RVA: 0x0006B158 File Offset: 0x00069358
		private void GroupElevatedLevelOnChanged(byte obj)
		{
			this.m_gelUi.Value = (int)obj;
		}

		// Token: 0x06003FB8 RID: 16312 RVA: 0x001898AC File Offset: 0x00187AAC
		public GroupMember GetGroupMemberByIndex(int index)
		{
			int num = 0;
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				if (!this.m_nameplates[i].IsEmpty && this.m_nameplates[i].gameObject.activeSelf)
				{
					if (num == index)
					{
						return this.m_nameplates[i].Member;
					}
					num++;
				}
			}
			return null;
		}

		// Token: 0x06003FB9 RID: 16313 RVA: 0x00189908 File Offset: 0x00187B08
		internal void RefreshLabels()
		{
			for (int i = 0; i < this.m_bindingLabels.Length; i++)
			{
				if (this.m_bindingLabels[i] != null)
				{
					GroupMember groupMemberByIndex = this.GetGroupMemberByIndex(i);
					this.m_bindingLabels[i].gameObject.SetActive(groupMemberByIndex != null && groupMemberByIndex.Entity != null);
				}
			}
		}

		// Token: 0x06003FBA RID: 16314 RVA: 0x00189964 File Offset: 0x00187B64
		private int GetActiveNameplateCount()
		{
			int num = 0;
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				if (this.m_nameplates[i].Member != null)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003FBB RID: 16315 RVA: 0x0006B166 File Offset: 0x00069366
		private void TargetControllerOnDefensiveTargetChanged(GameEntity obj)
		{
			this.RefreshSelected();
		}

		// Token: 0x06003FBC RID: 16316 RVA: 0x0018999C File Offset: 0x00187B9C
		private void RefreshSelected()
		{
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				if (this.m_nameplates[i])
				{
					this.m_nameplates[i].RefreshSelected();
				}
			}
		}

		// Token: 0x06003FBD RID: 16317 RVA: 0x001899D8 File Offset: 0x00187BD8
		private void RefreshLayout()
		{
			if (!this.m_nameplateSample)
			{
				return;
			}
			float height = this.m_nameplateSample.rect.height;
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				int siblingIndex = this.m_nameplates[i].gameObject.transform.GetSiblingIndex();
				int num = siblingIndex * this.m_spacing;
				float num2 = (float)siblingIndex * height;
				this.m_nameplates[i].RectTransform.localPosition = new Vector3(0f, -1f * ((float)num + num2), 0f);
			}
		}

		// Token: 0x06003FBE RID: 16318 RVA: 0x00189A6C File Offset: 0x00187C6C
		private void ResizeGroupWindowSize()
		{
			if (!this.m_nameplateParent || !this.m_nameplateSample || !this.m_rectTransform)
			{
				return;
			}
			float num = -1f * this.m_nameplateParent.anchoredPosition.y;
			float y = this.m_nameplateParent.localScale.y;
			float height = this.m_nameplateSample.rect.height;
			int num2 = Application.isPlaying ? this.GetActiveNameplateCount() : this.m_nameplates.Length;
			float y2 = num + (float)num2 * height * y + (float)((num2 - 1) * this.m_spacing) * y;
			this.m_rectTransform.sizeDelta = new Vector2(this.m_rectTransform.sizeDelta.x, y2);
			if (Application.isPlaying)
			{
				this.m_rectTransform.ClampToScreen();
			}
		}

		// Token: 0x06003FC0 RID: 16320 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D5A RID: 15706
		[SerializeField]
		private GroupNameplateControllerUI[] m_nameplates;

		// Token: 0x04003D5B RID: 15707
		[SerializeField]
		private BindingLabel[] m_bindingLabels;

		// Token: 0x04003D5C RID: 15708
		[SerializeField]
		private GroupWindowNearbyUI m_nearbyUi;

		// Token: 0x04003D5D RID: 15709
		[SerializeField]
		private GroupWindowGelUI m_gelUi;

		// Token: 0x04003D5E RID: 15710
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04003D5F RID: 15711
		[SerializeField]
		private RectTransform m_nameplateParent;

		// Token: 0x04003D60 RID: 15712
		[SerializeField]
		private RectTransform m_nameplateSample;

		// Token: 0x04003D61 RID: 15713
		[SerializeField]
		private int m_spacing = 6;

		// Token: 0x04003D62 RID: 15714
		[SerializeField]
		private DraggableUIWindow m_draggable;

		// Token: 0x04003D63 RID: 15715
		[SerializeField]
		private SolButton m_leaveGroupButton;

		// Token: 0x04003D64 RID: 15716
		private readonly HashSet<GroupMember> m_activeGroupMembers = new HashSet<GroupMember>();

		// Token: 0x04003D65 RID: 15717
		private readonly DictionaryList<string, GroupNameplateControllerUI> m_statBarDict = new DictionaryList<string, GroupNameplateControllerUI>(false);
	}
}
