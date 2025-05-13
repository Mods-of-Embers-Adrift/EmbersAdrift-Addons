using System;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CF9 RID: 3321
	public class AudioClipOptionCollection : AudioClipCollection
	{
		// Token: 0x17001820 RID: 6176
		// (get) Token: 0x0600647F RID: 25727 RVA: 0x0008394C File Offset: 0x00081B4C
		protected override AudioClip[] Clips
		{
			get
			{
				if (this.m_type != AudioClipOptionCollection.OptionType.Jump)
				{
					return this.m_clips;
				}
				if (!Options.AudioOptions.AlternateJumpAudio.Value)
				{
					return this.m_clips;
				}
				return this.m_alternateClips;
			}
		}

		// Token: 0x0400573D RID: 22333
		private const string kAlternate = "Alternate";

		// Token: 0x0400573E RID: 22334
		[SerializeField]
		private AudioClipOptionCollection.OptionType m_type;

		// Token: 0x0400573F RID: 22335
		[SerializeField]
		private AudioClip[] m_alternateClips;

		// Token: 0x02000CFA RID: 3322
		private enum OptionType
		{
			// Token: 0x04005741 RID: 22337
			None,
			// Token: 0x04005742 RID: 22338
			Jump
		}
	}
}
