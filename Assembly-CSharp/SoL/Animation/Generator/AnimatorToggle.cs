using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Animation.Generator
{
	// Token: 0x0200023B RID: 571
	public class AnimatorToggle : MonoBehaviour
	{
		// Token: 0x060012E8 RID: 4840 RVA: 0x000E842C File Offset: 0x000E662C
		public void Init(Animator animator, AnimatorControllerParameter param)
		{
			if (param.type != AnimatorControllerParameterType.Bool)
			{
				throw new ArgumentException("Should be of type Bool!");
			}
			this.m_controller = animator;
			this.m_parameter = param;
			this.m_text.text = param.name;
			this.m_toggle.isOn = animator.GetBool(this.m_parameter.nameHash);
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleClicked));
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0004F74E File Offset: 0x0004D94E
		private void ToggleClicked(bool value)
		{
			this.m_controller.SetBool(this.m_parameter.nameHash, value);
		}

		// Token: 0x040010B1 RID: 4273
		[SerializeField]
		private Toggle m_toggle;

		// Token: 0x040010B2 RID: 4274
		[SerializeField]
		private Text m_text;

		// Token: 0x040010B3 RID: 4275
		private Animator m_controller;

		// Token: 0x040010B4 RID: 4276
		private AnimatorControllerParameter m_parameter;
	}
}
