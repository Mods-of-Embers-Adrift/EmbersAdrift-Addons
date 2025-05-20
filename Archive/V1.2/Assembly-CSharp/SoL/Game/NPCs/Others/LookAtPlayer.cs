using System;
using RootMotion.FinalIK;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.NPCs.Others
{
	// Token: 0x02000834 RID: 2100
	public class LookAtPlayer : MonoBehaviour
	{
		// Token: 0x06003CCC RID: 15564 RVA: 0x000692EB File Offset: 0x000674EB
		private void Start()
		{
			if (GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			if (LocalPlayer.GameEntity == null)
			{
				LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
				return;
			}
			this.AssignLookIkTarget();
		}

		// Token: 0x06003CCD RID: 15565 RVA: 0x00069321 File Offset: 0x00067521
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.AssignLookIkTarget();
		}

		// Token: 0x06003CCE RID: 15566 RVA: 0x00181154 File Offset: 0x0017F354
		private void AssignLookIkTarget()
		{
			if (this.m_lookIk && LocalPlayer.GameEntity)
			{
				if (LookAtPlayer.m_lookAtGameObject == null)
				{
					if (this.m_lookAtCamera)
					{
						LookAtPlayer.m_lookAtGameObject = ClientGameManager.MainCamera.gameObject;
					}
					else
					{
						LookAtPlayer.m_lookAtGameObject = new GameObject("OtherLookAtTarget");
						LookAtPlayer.m_lookAtGameObject.transform.SetParent(LocalPlayer.GameEntity.gameObject.transform);
						LookAtPlayer.m_lookAtGameObject.transform.localPosition = Vector3.up * 1.8f;
					}
				}
				this.m_lookIk.solver.target = LookAtPlayer.m_lookAtGameObject.transform;
			}
		}

		// Token: 0x04003BA2 RID: 15266
		private static GameObject m_lookAtGameObject;

		// Token: 0x04003BA3 RID: 15267
		[SerializeField]
		private bool m_lookAtCamera;

		// Token: 0x04003BA4 RID: 15268
		[SerializeField]
		private LookAtIK m_lookIk;
	}
}
