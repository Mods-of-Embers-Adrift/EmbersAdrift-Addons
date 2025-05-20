using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ENet;
using SoL.Game;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000538 RID: 1336
	public static class CorpseManager
	{
		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x0600288B RID: 10379 RVA: 0x0005C2AD File Offset: 0x0005A4AD
		private static GameObject CorpsePrefab
		{
			get
			{
				if (CorpseManager.m_corpsePrefabs == null)
				{
					CorpseManager.m_corpsePrefabs = GameManager.NetworkManager.NetworkedPrefabs.GetPrefabForIdOrName(GlobalSettings.Values.Player.PlayerCorpse.name);
				}
				return CorpseManager.m_corpsePrefabs;
			}
		}

		// Token: 0x0600288C RID: 10380 RVA: 0x0013C2D4 File Offset: 0x0013A4D4
		public static void SpawnWorldCorpse(CharacterRecord record, NetworkEntity owner)
		{
			if (record.Corpse == null || LocalZoneManager.ZoneRecord.ZoneId != record.Corpse.Location.ZoneId)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(CorpseManager.CorpsePrefab);
			NetworkEntity component = gameObject.GetComponent<NetworkEntity>();
			component.ServerInit(default(Peer), true, false);
			Corpse component2 = gameObject.GetComponent<Corpse>();
			if (component2.Initialize(record, owner))
			{
				ServerGameManager.ServerNetworkEntityManager.SpawnNetworkEntityForRemoteClients(component);
				if (CorpseManager.m_activeCorpses == null)
				{
					CorpseManager.m_activeCorpses = new Dictionary<string, Corpse>();
				}
				CorpseManager.m_activeCorpses.Add(record.Id, component2);
			}
		}

		// Token: 0x0600288D RID: 10381 RVA: 0x0013C368 File Offset: 0x0013A568
		public static void RemoveWorldCorpse(CharacterRecord record)
		{
			Corpse corpse;
			if (CorpseManager.m_activeCorpses != null && CorpseManager.m_activeCorpses.TryGetValue(record.Id, out corpse))
			{
				UnityEngine.Object.Destroy(corpse.gameObject);
				CorpseManager.m_activeCorpses.Remove(record.Id);
			}
		}

		// Token: 0x0600288E RID: 10382 RVA: 0x0013C3AC File Offset: 0x0013A5AC
		public static bool TryGetCorpsePosition(string playerName, out Vector3 pos)
		{
			pos = Vector3.zero;
			if (CorpseManager.m_activeCorpses == null || CorpseManager.m_activeCorpses.Count <= 0)
			{
				return false;
			}
			foreach (KeyValuePair<string, Corpse> keyValuePair in CorpseManager.m_activeCorpses)
			{
				if (keyValuePair.Value.NameMatches(playerName))
				{
					pos = keyValuePair.Value.GetPosition();
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600288F RID: 10383 RVA: 0x0013C440 File Offset: 0x0013A640
		public static void RemoveCorpseForPlayer(GameEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (entity.CharacterData)
			{
				entity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.MissingBag;
			}
			if (entity.CollectionController != null && entity.CollectionController.Record != null)
			{
				entity.CollectionController.Record.Corpse = null;
				entity.CollectionController.Record.UpdateCorpseData(ExternalGameDatabase.Database);
				CorpseManager.RemoveWorldCorpse(entity.CollectionController.Record);
			}
		}

		// Token: 0x06002890 RID: 10384 RVA: 0x0005C2E9 File Offset: 0x0005A4E9
		private static bool IsTooSoon()
		{
			return Time.time - CorpseManager.m_timeOfLastDrag <= 1f;
		}

		// Token: 0x06002891 RID: 10385 RVA: 0x0013C4CC File Offset: 0x0013A6CC
		public static void Client_AttemptToDragCorpse(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				CorpseManager.DragSelfOrClosestGroupCorpse();
				return;
			}
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null)
			{
				return;
			}
			string text = args[0].Trim().ToLowerInvariant();
			CorpseManager.Client_AttemptToDragCorpse((text != LocalPlayer.GameEntity.CharacterData.Name.Value.ToLowerInvariant()) ? text : string.Empty);
		}

		// Token: 0x06002892 RID: 10386 RVA: 0x0013C544 File Offset: 0x0013A744
		public static void DragSelfOrClosestGroupCorpse()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (LocalPlayer.Corpse && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
			{
				float sqrMagnitude = (LocalPlayer.Corpse.gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude;
				flag = true;
				flag2 = (sqrMagnitude <= 64f);
			}
			GroupMember groupMember = null;
			float num = float.MaxValue;
			if ((!flag || !flag2) && ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped)
			{
				Collider[] colliders = Hits.Colliders100;
				int num2 = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, 24f, colliders, LayerMap.Interaction.LayerMask, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < num2; i++)
				{
					CorpseSyncVarReplicator corpseSyncVarReplicator;
					GroupMember groupMember2;
					if (colliders[i] && colliders[i].gameObject && colliders[i].gameObject.TryGetComponent<CorpseSyncVarReplicator>(out corpseSyncVarReplicator) && corpseSyncVarReplicator.CorpseData != null && ClientGameManager.GroupManager && ClientGameManager.GroupManager.TryGetGroupMember(corpseSyncVarReplicator.CorpseData.Value.CharacterName, out groupMember2))
					{
						float sqrMagnitude2 = (colliders[i].gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							groupMember = groupMember2;
						}
					}
				}
			}
			if (flag && flag2)
			{
				if (CorpseManager.IsTooSoon())
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, CorpseManager.kTooSoon);
					return;
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestSelfCorpseDrag();
				CorpseManager.m_timeOfLastDrag = Time.time;
				return;
			}
			else if (groupMember != null)
			{
				if (CorpseManager.IsTooSoon())
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, CorpseManager.kTooSoon);
					return;
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestGroupMemberCorpseDrag(groupMember.Name);
				CorpseManager.m_timeOfLastDrag = Time.time;
				return;
			}
			else
			{
				if (flag)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Your bag is too far away to drag!");
					return;
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No bag to drag!");
				return;
			}
		}

		// Token: 0x06002893 RID: 10387 RVA: 0x0013C7C8 File Offset: 0x0013A9C8
		private static void Client_AttemptToDragCorpse(string targetName)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(targetName))
			{
				if (LocalPlayer.Corpse == null || !LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No bag to drag!");
					return;
				}
				float sqrMagnitude = (LocalPlayer.Corpse.gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude;
				if (sqrMagnitude > 64f)
				{
					string content = (sqrMagnitude <= 576f) ? "Bag is too far away to drag!" : "You cannot find a bag nearby!";
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
					return;
				}
				if (CorpseManager.IsTooSoon())
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, CorpseManager.kTooSoon);
					return;
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestSelfCorpseDrag();
				CorpseManager.m_timeOfLastDrag = Time.time;
				return;
			}
			else
			{
				GroupMember groupMember;
				if (!ClientGameManager.GroupManager.TryGetGroupMember(targetName, out groupMember))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not a member of your group!");
					return;
				}
				Collider[] colliders = Hits.Colliders100;
				bool flag = false;
				bool flag2 = false;
				int num = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, 24f, colliders, LayerMap.Interaction.LayerMask, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < num; i++)
				{
					if (colliders[i] && colliders[i].gameObject)
					{
						CorpseSyncVarReplicator component = colliders[i].gameObject.GetComponent<CorpseSyncVarReplicator>();
						if (component && component.CorpseData != null && component.CorpseData.Value.CharacterName.Equals(targetName, StringComparison.InvariantCultureIgnoreCase))
						{
							float sqrMagnitude2 = (component.gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude;
							if (sqrMagnitude2 > 64f)
							{
								string content2 = (sqrMagnitude2 <= 576f) ? "Bag is too far away to drag!" : "You cannot find a bag nearby!";
								MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content2);
								return;
							}
							flag = true;
							if (CorpseManager.IsTooSoon())
							{
								flag2 = true;
								break;
							}
							LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestGroupMemberCorpseDrag(targetName);
							CorpseManager.m_timeOfLastDrag = Time.time;
							break;
						}
					}
				}
				if (!flag)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No bag to drag!");
					return;
				}
				if (flag2)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, CorpseManager.kTooSoon);
				}
				return;
			}
		}

		// Token: 0x06002894 RID: 10388 RVA: 0x0013CA88 File Offset: 0x0013AC88
		public static void Server_AttemptToDragCorpse(GameEntity dragEntity, GameEntity corpseEntity)
		{
			CorpseManager.<Server_AttemptToDragCorpse>d__24 <Server_AttemptToDragCorpse>d__;
			<Server_AttemptToDragCorpse>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Server_AttemptToDragCorpse>d__.dragEntity = dragEntity;
			<Server_AttemptToDragCorpse>d__.corpseEntity = corpseEntity;
			<Server_AttemptToDragCorpse>d__.<>1__state = -1;
			<Server_AttemptToDragCorpse>d__.<>t__builder.Start<CorpseManager.<Server_AttemptToDragCorpse>d__24>(ref <Server_AttemptToDragCorpse>d__);
		}

		// Token: 0x040029BA RID: 10682
		private static Dictionary<string, Corpse> m_activeCorpses = null;

		// Token: 0x040029BB RID: 10683
		private static GameObject m_corpsePrefabs = null;

		// Token: 0x040029BC RID: 10684
		private const float kMaxDragDistance = 8f;

		// Token: 0x040029BD RID: 10685
		private const float kMaxDragDistanceSqr = 64f;

		// Token: 0x040029BE RID: 10686
		private const float kTooFarDistance = 24f;

		// Token: 0x040029BF RID: 10687
		private const float kTooFarDistanceSqr = 576f;

		// Token: 0x040029C0 RID: 10688
		private const string kNoBagToDrag = "No bag to drag!";

		// Token: 0x040029C1 RID: 10689
		private const string kBagIsTooFar = "Bag is too far away to drag!";

		// Token: 0x040029C2 RID: 10690
		private const string kCannotFindNearby = "You cannot find a bag nearby!";

		// Token: 0x040029C3 RID: 10691
		private const string kCannotDragBagNav = "You cannot drag this bag to here!";

		// Token: 0x040029C4 RID: 10692
		private const string kNotAMemberOfYourGroup = "Not a member of your group!";

		// Token: 0x040029C5 RID: 10693
		private static readonly string kTooSoon = "Too soon to drag a bag! Please wait " + Mathf.FloorToInt(1f).ToString() + "s between drags.";

		// Token: 0x040029C6 RID: 10694
		private const float kMaxFrequency = 1f;

		// Token: 0x040029C7 RID: 10695
		private static float m_timeOfLastDrag = float.MinValue;
	}
}
