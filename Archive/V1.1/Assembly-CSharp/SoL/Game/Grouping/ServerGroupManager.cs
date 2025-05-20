using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Grouping
{
	// Token: 0x02000BE2 RID: 3042
	public class ServerGroupManager : MonoBehaviour
	{
		// Token: 0x06005E25 RID: 24101 RVA: 0x0007F4FC File Offset: 0x0007D6FC
		private void Start()
		{
			this.m_wait = new WaitForSeconds(1f);
			this.m_updateInRangeCoroutine = this.UpdateGroupMembersInRange();
			base.StartCoroutine(this.m_updateInRangeCoroutine);
		}

		// Token: 0x06005E26 RID: 24102 RVA: 0x0007F527 File Offset: 0x0007D727
		private void OnDestroy()
		{
			base.StopCoroutine(this.m_updateInRangeCoroutine);
		}

		// Token: 0x06005E27 RID: 24103 RVA: 0x0004475B File Offset: 0x0004295B
		public void UpdatePlayerGroupId(NetworkEntity networkEntity, UniqueId groupId, CharacterRecord characterRecord)
		{
		}

		// Token: 0x06005E28 RID: 24104 RVA: 0x0004475B File Offset: 0x0004295B
		public void UpdatePlayerRaidId(NetworkEntity networkEntity, UniqueId raidId, CharacterRecord characterRecord)
		{
		}

		// Token: 0x06005E29 RID: 24105 RVA: 0x001F5E64 File Offset: 0x001F4064
		private void RegisterPlayer(NetworkEntity networkEntity, UniqueId groupId)
		{
			ServerGroupManager.GroupData groupData;
			if (this.m_groups.TryGetValue(groupId, out groupData))
			{
				groupData.Members.Add(networkEntity);
				return;
			}
			this.m_groups.Add(groupId, new ServerGroupManager.GroupData(groupId, networkEntity));
		}

		// Token: 0x06005E2A RID: 24106 RVA: 0x001F5EA4 File Offset: 0x001F40A4
		public void UnregisterPlayer(NetworkEntity networkEntity, UniqueId previousGroupId)
		{
			ServerGroupManager.GroupData groupData;
			if (!previousGroupId.IsEmpty && this.m_groups.TryGetValue(previousGroupId, out groupData))
			{
				groupData.Members.Remove(networkEntity);
			}
		}

		// Token: 0x06005E2B RID: 24107 RVA: 0x0007F535 File Offset: 0x0007D735
		private IEnumerator UpdateGroupMembersInRange()
		{
			for (;;)
			{
				int num;
				for (int i = 0; i < this.m_groups.Count; i = num + 1)
				{
					if (this.m_groups[i].Members.Count <= 0)
					{
						this.m_groups.Remove(this.m_groups[i].GroupId);
						i--;
					}
					else
					{
						foreach (NetworkEntity netEntity in this.m_groups[i].Members)
						{
							this.UpdateInRangeForMember(netEntity);
						}
						yield return null;
					}
					num = i;
				}
				yield return this.m_wait;
			}
			yield break;
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x001F5ED8 File Offset: 0x001F40D8
		private void UpdateInRangeForMember(NetworkEntity netEntity)
		{
			if (!netEntity || !netEntity.GameEntity || !netEntity.GameEntity.CharacterData)
			{
				return;
			}
			int num = 0;
			int adventuringLevel = netEntity.GameEntity.CharacterData.AdventuringLevel;
			int num2 = adventuringLevel;
			this.m_nearbyGameEntities.Clear();
			if (!netEntity.GameEntity.CharacterData.GroupId.IsEmpty)
			{
				ServerGameManager.SpatialManager.PhysicsQueryRadius(netEntity.GameEntity.transform.position, 50f, this.m_results, true, null);
				for (int i = 0; i < this.m_results.Count; i++)
				{
					if (this.m_results[i] && this.m_results[i] != netEntity && this.m_results[i].GameEntity && this.m_results[i].GameEntity.Type == GameEntityType.Player && this.m_results[i].GameEntity.CharacterData)
					{
						this.m_nearbyGameEntities.Add(this.m_results[i].GameEntity);
					}
				}
			}
			bool flag = !netEntity.GameEntity.CharacterData.GroupId.IsEmpty;
			netEntity.GameEntity.CharacterData.NearbyGroupMembers.Clear();
			bool flag2 = !netEntity.GameEntity.CharacterData.RaidId.IsEmpty;
			netEntity.GameEntity.CharacterData.NearbyRaidMembers.Clear();
			for (int j = 0; j < this.m_nearbyGameEntities.Count; j++)
			{
				if (this.m_nearbyGameEntities[j] && this.m_nearbyGameEntities[j].CharacterData)
				{
					if (flag && this.m_nearbyGameEntities[j].CharacterData.GroupId == netEntity.GameEntity.CharacterData.GroupId)
					{
						netEntity.GameEntity.CharacterData.NearbyGroupMembers.Add(this.m_nearbyGameEntities[j]);
						num++;
						if (this.m_nearbyGameEntities[j].CharacterData.AdventuringLevel > num2)
						{
							num2 = this.m_nearbyGameEntities[j].CharacterData.AdventuringLevel;
						}
					}
					if (flag2 && this.m_nearbyGameEntities[j].CharacterData.RaidId == netEntity.GameEntity.CharacterData.RaidId)
					{
						netEntity.GameEntity.CharacterData.NearbyRaidMembers.Add(this.m_nearbyGameEntities[j]);
					}
				}
			}
			int num3 = Mathf.Clamp(num2, adventuringLevel, adventuringLevel + ServerGameManager.GameServerConfig.UpscaleGroupMembersMaxLevel);
			int num4 = num;
			if (netEntity.GameEntity.CharacterData.GroupedLevel != num3 || netEntity.GameEntity.CharacterData.GroupMembersNearby != num4)
			{
				netEntity.GameEntity.CharacterData.SetNearbyGroupInfo(new NearbyGroupInfo
				{
					GroupedLevel = (byte)num3,
					GroupMembersNearby = (byte)num4
				});
			}
			netEntity.GameEntity.EffectController.NearbyGroupMembersUpdated();
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x0007F544 File Offset: 0x0007D744
		public IEnumerable<NetworkEntity> GroupMembersForGroupId(UniqueId groupId)
		{
			ServerGroupManager.GroupData groupData;
			if (this.m_groups.TryGetValue(groupId, out groupData))
			{
				foreach (NetworkEntity networkEntity in groupData.Members)
				{
					yield return networkEntity;
				}
				HashSet<NetworkEntity>.Enumerator enumerator = default(HashSet<NetworkEntity>.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x0400516A RID: 20842
		public const float kBonusRange = 50f;

		// Token: 0x0400516B RID: 20843
		public const float kBonusRangeSquared = 2500f;

		// Token: 0x0400516C RID: 20844
		private const float kCadence = 1f;

		// Token: 0x0400516D RID: 20845
		private readonly List<NetworkEntity> m_results = new List<NetworkEntity>(100);

		// Token: 0x0400516E RID: 20846
		private readonly List<GameEntity> m_nearbyGameEntities = new List<GameEntity>(100);

		// Token: 0x0400516F RID: 20847
		private readonly DictionaryList<UniqueId, ServerGroupManager.GroupData> m_groups = new DictionaryList<UniqueId, ServerGroupManager.GroupData>(default(UniqueIdComparer), false);

		// Token: 0x04005170 RID: 20848
		private IEnumerator m_updateInRangeCoroutine;

		// Token: 0x04005171 RID: 20849
		private WaitForSeconds m_wait;

		// Token: 0x02000BE3 RID: 3043
		private struct GroupData
		{
			// Token: 0x06005E2F RID: 24111 RVA: 0x0007F55B File Offset: 0x0007D75B
			public GroupData(UniqueId id, NetworkEntity entity)
			{
				this.GroupId = id;
				this.Members = new HashSet<NetworkEntity>
				{
					entity
				};
			}

			// Token: 0x04005172 RID: 20850
			public readonly UniqueId GroupId;

			// Token: 0x04005173 RID: 20851
			public readonly HashSet<NetworkEntity> Members;
		}
	}
}
