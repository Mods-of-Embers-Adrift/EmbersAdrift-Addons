using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007E4 RID: 2020
	public class VfxDurationAdjuster : MonoBehaviour
	{
		// Token: 0x06003AD2 RID: 15058 RVA: 0x00179394 File Offset: 0x00177594
		private void Awake()
		{
			if (this.m_systems != null && this.m_systems.Length != 0 && this.m_systems[0])
			{
				this.m_parent = new VfxDurationAdjuster.VfxData(this.m_systems[0]);
				this.m_children = new List<VfxDurationAdjuster.VfxData>(this.m_systems.Length - 1);
				for (int i = 1; i < this.m_systems.Length; i++)
				{
					this.m_children.Add(new VfxDurationAdjuster.VfxData(this.m_systems[i]));
				}
			}
		}

		// Token: 0x06003AD3 RID: 15059 RVA: 0x00179414 File Offset: 0x00177614
		private void ResetDurations()
		{
			if (this.m_parent != null && this.m_parent.System)
			{
				this.m_parent.System.Stop(true);
				this.m_parent.ResetDuration();
				for (int i = 0; i < this.m_children.Count; i++)
				{
					this.m_children[i].ResetDuration();
				}
			}
		}

		// Token: 0x06003AD4 RID: 15060 RVA: 0x00179480 File Offset: 0x00177680
		public void Init(float multiplier)
		{
			if (this.m_parent != null && this.m_parent.System)
			{
				this.m_parent.System.Stop(true);
				this.m_parent.UpdateDuration(multiplier);
				for (int i = 0; i < this.m_children.Count; i++)
				{
					this.m_children[i].UpdateDuration(multiplier);
				}
				this.m_parent.System.Play(true);
			}
		}

		// Token: 0x0400395B RID: 14683
		[SerializeField]
		private ParticleSystem[] m_systems;

		// Token: 0x0400395C RID: 14684
		private VfxDurationAdjuster.VfxData m_parent;

		// Token: 0x0400395D RID: 14685
		private List<VfxDurationAdjuster.VfxData> m_children;

		// Token: 0x020007E5 RID: 2021
		private class VfxData
		{
			// Token: 0x17000D7B RID: 3451
			// (get) Token: 0x06003AD6 RID: 15062 RVA: 0x00067DFA File Offset: 0x00065FFA
			public ParticleSystem System
			{
				get
				{
					return this.m_system;
				}
			}

			// Token: 0x06003AD7 RID: 15063 RVA: 0x00179500 File Offset: 0x00177700
			public VfxData(ParticleSystem system)
			{
				if (!system)
				{
					throw new ArgumentNullException("system");
				}
				this.m_system = system;
				ParticleSystem.MainModule main = this.m_system.main;
				this.m_adjustDuration = !main.loop;
				this.m_defaultDuration = main.duration;
			}

			// Token: 0x06003AD8 RID: 15064 RVA: 0x00179564 File Offset: 0x00177764
			public void ResetDuration()
			{
				if (this.m_adjustDuration)
				{
					this.m_system.main.duration = this.m_defaultDuration;
				}
			}

			// Token: 0x06003AD9 RID: 15065 RVA: 0x00179594 File Offset: 0x00177794
			public void UpdateDuration(float multiplier)
			{
				if (this.m_adjustDuration)
				{
					this.m_system.main.duration = this.m_defaultDuration * multiplier;
				}
			}

			// Token: 0x0400395E RID: 14686
			private readonly ParticleSystem m_system;

			// Token: 0x0400395F RID: 14687
			private readonly bool m_adjustDuration;

			// Token: 0x04003960 RID: 14688
			private readonly float m_defaultDuration = 1f;
		}
	}
}
