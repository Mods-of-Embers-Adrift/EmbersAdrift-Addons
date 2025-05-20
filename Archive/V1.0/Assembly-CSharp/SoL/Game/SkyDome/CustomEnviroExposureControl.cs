using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006E9 RID: 1769
	[ExecuteInEditMode]
	public class CustomEnviroExposureControl : MonoBehaviour, IExposureController
	{
		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06003585 RID: 13701 RVA: 0x00064994 File Offset: 0x00062B94
		public float Exposure
		{
			get
			{
				return this.m_exposure.fixedExposure.value;
			}
		}

		// Token: 0x06003586 RID: 13702 RVA: 0x000649A6 File Offset: 0x00062BA6
		private void OnEnable()
		{
			this.GrabExposureProfile();
			SkyDomeManager.ExposureController = this;
		}

		// Token: 0x06003587 RID: 13703 RVA: 0x00166F64 File Offset: 0x00165164
		private void GrabExposureProfile()
		{
			Volume component = base.gameObject.GetComponent<Volume>();
			Exposure exposure;
			if (component && component.sharedProfile && component.sharedProfile.TryGet<Exposure>(out exposure))
			{
				this.m_exposure = exposure;
				this.m_exposure.mode.overrideState = true;
				this.m_exposure.fixedExposure.overrideState = true;
			}
		}

		// Token: 0x04003384 RID: 13188
		[SerializeField]
		private float m_newMoonExposureMod = -2f;

		// Token: 0x04003385 RID: 13189
		private Exposure m_exposure;

		// Token: 0x04003386 RID: 13190
		[TextArea]
		[SerializeField]
		private string m_notes;
	}
}
