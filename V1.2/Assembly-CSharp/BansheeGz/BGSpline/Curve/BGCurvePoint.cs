using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000196 RID: 406
	[Serializable]
	public class BGCurvePoint : BGCurvePointI
	{
		// Token: 0x06000DBB RID: 3515 RVA: 0x0004BD99 File Offset: 0x00049F99
		public BGCurvePoint(BGCurve curve, Vector3 position, bool useWorldCoordinates = false) : this(curve, position, BGCurvePoint.ControlTypeEnum.Absent, useWorldCoordinates)
		{
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x0004BDA5 File Offset: 0x00049FA5
		public BGCurvePoint(BGCurve curve, Vector3 position, BGCurvePoint.ControlTypeEnum controlType, bool useWorldCoordinates = false) : this(curve, position, controlType, Vector3.zero, Vector3.zero, useWorldCoordinates)
		{
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x0004BDBC File Offset: 0x00049FBC
		public BGCurvePoint(BGCurve curve, Vector3 position, BGCurvePoint.ControlTypeEnum controlType, Vector3 controlFirst, Vector3 controlSecond, bool useWorldCoordinates = false) : this(curve, null, position, controlType, controlFirst, controlSecond, useWorldCoordinates)
		{
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x000D8BB0 File Offset: 0x000D6DB0
		public BGCurvePoint(BGCurve curve, Transform pointTransform, Vector3 position, BGCurvePoint.ControlTypeEnum controlType, Vector3 controlFirst, Vector3 controlSecond, bool useWorldCoordinates = false)
		{
			this.curve = curve;
			this.controlType = controlType;
			this.pointTransform = pointTransform;
			if (useWorldCoordinates)
			{
				this.positionLocal = curve.transform.InverseTransformPoint(position);
				this.controlFirstLocal = curve.transform.InverseTransformDirection(controlFirst - position);
				this.controlSecondLocal = curve.transform.InverseTransformDirection(controlSecond - position);
				return;
			}
			this.positionLocal = position;
			this.controlFirstLocal = controlFirst;
			this.controlSecondLocal = controlSecond;
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000DBF RID: 3519 RVA: 0x0004BDCE File Offset: 0x00049FCE
		public BGCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000DC0 RID: 3520 RVA: 0x0004BDD6 File Offset: 0x00049FD6
		// (set) Token: 0x06000DC1 RID: 3521 RVA: 0x0004BE11 File Offset: 0x0004A011
		public BGCurvePoint.FieldsValues PrivateValuesForFields
		{
			get
			{
				if (this.fieldsValues == null || this.fieldsValues.Length < 1 || this.fieldsValues[0] == null)
				{
					this.fieldsValues = new BGCurvePoint.FieldsValues[]
					{
						new BGCurvePoint.FieldsValues()
					};
				}
				return this.fieldsValues[0];
			}
			set
			{
				if (this.fieldsValues == null || this.fieldsValues.Length < 1 || this.fieldsValues[0] == null)
				{
					this.fieldsValues = new BGCurvePoint.FieldsValues[]
					{
						new BGCurvePoint.FieldsValues()
					};
				}
				this.fieldsValues[0] = value;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x0004BE4D File Offset: 0x0004A04D
		// (set) Token: 0x06000DC3 RID: 3523 RVA: 0x0004BE7F File Offset: 0x0004A07F
		public Vector3 PositionLocal
		{
			get
			{
				if (!(this.pointTransform == null))
				{
					return this.curve.transform.InverseTransformPoint(this.pointTransform.position);
				}
				return this.positionLocal;
			}
			set
			{
				this.SetPosition(value, false);
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x000D8C3C File Offset: 0x000D6E3C
		// (set) Token: 0x06000DC5 RID: 3525 RVA: 0x0004BE89 File Offset: 0x0004A089
		public Vector3 PositionLocalTransformed
		{
			get
			{
				if (!(this.pointTransform == null))
				{
					return this.pointTransform.position - this.curve.transform.position;
				}
				return this.curve.transform.TransformPoint(this.positionLocal) - this.curve.transform.position;
			}
			set
			{
				this.SetPosition(value + this.curve.transform.position, true);
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x0004BEA8 File Offset: 0x0004A0A8
		// (set) Token: 0x06000DC7 RID: 3527 RVA: 0x0004BEDA File Offset: 0x0004A0DA
		public Vector3 PositionWorld
		{
			get
			{
				if (!(this.pointTransform == null))
				{
					return this.pointTransform.position;
				}
				return this.curve.transform.TransformPoint(this.positionLocal);
			}
			set
			{
				this.SetPosition(value, true);
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x0004BEE4 File Offset: 0x0004A0E4
		// (set) Token: 0x06000DC9 RID: 3529 RVA: 0x0004BEEC File Offset: 0x0004A0EC
		public Vector3 ControlFirstLocal
		{
			get
			{
				return this.controlFirstLocal;
			}
			set
			{
				this.SetControlFirstLocal(value);
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x0004BEF5 File Offset: 0x0004A0F5
		// (set) Token: 0x06000DCB RID: 3531 RVA: 0x000D8CA4 File Offset: 0x000D6EA4
		public Vector3 ControlFirstLocalTransformed
		{
			get
			{
				return ((this.pointTransform == null) ? this.curve.transform : this.pointTransform).TransformVector(this.controlFirstLocal);
			}
			set
			{
				Transform transform = (this.pointTransform == null) ? this.curve.transform : this.pointTransform;
				this.SetControlFirstLocal(transform.InverseTransformVector(value));
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000DCC RID: 3532 RVA: 0x000D8CE0 File Offset: 0x000D6EE0
		// (set) Token: 0x06000DCD RID: 3533 RVA: 0x000D8D78 File Offset: 0x000D6F78
		public Vector3 ControlFirstWorld
		{
			get
			{
				if (this.pointTransform == null)
				{
					return this.curve.transform.TransformPoint(new Vector3(this.positionLocal.x + this.controlFirstLocal.x, this.positionLocal.y + this.controlFirstLocal.y, this.positionLocal.z + this.controlFirstLocal.z));
				}
				return this.pointTransform.position + this.pointTransform.TransformVector(this.controlFirstLocal);
			}
			set
			{
				Vector3 vector = (this.pointTransform == null) ? (this.curve.transform.InverseTransformPoint(value) - this.positionLocal) : this.pointTransform.InverseTransformVector(value - this.pointTransform.position);
				this.SetControlFirstLocal(vector);
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0004BF23 File Offset: 0x0004A123
		// (set) Token: 0x06000DCF RID: 3535 RVA: 0x0004BF2B File Offset: 0x0004A12B
		public Vector3 ControlSecondLocal
		{
			get
			{
				return this.controlSecondLocal;
			}
			set
			{
				this.SetControlSecondLocal(value);
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x0004BF34 File Offset: 0x0004A134
		// (set) Token: 0x06000DD1 RID: 3537 RVA: 0x000D8DD8 File Offset: 0x000D6FD8
		public Vector3 ControlSecondLocalTransformed
		{
			get
			{
				return ((this.pointTransform == null) ? this.curve.transform : this.pointTransform).TransformVector(this.controlSecondLocal);
			}
			set
			{
				Transform transform = (this.pointTransform == null) ? this.curve.transform : this.pointTransform;
				this.SetControlSecondLocal(transform.InverseTransformVector(value));
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x000D8E14 File Offset: 0x000D7014
		// (set) Token: 0x06000DD3 RID: 3539 RVA: 0x000D8EAC File Offset: 0x000D70AC
		public Vector3 ControlSecondWorld
		{
			get
			{
				if (this.pointTransform == null)
				{
					return this.curve.transform.TransformPoint(new Vector3(this.positionLocal.x + this.controlSecondLocal.x, this.positionLocal.y + this.controlSecondLocal.y, this.positionLocal.z + this.controlSecondLocal.z));
				}
				return this.pointTransform.position + this.pointTransform.TransformVector(this.controlSecondLocal);
			}
			set
			{
				Vector3 vector = (this.pointTransform == null) ? (this.curve.transform.InverseTransformPoint(value) - this.positionLocal) : this.pointTransform.InverseTransformVector(value - this.pointTransform.position);
				this.SetControlSecondLocal(vector);
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x0004BF62 File Offset: 0x0004A162
		// (set) Token: 0x06000DD5 RID: 3541 RVA: 0x000D8F0C File Offset: 0x000D710C
		public BGCurvePoint.ControlTypeEnum ControlType
		{
			get
			{
				return this.controlType;
			}
			set
			{
				if (this.controlType == value)
				{
					return;
				}
				this.curve.FireBeforeChange("point control type is changed");
				this.controlType = value;
				if (this.controlType == BGCurvePoint.ControlTypeEnum.BezierSymmetrical)
				{
					this.controlSecondLocal = -this.controlFirstLocal;
				}
				this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point control type is changed") : null, false, this);
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x0004BF6A File Offset: 0x0004A16A
		// (set) Token: 0x06000DD7 RID: 3543 RVA: 0x000D8F84 File Offset: 0x000D7184
		public Transform PointTransform
		{
			get
			{
				return this.pointTransform;
			}
			set
			{
				if (this.pointTransform == value)
				{
					return;
				}
				this.curve.FireBeforeChange("point transform is changed");
				bool flag = this.pointTransform == null && value != null;
				bool flag2 = value == null && this.pointTransform != null;
				Vector3 controlFirstLocalTransformed = this.ControlFirstLocalTransformed;
				Vector3 controlSecondLocalTransformed = this.ControlSecondLocalTransformed;
				Vector3 positionWorld = this.PositionWorld;
				this.pointTransform = value;
				if (this.pointTransform != null)
				{
					this.pointTransform.position = positionWorld;
					this.controlFirstLocal = this.pointTransform.InverseTransformVector(controlFirstLocalTransformed);
					this.controlSecondLocal = this.pointTransform.InverseTransformVector(controlSecondLocalTransformed);
				}
				else
				{
					this.positionLocal = this.curve.transform.InverseTransformPoint(positionWorld);
					this.controlFirstLocal = this.curve.transform.InverseTransformVector(controlFirstLocalTransformed);
					this.controlSecondLocal = this.curve.transform.InverseTransformVector(controlSecondLocalTransformed);
				}
				if (flag)
				{
					this.curve.PrivateTransformForPointAdded(this.curve.IndexOf(this));
				}
				else if (flag2)
				{
					this.curve.PrivateTransformForPointRemoved(this.curve.IndexOf(this));
				}
				this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point transform is changed") : null, false, this);
			}
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x000D90E8 File Offset: 0x000D72E8
		public T GetField<T>(string name)
		{
			Type typeFromHandle = typeof(T);
			return (T)((object)this.GetField(name, typeFromHandle));
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x0004BF72 File Offset: 0x0004A172
		public float GetFloat(string name)
		{
			return this.PrivateValuesForFields.floatValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x0004BF8C File Offset: 0x0004A18C
		public bool GetBool(string name)
		{
			return this.PrivateValuesForFields.boolValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x0004BFA6 File Offset: 0x0004A1A6
		public int GetInt(string name)
		{
			return this.PrivateValuesForFields.intValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x0004BFC0 File Offset: 0x0004A1C0
		public Vector3 GetVector3(string name)
		{
			return this.PrivateValuesForFields.vector3Values[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDD RID: 3549 RVA: 0x0004BFDE File Offset: 0x0004A1DE
		public Quaternion GetQuaternion(string name)
		{
			return this.PrivateValuesForFields.quaternionValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x0004BFFC File Offset: 0x0004A1FC
		public Bounds GetBounds(string name)
		{
			return this.PrivateValuesForFields.boundsValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x0004C01A File Offset: 0x0004A21A
		public Color GetColor(string name)
		{
			return this.PrivateValuesForFields.colorValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x0004C038 File Offset: 0x0004A238
		public object GetField(string name, Type type)
		{
			return BGCurvePoint.FieldTypes.GetField(this.curve, type, name, this.PrivateValuesForFields);
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x0004C04D File Offset: 0x0004A24D
		public void SetField<T>(string name, T value)
		{
			this.SetField(name, value, typeof(T));
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x000D9110 File Offset: 0x000D7310
		public void SetField(string name, object value, Type type)
		{
			this.curve.FireBeforeChange("point field value is changed");
			BGCurvePoint.FieldTypes.SetField(this.curve, type, name, value, this.PrivateValuesForFields);
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x000D9170 File Offset: 0x000D7370
		public void SetFloat(string name, float value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.floatValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE4 RID: 3556 RVA: 0x000D91D4 File Offset: 0x000D73D4
		public void SetBool(string name, bool value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.boolValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x000D9238 File Offset: 0x000D7438
		public void SetInt(string name, int value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.intValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x000D929C File Offset: 0x000D749C
		public void SetVector3(string name, Vector3 value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.vector3Values[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x000D9304 File Offset: 0x000D7504
		public void SetQuaternion(string name, Quaternion value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.quaternionValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE8 RID: 3560 RVA: 0x000D936C File Offset: 0x000D756C
		public void SetBounds(string name, Bounds value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.boundsValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x000D93D4 File Offset: 0x000D75D4
		public void SetColor(string name, Color value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.colorValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x000D943C File Offset: 0x000D763C
		public Vector3 Get(BGCurvePoint.FieldEnum field)
		{
			Vector3 result;
			switch (field)
			{
			case BGCurvePoint.FieldEnum.PositionWorld:
				result = this.PositionWorld;
				break;
			case BGCurvePoint.FieldEnum.PositionLocal:
				result = this.positionLocal;
				break;
			case BGCurvePoint.FieldEnum.ControlFirstWorld:
				result = this.ControlFirstWorld;
				break;
			case BGCurvePoint.FieldEnum.ControlFirstLocal:
				result = this.controlFirstLocal;
				break;
			case BGCurvePoint.FieldEnum.ControlSecondWorld:
				result = this.ControlSecondWorld;
				break;
			default:
				result = this.controlSecondLocal;
				break;
			}
			return result;
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x000D949C File Offset: 0x000D769C
		public override string ToString()
		{
			string str = "Point [localPosition=";
			Vector3 vector = this.positionLocal;
			return str + vector.ToString() + "]";
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x000D94CC File Offset: 0x000D76CC
		private void SetPosition(Vector3 value, bool worldSpaceIsUsed = false)
		{
			this.curve.FireBeforeChange("point position is changed");
			if (this.curve.SnapType != BGCurve.SnapTypeEnum.Off)
			{
				if (worldSpaceIsUsed)
				{
					this.curve.ApplySnapping(ref value);
				}
				else
				{
					Vector3 position = this.curve.transform.TransformPoint(value);
					if (this.curve.ApplySnapping(ref position))
					{
						value = this.curve.transform.InverseTransformPoint(position);
					}
				}
			}
			if (this.pointTransform == null)
			{
				if (worldSpaceIsUsed)
				{
					Vector3 point = this.curve.transform.InverseTransformPoint(value);
					if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
					{
						point = this.curve.Apply2D(point);
					}
					this.positionLocal = point;
				}
				else
				{
					if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
					{
						value = this.curve.Apply2D(value);
					}
					this.positionLocal = value;
				}
			}
			else
			{
				if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
				{
					value = this.curve.Apply2D(value);
				}
				this.pointTransform.position = (worldSpaceIsUsed ? value : this.curve.transform.TransformPoint(value));
			}
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point position is changed") : null, false, this);
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x000D9614 File Offset: 0x000D7814
		private void SetControlFirstLocal(Vector3 value)
		{
			this.curve.FireBeforeChange("point control is changed");
			if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
			{
				value = this.curve.Apply2D(value);
			}
			if (this.controlType == BGCurvePoint.ControlTypeEnum.BezierSymmetrical)
			{
				this.controlSecondLocal = -value;
			}
			this.controlFirstLocal = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point control is changed") : null, false, this);
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x000D9698 File Offset: 0x000D7898
		private void SetControlSecondLocal(Vector3 value)
		{
			this.curve.FireBeforeChange("point control is changed");
			if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
			{
				value = this.curve.Apply2D(value);
			}
			if (this.controlType == BGCurvePoint.ControlTypeEnum.BezierSymmetrical)
			{
				this.controlFirstLocal = -value;
			}
			this.controlSecondLocal = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point control is changed") : null, false, this);
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x000D971C File Offset: 0x000D791C
		public static void PrivateFieldDeleted(BGCurvePointField field, int indexOfField, BGCurvePoint.FieldsValues fieldsValues)
		{
			BGCurvePointField.TypeEnum type = field.Type;
			if (type <= BGCurvePointField.TypeEnum.Quaternion)
			{
				switch (type)
				{
				case BGCurvePointField.TypeEnum.Bool:
					BGCurvePoint.Ensure<bool>(ref fieldsValues.boolValues);
					fieldsValues.boolValues = BGCurve.Remove<bool>(fieldsValues.boolValues, indexOfField);
					return;
				case BGCurvePointField.TypeEnum.Int:
					BGCurvePoint.Ensure<int>(ref fieldsValues.intValues);
					fieldsValues.intValues = BGCurve.Remove<int>(fieldsValues.intValues, indexOfField);
					return;
				case BGCurvePointField.TypeEnum.Float:
					BGCurvePoint.Ensure<float>(ref fieldsValues.floatValues);
					fieldsValues.floatValues = BGCurve.Remove<float>(fieldsValues.floatValues, indexOfField);
					return;
				case BGCurvePointField.TypeEnum.String:
					BGCurvePoint.Ensure<string>(ref fieldsValues.stringValues);
					fieldsValues.stringValues = BGCurve.Remove<string>(fieldsValues.stringValues, indexOfField);
					return;
				default:
					switch (type)
					{
					case BGCurvePointField.TypeEnum.Vector3:
						BGCurvePoint.Ensure<Vector3>(ref fieldsValues.vector3Values);
						fieldsValues.vector3Values = BGCurve.Remove<Vector3>(fieldsValues.vector3Values, indexOfField);
						return;
					case BGCurvePointField.TypeEnum.Bounds:
						BGCurvePoint.Ensure<Bounds>(ref fieldsValues.boundsValues);
						fieldsValues.boundsValues = BGCurve.Remove<Bounds>(fieldsValues.boundsValues, indexOfField);
						return;
					case BGCurvePointField.TypeEnum.Color:
						BGCurvePoint.Ensure<Color>(ref fieldsValues.colorValues);
						fieldsValues.colorValues = BGCurve.Remove<Color>(fieldsValues.colorValues, indexOfField);
						return;
					case BGCurvePointField.TypeEnum.Quaternion:
						BGCurvePoint.Ensure<Quaternion>(ref fieldsValues.quaternionValues);
						fieldsValues.quaternionValues = BGCurve.Remove<Quaternion>(fieldsValues.quaternionValues, indexOfField);
						return;
					}
					break;
				}
			}
			else
			{
				switch (type)
				{
				case BGCurvePointField.TypeEnum.AnimationCurve:
					BGCurvePoint.Ensure<AnimationCurve>(ref fieldsValues.animationCurveValues);
					fieldsValues.animationCurveValues = BGCurve.Remove<AnimationCurve>(fieldsValues.animationCurveValues, indexOfField);
					return;
				case BGCurvePointField.TypeEnum.GameObject:
					BGCurvePoint.Ensure<GameObject>(ref fieldsValues.gameObjectValues);
					fieldsValues.gameObjectValues = BGCurve.Remove<GameObject>(fieldsValues.gameObjectValues, indexOfField);
					return;
				case BGCurvePointField.TypeEnum.Component:
					BGCurvePoint.Ensure<Component>(ref fieldsValues.componentValues);
					fieldsValues.componentValues = BGCurve.Remove<Component>(fieldsValues.componentValues, indexOfField);
					return;
				default:
					switch (type)
					{
					case BGCurvePointField.TypeEnum.BGCurve:
						BGCurvePoint.Ensure<BGCurve>(ref fieldsValues.bgCurveValues);
						fieldsValues.bgCurveValues = BGCurve.Remove<BGCurve>(fieldsValues.bgCurveValues, indexOfField);
						return;
					case BGCurvePointField.TypeEnum.BGCurvePointComponent:
						BGCurvePoint.Ensure<BGCurvePointComponent>(ref fieldsValues.bgCurvePointComponentValues);
						fieldsValues.bgCurvePointComponentValues = BGCurve.Remove<BGCurvePointComponent>(fieldsValues.bgCurvePointComponentValues, indexOfField);
						return;
					case BGCurvePointField.TypeEnum.BGCurvePointGO:
						BGCurvePoint.Ensure<BGCurvePointGO>(ref fieldsValues.bgCurvePointGOValues);
						fieldsValues.bgCurvePointGOValues = BGCurve.Remove<BGCurvePointGO>(fieldsValues.bgCurvePointGOValues, indexOfField);
						return;
					}
					break;
				}
			}
			throw new ArgumentOutOfRangeException("field.Type", field.Type, "Unsupported type " + field.Type.ToString());
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x000D9978 File Offset: 0x000D7B78
		public static void PrivateFieldAdded(BGCurvePointField field, BGCurvePoint.FieldsValues fieldsValues)
		{
			Type type = BGCurvePoint.FieldTypes.GetType(field.Type);
			object obj = BGReflectionAdapter.IsValueType(type) ? Activator.CreateInstance(type) : null;
			BGCurvePointField.TypeEnum type2 = field.Type;
			if (type2 <= BGCurvePointField.TypeEnum.Quaternion)
			{
				switch (type2)
				{
				case BGCurvePointField.TypeEnum.Bool:
					BGCurvePoint.Ensure<bool>(ref fieldsValues.boolValues);
					fieldsValues.boolValues = BGCurve.Insert<bool>(fieldsValues.boolValues, fieldsValues.boolValues.Length, (bool)obj);
					return;
				case BGCurvePointField.TypeEnum.Int:
					BGCurvePoint.Ensure<int>(ref fieldsValues.intValues);
					fieldsValues.intValues = BGCurve.Insert<int>(fieldsValues.intValues, fieldsValues.intValues.Length, (int)obj);
					return;
				case BGCurvePointField.TypeEnum.Float:
					BGCurvePoint.Ensure<float>(ref fieldsValues.floatValues);
					fieldsValues.floatValues = BGCurve.Insert<float>(fieldsValues.floatValues, fieldsValues.floatValues.Length, (float)obj);
					return;
				case BGCurvePointField.TypeEnum.String:
					BGCurvePoint.Ensure<string>(ref fieldsValues.stringValues);
					fieldsValues.stringValues = BGCurve.Insert<string>(fieldsValues.stringValues, fieldsValues.stringValues.Length, (string)obj);
					return;
				default:
					switch (type2)
					{
					case BGCurvePointField.TypeEnum.Vector3:
						BGCurvePoint.Ensure<Vector3>(ref fieldsValues.vector3Values);
						fieldsValues.vector3Values = BGCurve.Insert<Vector3>(fieldsValues.vector3Values, fieldsValues.vector3Values.Length, (Vector3)obj);
						return;
					case BGCurvePointField.TypeEnum.Bounds:
						BGCurvePoint.Ensure<Bounds>(ref fieldsValues.boundsValues);
						fieldsValues.boundsValues = BGCurve.Insert<Bounds>(fieldsValues.boundsValues, fieldsValues.boundsValues.Length, (Bounds)obj);
						return;
					case BGCurvePointField.TypeEnum.Color:
						BGCurvePoint.Ensure<Color>(ref fieldsValues.colorValues);
						fieldsValues.colorValues = BGCurve.Insert<Color>(fieldsValues.colorValues, fieldsValues.colorValues.Length, (Color)obj);
						return;
					case BGCurvePointField.TypeEnum.Quaternion:
						BGCurvePoint.Ensure<Quaternion>(ref fieldsValues.quaternionValues);
						fieldsValues.quaternionValues = BGCurve.Insert<Quaternion>(fieldsValues.quaternionValues, fieldsValues.quaternionValues.Length, (Quaternion)obj);
						return;
					}
					break;
				}
			}
			else
			{
				switch (type2)
				{
				case BGCurvePointField.TypeEnum.AnimationCurve:
					BGCurvePoint.Ensure<AnimationCurve>(ref fieldsValues.animationCurveValues);
					fieldsValues.animationCurveValues = BGCurve.Insert<AnimationCurve>(fieldsValues.animationCurveValues, fieldsValues.animationCurveValues.Length, (AnimationCurve)obj);
					return;
				case BGCurvePointField.TypeEnum.GameObject:
					BGCurvePoint.Ensure<GameObject>(ref fieldsValues.gameObjectValues);
					fieldsValues.gameObjectValues = BGCurve.Insert<GameObject>(fieldsValues.gameObjectValues, fieldsValues.gameObjectValues.Length, (GameObject)obj);
					return;
				case BGCurvePointField.TypeEnum.Component:
					BGCurvePoint.Ensure<Component>(ref fieldsValues.componentValues);
					fieldsValues.componentValues = BGCurve.Insert<Component>(fieldsValues.componentValues, fieldsValues.componentValues.Length, (Component)obj);
					return;
				default:
					switch (type2)
					{
					case BGCurvePointField.TypeEnum.BGCurve:
						BGCurvePoint.Ensure<BGCurve>(ref fieldsValues.bgCurveValues);
						fieldsValues.bgCurveValues = BGCurve.Insert<BGCurve>(fieldsValues.bgCurveValues, fieldsValues.bgCurveValues.Length, (BGCurve)obj);
						return;
					case BGCurvePointField.TypeEnum.BGCurvePointComponent:
						BGCurvePoint.Ensure<BGCurvePointComponent>(ref fieldsValues.bgCurvePointComponentValues);
						fieldsValues.bgCurvePointComponentValues = BGCurve.Insert<BGCurvePointComponent>(fieldsValues.bgCurvePointComponentValues, fieldsValues.bgCurvePointComponentValues.Length, (BGCurvePointComponent)obj);
						return;
					case BGCurvePointField.TypeEnum.BGCurvePointGO:
						BGCurvePoint.Ensure<BGCurvePointGO>(ref fieldsValues.bgCurvePointGOValues);
						fieldsValues.bgCurvePointGOValues = BGCurve.Insert<BGCurvePointGO>(fieldsValues.bgCurvePointGOValues, fieldsValues.bgCurvePointGOValues.Length, (BGCurvePointGO)obj);
						return;
					}
					break;
				}
			}
			throw new ArgumentOutOfRangeException("field.Type", field.Type, "Unsupported type " + field.Type.ToString());
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x0004C066 File Offset: 0x0004A266
		private static void Ensure<T>(ref T[] array)
		{
			if (array == null)
			{
				array = new T[0];
			}
		}

		// Token: 0x04000CBA RID: 3258
		[SerializeField]
		private BGCurvePoint.ControlTypeEnum controlType;

		// Token: 0x04000CBB RID: 3259
		[SerializeField]
		private Vector3 positionLocal;

		// Token: 0x04000CBC RID: 3260
		[SerializeField]
		private Vector3 controlFirstLocal;

		// Token: 0x04000CBD RID: 3261
		[SerializeField]
		private Vector3 controlSecondLocal;

		// Token: 0x04000CBE RID: 3262
		[SerializeField]
		private Transform pointTransform;

		// Token: 0x04000CBF RID: 3263
		[SerializeField]
		private BGCurve curve;

		// Token: 0x04000CC0 RID: 3264
		[SerializeField]
		private BGCurvePoint.FieldsValues[] fieldsValues;

		// Token: 0x02000197 RID: 407
		public enum ControlTypeEnum
		{
			// Token: 0x04000CC2 RID: 3266
			Absent,
			// Token: 0x04000CC3 RID: 3267
			BezierSymmetrical,
			// Token: 0x04000CC4 RID: 3268
			BezierIndependant
		}

		// Token: 0x02000198 RID: 408
		public enum FieldEnum
		{
			// Token: 0x04000CC6 RID: 3270
			PositionWorld,
			// Token: 0x04000CC7 RID: 3271
			PositionLocal,
			// Token: 0x04000CC8 RID: 3272
			ControlFirstWorld,
			// Token: 0x04000CC9 RID: 3273
			ControlFirstLocal,
			// Token: 0x04000CCA RID: 3274
			ControlSecondWorld,
			// Token: 0x04000CCB RID: 3275
			ControlSecondLocal
		}

		// Token: 0x02000199 RID: 409
		[Serializable]
		public sealed class FieldsValues
		{
			// Token: 0x04000CCC RID: 3276
			[SerializeField]
			public bool[] boolValues;

			// Token: 0x04000CCD RID: 3277
			[SerializeField]
			public int[] intValues;

			// Token: 0x04000CCE RID: 3278
			[SerializeField]
			public float[] floatValues;

			// Token: 0x04000CCF RID: 3279
			[SerializeField]
			public string[] stringValues;

			// Token: 0x04000CD0 RID: 3280
			[SerializeField]
			public Vector3[] vector3Values;

			// Token: 0x04000CD1 RID: 3281
			[SerializeField]
			public Bounds[] boundsValues;

			// Token: 0x04000CD2 RID: 3282
			[SerializeField]
			public Color[] colorValues;

			// Token: 0x04000CD3 RID: 3283
			[SerializeField]
			public Quaternion[] quaternionValues;

			// Token: 0x04000CD4 RID: 3284
			[SerializeField]
			public AnimationCurve[] animationCurveValues;

			// Token: 0x04000CD5 RID: 3285
			[SerializeField]
			public GameObject[] gameObjectValues;

			// Token: 0x04000CD6 RID: 3286
			[SerializeField]
			public Component[] componentValues;

			// Token: 0x04000CD7 RID: 3287
			[SerializeField]
			public BGCurve[] bgCurveValues;

			// Token: 0x04000CD8 RID: 3288
			[SerializeField]
			public BGCurvePointComponent[] bgCurvePointComponentValues;

			// Token: 0x04000CD9 RID: 3289
			[SerializeField]
			public BGCurvePointGO[] bgCurvePointGOValues;
		}

		// Token: 0x0200019A RID: 410
		public static class FieldTypes
		{
			// Token: 0x06000DF3 RID: 3571 RVA: 0x000D9CA8 File Offset: 0x000D7EA8
			static FieldTypes()
			{
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Bool, typeof(bool), (BGCurvePoint.FieldsValues value, int index) => value.boolValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.boolValues[index] = Convert.ToBoolean(o);
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Int, typeof(int), (BGCurvePoint.FieldsValues value, int index) => value.intValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.intValues[index] = Convert.ToInt32(o);
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Float, typeof(float), (BGCurvePoint.FieldsValues value, int index) => value.floatValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.floatValues[index] = Convert.ToSingle(o);
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.String, typeof(string), (BGCurvePoint.FieldsValues value, int index) => value.stringValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.stringValues[index] = (string)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Vector3, typeof(Vector3), (BGCurvePoint.FieldsValues value, int index) => value.vector3Values[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.vector3Values[index] = (Vector3)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Bounds, typeof(Bounds), (BGCurvePoint.FieldsValues value, int index) => value.boundsValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.boundsValues[index] = (Bounds)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Quaternion, typeof(Quaternion), (BGCurvePoint.FieldsValues value, int index) => value.quaternionValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.quaternionValues[index] = (Quaternion)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Color, typeof(Color), (BGCurvePoint.FieldsValues value, int index) => value.colorValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.colorValues[index] = (Color)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.AnimationCurve, typeof(AnimationCurve), (BGCurvePoint.FieldsValues value, int index) => value.animationCurveValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.animationCurveValues[index] = (AnimationCurve)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.GameObject, typeof(GameObject), (BGCurvePoint.FieldsValues value, int index) => value.gameObjectValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.gameObjectValues[index] = (GameObject)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.Component, typeof(Component), (BGCurvePoint.FieldsValues value, int index) => value.componentValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.componentValues[index] = (Component)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.BGCurve, typeof(BGCurve), (BGCurvePoint.FieldsValues value, int index) => value.bgCurveValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.bgCurveValues[index] = (BGCurve)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.BGCurvePointComponent, typeof(BGCurvePointComponent), (BGCurvePoint.FieldsValues value, int index) => value.bgCurvePointComponentValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.bgCurvePointComponentValues[index] = (BGCurvePointComponent)o;
				});
				BGCurvePoint.FieldTypes.Register(BGCurvePointField.TypeEnum.BGCurvePointGO, typeof(BGCurvePointGO), (BGCurvePoint.FieldsValues value, int index) => value.bgCurvePointGOValues[index], delegate(BGCurvePoint.FieldsValues value, int index, object o)
				{
					value.bgCurvePointGOValues[index] = (BGCurvePointGO)o;
				});
			}

			// Token: 0x06000DF4 RID: 3572 RVA: 0x0004C074 File Offset: 0x0004A274
			private static void Register(BGCurvePointField.TypeEnum typeEnum, Type type, Func<BGCurvePoint.FieldsValues, int, object> getter, Action<BGCurvePoint.FieldsValues, int, object> setter)
			{
				BGCurvePoint.FieldTypes.type2Type[typeEnum] = type;
				BGCurvePoint.FieldTypes.type2fieldGetter[type] = getter;
				BGCurvePoint.FieldTypes.type2fieldSetter[type] = setter;
			}

			// Token: 0x06000DF5 RID: 3573 RVA: 0x0004C09A File Offset: 0x0004A29A
			public static Type GetType(BGCurvePointField.TypeEnum type)
			{
				return BGCurvePoint.FieldTypes.type2Type[type];
			}

			// Token: 0x06000DF6 RID: 3574 RVA: 0x000D9F90 File Offset: 0x000D8190
			public static object GetField(BGCurve curve, Type type, string name, BGCurvePoint.FieldsValues values)
			{
				Func<BGCurvePoint.FieldsValues, int, object> func;
				if (!BGCurvePoint.FieldTypes.type2fieldGetter.TryGetValue(type, out func))
				{
					throw new UnityException("Unsupported type for a field, type= " + ((type != null) ? type.ToString() : null));
				}
				return func(values, BGCurvePoint.FieldTypes.IndexOfFieldRelative(curve, name));
			}

			// Token: 0x06000DF7 RID: 3575 RVA: 0x000D9FD8 File Offset: 0x000D81D8
			public static void SetField(BGCurve curve, Type type, string name, object value, BGCurvePoint.FieldsValues values)
			{
				Action<BGCurvePoint.FieldsValues, int, object> action;
				if (!BGCurvePoint.FieldTypes.type2fieldSetter.TryGetValue(type, out action))
				{
					throw new UnityException("Unsupported type for a field, type= " + ((type != null) ? type.ToString() : null));
				}
				action(values, BGCurvePoint.FieldTypes.IndexOfFieldRelative(curve, name), value);
			}

			// Token: 0x06000DF8 RID: 3576 RVA: 0x0004C0A7 File Offset: 0x0004A2A7
			private static int IndexOfFieldRelative(BGCurve curve, string name)
			{
				int num = curve.IndexOfFieldValue(name);
				if (num < 0)
				{
					throw new UnityException("Can not find a field with name " + name);
				}
				return num;
			}

			// Token: 0x04000CDA RID: 3290
			private static readonly Dictionary<Type, Func<BGCurvePoint.FieldsValues, int, object>> type2fieldGetter = new Dictionary<Type, Func<BGCurvePoint.FieldsValues, int, object>>();

			// Token: 0x04000CDB RID: 3291
			private static readonly Dictionary<Type, Action<BGCurvePoint.FieldsValues, int, object>> type2fieldSetter = new Dictionary<Type, Action<BGCurvePoint.FieldsValues, int, object>>();

			// Token: 0x04000CDC RID: 3292
			private static readonly Dictionary<BGCurvePointField.TypeEnum, Type> type2Type = new Dictionary<BGCurvePointField.TypeEnum, Type>();
		}
	}
}
