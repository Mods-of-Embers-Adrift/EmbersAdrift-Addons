using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002DA RID: 730
	public class RuptureDecalGrabber : MonoBehaviour
	{
		// Token: 0x0600150C RID: 5388 RVA: 0x00050B82 File Offset: 0x0004ED82
		private void Start()
		{
			if (GameManager.IsOnline && !GameManager.IsServer && RuptureDecalAnimator.SharedMaterial && this.m_decalProjector)
			{
				this.m_decalProjector.material = RuptureDecalAnimator.SharedMaterial;
			}
		}

		// Token: 0x04001D4E RID: 7502
		[SerializeField]
		private DecalProjector m_decalProjector;
	}
}
