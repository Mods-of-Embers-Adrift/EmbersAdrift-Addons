using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Audio;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C0 RID: 2496
	public class ArchetypeInstanceUI : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IArchetypeDropZone, ITooltip, IInteractiveBase, IContextMenu, IDraggable
	{
		// Token: 0x140000EC RID: 236
		// (add) Token: 0x06004BA3 RID: 19363 RVA: 0x001B95A0 File Offset: 0x001B77A0
		// (remove) Token: 0x06004BA4 RID: 19364 RVA: 0x001B95D4 File Offset: 0x001B77D4
		public static event Action<ArchetypeInstance> BeginDragEvent;

		// Token: 0x140000ED RID: 237
		// (add) Token: 0x06004BA5 RID: 19365 RVA: 0x001B9608 File Offset: 0x001B7808
		// (remove) Token: 0x06004BA6 RID: 19366 RVA: 0x001B963C File Offset: 0x001B783C
		public static event Action<ArchetypeInstance> EndDragEvent;

		// Token: 0x170010A3 RID: 4259
		// (get) Token: 0x06004BA7 RID: 19367 RVA: 0x001B9670 File Offset: 0x001B7870
		public bool Locked
		{
			get
			{
				if (this.m_instance != null)
				{
					bool flag = this.m_eventsUI != null && this.m_eventsUI.CooldownsActive();
					if (this.m_instance.IsAbility)
					{
						return flag;
					}
					if (this.m_instance.IsItem)
					{
						return (this.m_locked && this.m_instance.ItemData.Locked) || flag;
					}
				}
				return this.m_locked;
			}
		}

		// Token: 0x170010A4 RID: 4260
		// (get) Token: 0x06004BA8 RID: 19368 RVA: 0x00073364 File Offset: 0x00071564
		private bool EventCanModify
		{
			get
			{
				return this.m_eventsUI == null || this.m_eventsUI.CanModify;
			}
		}

		// Token: 0x170010A5 RID: 4261
		// (get) Token: 0x06004BA9 RID: 19369 RVA: 0x001B96DC File Offset: 0x001B78DC
		public virtual bool CanModify
		{
			get
			{
				bool eventCanModify = this.EventCanModify;
				if (this.m_instance != null && this.m_instance.IsItem)
				{
					return eventCanModify && !this.m_locked && !this.m_instance.ItemData.Locked;
				}
				return eventCanModify && !this.m_locked;
			}
		}

		// Token: 0x170010A6 RID: 4262
		// (get) Token: 0x06004BAA RID: 19370 RVA: 0x0007337B File Offset: 0x0007157B
		public ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x170010A7 RID: 4263
		// (get) Token: 0x06004BAB RID: 19371 RVA: 0x00073383 File Offset: 0x00071583
		public RectTransform RectTransform
		{
			get
			{
				return this.m_rectTransform;
			}
		}

		// Token: 0x170010A8 RID: 4264
		// (get) Token: 0x06004BAC RID: 19372 RVA: 0x0007338B File Offset: 0x0007158B
		public ContainerSlotUI SlotUI
		{
			get
			{
				return this.m_containerSlotUI;
			}
		}

		// Token: 0x170010A9 RID: 4265
		// (get) Token: 0x06004BAD RID: 19373 RVA: 0x001B9734 File Offset: 0x001B7934
		private bool RequireShiftToMove
		{
			get
			{
				if (this.m_instance != null)
				{
					if (this.m_instance.IsAbility && this.m_instance.Index >= 0)
					{
						return true;
					}
					if (this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType.RequireShiftToMove())
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x170010AA RID: 4266
		// (get) Token: 0x06004BAE RID: 19374 RVA: 0x00073393 File Offset: 0x00071593
		private bool AuraIsActive
		{
			get
			{
				return this.m_eventsUI == null || this.m_eventsUI.AuraIsActive;
			}
		}

		// Token: 0x170010AB RID: 4267
		// (get) Token: 0x06004BAF RID: 19375 RVA: 0x000733AA File Offset: 0x000715AA
		private bool AttemptToCancelAura
		{
			get
			{
				return this.m_eventsUI != null && this.m_eventsUI.AuraIsActive;
			}
		}

		// Token: 0x170010AC RID: 4268
		// (get) Token: 0x06004BB0 RID: 19376 RVA: 0x000733C1 File Offset: 0x000715C1
		internal IExecutable Executable
		{
			get
			{
				return this.m_executable;
			}
		}

		// Token: 0x170010AD RID: 4269
		// (get) Token: 0x06004BB1 RID: 19377 RVA: 0x000733C9 File Offset: 0x000715C9
		internal Image Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x170010AE RID: 4270
		// (get) Token: 0x06004BB2 RID: 19378 RVA: 0x000733D1 File Offset: 0x000715D1
		internal Image AvailableOverlay
		{
			get
			{
				return this.m_availableOverlay;
			}
		}

		// Token: 0x170010AF RID: 4271
		// (get) Token: 0x06004BB3 RID: 19379 RVA: 0x000733D9 File Offset: 0x000715D9
		internal Image DistanceAngleOverlay
		{
			get
			{
				return this.m_distanceAngleOverlay;
			}
		}

		// Token: 0x170010B0 RID: 4272
		// (get) Token: 0x06004BB4 RID: 19380 RVA: 0x000733E1 File Offset: 0x000715E1
		internal TextMeshProUGUI CenterLabel
		{
			get
			{
				return this.m_centerLabel;
			}
		}

		// Token: 0x170010B1 RID: 4273
		// (get) Token: 0x06004BB5 RID: 19381 RVA: 0x000733E9 File Offset: 0x000715E9
		internal TextMeshProUGUI LowerLeftLabel
		{
			get
			{
				return this.m_lowerLeftLabel;
			}
		}

		// Token: 0x170010B2 RID: 4274
		// (get) Token: 0x06004BB6 RID: 19382 RVA: 0x000733F1 File Offset: 0x000715F1
		internal TextMeshProUGUI LowerRightLabel
		{
			get
			{
				return this.m_lowerRightLabel;
			}
		}

		// Token: 0x170010B3 RID: 4275
		// (get) Token: 0x06004BB7 RID: 19383 RVA: 0x000733F9 File Offset: 0x000715F9
		internal TextMeshProUGUI UpperRightLabel
		{
			get
			{
				return this.m_upperRightLabel;
			}
		}

		// Token: 0x170010B4 RID: 4276
		// (get) Token: 0x06004BB8 RID: 19384 RVA: 0x00073401 File Offset: 0x00071601
		internal CanvasGroup DisabledPanel
		{
			get
			{
				return this.m_disabledPanel;
			}
		}

		// Token: 0x170010B5 RID: 4277
		// (get) Token: 0x06004BB9 RID: 19385 RVA: 0x00073409 File Offset: 0x00071609
		internal CanvasGroup LowStaminaCanvas
		{
			get
			{
				return this.m_lowStaminaCanvas;
			}
		}

		// Token: 0x170010B6 RID: 4278
		// (get) Token: 0x06004BBA RID: 19386 RVA: 0x00073411 File Offset: 0x00071611
		internal CanvasGroup LowLevelCanvas
		{
			get
			{
				return this.m_lowLevelCanvas;
			}
		}

		// Token: 0x170010B7 RID: 4279
		// (get) Token: 0x06004BBB RID: 19387 RVA: 0x00073419 File Offset: 0x00071619
		internal CanvasGroup LockedCanvas
		{
			get
			{
				return this.m_lockedCanvas;
			}
		}

		// Token: 0x170010B8 RID: 4280
		// (get) Token: 0x06004BBC RID: 19388 RVA: 0x00073421 File Offset: 0x00071621
		internal CanvasGroup BehaviorCanvas
		{
			get
			{
				return this.m_behaviorCanvas;
			}
		}

		// Token: 0x170010B9 RID: 4281
		// (get) Token: 0x06004BBD RID: 19389 RVA: 0x00073429 File Offset: 0x00071629
		// (set) Token: 0x06004BBE RID: 19390 RVA: 0x00073431 File Offset: 0x00071631
		internal TargetOverlayState TargetOverlay
		{
			get
			{
				return this.m_targetOverlay;
			}
			set
			{
				if (this.m_targetOverlay == value)
				{
					return;
				}
				this.m_targetOverlay = value;
				this.RefreshTargetOverlays();
			}
		}

		// Token: 0x06004BBF RID: 19391 RVA: 0x001B9790 File Offset: 0x001B7990
		private void RefreshTargetOverlays()
		{
			bool enabled = false;
			bool enabled2 = false;
			TargetOverlayState targetOverlay = this.TargetOverlay;
			if (targetOverlay != TargetOverlayState.Invalid)
			{
				if (targetOverlay == TargetOverlayState.DistanceAngle)
				{
					enabled2 = true;
				}
			}
			else
			{
				enabled = true;
			}
			if (this.m_invalidTargetOverlay)
			{
				this.m_invalidTargetOverlay.enabled = enabled;
			}
			if (this.m_distanceAngleOverlay)
			{
				this.m_distanceAngleOverlay.enabled = enabled2;
			}
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x0007344A File Offset: 0x0007164A
		internal void ToggleHighlight(bool isEnabled)
		{
			if (this.m_highlight)
			{
				this.m_highlight.enabled = isEnabled;
			}
		}

		// Token: 0x06004BC1 RID: 19393 RVA: 0x001B97EC File Offset: 0x001B79EC
		protected virtual void Awake()
		{
			this.m_eventsUI = this.m_eventsObject.GetComponent<IArchetypeInstanceEventsUI>();
			this.ToggleHighlight(false);
			if (this.m_availableFrame)
			{
				this.m_availableFrame.enabled = false;
			}
			if (this.m_availableOverlay)
			{
				this.m_availableOverlay.enabled = false;
			}
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x00073465 File Offset: 0x00071665
		protected virtual void OnDestroy()
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.IDraggableDestroyed(this);
			}
			IArchetypeInstanceEventsUI eventsUI = this.m_eventsUI;
			if (eventsUI == null)
			{
				return;
			}
			eventsUI.ExternalOnDestroy();
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x0007348E File Offset: 0x0007168E
		public void ExternalOnDestroy()
		{
			IArchetypeInstanceEventsUI eventsUI = this.m_eventsUI;
			if (eventsUI == null)
			{
				return;
			}
			eventsUI.ExternalOnDestroy();
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x001B9844 File Offset: 0x001B7A44
		public void Initialize(ArchetypeInstance instance)
		{
			this.ResetLabelsAndPanels();
			this.m_targetOverlay = TargetOverlayState.None;
			this.RefreshTargetOverlays();
			this.m_instance = instance;
			this.m_icon.overrideSprite = instance.Archetype.Icon;
			this.m_icon.color = instance.Archetype.GetInstanceColor(instance);
			this.m_executable = ((instance.IsAbility || instance.IsItem) ? (this.m_instance.Archetype as IExecutable) : null);
			this.m_durability.Subscribe(this.m_instance);
			this.m_augment.Subscribe(this.m_instance);
			this.m_isRecipeOrGatheringAbility = (this.m_instance.Archetype is RecipeAbility || this.m_instance.Archetype is GatheringAbility);
			if (this.m_itemCategoryFrame)
			{
				Color color;
				if (instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.IconBorder, out color))
				{
					this.m_itemCategoryFrame.color = color;
					this.m_itemCategoryFrame.enabled = true;
				}
				else
				{
					this.m_itemCategoryFrame.enabled = false;
				}
			}
			if (this.m_eventsUI == null)
			{
				throw new ArgumentException("WTF");
			}
			IArchetypeInstanceEventsUI eventsUI = this.m_eventsUI;
			if (eventsUI != null)
			{
				eventsUI.Init(this);
			}
			this.RefreshDisabledPanel();
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x001B9980 File Offset: 0x001B7B80
		internal void RefreshDisabledPanel()
		{
			float alpha = 0f;
			if (this.m_allowDisabledPanel && this.m_executable != null && this.m_instance != null)
			{
				float associatedLevel = this.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity);
				alpha = (this.m_executable.MeetsRequirementsForUI(LocalPlayer.GameEntity, associatedLevel) ? 0f : 1f);
			}
			if (this.m_disabledPanel)
			{
				this.m_disabledPanel.alpha = alpha;
			}
			IArchetypeInstanceEventsUI eventsUI = this.m_eventsUI;
			if (eventsUI == null)
			{
				return;
			}
			eventsUI.RefreshDisabledPanel();
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x001B9A08 File Offset: 0x001B7C08
		protected virtual void OnPointerUp(PointerEventData eventData)
		{
			bool chatWasFocusedOnPointerDown = this.m_chatWasFocusedOnPointerDown;
			this.m_chatWasFocusedOnPointerDown = false;
			if (this.m_cursorInside)
			{
				ArchetypeInstance instance = this.m_instance;
				if (((instance != null) ? instance.ContainerInstance : null) != null)
				{
					if (this.m_isRecipeOrGatheringAbility)
					{
						return;
					}
					if (ClientGameManager.UIManager.IsDragging)
					{
						return;
					}
					if (this.m_eventsUI.OverrideOnPointerUp(eventData))
					{
						return;
					}
					if (CursorManager.GameMode != CursorGameMode.None)
					{
						if (!this.CanModify)
						{
							if (this.m_containerSlotUI)
							{
								IContainerUI containerUI = this.m_containerSlotUI.ContainerUI;
								if (containerUI == null)
								{
									return;
								}
								containerUI.IsLockedWithNotification();
							}
							return;
						}
						switch (CursorManager.GameMode)
						{
						case CursorGameMode.Sell:
							this.SellRequest();
							return;
						case CursorGameMode.Repair:
							this.RepairRequest();
							return;
						case CursorGameMode.Deconstruct:
							this.DeconstructRequest();
							return;
						case CursorGameMode.UtilityItem:
							UtilityItemExtensions.OnClientUtilityItemUsage(this.m_instance);
							return;
						}
					}
					bool holdingShift = ClientGameManager.InputManager.HoldingShift;
					bool requireShiftToMove = this.RequireShiftToMove;
					if (requireShiftToMove && !holdingShift)
					{
						IUtilityItem utilityItem;
						if ((this.EventCanModify || this.AttemptToCancelAura) && this.m_instance.Archetype is IExecutable)
						{
							LocalPlayer.GameEntity.SkillsController.BeginExecution(this.m_instance);
						}
						else if (this.EventCanModify && this.m_instance.Archetype.TryGetAsType(out utilityItem) && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType.AllowConsumableUse())
						{
							UtilityItemExtensions.InitializeUtilityItemMode(this.m_instance, utilityItem);
						}
						if (this.m_instance != null && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType == ContainerType.ReagentPouch && ClientGameManager.UIManager != null && ClientGameManager.UIManager.ActionBar != null)
						{
							ClientGameManager.UIManager.ActionBar.TriggerReagentIndex(this.m_instance.Index);
						}
						return;
					}
					ContainerInstance containerInstance;
					int num;
					if (!this.CanModify)
					{
						if (this.m_containerSlotUI)
						{
							IContainerUI containerUI2 = this.m_containerSlotUI.ContainerUI;
							if (containerUI2 != null)
							{
								containerUI2.IsLockedWithNotification();
							}
						}
						if (eventData.button != PointerEventData.InputButton.Left || !holdingShift || !chatWasFocusedOnPointerDown)
						{
							return;
						}
						if (this.m_instance.IsItem)
						{
							UIManager.ActiveChatInput.AddInstanceLink(this.m_instance);
							return;
						}
						UIManager.ActiveChatInput.AddArchetypeLink(this.m_instance.Archetype);
						return;
					}
					else if (!requireShiftToMove && holdingShift && this.TryGetTargetSplitContainer(out containerInstance) && containerInstance.TryGetFirstAvailableIndex(LocalPlayer.GameEntity, out num) && !chatWasFocusedOnPointerDown)
					{
						if (!this.m_instance.ContainerInstance.CanSplitSubscriberSlotCheck(this.m_instance.Index))
						{
							UIManager.TriggerCannotPerform("Must be a subscriber to split from here!");
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Must be a subscriber to split from here!");
							return;
						}
						SelectValueOptions opts = new SelectValueOptions
						{
							ShowCloseButton = true,
							AllowDragging = false,
							BlockInteractions = false,
							Title = "Split",
							Text = ZString.Format<string>("Split {0}", this.m_instance.Archetype.GetModifiedDisplayName(this.m_instance)),
							ConfirmationText = "Ok",
							CancelText = "Cancel",
							Callback = new Action<bool, object>(this.SplitRequest),
							DefaultValue = 1,
							MinValue = 1,
							MaxValue = this.m_instance.ItemData.Count.Value - 1
						};
						ClientGameManager.UIManager.SelectValueDialog.Init(opts);
						ClientGameManager.UIManager.SelectValueDialog.RectTransform.position = this.m_instance.InstanceUI.RectTransform.position;
						ClientGameManager.UIManager.SelectValueDialog.RectTransform.ClampToScreen();
						return;
					}
					else if (eventData.button == PointerEventData.InputButton.Left && holdingShift && !requireShiftToMove && chatWasFocusedOnPointerDown)
					{
						if (this.m_instance.IsItem)
						{
							UIManager.ActiveChatInput.AddInstanceLink(this.m_instance);
							return;
						}
						UIManager.ActiveChatInput.AddArchetypeLink(this.m_instance.Archetype);
						return;
					}
					else
					{
						PointerEventData.InputButton button = eventData.button;
						if (button == PointerEventData.InputButton.Left)
						{
							this.AttachToCursor(false);
							return;
						}
						if (button != PointerEventData.InputButton.Right)
						{
							return;
						}
						if (!requireShiftToMove)
						{
							IContainerUI containerUI3 = this.m_instance.ContainerInstance.ContainerUI;
							if (containerUI3 == null)
							{
								return;
							}
							containerUI3.InstanceClicked(eventData, this.m_instance);
						}
						return;
					}
				}
			}
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x000734A0 File Offset: 0x000716A0
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.AttachToCursor(true);
			}
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x0004475B File Offset: 0x0004295B
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x0004475B File Offset: 0x0004295B
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
		}

		// Token: 0x170010BA RID: 4282
		// (get) Token: 0x06004BCA RID: 19402 RVA: 0x000734B1 File Offset: 0x000716B1
		// (set) Token: 0x06004BCB RID: 19403 RVA: 0x000734B8 File Offset: 0x000716B8
		public static ArchetypeInstance HoveredInstance { get; private set; }

		// Token: 0x06004BCC RID: 19404 RVA: 0x000734C0 File Offset: 0x000716C0
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			this.OnPointerUp(eventData);
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x000734C9 File Offset: 0x000716C9
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocusedOnPointerDown = UIManager.IsChatActive;
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x000734D6 File Offset: 0x000716D6
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_cursorInside = true;
			ArchetypeInstanceUI.HoveredInstance = this.m_instance;
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x000734EA File Offset: 0x000716EA
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_cursorInside = false;
			if (ArchetypeInstanceUI.HoveredInstance == this.m_instance)
			{
				ArchetypeInstanceUI.HoveredInstance = null;
			}
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x001B9E4C File Offset: 0x001B804C
		private void AttachToCursor(bool checkConditions)
		{
			if (checkConditions)
			{
				ArchetypeInstance instance = this.m_instance;
				if (((instance != null) ? instance.ContainerInstance : null) == null)
				{
					return;
				}
				if (this.m_isRecipeOrGatheringAbility)
				{
					return;
				}
				if (ClientGameManager.UIManager.IsDragging)
				{
					return;
				}
				bool holdingShift = ClientGameManager.InputManager.HoldingShift;
				if (this.RequireShiftToMove && !holdingShift)
				{
					return;
				}
				if (!this.CanModify)
				{
					if (this.m_containerSlotUI)
					{
						IContainerUI containerUI = this.m_containerSlotUI.ContainerUI;
						if (containerUI == null)
						{
							return;
						}
						containerUI.IsLockedWithNotification();
					}
					return;
				}
			}
			if (ClientGameManager.UIManager.DragShadow)
			{
				ClientGameManager.UIManager.DragShadow.Enable(this);
			}
			this.m_previousPivot = new Vector2?(this.m_rectTransform.pivot);
			this.m_rectTransform.SetPivot(Vector2.up);
			this.m_canvasGroup.blocksRaycasts = false;
			ClientGameManager.UIManager.RegisterDrag(this);
			Action<ArchetypeInstance> beginDragEvent = ArchetypeInstanceUI.BeginDragEvent;
			if (beginDragEvent != null)
			{
				beginDragEvent(this.m_instance);
			}
			this.PlayDragDropAudio();
		}

		// Token: 0x170010BB RID: 4283
		// (get) Token: 0x06004BD1 RID: 19409 RVA: 0x0004479C File Offset: 0x0004299C
		bool IDraggable.ExternallyHandlePositionUpdate
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004BD2 RID: 19410 RVA: 0x001B9F44 File Offset: 0x001B8144
		void IDraggable.CompleteDrag(bool canceled)
		{
			if (this.m_previousPivot != null && this.m_rectTransform)
			{
				this.m_rectTransform.SetPivot(this.m_previousPivot.Value);
				this.m_previousPivot = null;
			}
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.DragShadow)
			{
				ClientGameManager.UIManager.DragShadow.Disable(this);
			}
			if (this.m_canvasGroup)
			{
				this.m_canvasGroup.blocksRaycasts = true;
			}
			Action<ArchetypeInstance> endDragEvent = ArchetypeInstanceUI.EndDragEvent;
			if (endDragEvent != null)
			{
				endDragEvent(this.m_instance);
			}
			this.PlayDragDropAudio();
			if (canceled)
			{
				this.ResetUI();
				return;
			}
			this.CheckForAndProcessDropZone();
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x001BA000 File Offset: 0x001B8200
		internal void PlayDragDropAudio()
		{
			if (this.m_instance == null || this.m_instance.Archetype == null)
			{
				return;
			}
			AudioClipCollection collection = (this.m_instance.Archetype.DragDropAudio == null) ? GlobalSettings.Values.Audio.DefaultDragDropClipCollection : this.m_instance.Archetype.DragDropAudio;
			ClientGameManager.UIManager.PlayRandomClip(collection, null);
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x001BA078 File Offset: 0x001B8278
		private void ResetLabelsAndPanels()
		{
			this.m_centerLabel.text = null;
			this.m_lowerLeftLabel.text = null;
			this.m_lowerRightLabel.text = null;
			this.m_upperLeftLabel.text = null;
			this.m_upperRightLabel.text = null;
			this.m_lowStaminaCanvas.alpha = 0f;
			this.m_lowLevelCanvas.alpha = 0f;
			this.m_lowStatCanvas.alpha = 0f;
			if (this.m_lockedCanvas)
			{
				this.m_lockedCanvas.alpha = 0f;
			}
			if (this.m_behaviorCanvas)
			{
				this.m_behaviorCanvas.alpha = 0f;
			}
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x00073506 File Offset: 0x00071706
		public void ToggleLock(bool locked)
		{
			this.m_locked = locked;
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x0007350F File Offset: 0x0007170F
		public void ResetUI()
		{
			this.AssignSlotUI(this.m_containerSlotUI);
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x001BA12C File Offset: 0x001B832C
		private void ResetState()
		{
			this.ResetPivot();
			this.m_rectTransform.SetParent(this.m_containerSlotUI ? this.m_containerSlotUI.RectTransform : null);
			this.m_rectTransform.anchoredPosition = Vector2.zero;
			this.m_rectTransform.localScale = Vector3.one;
			this.m_canvasGroup.blocksRaycasts = true;
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x001BA194 File Offset: 0x001B8394
		private void CheckForAndProcessDropZone()
		{
			IArchetypeDropZone archetypeDropZone = null;
			GameObject hoveredUIElement = InteractionManager.HoveredUIElement;
			bool flag = hoveredUIElement != null;
			if (flag)
			{
				archetypeDropZone = hoveredUIElement.GetComponent<IArchetypeDropZone>();
			}
			else if (ClientGameManager.MainCamera)
			{
				Ray ray = ClientGameManager.MainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Hits.Hits100;
				int num = Physics.RaycastNonAlloc(ray, hits, 10f);
				for (int i = 0; i < num; i++)
				{
					archetypeDropZone = hits[i].collider.gameObject.GetComponent<IArchetypeDropZone>();
					if (archetypeDropZone != null)
					{
						break;
					}
				}
			}
			bool flag2 = true;
			if (archetypeDropZone != null && archetypeDropZone.ContainerUI != null && !archetypeDropZone.ContainerUI.IsLockedWithNotification() && archetypeDropZone.CanPlace(this.m_instance, archetypeDropZone.Index))
			{
				if (archetypeDropZone.CurrentOccupant != null)
				{
					if (archetypeDropZone.CurrentOccupant != this.m_instance)
					{
						if (archetypeDropZone.CurrentOccupant.CanMergeWith(this.m_instance))
						{
							MergeRequest request = new MergeRequest
							{
								TransactionId = UniqueId.GenerateFromGuid(),
								SourceInstanceId = this.m_instance.InstanceId,
								TargetInstanceId = archetypeDropZone.CurrentOccupant.InstanceId,
								SourceContainer = this.m_instance.ContainerInstance.Id,
								TargetContainer = archetypeDropZone.CurrentOccupant.ContainerInstance.Id,
								LocalSourceDisplayName = (this.m_instance.Archetype ? this.m_instance.Archetype.GetModifiedDisplayName(this.m_instance) : string.Empty),
								LocalSourceQuantity = ((this.m_instance.ItemData != null) ? this.m_instance.ItemData.Quantity : null)
							};
							Debug.Log("Merge Request " + request.TransactionId.ToString() + " on " + archetypeDropZone.GO.name);
							LocalPlayer.NetworkEntity.PlayerRpcHandler.MergeRequest(request);
							flag2 = false;
						}
						else if (this.m_instance.ContainerInstance.CanPlace(archetypeDropZone.CurrentOccupant, this.m_instance.Index))
						{
							SwapRequest request2 = new SwapRequest
							{
								TransactionId = UniqueId.GenerateFromGuid(),
								InstanceIdA = this.m_instance.InstanceId,
								InstanceIdB = archetypeDropZone.CurrentOccupant.InstanceId,
								SourceContainerA = this.m_instance.ContainerInstance.Id,
								SourceContainerB = archetypeDropZone.CurrentOccupant.ContainerInstance.Id
							};
							Debug.Log("Swap Request " + request2.TransactionId.ToString() + " on " + archetypeDropZone.GO.name);
							LocalPlayer.NetworkEntity.PlayerRpcHandler.SwapRequest(request2);
							flag2 = false;
						}
					}
				}
				else
				{
					TransferRequest request3 = new TransferRequest
					{
						TransactionId = UniqueId.GenerateFromGuid(),
						InstanceId = this.m_instance.InstanceId,
						SourceContainer = this.m_instance.ContainerInstance.Id,
						TargetContainer = archetypeDropZone.ContainerUI.Id,
						TargetIndex = archetypeDropZone.Index,
						Instance = this.m_instance
					};
					Debug.Log("Transfer Request " + request3.TransactionId.ToString() + " on " + archetypeDropZone.GO.name);
					LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request3);
					flag2 = false;
				}
			}
			if (flag2)
			{
				this.ResetUI();
			}
			if (archetypeDropZone == null && !flag && this.m_instance != null)
			{
				if (this.m_instance.IsAbility)
				{
					this.UnlearnAbility();
					return;
				}
				if (this.m_instance.IsItem)
				{
					this.AttemptToDestroyItem();
				}
			}
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x001BA568 File Offset: 0x001B8768
		public void AssignSlotUI(ContainerSlotUI containerSlotUI)
		{
			bool flag = this.IsContainerVisible(this.m_containerSlotUI);
			this.m_containerSlotUI = containerSlotUI;
			bool flag2 = this.IsContainerVisible(this.m_containerSlotUI);
			if (flag && flag2 && base.gameObject.activeInHierarchy)
			{
				if (this.m_repositionCo != null)
				{
					base.StopCoroutine(this.m_repositionCo);
				}
				this.m_repositionCo = this.RepositionCo();
				base.StartCoroutine(this.m_repositionCo);
			}
			else
			{
				this.ResetState();
			}
			if (this.m_eventsUI != null && this.m_eventsUI.ContextualDisabledPanel)
			{
				bool flag3 = true;
				if (this.m_instance.IsAbility)
				{
					flag3 = (this.m_instance.Index != -1);
				}
				else if (this.m_instance.IsItem && this.m_instance.ContainerInstance != null)
				{
					flag3 = this.m_instance.ContainerInstance.ContainerType.IsLocal();
				}
				if (flag3 != this.m_allowDisabledPanel)
				{
					this.m_allowDisabledPanel = flag3;
					this.RefreshDisabledPanel();
				}
			}
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x001BA65C File Offset: 0x001B885C
		private bool IsContainerVisible(ContainerSlotUI containerSlotUI)
		{
			bool result = false;
			if (containerSlotUI != null && containerSlotUI.ContainerUI != null && containerSlotUI.ContainerUI.ContainerInstance != null)
			{
				ContainerType containerType = containerSlotUI.ContainerUI.ContainerInstance.ContainerType;
				if (containerType - ContainerType.Pouch <= 1)
				{
					return true;
				}
				if (containerType == ContainerType.Abilities)
				{
					return this.m_instance != null && this.m_instance.Index >= 0;
				}
				IContainerUI containerUI;
				result = (UIManager.TryGetContainerUI(containerSlotUI.ContainerUI.ContainerInstance.Id, out containerUI) && containerUI.Visible);
			}
			return result;
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x0007351D File Offset: 0x0007171D
		private IEnumerator RepositionCo()
		{
			this.ResetPivot();
			this.m_rectTransform.SetParent(ClientGameManager.UIManager.DragPanel);
			float t = 0f;
			Vector3 startPos = this.m_rectTransform.position;
			Vector3 endPos = this.m_containerSlotUI.RectTransform.position;
			while (t <= 0.15f)
			{
				float t2 = t / 0.15f;
				this.m_rectTransform.position = Vector3.Lerp(startPos, endPos, t2);
				t += Time.deltaTime;
				yield return null;
			}
			this.m_rectTransform.position = endPos;
			this.ResetState();
			this.m_repositionCo = null;
			yield break;
		}

		// Token: 0x170010BC RID: 4284
		// (get) Token: 0x06004BDC RID: 19420 RVA: 0x0007352C File Offset: 0x0007172C
		public int Index
		{
			get
			{
				return this.m_instance.Index;
			}
		}

		// Token: 0x170010BD RID: 4285
		// (get) Token: 0x06004BDD RID: 19421 RVA: 0x0007337B File Offset: 0x0007157B
		public ArchetypeInstance CurrentOccupant
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x170010BE RID: 4286
		// (get) Token: 0x06004BDE RID: 19422 RVA: 0x00073539 File Offset: 0x00071739
		public IContainerUI ContainerUI
		{
			get
			{
				if (this.m_instance == null || this.m_instance.ContainerInstance == null)
				{
					return null;
				}
				return this.m_instance.ContainerInstance.ContainerUI;
			}
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x001BA6EC File Offset: 0x001B88EC
		public bool CanPlace(ArchetypeInstance instance, int targetIndex)
		{
			return this.m_instance != null && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.CanPlace(instance, targetIndex) && (this.m_eventsUI == null || this.m_eventsUI.CanPlace);
		}

		// Token: 0x170010BF RID: 4287
		// (get) Token: 0x06004BE0 RID: 19424 RVA: 0x00052028 File Offset: 0x00050228
		public GameObject GO
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x001BA73C File Offset: 0x001B893C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance == null)
			{
				return null;
			}
			if (Options.GameOptions.DisableTooltipForActionBarAbilities.Value && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType == ContainerType.Abilities && this.m_instance.Index != -1)
			{
				return null;
			}
			string text = string.Empty;
			if (this.RequireShiftToMove)
			{
				text = "<i>Shift+Left Click to move</i>";
			}
			else if (this.CanSplit())
			{
				text = "<i>Shift+Left Click to split stack</i>";
				if (this.m_instance.ContainerInstance != null && !this.m_instance.ContainerInstance.CanSplitSubscriberSlotCheck(this.m_instance.Index))
				{
					text = ZString.Format<string, string>("<color={0}>{1}</color>", UIManager.SubscriberColor.ToHex(), text);
				}
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance,
				SubHeadingText = text
			};
		}

		// Token: 0x170010C0 RID: 4288
		// (get) Token: 0x06004BE2 RID: 19426 RVA: 0x00073562 File Offset: 0x00071762
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010C1 RID: 4289
		// (get) Token: 0x06004BE3 RID: 19427 RVA: 0x001BA818 File Offset: 0x001B8A18
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return default(TooltipSettings);
			}
		}

		// Token: 0x170010C2 RID: 4290
		// (get) Token: 0x06004BE4 RID: 19428 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x001BA830 File Offset: 0x001B8A30
		public virtual string FillActionsGetTitle()
		{
			string result;
			if (this.m_eventsUI != null && this.m_eventsUI.OverrideFillActionsGetTitle(out result))
			{
				return result;
			}
			if (this.m_instance == null || this.m_instance.ContainerInstance == null || !this.m_instance.ContainerInstance.ContainerType.ShowContextMenuForRightClick())
			{
				return null;
			}
			bool flag = this.m_instance.ContainerInstance.LockFlags.IsLocked() || !this.CanModify;
			if (this.m_instance.IsItem)
			{
				if (this.m_instance.ContainerInstance != null)
				{
					if (this.m_instance.Archetype is ConsumableItem)
					{
						ContextMenuUI.AddContextAction("Use Item", !flag, new Action(this.UseItem), null, null);
					}
					EquipableItem equipableItem;
					if (this.m_instance.Archetype.TryGetAsType(out equipableItem))
					{
						if (this.m_instance.ContainerInstance.ContainerType == ContainerType.Equipment)
						{
							if (equipableItem.Type == EquipmentType.Head)
							{
								ContextMenuUI.AddContextAction(LocalPlayer.GameEntity.CharacterData.HideHelm ? "Show Helm" : "Hide Helm", true, new Action(this.ToggleHideHelm), null, null);
							}
							ContextMenuUI.AddContextAction("Unequip", !flag, new Action(this.InteractWithInstance), null, null);
						}
						else if (this.m_instance.ContainerInstance.ContainerType == ContainerType.Inventory)
						{
							ContextMenuUI.AddContextAction("Equip", !flag, new Action(this.InteractWithInstance), null, null);
						}
					}
					if (this.m_instance.ContainerInstance.CanDestroyItem())
					{
						string text = "Destroy Item";
						bool flag2 = !flag && this.m_instance.ItemData.ItemFlags.AllowDestruction();
						if (!flag2)
						{
							text = text.Strikethrough();
						}
						ContextMenuUI.AddContextAction(text, flag2, new Action(this.DestroyItemConfirmation), null, null);
					}
					return this.m_instance.Archetype.DisplayName ?? "";
				}
			}
			else if (this.m_instance.IsAbility)
			{
				if (!false)
				{
					return null;
				}
				return this.m_instance.Archetype.DisplayName ?? "";
			}
			return null;
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x00073570 File Offset: 0x00071770
		string IContextMenu.FillActionsGetTitle()
		{
			return this.FillActionsGetTitle();
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x00073578 File Offset: 0x00071778
		protected void ResetPivot()
		{
			this.m_previousPivot = null;
			this.m_rectTransform.SetPivot(ArchetypeInstanceUI.kDefaultPivot);
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00073596 File Offset: 0x00071796
		private void InteractWithInstance()
		{
			if (LocalPlayer.GameEntity.Vitals.Stance != Stance.Combat)
			{
				this.m_instance.ContainerInstance.ContainerUI.InteractWithInstance(this.m_instance);
			}
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x001BAA48 File Offset: 0x001B8C48
		private void SplitRequest(bool accept, object obj)
		{
			if (accept)
			{
				ContainerInstance containerInstance;
				int num;
				if (this.TryGetTargetSplitContainer(out containerInstance) && containerInstance.TryGetFirstAvailableIndex(LocalPlayer.GameEntity, out num))
				{
					float num2 = (float)obj;
					SplitRequest request = new SplitRequest
					{
						TransactionId = UniqueId.GenerateFromGuid(),
						InstanceId = this.m_instance.InstanceId,
						SourceContainer = this.m_instance.ContainerInstance.Id,
						TargetContainer = containerInstance.Id,
						SplitCount = (int)num2
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.SplitRequest(request);
					return;
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Invalid Split Request!");
			}
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x001BAAF4 File Offset: 0x001B8CF4
		private bool CanSell()
		{
			InteractiveMerchant interactiveMerchant;
			return CursorManager.GameMode == CursorGameMode.Sell && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Inventory != null && LocalPlayer.GameEntity.CollectionController.InteractiveStation != null && LocalPlayer.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveMerchant) && this.Instance != null && this.Instance.ItemData != null && this.Instance.ItemData.AssociatedMasteryId == null && this.Instance.ContainerInstance != null && this.Instance.ContainerInstance.ContainerType.CanSellFrom();
		}

		// Token: 0x06004BEB RID: 19435 RVA: 0x001BABBC File Offset: 0x001B8DBC
		private bool SellRequest()
		{
			if (this.CanSell())
			{
				if (this.Instance.GetSellPrice() <= 0UL)
				{
					ClientGameManager.CombatTextManager.InitializeOverheadCombatText("Cannot be sold!", LocalPlayer.GameEntity, Color.yellow, null);
				}
				else if (this.Instance.ContainerInstance.ContainerType == ContainerType.Equipment)
				{
					string title;
					string text;
					if (this.Instance.ItemData != null && this.Instance.ItemData.IsSoulbound)
					{
						title = "Sell Equipped Soulbound Item";
						text = ZString.Format<string, string, string, string>("{0} {1} this {2} item which is currently {3}?", "Are you sure you want to", ArchetypeInstanceUI.kSell, ArchetypeInstanceUI.kSoulbound, ArchetypeInstanceUI.kEquipped);
					}
					else if (this.Instance.ItemData != null && this.Instance.ItemData.IsNoTrade)
					{
						title = "Sell Equipped No Trade Item";
						text = ZString.Format<string, string, string, string>("{0} {1} this {2} item which is currently {3}?", "Are you sure you want to", ArchetypeInstanceUI.kSell, ArchetypeInstanceUI.kNoTrade, ArchetypeInstanceUI.kEquipped);
					}
					else
					{
						title = "Sell Equipped Item";
						text = ZString.Format<string, string, string>("{0} {1} this item which is currently {2}?", "Are you sure you want to", ArchetypeInstanceUI.kSell, ArchetypeInstanceUI.kEquipped);
					}
					DialogOptions opts = new DialogOptions
					{
						Title = title,
						Text = text,
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = new Action<bool, object>(this.SellConfirmationResponse),
						Instance = this.Instance
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
				}
				else if (this.Instance.ItemData != null && this.Instance.ItemData.IsSoulbound)
				{
					DialogOptions opts2 = new DialogOptions
					{
						Title = "Sell Soulbound Item",
						Text = ZString.Format<string, string, string, string>("{0} {1} this {2} item from your <b>{3}</b>?", "Are you sure you want to", ArchetypeInstanceUI.kSell, ArchetypeInstanceUI.kSoulbound, this.Instance.ContainerInstance.ContainerType.GetDescription()),
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = new Action<bool, object>(this.SellConfirmationResponse),
						Instance = this.Instance
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts2);
				}
				else if (this.Instance.ItemData != null && this.Instance.ItemData.IsNoTrade)
				{
					DialogOptions opts3 = new DialogOptions
					{
						Title = "Sell No Trade Item",
						Text = ZString.Format<string, string, string, string>("{0} {1} this {2} item from your <b>{3}</b>?", "Are you sure you want to", ArchetypeInstanceUI.kSell, ArchetypeInstanceUI.kNoTrade, this.Instance.ContainerInstance.ContainerType.GetDescription()),
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = new Action<bool, object>(this.SellConfirmationResponse),
						Instance = this.Instance
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts3);
				}
				else
				{
					LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.MerchantItemSellRequest(this.Instance.InstanceId, this.Instance.ContainerInstance.ContainerType);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004BEC RID: 19436 RVA: 0x000735C5 File Offset: 0x000717C5
		private void SellConfirmationResponse(bool arg1, object arg2)
		{
			if (arg1 && this.CanSell())
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.MerchantItemSellRequest(this.Instance.InstanceId, this.Instance.ContainerInstance.ContainerType);
			}
		}

		// Token: 0x06004BED RID: 19437 RVA: 0x001BAED8 File Offset: 0x001B90D8
		private bool RepairRequest()
		{
			InteractiveBlacksmith interactiveBlacksmith;
			if (CursorManager.GameMode == CursorGameMode.Repair && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Inventory != null && LocalPlayer.GameEntity.CollectionController.InteractiveStation != null && LocalPlayer.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveBlacksmith) && this.Instance != null && this.Instance.ContainerInstance != null && this.Instance.ContainerInstance.ContainerType.CanRepairFrom())
			{
				uint repairCost = this.Instance.GetRepairCost();
				if (repairCost > 0U)
				{
					CurrencySources currencySources;
					ulong availableCurrency = interactiveBlacksmith.GetAvailableCurrency(LocalPlayer.GameEntity, out currencySources);
					if ((ulong)repairCost <= availableCurrency)
					{
						LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.BlacksmithItemRepairRequest(this.Instance.InstanceId, this.Instance.ContainerInstance.ContainerType);
					}
					else
					{
						ClientGameManager.CombatTextManager.InitializeOverheadCombatText("Insufficient funds!", LocalPlayer.GameEntity, Color.yellow, null);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004BEE RID: 19438 RVA: 0x001BAFFC File Offset: 0x001B91FC
		private bool DeconstructRequest()
		{
			if (CursorManager.GameMode == CursorGameMode.Deconstruct && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Inventory != null && LocalPlayer.GameEntity.CollectionController.RefinementStation != null && this.Instance != null && this.Instance.ContainerInstance != null && this.Instance.ContainerInstance.ContainerType.CanDeconstructFrom() && this.Instance.CanDeconstruct(LocalPlayer.GameEntity))
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.DeconstructRequest(this.Instance.InstanceId);
				return true;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unable to deconstruct");
			return false;
		}

		// Token: 0x06004BEF RID: 19439 RVA: 0x001BB0CC File Offset: 0x001B92CC
		private void UnlearnAbility()
		{
			if (this.m_instance.Index != -1 && !this.m_instance.ContainerInstance.LockFlags.IsLocked() && this.m_instance.AbilityData.CooldownFlags.CanUnmemorize() && !this.AuraIsActive)
			{
				TransferRequest request = new TransferRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					InstanceId = this.m_instance.InstanceId,
					SourceContainer = this.m_instance.ContainerInstance.Id,
					TargetContainer = ContainerType.Abilities.ToString(),
					TargetIndex = -1,
					Instance = this.m_instance
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request);
				return;
			}
			Debug.LogWarning("Cannot unmemorize! (index=" + this.m_instance.Index.ToString() + ", flags=" + this.m_instance.AbilityData.CooldownFlags.ToString());
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x00073601 File Offset: 0x00071801
		private void ToggleHideHelm()
		{
			LocalPlayer.GameEntity.CharacterData.HideHelm = !LocalPlayer.GameEntity.CharacterData.HideHelm;
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x00073624 File Offset: 0x00071824
		private void UseItem()
		{
			LocalPlayer.GameEntity.SkillsController.BeginExecution(this.m_instance);
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x001BB1E4 File Offset: 0x001B93E4
		private void AttemptToDestroyItem()
		{
			if (!this.CanModify || this.m_instance == null || this.m_instance.ItemData == null || this.m_instance.ContainerInstance == null)
			{
				return;
			}
			if (this.CanModify && this.m_instance.ContainerInstance.CanDestroyItem() && this.m_instance.ItemData.ItemFlags.AllowDestruction())
			{
				this.DestroyItemConfirmation();
			}
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x001BB258 File Offset: 0x001B9458
		private void DestroyItemConfirmation()
		{
			if (this.m_instance != null && this.m_instance.IsItem)
			{
				string title;
				string text;
				if (this.m_instance.ContainerInstance != null)
				{
					if (this.m_instance.ContainerInstance.ContainerType == ContainerType.Equipment)
					{
						if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsSoulbound)
						{
							title = "Destroy Equipped Soulbound Item";
							text = ZString.Format<string, string, string, string>("{0} {1} this {2} item which is currently {3}?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kSoulbound, ArchetypeInstanceUI.kEquipped);
						}
						else if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsNoTrade)
						{
							title = "Destroy Equipped No Trade Item";
							text = ZString.Format<string, string, string, string>("{0} {1} this {2} item which is currently {3}?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kNoTrade, ArchetypeInstanceUI.kEquipped);
						}
						else
						{
							title = "Destroy Equipped Item";
							text = ZString.Format<string, string, string>("{0} {1} this item which is currently {2}?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kEquipped);
						}
					}
					else
					{
						string description = this.m_instance.ContainerInstance.ContainerType.GetDescription();
						if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsSoulbound)
						{
							title = "Destroy Soulbound Item";
							text = ZString.Format<string, string, string, string>("{0} {1} this {2} item from your <b>{3}</b>?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kSoulbound, description);
						}
						else if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsNoTrade)
						{
							title = "Destroy No Trade Item";
							text = ZString.Format<string, string, string, string>("{0} {1} this {2} item from your <b>{3}</b>?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kNoTrade, description);
						}
						else
						{
							title = "Destroy Item";
							text = ZString.Format<string, string, string>("{0} {1} this item from your <b>{2}</b>?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, description);
						}
					}
				}
				else if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsSoulbound)
				{
					title = "Destroy Soulbound Item";
					text = ZString.Format<string, string, string>("{0} {1} this {2} item?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kSoulbound);
				}
				else if (this.m_instance.ItemData != null && this.m_instance.ItemData.IsNoTrade)
				{
					title = "Destroy No Trade Item";
					text = ZString.Format<string, string, string>("{0} {1} this {2} item?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy, ArchetypeInstanceUI.kNoTrade);
				}
				else
				{
					title = "Destroy Item";
					text = ZString.Format<string, string>("{0} {1} this item?", "Are you sure you want to", ArchetypeInstanceUI.kDestroy);
				}
				DialogOptions opts = new DialogOptions
				{
					Title = title,
					Text = text,
					ConfirmationText = "Yes",
					CancelText = "NO",
					Callback = new Action<bool, object>(this.DestroyItemConfirmationResult),
					Instance = this.m_instance
				};
				ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x001BB520 File Offset: 0x001B9720
		private void DestroyItemConfirmationResult(bool arg1, object arg2)
		{
			if (arg1 && this.m_instance != null && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.CanDestroyItem() && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
			{
				ItemDestructionTransaction request = new ItemDestructionTransaction
				{
					InstanceId = this.m_instance.InstanceId,
					SourceContainer = this.m_instance.ContainerInstance.Id,
					Context = ItemDestructionContext.Request
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.DestroyItemRequest(request);
			}
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x001BB5C8 File Offset: 0x001B97C8
		private bool CanSplit()
		{
			return this.m_instance != null && this.m_instance.IsItem && !this.m_instance.ItemData.Locked && this.m_instance.Archetype != null && this.m_instance.Archetype is IStackable && this.m_instance.ItemData.Count != null && this.m_instance.ItemData.Count.Value > 1 && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.CanSplit() && LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Inventory != null && LocalPlayer.GameEntity.CollectionController.Inventory.HasRoom() && LocalPlayer.GameEntity.CollectionController.Inventory.CanSplit();
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x001BB6E0 File Offset: 0x001B98E0
		private bool TryGetTargetSplitContainer(out ContainerInstance targetContainerInstance)
		{
			targetContainerInstance = null;
			if (this.m_instance != null && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType.AllowSplitting() && this.m_instance.IsItem && !this.m_instance.ItemData.Locked && this.m_instance.Archetype is IStackable && this.m_instance.ItemData.Count != null && this.m_instance.ItemData.Count.Value > 1 && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				targetContainerInstance = LocalPlayer.GameEntity.CollectionController.Inventory;
				ContainerInstance containerInstance;
				if (this.m_instance.ContainerInstance.ContainerType.AllowSplitToSelf() && LocalPlayer.GameEntity.CollectionController.TryGetInstance(this.m_instance.ContainerInstance.Id, out containerInstance) && containerInstance.HasRoom() && containerInstance.IsUnlocked())
				{
					targetContainerInstance = containerInstance;
				}
			}
			return targetContainerInstance != null && targetContainerInstance.HasRoom() && targetContainerInstance.IsUnlocked();
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040045FE RID: 17918
		private const float kRepositionTime = 0.15f;

		// Token: 0x040045FF RID: 17919
		private const float kMaxDistance = 10f;

		// Token: 0x04004600 RID: 17920
		private bool m_locked;

		// Token: 0x04004601 RID: 17921
		private ArchetypeInstance m_instance;

		// Token: 0x04004602 RID: 17922
		private IExecutable m_executable;

		// Token: 0x04004603 RID: 17923
		private ContainerSlotUI m_containerSlotUI;

		// Token: 0x04004604 RID: 17924
		private IEnumerator m_repositionCo;

		// Token: 0x04004605 RID: 17925
		private IArchetypeInstanceEventsUI m_eventsUI;

		// Token: 0x04004606 RID: 17926
		private bool m_allowDisabledPanel = true;

		// Token: 0x04004607 RID: 17927
		private bool m_isRecipeOrGatheringAbility;

		// Token: 0x04004608 RID: 17928
		private Vector2? m_previousPivot;

		// Token: 0x04004609 RID: 17929
		private bool m_cursorInside;

		// Token: 0x0400460A RID: 17930
		private bool m_chatWasFocusedOnPointerDown;

		// Token: 0x0400460B RID: 17931
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x0400460C RID: 17932
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x0400460D RID: 17933
		[SerializeField]
		protected Image m_icon;

		// Token: 0x0400460E RID: 17934
		[SerializeField]
		private TextMeshProUGUI m_centerLabel;

		// Token: 0x0400460F RID: 17935
		[SerializeField]
		private TextMeshProUGUI m_lowerLeftLabel;

		// Token: 0x04004610 RID: 17936
		[SerializeField]
		private TextMeshProUGUI m_lowerRightLabel;

		// Token: 0x04004611 RID: 17937
		[SerializeField]
		private TextMeshProUGUI m_upperLeftLabel;

		// Token: 0x04004612 RID: 17938
		[SerializeField]
		private TextMeshProUGUI m_upperRightLabel;

		// Token: 0x04004613 RID: 17939
		[SerializeField]
		private CanvasGroup m_disabledPanel;

		// Token: 0x04004614 RID: 17940
		[SerializeField]
		private CanvasGroup m_lowLevelCanvas;

		// Token: 0x04004615 RID: 17941
		[SerializeField]
		private CanvasGroup m_lowStatCanvas;

		// Token: 0x04004616 RID: 17942
		[SerializeField]
		private CanvasGroup m_lowStaminaCanvas;

		// Token: 0x04004617 RID: 17943
		[SerializeField]
		private CanvasGroup m_lockedCanvas;

		// Token: 0x04004618 RID: 17944
		[SerializeField]
		private CanvasGroup m_behaviorCanvas;

		// Token: 0x04004619 RID: 17945
		[SerializeField]
		private DurabilityPanelUI m_durability;

		// Token: 0x0400461A RID: 17946
		[SerializeField]
		private AugmentPanelUI m_augment;

		// Token: 0x0400461B RID: 17947
		[SerializeField]
		private Image m_highlight;

		// Token: 0x0400461C RID: 17948
		[SerializeField]
		private Image m_availableFrame;

		// Token: 0x0400461D RID: 17949
		[SerializeField]
		private Image m_availableOverlay;

		// Token: 0x0400461E RID: 17950
		[SerializeField]
		private Image m_invalidTargetOverlay;

		// Token: 0x0400461F RID: 17951
		[SerializeField]
		private Image m_distanceAngleOverlay;

		// Token: 0x04004620 RID: 17952
		[SerializeField]
		private Image m_itemCategoryFrame;

		// Token: 0x04004621 RID: 17953
		[SerializeField]
		private GameObject m_eventsObject;

		// Token: 0x04004622 RID: 17954
		private TargetOverlayState m_targetOverlay;

		// Token: 0x04004624 RID: 17956
		private static readonly Vector2 kDefaultPivot = new Vector2(0.5f, 0.5f);

		// Token: 0x04004625 RID: 17957
		private const string kItemQuery = "Are you sure you want to";

		// Token: 0x04004626 RID: 17958
		private static string kDestroy = "<b><color=" + UIManager.RedColor.ToHex() + ">DESTROY</color></b>";

		// Token: 0x04004627 RID: 17959
		private static string kSell = "<b><color=" + UIManager.RedColor.ToHex() + ">SELL</color></b>";

		// Token: 0x04004628 RID: 17960
		public static string kNoTrade = "<b><color=" + UIManager.RedColor.ToHex() + ">NO TRADE</color></b>";

		// Token: 0x04004629 RID: 17961
		public static string kSoulbound = "<b><color=" + UIManager.RedColor.ToHex() + ">SOULBOUND</color></b>";

		// Token: 0x0400462A RID: 17962
		private static string kEquipped = "<b><color=" + UIManager.BlueColor.ToHex() + ">EQUIPPED</color></b>";
	}
}
