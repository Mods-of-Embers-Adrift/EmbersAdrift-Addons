using System;
using SoL.Networking.Database;

namespace SoL.Game
{
	// Token: 0x020005B3 RID: 1459
	public class TimePlayed
	{
		// Token: 0x06002E28 RID: 11816 RVA: 0x0015182C File Offset: 0x0014FA2C
		public TimePlayed(CharacterRecord record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			this.m_record = record;
			this.m_referenceTime = DateTime.UtcNow;
			if (this.m_record.TimePlayed == null)
			{
				this.m_record.TimePlayed = new TimeSpan?(TimeSpan.Zero);
			}
			this.m_timePlayed = this.m_record.TimePlayed.Value;
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x0015189C File Offset: 0x0014FA9C
		public TimeSpan GetUpdateTimePlayed()
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan t = utcNow - this.m_referenceTime;
			this.m_timePlayed += t;
			this.m_referenceTime = utcNow;
			this.m_record.TimePlayed = new TimeSpan?(this.m_timePlayed);
			return this.m_timePlayed;
		}

		// Token: 0x04002D93 RID: 11667
		private readonly CharacterRecord m_record;

		// Token: 0x04002D94 RID: 11668
		private DateTime m_referenceTime;

		// Token: 0x04002D95 RID: 11669
		private TimeSpan m_timePlayed;
	}
}
