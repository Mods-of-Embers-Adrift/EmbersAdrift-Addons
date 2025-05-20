using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200019C RID: 412
	public class BGCurvePointComponent : MonoBehaviour, BGCurvePointI
	{
		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000E17 RID: 3607 RVA: 0x0004C280 File Offset: 0x0004A480
		public BGCurve Curve
		{
			get
			{
				return this.point.Curve;
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x0004C28D File Offset: 0x0004A48D
		// (set) Token: 0x06000E19 RID: 3609 RVA: 0x0004C29A File Offset: 0x0004A49A
		public Vector3 PositionLocal
		{
			get
			{
				return this.point.PositionLocal;
			}
			set
			{
				this.point.PositionLocal = value;
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000E1A RID: 3610 RVA: 0x0004C2A8 File Offset: 0x0004A4A8
		// (set) Token: 0x06000E1B RID: 3611 RVA: 0x0004C2B5 File Offset: 0x0004A4B5
		public Vector3 PositionLocalTransformed
		{
			get
			{
				return this.point.PositionLocalTransformed;
			}
			set
			{
				this.point.PositionLocalTransformed = value;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000E1C RID: 3612 RVA: 0x0004C2C3 File Offset: 0x0004A4C3
		// (set) Token: 0x06000E1D RID: 3613 RVA: 0x0004C2D0 File Offset: 0x0004A4D0
		public Vector3 PositionWorld
		{
			get
			{
				return this.point.PositionWorld;
			}
			set
			{
				this.point.PositionWorld = value;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000E1E RID: 3614 RVA: 0x0004C2DE File Offset: 0x0004A4DE
		// (set) Token: 0x06000E1F RID: 3615 RVA: 0x0004C2EB File Offset: 0x0004A4EB
		public Vector3 ControlFirstLocal
		{
			get
			{
				return this.point.ControlFirstLocal;
			}
			set
			{
				this.point.ControlFirstLocal = value;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000E20 RID: 3616 RVA: 0x0004C2F9 File Offset: 0x0004A4F9
		// (set) Token: 0x06000E21 RID: 3617 RVA: 0x0004C306 File Offset: 0x0004A506
		public Vector3 ControlFirstLocalTransformed
		{
			get
			{
				return this.point.ControlFirstLocalTransformed;
			}
			set
			{
				this.point.ControlFirstLocalTransformed = value;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000E22 RID: 3618 RVA: 0x0004C314 File Offset: 0x0004A514
		// (set) Token: 0x06000E23 RID: 3619 RVA: 0x0004C321 File Offset: 0x0004A521
		public Vector3 ControlFirstWorld
		{
			get
			{
				return this.point.ControlFirstWorld;
			}
			set
			{
				this.point.ControlFirstWorld = value;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000E24 RID: 3620 RVA: 0x0004C32F File Offset: 0x0004A52F
		// (set) Token: 0x06000E25 RID: 3621 RVA: 0x0004C33C File Offset: 0x0004A53C
		public Vector3 ControlSecondLocal
		{
			get
			{
				return this.point.ControlSecondLocal;
			}
			set
			{
				this.point.ControlSecondLocal = value;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000E26 RID: 3622 RVA: 0x0004C34A File Offset: 0x0004A54A
		// (set) Token: 0x06000E27 RID: 3623 RVA: 0x0004C357 File Offset: 0x0004A557
		public Vector3 ControlSecondLocalTransformed
		{
			get
			{
				return this.point.ControlSecondLocalTransformed;
			}
			set
			{
				this.point.ControlSecondLocalTransformed = value;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000E28 RID: 3624 RVA: 0x0004C365 File Offset: 0x0004A565
		// (set) Token: 0x06000E29 RID: 3625 RVA: 0x0004C372 File Offset: 0x0004A572
		public Vector3 ControlSecondWorld
		{
			get
			{
				return this.point.ControlSecondWorld;
			}
			set
			{
				this.point.ControlSecondWorld = value;
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0004C380 File Offset: 0x0004A580
		// (set) Token: 0x06000E2B RID: 3627 RVA: 0x0004C38D File Offset: 0x0004A58D
		public BGCurvePoint.ControlTypeEnum ControlType
		{
			get
			{
				return this.point.ControlType;
			}
			set
			{
				this.point.ControlType = value;
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000E2C RID: 3628 RVA: 0x0004C39B File Offset: 0x0004A59B
		// (set) Token: 0x06000E2D RID: 3629 RVA: 0x0004C3A8 File Offset: 0x0004A5A8
		public Transform PointTransform
		{
			get
			{
				return this.point.PointTransform;
			}
			set
			{
				this.point.PointTransform = value;
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0004C3B6 File Offset: 0x0004A5B6
		public float GetFloat(string name)
		{
			return this.point.GetFloat(name);
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x0004C3C4 File Offset: 0x0004A5C4
		public bool GetBool(string name)
		{
			return this.point.GetBool(name);
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x0004C3D2 File Offset: 0x0004A5D2
		public int GetInt(string name)
		{
			return this.point.GetInt(name);
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x0004C3E0 File Offset: 0x0004A5E0
		public Vector3 GetVector3(string name)
		{
			return this.point.GetVector3(name);
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x0004C3EE File Offset: 0x0004A5EE
		public Quaternion GetQuaternion(string name)
		{
			return this.point.GetQuaternion(name);
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x0004C3FC File Offset: 0x0004A5FC
		public Bounds GetBounds(string name)
		{
			return this.point.GetBounds(name);
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0004C40A File Offset: 0x0004A60A
		public Color GetColor(string name)
		{
			return this.point.GetColor(name);
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0004C418 File Offset: 0x0004A618
		public T GetField<T>(string name)
		{
			return this.point.GetField<T>(name);
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0004C426 File Offset: 0x0004A626
		public object GetField(string name, Type type)
		{
			return this.point.GetField(name, type);
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x0004C435 File Offset: 0x0004A635
		public void SetField(string name, object value, Type type)
		{
			this.point.SetField(name, value, type);
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x0004C445 File Offset: 0x0004A645
		public void SetField<T>(string name, T value)
		{
			this.point.SetField<T>(name, value);
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x0004C454 File Offset: 0x0004A654
		public void SetFloat(string name, float value)
		{
			this.point.SetFloat(name, value);
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x0004C463 File Offset: 0x0004A663
		public void SetBool(string name, bool value)
		{
			this.point.SetBool(name, value);
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x0004C472 File Offset: 0x0004A672
		public void SetInt(string name, int value)
		{
			this.point.SetInt(name, value);
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x0004C481 File Offset: 0x0004A681
		public void SetVector3(string name, Vector3 value)
		{
			this.point.SetVector3(name, value);
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x0004C490 File Offset: 0x0004A690
		public void SetQuaternion(string name, Quaternion value)
		{
			this.point.SetQuaternion(name, value);
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x0004C49F File Offset: 0x0004A69F
		public void SetBounds(string name, Bounds value)
		{
			this.point.SetBounds(name, value);
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x0004C4AE File Offset: 0x0004A6AE
		public void SetColor(string name, Color value)
		{
			this.point.SetColor(name, value);
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x0004C4BD File Offset: 0x0004A6BD
		public BGCurvePoint Point
		{
			get
			{
				return this.point;
			}
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x0004C4C5 File Offset: 0x0004A6C5
		public void PrivateInit(BGCurvePoint point)
		{
			this.point = point;
			base.hideFlags = HideFlags.HideInInspector;
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x0004C4D5 File Offset: 0x0004A6D5
		public override string ToString()
		{
			if (this.point != null)
			{
				BGCurvePoint bgcurvePoint = this.point;
				return ((bgcurvePoint != null) ? bgcurvePoint.ToString() : null) + " as Component";
			}
			return "no data";
		}

		// Token: 0x04000CDE RID: 3294
		[SerializeField]
		private BGCurvePoint point;
	}
}
