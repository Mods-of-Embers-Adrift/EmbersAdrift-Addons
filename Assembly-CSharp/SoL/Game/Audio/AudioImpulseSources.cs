using System;
using Cinemachine;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D02 RID: 3330
	public class AudioImpulseSources : MonoBehaviour
	{
		// Token: 0x060064A8 RID: 25768 RVA: 0x00083B64 File Offset: 0x00081D64
		private void Awake()
		{
			if (AudioImpulseSources.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			AudioImpulseSources.Instance = this;
		}

		// Token: 0x060064A9 RID: 25769 RVA: 0x0020A60C File Offset: 0x0020880C
		public void TriggerImpulseAtPosition(AudioImpulseFlags impulseFlags, float force, Vector3 pos)
		{
			if (impulseFlags == AudioImpulseFlags.None || force <= 0f || !Options.VideoOptions.CameraShake.Value)
			{
				return;
			}
			for (int i = 0; i < this.m_impulseSources.Length; i++)
			{
				if (impulseFlags.HasAnyFlags(this.m_impulseSources[i].Type))
				{
					Vector3 velocity = this.m_impulseSources[i].Source.m_DefaultVelocity * force;
					this.m_impulseSources[i].Source.GenerateImpulseAtPositionWithVelocity(pos, velocity);
				}
			}
		}

		// Token: 0x04005768 RID: 22376
		public static AudioImpulseSources Instance;

		// Token: 0x04005769 RID: 22377
		[SerializeField]
		private AudioImpulseSources.AudioImpulseData[] m_impulseSources;

		// Token: 0x02000D03 RID: 3331
		[Serializable]
		private class AudioImpulseData
		{
			// Token: 0x1700182C RID: 6188
			// (get) Token: 0x060064AB RID: 25771 RVA: 0x00083B80 File Offset: 0x00081D80
			public AudioImpulseFlags Type
			{
				get
				{
					return this.m_type;
				}
			}

			// Token: 0x1700182D RID: 6189
			// (get) Token: 0x060064AC RID: 25772 RVA: 0x00083B88 File Offset: 0x00081D88
			public CinemachineImpulseSource Source
			{
				get
				{
					return this.m_source;
				}
			}

			// Token: 0x0400576A RID: 22378
			[SerializeField]
			private AudioImpulseFlags m_type;

			// Token: 0x0400576B RID: 22379
			[SerializeField]
			private CinemachineImpulseSource m_source;
		}
	}
}
