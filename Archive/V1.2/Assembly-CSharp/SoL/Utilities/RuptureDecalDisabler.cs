using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002D8 RID: 728
	public class RuptureDecalDisabler : MonoBehaviour
	{
		// Token: 0x06001504 RID: 5380 RVA: 0x00050AF3 File Offset: 0x0004ECF3
		private void Start()
		{
			if (GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (LocalPlayer.IsInitialized)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00050B28 File Offset: 0x0004ED28
		private void OnDestroy()
		{
			base.CancelInvoke("UpdateInternal");
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x00050B35 File Offset: 0x0004ED35
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			if (this && base.gameObject.activeInHierarchy)
			{
				base.InvokeRepeating("UpdateInternal", 2f, 5f);
			}
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x000FBE94 File Offset: 0x000FA094
		private void UpdateInternal()
		{
			if (this.m_driftBounds == null || this.m_driftBounds.Length == 0 || !LocalPlayer.GameEntity)
			{
				for (int i = 0; i < this.m_driftBounds.Length; i++)
				{
					if (this.m_driftBounds[i].Decal)
					{
						this.m_driftBounds[i].Decal.gameObject.SetActive(false);
					}
				}
				return;
			}
			Vector3 position = LocalPlayer.GameEntity.gameObject.transform.position;
			for (int j = 0; j < this.m_driftBounds.Length; j++)
			{
				if (this.m_driftBounds[j].Decal)
				{
					this.m_driftBounds[j].Decal.gameObject.SetActive(this.m_driftBounds[j].Volume && this.m_driftBounds[j].Volume.IsWithinBounds(position));
				}
			}
		}

		// Token: 0x04001D4B RID: 7499
		[SerializeField]
		private RuptureDecalDisabler.DriftBounds[] m_driftBounds;

		// Token: 0x020002D9 RID: 729
		[Serializable]
		private class DriftBounds
		{
			// Token: 0x1700051D RID: 1309
			// (get) Token: 0x06001509 RID: 5385 RVA: 0x00050B72 File Offset: 0x0004ED72
			public GameObject Decal
			{
				get
				{
					return this.m_decal;
				}
			}

			// Token: 0x1700051E RID: 1310
			// (get) Token: 0x0600150A RID: 5386 RVA: 0x00050B7A File Offset: 0x0004ED7A
			public BaseVolumeOverride Volume
			{
				get
				{
					return this.m_volume;
				}
			}

			// Token: 0x04001D4C RID: 7500
			[SerializeField]
			private GameObject m_decal;

			// Token: 0x04001D4D RID: 7501
			[SerializeField]
			private BaseVolumeOverride m_volume;
		}
	}
}
