using System;
using SoL.Game.SkyDome;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200056D RID: 1389
	public class EmberHighlightObject : MonoBehaviour
	{
		// Token: 0x06002ADE RID: 10974 RVA: 0x00144D84 File Offset: 0x00142F84
		private void Awake()
		{
			if (GameManager.IsServer || !this.m_particleSystem)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.UpdateDistances();
			this.m_particleSystem.Stop(true);
			this.m_defaultScale = this.m_particleSystem.gameObject.transform.localScale;
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x00144DE0 File Offset: 0x00142FE0
		private void Update()
		{
			if (!this.m_particleSystem)
			{
				return;
			}
			if (SkyDomeManager.InEmberHighlightZone)
			{
				if (this.m_scale && ClientGameManager.MainCamera)
				{
					float sqrMagnitude = (ClientGameManager.MainCamera.gameObject.transform.position - this.m_particleSystem.gameObject.transform.position).sqrMagnitude;
					Vector3 vector = this.m_defaultScale;
					if (sqrMagnitude > this.m_scaleDistanceRangeSqr.y)
					{
						vector *= this.m_maxScaleMultiplier;
					}
					else if (sqrMagnitude > this.m_scaleDistanceRangeSqr.x)
					{
						float t = (sqrMagnitude - this.m_scaleDistanceRangeSqr.x) / (this.m_scaleDistanceRangeSqr.y - this.m_scaleDistanceRangeSqr.x);
						vector *= Mathf.Lerp(1f, this.m_maxScaleMultiplier, t);
					}
					this.m_particleSystem.gameObject.transform.localScale = vector;
				}
				if (!this.m_particleSystem.isPlaying)
				{
					this.m_particleSystem.Play(true);
					return;
				}
			}
			else if (this.m_particleSystem.isPlaying)
			{
				this.m_particleSystem.Stop(true, this.m_stopBehavior);
			}
		}

		// Token: 0x06002AE0 RID: 10976 RVA: 0x0005DB14 File Offset: 0x0005BD14
		private void UpdateDistances()
		{
			this.m_scaleDistanceRangeSqr = this.m_scaleDistanceRange * this.m_scaleDistanceRange;
		}

		// Token: 0x04002B1E RID: 11038
		[SerializeField]
		private ParticleSystem m_particleSystem;

		// Token: 0x04002B1F RID: 11039
		[SerializeField]
		private ParticleSystemStopBehavior m_stopBehavior = ParticleSystemStopBehavior.StopEmitting;

		// Token: 0x04002B20 RID: 11040
		[SerializeField]
		private bool m_scale;

		// Token: 0x04002B21 RID: 11041
		[SerializeField]
		private Vector2 m_scaleDistanceRange = new Vector2(50f, 150f);

		// Token: 0x04002B22 RID: 11042
		[SerializeField]
		private float m_maxScaleMultiplier = 4f;

		// Token: 0x04002B23 RID: 11043
		private Vector2 m_scaleDistanceRangeSqr;

		// Token: 0x04002B24 RID: 11044
		private Vector3 m_defaultScale;
	}
}
