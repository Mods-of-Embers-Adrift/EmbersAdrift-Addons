using System;

namespace SoL.Game.Grouping
{
	// Token: 0x02000BE1 RID: 3041
	[Flags]
	public enum LookingTags
	{
		// Token: 0x0400514D RID: 20813
		None = 0,
		// Token: 0x0400514E RID: 20814
		Defender = 2,
		// Token: 0x0400514F RID: 20815
		Supporter = 4,
		// Token: 0x04005150 RID: 20816
		Striker = 8,
		// Token: 0x04005151 RID: 20817
		Hunting = 16,
		// Token: 0x04005152 RID: 20818
		Dungeoneering = 32,
		// Token: 0x04005153 RID: 20819
		Crafting = 64,
		// Token: 0x04005154 RID: 20820
		Gathering = 128,
		// Token: 0x04005155 RID: 20821
		Mentoring = 256,
		// Token: 0x04005156 RID: 20822
		Questing = 512,
		// Token: 0x04005157 RID: 20823
		GrizzledPeaks = 524288,
		// Token: 0x04005158 RID: 20824
		HighlandHills = 1048576,
		// Token: 0x04005159 RID: 20825
		Karst = 2097152,
		// Token: 0x0400515A RID: 20826
		Undercroft = 2097152,
		// Token: 0x0400515B RID: 20827
		EmberVeins = 2097152,
		// Token: 0x0400515C RID: 20828
		CentralVeins2 = 4194304,
		// Token: 0x0400515D RID: 20829
		CentralVeins1 = 8388608,
		// Token: 0x0400515E RID: 20830
		GrimstoneCanyon = 16777216,
		// Token: 0x0400515F RID: 20831
		DryfootStronghold = 33554432,
		// Token: 0x04005160 RID: 20832
		Dryfoot = 67108864,
		// Token: 0x04005161 RID: 20833
		RedshoreRidge = 134217728,
		// Token: 0x04005162 RID: 20834
		Redshore = 268435456,
		// Token: 0x04005163 RID: 20835
		Meadowlands = 536870912,
		// Token: 0x04005164 RID: 20836
		Northreach = 1073741824,
		// Token: 0x04005165 RID: 20837
		NewhavenValley = -2147483648,
		// Token: 0x04005166 RID: 20838
		AllRoles = 14,
		// Token: 0x04005167 RID: 20839
		AllActivities = 1008,
		// Token: 0x04005168 RID: 20840
		AllZones = -182976512,
		// Token: 0x04005169 RID: 20841
		All = -182975490
	}
}
