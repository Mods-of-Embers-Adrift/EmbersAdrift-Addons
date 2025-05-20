using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D8C RID: 3468
	public class AnimancerSetIndex : MonoBehaviour
	{
		// Token: 0x06006856 RID: 26710 RVA: 0x002141A0 File Offset: 0x002123A0
		private void Awake()
		{
			AnimancerSetIndex.AnimancerIndexDict = new Dictionary<int, AnimancerAnimationSet>();
			for (int i = 0; i < this.m_animancerSetIndices.Length; i++)
			{
				AnimancerSetIndex.AnimancerIndexDict.Add(this.m_animancerSetIndices[i].Index, this.m_animancerSetIndices[i].Set);
			}
		}

		// Token: 0x04005A90 RID: 23184
		[SerializeField]
		private AnimancerSetIndex.AnimancerSetIndexData[] m_animancerSetIndices;

		// Token: 0x04005A91 RID: 23185
		public static Dictionary<int, AnimancerAnimationSet> AnimancerIndexDict;

		// Token: 0x02000D8D RID: 3469
		[Serializable]
		private class AnimancerSetIndexData
		{
			// Token: 0x04005A92 RID: 23186
			public int Index;

			// Token: 0x04005A93 RID: 23187
			public AnimancerAnimationSet Set;
		}
	}
}
