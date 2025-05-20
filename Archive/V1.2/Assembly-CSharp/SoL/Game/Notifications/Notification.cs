using System;
using UnityEngine;

namespace SoL.Game.Notifications
{
	// Token: 0x02000843 RID: 2115
	public struct Notification : IEquatable<Notification>
	{
		// Token: 0x17000E1C RID: 3612
		// (get) Token: 0x06003D16 RID: 15638 RVA: 0x001819E0 File Offset: 0x0017FBE0
		public string TitleWithData
		{
			get
			{
				if (this.TextData != null)
				{
					return this.Type.Title.Replace("{data}", this.TextData);
				}
				if (this.Type.Title.Contains("{data}"))
				{
					Debug.LogWarning("The title for " + this.Type.name + " contains a {data} token, but no data was provided for this notification!");
				}
				return this.Type.Title;
			}
		}

		// Token: 0x17000E1D RID: 3613
		// (get) Token: 0x06003D17 RID: 15639 RVA: 0x00181A54 File Offset: 0x0017FC54
		public string DescriptionWithData
		{
			get
			{
				if (this.TextData != null)
				{
					return this.Type.Description.Replace("{data}", this.TextData);
				}
				if (this.Type.Description.Contains("{data}"))
				{
					Debug.LogWarning("The description for " + this.Type.name + " contains a {data} token, but no data was provided for this notification!");
				}
				return this.Type.Description;
			}
		}

		// Token: 0x06003D18 RID: 15640 RVA: 0x00181AC8 File Offset: 0x0017FCC8
		public bool Equals(Notification other)
		{
			return other != null && (this == other || (this.Type == other.Type && this.TextData == other.TextData && this.OpenData == other.OpenData && this.Created == other.Created && this.Shown == other.Shown));
		}

		// Token: 0x06003D19 RID: 15641 RVA: 0x00181B50 File Offset: 0x0017FD50
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((Notification)obj)));
		}

		// Token: 0x06003D1A RID: 15642 RVA: 0x00181BA0 File Offset: 0x0017FDA0
		public override int GetHashCode()
		{
			return (((this.Type.GetHashCode() * 397 ^ ((this.TextData != null) ? this.TextData.GetHashCode() : 0)) * 397 ^ ((this.OpenData != null) ? this.OpenData.GetHashCode() : 0)) * 397 ^ this.Created.GetHashCode()) * 397 ^ this.Shown.GetHashCode();
		}

		// Token: 0x06003D1B RID: 15643 RVA: 0x00069656 File Offset: 0x00067856
		public static bool operator ==(Notification a, Notification b)
		{
			return a.Equals(b);
		}

		// Token: 0x06003D1C RID: 15644 RVA: 0x00069660 File Offset: 0x00067860
		public static bool operator !=(Notification a, Notification b)
		{
			return !a.Equals(b);
		}

		// Token: 0x04003BE2 RID: 15330
		public BaseNotification Type;

		// Token: 0x04003BE3 RID: 15331
		public string TextData;

		// Token: 0x04003BE4 RID: 15332
		public object OpenData;

		// Token: 0x04003BE5 RID: 15333
		public DateTime Created;

		// Token: 0x04003BE6 RID: 15334
		public DateTime Shown;
	}
}
