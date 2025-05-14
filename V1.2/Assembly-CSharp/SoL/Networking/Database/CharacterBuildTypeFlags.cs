using System;

namespace SoL.Networking.Database
{
	// Token: 0x02000425 RID: 1061
	[Flags]
	public enum CharacterBuildTypeFlags
	{
		// Token: 0x040023D5 RID: 9173
		None = 0,
		// Token: 0x040023D6 RID: 9174
		Stoic = 1,
		// Token: 0x040023D7 RID: 9175
		Brawny = 2,
		// Token: 0x040023D8 RID: 9176
		Lean = 4,
		// Token: 0x040023D9 RID: 9177
		Heavyset = 8,
		// Token: 0x040023DA RID: 9178
		Rotund = 16,
		// Token: 0x040023DB RID: 9179
		All = 31
	}
}
