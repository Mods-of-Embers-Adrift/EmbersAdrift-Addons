using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D15 RID: 3349
	public class RefreshMixerGroupOnAwake : MonoBehaviour
	{
		// Token: 0x060064F8 RID: 25848 RVA: 0x00083F88 File Offset: 0x00082188
		private void Awake()
		{
			if (this.m_source)
			{
				this.m_source.RefreshMixerGroup();
			}
		}

		// Token: 0x040057BB RID: 22459
		[SerializeField]
		private AudioSource m_source;
	}
}
