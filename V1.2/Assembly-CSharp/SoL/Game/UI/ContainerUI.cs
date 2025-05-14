using System;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200086D RID: 2157
	public abstract class ContainerUI<TKey, TValue> : MonoBehaviour, IContainerUI where TValue : ContainerSlotUI
	{
		// Token: 0x06003E73 RID: 15987
		protected abstract void InitializeSlots();

		// Token: 0x06003E74 RID: 15988 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LeftInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003E75 RID: 15989 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003E76 RID: 15990 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void MiddleInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003E77 RID: 15991 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003E78 RID: 15992 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InteractWithInstance(ArchetypeInstance instance)
		{
		}

		// Token: 0x17000E6E RID: 3694
		// (get) Token: 0x06003E79 RID: 15993 RVA: 0x0006A43B File Offset: 0x0006863B
		public bool Locked
		{
			get
			{
				return this.m_container == null || this.m_container.LockFlags > ContainerLockFlags.None;
			}
		}

		// Token: 0x17000E6F RID: 3695
		// (get) Token: 0x06003E7A RID: 15994 RVA: 0x0006A455 File Offset: 0x00068655
		public ContainerInstance ContainerInstance
		{
			get
			{
				return this.m_container;
			}
		}

		// Token: 0x17000E70 RID: 3696
		// (get) Token: 0x06003E7B RID: 15995 RVA: 0x0006A45D File Offset: 0x0006865D
		internal UIWindow Window
		{
			get
			{
				return this.m_uiWindow;
			}
		}

		// Token: 0x06003E7C RID: 15996 RVA: 0x0018546C File Offset: 0x0018366C
		protected virtual void Awake()
		{
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.WindowClosed += this.UiWindowOnWindowClosed;
				this.m_uiWindow.LockButtonPressedEvent += this.UiWindowLockButtonPressed;
			}
			if (this.m_lockOverlay != null)
			{
				this.m_lockOverlay.gameObject.SetActive(true);
				this.m_lockOverlay.CrossFadeAlpha(0f, 0f, true);
			}
		}

		// Token: 0x06003E7D RID: 15997 RVA: 0x001854EC File Offset: 0x001836EC
		protected virtual void OnDestroy()
		{
			UIManager.UnregisterContainerUI(this);
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.WindowClosed -= this.UiWindowOnWindowClosed;
				this.m_uiWindow.LockButtonPressedEvent -= this.UiWindowLockButtonPressed;
			}
			if (this.m_container != null)
			{
				this.m_container.LockFlagsChanged -= this.LockFlagsChanged;
			}
		}

		// Token: 0x06003E7E RID: 15998 RVA: 0x0006A465 File Offset: 0x00068665
		public void Initialize(ContainerInstance container)
		{
			this.m_container = container;
			this.InitializeSlots();
			UIManager.RegisterContainerUI(this);
			this.LockFlagsChanged();
			this.m_container.LockFlagsChanged += this.LockFlagsChanged;
		}

		// Token: 0x06003E7F RID: 15999 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void PostInit()
		{
		}

		// Token: 0x06003E80 RID: 16000 RVA: 0x0018555C File Offset: 0x0018375C
		public virtual void AddInstance(ArchetypeInstance instance)
		{
			TKey key = (TKey)((object)instance.Index);
			TValue tvalue;
			if (this.m_slots.TryGetValue(key, out tvalue))
			{
				tvalue.InstanceAdded(instance);
			}
		}

		// Token: 0x06003E81 RID: 16001 RVA: 0x00185598 File Offset: 0x00183798
		public virtual void RemoveInstance(ArchetypeInstance instance)
		{
			TKey key = (TKey)((object)instance.Index);
			TValue tvalue;
			if (this.m_slots.TryGetValue(key, out tvalue))
			{
				tvalue.InstanceRemoved(instance);
			}
		}

		// Token: 0x06003E82 RID: 16002 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ItemsSwapped()
		{
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x001855D4 File Offset: 0x001837D4
		public void InstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			if (this.Locked && !instance.IsAbility)
			{
				return;
			}
			switch (eventData.button)
			{
			case PointerEventData.InputButton.Left:
				if ((DateTime.UtcNow - instance.TimeOfLastClick).TotalSeconds <= (double)InteractionManager.kDoubleClickThreshold)
				{
					this.InteractWithInstance(instance);
				}
				else
				{
					this.LeftInstanceClicked(eventData, instance);
				}
				instance.TimeOfLastClick = DateTime.UtcNow;
				return;
			case PointerEventData.InputButton.Right:
				this.RightInstanceClicked(eventData, instance);
				return;
			case PointerEventData.InputButton.Middle:
				this.MiddleInstanceClicked(eventData, instance);
				return;
			default:
				return;
			}
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x00185688 File Offset: 0x00183888
		protected virtual void LockFlagsChanged()
		{
			bool flag = this.m_container.LockFlags.IsLocked();
			if (this.m_lockOverlay != null)
			{
				this.m_lockOverlay.CrossFadeAlpha(flag ? 0.7f : 0f, 0.2f, true);
			}
			if (this.m_slots != null)
			{
				for (int i = 0; i < this.m_slots.Count; i++)
				{
					if (this.m_slots[i].Instance != null && this.m_slots[i].Instance.InstanceUI != null)
					{
						this.m_slots[i].Instance.InstanceUI.ToggleLock(flag);
					}
				}
			}
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x0006A498 File Offset: 0x00068698
		private void UiWindowLockButtonPressed()
		{
			if (this.m_container == null)
			{
				return;
			}
			if (this.m_uiWindow.Locked)
			{
				this.m_container.LockFlags |= ContainerLockFlags.UI;
				return;
			}
			this.m_container.LockFlags &= ~ContainerLockFlags.UI;
		}

		// Token: 0x06003E86 RID: 16006 RVA: 0x0006A4D8 File Offset: 0x000686D8
		public void ToggleWindow()
		{
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.ToggleWindow();
			}
		}

		// Token: 0x17000E71 RID: 3697
		// (get) Token: 0x06003E87 RID: 16007 RVA: 0x0006A4F3 File Offset: 0x000686F3
		public bool IsShown
		{
			get
			{
				return this.m_uiWindow != null && this.m_uiWindow.Visible;
			}
		}

		// Token: 0x06003E88 RID: 16008 RVA: 0x0006A510 File Offset: 0x00068710
		public void ForceToggleWindow(bool show)
		{
			if (this.m_uiWindow)
			{
				if (show)
				{
					this.m_uiWindow.Show(false);
					return;
				}
				this.m_uiWindow.Hide(false);
			}
		}

		// Token: 0x06003E89 RID: 16009 RVA: 0x0006A53B File Offset: 0x0006873B
		protected virtual void UiWindowOnWindowClosed()
		{
			ContainerInstance container = this.m_container;
			if (container == null)
			{
				return;
			}
			IInteractive interactive = container.Interactive;
			if (interactive == null)
			{
				return;
			}
			interactive.EndInteraction(LocalPlayer.GameEntity, true);
		}

		// Token: 0x06003E8A RID: 16010 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		protected virtual bool TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			slotUI = null;
			return false;
		}

		// Token: 0x17000E72 RID: 3698
		// (get) Token: 0x06003E8B RID: 16011 RVA: 0x0006A55D File Offset: 0x0006875D
		string IContainerUI.Id
		{
			get
			{
				if (this.m_container != null)
				{
					return this.m_container.Id;
				}
				return null;
			}
		}

		// Token: 0x17000E73 RID: 3699
		// (get) Token: 0x06003E8C RID: 16012 RVA: 0x0006A574 File Offset: 0x00068774
		bool IContainerUI.Locked
		{
			get
			{
				return this.Locked;
			}
		}

		// Token: 0x17000E74 RID: 3700
		// (get) Token: 0x06003E8D RID: 16013 RVA: 0x0006A4F3 File Offset: 0x000686F3
		bool IContainerUI.Visible
		{
			get
			{
				return this.m_uiWindow != null && this.m_uiWindow.Visible;
			}
		}

		// Token: 0x06003E8E RID: 16014 RVA: 0x0006A57C File Offset: 0x0006877C
		void IContainerUI.CloseButtonPressed()
		{
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.CloseButtonPressed();
			}
		}

		// Token: 0x06003E8F RID: 16015 RVA: 0x0006A597 File Offset: 0x00068797
		void IContainerUI.AddInstance(ArchetypeInstance instance)
		{
			this.AddInstance(instance);
		}

		// Token: 0x06003E90 RID: 16016 RVA: 0x0006A5A0 File Offset: 0x000687A0
		void IContainerUI.RemoveInstance(ArchetypeInstance instance)
		{
			this.RemoveInstance(instance);
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x0006A5A9 File Offset: 0x000687A9
		void IContainerUI.ItemsSwapped()
		{
			this.ItemsSwapped();
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x0006A5B1 File Offset: 0x000687B1
		void IContainerUI.Initialize(ContainerInstance containerInstance)
		{
			this.Initialize(containerInstance);
		}

		// Token: 0x06003E93 RID: 16019 RVA: 0x0006A5BA File Offset: 0x000687BA
		void IContainerUI.InstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.InstanceClicked(eventData, instance);
		}

		// Token: 0x17000E75 RID: 3701
		// (get) Token: 0x06003E94 RID: 16020 RVA: 0x0006A5C4 File Offset: 0x000687C4
		ContainerInstance IContainerUI.ContainerInstance
		{
			get
			{
				return this.ContainerInstance;
			}
		}

		// Token: 0x06003E95 RID: 16021 RVA: 0x0006A5CC File Offset: 0x000687CC
		void IContainerUI.Hide()
		{
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.Hide(false);
			}
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x0006A5E8 File Offset: 0x000687E8
		void IContainerUI.Show()
		{
			if (this.m_uiWindow != null)
			{
				this.m_uiWindow.Show(false);
			}
		}

		// Token: 0x06003E97 RID: 16023 RVA: 0x0006A604 File Offset: 0x00068804
		void IContainerUI.InteractWithInstance(ArchetypeInstance instance)
		{
			this.InteractWithInstance(instance);
		}

		// Token: 0x06003E98 RID: 16024 RVA: 0x00185750 File Offset: 0x00183950
		bool IContainerUI.IsLockedWithNotification()
		{
			bool locked = this.Locked;
			if (locked && !GameManager.IsServer)
			{
				string text = "Container is locked!";
				if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.MissingBag))
				{
					text += " (missing bag)";
				}
				else if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.Combat))
				{
					text += " (in combat stance)";
					UIManager.TriggerCannotPerform("In Combat Stance!");
				}
				else if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.Trade))
				{
					text += " (in trade)";
				}
				else if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.Inspection))
				{
					text += " (inspection)";
				}
				else if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.UI))
				{
					text += " (UI locked)";
				}
				else if (this.m_container.LockFlags.HasBitFlag(ContainerLockFlags.NotAlive))
				{
					text += " (not awake)";
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text);
			}
			return locked;
		}

		// Token: 0x06003E99 RID: 16025 RVA: 0x0006A60D File Offset: 0x0006880D
		bool IContainerUI.TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			return this.TryGetContainerSlotUI(index, out slotUI);
		}

		// Token: 0x04003CA2 RID: 15522
		[SerializeField]
		private Image m_lockOverlay;

		// Token: 0x04003CA3 RID: 15523
		[SerializeField]
		protected UIWindow m_uiWindow;

		// Token: 0x04003CA4 RID: 15524
		protected DictionaryList<TKey, TValue> m_slots;

		// Token: 0x04003CA5 RID: 15525
		protected ContainerInstance m_container;
	}
}
