using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001C5 RID: 453
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcTrs")]
	[BGCc.CcDescriptor(Description = "Translate + rotate + scale an object with one single component. It's 5 components in one (Cursor+CursorChangeLinear+MoveByCursor+RotateByCursor+ScaleByCursor) with basic functionality", Name = "TRS", Icon = "BGCcTrs123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcTrs")]
	public class BGCcTrs : BGCcCursor
	{
		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x0004D979 File Offset: 0x0004BB79
		// (set) Token: 0x0600103F RID: 4159 RVA: 0x0004D981 File Offset: 0x0004BB81
		public bool SpeedIsReversed { get; set; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x000DEF00 File Offset: 0x000DD100
		public override string Error
		{
			get
			{
				if (this.objectToManipulate == null)
				{
					return "Object To Manipulate is not set.";
				}
				BGCcTrs.CursorChangeModeEnum cursorChangeModeEnum = this.cursorChangeMode;
				if (cursorChangeModeEnum - BGCcTrs.CursorChangeModeEnum.LinearField <= 1)
				{
					if (this.speedField == null)
					{
						return "Speed field is not set.";
					}
					if (this.speedField.Type != BGCurvePointField.TypeEnum.Float)
					{
						return "Speed field should have float type.";
					}
				}
				if (this.rotateObject)
				{
					BGCcMath math = base.Math;
					if (math == null || !math.IsCalculated(BGCurveBaseMath.Field.Tangent))
					{
						return "Math does not calculate tangents.";
					}
					if (this.rotationField != null && this.rotationField.Type != BGCurvePointField.TypeEnum.Quaternion)
					{
						return "Rotate field should have Quaternion type.";
					}
				}
				if (this.scaleObject)
				{
					if (this.scaleField == null)
					{
						return "Scale field is not set.";
					}
					if (this.scaleField.Type != BGCurvePointField.TypeEnum.Vector3)
					{
						return "Scale field should have Vector3 type.";
					}
				}
				return null;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x0004D98A File Offset: 0x0004BB8A
		// (set) Token: 0x06001042 RID: 4162 RVA: 0x0004D992 File Offset: 0x0004BB92
		public BGCcTrs.OverflowControlEnum OverflowControl
		{
			get
			{
				return this.overflowControl;
			}
			set
			{
				base.ParamChanged<BGCcTrs.OverflowControlEnum>(ref this.overflowControl, value);
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x0004D9A2 File Offset: 0x0004BBA2
		// (set) Token: 0x06001044 RID: 4164 RVA: 0x0004D9AA File Offset: 0x0004BBAA
		public BGCcTrs.CursorChangeModeEnum CursorChangeMode
		{
			get
			{
				return this.cursorChangeMode;
			}
			set
			{
				if (this.cursorChangeMode == value)
				{
					return;
				}
				this.cursorChangeMode = value;
				base.FireChangedParams();
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x0004D9C3 File Offset: 0x0004BBC3
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x0004D9CB File Offset: 0x0004BBCB
		public float Speed
		{
			get
			{
				return this.speed;
			}
			set
			{
				base.ParamChanged<float>(ref this.speed, value);
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x0004D9DB File Offset: 0x0004BBDB
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x0004D9E3 File Offset: 0x0004BBE3
		public BGCurvePointField SpeedField
		{
			get
			{
				return this.speedField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.speedField, value);
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x0004D9F3 File Offset: 0x0004BBF3
		// (set) Token: 0x0600104A RID: 4170 RVA: 0x0004D9FB File Offset: 0x0004BBFB
		public Transform ObjectToManipulate
		{
			get
			{
				return this.objectToManipulate;
			}
			set
			{
				base.ParamChanged<Transform>(ref this.objectToManipulate, value);
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x0004DA0B File Offset: 0x0004BC0B
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x0004DA13 File Offset: 0x0004BC13
		public bool UseFixedUpdate
		{
			get
			{
				return this.useFixedUpdate;
			}
			set
			{
				base.ParamChanged<bool>(ref this.useFixedUpdate, value);
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x0004DA23 File Offset: 0x0004BC23
		// (set) Token: 0x0600104E RID: 4174 RVA: 0x0004DA2B File Offset: 0x0004BC2B
		public bool MoveObject
		{
			get
			{
				return this.moveObject;
			}
			set
			{
				base.ParamChanged<bool>(ref this.moveObject, value);
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x0004DA3B File Offset: 0x0004BC3B
		// (set) Token: 0x06001050 RID: 4176 RVA: 0x0004DA43 File Offset: 0x0004BC43
		public bool RotateObject
		{
			get
			{
				return this.rotateObject;
			}
			set
			{
				base.ParamChanged<bool>(ref this.rotateObject, value);
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001051 RID: 4177 RVA: 0x0004DA53 File Offset: 0x0004BC53
		// (set) Token: 0x06001052 RID: 4178 RVA: 0x0004DA5B File Offset: 0x0004BC5B
		public bool ScaleObject
		{
			get
			{
				return this.scaleObject;
			}
			set
			{
				base.ParamChanged<bool>(ref this.scaleObject, value);
			}
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001053 RID: 4179 RVA: 0x0004DA6B File Offset: 0x0004BC6B
		// (set) Token: 0x06001054 RID: 4180 RVA: 0x0004DA73 File Offset: 0x0004BC73
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

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x0004DA83 File Offset: 0x0004BC83
		// (set) Token: 0x06001056 RID: 4182 RVA: 0x0004DA8B File Offset: 0x0004BC8B
		public BGCcTrs.RotationInterpolationEnum RotationInterpolation
		{
			get
			{
				return this.rotationInterpolation;
			}
			set
			{
				base.ParamChanged<BGCcTrs.RotationInterpolationEnum>(ref this.rotationInterpolation, value);
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001057 RID: 4183 RVA: 0x0004DA9B File Offset: 0x0004BC9B
		// (set) Token: 0x06001058 RID: 4184 RVA: 0x0004DAA3 File Offset: 0x0004BCA3
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

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001059 RID: 4185 RVA: 0x0004DAB3 File Offset: 0x0004BCB3
		// (set) Token: 0x0600105A RID: 4186 RVA: 0x0004DABB File Offset: 0x0004BCBB
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

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x0004DACB File Offset: 0x0004BCCB
		// (set) Token: 0x0600105C RID: 4188 RVA: 0x0004DAD3 File Offset: 0x0004BCD3
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

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x0600105D RID: 4189 RVA: 0x0004DAE3 File Offset: 0x0004BCE3
		// (set) Token: 0x0600105E RID: 4190 RVA: 0x0004DAEB File Offset: 0x0004BCEB
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

		// Token: 0x0600105F RID: 4191 RVA: 0x0004DAFB File Offset: 0x0004BCFB
		private void Update()
		{
			if (this.useFixedUpdate)
			{
				return;
			}
			this.Step();
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x0004DB0C File Offset: 0x0004BD0C
		private void FixedUpdate()
		{
			if (!this.useFixedUpdate)
			{
				return;
			}
			this.Step();
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x000DEFD4 File Offset: 0x000DD1D4
		private void Step()
		{
			if (this.cursorChangeMode == BGCcTrs.CursorChangeModeEnum.Constant && System.Math.Abs(this.speed) < 1E-05f)
			{
				return;
			}
			if (base.Curve.PointsCount < 2 || this.Error != null)
			{
				return;
			}
			int num = -1;
			float num2;
			switch (this.cursorChangeMode)
			{
			case BGCcTrs.CursorChangeModeEnum.Constant:
				num2 = this.speed * Time.deltaTime;
				break;
			case BGCcTrs.CursorChangeModeEnum.LinearField:
				num = base.CalculateSectionIndex();
				num2 = base.Curve[num].GetFloat(this.speedField.FieldName) * Time.deltaTime;
				break;
			case BGCcTrs.CursorChangeModeEnum.LinearFieldInterpolate:
			{
				BGCurvePointI bgcurvePointI;
				BGCurvePointI bgcurvePointI2;
				float t;
				this.FillInterpolationInfo(ref num, out bgcurvePointI, out bgcurvePointI2, out t);
				num2 = Mathf.Lerp(bgcurvePointI.GetFloat(this.speedField.FieldName), bgcurvePointI2.GetFloat(this.speedField.FieldName), t) * Time.deltaTime;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (this.SpeedIsReversed)
			{
				num2 = -num2;
			}
			this.distance += num2;
			if (this.distance < 0f)
			{
				switch (this.overflowControl)
				{
				case BGCcTrs.OverflowControlEnum.Cycle:
					this.distance = base.Math.GetDistance(-1);
					break;
				case BGCcTrs.OverflowControlEnum.PingPong:
					this.SpeedIsReversed = !this.SpeedIsReversed;
					this.distance = 0f;
					break;
				case BGCcTrs.OverflowControlEnum.Stop:
					this.Speed = 0f;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				float distance = base.Math.GetDistance(-1);
				if (this.distance > distance)
				{
					switch (this.overflowControl)
					{
					case BGCcTrs.OverflowControlEnum.Cycle:
						this.distance = 0f;
						break;
					case BGCcTrs.OverflowControlEnum.PingPong:
						this.SpeedIsReversed = !this.SpeedIsReversed;
						this.distance = distance;
						break;
					case BGCcTrs.OverflowControlEnum.Stop:
						this.Speed = 0f;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
			this.Trs(num);
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x000DF1B4 File Offset: 0x000DD3B4
		public void Trs(int sectionIndex = -1)
		{
			if (this.objectToManipulate == null)
			{
				return;
			}
			if (this.moveObject)
			{
				this.objectToManipulate.position = base.Math.CalcPositionByDistance(this.distance, false);
			}
			if (this.rotateObject)
			{
				Quaternion quaternion = (this.rotationField == null) ? Quaternion.LookRotation(base.CalculateTangent(), Vector3.up) : this.LerpQuaternion(ref sectionIndex, this.rotationField.FieldName);
				if (quaternion.x != 0f || quaternion.y != 0f || quaternion.z != 0f || quaternion.w != 0f)
				{
					quaternion *= Quaternion.Euler(this.offsetAngle);
					switch (this.rotationInterpolation)
					{
					case BGCcTrs.RotationInterpolationEnum.None:
						this.ObjectToManipulate.rotation = quaternion;
						break;
					case BGCcTrs.RotationInterpolationEnum.Lerp:
						this.ObjectToManipulate.rotation = Quaternion.Lerp(this.ObjectToManipulate.rotation, quaternion, this.lerpSpeed * Time.deltaTime);
						break;
					case BGCcTrs.RotationInterpolationEnum.Slerp:
						this.ObjectToManipulate.rotation = Quaternion.Slerp(this.ObjectToManipulate.rotation, quaternion, this.slerpSpeed * Time.deltaTime);
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
			if (this.scaleObject && this.scaleField != null)
			{
				this.ObjectToManipulate.localScale = this.LerpVector3(ref sectionIndex, this.scaleField.FieldName);
			}
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x000DF334 File Offset: 0x000DD534
		private Vector3 LerpVector3(ref int sectionIndex, string fieldName)
		{
			BGCurvePointI bgcurvePointI;
			BGCurvePointI bgcurvePointI2;
			float t;
			this.FillInterpolationInfo(ref sectionIndex, out bgcurvePointI, out bgcurvePointI2, out t);
			return Vector3.Lerp(bgcurvePointI.GetVector3(fieldName), bgcurvePointI2.GetVector3(fieldName), t);
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x000DF364 File Offset: 0x000DD564
		private Quaternion LerpQuaternion(ref int sectionIndex, string fieldName)
		{
			BGCurvePointI bgcurvePointI;
			BGCurvePointI bgcurvePointI2;
			float t;
			this.FillInterpolationInfo(ref sectionIndex, out bgcurvePointI, out bgcurvePointI2, out t);
			return Quaternion.Lerp(bgcurvePointI.GetQuaternion(fieldName), bgcurvePointI2.GetQuaternion(fieldName), t);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x000DF394 File Offset: 0x000DD594
		private void FillInterpolationInfo(ref int sectionIndex, out BGCurvePointI fromPoint, out BGCurvePointI toPoint, out float ratio)
		{
			if (sectionIndex == -1)
			{
				sectionIndex = base.CalculateSectionIndex();
			}
			fromPoint = base.Curve[sectionIndex];
			toPoint = ((sectionIndex == base.Curve.PointsCount - 1) ? base.Curve[0] : base.Curve[sectionIndex + 1]);
			BGCurveBaseMath.SectionInfo sectionInfo = base.Math[sectionIndex];
			ratio = (base.Distance - sectionInfo.DistanceFromStartToOrigin) / (sectionInfo.DistanceFromEndToOrigin - sectionInfo.DistanceFromStartToOrigin);
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0004DB1D File Offset: 0x0004BD1D
		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				return;
			}
			if (this.objectToManipulate == null)
			{
				return;
			}
			this.Trs(-1);
		}

		// Token: 0x04000DAB RID: 3499
		[SerializeField]
		[Tooltip("Object to manipulate.\r\n")]
		private Transform objectToManipulate;

		// Token: 0x04000DAC RID: 3500
		[SerializeField]
		[Tooltip("Modes for changing cursor position.\n1)Constant- speed value is constant.\n2)LinearField- each point has its own speed value.\n3)LinearFieldInterpolate- each point has its own speed value and the final speed is linear interpolation based on the distance between 2 points values")]
		private BGCcTrs.CursorChangeModeEnum cursorChangeMode;

		// Token: 0x04000DAD RID: 3501
		[SerializeField]
		[Tooltip("Constant movement speed along the curve (Speed * Time.deltaTime). You can override this value for each point with speedField")]
		private float speed = 5f;

		// Token: 0x04000DAE RID: 3502
		[SerializeField]
		[Tooltip("Field to store the speed between each point. It should be a float field.")]
		private BGCurvePointField speedField;

		// Token: 0x04000DAF RID: 3503
		[SerializeField]
		[Tooltip("Cursor will be moved in FixedUpdate instead of Update")]
		private bool useFixedUpdate;

		// Token: 0x04000DB0 RID: 3504
		[SerializeField]
		[Tooltip("How to change speed, when curve reaches the end.")]
		private BGCcTrs.OverflowControlEnum overflowControl;

		// Token: 0x04000DB1 RID: 3505
		[SerializeField]
		[Tooltip("Object should be translated.\r\n")]
		private bool moveObject = true;

		// Token: 0x04000DB2 RID: 3506
		[SerializeField]
		[Tooltip("Object should be rotated.\r\n")]
		private bool rotateObject;

		// Token: 0x04000DB3 RID: 3507
		[SerializeField]
		[Tooltip("Rotation interpolation mode.\r\n")]
		private BGCcTrs.RotationInterpolationEnum rotationInterpolation;

		// Token: 0x04000DB4 RID: 3508
		[SerializeField]
		[Tooltip("Rotation Lerp rotationSpeed. (Quaternion.Lerp(from,to, lerpSpeed * Time.deltaTime)) ")]
		private float lerpSpeed = 5f;

		// Token: 0x04000DB5 RID: 3509
		[SerializeField]
		[Tooltip("Rotation Slerp rotationSpeed. (Quaternion.Slerp(from,to, slerpSpeed * Time.deltaTime)) ")]
		private float slerpSpeed = 5f;

		// Token: 0x04000DB6 RID: 3510
		[SerializeField]
		[Tooltip("Angle to add to final result.")]
		private Vector3 offsetAngle;

		// Token: 0x04000DB7 RID: 3511
		[SerializeField]
		[Tooltip("Field to store the rotation between each point. It should be a Quaternion field.")]
		private BGCurvePointField rotationField;

		// Token: 0x04000DB8 RID: 3512
		[SerializeField]
		[Tooltip("Object should be scaled.\r\n")]
		private bool scaleObject;

		// Token: 0x04000DB9 RID: 3513
		[SerializeField]
		[Tooltip("Field to store the scale value at points. It should be a Vector3 field.")]
		private BGCurvePointField scaleField;

		// Token: 0x020001C6 RID: 454
		public enum OverflowControlEnum
		{
			// Token: 0x04000DBC RID: 3516
			Cycle,
			// Token: 0x04000DBD RID: 3517
			PingPong,
			// Token: 0x04000DBE RID: 3518
			Stop
		}

		// Token: 0x020001C7 RID: 455
		public enum CursorChangeModeEnum
		{
			// Token: 0x04000DC0 RID: 3520
			Constant,
			// Token: 0x04000DC1 RID: 3521
			LinearField,
			// Token: 0x04000DC2 RID: 3522
			LinearFieldInterpolate
		}

		// Token: 0x020001C8 RID: 456
		public enum RotationInterpolationEnum
		{
			// Token: 0x04000DC4 RID: 3524
			None,
			// Token: 0x04000DC5 RID: 3525
			Lerp,
			// Token: 0x04000DC6 RID: 3526
			Slerp
		}
	}
}
