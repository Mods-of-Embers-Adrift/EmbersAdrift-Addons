using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002BA RID: 698
	public class RotateAroundAxis : MonoBehaviour
	{
		// Token: 0x060014AC RID: 5292 RVA: 0x000FB538 File Offset: 0x000F9738
		private void Update()
		{
			Vector3 axis;
			switch (this.m_rotationAxis)
			{
			case RotateAroundAxis.RotationAxis.X:
				axis = Vector3.right;
				break;
			case RotateAroundAxis.RotationAxis.Y:
				axis = Vector3.up;
				break;
			case RotateAroundAxis.RotationAxis.Z:
				axis = Vector3.forward;
				break;
			default:
				throw new ArgumentException("m_rotationAxis");
			}
			base.gameObject.transform.RotateAround(base.gameObject.transform.position, axis, Time.deltaTime * this.m_speed);
		}

		// Token: 0x04001CE7 RID: 7399
		[SerializeField]
		private RotateAroundAxis.RotationAxis m_rotationAxis = RotateAroundAxis.RotationAxis.Y;

		// Token: 0x04001CE8 RID: 7400
		[SerializeField]
		private float m_speed = 10f;

		// Token: 0x020002BB RID: 699
		private enum RotationAxis
		{
			// Token: 0x04001CEA RID: 7402
			X,
			// Token: 0x04001CEB RID: 7403
			Y,
			// Token: 0x04001CEC RID: 7404
			Z
		}
	}
}
