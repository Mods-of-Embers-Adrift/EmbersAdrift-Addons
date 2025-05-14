using System;

namespace SoL.Game.Randomization
{
	// Token: 0x02000772 RID: 1906
	public static class NetworkRandomizerExtensions
	{
		// Token: 0x06003857 RID: 14423 RVA: 0x0005D0CD File Offset: 0x0005B2CD
		public static bool TryGetAsType<T>(this NetworkedRandomizer inRandom, out T outRandom) where T : NetworkedRandomizer
		{
			outRandom = (inRandom as T);
			return outRandom != null;
		}
	}
}
