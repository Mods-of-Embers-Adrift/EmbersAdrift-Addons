using System;
using SoL.Game.Player;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000271 RID: 625
	public class DebugPositionPair : MonoBehaviour
	{
		// Token: 0x060013B1 RID: 5041 RVA: 0x0004FDB5 File Offset: 0x0004DFB5
		private void Awake()
		{
			if (Application.isPlaying)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x0004FDCA File Offset: 0x0004DFCA
		private void Go()
		{
			this.MoveObject(this.m_first);
			this.MoveObject(this.m_second);
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x000F773C File Offset: 0x000F593C
		private void MoveObject(DebugPositionPair.DebugPositionObject obj)
		{
			if (obj == null || string.IsNullOrEmpty(obj.Position))
			{
				return;
			}
			DebugLocation debugLocation = new DebugLocation(obj.Position);
			obj.Obj.gameObject.transform.position = debugLocation.Position;
			obj.Obj.gameObject.transform.rotation = debugLocation.Rotation;
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x000F77A0 File Offset: 0x000F59A0
		private void CanFirstReachSecond()
		{
			if (this.m_first != null && this.m_first.Obj && this.m_second != null && this.m_second.Obj)
			{
				this.CanReach(this.m_second.Obj.transform.position, this.m_first.Obj.transform.position);
			}
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x000F7814 File Offset: 0x000F5A14
		private void CanSecondReachFirst()
		{
			if (this.m_first != null && this.m_first.Obj && this.m_second != null && this.m_second.Obj)
			{
				this.CanReach(this.m_first.Obj.transform.position, this.m_second.Obj.transform.position);
			}
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0004475B File Offset: 0x0004295B
		private void CanReach(Vector3 targetPos, Vector3 sourcePos)
		{
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x000F7888 File Offset: 0x000F5A88
		private void OnDrawGizmosSelected()
		{
			if (this.m_first.Obj && this.m_second.Obj)
			{
				Gizmos.DrawLine(this.m_first.Obj.transform.position, this.m_second.Obj.transform.position);
			}
		}

		// Token: 0x04001BF4 RID: 7156
		[SerializeField]
		private DebugPositionPair.DebugPositionObject m_first;

		// Token: 0x04001BF5 RID: 7157
		[SerializeField]
		private DebugPositionPair.DebugPositionObject m_second;

		// Token: 0x02000272 RID: 626
		[Serializable]
		private class DebugPositionObject
		{
			// Token: 0x04001BF6 RID: 7158
			public string Position;

			// Token: 0x04001BF7 RID: 7159
			public GameObject Obj;
		}
	}
}
