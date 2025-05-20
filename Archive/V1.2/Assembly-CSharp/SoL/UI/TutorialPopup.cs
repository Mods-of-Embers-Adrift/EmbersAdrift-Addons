using System;
using SoL.Game.Messages;
using SoL.Game.UI.Quests;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.UI
{
	// Token: 0x0200035E RID: 862
	public class TutorialPopup : BaseDialog<TutorialPopupOptions>
	{
		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x00052737 File Offset: 0x00050937
		public bool Active
		{
			get
			{
				return this.m_active;
			}
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x0005273F File Offset: 0x0005093F
		protected override void Start()
		{
			base.Start();
			if (this.m_open)
			{
				this.m_open.onClick.AddListener(new UnityAction(this.OnOpenClicked));
			}
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00052770 File Offset: 0x00050970
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_open)
			{
				this.m_open.onClick.RemoveListener(new UnityAction(this.OnOpenClicked));
			}
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00102748 File Offset: 0x00100948
		protected override void InitInternal()
		{
			base.InitInternal();
			this.m_active = true;
			base.PreventDragging = this.m_currentOptions.PreventDragging;
			if (this.m_currentOptions.MessageType != MessageType.None)
			{
				MessageManager.ChatQueue.AddToQueue(this.m_currentOptions.MessageType, this.m_currentOptions.Text.ReplaceActionMappings());
			}
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x000527A1 File Offset: 0x000509A1
		protected override void Confirm()
		{
			base.Confirm();
			this.m_active = false;
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000527B0 File Offset: 0x000509B0
		public override void ResetDialog()
		{
			this.m_active = false;
			base.ResetDialog();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000527BF File Offset: 0x000509BF
		private void OnOpenClicked()
		{
			ClientGameManager.UIManager.LogUI.Show(LogUITabs.Tutorials);
			ClientGameManager.UIManager.LogUI.Tutorials.SelectTutorial(this.m_currentOptions.Tutorial);
		}

		// Token: 0x04001F3F RID: 7999
		[SerializeField]
		private SolButton m_open;

		// Token: 0x04001F40 RID: 8000
		private bool m_active;
	}
}
