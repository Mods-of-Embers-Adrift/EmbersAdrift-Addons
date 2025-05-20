using System;
using SoL.Game;
using SoL.GameCamera;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002FA RID: 762
	public class UpperLineOfSightTarget : GameEntityComponent
	{
		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x00051256 File Offset: 0x0004F456
		public bool UseFullPosition
		{
			get
			{
				return this.m_useFullPosition;
			}
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0005125E File Offset: 0x0004F45E
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.UpperLineOfSightTarget = this;
				this.m_headCollider = base.gameObject.GetComponent<Collider>();
				this.RefreshHeadCollider();
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x000FCBB8 File Offset: 0x000FADB8
		public void RefreshHeadCollider()
		{
			if (this.m_headCollider && base.GameEntity && base.GameEntity == LocalPlayer.GameEntity)
			{
				this.m_headCollider.enabled = (CameraManager.ActiveType != ActiveCameraTypes.FirstPerson);
			}
		}

		// Token: 0x04001D9D RID: 7581
		[SerializeField]
		private bool m_useFullPosition;

		// Token: 0x04001D9E RID: 7582
		[NonSerialized]
		private Collider m_headCollider;
	}
}
