using System;
using Rewired;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008A0 RID: 2208
	public class KeybindItemUI : MonoBehaviour
	{
		// Token: 0x17000EC7 RID: 3783
		// (get) Token: 0x06004052 RID: 16466 RVA: 0x0006B900 File Offset: 0x00069B00
		// (set) Token: 0x06004053 RID: 16467 RVA: 0x0006B908 File Offset: 0x00069B08
		public int ActionId { get; set; }

		// Token: 0x17000EC8 RID: 3784
		// (get) Token: 0x06004054 RID: 16468 RVA: 0x0006B911 File Offset: 0x00069B11
		// (set) Token: 0x06004055 RID: 16469 RVA: 0x0006B91E File Offset: 0x00069B1E
		public string Label
		{
			get
			{
				return this.m_actionLabel.text;
			}
			set
			{
				this.m_actionLabel.text = value;
			}
		}

		// Token: 0x140000BE RID: 190
		// (add) Token: 0x06004056 RID: 16470 RVA: 0x0018C100 File Offset: 0x0018A300
		// (remove) Token: 0x06004057 RID: 16471 RVA: 0x0018C138 File Offset: 0x0018A338
		public event Action<int, int> BindActivated;

		// Token: 0x140000BF RID: 191
		// (add) Token: 0x06004058 RID: 16472 RVA: 0x0018C170 File Offset: 0x0018A370
		// (remove) Token: 0x06004059 RID: 16473 RVA: 0x0018C1A8 File Offset: 0x0018A3A8
		public event Action<int> ResetToDefaults;

		// Token: 0x140000C0 RID: 192
		// (add) Token: 0x0600405A RID: 16474 RVA: 0x0018C1E0 File Offset: 0x0018A3E0
		// (remove) Token: 0x0600405B RID: 16475 RVA: 0x0018C218 File Offset: 0x0018A418
		public event Action<int> UnbindAction;

		// Token: 0x0600405C RID: 16476 RVA: 0x0018C250 File Offset: 0x0018A450
		private void Awake()
		{
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				int index = i;
				this.m_bindingButtons[i].onClick.AddListener(delegate()
				{
					this.ActivateBindClicked(index);
				});
			}
			this.m_resetButton.onClick.AddListener(new UnityAction(this.ResetToDefaultsClicked));
			this.m_unbindButton.onClick.AddListener(new UnityAction(this.UnbindClicked));
		}

		// Token: 0x0600405D RID: 16477 RVA: 0x0018C2DC File Offset: 0x0018A4DC
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				this.m_bindingButtons[i].onClick.RemoveAllListeners();
			}
			this.m_resetButton.onClick.RemoveListener(new UnityAction(this.ResetToDefaultsClicked));
			this.m_unbindButton.onClick.RemoveListener(new UnityAction(this.UnbindClicked));
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x0018C348 File Offset: 0x0018A548
		public void Bind(int bindIndex, string elementName)
		{
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				if (i == bindIndex)
				{
					this.m_bindingButtons[i].text = elementName;
				}
			}
		}

		// Token: 0x0600405F RID: 16479 RVA: 0x0018C37C File Offset: 0x0018A57C
		public void Unbind(int indexToUnbind)
		{
			if (indexToUnbind == -1)
			{
				return;
			}
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				if (i == indexToUnbind)
				{
					this.m_bindingButtons[i].text = "Unbound";
				}
				else if (i > indexToUnbind)
				{
					this.m_bindingButtons[i - 1].text = this.m_bindingButtons[i].text;
					this.m_bindingButtons[i].text = "Unbound";
				}
			}
		}

		// Token: 0x06004060 RID: 16480 RVA: 0x0018C3EC File Offset: 0x0018A5EC
		public void Unbind()
		{
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				this.m_bindingButtons[i].text = "Unbound";
			}
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x0018C420 File Offset: 0x0018A620
		public void SetElementMaps(ActionElementMap[] elementMaps)
		{
			for (int i = 0; i < this.m_bindingButtons.Length; i++)
			{
				if (i < elementMaps.Length && elementMaps[i] != null)
				{
					this.m_bindingButtons[i].text = elementMaps[i].elementIdentifierName;
				}
				else
				{
					this.m_bindingButtons[i].text = "Unbound";
				}
			}
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x0006B92C File Offset: 0x00069B2C
		private void ActivateBindClicked(int bindingIndex)
		{
			Action<int, int> bindActivated = this.BindActivated;
			if (bindActivated == null)
			{
				return;
			}
			bindActivated(this.ActionId, bindingIndex);
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x0006B945 File Offset: 0x00069B45
		private void ResetToDefaultsClicked()
		{
			Action<int> resetToDefaults = this.ResetToDefaults;
			if (resetToDefaults == null)
			{
				return;
			}
			resetToDefaults(this.ActionId);
		}

		// Token: 0x06004064 RID: 16484 RVA: 0x0006B95D File Offset: 0x00069B5D
		private void UnbindClicked()
		{
			Action<int> unbindAction = this.UnbindAction;
			if (unbindAction == null)
			{
				return;
			}
			unbindAction(this.ActionId);
		}

		// Token: 0x04003E26 RID: 15910
		[SerializeField]
		private TextMeshProUGUI m_actionLabel;

		// Token: 0x04003E27 RID: 15911
		[SerializeField]
		private SolButton m_resetButton;

		// Token: 0x04003E28 RID: 15912
		[SerializeField]
		private SolButton m_unbindButton;

		// Token: 0x04003E29 RID: 15913
		[SerializeField]
		private SolButton[] m_bindingButtons;

		// Token: 0x04003E2B RID: 15915
		private const string kNullBindingName = "Unbound";
	}
}
