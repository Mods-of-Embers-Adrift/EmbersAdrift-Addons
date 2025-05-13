using System;
using System.Collections;
using System.Globalization;
using ENet;
using SoL.Game.NPCs;
using SoL.Game.Player;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B5 RID: 1717
	[CreateAssetMenu(menuName = "SoL/Profiles/Base Spawn")]
	public class SpawnProfile : ScriptableObject
	{
		// Token: 0x0600343E RID: 13374 RVA: 0x00063D3A File Offset: 0x00061F3A
		public void StaticSpawn(ISpawnController controller, GameEntity gameEntity)
		{
			this.SpawnInternal(controller, gameEntity);
			ISpawnable spawnable = gameEntity.Spawnable;
			if (spawnable == null)
			{
				return;
			}
			spawnable.Spawned();
		}

		// Token: 0x0600343F RID: 13375 RVA: 0x00063D54 File Offset: 0x00061F54
		public GameEntity DynamicSpawn(ISpawnController controller, Vector3 pos, float heading, uint? isReplacingId = null)
		{
			return this.DynamicSpawn(controller, pos, Quaternion.Euler(new Vector3(0f, heading, 0f)), isReplacingId);
		}

		// Token: 0x06003440 RID: 13376 RVA: 0x001646E4 File Offset: 0x001628E4
		public GameEntity DynamicSpawn(ISpawnController controller, Vector3 pos, Quaternion rot, uint? isReplacingId = null)
		{
			if (!this.m_prefabReference || !this.m_prefabReference.Prefab)
			{
				Debug.LogWarning("Invalid prefab reference on " + base.name + "!");
				return null;
			}
			GameEntity component = UnityEngine.Object.Instantiate<GameObject>(this.m_prefabReference.Prefab, pos, rot).GetComponent<GameEntity>();
			component.NetworkEntity.IsReplacingId = isReplacingId;
			component.NetworkEntity.ServerInit(default(Peer), true, !GlobalSettings.Values.Player.NpcsUseProximity);
			this.SpawnInternal(controller, component);
			ISpawnable spawnable = component.Spawnable;
			if (spawnable != null)
			{
				spawnable.Spawned();
			}
			if (!component.NetworkEntity.UseProximity)
			{
				component.NetworkEntity.BroadcastSpawnPacket();
			}
			return component;
		}

		// Token: 0x06003441 RID: 13377 RVA: 0x00063D75 File Offset: 0x00061F75
		protected virtual void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			if (gameEntity.InfluenceSource != null)
			{
				ServerGameManager.SpatialManager.ForceRebuild();
			}
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x00063D89 File Offset: 0x00061F89
		public void SpawnMessage(Vector3 pos, Quaternion rot)
		{
			this.TimestampedMessage("SPAWNING", pos, rot);
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x00063D98 File Offset: 0x00061F98
		public void KilledMessage(Vector3 pos, Quaternion rot)
		{
			this.TimestampedMessage("KILLED", pos, rot);
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x001647AC File Offset: 0x001629AC
		private void TimestampedMessage(string actionMsg, Vector3 pos, Quaternion rot)
		{
			int num = (LocalZoneManager.ZoneRecord != null) ? LocalZoneManager.ZoneRecord.ZoneId : 0;
			int instanceId = ServerNetworkManager.InstanceId;
			DebugLocation debugLocation = new DebugLocation(num, pos, rot);
			string text = (GlobalSettings.Values && GlobalSettings.Values.Configs != null && GlobalSettings.Values.Configs.Data) ? GlobalSettings.Values.Configs.Data.DeploymentBranch.ToUpper() : "UNKNOWN";
			string text2 = string.Format("[{0}][{1}][{2}.{3}] {4} {5} @ {6}", new object[]
			{
				DateTime.Now.ToString(CultureInfo.InvariantCulture),
				text,
				num,
				instanceId,
				actionMsg,
				base.name,
				debugLocation.DebugString
			});
			Debug.Log(text2);
			if (GameManager.Instance && DeploymentBranchFlagsExtensions.GetBranchFlags() == DeploymentBranchFlags.LIVE)
			{
				GameManager.Instance.SendMessageToStatusChannel(text2);
			}
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x001648AC File Offset: 0x00162AAC
		protected static SpawnTier GetSpawnTier(SpawnTierFlags flags)
		{
			if (flags == SpawnTierFlags.None)
			{
				return SpawnTier.Normal;
			}
			float num = SolMath.Gaussian();
			if (num < -1f && flags.HasBitFlag(SpawnTierFlags.Weak))
			{
				return SpawnTier.Weak;
			}
			if (-1f <= num && num <= 1f && flags.HasBitFlag(SpawnTierFlags.Normal))
			{
				return SpawnTier.Normal;
			}
			if (1f < num && num <= 1.5f && flags.HasBitFlag(SpawnTierFlags.Strong))
			{
				return SpawnTier.Strong;
			}
			if (1.5f < num && num <= 2f && flags.HasBitFlag(SpawnTierFlags.Champion))
			{
				return SpawnTier.Champion;
			}
			if (2f < num && flags.HasBitFlag(SpawnTierFlags.Elite))
			{
				return SpawnTier.Elite;
			}
			return SpawnTier.Normal;
		}

		// Token: 0x06003446 RID: 13382 RVA: 0x00063DA7 File Offset: 0x00061FA7
		private IEnumerable GetPrefabReferences()
		{
			return SolOdinUtilities.GetDropdownItems<SpawnablePrefab>((SpawnablePrefab a) => a != null);
		}

		// Token: 0x04003261 RID: 12897
		[SerializeField]
		protected SpawnablePrefab m_prefabReference;
	}
}
