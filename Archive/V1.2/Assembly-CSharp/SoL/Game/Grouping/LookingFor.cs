using System;
using System.Collections.Generic;

namespace SoL.Game.Grouping
{
	// Token: 0x02000BE0 RID: 3040
	public class LookingFor : IEquatable<LookingFor>, IEqualityComparer<LookingFor>
	{
		// Token: 0x17001638 RID: 5688
		// (get) Token: 0x06005E0D RID: 24077 RVA: 0x0007F45A File Offset: 0x0007D65A
		// (set) Token: 0x06005E0E RID: 24078 RVA: 0x0007F462 File Offset: 0x0007D662
		public string Key { get; set; }

		// Token: 0x17001639 RID: 5689
		// (get) Token: 0x06005E0F RID: 24079 RVA: 0x0007F46B File Offset: 0x0007D66B
		// (set) Token: 0x06005E10 RID: 24080 RVA: 0x0007F473 File Offset: 0x0007D673
		public string ContactName { get; set; }

		// Token: 0x1700163A RID: 5690
		// (get) Token: 0x06005E11 RID: 24081 RVA: 0x0007F47C File Offset: 0x0007D67C
		// (set) Token: 0x06005E12 RID: 24082 RVA: 0x0007F484 File Offset: 0x0007D684
		public ZoneId ZoneId { get; set; }

		// Token: 0x1700163B RID: 5691
		// (get) Token: 0x06005E13 RID: 24083 RVA: 0x0007F48D File Offset: 0x0007D68D
		// (set) Token: 0x06005E14 RID: 24084 RVA: 0x0007F495 File Offset: 0x0007D695
		public int MinLevel { get; set; }

		// Token: 0x1700163C RID: 5692
		// (get) Token: 0x06005E15 RID: 24085 RVA: 0x0007F49E File Offset: 0x0007D69E
		// (set) Token: 0x06005E16 RID: 24086 RVA: 0x0007F4A6 File Offset: 0x0007D6A6
		public int MaxLevel { get; set; }

		// Token: 0x1700163D RID: 5693
		// (get) Token: 0x06005E17 RID: 24087 RVA: 0x0007F4AF File Offset: 0x0007D6AF
		// (set) Token: 0x06005E18 RID: 24088 RVA: 0x0007F4B7 File Offset: 0x0007D6B7
		public LookingTags Tags { get; set; }

		// Token: 0x1700163E RID: 5694
		// (get) Token: 0x06005E19 RID: 24089 RVA: 0x0007F4C0 File Offset: 0x0007D6C0
		// (set) Token: 0x06005E1A RID: 24090 RVA: 0x0007F4C8 File Offset: 0x0007D6C8
		public DateTime Created { get; set; }

		// Token: 0x1700163F RID: 5695
		// (get) Token: 0x06005E1B RID: 24091 RVA: 0x0007F4D1 File Offset: 0x0007D6D1
		// (set) Token: 0x06005E1C RID: 24092 RVA: 0x0007F4D9 File Offset: 0x0007D6D9
		public LookingType Type { get; set; }

		// Token: 0x17001640 RID: 5696
		// (get) Token: 0x06005E1D RID: 24093 RVA: 0x0007F4E2 File Offset: 0x0007D6E2
		// (set) Token: 0x06005E1E RID: 24094 RVA: 0x0007F4EA File Offset: 0x0007D6EA
		public string Note { get; set; }

		// Token: 0x06005E1F RID: 24095 RVA: 0x001F5D74 File Offset: 0x001F3F74
		public bool Equals(LookingFor other)
		{
			return other != null && this.Key == other.Key && this.ContactName == other.ContactName && this.MinLevel == other.MinLevel && this.MaxLevel == other.MaxLevel && this.Tags == other.Tags && this.Type == other.Type && this.ZoneId == other.ZoneId && this.Note == other.Note;
		}

		// Token: 0x06005E20 RID: 24096 RVA: 0x001F5E04 File Offset: 0x001F4004
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			LookingFor lookingFor = obj as LookingFor;
			return lookingFor != null && this.Equals(lookingFor);
		}

		// Token: 0x06005E21 RID: 24097 RVA: 0x001F5E2C File Offset: 0x001F402C
		public override int GetHashCode()
		{
			return ((((((((this.Key != null) ? this.Key.GetHashCode() : 0) * 397 ^ ((this.ContactName != null) ? this.ContactName.GetHashCode() : 0)) * 397 ^ (int)this.ZoneId) * 397 ^ this.MinLevel) * 397 ^ this.MaxLevel) * 397 ^ (int)this.Tags) * 397 ^ this.Created.GetHashCode()) * 397 ^ (int)this.Type;
		}

		// Token: 0x06005E22 RID: 24098 RVA: 0x0007F4F3 File Offset: 0x0007D6F3
		public bool Equals(LookingFor x, LookingFor y)
		{
			return x.Equals(y);
		}

		// Token: 0x06005E23 RID: 24099 RVA: 0x00050A5F File Offset: 0x0004EC5F
		public int GetHashCode(LookingFor obj)
		{
			return obj.GetHashCode();
		}
	}
}
