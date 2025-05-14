using System;
using RESTfulHTTPServer.src.controller;
using RESTfulHTTPServer.src.models;
using SoL.Game.Dungeons;
using SoL.Utilities;

namespace SoL.Networking.REST.Invokers
{
	// Token: 0x02000404 RID: 1028
	public static class DungeonInvoker
	{
		// Token: 0x06001B2E RID: 6958 RVA: 0x0010B7E8 File Offset: 0x001099E8
		public static void RegisterInvokers(RoutingManager routingManager)
		{
			Route route = new Route(Route.Type.GET, "/refreshDungeons", "DungeonInvoker.RefreshDungeons");
			routingManager.AddRoute(route);
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x0010B810 File Offset: 0x00109A10
		public static Response RefreshDungeons(Request request)
		{
			Response response = new Response();
			string responseData = string.Empty;
			MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
			{
				if (OverworldDungeonEntranceSpawnManager.Instance)
				{
					OverworldDungeonEntranceSpawnManager.Instance.ExternalRefresh();
					responseData = "Refreshed";
				}
			});
			while (responseData.Equals(string.Empty))
			{
			}
			response.SetContent(responseData);
			response.SetMimeType("application/json");
			response.SetHTTPStatusCode(200);
			return response;
		}

		// Token: 0x04002269 RID: 8809
		public const string kRefreshDungeonsURL = "refreshDungeons";
	}
}
