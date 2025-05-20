using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200017C RID: 380
	[HelpURL("http://www.bansheegz.com/BGCurve/")]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("BansheeGz/BGCurve/BGCurve")]
	[Serializable]
	public class BGCurve : MonoBehaviour
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000CBA RID: 3258 RVA: 0x000D1E44 File Offset: 0x000D0044
		// (remove) Token: 0x06000CBB RID: 3259 RVA: 0x000D1E7C File Offset: 0x000D007C
		public event EventHandler<BGCurveChangedArgs> Changed;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000CBC RID: 3260 RVA: 0x000D1EB4 File Offset: 0x000D00B4
		// (remove) Token: 0x06000CBD RID: 3261 RVA: 0x000D1EEC File Offset: 0x000D00EC
		public event EventHandler<BGCurveChangedArgs.BeforeChange> BeforeChange;

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000CBE RID: 3262 RVA: 0x000D1F24 File Offset: 0x000D0124
		public BGCurvePointI[] Points
		{
			get
			{
				switch (this.pointsMode)
				{
				case BGCurve.PointsModeEnum.Inlined:
					return this.points;
				case BGCurve.PointsModeEnum.Components:
					return this.pointsComponents;
				case BGCurve.PointsModeEnum.GameObjectsNoTransform:
				case BGCurve.PointsModeEnum.GameObjectsTransform:
					return this.pointsGameObjects;
				default:
					throw new ArgumentOutOfRangeException("pointsMode");
				}
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000CBF RID: 3263 RVA: 0x000D1F78 File Offset: 0x000D0178
		public int PointsCount
		{
			get
			{
				switch (this.pointsMode)
				{
				case BGCurve.PointsModeEnum.Inlined:
					return this.points.Length;
				case BGCurve.PointsModeEnum.Components:
					return this.pointsComponents.Length;
				case BGCurve.PointsModeEnum.GameObjectsNoTransform:
				case BGCurve.PointsModeEnum.GameObjectsTransform:
					return this.pointsGameObjects.Length;
				default:
					throw new ArgumentOutOfRangeException("pointsMode");
				}
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x0004B3C9 File Offset: 0x000495C9
		public BGCurvePointField[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x0004B3D1 File Offset: 0x000495D1
		public int FieldsCount
		{
			get
			{
				return this.fields.Length;
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x0004B3DB File Offset: 0x000495DB
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x0004B3E3 File Offset: 0x000495E3
		public bool Closed
		{
			get
			{
				return this.closed;
			}
			set
			{
				if (value == this.closed)
				{
					return;
				}
				this.FireBeforeChange("closed is changed");
				this.closed = value;
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "closed is changed"), false, null);
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0004B415 File Offset: 0x00049615
		// (set) Token: 0x06000CC5 RID: 3269 RVA: 0x0004B41D File Offset: 0x0004961D
		public BGCurve.PointsModeEnum PointsMode
		{
			get
			{
				return this.pointsMode;
			}
			set
			{
				if (this.pointsMode == value)
				{
					return;
				}
				this.ConvertPoints(value, null, null);
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0004B432 File Offset: 0x00049632
		// (set) Token: 0x06000CC7 RID: 3271 RVA: 0x0004B43A File Offset: 0x0004963A
		public BGCurve.Mode2DEnum Mode2D
		{
			get
			{
				return this.mode2D;
			}
			set
			{
				if (this.mode2D == value)
				{
					return;
				}
				this.Apply2D(value);
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0004B44D File Offset: 0x0004964D
		public bool Mode2DOn
		{
			get
			{
				return this.mode2D > BGCurve.Mode2DEnum.Off;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000CC9 RID: 3273 RVA: 0x0004B458 File Offset: 0x00049658
		// (set) Token: 0x06000CCA RID: 3274 RVA: 0x000D1FCC File Offset: 0x000D01CC
		public BGCurve.SnapTypeEnum SnapType
		{
			get
			{
				return this.snapType;
			}
			set
			{
				if (this.snapType == value)
				{
					return;
				}
				this.FireBeforeChange("snapType is changed");
				this.snapType = value;
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapType is changed"), false, null);
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000CCB RID: 3275 RVA: 0x0004B460 File Offset: 0x00049660
		// (set) Token: 0x06000CCC RID: 3276 RVA: 0x000D2018 File Offset: 0x000D0218
		public BGCurve.SnapAxisEnum SnapAxis
		{
			get
			{
				return this.snapAxis;
			}
			set
			{
				if (this.snapAxis == value)
				{
					return;
				}
				this.FireBeforeChange("snapAxis is changed");
				this.snapAxis = value;
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapAxis is changed"), false, null);
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x0004B468 File Offset: 0x00049668
		// (set) Token: 0x06000CCE RID: 3278 RVA: 0x000D2064 File Offset: 0x000D0264
		public float SnapDistance
		{
			get
			{
				return this.snapDistance;
			}
			set
			{
				if (Math.Abs(this.snapDistance - value) < 1E-05f)
				{
					return;
				}
				this.FireBeforeChange("snapDistance is changed");
				this.snapDistance = Mathf.Clamp(value, 0.1f, 100f);
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapDistance is changed"), false, null);
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x0004B470 File Offset: 0x00049670
		// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x000D20CC File Offset: 0x000D02CC
		public QueryTriggerInteraction SnapTriggerInteraction
		{
			get
			{
				return this.snapTriggerInteraction;
			}
			set
			{
				if (this.snapTriggerInteraction == value)
				{
					return;
				}
				this.FireBeforeChange("snapTriggerInteraction is changed");
				this.snapTriggerInteraction = value;
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapTriggerInteraction is changed"), false, null);
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0004B478 File Offset: 0x00049678
		// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x000D2118 File Offset: 0x000D0318
		public bool SnapToBackFaces
		{
			get
			{
				return this.snapToBackFaces;
			}
			set
			{
				if (this.snapToBackFaces == value)
				{
					return;
				}
				this.FireBeforeChange("snapToBackFaces is changed");
				this.snapToBackFaces = value;
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapToBackFaces is changed"), false, null);
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0004B480 File Offset: 0x00049680
		// (set) Token: 0x06000CD4 RID: 3284 RVA: 0x000D2164 File Offset: 0x000D0364
		public LayerMask SnapLayerMask
		{
			get
			{
				return this.snapLayerMask;
			}
			set
			{
				if (this.snapLayerMask == value)
				{
					return;
				}
				this.FireBeforeChange("snapLayerMask is changed");
				this.snapLayerMask = value;
				if (this.snapType != BGCurve.SnapTypeEnum.Off)
				{
					this.ApplySnapping();
				}
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapLayerMask is changed"), false, null);
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x0004B488 File Offset: 0x00049688
		// (set) Token: 0x06000CD6 RID: 3286 RVA: 0x0004B490 File Offset: 0x00049690
		public bool SnapMonitoring
		{
			get
			{
				return this.snapMonitoring;
			}
			set
			{
				if (this.snapMonitoring == value)
				{
					return;
				}
				this.FireBeforeChange("snapMonitoring is changed");
				this.snapMonitoring = value;
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Snap, "snapMonitoring is changed"), false, null);
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x0004B4C2 File Offset: 0x000496C2
		// (set) Token: 0x06000CD8 RID: 3288 RVA: 0x0004B4CA File Offset: 0x000496CA
		public BGCurve.ForceChangedEventModeEnum ForceChangedEventMode
		{
			get
			{
				return this.forceChangedEventMode;
			}
			set
			{
				if (this.forceChangedEventMode == value)
				{
					return;
				}
				this.FireBeforeChange("force update is changed");
				this.forceChangedEventMode = value;
				this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Curve, "force update is changed"), false, null);
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x0004B4FC File Offset: 0x000496FC
		// (set) Token: 0x06000CDA RID: 3290 RVA: 0x0004475B File Offset: 0x0004295B
		[Obsolete("It is not used anymore and should be removed")]
		public bool TraceChanges
		{
			get
			{
				return this.Changed != null;
			}
			set
			{
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000CDB RID: 3291 RVA: 0x0004B507 File Offset: 0x00049707
		// (set) Token: 0x06000CDC RID: 3292 RVA: 0x0004B512 File Offset: 0x00049712
		public bool SupressEvents
		{
			get
			{
				return this.eventMode == BGCurve.EventModeEnum.NoEvents;
			}
			set
			{
				if (value && this.eventMode != BGCurve.EventModeEnum.NoEvents)
				{
					this.eventModeOld = this.eventMode;
				}
				this.eventMode = (value ? BGCurve.EventModeEnum.NoEvents : this.eventModeOld);
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000CDD RID: 3293 RVA: 0x0004B53E File Offset: 0x0004973E
		// (set) Token: 0x06000CDE RID: 3294 RVA: 0x0004B546 File Offset: 0x00049746
		public bool UseEventsArgs { get; set; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000CDF RID: 3295 RVA: 0x0004B54F File Offset: 0x0004974F
		// (set) Token: 0x06000CE0 RID: 3296 RVA: 0x0004B557 File Offset: 0x00049757
		public BGCurve.EventModeEnum EventMode
		{
			get
			{
				return this.eventMode;
			}
			set
			{
				this.eventMode = value;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000CE1 RID: 3297 RVA: 0x0004B560 File Offset: 0x00049760
		// (set) Token: 0x06000CE2 RID: 3298 RVA: 0x0004B568 File Offset: 0x00049768
		public bool ImmediateChangeEvents
		{
			get
			{
				return this.immediateChangeEvents;
			}
			set
			{
				this.immediateChangeEvents = value;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x000D21BC File Offset: 0x000D03BC
		private List<BGCurveChangedArgs> ChangeList
		{
			get
			{
				List<BGCurveChangedArgs> result;
				if ((result = this.changeList) == null)
				{
					result = (this.changeList = new List<BGCurveChangedArgs>());
				}
				return result;
			}
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x0004B571 File Offset: 0x00049771
		public BGCurvePoint CreatePointFromWorldPosition(Vector3 worldPos, BGCurvePoint.ControlTypeEnum controlType)
		{
			return new BGCurvePoint(this, worldPos, controlType, true);
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x0004B57C File Offset: 0x0004977C
		public BGCurvePoint CreatePointFromWorldPosition(Vector3 worldPos, BGCurvePoint.ControlTypeEnum controlType, Vector3 control1WorldPos, Vector3 control2WorldPos)
		{
			return new BGCurvePoint(this, worldPos, controlType, control1WorldPos, control2WorldPos, true);
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x0004B58A File Offset: 0x0004978A
		public BGCurvePoint CreatePointFromLocalPosition(Vector3 localPos, BGCurvePoint.ControlTypeEnum controlType)
		{
			return new BGCurvePoint(this, localPos, controlType, false);
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0004B595 File Offset: 0x00049795
		public BGCurvePoint CreatePointFromLocalPosition(Vector3 localPos, BGCurvePoint.ControlTypeEnum controlType, Vector3 control1LocalPos, Vector3 control2LocalPos)
		{
			return new BGCurvePoint(this, localPos, controlType, control1LocalPos, control2LocalPos, false);
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000D21E4 File Offset: 0x000D03E4
		public void Clear()
		{
			int pointsCount = this.PointsCount;
			if (pointsCount == 0)
			{
				return;
			}
			this.FireBeforeChange("clear all points");
			switch (this.pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				this.points = new BGCurvePoint[0];
				break;
			case BGCurve.PointsModeEnum.Components:
				if (pointsCount > 0)
				{
					for (int i = pointsCount - 1; i >= 0; i--)
					{
						BGCurve.DestroyIt(this.pointsComponents[i]);
					}
				}
				this.pointsComponents = new BGCurvePointComponent[0];
				break;
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
				if (pointsCount > 0)
				{
					for (int j = pointsCount - 1; j >= 0; j--)
					{
						BGCurve.DestroyIt(this.pointsGameObjects[j].gameObject);
					}
				}
				this.pointsGameObjects = new BGCurvePointGO[0];
				break;
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "clear all points"), false, null);
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0004B5A3 File Offset: 0x000497A3
		public int IndexOf(BGCurvePointI point)
		{
			return BGCurve.IndexOf<BGCurvePointI>(this.Points, point);
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0004B5B1 File Offset: 0x000497B1
		public BGCurvePointI AddPoint(BGCurvePoint point)
		{
			return this.AddPoint(point, this.PointsCount, null);
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0004B5C1 File Offset: 0x000497C1
		public BGCurvePointI AddPoint(BGCurvePoint point, int index)
		{
			return this.AddPoint(point, index, null);
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0004B5CC File Offset: 0x000497CC
		public void AddPoints(BGCurvePoint[] points)
		{
			this.AddPoints(points, this.PointsCount, false, null);
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0004B5DD File Offset: 0x000497DD
		public void AddPoints(BGCurvePoint[] points, int index)
		{
			this.AddPoints(points, index, false, null);
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0004B5E9 File Offset: 0x000497E9
		public void Delete(BGCurvePointI point)
		{
			this.Delete(this.IndexOf(point), null);
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0004B5F9 File Offset: 0x000497F9
		public void Delete(int index)
		{
			this.Delete(index, null);
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0004B603 File Offset: 0x00049803
		public void Delete(BGCurvePointI[] points)
		{
			this.Delete(points, null);
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000D22A8 File Offset: 0x000D04A8
		public void Swap(int index1, int index2)
		{
			if (index1 < 0 || index1 >= this.PointsCount || index2 < 0 || index2 >= this.PointsCount)
			{
				throw new UnityException("Unable to remove a point. Invalid indexes: " + index1.ToString() + ", " + index2.ToString());
			}
			this.FireBeforeChange("swap points");
			BGCurvePointI[] array = this.Points;
			BGCurvePointI bgcurvePointI = array[index1];
			BGCurvePointI bgcurvePointI2 = array[index2];
			bool flag = bgcurvePointI.PointTransform != null || bgcurvePointI2.PointTransform != null;
			array[index2] = bgcurvePointI;
			array[index1] = bgcurvePointI2;
			if (BGCurve.IsGoMode(this.pointsMode))
			{
				this.SetPointsNames();
			}
			if (flag)
			{
				this.CachePointsWithTransforms();
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "swap points"), false, null);
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x000D2360 File Offset: 0x000D0560
		public void Reverse()
		{
			int pointsCount = this.PointsCount;
			if (pointsCount < 2)
			{
				return;
			}
			this.FireBeforeChange("reverse points");
			BGCurvePointI[] array = this.Points;
			bool flag = this.FieldsCount > 0;
			BGCurve.PointsModeEnum pointsModeEnum = this.PointsMode;
			int num = pointsCount >> 1;
			int num2 = pointsCount - 1;
			for (int i = 0; i < num; i++)
			{
				BGCurvePointI bgcurvePointI = array[i];
				BGCurvePointI bgcurvePointI2 = array[num2 - i];
				Vector3 positionLocal = bgcurvePointI2.PositionLocal;
				BGCurvePoint.ControlTypeEnum controlType = bgcurvePointI2.ControlType;
				Vector3 controlFirstLocal = bgcurvePointI2.ControlFirstLocal;
				Vector3 controlSecondLocal = bgcurvePointI2.ControlSecondLocal;
				BGCurvePoint.FieldsValues fieldsValues = flag ? this.GetFieldsValues(bgcurvePointI2, pointsModeEnum) : null;
				bgcurvePointI2.PositionLocal = bgcurvePointI.PositionLocal;
				bgcurvePointI2.ControlType = bgcurvePointI.ControlType;
				bgcurvePointI2.ControlFirstLocal = bgcurvePointI.ControlSecondLocal;
				bgcurvePointI2.ControlSecondLocal = bgcurvePointI.ControlFirstLocal;
				if (flag)
				{
					this.SetFieldsValues(bgcurvePointI2, pointsModeEnum, this.GetFieldsValues(bgcurvePointI, pointsModeEnum));
				}
				bgcurvePointI.PositionLocal = positionLocal;
				bgcurvePointI.ControlType = controlType;
				bgcurvePointI.ControlFirstLocal = controlSecondLocal;
				bgcurvePointI.ControlSecondLocal = controlFirstLocal;
				if (flag)
				{
					this.SetFieldsValues(bgcurvePointI, pointsModeEnum, fieldsValues);
				}
			}
			if (pointsCount % 2 != 0)
			{
				BGCurvePointI bgcurvePointI3 = array[num];
				Vector3 controlFirstLocal2 = bgcurvePointI3.ControlFirstLocal;
				bgcurvePointI3.ControlFirstLocal = bgcurvePointI3.ControlSecondLocal;
				bgcurvePointI3.ControlSecondLocal = controlFirstLocal2;
			}
			if (BGCurve.IsGoMode(pointsModeEnum))
			{
				this.SetPointsNames();
			}
			this.CachePointsWithTransforms();
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "reverse points"), false, null);
		}

		// Token: 0x1700039E RID: 926
		public BGCurvePointI this[int i]
		{
			get
			{
				switch (this.pointsMode)
				{
				case BGCurve.PointsModeEnum.Inlined:
					return this.points[i];
				case BGCurve.PointsModeEnum.Components:
					return this.pointsComponents[i];
				case BGCurve.PointsModeEnum.GameObjectsNoTransform:
				case BGCurve.PointsModeEnum.GameObjectsTransform:
					return this.pointsGameObjects[i];
				default:
					throw new ArgumentOutOfRangeException("pointsMode");
				}
			}
			set
			{
				switch (this.pointsMode)
				{
				case BGCurve.PointsModeEnum.Inlined:
					this.points[i] = (BGCurvePoint)value;
					return;
				case BGCurve.PointsModeEnum.Components:
					this.pointsComponents[i] = (BGCurvePointComponent)value;
					return;
				case BGCurve.PointsModeEnum.GameObjectsNoTransform:
				case BGCurve.PointsModeEnum.GameObjectsTransform:
					this.pointsGameObjects[i] = (BGCurvePointGO)value;
					return;
				default:
					throw new ArgumentOutOfRangeException("pointsMode");
				}
			}
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0004B60D File Offset: 0x0004980D
		public BGCurvePointField AddField(string name, BGCurvePointField.TypeEnum type)
		{
			return this.AddField(name, type, null);
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0004B618 File Offset: 0x00049818
		public void DeleteField(BGCurvePointField field)
		{
			this.DeleteField(field, null);
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x0004B622 File Offset: 0x00049822
		public int IndexOf(BGCurvePointField field)
		{
			return BGCurve.IndexOf<BGCurvePointField>(this.fields, field);
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x000D2584 File Offset: 0x000D0784
		public BGCurvePointField GetField(string name)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				BGCurvePointField bgcurvePointField = this.fields[i];
				if (string.Equals(bgcurvePointField.FieldName, name))
				{
					return bgcurvePointField;
				}
			}
			return null;
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x000D25C0 File Offset: 0x000D07C0
		public bool HasField(string name)
		{
			if (this.FieldsCount == 0)
			{
				return false;
			}
			foreach (BGCurvePointField bgcurvePointField in this.fields)
			{
				if (string.Equals(name, bgcurvePointField.FieldName))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x0004B630 File Offset: 0x00049830
		public int IndexOfFieldValue(string name)
		{
			if (this.fieldsTree == null || !this.fieldsTree.Comply(this.fields))
			{
				this.PrivateUpdateFieldsValuesIndexes();
			}
			return this.fieldsTree.GetIndex(name);
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0004B65F File Offset: 0x0004985F
		public void PrivateUpdateFieldsValuesIndexes()
		{
			this.fieldsTree = (this.fieldsTree ?? new BGCurve.FieldsTree());
			this.fieldsTree.Update(this.fields);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x000D2604 File Offset: 0x000D0804
		public void Apply2D(BGCurve.Mode2DEnum value)
		{
			this.FireBeforeChange("2d mode is changed");
			this.mode2D = value;
			if (this.mode2D != BGCurve.Mode2DEnum.Off && this.PointsCount > 0)
			{
				this.Transaction(delegate
				{
					BGCurvePointI[] array = this.Points;
					int num = array.Length;
					for (int i = 0; i < num; i++)
					{
						this.Apply2D(array[i]);
					}
				});
				return;
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "2d mode is changed"), false, null);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0004B687 File Offset: 0x00049887
		public virtual void Apply2D(BGCurvePointI point)
		{
			point.PositionLocal = point.PositionLocal;
			point.ControlFirstLocal = point.ControlFirstLocal;
			point.ControlSecondLocal = point.ControlSecondLocal;
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x000D265C File Offset: 0x000D085C
		public virtual Vector3 Apply2D(Vector3 point)
		{
			switch (this.mode2D)
			{
			case BGCurve.Mode2DEnum.XY:
				return new Vector3(point.x, point.y, 0f);
			case BGCurve.Mode2DEnum.XZ:
				return new Vector3(point.x, 0f, point.z);
			case BGCurve.Mode2DEnum.YZ:
				return new Vector3(0f, point.y, point.z);
			default:
				return point;
			}
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x000D26CC File Offset: 0x000D08CC
		public bool ApplySnapping()
		{
			if (this.snapType == BGCurve.SnapTypeEnum.Off)
			{
				return false;
			}
			BGCurvePointI[] array = this.Points;
			int num = array.Length;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				flag |= this.ApplySnapping(array[i]);
			}
			return flag;
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x000D270C File Offset: 0x000D090C
		public bool ApplySnapping(BGCurvePointI point)
		{
			if (this.snapType == BGCurve.SnapTypeEnum.Off)
			{
				return false;
			}
			Vector3 positionWorld = point.PositionWorld;
			bool flag = this.ApplySnapping(ref positionWorld);
			if (flag)
			{
				point.PositionWorld = positionWorld;
			}
			return flag;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x000D273C File Offset: 0x000D093C
		public bool ApplySnapping(ref Vector3 pos)
		{
			if (this.snapType == BGCurve.SnapTypeEnum.Off)
			{
				return false;
			}
			BGCurve.SnapAxisEnum snapAxisEnum = this.snapAxis;
			Vector3 vector;
			if (snapAxisEnum != BGCurve.SnapAxisEnum.X)
			{
				if (snapAxisEnum == BGCurve.SnapAxisEnum.Y)
				{
					vector = Vector3.up;
				}
				else
				{
					vector = Vector3.forward;
				}
			}
			else
			{
				vector = Vector3.right;
			}
			Vector3 vector2 = default(Vector3);
			float num = -1f;
			for (int i = 0; i < 2; i++)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(pos, (i == 0) ? vector : (-vector)), out raycastHit, this.snapDistance, this.snapLayerMask, this.snapTriggerInteraction) && (num < 0f || num > raycastHit.distance))
				{
					num = raycastHit.distance;
					vector2 = raycastHit.point;
				}
			}
			if (this.snapToBackFaces)
			{
				for (int j = 0; j < 2; j++)
				{
					int num2 = Physics.RaycastNonAlloc((j == 0) ? new Ray(new Vector3(pos.x + vector.x * this.snapDistance, pos.y + vector.y * this.snapDistance, pos.z + vector.z * this.snapDistance), -vector) : new Ray(new Vector3(pos.x - vector.x * this.snapDistance, pos.y - vector.y * this.snapDistance, pos.z - vector.z * this.snapDistance), vector), BGCurve.raycastHitArray, this.snapDistance, this.snapLayerMask, this.snapTriggerInteraction);
					if (num2 != 0)
					{
						for (int k = 0; k < num2; k++)
						{
							RaycastHit raycastHit2 = BGCurve.raycastHitArray[k];
							float num3 = this.snapDistance - raycastHit2.distance;
							if (num < 0f || num > num3)
							{
								num = num3;
								vector2 = raycastHit2.point;
							}
						}
					}
				}
			}
			if (num < 0f)
			{
				return false;
			}
			pos = vector2;
			return true;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x000D292C File Offset: 0x000D0B2C
		private void SnapIt()
		{
			switch (this.snapType)
			{
			case BGCurve.SnapTypeEnum.Off:
				break;
			case BGCurve.SnapTypeEnum.Points:
				this.ApplySnapping();
				return;
			case BGCurve.SnapTypeEnum.Curve:
				if (!this.ApplySnapping())
				{
					if (this.immediateChangeEvents && this.forceChangedEventMode != BGCurve.ForceChangedEventModeEnum.EditorAndRuntime)
					{
						this.FireChange(this.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this, this.lastEventType, this.lastEventMessage) : null, true, null);
						return;
					}
					this.changed = true;
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x000D29AC File Offset: 0x000D0BAC
		public void Transaction(Action action)
		{
			this.FireBeforeChange("changes in transaction");
			this.transactionLevel++;
			if (this.UseEventsArgs && this.transactionLevel == 1)
			{
				this.ChangeList.Clear();
			}
			try
			{
				action();
			}
			finally
			{
				this.transactionLevel--;
				if (this.transactionLevel == 0)
				{
					this.FireChange(this.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this, this.ChangeList.ToArray(), "changes in transaction") : null, false, null);
					if (this.UseEventsArgs)
					{
						this.ChangeList.Clear();
					}
				}
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000D04 RID: 3332 RVA: 0x0004B6AD File Offset: 0x000498AD
		public int TransactionLevel
		{
			get
			{
				return this.transactionLevel;
			}
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0004B6B5 File Offset: 0x000498B5
		protected internal static int IndexOf<T>(T[] array, T item)
		{
			return Array.IndexOf<T>(array, item);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0004B6BE File Offset: 0x000498BE
		public void FireBeforeChange(string operation)
		{
			if (this.eventMode == BGCurve.EventModeEnum.NoEvents || this.transactionLevel > 0 || this.BeforeChange == null)
			{
				return;
			}
			this.BeforeChange(this, this.UseEventsArgs ? BGCurveChangedArgs.BeforeChange.GetInstance(operation) : null);
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000D2A5C File Offset: 0x000D0C5C
		public void FireChange(BGCurveChangedArgs change, bool ignoreEventsGrouping = false, object sender = null)
		{
			if (this.eventMode == BGCurve.EventModeEnum.NoEvents || this.Changed == null)
			{
				return;
			}
			if (this.transactionLevel > 0 || (!this.immediateChangeEvents && !ignoreEventsGrouping))
			{
				this.changed = true;
				if (change != null)
				{
					this.lastEventType = change.ChangeType;
					this.lastEventMessage = change.Message;
				}
				if (this.UseEventsArgs && !this.ChangeList.Contains(change))
				{
					this.ChangeList.Add((BGCurveChangedArgs)change.Clone());
				}
				return;
			}
			this.Changed(sender ?? this, change);
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0004B6F8 File Offset: 0x000498F8
		public void Start()
		{
			this.CachePointsWithTransforms();
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x000D2AF0 File Offset: 0x000D0CF0
		protected void Update()
		{
			if (Time.frameCount == 1)
			{
				base.transform.hasChanged = false;
			}
			if (this.eventMode != BGCurve.EventModeEnum.Update)
			{
				return;
			}
			if (this.snapMonitoring && this.snapType != BGCurve.SnapTypeEnum.Off)
			{
				this.SnapIt();
			}
			if (this.Changed == null)
			{
				return;
			}
			this.FireFinalEvent();
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0004B700 File Offset: 0x00049900
		protected void LateUpdate()
		{
			if (this.eventMode != BGCurve.EventModeEnum.LateUpdate)
			{
				return;
			}
			if (this.snapMonitoring && this.snapType != BGCurve.SnapTypeEnum.Off)
			{
				this.SnapIt();
			}
			if (this.Changed == null)
			{
				return;
			}
			this.FireFinalEvent();
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0004B731 File Offset: 0x00049931
		public Vector3 ToLocal(Vector3 worldPoint)
		{
			return base.transform.InverseTransformPoint(worldPoint);
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0004B73F File Offset: 0x0004993F
		public Vector3 ToWorld(Vector3 localPoint)
		{
			return base.transform.TransformPoint(localPoint);
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0004B74D File Offset: 0x0004994D
		public Vector3 ToLocalDirection(Vector3 direction)
		{
			return base.transform.InverseTransformDirection(direction);
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0004B75B File Offset: 0x0004995B
		public Vector3 ToWorldDirection(Vector3 direction)
		{
			return base.transform.TransformDirection(direction);
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x000D2B40 File Offset: 0x000D0D40
		public void ForEach(BGCurve.IterationCallback iterationCallback)
		{
			for (int i = 0; i < this.PointsCount; i++)
			{
				iterationCallback(this.Points[i], i, this.PointsCount);
			}
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x000D2B74 File Offset: 0x000D0D74
		private void SetPointsNames()
		{
			try
			{
				if (base.gameObject == null)
				{
					return;
				}
			}
			catch (MissingReferenceException)
			{
				return;
			}
			string name = base.gameObject.name;
			int num = this.pointsGameObjects.Length;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = this.pointsGameObjects[i].gameObject;
				gameObject.name = name + "[" + i.ToString() + "]";
				gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x000D2C10 File Offset: 0x000D0E10
		public static T[] Insert<T>(T[] oldArray, int index, T[] newElements)
		{
			T[] array = new T[oldArray.Length + newElements.Length];
			if (index > 0)
			{
				Array.Copy(oldArray, array, index);
			}
			if (index < oldArray.Length)
			{
				Array.Copy(oldArray, index, array, index + newElements.Length, oldArray.Length - index);
			}
			Array.Copy(newElements, 0, array, index, newElements.Length);
			return array;
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x000D2C5C File Offset: 0x000D0E5C
		public static T[] Insert<T>(T[] oldArray, int index, T newElement)
		{
			T[] array = new T[oldArray.Length + 1];
			if (index > 0)
			{
				Array.Copy(oldArray, array, index);
			}
			if (index < oldArray.Length)
			{
				Array.Copy(oldArray, index, array, index + 1, oldArray.Length - index);
			}
			array[index] = newElement;
			return array;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x000D2CA0 File Offset: 0x000D0EA0
		public static T[] Remove<T>(T[] oldArray, int index)
		{
			T[] array = new T[oldArray.Length - 1];
			if (index > 0)
			{
				Array.Copy(oldArray, array, index);
			}
			if (index < oldArray.Length - 1)
			{
				Array.Copy(oldArray, index + 1, array, index, oldArray.Length - 1 - index);
			}
			return array;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x000D2CE0 File Offset: 0x000D0EE0
		public override string ToString()
		{
			return "BGCurve [id=" + base.GetInstanceID().ToString() + "], points=" + this.PointsCount.ToString();
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0004B769 File Offset: 0x00049969
		public static bool IsGoMode(BGCurve.PointsModeEnum pointsMode)
		{
			return pointsMode == BGCurve.PointsModeEnum.GameObjectsNoTransform || pointsMode == BGCurve.PointsModeEnum.GameObjectsTransform;
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0004B775 File Offset: 0x00049975
		public void PrivateTransformForPointAdded(int index)
		{
			if (index < 0 || this.PointsCount >= index)
			{
				return;
			}
			if (this.pointsWithTransforms == null)
			{
				this.pointsWithTransforms = new List<int>();
			}
			if (this.pointsWithTransforms.IndexOf(index) == -1)
			{
				this.pointsWithTransforms.Add(index);
			}
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0004B7B3 File Offset: 0x000499B3
		public void PrivateTransformForPointRemoved(int index)
		{
			if (this.pointsWithTransforms == null || this.pointsWithTransforms.IndexOf(index) == -1)
			{
				return;
			}
			this.pointsWithTransforms.Remove(index);
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x000D2D18 File Offset: 0x000D0F18
		private BGCurvePointI AddPoint(BGCurvePoint point, int index, Func<BGCurvePointI> provider = null)
		{
			if (index < 0 || index > this.PointsCount)
			{
				throw new UnityException("Unable to add a point. Invalid index: " + index.ToString());
			}
			this.FireBeforeChange("add a point");
			BGCurvePointI bgcurvePointI;
			switch (this.pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				this.points = BGCurve.Insert<BGCurvePoint>(this.points, index, point);
				bgcurvePointI = point;
				break;
			case BGCurve.PointsModeEnum.Components:
			{
				BGCurvePointComponent bgcurvePointComponent = (BGCurvePointComponent)BGCurve.Convert(point, BGCurve.PointsModeEnum.Inlined, this.pointsMode, provider);
				this.pointsComponents = BGCurve.Insert<BGCurvePointComponent>(this.pointsComponents, index, bgcurvePointComponent);
				bgcurvePointI = bgcurvePointComponent;
				break;
			}
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
			{
				BGCurvePointGO bgcurvePointGO = (BGCurvePointGO)BGCurve.Convert(point, BGCurve.PointsModeEnum.Inlined, this.pointsMode, provider);
				this.pointsGameObjects = BGCurve.Insert<BGCurvePointGO>(this.pointsGameObjects, index, bgcurvePointGO);
				this.SetPointsNames();
				bgcurvePointI = bgcurvePointGO;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
			if (this.FieldsCount > 0)
			{
				BGCurvePoint.FieldsValues fieldsValues = this.GetFieldsValues(bgcurvePointI, this.pointsMode);
				BGCurvePointField[] array = this.fields;
				for (int i = 0; i < array.Length; i++)
				{
					BGCurvePoint.PrivateFieldAdded(array[i], fieldsValues);
				}
			}
			if (point.PointTransform != null || (this.pointsWithTransforms != null && this.pointsWithTransforms.Count > 0))
			{
				this.CachePointsWithTransforms();
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Point, "add a point"), false, null);
			return bgcurvePointI;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x000D2E70 File Offset: 0x000D1070
		private BGCurvePoint.FieldsValues GetFieldsValues(BGCurvePointI point, BGCurve.PointsModeEnum pointsMode)
		{
			BGCurvePoint.FieldsValues privateValuesForFields;
			switch (pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				privateValuesForFields = ((BGCurvePoint)point).PrivateValuesForFields;
				break;
			case BGCurve.PointsModeEnum.Components:
				privateValuesForFields = ((BGCurvePointComponent)point).Point.PrivateValuesForFields;
				break;
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
				privateValuesForFields = ((BGCurvePointGO)point).PrivateValuesForFields;
				break;
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
			return privateValuesForFields;
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000D2ED0 File Offset: 0x000D10D0
		private void SetFieldsValues(BGCurvePointI point, BGCurve.PointsModeEnum pointsMode, BGCurvePoint.FieldsValues fieldsValues)
		{
			switch (pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				((BGCurvePoint)point).PrivateValuesForFields = fieldsValues;
				return;
			case BGCurve.PointsModeEnum.Components:
				((BGCurvePointComponent)point).Point.PrivateValuesForFields = fieldsValues;
				return;
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
				((BGCurvePointGO)point).PrivateValuesForFields = fieldsValues;
				return;
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x000D2F2C File Offset: 0x000D112C
		private void AddPoints(BGCurvePoint[] points, int index, bool skipFieldsProcessing = false, Func<BGCurvePointI> provider = null)
		{
			if (points == null)
			{
				return;
			}
			int num = points.Length;
			if (num == 0)
			{
				return;
			}
			if (index < 0 || index > this.PointsCount)
			{
				throw new UnityException("Unable to add points. Invalid index: " + index.ToString());
			}
			this.FireBeforeChange("add points");
			bool flag = this.pointsWithTransforms != null && this.pointsWithTransforms.Count > 0;
			BGCurvePointI[] addedPoints;
			switch (this.pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				this.points = BGCurve.Insert<BGCurvePoint>(this.points, index, points);
				if (!flag)
				{
					for (int i = 0; i < num; i++)
					{
						if (!(points[i].PointTransform == null))
						{
							flag = true;
							break;
						}
					}
				}
				addedPoints = points;
				break;
			case BGCurve.PointsModeEnum.Components:
			{
				BGCurvePointComponent[] array = new BGCurvePointComponent[num];
				for (int j = 0; j < num; j++)
				{
					BGCurvePoint bgcurvePoint = points[j];
					flag = (flag || bgcurvePoint.PointTransform != null);
					array[j] = (BGCurvePointComponent)BGCurve.Convert(bgcurvePoint, BGCurve.PointsModeEnum.Inlined, this.pointsMode, provider);
				}
				this.pointsComponents = BGCurve.Insert<BGCurvePointComponent>(this.pointsComponents, index, array);
				addedPoints = points;
				break;
			}
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
			{
				BGCurvePointGO[] array2 = new BGCurvePointGO[num];
				for (int k = 0; k < num; k++)
				{
					BGCurvePoint bgcurvePoint2 = points[k];
					flag = (flag || bgcurvePoint2.PointTransform != null);
					array2[k] = (BGCurvePointGO)BGCurve.Convert(bgcurvePoint2, BGCurve.PointsModeEnum.Inlined, this.pointsMode, provider);
				}
				this.pointsGameObjects = BGCurve.Insert<BGCurvePointGO>(this.pointsGameObjects, index, array2);
				this.SetPointsNames();
				BGCurvePointI[] array3 = array2;
				addedPoints = array3;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
			if (!skipFieldsProcessing && this.FieldsCount > 0)
			{
				this.AddFields(this.pointsMode, addedPoints);
			}
			if (flag)
			{
				this.CachePointsWithTransforms();
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "add points"), false, null);
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x000D3110 File Offset: 0x000D1310
		private void Delete(int index, Action<BGCurvePointI> destroyer)
		{
			if (index < 0 || index >= this.PointsCount)
			{
				throw new UnityException("Unable to remove a point. Invalid index: " + index.ToString());
			}
			switch (this.pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
				BGCurve.pointArray[0] = this.points[index];
				break;
			case BGCurve.PointsModeEnum.Components:
				BGCurve.pointArray[0] = this.pointsComponents[index];
				break;
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
				BGCurve.pointArray[0] = this.pointsGameObjects[index];
				break;
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
			this.Delete(BGCurve.pointArray, destroyer);
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x000D31AC File Offset: 0x000D13AC
		private void Delete(BGCurvePointI[] pointsToDelete, Action<BGCurvePointI> destroyer)
		{
			if (pointsToDelete == null || pointsToDelete.Length == 0 || this.PointsCount == 0)
			{
				return;
			}
			BGCurve.pointsList.Clear();
			BGCurve.pointsIndexesList.Clear();
			BGCurvePointI[] array = this.Points;
			int num = pointsToDelete.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = Array.IndexOf<BGCurvePointI>(array, pointsToDelete[i]);
				if (num2 >= 0)
				{
					BGCurve.pointsIndexesList.Add(num2);
				}
			}
			if (BGCurve.pointsIndexesList.Count == 0)
			{
				return;
			}
			this.FireBeforeChange("delete points");
			int num3 = array.Length - BGCurve.pointsIndexesList.Count;
			BGCurvePointI[] destinationArray;
			switch (this.pointsMode)
			{
			case BGCurve.PointsModeEnum.Inlined:
			{
				this.points = new BGCurvePoint[num3];
				BGCurvePointI[] array2 = this.points;
				destinationArray = array2;
				break;
			}
			case BGCurve.PointsModeEnum.Components:
			{
				this.pointsComponents = new BGCurvePointComponent[num3];
				BGCurvePointI[] array2 = this.pointsComponents;
				destinationArray = array2;
				break;
			}
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
			{
				this.pointsGameObjects = new BGCurvePointGO[num3];
				BGCurvePointI[] array2 = this.pointsGameObjects;
				destinationArray = array2;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("pointsMode");
			}
			BGCurve.pointsIndexesList.Sort();
			int num4 = 0;
			int count = BGCurve.pointsIndexesList.Count;
			BGCurve.PointsModeEnum pointsModeEnum;
			for (int j = 0; j < count; j++)
			{
				int num5 = BGCurve.pointsIndexesList[j];
				if (num5 > num4)
				{
					Array.Copy(array, num4, destinationArray, num4 - j, num5 - num4);
				}
				num4 = num5 + 1;
				pointsModeEnum = this.pointsMode;
				if (pointsModeEnum - BGCurve.PointsModeEnum.Components <= 2)
				{
					BGCurve.pointsList.Add(array[num5]);
				}
			}
			if (num4 < array.Length)
			{
				Array.Copy(array, num4, destinationArray, num4 - count, array.Length - num4);
			}
			pointsModeEnum = this.pointsMode;
			if (pointsModeEnum - BGCurve.PointsModeEnum.GameObjectsNoTransform <= 1)
			{
				this.SetPointsNames();
			}
			if (BGCurve.pointsList.Count > 0)
			{
				int count2 = BGCurve.pointsList.Count;
				for (int k = 0; k < count2; k++)
				{
					BGCurvePointI bgcurvePointI = BGCurve.pointsList[k];
					if (destroyer != null)
					{
						destroyer(bgcurvePointI);
					}
					else
					{
						pointsModeEnum = this.pointsMode;
						if (pointsModeEnum != BGCurve.PointsModeEnum.Components)
						{
							if (pointsModeEnum - BGCurve.PointsModeEnum.GameObjectsNoTransform > 1)
							{
								throw new ArgumentOutOfRangeException("pointsMode");
							}
							BGCurve.DestroyIt(((BGCurvePointGO)bgcurvePointI).gameObject);
						}
						else
						{
							BGCurve.DestroyIt((UnityEngine.Object)bgcurvePointI);
						}
					}
				}
			}
			this.CachePointsWithTransforms();
			BGCurve.pointsList.Clear();
			BGCurve.pointsIndexesList.Clear();
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "delete points"), false, null);
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x000D340C File Offset: 0x000D160C
		private void ConvertPoints(BGCurve.PointsModeEnum pointsMode, Func<BGCurvePointI> provider = null, Action<BGCurvePointI> destroyer = null)
		{
			BGCurve.PointsModeEnum pointsModeEnum = this.PointsMode;
			if (pointsModeEnum == pointsMode)
			{
				return;
			}
			this.FireBeforeChange("points mode is changed");
			if (pointsModeEnum != BGCurve.PointsModeEnum.Inlined)
			{
				if (pointsModeEnum - BGCurve.PointsModeEnum.Components > 2)
				{
					throw new ArgumentOutOfRangeException("pointsMode");
				}
				if ((pointsModeEnum == BGCurve.PointsModeEnum.Components && this.pointsComponents.Length != 0) || (BGCurve.IsGoMode(pointsModeEnum) && this.pointsGameObjects.Length != 0))
				{
					BGCurvePointI[] array = null;
					if (pointsModeEnum == BGCurve.PointsModeEnum.Components)
					{
						BGCurvePointI[] array3;
						if (pointsMode != BGCurve.PointsModeEnum.Inlined)
						{
							if (pointsMode - BGCurve.PointsModeEnum.GameObjectsNoTransform <= 1)
							{
								BGCurvePointGO[] array2 = new BGCurvePointGO[this.pointsComponents.Length];
								for (int i = 0; i < this.pointsComponents.Length; i++)
								{
									array2[i] = (BGCurvePointGO)BGCurve.Convert(this.pointsComponents[i], pointsModeEnum, pointsMode, provider);
								}
								this.pointsGameObjects = BGCurve.Insert<BGCurvePointGO>(this.pointsGameObjects, 0, array2);
								this.SetPointsNames();
								if (this.FieldsCount > 0)
								{
									array3 = array2;
									this.AddFields(pointsMode, array3);
								}
							}
						}
						else
						{
							BGCurvePoint[] array4 = new BGCurvePoint[this.pointsComponents.Length];
							for (int j = 0; j < this.pointsComponents.Length; j++)
							{
								array4[j] = (BGCurvePoint)BGCurve.Convert(this.pointsComponents[j], pointsModeEnum, pointsMode, provider);
							}
							this.points = BGCurve.Insert<BGCurvePoint>(this.points, 0, array4);
						}
						array3 = this.pointsComponents;
						array = array3;
						this.pointsComponents = new BGCurvePointComponent[0];
					}
					else
					{
						switch (pointsMode)
						{
						case BGCurve.PointsModeEnum.Inlined:
						{
							BGCurvePoint[] array5 = new BGCurvePoint[this.pointsGameObjects.Length];
							for (int k = 0; k < this.pointsGameObjects.Length; k++)
							{
								array5[k] = (BGCurvePoint)BGCurve.Convert(this.pointsGameObjects[k], pointsModeEnum, pointsMode, provider);
							}
							this.points = BGCurve.Insert<BGCurvePoint>(this.points, 0, array5);
							BGCurvePointI[] array3 = this.pointsGameObjects;
							array = array3;
							break;
						}
						case BGCurve.PointsModeEnum.Components:
						{
							BGCurvePointComponent[] array6 = new BGCurvePointComponent[this.pointsGameObjects.Length];
							for (int l = 0; l < this.pointsGameObjects.Length; l++)
							{
								array6[l] = (BGCurvePointComponent)BGCurve.Convert(this.pointsGameObjects[l], pointsModeEnum, pointsMode, provider);
							}
							this.pointsComponents = BGCurve.Insert<BGCurvePointComponent>(this.pointsComponents, 0, array6);
							BGCurvePointI[] array3 = this.pointsGameObjects;
							array = array3;
							break;
						}
						case BGCurve.PointsModeEnum.GameObjectsNoTransform:
						case BGCurve.PointsModeEnum.GameObjectsTransform:
							for (int m = 0; m < this.pointsGameObjects.Length; m++)
							{
								BGCurve.Convert(this.pointsGameObjects[m], pointsModeEnum, pointsMode, provider);
							}
							break;
						}
						if (array != null)
						{
							this.pointsGameObjects = new BGCurvePointGO[0];
						}
					}
					this.pointsMode = pointsMode;
					if (array != null)
					{
						bool flag = pointsModeEnum == BGCurve.PointsModeEnum.Components;
						foreach (BGCurvePointI bgcurvePointI in array)
						{
							if (destroyer != null)
							{
								destroyer(bgcurvePointI);
							}
							else
							{
								BGCurve.DestroyIt(flag ? ((UnityEngine.Object)bgcurvePointI) : ((BGCurvePointGO)bgcurvePointI).gameObject);
							}
						}
					}
				}
			}
			else if (this.points.Length != 0)
			{
				if (pointsMode != BGCurve.PointsModeEnum.Components)
				{
					if (pointsMode - BGCurve.PointsModeEnum.GameObjectsNoTransform <= 1)
					{
						BGCurvePointGO[] array7 = new BGCurvePointGO[this.points.Length];
						for (int num = 0; num < this.points.Length; num++)
						{
							array7[num] = (BGCurvePointGO)BGCurve.Convert(this.points[num], pointsModeEnum, pointsMode, provider);
						}
						this.pointsMode = pointsMode;
						this.pointsGameObjects = BGCurve.Insert<BGCurvePointGO>(this.pointsGameObjects, 0, array7);
						this.SetPointsNames();
						if (this.FieldsCount > 0)
						{
							BGCurvePointI[] array3 = array7;
							this.AddFields(pointsMode, array3);
						}
					}
				}
				else
				{
					BGCurvePointComponent[] array8 = new BGCurvePointComponent[this.points.Length];
					for (int num2 = 0; num2 < this.points.Length; num2++)
					{
						array8[num2] = (BGCurvePointComponent)BGCurve.Convert(this.points[num2], pointsModeEnum, pointsMode, provider);
					}
					this.pointsMode = pointsMode;
					this.pointsComponents = BGCurve.Insert<BGCurvePointComponent>(this.pointsComponents, 0, array8);
				}
				this.points = new BGCurvePoint[0];
			}
			this.pointsMode = pointsMode;
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Points, "points mode is changed"), false, null);
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x000D37F8 File Offset: 0x000D19F8
		private static BGCurvePointI Convert(BGCurvePointI point, BGCurve.PointsModeEnum from, BGCurve.PointsModeEnum to, Func<BGCurvePointI> provider)
		{
			BGCurvePointI bgcurvePointI;
			switch (from)
			{
			case BGCurve.PointsModeEnum.Inlined:
				if (to != BGCurve.PointsModeEnum.Components)
				{
					if (to - BGCurve.PointsModeEnum.GameObjectsNoTransform > 1)
					{
						throw new ArgumentOutOfRangeException("to", to, null);
					}
					bgcurvePointI = BGCurve.ConvertInlineToGo((BGCurvePoint)point, to, provider);
				}
				else
				{
					bgcurvePointI = ((provider == null) ? point.Curve.gameObject.AddComponent<BGCurvePointComponent>() : ((BGCurvePointComponent)provider()));
					((BGCurvePointComponent)bgcurvePointI).PrivateInit((BGCurvePoint)point);
				}
				break;
			case BGCurve.PointsModeEnum.Components:
				if (to != BGCurve.PointsModeEnum.Inlined)
				{
					if (to - BGCurve.PointsModeEnum.GameObjectsNoTransform > 1)
					{
						throw new ArgumentOutOfRangeException("to", to, null);
					}
					bgcurvePointI = BGCurve.ConvertInlineToGo(((BGCurvePointComponent)point).Point, to, provider);
				}
				else
				{
					bgcurvePointI = ((BGCurvePointComponent)point).Point;
				}
				break;
			case BGCurve.PointsModeEnum.GameObjectsNoTransform:
			case BGCurve.PointsModeEnum.GameObjectsTransform:
				switch (to)
				{
				case BGCurve.PointsModeEnum.Inlined:
					bgcurvePointI = BGCurve.ConvertGoToInline((BGCurvePointGO)point, from);
					break;
				case BGCurve.PointsModeEnum.Components:
					bgcurvePointI = ((provider != null) ? ((BGCurvePointComponent)provider()) : point.Curve.gameObject.AddComponent<BGCurvePointComponent>());
					((BGCurvePointComponent)bgcurvePointI).PrivateInit(BGCurve.ConvertGoToInline((BGCurvePointGO)point, from));
					break;
				case BGCurve.PointsModeEnum.GameObjectsNoTransform:
				case BGCurve.PointsModeEnum.GameObjectsTransform:
					((BGCurvePointGO)point).PrivateInit(null, to);
					bgcurvePointI = point;
					break;
				default:
					throw new ArgumentOutOfRangeException("to", to, null);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("from", from, null);
			}
			Transform pointTransform = point.PointTransform;
			if (pointTransform != null)
			{
				BGCurveReferenceToPoint bgcurveReferenceToPoint = null;
				if (from != BGCurve.PointsModeEnum.Inlined)
				{
					bgcurveReferenceToPoint = BGCurveReferenceToPoint.GetReferenceToPoint(point);
				}
				if (to != BGCurve.PointsModeEnum.Inlined)
				{
					if (bgcurveReferenceToPoint == null)
					{
						bgcurveReferenceToPoint = pointTransform.gameObject.AddComponent<BGCurveReferenceToPoint>();
					}
					bgcurveReferenceToPoint.Point = bgcurvePointI;
				}
				else if (bgcurveReferenceToPoint != null)
				{
					BGCurve.DestroyIt(bgcurveReferenceToPoint);
				}
			}
			return bgcurvePointI;
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x000D39B4 File Offset: 0x000D1BB4
		private static BGCurvePointGO ConvertInlineToGo(BGCurvePoint point, BGCurve.PointsModeEnum to, Func<BGCurvePointI> provider)
		{
			BGCurvePointGO bgcurvePointGO;
			if (provider != null)
			{
				bgcurvePointGO = (BGCurvePointGO)provider();
			}
			else
			{
				GameObject gameObject = new GameObject();
				Transform transform = gameObject.transform;
				transform.parent = point.Curve.transform;
				transform.localRotation = Quaternion.identity;
				transform.localPosition = Vector3.zero;
				transform.localScale = Vector3.one;
				bgcurvePointGO = gameObject.AddComponent<BGCurvePointGO>();
			}
			bgcurvePointGO.PrivateInit(point, to);
			bgcurvePointGO.PrivateValuesForFields = point.PrivateValuesForFields;
			return bgcurvePointGO;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000D3A2C File Offset: 0x000D1C2C
		private static BGCurvePoint ConvertGoToInline(BGCurvePointGO pointGO, BGCurve.PointsModeEnum from)
		{
			BGCurvePoint bgcurvePoint;
			if (from != BGCurve.PointsModeEnum.GameObjectsNoTransform)
			{
				if (from != BGCurve.PointsModeEnum.GameObjectsTransform)
				{
					throw new ArgumentOutOfRangeException("PointsModeEnum");
				}
				Transform transform = (pointGO.PointTransform != null) ? pointGO.PointTransform : pointGO.Curve.transform;
				Vector3 controlFirst = transform.InverseTransformVector(pointGO.ControlFirstLocalTransformed);
				Vector3 controlSecond = transform.InverseTransformVector(pointGO.ControlSecondLocalTransformed);
				bgcurvePoint = new BGCurvePoint(pointGO.Curve, pointGO.PointTransform, pointGO.PositionLocal, pointGO.ControlType, controlFirst, controlSecond, false);
			}
			else
			{
				bgcurvePoint = new BGCurvePoint(pointGO.Curve, pointGO.PointTransform, pointGO.PositionLocal, pointGO.ControlType, pointGO.ControlFirstLocal, pointGO.ControlSecondLocal, false);
			}
			if (pointGO.Curve.FieldsCount > 0)
			{
				bgcurvePoint.PrivateValuesForFields = pointGO.PrivateValuesForFields;
			}
			return bgcurvePoint;
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x000D3AF8 File Offset: 0x000D1CF8
		private void FireFinalEvent()
		{
			bool hasChanged = base.transform.hasChanged;
			bool flag = this.forceChangedEventMode == BGCurve.ForceChangedEventModeEnum.EditorAndRuntime;
			if (!hasChanged && this.immediateChangeEvents && !flag)
			{
				return;
			}
			if (this.pointsMode == BGCurve.PointsModeEnum.GameObjectsTransform)
			{
				BGCurvePointGO[] array = (BGCurvePointGO[])this.Points;
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					Transform transform = array[i].gameObject.transform;
					if (transform.hasChanged)
					{
						transform.hasChanged = false;
						this.changed = true;
						this.lastEventType = BGCurveChangedArgs.ChangeTypeEnum.Points;
						this.lastEventMessage = "point position is changed";
					}
				}
			}
			if (this.pointsWithTransforms != null)
			{
				int count = this.pointsWithTransforms.Count;
				if (count > 0)
				{
					BGCurvePointI[] array2 = this.Points;
					int num2 = array2.Length;
					for (int j = 0; j < count; j++)
					{
						int num3 = this.pointsWithTransforms[j];
						if (num3 < num2)
						{
							Transform pointTransform = array2[num3].PointTransform;
							if (!(pointTransform == null) && pointTransform.hasChanged)
							{
								pointTransform.hasChanged = false;
								this.changed = true;
								this.lastEventType = BGCurveChangedArgs.ChangeTypeEnum.Points;
								this.lastEventMessage = "point position is changed";
							}
						}
					}
				}
			}
			if (!hasChanged && !this.changed && !flag)
			{
				return;
			}
			if (this.changed)
			{
				this.FireChange(this.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this, this.lastEventType, this.lastEventMessage) : null, true, null);
			}
			else if (hasChanged)
			{
				this.FireChange(this.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.CurveTransform, "transform is changed") : null, true, null);
			}
			else
			{
				this.FireChange(this.UseEventsArgs ? BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Curve, "forced update") : null, true, null);
			}
			base.transform.hasChanged = (this.changed = false);
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x000D3CB8 File Offset: 0x000D1EB8
		private void AddFields(BGCurve.PointsModeEnum pointsMode, BGCurvePointI[] addedPoints)
		{
			foreach (BGCurvePointI point in addedPoints)
			{
				BGCurvePoint.FieldsValues fieldsValues = this.GetFieldsValues(point, pointsMode);
				BGCurvePointField[] array = this.fields;
				for (int j = 0; j < array.Length; j++)
				{
					BGCurvePoint.PrivateFieldAdded(array[j], fieldsValues);
				}
			}
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x000D3D08 File Offset: 0x000D1F08
		private BGCurvePointField AddField(string name, BGCurvePointField.TypeEnum type, Func<BGCurvePointField> provider = null)
		{
			BGCurvePointField.CheckName(this, name, true);
			this.FireBeforeChange("add a field");
			BGCurvePointField bgcurvePointField = (provider == null) ? base.gameObject.AddComponent<BGCurvePointField>() : provider();
			bgcurvePointField.hideFlags = HideFlags.HideInInspector;
			bgcurvePointField.Init(this, name, type);
			this.fields = BGCurve.Insert<BGCurvePointField>(this.fields, this.fields.Length, bgcurvePointField);
			this.PrivateUpdateFieldsValuesIndexes();
			if (this.PointsCount > 0)
			{
				foreach (BGCurvePointI point in this.Points)
				{
					BGCurvePoint.PrivateFieldAdded(bgcurvePointField, this.GetFieldsValues(point, this.pointsMode));
				}
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Fields, "add a field"), false, null);
			return bgcurvePointField;
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x000D3DBC File Offset: 0x000D1FBC
		private void DeleteField(BGCurvePointField field, Action<BGCurvePointField> destroyer = null)
		{
			int num = BGCurve.IndexOf<BGCurvePointField>(this.fields, field);
			if (num < 0 || num >= this.fields.Length)
			{
				throw new UnityException("Unable to remove a fields. Invalid index: " + num.ToString());
			}
			int indexOfField = this.IndexOfFieldValue(field.FieldName);
			this.FireBeforeChange("delete a field");
			this.fields = BGCurve.Remove<BGCurvePointField>(this.fields, num);
			this.PrivateUpdateFieldsValuesIndexes();
			if (this.PointsCount > 0)
			{
				foreach (BGCurvePointI point in this.Points)
				{
					BGCurvePoint.PrivateFieldDeleted(field, indexOfField, this.GetFieldsValues(point, this.pointsMode));
				}
			}
			if (destroyer == null)
			{
				BGCurve.DestroyIt(field);
			}
			else
			{
				destroyer(field);
			}
			this.FireChange(BGCurveChangedArgs.GetInstance(this, BGCurveChangedArgs.ChangeTypeEnum.Fields, "delete a field"), false, null);
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x000D3E8C File Offset: 0x000D208C
		private void CachePointsWithTransforms()
		{
			if (this.pointsWithTransforms != null)
			{
				this.pointsWithTransforms.Clear();
			}
			BGCurvePointI[] array = this.Points;
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				if (array[i].PointTransform != null)
				{
					if (this.pointsWithTransforms == null)
					{
						this.pointsWithTransforms = new List<int>();
					}
					this.pointsWithTransforms.Add(i);
				}
			}
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x0004B7DA File Offset: 0x000499DA
		public static void DestroyIt(UnityEngine.Object obj)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(obj);
				return;
			}
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x04000C03 RID: 3075
		public const float Version = 1.25f;

		// Token: 0x04000C04 RID: 3076
		public const float Epsilon = 1E-05f;

		// Token: 0x04000C05 RID: 3077
		public const float MinSnapDistance = 0.1f;

		// Token: 0x04000C06 RID: 3078
		public const float MaxSnapDistance = 100f;

		// Token: 0x04000C07 RID: 3079
		public const string MethodAddPoint = "AddPoint";

		// Token: 0x04000C08 RID: 3080
		public const string MethodDeletePoint = "Delete";

		// Token: 0x04000C09 RID: 3081
		public const string MethodSetPointsNames = "SetPointsNames";

		// Token: 0x04000C0A RID: 3082
		public const string MethodAddField = "AddField";

		// Token: 0x04000C0B RID: 3083
		public const string MethodDeleteField = "DeleteField";

		// Token: 0x04000C0C RID: 3084
		public const string MethodConvertPoints = "ConvertPoints";

		// Token: 0x04000C0D RID: 3085
		public const string EventClosed = "closed is changed";

		// Token: 0x04000C0E RID: 3086
		public const string EventSnapType = "snapType is changed";

		// Token: 0x04000C0F RID: 3087
		public const string EventSnapAxis = "snapAxis is changed";

		// Token: 0x04000C10 RID: 3088
		public const string EventSnapDistance = "snapDistance is changed";

		// Token: 0x04000C11 RID: 3089
		public const string EventSnapTrigger = "snapTriggerInteraction is changed";

		// Token: 0x04000C12 RID: 3090
		public const string EventSnapBackfaces = "snapToBackFaces is changed";

		// Token: 0x04000C13 RID: 3091
		public const string EventSnapLayerMask = "snapLayerMask is changed";

		// Token: 0x04000C14 RID: 3092
		public const string EventSnapMonitoring = "snapMonitoring is changed";

		// Token: 0x04000C15 RID: 3093
		public const string EventAddField = "add a field";

		// Token: 0x04000C16 RID: 3094
		public const string EventDeleteField = "delete a field";

		// Token: 0x04000C17 RID: 3095
		public const string EventFieldName = "field name is changed";

		// Token: 0x04000C18 RID: 3096
		public const string Event2D = "2d mode is changed";

		// Token: 0x04000C19 RID: 3097
		public const string EventForceUpdate = "force update is changed";

		// Token: 0x04000C1A RID: 3098
		public const string EventPointsMode = "points mode is changed";

		// Token: 0x04000C1B RID: 3099
		public const string EventClearAllPoints = "clear all points";

		// Token: 0x04000C1C RID: 3100
		public const string EventAddPoint = "add a point";

		// Token: 0x04000C1D RID: 3101
		public const string EventAddPoints = "add points";

		// Token: 0x04000C1E RID: 3102
		public const string EventDeletePoints = "delete points";

		// Token: 0x04000C1F RID: 3103
		public const string EventSwapPoints = "swap points";

		// Token: 0x04000C20 RID: 3104
		public const string EventReversePoints = "reverse points";

		// Token: 0x04000C21 RID: 3105
		public const string EventTransaction = "changes in transaction";

		// Token: 0x04000C22 RID: 3106
		public const string EventTransform = "transform is changed";

		// Token: 0x04000C23 RID: 3107
		public const string EventForcedUpdate = "forced update";

		// Token: 0x04000C24 RID: 3108
		public const string EventPointPosition = "point position is changed";

		// Token: 0x04000C25 RID: 3109
		public const string EventPointTransform = "point transform is changed";

		// Token: 0x04000C26 RID: 3110
		public const string EventPointControl = "point control is changed";

		// Token: 0x04000C27 RID: 3111
		public const string EventPointControlType = "point control type is changed";

		// Token: 0x04000C28 RID: 3112
		public const string EventPointField = "point field value is changed";

		// Token: 0x04000C29 RID: 3113
		private static readonly RaycastHit[] raycastHitArray = new RaycastHit[50];

		// Token: 0x04000C2A RID: 3114
		private static readonly BGCurvePointI[] pointArray = new BGCurvePointI[1];

		// Token: 0x04000C2B RID: 3115
		private static readonly List<BGCurvePointI> pointsList = new List<BGCurvePointI>();

		// Token: 0x04000C2C RID: 3116
		private static readonly List<int> pointsIndexesList = new List<int>();

		// Token: 0x04000C2F RID: 3119
		[Tooltip("2d Mode for a curve. In 2d mode, only 2 coordinates matter, the third will always be 0 (including controls). Handles in Editor will also be switched to 2d mode")]
		[SerializeField]
		private BGCurve.Mode2DEnum mode2D;

		// Token: 0x04000C30 RID: 3120
		[Tooltip("If curve is closed")]
		[SerializeField]
		private bool closed;

		// Token: 0x04000C31 RID: 3121
		[SerializeField]
		private BGCurvePoint[] points = new BGCurvePoint[0];

		// Token: 0x04000C32 RID: 3122
		[SerializeField]
		private BGCurvePointComponent[] pointsComponents = new BGCurvePointComponent[0];

		// Token: 0x04000C33 RID: 3123
		[SerializeField]
		private BGCurvePointGO[] pointsGameObjects = new BGCurvePointGO[0];

		// Token: 0x04000C34 RID: 3124
		[SerializeField]
		private BGCurvePointField[] fields = new BGCurvePointField[0];

		// Token: 0x04000C35 RID: 3125
		[Tooltip("Snap type. A collider should exists for points to snap to.\r\n 1) Off - snaping is off\r\n 2) Points - only curve's points will be snapped.\r\n 3) Curve - both curve's points and split points will be snapped. With 'Curve' mode Base Math type gives better results, than Adaptive Math, cause snapping occurs after approximation.Also, 'Curve' mode can add a huge overhead if you are changing curve's points at runtime.")]
		[SerializeField]
		private BGCurve.SnapTypeEnum snapType;

		// Token: 0x04000C36 RID: 3126
		[Tooltip("Axis for snapping points")]
		[SerializeField]
		private BGCurve.SnapAxisEnum snapAxis = BGCurve.SnapAxisEnum.Y;

		// Token: 0x04000C37 RID: 3127
		[Tooltip("Snapping distance.")]
		[SerializeField]
		[Range(0.1f, 100f)]
		private float snapDistance = 10f;

		// Token: 0x04000C38 RID: 3128
		[Tooltip("Layer mask for snapping")]
		[SerializeField]
		private LayerMask snapLayerMask = -1;

		// Token: 0x04000C39 RID: 3129
		[Tooltip("Should snapping takes triggers into account")]
		[SerializeField]
		private QueryTriggerInteraction snapTriggerInteraction;

		// Token: 0x04000C3A RID: 3130
		[Tooltip("Should snapping takes backfaces of colliders into account")]
		[SerializeField]
		private bool snapToBackFaces;

		// Token: 0x04000C3B RID: 3131
		[Tooltip("Should curve monitor surrounding environment every frame. This is super costly in terms of performance (especially for Curve snap mode)")]
		[SerializeField]
		private bool snapMonitoring;

		// Token: 0x04000C3C RID: 3132
		[Tooltip("Event mode for runtime")]
		[SerializeField]
		private BGCurve.EventModeEnum eventMode;

		// Token: 0x04000C3D RID: 3133
		[Tooltip("Points mode, how points are stored. \r\n 1) Inline - points stored inlined with the curve's component.\r\n 2) Component - points are stored as MonoBehaviour scripts attached to the curve's GameObject.\r\n 3) GameObject - points are stored as MonoBehaviour scripts attached to separate GameObject for each point.")]
		[SerializeField]
		private BGCurve.PointsModeEnum pointsMode;

		// Token: 0x04000C3E RID: 3134
		[Tooltip("Force firing of Changed event. This can be useful if you use Unity's Animation. Do not use it unless you really need it.")]
		[SerializeField]
		private BGCurve.ForceChangedEventModeEnum forceChangedEventMode;

		// Token: 0x04000C3F RID: 3135
		private int transactionLevel;

		// Token: 0x04000C40 RID: 3136
		private List<BGCurveChangedArgs> changeList;

		// Token: 0x04000C41 RID: 3137
		private BGCurve.FieldsTree fieldsTree;

		// Token: 0x04000C42 RID: 3138
		private bool changed;

		// Token: 0x04000C43 RID: 3139
		private bool immediateChangeEvents;

		// Token: 0x04000C44 RID: 3140
		private BGCurve.EventModeEnum eventModeOld;

		// Token: 0x04000C45 RID: 3141
		private BGCurveChangedArgs.ChangeTypeEnum lastEventType;

		// Token: 0x04000C46 RID: 3142
		private string lastEventMessage;

		// Token: 0x04000C47 RID: 3143
		private List<int> pointsWithTransforms;

		// Token: 0x0200017D RID: 381
		public enum Mode2DEnum
		{
			// Token: 0x04000C4A RID: 3146
			Off,
			// Token: 0x04000C4B RID: 3147
			XY,
			// Token: 0x04000C4C RID: 3148
			XZ,
			// Token: 0x04000C4D RID: 3149
			YZ
		}

		// Token: 0x0200017E RID: 382
		public enum SnapTypeEnum
		{
			// Token: 0x04000C4F RID: 3151
			Off,
			// Token: 0x04000C50 RID: 3152
			Points,
			// Token: 0x04000C51 RID: 3153
			Curve
		}

		// Token: 0x0200017F RID: 383
		public enum SnapAxisEnum
		{
			// Token: 0x04000C53 RID: 3155
			X,
			// Token: 0x04000C54 RID: 3156
			Y,
			// Token: 0x04000C55 RID: 3157
			Z
		}

		// Token: 0x02000180 RID: 384
		public enum EventModeEnum
		{
			// Token: 0x04000C57 RID: 3159
			Update,
			// Token: 0x04000C58 RID: 3160
			LateUpdate,
			// Token: 0x04000C59 RID: 3161
			NoEvents
		}

		// Token: 0x02000181 RID: 385
		public enum ForceChangedEventModeEnum
		{
			// Token: 0x04000C5B RID: 3163
			Off,
			// Token: 0x04000C5C RID: 3164
			EditorOnly,
			// Token: 0x04000C5D RID: 3165
			EditorAndRuntime
		}

		// Token: 0x02000182 RID: 386
		public enum PointsModeEnum
		{
			// Token: 0x04000C5F RID: 3167
			Inlined,
			// Token: 0x04000C60 RID: 3168
			Components,
			// Token: 0x04000C61 RID: 3169
			GameObjectsNoTransform,
			// Token: 0x04000C62 RID: 3170
			GameObjectsTransform
		}

		// Token: 0x02000183 RID: 387
		// (Invoke) Token: 0x06000D2C RID: 3372
		public delegate void IterationCallback(BGCurvePointI point, int index, int count);

		// Token: 0x02000184 RID: 388
		private sealed class FieldsTree
		{
			// Token: 0x06000D2F RID: 3375 RVA: 0x0004B81D File Offset: 0x00049A1D
			public bool Comply(BGCurvePointField[] fields)
			{
				if (fields != null)
				{
					return this.fieldName2Index.Count == fields.Length;
				}
				return this.fieldName2Index.Count == 0;
			}

			// Token: 0x06000D30 RID: 3376 RVA: 0x000D3F88 File Offset: 0x000D2188
			public int GetIndex(string name)
			{
				int result;
				if (this.fieldName2Index.TryGetValue(name, out result))
				{
					return result;
				}
				throw new UnityException("Can not find a index of field " + name);
			}

			// Token: 0x06000D31 RID: 3377 RVA: 0x000D3FB8 File Offset: 0x000D21B8
			public void Update(BGCurvePointField[] fields)
			{
				this.fieldName2Index.Clear();
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				int i = 0;
				while (i < fields.Length)
				{
					BGCurvePointField bgcurvePointField = fields[i];
					BGCurvePointField.TypeEnum type = bgcurvePointField.Type;
					int value;
					if (type <= BGCurvePointField.TypeEnum.Quaternion)
					{
						switch (type)
						{
						case BGCurvePointField.TypeEnum.Bool:
							value = num++;
							break;
						case BGCurvePointField.TypeEnum.Int:
							value = num2++;
							break;
						case BGCurvePointField.TypeEnum.Float:
							value = num3++;
							break;
						case BGCurvePointField.TypeEnum.String:
							value = num7++;
							break;
						default:
							switch (type)
							{
							case BGCurvePointField.TypeEnum.Vector3:
								value = num4++;
								break;
							case BGCurvePointField.TypeEnum.Bounds:
								value = num5++;
								break;
							case BGCurvePointField.TypeEnum.Color:
								value = num6++;
								break;
							case BGCurvePointField.TypeEnum.Quaternion:
								value = num8++;
								break;
							default:
								goto IL_160;
							}
							break;
						}
					}
					else
					{
						switch (type)
						{
						case BGCurvePointField.TypeEnum.AnimationCurve:
							value = num9++;
							break;
						case BGCurvePointField.TypeEnum.GameObject:
							value = num10++;
							break;
						case BGCurvePointField.TypeEnum.Component:
							value = num11++;
							break;
						default:
							switch (type)
							{
							case BGCurvePointField.TypeEnum.BGCurve:
								value = num12++;
								break;
							case BGCurvePointField.TypeEnum.BGCurvePointComponent:
								value = num13++;
								break;
							case BGCurvePointField.TypeEnum.BGCurvePointGO:
								value = num14++;
								break;
							default:
								goto IL_160;
							}
							break;
						}
					}
					this.fieldName2Index[bgcurvePointField.FieldName] = value;
					i++;
					continue;
					IL_160:
					throw new UnityException("Unknown type " + bgcurvePointField.Type.ToString());
				}
			}

			// Token: 0x04000C63 RID: 3171
			private readonly Dictionary<string, int> fieldName2Index = new Dictionary<string, int>();
		}
	}
}
