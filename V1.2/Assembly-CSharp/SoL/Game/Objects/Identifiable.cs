using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F3 RID: 2547
	public class Identifiable : ScriptableObject, IEquatable<Identifiable>
	{
		// Token: 0x17001117 RID: 4375
		// (get) Token: 0x06004D76 RID: 19830 RVA: 0x00074515 File Offset: 0x00072715
		public UniqueId Id
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x06004D77 RID: 19831 RVA: 0x0007451D File Offset: 0x0007271D
		public bool Equals(Identifiable other)
		{
			return other != null && (this == other || (base.Equals(other) && this.m_id.Equals(other.m_id)));
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x00074546 File Offset: 0x00072746
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((Identifiable)obj)));
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x00074574 File Offset: 0x00072774
		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ this.m_id.GetHashCode();
		}

		// Token: 0x04004720 RID: 18208
		[SerializeField]
		protected UniqueId m_id;
	}
}
