using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SoL.Game
{
	// Token: 0x020005CE RID: 1486
	[ExecuteInEditMode]
	public class SolReflectionProbeUpdater : MonoBehaviour
	{
		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06002F4C RID: 12108 RVA: 0x00060A1B File Offset: 0x0005EC1B
		public ReflectionProbe Probe
		{
			get
			{
				return this.m_probe;
			}
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x00060A23 File Offset: 0x0005EC23
		private void OnEnable()
		{
			if (!this.Probe)
			{
				return;
			}
			this.m_renderId = null;
			this.SetupReflectionProbe();
			this.RenderProbe();
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x00060A4B File Offset: 0x0005EC4B
		private void OnDisable()
		{
			if (this.Probe)
			{
				this.Probe.enabled = false;
			}
		}

		// Token: 0x06002F4F RID: 12111 RVA: 0x00060A66 File Offset: 0x0005EC66
		private void OnValidate()
		{
			if (!SolReflectionProbeUpdater.m_validResolutions.Contains(this.m_resolution))
			{
				this.m_resolution = 128;
			}
			this.SetupReflectionProbe();
		}

		// Token: 0x06002F50 RID: 12112 RVA: 0x00060A8B File Offset: 0x0005EC8B
		private void Awake()
		{
			Options.VideoOptions.Reflections.Changed += this.ReflectionsOnChanged;
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x00060AA3 File Offset: 0x0005ECA3
		private void OnDestroy()
		{
			Options.VideoOptions.Reflections.Changed -= this.ReflectionsOnChanged;
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x00156A30 File Offset: 0x00154C30
		private void Update()
		{
			if (!this.Probe || !Options.VideoOptions.Reflections.Value)
			{
				return;
			}
			if (this.m_renderId != null)
			{
				if (this.Probe.IsFinishedRendering(this.m_renderId.Value))
				{
					this.Probe.timeSlicingMode = this.m_timeSlicingMode;
					this.m_timeOfNextUpdate = Time.time + this.m_updateInterval;
					this.m_renderId = null;
				}
				return;
			}
			if (Time.time >= this.m_timeOfNextUpdate)
			{
				this.RenderProbe();
			}
		}

		// Token: 0x06002F53 RID: 12115 RVA: 0x00156AC0 File Offset: 0x00154CC0
		private void SetupReflectionProbe()
		{
			if (!this.Probe)
			{
				return;
			}
			this.Probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			this.Probe.mode = ReflectionProbeMode.Realtime;
			this.Probe.resolution = this.m_resolution;
			this.Probe.timeSlicingMode = (Application.isPlaying ? ReflectionProbeTimeSlicingMode.AllFacesAtOnce : this.m_timeSlicingMode);
			if (Application.isPlaying)
			{
				this.Probe.enabled = Options.VideoOptions.Reflections.Value;
			}
		}

		// Token: 0x06002F54 RID: 12116 RVA: 0x00060ABB File Offset: 0x0005ECBB
		private void RenderProbe()
		{
			if (this.Probe && this.Probe.enabled)
			{
				this.m_renderId = new int?(this.Probe.RenderProbe());
			}
		}

		// Token: 0x06002F55 RID: 12117 RVA: 0x00060AED File Offset: 0x0005ECED
		private void ReflectionsOnChanged()
		{
			if (this.Probe)
			{
				this.Probe.enabled = Options.VideoOptions.Reflections.Value;
				if (!this.Probe.enabled)
				{
					this.m_renderId = null;
				}
			}
		}

		// Token: 0x04002E61 RID: 11873
		[SerializeField]
		private ReflectionProbe m_probe;

		// Token: 0x04002E62 RID: 11874
		[SerializeField]
		private ReflectionProbeTimeSlicingMode m_timeSlicingMode = ReflectionProbeTimeSlicingMode.IndividualFaces;

		// Token: 0x04002E63 RID: 11875
		[SerializeField]
		private int m_resolution = 256;

		// Token: 0x04002E64 RID: 11876
		[SerializeField]
		private float m_updateInterval = 1f;

		// Token: 0x04002E65 RID: 11877
		private float m_timeOfNextUpdate;

		// Token: 0x04002E66 RID: 11878
		private int? m_renderId;

		// Token: 0x04002E67 RID: 11879
		private static readonly List<int> m_validResolutions = new List<int>
		{
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048
		};
	}
}
