using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x020001A0 RID: 416
	public interface BGCurvePointI
	{
		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000E82 RID: 3714
		BGCurve Curve { get; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000E83 RID: 3715
		// (set) Token: 0x06000E84 RID: 3716
		Vector3 PositionLocal { get; set; }

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000E85 RID: 3717
		// (set) Token: 0x06000E86 RID: 3718
		Vector3 PositionLocalTransformed { get; set; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000E87 RID: 3719
		// (set) Token: 0x06000E88 RID: 3720
		Vector3 PositionWorld { get; set; }

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000E89 RID: 3721
		// (set) Token: 0x06000E8A RID: 3722
		Vector3 ControlFirstLocal { get; set; }

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000E8B RID: 3723
		// (set) Token: 0x06000E8C RID: 3724
		Vector3 ControlFirstLocalTransformed { get; set; }

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000E8D RID: 3725
		// (set) Token: 0x06000E8E RID: 3726
		Vector3 ControlFirstWorld { get; set; }

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000E8F RID: 3727
		// (set) Token: 0x06000E90 RID: 3728
		Vector3 ControlSecondLocal { get; set; }

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000E91 RID: 3729
		// (set) Token: 0x06000E92 RID: 3730
		Vector3 ControlSecondLocalTransformed { get; set; }

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000E93 RID: 3731
		// (set) Token: 0x06000E94 RID: 3732
		Vector3 ControlSecondWorld { get; set; }

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000E95 RID: 3733
		// (set) Token: 0x06000E96 RID: 3734
		BGCurvePoint.ControlTypeEnum ControlType { get; set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000E97 RID: 3735
		// (set) Token: 0x06000E98 RID: 3736
		Transform PointTransform { get; set; }

		// Token: 0x06000E99 RID: 3737
		T GetField<T>(string name);

		// Token: 0x06000E9A RID: 3738
		object GetField(string name, Type type);

		// Token: 0x06000E9B RID: 3739
		float GetFloat(string name);

		// Token: 0x06000E9C RID: 3740
		bool GetBool(string name);

		// Token: 0x06000E9D RID: 3741
		int GetInt(string name);

		// Token: 0x06000E9E RID: 3742
		Vector3 GetVector3(string name);

		// Token: 0x06000E9F RID: 3743
		Quaternion GetQuaternion(string name);

		// Token: 0x06000EA0 RID: 3744
		Bounds GetBounds(string name);

		// Token: 0x06000EA1 RID: 3745
		Color GetColor(string name);

		// Token: 0x06000EA2 RID: 3746
		void SetField<T>(string name, T value);

		// Token: 0x06000EA3 RID: 3747
		void SetField(string name, object value, Type type);

		// Token: 0x06000EA4 RID: 3748
		void SetFloat(string name, float value);

		// Token: 0x06000EA5 RID: 3749
		void SetBool(string name, bool value);

		// Token: 0x06000EA6 RID: 3750
		void SetInt(string name, int value);

		// Token: 0x06000EA7 RID: 3751
		void SetVector3(string name, Vector3 value);

		// Token: 0x06000EA8 RID: 3752
		void SetQuaternion(string name, Quaternion value);

		// Token: 0x06000EA9 RID: 3753
		void SetBounds(string name, Bounds value);

		// Token: 0x06000EAA RID: 3754
		void SetColor(string name, Color value);
	}
}
