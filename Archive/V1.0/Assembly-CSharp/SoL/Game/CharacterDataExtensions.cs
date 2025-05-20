using System;

namespace SoL.Game
{
	// Token: 0x02000564 RID: 1380
	public static class CharacterDataExtensions
	{
		// Token: 0x06002A31 RID: 10801 RVA: 0x0005D0CD File Offset: 0x0005B2CD
		public static bool TryGetAsType<T>(this CharacterData data, out T outData) where T : CharacterData
		{
			outData = (data as T);
			return outData != null;
		}
	}
}
