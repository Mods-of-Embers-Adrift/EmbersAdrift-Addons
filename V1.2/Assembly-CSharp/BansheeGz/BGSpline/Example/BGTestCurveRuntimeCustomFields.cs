using System;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001E6 RID: 486
	public class BGTestCurveRuntimeCustomFields : MonoBehaviour
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x000E28F0 File Offset: 0x000E0AF0
		private void Start()
		{
			base.gameObject.AddComponent<BGCcCursorObjectTranslate>().ObjectToManipulate = this.ObjectToMove;
			BGCcCursorChangeLinear bgccCursorChangeLinear = base.gameObject.AddComponent<BGCcCursorChangeLinear>();
			base.gameObject.AddComponent<BGCcVisualizationLineRenderer>();
			LineRenderer component = base.gameObject.GetComponent<LineRenderer>();
			component.sharedMaterial = this.LineRendererMaterial;
			component.startWidth = (component.endWidth = 0.02f);
			BGCurve curve = bgccCursorChangeLinear.Curve;
			curve.Closed = true;
			curve.Mode2D = BGCurve.Mode2DEnum.XY;
			curve.PointsMode = BGCurve.PointsModeEnum.GameObjectsTransform;
			curve.AddPoint(new BGCurvePoint(curve, new Vector2(-5f, 0f), false));
			curve.AddPoint(new BGCurvePoint(curve, new Vector2(0f, 5f), BGCurvePoint.ControlTypeEnum.BezierSymmetrical, new Vector2(-5f, 0f), new Vector2(5f, 0f), false));
			curve.AddPoint(new BGCurvePoint(curve, new Vector2(5f, 0f), false));
			bgccCursorChangeLinear.SpeedField = BGTestCurveRuntimeCustomFields.NewFloatField(bgccCursorChangeLinear, "speed", new float[]
			{
				5f,
				10f,
				15f
			});
			bgccCursorChangeLinear.DelayField = BGTestCurveRuntimeCustomFields.NewFloatField(bgccCursorChangeLinear, "delay", new float[]
			{
				3f,
				1f,
				2f
			});
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x000E2A44 File Offset: 0x000E0C44
		private static BGCurvePointField NewFloatField(BGCcCursorChangeLinear changeCursor, string fieldName, params float[] values)
		{
			BGCurve curve = changeCursor.Curve;
			BGCurvePointField result = curve.AddField(fieldName, BGCurvePointField.TypeEnum.Float);
			for (int i = 0; i < values.Length; i++)
			{
				curve[i].SetFloat(fieldName, values[i]);
			}
			return result;
		}

		// Token: 0x04000E5F RID: 3679
		private const string SpeedFieldName = "speed";

		// Token: 0x04000E60 RID: 3680
		private const string DelayFieldName = "delay";

		// Token: 0x04000E61 RID: 3681
		private const float Width = 0.02f;

		// Token: 0x04000E62 RID: 3682
		public Transform ObjectToMove;

		// Token: 0x04000E63 RID: 3683
		public Material LineRendererMaterial;
	}
}
