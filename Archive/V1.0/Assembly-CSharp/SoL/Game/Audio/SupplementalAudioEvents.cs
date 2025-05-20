using System;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D17 RID: 3351
	public class SupplementalAudioEvents : GameEntityComponent
	{
		// Token: 0x060064FD RID: 25853 RVA: 0x0020B4A0 File Offset: 0x002096A0
		private void Start()
		{
			if (base.GameEntity && base.GameEntity.AudioEventController)
			{
				for (int i = 0; i < this.m_audioEvents.Length; i++)
				{
					base.GameEntity.AudioEventController.RegisterEvent(this.m_audioEvents[i]);
				}
			}
		}

		// Token: 0x040057C0 RID: 22464
		[SerializeField]
		private AudioEvent[] m_audioEvents;
	}
}
