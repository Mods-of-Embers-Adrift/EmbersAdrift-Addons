using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RESTfulHTTPServer.src.controller;
using RESTfulHTTPServer.src.models;
using SoL.Game.Objects;
using SoL.Game.Quests;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.REST.Invokers
{
	// Token: 0x02000400 RID: 1024
	public static class CrossZoneInvoker
	{
		// Token: 0x06001B21 RID: 6945 RVA: 0x0010B2F4 File Offset: 0x001094F4
		public static void RegisterInvokers(RoutingManager routingManager)
		{
			Route route = new Route(Route.Type.POST, "/summonPlayer", "CrossZoneInvoker.SummonPlayer");
			routingManager.AddRoute(route);
			Route route2 = new Route(Route.Type.POST, "/getPlayerLocation", "CrossZoneInvoker.GetPlayerLocation");
			routingManager.AddRoute(route2);
			Route route3 = new Route(Route.Type.POST, "/resetTargetQuest", "CrossZoneInvoker.ResetTargetQuest");
			routingManager.AddRoute(route3);
			Route route4 = new Route(Route.Type.POST, "/disconnectPlayer", "CrossZoneInvoker.DisconnectPlayer");
			routingManager.AddRoute(route4);
			Route route5 = new Route(Route.Type.GET, "/disconnectAll", "CrossZoneInvoker.DisconnectAll");
			routingManager.AddRoute(route5);
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x0010B37C File Offset: 0x0010957C
		public static Response SummonPlayer(Request request)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.GetPOSTData());
			Response response = new Response();
			response.SetMimeType("application/json");
			string playerName;
			string value;
			if (dictionary.TryGetValue("PlayerName", out playerName) && dictionary.TryGetValue("Location", out value))
			{
				try
				{
					CharacterLocation newLocation = JsonConvert.DeserializeObject<CharacterLocation>(value);
					MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
					{
						NetworkEntity networkEntity;
						if (ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out networkEntity))
						{
							networkEntity.GameEntity.ServerPlayerController.ZonePlayerToCustomLocation(newLocation);
							networkEntity.PlayerRpcHandler.SendChatNotification("You have been summoned!");
							networkEntity.PlayerRpcHandler.AuthorizeZone(true, newLocation.ZoneId, -1);
						}
					});
					response.SetHTTPStatusCode(200);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Invalid NewLocation! " + ex.ToString());
					response.SetHTTPStatusCode(404);
				}
			}
			return response;
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x0010B434 File Offset: 0x00109634
		public static Response GetPlayerLocation(Request request)
		{
			string postdata = request.GetPOSTData();
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(postdata);
			Response response = new Response();
			response.SetMimeType("application/json");
			string text;
			NetworkEntity networkEntity;
			if (dictionary.TryGetValue("PlayerName", out text) && ServerNetworkEntityManager.TryGetNetworkEntityByName(text, out networkEntity))
			{
				Transform transform = networkEntity.gameObject.transform;
				Vector3 position = transform.position;
				CharacterLocation value = new CharacterLocation
				{
					ZoneId = LocalZoneManager.ZoneRecord.ZoneId,
					h = transform.rotation.y,
					x = position.x,
					y = position.y,
					z = position.z
				};
				Dictionary<string, string> value2 = new Dictionary<string, string>
				{
					{
						"PlayerName",
						text
					},
					{
						"Location",
						JsonConvert.SerializeObject(value)
					}
				};
				response.SetContent(JsonConvert.SerializeObject(value2));
				response.SetHTTPStatusCode(200);
			}
			else
			{
				Debug.LogWarning("UNABLE TO FIND KEY PlayerName!  " + postdata);
				response.SetHTTPStatusCode(404);
			}
			return response;
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x0010B540 File Offset: 0x00109740
		public static Response ResetTargetQuest(Request request)
		{
			string postdata = request.GetPOSTData();
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(postdata);
			Response response = new Response();
			string text;
			if (!dictionary.TryGetValue("TargetName", out text))
			{
				Debug.LogWarning("UNABLE TO FIND KEY TargetName!  " + postdata);
				response.SetHTTPStatusCode(404);
				return response;
			}
			string value;
			if (!dictionary.TryGetValue("QuestId", out value))
			{
				Debug.LogWarning("UNABLE TO FIND KEY QuestId!  " + postdata);
				response.SetHTTPStatusCode(404);
				return response;
			}
			NetworkEntity networkEntity;
			if (!ServerNetworkEntityManager.TryGetNetworkEntityByName(text, out networkEntity))
			{
				Debug.LogWarning("UNABLE TO FIND PLAYER " + text + "!");
				response.SetHTTPStatusCode(404);
				return response;
			}
			UniqueId uniqueId = new UniqueId(value);
			Quest quest;
			if (!InternalGameDatabase.Quests.TryGetItem(uniqueId, out quest))
			{
				Debug.LogWarning(string.Format("UNABLE TO FIND QUEST {0}!", uniqueId));
				response.SetHTTPStatusCode(404);
				return response;
			}
			GameManager.QuestManager.ResetQuest(networkEntity.GameEntity, quest);
			networkEntity.PlayerRpcHandler.NotifyGMQuestReset(uniqueId);
			response.SetHTTPStatusCode(200);
			return response;
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x0010B650 File Offset: 0x00109850
		public static Response DisconnectPlayer(Request request)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.GetPOSTData());
			Response response = new Response();
			response.SetMimeType("application/json");
			NetworkEntity netEntity;
			string playerName;
			if (dictionary.TryGetValue("PlayerName", out playerName) && ServerNetworkEntityManager.TryGetNetworkEntityByName(playerName, out netEntity))
			{
				MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
				{
					ServerNetworkEntityManager.DisconnectNetworkEntity(netEntity);
				});
				response.SetHTTPStatusCode(200);
			}
			else
			{
				response.SetHTTPStatusCode(404);
			}
			return response;
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x0010B6CC File Offset: 0x001098CC
		public static Response DisconnectAll(Request request)
		{
			int num = (BaseNetworkEntityManager.Peers != null) ? BaseNetworkEntityManager.Peers.Count : 0;
			MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
			{
				ServerNetworkEntityManager.DisconnectAll();
			});
			int num2 = (LocalZoneManager.ZoneRecord != null) ? LocalZoneManager.ZoneRecord.ZoneId : 0;
			Dictionary<string, string> value = new Dictionary<string, string>
			{
				{
					"ZoneId",
					num2.ToString()
				},
				{
					"Disconnected",
					num.ToString()
				}
			};
			Response response = new Response();
			response.SetMimeType("application/json");
			response.SetContent(JsonConvert.SerializeObject(value));
			HttpStatusCode httpstatusCode = (num > 0) ? HttpStatusCode.OK : HttpStatusCode.NotFound;
			response.SetHTTPStatusCode((int)httpstatusCode);
			return response;
		}

		// Token: 0x04002259 RID: 8793
		public const string kSummonPlayerURL = "summonPlayer";

		// Token: 0x0400225A RID: 8794
		public const string kGetPlayerLocationURL = "getPlayerLocation";

		// Token: 0x0400225B RID: 8795
		public const string kResetTargetQuest = "resetTargetQuest";

		// Token: 0x0400225C RID: 8796
		public const string kDisconnectPlayer = "disconnectPlayer";

		// Token: 0x0400225D RID: 8797
		public const string kDisconnectAll = "disconnectAll";

		// Token: 0x0400225E RID: 8798
		public const string kPlayerNameKey = "PlayerName";

		// Token: 0x0400225F RID: 8799
		public const string kLocationKey = "Location";

		// Token: 0x04002260 RID: 8800
		public const string kTargetNameKey = "TargetName";

		// Token: 0x04002261 RID: 8801
		public const string kQuestIdKey = "QuestId";

		// Token: 0x04002262 RID: 8802
		public const string kDisconnectCountKey = "Disconnected";

		// Token: 0x04002263 RID: 8803
		public const string kZoneIdKey = "ZoneId";
	}
}
