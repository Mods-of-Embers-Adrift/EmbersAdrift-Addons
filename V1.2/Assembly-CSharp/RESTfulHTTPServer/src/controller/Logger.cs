using System;
using UnityEngine;

namespace RESTfulHTTPServer.src.controller
{
	// Token: 0x0200007F RID: 127
	public static class Logger
	{
		// Token: 0x06000530 RID: 1328 RVA: 0x0009EA54 File Offset: 0x0009CC54
		public static void Log(string tag, string msg)
		{
			string message = "[REST] " + tag + " - " + msg;
			Debug.Log(message);
		}
	}
}
