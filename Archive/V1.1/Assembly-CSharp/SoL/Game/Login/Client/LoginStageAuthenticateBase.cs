using System;
using System.Collections;
using SoL.Managers;
using SoL.Networking.SolServer;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B32 RID: 2866
	public abstract class LoginStageAuthenticateBase : LoginStage
	{
		// Token: 0x0600580F RID: 22543 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void StageEnterInternal()
		{
		}

		// Token: 0x06005810 RID: 22544 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void StageEnterInternalEnd()
		{
		}

		// Token: 0x06005811 RID: 22545 RVA: 0x001E5014 File Offset: 0x001E3214
		public override void StageEnter()
		{
			base.StageEnter();
			this.m_controller.SetStatusText("");
			this.m_loggingIn = false;
			this.StageEnterInternal();
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.ResetRaid();
			}
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.ResetGroup();
			}
			if (SessionData.ReturnToCharacterSelection)
			{
				return;
			}
			SessionData.Clear();
			if (this.m_connecting != null)
			{
				base.StopCoroutine(this.m_connecting);
			}
			this.m_connecting = this.Connect();
			base.StartCoroutine(this.m_connecting);
			this.StageEnterInternalEnd();
		}

		// Token: 0x06005812 RID: 22546 RVA: 0x0007ABB0 File Offset: 0x00078DB0
		public override void StageError(string err)
		{
			base.StageError(err);
			this.m_loggingIn = false;
		}

		// Token: 0x06005813 RID: 22547 RVA: 0x0007ABC0 File Offset: 0x00078DC0
		public override void StageErrorCritical(string err)
		{
			base.StageErrorCritical(err);
			this.m_loggingIn = false;
		}

		// Token: 0x06005814 RID: 22548 RVA: 0x0007ABD0 File Offset: 0x00078DD0
		private IEnumerator Connect()
		{
			if (SolServerConnectionManager.CurrentConnection != null && SolServerConnectionManager.CurrentConnection.IsConnected)
			{
				GameManager.SolServerConnectionManager.Disconnect();
			}
			while (SolServerConnectionManager.CurrentConnection != null && SolServerConnectionManager.CurrentConnection.IsConnected)
			{
				yield return null;
			}
			this.m_atLogin = false;
			GameManager.SolServerConnectionManager.InitializeConnection();
			this.m_connecting = null;
			yield return null;
			yield break;
		}

		// Token: 0x04004D99 RID: 19865
		[NonSerialized]
		protected bool m_loggingIn;

		// Token: 0x04004D9A RID: 19866
		[NonSerialized]
		protected bool m_atLogin;

		// Token: 0x04004D9B RID: 19867
		private IEnumerator m_connecting;
	}
}
