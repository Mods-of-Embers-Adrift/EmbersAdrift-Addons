using System;
using SoL.Game.SkyDome;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC1 RID: 3265
	public abstract class CulledObject : MonoBehaviour, ICullee, IDayNightToggle
	{
		// Token: 0x170017A1 RID: 6049
		// (get) Token: 0x060062EB RID: 25323 RVA: 0x00082A6B File Offset: 0x00080C6B
		private DayNightEnableCondition DayNightCondition
		{
			get
			{
				if (!this.m_useZoneDayNightCondition || !(ZoneSettings.SettingsProfile != null))
				{
					return this.m_dayNightCondition;
				}
				return ZoneSettings.SettingsProfile.DayNightCondition;
			}
		}

		// Token: 0x170017A2 RID: 6050
		// (get) Token: 0x060062EC RID: 25324 RVA: 0x00082A93 File Offset: 0x00080C93
		internal bool InternalInitialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		// Token: 0x170017A3 RID: 6051
		// (get) Token: 0x060062ED RID: 25325 RVA: 0x00082A9B File Offset: 0x00080C9B
		internal int? InternalIndex
		{
			get
			{
				return this.m_index;
			}
		}

		// Token: 0x170017A4 RID: 6052
		// (get) Token: 0x060062EE RID: 25326 RVA: 0x00082AA3 File Offset: 0x00080CA3
		protected virtual float Radius
		{
			get
			{
				return this.m_radius;
			}
		}

		// Token: 0x060062EF RID: 25327 RVA: 0x00082AAB File Offset: 0x00080CAB
		protected virtual bool IsCulled()
		{
			return this.m_cullingFlags.HasBitFlag(CullingFlags.Manual) || this.m_cullingFlags.HasBitFlag(CullingFlags.Distance) || this.m_cullingFlags.HasBitFlag(CullingFlags.DayNight);
		}

		// Token: 0x060062F0 RID: 25328 RVA: 0x00082AD7 File Offset: 0x00080CD7
		protected virtual void OnEnable()
		{
			if (!GameManager.IsServer)
			{
				if (CullingManager.Instance != null)
				{
					CullingManager.Instance.RegisterCulledObject(this);
				}
				if (this.DayNightCondition != DayNightEnableCondition.Always)
				{
					SkyDomeManager.RegisterFX(this);
				}
			}
		}

		// Token: 0x060062F1 RID: 25329 RVA: 0x00082B06 File Offset: 0x00080D06
		protected virtual void OnDisable()
		{
			if (!GameManager.IsServer)
			{
				if (CullingManager.Instance != null)
				{
					CullingManager.Instance.DeregisterCulledObject(this);
				}
				if (this.DayNightCondition != DayNightEnableCondition.Always)
				{
					this.UnsetFlag(CullingFlags.DayNight);
					SkyDomeManager.UnregisterFX(this);
				}
			}
		}

		// Token: 0x060062F2 RID: 25330 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnCulleeBecameVisible()
		{
		}

		// Token: 0x060062F3 RID: 25331 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnCulleeBecameInvisible()
		{
		}

		// Token: 0x060062F4 RID: 25332 RVA: 0x00205E54 File Offset: 0x00204054
		protected virtual void OnDistanceBandChanged(CullingDistance previous, CullingDistance current, bool force)
		{
			if (force || !this.m_initialized || this.m_currentBand != current)
			{
				if (previous == CullingDistance.VeryNear)
				{
					this.m_cullingFlags = this.m_cullingFlags.UnsetLimitBitFlags();
				}
				this.m_currentBand = current;
				CullingFlags cullingFlags = this.m_cullingFlags;
				this.m_cullingFlags = this.GetUpdatedFlags(cullingFlags);
				this.RefreshCullee();
			}
			this.m_initialized = true;
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x00205EB4 File Offset: 0x002040B4
		public void SetFlag(CullingFlags flag)
		{
			CullingFlags cullingFlags = this.m_cullingFlags;
			this.m_cullingFlags |= flag;
			if (!this.m_initialized || this.m_cullingFlags != cullingFlags)
			{
				this.RefreshCullee();
			}
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x00205EF0 File Offset: 0x002040F0
		public void UnsetFlag(CullingFlags flag)
		{
			CullingFlags cullingFlags = this.m_cullingFlags;
			this.m_cullingFlags &= ~flag;
			if (!this.m_initialized || this.m_cullingFlags != cullingFlags)
			{
				this.RefreshCullee();
			}
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshCullee()
		{
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x00082B3C File Offset: 0x00080D3C
		protected virtual CullingFlags GetUpdatedFlags(CullingFlags currentFlags)
		{
			if (this.m_cullingDistance.ShouldBeCulled(this.m_currentBand))
			{
				currentFlags |= CullingFlags.Distance;
			}
			else
			{
				currentFlags &= ~CullingFlags.Distance;
			}
			return currentFlags;
		}

		// Token: 0x170017A5 RID: 6053
		// (get) Token: 0x060062F9 RID: 25337 RVA: 0x00082A9B File Offset: 0x00080C9B
		// (set) Token: 0x060062FA RID: 25338 RVA: 0x00082B5F File Offset: 0x00080D5F
		int? ICullee.Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				this.m_index = value;
				if (this.m_index == null)
				{
					this.m_initialized = false;
				}
			}
		}

		// Token: 0x170017A6 RID: 6054
		// (get) Token: 0x060062FB RID: 25339 RVA: 0x00082B7C File Offset: 0x00080D7C
		float ICullee.Radius
		{
			get
			{
				return this.Radius;
			}
		}

		// Token: 0x170017A7 RID: 6055
		// (get) Token: 0x060062FC RID: 25340 RVA: 0x00082B84 File Offset: 0x00080D84
		// (set) Token: 0x060062FD RID: 25341 RVA: 0x00082B8C File Offset: 0x00080D8C
		float ICullee.SqrMagnitudeDistance
		{
			get
			{
				return this.m_sqrMagnitudeDistance;
			}
			set
			{
				this.m_sqrMagnitudeDistance = value;
			}
		}

		// Token: 0x170017A8 RID: 6056
		// (get) Token: 0x060062FE RID: 25342 RVA: 0x00052028 File Offset: 0x00050228
		GameObject ICullee.gameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x170017A9 RID: 6057
		// (get) Token: 0x060062FF RID: 25343 RVA: 0x00082981 File Offset: 0x00080B81
		CullingFlags ICullee.CullingFlags
		{
			get
			{
				return this.m_cullingFlags;
			}
		}

		// Token: 0x170017AA RID: 6058
		// (get) Token: 0x06006300 RID: 25344 RVA: 0x00082971 File Offset: 0x00080B71
		CullingFlags ICullee.LimitFlags
		{
			get
			{
				return this.m_limitFlags;
			}
		}

		// Token: 0x170017AB RID: 6059
		// (get) Token: 0x06006301 RID: 25345 RVA: 0x00082979 File Offset: 0x00080B79
		CullingDistance ICullee.CurrentDistance
		{
			get
			{
				return this.m_currentBand;
			}
		}

		// Token: 0x06006302 RID: 25346 RVA: 0x00082B95 File Offset: 0x00080D95
		void ICullee.OnCulleeBecameVisible()
		{
			this.OnCulleeBecameVisible();
		}

		// Token: 0x06006303 RID: 25347 RVA: 0x00082B9D File Offset: 0x00080D9D
		void ICullee.OnCulleeBecameInvisible()
		{
			this.OnCulleeBecameInvisible();
		}

		// Token: 0x06006304 RID: 25348 RVA: 0x00082BA5 File Offset: 0x00080DA5
		void ICullee.OnDistanceBandChanged(int previous, int current, bool force)
		{
			this.OnDistanceBandChanged((CullingDistance)previous, (CullingDistance)current, force);
		}

		// Token: 0x06006305 RID: 25349 RVA: 0x00082BB0 File Offset: 0x00080DB0
		void ICullee.SetFlag(CullingFlags flag)
		{
			this.SetFlag(flag);
		}

		// Token: 0x06006306 RID: 25350 RVA: 0x00082BB9 File Offset: 0x00080DB9
		void ICullee.UnsetFlag(CullingFlags flag)
		{
			this.UnsetFlag(flag);
		}

		// Token: 0x06006307 RID: 25351 RVA: 0x00082BC2 File Offset: 0x00080DC2
		int ICullee.GetHashCode()
		{
			return this.GetHashCode();
		}

		// Token: 0x170017AC RID: 6060
		// (get) Token: 0x06006308 RID: 25352 RVA: 0x00082BCA File Offset: 0x00080DCA
		DayNightEnableCondition IDayNightToggle.DayNightEnableCondition
		{
			get
			{
				return this.DayNightCondition;
			}
		}

		// Token: 0x06006309 RID: 25353 RVA: 0x00082BD2 File Offset: 0x00080DD2
		void IDayNightToggle.Toggle(bool isEnabled)
		{
			if (isEnabled)
			{
				this.UnsetFlag(CullingFlags.DayNight);
				return;
			}
			this.SetFlag(CullingFlags.DayNight);
		}

		// Token: 0x0400562B RID: 22059
		protected const string kCullingDistanceGroup = "Culling Distances";

		// Token: 0x0400562C RID: 22060
		[SerializeField]
		private bool m_useZoneDayNightCondition;

		// Token: 0x0400562D RID: 22061
		[SerializeField]
		private DayNightEnableCondition m_dayNightCondition;

		// Token: 0x0400562E RID: 22062
		[SerializeField]
		protected CullingDistance m_cullingDistance;

		// Token: 0x0400562F RID: 22063
		private bool m_initialized;

		// Token: 0x04005630 RID: 22064
		private int? m_index;

		// Token: 0x04005631 RID: 22065
		private float m_radius = 0.1f;

		// Token: 0x04005632 RID: 22066
		protected float m_sqrMagnitudeDistance = float.MaxValue;

		// Token: 0x04005633 RID: 22067
		protected CullingDistance m_currentBand = CullingDistance.NotCulled;

		// Token: 0x04005634 RID: 22068
		protected CullingFlags m_limitFlags;

		// Token: 0x04005635 RID: 22069
		protected CullingFlags m_cullingFlags;
	}
}
