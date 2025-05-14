using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CD0 RID: 3280
	[ExecuteInEditMode]
	[DefaultExecutionOrder(10)]
	public class CulledTreeImposter : MonoBehaviour
	{
		// Token: 0x170017B8 RID: 6072
		// (get) Token: 0x06006357 RID: 25431 RVA: 0x00082E6B File Offset: 0x0008106B
		// (set) Token: 0x06006358 RID: 25432 RVA: 0x00082E73 File Offset: 0x00081073
		internal int? Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				this.m_index = value;
			}
		}

		// Token: 0x170017B9 RID: 6073
		// (get) Token: 0x06006359 RID: 25433 RVA: 0x00082E7C File Offset: 0x0008107C
		internal Vector3 DistanceCenter
		{
			get
			{
				return base.gameObject.transform.position;
			}
		}

		// Token: 0x170017BA RID: 6074
		// (get) Token: 0x0600635A RID: 25434 RVA: 0x00082E8E File Offset: 0x0008108E
		internal float DistanceRadius
		{
			get
			{
				return 0.1f;
			}
		}

		// Token: 0x170017BB RID: 6075
		// (get) Token: 0x0600635B RID: 25435 RVA: 0x00082E95 File Offset: 0x00081095
		internal Vector3 FrustumCenter
		{
			get
			{
				return this.m_frustumCenter;
			}
		}

		// Token: 0x170017BC RID: 6076
		// (get) Token: 0x0600635C RID: 25436 RVA: 0x00082E9D File Offset: 0x0008109D
		internal float FrustumRadius
		{
			get
			{
				return this.m_frustumRadius;
			}
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x00206B50 File Offset: 0x00204D50
		private void Awake()
		{
			this.m_renderer = base.gameObject.GetComponent<MeshRenderer>();
			if (this.m_renderer)
			{
				this.m_frustumCenter = this.m_renderer.bounds.center;
				Vector3 size = this.m_renderer.localBounds.size;
				this.m_frustumRadius = Mathf.Max(new float[]
				{
					size.x,
					size.y,
					size.z
				}) * 1.1f;
				this.RefreshCullee();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x00082EA5 File Offset: 0x000810A5
		private void OnEnable()
		{
			if (!GameManager.IsServer && Application.isPlaying && TreeImposterCullingManager.Instance)
			{
				TreeImposterCullingManager.Instance.RegisterCulledObject(this);
			}
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x00082ECC File Offset: 0x000810CC
		private void OnDisable()
		{
			if (!GameManager.IsServer && Application.isPlaying && TreeImposterCullingManager.Instance)
			{
				TreeImposterCullingManager.Instance.DeregisterCulledObject(this);
			}
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x00082EF3 File Offset: 0x000810F3
		internal void DelayedInit(bool isVisible, int currentBand)
		{
			this.m_isVisible = isVisible;
			this.m_current = (TreeImposterCullingDistance)currentBand;
			this.RefreshCullee();
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x00082F09 File Offset: 0x00081109
		internal void OnDistanceBandChanged(int previous, int current, bool force)
		{
			this.m_current = (TreeImposterCullingDistance)current;
			this.RefreshCullee();
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x00082F18 File Offset: 0x00081118
		internal void OnCulleeBecameVisible()
		{
			this.m_isVisible = true;
			this.RefreshCullee();
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x00082F27 File Offset: 0x00081127
		internal void OnCulleeBecameInvisible()
		{
			this.m_isVisible = false;
			this.RefreshCullee();
		}

		// Token: 0x06006364 RID: 25444 RVA: 0x00082F36 File Offset: 0x00081136
		internal void RefreshCullee()
		{
			if (this.m_renderer)
			{
				this.m_renderer.enabled = (this.m_isVisible && this.m_current > TreeImposterCullingManager.CloseThreshold && this.m_current <= TreeImposterCullingDistance.FarCull);
			}
		}

		// Token: 0x0400566F RID: 22127
		private MeshRenderer m_renderer;

		// Token: 0x04005670 RID: 22128
		private bool m_isVisible;

		// Token: 0x04005671 RID: 22129
		private int? m_index;

		// Token: 0x04005672 RID: 22130
		private Vector3 m_frustumCenter;

		// Token: 0x04005673 RID: 22131
		private float m_frustumRadius = 1f;

		// Token: 0x04005674 RID: 22132
		private TreeImposterCullingDistance m_current = TreeImposterCullingDistance.Medium;
	}
}
