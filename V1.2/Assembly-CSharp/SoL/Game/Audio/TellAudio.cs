using System;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D19 RID: 3353
	public static class TellAudio
	{
		// Token: 0x1700183B RID: 6203
		// (get) Token: 0x060064FF RID: 25855 RVA: 0x00083FC5 File Offset: 0x000821C5
		public static TellAudioType[] TellAudioTypes
		{
			get
			{
				if (TellAudio.m_tellAudioTypes == null)
				{
					TellAudio.m_tellAudioTypes = (TellAudioType[])Enum.GetValues(typeof(TellAudioType));
				}
				return TellAudio.m_tellAudioTypes;
			}
		}

		// Token: 0x06006500 RID: 25856 RVA: 0x0020B558 File Offset: 0x00209758
		public static void PlayTellAudioClip()
		{
			AudioClip clip;
			if (GlobalSettings.Values && GlobalSettings.Values.Audio != null && GlobalSettings.Values.Audio.TryGetTellAudioClip(out clip) && ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.PlayClip(clip, new float?(1f), new float?(GlobalSettings.Values.Audio.TellVolume));
			}
		}

		// Token: 0x040057C7 RID: 22471
		private static TellAudioType[] m_tellAudioTypes;
	}
}
