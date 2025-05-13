using System;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D27 RID: 3367
	public class AmbientAudioInjector : MonoBehaviour, IAmbientAudioZone
	{
		// Token: 0x1700184E RID: 6222
		// (get) Token: 0x06006553 RID: 25939 RVA: 0x00084357 File Offset: 0x00082557
		public AmbientAudioZoneProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x1700184F RID: 6223
		// (get) Token: 0x06006554 RID: 25940 RVA: 0x00082BC2 File Offset: 0x00080DC2
		public int Key
		{
			get
			{
				return this.GetHashCode();
			}
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x0008435F File Offset: 0x0008255F
		private void Start()
		{
			if (LocalPlayer.GameEntity)
			{
				this.Inject();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			this.m_subscribed = true;
		}

		// Token: 0x06006556 RID: 25942 RVA: 0x0020C898 File Offset: 0x0020AA98
		private void OnDestroy()
		{
			this.Unsubscribe();
			if (this.m_injected && LocalPlayer.GameEntity && LocalPlayer.GameEntity.AmbientAudioController)
			{
				LocalPlayer.GameEntity.AmbientAudioController.ExitZone(this);
				this.m_injected = false;
			}
		}

		// Token: 0x06006557 RID: 25943 RVA: 0x0008438C File Offset: 0x0008258C
		private void Unsubscribe()
		{
			if (this.m_subscribed)
			{
				LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
				this.m_subscribed = false;
			}
		}

		// Token: 0x06006558 RID: 25944 RVA: 0x000843AE File Offset: 0x000825AE
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.Unsubscribe();
			this.Inject();
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x000843BC File Offset: 0x000825BC
		private void Inject()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.AmbientAudioController)
			{
				LocalPlayer.GameEntity.AmbientAudioController.EnterZone(this);
				this.m_injected = true;
			}
		}

		// Token: 0x0400581B RID: 22555
		[SerializeField]
		private AmbientAudioZoneProfile m_profile;

		// Token: 0x0400581C RID: 22556
		private bool m_subscribed;

		// Token: 0x0400581D RID: 22557
		private bool m_injected;
	}
}
