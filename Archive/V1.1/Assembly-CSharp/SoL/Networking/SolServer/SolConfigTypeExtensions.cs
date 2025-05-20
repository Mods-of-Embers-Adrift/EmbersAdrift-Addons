using System;
using SoL.Game.Settings;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003DA RID: 986
	public static class SolConfigTypeExtensions
	{
		// Token: 0x06001A79 RID: 6777 RVA: 0x00108848 File Offset: 0x00106A48
		public static string GetBranchName(this SolConfigType configType)
		{
			switch (configType)
			{
			case SolConfigType.Local:
				return "local";
			case SolConfigType.Dev:
				return "dev";
			case SolConfigType.QA:
				return "qa";
			case SolConfigType.Live:
				return "live";
			}
			return GlobalSettings.Values.Configs.Data.DeploymentBranch.ToLowerInvariant();
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0005493E File Offset: 0x00052B3E
		public static string GetApiBranch()
		{
			return GlobalSettings.Values.Configs.Data.DeploymentBranch.ToLowerInvariant();
		}
	}
}
