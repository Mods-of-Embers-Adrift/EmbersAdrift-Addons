using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001CA RID: 458
	[RequireComponent(typeof(BGCcCursor))]
	public abstract class BGCcWithCursor : BGCc
	{
		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x0004DC13 File Offset: 0x0004BE13
		// (set) Token: 0x06001078 RID: 4216 RVA: 0x0004DC35 File Offset: 0x0004BE35
		public BGCcCursor Cursor
		{
			get
			{
				if (this.cursor == null)
				{
					this.cursor = base.GetParent<BGCcCursor>();
				}
				return this.cursor;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				this.cursor = value;
				base.SetParent(value);
			}
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06001079 RID: 4217 RVA: 0x0004DC4F File Offset: 0x0004BE4F
		public override string Error
		{
			get
			{
				if (!(this.Cursor == null))
				{
					return null;
				}
				return "Cursor is null";
			}
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x000DF620 File Offset: 0x000DD820
		public Quaternion LerpQuaternion(string fieldName, int currentSection = -1)
		{
			int i;
			int i2;
			float t = this.GetT(out i, out i2, currentSection);
			Quaternion quaternion = base.Curve[i].GetQuaternion(fieldName);
			Quaternion quaternion2 = base.Curve[i2].GetQuaternion(fieldName);
			if (quaternion.x == 0f && quaternion.y == 0f && quaternion.z == 0f && quaternion.w == 0f)
			{
				quaternion = Quaternion.identity;
			}
			if (quaternion2.x == 0f && quaternion2.y == 0f && quaternion2.z == 0f && quaternion2.w == 0f)
			{
				quaternion2 = Quaternion.identity;
			}
			Quaternion quaternion3 = Quaternion.Lerp(quaternion, quaternion2, t);
			if (!float.IsNaN(quaternion3.x) && !float.IsNaN(quaternion3.y) && !float.IsNaN(quaternion3.z) && !float.IsNaN(quaternion3.w))
			{
				return quaternion3;
			}
			return Quaternion.identity;
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x000DF724 File Offset: 0x000DD924
		public Vector3 LerpVector(string name, int currentSection = -1)
		{
			int i;
			int i2;
			float t = this.GetT(out i, out i2, currentSection);
			Vector3 vector = base.Curve[i].GetVector3(name);
			Vector3 vector2 = base.Curve[i2].GetVector3(name);
			return Vector3.Lerp(vector, vector2, t);
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x000DF76C File Offset: 0x000DD96C
		public float GetT(out int indexFrom, out int indexTo, int currentSection = -1)
		{
			BGCurveBaseMath math = this.Cursor.Math.Math;
			float distance = this.Cursor.Distance;
			this.GetFromToIndexes(out indexFrom, out indexTo, currentSection);
			BGCurveBaseMath.SectionInfo sectionInfo = math[indexFrom];
			return (distance - sectionInfo.DistanceFromStartToOrigin) / sectionInfo.Distance;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0004DC66 File Offset: 0x0004BE66
		protected void GetFromToIndexes(out int indexFrom, out int indexTo, int currentSection = -1)
		{
			indexFrom = ((currentSection < 0) ? this.Cursor.CalculateSectionIndex() : currentSection);
			indexTo = ((indexFrom == base.Curve.PointsCount - 1) ? 0 : (indexFrom + 1));
		}

		// Token: 0x04000DCA RID: 3530
		private BGCcCursor cursor;
	}
}
