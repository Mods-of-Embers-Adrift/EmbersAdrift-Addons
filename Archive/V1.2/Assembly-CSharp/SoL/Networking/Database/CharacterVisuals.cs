using System;
using System.Collections.Generic;

namespace SoL.Networking.Database
{
	// Token: 0x02000432 RID: 1074
	[Serializable]
	public class CharacterVisuals
	{
		// Token: 0x04002422 RID: 9250
		public CharacterSex Sex;

		// Token: 0x04002423 RID: 9251
		public CharacterBuildType BuildType;

		// Token: 0x04002424 RID: 9252
		public Dictionary<string, float> Dna;

		// Token: 0x04002425 RID: 9253
		public Dictionary<CharacterColorType, string> SharedColors;

		// Token: 0x04002426 RID: 9254
		public List<UniqueId> CustomizedSlots;
	}
}
