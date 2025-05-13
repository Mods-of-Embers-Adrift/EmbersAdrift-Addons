using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3E RID: 3134
	[Serializable]
	public class GameEntityData
	{
		// Token: 0x060060C2 RID: 24770 RVA: 0x0008130C File Offset: 0x0007F50C
		public void Reset()
		{
			this.Type = GameEntityType.None;
			this.Name = string.Empty;
			this.Level = 0;
		}

		// Token: 0x0400535E RID: 21342
		public GameEntityType Type;

		// Token: 0x0400535F RID: 21343
		public string Name;

		// Token: 0x04005360 RID: 21344
		public int Level;
	}
}
