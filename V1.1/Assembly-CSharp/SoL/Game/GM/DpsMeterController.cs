using System;
using System.Collections.Generic;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.GM
{
	// Token: 0x02000BE7 RID: 3047
	public class DpsMeterController : MonoBehaviour
	{
		// Token: 0x17001645 RID: 5701
		// (get) Token: 0x06005E40 RID: 24128 RVA: 0x0007F5D2 File Offset: 0x0007D7D2
		private bool IsPaused
		{
			get
			{
				return (this.m_ui && !this.m_ui.IsEnabled) || !LocalPlayer.GameEntity;
			}
		}

		// Token: 0x06005E42 RID: 24130 RVA: 0x0007F61C File Offset: 0x0007D81C
		private void Start()
		{
			DpsMeterOverlay.Launch();
		}

		// Token: 0x04005187 RID: 20871
		private List<UnprocessedDpsMeterInfo> m_unprocessedLogs;

		// Token: 0x04005188 RID: 20872
		private Dictionary<string, DpsMeterInfo> m_dpsInfoDict;

		// Token: 0x04005189 RID: 20873
		internal List<DpsMeterInfo> DpsInfoList;

		// Token: 0x0400518A RID: 20874
		private float[] m_totals = new float[7];

		// Token: 0x0400518B RID: 20875
		private float m_tick;

		// Token: 0x0400518C RID: 20876
		private float m_totalTime;

		// Token: 0x0400518D RID: 20877
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x0400518E RID: 20878
		[SerializeField]
		private DpsMeterList m_ui;

		// Token: 0x0400518F RID: 20879
		[SerializeField]
		private float m_updateFrequency = 0.2f;

		// Token: 0x04005190 RID: 20880
		internal const int kInitialCount = 200;
	}
}
