using System;
using SoL.Game.Audio;
using SoL.Game.Discovery;
using SoL.Game.States;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200054C RID: 1356
	public class ActivatedMonolith : BaseState
	{
		// Token: 0x14000083 RID: 131
		// (add) Token: 0x0600293A RID: 10554 RVA: 0x00140F40 File Offset: 0x0013F140
		// (remove) Token: 0x0600293B RID: 10555 RVA: 0x00140F74 File Offset: 0x0013F174
		public static event Action MonolithStateChanged;

		// Token: 0x0600293C RID: 10556 RVA: 0x0005C7D7 File Offset: 0x0005A9D7
		private void Start()
		{
			this.RefreshVisuals();
		}

		// Token: 0x0600293D RID: 10557 RVA: 0x0005C7DF File Offset: 0x0005A9DF
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x00140FA8 File Offset: 0x0013F1A8
		private void RefreshVisuals()
		{
			bool flag = base.CurrentState == 1;
			if (this.m_pulser)
			{
				this.m_pulser.EnablePulsing = flag;
			}
			for (int i = 0; i < this.m_visualsToToggle.Length; i++)
			{
				if (this.m_visualsToToggle[i])
				{
					this.m_visualsToToggle[i].SetActive(flag);
				}
			}
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x00141008 File Offset: 0x0013F208
		protected override void StateChangedInternal()
		{
			this.RefreshVisuals();
			if (base.CurrentState != 1)
			{
				if (base.CurrentState == 0)
				{
					AudioEvent deactivatedEvent = this.m_deactivatedEvent;
					if (deactivatedEvent == null)
					{
						return;
					}
					deactivatedEvent.Play(1f);
				}
				return;
			}
			AudioEvent activatedEvent = this.m_activatedEvent;
			if (activatedEvent == null)
			{
				return;
			}
			activatedEvent.Play(1f);
		}

		// Token: 0x04002A2E RID: 10798
		public const float kPollRate = 30f;

		// Token: 0x04002A2F RID: 10799
		private const string kPrefix = "Ley";

		// Token: 0x04002A30 RID: 10800
		public const string kCompassName = "Ley Finder";

		// Token: 0x04002A31 RID: 10801
		public const string kPedestalName = "Ley Anchor";

		// Token: 0x04002A33 RID: 10803
		[SerializeField]
		private MonolithProfile m_profile;

		// Token: 0x04002A34 RID: 10804
		[SerializeField]
		private ActivatedMonolithEmissionPulser m_pulser;

		// Token: 0x04002A35 RID: 10805
		[SerializeField]
		private AudioEvent m_activatedEvent;

		// Token: 0x04002A36 RID: 10806
		[SerializeField]
		private AudioEvent m_deactivatedEvent;

		// Token: 0x04002A37 RID: 10807
		[SerializeField]
		private GameObject[] m_visualsToToggle;

		// Token: 0x04002A38 RID: 10808
		private ActiveMonolithRecord m_record;
	}
}
