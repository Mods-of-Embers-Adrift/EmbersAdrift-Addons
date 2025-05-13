using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CF7 RID: 3319
	public class AnimationAudioEventHandler : MonoBehaviour
	{
		// Token: 0x06006476 RID: 25718 RVA: 0x00209E2C File Offset: 0x0020802C
		private void AudioEvent(string eventName)
		{
			if (this.m_events == null && this.m_audioEvents != null && this.m_audioEvents.Length != 0)
			{
				this.m_events = new Dictionary<string, IAudioEvent>(32, StringComparer.InvariantCultureIgnoreCase);
				for (int i = 0; i < this.m_audioEvents.Length; i++)
				{
					if (!string.IsNullOrEmpty(this.m_audioEvents[i].EventName))
					{
						this.m_events.AddOrReplace(this.m_audioEvents[i].EventName, this.m_audioEvents[i]);
					}
				}
			}
			IAudioEvent audioEvent;
			if (this.m_events != null && this.m_events.TryGetValue(eventName, out audioEvent) && audioEvent != null)
			{
				audioEvent.Play(1f);
				if (audioEvent.ImpulseFlags != AudioImpulseFlags.None && this.m_impulseFlags.HasBitFlag(audioEvent.ImpulseFlags) && AudioImpulseSources.Instance != null)
				{
					AudioImpulseSources.Instance.TriggerImpulseAtPosition(audioEvent.ImpulseFlags, audioEvent.ImpulseForce, base.gameObject.transform.position);
				}
			}
		}

		// Token: 0x04005738 RID: 22328
		[SerializeField]
		private AudioImpulseFlags m_impulseFlags;

		// Token: 0x04005739 RID: 22329
		[SerializeField]
		private AudioEvent[] m_audioEvents;

		// Token: 0x0400573A RID: 22330
		private Dictionary<string, IAudioEvent> m_events;
	}
}
