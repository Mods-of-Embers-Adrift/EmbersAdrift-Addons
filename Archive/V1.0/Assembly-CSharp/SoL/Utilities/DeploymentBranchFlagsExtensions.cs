using System;
using SoL.Game.Settings;

namespace SoL.Utilities
{
	// Token: 0x02000274 RID: 628
	public static class DeploymentBranchFlagsExtensions
	{
		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x0004FDE4 File Offset: 0x0004DFE4
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x0004FDEB File Offset: 0x0004DFEB
		public static bool BatchMode { get; set; }

		// Token: 0x060013BC RID: 5052 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this DeploymentBranchFlags a, DeploymentBranchFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x000F78E8 File Offset: 0x000F5AE8
		public static DeploymentBranchFlags GetBranchFlags()
		{
			DeploymentBranchFlags result = DeploymentBranchFlags.None;
			if (GlobalSettings.Values != null && GlobalSettings.Values.Configs != null && GlobalSettings.Values.Configs.Data != null)
			{
				result = DeploymentBranchFlagsExtensions.GetBranchFlags(GlobalSettings.Values.Configs.Data.DeploymentBranch);
			}
			return result;
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x000F7944 File Offset: 0x000F5B44
		public static DeploymentBranchFlags GetBranchFlags(string deploymentBranch)
		{
			DeploymentBranchFlags result = DeploymentBranchFlags.None;
			if (!(deploymentBranch == "dev"))
			{
				if (!(deploymentBranch == "qa"))
				{
					if (deploymentBranch == "live")
					{
						result = DeploymentBranchFlags.LIVE;
					}
				}
				else
				{
					result = DeploymentBranchFlags.QA;
				}
			}
			else
			{
				result = DeploymentBranchFlags.DEV;
			}
			return result;
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x0004FDF3 File Offset: 0x0004DFF3
		public static bool IsQA()
		{
			return !DeploymentBranchFlagsExtensions.GetBranchFlags().HasBitFlag(DeploymentBranchFlags.LIVE);
		}
	}
}
