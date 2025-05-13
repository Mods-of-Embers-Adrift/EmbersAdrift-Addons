using System;
using SoL.Networking.Database;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B6C RID: 2924
	internal struct NewCharacterKey
	{
		// Token: 0x060059DF RID: 23007 RVA: 0x0007C445 File Offset: 0x0007A645
		internal NewCharacterKey(CharacterSex sex, CharacterBuildType buildType)
		{
			this.Sex = sex;
			this.BuildType = buildType;
		}

		// Token: 0x04004F04 RID: 20228
		public readonly CharacterSex Sex;

		// Token: 0x04004F05 RID: 20229
		public readonly CharacterBuildType BuildType;
	}
}
