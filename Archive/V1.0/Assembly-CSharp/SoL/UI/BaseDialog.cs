using System;
using Cysharp.Text;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.UI
{
	// Token: 0x0200034C RID: 844
	public abstract class BaseDialog<T> : DraggableUIWindow where T : IDialogOptions
	{
		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001710 RID: 5904 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual object Result
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001711 RID: 5905 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_reparentOnDrag
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x00052219 File Offset: 0x00050419
		public override bool CanModify
		{
			get
			{
				return this.m_currentOptions.AllowDragging && base.CanModify;
			}
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00101B84 File Offset: 0x000FFD84
		protected override void Start()
		{
			base.Start();
			if (this.m_confirm != null)
			{
				this.m_confirm.onClick.AddListener(new UnityAction(this.Confirm));
			}
			if (this.m_cancel != null)
			{
				this.m_cancel.onClick.AddListener(new UnityAction(this.Cancel));
			}
			if (this.m_centerOnStart)
			{
				base.RectTransform.localPosition = Vector2.zero;
			}
			this.Hide(true);
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x00101C10 File Offset: 0x000FFE10
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_confirm != null)
			{
				this.m_confirm.onClick.RemoveAllListeners();
			}
			if (this.m_cancel != null)
			{
				this.m_cancel.onClick.RemoveAllListeners();
			}
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x00052236 File Offset: 0x00050436
		public override void CloseButtonPressed()
		{
			this.Cancel();
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x00101C60 File Offset: 0x000FFE60
		public void Init(T opts)
		{
			if (this.m_isActive)
			{
				this.m_isActive = false;
				this.SetButtonInteractable(false);
				ref T ptr = ref this.m_currentOptions;
				T t = default(T);
				if (t == null)
				{
					t = this.m_currentOptions;
					ptr = ref t;
					if (t == null)
					{
						goto IL_57;
					}
				}
				Action<bool, object> callback = ptr.Callback;
				if (callback != null)
				{
					callback(false, null);
				}
			}
			IL_57:
			this.m_currentOptions = opts;
			this.SetButtonInteractable(true);
			this.m_closeButton.SetActive(opts.ShowCloseButton);
			if (this.m_backGroundBlocker)
			{
				this.m_backGroundBlocker.gameObject.SetActive(opts.BlockInteractions);
				this.m_backGroundBlocker.SetColor(opts.BackgroundBlockerColor);
			}
			this.SetTexts(opts);
			this.InitInternal();
			this.Show(false);
			this.m_isActive = true;
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x00101D48 File Offset: 0x000FFF48
		protected void SetTexts(T opts)
		{
			this.m_title.ZStringSetText(opts.Title);
			this.m_text.SetTextWithReplacements(opts.Text);
			if (this.m_confirm)
			{
				this.m_confirm.text = opts.ConfirmationText;
			}
			if (this.m_cancel)
			{
				this.m_cancel.text = opts.CancelText;
				this.m_cancel.gameObject.SetActive(!opts.HideCancelButton);
			}
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InitInternal()
		{
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0005223E File Offset: 0x0005043E
		protected virtual void Confirm()
		{
			this.m_isActive = false;
			this.SetButtonInteractable(false);
			Action<bool, object> callback = this.m_currentOptions.Callback;
			if (callback != null)
			{
				callback(true, this.Result);
			}
			this.Hide(false);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x00052278 File Offset: 0x00050478
		protected void Cancel()
		{
			this.m_isActive = false;
			this.SetButtonInteractable(false);
			Action<bool, object> callback = this.m_currentOptions.Callback;
			if (callback != null)
			{
				callback(false, null);
			}
			this.Hide(false);
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x000522AD File Offset: 0x000504AD
		private void SetButtonInteractable(bool interactable)
		{
			if (this.m_confirm)
			{
				this.m_confirm.interactable = interactable;
			}
			if (this.m_cancel)
			{
				this.m_cancel.interactable = interactable;
			}
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00052236 File Offset: 0x00050436
		public virtual void ResetDialog()
		{
			this.Cancel();
		}

		// Token: 0x04001ED6 RID: 7894
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04001ED7 RID: 7895
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04001ED8 RID: 7896
		[SerializeField]
		protected SolButton m_confirm;

		// Token: 0x04001ED9 RID: 7897
		[SerializeField]
		protected SolButton m_cancel;

		// Token: 0x04001EDA RID: 7898
		[SerializeField]
		private bool m_centerOnStart;

		// Token: 0x04001EDB RID: 7899
		protected T m_currentOptions;

		// Token: 0x04001EDC RID: 7900
		private bool m_isActive;
	}
}
