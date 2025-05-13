using System;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200056E RID: 1390
	public class EmberHighlightZone : MonoBehaviour
	{
		// Token: 0x06002AE2 RID: 10978 RVA: 0x00144F18 File Offset: 0x00143118
		private void Awake()
		{
			if (GameManager.IsServer || !this.m_collider)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_collider.enabled = false;
			this.m_collider.isTrigger = true;
			base.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x06002AE3 RID: 10979 RVA: 0x0005DB5C File Offset: 0x0005BD5C
		private void Start()
		{
			this.m_collider.enabled = true;
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x0005DB6A File Offset: 0x0005BD6A
		private void OnTriggerEnter(Collider other)
		{
			if (LocalPlayer.DetectionCollider && LocalPlayer.DetectionCollider.Collider == other)
			{
				SkyDomeManager.InEmberHighlightZone = true;
			}
		}

		// Token: 0x06002AE5 RID: 10981 RVA: 0x0005DB90 File Offset: 0x0005BD90
		private void OnTriggerExit(Collider other)
		{
			if (LocalPlayer.DetectionCollider && LocalPlayer.DetectionCollider.Collider == other)
			{
				SkyDomeManager.InEmberHighlightZone = false;
			}
		}

		// Token: 0x04002B25 RID: 11045
		[SerializeField]
		private Collider m_collider;
	}
}
