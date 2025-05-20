using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CFB RID: 3323
	public class AudioController : MonoBehaviour, IAudioController
	{
		// Token: 0x17001821 RID: 6177
		// (get) Token: 0x06006481 RID: 25729 RVA: 0x0008397F File Offset: 0x00081B7F
		public AudioEvent[] AudioEvents
		{
			get
			{
				return this.m_audioEvents;
			}
		}

		// Token: 0x04005743 RID: 22339
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x04005744 RID: 22340
		[SerializeField]
		private MinMaxFloatRange m_pitchShift = new MinMaxFloatRange(1f, 1f);

		// Token: 0x04005745 RID: 22341
		[SerializeField]
		private AudioEvent[] m_audioEvents;
	}
}
