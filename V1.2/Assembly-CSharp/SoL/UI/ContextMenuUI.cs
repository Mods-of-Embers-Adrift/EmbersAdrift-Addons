using System;
using System.Collections.Generic;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000348 RID: 840
	public class ContextMenuUI : DynamicUIWindow
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x000520A3 File Offset: 0x000502A3
		public static void AddContextAction(string text, bool enabled, Action callback, List<ContextMenuAction> nestedActions = null, Func<bool> interactiveCheck = null)
		{
			ContextMenuAction fromPool = StaticPool<ContextMenuAction>.GetFromPool();
			fromPool.Text = text;
			fromPool.Enabled = enabled;
			fromPool.Callback = callback;
			fromPool.NestedActions = nestedActions;
			fromPool.InteractiveCheck = interactiveCheck;
			ContextMenuUI.AddContextAction(fromPool);
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x000520D3 File Offset: 0x000502D3
		public static void AddContextAction(ContextMenuAction action)
		{
			ContextMenuUI.ActionList.Add(action);
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x00101710 File Offset: 0x000FF910
		public static void ClearContextActions()
		{
			for (int i = 0; i < ContextMenuUI.ActionList.Count; i++)
			{
				StaticPool<ContextMenuAction>.ReturnToPool(ContextMenuUI.ActionList[i]);
				ContextMenuUI.ActionList[i] = null;
			}
			ContextMenuUI.ActionList.Clear();
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x000520E0 File Offset: 0x000502E0
		public static ContextMenuAction GetContextMenuAction()
		{
			return StaticPool<ContextMenuAction>.GetFromPool();
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x00101758 File Offset: 0x000FF958
		protected override void Start()
		{
			base.Start();
			if (this.m_title != null)
			{
				this.m_title.text = null;
			}
			if (this.m_nestedContextMenu != null)
			{
				this.m_nestedContextMenu.SetParentContextMenu(this);
			}
			this.Hide(true);
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x001017A8 File Offset: 0x000FF9A8
		private void Update()
		{
			if (this.m_state != UIWindow.UIWindowState.Shown)
			{
				return;
			}
			if (Input.GetMouseButtonUp(0) && !base.CursorInside && !this.PointerInsideButtons() && this.m_parentContextMenu == null)
			{
				this.Hide(false);
				return;
			}
			for (int i = 0; i < this.m_contextMenuButtons.Count; i++)
			{
				if (this.m_contextMenuButtons[i] && this.m_contextMenuButtons[i].gameObject.activeSelf)
				{
					this.m_contextMenuButtons[i].UpdateInteractive();
				}
			}
			if (this.m_nestedContextMenu == null || this.m_nestedCaller == null)
			{
				return;
			}
			if (!this.m_nestedCaller.CursorInside && !this.PointerInsideButtons())
			{
				this.m_nestedContextMenu.Hide(false);
				this.m_nestedCaller = null;
			}
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x00101884 File Offset: 0x000FFA84
		private void InitializeNewContextMenuButton()
		{
			SolContextMenuButton component = UnityEngine.Object.Instantiate<GameObject>(this.m_contextMenuButtonPrefab, this.m_content).GetComponent<SolContextMenuButton>();
			component.text = null;
			component.interactable = false;
			component.SetContextMenu(this);
			if (!this.m_contextMenuButtons.Contains(component))
			{
				this.m_contextMenuButtons.Add(component);
			}
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x000520E7 File Offset: 0x000502E7
		public void SetParentContextMenu(ContextMenuUI parentMenu)
		{
			this.m_parentContextMenu = parentMenu;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x001018D8 File Offset: 0x000FFAD8
		public bool PointerInsideButtons()
		{
			for (int i = 0; i < this.m_contextMenuButtons.Count; i++)
			{
				if (this.m_contextMenuButtons[i].CursorInside)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x00101914 File Offset: 0x000FFB14
		public void Init(List<ContextMenuAction> actions, string title)
		{
			if (this.m_title != null)
			{
				this.m_title.text = title;
			}
			for (int i = 0; i < actions.Count; i++)
			{
				if (i > this.m_contextMenuButtons.Count - 1)
				{
					this.InitializeNewContextMenuButton();
				}
				this.m_contextMenuButtons[i].SetCurrentAction(actions[i]);
				this.m_contextMenuButtons[i].gameObject.SetActive(true);
			}
			if (this.m_contextMenuButtons.Count > actions.Count)
			{
				for (int j = actions.Count; j < this.m_contextMenuButtons.Count; j++)
				{
					this.m_contextMenuButtons[j].gameObject.SetActive(false);
				}
			}
			base.gameObject.transform.position = Input.mousePosition + ContextMenuUI.m_contextOffset;
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.RectTransform);
			base.RectTransform.ClampToScreen();
			this.Show(false);
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x00101A14 File Offset: 0x000FFC14
		public void Init(string title)
		{
			this.Init(ContextMenuUI.ActionList, title);
			ClientGameManager.UIManager.PlayClip(GlobalSettings.Values.Audio.DefaultClickClip, null, null);
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x00101A58 File Offset: 0x000FFC58
		public void InitNested(SolContextMenuButton caller, List<ContextMenuAction> actions)
		{
			this.m_nestedCaller = caller;
			this.m_nestedContextMenu.gameObject.transform.SetParent(caller.transform);
			this.m_nestedContextMenu.Init(actions, null);
			base.RectTransform.GetWorldCorners(ContextMenuUI.m_corners);
			if (ContextMenuUI.m_corners[2].x + base.RectTransform.rect.width > (float)Screen.width)
			{
				this.m_nestedContextMenu.RectTransform.anchorMin = new Vector2(0f, 1f);
				this.m_nestedContextMenu.RectTransform.anchorMax = new Vector2(0f, 1f);
				this.m_nestedContextMenu.RectTransform.pivot = new Vector2(1f, 1f);
			}
			else
			{
				this.m_nestedContextMenu.RectTransform.anchorMin = new Vector2(1f, 1f);
				this.m_nestedContextMenu.RectTransform.anchorMax = new Vector2(1f, 1f);
				this.m_nestedContextMenu.RectTransform.pivot = new Vector2(0f, 1f);
			}
			this.m_nestedContextMenu.RectTransform.anchoredPosition = Vector2.zero;
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x000520F0 File Offset: 0x000502F0
		public void OptionSelected()
		{
			this.Hide(false);
			if (this.m_parentContextMenu != null)
			{
				this.m_parentContextMenu.Hide(false);
			}
		}

		// Token: 0x04001EB7 RID: 7863
		private static Vector3[] m_corners = new Vector3[4];

		// Token: 0x04001EB8 RID: 7864
		public static readonly List<ContextMenuAction> ActionList = new List<ContextMenuAction>();

		// Token: 0x04001EB9 RID: 7865
		private static Vector3 m_contextOffset = new Vector3(-10f, 10f, 0f);

		// Token: 0x04001EBA RID: 7866
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04001EBB RID: 7867
		[SerializeField]
		private ContextMenuUI m_nestedContextMenu;

		// Token: 0x04001EBC RID: 7868
		[SerializeField]
		private GameObject m_contextMenuButtonPrefab;

		// Token: 0x04001EBD RID: 7869
		private readonly List<SolContextMenuButton> m_contextMenuButtons = new List<SolContextMenuButton>(10);

		// Token: 0x04001EBE RID: 7870
		private ContextMenuUI m_parentContextMenu;

		// Token: 0x04001EBF RID: 7871
		private SolContextMenuButton m_nestedCaller;
	}
}
