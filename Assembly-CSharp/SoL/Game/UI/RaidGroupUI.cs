using System;
using System.Collections.Generic;
using SoL.Game.Grouping;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008CE RID: 2254
	public class RaidGroupUI : MonoBehaviour
	{
		// Token: 0x060041E2 RID: 16866 RVA: 0x00190C2C File Offset: 0x0018EE2C
		private void Awake()
		{
			NetworkEntity.Initialized += this.OnEntityInitialized;
			NetworkEntity.Destroyed += this.OnEntityDestroyed;
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.PlayerStatusesChanged += this.SocialManagerOnPlayerStatusesChanged;
			}
		}

		// Token: 0x060041E3 RID: 16867 RVA: 0x00190C80 File Offset: 0x0018EE80
		private void OnDestroy()
		{
			NetworkEntity.Initialized -= this.OnEntityInitialized;
			NetworkEntity.Destroyed -= this.OnEntityDestroyed;
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.PlayerStatusesChanged -= this.SocialManagerOnPlayerStatusesChanged;
			}
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x00190CD4 File Offset: 0x0018EED4
		private void OnEntityInitialized(NetworkEntity obj)
		{
			if (this.m_currentGroupMembers != null && this.m_currentGroupMembers.Count > 0 && obj && obj.GameEntity && obj.GameEntity.Type == GameEntityType.Player && obj.GameEntity.CharacterData && !string.IsNullOrEmpty(obj.GameEntity.CharacterData.Name.Value))
			{
				for (int i = 0; i < this.m_currentGroupMembers.Count; i++)
				{
					if (this.m_currentGroupMembers[i].Name.Equals(obj.GameEntity.CharacterData.Name.Value, StringComparison.InvariantCultureIgnoreCase))
					{
						this.m_currentGroupMembers[i].Entity = obj.GameEntity;
						return;
					}
				}
			}
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x00190DB4 File Offset: 0x0018EFB4
		private void OnEntityDestroyed(NetworkEntity obj)
		{
			if (this.m_currentGroupMembers != null && this.m_currentGroupMembers.Count > 0 && obj && obj.GameEntity && obj.GameEntity.Type == GameEntityType.Player && obj.GameEntity.CharacterData && !string.IsNullOrEmpty(obj.GameEntity.CharacterData.Name.Value))
			{
				for (int i = 0; i < this.m_currentGroupMembers.Count; i++)
				{
					if (this.m_currentGroupMembers[i].Name.Equals(obj.GameEntity.CharacterData.Name.Value, StringComparison.InvariantCultureIgnoreCase))
					{
						this.m_currentGroupMembers[i].Entity = null;
						return;
					}
				}
			}
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x00190E90 File Offset: 0x0018F090
		private void SocialManagerOnPlayerStatusesChanged()
		{
			if (this.m_currentGroupMembers == null || this.m_currentGroupMembers.Count <= 0 || !ClientGameManager.SocialManager)
			{
				return;
			}
			for (int i = 0; i < this.m_currentGroupMembers.Count; i++)
			{
				PlayerStatus status;
				if (this.m_currentGroupMembers[i] != null && ClientGameManager.SocialManager.TryGetLatestStatus(this.m_currentGroupMembers[i].Name, out status))
				{
					this.m_currentGroupMembers[i].UpdateStatus(status);
				}
			}
		}

		// Token: 0x060041E7 RID: 16871 RVA: 0x00190F18 File Offset: 0x0018F118
		public void RefreshRaidGroup(RaidGroup raidGroup)
		{
			this.m_raidGroup = raidGroup;
			if (this.m_currentGroupMembers == null)
			{
				this.m_currentGroupMembers = new List<GroupMember>(6);
			}
			else
			{
				for (int i = 0; i < this.m_currentGroupMembers.Count; i++)
				{
					StaticPool<GroupMember>.ReturnToPool(this.m_currentGroupMembers[i]);
				}
				this.m_currentGroupMembers.Clear();
			}
			int num = 0;
			if (raidGroup != null)
			{
				for (int j = 0; j < raidGroup.Members.Count; j++)
				{
					string text = raidGroup.Members[j];
					PlayerStatus playerStatus;
					if (ClientGameManager.SocialManager.TryGetLatestStatus(text, out playerStatus) && num < this.m_nameplates.Length)
					{
						GroupMemberZoneStatus status = new GroupMemberZoneStatus
						{
							CharacterName = playerStatus.Character,
							ZoneId = playerStatus.ZoneId,
							SubZoneId = playerStatus.SubZoneId,
							Role = playerStatus.Role,
							Level = playerStatus.Level
						};
						GroupMember fromPool = StaticPool<GroupMember>.GetFromPool();
						fromPool.UpdateStatus(status);
						fromPool.Name = text;
						this.m_currentGroupMembers.Add(fromPool);
						this.m_nameplates[num].AssignMember(fromPool);
						bool flag = raidGroup.Leader.Equals(text, StringComparison.InvariantCultureIgnoreCase);
						bool isRaidLeader = flag && ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsRaidLeader(text);
						this.m_nameplates[num].SetLeader(flag, isRaidLeader);
						num++;
					}
				}
			}
			for (int k = num; k < this.m_nameplates.Length; k++)
			{
				this.m_nameplates[k].AssignMember(null);
			}
			this.RefreshSelected();
		}

		// Token: 0x060041E8 RID: 16872 RVA: 0x001910BC File Offset: 0x0018F2BC
		internal int GetActiveNameplateCount()
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

		// Token: 0x060041E9 RID: 16873 RVA: 0x001910F4 File Offset: 0x0018F2F4
		internal void RefreshSelected()
		{
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				if (this.m_nameplates[i])
				{
					this.m_nameplates[i].RefreshSelected();
				}
			}
		}

		// Token: 0x060041EA RID: 16874 RVA: 0x00191130 File Offset: 0x0018F330
		private void RefreshLayout()
		{
			float height = this.m_nameplateSample.rect.height;
			for (int i = 0; i < this.m_nameplates.Length; i++)
			{
				int siblingIndex = this.m_nameplates[i].gameObject.transform.GetSiblingIndex();
				int num = siblingIndex * this.m_spacing;
				float num2 = (float)siblingIndex * height;
				this.m_nameplates[i].RectTransform.localPosition = new Vector3(0f, -1f * ((float)num + num2), 0f);
			}
		}

		// Token: 0x060041EB RID: 16875 RVA: 0x001911B8 File Offset: 0x0018F3B8
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
		}

		// Token: 0x04003F15 RID: 16149
		[SerializeField]
		private GroupNameplateControllerUI[] m_nameplates;

		// Token: 0x04003F16 RID: 16150
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04003F17 RID: 16151
		[SerializeField]
		private RectTransform m_nameplateParent;

		// Token: 0x04003F18 RID: 16152
		[SerializeField]
		private RectTransform m_nameplateSample;

		// Token: 0x04003F19 RID: 16153
		[SerializeField]
		private int m_spacing = 6;

		// Token: 0x04003F1A RID: 16154
		private List<GroupMember> m_currentGroupMembers;

		// Token: 0x04003F1B RID: 16155
		private RaidGroup m_raidGroup;
	}
}
