using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AE RID: 430
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCursor")]
	[BGCc.CcDescriptor(Description = "Identify location on the curve by distance.", Name = "Cursor", Icon = "BGCcCursor123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCursor")]
	public class BGCcCursor : BGCcWithMath
	{
		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000F37 RID: 3895 RVA: 0x0004CD7C File Offset: 0x0004AF7C
		// (set) Token: 0x06000F38 RID: 3896 RVA: 0x0004CD84 File Offset: 0x0004AF84
		public float Distance
		{
			get
			{
				return this.distance;
			}
			set
			{
				this.distance = base.Math.ClampDistance(value);
				base.FireChangedParams();
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000F39 RID: 3897 RVA: 0x000DC320 File Offset: 0x000DA520
		// (set) Token: 0x06000F3A RID: 3898 RVA: 0x0004CD9E File Offset: 0x0004AF9E
		public float DistanceRatio
		{
			get
			{
				float num = base.Math.GetDistance(-1);
				if (num != 0f)
				{
					return Mathf.Clamp01(this.distance / num);
				}
				return 0f;
			}
			set
			{
				this.distance = base.Math.GetDistance(-1) * Mathf.Clamp01(value);
				base.FireChangedParams();
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandles
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandlesSettings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000F3D RID: 3901 RVA: 0x0004CDBF File Offset: 0x0004AFBF
		// (set) Token: 0x06000F3E RID: 3902 RVA: 0x0004CDC7 File Offset: 0x0004AFC7
		public float HandlesScale
		{
			get
			{
				return this.handlesScale;
			}
			set
			{
				this.handlesScale = value;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000F3F RID: 3903 RVA: 0x0004CDD0 File Offset: 0x0004AFD0
		// (set) Token: 0x06000F40 RID: 3904 RVA: 0x0004CDD8 File Offset: 0x0004AFD8
		public Color HandlesColor
		{
			get
			{
				return this.handlesColor;
			}
			set
			{
				this.handlesColor = value;
			}
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0004CDE1 File Offset: 0x0004AFE1
		public Vector3 CalculateTangent()
		{
			return base.Math.CalcByDistance(BGCurveBaseMath.Field.Tangent, this.distance, false);
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0004CDF6 File Offset: 0x0004AFF6
		public Vector3 CalculatePosition()
		{
			return base.Math.CalcByDistance(BGCurveBaseMath.Field.Position, this.distance, false);
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0004CE0B File Offset: 0x0004B00B
		public int CalculateSectionIndex()
		{
			return base.Math.CalcSectionIndexByDistance(this.distance);
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x000DC358 File Offset: 0x000DA558
		public TR Lerp<T, TR>(string fieldName, Func<T, T, float, TR> lerpFunction)
		{
			if (base.Curve.PointsCount == 0)
			{
				return lerpFunction(default(T), default(T), 0f);
			}
			T arg;
			T arg2;
			float adjacentFieldValues = this.GetAdjacentFieldValues<T>(fieldName, out arg, out arg2);
			return lerpFunction(arg, arg2, adjacentFieldValues);
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x000DC3A8 File Offset: 0x000DA5A8
		public Quaternion LerpQuaternion(string fieldName, Func<Quaternion, Quaternion, float, Quaternion> customLerp = null)
		{
			if (base.Curve.PointsCount == 0)
			{
				return Quaternion.identity;
			}
			Quaternion identity;
			Quaternion identity2;
			float adjacentFieldValues = this.GetAdjacentFieldValues(fieldName, out identity, out identity2);
			if (identity.x == 0f && identity.y == 0f && identity.z == 0f && identity.w == 0f)
			{
				identity = Quaternion.identity;
			}
			if (identity2.x == 0f && identity2.y == 0f && identity2.z == 0f && identity2.w == 0f)
			{
				identity2 = Quaternion.identity;
			}
			Quaternion quaternion = (customLerp == null) ? Quaternion.Lerp(identity, identity2, adjacentFieldValues) : customLerp(identity, identity2, adjacentFieldValues);
			if (!float.IsNaN(quaternion.x) && !float.IsNaN(quaternion.y) && !float.IsNaN(quaternion.z) && !float.IsNaN(quaternion.w))
			{
				return quaternion;
			}
			return Quaternion.identity;
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x000DC49C File Offset: 0x000DA69C
		public Vector3 LerpVector(string fieldName, Func<Vector3, Vector3, float, Vector3> customLerp = null)
		{
			if (base.Curve.PointsCount == 0)
			{
				return Vector3.zero;
			}
			Vector3 vector;
			Vector3 vector2;
			float adjacentFieldValues = this.GetAdjacentFieldValues(fieldName, out vector, out vector2);
			if (customLerp != null)
			{
				return customLerp(vector, vector2, adjacentFieldValues);
			}
			return Vector3.Lerp(vector, vector2, adjacentFieldValues);
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x000DC4E0 File Offset: 0x000DA6E0
		public float LerpFloat(string fieldName, Func<float, float, float, float> customLerp = null)
		{
			if (base.Curve.PointsCount == 0)
			{
				return 0f;
			}
			float num;
			float num2;
			float adjacentFieldValues = this.GetAdjacentFieldValues(fieldName, out num, out num2);
			if (customLerp != null)
			{
				return customLerp(num, num2, adjacentFieldValues);
			}
			return Mathf.Lerp(num, num2, adjacentFieldValues);
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x000DC524 File Offset: 0x000DA724
		public Color LerpColor(string fieldName, Func<Color, Color, float, Color> customLerp = null)
		{
			if (base.Curve.PointsCount == 0)
			{
				return Color.clear;
			}
			Color color;
			Color color2;
			float adjacentFieldValues = this.GetAdjacentFieldValues(fieldName, out color, out color2);
			if (customLerp != null)
			{
				return customLerp(color, color2, adjacentFieldValues);
			}
			return Color.Lerp(color, color2, adjacentFieldValues);
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x000DC568 File Offset: 0x000DA768
		public float GetAdjacentFieldValues<T>(string fieldName, out T fromValue, out T toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetField<T>(fieldName);
			toValue = base.Curve[i2].GetField<T>(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x000DC5B0 File Offset: 0x000DA7B0
		public float GetAdjacentFieldValues(string fieldName, out float fromValue, out float toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetFloat(fieldName);
			toValue = base.Curve[i2].GetFloat(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x000DC5F0 File Offset: 0x000DA7F0
		public float GetAdjacentFieldValues(string fieldName, out int fromValue, out int toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetInt(fieldName);
			toValue = base.Curve[i2].GetInt(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x000DC630 File Offset: 0x000DA830
		public float GetAdjacentFieldValues(string fieldName, out bool fromValue, out bool toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetBool(fieldName);
			toValue = base.Curve[i2].GetBool(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x000DC670 File Offset: 0x000DA870
		public float GetAdjacentFieldValues(string fieldName, out Bounds fromValue, out Bounds toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetBounds(fieldName);
			toValue = base.Curve[i2].GetBounds(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x000DC6B8 File Offset: 0x000DA8B8
		public float GetAdjacentFieldValues(string fieldName, out Color fromValue, out Color toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetColor(fieldName);
			toValue = base.Curve[i2].GetColor(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x000DC700 File Offset: 0x000DA900
		public float GetAdjacentFieldValues(string fieldName, out Quaternion fromValue, out Quaternion toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetQuaternion(fieldName);
			toValue = base.Curve[i2].GetQuaternion(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x000DC748 File Offset: 0x000DA948
		public float GetAdjacentFieldValues(string fieldName, out Vector3 fromValue, out Vector3 toValue)
		{
			int i;
			int i2;
			float tforLerp = this.GetTForLerp(out i, out i2);
			fromValue = base.Curve[i].GetVector3(fieldName);
			toValue = base.Curve[i2].GetVector3(fieldName);
			return tforLerp;
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x000DC790 File Offset: 0x000DA990
		public float GetTForLerp(out int indexFrom, out int indexTo)
		{
			this.GetAdjacentPointIndexes(out indexFrom, out indexTo);
			BGCurveBaseMath.SectionInfo sectionInfo = base.Math[indexFrom];
			return (this.distance - sectionInfo.DistanceFromStartToOrigin) / sectionInfo.Distance;
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0004CE1E File Offset: 0x0004B01E
		public void GetAdjacentPointIndexes(out int indexFrom, out int indexTo)
		{
			indexFrom = this.CalculateSectionIndex();
			indexTo = ((indexFrom == base.Curve.PointsCount - 1) ? 0 : (indexFrom + 1));
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0004CE42 File Offset: 0x0004B042
		public override void Start()
		{
			this.Distance = this.distance;
		}

		// Token: 0x04000D2B RID: 3371
		[SerializeField]
		[Tooltip("Distance from start of the curve.")]
		protected float distance;

		// Token: 0x04000D2C RID: 3372
		[Range(0.5f, 1.5f)]
		[SerializeField]
		private float handlesScale = 1f;

		// Token: 0x04000D2D RID: 3373
		[SerializeField]
		private Color handlesColor = Color.white;
	}
}
