using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game;
using SoL.Game.Animation;
using SoL.Game.AuctionHouse;
using SoL.Game.Discovery;
using SoL.Game.EffectSystem;
using SoL.Game.GM;
using SoL.Game.HuntingLog;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.Messages;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Player;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.States;
using SoL.Game.Trading;
using SoL.Game.Transactions;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.Networking.REST;
using SoL.Networking.RPC;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace SoL.Networking.Game
{
	// Token: 0x0200040D RID: 1037
	public class PlayerRpcHandler : NetworkEntityRpcs
	{
		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06001B4E RID: 6990 RVA: 0x000552B3 File Offset: 0x000534B3
		private PlayerCollectionController m_playerCollectionController
		{
			get
			{
				if (this._playerCollectionController == null)
				{
					base.GameEntity.CollectionController.TryGetAsType(out this._playerCollectionController);
				}
				return this._playerCollectionController;
			}
		}

		// Token: 0x06001B4F RID: 6991 RVA: 0x0010BCA8 File Offset: 0x00109EA8
		private void RequestZone_Internal(int targetZoneId, int targetZonePointIndex, int zonePointInstanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(621828145);
			rpcBuffer.AddInt(targetZoneId);
			rpcBuffer.AddInt(targetZonePointIndex);
			rpcBuffer.AddInt(zonePointInstanceId);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B50 RID: 6992 RVA: 0x0010BCF8 File Offset: 0x00109EF8
		private void RequestZoneToDiscovery_Internal(UniqueId sourceDiscoveryId, int targetZoneId, UniqueId discoveryId, bool useTravelEssence)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2013177559);
			sourceDiscoveryId.PackData(rpcBuffer);
			rpcBuffer.AddInt(targetZoneId);
			discoveryId.PackData(rpcBuffer);
			rpcBuffer.AddBool(useTravelEssence);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B51 RID: 6993 RVA: 0x0010BD54 File Offset: 0x00109F54
		private void RequestZoneToGroup_Internal(UniqueId sourceDiscoveryId, int targetZoneId, byte groupTeleportIndex, bool useTravelEssence)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-189487217);
			sourceDiscoveryId.PackData(rpcBuffer);
			rpcBuffer.AddInt(targetZoneId);
			rpcBuffer.AddByte(groupTeleportIndex);
			rpcBuffer.AddBool(useTravelEssence);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RequestZone_Internal(int targetZoneId, int targetZonePointIndex)
		{
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x0010BDB0 File Offset: 0x00109FB0
		private void AuthorizeZone_Internal(bool authorized, int zoneId, int zonePointInstanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1543406291);
			rpcBuffer.AddBool(authorized);
			rpcBuffer.AddInt(zoneId);
			rpcBuffer.AddInt(zonePointInstanceId);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B54 RID: 6996 RVA: 0x0010BE00 File Offset: 0x0010A000
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestZone(int targetZoneId, int targetZonePointIndex, int zonePointInstanceId)
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = true;
				this.RequestZone_Internal(targetZoneId, targetZonePointIndex, zonePointInstanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				bool flag = false;
				string text = string.Empty;
				Vector3 vector = Vector3.zero;
				ZoneId zoneId;
				if (ZoneIdExtensions.ZoneIdDict.TryGetValue(targetZoneId, out zoneId))
				{
					ZonePoint zonePoint = LocalZoneManager.GetZonePoint(zoneId, targetZonePointIndex);
					if (zonePoint && base.GameEntity.ServerPlayerController)
					{
						vector = zonePoint.gameObject.transform.position;
						if (zonePoint.IsWithinRange(base.GameEntity))
						{
							if (zonePoint.ServerCanEntityInteract(this.m_netEntity.GameEntity))
							{
								base.GameEntity.ServerPlayerController.ZonePlayer(zoneId, targetZonePointIndex, ZoningState.Zoning);
								flag = true;
							}
							else
							{
								text = "Unauthorized";
							}
						}
						else
						{
							text = "Out of range!";
						}
					}
					else
					{
						text = "Null zone point!";
					}
					if (flag && base.GameEntity && base.GameEntity.Vitals && base.GameEntity.Vitals.GetCurrentHealthState() != HealthState.Alive)
					{
						text = "Cannot zone while not alive!";
						flag = false;
					}
				}
				else
				{
					text = "Unable to find ZoneId!";
				}
				if (!flag)
				{
					string text2 = (base.GameEntity && base.GameEntity.CharacterData) ? base.GameEntity.CharacterData.Name.Value : "UNKNOWN";
					Debug.Log(string.Concat(new string[]
					{
						"[Zone Request Denied] ",
						text2,
						" (loc=",
						base.gameObject.transform.position.ToString(),
						") requested zone to ",
						targetZoneId.ToString(),
						" (index=",
						targetZonePointIndex.ToString(),
						", loc=",
						vector.ToString(),
						") but was denied for reason: ",
						text
					}));
				}
				this.AuthorizeZone(flag, targetZoneId, zonePointInstanceId);
			}
		}

		// Token: 0x06001B55 RID: 6997 RVA: 0x0010C010 File Offset: 0x0010A210
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestZoneToDiscovery(UniqueId sourceDiscoveryId, int targetZoneId, UniqueId targetDiscoveryId, bool useTravelEssence)
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = (targetZoneId != LocalZoneManager.ZoneRecord.ZoneId);
				this.RequestZoneToDiscovery_Internal(sourceDiscoveryId, targetZoneId, targetDiscoveryId, useTravelEssence);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				bool flag = false;
				string text = string.Empty;
				ZoneId zoneId;
				MonolithProfile monolithProfile;
				InteractiveZonePointTeleporter interactiveZonePointTeleporter;
				MonolithProfile monolithProfile2;
				if (ZoneIdExtensions.ZoneIdDict.TryGetValue(targetZoneId, out zoneId) && InternalGameDatabase.Archetypes.TryGetAsType<MonolithProfile>(targetDiscoveryId, out monolithProfile) && LocalZoneManager.TryGetTeleporter(sourceDiscoveryId, out interactiveZonePointTeleporter) && interactiveZonePointTeleporter.IsValid && InternalGameDatabase.Archetypes.TryGetAsType<MonolithProfile>(interactiveZonePointTeleporter.DiscoveryId, out monolithProfile2))
				{
					if (interactiveZonePointTeleporter.ZonePoint.IsWithinRange(base.GameEntity))
					{
						List<UniqueId> list;
						if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null && base.GameEntity.CollectionController.Record.Discoveries != null && base.GameEntity.CollectionController.Record.Discoveries.TryGetValue(zoneId, out list) && list.Contains(targetDiscoveryId))
						{
							MonolithFlags flags = monolithProfile2.MonolithFlag | monolithProfile.MonolithFlag;
							int monolithCost = GlobalSettings.Values.Ashen.GetMonolithCost(flags);
							ValueTuple<int, int> emberAndTravelEssenceCounts = base.GameEntity.CollectionController.GetEmberAndTravelEssenceCounts();
							int item = emberAndTravelEssenceCounts.Item1;
							int item2 = emberAndTravelEssenceCounts.Item2;
							if (useTravelEssence ? (item + item2 >= monolithCost) : (item >= monolithCost))
							{
								if (base.GameEntity.InCombat)
								{
									text = "In combat!";
								}
								else if (targetZoneId == LocalZoneManager.ZoneRecord.ZoneId)
								{
									ZonePoint zonePointForDiscovery = LocalZoneManager.GetZonePointForDiscovery(targetDiscoveryId);
									if (zonePointForDiscovery != null)
									{
										Vector3 position = zonePointForDiscovery.GetPosition();
										Quaternion rotation = zonePointForDiscovery.GetRotation();
										this.m_playerCollectionController.UseEssenceForTravel(monolithCost, useTravelEssence);
										this.SetRemotePlayerPosition(base.GameEntity.NetworkEntity, position, rotation.eulerAngles.y, "Teleported!");
										return;
									}
									text = "Invalid target!";
								}
								else
								{
									base.GameEntity.ServerPlayerController.ZonePlayer(zoneId, targetDiscoveryId, ZoningState.DiscoveryTeleport, monolithCost, useTravelEssence);
									flag = true;
								}
							}
							else
							{
								text = "Not enough essence!";
							}
						}
						else
						{
							text = "Invalid target!";
						}
					}
					else
					{
						text = "Out of range!";
					}
				}
				else
				{
					text = "Invalid data!";
				}
				if (!flag)
				{
					string text2 = (base.GameEntity && base.GameEntity.CharacterData) ? base.GameEntity.CharacterData.Name.Value : "UNKNOWN";
					Debug.Log(string.Concat(new string[]
					{
						"[Zone to Discovery Request Denied] ",
						text2,
						" requested zone to ",
						targetZoneId.ToString(),
						" (",
						targetDiscoveryId.ToString(),
						") but was denied for reason: ",
						text
					}));
				}
				this.AuthorizeZone(flag, targetZoneId, -1);
			}
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x0010C308 File Offset: 0x0010A508
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestZoneToGroup(UniqueId sourceDiscoveryId, int targetZoneId, byte groupTeleportIndex, bool useTravelEssence)
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = (targetZoneId != LocalZoneManager.ZoneRecord.ZoneId);
				this.RequestZoneToGroup_Internal(sourceDiscoveryId, targetZoneId, groupTeleportIndex, useTravelEssence);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.RequestZoneToGroup_Server(sourceDiscoveryId, targetZoneId, groupTeleportIndex, useTravelEssence);
			}
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x0010C364 File Offset: 0x0010A564
		private void RequestZoneToGroup_Server(UniqueId sourceDiscoveryId, int targetZoneId, byte groupTeleportIndex, bool useTravelEssence)
		{
			PlayerRpcHandler.<RequestZoneToGroup_Server>d__12 <RequestZoneToGroup_Server>d__;
			<RequestZoneToGroup_Server>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RequestZoneToGroup_Server>d__.<>1__state = -1;
			<RequestZoneToGroup_Server>d__.<>t__builder.Start<PlayerRpcHandler.<RequestZoneToGroup_Server>d__12>(ref <RequestZoneToGroup_Server>d__);
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x0010C394 File Offset: 0x0010A594
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RequestZone(int targetZoneId, int targetZonePointIndex)
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = true;
				this.GM_RequestZone_Internal(targetZoneId, targetZonePointIndex);
				return;
			}
			if (this.m_netEntity.IsServer && this.m_netEntity.GameEntity.GM)
			{
				bool authorized = false;
				ZoneId targetZoneId2;
				if (ZoneIdExtensions.ZoneIdDict.TryGetValue(targetZoneId, out targetZoneId2))
				{
					base.GameEntity.ServerPlayerController.ZonePlayer(targetZoneId2, targetZonePointIndex, ZoningState.GMZoning);
					authorized = true;
				}
				this.AuthorizeZone(authorized, targetZoneId, -1);
			}
		}

		// Token: 0x06001B59 RID: 7001 RVA: 0x0010C414 File Offset: 0x0010A614
		[NetworkRPC(RpcType.ServerToClient)]
		public void AuthorizeZone(bool authorized, int zoneId, int zonePointInstanceId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.AuthorizeZone_Internal(authorized, zoneId, zonePointInstanceId);
				return;
			}
			ZonePoint.ReceiveZoneResponse(zonePointInstanceId, authorized);
			if (authorized)
			{
				Debug.Log("Received authorization to zone to " + zoneId.ToString() + "!");
				GameManager.SceneCompositionManager.DisconnectAndLoadZone(zoneId);
			}
			else
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)zoneId);
				string text;
				if (zoneRecord != null)
				{
					text = zoneRecord.DisplayName;
				}
				else
				{
					ZoneId zoneId2 = (ZoneId)zoneId;
					text = zoneId2.ToString();
				}
				string str = text;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unauthorized to zone to " + str);
			}
			this.m_zoneRequested = false;
		}

		// Token: 0x06001B5A RID: 7002 RVA: 0x0010C4AC File Offset: 0x0010A6AC
		private void RequestZoneToMapDiscovery_Internal(UniqueId discoveryId, bool useTravelEssence)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1231663422);
			discoveryId.PackData(rpcBuffer);
			rpcBuffer.AddBool(useTravelEssence);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x000552E0 File Offset: 0x000534E0
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestZoneToMapDiscovery(UniqueId discoveryId, bool useTravelEssence)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestZoneToMapDiscovery_Internal(discoveryId, useTravelEssence);
				return;
			}
			bool isServer = this.m_netEntity.IsServer;
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x0010C4F4 File Offset: 0x0010A6F4
		private void Client_RequestInteraction_Internal(NetworkEntity targetNetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1990732059);
			rpcBuffer.AddUInt(targetNetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x0010C540 File Offset: 0x0010A740
		private void Server_RequestInteraction_Internal(NetworkEntity targetNetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1926620825);
			rpcBuffer.AddUInt(targetNetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x0010C58C File Offset: 0x0010A78C
		private void Client_CancelInteraction_Internal(NetworkEntity targetNetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1371032526);
			rpcBuffer.AddUInt(targetNetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x0010C5D8 File Offset: 0x0010A7D8
		private void Server_CancelInteraction_Internal(NetworkEntity targetNetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(389710554);
			rpcBuffer.AddUInt(targetNetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x0010C624 File Offset: 0x0010A824
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_RequestInteraction(NetworkEntity targetNetEntity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_RequestInteraction_Internal(targetNetEntity);
				return;
			}
			if (this.m_netEntity.IsServer && targetNetEntity != null)
			{
				IInteractive component = targetNetEntity.GetComponent<IInteractive>();
				if (component != null && component.CanInteract(this.m_netEntity.GameEntity))
				{
					component.BeginInteraction(this.m_netEntity.GameEntity);
				}
			}
		}

		// Token: 0x06001B61 RID: 7009 RVA: 0x0010C68C File Offset: 0x0010A88C
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_RequestInteraction(NetworkEntity targetNetEntity)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_RequestInteraction_Internal(targetNetEntity);
				return;
			}
			if (this.m_netEntity.IsLocal && targetNetEntity != null)
			{
				IInteractive component = targetNetEntity.GetComponent<IInteractive>();
				if (component != null && component.CanInteract(this.m_netEntity.GameEntity))
				{
					component.BeginInteraction(this.m_netEntity.GameEntity);
				}
			}
		}

		// Token: 0x06001B62 RID: 7010 RVA: 0x0010C6F4 File Offset: 0x0010A8F4
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_CancelInteraction(NetworkEntity targetNetEntity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_CancelInteraction_Internal(targetNetEntity);
				return;
			}
			if (this.m_netEntity.IsServer && targetNetEntity != null)
			{
				IInteractive component = targetNetEntity.GetComponent<IInteractive>();
				if (component == null)
				{
					return;
				}
				component.EndInteraction(this.m_netEntity.GameEntity, true);
			}
		}

		// Token: 0x06001B63 RID: 7011 RVA: 0x0010C748 File Offset: 0x0010A948
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_CancelInteraction(NetworkEntity targetNetEntity)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_CancelInteraction_Internal(targetNetEntity);
				return;
			}
			if (this.m_netEntity.IsLocal && targetNetEntity != null)
			{
				IInteractive component = targetNetEntity.GetComponent<IInteractive>();
				if (component == null)
				{
					return;
				}
				component.EndInteraction(this.m_netEntity.GameEntity, false);
			}
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x0010C79C File Offset: 0x0010A99C
		private void Client_RequestStateInteraction_Internal(int key)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-179190786);
			rpcBuffer.AddInt(key);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B65 RID: 7013 RVA: 0x0010C7DC File Offset: 0x0010A9DC
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_RequestStateInteraction(int key)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_RequestStateInteraction_Internal(key);
				return;
			}
			IState state;
			if (this.m_netEntity.IsServer && StateReplicator.Instance && StateReplicator.Instance.TryGetState(key, out state))
			{
				IInteractive interactive = state as IInteractive;
				if (interactive != null && interactive.CanInteract(this.m_netEntity.GameEntity))
				{
					interactive.BeginInteraction(this.m_netEntity.GameEntity);
				}
			}
		}

		// Token: 0x06001B66 RID: 7014 RVA: 0x0010C854 File Offset: 0x0010AA54
		private void SetTrackedMasteryOption_Internal(UniqueId trackedMasteryId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-8957207);
			trackedMasteryId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x0010C894 File Offset: 0x0010AA94
		[NetworkRPC(RpcType.ClientToServer)]
		public void SetTrackedMasteryOption(UniqueId trackedMasteryId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SetTrackedMasteryOption_Internal(trackedMasteryId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.CollectionController.Record.Settings.TrackedMastery = (trackedMasteryId.IsEmpty ? new UniqueId("") : trackedMasteryId);
			}
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x0010C8F4 File Offset: 0x0010AAF4
		private void Client_GiveUp_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(597638525);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x00055304 File Offset: 0x00053504
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_GiveUp()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_GiveUp_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				((PlayerVitals_Server)base.GameEntity.Vitals).GiveUp();
			}
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x0010C92C File Offset: 0x0010AB2C
		private void Server_ApplyDamageToGearForDeath_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-55666921);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x0010C964 File Offset: 0x0010AB64
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_ApplyDamageToGearForDeath()
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_ApplyDamageToGearForDeath_Internal();
				return;
			}
			if (this.m_netEntity.IsLocal && this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.Vitals)
			{
				this.m_netEntity.GameEntity.Vitals.ApplyDamageToGearForDeath();
			}
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x0010C9D4 File Offset: 0x0010ABD4
		private void Server_Respawn_Internal(Vector3 pos, float h)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-411380183);
			rpcBuffer.AddVector3(pos, NetworkManager.Range);
			rpcBuffer.AddFloat(h);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x0010CA20 File Offset: 0x0010AC20
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Respawn(Vector3 pos, float h)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Respawn_Internal(pos, h);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				LocalPlayer.Motor.SetPositionAndRotation(pos, Quaternion.Euler(new Vector3(0f, h, 0f)), true);
				return;
			}
			Quaternion quaternion = Quaternion.Euler(new Vector3(0f, h, 0f));
			TransformReplicator component = base.gameObject.GetComponent<TransformReplicator>();
			if (component != null)
			{
				component.ExternallySetTargetPosRot(pos, quaternion);
			}
			base.gameObject.transform.SetPositionAndRotation(pos, quaternion);
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x0010CAB8 File Offset: 0x0010ACB8
		private void SetGroupId_Internal(UniqueId groupId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-269373221);
			groupId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x0010CAF8 File Offset: 0x0010ACF8
		[NetworkRPC(RpcType.ClientToServer)]
		public void SetGroupId(UniqueId groupId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SetGroupId_Internal(groupId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.GroupManager.UpdatePlayerGroupId(this.m_netEntity, groupId, base.GameEntity.CollectionController.Record);
			}
		}

		// Token: 0x06001B70 RID: 7024 RVA: 0x0010CB48 File Offset: 0x0010AD48
		private void SetRaidId_Internal(UniqueId raidId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-124764458);
			raidId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B71 RID: 7025 RVA: 0x0010CB88 File Offset: 0x0010AD88
		[NetworkRPC(RpcType.ClientToServer)]
		public void SetRaidId(UniqueId raidId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SetRaidId_Internal(raidId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.GroupManager.UpdatePlayerRaidId(this.m_netEntity, raidId, base.GameEntity.CollectionController.Record);
			}
		}

		// Token: 0x06001B72 RID: 7026 RVA: 0x0010CBD8 File Offset: 0x0010ADD8
		private void ForfeitInventoryRequest_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1884773577);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B73 RID: 7027 RVA: 0x0010CC10 File Offset: 0x0010AE10
		private void ForfeitInventoryResponse_Internal(OpCodes op)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(820392702);
			rpcBuffer.AddEnum(op);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x0005533C File Offset: 0x0005353C
		[NetworkRPC(RpcType.ClientToServer)]
		public void ForfeitInventoryRequest()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ForfeitInventoryRequest_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessForfeitInventoryRequest();
			}
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x0005536A File Offset: 0x0005356A
		[NetworkRPC(RpcType.ServerToClient)]
		public void ForfeitInventoryResponse(OpCodes op)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ForfeitInventoryResponse_Internal(op);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessForfeitInventoryResponse(op);
			}
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x0010CC50 File Offset: 0x0010AE50
		private void ClientJumped_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-983144067);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x0010CC88 File Offset: 0x0010AE88
		private void ClientJumpedBroadcast_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1143853980);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x0010CCC0 File Offset: 0x0010AEC0
		[NetworkRPC(RpcType.ClientToServer)]
		public void ClientJumped()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ClientJumped_Internal();
				this.TriggerJump();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (base.GameEntity.SkillsController.PendingIsActive)
				{
					base.GameEntity.SkillsController.Server_Execution_Cancel(base.GameEntity.SkillsController.Pending.ArchetypeId);
				}
				base.GameEntity.Vitals.AlterStamina(-1f * GlobalSettings.Values.Player.StaminaCostJump * 100f);
				this.ClientJumpedBroadcast();
			}
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x0005539A File Offset: 0x0005359A
		[NetworkRPC(RpcType.ServerBroadcast)]
		private void ClientJumpedBroadcast()
		{
			if (this.m_netEntity.IsServer)
			{
				this.ClientJumpedBroadcast_Internal();
				return;
			}
			if (!this.m_netEntity.IsLocal)
			{
				this.TriggerJump();
			}
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x000553C3 File Offset: 0x000535C3
		private void TriggerJump()
		{
			if (base.GameEntity.AudioEventController != null)
			{
				base.GameEntity.AudioEventController.PlayAudioEvent("Jump");
			}
			IAnimationController animancerController = base.GameEntity.AnimancerController;
			if (animancerController == null)
			{
				return;
			}
			animancerController.TriggerJump();
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x0010CD5C File Offset: 0x0010AF5C
		private void SendChatNotification_Internal(string msg)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1627622002);
			rpcBuffer.AddString(msg);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x0010CD9C File Offset: 0x0010AF9C
		private void SendLongChatNotification_Internal(LongString msg)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1972585747);
			msg.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x0010CDDC File Offset: 0x0010AFDC
		private void TriggerCannotPerform_Internal(string chatNotification, string overhead)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1993149079);
			rpcBuffer.AddString(chatNotification);
			rpcBuffer.AddString(overhead);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x00055402 File Offset: 0x00053602
		[NetworkRPC(RpcType.ServerToClient)]
		public void SendChatNotification(string msg)
		{
			if (GameManager.IsServer)
			{
				this.SendChatNotification_Internal(msg);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, msg);
			}
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x0005542D File Offset: 0x0005362D
		[NetworkRPC(RpcType.ServerToClient)]
		public void SendLongChatNotification(LongString msg)
		{
			if (GameManager.IsServer)
			{
				this.SendLongChatNotification_Internal(msg);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, msg.Value);
			}
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x0005545D File Offset: 0x0005365D
		[NetworkRPC(RpcType.ServerToClient)]
		public void TriggerCannotPerform(string chatNotification, string overhead)
		{
			if (GameManager.IsServer)
			{
				this.TriggerCannotPerform_Internal(chatNotification, overhead);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (!string.IsNullOrEmpty(chatNotification))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, chatNotification);
				}
				UIManager.TriggerCannotPerform(overhead);
			}
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x0010CE24 File Offset: 0x0010B024
		private void ForgetMastery_Internal(UniqueId instanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-119425);
			instanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x00055497 File Offset: 0x00053697
		[NetworkRPC(RpcType.ClientToServer)]
		public void ForgetMastery(UniqueId instanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ForgetMastery_Internal(instanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessForgetMasteryRequest(instanceId);
			}
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x0010CE64 File Offset: 0x0010B064
		private void ProcessCurrencyTransaction_Internal(CurrencyTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1832983623);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x0010CEA4 File Offset: 0x0010B0A4
		private void ProcessInteractiveStationCurrencyTransaction_Internal(ulong? inventoryCurrency, ulong? personalBankCurrency)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-592976503);
			rpcBuffer.AddNullableUlong(inventoryCurrency);
			rpcBuffer.AddNullableUlong(personalBankCurrency);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x0010CEEC File Offset: 0x0010B0EC
		private void CurrencyTransferRequest_Internal(CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(247163380);
			toRemove.PackData(rpcBuffer);
			toAdd.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x0010CF38 File Offset: 0x0010B138
		private void CurrencyTransferRequestResponse_Internal(OpCodes op, CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1251682354);
			rpcBuffer.AddEnum(op);
			toRemove.PackData(rpcBuffer);
			toAdd.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x0010CF8C File Offset: 0x0010B18C
		private void CurrencyModifyEvent_Internal(CurrencyModifyTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1722530210);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x0010CFCC File Offset: 0x0010B1CC
		private void ProcessMultiContainerCurrencyTransaction_Internal(MultiContainerCurrencyTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(398795495);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x0010D00C File Offset: 0x0010B20C
		private void ProcessEventCurrencyTransaction_Internal(ulong? eventCurrency)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1136993507);
			rpcBuffer.AddNullableUlong(eventCurrency);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x000554C7 File Offset: 0x000536C7
		[NetworkRPC(RpcType.ServerToClient)]
		public void ProcessCurrencyTransaction(CurrencyTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ProcessCurrencyTransaction_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessCurrencyTransaction(transaction);
			}
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x000554F7 File Offset: 0x000536F7
		[NetworkRPC(RpcType.ServerToClient)]
		public void ProcessInteractiveStationCurrencyTransaction(ulong? inventoryCurrency, ulong? personalBankCurrency)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ProcessInteractiveStationCurrencyTransaction_Internal(inventoryCurrency, personalBankCurrency);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessInteractiveStationCurrencyTransaction(inventoryCurrency, personalBankCurrency);
			}
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00055529 File Offset: 0x00053729
		[NetworkRPC(RpcType.ClientToServer)]
		public void CurrencyTransferRequest(CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.CurrencyTransferRequest_Internal(toRemove, toAdd);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessCurrencyTransferRequest(toRemove, toAdd);
			}
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x0005555B File Offset: 0x0005375B
		[NetworkRPC(RpcType.ServerToClient)]
		public void CurrencyTransferRequestResponse(OpCodes op, CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			if (this.m_netEntity.IsServer)
			{
				this.CurrencyTransferRequestResponse_Internal(op, toRemove, toAdd);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessCurrencyTransferResponse(op, toRemove, toAdd);
			}
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x0005558F File Offset: 0x0005378F
		[NetworkRPC(RpcType.ServerToClient)]
		public void CurrencyModifyEvent(CurrencyModifyTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.CurrencyModifyEvent_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessCurrencyModifiedEvent(transaction);
			}
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000555BF File Offset: 0x000537BF
		[NetworkRPC(RpcType.ServerToClient)]
		public void ProcessMultiContainerCurrencyTransaction(MultiContainerCurrencyTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ProcessMultiContainerCurrencyTransaction_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessMultiContainerCurrencyTransaction(transaction);
			}
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000555EF File Offset: 0x000537EF
		[NetworkRPC(RpcType.ServerToClient)]
		public void ProcessEventCurrencyTransaction(ulong? eventCurrency)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ProcessEventCurrencyTransaction_Internal(eventCurrency);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessEventCurrencyTransaction(eventCurrency);
			}
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x0010D04C File Offset: 0x0010B24C
		private void RequestObjectiveIteration_Internal(ObjectiveIterationCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1047154739);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x0010D08C File Offset: 0x0010B28C
		private void NotifyObjectiveIteration_Internal(OpCodes opCode, string message, ObjectiveIterationCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1996453892);
			rpcBuffer.AddEnum(opCode);
			rpcBuffer.AddString(message);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x0010D0DC File Offset: 0x0010B2DC
		private void RequestRewardReissue_Internal(ObjectiveIterationCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1931216826);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x0010D11C File Offset: 0x0010B31C
		private void NotifyGMQuestReset_Internal(UniqueId questId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-382355451);
			questId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x0010D15C File Offset: 0x0010B35C
		private void MuteQuest_Internal(UniqueId questId, bool mute)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1937016485);
			questId.PackData(rpcBuffer);
			rpcBuffer.AddBool(mute);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x0010D1A4 File Offset: 0x0010B3A4
		private void RequestDropQuestsAndTasksForMastery_Internal(UniqueId archetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(133330845);
			archetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x0005561F File Offset: 0x0005381F
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestObjectiveIteration(ObjectiveIterationCache cache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestObjectiveIteration_Internal(cache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.Progress(cache, base.GameEntity, false);
			}
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x0010D1E4 File Offset: 0x0010B3E4
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyObjectiveIteration(OpCodes opCode, string message, ObjectiveIterationCache cache)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyObjectiveIteration_Internal(opCode, message, cache);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (opCode == OpCodes.Ok)
				{
					GameManager.QuestManager.NotifyQuestsUpdated(cache, base.GameEntity);
					return;
				}
				if (opCode == OpCodes.Error && !string.IsNullOrEmpty(message))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, message);
				}
			}
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x00055655 File Offset: 0x00053855
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestRewardReissue(ObjectiveIterationCache cache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestRewardReissue_Internal(cache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.ReissueReward(cache, base.GameEntity);
			}
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x0010D248 File Offset: 0x0010B448
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyGMQuestReset(UniqueId questId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyGMQuestReset_Internal(questId);
				return;
			}
			Quest quest;
			if (this.m_netEntity.IsLocal && InternalGameDatabase.Quests.TryGetItem(questId, out quest))
			{
				GameManager.QuestManager.ResetQuest(base.GameEntity, quest);
			}
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x0005568A File Offset: 0x0005388A
		[NetworkRPC(RpcType.ClientToServer)]
		public void MuteQuest(UniqueId questId, bool mute)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.MuteQuest_Internal(questId, mute);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.MuteQuest(base.GameEntity, questId, mute);
			}
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x0010D298 File Offset: 0x0010B498
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestDropQuestsAndTasksForMastery(UniqueId archetypeId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestDropQuestsAndTasksForMastery_Internal(archetypeId);
				GameManager.QuestManager.DropQuestsAndTasksForMastery(archetypeId, base.GameEntity);
				return;
			}
			if (this.m_netEntity.IsServer && GameManager.QuestManager.DropQuestsAndTasksForMastery(archetypeId, base.GameEntity))
			{
				base.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
			}
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x0010D308 File Offset: 0x0010B508
		private void DrawBBTask_Internal(BBTaskDrawCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(744560740);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x0010D348 File Offset: 0x0010B548
		private void NotifyDrawBBTask_Internal(OpCodes opCode, string message, BBTaskDrawCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-172514876);
			rpcBuffer.AddEnum(opCode);
			rpcBuffer.AddString(message);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x0010D398 File Offset: 0x0010B598
		private void IterateBBTask_Internal(ObjectiveIterationCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-120975047);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x0010D3D8 File Offset: 0x0010B5D8
		private void NotifyBBTaskIterated_Internal(OpCodes opCode, string message, ObjectiveIterationCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-209396829);
			rpcBuffer.AddEnum(opCode);
			rpcBuffer.AddString(message);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x000556C1 File Offset: 0x000538C1
		[NetworkRPC(RpcType.ClientToServer)]
		public void DrawBBTask(BBTaskDrawCache cache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DrawBBTask_Internal(cache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.DrawBBTask(cache, base.GameEntity);
			}
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x0010D428 File Offset: 0x0010B628
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyDrawBBTask(OpCodes opCode, string message, BBTaskDrawCache cache)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyDrawBBTask_Internal(opCode, message, cache);
				return;
			}
			if (this.m_netEntity.IsLocal && opCode != OpCodes.Ok && opCode == OpCodes.Error && !string.IsNullOrEmpty(message))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, message);
			}
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x000556F6 File Offset: 0x000538F6
		[NetworkRPC(RpcType.ClientToServer)]
		public void IterateBBTask(ObjectiveIterationCache cache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.IterateBBTask_Internal(cache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.ProgressTask(cache, base.GameEntity, false);
			}
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x0010D478 File Offset: 0x0010B678
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyBBTaskIterated(OpCodes opCode, string message, ObjectiveIterationCache cache)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyBBTaskIterated_Internal(opCode, message, cache);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (opCode == OpCodes.Ok)
				{
					GameManager.QuestManager.NotifyTaskUpdated(cache, base.GameEntity);
					return;
				}
				if (opCode == OpCodes.Error && !string.IsNullOrEmpty(message))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, message);
				}
			}
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x0010D4DC File Offset: 0x0010B6DC
		private void RequestNpcLearn_Internal(NpcLearningCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1084803737);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BA6 RID: 7078 RVA: 0x0010D51C File Offset: 0x0010B71C
		private void NotifyNpcLearn_Internal(OpCodes opCode, string message, NpcLearningCache cache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-333753616);
			rpcBuffer.AddEnum(opCode);
			rpcBuffer.AddString(message);
			cache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x0005572C File Offset: 0x0005392C
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestNpcLearn(NpcLearningCache cache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestNpcLearn_Internal(cache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				GameManager.QuestManager.Learn(cache, base.GameEntity);
			}
		}

		// Token: 0x06001BA8 RID: 7080 RVA: 0x0010D56C File Offset: 0x0010B76C
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyNpcLearn(OpCodes opCode, string message, NpcLearningCache cache)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyNpcLearn_Internal(opCode, message, cache);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (opCode == OpCodes.Ok)
				{
					GameManager.QuestManager.NotifyLearned(cache, base.GameEntity);
					return;
				}
				if (opCode == OpCodes.Error && !string.IsNullOrEmpty(message))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, message);
				}
			}
		}

		// Token: 0x06001BA9 RID: 7081 RVA: 0x0010D5D0 File Offset: 0x0010B7D0
		private void DiscoveryNotification_Internal(UniqueId discoveryProfileId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2086276638);
			discoveryProfileId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BAA RID: 7082 RVA: 0x00055761 File Offset: 0x00053961
		[NetworkRPC(RpcType.ServerToClient)]
		public void DiscoveryNotification(UniqueId discoveryProfileId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.DiscoveryNotification_Internal(discoveryProfileId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				DiscoveryProgression.DiscoverForEntity(this.m_netEntity.GameEntity, discoveryProfileId);
			}
		}

		// Token: 0x06001BAB RID: 7083 RVA: 0x0010D610 File Offset: 0x0010B810
		private void ModifyEquipmentAbsorbed_Internal(UniqueId instanceId, int absorbed)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2016753354);
			instanceId.PackData(rpcBuffer);
			rpcBuffer.AddInt(absorbed);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x0010D658 File Offset: 0x0010B858
		private void RemoteRefreshHighestLevelMastery_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1412194063);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x00055796 File Offset: 0x00053996
		[NetworkRPC(RpcType.ServerToClient)]
		public void ModifyEquipmentAbsorbed(UniqueId instanceId, int absorbed)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ModifyEquipmentAbsorbed_Internal(instanceId, absorbed);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessModifyEquipmentAbsorbed(instanceId, absorbed);
			}
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x000557C8 File Offset: 0x000539C8
		[NetworkRPC(RpcType.ServerToClient)]
		public void RemoteRefreshHighestLevelMastery()
		{
			if (this.m_netEntity.IsServer)
			{
				this.RemoteRefreshHighestLevelMastery_Internal();
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
			}
		}

		// Token: 0x06001BAF RID: 7087 RVA: 0x0010D690 File Offset: 0x0010B890
		private void TakeFallDamage_Internal(float distanceFallen)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2019658019);
			rpcBuffer.AddFloat(distanceFallen);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BB0 RID: 7088 RVA: 0x0010D6D0 File Offset: 0x0010B8D0
		private void Server_TakeFallDamage_Internal(int damage, bool knockedOut)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-171816164);
			rpcBuffer.AddInt(damage);
			rpcBuffer.AddBool(knockedOut);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001BB1 RID: 7089 RVA: 0x000557F6 File Offset: 0x000539F6
		[NetworkRPC(RpcType.ClientToServer)]
		public void TakeFallDamage(float distanceFallen)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.TakeFallDamage_Internal(distanceFallen);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.Vitals.TakeFallDamage(distanceFallen);
			}
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x0010D718 File Offset: 0x0010B918
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_TakeFallDamage(int damage, bool knockedOut)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TakeFallDamage_Internal(damage, knockedOut);
				return;
			}
			if (base.GameEntity && LocalPlayer.GameEntity)
			{
				bool flag = this.m_netEntity.IsLocal;
				if (!flag && LocalPlayer.GameEntity)
				{
					flag = ((base.GameEntity.gameObject.transform.position - LocalPlayer.GameEntity.transform.position).sqrMagnitude <= 625f);
				}
				if (flag)
				{
					if (this.m_netEntity.IsLocal)
					{
						MessageManager.CombatQueue.AddToQueue(MessageType.MyCombatIn, "You have fallen for " + damage.ToString() + " damage!");
					}
					else
					{
						MessageManager.CombatQueue.AddToQueue(MessageType.OtherCombat, base.GameEntity.CharacterData.Name.Value + " has fallen for " + damage.ToString() + " damage!");
					}
					if (!knockedOut)
					{
						IAnimationController animancerController = base.GameEntity.AnimancerController;
						if (animancerController != null)
						{
							animancerController.TriggerHit();
						}
						if (base.GameEntity.AudioEventController != null)
						{
							base.GameEntity.AudioEventController.PlayAudioEvent("Hit");
						}
					}
				}
			}
		}

		// Token: 0x06001BB3 RID: 7091 RVA: 0x0010D86C File Offset: 0x0010BA6C
		private void RequestTitleChange_Internal(string title)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-99391108);
			rpcBuffer.AddString(title);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x0005582B File Offset: 0x00053A2B
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestTitleChange(string title)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestTitleChange_Internal(title);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				TitleManager.AssignTitleToPlayer(base.GameEntity, title);
			}
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x0010D8AC File Offset: 0x0010BAAC
		private void LootRollResponse_Internal(LootRollItemResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1347211544);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x0005585B File Offset: 0x00053A5B
		[NetworkRPC(RpcType.ClientToServer)]
		public void LootRollResponse(LootRollItemResponse response)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.LootRollResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.LootRollManager.LootRollResponse(base.GameEntity, response);
			}
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x0010D8EC File Offset: 0x0010BAEC
		private void PlayEmoteRequest_Internal(UniqueId emoteId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-57037076);
			emoteId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x0010D92C File Offset: 0x0010BB2C
		private void PlayEmoteResponse_Internal(UniqueId emoteId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-626409346);
			emoteId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x0010D96C File Offset: 0x0010BB6C
		[NetworkRPC(RpcType.ClientToServer)]
		public void PlayEmoteRequest(UniqueId emoteId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.PlayEmoteRequest_Internal(emoteId);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity)
			{
				Emote emote;
				LearnableArchetype archetype;
				if (GlobalSettings.Values.Animation.TryGetDefaultEmote(emoteId, out emote))
				{
					if (emote.SubscriberOnly && !base.GameEntity.Subscriber)
					{
						this.SendLongChatNotification(new LongString
						{
							Value = "This emote is reserved for subscribers. <link=\"activateSub\"><u>Activate your subscription.</u></link>"
						});
						return;
					}
					this.PlayEmoteResponse(emoteId);
					this.AdvanceEmoteOrders(emoteId);
					return;
				}
				else if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Emotes != null && base.GameEntity.CollectionController.Emotes.TryGetLearnableForId(emoteId, out archetype) && archetype.TryGetAsType(out emote))
				{
					if (emote.SubscriberOnly && !base.GameEntity.Subscriber)
					{
						this.SendLongChatNotification(new LongString
						{
							Value = "This emote is reserved for subscribers. <link=\"activateSub\"><u>Activate your subscription.</u></link>"
						});
						return;
					}
					this.PlayEmoteResponse(emoteId);
					this.AdvanceEmoteOrders(emoteId);
				}
			}
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x0010DA84 File Offset: 0x0010BC84
		private void AdvanceEmoteOrders(UniqueId emoteId)
		{
			if (base.GameEntity.CharacterData.ObjectiveOrders.HasOrders<EmoteObjective>())
			{
				List<ValueTuple<UniqueId, EmoteObjective>> pooledOrderList = base.GameEntity.CharacterData.ObjectiveOrders.GetPooledOrderList<EmoteObjective>();
				foreach (ValueTuple<UniqueId, EmoteObjective> valueTuple in pooledOrderList)
				{
					if (valueTuple.Item2.IsValidEmote(emoteId))
					{
						Quest quest;
						int hash;
						BBTask bbtask;
						if (InternalGameDatabase.Quests.TryGetItem(valueTuple.Item1, out quest) && quest.TryGetObjectiveHashForActiveObjective(valueTuple.Item2.Id, base.GameEntity, out hash))
						{
							GameManager.QuestManager.Progress(new ObjectiveIterationCache
							{
								QuestId = valueTuple.Item1,
								ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
							}, base.GameEntity, true);
						}
						else if (InternalGameDatabase.BBTasks.TryGetItem(valueTuple.Item1, out bbtask))
						{
							GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
							{
								QuestId = valueTuple.Item1,
								ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(valueTuple.Item2.CombinedId(valueTuple.Item1))
							}, base.GameEntity, true);
						}
					}
				}
				base.GameEntity.CharacterData.ObjectiveOrders.ReturnPooledOrderList<EmoteObjective>(pooledOrderList);
			}
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x0010DBE8 File Offset: 0x0010BDE8
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void PlayEmoteResponse(UniqueId emoteId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.PlayEmoteResponse_Internal(emoteId);
				return;
			}
			Emote emote;
			if (base.GameEntity && InternalGameDatabase.Archetypes.TryGetAsType<Emote>(emoteId, out emote))
			{
				if (this.m_netEntity.IsLocal || base.GameEntity.GetCachedSqrDistanceFromLocalPlayer() < 900f)
				{
					Emote.AddEmoteTextToChat(base.GameEntity, emote);
				}
				if (base.GameEntity.Vitals && base.GameEntity.Vitals.Stance.CanPlayEmoteAnimation())
				{
					IAnimationController animancerController = base.GameEntity.AnimancerController;
					if (animancerController == null)
					{
						return;
					}
					animancerController.PlayEmote(emote);
				}
			}
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x0010DC90 File Offset: 0x0010BE90
		private void StuckRequest_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1964341464);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x0010DCC8 File Offset: 0x0010BEC8
		private void RopeRequest_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1565556672);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x0010DD00 File Offset: 0x0010BF00
		private void StuckResponse_Internal(StuckTransaction stuckResponse)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1501188591);
			stuckResponse.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x0010DD40 File Offset: 0x0010BF40
		[NetworkRPC(RpcType.ClientToServer)]
		public void StuckRequest()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.StuckRequest_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				StuckTransaction stuckResponse = new StuckTransaction
				{
					Op = OpCodes.Error
				};
				for (int i = 0; i < PlayerRpcHandler.m_stuckDistances.Length; i++)
				{
					NavMeshHit navMeshHit;
					if (NavMeshUtilities.SamplePosition(base.GameEntity.gameObject.transform.position, out navMeshHit, PlayerRpcHandler.m_stuckDistances[i], -1))
					{
						stuckResponse.Op = OpCodes.Ok;
						stuckResponse.Position = new Vector3?(navMeshHit.position);
						break;
					}
				}
				this.StuckResponse(stuckResponse);
				this.LogStuck();
			}
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x0010DDE4 File Offset: 0x0010BFE4
		[NetworkRPC(RpcType.ClientToServer)]
		public void RopeRequest()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RopeRequest_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				StuckTransaction stuckResponse = new StuckTransaction
				{
					Op = OpCodes.Error
				};
				NavMeshHit navMeshHit;
				if (base.GameEntity && base.GameEntity.TargetController && base.GameEntity.TargetController.DefensiveTarget && PlayerMotorController.ValidRopeRequest(base.GameEntity) && NavMeshUtilities.SamplePosition(base.GameEntity.TargetController.DefensiveTarget.gameObject.transform.position, out navMeshHit, PlayerMotorController.kMaxRopeDistance, -1))
				{
					stuckResponse.Op = OpCodes.Ok;
					stuckResponse.Position = new Vector3?(navMeshHit.position);
					this.LogRope();
				}
				this.StuckResponse(stuckResponse);
			}
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x00055890 File Offset: 0x00053A90
		[NetworkRPC(RpcType.ServerToClient)]
		public void StuckResponse(StuckTransaction stuckResponse)
		{
			if (this.m_netEntity.IsServer)
			{
				this.StuckResponse_Internal(stuckResponse);
				return;
			}
			if (this.m_netEntity.IsLocal && LocalPlayer.Motor != null)
			{
				LocalPlayer.Motor.StuckResponse(stuckResponse);
			}
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x0010DEC4 File Offset: 0x0010C0C4
		public static PlayerRpcHandler.PlayerUserData GetPlayerUserData(GameEntity entity)
		{
			PlayerRpcHandler.PlayerUserData result = new PlayerRpcHandler.PlayerUserData
			{
				UserId = "UNKNOWN",
				CharacterId = "UNKNOWN",
				CharacterName = "UNKNOWN"
			};
			if (entity)
			{
				if (entity.User != null)
				{
					result.UserId = entity.User.Id;
				}
				if (entity.CollectionController != null && entity.CollectionController.Record != null)
				{
					result.CharacterId = entity.CollectionController.Record.Id;
					result.CharacterName = entity.CollectionController.Record.Name;
				}
			}
			return result;
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x0010DF68 File Offset: 0x0010C168
		private void LogStuck()
		{
			PlayerRpcHandler.m_stuckArguments[0] = "Stuck";
			PlayerRpcHandler.m_stuckArguments[1] = "UNKNOWN";
			PlayerRpcHandler.m_stuckArguments[2] = "UNKNOWN";
			PlayerRpcHandler.m_stuckArguments[3] = "UNKNOWN";
			PlayerRpcHandler.m_stuckArguments[4] = -1;
			PlayerRpcHandler.m_stuckArguments[5] = "UNKNOWN";
			PlayerRpcHandler.m_stuckArguments[6] = "UNKNOWN";
			if (base.GameEntity)
			{
				PlayerRpcHandler.PlayerUserData playerUserData = PlayerRpcHandler.GetPlayerUserData(base.GameEntity);
				PlayerRpcHandler.m_stuckArguments[1] = playerUserData.UserId;
				PlayerRpcHandler.m_stuckArguments[2] = playerUserData.CharacterId;
				PlayerRpcHandler.m_stuckArguments[3] = playerUserData.CharacterName;
				PlayerRpcHandler.m_stuckArguments[5] = base.GameEntity.gameObject.transform.position.ToString();
			}
			if (LocalZoneManager.ZoneRecord != null)
			{
				PlayerRpcHandler.m_stuckArguments[4] = LocalZoneManager.ZoneRecord.ZoneId;
			}
			if (base.GameEntity && LocalZoneManager.ZoneRecord != null)
			{
				PlayerRpcHandler.m_stuckArguments[6] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, base.GameEntity.gameObject).DebugString;
			}
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.Stuck, "{@EventType} {@UserId}.{@CharacterId}.{@PlayerName} has used /stuck in {@ZoneId} at {@PlayerPosition} ({@PlayerDebugLocation})", PlayerRpcHandler.m_stuckArguments);
			base.StartCoroutine(this.StuckReportToSlack());
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x0010E0B0 File Offset: 0x0010C2B0
		private void LogRope()
		{
			PlayerRpcHandler.m_ropeArguments[0] = "Rope";
			PlayerRpcHandler.m_ropeArguments[1] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[2] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[3] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[4] = -1;
			PlayerRpcHandler.m_ropeArguments[5] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[6] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[7] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[8] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[9] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[10] = "UNKNOWN";
			PlayerRpcHandler.m_ropeArguments[11] = "UNKNOWN";
			if (base.GameEntity)
			{
				PlayerRpcHandler.PlayerUserData playerUserData = PlayerRpcHandler.GetPlayerUserData(base.GameEntity);
				PlayerRpcHandler.m_ropeArguments[1] = playerUserData.UserId;
				PlayerRpcHandler.m_ropeArguments[2] = playerUserData.CharacterId;
				PlayerRpcHandler.m_ropeArguments[3] = playerUserData.CharacterName;
				PlayerRpcHandler.m_ropeArguments[5] = base.GameEntity.gameObject.transform.position.ToString();
			}
			if (LocalZoneManager.ZoneRecord != null)
			{
				PlayerRpcHandler.m_ropeArguments[4] = LocalZoneManager.ZoneRecord.ZoneId;
			}
			if (base.GameEntity && LocalZoneManager.ZoneRecord != null)
			{
				PlayerRpcHandler.m_ropeArguments[6] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, base.GameEntity.gameObject).DebugString;
			}
			if (base.GameEntity && base.GameEntity.TargetController && base.GameEntity.TargetController.DefensiveTarget)
			{
				PlayerRpcHandler.PlayerUserData playerUserData2 = PlayerRpcHandler.GetPlayerUserData(base.GameEntity.TargetController.DefensiveTarget);
				PlayerRpcHandler.m_ropeArguments[7] = playerUserData2.UserId;
				PlayerRpcHandler.m_ropeArguments[8] = playerUserData2.CharacterId;
				PlayerRpcHandler.m_ropeArguments[9] = playerUserData2.CharacterName;
				PlayerRpcHandler.m_ropeArguments[10] = base.GameEntity.TargetController.DefensiveTarget.gameObject.transform.position.ToString();
				if (LocalZoneManager.ZoneRecord != null)
				{
					PlayerRpcHandler.m_ropeArguments[11] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, base.GameEntity.TargetController.DefensiveTarget.gameObject).DebugString;
				}
			}
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.Stuck, "{@EventType} {@UserId}.{@CharacterId}.{@PlayerName} has used /rope in {@ZoneId} at {@PlayerPosition} ({@PlayerDebugLocation}) TO {@TargetUserId}.{@TargetCharacterId}.{@TargetPlayerName} {@TargetPosition} ({@TargetDebugLocation})", PlayerRpcHandler.m_ropeArguments);
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x0010E314 File Offset: 0x0010C514
		private void DuelRequest_Internal(NetworkEntity opponent)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1000303217);
			rpcBuffer.AddUInt(opponent.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x0010E360 File Offset: 0x0010C560
		private void DuelResponse_Internal(UniqueId duelId, bool response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1676310227);
			duelId.PackData(rpcBuffer);
			rpcBuffer.AddBool(response);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x0010E3A8 File Offset: 0x0010C5A8
		private void Server_DuelRequest_Internal(UniqueId duelId, string sourceName)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1169266588);
			duelId.PackData(rpcBuffer);
			rpcBuffer.AddString(sourceName);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x0010E3F0 File Offset: 0x0010C5F0
		[NetworkRPC(RpcType.ClientToServer)]
		public void DuelRequest(NetworkEntity opponent)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DuelRequest_Internal(opponent);
				string str = (opponent.GameEntity && opponent.GameEntity.CharacterData) ? opponent.GameEntity.CharacterData.Name.Value : "UNKNOWN";
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You challenge " + str + " to a duel.");
				return;
			}
			if (this.m_netEntity.IsServer && ServerGameManager.DuelManager)
			{
				ServerGameManager.DuelManager.Server_DuelRequest(this.m_netEntity, opponent);
			}
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x000558CC File Offset: 0x00053ACC
		[NetworkRPC(RpcType.ClientToServer)]
		public void DuelResponse(UniqueId duelId, bool response)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DuelResponse_Internal(duelId, response);
				return;
			}
			if (this.m_netEntity.IsServer && ServerGameManager.DuelManager)
			{
				ServerGameManager.DuelManager.Server_DuelResponse(duelId, response);
			}
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x0010E494 File Offset: 0x0010C694
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_DuelRequest(UniqueId duelId, string sourceName)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_DuelRequest_Internal(duelId, sourceName);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (ClientGameManager.SocialManager.IsBlocked(sourceName))
				{
					this.DuelResponse(duelId, false);
					return;
				}
				DialogOptions opts = new DialogOptions
				{
					Title = "Duel",
					Text = sourceName + " has challenged you to a duel!",
					ConfirmationText = "Accept",
					CancelText = "Decline",
					Callback = delegate(bool answer, object obj)
					{
						this.DuelResponse(duelId, answer);
					}
				};
				ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x0010E55C File Offset: 0x0010C75C
		private void RequestSelfCorpseDrag_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-96600820);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x0010E594 File Offset: 0x0010C794
		private void RequestGroupMemberCorpseDrag_Internal(string targetName)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(46416971);
			rpcBuffer.AddString(targetName);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x0010E5D4 File Offset: 0x0010C7D4
		private void ToggleGroupConsent_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2046457682);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x00055909 File Offset: 0x00053B09
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestSelfCorpseDrag()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestSelfCorpseDrag_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				CorpseManager.Server_AttemptToDragCorpse(this.m_netEntity.GameEntity, this.m_netEntity.GameEntity);
			}
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x0010E60C File Offset: 0x0010C80C
		[NetworkRPC(RpcType.ClientToServer)]
		public void RequestGroupMemberCorpseDrag(string targetName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RequestGroupMemberCorpseDrag_Internal(targetName);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity))
				{
					CorpseManager.Server_AttemptToDragCorpse(this.m_netEntity.GameEntity, networkEntity.GameEntity);
					return;
				}
				this.SendChatNotification("Could not locate " + targetName + "!");
			}
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x0010E674 File Offset: 0x0010C874
		[NetworkRPC(RpcType.ClientToServer)]
		public void ToggleGroupConsent()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ToggleGroupConsent_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_netEntity.GameEntity.GroupBagDragConsent = !this.m_netEntity.GameEntity.GroupBagDragConsent;
				string msg = this.m_netEntity.GameEntity.GroupBagDragConsent ? "Group members have been given consent to drag your bag." : "Group members can no longer drag your bag.";
				this.SendChatNotification(msg);
			}
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x0010E6EC File Offset: 0x0010C8EC
		private void ChangePortraitRequest_Internal(UniqueId portraitId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1239755285);
			portraitId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x0010E72C File Offset: 0x0010C92C
		[NetworkRPC(RpcType.ClientToServer)]
		public void ChangePortraitRequest(UniqueId portraitId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ChangePortraitRequest_Internal(portraitId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				UniqueId uniqueId = (base.GameEntity.Subscriber ? GlobalSettings.Values.Portraits.PlayerPortraitIds : GlobalSettings.Values.Portraits.BasePortraitIds).Contains(portraitId) ? portraitId : UniqueId.Empty;
				if (this.m_playerCollectionController && this.m_playerCollectionController.Record != null)
				{
					this.m_playerCollectionController.Record.Settings.PortraitId = uniqueId;
				}
				if (base.GameEntity && base.GameEntity.CharacterData)
				{
					base.GameEntity.CharacterData.PortraitId.Value = uniqueId;
				}
			}
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x0010E804 File Offset: 0x0010CA04
		private void InspectRequest_Internal(NetworkEntity toInspect)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1793832839);
			rpcBuffer.AddUInt(toInspect.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x0010E850 File Offset: 0x0010CA50
		private void InspectResponse_Internal(InspectionTransaction inspectionTransaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1910680035);
			inspectionTransaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x0010E890 File Offset: 0x0010CA90
		private void InspectNotification_Internal(string sourceName)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1488160658);
			rpcBuffer.AddString(sourceName);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x0010E8D0 File Offset: 0x0010CAD0
		[NetworkRPC(RpcType.ClientToServer)]
		public void InspectRequest(NetworkEntity toInspect)
		{
			if (!this.m_netEntity.IsLocal)
			{
				if (this.m_netEntity.IsServer && toInspect && toInspect.GameEntity && toInspect.GameEntity.CharacterData && !toInspect.GameEntity.CharacterData.BlockInspections && toInspect.GameEntity.CollectionController != null && toInspect.GameEntity.CollectionController.Equipment != null)
				{
					InspectionTransaction inspectionTransaction = new InspectionTransaction
					{
						Op = OpCodes.Ok,
						TargetName = toInspect.GameEntity.CharacterData.Name.Value,
						Instances = StaticListPool<ArchetypeInstance>.GetFromPool()
					};
					foreach (ArchetypeInstance archetypeInstance in toInspect.GameEntity.CollectionController.Equipment.Instances)
					{
						if (((EquipmentSlot)archetypeInstance.Index).IncludeInInspection() && !archetypeInstance.Archetype.ExcludeFromInspection)
						{
							inspectionTransaction.Instances.Add(archetypeInstance);
						}
					}
					this.InspectResponse(inspectionTransaction);
					if (toInspect.PlayerRpcHandler)
					{
						string sourceName = (this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.CharacterData) ? this.m_netEntity.GameEntity.CharacterData.Name.Value : string.Empty;
						toInspect.PlayerRpcHandler.InspectNotification(sourceName);
					}
					StaticListPool<ArchetypeInstance>.ReturnToPool(inspectionTransaction.Instances);
					inspectionTransaction.Instances = null;
				}
				return;
			}
			if (Time.time - this.m_timeOfLastInspect <= 1f)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, PlayerRpcHandler.kInspectTooSoon);
				return;
			}
			this.m_timeOfLastInspect = Time.time;
			this.InspectRequest_Internal(toInspect);
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x0010EAC4 File Offset: 0x0010CCC4
		[NetworkRPC(RpcType.ServerToClient)]
		public void InspectResponse(InspectionTransaction inspectionTransaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.InspectResponse_Internal(inspectionTransaction);
				return;
			}
			ContainerInstance containerInstance;
			if (this.m_netEntity.IsLocal && inspectionTransaction.Op == OpCodes.Ok && inspectionTransaction.Instances != null && inspectionTransaction.Instances.Count > 0 && this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.CollectionController != null && this.m_netEntity.GameEntity.CollectionController.TryGetInstance(ContainerType.Inspection, out containerInstance))
			{
				containerInstance.DestroyContents();
				for (int i = 0; i < inspectionTransaction.Instances.Count; i++)
				{
					ArchetypeInstance archetypeInstance = inspectionTransaction.Instances[i];
					archetypeInstance.CreateItemInstanceUI();
					archetypeInstance.InstanceUI.ToggleLock(true);
					containerInstance.Add(archetypeInstance, true);
				}
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.Inspection)
				{
					ClientGameManager.UIManager.Inspection.InitInspection(inspectionTransaction.TargetName);
				}
				StaticListPool<ArchetypeInstance>.ReturnToPool(inspectionTransaction.Instances);
				inspectionTransaction.Instances = null;
			}
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x0010EBF0 File Offset: 0x0010CDF0
		[NetworkRPC(RpcType.ServerToClient)]
		public void InspectNotification(string sourceName)
		{
			if (this.m_netEntity.IsServer)
			{
				this.InspectNotification_Internal(sourceName);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsBlocked(sourceName))
				{
					return;
				}
				string content = string.IsNullOrEmpty(sourceName) ? "You are being inspected." : ZString.Format<string, string>("You are being inspected by <link=\"{0}\">{1}</link>.", "playerName", sourceName);
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
			}
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x0010EC68 File Offset: 0x0010CE68
		private void SendMailRequest_Internal(SendMailTransaction sendMailTransaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1435172201);
			sendMailTransaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x0010ECA8 File Offset: 0x0010CEA8
		private void SendMailResponse_Internal(SendMailResponse sendMailResponse)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-58209770);
			sendMailResponse.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x0010ECE8 File Offset: 0x0010CEE8
		private void AcceptMailRequest_Internal(string mailId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-928962019);
			rpcBuffer.AddString(mailId);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x0010ED28 File Offset: 0x0010CF28
		private void AcceptMailResponse_Internal(OpCodes opCode, string message)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(761166128);
			rpcBuffer.AddEnum(opCode);
			rpcBuffer.AddString(message);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x0010ED70 File Offset: 0x0010CF70
		[NetworkRPC(RpcType.ClientToServer)]
		public void SendMailRequest(SendMailTransaction sendMailTransaction)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SendMailRequest_Internal(sendMailTransaction);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				SendMailResponse sendMailResponse = new SendMailResponse
				{
					OpCode = OpCodes.Error,
					Error = "Unknown"
				};
				if (base.GameEntity.IsTrial)
				{
					sendMailResponse.Error = "You must purchase the game in order to send mail.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				if (sendMailTransaction.Recipient == base.GameEntity.CharacterData.CharacterId)
				{
					sendMailResponse.Error = "Invalid recipient.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				ContainerInstance containerInstance;
				if (base.GameEntity.CollectionController == null || !base.GameEntity.CollectionController.TryGetInstance(ContainerType.PostOutgoing, out containerInstance))
				{
					sendMailResponse.Error = "Failed to send mail. Unknown collection error.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				InteractiveMailbox interactiveMailbox;
				if (base.GameEntity.CollectionController.InteractiveStation == null || !base.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveMailbox))
				{
					sendMailResponse.Error = "Failed to send mail. Not at a mail box.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				CharacterRecord characterRecord = CharacterRecord.Load(ExternalGameDatabase.Database, sendMailTransaction.Recipient.Value);
				if (characterRecord == null)
				{
					sendMailResponse.Error = "Invalid recipient.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				if (this.m_playerCollectionController == null || this.m_playerCollectionController.Record == null)
				{
					sendMailResponse.Error = "Unknown error.";
					this.SendMailResponse(sendMailResponse);
					return;
				}
				List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
				fromPool.AddRange(containerInstance.Instances);
				ulong postage = Mail.GetPostage(base.GameEntity, containerInstance.Instances);
				ulong? currencyAttachment = sendMailTransaction.CurrencyAttachment;
				ulong num = 0UL;
				ulong currencyToRemove = (currencyAttachment.GetValueOrDefault() > num & currencyAttachment != null) ? (postage + sendMailTransaction.CurrencyAttachment.Value) : postage;
				if (interactiveMailbox.TryRemoveCurrency(base.GameEntity, currencyToRemove))
				{
					double value = (ServerGameManager.MailConfig != null) ? ((double)ServerGameManager.MailConfig.PostExpirationInDays) : GlobalSettings.Values.Social.PostExpirationInDays;
					Mail mail = new Mail
					{
						Type = MailType.Post,
						Created = DateTime.UtcNow,
						Expires = new DateTime?(DateTime.UtcNow.AddDays(value)),
						Sender = base.GameEntity.CollectionController.Record.Id,
						Recipient = characterRecord.Id,
						Subject = sendMailTransaction.Subject,
						Message = sendMailTransaction.Message,
						ItemAttachments = fromPool,
						CurrencyAttachment = new ulong?(sendMailTransaction.CurrencyAttachment.GetValueOrDefault()),
						CashOnDelivery = sendMailTransaction.CashOnDelivery,
						Returned = new bool?(false)
					};
					try
					{
						mail.StoreNew(ExternalGameDatabase.Database);
					}
					catch (Exception arg)
					{
						sendMailResponse.Error = "Failed to save mail to the database.";
						this.SendMailResponse(sendMailResponse);
						Debug.LogError(string.Format("Failed to save mail to the database! {0}", arg));
						return;
					}
					bool flag = base.GameEntity.CollectionController.Record.UpdateStorage(ExternalGameDatabase.Database);
					sendMailResponse.OpCode = OpCodes.Ok;
					sendMailResponse.MailId = mail._id;
					sendMailResponse.Recipient = mail.Recipient;
					sendMailResponse.ItemAttachments = fromPool;
					this.SendMailResponse(sendMailResponse);
					containerInstance.DestroyContents();
					if (!flag)
					{
						Debug.LogWarning("Failed to update the storage record for " + mail.Recipient + " during mail sending!");
					}
					try
					{
						PlayerRpcHandler.m_mailCurrencyArguments[0] = "SendMail";
						PlayerRpcHandler.m_mailCurrencyArguments[1] = base.GameEntity.User.Id;
						PlayerRpcHandler.m_mailCurrencyArguments[2] = base.GameEntity.CollectionController.Record.Id;
						PlayerRpcHandler.m_mailCurrencyArguments[3] = base.GameEntity.CollectionController.Record.Name;
						PlayerRpcHandler.m_mailCurrencyArguments[4] = postage;
						PlayerRpcHandler.m_mailCurrencyArguments[5] = fromPool.Count;
						SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName} spent {@Currency} to send mail with {@AttachmentCount} attachments", PlayerRpcHandler.m_mailCurrencyArguments);
						goto IL_400;
					}
					catch
					{
						goto IL_400;
					}
				}
				sendMailResponse.Error = "Not enough money to cover postage and/or sent funds.";
				this.SendMailResponse(sendMailResponse);
				IL_400:
				StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
			}
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x0010F1A0 File Offset: 0x0010D3A0
		[NetworkRPC(RpcType.ServerToClient)]
		public void SendMailResponse(SendMailResponse sendMailResponse)
		{
			if (this.m_netEntity.IsServer)
			{
				this.SendMailResponse_Internal(sendMailResponse);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (sendMailResponse.OpCode == OpCodes.Ok)
				{
					ContainerInstance containerInstance;
					if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.TryGetInstance(ContainerType.PostOutgoing, out containerInstance))
					{
						containerInstance.DestroyContents();
					}
					if (SocialManager.PendingOutgoingMail == null)
					{
						Debug.LogWarning("No pending outgoing mail??");
					}
					else if (base.GameEntity.CharacterData)
					{
						Mail pendingOutgoingMail = SocialManager.PendingOutgoingMail;
						pendingOutgoingMail._id = sendMailResponse.MailId;
						CharacterIdentification characterIdentification;
						if (ClientGameManager.SocialManager.TryGetPlayerIdentById(new UniqueId(sendMailResponse.Recipient), out characterIdentification))
						{
							pendingOutgoingMail.Recipient = characterIdentification.Name;
						}
						pendingOutgoingMail.Sender = base.GameEntity.CharacterData.Name.Value;
						pendingOutgoingMail.ItemAttachments = sendMailResponse.ItemAttachments;
						ClientGameManager.SocialManager.EnqueueMail(new Mail[]
						{
							pendingOutgoingMail
						});
						SocialManager.PendingOutgoingMail = null;
					}
					else
					{
						Debug.LogWarning("No Character data??");
					}
				}
				else
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, sendMailResponse.Error);
				}
				ClientGameManager.UIManager.MailboxUI.UnlockComposeForm();
			}
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x0010F2D8 File Offset: 0x0010D4D8
		[NetworkRPC(RpcType.ClientToServer)]
		public void AcceptMailRequest(string mailId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.AcceptMailRequest_Internal(mailId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				Mail mail = Mail.Load(ExternalGameDatabase.Database, mailId);
				if (mail == null)
				{
					this.AcceptMailResponse(OpCodes.Error, "Unable to accept mail, invalid mail ID provided.");
					return;
				}
				if (mail.Expires != null)
				{
					DateTime t = mail.Expires.Value.AddSeconds(-30.0);
					if (DateTime.UtcNow >= t)
					{
						this.AcceptMailResponse(OpCodes.Error, "Unable to accept mail as it is expired!");
						return;
					}
				}
				if ((mail.ItemAttachments == null || mail.ItemAttachments.Count <= 0) && (mail.CurrencyAttachment == null || mail.CurrencyAttachment.Value <= 0UL))
				{
					this.AcceptMailResponse(OpCodes.Error, "Unable to accept mail, no attachments to accept.");
					return;
				}
				List<ItemArchetype> fromPool = StaticListPool<ItemArchetype>.GetFromPool();
				if (mail.ItemAttachments != null)
				{
					for (int i = 0; i < mail.ItemAttachments.Count; i++)
					{
						ItemArchetype item;
						if (mail.ItemAttachments[i].Archetype && mail.ItemAttachments[i].Archetype.TryGetAsType(out item))
						{
							fromPool.Add(item);
						}
					}
				}
				CannotAcquireReason cannotAcquireReason;
				bool flag = base.GameEntity.EntityCanAcquire(fromPool, out cannotAcquireReason);
				StaticListPool<ItemArchetype>.ReturnToPool(fromPool);
				if (flag && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.InteractiveStation != null)
				{
					InteractiveMailbox interactiveMailbox = base.GameEntity.CollectionController.InteractiveStation as InteractiveMailbox;
					ContainerInstance containerInstance;
					if (interactiveMailbox != null && base.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance))
					{
						ulong? cashOnDelivery = mail.CashOnDelivery;
						List<ArchetypeInstance> itemAttachments = mail.ItemAttachments;
						ulong? currencyAttachment = mail.CurrencyAttachment;
						if (cashOnDelivery != null)
						{
							if (interactiveMailbox.TryRemoveCurrency(base.GameEntity, cashOnDelivery.Value))
							{
								mail.CashOnDelivery = null;
								Mail mail2 = new Mail
								{
									Type = MailType.Post,
									Sender = "000000000000000000000000",
									Recipient = mail.Sender,
									Message = "Thank you for using our cash-on-delivery service. You may find the sum of your recent transaction enclosed.",
									Subject = "Cash-On-Delivery",
									CurrencyAttachment = new ulong?(cashOnDelivery.Value),
									Created = DateTime.UtcNow,
									Expires = new DateTime?(DateTime.UtcNow.AddDays(GlobalSettings.Values.Social.PostExpirationInDays))
								};
								try
								{
									mail2.StoreNew(ExternalGameDatabase.Database);
									goto IL_2B5;
								}
								catch (Exception ex)
								{
									Debug.LogError(string.Format("Failed to save cash-on-delivery response mail to database: {0}", ex.ToString()));
									goto IL_2B5;
								}
							}
							this.AcceptMailResponse(OpCodes.Error, "Unable to accept mail. COD error.");
							return;
						}
						IL_2B5:
						mail.CashOnDelivery = null;
						mail.ItemAttachments = null;
						mail.CurrencyAttachment = null;
						if (mail.UpdateAttachmentsAndCod(ExternalGameDatabase.Database))
						{
							if (itemAttachments != null)
							{
								for (int j = 0; j < itemAttachments.Count; j++)
								{
									base.GameEntity.CollectionController.AddItemInstanceToPlayer(itemAttachments[j], ItemAddContext.Post, -1, false);
								}
							}
							if (currencyAttachment != null && currencyAttachment.Value > 0UL)
							{
								CurrencyTransaction currencyTransaction = new CurrencyTransaction
								{
									Add = true,
									Amount = currencyAttachment.Value,
									TargetContainer = ContainerType.Inventory.ToString(),
									Message = "post",
									Context = CurrencyContext.Post
								};
								CurrencyDistributor.AddCurrencyToEntity(base.GameEntity, ref currencyTransaction);
							}
							this.AcceptMailResponse(OpCodes.Ok, mail._id);
							return;
						}
						mail.CashOnDelivery = cashOnDelivery;
						mail.ItemAttachments = itemAttachments;
						mail.CurrencyAttachment = currencyAttachment;
						Debug.LogError("Failed to save mail attachments to database.");
						this.AcceptMailResponse(OpCodes.Error, "Unable to accept mail, database connection failure.");
						return;
					}
				}
				string errorOrId;
				ContainerInstance containerInstance2;
				if (!flag)
				{
					string text;
					if (cannotAcquireReason != CannotAcquireReason.NoRoom)
					{
						if (cannotAcquireReason != CannotAcquireReason.Dead)
						{
							text = "Cannot receive item for unknown reason";
						}
						else
						{
							text = "Cannot receive while missing your bag";
						}
					}
					else
					{
						text = "Not enough room in inventory";
					}
					errorOrId = text;
				}
				else if (base.GameEntity.CollectionController == null)
				{
					errorOrId = "Unable to accept mail, internal collection error.";
				}
				else if (base.GameEntity.CollectionController.InteractiveStation == null || base.GameEntity.CollectionController.InteractiveStation is InteractiveMailbox)
				{
					errorOrId = "Unable to accept mail, not at a mail box.";
				}
				else if (!base.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance2))
				{
					errorOrId = "Unable to accept mail, internal container error.";
				}
				else
				{
					errorOrId = "Insufficient funds to cover Cash-On-Delivery charge.";
				}
				this.AcceptMailResponse(OpCodes.Error, errorOrId);
			}
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x0010F76C File Offset: 0x0010D96C
		[NetworkRPC(RpcType.ServerToClient)]
		public void AcceptMailResponse(OpCodes opCode, string errorOrId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.AcceptMailResponse_Internal(opCode, errorOrId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (opCode == OpCodes.Ok)
				{
					Mail mail;
					if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.TryGetInboxItemById(errorOrId, out mail))
					{
						ContainerInstance containerInstance;
						if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.TryGetInstance(ContainerType.PostIncoming, out containerInstance))
						{
							containerInstance.DestroyContents();
						}
						mail.ItemAttachments = null;
						mail.CurrencyAttachment = null;
						mail.CashOnDelivery = null;
						if (ClientGameManager.UIManager.MailboxUI.MailDetail.gameObject.activeInHierarchy)
						{
							ClientGameManager.UIManager.MailboxUI.MailDetail.Init(mail);
							ClientGameManager.UIManager.MailboxUI.UpdateInboxListWhenReady();
							return;
						}
					}
				}
				else
				{
					string content = string.IsNullOrEmpty(errorOrId) ? "Unknown error." : errorOrId;
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
			}
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x0010F870 File Offset: 0x0010DA70
		private void LogArmstrong_Internal(int armstrongCount, int totalCount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1771114288);
			rpcBuffer.AddInt(armstrongCount);
			rpcBuffer.AddInt(totalCount);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x00055947 File Offset: 0x00053B47
		[NetworkRPC(RpcType.ClientToServer)]
		public void LogArmstrong(int armstrongCount, int totalCount)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.LogArmstrong_Internal(armstrongCount, totalCount);
				return;
			}
			bool isServer = this.m_netEntity.IsServer;
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x0010F8B8 File Offset: 0x0010DAB8
		private void SendReport_Internal(LongString report)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1408766942);
			report.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x0005596B File Offset: 0x00053B6B
		[NetworkRPC(RpcType.ClientToServer)]
		public void SendReport(LongString report)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SendReport_Internal(report);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.StartCoroutine(this.SendReportInternal(report));
			}
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x0005599D File Offset: 0x00053B9D
		private IEnumerator SendReportInternal(LongString report)
		{
			yield return null;
			yield break;
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x000559A5 File Offset: 0x00053BA5
		private IEnumerator StuckReportToSlack()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x00045BC3 File Offset: 0x00043DC3
		private string GetUserNameForReport()
		{
			return string.Empty;
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x0010F8F8 File Offset: 0x0010DAF8
		private void Server_AuctionHouse_AuctionList_Internal(AuctionList allListings)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1598352336);
			allListings.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x0010F938 File Offset: 0x0010DB38
		private void SendAuctionResponse_Internal(AuctionResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1998790690);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x0010F978 File Offset: 0x0010DB78
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_AuctionHouse_AuctionList(AuctionList allListings)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_AuctionHouse_AuctionList_Internal(allListings);
				return;
			}
			if (this.m_netEntity.IsLocal && ClientGameManager.UIManager && ClientGameManager.UIManager.AuctionHouseUI)
			{
				ClientGameManager.UIManager.AuctionHouseUI.UpdateAuctionList(allListings.Auctions);
			}
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x0010F9DC File Offset: 0x0010DBDC
		[NetworkRPC(RpcType.ServerToClient)]
		public void SendAuctionResponse(AuctionResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.SendAuctionResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (response.OpCode == OpCodes.Ok)
				{
					ContainerInstance containerInstance;
					if (response.DestroyContents && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.TryGetInstance(ContainerType.AuctionOutgoing, out containerInstance))
					{
						containerInstance.DestroyContents();
					}
					if (!string.IsNullOrEmpty(response.Message))
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, response.Message);
						return;
					}
				}
				else
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, response.Message);
				}
			}
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x0010FA78 File Offset: 0x0010DC78
		private void SendAuctionError(string err)
		{
			AuctionResponse response = new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = err
			};
			this.SendAuctionResponse(response);
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0010FAB0 File Offset: 0x0010DCB0
		private bool AllowAuctionHouseUse(out ContainerInstance auctionOutgoing, out InteractiveAuctionHouse interactiveAuctionHouse)
		{
			auctionOutgoing = null;
			interactiveAuctionHouse = null;
			if (!this.m_netEntity.IsServer)
			{
				return false;
			}
			if (ServerGameManager.AuctionHouseManager == null)
			{
				this.SendAuctionError("Invalid Auction House");
				return false;
			}
			if (base.GameEntity.IsTrial)
			{
				this.SendAuctionError("You must purchase the game to use the auction house.");
				return false;
			}
			if (base.GameEntity.User == null)
			{
				this.SendAuctionError("Invalid User.");
				return false;
			}
			if (base.GameEntity.CollectionController == null || !base.GameEntity.CollectionController.TryGetInstance(ContainerType.AuctionOutgoing, out auctionOutgoing))
			{
				this.SendAuctionError("Unknown collection error.");
				return false;
			}
			if (base.GameEntity.CollectionController.InteractiveStation == null || !base.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveAuctionHouse))
			{
				this.SendAuctionError("Not at an Auction House");
				return false;
			}
			return true;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x0010FB90 File Offset: 0x0010DD90
		private void Client_AuctionHouse_NewAuction_Internal(AuctionRequest auctionRequest)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1050879209);
			auctionRequest.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x0010FBD0 File Offset: 0x0010DDD0
		private void Client_AuctionHouse_PlaceBid_Internal(string auctionId, ulong newBid)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(339243336);
			rpcBuffer.AddString(auctionId);
			rpcBuffer.AddULong(newBid);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x0010FC18 File Offset: 0x0010DE18
		private void Client_AuctionHouse_BuyItNow_Internal(string auctionId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1019783178);
			rpcBuffer.AddString(auctionId);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x0010FC58 File Offset: 0x0010DE58
		private void Client_AuctionHouse_CancelAuction_Internal(string auctionId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1008334374);
			rpcBuffer.AddString(auctionId);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x0010FC98 File Offset: 0x0010DE98
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_AuctionHouse_NewAuction(AuctionRequest auctionRequest)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_AuctionHouse_NewAuction_Internal(auctionRequest);
				return;
			}
			ContainerInstance auctionOutgoing;
			InteractiveAuctionHouse interactiveAuctionHouse;
			if (this.AllowAuctionHouseUse(out auctionOutgoing, out interactiveAuctionHouse))
			{
				AuctionResponse response = ServerGameManager.AuctionHouseManager.NewAuction(base.GameEntity, auctionOutgoing, interactiveAuctionHouse, ref auctionRequest);
				this.SendAuctionResponse(response);
			}
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x0010FCE4 File Offset: 0x0010DEE4
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_AuctionHouse_PlaceBid(string auctionId, ulong newBid)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_AuctionHouse_PlaceBid_Internal(auctionId, newBid);
				return;
			}
			ContainerInstance containerInstance;
			InteractiveAuctionHouse interactiveAuctionHouse;
			if (this.AllowAuctionHouseUse(out containerInstance, out interactiveAuctionHouse))
			{
				AuctionResponse response = ServerGameManager.AuctionHouseManager.PlaceBid(base.GameEntity, interactiveAuctionHouse, auctionId, newBid);
				this.SendAuctionResponse(response);
			}
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x0010FD30 File Offset: 0x0010DF30
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_AuctionHouse_BuyItNow(string auctionId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_AuctionHouse_BuyItNow_Internal(auctionId);
				return;
			}
			ContainerInstance containerInstance;
			InteractiveAuctionHouse interactiveAuctionHouse;
			if (this.AllowAuctionHouseUse(out containerInstance, out interactiveAuctionHouse))
			{
				AuctionResponse response = ServerGameManager.AuctionHouseManager.BuyItNow(base.GameEntity, interactiveAuctionHouse, auctionId);
				this.SendAuctionResponse(response);
			}
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x0010FD78 File Offset: 0x0010DF78
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_AuctionHouse_CancelAuction(string auctionId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_AuctionHouse_CancelAuction_Internal(auctionId);
				return;
			}
			ContainerInstance containerInstance;
			InteractiveAuctionHouse interactiveAuctionHouse;
			if (this.AllowAuctionHouseUse(out containerInstance, out interactiveAuctionHouse))
			{
				AuctionResponse response = ServerGameManager.AuctionHouseManager.CancelAuction(base.GameEntity, auctionId);
				this.SendAuctionResponse(response);
			}
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x0010FDC0 File Offset: 0x0010DFC0
		private void TransferRequest_Internal(TransferRequest request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-493513046);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF7 RID: 7159 RVA: 0x0010FE00 File Offset: 0x0010E000
		private void SwapRequest_Internal(SwapRequest request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(59918634);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x0010FE40 File Offset: 0x0010E040
		private void TakeAllRequest_Internal(TakeAllRequest request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1547673192);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x0010FE80 File Offset: 0x0010E080
		private void TransferRequestResponse_Internal(TransferResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1736144391);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x0010FEC0 File Offset: 0x0010E0C0
		private void SwapRequestResponse_Internal(SwapResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(323744441);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x0010FF00 File Offset: 0x0010E100
		private void TakeAllRequestResponse_Internal(TakeAllResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1890873849);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x0010FF40 File Offset: 0x0010E140
		private void DestroyItemRequest_Internal(ItemDestructionTransaction request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1329992561);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BFD RID: 7165 RVA: 0x0010FF80 File Offset: 0x0010E180
		private void DestroyItemRequestResponse_Internal(ItemDestructionTransaction request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1103831550);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x0010FFC0 File Offset: 0x0010E1C0
		private void DestroyMultiItemRequest_Internal(ItemMultiDestructionTransaction request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(992505585);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001BFF RID: 7167 RVA: 0x00110000 File Offset: 0x0010E200
		private void DestroyMultiItemRequestResponse_Internal(ItemMultiDestructionTransaction request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2142754784);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x00110040 File Offset: 0x0010E240
		private void AddItemResponse_Internal(ArchetypeAddedTransaction response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(734614732);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x00110080 File Offset: 0x0010E280
		private void AddRemoveItems_Internal(ArchetypeAddRemoveTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2026643213);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x001100C0 File Offset: 0x0010E2C0
		private void UpdateItemCount_Internal(ItemCountUpdatedTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-504071272);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x00110100 File Offset: 0x0010E300
		private void LearnablesAdded_Internal(LearnablesAddedTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1001766302);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00110140 File Offset: 0x0010E340
		private void MerchantItemSellRequest_Internal(UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(236011962);
			itemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(sourceContainerType);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x00110188 File Offset: 0x0010E388
		private void MerchantPurchaseRequest_Internal(UniqueId itemId, uint quantity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1782128149);
			itemId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(quantity);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x001101D0 File Offset: 0x0010E3D0
		private void MerchantBuybackUpdateRequest_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2014142238);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x00110208 File Offset: 0x0010E408
		private void MerchantBuybackRequest_Internal(UniqueId instanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1751600441);
			instanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x00110248 File Offset: 0x0010E448
		private void BlacksmithItemRepairRequest_Internal(UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(567746561);
			itemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(sourceContainerType);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C09 RID: 7177 RVA: 0x00110290 File Offset: 0x0010E490
		private void BlacksmithContainerRepairRequest_Internal(ContainerType sourceContainerType)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1588305323);
			rpcBuffer.AddEnum(sourceContainerType);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x001102D0 File Offset: 0x0010E4D0
		private void DeconstructRequest_Internal(UniqueId itemInstanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(682403610);
			itemInstanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x00110310 File Offset: 0x0010E510
		private void DeconstructResponse_Internal(ItemDestructionTransaction destroyTransaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(217864364);
			destroyTransaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x00110350 File Offset: 0x0010E550
		private void LearnAbilityRequest_Internal(UniqueId abilityId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-154907426);
			abilityId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x00110390 File Offset: 0x0010E590
		private void TrainSpecializationRequest_Internal(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2096302217);
			masteryInstanceId.PackData(rpcBuffer);
			specializationArchetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x001103DC File Offset: 0x0010E5DC
		private void ForgetSpecializationRequest_Internal(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1924536384);
			masteryInstanceId.PackData(rpcBuffer);
			specializationArchetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x00110428 File Offset: 0x0010E628
		private void SplitRequest_Internal(SplitRequest request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1196278984);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x00110468 File Offset: 0x0010E668
		private void SplitRequestResponse_Internal(SplitResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1505132217);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x001104A8 File Offset: 0x0010E6A8
		private void MergeRequest_Internal(MergeRequest request)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-510747324);
			request.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x001104E8 File Offset: 0x0010E6E8
		private void MergeRequestResponse_Internal(MergeResponse response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1149873811);
			response.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00110528 File Offset: 0x0010E728
		private void MerchantInventoryUpdate_Internal(MerchantType merchantType, ForSaleItemIds forSaleItemIds)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(357992779);
			rpcBuffer.AddEnum(merchantType);
			forSaleItemIds.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x00110570 File Offset: 0x0010E770
		private void MerchantBuybackInventoryUpdate_Internal(MerchantType merchantType, BuybackItemData buybackItemData)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-87533323);
			rpcBuffer.AddEnum(merchantType);
			buybackItemData.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x001105B8 File Offset: 0x0010E7B8
		private void MerchantItemSellResponse_Internal(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType, ulong sellPrice)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-889718254);
			rpcBuffer.AddEnum(op);
			itemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(sourceContainerType);
			rpcBuffer.AddULong(sellPrice);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x00110614 File Offset: 0x0010E814
		private void BlacksmithItemRepairResponse_Internal(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2127818826);
			rpcBuffer.AddEnum(op);
			itemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(sourceContainerType);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x00110664 File Offset: 0x0010E864
		private void BlacksmithContainerRepairResponse_Internal(OpCodes op, ContainerType sourceContainerType)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1278363166);
			rpcBuffer.AddEnum(op);
			rpcBuffer.AddEnum(sourceContainerType);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x001106AC File Offset: 0x0010E8AC
		private void TrainSpecializationResponse_Internal(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId, float specLevel)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(35437109);
			rpcBuffer.AddEnum(op);
			masteryInstanceId.PackData(rpcBuffer);
			specializationArchetypeId.PackData(rpcBuffer);
			rpcBuffer.AddFloat(specLevel);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x00110708 File Offset: 0x0010E908
		private void ForgetSpecializationResponse_Internal(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1108426403);
			rpcBuffer.AddEnum(op);
			masteryInstanceId.PackData(rpcBuffer);
			specializationArchetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x0011075C File Offset: 0x0010E95C
		private void PurchaseContainerExpansionRequest_Internal(string containerId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1359816363);
			rpcBuffer.AddString(containerId);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x0011079C File Offset: 0x0010E99C
		private void PurchaseContainerExpansionResponse_Internal(PurchaseContainerExpansionTransaction transaction)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(489358183);
			transaction.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x001107DC File Offset: 0x0010E9DC
		private void CacheTransaction(UniqueId transactionId, ITransaction transaction)
		{
			if (this.m_transactions == null)
			{
				this.m_transactions = new Dictionary<UniqueId, ITransaction>(default(UniqueIdComparer));
			}
			this.m_transactions.Add(transactionId, transaction);
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x00110818 File Offset: 0x0010EA18
		[NetworkRPC(RpcType.ClientToServer)]
		public void MergeRequest(MergeRequest request)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessMergeRequest(request);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.CacheTransaction(request.TransactionId, request);
				this.MergeRequest_Internal(request);
			}
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x00110868 File Offset: 0x0010EA68
		[NetworkRPC(RpcType.ClientToServer)]
		public void SplitRequest(SplitRequest request)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessSplitRequest(request);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.CacheTransaction(request.TransactionId, request);
				this.SplitRequest_Internal(request);
			}
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000559AD File Offset: 0x00053BAD
		private bool AutoCancelLootItem()
		{
			return !this.CanLootItem();
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x001108B8 File Offset: 0x0010EAB8
		private bool CanLootItem()
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			return this.m_lootConfirmationInstance != null && this.m_lootConfirmationInstance.ContainerInstance != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(this.m_lootConfirmationInstance.ContainerInstance.ContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(this.m_lootConfirmationInstance.InstanceId, out archetypeInstance);
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x0011092C File Offset: 0x0010EB2C
		[NetworkRPC(RpcType.ClientToServer)]
		public void TransferRequest(TransferRequest request)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessTransferRequest(request);
				return;
			}
			if (request.Instance != null && request.Instance.ContainerInstance != null && request.Instance.ContainerInstance.ContainerType == ContainerType.Loot && request.Instance.ItemData != null && request.Instance.ItemData.IsNoTrade)
			{
				string title = "No Trade Loot";
				string arg = ArchetypeInstanceUI.kNoTrade;
				if (request.Instance.ItemData.IsSoulbound)
				{
					title = "Soulbound Loot";
					arg = ArchetypeInstanceUI.kSoulbound;
				}
				string text = ZString.Format<string>("This item is marked as {0}! Are you sure you want to loot it? Once looted you can no longer trade this item.", arg);
				this.m_lootConfirmationInstance = request.Instance;
				DialogOptions opts = new DialogOptions
				{
					Title = title,
					Text = text,
					ConfirmationText = "Yes",
					CancelText = "NO",
					Instance = request.Instance,
					AutoCancel = new Func<bool>(this.AutoCancelLootItem),
					Callback = delegate(bool answer, object obj)
					{
						if (answer && this.CanLootItem())
						{
							request.Instance = null;
							this.m_lootConfirmationInstance = null;
							this.TransferRequest(request);
							return;
						}
						if (request.Instance != null && request.Instance.InstanceUI != null)
						{
							request.Instance.InstanceUI.ResetUI();
						}
						request.Instance = null;
						this.m_lootConfirmationInstance = null;
					}
				};
				ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
				return;
			}
			this.CacheTransaction(request.TransactionId, request);
			this.TransferRequest_Internal(request);
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000559B8 File Offset: 0x00053BB8
		[NetworkRPC(RpcType.ClientToServer)]
		public void SwapRequest(SwapRequest request)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessSwapRequest(request);
				return;
			}
			this.CacheTransaction(request.TransactionId, request);
			this.SwapRequest_Internal(request);
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x000559ED File Offset: 0x00053BED
		[NetworkRPC(RpcType.ClientToServer)]
		public void TakeAllRequest(TakeAllRequest request)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessTakeAllRequest(request);
				return;
			}
			this.CacheTransaction(request.TransactionId, request);
			this.TakeAllRequest_Internal(request);
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x00055A22 File Offset: 0x00053C22
		[NetworkRPC(RpcType.ClientToServer)]
		public void DestroyItemRequest(ItemDestructionTransaction request)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DestroyItemRequest_Internal(request);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessItemDestructionRequest(request);
			}
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x00055A52 File Offset: 0x00053C52
		[NetworkRPC(RpcType.ClientToServer)]
		public void DestroyMultiItemRequest(ItemMultiDestructionTransaction request)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DestroyMultiItemRequest_Internal(request);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessItemMultiDestructionRequest(request);
			}
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x00055A82 File Offset: 0x00053C82
		[NetworkRPC(RpcType.ClientToServer)]
		public void MerchantItemSellRequest(UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.MerchantItemSellRequest_Internal(itemInstanceId, sourceContainerType);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessMerchantItemSellRequest(itemInstanceId, sourceContainerType);
			}
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x00110AD0 File Offset: 0x0010ECD0
		[NetworkRPC(RpcType.ClientToServer)]
		public void MerchantBuybackUpdateRequest()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.MerchantBuybackUpdateRequest_Internal();
				return;
			}
			InteractiveMerchant interactiveMerchant;
			if (this.m_netEntity.IsServer && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null && base.GameEntity.CollectionController.InteractiveStation != null && base.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveMerchant))
			{
				MerchantType merchantType = interactiveMerchant.MerchantType;
				List<MerchantBuybackItem> items = (merchantType == MerchantType.Standard) ? base.GameEntity.CollectionController.Record.MerchantBuybackItems : base.GameEntity.CollectionController.Record.BagBuybackItems;
				this.MerchantBuybackInventoryUpdate(merchantType, new BuybackItemData
				{
					Items = items
				});
			}
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x00110BA8 File Offset: 0x0010EDA8
		[NetworkRPC(RpcType.ClientToServer)]
		public void MerchantPurchaseRequest(UniqueId itemId, uint quantity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.MerchantPurchaseRequest_Internal(itemId, quantity);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (base.GameEntity.CollectionController.InteractiveStation == null)
				{
					this.SendChatNotification("Invalid interactive station!");
					return;
				}
				InteractiveMerchant interactiveMerchant = base.GameEntity.CollectionController.InteractiveStation as InteractiveMerchant;
				if (interactiveMerchant == null)
				{
					this.SendChatNotification("Not an interactive merchant!");
					return;
				}
				interactiveMerchant.PurchaseRequest(base.GameEntity, itemId, quantity);
			}
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x00110C38 File Offset: 0x0010EE38
		[NetworkRPC(RpcType.ClientToServer)]
		public void MerchantBuybackRequest(UniqueId instanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.MerchantBuybackRequest_Internal(instanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (base.GameEntity.CollectionController.InteractiveStation == null)
				{
					this.SendChatNotification("Invalid interactive station!");
					return;
				}
				InteractiveMerchant interactiveMerchant = base.GameEntity.CollectionController.InteractiveStation as InteractiveMerchant;
				if (interactiveMerchant == null)
				{
					this.SendChatNotification("Not an interactive merchant!");
					return;
				}
				this.m_playerCollectionController.ProcessMerchantBuybackRequest(interactiveMerchant, instanceId);
			}
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x00110CC4 File Offset: 0x0010EEC4
		[NetworkRPC(RpcType.ClientToServer)]
		public void BlacksmithItemRepairRequest(UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.BlacksmithItemRepairRequest_Internal(itemInstanceId, sourceContainerType);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (base.GameEntity.CollectionController.InteractiveStation == null)
				{
					this.SendChatNotification("Invalid interactive station!");
					return;
				}
				InteractiveBlacksmith interactiveBlacksmith = base.GameEntity.CollectionController.InteractiveStation as InteractiveBlacksmith;
				if (interactiveBlacksmith == null)
				{
					this.SendChatNotification("Not an interactive blacksmith!");
					return;
				}
				interactiveBlacksmith.ItemRepairRequest(base.GameEntity, itemInstanceId, sourceContainerType);
			}
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x00110D54 File Offset: 0x0010EF54
		[NetworkRPC(RpcType.ClientToServer)]
		public void BlacksmithContainerRepairRequest(ContainerType sourceContainerType)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.BlacksmithContainerRepairRequest_Internal(sourceContainerType);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (base.GameEntity.CollectionController.InteractiveStation == null)
				{
					this.SendChatNotification("Invalid interactive station!");
					return;
				}
				InteractiveBlacksmith interactiveBlacksmith = base.GameEntity.CollectionController.InteractiveStation as InteractiveBlacksmith;
				if (interactiveBlacksmith == null)
				{
					this.SendChatNotification("Not an interactive blacksmith!");
					return;
				}
				interactiveBlacksmith.ContainerRepairRequest(base.GameEntity, sourceContainerType);
			}
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x00055AB4 File Offset: 0x00053CB4
		[NetworkRPC(RpcType.ClientToServer)]
		public void DeconstructRequest(UniqueId itemInstanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.DeconstructRequest_Internal(itemInstanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessDeconstructRequest(itemInstanceId);
			}
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x00055AE4 File Offset: 0x00053CE4
		[NetworkRPC(RpcType.ClientToServer)]
		public void LearnAbilityRequest(UniqueId abilityId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.LearnAbilityRequest_Internal(abilityId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessLearnAbilityRequest(abilityId);
			}
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x00055B14 File Offset: 0x00053D14
		[NetworkRPC(RpcType.ClientToServer)]
		public void TrainSpecializationRequest(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.TrainSpecializationRequest_Internal(masteryInstanceId, specializationArchetypeId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessTrainSpecializationRequest(masteryInstanceId, specializationArchetypeId);
			}
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x00055B46 File Offset: 0x00053D46
		[NetworkRPC(RpcType.ClientToServer)]
		public void ForgetSpecializationRequest(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ForgetSpecializationRequest_Internal(masteryInstanceId, specializationArchetypeId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessForgetSpecializationRequest(masteryInstanceId, specializationArchetypeId);
			}
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x00055B78 File Offset: 0x00053D78
		[NetworkRPC(RpcType.ClientToServer)]
		public void PurchaseContainerExpansionRequest(string containerId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.PurchaseContainerExpansionRequest_Internal(containerId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.m_playerCollectionController.ProcessPurchaseContainerExpansionRequest(containerId);
			}
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x00110DE0 File Offset: 0x0010EFE0
		[NetworkRPC(RpcType.ServerToClient)]
		public void MergeRequestResponse(MergeResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.MergeRequestResponse_Internal(response);
				return;
			}
			Debug.Log("MergeRequestResponse! " + response.TransactionId.ToString() + " --> " + response.Op.ToString());
			ITransaction transaction;
			if (this.m_transactions.TryGetValue(response.TransactionId, out transaction))
			{
				if (response.Op == OpCodes.Ok)
				{
					this.m_playerCollectionController.ProcessMergeResponse((MergeRequest)transaction, response);
				}
				this.m_transactions.Remove(response.TransactionId);
			}
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x00110E7C File Offset: 0x0010F07C
		[NetworkRPC(RpcType.ServerToClient)]
		public void SplitRequestResponse(SplitResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.SplitRequestResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				Debug.Log("SplitRequestResponse! " + response.TransactionId.ToString() + " --> " + response.Op.ToString());
				ITransaction transaction;
				if (this.m_transactions.TryGetValue(response.TransactionId, out transaction))
				{
					if (response.Op == OpCodes.Ok)
					{
						this.m_playerCollectionController.ProcessSplitResponse((SplitRequest)transaction, response);
					}
					this.m_transactions.Remove(response.TransactionId);
				}
			}
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x00110F28 File Offset: 0x0010F128
		[NetworkRPC(RpcType.ServerToClient)]
		public void TransferRequestResponse(TransferResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.TransferRequestResponse_Internal(response);
				return;
			}
			Debug.Log("TransferRequestResponse! " + response.TransactionId.ToString() + " --> " + response.Op.ToString());
			ITransaction transaction;
			if (this.m_transactions.TryGetValue(response.TransactionId, out transaction))
			{
				this.m_transactions.Remove(response.TransactionId);
				this.m_playerCollectionController.ProcessTransferResponse((TransferRequest)transaction, response);
			}
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x00110FBC File Offset: 0x0010F1BC
		[NetworkRPC(RpcType.ServerToClient)]
		public void SwapRequestResponse(SwapResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.SwapRequestResponse_Internal(response);
				return;
			}
			Debug.Log("SwapRequestResponse! " + response.TransactionId.ToString() + " --> " + response.Op.ToString());
			ITransaction transaction;
			if (this.m_transactions.TryGetValue(response.TransactionId, out transaction))
			{
				this.m_transactions.Remove(response.TransactionId);
				this.m_playerCollectionController.ProcessSwapResponse((SwapRequest)transaction, response);
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x00111050 File Offset: 0x0010F250
		[NetworkRPC(RpcType.ServerToClient)]
		public void TakeAllRequestResponse(TakeAllResponse response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.TakeAllRequestResponse_Internal(response);
				return;
			}
			Debug.Log("TakeAllRequestResponse! " + response.TransactionId.ToString() + " --> " + response.Op.ToString());
			if (response.TransactionId.IsEmpty)
			{
				this.m_playerCollectionController.ProcessTakeAllResponse(default(TakeAllRequest), response);
				return;
			}
			ITransaction transaction;
			if (this.m_transactions.TryGetValue(response.TransactionId, out transaction))
			{
				this.m_transactions.Remove(response.TransactionId);
				this.m_playerCollectionController.ProcessTakeAllResponse((TakeAllRequest)transaction, response);
			}
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00055BA8 File Offset: 0x00053DA8
		[NetworkRPC(RpcType.ServerToClient)]
		public void DestroyItemRequestResponse(ItemDestructionTransaction response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.DestroyItemRequestResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessItemDestructionResponse(response);
			}
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x00055BD8 File Offset: 0x00053DD8
		[NetworkRPC(RpcType.ServerToClient)]
		public void DestroyMultiItemRequestResponse(ItemMultiDestructionTransaction response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.DestroyMultiItemRequestResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessItemMultiDestructionResponse(response);
			}
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x00055C08 File Offset: 0x00053E08
		[NetworkRPC(RpcType.ServerToClient)]
		public void AddItemResponse(ArchetypeAddedTransaction response)
		{
			if (this.m_netEntity.IsServer)
			{
				this.AddItemResponse_Internal(response);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessItemAdded(response);
			}
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x00055C38 File Offset: 0x00053E38
		[NetworkRPC(RpcType.ServerToClient)]
		public void AddRemoveItems(ArchetypeAddRemoveTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.AddRemoveItems_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessAddRemoveItems(transaction);
			}
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x00055C68 File Offset: 0x00053E68
		[NetworkRPC(RpcType.ServerToClient)]
		public void UpdateItemCount(ItemCountUpdatedTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.UpdateItemCount_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessItemCountUpdated(transaction);
			}
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x00055C98 File Offset: 0x00053E98
		[NetworkRPC(RpcType.ServerToClient)]
		public void LearnablesAdded(LearnablesAddedTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.LearnablesAdded_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessLearnablesAdded(transaction);
			}
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x00055CC8 File Offset: 0x00053EC8
		[NetworkRPC(RpcType.ServerToClient)]
		public void MerchantInventoryUpdate(MerchantType merchantType, ForSaleItemIds forSaleItemIds)
		{
			if (this.m_netEntity.IsServer)
			{
				this.MerchantInventoryUpdate_Internal(merchantType, forSaleItemIds);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.UIManager.MerchantUI.UpdateForSaleItems(merchantType, forSaleItemIds);
			}
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x00055CFE File Offset: 0x00053EFE
		[NetworkRPC(RpcType.ServerToClient)]
		public void MerchantBuybackInventoryUpdate(MerchantType merchantType, BuybackItemData buybackItemData)
		{
			if (this.m_netEntity.IsServer)
			{
				this.MerchantBuybackInventoryUpdate_Internal(merchantType, buybackItemData);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.UIManager.MerchantUI.UpdateBuybackItems(merchantType, buybackItemData);
			}
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x00055D34 File Offset: 0x00053F34
		[NetworkRPC(RpcType.ServerToClient)]
		public void MerchantItemSellResponse(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType, ulong sellPrice)
		{
			if (this.m_netEntity.IsServer)
			{
				this.MerchantItemSellResponse_Internal(op, itemInstanceId, sourceContainerType, sellPrice);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessMerchantItemSellResponse(op, itemInstanceId, sourceContainerType, sellPrice);
			}
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x00055D6C File Offset: 0x00053F6C
		[NetworkRPC(RpcType.ServerToClient)]
		public void BlacksmithItemRepairResponse(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			if (this.m_netEntity.IsServer)
			{
				this.BlacksmithItemRepairResponse_Internal(op, itemInstanceId, sourceContainerType);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessBlacksmithItemRepairResponse(op, itemInstanceId, sourceContainerType);
			}
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x00055DA0 File Offset: 0x00053FA0
		[NetworkRPC(RpcType.ServerToClient)]
		public void BlacksmithContainerRepairResponse(OpCodes op, ContainerType sourceContainerType)
		{
			if (this.m_netEntity.IsServer)
			{
				this.BlacksmithContainerRepairResponse_Internal(op, sourceContainerType);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessBlacksmithContainerRepairResponse(op, sourceContainerType);
			}
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x00111110 File Offset: 0x0010F310
		[NetworkRPC(RpcType.ServerToClient)]
		public void TrainSpecializationResponse(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId, float specLevel)
		{
			if (this.m_netEntity.IsServer)
			{
				this.TrainSpecializationResponse_Internal(op, masteryInstanceId, specializationArchetypeId, specLevel);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessTrainSpecializationResponse(op, masteryInstanceId, specializationArchetypeId, specLevel);
				base.GameEntity.CharacterData.RefreshRole();
			}
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x00111164 File Offset: 0x0010F364
		[NetworkRPC(RpcType.ServerToClient)]
		public void ForgetSpecializationResponse(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ForgetSpecializationResponse_Internal(op, masteryInstanceId, specializationArchetypeId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessForgetSpecializationResponse(op, masteryInstanceId, specializationArchetypeId);
				base.GameEntity.CharacterData.RefreshRole();
			}
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00055DD2 File Offset: 0x00053FD2
		[NetworkRPC(RpcType.ServerToClient)]
		public void PurchaseContainerExpansionResponse(PurchaseContainerExpansionTransaction transaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.PurchaseContainerExpansionResponse_Internal(transaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessContainerExpansionResponse(transaction);
			}
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x00055E02 File Offset: 0x00054002
		[NetworkRPC(RpcType.ServerToClient)]
		public void DeconstructResponse(ItemDestructionTransaction destructionTransaction)
		{
			if (this.m_netEntity.IsServer)
			{
				this.DeconstructResponse_Internal(destructionTransaction);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.ProcessItemDestructionRequest(destructionTransaction);
			}
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x001111B4 File Offset: 0x0010F3B4
		private void OpenRemoteContainer_Internal(NetworkEntity interactionSource, ContainerRecord record)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(802438285);
			rpcBuffer.AddUInt(interactionSource.NetworkId.Value);
			record.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C46 RID: 7238 RVA: 0x00055E32 File Offset: 0x00054032
		[NetworkRPC(RpcType.ServerToClient)]
		public void OpenRemoteContainer(NetworkEntity interactionSource, ContainerRecord record)
		{
			if (this.m_netEntity.IsServer)
			{
				this.OpenRemoteContainer_Internal(interactionSource, record);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.OpenRemoteContainer(interactionSource.GameEntity.Interactive, record);
			}
		}

		// Token: 0x06001C47 RID: 7239 RVA: 0x00111208 File Offset: 0x0010F408
		private void UpdateArchetypeInstanceLock_Internal(string containerId, UniqueId instanceId, bool lockState)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1376374012);
			rpcBuffer.AddString(containerId);
			instanceId.PackData(rpcBuffer);
			rpcBuffer.AddBool(lockState);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x00055E6E File Offset: 0x0005406E
		[NetworkRPC(RpcType.ServerToClient)]
		public void UpdateArchetypeInstanceLock(string containerId, UniqueId instanceId, bool lockState)
		{
			if (this.m_netEntity.IsServer)
			{
				this.UpdateArchetypeInstanceLock_Internal(containerId, instanceId, lockState);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.UpdateArchetypeInstanceLock(containerId, instanceId, lockState);
			}
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x00111258 File Offset: 0x0010F458
		private void ToggleReagent_Internal(int index, bool value)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1056514907);
			rpcBuffer.AddInt(index);
			rpcBuffer.AddBool(value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x00055EA2 File Offset: 0x000540A2
		[NetworkRPC(RpcType.ClientToServer)]
		public void ToggleReagent(int index, bool value)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.ToggleReagent_Internal(index, value);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ContainerInstance reagentPouch = ((ICollectionController)this.m_playerCollectionController).ReagentPouch;
				if (reagentPouch == null)
				{
					return;
				}
				reagentPouch.SetToggle(index, value);
			}
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x001112A0 File Offset: 0x0010F4A0
		private void Client_RequestExecuteUtility_Internal(ContainerType sourceContainerType, UniqueId sourceItemInstanceId, ContainerType targetContainerType, UniqueId targetItemInstanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-237529261);
			rpcBuffer.AddEnum(sourceContainerType);
			sourceItemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(targetContainerType);
			targetItemInstanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x001112FC File Offset: 0x0010F4FC
		private void Server_ExecuteUtilityResponse_Internal(OpCodes op, UniqueId sourceArchetypeId, ContainerType targetContainerType, UniqueId targetItemInstanceId, bool consumed)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1810121394);
			rpcBuffer.AddEnum(op);
			sourceArchetypeId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(targetContainerType);
			targetItemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddBool(consumed);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x00111360 File Offset: 0x0010F560
		private void Server_UpdateClientAugment_Internal(ContainerType sourceContainerType, UniqueId itemInstanceId, AugmentUpdateInfo update)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(623011239);
			rpcBuffer.AddEnum(sourceContainerType);
			itemInstanceId.PackData(rpcBuffer);
			update.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x001113B4 File Offset: 0x0010F5B4
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_RequestExecuteUtility(ContainerType sourceContainerType, UniqueId sourceItemInstanceId, ContainerType targetContainerType, UniqueId targetItemInstanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_RequestExecuteUtility_Internal(sourceContainerType, sourceItemInstanceId, targetContainerType, targetItemInstanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ContainerInstance containerInstance;
				ArchetypeInstance archetypeInstance;
				IUtilityItem utilityItem;
				ContainerInstance containerInstance2;
				ArchetypeInstance targetInstance;
				if (this.m_netEntity.GameEntity.CollectionController.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(sourceItemInstanceId, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out utilityItem) && this.m_netEntity.GameEntity.CollectionController.TryGetInstance(targetContainerType, out containerInstance2) && containerInstance2.TryGetInstanceForInstanceId(targetItemInstanceId, out targetInstance))
				{
					utilityItem.ExecuteUtility(this.m_netEntity.GameEntity, archetypeInstance, targetInstance);
					return;
				}
				this.Server_ExecuteUtilityResponse(OpCodes.Error, UniqueId.Empty, targetContainerType, targetItemInstanceId, false);
			}
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x0011146C File Offset: 0x0010F66C
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_ExecuteUtilityResponse(OpCodes op, UniqueId sourceArchetypeId, ContainerType targetContainerType, UniqueId targetItemInstanceId, bool consumed)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_ExecuteUtilityResponse_Internal(op, sourceArchetypeId, targetContainerType, targetItemInstanceId, consumed);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				IUtilityItem utilityItem;
				ContainerInstance containerInstance;
				ArchetypeInstance targetInstance;
				if (op == OpCodes.Ok && InternalGameDatabase.Archetypes.TryGetAsType<IUtilityItem>(sourceArchetypeId, out utilityItem) && this.m_netEntity.GameEntity.CollectionController.TryGetInstance(targetContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(targetItemInstanceId, out targetInstance))
				{
					utilityItem.ExecuteUtility(this.m_netEntity.GameEntity, null, targetInstance);
					if (consumed || utilityItem.ResetCursorGameMode)
					{
						CursorManager.ResetGameMode();
						return;
					}
				}
				else
				{
					Debug.Log("Failure!");
				}
			}
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x0011150C File Offset: 0x0010F70C
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_UpdateClientAugment(ContainerType sourceContainerType, UniqueId itemInstanceId, AugmentUpdateInfo update)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_UpdateClientAugment_Internal(sourceContainerType, itemInstanceId, update);
				return;
			}
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (this.m_netEntity.IsLocal && this.m_playerCollectionController.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance) && archetypeInstance.ItemData != null && archetypeInstance.ItemData.Augment != null)
			{
				if (update.Expired)
				{
					archetypeInstance.ItemData.Augment = null;
					base.GameEntity.Vitals.RefreshAugmentStats(true);
					if (archetypeInstance.Archetype != null)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Your augment on " + archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance) + " has worn off!");
					}
				}
				else
				{
					archetypeInstance.ItemData.Augment.Count = update.Count;
					archetypeInstance.ItemData.Augment.StackCount = update.StackCount;
				}
				archetypeInstance.ItemData.TriggerAugmentChanged();
			}
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x00111610 File Offset: 0x0010F810
		private void AssignEmberStone_Internal(UniqueId? stoneId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1663417641);
			rpcBuffer.AddNullableUniqueId(stoneId);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x00111650 File Offset: 0x0010F850
		private void UpdateEmberEssenceCount_Internal(int updatedCount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1155165155);
			rpcBuffer.AddInt(updatedCount);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00111690 File Offset: 0x0010F890
		private void UpdateEmberEssenceCountForTravel_Internal(int updatedCount, int updatedTravelCount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1147091225);
			rpcBuffer.AddInt(updatedCount);
			rpcBuffer.AddInt(updatedTravelCount);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x001116D8 File Offset: 0x0010F8D8
		private void PurchaseTravelEssence_Internal(int amount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-932029047);
			rpcBuffer.AddInt(amount);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x00111718 File Offset: 0x0010F918
		[NetworkRPC(RpcType.ServerToClient)]
		public void AssignEmberStone(UniqueId? stoneId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.AssignEmberStone_Internal(stoneId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				EmberStone currentEmberStone;
				if (stoneId != null && InternalGameDatabase.Archetypes.TryGetAsType<EmberStone>(stoneId.Value, out currentEmberStone))
				{
					this.m_playerCollectionController.CurrentEmberStone = currentEmberStone;
					return;
				}
				this.m_playerCollectionController.CurrentEmberStone = null;
			}
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x00055EDE File Offset: 0x000540DE
		[NetworkRPC(RpcType.ServerToClient)]
		public void UpdateEmberEssenceCount(int updatedCount)
		{
			if (this.m_netEntity.IsServer)
			{
				this.UpdateEmberEssenceCount_Internal(updatedCount);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.SetEmberEssenceCount(updatedCount);
			}
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x00055F0E File Offset: 0x0005410E
		[NetworkRPC(RpcType.ServerToClient)]
		public void UpdateEmberEssenceCountForTravel(int updatedCount, int updatedTravelCount)
		{
			if (this.m_netEntity.IsServer)
			{
				this.UpdateEmberEssenceCountForTravel_Internal(updatedCount, updatedTravelCount);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.m_playerCollectionController.SetEmberEssenceCountForTravel(updatedCount, updatedTravelCount);
			}
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x00111780 File Offset: 0x0010F980
		[NetworkRPC(RpcType.ClientToServer)]
		public void PurchaseTravelEssence(int amount)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.PurchaseTravelEssence_Internal(amount);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				InteractiveEssenceConverter interactiveEssenceConverter;
				if (base.GameEntity.CollectionController == null || base.GameEntity.CollectionController.InteractiveStation == null || !base.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveEssenceConverter))
				{
					this.SendChatNotification("Invalid merchant!");
					return;
				}
				interactiveEssenceConverter.PurchaseRequest(base.GameEntity, amount);
			}
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x00111808 File Offset: 0x0010FA08
		private void IncrementHuntingLog_Internal(UniqueId profileId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2060107345);
			profileId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x00111848 File Offset: 0x0010FA48
		private void SelectHuntingLogPerk_Internal(UniqueId profileId, HuntingLogPerkType perkType, int threshold)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-313479755);
			profileId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(perkType);
			rpcBuffer.AddInt(threshold);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x00111898 File Offset: 0x0010FA98
		private void ConfirmHuntingLogPerk_Internal(OpCodes op, UniqueId profileId, HuntingLogPerkType perkType, int threshold)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(168958990);
			rpcBuffer.AddEnum(op);
			profileId.PackData(rpcBuffer);
			rpcBuffer.AddEnum(perkType);
			rpcBuffer.AddInt(threshold);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x001118F4 File Offset: 0x0010FAF4
		private void RespecHuntingLogRequest_Internal(UniqueId profileId, int backToThreshold)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-20754910);
			profileId.PackData(rpcBuffer);
			rpcBuffer.AddInt(backToThreshold);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x0011193C File Offset: 0x0010FB3C
		private void RespecHuntingLogResponse_Internal(OpCodes op, UniqueId profileId, int backToThreshold)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1399018367);
			rpcBuffer.AddEnum(op);
			profileId.PackData(rpcBuffer);
			rpcBuffer.AddInt(backToThreshold);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x0011198C File Offset: 0x0010FB8C
		[NetworkRPC(RpcType.ServerToClient)]
		public void IncrementHuntingLog(UniqueId profileId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.IncrementHuntingLog_Internal(profileId);
				return;
			}
			HuntingLogProfile profile;
			if (this.m_netEntity.IsLocal && InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(profileId, out profile))
			{
				this.m_playerCollectionController.IncrementHuntingLog(profile, 0);
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x001119D8 File Offset: 0x0010FBD8
		[NetworkRPC(RpcType.ClientToServer)]
		public void SelectHuntingLogPerk(UniqueId profileId, HuntingLogPerkType perkType, int threshold)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.SelectHuntingLogPerk_Internal(profileId, perkType, threshold);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				HuntingLogProfile huntingLogProfile;
				HuntingLogEntry huntingLogEntry;
				if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(profileId, out huntingLogProfile) && this.m_playerCollectionController.Record != null && this.m_playerCollectionController.Record.HuntingLog != null && this.m_playerCollectionController.Record.HuntingLog.TryGetValue(huntingLogProfile.Id, out huntingLogEntry) && huntingLogProfile.IsValidPerk(huntingLogEntry, perkType, threshold))
				{
					if (huntingLogEntry.ActivePerks == null)
					{
						huntingLogEntry.ActivePerks = new Dictionary<int, HuntingLogPerkType>();
					}
					else if (huntingLogEntry.ActivePerks.ContainsKey(threshold))
					{
						this.ConfirmHuntingLogPerk(OpCodes.Error, huntingLogProfile.Id, perkType, threshold);
						return;
					}
					huntingLogEntry.ActivePerks.Add(threshold, perkType);
					huntingLogEntry.CacheCombatPerks();
					this.ConfirmHuntingLogPerk(OpCodes.Ok, huntingLogProfile.Id, perkType, threshold);
					return;
				}
				this.ConfirmHuntingLogPerk(OpCodes.Error, huntingLogProfile.Id, perkType, threshold);
			}
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x00111AD4 File Offset: 0x0010FCD4
		[NetworkRPC(RpcType.ServerToClient)]
		public void ConfirmHuntingLogPerk(OpCodes op, UniqueId profileId, HuntingLogPerkType perkType, int threshold)
		{
			if (this.m_netEntity.IsServer)
			{
				this.ConfirmHuntingLogPerk_Internal(op, profileId, perkType, threshold);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				HuntingLogEntry huntingLogEntry;
				if (op == OpCodes.Ok && this.m_playerCollectionController.Record != null && this.m_playerCollectionController.Record.HuntingLog != null && this.m_playerCollectionController.Record.HuntingLog.TryGetValue(profileId, out huntingLogEntry))
				{
					if (huntingLogEntry.ActivePerks == null)
					{
						huntingLogEntry.ActivePerks = new Dictionary<int, HuntingLogPerkType>();
					}
					huntingLogEntry.ActivePerks.AddOrReplace(threshold, perkType);
					huntingLogEntry.CacheCombatPerks();
					if (perkType.IsTitle())
					{
						TitleManager.InvokeTitlesChangedEvent();
					}
				}
				this.m_playerCollectionController.InvokeHuntingLogEntryModified();
			}
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00111B84 File Offset: 0x0010FD84
		[NetworkRPC(RpcType.ClientToServer)]
		public void RespecHuntingLogRequest(UniqueId profileId, int backToThreshold)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.RespecHuntingLogRequest_Internal(profileId, backToThreshold);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				OpCodes op = this.RespecHuntingLogForThreshold(profileId, backToThreshold) ? OpCodes.Ok : OpCodes.Error;
				this.RespecHuntingLogResponse(op, profileId, backToThreshold);
			}
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x00055F40 File Offset: 0x00054140
		[NetworkRPC(RpcType.ServerToClient)]
		public void RespecHuntingLogResponse(OpCodes op, UniqueId profileId, int backToThreshold)
		{
			if (this.m_netEntity.IsServer)
			{
				this.RespecHuntingLogResponse_Internal(op, profileId, backToThreshold);
				return;
			}
			if (this.m_netEntity.IsLocal && op == OpCodes.Ok)
			{
				this.RespecHuntingLogForThreshold(profileId, backToThreshold);
				this.m_playerCollectionController.InvokeHuntingLogEntryModified();
			}
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x00111BD0 File Offset: 0x0010FDD0
		private bool RespecHuntingLogForThreshold(UniqueId profileId, int backToThreshold)
		{
			HuntingLogProfile huntingLogProfile;
			HuntingLogEntry huntingLogEntry;
			if (!profileId.IsEmpty && backToThreshold > 0 && InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(profileId, out huntingLogProfile) && this.m_playerCollectionController.Record != null && this.m_playerCollectionController.Record.HuntingLog != null && this.m_playerCollectionController.Record.HuntingLog.TryGetValue(huntingLogProfile.Id, out huntingLogEntry) && huntingLogEntry.PerkCount >= backToThreshold && huntingLogEntry.ActivePerks != null && huntingLogEntry.ActivePerks.Count > 0)
			{
				bool flag = false;
				List<int> fromPool = StaticListPool<int>.GetFromPool();
				foreach (KeyValuePair<int, HuntingLogPerkType> keyValuePair in huntingLogEntry.ActivePerks)
				{
					if (keyValuePair.Key >= backToThreshold)
					{
						fromPool.Add(keyValuePair.Key);
						if (keyValuePair.Value.IsTitle())
						{
							flag = true;
						}
					}
				}
				int count = fromPool.Count;
				for (int i = 0; i < count; i++)
				{
					huntingLogEntry.ActivePerks.Remove(fromPool[i]);
				}
				StaticListPool<int>.ReturnToPool(fromPool);
				if (count > 0)
				{
					huntingLogEntry.PerkCount = huntingLogProfile.GetNextLowerTierCount(backToThreshold);
					huntingLogEntry.CacheCombatPerks();
					if (flag)
					{
						TitleManager.InvokeTitlesChangedEvent();
					}
				}
				return count > 0;
			}
			return false;
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x00111D3C File Offset: 0x0010FF3C
		private void NotifyBBClear_Internal(OpCodes opCode)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1256085718);
			rpcBuffer.AddEnum(opCode);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x00055F7E File Offset: 0x0005417E
		[NetworkRPC(RpcType.ServerToClient)]
		public void NotifyBBClear(OpCodes opCode)
		{
			if (this.m_netEntity.IsServer)
			{
				this.NotifyBBClear_Internal(opCode);
				return;
			}
			if (opCode == OpCodes.Ok)
			{
				GameManager.QuestManager.ResetTasks(base.GameEntity);
				this.SendChatNotification("Bulletin Board tasks have been reset!");
			}
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00111D7C File Offset: 0x0010FF7C
		private void Client_Execution_Instant_Internal(ClientExecutionCache clientExecutionCache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1243190222);
			clientExecutionCache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x00111DBC File Offset: 0x0010FFBC
		private void Client_Execution_Begin_Internal(DateTime timestamp, ClientExecutionCache clientExecutionCache)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(768073826);
			rpcBuffer.AddDateTime(timestamp);
			clientExecutionCache.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00111E04 File Offset: 0x00110004
		private void Client_Execution_Cancel_Internal(UniqueId archetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1890319226);
			archetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x00111E44 File Offset: 0x00110044
		private void Client_Execution_Complete_Internal(UniqueId archetypeId, DateTime timestamp)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-723486673);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddDateTime(timestamp);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00111E8C File Offset: 0x0011008C
		private void Client_DismissEffectRequest_Internal(UniqueId instanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(876927945);
			instanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x00111ECC File Offset: 0x001100CC
		private void Client_Execute_AutoAttack_Internal(NetworkEntity targetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-412412423);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00111F18 File Offset: 0x00110118
		private void Client_DismissActiveAura_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1010266720);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x00055FB4 File Offset: 0x000541B4
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_Execution_Instant(ClientExecutionCache clientExecutionCache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_Execution_Instant_Internal(clientExecutionCache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.SkillsController.Server_Execute_Instant(clientExecutionCache);
			}
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00055FE9 File Offset: 0x000541E9
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_Execution_Begin(DateTime timestamp, ClientExecutionCache clientExecutionCache)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_Execution_Begin_Internal(timestamp, clientExecutionCache);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.SkillsController.Server_Execution_Begin(timestamp, clientExecutionCache);
			}
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x00056020 File Offset: 0x00054220
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_Execution_Cancel(UniqueId archetypeId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_Execution_Cancel_Internal(archetypeId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.SkillsController.Server_Execution_Cancel(archetypeId);
			}
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x00056055 File Offset: 0x00054255
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_Execution_Complete(UniqueId archetypeId, DateTime timestamp)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_Execution_Complete_Internal(archetypeId, timestamp);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.SkillsController.Server_Execution_Complete(archetypeId, timestamp);
			}
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x0005608C File Offset: 0x0005428C
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_DismissEffectRequest(UniqueId instanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_DismissEffectRequest_Internal(instanceId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.EffectController.DismissEffectRequest(instanceId);
			}
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000560C1 File Offset: 0x000542C1
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_Execute_AutoAttack(NetworkEntity targetEntity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_Execute_AutoAttack_Internal(targetEntity);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.SkillsController.Server_Execute_AutoAttack(targetEntity);
			}
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000560F6 File Offset: 0x000542F6
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_DismissActiveAura()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_DismissActiveAura_Internal();
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				base.GameEntity.EffectController.SourceAura = null;
			}
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x00111F50 File Offset: 0x00110150
		private void Server_Execute_Instant_Internal(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1175005518);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			rpcBuffer.AddByte(abilityLevel);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x00111FB0 File Offset: 0x001101B0
		private void Server_Execute_Instant_Failed_Internal(string message)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(573824098);
			rpcBuffer.AddString(message);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x00111FF0 File Offset: 0x001101F0
		private void Server_Execution_Begin_Internal(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1432660161);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			rpcBuffer.AddByte(abilityLevel);
			rpcBuffer.AddEnum(alchemyPowerLevel);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001C77 RID: 7287 RVA: 0x00112058 File Offset: 0x00110258
		private void Server_Execution_BeginFailed_Internal(UniqueId archetypeId, string message)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(534092910);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddString(message);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x001120A0 File Offset: 0x001102A0
		private void Server_Execution_Cancel_Internal(UniqueId archetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-972748282);
			archetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x001120E0 File Offset: 0x001102E0
		private void Server_Execution_Complete_Internal(UniqueId archetypeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-897782463);
			archetypeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x00112120 File Offset: 0x00110320
		private void Server_Execution_Complete_UpdateTarget_Internal(UniqueId archetypeId, NetworkEntity updatedTargetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(284955617);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(updatedTargetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x00112178 File Offset: 0x00110378
		private void Server_MasteryLevelChanged_Internal(UniqueId archetypeId, float newLevel)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2066966389);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddFloat(newLevel);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x001121C0 File Offset: 0x001103C0
		private void Server_MasteryAbilityLevelChanged_Internal(InstanceNewLevelData newLevelData)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-998007269);
			newLevelData.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x00112200 File Offset: 0x00110400
		private void Server_LevelProgressionEvent_Internal(LevelProgressionEvent levelProgressionEvent)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1437771947);
			levelProgressionEvent.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x00112240 File Offset: 0x00110440
		private void Server_LevelProgressionUpdate_Internal(LevelProgressionUpdate levelProgressionUpdate)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-121022329);
			levelProgressionUpdate.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x00112280 File Offset: 0x00110480
		private void Server_Execute_AutoAttack_Failed_Internal(string message)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1521387168);
			rpcBuffer.AddString(message);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x0005612A File Offset: 0x0005432A
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execute_Instant(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execute_Instant_Internal(archetypeId, targetEntity, abilityLevel);
				return;
			}
			base.GameEntity.SkillsController.Client_Execute_Instant(archetypeId, targetEntity, abilityLevel);
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x00056156 File Offset: 0x00054356
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_Execute_Instant_Failed(string message)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execute_Instant_Failed_Internal(message);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				base.GameEntity.SkillsController.Client_Execute_Instant_Failed(message);
			}
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x0005618B File Offset: 0x0005438B
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execution_Begin(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execution_Begin_Internal(archetypeId, targetEntity, abilityLevel, alchemyPowerLevel);
				return;
			}
			base.GameEntity.SkillsController.Client_Execution_Begin(archetypeId, targetEntity, abilityLevel, alchemyPowerLevel);
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x000561BB File Offset: 0x000543BB
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_Execution_BeginFailed(UniqueId archetypeId, string message)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execution_BeginFailed_Internal(archetypeId, message);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				base.GameEntity.SkillsController.Client_BeginExecution_Failed(archetypeId, message);
			}
		}

		// Token: 0x06001C84 RID: 7300 RVA: 0x000561F2 File Offset: 0x000543F2
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execution_Cancel(UniqueId archetypeId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execution_Cancel_Internal(archetypeId);
				return;
			}
			base.GameEntity.SkillsController.Client_Execution_Cancelled(archetypeId);
		}

		// Token: 0x06001C85 RID: 7301 RVA: 0x0005621A File Offset: 0x0005441A
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execution_Complete(UniqueId archetypeId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execution_Complete_Internal(archetypeId);
				return;
			}
			base.GameEntity.SkillsController.Client_Execution_Complete(archetypeId, null);
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x00056243 File Offset: 0x00054443
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execution_Complete_UpdateTarget(UniqueId archetypeId, NetworkEntity updatedTargetEntity)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execution_Complete_UpdateTarget_Internal(archetypeId, updatedTargetEntity);
				return;
			}
			base.GameEntity.SkillsController.Client_Execution_Complete(archetypeId, updatedTargetEntity);
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x001122C0 File Offset: 0x001104C0
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_MasteryLevelChanged(UniqueId masteryArchetypeId, float newLevel)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_MasteryLevelChanged_Internal(masteryArchetypeId, newLevel);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (base.GameEntity.SkillsController)
				{
					base.GameEntity.SkillsController.MasteryLevelChanged(masteryArchetypeId, newLevel);
				}
				MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
			}
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x00112320 File Offset: 0x00110520
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_MasteryAbilityLevelChanged(InstanceNewLevelData newLevelData)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_MasteryAbilityLevelChanged_Internal(newLevelData);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				if (base.GameEntity.SkillsController)
				{
					base.GameEntity.SkillsController.MasteryAbilityLevelChanged(newLevelData);
				}
				if (base.GameEntity.CharacterData)
				{
					base.GameEntity.CharacterData.UpdateHighestMasteryLevel(newLevelData);
				}
			}
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x00112398 File Offset: 0x00110598
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_LevelProgressionEvent(LevelProgressionEvent levelProgressionEvent)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_LevelProgressionEvent_Internal(levelProgressionEvent);
				return;
			}
			if (this.m_netEntity.IsLocal && base.GameEntity.SkillsController)
			{
				base.GameEntity.SkillsController.LevelProgressionEvent(levelProgressionEvent);
			}
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x001123EC File Offset: 0x001105EC
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_LevelProgressionUpdate(LevelProgressionUpdate levelProgressionUpdate)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_LevelProgressionUpdate_Internal(levelProgressionUpdate);
				return;
			}
			if (this.m_netEntity.IsLocal && base.GameEntity.SkillsController)
			{
				base.GameEntity.SkillsController.LevelProgressionUpdate(levelProgressionUpdate);
			}
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x0005626D File Offset: 0x0005446D
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_Execute_AutoAttack_Failed(string message)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execute_AutoAttack_Failed_Internal(message);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				base.GameEntity.SkillsController.ExecuteAutoAttackFailed(message);
			}
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetMasteryLevel_Internal(UniqueId masteryInstanceId, float level)
		{
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetTargetMasteryLevel_Internal(string target, string masteryName, float level)
		{
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Kill_Internal(NetworkEntity entity, bool rewardExperience)
		{
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Heal_Internal(NetworkEntity entity)
		{
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_HealStamina_Internal(NetworkEntity entity)
		{
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_HealWounds_Internal(NetworkEntity entity)
		{
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddCurrency_Internal(ulong currency)
		{
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddEventCurrency_Internal(ulong currency)
		{
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddNetworkEntityEventCurrency_Internal(NetworkEntity networkEntity, ulong currency)
		{
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddTargetEventCurrency_Internal(string targetName, ulong currency)
		{
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetQuests_Internal()
		{
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetQuest_Internal(UniqueId questId)
		{
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetTargetQuest_Internal(string targetName, UniqueId questId)
		{
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetNpcKnowledge_Internal()
		{
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Learn_Internal(string label, UniqueId profileId)
		{
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Unlearn_Internal(string label, UniqueId profileId)
		{
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetLearnables_Internal()
		{
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetDiscoveries_Internal()
		{
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetZoneDiscoveries_Internal()
		{
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_DiscoverZone_Internal()
		{
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AdjustPlayerFlags_Internal(PlayerFlags flags, bool adding)
		{
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x00112440 File Offset: 0x00110640
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetMasteryLevel(UniqueId masteryInstanceId, float level)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetMasteryLevel_Internal(masteryInstanceId, level);
				return;
			}
			ArchetypeInstance archetypeInstance;
			if (this.m_netEntity.IsServer && base.GameEntity.CollectionController.Masteries != null && base.GameEntity.CollectionController.Masteries.TryGetInstanceForInstanceId(masteryInstanceId, out archetypeInstance))
			{
				level = Mathf.Clamp(level, 1f, 50f);
				float associatedLevel = archetypeInstance.GetAssociatedLevel(base.GameEntity);
				archetypeInstance.MasteryData.BaseLevel = level;
				LevelProgressionUpdate levelProgressionUpdate = new LevelProgressionUpdate
				{
					ArchetypeId = archetypeInstance.ArchetypeId,
					BaseLevel = level
				};
				if (archetypeInstance.MasteryData.Specialization != null)
				{
					float num = Mathf.Clamp(level, 6f, 50f);
					archetypeInstance.MasteryData.SpecializationLevel = num;
					levelProgressionUpdate.SpecializationLevel = new float?(num);
				}
				base.GameEntity.Vitals.MasteryLevelChanged(archetypeInstance, associatedLevel, level);
				base.GameEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance);
				MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
				this.Server_LevelProgressionUpdate(levelProgressionUpdate);
			}
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x00112568 File Offset: 0x00110768
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetTargetMasteryLevel(string targetName, string masteryName, float level)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetTargetMasteryLevel_Internal(targetName, masteryName, level);
				return;
			}
			if (this.m_netEntity.IsServer && this.m_netEntity.GameEntity.GM)
			{
				level = Mathf.Clamp(level, 1f, 50f);
				NetworkEntity networkEntity;
				if (!ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity))
				{
					this.SendChatNotification("Unable to find player by the name of " + targetName);
					return;
				}
				if (masteryName == "all")
				{
					foreach (ArchetypeInstance archetypeInstance in networkEntity.GameEntity.CollectionController.Masteries.Instances)
					{
						float baseLevel = archetypeInstance.MasteryData.BaseLevel;
						archetypeInstance.MasteryData.BaseLevel = level;
						LevelProgressionUpdate levelProgressionUpdate = new LevelProgressionUpdate
						{
							ArchetypeId = archetypeInstance.ArchetypeId,
							BaseLevel = level
						};
						if (archetypeInstance.MasteryData.Specialization != null)
						{
							archetypeInstance.MasteryData.SpecializationLevel = level;
							levelProgressionUpdate.SpecializationLevel = new float?(level);
						}
						networkEntity.GameEntity.Vitals.MasteryLevelChanged(archetypeInstance, baseLevel, level);
						networkEntity.GameEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance);
						networkEntity.PlayerRpcHandler.Server_LevelProgressionUpdate(levelProgressionUpdate);
					}
					this.SendChatNotification(string.Concat(new string[]
					{
						"Changed ",
						networkEntity.GameEntity.CollectionController.Masteries.Count.ToString(),
						" of ",
						networkEntity.GameEntity.CharacterData.Name.Value,
						"'s masteries to ",
						Mathf.FloorToInt(level).ToString()
					}));
					return;
				}
				foreach (ArchetypeInstance archetypeInstance2 in networkEntity.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance2.Archetype.DisplayName.ToLowerInvariant() == masteryName)
					{
						float baseLevel2 = archetypeInstance2.MasteryData.BaseLevel;
						archetypeInstance2.MasteryData.BaseLevel = level;
						networkEntity.GameEntity.Vitals.MasteryLevelChanged(archetypeInstance2, baseLevel2, level);
						LevelProgressionUpdate levelProgressionUpdate2 = new LevelProgressionUpdate
						{
							ArchetypeId = archetypeInstance2.ArchetypeId,
							BaseLevel = level
						};
						if (archetypeInstance2.MasteryData.Specialization != null)
						{
							archetypeInstance2.MasteryData.SpecializationLevel = level;
							levelProgressionUpdate2.SpecializationLevel = new float?(level);
						}
						networkEntity.GameEntity.Vitals.MasteryLevelChanged(archetypeInstance2, baseLevel2, level);
						networkEntity.GameEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance2);
						networkEntity.PlayerRpcHandler.Server_LevelProgressionUpdate(levelProgressionUpdate2);
						this.SendChatNotification(string.Concat(new string[]
						{
							"Changed ",
							networkEntity.GameEntity.CharacterData.Name.Value,
							"'s ",
							archetypeInstance2.Archetype.DisplayName,
							" mastery level from ",
							Mathf.FloorToInt(baseLevel2).ToString(),
							" to ",
							Mathf.FloorToInt(level).ToString()
						}));
						return;
					}
				}
				this.SendChatNotification(networkEntity.GameEntity.CharacterData.Name.Value + " does not have a mastery by the name of " + masteryName);
			}
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x00112928 File Offset: 0x00110B28
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Kill(NetworkEntity killTarget, bool rewardExperience)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Kill_Internal(killTarget, rewardExperience);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && killTarget != null && killTarget.GameEntity != null && killTarget.GameEntity.Vitals != null)
			{
				InteractiveNpc interactiveNpc;
				if (killTarget.GameEntity.Interactive != null && killTarget.GameEntity.Interactive.TryGetAsType(out interactiveNpc))
				{
					interactiveNpc.PreventExperienceDistribution = !rewardExperience;
					interactiveNpc.AddAsTagger(base.GameEntity);
				}
				killTarget.GameEntity.Vitals.AlterHealth(float.MinValue);
			}
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x001129E0 File Offset: 0x00110BE0
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Heal(NetworkEntity healTarget)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Heal_Internal(healTarget);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && healTarget != null)
			{
				healTarget.GameEntity.Vitals.AlterHealth(float.MaxValue);
			}
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x00112A3C File Offset: 0x00110C3C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_HealStamina(NetworkEntity healTarget)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_HealStamina_Internal(healTarget);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && healTarget != null)
			{
				healTarget.GameEntity.Vitals.AlterStamina(float.MaxValue);
			}
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x00112A98 File Offset: 0x00110C98
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_HealWounds(NetworkEntity healTarget)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_HealWounds_Internal(healTarget);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && healTarget != null)
			{
				healTarget.GameEntity.Vitals.AlterHealthWound(float.MinValue);
				healTarget.GameEntity.Vitals.AlterStaminaWound(float.MinValue);
			}
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x00112B08 File Offset: 0x00110D08
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AdjustPlayerFlags(PlayerFlags flags, bool adding)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AdjustPlayerFlags_Internal(flags, adding);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				if (adding)
				{
					base.GameEntity.CharacterData.CharacterFlags.Value |= flags;
					return;
				}
				base.GameEntity.CharacterData.CharacterFlags.Value &= ~flags;
			}
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x00112B84 File Offset: 0x00110D84
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetQuests()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetQuests_Internal();
				GameManager.QuestManager.ResetQuests(base.GameEntity);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				GameManager.QuestManager.ResetQuests(base.GameEntity);
				this.SendChatNotification("All quests reset");
			}
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00112BEC File Offset: 0x00110DEC
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetQuest(UniqueId questId)
		{
			Quest quest2;
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetQuest_Internal(questId);
				Quest quest;
				if (InternalGameDatabase.Quests.TryGetItem(questId, out quest))
				{
					GameManager.QuestManager.ResetQuest(base.GameEntity, quest);
					return;
				}
			}
			else if (this.m_netEntity.IsServer && base.GameEntity.GM && InternalGameDatabase.Quests.TryGetItem(questId, out quest2))
			{
				GameManager.QuestManager.ResetQuest(base.GameEntity, quest2);
				this.SendChatNotification("Quest \"" + quest2.Title + "\" reset");
			}
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x00112C84 File Offset: 0x00110E84
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetTargetQuest(string targetName, UniqueId questId)
		{
			if (this.m_netEntity.IsLocal)
			{
				Quest quest;
				if (base.GameEntity.CharacterData.Name.Value.ToLowerInvariant() == targetName && InternalGameDatabase.Quests.TryGetItem(questId, out quest))
				{
					GameManager.QuestManager.ResetQuest(base.GameEntity, quest);
				}
				this.GM_ResetTargetQuest_Internal(targetName, questId);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				Quest quest2;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity) && InternalGameDatabase.Quests.TryGetItem(questId, out quest2))
				{
					GameManager.QuestManager.ResetQuest(networkEntity.GameEntity, quest2);
					networkEntity.PlayerRpcHandler.NotifyGMQuestReset(questId);
					this.SendChatNotification("Quest \"" + quest2.Title + "\" reset for " + targetName);
					return;
				}
				string text = targetName.ToTitleCase();
				CharacterRecord characterRecord = CharacterRecord.LoadForCharacterName(ExternalGameDatabase.Database, text);
				if (characterRecord == null)
				{
					this.SendChatNotification("Unable to find CharacterRecord for " + text);
					return;
				}
				ZoneId zoneId = (ZoneId)characterRecord.Location.ZoneId;
				Dictionary<string, string> fields = new Dictionary<string, string>
				{
					{
						"TargetName",
						characterRecord.Name
					},
					{
						"QuestId",
						questId.ToString()
					}
				};
				ServerCommunicator.POST(zoneId, "resetTargetQuest", fields, null);
				this.SendChatNotification("Sent ResetTargetQuest request to " + characterRecord.Name + " in " + zoneId.ToString());
			}
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x00112E00 File Offset: 0x00111000
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetNpcKnowledge()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetNpcKnowledge_Internal();
				if (base.GameEntity.CollectionController.Record.Progression != null)
				{
					if (base.GameEntity.CollectionController.Record.Progression.Quests == null)
					{
						base.GameEntity.CollectionController.Record.Progression = null;
					}
					else
					{
						base.GameEntity.CollectionController.Record.Progression.NpcKnowledge = null;
					}
				}
			}
			else if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController.Record.Progression != null)
			{
				if (base.GameEntity.CollectionController.Record.Progression.Quests == null)
				{
					base.GameEntity.CollectionController.Record.Progression = null;
					base.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
				}
				else
				{
					base.GameEntity.CollectionController.Record.Progression.NpcKnowledge = null;
					base.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
				}
			}
			base.GameEntity.CharacterData.ReinitKnowledge();
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x00112F60 File Offset: 0x00111160
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Learn(string label, UniqueId profileId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Learn_Internal(label, profileId);
				return;
			}
			NpcProfile npcProfile;
			if (this.m_netEntity.IsServer && base.GameEntity.GM && InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(profileId, out npcProfile))
			{
				NpcLearningCache cache = new NpcLearningCache
				{
					NpcProfile = npcProfile,
					KnowledgeIndex = Array.IndexOf<string>(npcProfile.KnowledgeLabels, label)
				};
				GameManager.QuestManager.UpdateNpcKnowledge(cache, this.m_netEntity.GameEntity);
				base.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
				GameManager.QuestManager.NotifyLearned(cache, this.m_netEntity.GameEntity);
				this.m_netEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Ok, string.Empty, cache);
				this.SendChatNotification("Learned " + label + "!");
			}
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00113048 File Offset: 0x00111248
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Unlearn(string label, UniqueId profileId)
		{
			NpcProfile npcProfile2;
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Unlearn_Internal(label, profileId);
				NpcProfile npcProfile;
				if (base.GameEntity.CollectionController.Record.Progression != null && base.GameEntity.CollectionController.Record.Progression.NpcKnowledge != null && base.GameEntity.CollectionController.Record.Progression.NpcKnowledge.ContainsKey(profileId) && InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(profileId, out npcProfile))
				{
					int num = Array.IndexOf<string>(npcProfile.KnowledgeLabels, label);
					if (num >= 0)
					{
						base.GameEntity.CollectionController.Record.Progression.NpcKnowledge[npcProfile.Id][num] = false;
						base.GameEntity.CharacterData.ResetLabel(label);
						return;
					}
				}
			}
			else if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController.Record.Progression != null && base.GameEntity.CollectionController.Record.Progression.NpcKnowledge != null && base.GameEntity.CollectionController.Record.Progression.NpcKnowledge.ContainsKey(profileId) && InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(profileId, out npcProfile2))
			{
				int num2 = Array.IndexOf<string>(npcProfile2.KnowledgeLabels, label);
				if (num2 >= 0)
				{
					base.GameEntity.CollectionController.Record.Progression.NpcKnowledge[npcProfile2.Id][num2] = false;
					base.GameEntity.CharacterData.ResetLabel(label);
					base.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					this.SendChatNotification("Unlearned " + label + "!");
				}
			}
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x0011323C File Offset: 0x0011143C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetLearnables()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetLearnables_Internal();
				base.GameEntity.CollectionController.Recipes.Clear();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				base.GameEntity.CollectionController.Recipes.Clear();
				base.GameEntity.CollectionController.Record.UpdateLearnables(ExternalGameDatabase.Database);
			}
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x001132BC File Offset: 0x001114BC
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetDiscoveries()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetDiscoveries_Internal();
				DiscoveryProgression.ResetAllDiscoveries(base.GameEntity);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				DiscoveryProgression.ResetAllDiscoveries(base.GameEntity);
			}
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00113310 File Offset: 0x00111510
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetZoneDiscoveries()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetZoneDiscoveries_Internal();
				DiscoveryProgression.ResetZoneDiscoveries(base.GameEntity);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				DiscoveryProgression.ResetZoneDiscoveries(base.GameEntity);
			}
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00113364 File Offset: 0x00111564
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_DiscoverZone()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_DiscoverZone_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				DiscoveryTrigger[] array = UnityEngine.Object.FindObjectsByType<DiscoveryTrigger>(FindObjectsSortMode.None);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] && array[i].gameObject && array[i].gameObject.activeInHierarchy && array[i].Profile)
					{
						DiscoveryProgression.DiscoverForEntity(base.GameEntity, array[i].Profile);
					}
				}
			}
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RequestResetPosition_Internal()
		{
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RequestResetTargetPositionByName_Internal(string targetName)
		{
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RequestResetTargetPositionByEntity_Internal(NetworkEntity targetEntity)
		{
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x000562A2 File Offset: 0x000544A2
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RequestResetPosition()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RequestResetPosition_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				this.ResetDefaultPosition();
			}
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x00113400 File Offset: 0x00111600
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RequestResetTargetPositionByName(string targetName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RequestResetTargetPositionByName_Internal(targetName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity))
				{
					networkEntity.PlayerRpcHandler.ResetDefaultPosition();
					this.SendChatNotification("Position reset for " + targetName);
					return;
				}
				this.SendChatNotification("Unable to find player by the name of " + targetName);
			}
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x00113474 File Offset: 0x00111674
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RequestResetTargetPositionByEntity(NetworkEntity targetEntity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RequestResetTargetPositionByEntity_Internal(targetEntity);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && targetEntity != null)
			{
				targetEntity.PlayerRpcHandler.ResetDefaultPosition();
			}
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Summon_Internal(string playerName)
		{
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SummonGroup_Internal(string playerName)
		{
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_TeleportTo_Internal(string playerName)
		{
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_TeleportToCorpse_Internal(string playerName)
		{
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x001134C4 File Offset: 0x001116C4
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Summon(string playerName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Summon_Internal(playerName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity netEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out netEntity))
				{
					Vector3 position = base.GameEntity.gameObject.transform.position;
					float y = base.GameEntity.gameObject.transform.eulerAngles.y;
					this.SetRemotePlayerPosition(netEntity, position, y, "Summoned by " + base.GameEntity.CharacterData.Name.Value);
					this.SendChatNotification("Summoned " + playerName);
					return;
				}
				string characterName = playerName.ToTitleCase();
				CharacterRecord characterRecord = CharacterRecord.LoadForCharacterName(ExternalGameDatabase.Database, characterName);
				if (characterRecord == null)
				{
					this.SendChatNotification("Unable to find CharacterRecord for " + playerName);
					return;
				}
				if (AuthRecord.Load(ExternalGameDatabase.Database, characterRecord.UserId) == null)
				{
					this.SendChatNotification("Unable to find AuthRecord for " + playerName);
					return;
				}
				UserRecord userRecord = UserRecord.Load(ExternalGameDatabase.Database, characterRecord.UserId);
				if (userRecord == null)
				{
					this.SendChatNotification("Unable to find UserRecord for " + playerName);
					return;
				}
				ZoneId zoneId = (ZoneId)LocalZoneManager.ZoneRecord.ZoneId;
				ZoneRecord zoneRecord = ZoneRecord.LoadZoneId(ExternalGameDatabase.Database, zoneId);
				if (zoneRecord == null)
				{
					this.SendChatNotification("Unable to find ZoneRecord for current zone??");
					return;
				}
				if (!AccessFlagsExtensions.HasAccess(zoneRecord.Flags, (int)userRecord.Flags))
				{
					this.SendChatNotification(playerName + " has insufficient permissions to zone to " + zoneId.ToString());
					return;
				}
				Transform transform = base.GameEntity.gameObject.transform;
				Vector3 position2 = transform.position;
				CharacterLocation value = new CharacterLocation
				{
					ZoneId = zoneRecord.ZoneId,
					x = position2.x,
					y = position2.y,
					z = position2.z,
					h = transform.eulerAngles.y
				};
				ZoneId zoneId2 = (ZoneId)characterRecord.Location.ZoneId;
				Dictionary<string, string> fields = new Dictionary<string, string>
				{
					{
						"PlayerName",
						characterRecord.Name
					},
					{
						"Location",
						JsonConvert.SerializeObject(value)
					}
				};
				ServerCommunicator.POST(zoneId2, "summonPlayer", fields, null);
				this.SendChatNotification("Sent summon request to " + playerName + " in " + zoneId2.ToString());
			}
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x00113724 File Offset: 0x00111924
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SummonGroup(string playerName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SummonGroup_Internal(playerName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out networkEntity))
				{
					Vector3 position = base.GameEntity.gameObject.transform.position;
					float y = base.GameEntity.gameObject.transform.eulerAngles.y;
					string msg = "Summoned by " + base.GameEntity.CharacterData.Name.Value;
					this.SetRemotePlayerPosition(networkEntity, position, y, msg);
					this.SendChatNotification("Summoned " + playerName);
					UniqueId groupId = networkEntity.GameEntity.CharacterData.GroupId;
					if (groupId.IsEmpty)
					{
						return;
					}
					using (IEnumerator<NetworkEntity> enumerator = ServerGameManager.GroupManager.GroupMembersForGroupId(groupId).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NetworkEntity networkEntity2 = enumerator.Current;
							if (!(networkEntity2 == networkEntity))
							{
								this.SetRemotePlayerPosition(networkEntity2, position, y, msg);
								this.SendChatNotification("Summoned " + networkEntity2.GameEntity.CharacterData.Name.Value);
							}
						}
						return;
					}
				}
				this.SendChatNotification("Unable to find player by the name of " + playerName);
			}
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x000562D8 File Offset: 0x000544D8
		private void SetRemotePlayerPosition(NetworkEntity netEntity, Vector3 pos, float rot, string msg)
		{
			if (!netEntity)
			{
				return;
			}
			netEntity.PlayerRpcHandler.ResetPosition(pos, rot);
			netEntity.PlayerRpcHandler.SendChatNotification(msg);
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x00113890 File Offset: 0x00111A90
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_TeleportTo(string playerName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_TeleportTo_Internal(playerName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out networkEntity))
				{
					Vector3 position = networkEntity.GameEntity.gameObject.transform.position;
					float y = networkEntity.GameEntity.gameObject.transform.eulerAngles.y;
					this.ResetPosition(position, y);
					return;
				}
				string characterName = playerName.ToTitleCase();
				CharacterRecord characterRecord = CharacterRecord.LoadForCharacterName(ExternalGameDatabase.Database, characterName);
				if (characterRecord == null)
				{
					this.SendChatNotification("Unable to find player by the name of " + playerName);
					return;
				}
				float num = (float)(DateTime.UtcNow - characterRecord.Updated).TotalSeconds;
				if (num > 60f)
				{
					this.SendChatNotification(string.Concat(new string[]
					{
						playerName,
						"'s CharacterRecord was last updated more than ",
						Mathf.FloorToInt(60f).ToString(),
						"s ago (",
						num.GetFormattedTime(false),
						") and is likely offline."
					}));
					return;
				}
				if (AuthRecord.Load(ExternalGameDatabase.Database, characterRecord.UserId) == null)
				{
					this.SendChatNotification("No AuthRecord found for " + playerName);
					return;
				}
				ZoneId zoneId = (ZoneId)characterRecord.Location.ZoneId;
				this.SendChatNotification("Teleporting you to " + playerName + " in " + zoneId.ToString());
				this.m_netEntity.GameEntity.ServerPlayerController.ZonePlayerToCustomLocation(characterRecord.Location.Clone());
				this.AuthorizeZone(true, characterRecord.Location.ZoneId, -1);
			}
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x00113A40 File Offset: 0x00111C40
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_TeleportToCorpse(string playerName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_TeleportToCorpse_Internal(playerName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				playerName = playerName.ToLowerInvariant();
				Vector3 pos;
				if (CorpseManager.TryGetCorpsePosition(playerName, out pos))
				{
					this.ResetPosition(pos, base.GameEntity.gameObject.transform.eulerAngles.y);
					return;
				}
				this.SendChatNotification("Unable to locate corpse for " + playerName);
			}
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x00113AC4 File Offset: 0x00111CC4
		public void ResetDefaultPosition()
		{
			if (this.m_netEntity.IsServer)
			{
				PlayerSpawn defaultPlayerSpawn = LocalZoneManager.GetDefaultPlayerSpawn(base.GameEntity);
				Vector3 position = defaultPlayerSpawn.GetPosition();
				float y = defaultPlayerSpawn.GetRotation().eulerAngles.y;
				this.ResetPosition(position, y);
			}
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x000562FD File Offset: 0x000544FD
		public void ResetPosition(Vector3 pos, float rot)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Respawn(pos, rot);
			}
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetGameTime_Internal(float desiredValue)
		{
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetGameTime_Internal()
		{
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x00056314 File Offset: 0x00054514
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetGameTime(float desiredTime)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetGameTime_Internal(desiredTime);
				return;
			}
			if (this.m_netEntity.IsServer && GameTimeReplicator.Instance != null)
			{
				GameTimeReplicator.Instance.SetGameTimeOverride(desiredTime);
			}
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x00056350 File Offset: 0x00054550
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetGameTime()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetGameTime_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && GameTimeReplicator.Instance != null)
			{
				GameTimeReplicator.Instance.ResetGameTime();
			}
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x00113B0C File Offset: 0x00111D0C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddCurrency(ulong currency)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddCurrency_Internal(currency);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Inventory != null)
			{
				base.GameEntity.CollectionController.Inventory.AddCurrency(currency);
				CurrencyTransaction transaction = new CurrencyTransaction
				{
					Add = true,
					Amount = currency,
					Message = "GM Command",
					TargetContainer = base.GameEntity.CollectionController.Inventory.Id
				};
				this.ProcessCurrencyTransaction(transaction);
			}
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x00113BCC File Offset: 0x00111DCC
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddEventCurrency(ulong currency)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddEventCurrency_Internal(currency);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null)
			{
				base.GameEntity.CollectionController.ModifyEventCurrency(currency, false);
			}
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x00113C28 File Offset: 0x00111E28
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddNetworkEntityEventCurrency(NetworkEntity networkEntity, ulong currency)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddNetworkEntityEventCurrency_Internal(networkEntity, currency);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.CollectionController != null)
			{
				networkEntity.GameEntity.CollectionController.ModifyEventCurrency(currency, false);
			}
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x00113C9C File Offset: 0x00111E9C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddTargetEventCurrency(string targetName, ulong currency)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddTargetEventCurrency_Internal(targetName, currency);
				return;
			}
			NetworkEntity networkEntity;
			if (this.m_netEntity.IsServer && base.GameEntity.GM && ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity) && networkEntity.GameEntity && networkEntity.GameEntity.CollectionController != null)
			{
				networkEntity.GameEntity.CollectionController.ModifyEventCurrency(currency, false);
				this.SendChatNotification("Added " + currency.ToString() + " event currency to " + targetName);
			}
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddEmberStone_Internal()
		{
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RemoveEmberStone_Internal()
		{
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AlterEmberEssence_Internal(int value)
		{
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AlterTravelEssence_Internal(int value)
		{
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_UpgradeEmberStone_Internal()
		{
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x00113D2C File Offset: 0x00111F2C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddEmberStone()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddEmberStone_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.CurrentEmberStone == null && GlobalSettings.Values != null && GlobalSettings.Values.Progression != null && GlobalSettings.Values.Progression.StartingEmberStone != null)
			{
				base.GameEntity.CollectionController.CurrentEmberStone = GlobalSettings.Values.Progression.StartingEmberStone;
			}
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x00113DE0 File Offset: 0x00111FE0
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RemoveEmberStone()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RemoveEmberStone_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.CurrentEmberStone != null)
			{
				base.GameEntity.CollectionController.CurrentEmberStone = null;
			}
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x00113E54 File Offset: 0x00112054
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AlterEmberEssence(int value)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AlterEmberEssence_Internal(value);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null)
			{
				base.GameEntity.CollectionController.AdjustEmberEssenceCount(value);
			}
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x00113EB0 File Offset: 0x001120B0
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AlterTravelEssence(int value)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AlterTravelEssence_Internal(value);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null)
			{
				base.GameEntity.CollectionController.AdjustTravelEssenceCount(value);
			}
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x00113F0C File Offset: 0x0011210C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_UpgradeEmberStone()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_UpgradeEmberStone_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.CurrentEmberStone != null && base.GameEntity.CollectionController.CurrentEmberStone.NextEmberStone != null)
			{
				base.GameEntity.CollectionController.CurrentEmberStone = base.GameEntity.CollectionController.CurrentEmberStone.NextEmberStone;
			}
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ModifyTitle_Internal(string title)
		{
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ModifyTargetTitleByName_Internal(string targetName, string title)
		{
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ModifyTargetTitleByEntity_Internal(NetworkEntity targetEntity, string title)
		{
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x00113FB0 File Offset: 0x001121B0
		private void TitleModifiedResponse_Internal(string title)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1188281895);
			rpcBuffer.AddString(title);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x00113FF0 File Offset: 0x001121F0
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ModifyTitle(string title)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ModifyTitle_Internal(title);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && !TitleManager.AlterTitles(base.GameEntity, title))
			{
				this.SendChatNotification("Unable to add/remove the title `" + title + "` on yourself");
			}
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x00114050 File Offset: 0x00112250
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ModifyTargetTitleByName(string targetName, string title)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ModifyTargetTitleByName_Internal(targetName, title);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity))
				{
					if (TitleManager.AlterTitles(networkEntity.GameEntity, title))
					{
						this.SendChatNotification(string.Concat(new string[]
						{
							"Title modified ",
							networkEntity.GameEntity.CharacterData.Name.Value,
							" (`",
							title,
							"`)"
						}));
						return;
					}
					this.SendChatNotification("Unable to add/remove the title `" + title + "` on " + networkEntity.GameEntity.CharacterData.Name.Value);
					return;
				}
				else
				{
					this.SendChatNotification("Unable to find player by the name of " + targetName);
				}
			}
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x00114134 File Offset: 0x00112334
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ModifyTargetTitleByEntity(NetworkEntity targetEntity, string title)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ModifyTargetTitleByEntity_Internal(targetEntity, title);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM && !TitleManager.AlterTitles(targetEntity.GameEntity, title))
			{
				this.SendChatNotification("Unable to add/remove the title `" + title + "` on " + targetEntity.GameEntity.CharacterData.Name.Value);
			}
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x0005638A File Offset: 0x0005458A
		[NetworkRPC(RpcType.ServerToClient)]
		public void TitleModifiedResponse(string title)
		{
			if (this.m_netEntity.IsServer)
			{
				this.TitleModifiedResponse_Internal(title);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				TitleManager.AlterTitles(base.GameEntity, title);
			}
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetAbilityTimers_Internal()
		{
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x001141AC File Offset: 0x001123AC
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetAbilityTimers()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetAbilityTimers_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				foreach (ArchetypeInstance archetypeInstance in base.GameEntity.CollectionController.Abilities.Instances)
				{
					archetypeInstance.AbilityData.GM_ResetCooldown();
				}
				Dictionary<ConsumableCategory, DateTime> consumableLastUseTimes = base.GameEntity.CollectionController.Record.ConsumableLastUseTimes;
				if (consumableLastUseTimes == null)
				{
					return;
				}
				consumableLastUseTimes.Clear();
			}
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_GetNpcTickRate_Internal()
		{
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetNpcTickRate_Internal(int tickRate)
		{
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_GetNpcBucketSize_Internal()
		{
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetNpcBucketSize_Internal(int bucketSize)
		{
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_GetPathfindingIterations_Internal()
		{
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_SetPathfindingIterations_Internal(int iterations)
		{
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x00114258 File Offset: 0x00112458
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_GetNpcTickRate()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_GetNpcTickRate_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				this.SendChatNotification("Current NpcTicksPerFrame is " + ServerGameManager.NpcBehaviorManager.TicksPerFrame.ToString());
			}
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x001142B8 File Offset: 0x001124B8
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetNpcTickRate(int tickRate)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetNpcTickRate_Internal(tickRate);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				int ticksPerFrame = ServerGameManager.NpcBehaviorManager.TicksPerFrame;
				ServerGameManager.NpcBehaviorManager.TicksPerFrame = tickRate;
				int ticksPerFrame2 = ServerGameManager.NpcBehaviorManager.TicksPerFrame;
				this.SendChatNotification("Adjusted NpcTicksPerFrame from " + ticksPerFrame.ToString() + " to " + ticksPerFrame2.ToString());
			}
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x00114338 File Offset: 0x00112538
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_GetNpcBucketSize()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_GetNpcBucketSize_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				this.SendChatNotification("Current NpcBucketSize is " + ServerGameManager.NpcTargetManager.BucketSize.ToString());
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x00114398 File Offset: 0x00112598
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetNpcBucketSize(int bucketSize)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetNpcBucketSize_Internal(bucketSize);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				int bucketSize2 = ServerGameManager.NpcTargetManager.BucketSize;
				ServerGameManager.NpcTargetManager.BucketSize = bucketSize;
				int bucketSize3 = ServerGameManager.NpcTargetManager.BucketSize;
				this.SendChatNotification("Adjusted NpcBucketSize from " + bucketSize2.ToString() + " to " + bucketSize3.ToString());
			}
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x00114418 File Offset: 0x00112618
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_GetPathfindingIterations()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_GetPathfindingIterations_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				this.SendChatNotification("Current PathfindingIterationsPerFrame is " + NavMeshUtilities.GetPathfindingIterationsPerFrame().ToString());
			}
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00114470 File Offset: 0x00112670
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_SetPathfindingIterations(int iterations)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_SetPathfindingIterations_Internal(iterations);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				int pathfindingIterationsPerFrame = NavMeshUtilities.GetPathfindingIterationsPerFrame();
				NavMeshUtilities.SetPathfindingIterationsPerFrame(iterations);
				int pathfindingIterationsPerFrame2 = NavMeshUtilities.GetPathfindingIterationsPerFrame();
				this.SendChatNotification("Adjusted PathfindingIterationsPerFrame from " + pathfindingIterationsPerFrame.ToString() + " to " + pathfindingIterationsPerFrame2.ToString());
			}
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_Disconnect_Internal(string playerName)
		{
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_DisconnectAllCurrentZone_Internal()
		{
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_DisconnectAllTargetZone_Internal(ZoneId zoneId)
		{
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_DisconnectAll_Internal()
		{
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x001144E4 File Offset: 0x001126E4
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_Disconnect(string playerName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_Disconnect_Internal(playerName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity netEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out netEntity))
				{
					string msg = ServerNetworkEntityManager.DisconnectNetworkEntity(netEntity) ? ("Player Disconnected: " + playerName) : ("Unable to disconnect: " + playerName);
					this.SendChatNotification(msg);
					return;
				}
				string text = playerName.ToTitleCase();
				CharacterRecord characterRecord = CharacterRecord.LoadForCharacterName(ExternalGameDatabase.Database, text);
				if (characterRecord == null)
				{
					this.SendChatNotification("Unable to find CharacterRecord for " + text);
					return;
				}
				ZoneId zoneId = (ZoneId)characterRecord.Location.ZoneId;
				Dictionary<string, string> fields = new Dictionary<string, string>
				{
					{
						"PlayerName",
						characterRecord.Name
					}
				};
				ServerCommunicator.POST(zoneId, "disconnectPlayer", fields, null);
				this.SendChatNotification("Sent disconnect request to " + characterRecord.Name + " in " + zoneId.ToString());
			}
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x001145E0 File Offset: 0x001127E0
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_DisconnectAllCurrentZone()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_DisconnectAllCurrentZone_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				int num = ServerNetworkEntityManager.DisconnectAll();
				string msg = (num > 0) ? ("Disconnected " + num.ToString() + " players from current zone") : "No players disconnected from current zone";
				this.SendChatNotification(msg);
			}
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x0011464C File Offset: 0x0011284C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_DisconnectAllTargetZone(ZoneId zoneId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_DisconnectAllTargetZone_Internal(zoneId);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == (int)zoneId)
				{
					this.GM_DisconnectAllCurrentZone();
					return;
				}
				ServerCommunicator.GET(zoneId, "disconnectAll", new Action<UnityWebRequest>(this.CrossZoneDisconnectResponse));
			}
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x001146BC File Offset: 0x001128BC
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_DisconnectAll()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_DisconnectAll_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				this.GM_DisconnectAllCurrentZone();
				for (int i = 0; i < ZoneIdExtensions.ZoneIds.Length; i++)
				{
					if (LocalZoneManager.ZoneRecord == null || LocalZoneManager.ZoneRecord.ZoneId != (int)ZoneIdExtensions.ZoneIds[i])
					{
						ServerCommunicator.GET(ZoneIdExtensions.ZoneIds[i], "disconnectAll", new Action<UnityWebRequest>(this.CrossZoneDisconnectResponse));
					}
				}
			}
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x000563BB File Offset: 0x000545BB
		private void CrossZoneDisconnectResponse(UnityWebRequest obj)
		{
			if (obj.IsWebError())
			{
				Debug.Log(obj.error);
				return;
			}
			if (obj.downloadHandler != null)
			{
				Debug.Log(obj.downloadHandler.text);
			}
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ListRemoteSpawns_Internal(string filter)
		{
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ListRemoteNodes_Internal(string filter)
		{
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RemoteSpawnNpc_Internal(string spawnName, int spawnLevel, SpawnBehaviorType behaviorType)
		{
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RemoteSpawnNode_Internal(string spawnName)
		{
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x000563E9 File Offset: 0x000545E9
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ListRemoteSpawns(string filter)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ListRemoteSpawns_Internal(filter);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.ListRemote(ServerGameManager.RemoteSpawnableNpcs, filter);
			}
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x00056419 File Offset: 0x00054619
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ListRemoteNodes(string filter)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ListRemoteNodes_Internal(filter);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.ListRemote(ServerGameManager.RemoteSpawnableNodes, filter);
			}
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x00056449 File Offset: 0x00054649
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RemoteSpawnNpc(string spawnName, int spawnLevel, SpawnBehaviorType behaviorType)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RemoteSpawnNpc_Internal(spawnName, spawnLevel, behaviorType);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.SpawnRemote(ServerGameManager.RemoteSpawnableNpcs, spawnName, spawnLevel, behaviorType);
			}
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x0005647D File Offset: 0x0005467D
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RemoteSpawnNode(string spawnName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RemoteSpawnNode_Internal(spawnName);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				this.SpawnRemote(ServerGameManager.RemoteSpawnableNodes, spawnName, -1, SpawnBehaviorType.Default);
			}
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x00114748 File Offset: 0x00112948
		private void ListRemote(ISpawnControllerRemoteSpawns controller, string filter)
		{
			string value = string.Empty;
			if (controller == null)
			{
				value = "Invalid Remote Spawnable Collection!";
			}
			else
			{
				value = controller.GetRemoteNames(filter);
				if (string.IsNullOrEmpty(value))
				{
					value = "Nothing to spawn!";
				}
			}
			this.SendLongChatNotification(new LongString
			{
				Value = value
			});
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x0004475B File Offset: 0x0004295B
		private void SpawnRemote(ISpawnControllerRemoteSpawns controller, string spawnName, int spawnLevel, SpawnBehaviorType behaviorType = SpawnBehaviorType.Default)
		{
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_RemoveCorpse_Internal(string targetName)
		{
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x00114794 File Offset: 0x00112994
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_RemoveCorpse(string targetName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_RemoveCorpse_Internal(targetName);
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity))
				{
					if (!networkEntity.GameEntity || networkEntity.GameEntity.CollectionController == null || networkEntity.GameEntity.CollectionController.Record == null)
					{
						this.SendChatNotification("Invalid entity!");
						return;
					}
					if (networkEntity.GameEntity.CollectionController.Record.Corpse != null)
					{
						CorpseManager.RemoveCorpseForPlayer(networkEntity.GameEntity);
						return;
					}
					this.SendChatNotification("Player " + targetName + " has no corpse to remove!");
					return;
				}
				else
				{
					this.SendChatNotification("Unable to find player by the name of " + targetName);
				}
			}
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_AddToHuntingLog_Internal(UniqueId profileId, int value)
		{
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetHuntingLog_Internal(UniqueId profileId)
		{
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ClearHuntingLog_Internal()
		{
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x00114864 File Offset: 0x00112A64
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_AddToHuntingLog(UniqueId profileId, int value)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_AddToHuntingLog_Internal(profileId, value);
				return;
			}
			HuntingLogEntry huntingLogEntry;
			HuntingLogProfile huntingLogProfile;
			if (this.m_netEntity.IsServer && base.GameEntity && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null && base.GameEntity.CollectionController.Record.HuntingLog != null && base.GameEntity.CollectionController.Record.HuntingLog.TryGetValue(profileId, out huntingLogEntry) && huntingLogEntry.TryGetProfile(out huntingLogProfile) && huntingLogProfile.Settings)
			{
				huntingLogEntry.PerkCount = Mathf.Clamp(huntingLogEntry.PerkCount + value, 0, huntingLogProfile.Settings.MaxPerkCount);
				huntingLogEntry.TotalCount += (ulong)((long)value);
			}
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x00114954 File Offset: 0x00112B54
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetHuntingLog(UniqueId profileId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetHuntingLog_Internal(profileId);
				return;
			}
			HuntingLogEntry huntingLogEntry;
			if (this.m_netEntity.IsServer && base.GameEntity && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null && base.GameEntity.CollectionController.Record.HuntingLog != null && base.GameEntity.CollectionController.Record.HuntingLog.TryGetValue(profileId, out huntingLogEntry))
			{
				base.GameEntity.CollectionController.Record.HuntingLog.Remove(profileId);
			}
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x00114A14 File Offset: 0x00112C14
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ClearHuntingLog()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ClearHuntingLog_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null)
			{
				base.GameEntity.CollectionController.Record.HuntingLog = null;
			}
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_TriggerFireworks_Internal()
		{
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x00114A94 File Offset: 0x00112C94
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_TriggerFireworks()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_TriggerFireworks_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity && base.GameEntity.GM && RandomFireworksController.Instance)
			{
				RandomFireworksController.Instance.TriggerAllFireworks();
			}
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_LearnAlchemyII_Internal(UniqueId abilityInstanceId)
		{
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x00114AF4 File Offset: 0x00112CF4
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_LearnAlchemyII(UniqueId abilityInstanceId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_LearnAlchemyII_Internal(abilityInstanceId);
				return;
			}
			ArchetypeInstance archetypeInstance;
			if (this.m_netEntity.IsServer && base.GameEntity && base.GameEntity.GM && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Abilities != null && base.GameEntity.CollectionController.Abilities.TryGetInstanceForInstanceId(abilityInstanceId, out archetypeInstance) && archetypeInstance.AbilityData != null)
			{
				int usageDeltaToUnlockAlchemyII = AlchemyExtensions.GetUsageDeltaToUnlockAlchemyII(archetypeInstance);
				if (usageDeltaToUnlockAlchemyII > 0)
				{
					archetypeInstance.AbilityData.GM_AdjustUseCount(AlchemyPowerLevel.I, usageDeltaToUnlockAlchemyII);
				}
			}
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_BBDrop_Internal(UniqueId taskId)
		{
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_NotifyBBDrop_Internal(OpCodes opCode, UniqueId taskId)
		{
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_BBClear_Internal()
		{
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_BBClearTarget_Internal(string targetName)
		{
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x000564AF File Offset: 0x000546AF
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_BBDrop(UniqueId taskId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_BBDrop_Internal(taskId);
				return;
			}
			GameManager.QuestManager.DropTask(taskId, base.GameEntity);
			this.GM_NotifyBBDrop(OpCodes.Ok, taskId);
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x000564DF File Offset: 0x000546DF
		[NetworkRPC(RpcType.ServerToClient)]
		public void GM_NotifyBBDrop(OpCodes opCode, UniqueId taskId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.GM_NotifyBBDrop_Internal(opCode, taskId);
				return;
			}
			if (opCode == OpCodes.Ok)
			{
				GameManager.QuestManager.DropTask(taskId, base.GameEntity);
				this.SendChatNotification("Task dropped!");
			}
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x00056517 File Offset: 0x00054717
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_BBClear()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_BBClear_Internal();
				return;
			}
			GameManager.QuestManager.ResetTasks(base.GameEntity);
			this.NotifyBBClear(OpCodes.Ok);
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x00114B98 File Offset: 0x00112D98
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_BBClearTarget(string targetName)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_BBClearTarget_Internal(targetName);
				return;
			}
			if (this.m_netEntity.IsServer && this.m_netEntity.GameEntity.GM)
			{
				NetworkEntity networkEntity;
				if (ServerNetworkEntityManager.TryGetNetworkEntityByName(targetName, out networkEntity) && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Player)
				{
					GameManager.QuestManager.ResetTasks(networkEntity.GameEntity);
					networkEntity.PlayerRpcHandler.NotifyBBClear(OpCodes.Ok);
					return;
				}
				this.SendChatNotification("Unable to find player by the name of " + targetName);
			}
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ReloadGameServerConfig_Internal()
		{
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x00114C2C File Offset: 0x00112E2C
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ReloadGameServerConfig()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ReloadGameServerConfig_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && this.m_netEntity.GameEntity.GM && GameManager.Instance)
			{
				string msg = GameManager.Instance.LoadGameServerConfig() ? "GameServerConfig Successfully Reloaded!" : "GameServerConfig Failed to Reload :(";
				this.SendChatNotification(msg);
			}
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_GetNpcStats_Internal()
		{
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x00114C98 File Offset: 0x00112E98
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_GetNpcStats()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_GetNpcStats_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.GM && ServerGameManager.NpcBehaviorManager)
			{
				this.SendChatNotification(ServerGameManager.NpcBehaviorManager.GetNpcStatSummary());
			}
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_GetActivatedMonolith_Internal()
		{
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ResetActivatedMonolith_Internal()
		{
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00056544 File Offset: 0x00054744
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_GetActivatedMonolith()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_GetActivatedMonolith_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity)
			{
				bool gm = base.GameEntity.GM;
			}
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x00056580 File Offset: 0x00054780
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ResetActivatedMonolith()
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ResetActivatedMonolith_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && base.GameEntity)
			{
				bool gm = base.GameEntity.GM;
			}
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x0004475B File Offset: 0x0004295B
		private void GM_ReturnAuction_Internal(string auctionId)
		{
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x00114D08 File Offset: 0x00112F08
		[NetworkRPC(RpcType.ClientToServer)]
		public void GM_ReturnAuction(string auctionId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.GM_ReturnAuction_Internal(auctionId);
				return;
			}
			if (this.m_netEntity.IsServer && ServerGameManager.AuctionHouseManager && base.GameEntity && base.GameEntity.GM)
			{
				AuctionResponse response = ServerGameManager.AuctionHouseManager.RefundAuction(base.GameEntity, auctionId);
				this.SendAuctionResponse(response);
			}
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x00114D78 File Offset: 0x00112F78
		private void QA_RequestZoneToQA_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1769821620);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x00114DB0 File Offset: 0x00112FB0
		[NetworkRPC(RpcType.ClientToServer)]
		public void QA_RequestZoneToQA()
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = true;
				this.QA_RequestZoneToQA_Internal();
				return;
			}
			if (this.m_netEntity.IsServer && DeploymentBranchFlagsExtensions.IsQA())
			{
				bool authorized = false;
				ZoneId targetZoneId;
				if (this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.CharacterData && this.m_netEntity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire) && ZoneIdExtensions.ZoneIdDict.TryGetValue(50, out targetZoneId))
				{
					base.GameEntity.ServerPlayerController.ZonePlayer(targetZoneId, 0, ZoningState.GMZoning);
					authorized = true;
				}
				this.AuthorizeZone(authorized, 50, -1);
			}
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x00114E74 File Offset: 0x00113074
		private void QA_RequestZoneToPOI_Internal(string poiName)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1958747132);
			rpcBuffer.AddString(poiName);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x00114EB4 File Offset: 0x001130B4
		[NetworkRPC(RpcType.ClientToServer)]
		public void QA_RequestZoneToPOI(string poiName)
		{
			if (this.m_netEntity.IsLocal && !this.m_zoneRequested)
			{
				this.m_zoneRequested = true;
				this.QA_RequestZoneToPOI_Internal(poiName);
				return;
			}
			if (this.m_netEntity.IsServer && (DeploymentBranchFlagsExtensions.IsQA() || (this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.GM)))
			{
				bool authorized = false;
				int num = 0;
				string value;
				if (this.m_netEntity.GameEntity && this.m_netEntity.GameEntity.CharacterData && this.m_netEntity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire) && POICommands.DebugLocs.TryGetValue(poiName, out value))
				{
					DebugLocation debugLocation = new DebugLocation(value);
					ZoneId zoneId;
					if (debugLocation.Valid && ZoneIdExtensions.ZoneIdDict.TryGetValue(debugLocation.ZoneId, out zoneId))
					{
						num = debugLocation.ZoneId;
						if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == num)
						{
							this.ResetPosition(debugLocation.Position, debugLocation.Rotation.eulerAngles.y);
						}
						else
						{
							CharacterLocation location = new CharacterLocation
							{
								ZoneId = debugLocation.ZoneId,
								x = debugLocation.Position.x,
								y = debugLocation.Position.y,
								z = debugLocation.Position.z,
								h = debugLocation.Rotation.eulerAngles.y
							};
							base.GameEntity.ServerPlayerController.ZonePlayerToCustomLocation(location);
							authorized = true;
						}
					}
				}
				this.AuthorizeZone(authorized, num, -1);
			}
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x00115080 File Offset: 0x00113280
		private void Server_RequestTrade_Response_Internal(UniqueId clientTradeId, UniqueId newTradeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1358052384);
			clientTradeId.PackData(rpcBuffer);
			newTradeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x001150CC File Offset: 0x001132CC
		private void Server_RequestTrade_Internal(UniqueId tradeId, NetworkEntity sourceEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1310159409);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(sourceEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x00115124 File Offset: 0x00113324
		private void Server_CompleteTradeHandshake_Internal(UniqueId tradeId, bool proceed)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(508250884);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddBool(proceed);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x0011516C File Offset: 0x0011336C
		private void Server_TradeTransactionConcluded_Internal(TakeAllResponse response, TradeCompletionCode code)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2054250220);
			response.PackData(rpcBuffer);
			rpcBuffer.AddEnum(code);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x001151B4 File Offset: 0x001133B4
		private void Server_TradeTermsAccepted_Internal(UniqueId tradeId, NetworkEntity entity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1720681940);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(entity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x0011520C File Offset: 0x0011340C
		private void Server_TradeItemAdded_Internal(UniqueId tradeId, ArchetypeInstance instance)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(511747974);
			tradeId.PackData(rpcBuffer);
			instance.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x00115254 File Offset: 0x00113454
		private void Server_TradeItemRemoved_Internal(UniqueId tradeId, UniqueId instanceId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(171329702);
			tradeId.PackData(rpcBuffer);
			instanceId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x001152A0 File Offset: 0x001134A0
		private void Server_TradeItemsSwapped_Internal(UniqueId tradeId, UniqueId instanceIdA, UniqueId instanceIdB)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1824072721);
			tradeId.PackData(rpcBuffer);
			instanceIdA.PackData(rpcBuffer);
			instanceIdB.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x001152F4 File Offset: 0x001134F4
		private void Server_ResetTradeAgreement_Internal(UniqueId tradeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(527547978);
			tradeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x00115334 File Offset: 0x00113534
		private void Server_CurrencyChanged_Internal(UniqueId tradeId, ulong newValue)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1252864005);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddULong(newValue);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x0011537C File Offset: 0x0011357C
		private void Server_ItemCountChanged_Internal(UniqueId tradeId, UniqueId itemInstanceId, int newCount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1580250234);
			tradeId.PackData(rpcBuffer);
			itemInstanceId.PackData(rpcBuffer);
			rpcBuffer.AddInt(newCount);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerToClient);
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x000565BC File Offset: 0x000547BC
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_RequestTrade_Response(UniqueId clientTradeId, UniqueId newTradeId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_RequestTrade_Response_Internal(clientTradeId, newTradeId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_RequestTrade_Response(clientTradeId, newTradeId);
			}
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x000565ED File Offset: 0x000547ED
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_RequestTrade(UniqueId tradeId, NetworkEntity sourceEntity)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_RequestTrade_Internal(tradeId, sourceEntity);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_IncomingTradeRequest(tradeId, sourceEntity);
			}
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x0005661E File Offset: 0x0005481E
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_CompleteTradeHandshake(UniqueId tradeId, bool proceed)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_CompleteTradeHandshake_Internal(tradeId, proceed);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_CompleteTradeHandshake(tradeId, proceed);
			}
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0005664F File Offset: 0x0005484F
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_TradeTransactionConcluded(TakeAllResponse response, TradeCompletionCode code)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TradeTransactionConcluded_Internal(response, code);
				return;
			}
			ClientGameManager.TradeManager.Client_TradeTransactionConcluded(response, code);
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x00056673 File Offset: 0x00054873
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_TradeTermsAccepted(UniqueId tradeId, NetworkEntity entity)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TradeTermsAccepted_Internal(tradeId, entity);
				return;
			}
			ClientGameManager.TradeManager.Client_TradeAccepted(tradeId, entity);
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x00056697 File Offset: 0x00054897
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_TradeItemAdded(UniqueId tradeId, ArchetypeInstance instance)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TradeItemAdded_Internal(tradeId, instance);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_ItemAdded(tradeId, instance);
			}
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x000566C8 File Offset: 0x000548C8
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_TradeItemRemoved(UniqueId tradeId, UniqueId instanceId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TradeItemRemoved_Internal(tradeId, instanceId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_ItemRemoved(tradeId, instanceId);
			}
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x000566F9 File Offset: 0x000548F9
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_TradeItemsSwapped(UniqueId tradeId, UniqueId instanceIdA, UniqueId instanceIdB)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_TradeItemsSwapped_Internal(tradeId, instanceIdA, instanceIdB);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_TradeItemsSwapped(tradeId, instanceIdA, instanceIdB);
			}
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x0005672C File Offset: 0x0005492C
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_ResetTradeAgreement(UniqueId tradeId)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_ResetTradeAgreement_Internal(tradeId);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.UIManager.Trade.ResetTradeAccepted();
			}
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x0005675F File Offset: 0x0005495F
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_CurrencyChanged(UniqueId tradeId, ulong newValue)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_CurrencyChanged_Internal(tradeId, newValue);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_CurrencyChanged(tradeId, newValue);
			}
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x00056790 File Offset: 0x00054990
		[NetworkRPC(RpcType.ServerToClient)]
		public void Server_ItemCountChanged(UniqueId tradeId, UniqueId itemInstanceId, int newCount)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_ItemCountChanged_Internal(tradeId, itemInstanceId, newCount);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				ClientGameManager.TradeManager.Client_ItemCountChanged(tradeId, itemInstanceId, newCount);
			}
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x001153D0 File Offset: 0x001135D0
		private void Client_RequestTrade_Internal(UniqueId clientTradeId, NetworkEntity targetEntity)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1470714163);
			clientTradeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x00115428 File Offset: 0x00113628
		private void Client_ProposedTrade_Response_Internal(UniqueId tradeId, bool response)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1493732902);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddBool(response);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x00115470 File Offset: 0x00113670
		private void Client_AcceptCancelTrade_Internal(UniqueId tradeId, bool accept)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-637832368);
			tradeId.PackData(rpcBuffer);
			rpcBuffer.AddBool(accept);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x001154B8 File Offset: 0x001136B8
		private void Client_ResetTradeAgreement_Internal(UniqueId tradeId)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-679124354);
			tradeId.PackData(rpcBuffer);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x000567C3 File Offset: 0x000549C3
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_RequestTrade(UniqueId clientTradeId, NetworkEntity targetEntity)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_RequestTrade_Internal(clientTradeId, targetEntity);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.TradeManager.Server_RequestTrade(this.m_netEntity, targetEntity, clientTradeId);
			}
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x000567FA File Offset: 0x000549FA
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_ProposedTrade_Response(UniqueId tradeId, bool response)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_ProposedTrade_Response_Internal(tradeId, response);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.TradeManager.Server_ProposedTrade_Response(tradeId, response);
			}
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x001154F8 File Offset: 0x001136F8
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_AcceptCancelTrade(UniqueId tradeId, bool accept)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_AcceptCancelTrade_Internal(tradeId, accept);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				if (accept)
				{
					ServerGameManager.TradeManager.Server_ClientAcceptedTradeTerms(tradeId, this.m_netEntity);
					return;
				}
				ServerGameManager.TradeManager.Server_ClientCancelTrade(tradeId, this.m_netEntity);
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x0005682B File Offset: 0x00054A2B
		[NetworkRPC(RpcType.ClientToServer)]
		public void Client_ResetTradeAgreement(UniqueId tradeId)
		{
			if (this.m_netEntity.IsLocal)
			{
				this.Client_ResetTradeAgreement_Internal(tradeId);
				return;
			}
			if (this.m_netEntity.IsServer)
			{
				ServerGameManager.TradeManager.Server_ResetTradeAgreement(tradeId, this.m_netEntity);
			}
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x00115550 File Offset: 0x00113750
		static PlayerRpcHandler()
		{
			RpcHandler.RegisterCommandDelegate("RequestZone", typeof(PlayerRpcHandler), RpcType.ClientToServer, 621828145, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestZone));
			RpcHandler.RegisterCommandDelegate("RequestZoneToDiscovery", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2013177559, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestZoneToDiscovery));
			RpcHandler.RegisterCommandDelegate("RequestZoneToGroup", typeof(PlayerRpcHandler), RpcType.ClientToServer, -189487217, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestZoneToGroup));
			RpcHandler.RegisterCommandDelegate("GM_RequestZone", typeof(PlayerRpcHandler), RpcType.ClientToServer, 332417141, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RequestZone));
			RpcHandler.RegisterCommandDelegate("AuthorizeZone", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1543406291, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AuthorizeZone));
			RpcHandler.RegisterCommandDelegate("RequestZoneToMapDiscovery", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1231663422, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestZoneToMapDiscovery));
			RpcHandler.RegisterCommandDelegate("Client_RequestInteraction", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1990732059, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_RequestInteraction));
			RpcHandler.RegisterCommandDelegate("Server_RequestInteraction", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1926620825, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_RequestInteraction));
			RpcHandler.RegisterCommandDelegate("Client_CancelInteraction", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1371032526, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_CancelInteraction));
			RpcHandler.RegisterCommandDelegate("Server_CancelInteraction", typeof(PlayerRpcHandler), RpcType.ServerToClient, 389710554, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_CancelInteraction));
			RpcHandler.RegisterCommandDelegate("Client_RequestStateInteraction", typeof(PlayerRpcHandler), RpcType.ClientToServer, -179190786, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_RequestStateInteraction));
			RpcHandler.RegisterCommandDelegate("SetTrackedMasteryOption", typeof(PlayerRpcHandler), RpcType.ClientToServer, -8957207, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SetTrackedMasteryOption));
			RpcHandler.RegisterCommandDelegate("Client_GiveUp", typeof(PlayerRpcHandler), RpcType.ClientToServer, 597638525, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_GiveUp));
			RpcHandler.RegisterCommandDelegate("Server_ApplyDamageToGearForDeath", typeof(PlayerRpcHandler), RpcType.ServerToClient, -55666921, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_ApplyDamageToGearForDeath));
			RpcHandler.RegisterCommandDelegate("Server_Respawn", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -411380183, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Respawn));
			RpcHandler.RegisterCommandDelegate("SetGroupId", typeof(PlayerRpcHandler), RpcType.ClientToServer, -269373221, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SetGroupId));
			RpcHandler.RegisterCommandDelegate("SetRaidId", typeof(PlayerRpcHandler), RpcType.ClientToServer, -124764458, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SetRaidId));
			RpcHandler.RegisterCommandDelegate("ForfeitInventoryRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1884773577, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ForfeitInventoryRequest));
			RpcHandler.RegisterCommandDelegate("ForfeitInventoryResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 820392702, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ForfeitInventoryResponse));
			RpcHandler.RegisterCommandDelegate("ClientJumped", typeof(PlayerRpcHandler), RpcType.ClientToServer, -983144067, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ClientJumped));
			RpcHandler.RegisterCommandDelegate("ClientJumpedBroadcast", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, 1143853980, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ClientJumpedBroadcast));
			RpcHandler.RegisterCommandDelegate("SendChatNotification", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1627622002, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendChatNotification));
			RpcHandler.RegisterCommandDelegate("SendLongChatNotification", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1972585747, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendLongChatNotification));
			RpcHandler.RegisterCommandDelegate("TriggerCannotPerform", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1993149079, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TriggerCannotPerform));
			RpcHandler.RegisterCommandDelegate("ForgetMastery", typeof(PlayerRpcHandler), RpcType.ClientToServer, -119425, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ForgetMastery));
			RpcHandler.RegisterCommandDelegate("ProcessCurrencyTransaction", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1832983623, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ProcessCurrencyTransaction));
			RpcHandler.RegisterCommandDelegate("ProcessInteractiveStationCurrencyTransaction", typeof(PlayerRpcHandler), RpcType.ServerToClient, -592976503, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ProcessInteractiveStationCurrencyTransaction));
			RpcHandler.RegisterCommandDelegate("CurrencyTransferRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 247163380, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_CurrencyTransferRequest));
			RpcHandler.RegisterCommandDelegate("CurrencyTransferRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1251682354, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_CurrencyTransferRequestResponse));
			RpcHandler.RegisterCommandDelegate("CurrencyModifyEvent", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1722530210, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_CurrencyModifyEvent));
			RpcHandler.RegisterCommandDelegate("ProcessMultiContainerCurrencyTransaction", typeof(PlayerRpcHandler), RpcType.ServerToClient, 398795495, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ProcessMultiContainerCurrencyTransaction));
			RpcHandler.RegisterCommandDelegate("ProcessEventCurrencyTransaction", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1136993507, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ProcessEventCurrencyTransaction));
			RpcHandler.RegisterCommandDelegate("RequestObjectiveIteration", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1047154739, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestObjectiveIteration));
			RpcHandler.RegisterCommandDelegate("NotifyObjectiveIteration", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1996453892, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyObjectiveIteration));
			RpcHandler.RegisterCommandDelegate("RequestRewardReissue", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1931216826, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestRewardReissue));
			RpcHandler.RegisterCommandDelegate("NotifyGMQuestReset", typeof(PlayerRpcHandler), RpcType.ServerToClient, -382355451, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyGMQuestReset));
			RpcHandler.RegisterCommandDelegate("MuteQuest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1937016485, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MuteQuest));
			RpcHandler.RegisterCommandDelegate("RequestDropQuestsAndTasksForMastery", typeof(PlayerRpcHandler), RpcType.ClientToServer, 133330845, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestDropQuestsAndTasksForMastery));
			RpcHandler.RegisterCommandDelegate("DrawBBTask", typeof(PlayerRpcHandler), RpcType.ClientToServer, 744560740, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DrawBBTask));
			RpcHandler.RegisterCommandDelegate("NotifyDrawBBTask", typeof(PlayerRpcHandler), RpcType.ServerToClient, -172514876, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyDrawBBTask));
			RpcHandler.RegisterCommandDelegate("IterateBBTask", typeof(PlayerRpcHandler), RpcType.ClientToServer, -120975047, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_IterateBBTask));
			RpcHandler.RegisterCommandDelegate("NotifyBBTaskIterated", typeof(PlayerRpcHandler), RpcType.ServerToClient, -209396829, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyBBTaskIterated));
			RpcHandler.RegisterCommandDelegate("RequestNpcLearn", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1084803737, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestNpcLearn));
			RpcHandler.RegisterCommandDelegate("NotifyNpcLearn", typeof(PlayerRpcHandler), RpcType.ServerToClient, -333753616, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyNpcLearn));
			RpcHandler.RegisterCommandDelegate("DiscoveryNotification", typeof(PlayerRpcHandler), RpcType.ServerToClient, -2086276638, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DiscoveryNotification));
			RpcHandler.RegisterCommandDelegate("ModifyEquipmentAbsorbed", typeof(PlayerRpcHandler), RpcType.ServerToClient, -2016753354, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ModifyEquipmentAbsorbed));
			RpcHandler.RegisterCommandDelegate("RemoteRefreshHighestLevelMastery", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1412194063, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RemoteRefreshHighestLevelMastery));
			RpcHandler.RegisterCommandDelegate("TakeFallDamage", typeof(PlayerRpcHandler), RpcType.ClientToServer, -2019658019, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TakeFallDamage));
			RpcHandler.RegisterCommandDelegate("Server_TakeFallDamage", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -171816164, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TakeFallDamage));
			RpcHandler.RegisterCommandDelegate("RequestTitleChange", typeof(PlayerRpcHandler), RpcType.ClientToServer, -99391108, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestTitleChange));
			RpcHandler.RegisterCommandDelegate("LootRollResponse", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1347211544, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_LootRollResponse));
			RpcHandler.RegisterCommandDelegate("PlayEmoteRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -57037076, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_PlayEmoteRequest));
			RpcHandler.RegisterCommandDelegate("PlayEmoteResponse", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -626409346, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_PlayEmoteResponse));
			RpcHandler.RegisterCommandDelegate("StuckRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1964341464, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_StuckRequest));
			RpcHandler.RegisterCommandDelegate("RopeRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1565556672, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RopeRequest));
			RpcHandler.RegisterCommandDelegate("StuckResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1501188591, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_StuckResponse));
			RpcHandler.RegisterCommandDelegate("DuelRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1000303217, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DuelRequest));
			RpcHandler.RegisterCommandDelegate("DuelResponse", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1676310227, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DuelResponse));
			RpcHandler.RegisterCommandDelegate("Server_DuelRequest", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1169266588, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_DuelRequest));
			RpcHandler.RegisterCommandDelegate("RequestSelfCorpseDrag", typeof(PlayerRpcHandler), RpcType.ClientToServer, -96600820, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestSelfCorpseDrag));
			RpcHandler.RegisterCommandDelegate("RequestGroupMemberCorpseDrag", typeof(PlayerRpcHandler), RpcType.ClientToServer, 46416971, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RequestGroupMemberCorpseDrag));
			RpcHandler.RegisterCommandDelegate("ToggleGroupConsent", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2046457682, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ToggleGroupConsent));
			RpcHandler.RegisterCommandDelegate("ChangePortraitRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1239755285, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ChangePortraitRequest));
			RpcHandler.RegisterCommandDelegate("InspectRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1793832839, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_InspectRequest));
			RpcHandler.RegisterCommandDelegate("InspectResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1910680035, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_InspectResponse));
			RpcHandler.RegisterCommandDelegate("InspectNotification", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1488160658, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_InspectNotification));
			RpcHandler.RegisterCommandDelegate("SendMailRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1435172201, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendMailRequest));
			RpcHandler.RegisterCommandDelegate("SendMailResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -58209770, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendMailResponse));
			RpcHandler.RegisterCommandDelegate("AcceptMailRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -928962019, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AcceptMailRequest));
			RpcHandler.RegisterCommandDelegate("AcceptMailResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 761166128, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AcceptMailResponse));
			RpcHandler.RegisterCommandDelegate("LogArmstrong", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1771114288, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_LogArmstrong));
			RpcHandler.RegisterCommandDelegate("SendReport", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1408766942, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendReport));
			RpcHandler.RegisterCommandDelegate("Server_AuctionHouse_AuctionList", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1598352336, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_AuctionHouse_AuctionList));
			RpcHandler.RegisterCommandDelegate("SendAuctionResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1998790690, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SendAuctionResponse));
			RpcHandler.RegisterCommandDelegate("Client_AuctionHouse_NewAuction", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1050879209, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_AuctionHouse_NewAuction));
			RpcHandler.RegisterCommandDelegate("Client_AuctionHouse_PlaceBid", typeof(PlayerRpcHandler), RpcType.ClientToServer, 339243336, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_AuctionHouse_PlaceBid));
			RpcHandler.RegisterCommandDelegate("Client_AuctionHouse_BuyItNow", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1019783178, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_AuctionHouse_BuyItNow));
			RpcHandler.RegisterCommandDelegate("Client_AuctionHouse_CancelAuction", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1008334374, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_AuctionHouse_CancelAuction));
			RpcHandler.RegisterCommandDelegate("MergeRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -510747324, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MergeRequest));
			RpcHandler.RegisterCommandDelegate("SplitRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1196278984, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SplitRequest));
			RpcHandler.RegisterCommandDelegate("TransferRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -493513046, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TransferRequest));
			RpcHandler.RegisterCommandDelegate("SwapRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 59918634, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SwapRequest));
			RpcHandler.RegisterCommandDelegate("TakeAllRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1547673192, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TakeAllRequest));
			RpcHandler.RegisterCommandDelegate("DestroyItemRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1329992561, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DestroyItemRequest));
			RpcHandler.RegisterCommandDelegate("DestroyMultiItemRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 992505585, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DestroyMultiItemRequest));
			RpcHandler.RegisterCommandDelegate("MerchantItemSellRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 236011962, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantItemSellRequest));
			RpcHandler.RegisterCommandDelegate("MerchantBuybackUpdateRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -2014142238, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantBuybackUpdateRequest));
			RpcHandler.RegisterCommandDelegate("MerchantPurchaseRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1782128149, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantPurchaseRequest));
			RpcHandler.RegisterCommandDelegate("MerchantBuybackRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1751600441, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantBuybackRequest));
			RpcHandler.RegisterCommandDelegate("BlacksmithItemRepairRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 567746561, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_BlacksmithItemRepairRequest));
			RpcHandler.RegisterCommandDelegate("BlacksmithContainerRepairRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1588305323, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_BlacksmithContainerRepairRequest));
			RpcHandler.RegisterCommandDelegate("DeconstructRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 682403610, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DeconstructRequest));
			RpcHandler.RegisterCommandDelegate("LearnAbilityRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -154907426, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_LearnAbilityRequest));
			RpcHandler.RegisterCommandDelegate("TrainSpecializationRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2096302217, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TrainSpecializationRequest));
			RpcHandler.RegisterCommandDelegate("ForgetSpecializationRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1924536384, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ForgetSpecializationRequest));
			RpcHandler.RegisterCommandDelegate("PurchaseContainerExpansionRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1359816363, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_PurchaseContainerExpansionRequest));
			RpcHandler.RegisterCommandDelegate("MergeRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1149873811, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MergeRequestResponse));
			RpcHandler.RegisterCommandDelegate("SplitRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1505132217, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SplitRequestResponse));
			RpcHandler.RegisterCommandDelegate("TransferRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1736144391, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TransferRequestResponse));
			RpcHandler.RegisterCommandDelegate("SwapRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 323744441, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SwapRequestResponse));
			RpcHandler.RegisterCommandDelegate("TakeAllRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1890873849, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TakeAllRequestResponse));
			RpcHandler.RegisterCommandDelegate("DestroyItemRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1103831550, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DestroyItemRequestResponse));
			RpcHandler.RegisterCommandDelegate("DestroyMultiItemRequestResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 2142754784, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DestroyMultiItemRequestResponse));
			RpcHandler.RegisterCommandDelegate("AddItemResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 734614732, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AddItemResponse));
			RpcHandler.RegisterCommandDelegate("AddRemoveItems", typeof(PlayerRpcHandler), RpcType.ServerToClient, -2026643213, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AddRemoveItems));
			RpcHandler.RegisterCommandDelegate("UpdateItemCount", typeof(PlayerRpcHandler), RpcType.ServerToClient, -504071272, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_UpdateItemCount));
			RpcHandler.RegisterCommandDelegate("LearnablesAdded", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1001766302, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_LearnablesAdded));
			RpcHandler.RegisterCommandDelegate("MerchantInventoryUpdate", typeof(PlayerRpcHandler), RpcType.ServerToClient, 357992779, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantInventoryUpdate));
			RpcHandler.RegisterCommandDelegate("MerchantBuybackInventoryUpdate", typeof(PlayerRpcHandler), RpcType.ServerToClient, -87533323, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantBuybackInventoryUpdate));
			RpcHandler.RegisterCommandDelegate("MerchantItemSellResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -889718254, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_MerchantItemSellResponse));
			RpcHandler.RegisterCommandDelegate("BlacksmithItemRepairResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -2127818826, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_BlacksmithItemRepairResponse));
			RpcHandler.RegisterCommandDelegate("BlacksmithContainerRepairResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1278363166, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_BlacksmithContainerRepairResponse));
			RpcHandler.RegisterCommandDelegate("TrainSpecializationResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 35437109, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TrainSpecializationResponse));
			RpcHandler.RegisterCommandDelegate("ForgetSpecializationResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1108426403, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ForgetSpecializationResponse));
			RpcHandler.RegisterCommandDelegate("PurchaseContainerExpansionResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 489358183, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_PurchaseContainerExpansionResponse));
			RpcHandler.RegisterCommandDelegate("DeconstructResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 217864364, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_DeconstructResponse));
			RpcHandler.RegisterCommandDelegate("OpenRemoteContainer", typeof(PlayerRpcHandler), RpcType.ServerToClient, 802438285, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_OpenRemoteContainer));
			RpcHandler.RegisterCommandDelegate("UpdateArchetypeInstanceLock", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1376374012, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_UpdateArchetypeInstanceLock));
			RpcHandler.RegisterCommandDelegate("ToggleReagent", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1056514907, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ToggleReagent));
			RpcHandler.RegisterCommandDelegate("Client_RequestExecuteUtility", typeof(PlayerRpcHandler), RpcType.ClientToServer, -237529261, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_RequestExecuteUtility));
			RpcHandler.RegisterCommandDelegate("Server_ExecuteUtilityResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1810121394, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_ExecuteUtilityResponse));
			RpcHandler.RegisterCommandDelegate("Server_UpdateClientAugment", typeof(PlayerRpcHandler), RpcType.ServerToClient, 623011239, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_UpdateClientAugment));
			RpcHandler.RegisterCommandDelegate("AssignEmberStone", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1663417641, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_AssignEmberStone));
			RpcHandler.RegisterCommandDelegate("UpdateEmberEssenceCount", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1155165155, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_UpdateEmberEssenceCount));
			RpcHandler.RegisterCommandDelegate("UpdateEmberEssenceCountForTravel", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1147091225, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_UpdateEmberEssenceCountForTravel));
			RpcHandler.RegisterCommandDelegate("PurchaseTravelEssence", typeof(PlayerRpcHandler), RpcType.ClientToServer, -932029047, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_PurchaseTravelEssence));
			RpcHandler.RegisterCommandDelegate("IncrementHuntingLog", typeof(PlayerRpcHandler), RpcType.ServerToClient, 2060107345, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_IncrementHuntingLog));
			RpcHandler.RegisterCommandDelegate("SelectHuntingLogPerk", typeof(PlayerRpcHandler), RpcType.ClientToServer, -313479755, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_SelectHuntingLogPerk));
			RpcHandler.RegisterCommandDelegate("ConfirmHuntingLogPerk", typeof(PlayerRpcHandler), RpcType.ServerToClient, 168958990, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_ConfirmHuntingLogPerk));
			RpcHandler.RegisterCommandDelegate("RespecHuntingLogRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -20754910, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RespecHuntingLogRequest));
			RpcHandler.RegisterCommandDelegate("RespecHuntingLogResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1399018367, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_RespecHuntingLogResponse));
			RpcHandler.RegisterCommandDelegate("NotifyBBClear", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1256085718, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_NotifyBBClear));
			RpcHandler.RegisterCommandDelegate("Client_Execution_Instant", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1243190222, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_Execution_Instant));
			RpcHandler.RegisterCommandDelegate("Client_Execution_Begin", typeof(PlayerRpcHandler), RpcType.ClientToServer, 768073826, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_Execution_Begin));
			RpcHandler.RegisterCommandDelegate("Client_Execution_Cancel", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1890319226, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_Execution_Cancel));
			RpcHandler.RegisterCommandDelegate("Client_Execution_Complete", typeof(PlayerRpcHandler), RpcType.ClientToServer, -723486673, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_Execution_Complete));
			RpcHandler.RegisterCommandDelegate("Client_DismissEffectRequest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 876927945, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_DismissEffectRequest));
			RpcHandler.RegisterCommandDelegate("Client_Execute_AutoAttack", typeof(PlayerRpcHandler), RpcType.ClientToServer, -412412423, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_Execute_AutoAttack));
			RpcHandler.RegisterCommandDelegate("Client_DismissActiveAura", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1010266720, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_DismissActiveAura));
			RpcHandler.RegisterCommandDelegate("Server_Execute_Instant", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -1175005518, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execute_Instant));
			RpcHandler.RegisterCommandDelegate("Server_Execute_Instant_Failed", typeof(PlayerRpcHandler), RpcType.ServerToClient, 573824098, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execute_Instant_Failed));
			RpcHandler.RegisterCommandDelegate("Server_Execution_Begin", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, 1432660161, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execution_Begin));
			RpcHandler.RegisterCommandDelegate("Server_Execution_BeginFailed", typeof(PlayerRpcHandler), RpcType.ServerToClient, 534092910, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execution_BeginFailed));
			RpcHandler.RegisterCommandDelegate("Server_Execution_Cancel", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -972748282, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execution_Cancel));
			RpcHandler.RegisterCommandDelegate("Server_Execution_Complete", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, -897782463, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execution_Complete));
			RpcHandler.RegisterCommandDelegate("Server_Execution_Complete_UpdateTarget", typeof(PlayerRpcHandler), RpcType.ServerBroadcast, 284955617, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execution_Complete_UpdateTarget));
			RpcHandler.RegisterCommandDelegate("Server_MasteryLevelChanged", typeof(PlayerRpcHandler), RpcType.ServerToClient, 2066966389, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_MasteryLevelChanged));
			RpcHandler.RegisterCommandDelegate("Server_MasteryAbilityLevelChanged", typeof(PlayerRpcHandler), RpcType.ServerToClient, -998007269, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_MasteryAbilityLevelChanged));
			RpcHandler.RegisterCommandDelegate("Server_LevelProgressionEvent", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1437771947, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_LevelProgressionEvent));
			RpcHandler.RegisterCommandDelegate("Server_LevelProgressionUpdate", typeof(PlayerRpcHandler), RpcType.ServerToClient, -121022329, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_LevelProgressionUpdate));
			RpcHandler.RegisterCommandDelegate("Server_Execute_AutoAttack_Failed", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1521387168, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_Execute_AutoAttack_Failed));
			RpcHandler.RegisterCommandDelegate("GM_SetMasteryLevel", typeof(PlayerRpcHandler), RpcType.ClientToServer, -855476682, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetMasteryLevel));
			RpcHandler.RegisterCommandDelegate("GM_SetTargetMasteryLevel", typeof(PlayerRpcHandler), RpcType.ClientToServer, -268498797, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetTargetMasteryLevel));
			RpcHandler.RegisterCommandDelegate("GM_Kill", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1746392490, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Kill));
			RpcHandler.RegisterCommandDelegate("GM_Heal", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1414214015, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Heal));
			RpcHandler.RegisterCommandDelegate("GM_HealStamina", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1254977680, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_HealStamina));
			RpcHandler.RegisterCommandDelegate("GM_HealWounds", typeof(PlayerRpcHandler), RpcType.ClientToServer, 632174633, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_HealWounds));
			RpcHandler.RegisterCommandDelegate("GM_AdjustPlayerFlags", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1184001640, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AdjustPlayerFlags));
			RpcHandler.RegisterCommandDelegate("GM_ResetQuests", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1678655430, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetQuests));
			RpcHandler.RegisterCommandDelegate("GM_ResetQuest", typeof(PlayerRpcHandler), RpcType.ClientToServer, 855097065, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetQuest));
			RpcHandler.RegisterCommandDelegate("GM_ResetTargetQuest", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1398880366, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetTargetQuest));
			RpcHandler.RegisterCommandDelegate("GM_ResetNpcKnowledge", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1450563964, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetNpcKnowledge));
			RpcHandler.RegisterCommandDelegate("GM_Learn", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1253262962, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Learn));
			RpcHandler.RegisterCommandDelegate("GM_Unlearn", typeof(PlayerRpcHandler), RpcType.ClientToServer, 442992167, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Unlearn));
			RpcHandler.RegisterCommandDelegate("GM_ResetLearnables", typeof(PlayerRpcHandler), RpcType.ClientToServer, 978014814, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetLearnables));
			RpcHandler.RegisterCommandDelegate("GM_ResetDiscoveries", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1788169021, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetDiscoveries));
			RpcHandler.RegisterCommandDelegate("GM_ResetZoneDiscoveries", typeof(PlayerRpcHandler), RpcType.ClientToServer, 196178281, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetZoneDiscoveries));
			RpcHandler.RegisterCommandDelegate("GM_DiscoverZone", typeof(PlayerRpcHandler), RpcType.ClientToServer, 45765027, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_DiscoverZone));
			RpcHandler.RegisterCommandDelegate("GM_RequestResetPosition", typeof(PlayerRpcHandler), RpcType.ClientToServer, -481322993, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RequestResetPosition));
			RpcHandler.RegisterCommandDelegate("GM_RequestResetTargetPositionByName", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1746940388, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RequestResetTargetPositionByName));
			RpcHandler.RegisterCommandDelegate("GM_RequestResetTargetPositionByEntity", typeof(PlayerRpcHandler), RpcType.ClientToServer, 450889685, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RequestResetTargetPositionByEntity));
			RpcHandler.RegisterCommandDelegate("GM_Summon", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1749038055, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Summon));
			RpcHandler.RegisterCommandDelegate("GM_SummonGroup", typeof(PlayerRpcHandler), RpcType.ClientToServer, 257729804, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SummonGroup));
			RpcHandler.RegisterCommandDelegate("GM_TeleportTo", typeof(PlayerRpcHandler), RpcType.ClientToServer, 114149606, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_TeleportTo));
			RpcHandler.RegisterCommandDelegate("GM_TeleportToCorpse", typeof(PlayerRpcHandler), RpcType.ClientToServer, 915656454, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_TeleportToCorpse));
			RpcHandler.RegisterCommandDelegate("GM_SetGameTime", typeof(PlayerRpcHandler), RpcType.ClientToServer, -775581464, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetGameTime));
			RpcHandler.RegisterCommandDelegate("GM_ResetGameTime", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1890687574, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetGameTime));
			RpcHandler.RegisterCommandDelegate("GM_AddCurrency", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1145536521, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddCurrency));
			RpcHandler.RegisterCommandDelegate("GM_AddEventCurrency", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2029962255, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddEventCurrency));
			RpcHandler.RegisterCommandDelegate("GM_AddNetworkEntityEventCurrency", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1110931611, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddNetworkEntityEventCurrency));
			RpcHandler.RegisterCommandDelegate("GM_AddTargetEventCurrency", typeof(PlayerRpcHandler), RpcType.ClientToServer, -117190846, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddTargetEventCurrency));
			RpcHandler.RegisterCommandDelegate("GM_AddEmberStone", typeof(PlayerRpcHandler), RpcType.ClientToServer, -285413375, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddEmberStone));
			RpcHandler.RegisterCommandDelegate("GM_RemoveEmberStone", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1987132316, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RemoveEmberStone));
			RpcHandler.RegisterCommandDelegate("GM_AlterEmberEssence", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1304271520, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AlterEmberEssence));
			RpcHandler.RegisterCommandDelegate("GM_AlterTravelEssence", typeof(PlayerRpcHandler), RpcType.ClientToServer, 325669177, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AlterTravelEssence));
			RpcHandler.RegisterCommandDelegate("GM_UpgradeEmberStone", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1357110558, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_UpgradeEmberStone));
			RpcHandler.RegisterCommandDelegate("GM_ModifyTitle", typeof(PlayerRpcHandler), RpcType.ClientToServer, 493162518, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ModifyTitle));
			RpcHandler.RegisterCommandDelegate("GM_ModifyTargetTitleByName", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1674385859, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ModifyTargetTitleByName));
			RpcHandler.RegisterCommandDelegate("GM_ModifyTargetTitleByEntity", typeof(PlayerRpcHandler), RpcType.ClientToServer, -787435742, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ModifyTargetTitleByEntity));
			RpcHandler.RegisterCommandDelegate("TitleModifiedResponse", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1188281895, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_TitleModifiedResponse));
			RpcHandler.RegisterCommandDelegate("GM_ResetAbilityTimers", typeof(PlayerRpcHandler), RpcType.ClientToServer, -592853019, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetAbilityTimers));
			RpcHandler.RegisterCommandDelegate("GM_GetNpcTickRate", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1775744048, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_GetNpcTickRate));
			RpcHandler.RegisterCommandDelegate("GM_SetNpcTickRate", typeof(PlayerRpcHandler), RpcType.ClientToServer, -382450981, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetNpcTickRate));
			RpcHandler.RegisterCommandDelegate("GM_GetNpcBucketSize", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2041766028, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_GetNpcBucketSize));
			RpcHandler.RegisterCommandDelegate("GM_SetNpcBucketSize", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1315032561, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetNpcBucketSize));
			RpcHandler.RegisterCommandDelegate("GM_GetPathfindingIterations", typeof(PlayerRpcHandler), RpcType.ClientToServer, -2108825460, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_GetPathfindingIterations));
			RpcHandler.RegisterCommandDelegate("GM_SetPathfindingIterations", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1184991325, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_SetPathfindingIterations));
			RpcHandler.RegisterCommandDelegate("GM_Disconnect", typeof(PlayerRpcHandler), RpcType.ClientToServer, 976695162, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_Disconnect));
			RpcHandler.RegisterCommandDelegate("GM_DisconnectAllCurrentZone", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1750559052, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_DisconnectAllCurrentZone));
			RpcHandler.RegisterCommandDelegate("GM_DisconnectAllTargetZone", typeof(PlayerRpcHandler), RpcType.ClientToServer, -2068083003, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_DisconnectAllTargetZone));
			RpcHandler.RegisterCommandDelegate("GM_DisconnectAll", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1887177783, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_DisconnectAll));
			RpcHandler.RegisterCommandDelegate("GM_ListRemoteSpawns", typeof(PlayerRpcHandler), RpcType.ClientToServer, 2131931770, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ListRemoteSpawns));
			RpcHandler.RegisterCommandDelegate("GM_ListRemoteNodes", typeof(PlayerRpcHandler), RpcType.ClientToServer, -511187303, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ListRemoteNodes));
			RpcHandler.RegisterCommandDelegate("GM_RemoteSpawnNpc", typeof(PlayerRpcHandler), RpcType.ClientToServer, -822204785, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RemoteSpawnNpc));
			RpcHandler.RegisterCommandDelegate("GM_RemoteSpawnNode", typeof(PlayerRpcHandler), RpcType.ClientToServer, -239004445, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RemoteSpawnNode));
			RpcHandler.RegisterCommandDelegate("GM_RemoveCorpse", typeof(PlayerRpcHandler), RpcType.ClientToServer, -113776008, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_RemoveCorpse));
			RpcHandler.RegisterCommandDelegate("GM_AddToHuntingLog", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1412803950, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_AddToHuntingLog));
			RpcHandler.RegisterCommandDelegate("GM_ResetHuntingLog", typeof(PlayerRpcHandler), RpcType.ClientToServer, 349879384, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetHuntingLog));
			RpcHandler.RegisterCommandDelegate("GM_ClearHuntingLog", typeof(PlayerRpcHandler), RpcType.ClientToServer, -539538836, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ClearHuntingLog));
			RpcHandler.RegisterCommandDelegate("GM_TriggerFireworks", typeof(PlayerRpcHandler), RpcType.ClientToServer, -691549882, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_TriggerFireworks));
			RpcHandler.RegisterCommandDelegate("GM_LearnAlchemyII", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1730372697, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_LearnAlchemyII));
			RpcHandler.RegisterCommandDelegate("GM_BBDrop", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1034152519, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_BBDrop));
			RpcHandler.RegisterCommandDelegate("GM_NotifyBBDrop", typeof(PlayerRpcHandler), RpcType.ServerToClient, -20962745, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_NotifyBBDrop));
			RpcHandler.RegisterCommandDelegate("GM_BBClear", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1574772615, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_BBClear));
			RpcHandler.RegisterCommandDelegate("GM_BBClearTarget", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1310965358, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_BBClearTarget));
			RpcHandler.RegisterCommandDelegate("GM_ReloadGameServerConfig", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1536835502, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ReloadGameServerConfig));
			RpcHandler.RegisterCommandDelegate("GM_GetNpcStats", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1180981944, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_GetNpcStats));
			RpcHandler.RegisterCommandDelegate("GM_GetActivatedMonolith", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1494770293, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_GetActivatedMonolith));
			RpcHandler.RegisterCommandDelegate("GM_ResetActivatedMonolith", typeof(PlayerRpcHandler), RpcType.ClientToServer, 744112614, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ResetActivatedMonolith));
			RpcHandler.RegisterCommandDelegate("GM_ReturnAuction", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1043307973, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_GM_ReturnAuction));
			RpcHandler.RegisterCommandDelegate("QA_RequestZoneToQA", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1769821620, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_QA_RequestZoneToQA));
			RpcHandler.RegisterCommandDelegate("QA_RequestZoneToPOI", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1958747132, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_QA_RequestZoneToPOI));
			RpcHandler.RegisterCommandDelegate("Server_RequestTrade_Response", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1358052384, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_RequestTrade_Response));
			RpcHandler.RegisterCommandDelegate("Server_RequestTrade", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1310159409, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_RequestTrade));
			RpcHandler.RegisterCommandDelegate("Server_CompleteTradeHandshake", typeof(PlayerRpcHandler), RpcType.ServerToClient, 508250884, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_CompleteTradeHandshake));
			RpcHandler.RegisterCommandDelegate("Server_TradeTransactionConcluded", typeof(PlayerRpcHandler), RpcType.ServerToClient, -2054250220, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TradeTransactionConcluded));
			RpcHandler.RegisterCommandDelegate("Server_TradeTermsAccepted", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1720681940, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TradeTermsAccepted));
			RpcHandler.RegisterCommandDelegate("Server_TradeItemAdded", typeof(PlayerRpcHandler), RpcType.ServerToClient, 511747974, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TradeItemAdded));
			RpcHandler.RegisterCommandDelegate("Server_TradeItemRemoved", typeof(PlayerRpcHandler), RpcType.ServerToClient, 171329702, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TradeItemRemoved));
			RpcHandler.RegisterCommandDelegate("Server_TradeItemsSwapped", typeof(PlayerRpcHandler), RpcType.ServerToClient, 1824072721, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_TradeItemsSwapped));
			RpcHandler.RegisterCommandDelegate("Server_ResetTradeAgreement", typeof(PlayerRpcHandler), RpcType.ServerToClient, 527547978, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_ResetTradeAgreement));
			RpcHandler.RegisterCommandDelegate("Server_CurrencyChanged", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1252864005, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_CurrencyChanged));
			RpcHandler.RegisterCommandDelegate("Server_ItemCountChanged", typeof(PlayerRpcHandler), RpcType.ServerToClient, -1580250234, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Server_ItemCountChanged));
			RpcHandler.RegisterCommandDelegate("Client_RequestTrade", typeof(PlayerRpcHandler), RpcType.ClientToServer, -1470714163, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_RequestTrade));
			RpcHandler.RegisterCommandDelegate("Client_ProposedTrade_Response", typeof(PlayerRpcHandler), RpcType.ClientToServer, 1493732902, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_ProposedTrade_Response));
			RpcHandler.RegisterCommandDelegate("Client_AcceptCancelTrade", typeof(PlayerRpcHandler), RpcType.ClientToServer, -637832368, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_AcceptCancelTrade));
			RpcHandler.RegisterCommandDelegate("Client_ResetTradeAgreement", typeof(PlayerRpcHandler), RpcType.ClientToServer, -679124354, new RpcHandler.CommandDelegate(PlayerRpcHandler.Invoke_Client_ResetTradeAgreement));
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x001178F4 File Offset: 0x00115AF4
		private static void Invoke_RequestZone(NetworkEntity target, BitBuffer buffer)
		{
			int targetZoneId = buffer.ReadInt();
			int targetZonePointIndex = buffer.ReadInt();
			int zonePointInstanceId = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).RequestZone(targetZoneId, targetZonePointIndex, zonePointInstanceId);
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0011792C File Offset: 0x00115B2C
		private static void Invoke_RequestZoneToDiscovery(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId sourceDiscoveryId = default(UniqueId);
			sourceDiscoveryId.ReadData(buffer);
			int targetZoneId = buffer.ReadInt();
			UniqueId targetDiscoveryId = default(UniqueId);
			targetDiscoveryId.ReadData(buffer);
			bool useTravelEssence = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).RequestZoneToDiscovery(sourceDiscoveryId, targetZoneId, targetDiscoveryId, useTravelEssence);
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x00117980 File Offset: 0x00115B80
		private static void Invoke_RequestZoneToGroup(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId sourceDiscoveryId = default(UniqueId);
			sourceDiscoveryId.ReadData(buffer);
			int targetZoneId = buffer.ReadInt();
			byte groupTeleportIndex = buffer.ReadByte();
			bool useTravelEssence = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).RequestZoneToGroup(sourceDiscoveryId, targetZoneId, groupTeleportIndex, useTravelEssence);
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RequestZone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x001179C8 File Offset: 0x00115BC8
		private static void Invoke_AuthorizeZone(NetworkEntity target, BitBuffer buffer)
		{
			bool authorized = buffer.ReadBool();
			int zoneId = buffer.ReadInt();
			int zonePointInstanceId = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).AuthorizeZone(authorized, zoneId, zonePointInstanceId);
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x00117A00 File Offset: 0x00115C00
		private static void Invoke_RequestZoneToMapDiscovery(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId discoveryId = default(UniqueId);
			discoveryId.ReadData(buffer);
			bool useTravelEssence = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).RequestZoneToMapDiscovery(discoveryId, useTravelEssence);
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x00117A38 File Offset: 0x00115C38
		private static void Invoke_Client_RequestInteraction(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Client_RequestInteraction(netEntityForId);
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x00117A6C File Offset: 0x00115C6C
		private static void Invoke_Server_RequestInteraction(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Server_RequestInteraction(netEntityForId);
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x00117AA0 File Offset: 0x00115CA0
		private static void Invoke_Client_CancelInteraction(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Client_CancelInteraction(netEntityForId);
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x00117AD4 File Offset: 0x00115CD4
		private static void Invoke_Server_CancelInteraction(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Server_CancelInteraction(netEntityForId);
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x00117B08 File Offset: 0x00115D08
		private static void Invoke_Client_RequestStateInteraction(NetworkEntity target, BitBuffer buffer)
		{
			int key = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).Client_RequestStateInteraction(key);
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x00117B30 File Offset: 0x00115D30
		private static void Invoke_SetTrackedMasteryOption(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId trackedMasteryOption = default(UniqueId);
			trackedMasteryOption.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SetTrackedMasteryOption(trackedMasteryOption);
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x00056860 File Offset: 0x00054A60
		private static void Invoke_Client_GiveUp(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).Client_GiveUp();
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x00056872 File Offset: 0x00054A72
		private static void Invoke_Server_ApplyDamageToGearForDeath(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).Server_ApplyDamageToGearForDeath();
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x00117B60 File Offset: 0x00115D60
		private static void Invoke_Server_Respawn(NetworkEntity target, BitBuffer buffer)
		{
			Vector3 pos = buffer.ReadVector3(NetworkManager.Range);
			float h = buffer.ReadFloat();
			((PlayerRpcHandler)target.RpcHandler).Server_Respawn(pos, h);
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x00117B94 File Offset: 0x00115D94
		private static void Invoke_SetGroupId(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId groupId = default(UniqueId);
			groupId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SetGroupId(groupId);
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x00117BC4 File Offset: 0x00115DC4
		private static void Invoke_SetRaidId(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId raidId = default(UniqueId);
			raidId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SetRaidId(raidId);
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x00056884 File Offset: 0x00054A84
		private static void Invoke_ForfeitInventoryRequest(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).ForfeitInventoryRequest();
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00117BF4 File Offset: 0x00115DF4
		private static void Invoke_ForfeitInventoryResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			((PlayerRpcHandler)target.RpcHandler).ForfeitInventoryResponse(op);
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x00056896 File Offset: 0x00054A96
		private static void Invoke_ClientJumped(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).ClientJumped();
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000568A8 File Offset: 0x00054AA8
		private static void Invoke_ClientJumpedBroadcast(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).ClientJumpedBroadcast();
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x00117C1C File Offset: 0x00115E1C
		private static void Invoke_SendChatNotification(NetworkEntity target, BitBuffer buffer)
		{
			string msg = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).SendChatNotification(msg);
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x00117C44 File Offset: 0x00115E44
		private static void Invoke_SendLongChatNotification(NetworkEntity target, BitBuffer buffer)
		{
			LongString msg = default(LongString);
			msg.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SendLongChatNotification(msg);
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x00117C74 File Offset: 0x00115E74
		private static void Invoke_TriggerCannotPerform(NetworkEntity target, BitBuffer buffer)
		{
			string chatNotification = buffer.ReadString();
			string overhead = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).TriggerCannotPerform(chatNotification, overhead);
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x00117CA4 File Offset: 0x00115EA4
		private static void Invoke_ForgetMastery(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ForgetMastery(instanceId);
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x00117CD4 File Offset: 0x00115ED4
		private static void Invoke_ProcessCurrencyTransaction(NetworkEntity target, BitBuffer buffer)
		{
			CurrencyTransaction transaction = default(CurrencyTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ProcessCurrencyTransaction(transaction);
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x00117D04 File Offset: 0x00115F04
		private static void Invoke_ProcessInteractiveStationCurrencyTransaction(NetworkEntity target, BitBuffer buffer)
		{
			ulong? inventoryCurrency = buffer.ReadNullableUlong();
			ulong? personalBankCurrency = buffer.ReadNullableUlong();
			((PlayerRpcHandler)target.RpcHandler).ProcessInteractiveStationCurrencyTransaction(inventoryCurrency, personalBankCurrency);
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00117D34 File Offset: 0x00115F34
		private static void Invoke_CurrencyTransferRequest(NetworkEntity target, BitBuffer buffer)
		{
			CurrencyTransaction toRemove = default(CurrencyTransaction);
			toRemove.ReadData(buffer);
			CurrencyTransaction toAdd = default(CurrencyTransaction);
			toAdd.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).CurrencyTransferRequest(toRemove, toAdd);
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x00117D78 File Offset: 0x00115F78
		private static void Invoke_CurrencyTransferRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			CurrencyTransaction toRemove = default(CurrencyTransaction);
			toRemove.ReadData(buffer);
			CurrencyTransaction toAdd = default(CurrencyTransaction);
			toAdd.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).CurrencyTransferRequestResponse(op, toRemove, toAdd);
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x00117DC4 File Offset: 0x00115FC4
		private static void Invoke_CurrencyModifyEvent(NetworkEntity target, BitBuffer buffer)
		{
			CurrencyModifyTransaction transaction = default(CurrencyModifyTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).CurrencyModifyEvent(transaction);
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x00117DF4 File Offset: 0x00115FF4
		private static void Invoke_ProcessMultiContainerCurrencyTransaction(NetworkEntity target, BitBuffer buffer)
		{
			MultiContainerCurrencyTransaction transaction = default(MultiContainerCurrencyTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ProcessMultiContainerCurrencyTransaction(transaction);
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00117E24 File Offset: 0x00116024
		private static void Invoke_ProcessEventCurrencyTransaction(NetworkEntity target, BitBuffer buffer)
		{
			ulong? eventCurrency = buffer.ReadNullableUlong();
			((PlayerRpcHandler)target.RpcHandler).ProcessEventCurrencyTransaction(eventCurrency);
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x00117E4C File Offset: 0x0011604C
		private static void Invoke_RequestObjectiveIteration(NetworkEntity target, BitBuffer buffer)
		{
			ObjectiveIterationCache cache = default(ObjectiveIterationCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).RequestObjectiveIteration(cache);
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x00117E7C File Offset: 0x0011607C
		private static void Invoke_NotifyObjectiveIteration(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			string message = buffer.ReadString();
			ObjectiveIterationCache cache = default(ObjectiveIterationCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).NotifyObjectiveIteration(opCode, message, cache);
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x00117EBC File Offset: 0x001160BC
		private static void Invoke_RequestRewardReissue(NetworkEntity target, BitBuffer buffer)
		{
			ObjectiveIterationCache cache = default(ObjectiveIterationCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).RequestRewardReissue(cache);
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x00117EEC File Offset: 0x001160EC
		private static void Invoke_NotifyGMQuestReset(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId questId = default(UniqueId);
			questId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).NotifyGMQuestReset(questId);
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x00117F1C File Offset: 0x0011611C
		private static void Invoke_MuteQuest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId questId = default(UniqueId);
			questId.ReadData(buffer);
			bool mute = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).MuteQuest(questId, mute);
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x00117F54 File Offset: 0x00116154
		private static void Invoke_RequestDropQuestsAndTasksForMastery(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).RequestDropQuestsAndTasksForMastery(archetypeId);
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x00117F84 File Offset: 0x00116184
		private static void Invoke_DrawBBTask(NetworkEntity target, BitBuffer buffer)
		{
			BBTaskDrawCache cache = default(BBTaskDrawCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DrawBBTask(cache);
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x00117FB4 File Offset: 0x001161B4
		private static void Invoke_NotifyDrawBBTask(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			string message = buffer.ReadString();
			BBTaskDrawCache cache = default(BBTaskDrawCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).NotifyDrawBBTask(opCode, message, cache);
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x00117FF4 File Offset: 0x001161F4
		private static void Invoke_IterateBBTask(NetworkEntity target, BitBuffer buffer)
		{
			ObjectiveIterationCache cache = default(ObjectiveIterationCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).IterateBBTask(cache);
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x00118024 File Offset: 0x00116224
		private static void Invoke_NotifyBBTaskIterated(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			string message = buffer.ReadString();
			ObjectiveIterationCache cache = default(ObjectiveIterationCache);
			cache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).NotifyBBTaskIterated(opCode, message, cache);
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x00118064 File Offset: 0x00116264
		private static void Invoke_RequestNpcLearn(NetworkEntity target, BitBuffer buffer)
		{
			NpcLearningCache npcLearningCache = new NpcLearningCache();
			npcLearningCache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).RequestNpcLearn(npcLearningCache);
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x00118090 File Offset: 0x00116290
		private static void Invoke_NotifyNpcLearn(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			string message = buffer.ReadString();
			NpcLearningCache npcLearningCache = new NpcLearningCache();
			npcLearningCache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).NotifyNpcLearn(opCode, message, npcLearningCache);
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x001180CC File Offset: 0x001162CC
		private static void Invoke_DiscoveryNotification(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId discoveryProfileId = default(UniqueId);
			discoveryProfileId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DiscoveryNotification(discoveryProfileId);
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x001180FC File Offset: 0x001162FC
		private static void Invoke_ModifyEquipmentAbsorbed(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			int absorbed = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).ModifyEquipmentAbsorbed(instanceId, absorbed);
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x000568BA File Offset: 0x00054ABA
		private static void Invoke_RemoteRefreshHighestLevelMastery(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).RemoteRefreshHighestLevelMastery();
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x00118134 File Offset: 0x00116334
		private static void Invoke_TakeFallDamage(NetworkEntity target, BitBuffer buffer)
		{
			float distanceFallen = buffer.ReadFloat();
			((PlayerRpcHandler)target.RpcHandler).TakeFallDamage(distanceFallen);
		}

		// Token: 0x06001D6F RID: 7535 RVA: 0x0011815C File Offset: 0x0011635C
		private static void Invoke_Server_TakeFallDamage(NetworkEntity target, BitBuffer buffer)
		{
			int damage = buffer.ReadInt();
			bool knockedOut = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).Server_TakeFallDamage(damage, knockedOut);
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x0011818C File Offset: 0x0011638C
		private static void Invoke_RequestTitleChange(NetworkEntity target, BitBuffer buffer)
		{
			string title = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).RequestTitleChange(title);
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x001181B4 File Offset: 0x001163B4
		private static void Invoke_LootRollResponse(NetworkEntity target, BitBuffer buffer)
		{
			LootRollItemResponse response = default(LootRollItemResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).LootRollResponse(response);
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x001181E4 File Offset: 0x001163E4
		private static void Invoke_PlayEmoteRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId emoteId = default(UniqueId);
			emoteId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).PlayEmoteRequest(emoteId);
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x00118214 File Offset: 0x00116414
		private static void Invoke_PlayEmoteResponse(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId emoteId = default(UniqueId);
			emoteId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).PlayEmoteResponse(emoteId);
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x000568CC File Offset: 0x00054ACC
		private static void Invoke_StuckRequest(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).StuckRequest();
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x000568DE File Offset: 0x00054ADE
		private static void Invoke_RopeRequest(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).RopeRequest();
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00118244 File Offset: 0x00116444
		private static void Invoke_StuckResponse(NetworkEntity target, BitBuffer buffer)
		{
			StuckTransaction stuckResponse = default(StuckTransaction);
			stuckResponse.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).StuckResponse(stuckResponse);
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00118274 File Offset: 0x00116474
		private static void Invoke_DuelRequest(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).DuelRequest(netEntityForId);
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x001182A8 File Offset: 0x001164A8
		private static void Invoke_DuelResponse(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId duelId = default(UniqueId);
			duelId.ReadData(buffer);
			bool response = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).DuelResponse(duelId, response);
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x001182E0 File Offset: 0x001164E0
		private static void Invoke_Server_DuelRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId duelId = default(UniqueId);
			duelId.ReadData(buffer);
			string sourceName = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Server_DuelRequest(duelId, sourceName);
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x000568F0 File Offset: 0x00054AF0
		private static void Invoke_RequestSelfCorpseDrag(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).RequestSelfCorpseDrag();
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x00118318 File Offset: 0x00116518
		private static void Invoke_RequestGroupMemberCorpseDrag(NetworkEntity target, BitBuffer buffer)
		{
			string targetName = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).RequestGroupMemberCorpseDrag(targetName);
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x00056902 File Offset: 0x00054B02
		private static void Invoke_ToggleGroupConsent(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).ToggleGroupConsent();
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x00118340 File Offset: 0x00116540
		private static void Invoke_ChangePortraitRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId portraitId = default(UniqueId);
			portraitId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ChangePortraitRequest(portraitId);
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x00118370 File Offset: 0x00116570
		private static void Invoke_InspectRequest(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).InspectRequest(netEntityForId);
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x001183A4 File Offset: 0x001165A4
		private static void Invoke_InspectResponse(NetworkEntity target, BitBuffer buffer)
		{
			InspectionTransaction inspectionTransaction = default(InspectionTransaction);
			inspectionTransaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).InspectResponse(inspectionTransaction);
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x001183D4 File Offset: 0x001165D4
		private static void Invoke_InspectNotification(NetworkEntity target, BitBuffer buffer)
		{
			string sourceName = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).InspectNotification(sourceName);
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x001183FC File Offset: 0x001165FC
		private static void Invoke_SendMailRequest(NetworkEntity target, BitBuffer buffer)
		{
			SendMailTransaction sendMailTransaction = default(SendMailTransaction);
			sendMailTransaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SendMailRequest(sendMailTransaction);
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x0011842C File Offset: 0x0011662C
		private static void Invoke_SendMailResponse(NetworkEntity target, BitBuffer buffer)
		{
			SendMailResponse sendMailResponse = default(SendMailResponse);
			sendMailResponse.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SendMailResponse(sendMailResponse);
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0011845C File Offset: 0x0011665C
		private static void Invoke_AcceptMailRequest(NetworkEntity target, BitBuffer buffer)
		{
			string mailId = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).AcceptMailRequest(mailId);
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x00118484 File Offset: 0x00116684
		private static void Invoke_AcceptMailResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			string errorOrId = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).AcceptMailResponse(opCode, errorOrId);
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x001184B4 File Offset: 0x001166B4
		private static void Invoke_LogArmstrong(NetworkEntity target, BitBuffer buffer)
		{
			int armstrongCount = buffer.ReadInt();
			int totalCount = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).LogArmstrong(armstrongCount, totalCount);
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x001184E4 File Offset: 0x001166E4
		private static void Invoke_SendReport(NetworkEntity target, BitBuffer buffer)
		{
			LongString report = default(LongString);
			report.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SendReport(report);
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x00118514 File Offset: 0x00116714
		private static void Invoke_Server_AuctionHouse_AuctionList(NetworkEntity target, BitBuffer buffer)
		{
			AuctionList allListings = default(AuctionList);
			allListings.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_AuctionHouse_AuctionList(allListings);
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x00118544 File Offset: 0x00116744
		private static void Invoke_SendAuctionResponse(NetworkEntity target, BitBuffer buffer)
		{
			AuctionResponse response = default(AuctionResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SendAuctionResponse(response);
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x00118574 File Offset: 0x00116774
		private static void Invoke_Client_AuctionHouse_NewAuction(NetworkEntity target, BitBuffer buffer)
		{
			AuctionRequest auctionRequest = default(AuctionRequest);
			auctionRequest.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_AuctionHouse_NewAuction(auctionRequest);
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x001185A4 File Offset: 0x001167A4
		private static void Invoke_Client_AuctionHouse_PlaceBid(NetworkEntity target, BitBuffer buffer)
		{
			string auctionId = buffer.ReadString();
			ulong newBid = buffer.ReadULong();
			((PlayerRpcHandler)target.RpcHandler).Client_AuctionHouse_PlaceBid(auctionId, newBid);
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x001185D4 File Offset: 0x001167D4
		private static void Invoke_Client_AuctionHouse_BuyItNow(NetworkEntity target, BitBuffer buffer)
		{
			string auctionId = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Client_AuctionHouse_BuyItNow(auctionId);
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x001185FC File Offset: 0x001167FC
		private static void Invoke_Client_AuctionHouse_CancelAuction(NetworkEntity target, BitBuffer buffer)
		{
			string auctionId = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Client_AuctionHouse_CancelAuction(auctionId);
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x00118624 File Offset: 0x00116824
		private static void Invoke_MergeRequest(NetworkEntity target, BitBuffer buffer)
		{
			MergeRequest request = default(MergeRequest);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).MergeRequest(request);
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x00118654 File Offset: 0x00116854
		private static void Invoke_SplitRequest(NetworkEntity target, BitBuffer buffer)
		{
			SplitRequest request = default(SplitRequest);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SplitRequest(request);
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x00118684 File Offset: 0x00116884
		private static void Invoke_TransferRequest(NetworkEntity target, BitBuffer buffer)
		{
			TransferRequest request = default(TransferRequest);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).TransferRequest(request);
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x001186B4 File Offset: 0x001168B4
		private static void Invoke_SwapRequest(NetworkEntity target, BitBuffer buffer)
		{
			SwapRequest request = default(SwapRequest);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SwapRequest(request);
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x001186E4 File Offset: 0x001168E4
		private static void Invoke_TakeAllRequest(NetworkEntity target, BitBuffer buffer)
		{
			TakeAllRequest request = default(TakeAllRequest);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).TakeAllRequest(request);
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x00118714 File Offset: 0x00116914
		private static void Invoke_DestroyItemRequest(NetworkEntity target, BitBuffer buffer)
		{
			ItemDestructionTransaction request = default(ItemDestructionTransaction);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DestroyItemRequest(request);
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x00118744 File Offset: 0x00116944
		private static void Invoke_DestroyMultiItemRequest(NetworkEntity target, BitBuffer buffer)
		{
			ItemMultiDestructionTransaction request = default(ItemMultiDestructionTransaction);
			request.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DestroyMultiItemRequest(request);
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x00118774 File Offset: 0x00116974
		private static void Invoke_MerchantItemSellRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			((PlayerRpcHandler)target.RpcHandler).MerchantItemSellRequest(itemInstanceId, sourceContainerType);
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x00056914 File Offset: 0x00054B14
		private static void Invoke_MerchantBuybackUpdateRequest(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).MerchantBuybackUpdateRequest();
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x001187AC File Offset: 0x001169AC
		private static void Invoke_MerchantPurchaseRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId itemId = default(UniqueId);
			itemId.ReadData(buffer);
			uint quantity = buffer.ReadUInt();
			((PlayerRpcHandler)target.RpcHandler).MerchantPurchaseRequest(itemId, quantity);
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x001187E4 File Offset: 0x001169E4
		private static void Invoke_MerchantBuybackRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).MerchantBuybackRequest(instanceId);
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00118814 File Offset: 0x00116A14
		private static void Invoke_BlacksmithItemRepairRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			((PlayerRpcHandler)target.RpcHandler).BlacksmithItemRepairRequest(itemInstanceId, sourceContainerType);
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0011884C File Offset: 0x00116A4C
		private static void Invoke_BlacksmithContainerRepairRequest(NetworkEntity target, BitBuffer buffer)
		{
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			((PlayerRpcHandler)target.RpcHandler).BlacksmithContainerRepairRequest(sourceContainerType);
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x00118874 File Offset: 0x00116A74
		private static void Invoke_DeconstructRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DeconstructRequest(itemInstanceId);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x001188A4 File Offset: 0x00116AA4
		private static void Invoke_LearnAbilityRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId abilityId = default(UniqueId);
			abilityId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).LearnAbilityRequest(abilityId);
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x001188D4 File Offset: 0x00116AD4
		private static void Invoke_TrainSpecializationRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId masteryInstanceId = default(UniqueId);
			masteryInstanceId.ReadData(buffer);
			UniqueId specializationArchetypeId = default(UniqueId);
			specializationArchetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).TrainSpecializationRequest(masteryInstanceId, specializationArchetypeId);
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x00118918 File Offset: 0x00116B18
		private static void Invoke_ForgetSpecializationRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId masteryInstanceId = default(UniqueId);
			masteryInstanceId.ReadData(buffer);
			UniqueId specializationArchetypeId = default(UniqueId);
			specializationArchetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ForgetSpecializationRequest(masteryInstanceId, specializationArchetypeId);
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x0011895C File Offset: 0x00116B5C
		private static void Invoke_PurchaseContainerExpansionRequest(NetworkEntity target, BitBuffer buffer)
		{
			string containerId = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).PurchaseContainerExpansionRequest(containerId);
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x00118984 File Offset: 0x00116B84
		private static void Invoke_MergeRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			MergeResponse response = default(MergeResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).MergeRequestResponse(response);
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x001189B4 File Offset: 0x00116BB4
		private static void Invoke_SplitRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			SplitResponse response = default(SplitResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SplitRequestResponse(response);
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x001189E4 File Offset: 0x00116BE4
		private static void Invoke_TransferRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			TransferResponse response = default(TransferResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).TransferRequestResponse(response);
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x00118A14 File Offset: 0x00116C14
		private static void Invoke_SwapRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			SwapResponse response = default(SwapResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).SwapRequestResponse(response);
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x00118A44 File Offset: 0x00116C44
		private static void Invoke_TakeAllRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			TakeAllResponse response = default(TakeAllResponse);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).TakeAllRequestResponse(response);
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x00118A74 File Offset: 0x00116C74
		private static void Invoke_DestroyItemRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			ItemDestructionTransaction response = default(ItemDestructionTransaction);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DestroyItemRequestResponse(response);
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x00118AA4 File Offset: 0x00116CA4
		private static void Invoke_DestroyMultiItemRequestResponse(NetworkEntity target, BitBuffer buffer)
		{
			ItemMultiDestructionTransaction response = default(ItemMultiDestructionTransaction);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DestroyMultiItemRequestResponse(response);
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x00118AD4 File Offset: 0x00116CD4
		private static void Invoke_AddItemResponse(NetworkEntity target, BitBuffer buffer)
		{
			ArchetypeAddedTransaction response = default(ArchetypeAddedTransaction);
			response.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).AddItemResponse(response);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x00118B04 File Offset: 0x00116D04
		private static void Invoke_AddRemoveItems(NetworkEntity target, BitBuffer buffer)
		{
			ArchetypeAddRemoveTransaction transaction = default(ArchetypeAddRemoveTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).AddRemoveItems(transaction);
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x00118B34 File Offset: 0x00116D34
		private static void Invoke_UpdateItemCount(NetworkEntity target, BitBuffer buffer)
		{
			ItemCountUpdatedTransaction transaction = default(ItemCountUpdatedTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).UpdateItemCount(transaction);
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x00118B64 File Offset: 0x00116D64
		private static void Invoke_LearnablesAdded(NetworkEntity target, BitBuffer buffer)
		{
			LearnablesAddedTransaction transaction = default(LearnablesAddedTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).LearnablesAdded(transaction);
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00118B94 File Offset: 0x00116D94
		private static void Invoke_MerchantInventoryUpdate(NetworkEntity target, BitBuffer buffer)
		{
			MerchantType merchantType = buffer.ReadEnum<MerchantType>();
			ForSaleItemIds forSaleItemIds = default(ForSaleItemIds);
			forSaleItemIds.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).MerchantInventoryUpdate(merchantType, forSaleItemIds);
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x00118BCC File Offset: 0x00116DCC
		private static void Invoke_MerchantBuybackInventoryUpdate(NetworkEntity target, BitBuffer buffer)
		{
			MerchantType merchantType = buffer.ReadEnum<MerchantType>();
			BuybackItemData buybackItemData = default(BuybackItemData);
			buybackItemData.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).MerchantBuybackInventoryUpdate(merchantType, buybackItemData);
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x00118C04 File Offset: 0x00116E04
		private static void Invoke_MerchantItemSellResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			ulong sellPrice = buffer.ReadULong();
			((PlayerRpcHandler)target.RpcHandler).MerchantItemSellResponse(op, itemInstanceId, sourceContainerType, sellPrice);
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x00118C4C File Offset: 0x00116E4C
		private static void Invoke_BlacksmithItemRepairResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			((PlayerRpcHandler)target.RpcHandler).BlacksmithItemRepairResponse(op, itemInstanceId, sourceContainerType);
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x00118C8C File Offset: 0x00116E8C
		private static void Invoke_BlacksmithContainerRepairResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			((PlayerRpcHandler)target.RpcHandler).BlacksmithContainerRepairResponse(op, sourceContainerType);
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x00118CBC File Offset: 0x00116EBC
		private static void Invoke_TrainSpecializationResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId masteryInstanceId = default(UniqueId);
			masteryInstanceId.ReadData(buffer);
			UniqueId specializationArchetypeId = default(UniqueId);
			specializationArchetypeId.ReadData(buffer);
			float specLevel = buffer.ReadFloat();
			((PlayerRpcHandler)target.RpcHandler).TrainSpecializationResponse(op, masteryInstanceId, specializationArchetypeId, specLevel);
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x00118D10 File Offset: 0x00116F10
		private static void Invoke_ForgetSpecializationResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId masteryInstanceId = default(UniqueId);
			masteryInstanceId.ReadData(buffer);
			UniqueId specializationArchetypeId = default(UniqueId);
			specializationArchetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).ForgetSpecializationResponse(op, masteryInstanceId, specializationArchetypeId);
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x00118D5C File Offset: 0x00116F5C
		private static void Invoke_PurchaseContainerExpansionResponse(NetworkEntity target, BitBuffer buffer)
		{
			PurchaseContainerExpansionTransaction transaction = default(PurchaseContainerExpansionTransaction);
			transaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).PurchaseContainerExpansionResponse(transaction);
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x00118D8C File Offset: 0x00116F8C
		private static void Invoke_DeconstructResponse(NetworkEntity target, BitBuffer buffer)
		{
			ItemDestructionTransaction destructionTransaction = default(ItemDestructionTransaction);
			destructionTransaction.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).DeconstructResponse(destructionTransaction);
		}

		// Token: 0x06001DB3 RID: 7603 RVA: 0x00118DBC File Offset: 0x00116FBC
		private static void Invoke_OpenRemoteContainer(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			ContainerRecord containerRecord = new ContainerRecord();
			containerRecord.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).OpenRemoteContainer(netEntityForId, containerRecord);
		}

		// Token: 0x06001DB4 RID: 7604 RVA: 0x00118DFC File Offset: 0x00116FFC
		private static void Invoke_UpdateArchetypeInstanceLock(NetworkEntity target, BitBuffer buffer)
		{
			string containerId = buffer.ReadString();
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			bool lockState = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).UpdateArchetypeInstanceLock(containerId, instanceId, lockState);
		}

		// Token: 0x06001DB5 RID: 7605 RVA: 0x00118E3C File Offset: 0x0011703C
		private static void Invoke_ToggleReagent(NetworkEntity target, BitBuffer buffer)
		{
			int index = buffer.ReadInt();
			bool value = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).ToggleReagent(index, value);
		}

		// Token: 0x06001DB6 RID: 7606 RVA: 0x00118E6C File Offset: 0x0011706C
		private static void Invoke_Client_RequestExecuteUtility(NetworkEntity target, BitBuffer buffer)
		{
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			UniqueId sourceItemInstanceId = default(UniqueId);
			sourceItemInstanceId.ReadData(buffer);
			ContainerType targetContainerType = buffer.ReadEnum<ContainerType>();
			UniqueId targetItemInstanceId = default(UniqueId);
			targetItemInstanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_RequestExecuteUtility(sourceContainerType, sourceItemInstanceId, targetContainerType, targetItemInstanceId);
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x00118EC0 File Offset: 0x001170C0
		private static void Invoke_Server_ExecuteUtilityResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId sourceArchetypeId = default(UniqueId);
			sourceArchetypeId.ReadData(buffer);
			ContainerType targetContainerType = buffer.ReadEnum<ContainerType>();
			UniqueId targetItemInstanceId = default(UniqueId);
			targetItemInstanceId.ReadData(buffer);
			bool consumed = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).Server_ExecuteUtilityResponse(op, sourceArchetypeId, targetContainerType, targetItemInstanceId, consumed);
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x00118F1C File Offset: 0x0011711C
		private static void Invoke_Server_UpdateClientAugment(NetworkEntity target, BitBuffer buffer)
		{
			ContainerType sourceContainerType = buffer.ReadEnum<ContainerType>();
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			AugmentUpdateInfo update = default(AugmentUpdateInfo);
			update.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_UpdateClientAugment(sourceContainerType, itemInstanceId, update);
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x00118F68 File Offset: 0x00117168
		private static void Invoke_AssignEmberStone(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId? stoneId = buffer.ReadNullableUniqueId();
			((PlayerRpcHandler)target.RpcHandler).AssignEmberStone(stoneId);
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x00118F90 File Offset: 0x00117190
		private static void Invoke_UpdateEmberEssenceCount(NetworkEntity target, BitBuffer buffer)
		{
			int updatedCount = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).UpdateEmberEssenceCount(updatedCount);
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x00118FB8 File Offset: 0x001171B8
		private static void Invoke_UpdateEmberEssenceCountForTravel(NetworkEntity target, BitBuffer buffer)
		{
			int updatedCount = buffer.ReadInt();
			int updatedTravelCount = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).UpdateEmberEssenceCountForTravel(updatedCount, updatedTravelCount);
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x00118FE8 File Offset: 0x001171E8
		private static void Invoke_PurchaseTravelEssence(NetworkEntity target, BitBuffer buffer)
		{
			int amount = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).PurchaseTravelEssence(amount);
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x00119010 File Offset: 0x00117210
		private static void Invoke_IncrementHuntingLog(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId profileId = default(UniqueId);
			profileId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).IncrementHuntingLog(profileId);
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x00119040 File Offset: 0x00117240
		private static void Invoke_SelectHuntingLogPerk(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId profileId = default(UniqueId);
			profileId.ReadData(buffer);
			HuntingLogPerkType perkType = buffer.ReadEnum<HuntingLogPerkType>();
			int threshold = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).SelectHuntingLogPerk(profileId, perkType, threshold);
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x00119080 File Offset: 0x00117280
		private static void Invoke_ConfirmHuntingLogPerk(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId profileId = default(UniqueId);
			profileId.ReadData(buffer);
			HuntingLogPerkType perkType = buffer.ReadEnum<HuntingLogPerkType>();
			int threshold = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).ConfirmHuntingLogPerk(op, profileId, perkType, threshold);
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x001190C8 File Offset: 0x001172C8
		private static void Invoke_RespecHuntingLogRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId profileId = default(UniqueId);
			profileId.ReadData(buffer);
			int backToThreshold = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).RespecHuntingLogRequest(profileId, backToThreshold);
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x00119100 File Offset: 0x00117300
		private static void Invoke_RespecHuntingLogResponse(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes op = buffer.ReadEnum<OpCodes>();
			UniqueId profileId = default(UniqueId);
			profileId.ReadData(buffer);
			int backToThreshold = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).RespecHuntingLogResponse(op, profileId, backToThreshold);
		}

		// Token: 0x06001DC2 RID: 7618 RVA: 0x00119140 File Offset: 0x00117340
		private static void Invoke_NotifyBBClear(NetworkEntity target, BitBuffer buffer)
		{
			OpCodes opCode = buffer.ReadEnum<OpCodes>();
			((PlayerRpcHandler)target.RpcHandler).NotifyBBClear(opCode);
		}

		// Token: 0x06001DC3 RID: 7619 RVA: 0x00119168 File Offset: 0x00117368
		private static void Invoke_Client_Execution_Instant(NetworkEntity target, BitBuffer buffer)
		{
			ClientExecutionCache clientExecutionCache = default(ClientExecutionCache);
			clientExecutionCache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_Execution_Instant(clientExecutionCache);
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x00119198 File Offset: 0x00117398
		private static void Invoke_Client_Execution_Begin(NetworkEntity target, BitBuffer buffer)
		{
			DateTime timestamp = buffer.ReadDateTime();
			ClientExecutionCache clientExecutionCache = default(ClientExecutionCache);
			clientExecutionCache.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_Execution_Begin(timestamp, clientExecutionCache);
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x001191D0 File Offset: 0x001173D0
		private static void Invoke_Client_Execution_Cancel(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_Execution_Cancel(archetypeId);
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x00119200 File Offset: 0x00117400
		private static void Invoke_Client_Execution_Complete(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			DateTime timestamp = buffer.ReadDateTime();
			((PlayerRpcHandler)target.RpcHandler).Client_Execution_Complete(archetypeId, timestamp);
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x00119238 File Offset: 0x00117438
		private static void Invoke_Client_DismissEffectRequest(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_DismissEffectRequest(instanceId);
		}

		// Token: 0x06001DC8 RID: 7624 RVA: 0x00119268 File Offset: 0x00117468
		private static void Invoke_Client_Execute_AutoAttack(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Client_Execute_AutoAttack(netEntityForId);
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x00056926 File Offset: 0x00054B26
		private static void Invoke_Client_DismissActiveAura(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).Client_DismissActiveAura();
		}

		// Token: 0x06001DCA RID: 7626 RVA: 0x0011929C File Offset: 0x0011749C
		private static void Invoke_Server_Execute_Instant(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			byte abilityLevel = buffer.ReadByte();
			((PlayerRpcHandler)target.RpcHandler).Server_Execute_Instant(archetypeId, netEntityForId, abilityLevel);
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x001192E8 File Offset: 0x001174E8
		private static void Invoke_Server_Execute_Instant_Failed(NetworkEntity target, BitBuffer buffer)
		{
			string message = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Server_Execute_Instant_Failed(message);
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x00119310 File Offset: 0x00117510
		private static void Invoke_Server_Execution_Begin(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			byte abilityLevel = buffer.ReadByte();
			AlchemyPowerLevel alchemyPowerLevel = buffer.ReadEnum<AlchemyPowerLevel>();
			((PlayerRpcHandler)target.RpcHandler).Server_Execution_Begin(archetypeId, netEntityForId, abilityLevel, alchemyPowerLevel);
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x00119368 File Offset: 0x00117568
		private static void Invoke_Server_Execution_BeginFailed(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			string message = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Server_Execution_BeginFailed(archetypeId, message);
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x001193A0 File Offset: 0x001175A0
		private static void Invoke_Server_Execution_Cancel(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_Execution_Cancel(archetypeId);
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x001193D0 File Offset: 0x001175D0
		private static void Invoke_Server_Execution_Complete(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_Execution_Complete(archetypeId);
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x00119400 File Offset: 0x00117600
		private static void Invoke_Server_Execution_Complete_UpdateTarget(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Server_Execution_Complete_UpdateTarget(archetypeId, netEntityForId);
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x00119444 File Offset: 0x00117644
		private static void Invoke_Server_MasteryLevelChanged(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId masteryArchetypeId = default(UniqueId);
			masteryArchetypeId.ReadData(buffer);
			float newLevel = buffer.ReadFloat();
			((PlayerRpcHandler)target.RpcHandler).Server_MasteryLevelChanged(masteryArchetypeId, newLevel);
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x0011947C File Offset: 0x0011767C
		private static void Invoke_Server_MasteryAbilityLevelChanged(NetworkEntity target, BitBuffer buffer)
		{
			InstanceNewLevelData newLevelData = default(InstanceNewLevelData);
			newLevelData.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_MasteryAbilityLevelChanged(newLevelData);
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x001194AC File Offset: 0x001176AC
		private static void Invoke_Server_LevelProgressionEvent(NetworkEntity target, BitBuffer buffer)
		{
			LevelProgressionEvent levelProgressionEvent = default(LevelProgressionEvent);
			levelProgressionEvent.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_LevelProgressionEvent(levelProgressionEvent);
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x001194DC File Offset: 0x001176DC
		private static void Invoke_Server_LevelProgressionUpdate(NetworkEntity target, BitBuffer buffer)
		{
			LevelProgressionUpdate levelProgressionUpdate = default(LevelProgressionUpdate);
			levelProgressionUpdate.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_LevelProgressionUpdate(levelProgressionUpdate);
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x0011950C File Offset: 0x0011770C
		private static void Invoke_Server_Execute_AutoAttack_Failed(NetworkEntity target, BitBuffer buffer)
		{
			string message = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).Server_Execute_AutoAttack_Failed(message);
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetMasteryLevel(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetTargetMasteryLevel(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Kill(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Heal(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_HealStamina(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_HealWounds(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AdjustPlayerFlags(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetQuests(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetQuest(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetTargetQuest(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetNpcKnowledge(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Learn(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Unlearn(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetLearnables(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetDiscoveries(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetZoneDiscoveries(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_DiscoverZone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RequestResetPosition(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RequestResetTargetPositionByName(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RequestResetTargetPositionByEntity(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DEA RID: 7658 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Summon(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SummonGroup(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_TeleportTo(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_TeleportToCorpse(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetGameTime(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetGameTime(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF0 RID: 7664 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddCurrency(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddEventCurrency(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddNetworkEntityEventCurrency(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddTargetEventCurrency(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddEmberStone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RemoveEmberStone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AlterEmberEssence(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AlterTravelEssence(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_UpgradeEmberStone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ModifyTitle(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ModifyTargetTitleByName(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ModifyTargetTitleByEntity(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x00119534 File Offset: 0x00117734
		private static void Invoke_TitleModifiedResponse(NetworkEntity target, BitBuffer buffer)
		{
			string title = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).TitleModifiedResponse(title);
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetAbilityTimers(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_GetNpcTickRate(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetNpcTickRate(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_GetNpcBucketSize(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetNpcBucketSize(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_GetPathfindingIterations(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_SetPathfindingIterations(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_Disconnect(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E05 RID: 7685 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_DisconnectAllCurrentZone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_DisconnectAllTargetZone(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_DisconnectAll(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ListRemoteSpawns(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ListRemoteNodes(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RemoteSpawnNpc(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0B RID: 7691 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RemoteSpawnNode(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_RemoveCorpse(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_AddToHuntingLog(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetHuntingLog(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ClearHuntingLog(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_TriggerFireworks(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_LearnAlchemyII(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_BBDrop(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_NotifyBBDrop(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_BBClear(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_BBClearTarget(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ReloadGameServerConfig(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E17 RID: 7703 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_GetNpcStats(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_GetActivatedMonolith(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ResetActivatedMonolith(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E1A RID: 7706 RVA: 0x0004475B File Offset: 0x0004295B
		private static void Invoke_GM_ReturnAuction(NetworkEntity target, BitBuffer buffer)
		{
		}

		// Token: 0x06001E1B RID: 7707 RVA: 0x00056938 File Offset: 0x00054B38
		private static void Invoke_QA_RequestZoneToQA(NetworkEntity target, BitBuffer buffer)
		{
			((PlayerRpcHandler)target.RpcHandler).QA_RequestZoneToQA();
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x0011955C File Offset: 0x0011775C
		private static void Invoke_QA_RequestZoneToPOI(NetworkEntity target, BitBuffer buffer)
		{
			string poiName = buffer.ReadString();
			((PlayerRpcHandler)target.RpcHandler).QA_RequestZoneToPOI(poiName);
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x00119584 File Offset: 0x00117784
		private static void Invoke_Server_RequestTrade_Response(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId clientTradeId = default(UniqueId);
			clientTradeId.ReadData(buffer);
			UniqueId newTradeId = default(UniqueId);
			newTradeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_RequestTrade_Response(clientTradeId, newTradeId);
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x001195C8 File Offset: 0x001177C8
		private static void Invoke_Server_RequestTrade(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Server_RequestTrade(tradeId, netEntityForId);
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x0011960C File Offset: 0x0011780C
		private static void Invoke_Server_CompleteTradeHandshake(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			bool proceed = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).Server_CompleteTradeHandshake(tradeId, proceed);
		}

		// Token: 0x06001E20 RID: 7712 RVA: 0x00119644 File Offset: 0x00117844
		private static void Invoke_Server_TradeTransactionConcluded(NetworkEntity target, BitBuffer buffer)
		{
			TakeAllResponse response = default(TakeAllResponse);
			response.ReadData(buffer);
			TradeCompletionCode code = buffer.ReadEnum<TradeCompletionCode>();
			((PlayerRpcHandler)target.RpcHandler).Server_TradeTransactionConcluded(response, code);
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x0011967C File Offset: 0x0011787C
		private static void Invoke_Server_TradeTermsAccepted(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Server_TradeTermsAccepted(tradeId, netEntityForId);
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x001196C0 File Offset: 0x001178C0
		private static void Invoke_Server_TradeItemAdded(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			ArchetypeInstance archetypeInstance = new ArchetypeInstance();
			archetypeInstance.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_TradeItemAdded(tradeId, archetypeInstance);
		}

		// Token: 0x06001E23 RID: 7715 RVA: 0x00119700 File Offset: 0x00117900
		private static void Invoke_Server_TradeItemRemoved(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_TradeItemRemoved(tradeId, instanceId);
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x00119744 File Offset: 0x00117944
		private static void Invoke_Server_TradeItemsSwapped(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			UniqueId instanceIdA = default(UniqueId);
			instanceIdA.ReadData(buffer);
			UniqueId instanceIdB = default(UniqueId);
			instanceIdB.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_TradeItemsSwapped(tradeId, instanceIdA, instanceIdB);
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x00119798 File Offset: 0x00117998
		private static void Invoke_Server_ResetTradeAgreement(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Server_ResetTradeAgreement(tradeId);
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x001197C8 File Offset: 0x001179C8
		private static void Invoke_Server_CurrencyChanged(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			ulong newValue = buffer.ReadULong();
			((PlayerRpcHandler)target.RpcHandler).Server_CurrencyChanged(tradeId, newValue);
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x00119800 File Offset: 0x00117A00
		private static void Invoke_Server_ItemCountChanged(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			UniqueId itemInstanceId = default(UniqueId);
			itemInstanceId.ReadData(buffer);
			int newCount = buffer.ReadInt();
			((PlayerRpcHandler)target.RpcHandler).Server_ItemCountChanged(tradeId, itemInstanceId, newCount);
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x0011984C File Offset: 0x00117A4C
		private static void Invoke_Client_RequestTrade(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId clientTradeId = default(UniqueId);
			clientTradeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			((PlayerRpcHandler)target.RpcHandler).Client_RequestTrade(clientTradeId, netEntityForId);
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x00119890 File Offset: 0x00117A90
		private static void Invoke_Client_ProposedTrade_Response(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			bool response = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).Client_ProposedTrade_Response(tradeId, response);
		}

		// Token: 0x06001E2A RID: 7722 RVA: 0x001198C8 File Offset: 0x00117AC8
		private static void Invoke_Client_AcceptCancelTrade(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			bool accept = buffer.ReadBool();
			((PlayerRpcHandler)target.RpcHandler).Client_AcceptCancelTrade(tradeId, accept);
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x00119900 File Offset: 0x00117B00
		private static void Invoke_Client_ResetTradeAgreement(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId tradeId = default(UniqueId);
			tradeId.ReadData(buffer);
			((PlayerRpcHandler)target.RpcHandler).Client_ResetTradeAgreement(tradeId);
		}

		// Token: 0x04002276 RID: 8822
		private PlayerCollectionController _playerCollectionController;

		// Token: 0x04002277 RID: 8823
		private bool m_zoneRequested;

		// Token: 0x04002278 RID: 8824
		private static readonly float[] m_stuckDistances = new float[]
		{
			10f,
			20f,
			40f,
			60f
		};

		// Token: 0x04002279 RID: 8825
		public const string kUnknownArgument = "UNKNOWN";

		// Token: 0x0400227A RID: 8826
		private static readonly object[] m_stuckArguments = new object[7];

		// Token: 0x0400227B RID: 8827
		private static readonly object[] m_ropeArguments = new object[12];

		// Token: 0x0400227C RID: 8828
		private const float kMaxInspectFrequency = 1f;

		// Token: 0x0400227D RID: 8829
		private float m_timeOfLastInspect = float.MinValue;

		// Token: 0x0400227E RID: 8830
		private static readonly string kInspectTooSoon = "Too soon to inspect! Please wait " + Mathf.FloorToInt(1f).ToString() + "s between inspections.";

		// Token: 0x0400227F RID: 8831
		private static readonly object[] m_mailCurrencyArguments = new object[6];

		// Token: 0x04002280 RID: 8832
		private static readonly object[] m_armstrongArguments = new object[8];

		// Token: 0x04002281 RID: 8833
		private Dictionary<UniqueId, ITransaction> m_transactions;

		// Token: 0x04002282 RID: 8834
		private ArchetypeInstance m_lootConfirmationInstance;

		// Token: 0x04002283 RID: 8835
		private const int kHash_RequestZone = 621828145;

		// Token: 0x04002284 RID: 8836
		private const int kHash_RequestZoneToDiscovery = 2013177559;

		// Token: 0x04002285 RID: 8837
		private const int kHash_RequestZoneToGroup = -189487217;

		// Token: 0x04002286 RID: 8838
		private const int kHash_GM_RequestZone = 332417141;

		// Token: 0x04002287 RID: 8839
		private const int kHash_AuthorizeZone = -1543406291;

		// Token: 0x04002288 RID: 8840
		private const int kHash_RequestZoneToMapDiscovery = 1231663422;

		// Token: 0x04002289 RID: 8841
		private const int kHash_Client_RequestInteraction = -1990732059;

		// Token: 0x0400228A RID: 8842
		private const int kHash_Server_RequestInteraction = 1926620825;

		// Token: 0x0400228B RID: 8843
		private const int kHash_Client_CancelInteraction = 1371032526;

		// Token: 0x0400228C RID: 8844
		private const int kHash_Server_CancelInteraction = 389710554;

		// Token: 0x0400228D RID: 8845
		private const int kHash_Client_RequestStateInteraction = -179190786;

		// Token: 0x0400228E RID: 8846
		private const int kHash_SetTrackedMasteryOption = -8957207;

		// Token: 0x0400228F RID: 8847
		private const int kHash_Client_GiveUp = 597638525;

		// Token: 0x04002290 RID: 8848
		private const int kHash_Server_ApplyDamageToGearForDeath = -55666921;

		// Token: 0x04002291 RID: 8849
		private const int kHash_Server_Respawn = -411380183;

		// Token: 0x04002292 RID: 8850
		private const int kHash_SetGroupId = -269373221;

		// Token: 0x04002293 RID: 8851
		private const int kHash_SetRaidId = -124764458;

		// Token: 0x04002294 RID: 8852
		private const int kHash_ForfeitInventoryRequest = 1884773577;

		// Token: 0x04002295 RID: 8853
		private const int kHash_ForfeitInventoryResponse = 820392702;

		// Token: 0x04002296 RID: 8854
		private const int kHash_ClientJumped = -983144067;

		// Token: 0x04002297 RID: 8855
		private const int kHash_ClientJumpedBroadcast = 1143853980;

		// Token: 0x04002298 RID: 8856
		private const int kHash_SendChatNotification = -1627622002;

		// Token: 0x04002299 RID: 8857
		private const int kHash_SendLongChatNotification = 1972585747;

		// Token: 0x0400229A RID: 8858
		private const int kHash_TriggerCannotPerform = 1993149079;

		// Token: 0x0400229B RID: 8859
		private const int kHash_ForgetMastery = -119425;

		// Token: 0x0400229C RID: 8860
		private const int kHash_ProcessCurrencyTransaction = 1832983623;

		// Token: 0x0400229D RID: 8861
		private const int kHash_ProcessInteractiveStationCurrencyTransaction = -592976503;

		// Token: 0x0400229E RID: 8862
		private const int kHash_CurrencyTransferRequest = 247163380;

		// Token: 0x0400229F RID: 8863
		private const int kHash_CurrencyTransferRequestResponse = -1251682354;

		// Token: 0x040022A0 RID: 8864
		private const int kHash_CurrencyModifyEvent = -1722530210;

		// Token: 0x040022A1 RID: 8865
		private const int kHash_ProcessMultiContainerCurrencyTransaction = 398795495;

		// Token: 0x040022A2 RID: 8866
		private const int kHash_ProcessEventCurrencyTransaction = -1136993507;

		// Token: 0x040022A3 RID: 8867
		private const int kHash_RequestObjectiveIteration = 1047154739;

		// Token: 0x040022A4 RID: 8868
		private const int kHash_NotifyObjectiveIteration = 1996453892;

		// Token: 0x040022A5 RID: 8869
		private const int kHash_RequestRewardReissue = -1931216826;

		// Token: 0x040022A6 RID: 8870
		private const int kHash_NotifyGMQuestReset = -382355451;

		// Token: 0x040022A7 RID: 8871
		private const int kHash_MuteQuest = -1937016485;

		// Token: 0x040022A8 RID: 8872
		private const int kHash_RequestDropQuestsAndTasksForMastery = 133330845;

		// Token: 0x040022A9 RID: 8873
		private const int kHash_DrawBBTask = 744560740;

		// Token: 0x040022AA RID: 8874
		private const int kHash_NotifyDrawBBTask = -172514876;

		// Token: 0x040022AB RID: 8875
		private const int kHash_IterateBBTask = -120975047;

		// Token: 0x040022AC RID: 8876
		private const int kHash_NotifyBBTaskIterated = -209396829;

		// Token: 0x040022AD RID: 8877
		private const int kHash_RequestNpcLearn = -1084803737;

		// Token: 0x040022AE RID: 8878
		private const int kHash_NotifyNpcLearn = -333753616;

		// Token: 0x040022AF RID: 8879
		private const int kHash_DiscoveryNotification = -2086276638;

		// Token: 0x040022B0 RID: 8880
		private const int kHash_ModifyEquipmentAbsorbed = -2016753354;

		// Token: 0x040022B1 RID: 8881
		private const int kHash_RemoteRefreshHighestLevelMastery = -1412194063;

		// Token: 0x040022B2 RID: 8882
		private const int kHash_TakeFallDamage = -2019658019;

		// Token: 0x040022B3 RID: 8883
		private const int kHash_Server_TakeFallDamage = -171816164;

		// Token: 0x040022B4 RID: 8884
		private const int kHash_RequestTitleChange = -99391108;

		// Token: 0x040022B5 RID: 8885
		private const int kHash_LootRollResponse = 1347211544;

		// Token: 0x040022B6 RID: 8886
		private const int kHash_PlayEmoteRequest = -57037076;

		// Token: 0x040022B7 RID: 8887
		private const int kHash_PlayEmoteResponse = -626409346;

		// Token: 0x040022B8 RID: 8888
		private const int kHash_StuckRequest = -1964341464;

		// Token: 0x040022B9 RID: 8889
		private const int kHash_RopeRequest = -1565556672;

		// Token: 0x040022BA RID: 8890
		private const int kHash_StuckResponse = 1501188591;

		// Token: 0x040022BB RID: 8891
		private const int kHash_DuelRequest = 1000303217;

		// Token: 0x040022BC RID: 8892
		private const int kHash_DuelResponse = 1676310227;

		// Token: 0x040022BD RID: 8893
		private const int kHash_Server_DuelRequest = 1169266588;

		// Token: 0x040022BE RID: 8894
		private const int kHash_RequestSelfCorpseDrag = -96600820;

		// Token: 0x040022BF RID: 8895
		private const int kHash_RequestGroupMemberCorpseDrag = 46416971;

		// Token: 0x040022C0 RID: 8896
		private const int kHash_ToggleGroupConsent = 2046457682;

		// Token: 0x040022C1 RID: 8897
		private const int kHash_ChangePortraitRequest = -1239755285;

		// Token: 0x040022C2 RID: 8898
		private const int kHash_InspectRequest = 1793832839;

		// Token: 0x040022C3 RID: 8899
		private const int kHash_InspectResponse = 1910680035;

		// Token: 0x040022C4 RID: 8900
		private const int kHash_InspectNotification = 1488160658;

		// Token: 0x040022C5 RID: 8901
		private const int kHash_SendMailRequest = 1435172201;

		// Token: 0x040022C6 RID: 8902
		private const int kHash_SendMailResponse = -58209770;

		// Token: 0x040022C7 RID: 8903
		private const int kHash_AcceptMailRequest = -928962019;

		// Token: 0x040022C8 RID: 8904
		private const int kHash_AcceptMailResponse = 761166128;

		// Token: 0x040022C9 RID: 8905
		private const int kHash_LogArmstrong = -1771114288;

		// Token: 0x040022CA RID: 8906
		private const int kHash_SendReport = -1408766942;

		// Token: 0x040022CB RID: 8907
		private const int kHash_Server_AuctionHouse_AuctionList = 1598352336;

		// Token: 0x040022CC RID: 8908
		private const int kHash_SendAuctionResponse = 1998790690;

		// Token: 0x040022CD RID: 8909
		private const int kHash_Client_AuctionHouse_NewAuction = -1050879209;

		// Token: 0x040022CE RID: 8910
		private const int kHash_Client_AuctionHouse_PlaceBid = 339243336;

		// Token: 0x040022CF RID: 8911
		private const int kHash_Client_AuctionHouse_BuyItNow = 1019783178;

		// Token: 0x040022D0 RID: 8912
		private const int kHash_Client_AuctionHouse_CancelAuction = -1008334374;

		// Token: 0x040022D1 RID: 8913
		private const int kHash_MergeRequest = -510747324;

		// Token: 0x040022D2 RID: 8914
		private const int kHash_SplitRequest = -1196278984;

		// Token: 0x040022D3 RID: 8915
		private const int kHash_TransferRequest = -493513046;

		// Token: 0x040022D4 RID: 8916
		private const int kHash_SwapRequest = 59918634;

		// Token: 0x040022D5 RID: 8917
		private const int kHash_TakeAllRequest = -1547673192;

		// Token: 0x040022D6 RID: 8918
		private const int kHash_DestroyItemRequest = 1329992561;

		// Token: 0x040022D7 RID: 8919
		private const int kHash_DestroyMultiItemRequest = 992505585;

		// Token: 0x040022D8 RID: 8920
		private const int kHash_MerchantItemSellRequest = 236011962;

		// Token: 0x040022D9 RID: 8921
		private const int kHash_MerchantBuybackUpdateRequest = -2014142238;

		// Token: 0x040022DA RID: 8922
		private const int kHash_MerchantPurchaseRequest = -1782128149;

		// Token: 0x040022DB RID: 8923
		private const int kHash_MerchantBuybackRequest = 1751600441;

		// Token: 0x040022DC RID: 8924
		private const int kHash_BlacksmithItemRepairRequest = 567746561;

		// Token: 0x040022DD RID: 8925
		private const int kHash_BlacksmithContainerRepairRequest = 1588305323;

		// Token: 0x040022DE RID: 8926
		private const int kHash_DeconstructRequest = 682403610;

		// Token: 0x040022DF RID: 8927
		private const int kHash_LearnAbilityRequest = -154907426;

		// Token: 0x040022E0 RID: 8928
		private const int kHash_TrainSpecializationRequest = 2096302217;

		// Token: 0x040022E1 RID: 8929
		private const int kHash_ForgetSpecializationRequest = 1924536384;

		// Token: 0x040022E2 RID: 8930
		private const int kHash_PurchaseContainerExpansionRequest = -1359816363;

		// Token: 0x040022E3 RID: 8931
		private const int kHash_MergeRequestResponse = -1149873811;

		// Token: 0x040022E4 RID: 8932
		private const int kHash_SplitRequestResponse = 1505132217;

		// Token: 0x040022E5 RID: 8933
		private const int kHash_TransferRequestResponse = -1736144391;

		// Token: 0x040022E6 RID: 8934
		private const int kHash_SwapRequestResponse = 323744441;

		// Token: 0x040022E7 RID: 8935
		private const int kHash_TakeAllRequestResponse = 1890873849;

		// Token: 0x040022E8 RID: 8936
		private const int kHash_DestroyItemRequestResponse = -1103831550;

		// Token: 0x040022E9 RID: 8937
		private const int kHash_DestroyMultiItemRequestResponse = 2142754784;

		// Token: 0x040022EA RID: 8938
		private const int kHash_AddItemResponse = 734614732;

		// Token: 0x040022EB RID: 8939
		private const int kHash_AddRemoveItems = -2026643213;

		// Token: 0x040022EC RID: 8940
		private const int kHash_UpdateItemCount = -504071272;

		// Token: 0x040022ED RID: 8941
		private const int kHash_LearnablesAdded = -1001766302;

		// Token: 0x040022EE RID: 8942
		private const int kHash_MerchantInventoryUpdate = 357992779;

		// Token: 0x040022EF RID: 8943
		private const int kHash_MerchantBuybackInventoryUpdate = -87533323;

		// Token: 0x040022F0 RID: 8944
		private const int kHash_MerchantItemSellResponse = -889718254;

		// Token: 0x040022F1 RID: 8945
		private const int kHash_BlacksmithItemRepairResponse = -2127818826;

		// Token: 0x040022F2 RID: 8946
		private const int kHash_BlacksmithContainerRepairResponse = -1278363166;

		// Token: 0x040022F3 RID: 8947
		private const int kHash_TrainSpecializationResponse = 35437109;

		// Token: 0x040022F4 RID: 8948
		private const int kHash_ForgetSpecializationResponse = -1108426403;

		// Token: 0x040022F5 RID: 8949
		private const int kHash_PurchaseContainerExpansionResponse = 489358183;

		// Token: 0x040022F6 RID: 8950
		private const int kHash_DeconstructResponse = 217864364;

		// Token: 0x040022F7 RID: 8951
		private const int kHash_OpenRemoteContainer = 802438285;

		// Token: 0x040022F8 RID: 8952
		private const int kHash_UpdateArchetypeInstanceLock = -1376374012;

		// Token: 0x040022F9 RID: 8953
		private const int kHash_ToggleReagent = -1056514907;

		// Token: 0x040022FA RID: 8954
		private const int kHash_Client_RequestExecuteUtility = -237529261;

		// Token: 0x040022FB RID: 8955
		private const int kHash_Server_ExecuteUtilityResponse = 1810121394;

		// Token: 0x040022FC RID: 8956
		private const int kHash_Server_UpdateClientAugment = 623011239;

		// Token: 0x040022FD RID: 8957
		private const int kHash_AssignEmberStone = 1663417641;

		// Token: 0x040022FE RID: 8958
		private const int kHash_UpdateEmberEssenceCount = -1155165155;

		// Token: 0x040022FF RID: 8959
		private const int kHash_UpdateEmberEssenceCountForTravel = -1147091225;

		// Token: 0x04002300 RID: 8960
		private const int kHash_PurchaseTravelEssence = -932029047;

		// Token: 0x04002301 RID: 8961
		private const int kHash_IncrementHuntingLog = 2060107345;

		// Token: 0x04002302 RID: 8962
		private const int kHash_SelectHuntingLogPerk = -313479755;

		// Token: 0x04002303 RID: 8963
		private const int kHash_ConfirmHuntingLogPerk = 168958990;

		// Token: 0x04002304 RID: 8964
		private const int kHash_RespecHuntingLogRequest = -20754910;

		// Token: 0x04002305 RID: 8965
		private const int kHash_RespecHuntingLogResponse = 1399018367;

		// Token: 0x04002306 RID: 8966
		private const int kHash_NotifyBBClear = -1256085718;

		// Token: 0x04002307 RID: 8967
		private const int kHash_Client_Execution_Instant = 1243190222;

		// Token: 0x04002308 RID: 8968
		private const int kHash_Client_Execution_Begin = 768073826;

		// Token: 0x04002309 RID: 8969
		private const int kHash_Client_Execution_Cancel = 1890319226;

		// Token: 0x0400230A RID: 8970
		private const int kHash_Client_Execution_Complete = -723486673;

		// Token: 0x0400230B RID: 8971
		private const int kHash_Client_DismissEffectRequest = 876927945;

		// Token: 0x0400230C RID: 8972
		private const int kHash_Client_Execute_AutoAttack = -412412423;

		// Token: 0x0400230D RID: 8973
		private const int kHash_Client_DismissActiveAura = 1010266720;

		// Token: 0x0400230E RID: 8974
		private const int kHash_Server_Execute_Instant = -1175005518;

		// Token: 0x0400230F RID: 8975
		private const int kHash_Server_Execute_Instant_Failed = 573824098;

		// Token: 0x04002310 RID: 8976
		private const int kHash_Server_Execution_Begin = 1432660161;

		// Token: 0x04002311 RID: 8977
		private const int kHash_Server_Execution_BeginFailed = 534092910;

		// Token: 0x04002312 RID: 8978
		private const int kHash_Server_Execution_Cancel = -972748282;

		// Token: 0x04002313 RID: 8979
		private const int kHash_Server_Execution_Complete = -897782463;

		// Token: 0x04002314 RID: 8980
		private const int kHash_Server_Execution_Complete_UpdateTarget = 284955617;

		// Token: 0x04002315 RID: 8981
		private const int kHash_Server_MasteryLevelChanged = 2066966389;

		// Token: 0x04002316 RID: 8982
		private const int kHash_Server_MasteryAbilityLevelChanged = -998007269;

		// Token: 0x04002317 RID: 8983
		private const int kHash_Server_LevelProgressionEvent = 1437771947;

		// Token: 0x04002318 RID: 8984
		private const int kHash_Server_LevelProgressionUpdate = -121022329;

		// Token: 0x04002319 RID: 8985
		private const int kHash_Server_Execute_AutoAttack_Failed = -1521387168;

		// Token: 0x0400231A RID: 8986
		private const int kHash_GM_SetMasteryLevel = -855476682;

		// Token: 0x0400231B RID: 8987
		private const int kHash_GM_SetTargetMasteryLevel = -268498797;

		// Token: 0x0400231C RID: 8988
		private const int kHash_GM_Kill = 1746392490;

		// Token: 0x0400231D RID: 8989
		private const int kHash_GM_Heal = -1414214015;

		// Token: 0x0400231E RID: 8990
		private const int kHash_GM_HealStamina = 1254977680;

		// Token: 0x0400231F RID: 8991
		private const int kHash_GM_HealWounds = 632174633;

		// Token: 0x04002320 RID: 8992
		private const int kHash_GM_AdjustPlayerFlags = -1184001640;

		// Token: 0x04002321 RID: 8993
		private const int kHash_GM_ResetQuests = 1678655430;

		// Token: 0x04002322 RID: 8994
		private const int kHash_GM_ResetQuest = 855097065;

		// Token: 0x04002323 RID: 8995
		private const int kHash_GM_ResetTargetQuest = -1398880366;

		// Token: 0x04002324 RID: 8996
		private const int kHash_GM_ResetNpcKnowledge = 1450563964;

		// Token: 0x04002325 RID: 8997
		private const int kHash_GM_Learn = 1253262962;

		// Token: 0x04002326 RID: 8998
		private const int kHash_GM_Unlearn = 442992167;

		// Token: 0x04002327 RID: 8999
		private const int kHash_GM_ResetLearnables = 978014814;

		// Token: 0x04002328 RID: 9000
		private const int kHash_GM_ResetDiscoveries = -1788169021;

		// Token: 0x04002329 RID: 9001
		private const int kHash_GM_ResetZoneDiscoveries = 196178281;

		// Token: 0x0400232A RID: 9002
		private const int kHash_GM_DiscoverZone = 45765027;

		// Token: 0x0400232B RID: 9003
		private const int kHash_GM_RequestResetPosition = -481322993;

		// Token: 0x0400232C RID: 9004
		private const int kHash_GM_RequestResetTargetPositionByName = 1746940388;

		// Token: 0x0400232D RID: 9005
		private const int kHash_GM_RequestResetTargetPositionByEntity = 450889685;

		// Token: 0x0400232E RID: 9006
		private const int kHash_GM_Summon = 1749038055;

		// Token: 0x0400232F RID: 9007
		private const int kHash_GM_SummonGroup = 257729804;

		// Token: 0x04002330 RID: 9008
		private const int kHash_GM_TeleportTo = 114149606;

		// Token: 0x04002331 RID: 9009
		private const int kHash_GM_TeleportToCorpse = 915656454;

		// Token: 0x04002332 RID: 9010
		private const int kHash_GM_SetGameTime = -775581464;

		// Token: 0x04002333 RID: 9011
		private const int kHash_GM_ResetGameTime = 1890687574;

		// Token: 0x04002334 RID: 9012
		private const int kHash_GM_AddCurrency = -1145536521;

		// Token: 0x04002335 RID: 9013
		private const int kHash_GM_AddEventCurrency = 2029962255;

		// Token: 0x04002336 RID: 9014
		private const int kHash_GM_AddNetworkEntityEventCurrency = -1110931611;

		// Token: 0x04002337 RID: 9015
		private const int kHash_GM_AddTargetEventCurrency = -117190846;

		// Token: 0x04002338 RID: 9016
		private const int kHash_GM_AddEmberStone = -285413375;

		// Token: 0x04002339 RID: 9017
		private const int kHash_GM_RemoveEmberStone = -1987132316;

		// Token: 0x0400233A RID: 9018
		private const int kHash_GM_AlterEmberEssence = -1304271520;

		// Token: 0x0400233B RID: 9019
		private const int kHash_GM_AlterTravelEssence = 325669177;

		// Token: 0x0400233C RID: 9020
		private const int kHash_GM_UpgradeEmberStone = -1357110558;

		// Token: 0x0400233D RID: 9021
		private const int kHash_GM_ModifyTitle = 493162518;

		// Token: 0x0400233E RID: 9022
		private const int kHash_GM_ModifyTargetTitleByName = 1674385859;

		// Token: 0x0400233F RID: 9023
		private const int kHash_GM_ModifyTargetTitleByEntity = -787435742;

		// Token: 0x04002340 RID: 9024
		private const int kHash_TitleModifiedResponse = 1188281895;

		// Token: 0x04002341 RID: 9025
		private const int kHash_GM_ResetAbilityTimers = -592853019;

		// Token: 0x04002342 RID: 9026
		private const int kHash_GM_GetNpcTickRate = 1775744048;

		// Token: 0x04002343 RID: 9027
		private const int kHash_GM_SetNpcTickRate = -382450981;

		// Token: 0x04002344 RID: 9028
		private const int kHash_GM_GetNpcBucketSize = 2041766028;

		// Token: 0x04002345 RID: 9029
		private const int kHash_GM_SetNpcBucketSize = -1315032561;

		// Token: 0x04002346 RID: 9030
		private const int kHash_GM_GetPathfindingIterations = -2108825460;

		// Token: 0x04002347 RID: 9031
		private const int kHash_GM_SetPathfindingIterations = 1184991325;

		// Token: 0x04002348 RID: 9032
		private const int kHash_GM_Disconnect = 976695162;

		// Token: 0x04002349 RID: 9033
		private const int kHash_GM_DisconnectAllCurrentZone = -1750559052;

		// Token: 0x0400234A RID: 9034
		private const int kHash_GM_DisconnectAllTargetZone = -2068083003;

		// Token: 0x0400234B RID: 9035
		private const int kHash_GM_DisconnectAll = 1887177783;

		// Token: 0x0400234C RID: 9036
		private const int kHash_GM_ListRemoteSpawns = 2131931770;

		// Token: 0x0400234D RID: 9037
		private const int kHash_GM_ListRemoteNodes = -511187303;

		// Token: 0x0400234E RID: 9038
		private const int kHash_GM_RemoteSpawnNpc = -822204785;

		// Token: 0x0400234F RID: 9039
		private const int kHash_GM_RemoteSpawnNode = -239004445;

		// Token: 0x04002350 RID: 9040
		private const int kHash_GM_RemoveCorpse = -113776008;

		// Token: 0x04002351 RID: 9041
		private const int kHash_GM_AddToHuntingLog = 1412803950;

		// Token: 0x04002352 RID: 9042
		private const int kHash_GM_ResetHuntingLog = 349879384;

		// Token: 0x04002353 RID: 9043
		private const int kHash_GM_ClearHuntingLog = -539538836;

		// Token: 0x04002354 RID: 9044
		private const int kHash_GM_TriggerFireworks = -691549882;

		// Token: 0x04002355 RID: 9045
		private const int kHash_GM_LearnAlchemyII = 1730372697;

		// Token: 0x04002356 RID: 9046
		private const int kHash_GM_BBDrop = 1034152519;

		// Token: 0x04002357 RID: 9047
		private const int kHash_GM_NotifyBBDrop = -20962745;

		// Token: 0x04002358 RID: 9048
		private const int kHash_GM_BBClear = 1574772615;

		// Token: 0x04002359 RID: 9049
		private const int kHash_GM_BBClearTarget = -1310965358;

		// Token: 0x0400235A RID: 9050
		private const int kHash_GM_ReloadGameServerConfig = 1536835502;

		// Token: 0x0400235B RID: 9051
		private const int kHash_GM_GetNpcStats = -1180981944;

		// Token: 0x0400235C RID: 9052
		private const int kHash_GM_GetActivatedMonolith = -1494770293;

		// Token: 0x0400235D RID: 9053
		private const int kHash_GM_ResetActivatedMonolith = 744112614;

		// Token: 0x0400235E RID: 9054
		private const int kHash_GM_ReturnAuction = 1043307973;

		// Token: 0x0400235F RID: 9055
		private const int kHash_QA_RequestZoneToQA = 1769821620;

		// Token: 0x04002360 RID: 9056
		private const int kHash_QA_RequestZoneToPOI = 1958747132;

		// Token: 0x04002361 RID: 9057
		private const int kHash_Server_RequestTrade_Response = 1358052384;

		// Token: 0x04002362 RID: 9058
		private const int kHash_Server_RequestTrade = 1310159409;

		// Token: 0x04002363 RID: 9059
		private const int kHash_Server_CompleteTradeHandshake = 508250884;

		// Token: 0x04002364 RID: 9060
		private const int kHash_Server_TradeTransactionConcluded = -2054250220;

		// Token: 0x04002365 RID: 9061
		private const int kHash_Server_TradeTermsAccepted = -1720681940;

		// Token: 0x04002366 RID: 9062
		private const int kHash_Server_TradeItemAdded = 511747974;

		// Token: 0x04002367 RID: 9063
		private const int kHash_Server_TradeItemRemoved = 171329702;

		// Token: 0x04002368 RID: 9064
		private const int kHash_Server_TradeItemsSwapped = 1824072721;

		// Token: 0x04002369 RID: 9065
		private const int kHash_Server_ResetTradeAgreement = 527547978;

		// Token: 0x0400236A RID: 9066
		private const int kHash_Server_CurrencyChanged = -1252864005;

		// Token: 0x0400236B RID: 9067
		private const int kHash_Server_ItemCountChanged = -1580250234;

		// Token: 0x0400236C RID: 9068
		private const int kHash_Client_RequestTrade = -1470714163;

		// Token: 0x0400236D RID: 9069
		private const int kHash_Client_ProposedTrade_Response = 1493732902;

		// Token: 0x0400236E RID: 9070
		private const int kHash_Client_AcceptCancelTrade = -637832368;

		// Token: 0x0400236F RID: 9071
		private const int kHash_Client_ResetTradeAgreement = -679124354;

		// Token: 0x0200040E RID: 1038
		public struct PlayerUserData
		{
			// Token: 0x04002370 RID: 9072
			public string UserId;

			// Token: 0x04002371 RID: 9073
			public string CharacterId;

			// Token: 0x04002372 RID: 9074
			public string CharacterName;
		}

		// Token: 0x0200040F RID: 1039
		[Serializable]
		private struct SlackMessage
		{
			// Token: 0x04002373 RID: 9075
			public string text;
		}
	}
}
