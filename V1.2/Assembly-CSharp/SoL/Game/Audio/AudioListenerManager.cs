using System;
using System.Collections.Generic;

namespace SoL.Game.Audio
{
	// Token: 0x02000D06 RID: 3334
	public static class AudioListenerManager
	{
		// Token: 0x060064B4 RID: 25780 RVA: 0x00083BC6 File Offset: 0x00081DC6
		public static void RegisterAudioListener(AudioListenerSource source)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			if (source.IsFallback)
			{
				AudioListenerManager.m_fallbackSource = source;
			}
			else
			{
				AudioListenerManager.m_nonDefaultSources.Add(source);
			}
			AudioListenerManager.RefreshFallbackAudioListener();
		}

		// Token: 0x060064B5 RID: 25781 RVA: 0x00083BFC File Offset: 0x00081DFC
		public static void UnregisterAudioListener(AudioListenerSource source)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			if (source.IsFallback)
			{
				AudioListenerManager.m_fallbackSource = null;
			}
			else
			{
				AudioListenerManager.m_nonDefaultSources.Remove(source);
			}
			AudioListenerManager.RefreshFallbackAudioListener();
		}

		// Token: 0x060064B6 RID: 25782 RVA: 0x0020A73C File Offset: 0x0020893C
		private static void RefreshFallbackAudioListener()
		{
			if (!AudioListenerManager.m_fallbackSource)
			{
				return;
			}
			bool flag = false;
			foreach (AudioListenerSource audioListenerSource in AudioListenerManager.m_nonDefaultSources)
			{
				flag = (flag || audioListenerSource.ListenerEnabled);
			}
			AudioListenerManager.m_fallbackSource.ListenerEnabled = !flag;
		}

		// Token: 0x0400576D RID: 22381
		private static AudioListenerSource m_fallbackSource = null;

		// Token: 0x0400576E RID: 22382
		private static readonly HashSet<AudioListenerSource> m_nonDefaultSources = new HashSet<AudioListenerSource>(2);
	}
}
