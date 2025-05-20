using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C59 RID: 3161
	public readonly struct EffectCategoryKey : IEquatable<EffectCategoryKey>
	{
		// Token: 0x17001753 RID: 5971
		// (get) Token: 0x06006128 RID: 24872 RVA: 0x00081749 File Offset: 0x0007F949
		public bool IsValidCategory
		{
			get
			{
				return this.CategoryFlags > EffectCategoryFlags.None;
			}
		}

		// Token: 0x06006129 RID: 24873 RVA: 0x00081754 File Offset: 0x0007F954
		public EffectCategoryKey(Polarity polarity, EffectCategory category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			this.Polarity = polarity;
			this.CategoryType = category.CategoryType;
			this.CategoryFlags = category.CategoryFlags;
			this.VariantType = category.VariantType;
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x0008178F File Offset: 0x0007F98F
		public bool Equals(EffectCategoryKey other)
		{
			return this.Polarity == other.Polarity && this.VariantType == other.VariantType && this.CategoryFlags.HasAnyFlags(other.CategoryFlags);
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x001FF4A4 File Offset: 0x001FD6A4
		public override bool Equals(object obj)
		{
			if (obj is EffectCategoryKey)
			{
				EffectCategoryKey other = (EffectCategoryKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x000817C0 File Offset: 0x0007F9C0
		public override int GetHashCode()
		{
			return (int)((this.Polarity * (Polarity)397 ^ (Polarity)this.CategoryFlags) * (Polarity)397 ^ (Polarity)this.VariantType);
		}

		// Token: 0x0600612D RID: 24877 RVA: 0x000817E2 File Offset: 0x0007F9E2
		public static bool operator ==(EffectCategoryKey left, EffectCategoryKey right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600612E RID: 24878 RVA: 0x000817EC File Offset: 0x0007F9EC
		public static bool operator !=(EffectCategoryKey left, EffectCategoryKey right)
		{
			return !(left == right);
		}

		// Token: 0x04005446 RID: 21574
		public readonly Polarity Polarity;

		// Token: 0x04005447 RID: 21575
		public readonly EffectCategoryType CategoryType;

		// Token: 0x04005448 RID: 21576
		public readonly EffectCategoryFlags CategoryFlags;

		// Token: 0x04005449 RID: 21577
		public readonly EffectVariantType VariantType;
	}
}
