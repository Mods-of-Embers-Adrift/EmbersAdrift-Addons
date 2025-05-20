using System;
using System.Collections.Generic;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B6D RID: 2925
	internal struct NewCharacterKeyComparer : IEqualityComparer<NewCharacterKey>
	{
		// Token: 0x060059E0 RID: 23008 RVA: 0x0007C455 File Offset: 0x0007A655
		public bool Equals(NewCharacterKey x, NewCharacterKey y)
		{
			return x.Sex == y.Sex && x.BuildType == y.BuildType;
		}

		// Token: 0x060059E1 RID: 23009 RVA: 0x0007C475 File Offset: 0x0007A675
		public int GetHashCode(NewCharacterKey obj)
		{
			return (int)obj.Sex * 397 ^ (int)obj.BuildType;
		}
	}
}
