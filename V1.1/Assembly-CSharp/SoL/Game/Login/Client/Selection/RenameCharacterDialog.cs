using System;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B42 RID: 2882
	public class RenameCharacterDialog : MonoBehaviour
	{
		// Token: 0x0600589C RID: 22684 RVA: 0x001E6B5C File Offset: 0x001E4D5C
		private void Awake()
		{
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancelButton.onClick.AddListener(new UnityAction(this.OnCancelClicked));
			this.m_inputField.onValueChanged.AddListener(new UnityAction<string>(this.InputFieldChanged));
			TMP_InputField inputField = this.m_inputField;
			inputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(inputField.onValidateInput, new TMP_InputField.OnValidateInput(NewCharacterManager.OnValidateNameInput));
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x001E6BE4 File Offset: 0x001E4DE4
		private void OnDestroy()
		{
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.OnCancelClicked));
			this.m_inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.InputFieldChanged));
			TMP_InputField inputField = this.m_inputField;
			inputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Remove(inputField.onValidateInput, new TMP_InputField.OnValidateInput(NewCharacterManager.OnValidateNameInput));
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001E6C6C File Offset: 0x001E4E6C
		private void InputFieldChanged(string arg0)
		{
			this.m_acceptButton.interactable = (this.m_record != null && this.m_record.Name != arg0 && !string.IsNullOrEmpty(arg0) && !arg0.Contains(this.m_record.Name));
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001E6CC0 File Offset: 0x001E4EC0
		private void OnAcceptClicked()
		{
			if (!string.IsNullOrEmpty(this.m_inputField.text))
			{
				LoginApiManager.RenameCharacter(this.m_record, this.m_inputField.text, new Action<bool, string>(this.Callback));
				this.m_acceptButton.interactable = false;
				this.m_cancelButton.interactable = false;
			}
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x0007B45B File Offset: 0x0007965B
		private void OnCancelClicked()
		{
			this.m_window.Hide(false);
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x001E6D1C File Offset: 0x001E4F1C
		private void Callback(bool result, string error)
		{
			if (result)
			{
				Debug.Log("SUCCESS!");
				this.m_record.Name = this.m_inputField.text;
				this.m_record.RequiresRenaming = new bool?(false);
				this.m_errorLabel.text = string.Empty;
				this.m_window.Hide(false);
				if (SelectionDirector.Instance)
				{
					SelectionDirector.Instance.RefreshCharacterNames();
					return;
				}
			}
			else
			{
				this.m_errorLabel.text = error;
				this.m_acceptButton.interactable = true;
				this.m_cancelButton.interactable = true;
			}
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x001E6DB4 File Offset: 0x001E4FB4
		public void Init(CharacterRecord record)
		{
			this.m_record = record;
			this.m_errorLabel.text = string.Empty;
			this.m_cancelButton.interactable = true;
			if (record != null)
			{
				this.m_acceptButton.interactable = true;
				this.m_label.text = record.Name;
				this.m_inputField.text = record.Name;
				this.m_window.Show(false);
				return;
			}
			this.m_acceptButton.interactable = false;
			this.m_label.text = string.Empty;
			this.m_inputField.text = string.Empty;
			this.m_window.Hide(false);
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x0007B469 File Offset: 0x00079669
		public void Hide(bool bypassTransition)
		{
			if (this.m_window)
			{
				this.m_window.Hide(bypassTransition);
			}
		}

		// Token: 0x04004E08 RID: 19976
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04004E09 RID: 19977
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004E0A RID: 19978
		[SerializeField]
		private TextMeshProUGUI m_errorLabel;

		// Token: 0x04004E0B RID: 19979
		[SerializeField]
		private TMP_InputField m_inputField;

		// Token: 0x04004E0C RID: 19980
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x04004E0D RID: 19981
		[SerializeField]
		private SolButton m_cancelButton;

		// Token: 0x04004E0E RID: 19982
		private CharacterRecord m_record;
	}
}
