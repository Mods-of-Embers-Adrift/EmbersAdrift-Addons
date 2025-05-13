using System;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007E6 RID: 2022
	public class Armstrong
	{
		// Token: 0x06003ADA RID: 15066 RVA: 0x001795C4 File Offset: 0x001777C4
		public void UpdateExternal()
		{
			this.m_totalCounter++;
			if (this.m_previousTime == 0f)
			{
				this.m_prevTicks = DateTime.UtcNow.Ticks;
				this.m_previousTime = Time.realtimeSinceStartup;
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			long ticks = DateTime.UtcNow.Ticks;
			float num = (float)(ticks - this.m_prevTicks) / 1000f * 0.0001f;
			float num2 = realtimeSinceStartup - this.m_previousTime;
			this.m_accumulatedDelta += num2 - num;
			bool flag = false;
			if (this.m_accumulatedDelta > 1f)
			{
				flag = true;
				this.m_armstrongCounter++;
				this.m_accumulatedDelta = 0f;
			}
			this.m_accumulatedDelta -= 0.01f;
			this.m_accumulatedDelta = Mathf.Clamp(this.m_accumulatedDelta, 0f, 1f);
			if (flag && this.m_armstrongCounter % 10 == 0 && this.m_armstrongCounter > 100 && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.LogArmstrong(this.m_armstrongCounter, this.m_totalCounter);
			}
			if (this.m_totalCounter % 500 == 0 && this.m_armstrongCounter > 0)
			{
				this.m_armstrongCounter--;
			}
			this.m_previousTime = realtimeSinceStartup;
			this.m_prevTicks = ticks;
		}

		// Token: 0x04003961 RID: 14689
		private const float kDeltaTimeToConsiderSuspicious = 1f;

		// Token: 0x04003962 RID: 14690
		private const float kFrameTimeToIgnore = 0.01f;

		// Token: 0x04003963 RID: 14691
		private float m_previousTime;

		// Token: 0x04003964 RID: 14692
		private long m_prevTicks;

		// Token: 0x04003965 RID: 14693
		private float m_accumulatedDelta;

		// Token: 0x04003966 RID: 14694
		private int m_armstrongCounter;

		// Token: 0x04003967 RID: 14695
		private int m_totalCounter;
	}
}
