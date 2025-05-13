using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CF RID: 1743
	public class SpawnControllerOverrides : MonoBehaviour
	{
		// Token: 0x060034FC RID: 13564 RVA: 0x00166854 File Offset: 0x00164A54
		public bool TryGetOverrideData(SpawnProfile spawnProfile, out SpawnControllerOverrideData data)
		{
			data = null;
			if (spawnProfile && this.m_overrides != null)
			{
				for (int i = 0; i < this.m_overrides.Length; i++)
				{
					if (this.m_overrides[i] != null && this.m_overrides[i].SpawnProfile == spawnProfile)
					{
						data = this.m_overrides[i];
						break;
					}
				}
			}
			return data != null;
		}

		// Token: 0x0400332B RID: 13099
		[SerializeField]
		private SpawnControllerOverrideData[] m_overrides;
	}
}
