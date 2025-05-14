using System;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.UI
{
	// Token: 0x020003A7 RID: 935
	public class WorldSpaceUIPass : MonoBehaviour
	{
		// Token: 0x0600198A RID: 6538 RVA: 0x00054048 File Offset: 0x00052248
		private void Awake()
		{
			Options.VideoOptions.NvidiaDLSSEnable.Changed += this.NvidiaDLSSEnableOnChanged;
			Options.VideoOptions.ResolutionScale.Changed += this.ResolutionScaleOnChanged;
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x00054076 File Offset: 0x00052276
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0005407E File Offset: 0x0005227E
		private void OnDestroy()
		{
			Options.VideoOptions.NvidiaDLSSEnable.Changed -= this.NvidiaDLSSEnableOnChanged;
			Options.VideoOptions.ResolutionScale.Changed -= this.ResolutionScaleOnChanged;
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x00054076 File Offset: 0x00052276
		private void ResolutionScaleOnChanged()
		{
			this.Refresh();
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x00054076 File Offset: 0x00052276
		private void NvidiaDLSSEnableOnChanged()
		{
			this.Refresh();
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x00106C70 File Offset: 0x00104E70
		private void Refresh()
		{
			if (NvidiaDLSS.SupportsNvidiaDLSS && Options.VideoOptions.NvidiaDLSSEnable.Value)
			{
				this.m_volume.enabled = false;
				return;
			}
			if (Options.VideoOptions.ResolutionScale.Value < 1f)
			{
				this.m_volume.enabled = false;
				return;
			}
			this.m_volume.enabled = true;
		}

		// Token: 0x0400207F RID: 8319
		[SerializeField]
		private CustomPassVolume m_volume;
	}
}
