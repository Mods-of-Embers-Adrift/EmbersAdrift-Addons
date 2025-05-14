using System;
using SoL.Game.Login.Client;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E1 RID: 993
	public static class LoginHandler
	{
		// Token: 0x06001A85 RID: 6789 RVA: 0x001096D8 File Offset: 0x001078D8
		public static void Handle(SolServerCommand cmd)
		{
			if (!cmd.State)
			{
				LoginHandler.RaiseError(cmd.Args["err"].ToString());
				Debug.LogWarning(string.Format("State=false, {0} {1}", cmd.Args["err"], cmd.Command));
				return;
			}
			if (cmd.Command == CommandType.init_login)
			{
				if (!cmd.Args.ContainsKey("version"))
				{
					LoginHandler.RaiseError("No server API version key sent back!\nPlease restart your client.");
					return;
				}
				if ((string)cmd.Args["version"] != GlobalSettings.Values.Configs.Data.ServerApiVersion)
				{
					LoginHandler.RaiseError("Mismatched server API version!\nPlease update your client.");
					Debug.Log("Current server API version: " + GlobalSettings.Values.Configs.Data.ServerApiVersion);
				}
			}
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x0005499E File Offset: 0x00052B9E
		private static void RaiseError(string err)
		{
			if (LoginController.Instance != null)
			{
				Debug.LogWarning(err);
				LoginController.Instance.RaiseErrorCritical(err.Color(Color.red));
			}
		}

		// Token: 0x04002173 RID: 8563
		private const string kVersionKey = "version";
	}
}
