using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x0200070A RID: 1802
	[ExecuteInEditMode]
	public class SolHDRPReflectionProbeUpdater : MonoBehaviour
	{
		// Token: 0x06003642 RID: 13890 RVA: 0x00065295 File Offset: 0x00063495
		private void OnEnable()
		{
			this.SetupReflectionProbe();
			this.RenderProbe();
		}

		// Token: 0x06003643 RID: 13891 RVA: 0x000652A3 File Offset: 0x000634A3
		private void OnDisable()
		{
			this.m_timeOfNextUpdate = null;
			if (this.m_probe)
			{
				this.m_probe.enabled = false;
			}
		}

		// Token: 0x06003644 RID: 13892 RVA: 0x000652CA File Offset: 0x000634CA
		private void Awake()
		{
			Options.VideoOptions.Reflections.Changed += this.ReflectionsOnChanged;
		}

		// Token: 0x06003645 RID: 13893 RVA: 0x000652E2 File Offset: 0x000634E2
		private void OnDestroy()
		{
			Options.VideoOptions.Reflections.Changed -= this.ReflectionsOnChanged;
		}

		// Token: 0x06003646 RID: 13894 RVA: 0x000652FA File Offset: 0x000634FA
		private void Update()
		{
			if (Options.VideoOptions.Reflections.Value && this.m_timeOfNextUpdate != null && this.m_timeOfNextUpdate.Value >= Time.time)
			{
				this.RenderProbe();
			}
		}

		// Token: 0x06003647 RID: 13895 RVA: 0x00169314 File Offset: 0x00167514
		private void SetupReflectionProbe()
		{
			if (!this.m_probe || !this.m_reflectionData)
			{
				return;
			}
			this.m_reflectionData.settingsRaw.mode = ProbeSettings.Mode.Realtime;
			this.m_reflectionData.settingsRaw.realtimeMode = ProbeSettings.RealtimeMode.OnDemand;
			this.m_probe.cullingMask = this.m_mask;
			if (Application.isPlaying)
			{
				this.m_probe.enabled = Options.VideoOptions.Reflections.Value;
			}
		}

		// Token: 0x06003648 RID: 13896 RVA: 0x00169390 File Offset: 0x00167590
		private void RenderProbe()
		{
			this.m_timeOfNextUpdate = null;
			if (this.m_probe && this.m_probe.enabled)
			{
				this.m_reflectionData.RequestRenderNextUpdate();
				this.m_timeOfNextUpdate = new float?(Time.time + this.m_updateInterval);
			}
		}

		// Token: 0x06003649 RID: 13897 RVA: 0x001693E8 File Offset: 0x001675E8
		private void ReflectionsOnChanged()
		{
			if (this.m_probe)
			{
				bool value = Options.VideoOptions.Reflections.Value;
				this.m_probe.enabled = value;
				if (value)
				{
					this.m_timeOfNextUpdate = new float?(Time.time);
					return;
				}
				this.m_timeOfNextUpdate = null;
			}
		}

		// Token: 0x04003431 RID: 13361
		[SerializeField]
		private LayerMask m_mask;

		// Token: 0x04003432 RID: 13362
		[SerializeField]
		private ReflectionProbe m_probe;

		// Token: 0x04003433 RID: 13363
		[SerializeField]
		private HDAdditionalReflectionData m_reflectionData;

		// Token: 0x04003434 RID: 13364
		[SerializeField]
		private float m_updateInterval = 1f;

		// Token: 0x04003435 RID: 13365
		private float? m_timeOfNextUpdate;
	}
}
