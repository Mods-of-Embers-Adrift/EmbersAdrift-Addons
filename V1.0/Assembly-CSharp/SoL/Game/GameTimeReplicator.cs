using System;
using NetStack.Serialization;
using SoL.Game.SkyDome;
using SoL.Networking;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005CA RID: 1482
	public class GameTimeReplicator : SyncVarReplicator
	{
		// Token: 0x06002F3C RID: 12092 RVA: 0x00060977 File Offset: 0x0005EB77
		private void Awake()
		{
			if (GameTimeReplicator.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			GameTimeReplicator.Instance = this;
			SkyDomeManager.GameTimeReplicator = this;
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x0015670C File Offset: 0x0015490C
		public void SetGameTimeOverride(float value)
		{
			this.TimeOfLastTimeOverrideChange.Value = new long?(DateTime.UtcNow.Ticks);
			this.TimeOverride.Value = new float?(value);
			DynamicGI.UpdateEnvironment();
		}

		// Token: 0x06002F3E RID: 12094 RVA: 0x0015674C File Offset: 0x0015494C
		public void ResetGameTime()
		{
			this.TimeOfLastTimeOverrideChange.Value = null;
			this.TimeOverride.Value = null;
			DynamicGI.UpdateEnvironment();
		}

		// Token: 0x06002F3F RID: 12095 RVA: 0x00156788 File Offset: 0x00154988
		public static void ProcessServerTimeUpdate(BitBuffer buffer, uint roundTripTime)
		{
			DateTime dateTime = buffer.ReadDateTime();
			dateTime += TimeSpan.FromMilliseconds(roundTripTime * 0.5);
			GameTimeReplicator.m_localTimeDelta = new TimeSpan?(dateTime - DateTime.UtcNow);
			SkyDomeManager.RefreshGameTimeIfNeeded();
		}

		// Token: 0x06002F40 RID: 12096 RVA: 0x001567D0 File Offset: 0x001549D0
		public static DateTime GetServerCorrectedDateTimeUtc()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (GameTimeReplicator.m_localTimeDelta == null)
			{
				return utcNow;
			}
			return utcNow + GameTimeReplicator.m_localTimeDelta.Value;
		}

		// Token: 0x06002F41 RID: 12097 RVA: 0x0006099E File Offset: 0x0005EB9E
		public static DateTime GetServerCorrectedDateTime(DateTime inputTime)
		{
			if (GameTimeReplicator.m_localTimeDelta == null)
			{
				return inputTime;
			}
			return inputTime + GameTimeReplicator.m_localTimeDelta.Value;
		}

		// Token: 0x06002F42 RID: 12098 RVA: 0x00156804 File Offset: 0x00154A04
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.TimeOfLastTimeOverrideChange);
			this.TimeOfLastTimeOverrideChange.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.TimeOverride);
			this.TimeOverride.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002E51 RID: 11857
		public static GameTimeReplicator Instance;

		// Token: 0x04002E52 RID: 11858
		public readonly SynchronizedNullableLong TimeOfLastTimeOverrideChange = new SynchronizedNullableLong();

		// Token: 0x04002E53 RID: 11859
		public readonly SynchronizedNullableFloat TimeOverride = new SynchronizedNullableFloat();

		// Token: 0x04002E54 RID: 11860
		private static TimeSpan? m_localTimeDelta;
	}
}
