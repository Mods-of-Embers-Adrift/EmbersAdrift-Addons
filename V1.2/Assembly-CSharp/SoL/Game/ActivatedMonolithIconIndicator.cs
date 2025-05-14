using System;
using SoL.Game.Discovery;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x0200054F RID: 1359
	public class ActivatedMonolithIconIndicator : MonoBehaviour
	{
		// Token: 0x0600294D RID: 10573 RVA: 0x0005C8E4 File Offset: 0x0005AAE4
		private void Awake()
		{
			ActivatedMonolithReplicator.ActiveMonolithListChanged += this.OnActiveMonolithListChanged;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			this.RefreshIcon();
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x0005C90E File Offset: 0x0005AB0E
		private void OnDestroy()
		{
			ActivatedMonolithReplicator.ActiveMonolithListChanged -= this.OnActiveMonolithListChanged;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x0600294F RID: 10575 RVA: 0x0005C932 File Offset: 0x0005AB32
		private void OnActiveMonolithListChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x0005C932 File Offset: 0x0005AB32
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.RefreshIcon();
		}

		// Token: 0x06002951 RID: 10577 RVA: 0x0005C93A File Offset: 0x0005AB3A
		private void RefreshIcon()
		{
			if (this.m_image)
			{
				this.m_image.enabled = (this.m_toMonitor && this.m_toMonitor.IsAvailable());
			}
		}

		// Token: 0x04002A46 RID: 10822
		[SerializeField]
		private MonolithProfile m_toMonitor;

		// Token: 0x04002A47 RID: 10823
		[SerializeField]
		private Image m_image;
	}
}
