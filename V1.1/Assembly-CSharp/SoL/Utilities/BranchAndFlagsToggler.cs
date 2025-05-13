using System;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200024D RID: 589
	public class BranchAndFlagsToggler : MonoBehaviour
	{
		// Token: 0x0600133C RID: 4924 RVA: 0x000E8FA8 File Offset: 0x000E71A8
		private void Awake()
		{
			if (this.m_objects == null || this.m_objects.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_objects.Length; i++)
			{
				BranchAndFlagsToggler.ToggleObject toggleObject = this.m_objects[i];
				if (toggleObject != null)
				{
					toggleObject.RefreshToggle();
				}
			}
		}

		// Token: 0x040010F6 RID: 4342
		[SerializeField]
		private BranchAndFlagsToggler.ToggleObject[] m_objects;

		// Token: 0x0200024E RID: 590
		[Serializable]
		private class ToggleObject
		{
			// Token: 0x0600133E RID: 4926 RVA: 0x000E8FF0 File Offset: 0x000E71F0
			private bool EnabledForBranch()
			{
				if (this.m_branchFlags == DeploymentBranchFlags.None)
				{
					return true;
				}
				DeploymentBranchFlags branchFlags = DeploymentBranchFlagsExtensions.GetBranchFlags();
				return branchFlags == DeploymentBranchFlags.None || this.m_branchFlags.HasBitFlag(branchFlags);
			}

			// Token: 0x0600133F RID: 4927 RVA: 0x0004F969 File Offset: 0x0004DB69
			private bool EnabledForFlags()
			{
				return GameManager.IsServer || (SessionData.User != null && AccessFlagsExtensions.HasAccessForFlags(SessionData.User.Flags, this.m_accessFlagRequirement));
			}

			// Token: 0x06001340 RID: 4928 RVA: 0x000E9020 File Offset: 0x000E7220
			public void RefreshToggle()
			{
				if (this.m_object)
				{
					bool flag = this.EnabledForBranch() && this.EnabledForFlags();
					if (this.m_invert)
					{
						flag = !flag;
					}
					this.m_object.SetActive(flag);
				}
			}

			// Token: 0x040010F7 RID: 4343
			[SerializeField]
			private bool m_invert;

			// Token: 0x040010F8 RID: 4344
			[SerializeField]
			private GameObject m_object;

			// Token: 0x040010F9 RID: 4345
			[SerializeField]
			private DeploymentBranchFlags m_branchFlags;

			// Token: 0x040010FA RID: 4346
			[SerializeField]
			private AccessFlags m_accessFlagRequirement;
		}
	}
}
