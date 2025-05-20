using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D1B RID: 3355
	public class WaterAudioData : MonoBehaviour
	{
		// Token: 0x06006507 RID: 25863 RVA: 0x00084031 File Offset: 0x00082231
		private void OnDestroy()
		{
			this.ResetAudio();
		}

		// Token: 0x06006508 RID: 25864 RVA: 0x0020B5FC File Offset: 0x002097FC
		public void InitAudio(GameEntity entity)
		{
			if (!entity || !this.m_audioSource)
			{
				return;
			}
			if (!this.m_initialized)
			{
				this.m_initialized = true;
				this.m_defaultVolume = this.m_audioSource.volume;
				this.m_defaultSpatialBlend = this.m_audioSource.spatialBlend;
				this.m_defaultParent = this.m_audioSource.gameObject.transform.parent;
			}
			if (this.m_moveAlong)
			{
				this.m_moveAlong.enabled = false;
			}
			if (this.m_splineAdjustments)
			{
				this.m_splineAdjustments.enabled = false;
			}
			if (this.m_splineObj)
			{
				this.m_splineObj.SetActive(false);
			}
			this.m_audioSource.spatialBlend = 0f;
			this.m_audioSource.gameObject.transform.SetParent(entity.gameObject.transform);
			this.m_audioSource.gameObject.transform.localPosition = Vector3.zero;
			this.m_attached = true;
		}

		// Token: 0x06006509 RID: 25865 RVA: 0x0020B708 File Offset: 0x00209908
		public void ResetAudio()
		{
			if (!this.m_initialized || !this.m_audioSource)
			{
				return;
			}
			this.m_audioSource.volume = this.m_defaultVolume;
			this.m_audioSource.spatialBlend = this.m_defaultSpatialBlend;
			if (this.m_splineObj)
			{
				this.m_splineObj.SetActive(true);
			}
			if (this.m_splineAdjustments)
			{
				this.m_splineAdjustments.enabled = true;
			}
			if (this.m_moveAlong)
			{
				this.m_moveAlong.enabled = true;
			}
			this.m_audioSource.gameObject.transform.SetParent(this.m_defaultParent);
			this.m_attached = false;
		}

		// Token: 0x040057CC RID: 22476
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x040057CD RID: 22477
		[SerializeField]
		private MoveAlongSplineWithCamera m_moveAlong;

		// Token: 0x040057CE RID: 22478
		[SerializeField]
		private SplineAudioAdjustments m_splineAdjustments;

		// Token: 0x040057CF RID: 22479
		[SerializeField]
		private GameObject m_splineObj;

		// Token: 0x040057D0 RID: 22480
		[NonSerialized]
		private bool m_attached;

		// Token: 0x040057D1 RID: 22481
		[NonSerialized]
		private bool m_initialized;

		// Token: 0x040057D2 RID: 22482
		[NonSerialized]
		private float m_defaultVolume = 1f;

		// Token: 0x040057D3 RID: 22483
		[NonSerialized]
		private float m_defaultSpatialBlend = 1f;

		// Token: 0x040057D4 RID: 22484
		[NonSerialized]
		private Transform m_defaultParent;
	}
}
