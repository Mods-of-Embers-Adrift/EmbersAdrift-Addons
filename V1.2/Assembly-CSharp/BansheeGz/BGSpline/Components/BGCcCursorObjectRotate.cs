using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001B3 RID: 435
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCursorObjectRotate")]
	[BGCc.CcDescriptor(Description = "Align the object's rotation with curve's tangent or 'rotation' field values at the point, the Cursor provides.", Name = "Rotate Object By Cursor", Icon = "BGCcCursorObjectRotate123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcRotateObject")]
	[ExecuteInEditMode]
	public class BGCcCursorObjectRotate : BGCcWithCursorObject
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000F7E RID: 3966 RVA: 0x000DD0D4 File Offset: 0x000DB2D4
		// (remove) Token: 0x06000F7F RID: 3967 RVA: 0x000DD10C File Offset: 0x000DB30C
		public event EventHandler ChangedObjectRotation;

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000F80 RID: 3968 RVA: 0x0004D069 File Offset: 0x0004B269
		// (set) Token: 0x06000F81 RID: 3969 RVA: 0x0004D071 File Offset: 0x0004B271
		public BGCcCursorObjectRotate.RotationInterpolationEnum RotationInterpolation
		{
			get
			{
				return this.rotationInterpolation;
			}
			set
			{
				base.ParamChanged<BGCcCursorObjectRotate.RotationInterpolationEnum>(ref this.rotationInterpolation, value);
			}
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x0004D081 File Offset: 0x0004B281
		// (set) Token: 0x06000F83 RID: 3971 RVA: 0x0004D089 File Offset: 0x0004B289
		public float LerpSpeed
		{
			get
			{
				return this.lerpSpeed;
			}
			set
			{
				base.ParamChanged<float>(ref this.lerpSpeed, value);
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0004D099 File Offset: 0x0004B299
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x0004D0A1 File Offset: 0x0004B2A1
		public float SlerpSpeed
		{
			get
			{
				return this.slerpSpeed;
			}
			set
			{
				base.ParamChanged<float>(ref this.slerpSpeed, value);
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0004D0B1 File Offset: 0x0004B2B1
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0004D0B9 File Offset: 0x0004B2B9
		public Vector3 UpCustom
		{
			get
			{
				return this.upCustom;
			}
			set
			{
				base.ParamChanged<Vector3>(ref this.upCustom, value);
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x0004D0C9 File Offset: 0x0004B2C9
		// (set) Token: 0x06000F89 RID: 3977 RVA: 0x0004D0D1 File Offset: 0x0004B2D1
		public BGCcCursorObjectRotate.RotationUpEnum UpMode
		{
			get
			{
				return this.upMode;
			}
			set
			{
				base.ParamChanged<BGCcCursorObjectRotate.RotationUpEnum>(ref this.upMode, value);
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x0004D0E1 File Offset: 0x0004B2E1
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0004D0E9 File Offset: 0x0004B2E9
		public BGCurvePointField RotationField
		{
			get
			{
				return this.rotationField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.rotationField, value);
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x0004D0F9 File Offset: 0x0004B2F9
		// (set) Token: 0x06000F8D RID: 3981 RVA: 0x0004D101 File Offset: 0x0004B301
		public BGCurvePointField RevolutionsAroundTangentField
		{
			get
			{
				return this.revolutionsAroundTangentField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.revolutionsAroundTangentField, value);
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x0004D111 File Offset: 0x0004B311
		// (set) Token: 0x06000F8F RID: 3983 RVA: 0x0004D119 File Offset: 0x0004B319
		public int RevolutionsAroundTangent
		{
			get
			{
				return this.revolutionsAroundTangent;
			}
			set
			{
				base.ParamChanged<int>(ref this.revolutionsAroundTangent, value);
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x0004D129 File Offset: 0x0004B329
		// (set) Token: 0x06000F91 RID: 3985 RVA: 0x0004D131 File Offset: 0x0004B331
		public BGCurvePointField RevolutionsClockwiseField
		{
			get
			{
				return this.revolutionsClockwiseField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.revolutionsClockwiseField, value);
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0004D141 File Offset: 0x0004B341
		// (set) Token: 0x06000F93 RID: 3987 RVA: 0x0004D149 File Offset: 0x0004B349
		public bool RevolutionsClockwise
		{
			get
			{
				return this.revolutionsClockwise;
			}
			set
			{
				base.ParamChanged<bool>(ref this.revolutionsClockwise, value);
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x0004D159 File Offset: 0x0004B359
		// (set) Token: 0x06000F95 RID: 3989 RVA: 0x0004D161 File Offset: 0x0004B361
		public Vector3 OffsetAngle
		{
			get
			{
				return this.offsetAngle;
			}
			set
			{
				base.ParamChanged<Vector3>(ref this.offsetAngle, value);
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x0004D171 File Offset: 0x0004B371
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (!base.Cursor.Math.IsCalculated(BGCurveBaseMath.Field.Tangent))
					{
						if (this.rotationField == null)
						{
							return "Math should calculate tangents if rotation field is null.";
						}
						if (this.RevolutionsAroundTangent != 0 || this.RevolutionsAroundTangentField != null)
						{
							return "Math should calculate tangents if revolutions are used.";
						}
					}
					return null;
				});
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000F97 RID: 3991 RVA: 0x000DD144 File Offset: 0x000DB344
		public override string Warning
		{
			get
			{
				if (!(this.rotationField == null) || (this.upMode != BGCcCursorObjectRotate.RotationUpEnum.TargetParentUp && this.upMode != BGCcCursorObjectRotate.RotationUpEnum.TargetParentUpCustom) || !(base.ObjectToManipulate != null) || !(base.ObjectToManipulate.parent == null))
				{
					return null;
				}
				return "Up Mode is set to " + this.upMode.ToString() + ", however object's parent is null";
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandles
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000F99 RID: 3993 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandlesSettings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0004D18B File Offset: 0x0004B38B
		public Quaternion Rotation
		{
			get
			{
				return this.rotation;
			}
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x000DD1B4 File Offset: 0x000DB3B4
		private void Update()
		{
			if (base.Curve.PointsCount == 0)
			{
				return;
			}
			Transform objectToManipulate = base.ObjectToManipulate;
			if (objectToManipulate == null)
			{
				return;
			}
			if (!this.TryToCalculateRotation(ref this.rotation))
			{
				return;
			}
			objectToManipulate.rotation = this.rotation;
			if (this.ChangedObjectRotation != null)
			{
				this.ChangedObjectRotation(this, null);
			}
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x000DD210 File Offset: 0x000DB410
		public bool TryToCalculateRotation(ref Quaternion result)
		{
			int pointsCount = base.Curve.PointsCount;
			if (pointsCount == 0)
			{
				return false;
			}
			BGCcCursor cursor = base.Cursor;
			BGCcMath math = cursor.Math;
			if (this.rotationField == null)
			{
				if (math == null || !math.IsCalculated(BGCurveBaseMath.Field.Tangent))
				{
					return false;
				}
				if (pointsCount == 1)
				{
					result = Quaternion.identity;
				}
				else
				{
					Vector3 vector = cursor.CalculateTangent();
					if ((double)Vector3.SqrMagnitude(vector) < 0.01)
					{
						return false;
					}
					Vector3 upwards;
					switch (this.upMode)
					{
					case BGCcCursorObjectRotate.RotationUpEnum.WorldUp:
						upwards = Vector3.up;
						break;
					case BGCcCursorObjectRotate.RotationUpEnum.WorldCustom:
						upwards = this.upCustom;
						break;
					case BGCcCursorObjectRotate.RotationUpEnum.LocalUp:
						upwards = base.transform.InverseTransformDirection(Vector3.up);
						break;
					case BGCcCursorObjectRotate.RotationUpEnum.LocalCustom:
						upwards = base.transform.InverseTransformDirection(this.upCustom);
						break;
					case BGCcCursorObjectRotate.RotationUpEnum.TargetParentUp:
					case BGCcCursorObjectRotate.RotationUpEnum.TargetParentUpCustom:
					{
						Transform objectToManipulate = base.ObjectToManipulate;
						if (objectToManipulate.parent != null)
						{
							upwards = objectToManipulate.parent.InverseTransformDirection((this.upMode == BGCcCursorObjectRotate.RotationUpEnum.TargetParentUp) ? Vector3.up : this.upCustom);
						}
						else
						{
							upwards = ((this.upMode == BGCcCursorObjectRotate.RotationUpEnum.TargetParentUp) ? Vector3.up : this.upCustom);
						}
						break;
					}
					default:
						throw new Exception("Unsupported upMode:" + this.upMode.ToString());
					}
					result = Quaternion.LookRotation(vector, upwards);
				}
			}
			else if (pointsCount == 1)
			{
				result = base.Curve[0].GetQuaternion(this.rotationField.FieldName);
			}
			else if (this.revolutionsAroundTangentField == null && this.revolutionsAroundTangent == 0)
			{
				result = base.LerpQuaternion(this.rotationField.FieldName, -1);
			}
			else
			{
				int num = (this.revolutionsAroundTangentField != null || this.revolutionsClockwiseField != null) ? cursor.CalculateSectionIndex() : -1;
				result = base.LerpQuaternion(this.rotationField.FieldName, num);
				int num2 = Mathf.Clamp((this.revolutionsAroundTangentField != null) ? base.Curve[num].GetInt(this.revolutionsAroundTangentField.FieldName) : this.revolutionsAroundTangent, 0, int.MaxValue);
				if (num2 > 0 && math.IsCalculated(BGCurveBaseMath.Field.Tangent))
				{
					Vector3 vector2 = cursor.CalculateTangent();
					if ((double)Vector3.SqrMagnitude(vector2) > 0.01)
					{
						int num3 = 360 * num2;
						if ((this.revolutionsClockwiseField != null) ? base.Curve[num].GetBool(this.revolutionsClockwiseField.FieldName) : this.revolutionsClockwise)
						{
							num3 = -num3;
						}
						int num4;
						int num5;
						float t = base.GetT(out num4, out num5, num);
						float angle = Mathf.Lerp(0f, (float)num3, t);
						result *= Quaternion.AngleAxis(angle, vector2);
					}
				}
			}
			result *= Quaternion.Euler(this.offsetAngle);
			BGCcCursorObjectRotate.RotationInterpolationEnum rotationInterpolationEnum = this.rotationInterpolation;
			if (rotationInterpolationEnum != BGCcCursorObjectRotate.RotationInterpolationEnum.Lerp)
			{
				if (rotationInterpolationEnum == BGCcCursorObjectRotate.RotationInterpolationEnum.Slerp)
				{
					result = Quaternion.Slerp(base.ObjectToManipulate.rotation, this.rotation, this.slerpSpeed * Time.deltaTime);
				}
			}
			else
			{
				result = Quaternion.Lerp(base.ObjectToManipulate.rotation, this.rotation, this.lerpSpeed * Time.deltaTime);
			}
			return true;
		}

		// Token: 0x04000D46 RID: 3398
		[SerializeField]
		[Tooltip("Rotation interpolation mode.")]
		private BGCcCursorObjectRotate.RotationInterpolationEnum rotationInterpolation;

		// Token: 0x04000D47 RID: 3399
		[SerializeField]
		[Tooltip("Rotation Lerp rotationSpeed. (Quaternion.Lerp(from,to, lerpSpeed * Time.deltaTime)) ")]
		private float lerpSpeed = 5f;

		// Token: 0x04000D48 RID: 3400
		[SerializeField]
		[Tooltip("Rotation Slerp rotationSpeed. (Quaternion.Slerp(from,to, slerpSpeed * Time.deltaTime)) ")]
		private float slerpSpeed = 5f;

		// Token: 0x04000D49 RID: 3401
		[SerializeField]
		[Tooltip("Angle to add to final result.")]
		private Vector3 offsetAngle;

		// Token: 0x04000D4A RID: 3402
		[SerializeField]
		[Tooltip("Up mode for tangent Quaternion.LookRotation. It's used only if rotationField is not assigned.\r\n1) WorldUp - use Vector.up in world coordinates\r\n2) WorldCustom - use custom Vector in world coordinates\r\n3) LocalUp - use Vector.up in local coordinates \r\n4) LocalCustom - use custom Vector in local coordinates\r\n5) TargetParentUp - use Vector.up in target object parent's local coordinates\r\n6) TargetParentUpCustom- use custom Vector in target object parent's local coordinates")]
		private BGCcCursorObjectRotate.RotationUpEnum upMode;

		// Token: 0x04000D4B RID: 3403
		[SerializeField]
		[Tooltip("Custom Up vector for tangent Quaternion.LookRotation. It's used only if rotationField is not assigned.")]
		private Vector3 upCustom = Vector3.up;

		// Token: 0x04000D4C RID: 3404
		[SerializeField]
		[Tooltip("Field to store the rotation between each point. It should be a Quaternion field.")]
		private BGCurvePointField rotationField;

		// Token: 0x04000D4D RID: 3405
		[SerializeField]
		[Tooltip("Additional 360 degree revolutions around tangent. It's used only if rotationField is assigned. It can be overriden with 'int' revolutionsAroundTangentField field.")]
		private int revolutionsAroundTangent;

		// Token: 0x04000D4E RID: 3406
		[SerializeField]
		[Tooltip("Field to store additional 360 degree revolutions around tangent for each point. It's used only if rotationField is assigned. It should be an int field.")]
		private BGCurvePointField revolutionsAroundTangentField;

		// Token: 0x04000D4F RID: 3407
		[SerializeField]
		[Tooltip("By default revolutions around tangent is counter-clockwise. Set it to true to reverse direction. It's used only if rotationField is assigned.It can be overriden with bool field")]
		private bool revolutionsClockwise;

		// Token: 0x04000D50 RID: 3408
		[SerializeField]
		[Tooltip("Field to store direction for revolutions around tangent. It should be an bool field.  It's used only if rotationField is assigned.")]
		private BGCurvePointField revolutionsClockwiseField;

		// Token: 0x04000D51 RID: 3409
		private Quaternion rotation = Quaternion.identity;

		// Token: 0x020001B4 RID: 436
		public enum RotationInterpolationEnum
		{
			// Token: 0x04000D53 RID: 3411
			None,
			// Token: 0x04000D54 RID: 3412
			Lerp,
			// Token: 0x04000D55 RID: 3413
			Slerp
		}

		// Token: 0x020001B5 RID: 437
		public enum RotationUpEnum
		{
			// Token: 0x04000D57 RID: 3415
			WorldUp,
			// Token: 0x04000D58 RID: 3416
			WorldCustom,
			// Token: 0x04000D59 RID: 3417
			LocalUp,
			// Token: 0x04000D5A RID: 3418
			LocalCustom,
			// Token: 0x04000D5B RID: 3419
			TargetParentUp,
			// Token: 0x04000D5C RID: 3420
			TargetParentUpCustom
		}
	}
}
