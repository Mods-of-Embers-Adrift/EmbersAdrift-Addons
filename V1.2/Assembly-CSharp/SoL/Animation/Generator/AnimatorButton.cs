using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Animation.Generator
{
	// Token: 0x02000237 RID: 567
	public class AnimatorButton : MonoBehaviour
	{
		// Token: 0x060012DE RID: 4830 RVA: 0x000E80AC File Offset: 0x000E62AC
		public void Init(Animator animator, AnimatorControllerParameter param)
		{
			if (param.type != AnimatorControllerParameterType.Trigger)
			{
				throw new ArgumentException("Should be of type Trigger!");
			}
			this.m_controller = animator;
			this.m_parameter = param;
			this.m_text.text = param.name;
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0004F723 File Offset: 0x0004D923
		private void ButtonClicked()
		{
			this.m_controller.SetTrigger(this.m_parameter.nameHash);
		}

		// Token: 0x0400109C RID: 4252
		[SerializeField]
		private Button m_button;

		// Token: 0x0400109D RID: 4253
		[SerializeField]
		private Text m_text;

		// Token: 0x0400109E RID: 4254
		private Animator m_controller;

		// Token: 0x0400109F RID: 4255
		private AnimatorControllerParameter m_parameter;
	}
}
