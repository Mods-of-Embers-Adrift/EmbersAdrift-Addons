using System;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007FB RID: 2043
	[CreateAssetMenu(menuName = "SoL/Profiles/Npc Health Threshold Actions")]
	public class HealthThresholdActions : ScriptableObject
	{
		// Token: 0x06003B4C RID: 15180 RVA: 0x0017B010 File Offset: 0x00179210
		public void ExecuteActions(GameEntity entity, float prevHealthFraction, float currHealthFraction)
		{
			if (!entity)
			{
				return;
			}
			for (int i = 0; i < this.m_actions.Length; i++)
			{
				HealthThresholdActions.ThresholdAction thresholdAction = this.m_actions[i];
				if (thresholdAction != null)
				{
					thresholdAction.Execute(entity, prevHealthFraction, currHealthFraction);
				}
			}
		}

		// Token: 0x040039BC RID: 14780
		[SerializeField]
		private HealthThresholdActions.ThresholdAction[] m_actions;

		// Token: 0x020007FC RID: 2044
		[Serializable]
		private class ThresholdAction
		{
			// Token: 0x06003B4E RID: 15182 RVA: 0x0017B050 File Offset: 0x00179250
			internal void Execute(GameEntity entity, float prevHealthFraction, float currHealthFraction)
			{
				if (prevHealthFraction >= this.m_threshold && currHealthFraction < this.m_threshold)
				{
					this.m_emote.EmoteToNearbyPlayers(entity, null);
				}
			}

			// Token: 0x040039BD RID: 14781
			[Range(0.01f, 1f)]
			[SerializeField]
			private float m_threshold = 1f;

			// Token: 0x040039BE RID: 14782
			[SerializeField]
			private EmotiveCalls m_emote;
		}
	}
}
