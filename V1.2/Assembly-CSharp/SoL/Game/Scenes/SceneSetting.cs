using System;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x02000759 RID: 1881
	[Serializable]
	public class SceneSetting
	{
		// Token: 0x17000CC6 RID: 3270
		// (get) Token: 0x060037FB RID: 14331 RVA: 0x000662A6 File Offset: 0x000644A6
		public SceneReference SceneReference
		{
			get
			{
				return this.m_scene;
			}
		}

		// Token: 0x17000CC7 RID: 3271
		// (get) Token: 0x060037FC RID: 14332 RVA: 0x000662AE File Offset: 0x000644AE
		public SceneInclusionFlags InclusionFlags
		{
			get
			{
				return this.m_flags;
			}
		}

		// Token: 0x060037FD RID: 14333 RVA: 0x000662B6 File Offset: 0x000644B6
		public bool IsProperDeploymentBranch(DeploymentBranchFlags branchFlags)
		{
			return branchFlags == DeploymentBranchFlags.None || this.m_branchFlags == DeploymentBranchFlags.None || this.m_branchFlags.HasBitFlag(branchFlags);
		}

		// Token: 0x17000CC8 RID: 3272
		// (get) Token: 0x060037FE RID: 14334 RVA: 0x0016C6B4 File Offset: 0x0016A8B4
		public string IndexName
		{
			get
			{
				if (this.m_scene.IsValid())
				{
					string[] array = this.m_scene.ScenePath.Split('/', StringSplitOptions.None);
					string text = array[array.Length - 1].Replace(".unity", "");
					string text2 = this.m_scene.IsAddressable() ? "[A] " : "";
					string text3 = (this.m_branchFlags != DeploymentBranchFlags.None) ? (" (" + this.m_branchFlags.ToString() + ")") : "";
					return string.Concat(new string[]
					{
						text2,
						text,
						" (",
						this.m_flags.ToString(),
						")",
						text3
					});
				}
				return "None";
			}
		}

		// Token: 0x040036DF RID: 14047
		[SerializeField]
		private SceneReference m_scene;

		// Token: 0x040036E0 RID: 14048
		[SerializeField]
		private SceneInclusionFlags m_flags;

		// Token: 0x040036E1 RID: 14049
		[SerializeField]
		private DeploymentBranchFlags m_branchFlags;
	}
}
