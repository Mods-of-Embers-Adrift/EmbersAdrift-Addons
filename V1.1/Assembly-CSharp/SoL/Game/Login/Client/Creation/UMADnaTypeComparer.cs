using System;
using System.Collections.Generic;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B53 RID: 2899
	public struct UMADnaTypeComparer : IEqualityComparer<UMADnaType>
	{
		// Token: 0x06005944 RID: 22852 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(UMADnaType x, UMADnaType y)
		{
			return x == y;
		}

		// Token: 0x06005945 RID: 22853 RVA: 0x0007BC2E File Offset: 0x00079E2E
		public int GetHashCode(UMADnaType obj)
		{
			return obj.GetHashCode();
		}
	}
}
