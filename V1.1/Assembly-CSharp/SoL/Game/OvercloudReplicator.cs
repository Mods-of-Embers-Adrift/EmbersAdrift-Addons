using System;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005CD RID: 1485
	public class OvercloudReplicator : SyncVarReplicator
	{
		// Token: 0x06002F46 RID: 12102 RVA: 0x00156868 File Offset: 0x00154A68
		private void Awake()
		{
			if (OvercloudReplicator.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			OvercloudReplicator.Instance = this;
			if (GameManager.IsServer)
			{
				this.m_currentProfile = this.m_startingProfile;
				this.BaseWindTime.Value = 0f;
				this.TimeOfLastWeatherChange.Value = DateTime.UtcNow.Ticks;
				this.WeatherProfileId.Value = this.m_currentProfile.Id;
			}
		}

		// Token: 0x06002F47 RID: 12103 RVA: 0x0004475B File Offset: 0x0004295B
		public void SetNewProfile(SolWeatherProfile profile)
		{
		}

		// Token: 0x06002F48 RID: 12104 RVA: 0x001568E8 File Offset: 0x00154AE8
		public void SetGameTimeOverride(float value)
		{
			this.TimeOfLastTimeOverrideChange.Value = new long?(DateTime.UtcNow.Ticks);
			this.TimeOverride.Value = new float?(value);
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x00156924 File Offset: 0x00154B24
		public void ResetGameTime()
		{
			this.TimeOfLastTimeOverrideChange.Value = null;
			this.TimeOverride.Value = null;
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x0015695C File Offset: 0x00154B5C
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.BaseWindTime);
			this.BaseWindTime.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.TimeOfLastTimeOverrideChange);
			this.TimeOfLastTimeOverrideChange.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.TimeOfLastWeatherChange);
			this.TimeOfLastWeatherChange.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.TimeOverride);
			this.TimeOverride.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.WeatherProfileId);
			this.WeatherProfileId.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002E59 RID: 11865
		public static OvercloudReplicator Instance;

		// Token: 0x04002E5A RID: 11866
		[SerializeField]
		private SolWeatherProfile m_startingProfile;

		// Token: 0x04002E5B RID: 11867
		private SolWeatherProfile m_currentProfile;

		// Token: 0x04002E5C RID: 11868
		public readonly SynchronizedFloat BaseWindTime = new SynchronizedFloat();

		// Token: 0x04002E5D RID: 11869
		public readonly SynchronizedLong TimeOfLastWeatherChange = new SynchronizedLong();

		// Token: 0x04002E5E RID: 11870
		public readonly SynchronizedUniqueId WeatherProfileId = new SynchronizedUniqueId();

		// Token: 0x04002E5F RID: 11871
		public readonly SynchronizedNullableLong TimeOfLastTimeOverrideChange = new SynchronizedNullableLong();

		// Token: 0x04002E60 RID: 11872
		public readonly SynchronizedNullableFloat TimeOverride = new SynchronizedNullableFloat();
	}
}
