using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;

namespace SoL.UI
{
	// Token: 0x02000386 RID: 902
	public struct ArchetypeTooltipParameter : ITooltipParameter, IEquatable<ArchetypeTooltipParameter>
	{
		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x060018BE RID: 6334 RVA: 0x000534F3 File Offset: 0x000516F3
		public TooltipType Type
		{
			get
			{
				if (!this.IsDialogTooltip)
				{
					return TooltipType.Archetype;
				}
				return TooltipType.ArchetypeDialog;
			}
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x00105298 File Offset: 0x00103498
		public bool Equals(ArchetypeTooltipParameter other)
		{
			if (this.AlchemyPowerLevel != null != (other.AlchemyPowerLevel != null))
			{
				return false;
			}
			bool flag = (this.Instance == null && this.Archetype != null) ? object.Equals(this.Archetype, other.Archetype) : object.Equals(this.Instance, other.Instance);
			if (this.AlchemyPowerLevel != null && other.AlchemyPowerLevel != null)
			{
				return flag && this.AlchemyPowerLevel.Value == other.AlchemyPowerLevel.Value;
			}
			return flag;
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x00105338 File Offset: 0x00103538
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is ArchetypeTooltipParameter)
			{
				ArchetypeTooltipParameter other = (ArchetypeTooltipParameter)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x00105364 File Offset: 0x00103564
		public override int GetHashCode()
		{
			int num = 0;
			if (this.Instance != null)
			{
				num = this.Instance.GetHashCode();
			}
			else if (this.Archetype)
			{
				num = this.Archetype.GetHashCode();
			}
			if (this.AlchemyPowerLevel != null)
			{
				num = HashCode.Combine<int, AlchemyPowerLevel>(num, this.AlchemyPowerLevel.Value);
			}
			return num;
		}

		// Token: 0x04001FD0 RID: 8144
		public ArchetypeInstance Instance;

		// Token: 0x04001FD1 RID: 8145
		public BaseArchetype Archetype;

		// Token: 0x04001FD2 RID: 8146
		public string AdditionalText;

		// Token: 0x04001FD3 RID: 8147
		public string SubHeadingText;

		// Token: 0x04001FD4 RID: 8148
		public bool AtTrainer;

		// Token: 0x04001FD5 RID: 8149
		public bool AtMerchant;

		// Token: 0x04001FD6 RID: 8150
		public AlchemyPowerLevel? AlchemyPowerLevel;

		// Token: 0x04001FD7 RID: 8151
		public bool IsDialogTooltip;
	}
}
