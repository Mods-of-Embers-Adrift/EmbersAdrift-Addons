using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI.Macros
{
	// Token: 0x0200097C RID: 2428
	public class MacroEditDialog : BaseDialog<MacroEditDialogOptions>
	{
		// Token: 0x17001008 RID: 4104
		// (get) Token: 0x06004851 RID: 18513 RVA: 0x00070A9E File Offset: 0x0006EC9E
		protected override object Result
		{
			get
			{
				return new ValueTuple<string, string>(this.m_nameInput.text, this.m_macroInput.text);
			}
		}

		// Token: 0x06004852 RID: 18514 RVA: 0x001A96F4 File Offset: 0x001A78F4
		private void Update()
		{
			if (base.Visible)
			{
				if (this.m_currentOptions.AutoCancel != null && this.m_currentOptions.AutoCancel())
				{
					base.Cancel();
					this.m_currentOptions.AutoCancel = null;
					return;
				}
				if (ClientGameManager.InputManager != null)
				{
					if (ClientGameManager.InputManager.TabDown && EventSystem.current && EventSystem.current.currentSelectedGameObject)
					{
						if (EventSystem.current.currentSelectedGameObject == this.m_nameInput.gameObject)
						{
							this.m_macroInput.Select();
						}
						else if (EventSystem.current.currentSelectedGameObject == this.m_macroInput.gameObject)
						{
							this.m_nameInput.Select();
						}
					}
					if (ClientGameManager.InputManager.EnterDown)
					{
						this.Confirm();
					}
				}
			}
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x001A97D4 File Offset: 0x001A79D4
		protected override void InitInternal()
		{
			base.InitInternal();
			if (this.m_currentOptions.Data != null)
			{
				this.m_nameInput.SetTextWithoutNotify(this.m_currentOptions.Data.DisplayName);
				this.m_macroInput.SetTextWithoutNotify(this.m_currentOptions.Data.MacroText);
			}
		}

		// Token: 0x040043A2 RID: 17314
		[SerializeField]
		private SolTMP_InputField m_nameInput;

		// Token: 0x040043A3 RID: 17315
		[SerializeField]
		private SolTMP_InputField m_macroInput;
	}
}
