using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RESTfulHTTPServer.src.models;
using SoL.Game;
using SoL.Networking.Database;
using UnityEngine.Networking;

namespace SoL.Networking.REST
{
	// Token: 0x020003FB RID: 1019
	public static class ServerCommunicator
	{
		// Token: 0x06001B15 RID: 6933 RVA: 0x0010ADE4 File Offset: 0x00108FE4
		public static void GET(ZoneId targetZone, string url, Action<UnityWebRequest> callback = null)
		{
			ServerCommunicator.SendRequest(new ServerCommunicator.CommunicationData
			{
				URL = url,
				TargetZoneId = targetZone,
				Type = Route.Type.GET,
				Fields = null,
				Callback = callback
			});
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x0010AE28 File Offset: 0x00109028
		public static void POST(ZoneId targetZone, string url, Dictionary<string, string> fields, Action<UnityWebRequest> callback = null)
		{
			ServerCommunicator.SendRequest(new ServerCommunicator.CommunicationData
			{
				URL = url,
				TargetZoneId = targetZone,
				Type = Route.Type.POST,
				Fields = new Dictionary<string, string>(fields),
				Callback = callback
			});
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x0010AE70 File Offset: 0x00109070
		private static void SendRequest(ServerCommunicator.CommunicationData data)
		{
			ServerCommunicator.<SendRequest>d__3 <SendRequest>d__;
			<SendRequest>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendRequest>d__.data = data;
			<SendRequest>d__.<>1__state = -1;
			<SendRequest>d__.<>t__builder.Start<ServerCommunicator.<SendRequest>d__3>(ref <SendRequest>d__);
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x0010AEA8 File Offset: 0x001090A8
		private static void SendRequestToZone(ActiveZoneRecord activeZoneRecord, ServerCommunicator.CommunicationData data)
		{
			ServerCommunicator.<SendRequestToZone>d__4 <SendRequestToZone>d__;
			<SendRequestToZone>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendRequestToZone>d__.activeZoneRecord = activeZoneRecord;
			<SendRequestToZone>d__.data = data;
			<SendRequestToZone>d__.<>1__state = -1;
			<SendRequestToZone>d__.<>t__builder.Start<ServerCommunicator.<SendRequestToZone>d__4>(ref <SendRequestToZone>d__);
		}

		// Token: 0x020003FC RID: 1020
		private struct CommunicationData
		{
			// Token: 0x04002246 RID: 8774
			public string URL;

			// Token: 0x04002247 RID: 8775
			public ZoneId TargetZoneId;

			// Token: 0x04002248 RID: 8776
			public Route.Type Type;

			// Token: 0x04002249 RID: 8777
			public Dictionary<string, string> Fields;

			// Token: 0x0400224A RID: 8778
			public Action<UnityWebRequest> Callback;
		}
	}
}
