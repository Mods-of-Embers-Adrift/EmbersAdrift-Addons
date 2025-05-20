using System;
using RESTfulHTTPServer.src.controller;
using RESTfulHTTPServer.src.models;
using SoL.Networking.Managers;

namespace SoL.Networking.REST.Invokers
{
	// Token: 0x02000409 RID: 1033
	public static class StatusInvoker
	{
		// Token: 0x06001B39 RID: 6969 RVA: 0x0010BA48 File Offset: 0x00109C48
		public static void RegisterInvokers(RoutingManager routingManager)
		{
			Route route = new Route(Route.Type.GET, "/status", "StatusInvoker.GetStatus");
			routingManager.AddRoute(route);
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x0010BA70 File Offset: 0x00109C70
		public static Response GetStatus(Request request)
		{
			Response response = new Response();
			string content = string.Empty;
			foreach (string text in request.GetQuerys().Keys)
			{
				string query = request.GetQuery(text);
				Logger.Log("Server Endpoint", "key: " + text + " , value: " + query);
			}
			content = BaseNetworkEntityManager.PlayerConnectedCount.ToString() + " connected";
			response.SetContent(content);
			response.SetMimeType("application/json");
			response.SetHTTPStatusCode(200);
			return response;
		}

		// Token: 0x0400226F RID: 8815
		private const string TAG = "Server Endpoint";
	}
}
