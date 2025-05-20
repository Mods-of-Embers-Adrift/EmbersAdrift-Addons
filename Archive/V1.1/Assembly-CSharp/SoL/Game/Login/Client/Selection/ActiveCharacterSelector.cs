using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B3E RID: 2878
	public class ActiveCharacterSelector : MonoBehaviour
	{
		// Token: 0x170014BA RID: 5306
		// (get) Token: 0x0600586D RID: 22637 RVA: 0x0007B18D File Offset: 0x0007938D
		public bool IsLocked
		{
			get
			{
				return this.m_locked;
			}
		}

		// Token: 0x170014BB RID: 5307
		// (get) Token: 0x0600586E RID: 22638 RVA: 0x0007B195 File Offset: 0x00079395
		public bool IsSelected
		{
			get
			{
				return this.m_toggle && this.m_toggle.isOn;
			}
		}

		// Token: 0x170014BC RID: 5308
		// (get) Token: 0x0600586F RID: 22639 RVA: 0x0007B1B1 File Offset: 0x000793B1
		public CharacterRecord Record
		{
			get
			{
				return this.m_record;
			}
		}

		// Token: 0x06005870 RID: 22640 RVA: 0x001E5E94 File Offset: 0x001E4094
		private void Awake()
		{
			this.m_toggle.isOn = false;
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
			this.m_defaultLabelColor = this.m_label.color;
			Color defaultLabelColor = this.m_defaultLabelColor;
			defaultLabelColor.a *= 0.5f;
			this.m_unselectableLabelColor = defaultLabelColor;
		}

		// Token: 0x06005871 RID: 22641 RVA: 0x0007B1B9 File Offset: 0x000793B9
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x06005872 RID: 22642 RVA: 0x0007B1D7 File Offset: 0x000793D7
		private void OnValueChanged(bool arg0)
		{
			this.m_controller.RefreshCount();
		}

		// Token: 0x06005873 RID: 22643 RVA: 0x001E5EF8 File Offset: 0x001E40F8
		public void CountChanged(bool capMet)
		{
			if (this.m_locked)
			{
				return;
			}
			bool flag = this.IsSelected || !capMet;
			this.m_toggle.interactable = flag;
			this.m_label.color = (flag ? this.m_defaultLabelColor : this.m_unselectableLabelColor);
			this.m_roleIconCanvasGroup.alpha = (flag ? 1f : 0.25f);
		}

		// Token: 0x06005874 RID: 22644 RVA: 0x001E5F60 File Offset: 0x001E4160
		public void Init(SelectActiveCharactersDialog controller, CharacterRecord record)
		{
			if (SessionData.User == null)
			{
				return;
			}
			if (record == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_locked = false;
			this.m_controller = controller;
			this.m_record = record;
			this.SetLabelText();
			base.gameObject.SetActive(true);
			if (SessionData.User.ActiveCharacters != null)
			{
				for (int i = 0; i < SessionData.User.ActiveCharacters.Length; i++)
				{
					if (record.Id == SessionData.User.ActiveCharacters[i])
					{
						this.m_toggle.isOn = true;
						this.m_toggle.interactable = false;
						this.m_locked = true;
						return;
					}
				}
			}
			this.m_toggle.isOn = false;
		}

		// Token: 0x06005875 RID: 22645 RVA: 0x001E6014 File Offset: 0x001E4214
		private void SetLabelText()
		{
			CharacterRecordExtensions.LoginRoleData loginRoleData;
			if (this.m_record != null && this.m_record.TryGetLoginRoleData(out loginRoleData))
			{
				this.m_roleIcon.overrideSprite = loginRoleData.Icon;
				this.m_label.SetTextFormat("{0} (Level {1} {2})", this.m_record.Name, loginRoleData.Level, loginRoleData.Name);
				return;
			}
			this.m_roleIcon.overrideSprite = null;
			this.m_label.text = string.Empty;
		}

		// Token: 0x04004DD1 RID: 19921
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004DD2 RID: 19922
		[SerializeField]
		private Image m_roleIcon;

		// Token: 0x04004DD3 RID: 19923
		[SerializeField]
		private CanvasGroup m_roleIconCanvasGroup;

		// Token: 0x04004DD4 RID: 19924
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004DD5 RID: 19925
		private SelectActiveCharactersDialog m_controller;

		// Token: 0x04004DD6 RID: 19926
		private CharacterRecord m_record;

		// Token: 0x04004DD7 RID: 19927
		private bool m_locked;

		// Token: 0x04004DD8 RID: 19928
		private Color m_defaultLabelColor;

		// Token: 0x04004DD9 RID: 19929
		private Color m_unselectableLabelColor;
	}
}
