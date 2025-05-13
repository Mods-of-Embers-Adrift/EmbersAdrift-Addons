using System;
using SoL.Game.UI;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.UI
{
	// Token: 0x02000367 RID: 871
	public class InputFieldHooks : MonoBehaviour
	{
		// Token: 0x060017CD RID: 6093 RVA: 0x00102F78 File Offset: 0x00101178
		private void Awake()
		{
			if (this.m_inputField != null)
			{
				this.m_inputField.onSelect.AddListener(new UnityAction<string>(this.OnSelected));
				this.m_inputField.onDeselect.AddListener(new UnityAction<string>(this.OnDeselected));
			}
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00102FCC File Offset: 0x001011CC
		private void OnDestroy()
		{
			if (this.m_inputField != null)
			{
				this.m_inputField.onSelect.RemoveListener(new UnityAction<string>(this.OnSelected));
				this.m_inputField.onDeselect.RemoveListener(new UnityAction<string>(this.OnDeselected));
			}
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00052A89 File Offset: 0x00050C89
		private void OnSelected(string txt)
		{
			ClientGameManager.InputManager.SetInputPreventionFlag(InputPreventionFlags.InputField);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x00052A96 File Offset: 0x00050C96
		private void OnDeselected(string txt)
		{
			ClientGameManager.InputManager.UnsetInputPreventionFlag(InputPreventionFlags.InputField);
		}

		// Token: 0x04001F67 RID: 8039
		[SerializeField]
		private TMP_InputField m_inputField;
	}
}
