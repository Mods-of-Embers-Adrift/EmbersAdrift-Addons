using System;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Dueling
{
	// Token: 0x02000CA0 RID: 3232
	public class ServerDuelManager : MonoBehaviour
	{
		// Token: 0x0600620C RID: 25100 RVA: 0x0008210C File Offset: 0x0008030C
		private void Start()
		{
			this.m_timeOfNextMonitor = Time.time + 1f;
		}

		// Token: 0x0600620D RID: 25101 RVA: 0x002035BC File Offset: 0x002017BC
		private void Update()
		{
			float time = Time.time;
			if (time < this.m_timeOfNextMonitor)
			{
				return;
			}
			this.m_timeOfNextMonitor = time + 1f;
			for (int i = 0; i < this.m_duels.Count; i++)
			{
				Duel duel = this.m_duels[i];
				if (duel != null)
				{
					duel.Update();
					if (duel.IsFinished() && this.m_duels.Remove(duel.Id))
					{
						StaticPool<Duel>.ReturnToPool(duel);
						i--;
					}
				}
			}
		}

		// Token: 0x0600620E RID: 25102 RVA: 0x00203638 File Offset: 0x00201838
		public void Server_DuelRequest(NetworkEntity sourceEntity, NetworkEntity targetEntity)
		{
			if (sourceEntity && sourceEntity.GameEntity && sourceEntity.PlayerRpcHandler)
			{
				if (sourceEntity.GameEntity.DuelState != DuelStatus.None)
				{
					sourceEntity.PlayerRpcHandler.SendChatNotification("You are currently engaged in a duel!");
					return;
				}
				if (targetEntity && targetEntity.GameEntity)
				{
					if (targetEntity == sourceEntity)
					{
						sourceEntity.PlayerRpcHandler.SendChatNotification("You cannot duel yourself!");
						return;
					}
					if (targetEntity.GameEntity.DuelState != DuelStatus.None)
					{
						string str = targetEntity.GameEntity.CharacterData ? targetEntity.GameEntity.CharacterData.Name.Value : "UNKNOWN";
						sourceEntity.PlayerRpcHandler.SendChatNotification(str + " is currently engaged in a duel!");
						return;
					}
					Duel fromPool = StaticPool<Duel>.GetFromPool();
					fromPool.Init(sourceEntity, targetEntity);
					this.m_duels.Add(fromPool.Id, fromPool);
				}
			}
		}

		// Token: 0x0600620F RID: 25103 RVA: 0x00203738 File Offset: 0x00201938
		public void Server_DuelResponse(UniqueId duelId, bool response)
		{
			Duel duel;
			if (this.m_duels.TryGetValue(duelId, out duel))
			{
				duel.Status = (response ? DuelStatus.Accepted : DuelStatus.Declined);
			}
		}

		// Token: 0x040055AC RID: 21932
		private const float kMonitorUpdateRate = 1f;

		// Token: 0x040055AD RID: 21933
		private readonly DictionaryList<UniqueId, Duel> m_duels = new DictionaryList<UniqueId, Duel>(default(UniqueIdComparer), false);

		// Token: 0x040055AE RID: 21934
		private float m_timeOfNextMonitor;
	}
}
