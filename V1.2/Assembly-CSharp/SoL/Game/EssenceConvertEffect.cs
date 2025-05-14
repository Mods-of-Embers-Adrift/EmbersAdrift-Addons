using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000575 RID: 1397
	public class EssenceConvertEffect : MonoBehaviour
	{
		// Token: 0x06002B2A RID: 11050 RVA: 0x0005DF59 File Offset: 0x0005C159
		private void Awake()
		{
			EssenceConvertEffect.Instance = this;
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x001463AC File Offset: 0x001445AC
		public void Trigger()
		{
			if (this.m_animator)
			{
				this.m_animator.SetTrigger(EssenceConvertEffect.kAnimatorTrigger);
			}
			if (this.m_audioSource)
			{
				this.m_audioSource.pitch = UnityEngine.Random.Range(this.m_pitch.x, this.m_pitch.y);
				this.m_audioSource.Play();
			}
			if (this.m_particleSystem)
			{
				this.m_particleSystem.Play(true);
			}
		}

		// Token: 0x04002B57 RID: 11095
		public static EssenceConvertEffect Instance = null;

		// Token: 0x04002B58 RID: 11096
		private static int kAnimatorTrigger = Animator.StringToHash("Activate");

		// Token: 0x04002B59 RID: 11097
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04002B5A RID: 11098
		[SerializeField]
		private Vector2 m_pitch = new Vector2(0.8f, 1.2f);

		// Token: 0x04002B5B RID: 11099
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x04002B5C RID: 11100
		[SerializeField]
		private ParticleSystem m_particleSystem;
	}
}
