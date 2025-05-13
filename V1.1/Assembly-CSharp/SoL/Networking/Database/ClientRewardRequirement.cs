using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x0200043F RID: 1087
	[Serializable]
	public class ClientRewardRequirement
	{
		// Token: 0x06001EEF RID: 7919 RVA: 0x00056E93 File Offset: 0x00055093
		public bool ClientMeetsRewardRequirement(GameEntity entity)
		{
			return !this.m_rewardIsRequired || this.ClientMeetsRewardRequirementInternal(entity);
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x0011D824 File Offset: 0x0011BA24
		private bool ClientMeetsRewardRequirementInternal(GameEntity entity)
		{
			if (entity != null)
			{
				UserRecord userRecord = GameManager.IsServer ? entity.User : SessionData.User;
				if (userRecord != null && userRecord.Rewards != null)
				{
					for (int i = 0; i < userRecord.Rewards.Length; i++)
					{
						if (userRecord.Rewards[i].rewardType.Equals(this.m_rewardType, StringComparison.InvariantCultureIgnoreCase) && userRecord.Rewards[i].code.Equals(this.m_rewardCode, StringComparison.InvariantCultureIgnoreCase))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04002454 RID: 9300
		[SerializeField]
		private bool m_rewardIsRequired;

		// Token: 0x04002455 RID: 9301
		[SerializeField]
		private string m_rewardType;

		// Token: 0x04002456 RID: 9302
		[SerializeField]
		private string m_rewardCode;
	}
}
