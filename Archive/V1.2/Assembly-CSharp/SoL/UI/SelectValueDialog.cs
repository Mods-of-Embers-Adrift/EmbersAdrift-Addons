using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000357 RID: 855
	public class SelectValueDialog : BaseDialog<SelectValueOptions>
	{
		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x000524BE File Offset: 0x000506BE
		protected override object Result
		{
			get
			{
				return this.m_slider.value;
			}
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x001022F8 File Offset: 0x001004F8
		protected override void Start()
		{
			base.Start();
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.SliderValueChanged));
			this.m_slider.wholeNumbers = true;
			this.m_input.onValueChanged.AddListener(new UnityAction<string>(this.InputFieldChanged));
			this.m_input.characterValidation = TMP_InputField.CharacterValidation.Integer;
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x000524D0 File Offset: 0x000506D0
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_slider.onValueChanged.RemoveAllListeners();
			this.m_input.onValueChanged.RemoveAllListeners();
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x0010235C File Offset: 0x0010055C
		protected override void InitInternal()
		{
			this.m_slider.minValue = (float)this.m_currentOptions.MinValue;
			this.m_slider.maxValue = (float)this.m_currentOptions.MaxValue;
			this.m_slider.value = (float)this.m_currentOptions.DefaultValue;
			this.m_input.text = this.m_currentOptions.DefaultValue.ToString();
			this.m_input.ActivateInputField();
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x000524F8 File Offset: 0x000506F8
		private void SliderValueChanged(float value)
		{
			if (this.m_manuallySetting)
			{
				return;
			}
			this.m_manuallySetting = true;
			this.m_input.text = value.ToString();
			this.m_manuallySetting = false;
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x001023D4 File Offset: 0x001005D4
		private void InputFieldChanged(string txt)
		{
			if (this.m_manuallySetting)
			{
				return;
			}
			this.m_manuallySetting = true;
			int value = (int)this.m_slider.value;
			if (int.TryParse(txt, out value))
			{
				int num = Mathf.Clamp(value, this.m_currentOptions.MinValue, this.m_currentOptions.MaxValue);
				this.m_input.text = num.ToString();
				this.m_slider.value = (float)num;
			}
			this.m_manuallySetting = false;
		}

		// Token: 0x04001F18 RID: 7960
		[SerializeField]
		private TMP_InputField m_input;

		// Token: 0x04001F19 RID: 7961
		[SerializeField]
		private Slider m_slider;

		// Token: 0x04001F1A RID: 7962
		private bool m_manuallySetting;
	}
}
