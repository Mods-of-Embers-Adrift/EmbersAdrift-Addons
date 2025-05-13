using System;
using System.Collections;
using SoL.Game.EffectSystem;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000748 RID: 1864
	[Serializable]
	public class TaskSettings
	{
		// Token: 0x0600379A RID: 14234 RVA: 0x0016BF20 File Offset: 0x0016A120
		public bool TryGetTaskEffect(int taskLevel, out ScriptableEffectData effect)
		{
			effect = this.m_baseEffect;
			for (int i = this.m_overrides.Length - 1; i >= 0; i--)
			{
				if (taskLevel >= this.m_overrides[i].Threshold)
				{
					effect = this.m_overrides[i].Effect;
					break;
				}
			}
			return effect != null;
		}

		// Token: 0x0600379B RID: 14235 RVA: 0x0005CB3A File Offset: 0x0005AD3A
		private IEnumerable GetEffects()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableEffectData>();
		}

		// Token: 0x04003682 RID: 13954
		[SerializeField]
		private ScriptableEffectData m_baseEffect;

		// Token: 0x04003683 RID: 13955
		[SerializeField]
		private TaskSettings.EffectLevelOverride[] m_overrides;

		// Token: 0x02000749 RID: 1865
		[Serializable]
		private class EffectLevelOverride
		{
			// Token: 0x17000C92 RID: 3218
			// (get) Token: 0x0600379D RID: 14237 RVA: 0x000660C8 File Offset: 0x000642C8
			public int Threshold
			{
				get
				{
					return this.m_levelThreshold;
				}
			}

			// Token: 0x17000C93 RID: 3219
			// (get) Token: 0x0600379E RID: 14238 RVA: 0x000660D0 File Offset: 0x000642D0
			public ScriptableEffectData Effect
			{
				get
				{
					return this.m_effect;
				}
			}

			// Token: 0x0600379F RID: 14239 RVA: 0x0005CB3A File Offset: 0x0005AD3A
			private IEnumerable GetEffects()
			{
				return SolOdinUtilities.GetDropdownItems<ScriptableEffectData>();
			}

			// Token: 0x04003684 RID: 13956
			[Range(1f, 50f)]
			[SerializeField]
			private int m_levelThreshold = 1;

			// Token: 0x04003685 RID: 13957
			[SerializeField]
			private ScriptableEffectData m_effect;
		}
	}
}
