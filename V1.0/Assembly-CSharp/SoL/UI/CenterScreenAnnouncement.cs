using System;
using System.Collections;
using SoL.Game;
using SoL.Game.Messages;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200034E RID: 846
	public class CenterScreenAnnouncement : BaseDialog<CenterScreenAnnouncementOptions>
	{
		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00052300 File Offset: 0x00050500
		public UniqueId? SourceId
		{
			get
			{
				return this.m_currentOptions.SourceId;
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x0005230D File Offset: 0x0005050D
		public bool Active
		{
			get
			{
				return this.m_active;
			}
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x00101DF0 File Offset: 0x000FFFF0
		public void DelayedInit(CenterScreenAnnouncementOptions opts)
		{
			if (this.m_initCo != null)
			{
				base.StopCoroutine(this.m_initCo);
			}
			this.m_elapsedTime = 0f;
			this.m_currentOptions = opts;
			this.m_active = true;
			this.m_initCo = this.InitCo(opts);
			base.StartCoroutine(this.m_initCo);
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x00052315 File Offset: 0x00050515
		public void ExtendInit(CenterScreenAnnouncementOptions opts)
		{
			this.m_elapsedTime = 0f;
			this.m_currentOptions = opts;
			this.AddToChatQueue(opts);
			base.SetTexts(opts);
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00052337 File Offset: 0x00050537
		private IEnumerator InitCo(CenterScreenAnnouncementOptions opts)
		{
			while (LocalPlayer.GameEntity == null)
			{
				yield return null;
			}
			if (opts.ShowDelay > 0f)
			{
				yield return new WaitForSeconds(opts.ShowDelay);
			}
			base.Init(opts);
			this.AddToChatQueue(opts);
			while (this.m_elapsedTime < this.m_transitionSettings.ShowTime + opts.TimeShown)
			{
				this.m_elapsedTime += Time.deltaTime;
				yield return null;
			}
			this.Hide(false);
			this.m_initCo = null;
			this.m_active = false;
			yield break;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x0005234D File Offset: 0x0005054D
		private void AddToChatQueue(CenterScreenAnnouncementOptions opts)
		{
			if (opts.MessageType != MessageType.None)
			{
				MessageManager.ChatQueue.AddToQueue(opts.MessageType, opts.Text);
			}
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0005236E File Offset: 0x0005056E
		public override void ResetDialog()
		{
			if (this.m_initCo != null && this != null)
			{
				base.StopCoroutine(this.m_initCo);
				this.m_initCo = null;
			}
			this.m_active = false;
			base.ResetDialog();
		}

		// Token: 0x04001EE3 RID: 7907
		private IEnumerator m_initCo;

		// Token: 0x04001EE4 RID: 7908
		private bool m_active;

		// Token: 0x04001EE5 RID: 7909
		private float m_elapsedTime;
	}
}
