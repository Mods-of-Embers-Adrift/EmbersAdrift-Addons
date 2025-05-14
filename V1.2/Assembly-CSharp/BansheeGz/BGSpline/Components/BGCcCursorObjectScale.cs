using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001B6 RID: 438
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCursorObjectScale")]
	[BGCc.CcDescriptor(Description = "Scale the object, according to cursor position. Scale values are taken from curve's field values.", Name = "Scale Object By Cursor", Icon = "BGCcCursorObjectScale123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcScaleObject")]
	[ExecuteInEditMode]
	public class BGCcCursorObjectScale : BGCcWithCursorObject
	{
		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000F9F RID: 3999 RVA: 0x000DD5F0 File Offset: 0x000DB7F0
		// (remove) Token: 0x06000FA0 RID: 4000 RVA: 0x000DD628 File Offset: 0x000DB828
		public event EventHandler ObjectScaled;

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0004D1C7 File Offset: 0x0004B3C7
		// (set) Token: 0x06000FA2 RID: 4002 RVA: 0x0004D1CF File Offset: 0x0004B3CF
		public BGCurvePointField ScaleField
		{
			get
			{
				return this.scaleField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.scaleField, value);
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x0004D1DF File Offset: 0x0004B3DF
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (!(this.scaleField == null))
					{
						return null;
					}
					return "Scale field is not defined.";
				});
			}
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x000DD660 File Offset: 0x000DB860
		private void Update()
		{
			if (base.ObjectToManipulate == null || this.scaleField == null)
			{
				return;
			}
			int pointsCount = base.Curve.PointsCount;
			if (pointsCount == 0)
			{
				return;
			}
			if (pointsCount == 1)
			{
				base.ObjectToManipulate.localScale = base.Curve[0].GetVector3(this.scaleField.FieldName);
				return;
			}
			Vector3 vector = base.LerpVector(this.scaleField.FieldName, -1);
			if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
			{
				return;
			}
			base.ObjectToManipulate.localScale = vector;
			if (this.ObjectScaled != null)
			{
				this.ObjectScaled(this, null);
			}
		}

		// Token: 0x04000D5E RID: 3422
		[SerializeField]
		[Tooltip("Field to store the scale value at points. It should be a Vector3 field.")]
		private BGCurvePointField scaleField;
	}
}
