using System;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D00 RID: 3328
	public class AudioEventsProfile : ScriptableObject
	{
		// Token: 0x1700182B RID: 6187
		// (get) Token: 0x060064A6 RID: 25766 RVA: 0x00083B5C File Offset: 0x00081D5C
		public AudioEvent[] AudioEvents
		{
			get
			{
				return this.m_audioEvents;
			}
		}

		// Token: 0x04005762 RID: 22370
		[SerializeField]
		private AudioEvent[] m_audioEvents;
	}
}
