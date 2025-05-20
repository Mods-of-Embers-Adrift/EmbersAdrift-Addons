using System;
using System.Collections;
using SoL.Game.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D14 RID: 3348
	public class RandomAudioClipPlayer : MonoBehaviour
	{
		// Token: 0x060064F2 RID: 25842 RVA: 0x00083F00 File Offset: 0x00082100
		private void Awake()
		{
			if (this.m_source == null || this.m_clips == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060064F3 RID: 25843 RVA: 0x00083F2A File Offset: 0x0008212A
		private void OnEnable()
		{
			this.SetNextPlayTime();
		}

		// Token: 0x060064F4 RID: 25844 RVA: 0x00083F32 File Offset: 0x00082132
		private void Update()
		{
			if (Time.time < this.m_nextPlayTime)
			{
				return;
			}
			this.m_clips.PlayRandomClip(this.m_source);
			this.SetNextPlayTime();
		}

		// Token: 0x060064F5 RID: 25845 RVA: 0x00083F59 File Offset: 0x00082159
		private void SetNextPlayTime()
		{
			if (RandomAudioClipPlayer.m_random == null)
			{
				RandomAudioClipPlayer.m_random = new System.Random();
			}
			this.m_nextPlayTime = Time.time + this.m_delayTime.RandomWithinRange(RandomAudioClipPlayer.m_random);
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioClipCollections()
		{
			return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
		}

		// Token: 0x040057B6 RID: 22454
		private static System.Random m_random;

		// Token: 0x040057B7 RID: 22455
		[SerializeField]
		private AudioSource m_source;

		// Token: 0x040057B8 RID: 22456
		[SerializeField]
		private AudioClipCollection m_clips;

		// Token: 0x040057B9 RID: 22457
		[SerializeField]
		private MinMaxIntRange m_delayTime;

		// Token: 0x040057BA RID: 22458
		private float m_nextPlayTime;
	}
}
