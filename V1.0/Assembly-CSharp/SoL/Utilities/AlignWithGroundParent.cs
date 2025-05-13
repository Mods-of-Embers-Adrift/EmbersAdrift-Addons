using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200023F RID: 575
	public class AlignWithGroundParent : MonoBehaviour
	{
		// Token: 0x06001300 RID: 4864 RVA: 0x0004F859 File Offset: 0x0004DA59
		private void GetAllChildren()
		{
			this.m_alignWithGrounds = base.gameObject.GetComponentsInChildren<AlignWithGround>();
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06001301 RID: 4865 RVA: 0x0004F86C File Offset: 0x0004DA6C
		private bool m_showButtons
		{
			get
			{
				return this.m_alignWithGrounds != null && this.m_alignWithGrounds.Length != 0;
			}
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x000E8A0C File Offset: 0x000E6C0C
		public void PositionAndAlign()
		{
			for (int i = 0; i < this.m_alignWithGrounds.Length; i++)
			{
				this.m_alignWithGrounds[i].PositionAndAlign();
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x000E8A3C File Offset: 0x000E6C3C
		public void Position()
		{
			for (int i = 0; i < this.m_alignWithGrounds.Length; i++)
			{
				this.m_alignWithGrounds[i].Position();
			}
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x000E8A6C File Offset: 0x000E6C6C
		public void Align()
		{
			for (int i = 0; i < this.m_alignWithGrounds.Length; i++)
			{
				this.m_alignWithGrounds[i].Align();
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000E8A9C File Offset: 0x000E6C9C
		public void RandomizeRotation()
		{
			for (int i = 0; i < this.m_alignWithGrounds.Length; i++)
			{
				this.m_alignWithGrounds[i].RandomizeRotation();
			}
		}

		// Token: 0x040010D7 RID: 4311
		public const string kButtonGroupName = "Actions";

		// Token: 0x040010D8 RID: 4312
		[SerializeField]
		private AlignWithGround[] m_alignWithGrounds;
	}
}
