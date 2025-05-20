using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Game;
using SoL.Networking.Objects;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.Managers
{
	// Token: 0x020004BF RID: 1215
	public class ClientNetworkEntityManager : BaseNetworkEntityManager
	{
		// Token: 0x06002204 RID: 8708 RVA: 0x00125DCC File Offset: 0x00123FCC
		public override void RegisterWorldEntity(NetworkEntity entity)
		{
			this.m_worldEntities.AddOrReplace(entity.SpawnId.Value, entity);
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x00125DF4 File Offset: 0x00123FF4
		public override void DeregisterWorldEntity(NetworkEntity entity)
		{
			this.m_worldEntities.Remove(entity.SpawnId.Value);
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x000588DC File Offset: 0x00056ADC
		protected override void UpdateStates()
		{
			if (LocalPlayer.NetworkEntity != null)
			{
				LocalPlayer.NetworkEntity.ProcessLocalUpdates();
			}
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x00125E1C File Offset: 0x0012401C
		public override NetworkEntity InitEntity(NetworkedPrefabCollection networkedPrefabs, uint id, BitBuffer inBuffer, byte channel)
		{
			base.InitEntity(networkedPrefabs, id, inBuffer, channel);
			UniqueId value = inBuffer.ReadUniqueId();
			NetworkEntity networkEntity = null;
			GameObject prefabForIdOrName = networkedPrefabs.GetPrefabForIdOrName(value);
			if (prefabForIdOrName == null)
			{
				this.m_worldEntities.TryGetValue(value, out networkEntity);
			}
			else
			{
				networkEntity = UnityEngine.Object.Instantiate<GameObject>(prefabForIdOrName).GetComponent<NetworkEntity>();
			}
			if (networkEntity != null)
			{
				networkEntity.ClientInit(NetworkManager.Instance, id, inBuffer, NetworkChannelExtensions.GetChannel(channel));
			}
			else
			{
				bool flag = prefabForIdOrName != null;
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to locate netEntity from ",
					networkedPrefabs.name,
					" for spawnId ",
					value.ToString(),
					" on channel ",
					NetworkChannelExtensions.GetChannel(channel).ToString(),
					"! HasPrefab: ",
					flag.ToString()
				}));
			}
			return networkEntity;
		}

		// Token: 0x0400263F RID: 9791
		private readonly Dictionary<string, NetworkEntity> m_worldEntities = new Dictionary<string, NetworkEntity>();
	}
}
