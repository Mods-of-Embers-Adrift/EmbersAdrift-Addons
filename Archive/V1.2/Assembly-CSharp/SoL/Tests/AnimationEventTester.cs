using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D8E RID: 3470
	public class AnimationEventTester : MonoBehaviour
	{
		// Token: 0x06006859 RID: 26713 RVA: 0x00086108 File Offset: 0x00084308
		private void PlayClip()
		{
			this.m_audioSource.PlayOneShot(this.m_clip);
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x0004475B File Offset: 0x0004295B
		private void Turn()
		{
		}

		// Token: 0x0600685B RID: 26715 RVA: 0x0008611B File Offset: 0x0008431B
		private void Walk(string n)
		{
			Debug.Log(n);
			this.PlayClip();
		}

		// Token: 0x0600685C RID: 26716 RVA: 0x0008611B File Offset: 0x0008431B
		private void Run(string n)
		{
			Debug.Log(n);
			this.PlayClip();
		}

		// Token: 0x04005A94 RID: 23188
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x04005A95 RID: 23189
		[SerializeField]
		private AudioClip m_clip;
	}
}
