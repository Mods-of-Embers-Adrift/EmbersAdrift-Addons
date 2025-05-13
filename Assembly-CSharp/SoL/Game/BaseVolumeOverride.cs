using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000602 RID: 1538
	public abstract class BaseVolumeOverride : MonoBehaviour
	{
		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06003114 RID: 12564 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool DisableColliderOnStart
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void Register()
		{
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void Deregister()
		{
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x00061CBB File Offset: 0x0005FEBB
		protected virtual void Awake()
		{
			if (this.m_collider)
			{
				this.m_bounds = this.m_collider.bounds;
				this.m_collider.isTrigger = true;
			}
		}

		// Token: 0x06003118 RID: 12568 RVA: 0x00061CE7 File Offset: 0x0005FEE7
		private void Start()
		{
			if (this.m_collider)
			{
				if (this.DisableColliderOnStart)
				{
					this.m_collider.enabled = false;
				}
				this.Register();
			}
		}

		// Token: 0x06003119 RID: 12569 RVA: 0x00061D10 File Offset: 0x0005FF10
		private void OnDestroy()
		{
			if (this.m_collider)
			{
				this.Deregister();
			}
		}

		// Token: 0x0600311A RID: 12570 RVA: 0x00061D25 File Offset: 0x0005FF25
		public bool IsWithinBounds(Vector3 pos)
		{
			return this.m_collider && this.m_bounds.WithinBounds(pos);
		}

		// Token: 0x04002F56 RID: 12118
		[SerializeField]
		protected Collider m_collider;

		// Token: 0x04002F57 RID: 12119
		protected Bounds m_bounds;
	}
}
