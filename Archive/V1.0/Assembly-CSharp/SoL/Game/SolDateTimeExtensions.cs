using System;
using NetStack.Serialization;

namespace SoL.Game
{
	// Token: 0x0200057A RID: 1402
	public static class SolDateTimeExtensions
	{
		// Token: 0x06002B54 RID: 11092 RVA: 0x0005E084 File Offset: 0x0005C284
		public static GameDateTime AsGameDateTime(this DateTime dt)
		{
			return new GameDateTime(dt);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x0005E08C File Offset: 0x0005C28C
		public static BitBuffer AddGameDateTime(this BitBuffer buffer, GameDateTime sdt)
		{
			buffer.AddLong(sdt.Ticks);
			return buffer;
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x0005E09D File Offset: 0x0005C29D
		public static GameDateTime ReadGameDateTime(this BitBuffer buffer)
		{
			return new GameDateTime(buffer.ReadLong());
		}
	}
}
