using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200019F RID: 415
	[DisallowMultipleComponent]
	public class BGCurvePointGO : MonoBehaviour, BGCurvePointI
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0004C5DC File Offset: 0x0004A7DC
		public BGCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000E50 RID: 3664 RVA: 0x0004C5E4 File Offset: 0x0004A7E4
		// (set) Token: 0x06000E51 RID: 3665 RVA: 0x0004C61F File Offset: 0x0004A81F
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

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000E52 RID: 3666 RVA: 0x000DA174 File Offset: 0x000D8374
		// (set) Token: 0x06000E53 RID: 3667 RVA: 0x0004C65B File Offset: 0x0004A85B
		public Vector3 PositionLocal
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.curve.transform.InverseTransformPoint(this.pointTransform.position);
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.positionLocal;
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return this.curve.transform.InverseTransformPoint(base.transform.position);
			}
			set
			{
				this.SetPosition(value, false);
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x000DA1EC File Offset: 0x000D83EC
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x0004C665 File Offset: 0x0004A865
		public Vector3 PositionLocalTransformed
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.pointTransform.position - this.curve.transform.position;
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.curve.transform.TransformPoint(this.positionLocal) - this.curve.transform.position;
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return base.transform.position - this.curve.transform.position;
			}
			set
			{
				this.SetPosition(value + this.curve.transform.position, true);
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000E56 RID: 3670 RVA: 0x000DA290 File Offset: 0x000D8490
		// (set) Token: 0x06000E57 RID: 3671 RVA: 0x0004C684 File Offset: 0x0004A884
		public Vector3 PositionWorld
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.pointTransform.position;
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.curve.transform.TransformPoint(this.positionLocal);
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return base.transform.position;
			}
			set
			{
				this.SetPosition(value, true);
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000E58 RID: 3672 RVA: 0x0004C68E File Offset: 0x0004A88E
		// (set) Token: 0x06000E59 RID: 3673 RVA: 0x0004C696 File Offset: 0x0004A896
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

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000E5A RID: 3674 RVA: 0x0004C69F File Offset: 0x0004A89F
		// (set) Token: 0x06000E5B RID: 3675 RVA: 0x0004C6B2 File Offset: 0x0004A8B2
		public Vector3 ControlFirstLocalTransformed
		{
			get
			{
				return this.TargetTransform.TransformVector(this.controlFirstLocal);
			}
			set
			{
				this.SetControlFirstLocal(this.TargetTransform.InverseTransformVector(value));
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x000DA2F8 File Offset: 0x000D84F8
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x000DA3D0 File Offset: 0x000D85D0
		public Vector3 ControlFirstWorld
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.pointTransform.position + this.pointTransform.TransformVector(this.controlFirstLocal);
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.curve.transform.TransformPoint(new Vector3(this.positionLocal.x + this.controlFirstLocal.x, this.positionLocal.y + this.controlFirstLocal.y, this.positionLocal.z + this.controlFirstLocal.z));
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return base.transform.position + base.transform.TransformVector(this.controlFirstLocal);
			}
			set
			{
				Vector3 vector;
				if (this.pointTransform != null)
				{
					vector = this.pointTransform.InverseTransformVector(value - this.pointTransform.position);
				}
				else
				{
					BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
					if (pointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
					{
						if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
						{
							throw BGCurvePointGO.WrongMode();
						}
						vector = base.transform.InverseTransformVector(value - base.transform.position);
					}
					else
					{
						vector = this.curve.transform.InverseTransformPoint(value) - this.PositionLocal;
					}
				}
				this.SetControlFirstLocal(vector);
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0004C6C6 File Offset: 0x0004A8C6
		// (set) Token: 0x06000E5F RID: 3679 RVA: 0x0004C6CE File Offset: 0x0004A8CE
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

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000E60 RID: 3680 RVA: 0x0004C6D7 File Offset: 0x0004A8D7
		// (set) Token: 0x06000E61 RID: 3681 RVA: 0x0004C6EA File Offset: 0x0004A8EA
		public Vector3 ControlSecondLocalTransformed
		{
			get
			{
				return this.TargetTransform.TransformVector(this.controlSecondLocal);
			}
			set
			{
				this.SetControlSecondLocal(this.TargetTransform.InverseTransformVector(value));
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000E62 RID: 3682 RVA: 0x000DA46C File Offset: 0x000D866C
		// (set) Token: 0x06000E63 RID: 3683 RVA: 0x000DA544 File Offset: 0x000D8744
		public Vector3 ControlSecondWorld
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.pointTransform.position + this.pointTransform.TransformVector(this.controlSecondLocal);
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.curve.transform.TransformPoint(new Vector3(this.positionLocal.x + this.controlSecondLocal.x, this.positionLocal.y + this.controlSecondLocal.y, this.positionLocal.z + this.controlSecondLocal.z));
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return base.transform.position + base.transform.TransformVector(this.controlSecondLocal);
			}
			set
			{
				Vector3 vector;
				if (this.pointTransform != null)
				{
					vector = this.pointTransform.InverseTransformVector(value - this.pointTransform.position);
				}
				else
				{
					BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
					if (pointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
					{
						if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
						{
							throw BGCurvePointGO.WrongMode();
						}
						vector = base.transform.InverseTransformVector(value - base.transform.position);
					}
					else
					{
						vector = this.curve.transform.InverseTransformPoint(value) - this.PositionLocal;
					}
				}
				this.SetControlSecondLocal(vector);
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000E64 RID: 3684 RVA: 0x0004C6FE File Offset: 0x0004A8FE
		// (set) Token: 0x06000E65 RID: 3685 RVA: 0x000DA5E0 File Offset: 0x000D87E0
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

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000E66 RID: 3686 RVA: 0x0004C706 File Offset: 0x0004A906
		// (set) Token: 0x06000E67 RID: 3687 RVA: 0x000DA658 File Offset: 0x000D8858
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
					BGCurve.PointsModeEnum pointsMode = this.curve.PointsMode;
					if (pointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
					{
						if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
						{
							throw new ArgumentOutOfRangeException("curve.PointsMode");
						}
						base.transform.position = positionWorld;
						this.controlFirstLocal = base.transform.InverseTransformVector(controlFirstLocalTransformed);
						this.controlSecondLocal = base.transform.InverseTransformVector(controlSecondLocalTransformed);
					}
					else
					{
						this.positionLocal = this.curve.transform.InverseTransformPoint(positionWorld);
						this.controlFirstLocal = this.curve.transform.InverseTransformVector(controlFirstLocalTransformed);
						this.controlSecondLocal = this.curve.transform.InverseTransformVector(controlSecondLocalTransformed);
					}
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

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000E68 RID: 3688 RVA: 0x000DA81C File Offset: 0x000D8A1C
		private Transform TargetTransform
		{
			get
			{
				if (this.pointTransform != null)
				{
					return this.pointTransform;
				}
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					return this.curve.transform;
				}
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw BGCurvePointGO.WrongMode();
				}
				return base.transform;
			}
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x000DA86C File Offset: 0x000D8A6C
		public T GetField<T>(string name)
		{
			Type typeFromHandle = typeof(T);
			return (T)((object)this.GetField(name, typeFromHandle));
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0004C70E File Offset: 0x0004A90E
		public float GetFloat(string name)
		{
			return this.PrivateValuesForFields.floatValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x0004C728 File Offset: 0x0004A928
		public bool GetBool(string name)
		{
			return this.PrivateValuesForFields.boolValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x0004C742 File Offset: 0x0004A942
		public int GetInt(string name)
		{
			return this.PrivateValuesForFields.intValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x0004C75C File Offset: 0x0004A95C
		public Vector3 GetVector3(string name)
		{
			return this.PrivateValuesForFields.vector3Values[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0004C77A File Offset: 0x0004A97A
		public Quaternion GetQuaternion(string name)
		{
			return this.PrivateValuesForFields.quaternionValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x0004C798 File Offset: 0x0004A998
		public Bounds GetBounds(string name)
		{
			return this.PrivateValuesForFields.boundsValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x0004C7B6 File Offset: 0x0004A9B6
		public Color GetColor(string name)
		{
			return this.PrivateValuesForFields.colorValues[this.curve.IndexOfFieldValue(name)];
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x0004C7D4 File Offset: 0x0004A9D4
		public object GetField(string name, Type type)
		{
			return BGCurvePoint.FieldTypes.GetField(this.curve, type, name, this.PrivateValuesForFields);
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0004C7E9 File Offset: 0x0004A9E9
		public void SetField<T>(string name, T value)
		{
			this.SetField(name, value, typeof(T));
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000DA894 File Offset: 0x000D8A94
		public void SetField(string name, object value, Type type)
		{
			this.curve.FireBeforeChange("point field value is changed");
			BGCurvePoint.FieldTypes.SetField(this.curve, type, name, value, this.PrivateValuesForFields);
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x000DA8F4 File Offset: 0x000D8AF4
		public void SetFloat(string name, float value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.floatValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x000DA958 File Offset: 0x000D8B58
		public void SetBool(string name, bool value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.boolValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x000DA9BC File Offset: 0x000D8BBC
		public void SetInt(string name, int value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.intValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x000DAA20 File Offset: 0x000D8C20
		public void SetVector3(string name, Vector3 value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.vector3Values[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x000DAA88 File Offset: 0x000D8C88
		public void SetQuaternion(string name, Quaternion value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.quaternionValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x000DAAF0 File Offset: 0x000D8CF0
		public void SetBounds(string name, Bounds value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.boundsValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x000DAB58 File Offset: 0x000D8D58
		public void SetColor(string name, Color value)
		{
			this.curve.FireBeforeChange("point field value is changed");
			this.PrivateValuesForFields.colorValues[this.curve.IndexOfFieldValue(name)] = value;
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point field value is changed") : null, false, this);
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x000DABC0 File Offset: 0x000D8DC0
		public override string ToString()
		{
			string str = "Point [localPosition=";
			Vector3 vector = this.positionLocal;
			return str + vector.ToString() + "]";
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x000DABF0 File Offset: 0x000D8DF0
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
			if (this.pointTransform != null)
			{
				if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
				{
					value = this.curve.Apply2D(value);
				}
				this.pointTransform.position = (worldSpaceIsUsed ? value : this.curve.transform.TransformPoint(value));
			}
			else
			{
				BGCurve.PointsModeEnum pointsMode = this.Curve.PointsMode;
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
					{
						throw BGCurvePointGO.WrongMode();
					}
					if (worldSpaceIsUsed)
					{
						if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
						{
							value = this.curve.transform.TransformPoint(this.curve.Apply2D(this.curve.transform.InverseTransformPoint(value)));
						}
						base.transform.position = value;
					}
					else
					{
						if (this.curve.Mode2D != BGCurve.Mode2DEnum.Off)
						{
							value = this.curve.Apply2D(value);
						}
						base.transform.position = this.curve.transform.TransformPoint(value);
					}
				}
				else if (worldSpaceIsUsed)
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
			this.curve.FireChange(this.curve.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this.Curve, this, "point position is changed") : null, false, this);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x000DADE8 File Offset: 0x000D8FE8
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

		// Token: 0x06000E7E RID: 3710 RVA: 0x000DAE6C File Offset: 0x000D906C
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

		// Token: 0x06000E7F RID: 3711 RVA: 0x000DAEF0 File Offset: 0x000D90F0
		public void PrivateInit(BGCurvePoint point, BGCurve.PointsModeEnum pointsMode)
		{
			if (point == null)
			{
				Transform transform;
				if (pointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
				{
					if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
					{
						throw new ArgumentOutOfRangeException("pointsMode", pointsMode, null);
					}
					if (this.Curve.PointsMode != BGCurve.PointsModeEnum.GameObjectsNoTransform)
					{
						throw new ArgumentOutOfRangeException("Curve.PointsMode", "Curve points mode should be equal to GameObjectsNoTransform");
					}
					base.transform.position = this.PositionWorld;
					transform = ((this.pointTransform != null) ? this.pointTransform : base.transform);
				}
				else
				{
					if (this.Curve.PointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
					{
						throw new ArgumentOutOfRangeException("Curve.PointsMode", "Curve points mode should be equal to GameObjectsTransform");
					}
					this.positionLocal = base.transform.localPosition;
					transform = ((this.pointTransform != null) ? this.pointTransform : this.curve.transform);
				}
				this.controlFirstLocal = transform.InverseTransformVector(this.ControlFirstLocalTransformed);
				this.controlSecondLocal = transform.InverseTransformVector(this.ControlSecondLocalTransformed);
				return;
			}
			this.curve = point.Curve;
			this.controlType = point.ControlType;
			this.pointTransform = point.PointTransform;
			if (pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform)
			{
				this.positionLocal = point.PositionLocal;
				this.controlFirstLocal = point.ControlFirstLocal;
				this.controlSecondLocal = point.ControlSecondLocal;
				return;
			}
			if (pointsMode != BGCurve.PointsModeEnum.GameObjectsTransform)
			{
				throw new ArgumentOutOfRangeException("pointsMode", pointsMode, null);
			}
			base.transform.localPosition = point.PositionLocal;
			Transform transform2 = (this.pointTransform != null) ? this.pointTransform : base.transform;
			this.controlFirstLocal = transform2.InverseTransformVector(point.ControlFirstLocalTransformed);
			this.controlSecondLocal = transform2.InverseTransformVector(point.ControlSecondLocalTransformed);
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x0004C802 File Offset: 0x0004AA02
		private static ArgumentOutOfRangeException WrongMode()
		{
			return new ArgumentOutOfRangeException("Curve.PointsMode");
		}

		// Token: 0x04000CF1 RID: 3313
		[SerializeField]
		private BGCurvePoint.ControlTypeEnum controlType;

		// Token: 0x04000CF2 RID: 3314
		[SerializeField]
		private Vector3 positionLocal;

		// Token: 0x04000CF3 RID: 3315
		[SerializeField]
		private Vector3 controlFirstLocal;

		// Token: 0x04000CF4 RID: 3316
		[SerializeField]
		private Vector3 controlSecondLocal;

		// Token: 0x04000CF5 RID: 3317
		[SerializeField]
		private Transform pointTransform;

		// Token: 0x04000CF6 RID: 3318
		[SerializeField]
		private BGCurve curve;

		// Token: 0x04000CF7 RID: 3319
		[SerializeField]
		private BGCurvePoint.FieldsValues[] fieldsValues;
	}
}
