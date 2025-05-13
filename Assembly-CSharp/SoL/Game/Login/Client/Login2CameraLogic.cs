using System;
using Cinemachine;
using SoL.Game.SkyDome;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B29 RID: 2857
	public class Login2CameraLogic : MonoBehaviour
	{
		// Token: 0x060057BB RID: 22459 RVA: 0x001E4448 File Offset: 0x001E2648
		private void Awake()
		{
			this.m_dragToRotate.enabled = false;
			for (int i = 0; i < this.m_cameraButtons.Length; i++)
			{
				int index = i;
				this.m_cameraButtons[i].Button.onClick.AddListener(delegate()
				{
					this.CameraButtonClicked(index);
				});
			}
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x0007A875 File Offset: 0x00078A75
		private void Start()
		{
			this.CameraButtonClicked(0);
			if (SkyDomeManager.SkyDomeController != null)
			{
				this.InitializeSlider();
				return;
			}
			SkyDomeManager.SkydomeControllerChanged += this.SkyDomeManagerOnSkydomeControllerChanged;
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x001E44AC File Offset: 0x001E26AC
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_cameraButtons.Length; i++)
			{
				int index = i;
				this.m_cameraButtons[i].Button.onClick.RemoveListener(delegate()
				{
					this.CameraButtonClicked(index);
				});
			}
			if (this.m_sliderInitialized)
			{
				this.m_timeSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.SliderChanged));
			}
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x001E4528 File Offset: 0x001E2728
		private void CameraButtonClicked(int index)
		{
			for (int i = 0; i < this.m_cameraButtons.Length; i++)
			{
				this.m_cameraButtons[i].Camera.enabled = (i == index);
				if (i == index)
				{
					this.m_dragToRotate.enabled = this.m_cameraButtons[i].AllowDragToRotate;
				}
			}
		}

		// Token: 0x060057BF RID: 22463 RVA: 0x001E457C File Offset: 0x001E277C
		private void SliderChanged(float arg0)
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				return;
			}
			DateTime time = SkyDomeManager.SkyDomeController.GetTime();
			time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
			time = time.AddHours((double)(arg0 * 24f));
			SkyDomeManager.SkyDomeController.SetTime(time);
		}

		// Token: 0x060057C0 RID: 22464 RVA: 0x0007A89D File Offset: 0x00078A9D
		private void SkyDomeManagerOnSkydomeControllerChanged()
		{
			if (SkyDomeManager.SkyDomeController != null)
			{
				SkyDomeManager.SkydomeControllerChanged -= this.SkyDomeManagerOnSkydomeControllerChanged;
				this.InitializeSlider();
			}
		}

		// Token: 0x060057C1 RID: 22465 RVA: 0x001E45D8 File Offset: 0x001E27D8
		private void InitializeSlider()
		{
			SkyDomeManager.SkyDomeController.ProgressTime = this.m_simulateTime;
			DateTime time = SkyDomeManager.SkyDomeController.GetTime();
			this.m_timeSlider.value = (float)time.Hour / 24f;
			if (!this.m_sliderInitialized)
			{
				this.m_timeSlider.onValueChanged.AddListener(new UnityAction<float>(this.SliderChanged));
				this.m_sliderInitialized = true;
			}
		}

		// Token: 0x04004D64 RID: 19812
		[SerializeField]
		private Login2CameraLogic.CameraButton[] m_cameraButtons;

		// Token: 0x04004D65 RID: 19813
		[SerializeField]
		private Slider m_timeSlider;

		// Token: 0x04004D66 RID: 19814
		[SerializeField]
		private DragToRotate m_dragToRotate;

		// Token: 0x04004D67 RID: 19815
		[SerializeField]
		private bool m_simulateTime = true;

		// Token: 0x04004D68 RID: 19816
		private bool m_sliderInitialized;

		// Token: 0x02000B2A RID: 2858
		[Serializable]
		private class CameraButton
		{
			// Token: 0x170014AA RID: 5290
			// (get) Token: 0x060057C3 RID: 22467 RVA: 0x0007A8CC File Offset: 0x00078ACC
			public CinemachineVirtualCamera Camera
			{
				get
				{
					return this.m_camera;
				}
			}

			// Token: 0x170014AB RID: 5291
			// (get) Token: 0x060057C4 RID: 22468 RVA: 0x0007A8D4 File Offset: 0x00078AD4
			public Button Button
			{
				get
				{
					return this.m_button;
				}
			}

			// Token: 0x170014AC RID: 5292
			// (get) Token: 0x060057C5 RID: 22469 RVA: 0x0007A8DC File Offset: 0x00078ADC
			public bool AllowDragToRotate
			{
				get
				{
					return this.m_allowDragToRotate;
				}
			}

			// Token: 0x04004D69 RID: 19817
			[SerializeField]
			private CinemachineVirtualCamera m_camera;

			// Token: 0x04004D6A RID: 19818
			[SerializeField]
			private Button m_button;

			// Token: 0x04004D6B RID: 19819
			[SerializeField]
			private bool m_allowDragToRotate;
		}
	}
}
