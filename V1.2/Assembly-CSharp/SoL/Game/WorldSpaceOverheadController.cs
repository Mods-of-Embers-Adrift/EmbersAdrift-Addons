using System;
using SoL.Game.Crafting;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A9 RID: 1449
	public class WorldSpaceOverheadController : MonoBehaviour
	{
		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06002D7A RID: 11642 RVA: 0x0005F989 File Offset: 0x0005DB89
		// (set) Token: 0x06002D7B RID: 11643 RVA: 0x0005F991 File Offset: 0x0005DB91
		public GameEntity GameEntity { get; private set; }

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06002D7C RID: 11644 RVA: 0x0005F99A File Offset: 0x0005DB9A
		// (set) Token: 0x06002D7D RID: 11645 RVA: 0x0005F9A2 File Offset: 0x0005DBA2
		public Vector3 WorldPos { get; private set; }

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06002D7E RID: 11646 RVA: 0x0005F9AB File Offset: 0x0005DBAB
		// (set) Token: 0x06002D7F RID: 11647 RVA: 0x0005F9B3 File Offset: 0x0005DBB3
		public bool OffScreen { get; private set; }

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06002D80 RID: 11648 RVA: 0x0005F9BC File Offset: 0x0005DBBC
		public WorldSpaceNameplateController NameplateController
		{
			get
			{
				return this.m_nameplateController;
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06002D81 RID: 11649 RVA: 0x0005F9C4 File Offset: 0x0005DBC4
		public WorldSpaceInteractiveController InteractiveController
		{
			get
			{
				return this.m_interactiveController;
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06002D82 RID: 11650 RVA: 0x0005F9CC File Offset: 0x0005DBCC
		public OverheadNameplateMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06002D83 RID: 11651 RVA: 0x0014EC54 File Offset: 0x0014CE54
		private bool ProcessUpdates
		{
			get
			{
				return this.m_canvasGroup.alpha > 0f && ((this.m_nameplateController.isActiveAndEnabled && this.m_nameplateController.IsActive) || (this.m_chatBubbleController.isActiveAndEnabled && this.m_chatBubbleController.IsActive) || (this.m_interactiveController.isActiveAndEnabled && this.m_interactiveController.IsActive));
			}
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x0005F9D4 File Offset: 0x0005DBD4
		private void Update()
		{
			this.m_rotator.PreventRotation = !this.ProcessUpdates;
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x0014ECC8 File Offset: 0x0014CEC8
		private void LateUpdate()
		{
			if (!this.GameEntity)
			{
				return;
			}
			if (this.m_nameplateController.isActiveAndEnabled)
			{
				this.m_nameplateController.LateUpdateExternal();
				if (this.m_nameplateController.IsActive && this.m_nameplateLayoutElement.ignoreLayout)
				{
					this.m_nameplateLayoutElement.ignoreLayout = false;
				}
				else if (!this.m_nameplateController.IsActive && !this.m_nameplateLayoutElement.ignoreLayout)
				{
					this.m_nameplateLayoutElement.ignoreLayout = true;
				}
			}
			if (this.m_chatBubbleController.isActiveAndEnabled)
			{
				this.m_chatBubbleController.LateUpdateExternal();
			}
			if (this.m_interactiveController.isActiveAndEnabled)
			{
				this.m_interactiveController.LateUpdateExternal();
			}
			if (this.ProcessUpdates && this.GameEntity)
			{
				this.m_canvas.sortingOrder = Mathf.Clamp(0 - Mathf.FloorToInt(this.GameEntity.GetCachedSqrDistanceFromCamera()), -32767, 32767);
			}
			if (this.m_canvas.enabled != this.ProcessUpdates)
			{
				this.m_canvas.enabled = this.ProcessUpdates;
			}
		}

		// Token: 0x06002D86 RID: 11654 RVA: 0x0014EDE0 File Offset: 0x0014CFE0
		public void Init(GameEntity entity)
		{
			this.GameEntity = entity;
			if (this.GameEntity)
			{
				this.GameEntity.WorldSpaceOverheadController = this;
			}
			if (this.GameEntity && this.GameEntity.Interactive != null && this.GameEntity.Interactive is InteractiveGatheringNode)
			{
				this.m_nameplateController.gameObject.SetActive(false);
				this.m_chatBubbleController.gameObject.SetActive(false);
				this.m_interactiveController.gameObject.SetActive(true);
				this.m_interactiveController.Init(this);
			}
			else
			{
				this.m_interactiveController.gameObject.SetActive(false);
				this.m_nameplateController.gameObject.SetActive(true);
				this.m_nameplateController.Init(this);
				this.m_chatBubbleController.gameObject.SetActive(true);
				this.m_chatBubbleController.Init(this);
			}
			this.UpdatePosition();
			if (this.GameEntity)
			{
				if (this.m_nameplateController.isActiveAndEnabled)
				{
					this.m_nameplateController.UpdateScale();
				}
				if (this.m_interactiveController.isActiveAndEnabled)
				{
					this.m_interactiveController.UpdateScale();
				}
			}
			this.UIManagerOnUiHiddenChanged();
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x0014EF10 File Offset: 0x0014D110
		public void ResetData()
		{
			this.m_deadDelta = 0f;
			this.m_nameplateController.ResetData();
			this.m_interactiveController.ResetData();
			if (this.GameEntity)
			{
				this.GameEntity.WorldSpaceOverheadController = null;
			}
			this.GameEntity = null;
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x0014EF60 File Offset: 0x0014D160
		public void UpdatePosition()
		{
			if (this.GameEntity)
			{
				Vector3 vector;
				if (this.GameEntity.OverheadReference)
				{
					vector = this.GameEntity.OverheadReference.position;
				}
				else
				{
					vector = this.GameEntity.gameObject.transform.position;
					Vector3 vector2 = (this.GameEntity.NameplateHeightOffset != null) ? this.GameEntity.NameplateHeightOffset.Value : WorldSpaceOverheadController.kDefaultHeightOffset;
					if (this.GameEntity.UseFullNameplateHeightOffset)
					{
						vector += vector2;
					}
					else
					{
						vector.y += vector2.y;
					}
				}
				if (this.GameEntity.IsDead)
				{
					float target = Mathf.Abs(this.GameEntity.gameObject.transform.position.y - vector.y) * 0.2f;
					this.m_deadDelta = Mathf.MoveTowards(this.m_deadDelta, target, 0.01f);
					vector.y -= this.m_deadDelta;
				}
				else
				{
					this.m_deadDelta = 0f;
				}
				this.WorldPos = vector;
				Vector3 vector3;
				if (this.IsVisibleToCamera(ref vector, out vector3))
				{
					this.OffScreen = false;
					base.gameObject.transform.position = ((this.Mode == OverheadNameplateMode.UISpace) ? new Vector3(vector3.x * (float)Screen.width, vector3.y * (float)Screen.height, vector3.z) : vector);
					return;
				}
				this.OffScreen = true;
				base.gameObject.transform.position = ((this.Mode == OverheadNameplateMode.UISpace) ? new Vector3(-200f, -200f, 0f) : vector);
			}
		}

		// Token: 0x06002D89 RID: 11657 RVA: 0x0014F114 File Offset: 0x0014D314
		private bool IsVisibleToCamera(ref Vector3 pos, out Vector3 viewPortSpace)
		{
			if (!ClientGameManager.MainCamera)
			{
				viewPortSpace = Vector3.zero;
				return false;
			}
			viewPortSpace = ClientGameManager.MainCamera.WorldToViewportPoint(pos);
			return viewPortSpace.x >= 0f && viewPortSpace.x <= 1f && viewPortSpace.y >= 0f && viewPortSpace.y <= 1f && viewPortSpace.z >= 0f;
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x0005F9EA File Offset: 0x0005DBEA
		public void UIManagerOnUiHiddenChanged()
		{
			this.m_canvasGroup.alpha = (UIManager.UiHidden ? 0f : 1f);
		}

		// Token: 0x06002D8B RID: 11659 RVA: 0x0005FA0A File Offset: 0x0005DC0A
		public void InitializeChat(ChatMessage msg)
		{
			if (this.m_chatBubbleController)
			{
				this.m_chatBubbleController.InitializeChat(msg);
			}
		}

		// Token: 0x06002D8C RID: 11660 RVA: 0x0005FA25 File Offset: 0x0005DC25
		public void InitializeParsedChat(ChatMessage msg)
		{
			if (this.m_chatBubbleController)
			{
				this.m_chatBubbleController.InitializeParsedChat(msg);
			}
		}

		// Token: 0x04002D0C RID: 11532
		[SerializeField]
		private OverheadNameplateMode m_mode;

		// Token: 0x04002D0D RID: 11533
		[SerializeField]
		private Canvas m_canvas;

		// Token: 0x04002D0E RID: 11534
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x04002D0F RID: 11535
		[SerializeField]
		private LayoutElement m_nameplateLayoutElement;

		// Token: 0x04002D10 RID: 11536
		[SerializeField]
		private RotateToFaceCamera m_rotator;

		// Token: 0x04002D11 RID: 11537
		[SerializeField]
		private WorldSpaceNameplateController m_nameplateController;

		// Token: 0x04002D12 RID: 11538
		[SerializeField]
		private WorldSpaceChatBubbleController m_chatBubbleController;

		// Token: 0x04002D13 RID: 11539
		[SerializeField]
		private WorldSpaceInteractiveController m_interactiveController;

		// Token: 0x04002D14 RID: 11540
		public static readonly Vector3 kDefaultHeightOffset = Vector3.up * 2f;

		// Token: 0x04002D18 RID: 11544
		private float m_deadDelta;

		// Token: 0x04002D19 RID: 11545
		private const int kMaxValue = 32767;
	}
}
