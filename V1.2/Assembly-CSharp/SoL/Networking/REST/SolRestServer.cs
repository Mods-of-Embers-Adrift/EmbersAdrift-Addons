using System;
using RESTfulHTTPServer.src.controller;
using RESTfulHTTPServer.src.models;
using SoL.Managers;
using SoL.Networking.REST.Invokers;
using UnityEngine;

namespace SoL.Networking.REST
{
	// Token: 0x020003FF RID: 1023
	public class SolRestServer : MonoBehaviour
	{
		// Token: 0x06001B1D RID: 6941 RVA: 0x00055099 File Offset: 0x00053299
		private void OnDestroy()
		{
			SimpleRESTServer restServer = this.m_restServer;
			if (restServer == null)
			{
				return;
			}
			restServer.Stop();
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x0010B280 File Offset: 0x00109480
		public void Initialize(int port)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			string empty = string.Empty;
			string empty2 = string.Empty;
			this.SetupRoutes();
			if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty2))
			{
				RESTfulHTTPServer.src.controller.Logger.Log("Server Init", "Create basic auth");
				BasicAuth basicAuth = new BasicAuth(empty, empty2);
				this.m_restServer = new SimpleRESTServer(port, this.m_routingManager, basicAuth);
				return;
			}
			this.m_restServer = new SimpleRESTServer(port, this.m_routingManager);
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x000550AB File Offset: 0x000532AB
		private void SetupRoutes()
		{
			this.m_routingManager = new RoutingManager();
			StatusInvoker.RegisterInvokers(this.m_routingManager);
			DungeonInvoker.RegisterInvokers(this.m_routingManager);
			CrossZoneInvoker.RegisterInvokers(this.m_routingManager);
			PerformanceInvoker.RegisterInvokers(this.m_routingManager);
		}

		// Token: 0x04002256 RID: 8790
		private const string TAG = "Server Init";

		// Token: 0x04002257 RID: 8791
		private RoutingManager m_routingManager;

		// Token: 0x04002258 RID: 8792
		private SimpleRESTServer m_restServer;
	}
}
