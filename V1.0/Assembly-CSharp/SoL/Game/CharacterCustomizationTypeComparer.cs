using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x0200055E RID: 1374
	public struct CharacterCustomizationTypeComparer : IEqualityComparer<CharacterCustomizationType>
	{
		// Token: 0x060029A8 RID: 10664 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CharacterCustomizationType x, CharacterCustomizationType y)
		{
			return x == y;
		}

		// Token: 0x060029A9 RID: 10665 RVA: 0x0005CCD0 File Offset: 0x0005AED0
		public int GetHashCode(CharacterCustomizationType obj)
		{
			return obj.GetHashCode();
		}
	}
}
