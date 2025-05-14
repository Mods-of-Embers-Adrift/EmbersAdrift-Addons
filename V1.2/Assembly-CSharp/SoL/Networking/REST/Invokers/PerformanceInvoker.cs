using System;
using System.Diagnostics;
using RESTfulHTTPServer.src.controller;
using RESTfulHTTPServer.src.models;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.REST.Invokers
{
	// Token: 0x02000406 RID: 1030
	public static class PerformanceInvoker
	{
		// Token: 0x06001B32 RID: 6962 RVA: 0x0010B880 File Offset: 0x00109A80
		public static void RegisterInvokers(RoutingManager routingManager)
		{
			Route route = new Route(Route.Type.GET, "/unloadUnusedAssets", "PerformanceInvoker.UnloadUnusedAssets");
			routingManager.AddRoute(route);
			Route route2 = new Route(Route.Type.GET, "/forceGarbageCollection", "PerformanceInvoker.ForceGarbageCollection");
			routingManager.AddRoute(route2);
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x0010B8C0 File Offset: 0x00109AC0
		public static Response UnloadUnusedAssets(Request request)
		{
			Response response = new Response();
			string responseData = string.Empty;
			MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
			{
				UnityEngine.Debug.Log("Unloading Unused Assets...");
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				Resources.UnloadUnusedAssets();
				stopwatch.Stop();
				responseData = "Resources.UnloadUnusedAssets ran in " + stopwatch.ElapsedMilliseconds.ToString() + "ms";
			});
			while (responseData.Equals(string.Empty))
			{
			}
			response.SetContent(responseData);
			response.SetMimeType("application/json");
			response.SetHTTPStatusCode(200);
			return response;
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x0010B930 File Offset: 0x00109B30
		public static Response ForceGarbageCollection(Request request)
		{
			Response response = new Response();
			string responseData = string.Empty;
			MainThreadDispatcher.ExecuteOnMainThread.Enqueue(delegate
			{
				UnityEngine.Debug.Log("Forcing Garbage Collection...");
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				GC.Collect();
				stopwatch.Stop();
				responseData = "GC.Collect ran in " + stopwatch.ElapsedMilliseconds.ToString() + "ms";
			});
			while (responseData.Equals(string.Empty))
			{
			}
			response.SetContent(responseData);
			response.SetMimeType("application/json");
			response.SetHTTPStatusCode(200);
			return response;
		}

		// Token: 0x0400226B RID: 8811
		public const string kUnloadUnusedAssetsURL = "unloadUnusedAssets";

		// Token: 0x0400226C RID: 8812
		public const string kForceGarbageCollectionURL = "forceGarbageCollection";
	}
}
