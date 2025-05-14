using System;
using System.Collections.Generic;
using SoL.Tests;
using TMPro;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UMA
{
	// Token: 0x02000625 RID: 1573
	public class UMATestController : MonoBehaviour
	{
		// Token: 0x060031AC RID: 12716 RVA: 0x0015D720 File Offset: 0x0015B920
		private void Awake()
		{
			this.m_dcas = new List<DynamicCharacterAvatar>(10);
			this.m_atlasScaleSlider.minValue = 0.01f;
			this.m_atlasScaleSlider.maxValue = 1f;
			this.m_atlasScaleSlider.value = 1f;
			this.m_atlasScaleSlider.onValueChanged.AddListener(new UnityAction<float>(this.AtlasScaleChanged));
			this.m_countSlider.wholeNumbers = true;
			this.m_countSlider.minValue = 1f;
			this.m_countSlider.maxValue = (float)this.m_maxCount;
			this.m_countSlider.value = 1f;
			this.m_countSlider.onValueChanged.AddListener(new UnityAction<float>(this.CreateCountSliderChanged));
			this.m_refreshButton.onClick.AddListener(new UnityAction(this.RefreshClicked));
			this.m_createButton.onClick.AddListener(new UnityAction(this.CreateClicked));
			this.m_destroyButton.onClick.AddListener(new UnityAction(this.DestroyClicked));
			this.RefreshScaleLabel();
			this.RefreshCurrentCountLabel();
			this.RefreshCountSliderLabel();
		}

		// Token: 0x060031AD RID: 12717 RVA: 0x0015D848 File Offset: 0x0015BA48
		private void OnDestroy()
		{
			this.m_atlasScaleSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.AtlasScaleChanged));
			this.m_countSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.CreateCountSliderChanged));
			this.m_refreshButton.onClick.RemoveListener(new UnityAction(this.RefreshClicked));
			this.m_createButton.onClick.RemoveListener(new UnityAction(this.CreateClicked));
			this.m_destroyButton.onClick.RemoveListener(new UnityAction(this.DestroyClicked));
		}

		// Token: 0x060031AE RID: 12718 RVA: 0x0015D8E4 File Offset: 0x0015BAE4
		private void AtlasScaleChanged(float arg0)
		{
			for (int i = 0; i < this.m_dcas.Count; i++)
			{
				this.m_dcas[i].umaData.atlasResolutionScale = this.m_atlasScaleSlider.value;
				this.m_dcas[i].ForceUpdate(false, true, false);
			}
			this.RefreshScaleLabel();
		}

		// Token: 0x060031AF RID: 12719 RVA: 0x00062435 File Offset: 0x00060635
		private void CreateCountSliderChanged(float arg0)
		{
			this.RefreshCountSliderLabel();
		}

		// Token: 0x060031B0 RID: 12720 RVA: 0x0015D944 File Offset: 0x0015BB44
		private void DestroyClicked()
		{
			if (this.m_dcas.Count <= 0)
			{
				return;
			}
			int index = this.m_dcas.Count - 1;
			DynamicCharacterAvatar dynamicCharacterAvatar = this.m_dcas[index];
			if (dynamicCharacterAvatar != null)
			{
				UnityEngine.Object.Destroy(dynamicCharacterAvatar.gameObject);
			}
			this.m_dcas.RemoveAt(index);
			this.RefreshCurrentCountLabel();
		}

		// Token: 0x060031B1 RID: 12721 RVA: 0x0015D9A4 File Offset: 0x0015BBA4
		private void CreateClicked()
		{
			int sliderCount = this.GetSliderCount();
			System.Random incomingRandom = new System.Random(this.m_seed);
			for (int i = 0; i < sliderCount; i++)
			{
				DynamicCharacterAvatar dynamicCharacterAvatar = this.m_tester.InitNpcRandomInternal(incomingRandom);
				if (dynamicCharacterAvatar != null)
				{
					this.m_dcas.Add(dynamicCharacterAvatar);
				}
			}
			this.RefreshCurrentCountLabel();
		}

		// Token: 0x060031B2 RID: 12722 RVA: 0x0015D9F8 File Offset: 0x0015BBF8
		private void RefreshClicked()
		{
			for (int i = 0; i < this.m_dcas.Count; i++)
			{
				this.m_dcas[i].ForceUpdate(true, true, true);
			}
		}

		// Token: 0x060031B3 RID: 12723 RVA: 0x0006243D File Offset: 0x0006063D
		private int GetSliderCount()
		{
			return Mathf.Clamp(Mathf.FloorToInt(this.m_countSlider.value), 1, this.m_maxCount);
		}

		// Token: 0x060031B4 RID: 12724 RVA: 0x0015DA30 File Offset: 0x0015BC30
		private void RefreshCountSliderLabel()
		{
			this.m_countSliderLabel.text = this.GetSliderCount().ToString();
		}

		// Token: 0x060031B5 RID: 12725 RVA: 0x0015DA58 File Offset: 0x0015BC58
		private void RefreshCurrentCountLabel()
		{
			this.m_countLabel.text = this.m_dcas.Count.ToString();
		}

		// Token: 0x060031B6 RID: 12726 RVA: 0x0015DA84 File Offset: 0x0015BC84
		private void RefreshScaleLabel()
		{
			this.m_atlasScaleLabel.text = this.m_atlasScaleSlider.value.ToString("F02");
		}

		// Token: 0x04003016 RID: 12310
		[SerializeField]
		private int m_seed = 8675309;

		// Token: 0x04003017 RID: 12311
		[SerializeField]
		private DCATester m_tester;

		// Token: 0x04003018 RID: 12312
		[SerializeField]
		private Slider m_atlasScaleSlider;

		// Token: 0x04003019 RID: 12313
		[SerializeField]
		private TextMeshProUGUI m_atlasScaleLabel;

		// Token: 0x0400301A RID: 12314
		[SerializeField]
		private int m_maxCount = 20;

		// Token: 0x0400301B RID: 12315
		[SerializeField]
		private Slider m_countSlider;

		// Token: 0x0400301C RID: 12316
		[SerializeField]
		private TextMeshProUGUI m_countSliderLabel;

		// Token: 0x0400301D RID: 12317
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x0400301E RID: 12318
		[SerializeField]
		private Button m_refreshButton;

		// Token: 0x0400301F RID: 12319
		[SerializeField]
		private Button m_createButton;

		// Token: 0x04003020 RID: 12320
		[SerializeField]
		private Button m_destroyButton;

		// Token: 0x04003021 RID: 12321
		private List<DynamicCharacterAvatar> m_dcas;
	}
}
