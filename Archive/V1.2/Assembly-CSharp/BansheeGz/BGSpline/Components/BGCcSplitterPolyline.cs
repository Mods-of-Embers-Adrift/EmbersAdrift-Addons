using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001BD RID: 445
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcSplitterPolyline")]
	[BGCc.CcDescriptor(Description = "Calculates points positions for polyline along the curve. It does not change or modify anything. Use Positions field to access points.", Name = "Splitter Polyline", Icon = "BGCcSplitterPolyline123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcSplitterPolyline")]
	public class BGCcSplitterPolyline : BGCcWithMath
	{
		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000FEF RID: 4079 RVA: 0x000DE060 File Offset: 0x000DC260
		// (remove) Token: 0x06000FF0 RID: 4080 RVA: 0x000DE098 File Offset: 0x000DC298
		public event EventHandler ChangedPositions;

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0004D5D0 File Offset: 0x0004B7D0
		// (set) Token: 0x06000FF2 RID: 4082 RVA: 0x0004D5D8 File Offset: 0x0004B7D8
		public BGCcSplitterPolyline.SplitModeEnum SplitMode
		{
			get
			{
				return this.splitMode;
			}
			set
			{
				base.ParamChanged<BGCcSplitterPolyline.SplitModeEnum>(ref this.splitMode, value);
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0004D5E8 File Offset: 0x0004B7E8
		// (set) Token: 0x06000FF4 RID: 4084 RVA: 0x0004D5FB File Offset: 0x0004B7FB
		public int PartsTotal
		{
			get
			{
				return Mathf.Clamp(this.partsTotal, 1, 1000);
			}
			set
			{
				base.ParamChanged<int>(ref this.partsTotal, Mathf.Clamp(value, 1, 1000));
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0004D616 File Offset: 0x0004B816
		// (set) Token: 0x06000FF6 RID: 4086 RVA: 0x0004D629 File Offset: 0x0004B829
		public int PartsPerSection
		{
			get
			{
				return Mathf.Clamp(this.partsPerSection, 1, 150);
			}
			set
			{
				base.ParamChanged<int>(ref this.partsPerSection, Mathf.Clamp(value, 1, 150));
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x0004D644 File Offset: 0x0004B844
		// (set) Token: 0x06000FF8 RID: 4088 RVA: 0x0004D64C File Offset: 0x0004B84C
		public bool DoNotOptimizeStraightLines
		{
			get
			{
				return this.doNotOptimizeStraightLines;
			}
			set
			{
				base.ParamChanged<bool>(ref this.doNotOptimizeStraightLines, value);
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x0004D65C File Offset: 0x0004B85C
		// (set) Token: 0x06000FFA RID: 4090 RVA: 0x0004D664 File Offset: 0x0004B864
		public virtual bool UseLocal
		{
			get
			{
				return this.useLocal;
			}
			set
			{
				base.ParamChanged<bool>(ref this.useLocal, value);
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000FFB RID: 4091 RVA: 0x000DE0D0 File Offset: 0x000DC2D0
		public override string Warning
		{
			get
			{
				BGCcMath math = base.Math;
				string result = "";
				if (math == null)
				{
					return result;
				}
				BGCcSplitterPolyline.SplitModeEnum splitModeEnum = this.SplitMode;
				if (splitModeEnum != BGCcSplitterPolyline.SplitModeEnum.PartsTotal)
				{
					if (splitModeEnum == BGCcSplitterPolyline.SplitModeEnum.PartsPerSection)
					{
						if (this.PartsPerSection > math.SectionParts)
						{
							result = "Math use less parts per section (" + math.SectionParts.ToString() + "). You need to increase Math's 'SectionParts' field accordingly to increase polyline precision.";
						}
					}
				}
				else
				{
					int num = math.Math.SectionsCount - (this.DoNotOptimizeStraightLines ? 0 : BGPolylineSplitter.CountStraightLines(base.Math.Math, null));
					int num2 = (num == 0) ? 0 : (this.PartsTotal / num);
					if (num2 > math.SectionParts)
					{
						result = string.Concat(new string[]
						{
							"Math use less parts per section (",
							math.SectionParts.ToString(),
							"). You now use ",
							num2.ToString(),
							" parts for curved section. You need to increase Math's 'SectionParts' field accordingly to increase polyline precision."
						});
					}
				}
				return result;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x000DE1C0 File Offset: 0x000DC3C0
		public override string Info
		{
			get
			{
				return "Polyline has " + this.PointsCount.ToString() + " points";
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000FFD RID: 4093 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandles
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandlesSettings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0004D674 File Offset: 0x0004B874
		public int PointsCount
		{
			get
			{
				if (!this.dataValid)
				{
					this.UpdateData();
				}
				if (this.positions != null)
				{
					return this.positions.Count;
				}
				return 0;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0004D699 File Offset: 0x0004B899
		public List<Vector3> Positions
		{
			get
			{
				if (!this.dataValid)
				{
					this.UpdateData();
				}
				return this.positions;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x0004D6AF File Offset: 0x0004B8AF
		public List<BGCcSplitterPolyline.PolylinePoint> Points
		{
			get
			{
				if (!this.dataValid)
				{
					this.UpdateData();
				}
				return this.points;
			}
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x0004D6C5 File Offset: 0x0004B8C5
		public override void Start()
		{
			this.AddListeners();
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0004D6CD File Offset: 0x0004B8CD
		public override void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x000DE1EC File Offset: 0x000DC3EC
		public void AddListeners()
		{
			base.Math.ChangedMath -= this.UpdateRequested;
			base.Math.ChangedMath += this.UpdateRequested;
			base.ChangedParams -= this.UpdateRequested;
			base.ChangedParams += this.UpdateRequested;
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0004D6D5 File Offset: 0x0004B8D5
		public void InvalidateData()
		{
			this.dataValid = false;
			if (this.ChangedPositions != null)
			{
				this.ChangedPositions(this, null);
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x000DE250 File Offset: 0x000DC450
		public void RemoveListeners()
		{
			try
			{
				base.Math.ChangedMath -= this.UpdateRequested;
				base.ChangedParams -= this.UpdateRequested;
			}
			catch (MissingReferenceException)
			{
			}
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x000DE2A0 File Offset: 0x000DC4A0
		private void UpdateData()
		{
			this.dataValid = true;
			Transform transform;
			try
			{
				transform = base.transform;
			}
			catch (MissingReferenceException)
			{
				this.RemoveListeners();
				return;
			}
			bool flag = true;
			try
			{
				flag = (base.Math == null || base.Math.Math == null || base.Math.Math.SectionsCount == 0);
			}
			catch (MissingReferenceException)
			{
			}
			this.positions.Clear();
			this.points.Clear();
			if (flag)
			{
				return;
			}
			if (this.splitter == null)
			{
				this.splitter = new BGPolylineSplitter();
			}
			if (this.config == null)
			{
				this.config = new BGPolylineSplitter.Config();
			}
			this.config.DoNotOptimizeStraightLines = this.doNotOptimizeStraightLines;
			this.config.SplitMode = this.splitMode;
			this.config.PartsTotal = this.partsTotal;
			this.config.PartsPerSection = this.partsPerSection;
			this.config.UseLocal = this.UseLocal;
			this.config.Transform = transform;
			this.splitter.Bind(this.positions, base.Math, this.config, this.points);
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0004D6F3 File Offset: 0x0004B8F3
		protected virtual void UpdateRequested(object sender, EventArgs e)
		{
			this.InvalidateData();
		}

		// Token: 0x04000D7A RID: 3450
		[SerializeField]
		[Tooltip("How to split the curve. TotalSections -total sections for whole curve;\r\n PartSections - each part (between 2 points) will use the same amount of splits;\r\nUseMathData -use data, precalculated by Math component. Note, you can tweak some params at Math as well.")]
		private BGCcSplitterPolyline.SplitModeEnum splitMode;

		// Token: 0x04000D7B RID: 3451
		[SerializeField]
		[Range(1f, 1000f)]
		[Tooltip("Total number of parts to split a curve to. The actual number of parts can be less than partsTotal due to optimization, but never more.")]
		private int partsTotal = 30;

		// Token: 0x04000D7C RID: 3452
		[SerializeField]
		[Range(1f, 150f)]
		[Tooltip("Every section of the curve will be split on even parts. The actual number of parts can be less than partsPerSection due to optimization, but never more.")]
		private int partsPerSection = 30;

		// Token: 0x04000D7D RID: 3453
		[SerializeField]
		[Tooltip("Split straight lines. Straight lines are optimized by default and are not split.")]
		private bool doNotOptimizeStraightLines;

		// Token: 0x04000D7E RID: 3454
		[SerializeField]
		[Tooltip("By default positions in world coordinates. Set this parameter to true to use local coordinates. Local coordinates are calculated slower.")]
		protected bool useLocal;

		// Token: 0x04000D7F RID: 3455
		protected readonly List<Vector3> positions = new List<Vector3>();

		// Token: 0x04000D80 RID: 3456
		protected readonly List<BGCcSplitterPolyline.PolylinePoint> points = new List<BGCcSplitterPolyline.PolylinePoint>();

		// Token: 0x04000D81 RID: 3457
		protected bool dataValid;

		// Token: 0x04000D82 RID: 3458
		private BGPolylineSplitter splitter;

		// Token: 0x04000D83 RID: 3459
		private BGPolylineSplitter.Config config;

		// Token: 0x020001BE RID: 446
		public enum SplitModeEnum
		{
			// Token: 0x04000D85 RID: 3461
			UseMathData,
			// Token: 0x04000D86 RID: 3462
			PartsTotal,
			// Token: 0x04000D87 RID: 3463
			PartsPerSection
		}

		// Token: 0x020001BF RID: 447
		public struct PolylinePoint
		{
			// Token: 0x0600100A RID: 4106 RVA: 0x0004D729 File Offset: 0x0004B929
			public PolylinePoint(Vector3 position, float distance)
			{
				this = default(BGCcSplitterPolyline.PolylinePoint);
				this.Position = position;
				this.Distance = distance;
			}

			// Token: 0x0600100B RID: 4107 RVA: 0x0004D740 File Offset: 0x0004B940
			public PolylinePoint(Vector3 position, float distance, Vector3 tangent)
			{
				this = new BGCcSplitterPolyline.PolylinePoint(position, distance);
				this.Tangent = tangent;
			}

			// Token: 0x0600100C RID: 4108 RVA: 0x000DE3E4 File Offset: 0x000DC5E4
			public override string ToString()
			{
				string str = "Pos=";
				Vector3 position = this.Position;
				return str + position.ToString() + "; Distance=" + this.Distance.ToString();
			}

			// Token: 0x04000D88 RID: 3464
			public Vector3 Position;

			// Token: 0x04000D89 RID: 3465
			public float Distance;

			// Token: 0x04000D8A RID: 3466
			public Vector3 Tangent;
		}
	}
}
