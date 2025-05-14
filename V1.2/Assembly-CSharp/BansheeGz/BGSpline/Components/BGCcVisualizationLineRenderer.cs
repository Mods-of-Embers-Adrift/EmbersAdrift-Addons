using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001C9 RID: 457
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcVisualizationLineRenderer")]
	[RequireComponent(typeof(LineRenderer))]
	[DisallowMultipleComponent]
	[BGCc.CcDescriptor(Description = "Visualize curve with standard LineRenderer Unity component.", Name = "Cc Line Renderer", Icon = "BGCcVisualizationLineRenderer123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcLineRenderer")]
	public class BGCcVisualizationLineRenderer : BGCcSplitterPolyline
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06001068 RID: 4200 RVA: 0x000DF41C File Offset: 0x000DD61C
		// (remove) Token: 0x06001069 RID: 4201 RVA: 0x000DF454 File Offset: 0x000DD654
		public event EventHandler ChangedVisualization;

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x0004DB6D File Offset: 0x0004BD6D
		// (set) Token: 0x0600106B RID: 4203 RVA: 0x0004DB75 File Offset: 0x0004BD75
		public bool UpdateAtStart
		{
			get
			{
				return this.updateAtStart;
			}
			set
			{
				this.updateAtStart = value;
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x0600106C RID: 4204 RVA: 0x0004DB7E File Offset: 0x0004BD7E
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (!(this.LineRenderer == null))
					{
						return null;
					}
					return "LineRenderer is null";
				});
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x0600106D RID: 4205 RVA: 0x000DF48C File Offset: 0x000DD68C
		public override string Warning
		{
			get
			{
				string text = base.Warning;
				LineRenderer lineRenderer = this.LineRenderer;
				if (lineRenderer == null)
				{
					return text;
				}
				if (!lineRenderer.useWorldSpace)
				{
					text += "\r\nLineRenderer uses local space (LineRenderer.useWorldSpace=false)! This is not optimal, especially if you plan to update a curve at runtime. Try to set LineRenderer.useWorldSpace to true";
				}
				if (text.Length != 0)
				{
					return text;
				}
				return null;
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x0600106E RID: 4206 RVA: 0x000DF4D4 File Offset: 0x000DD6D4
		public override string Info
		{
			get
			{
				if (!(this.lineRenderer != null))
				{
					return "LineRenderer is null";
				}
				return "LineRenderer uses " + base.PointsCount.ToString() + " points";
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x0600106F RID: 4207 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool SupportHandles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06001070 RID: 4208 RVA: 0x0004DB98 File Offset: 0x0004BD98
		public LineRenderer LineRenderer
		{
			get
			{
				if (this.lineRenderer == null)
				{
					this.lineRenderer = base.GetComponent<LineRenderer>();
				}
				return this.lineRenderer;
			}
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0004DBBA File Offset: 0x0004BDBA
		public override void Start()
		{
			base.Start();
			if (this.updateAtStart)
			{
				this.UpdateUI();
				return;
			}
			base.Math.EnsureMathIsCreated();
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x0004DBDC File Offset: 0x0004BDDC
		public override void AddedInEditor()
		{
			this.UpdateUI();
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x000DF514 File Offset: 0x000DD714
		public void UpdateUI()
		{
			try
			{
				if (base.Math == null)
				{
					return;
				}
			}
			catch (MissingReferenceException)
			{
				return;
			}
			BGCurveBaseMath math = base.Math.Math;
			if (math == null)
			{
				return;
			}
			LineRenderer lineRenderer;
			try
			{
				lineRenderer = this.LineRenderer;
			}
			catch (MissingReferenceException)
			{
				return;
			}
			if (lineRenderer == null)
			{
				return;
			}
			if (base.Curve == null)
			{
				return;
			}
			if (math.SectionsCount == 0)
			{
				lineRenderer.positionCount = 0;
				if (this.positions != null && this.positions.Count > 0 && this.ChangedVisualization != null)
				{
					this.ChangedVisualization(this, null);
				}
				this.positions.Clear();
				return;
			}
			this.useLocal = !lineRenderer.useWorldSpace;
			List<Vector3> positions = base.Positions;
			int count = positions.Count;
			if (count > 0)
			{
				lineRenderer.positionCount = count;
				for (int i = 0; i < count; i++)
				{
					lineRenderer.SetPosition(i, positions[i]);
				}
			}
			else
			{
				lineRenderer.positionCount = 0;
			}
			if (this.ChangedVisualization != null)
			{
				this.ChangedVisualization(this, null);
			}
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0004DBE4 File Offset: 0x0004BDE4
		protected override void UpdateRequested(object sender, EventArgs e)
		{
			base.UpdateRequested(sender, e);
			this.UpdateUI();
		}

		// Token: 0x04000DC8 RID: 3528
		[SerializeField]
		[Tooltip("Update LineRenderer at Start method.")]
		private bool updateAtStart;

		// Token: 0x04000DC9 RID: 3529
		private LineRenderer lineRenderer;
	}
}
