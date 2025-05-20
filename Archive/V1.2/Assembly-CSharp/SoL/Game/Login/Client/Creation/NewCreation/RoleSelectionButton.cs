using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B72 RID: 2930
	public class RoleSelectionButton : MonoBehaviour
	{
		// Token: 0x170014FC RID: 5372
		// (get) Token: 0x06005A2F RID: 23087 RVA: 0x0007C86D File Offset: 0x0007AA6D
		public BaseRole Role
		{
			get
			{
				return this.m_role;
			}
		}

		// Token: 0x170014FD RID: 5373
		// (get) Token: 0x06005A30 RID: 23088 RVA: 0x0007C875 File Offset: 0x0007AA75
		private bool m_showSetIconButton
		{
			get
			{
				return this.m_role != null && this.m_icon != null;
			}
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x0007C893 File Offset: 0x0007AA93
		private void Awake()
		{
			this.SetIcon();
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x0007C8B7 File Offset: 0x0007AAB7
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x0007C8D5 File Offset: 0x0007AAD5
		private void ButtonClicked()
		{
			this.m_manager.RoleSelected(this);
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x0007C8E3 File Offset: 0x0007AAE3
		public void Init(NewCharacterManager manager)
		{
			this.m_manager = manager;
			this.SetSelected(false);
		}

		// Token: 0x06005A35 RID: 23093 RVA: 0x0007C8F3 File Offset: 0x0007AAF3
		public void SetSelected(bool isSelected)
		{
			this.m_button.interactable = !isSelected;
			this.m_highlight.enabled = isSelected;
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x001EC68C File Offset: 0x001EA88C
		private void SetIcon()
		{
			if (this.m_role && this.m_icon)
			{
				this.m_icon.sprite = this.m_role.Icon;
				this.m_icon.color = this.m_role.IconTint;
			}
		}

		// Token: 0x06005A37 RID: 23095 RVA: 0x00065EE9 File Offset: 0x000640E9
		private IEnumerable GetRoles()
		{
			return SolOdinUtilities.GetDropdownItems<BaseRole>();
		}

		// Token: 0x04004F50 RID: 20304
		[SerializeField]
		private BaseRole m_role;

		// Token: 0x04004F51 RID: 20305
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04004F52 RID: 20306
		[SerializeField]
		private Image m_icon;

		// Token: 0x04004F53 RID: 20307
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04004F54 RID: 20308
		private NewCharacterManager m_manager;
	}
}
