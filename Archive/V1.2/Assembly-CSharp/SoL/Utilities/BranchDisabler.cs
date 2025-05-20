using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200024F RID: 591
	public class BranchDisabler : MonoBehaviour
	{
		// Token: 0x06001342 RID: 4930 RVA: 0x000E9088 File Offset: 0x000E7288
		private void Awake()
		{
			if (this.m_branchFlags == DeploymentBranchFlags.None)
			{
				return;
			}
			DeploymentBranchFlags branchFlags = DeploymentBranchFlagsExtensions.GetBranchFlags();
			if (branchFlags == DeploymentBranchFlags.None)
			{
				return;
			}
			if (!this.m_branchFlags.HasBitFlag(branchFlags))
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x040010FB RID: 4347
		[SerializeField]
		private DeploymentBranchFlags m_branchFlags;
	}
}
