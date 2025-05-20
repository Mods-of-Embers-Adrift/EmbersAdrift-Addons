using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000195 RID: 405
	public interface BGCurveMathI
	{
		// Token: 0x06000DAA RID: 3498
		Vector3 CalcByDistanceRatio(BGCurveBaseMath.Field field, float distanceRatio, bool useLocal = false);

		// Token: 0x06000DAB RID: 3499
		Vector3 CalcByDistance(BGCurveBaseMath.Field field, float distance, bool useLocal = false);

		// Token: 0x06000DAC RID: 3500
		Vector3 CalcByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false);

		// Token: 0x06000DAD RID: 3501
		Vector3 CalcByDistance(float distance, out Vector3 tangent, bool useLocal = false);

		// Token: 0x06000DAE RID: 3502
		Vector3 CalcPositionAndTangentByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false);

		// Token: 0x06000DAF RID: 3503
		Vector3 CalcPositionAndTangentByDistance(float distance, out Vector3 tangent, bool useLocal = false);

		// Token: 0x06000DB0 RID: 3504
		Vector3 CalcPositionByDistanceRatio(float distanceRatio, bool useLocal = false);

		// Token: 0x06000DB1 RID: 3505
		Vector3 CalcPositionByDistance(float distance, bool useLocal = false);

		// Token: 0x06000DB2 RID: 3506
		Vector3 CalcTangentByDistanceRatio(float distanceRatio, bool useLocal = false);

		// Token: 0x06000DB3 RID: 3507
		Vector3 CalcTangentByDistance(float distance, bool useLocal = false);

		// Token: 0x06000DB4 RID: 3508
		int CalcSectionIndexByDistance(float distance);

		// Token: 0x06000DB5 RID: 3509
		int CalcSectionIndexByDistanceRatio(float ratio);

		// Token: 0x06000DB6 RID: 3510
		Vector3 CalcPositionByClosestPoint(Vector3 point, bool skipSectionsOptimization = false, bool skipPointsOptimization = false);

		// Token: 0x06000DB7 RID: 3511
		Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, bool skipSectionsOptimization = false, bool skipPointsOptimization = false);

		// Token: 0x06000DB8 RID: 3512
		Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, out Vector3 tangent, bool skipSectionsOptimization = false, bool skipPointsOptimization = false);

		// Token: 0x06000DB9 RID: 3513
		float GetDistance(int pointIndex = -1);

		// Token: 0x06000DBA RID: 3514
		void Recalculate(bool force = false);
	}
}
