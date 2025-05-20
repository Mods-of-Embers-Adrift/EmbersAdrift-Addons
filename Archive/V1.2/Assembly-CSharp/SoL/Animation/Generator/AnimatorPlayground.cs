using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Animation.Generator
{
	// Token: 0x02000238 RID: 568
	public class AnimatorPlayground : MonoBehaviour
	{
		// Token: 0x060012E1 RID: 4833 RVA: 0x000E810C File Offset: 0x000E630C
		private void Start()
		{
			for (int i = 0; i < this.m_sliderValues.Length; i++)
			{
				this.m_sliderValueDict.Add(this.m_sliderValues[i].Name, this.m_sliderValues[i]);
			}
			for (int j = 0; j < this.m_animator.parameterCount; j++)
			{
				AnimatorControllerParameter parameter = this.m_animator.GetParameter(j);
				AnimatorControllerParameterType type = parameter.type;
				switch (type)
				{
				case AnimatorControllerParameterType.Float:
				case AnimatorControllerParameterType.Int:
				{
					AnimatorSlider component = UnityEngine.Object.Instantiate<GameObject>(this.m_animatorSlider, this.m_parent).GetComponent<AnimatorSlider>();
					AnimatorPlayground.AnimatorSliderValues animatorSliderValues;
					if (this.m_sliderValueDict.TryGetValue(parameter.name, out animatorSliderValues))
					{
						component.Init(this.m_animator, parameter, animatorSliderValues.Min, animatorSliderValues.Max);
					}
					else
					{
						component.Init(this.m_animator, parameter, 0f, 20f);
					}
					break;
				}
				case (AnimatorControllerParameterType)2:
					break;
				case AnimatorControllerParameterType.Bool:
					UnityEngine.Object.Instantiate<GameObject>(this.m_animatorToggle, this.m_parent).GetComponent<AnimatorToggle>().Init(this.m_animator, parameter);
					break;
				default:
					if (type == AnimatorControllerParameterType.Trigger)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.m_animatorButton, this.m_parent).GetComponent<AnimatorButton>().Init(this.m_animator, parameter);
					}
					break;
				}
			}
		}

		// Token: 0x040010A0 RID: 4256
		[SerializeField]
		private AnimatorPlayground.AnimatorSliderValues[] m_sliderValues;

		// Token: 0x040010A1 RID: 4257
		[SerializeField]
		private GameObject m_animatorButton;

		// Token: 0x040010A2 RID: 4258
		[SerializeField]
		private GameObject m_animatorSlider;

		// Token: 0x040010A3 RID: 4259
		[SerializeField]
		private GameObject m_animatorToggle;

		// Token: 0x040010A4 RID: 4260
		[SerializeField]
		private Animator m_animator;

		// Token: 0x040010A5 RID: 4261
		[SerializeField]
		private Transform m_parent;

		// Token: 0x040010A6 RID: 4262
		private Dictionary<string, AnimatorPlayground.AnimatorSliderValues> m_sliderValueDict = new Dictionary<string, AnimatorPlayground.AnimatorSliderValues>();

		// Token: 0x02000239 RID: 569
		[Serializable]
		private class AnimatorSliderValues
		{
			// Token: 0x040010A7 RID: 4263
			public string Name;

			// Token: 0x040010A8 RID: 4264
			public float Min;

			// Token: 0x040010A9 RID: 4265
			public float Max;
		}
	}
}
