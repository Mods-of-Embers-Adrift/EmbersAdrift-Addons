using System;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D2E RID: 3374
	public interface IAmbientAudioZone
	{
		// Token: 0x1700185C RID: 6236
		// (get) Token: 0x0600657E RID: 25982
		int Key { get; }

		// Token: 0x1700185D RID: 6237
		// (get) Token: 0x0600657F RID: 25983
		AmbientAudioZoneProfile Profile { get; }
	}
}
