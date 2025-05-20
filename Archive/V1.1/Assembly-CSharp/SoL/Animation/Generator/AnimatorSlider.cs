using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Animation.Generator
{
	// Token: 0x0200023A RID: 570
	public class AnimatorSlider : MonoBehaviour
	{
		// Token: 0x060012E4 RID: 4836 RVA: 0x000E822C File Offset: 0x000E642C
		public void Init(Animator animator, AnimatorControllerParameter param, float min, float max)
		{
			if (param.type != AnimatorControllerParameterType.Int && param.type != AnimatorControllerParameterType.Float)
			{
				throw new ArgumentException("Should be of type int/float!");
			}
			this.m_controller = animator;
			this.m_parameter = param;
			this.m_text.text = param.name;
			this.m_slider.minValue = min;
			this.m_slider.maxValue = max;
			AnimatorControllerParameterType type = param.type;
			if (type != AnimatorControllerParameterType.Float)
			{
				if (type == AnimatorControllerParameterType.Int)
				{
					this.m_slider.wholeNumbers = true;
					this.m_slider.value = (float)animator.GetInteger(param.nameHash);
					this.m_value.text = ((int)this.m_slider.value).ToString();
				}
			}
			else
			{
				this.m_slider.wholeNumbers = false;
				this.m_slider.value = animator.GetFloat(param.nameHash);
				this.m_value.text = this.m_slider.value.ToString();
			}
			this.m_defaultValue = this.m_slider.value;
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.SliderChanged));
			this.m_reset.onClick.AddListener(new UnityAction(this.ResetClicked));
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x000E8370 File Offset: 0x000E6570
		private void ResetClicked()
		{
			AnimatorControllerParameterType type = this.m_parameter.type;
			if (type != AnimatorControllerParameterType.Float)
			{
				if (type == AnimatorControllerParameterType.Int)
				{
					this.m_slider.value = this.m_defaultValue;
					return;
				}
			}
			else
			{
				this.m_slider.value = this.m_defaultValue;
			}
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x000E83B4 File Offset: 0x000E65B4
		private void SliderChanged(float value)
		{
			AnimatorControllerParameterType type = this.m_parameter.type;
			if (type != AnimatorControllerParameterType.Float)
			{
				if (type == AnimatorControllerParameterType.Int)
				{
					this.m_value.text = ((int)value).ToString();
					this.m_controller.SetInteger(this.m_parameter.nameHash, (int)value);
					return;
				}
			}
			else
			{
				this.m_value.text = value.ToString();
				this.m_controller.SetFloat(this.m_parameter.nameHash, value);
			}
		}

		// Token: 0x040010AA RID: 4266
		[SerializeField]
		private Slider m_slider;

		// Token: 0x040010AB RID: 4267
		[SerializeField]
		private Text m_text;

		// Token: 0x040010AC RID: 4268
		[SerializeField]
		private Button m_reset;

		// Token: 0x040010AD RID: 4269
		[SerializeField]
		private Text m_value;

		// Token: 0x040010AE RID: 4270
		private Animator m_controller;

		// Token: 0x040010AF RID: 4271
		private AnimatorControllerParameter m_parameter;

		// Token: 0x040010B0 RID: 4272
		private float m_defaultValue;
	}
}
