using System;
using System.Collections.Generic;
using SoL.Game.Messages;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A5 RID: 1445
	public class WorldSpaceChatBubbleController : MonoBehaviour
	{
		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06002D4A RID: 11594 RVA: 0x0005F78B File Offset: 0x0005D98B
		internal WorldSpaceOverheadController Controller
		{
			get
			{
				return this.m_controller;
			}
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x06002D4B RID: 11595 RVA: 0x0005F793 File Offset: 0x0005D993
		public bool IsActive
		{
			get
			{
				return Options.GameOptions.ShowOverheadChat.Value && this.m_chatBubbles != null && this.m_chatBubbles.Count > 0;
			}
		}

		// Token: 0x06002D4C RID: 11596 RVA: 0x0005F7B9 File Offset: 0x0005D9B9
		private void Awake()
		{
			this.m_chatBubbles = StaticListPool<PooledChatBubble>.GetFromPool();
			this.m_defaultScale = this.m_rectTransform.localScale.x;
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x0014D7CC File Offset: 0x0014B9CC
		public void LateUpdateExternal()
		{
			if (!this.IsActive)
			{
				this.Toggle(false);
				return;
			}
			this.Toggle(true);
			float distanceFraction = this.m_controller.GameEntity.GetCachedSqrDistanceFromCamera() / LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr;
			this.m_rectTransform.localScale = Vector3.one * GlobalSettings.Values.Nameplates.GetOverheadChatScale(this.m_defaultScale, distanceFraction, this.m_controller.Mode);
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x0014D848 File Offset: 0x0014BA48
		private void OnDestroy()
		{
			if (this.m_chatBubbles != null)
			{
				for (int i = 0; i < this.m_chatBubbles.Count; i++)
				{
					this.m_chatBubbles[i].EarlyRemove();
				}
				StaticListPool<PooledChatBubble>.ReturnToPool(this.m_chatBubbles);
			}
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x0005F7DC File Offset: 0x0005D9DC
		public void Init(WorldSpaceOverheadController controller)
		{
			this.m_controller = controller;
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x0005F7E5 File Offset: 0x0005D9E5
		public void InitializeChat(ChatMessage msg)
		{
			this.InitializeChatInternal(msg, true);
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x0005F7EF File Offset: 0x0005D9EF
		public void InitializeParsedChat(ChatMessage msg)
		{
			this.InitializeChatInternal(msg, false);
		}

		// Token: 0x06002D52 RID: 11602 RVA: 0x0014D890 File Offset: 0x0014BA90
		private void InitializeChatInternal(ChatMessage msg, bool noparse)
		{
			if (!Options.GameOptions.ShowOverheadChat.Value)
			{
				return;
			}
			if (!this.m_prefab || this.m_chatBubbles == null)
			{
				return;
			}
			PooledChatBubble pooledInstance = this.m_prefab.GetPooledInstance<PooledChatBubble>();
			pooledInstance.Initialize(base.gameObject.transform, Vector3.zero, Quaternion.identity);
			pooledInstance.Init(this, msg, noparse);
			this.m_chatBubbles.Add(pooledInstance);
			while (this.m_chatBubbles.Count > 3 && this.m_chatBubbles.Count > 0)
			{
				this.m_chatBubbles[0].EarlyRemove();
				this.m_chatBubbles.RemoveAt(0);
			}
			this.RefreshAlphas();
		}

		// Token: 0x06002D53 RID: 11603 RVA: 0x0005F7F9 File Offset: 0x0005D9F9
		public void RemoveBubble(PooledChatBubble bubble)
		{
			this.m_chatBubbles.Remove(bubble);
		}

		// Token: 0x06002D54 RID: 11604 RVA: 0x0005F808 File Offset: 0x0005DA08
		private void Toggle(bool isEnabled)
		{
			if (this.m_layoutGroup.enabled != isEnabled)
			{
				this.m_layoutGroup.enabled = isEnabled;
			}
		}

		// Token: 0x06002D55 RID: 11605 RVA: 0x0014D940 File Offset: 0x0014BB40
		private void RefreshAlphas()
		{
			float num = 1f;
			for (int i = this.m_chatBubbles.Count - 1; i >= 0; i--)
			{
				this.m_chatBubbles[i].TargetAlpha = num;
				num -= 0.16666667f;
			}
		}

		// Token: 0x04002CDC RID: 11484
		private const int kMaxChatBubbles = 3;

		// Token: 0x04002CDD RID: 11485
		private const float kMinAlpha = 0.5f;

		// Token: 0x04002CDE RID: 11486
		private const float kMaxAlpha = 1f;

		// Token: 0x04002CDF RID: 11487
		private const float kAlphaDelta = 0.16666667f;

		// Token: 0x04002CE0 RID: 11488
		[SerializeField]
		private LayoutGroup m_layoutGroup;

		// Token: 0x04002CE1 RID: 11489
		[SerializeField]
		private PooledChatBubble m_prefab;

		// Token: 0x04002CE2 RID: 11490
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04002CE3 RID: 11491
		private float m_defaultScale = 1f;

		// Token: 0x04002CE4 RID: 11492
		private List<PooledChatBubble> m_chatBubbles;

		// Token: 0x04002CE5 RID: 11493
		private WorldSpaceOverheadController m_controller;
	}
}
