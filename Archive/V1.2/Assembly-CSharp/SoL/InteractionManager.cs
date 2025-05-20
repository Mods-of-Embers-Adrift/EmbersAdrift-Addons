using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL
{
	// Token: 0x0200021C RID: 540
	public class InteractionManager : MonoBehaviour
	{
		// Token: 0x1400001B RID: 27
		// (add) Token: 0x0600122F RID: 4655 RVA: 0x000E5E58 File Offset: 0x000E4058
		// (remove) Token: 0x06001230 RID: 4656 RVA: 0x000E5E8C File Offset: 0x000E408C
		public static event Action InteractionSuccess;

		// Token: 0x06001231 RID: 4657 RVA: 0x0004EF0F File Offset: 0x0004D10F
		private static void UpdateLastMouseInteractionFrame()
		{
			InteractionManager.LastMouseInteractionFrame = Time.frameCount;
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06001232 RID: 4658 RVA: 0x0004EF1B File Offset: 0x0004D11B
		// (set) Token: 0x06001233 RID: 4659 RVA: 0x0004EF22 File Offset: 0x0004D122
		public static int LastMouseInteractionFrame { get; set; } = 0;

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06001234 RID: 4660 RVA: 0x0004EF2A File Offset: 0x0004D12A
		// (set) Token: 0x06001235 RID: 4661 RVA: 0x0004EF31 File Offset: 0x0004D131
		public static GameObject HoveredUIElement { get; private set; }

		// Token: 0x06001236 RID: 4662 RVA: 0x0004EF39 File Offset: 0x0004D139
		public static Vector3 GetMouseRaySource()
		{
			return Input.mousePosition;
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x0004EF40 File Offset: 0x0004D140
		// (set) Token: 0x06001238 RID: 4664 RVA: 0x0004EF48 File Offset: 0x0004D148
		private IInteractive CurrentInteractive { get; set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x0004EF51 File Offset: 0x0004D151
		// (set) Token: 0x0600123A RID: 4666 RVA: 0x0004EF59 File Offset: 0x0004D159
		private IContextMenu CurrentContextMenu { get; set; }

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0004EF62 File Offset: 0x0004D162
		// (set) Token: 0x0600123C RID: 4668 RVA: 0x0004EF6A File Offset: 0x0004D16A
		private ITooltip CurrentTooltip { get; set; }

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0004EF73 File Offset: 0x0004D173
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x0004EF7B File Offset: 0x0004D17B
		private ICursor CurrentCursor { get; set; }

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x0004EF84 File Offset: 0x0004D184
		// (set) Token: 0x06001240 RID: 4672 RVA: 0x0004EF8C File Offset: 0x0004D18C
		private IHighlight CurrentHighlight
		{
			get
			{
				return this.m_currentHighlight;
			}
			set
			{
				if (this.m_currentHighlight == value)
				{
					return;
				}
				if (this.m_currentHighlight != null)
				{
					this.m_currentHighlight.HighlightEnabled = false;
				}
				this.m_currentHighlight = value;
			}
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x0004EFB3 File Offset: 0x0004D1B3
		private void SingleClick(InteractionManager.MouseButtonType mouseButtonType)
		{
			this.CheckInteraction(mouseButtonType, false);
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0004EFBD File Offset: 0x0004D1BD
		private void DoubleClick(InteractionManager.MouseButtonType mouseButtonType)
		{
			this.CheckInteraction(mouseButtonType, true);
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x000E5EC0 File Offset: 0x000E40C0
		private bool TryGetInteractivePlayer(out InteractivePlayer player)
		{
			player = null;
			if (this.m_contextMenuInteractive != null)
			{
				InteractivePlayer interactivePlayer = this.m_contextMenuInteractive as InteractivePlayer;
				if (interactivePlayer != null)
				{
					player = interactivePlayer;
					return true;
				}
				InteractiveForwarder interactiveForwarder = this.m_contextMenuInteractive as InteractiveForwarder;
				if (interactiveForwarder != null && interactiveForwarder.Interactive != null)
				{
					InteractivePlayer interactivePlayer2 = interactiveForwarder.Interactive as InteractivePlayer;
					if (interactivePlayer2 != null)
					{
						player = interactivePlayer2;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x000E5F18 File Offset: 0x000E4118
		private void CheckInteraction(InteractionManager.MouseButtonType mouseButtonType, bool doubleClick)
		{
			if (this.CurrentContextMenu != null)
			{
				bool flag = false;
				if (this.CurrentContextMenu.Settings != null)
				{
					flag = this.CurrentContextMenu.Settings.ValidateContext(mouseButtonType, doubleClick);
				}
				else if (this.m_hoveredObjectType == InteractionManager.HoveredObjectType.UI && mouseButtonType == InteractionManager.MouseButtonType.Right)
				{
					flag = true;
				}
				if (flag)
				{
					ContextMenuUI.ClearContextActions();
					string text = this.CurrentContextMenu.FillActionsGetTitle();
					if (!string.IsNullOrEmpty(text))
					{
						this.m_contextMenuInteractive = this.CurrentContextMenu;
						this.m_contextMenuWorldSpaceTransform = this.m_worldSpaceHitTransform;
						this.m_contextMenuWorldSpaceOffset = this.m_worldSpaceHitOffset;
						InteractivePlayer interactivePlayer;
						if (this.TryGetInteractivePlayer(out interactivePlayer) && interactivePlayer.GameEntity && interactivePlayer.GameEntity.gameObject && interactivePlayer.GameEntity.gameObject.transform)
						{
							float y = interactivePlayer.GameEntity.gameObject.transform.position.y;
							float num = this.m_contextMenuWorldSpaceTransform.position.y;
							Vector3 zero = Vector3.zero;
							if (this.m_contextMenuWorldSpaceOffset != null)
							{
								zero.x = this.m_contextMenuWorldSpaceOffset.Value.x;
								zero.z = this.m_contextMenuWorldSpaceOffset.Value.z;
								num += this.m_contextMenuWorldSpaceOffset.Value.y;
							}
							zero.y = ((y > num) ? (y - num) : (num - y));
							this.m_contextMenuWorldSpaceTransform = interactivePlayer.GameEntity.gameObject.transform;
							this.m_contextMenuWorldSpaceOffset = new Vector3?(zero);
						}
						ClientGameManager.UIManager.ContextMenu.Init(ContextMenuUI.ActionList, text);
						ClientGameManager.UIManager.ContextMenu.HideCallback = new Action(this.InteractionContextClosed);
						InteractionManager.UpdateLastMouseInteractionFrame();
						Action interactionSuccess = InteractionManager.InteractionSuccess;
						if (interactionSuccess == null)
						{
							return;
						}
						interactionSuccess();
						return;
					}
				}
			}
			if (this.CurrentInteractive != null)
			{
				bool flag2 = false;
				if (this.CurrentInteractive.Settings != null)
				{
					flag2 = this.CurrentInteractive.Settings.ValidateInteraction(mouseButtonType, doubleClick);
				}
				else if (this.m_hoveredObjectType == InteractionManager.HoveredObjectType.UI)
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (this.CurrentInteractive.ClientInteraction())
					{
						Action interactionSuccess2 = InteractionManager.InteractionSuccess;
						if (interactionSuccess2 != null)
						{
							interactionSuccess2();
						}
					}
					InteractionManager.UpdateLastMouseInteractionFrame();
				}
			}
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0004EFC7 File Offset: 0x0004D1C7
		private void InteractionContextClosed()
		{
			ClientGameManager.UIManager.ContextMenu.HideCallback = null;
			this.m_contextMenuInteractive = null;
			this.m_contextMenuWorldSpaceTransform = null;
			this.m_contextMenuWorldSpaceOffset = null;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0004EFF3 File Offset: 0x0004D1F3
		private void Awake()
		{
			this.m_left = new InteractionManager.MouseButtonInteraction(this, InteractionManager.MouseButtonType.Left);
			this.m_right = new InteractionManager.MouseButtonInteraction(this, InteractionManager.MouseButtonType.Right);
			this.m_middle = new InteractionManager.MouseButtonInteraction(this, InteractionManager.MouseButtonType.Middle);
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x0004F01C File Offset: 0x0004D21C
		private void Update()
		{
			this.SetInteractives();
			this.m_isWithinRange = this.IsWithinRange();
			this.m_cursorIsHidden = CursorManager.IsCursorHidden();
			this.UpdateInteractionContext();
			this.UpdateHighlight();
			this.UpdateTooltip();
			this.UpdateCursor();
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x000E6158 File Offset: 0x000E4358
		private void SetInteractives()
		{
			InteractionManager.HoveredUIElement = UIManager.InputModule.GameObjectUnderPointer();
			this.m_hoveredObject = InteractionManager.HoveredUIElement;
			Transform transform = null;
			Vector3? worldSpaceHitOffset = null;
			if (!this.m_hoveredObject && ClientGameManager.MainCamera)
			{
				Ray ray = ClientGameManager.MainCamera.ScreenPointToRay(InteractionManager.GetMouseRaySource());
				RaycastHit[] hits = Hits.Hits10;
				int num = Physics.RaycastNonAlloc(ray, hits, 20f, LayerMap.Interaction.LayerMask);
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = hits[i].collider.gameObject;
					if (!LocalPlayer.GameEntity || !(LocalPlayer.GameEntity.gameObject.transform.root == gameObject.transform.root))
					{
						this.m_hoveredObject = gameObject;
						transform = hits[i].collider.gameObject.transform;
						worldSpaceHitOffset = new Vector3?(transform.InverseTransformPoint(hits[i].point));
						break;
					}
				}
			}
			this.m_worldSpaceHitTransform = transform;
			this.m_worldSpaceHitOffset = worldSpaceHitOffset;
			IInteractive interactive = null;
			IContextMenu currentContextMenu = null;
			IHighlight currentHighlight = null;
			ITooltip currentTooltip = null;
			ICursor currentCursor = null;
			if (this.m_hoveredObject)
			{
				if (this.m_hoveredObject == this.m_hoveredObjectLastFrame)
				{
					return;
				}
				interactive = this.m_hoveredObject.GetComponent<IInteractive>();
				if (interactive != null && interactive.RequiresLos && (!ClientGameManager.MainCamera || !interactive.gameObject || !LineOfSight.HasLineOfSight(ClientGameManager.MainCamera.gameObject.transform.position, interactive.gameObject.transform.position)))
				{
					return;
				}
				currentContextMenu = this.m_hoveredObject.GetComponent<IContextMenu>();
				currentHighlight = this.m_hoveredObject.GetComponent<IHighlight>();
				currentTooltip = this.m_hoveredObject.GetComponent<ITooltip>();
				currentCursor = this.m_hoveredObject.GetComponent<ICursor>();
			}
			this.CurrentInteractive = interactive;
			this.CurrentContextMenu = currentContextMenu;
			this.CurrentHighlight = currentHighlight;
			this.CurrentTooltip = currentTooltip;
			this.CurrentCursor = currentCursor;
			if (this.m_hoveredObject)
			{
				this.m_hoveredObjectInstanceID = this.m_hoveredObject.GetInstanceID();
				this.m_hoveredObjectType = ((this.m_hoveredObject == InteractionManager.HoveredUIElement) ? InteractionManager.HoveredObjectType.UI : InteractionManager.HoveredObjectType.World);
			}
			else
			{
				this.m_hoveredObjectInstanceID = -1;
				this.m_hoveredObjectType = InteractionManager.HoveredObjectType.None;
			}
			this.m_hoveredObjectLastFrame = this.m_hoveredObject;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x000E63C4 File Offset: 0x000E45C4
		private void UpdateInteractionContext()
		{
			if (this.m_contextMenuInteractive != null && this.m_contextMenuWorldSpaceTransform && this.m_contextMenuWorldSpaceOffset != null && ClientGameManager.MainCamera)
			{
				Vector3 position = this.m_contextMenuWorldSpaceTransform.position + this.m_contextMenuWorldSpaceOffset.Value;
				ClientGameManager.UIManager.ContextMenu.RectTransform.position = ClientGameManager.MainCamera.WorldToScreenPoint(position);
			}
			if (this.m_isWithinRange)
			{
				if (this.m_left.CheckInput())
				{
					this.m_right.ResetValues();
					this.m_middle.ResetValues();
					return;
				}
				if (this.m_right.CheckInput())
				{
					this.m_left.ResetValues();
					this.m_middle.ResetValues();
					return;
				}
				if (this.m_middle.CheckInput())
				{
					this.m_left.ResetValues();
					this.m_right.ResetValues();
					return;
				}
			}
			else if (this.m_contextMenuInteractive != null && !InteractionManager.WithinRangeOfInteractive(this.m_contextMenuInteractive))
			{
				ClientGameManager.UIManager.ContextMenu.Hide(false);
			}
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0004F053 File Offset: 0x0004D253
		private static bool WithinRangeOfInteractive(IContextMenu interactive)
		{
			return interactive != null && (interactive.Settings == null || interactive.Settings.IsWithinRange(interactive.gameObject, LocalPlayer.GameEntity));
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x000E64D4 File Offset: 0x000E46D4
		private void UpdateHighlight()
		{
			if (this.CurrentHighlight == null)
			{
				return;
			}
			if (this.m_cursorIsHidden)
			{
				if (this.CurrentHighlight.HighlightEnabled)
				{
					this.CurrentHighlight.HighlightEnabled = false;
				}
				return;
			}
			if (this.m_isWithinRange && !this.CurrentHighlight.HighlightEnabled)
			{
				this.CurrentHighlight.HighlightEnabled = true;
				return;
			}
			if (!this.m_isWithinRange && this.CurrentHighlight.HighlightEnabled)
			{
				this.CurrentHighlight.HighlightEnabled = false;
			}
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x000E6550 File Offset: 0x000E4750
		private void UpdateTooltip()
		{
			this.m_prevTooltipParameter = this.m_currTooltipParameter;
			this.m_currTooltipParameter = null;
			if (!this.m_isWithinRange || this.m_cursorIsHidden || this.CurrentTooltip == null || this.CurrentTooltip.GetTooltipParameter == null)
			{
				this.DeactivateTooltip();
				return;
			}
			this.m_currTooltipParameter = this.CurrentTooltip.GetTooltipParameter();
			if (this.m_currTooltipParameter != null && this.m_currTooltipParameter.Equals(this.m_prevTooltipParameter))
			{
				if (this.m_tooltipWindow && !this.m_tooltipWindow.Visible)
				{
					this.m_tooltipHoverTime += Time.deltaTime;
					float num = this.CurrentTooltip.TooltipSettings.UseOverrideDelay ? this.CurrentTooltip.TooltipSettings.Delay : Options.GameOptions.TooltipDelay.Value;
					if (this.m_tooltipHoverTime > num)
					{
						this.m_tooltipWindow.ActivateTooltip(this.CurrentTooltip.GetTooltipParameter, false);
					}
				}
				return;
			}
			this.DeactivateTooltip();
			this.m_prevTooltipParameter = this.m_currTooltipParameter;
			if (this.CurrentTooltip != null)
			{
				if (this.m_currTooltipParameter == null || this.m_currTooltipParameter.Type == TooltipType.None)
				{
					return;
				}
				if (UIManager.TryGetTooltipUI(this.m_currTooltipParameter.Type, out this.m_tooltipWindow))
				{
					this.m_tooltipHoverTime = 0f;
				}
			}
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x0004F07A File Offset: 0x0004D27A
		private void DeactivateTooltip()
		{
			if (this.m_tooltipWindow)
			{
				this.m_tooltipWindow.DeactivateTooltip();
				this.m_tooltipWindow = null;
			}
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x000E66A0 File Offset: 0x000E48A0
		private void UpdateCursor()
		{
			CursorType cursorType = (this.m_isWithinRange && !this.m_cursorIsHidden && this.CurrentCursor != null) ? this.CurrentCursor.Type : CursorType.MainCursor;
			if (ClientGameManager.UIManager.Dragged != null)
			{
				cursorType = CursorType.DragArrow;
			}
			if (cursorType == CursorType.None)
			{
				cursorType = CursorType.MainCursor;
			}
			CursorManager.SetCursorImage(cursorType, null);
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x000E66F8 File Offset: 0x000E48F8
		private bool IsWithinRange()
		{
			InteractionManager.HoveredObjectType hoveredObjectType = this.m_hoveredObjectType;
			if (hoveredObjectType == InteractionManager.HoveredObjectType.UI)
			{
				return true;
			}
			if (hoveredObjectType != InteractionManager.HoveredObjectType.World)
			{
				return false;
			}
			GameEntityComponent gameEntityComponent = null;
			InteractionSettings interactionSettings = null;
			GameObject obj = null;
			if (this.CurrentInteractive != null)
			{
				gameEntityComponent = (this.CurrentInteractive as GameEntityComponent);
				interactionSettings = this.CurrentInteractive.Settings;
				obj = this.CurrentInteractive.gameObject;
			}
			else if (this.CurrentContextMenu != null)
			{
				gameEntityComponent = (this.CurrentContextMenu as GameEntityComponent);
				interactionSettings = this.CurrentContextMenu.Settings;
				obj = this.CurrentContextMenu.gameObject;
			}
			else if (this.CurrentHighlight != null)
			{
				gameEntityComponent = (this.CurrentHighlight as GameEntityComponent);
				interactionSettings = this.CurrentHighlight.Settings;
				obj = this.CurrentHighlight.gameObject;
			}
			else if (this.CurrentTooltip != null)
			{
				gameEntityComponent = (this.CurrentTooltip as GameEntityComponent);
				interactionSettings = this.CurrentTooltip.Settings;
				obj = this.CurrentTooltip.gameObject;
			}
			else if (this.CurrentCursor != null)
			{
				gameEntityComponent = (this.CurrentCursor as GameEntityComponent);
				interactionSettings = this.CurrentCursor.Settings;
				obj = this.CurrentCursor.gameObject;
			}
			if (interactionSettings == null)
			{
				return false;
			}
			if (!(gameEntityComponent != null))
			{
				return interactionSettings.IsWithinRange(obj, LocalPlayer.GameEntity);
			}
			return interactionSettings.IsWithinRange(gameEntityComponent.GameEntity, LocalPlayer.GameEntity);
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x000E683C File Offset: 0x000E4A3C
		public static void LootNearbyViaKeybind()
		{
			if (!LocalPlayer.GameEntity)
			{
				return;
			}
			InteractionManager.m_nearbyInteractives.Clear();
			InteractionManager.m_nearbyInteractiveCollectionList.Clear();
			InteractiveExtensions.GetNearbyInteractives(LocalPlayer.GameEntity.gameObject.transform.position, GlobalSettings.Values.General.GlobalInteractionDistance, InteractionManager.m_nearbyInteractiveCollectionList);
			for (int i = 0; i < InteractionManager.m_nearbyInteractiveCollectionList.Count; i++)
			{
				if (InteractionManager.m_nearbyInteractiveCollectionList[i] != null && InteractionManager.m_nearbyInteractiveCollectionList[i].Settings != null && InteractionManager.m_nearbyInteractiveCollectionList[i].gameObject)
				{
					GameEntityComponent gameEntityComponent = null;
					GameObject gameObject = null;
					GameEntityComponent gameEntityComponent2 = InteractionManager.m_nearbyInteractiveCollectionList[i] as GameEntityComponent;
					if (gameEntityComponent2 != null)
					{
						if (gameEntityComponent2.GameEntity && gameEntityComponent2.GameEntity.Type != GameEntityType.Player)
						{
							gameEntityComponent = gameEntityComponent2;
							gameObject = gameEntityComponent2.GameEntity.gameObject;
						}
					}
					else
					{
						gameObject = InteractionManager.m_nearbyInteractiveCollectionList[i].gameObject;
					}
					if (gameObject && gameEntityComponent)
					{
						InteractiveNpc interactiveNpc;
						InteractiveGatheringNode interactiveGatheringNode;
						if (gameEntityComponent.GameEntity.TryGetComponent<InteractiveNpc>(out interactiveNpc))
						{
							if (!gameEntityComponent.GameEntity.Vitals)
							{
								goto IL_230;
							}
							if (gameEntityComponent.GameEntity.Vitals.GetCurrentHealthState() != HealthState.Dead)
							{
								goto IL_230;
							}
						}
						else if (!gameEntityComponent.GameEntity.TryGetComponent<InteractiveGatheringNode>(out interactiveGatheringNode))
						{
							goto IL_230;
						}
						if (LocalPlayer.GameEntity.gameObject.transform.InverseTransformPoint(gameObject.transform.position).z > 0f && ((gameEntityComponent && gameEntityComponent.GameEntity) ? InteractionManager.m_nearbyInteractiveCollectionList[i].Settings.IsWithinRange(gameEntityComponent.GameEntity, LocalPlayer.GameEntity) : InteractionManager.m_nearbyInteractiveCollectionList[i].Settings.IsWithinRange(gameObject, LocalPlayer.GameEntity)) && InteractionManager.m_nearbyInteractiveCollectionList[i].CanInteract(LocalPlayer.GameEntity))
						{
							float sqrMagnitude = (gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude;
							InteractionManager.m_nearbyInteractives.Add(new InteractionManager.NearbyInteractive(InteractionManager.m_nearbyInteractiveCollectionList[i], sqrMagnitude));
						}
					}
				}
				IL_230:;
			}
			InteractionManager.m_nearbyInteractives.Sort((InteractionManager.NearbyInteractive a, InteractionManager.NearbyInteractive b) => a.DistanceSquared.CompareTo(b.DistanceSquared));
			int num = 0;
			while (num < InteractionManager.m_nearbyInteractives.Count && (InteractionManager.m_nearbyInteractives[num].Interactive == null || !InteractionManager.m_nearbyInteractives[num].Interactive.ClientInteraction()))
			{
				num++;
			}
		}

		// Token: 0x04000FC7 RID: 4039
		private const int kNoObjectInstanceID = -1;

		// Token: 0x04000FC8 RID: 4040
		private const float kMaxRaycastDistance = 20f;

		// Token: 0x04000FC9 RID: 4041
		public const float kDefaultTooltipDelay = 0.1f;

		// Token: 0x04000FCA RID: 4042
		public static float kDoubleClickThreshold = 0.25f;

		// Token: 0x04000FD1 RID: 4049
		private IHighlight m_currentHighlight;

		// Token: 0x04000FD2 RID: 4050
		private GameObject m_hoveredObject;

		// Token: 0x04000FD3 RID: 4051
		private GameObject m_hoveredObjectLastFrame;

		// Token: 0x04000FD4 RID: 4052
		private int m_hoveredObjectInstanceID = -1;

		// Token: 0x04000FD5 RID: 4053
		private Transform m_worldSpaceHitTransform;

		// Token: 0x04000FD6 RID: 4054
		private Vector3? m_worldSpaceHitOffset;

		// Token: 0x04000FD7 RID: 4055
		private bool m_isWithinRange;

		// Token: 0x04000FD8 RID: 4056
		private bool m_cursorIsHidden;

		// Token: 0x04000FD9 RID: 4057
		private InteractionManager.HoveredObjectType m_hoveredObjectType;

		// Token: 0x04000FDA RID: 4058
		private InteractionManager.MouseButtonInteraction m_left;

		// Token: 0x04000FDB RID: 4059
		private InteractionManager.MouseButtonInteraction m_right;

		// Token: 0x04000FDC RID: 4060
		private InteractionManager.MouseButtonInteraction m_middle;

		// Token: 0x04000FDD RID: 4061
		private IContextMenu m_contextMenuInteractive;

		// Token: 0x04000FDE RID: 4062
		private Vector3? m_contextMenuWorldSpaceOffset;

		// Token: 0x04000FDF RID: 4063
		private Transform m_contextMenuWorldSpaceTransform;

		// Token: 0x04000FE0 RID: 4064
		private float m_tooltipHoverTime;

		// Token: 0x04000FE1 RID: 4065
		private BaseTooltip m_tooltipWindow;

		// Token: 0x04000FE2 RID: 4066
		private ITooltipParameter m_prevTooltipParameter;

		// Token: 0x04000FE3 RID: 4067
		private ITooltipParameter m_currTooltipParameter;

		// Token: 0x04000FE4 RID: 4068
		private static readonly List<IInteractive> m_nearbyInteractiveCollectionList = new List<IInteractive>(10);

		// Token: 0x04000FE5 RID: 4069
		private static readonly List<InteractionManager.NearbyInteractive> m_nearbyInteractives = new List<InteractionManager.NearbyInteractive>(10);

		// Token: 0x0200021D RID: 541
		public enum MouseButtonType
		{
			// Token: 0x04000FE7 RID: 4071
			Left,
			// Token: 0x04000FE8 RID: 4072
			Right,
			// Token: 0x04000FE9 RID: 4073
			Middle
		}

		// Token: 0x0200021E RID: 542
		private class MouseButtonInteraction
		{
			// Token: 0x06001253 RID: 4691 RVA: 0x0004F0D4 File Offset: 0x0004D2D4
			public MouseButtonInteraction(InteractionManager controller, InteractionManager.MouseButtonType buttonType)
			{
				this.m_controller = controller;
				this.m_buttonType = buttonType;
				this.m_buttonTypeInt = (int)buttonType;
			}

			// Token: 0x06001254 RID: 4692 RVA: 0x000E6AFC File Offset: 0x000E4CFC
			public bool CheckInput()
			{
				bool result = false;
				if (Input.GetMouseButtonDown(this.m_buttonTypeInt))
				{
					result = true;
					this.m_localHoveredInstanceID = this.m_controller.m_hoveredObjectInstanceID;
					InteractionManager.UpdateLastMouseInteractionFrame();
				}
				else if (Input.GetMouseButtonUp(this.m_buttonTypeInt))
				{
					result = true;
					if (this.m_controller.m_hoveredObjectInstanceID == this.m_localHoveredInstanceID && this.m_interactionDragTime <= GlobalSettings.Values.General.InteractiveClickDragTimeThreshold)
					{
						if (Time.time - this.m_timeOfLastClick <= InteractionManager.kDoubleClickThreshold)
						{
							this.m_controller.DoubleClick(this.m_buttonType);
						}
						else
						{
							this.m_controller.SingleClick(this.m_buttonType);
							this.m_timeOfLastClick = Time.time;
						}
					}
					else
					{
						this.m_localHoveredInstanceID = -1;
					}
					InteractionManager.UpdateLastMouseInteractionFrame();
				}
				if (Input.GetMouseButton(this.m_buttonTypeInt))
				{
					result = true;
					this.m_interactionDragTime += Time.deltaTime;
				}
				else
				{
					this.m_interactionDragTime = 0f;
				}
				return result;
			}

			// Token: 0x06001255 RID: 4693 RVA: 0x0004F0F8 File Offset: 0x0004D2F8
			public void ResetValues()
			{
				this.m_localHoveredInstanceID = -1;
				this.m_timeOfLastClick = 0f;
				this.m_interactionDragTime = 0f;
			}

			// Token: 0x04000FEA RID: 4074
			private readonly InteractionManager m_controller;

			// Token: 0x04000FEB RID: 4075
			private readonly InteractionManager.MouseButtonType m_buttonType;

			// Token: 0x04000FEC RID: 4076
			private readonly int m_buttonTypeInt;

			// Token: 0x04000FED RID: 4077
			private float m_interactionDragTime;

			// Token: 0x04000FEE RID: 4078
			private float m_timeOfLastClick;

			// Token: 0x04000FEF RID: 4079
			private int m_localHoveredInstanceID = -1;
		}

		// Token: 0x0200021F RID: 543
		private enum HoveredObjectType
		{
			// Token: 0x04000FF1 RID: 4081
			None,
			// Token: 0x04000FF2 RID: 4082
			UI,
			// Token: 0x04000FF3 RID: 4083
			World
		}

		// Token: 0x02000220 RID: 544
		private struct NearbyInteractive
		{
			// Token: 0x06001256 RID: 4694 RVA: 0x0004F117 File Offset: 0x0004D317
			public NearbyInteractive(IInteractive interactive, float distanceSqr)
			{
				this.Interactive = interactive;
				this.DistanceSquared = distanceSqr;
			}

			// Token: 0x04000FF4 RID: 4084
			public readonly IInteractive Interactive;

			// Token: 0x04000FF5 RID: 4085
			public readonly float DistanceSquared;
		}
	}
}
