using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008A6 RID: 2214
	public class LinkHandsButton : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600408D RID: 16525 RVA: 0x0018C960 File Offset: 0x0018AB60
		private void Start()
		{
			this.m_acceptedKey = "LinkWarningAccepted_" + SessionData.SelectedCharacter.Name;
			this.m_callback = new Action<bool, object>(this.Callback);
			this.m_lockButton.State = ToggleController.ToggleState.ON;
			this.m_lockButton.ToggleChanged += this.LockButtonOnToggleChanged;
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x0006BAD7 File Offset: 0x00069CD7
		private void OnDestroy()
		{
			this.m_lockButton.ToggleChanged -= this.LockButtonOnToggleChanged;
		}

		// Token: 0x0600408F RID: 16527 RVA: 0x0018C9BC File Offset: 0x0018ABBC
		private void LockButtonOnToggleChanged(ToggleController.ToggleState obj)
		{
			if (this.m_desiredState != null)
			{
				return;
			}
			if (PlayerPrefs.GetInt(this.m_acceptedKey, 0) == 1)
			{
				this.SetLockedHands(obj == ToggleController.ToggleState.ON);
				return;
			}
			this.m_desiredState = new ToggleController.ToggleState?(obj);
			ToggleController.ToggleState? desiredState = this.m_desiredState;
			if (desiredState != null)
			{
				ToggleController.ToggleState valueOrDefault = desiredState.GetValueOrDefault();
				if (valueOrDefault != ToggleController.ToggleState.ON)
				{
					if (valueOrDefault == ToggleController.ToggleState.OFF)
					{
						this.m_lockButton.State = ToggleController.ToggleState.ON;
					}
				}
				else
				{
					this.m_lockButton.State = ToggleController.ToggleState.OFF;
				}
			}
			DialogOptions opts = new DialogOptions
			{
				ShowCloseButton = false,
				AllowDragging = false,
				BlockInteractions = false,
				Title = "WARNING!",
				Text = "Unlinking hands is an advanced feature, are you sure you want to do this?",
				ConfirmationText = "Yes",
				CancelText = "No",
				Callback = this.m_callback
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06004090 RID: 16528 RVA: 0x0018CAA8 File Offset: 0x0018ACA8
		private void Callback(bool arg1, object arg2)
		{
			if (arg1 && this.m_desiredState != null)
			{
				PlayerPrefs.SetInt(this.m_acceptedKey, 1);
				this.m_lockButton.State = this.m_desiredState.Value;
				this.SetLockedHands(this.m_lockButton.State == ToggleController.ToggleState.ON);
			}
			this.m_desiredState = null;
		}

		// Token: 0x06004091 RID: 16529 RVA: 0x0018CB08 File Offset: 0x0018AD08
		private void SetLockedHands(bool linked)
		{
			PlayerCharacterData playerCharacterData;
			LocalPlayer.GameEntity.CharacterData.TryGetAsType(out playerCharacterData);
		}

		// Token: 0x06004092 RID: 16530 RVA: 0x0018CB28 File Offset: 0x0018AD28
		private ITooltipParameter GetTooltipParameter()
		{
			string txt = (this.m_lockButton.State == ToggleController.ToggleState.ON) ? "Main and off hands are linked" : "Main and off hands are NOT linked";
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000ED4 RID: 3796
		// (get) Token: 0x06004093 RID: 16531 RVA: 0x0006BAF0 File Offset: 0x00069CF0
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000ED5 RID: 3797
		// (get) Token: 0x06004094 RID: 16532 RVA: 0x0006BAFE File Offset: 0x00069CFE
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000ED6 RID: 3798
		// (get) Token: 0x06004095 RID: 16533 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E42 RID: 15938
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003E43 RID: 15939
		[SerializeField]
		private LockButton m_lockButton;

		// Token: 0x04003E44 RID: 15940
		private Action<bool, object> m_callback;

		// Token: 0x04003E45 RID: 15941
		private ToggleController.ToggleState? m_desiredState;

		// Token: 0x04003E46 RID: 15942
		private string m_acceptedKey;

		// Token: 0x04003E47 RID: 15943
		private const string kAcceptedKey = "LinkWarningAccepted";
	}
}
