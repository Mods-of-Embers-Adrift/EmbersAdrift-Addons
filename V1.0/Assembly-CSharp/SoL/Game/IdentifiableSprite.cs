using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200058A RID: 1418
	[Serializable]
	public class IdentifiableSprite : IdentifiableObject<Sprite>, IEquatable<IdentifiableSprite>
	{
		// Token: 0x06002C4F RID: 11343 RVA: 0x0005EC16 File Offset: 0x0005CE16
		public bool Equals(IdentifiableSprite other)
		{
			return other != null && other.Id == base.Id && other.Obj == base.Obj;
		}

		// Token: 0x06002C50 RID: 11344 RVA: 0x0005EC41 File Offset: 0x0005CE41
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((IdentifiableSprite)obj)));
		}

		// Token: 0x06002C51 RID: 11345 RVA: 0x0005EC6F File Offset: 0x0005CE6F
		public override int GetHashCode()
		{
			return HashCode.Combine<UniqueId, Sprite>(base.Id, base.Obj);
		}
	}
}
