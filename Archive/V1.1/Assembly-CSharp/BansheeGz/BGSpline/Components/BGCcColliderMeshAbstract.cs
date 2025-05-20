using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AD RID: 429
	public abstract class BGCcColliderMeshAbstract<T> : BGCcSplitterPolyline
	{
		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000F2B RID: 3883 RVA: 0x0004CD0B File Offset: 0x0004AF0B
		public override string Error
		{
			get
			{
				if (base.Curve.Mode2DOn)
				{
					return null;
				}
				return "Curve should be in 2D mode";
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x0004CD21 File Offset: 0x0004AF21
		// (set) Token: 0x06000F2D RID: 3885 RVA: 0x0004CD29 File Offset: 0x0004AF29
		public bool ShowIfNotSelected
		{
			get
			{
				return this.showIfNotSelected;
			}
			set
			{
				this.showIfNotSelected = value;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x0004CD32 File Offset: 0x0004AF32
		// (set) Token: 0x06000F2F RID: 3887 RVA: 0x0004CD3A File Offset: 0x0004AF3A
		public Color CollidersColor
		{
			get
			{
				return this.collidersColor;
			}
			set
			{
				this.collidersColor = value;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x0004CD43 File Offset: 0x0004AF43
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x0004CD4B File Offset: 0x0004AF4B
		public bool IsMeshGenerationOn
		{
			get
			{
				return this.isMeshGenerationOn;
			}
			set
			{
				base.ParamChanged<bool>(ref this.isMeshGenerationOn, value);
			}
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0004CD5B File Offset: 0x0004AF5B
		private void Reset()
		{
			this.UpdateRequested(this, new EventArgs());
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x000DC29C File Offset: 0x000DA49C
		protected override void UpdateRequested(object sender, EventArgs e)
		{
			base.UpdateRequested(sender, e);
			if (this.Error != null)
			{
				return;
			}
			T component = base.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			if (!this.UseLocal)
			{
				this.useLocal = true;
				this.dataValid = false;
			}
			List<Vector3> positions = base.Positions;
			this.Build(component, positions);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x000DC2F0 File Offset: 0x000DA4F0
		protected T Get<T>() where T : Component
		{
			T t = base.GetComponent<T>();
			if (t == null)
			{
				t = base.gameObject.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06000F35 RID: 3893
		protected abstract void Build(T collider, List<Vector3> positions);

		// Token: 0x04000D28 RID: 3368
		[SerializeField]
		[Tooltip("Show even if not selected")]
		private bool showIfNotSelected;

		// Token: 0x04000D29 RID: 3369
		[SerializeField]
		[Tooltip("Colliders color then showing not selected")]
		private Color collidersColor = Color.white;

		// Token: 0x04000D2A RID: 3370
		[SerializeField]
		[Tooltip("If mesh should be generated along with colliders")]
		private bool isMeshGenerationOn;
	}
}
