using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD5 RID: 3541
	[Serializable]
	internal class ComparisonValues
	{
		// Token: 0x0600696B RID: 26987 RVA: 0x00086AF3 File Offset: 0x00084CF3
		internal ComparisonValues(Vector3 pos, Vector3 rot, Vector3 size)
		{
			this.PosDelta = this.GetRoundedVector(pos);
			this.RotDelta = this.GetRoundedVector(rot);
			this.SizeDelta = this.GetRoundedVector(size);
		}

		// Token: 0x0600696C RID: 26988 RVA: 0x00086B22 File Offset: 0x00084D22
		private Vector3 GetRoundedVector(Vector3 input)
		{
			return new Vector3(this.GetRoundedFloat(input.x), this.GetRoundedFloat(input.y), this.GetRoundedFloat(input.z));
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x00086B4D File Offset: 0x00084D4D
		private Quaternion GetRoundedQuaternion(Quaternion input)
		{
			return new Quaternion(this.GetRoundedFloat(input.x), this.GetRoundedFloat(input.y), this.GetRoundedFloat(input.z), this.GetRoundedFloat(input.w));
		}

		// Token: 0x0600696E RID: 26990 RVA: 0x00217A28 File Offset: 0x00215C28
		private float GetRoundedFloat(float input)
		{
			float num = 1000f;
			float num2 = (float)Mathf.FloorToInt(input * num) / num;
			float num3 = 1f / num;
			if (Mathf.Abs(num2) > num3)
			{
				return num2;
			}
			return 0f;
		}

		// Token: 0x04005BDD RID: 23517
		private const int kDigits = 3;

		// Token: 0x04005BDE RID: 23518
		public Vector3 PosDelta;

		// Token: 0x04005BDF RID: 23519
		public Vector3 RotDelta;

		// Token: 0x04005BE0 RID: 23520
		public Vector3 SizeDelta;
	}
}
