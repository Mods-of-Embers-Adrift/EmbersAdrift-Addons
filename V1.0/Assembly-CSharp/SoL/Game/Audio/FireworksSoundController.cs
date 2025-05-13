using System;
using SoL.Game.Objects;
using Unity.Collections;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D0B RID: 3339
	public class FireworksSoundController : MonoBehaviour
	{
		// Token: 0x060064D0 RID: 25808 RVA: 0x00083D5C File Offset: 0x00081F5C
		private void Start()
		{
			this.m_key = this.m_prefab.GetHashCode();
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x0020ACC8 File Offset: 0x00208EC8
		private void Update()
		{
			if (!this.m_particleSystem || !AudioSourcePool.Instance)
			{
				return;
			}
			int particleCount = this.m_particleSystem.particleCount;
			NativeArray<ParticleSystem.Particle> particles = new NativeArray<ParticleSystem.Particle>(particleCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
			this.m_particleSystem.GetParticles(particles, particleCount);
			for (int i = 0; i < particleCount; i++)
			{
				ParticleSystem.Particle particle = particles[i];
				if (this.m_explosion.HasData && particle.remainingLifetime < Time.deltaTime)
				{
					AudioSource audioSource = AudioSourcePool.Instance.RentSource(this.m_key, this.m_prefab, new float?(this.m_returnTime));
					this.m_explosion.ConfigureAudioSource(audioSource);
					audioSource.transform.position = particle.position;
					audioSource.enabled = true;
				}
				if (this.m_shoot.HasData && particle.remainingLifetime >= particle.startLifetime - Time.deltaTime)
				{
					AudioSource audioSource2 = AudioSourcePool.Instance.RentSource(this.m_key, this.m_prefab, new float?(this.m_returnTime));
					this.m_shoot.ConfigureAudioSource(audioSource2);
					audioSource2.transform.position = particle.position;
					audioSource2.enabled = true;
				}
			}
			particles.Dispose();
		}

		// Token: 0x04005783 RID: 22403
		[SerializeField]
		private GameObject m_prefab;

		// Token: 0x04005784 RID: 22404
		[SerializeField]
		private ParticleSystem m_particleSystem;

		// Token: 0x04005785 RID: 22405
		[SerializeField]
		private float m_returnTime = 1.5f;

		// Token: 0x04005786 RID: 22406
		[SerializeField]
		private FireworksSoundController.AudioSetting m_explosion;

		// Token: 0x04005787 RID: 22407
		[SerializeField]
		private FireworksSoundController.AudioSetting m_shoot;

		// Token: 0x04005788 RID: 22408
		[NonSerialized]
		private int m_key;

		// Token: 0x02000D0C RID: 3340
		[Serializable]
		private class AudioSetting
		{
			// Token: 0x060064D3 RID: 25811 RVA: 0x0020AE0C File Offset: 0x0020900C
			internal void ConfigureAudioSource(AudioSource source)
			{
				if (source)
				{
					source.priority = 138;
					source.clip = ((this.m_clips != null && this.m_clips.Length != 0) ? this.m_clips[UnityEngine.Random.Range(0, this.m_clips.Length)] : null);
					source.volume = this.m_volume.RandomWithinRange();
					source.pitch = this.m_pitch.RandomWithinRange();
				}
			}

			// Token: 0x17001833 RID: 6195
			// (get) Token: 0x060064D4 RID: 25812 RVA: 0x00083D82 File Offset: 0x00081F82
			internal bool HasData
			{
				get
				{
					return this.m_clips != null && this.m_clips.Length != 0;
				}
			}

			// Token: 0x04005789 RID: 22409
			private const int kPriority = 138;

			// Token: 0x0400578A RID: 22410
			[SerializeField]
			private AudioClip[] m_clips;

			// Token: 0x0400578B RID: 22411
			[SerializeField]
			private MinMaxFloatRange m_volume = new MinMaxFloatRange(0.3f, 0.7f);

			// Token: 0x0400578C RID: 22412
			[SerializeField]
			private MinMaxFloatRange m_pitch = new MinMaxFloatRange(0.75f, 1.25f);
		}
	}
}
