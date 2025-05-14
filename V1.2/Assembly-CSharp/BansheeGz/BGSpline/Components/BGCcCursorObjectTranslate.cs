using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001B7 RID: 439
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCursorObjectTranslate")]
	[BGCc.CcDescriptor(Description = "Translate an object to the position, the cursor provides.", Name = "Translate Object By Cursor", Icon = "BGCcCursorObjectTranslate123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcTranslateObject")]
	[ExecuteInEditMode]
	public class BGCcCursorObjectTranslate : BGCcWithCursorObject
	{
		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000FA7 RID: 4007 RVA: 0x000DD724 File Offset: 0x000DB924
		// (remove) Token: 0x06000FA8 RID: 4008 RVA: 0x000DD75C File Offset: 0x000DB95C
		public event EventHandler ObjectTranslated;

		// Token: 0x06000FA9 RID: 4009 RVA: 0x000DD794 File Offset: 0x000DB994
		private void Update()
		{
			Transform objectToManipulate = base.ObjectToManipulate;
			if (objectToManipulate == null)
			{
				return;
			}
			int pointsCount = base.Curve.PointsCount;
			if (pointsCount == 0)
			{
				return;
			}
			if (pointsCount != 1)
			{
				objectToManipulate.position = base.Cursor.CalculatePosition();
				if (this.ObjectTranslated != null)
				{
					this.ObjectTranslated(this, null);
				}
				return;
			}
			objectToManipulate.position = base.Curve[0].PositionWorld;
		}
	}
}
