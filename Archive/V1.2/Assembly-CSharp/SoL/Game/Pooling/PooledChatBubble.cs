using System;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007D6 RID: 2006
	public class PooledChatBubble : PooledObject
	{
		// Token: 0x17000D5F RID: 3423
		// (get) Token: 0x06003A7D RID: 14973 RVA: 0x00067A17 File Offset: 0x00065C17
		// (set) Token: 0x06003A7E RID: 14974 RVA: 0x00067A1F File Offset: 0x00065C1F
		public float TargetAlpha { get; set; }

		// Token: 0x17000D60 RID: 3424
		// (get) Token: 0x06003A7F RID: 14975 RVA: 0x00067A28 File Offset: 0x00065C28
		// (set) Token: 0x06003A80 RID: 14976 RVA: 0x00067A30 File Offset: 0x00065C30
		private OverheadNameplateMode Mode
		{
			get
			{
				return this.m_mode;
			}
			set
			{
				if (this.m_mode == value)
				{
					return;
				}
				this.m_mode = value;
			}
		}

		// Token: 0x06003A81 RID: 14977 RVA: 0x001780A0 File Offset: 0x001762A0
		public void Init(WorldSpaceChatBubbleController parent, ChatMessage msg, bool noparse)
		{
			base.gameObject.transform.localScale = Vector3.one;
			this.m_canvasGroup.alpha = 0f;
			this.TargetAlpha = 1f;
			this.m_parent = parent;
			this.Mode = ((this.m_parent && this.m_parent.Controller) ? this.m_parent.Controller.Mode : OverheadNameplateMode.WorldSpace);
			Color color;
			msg.Type.GetColor(out color, false);
			if (noparse)
			{
				this.m_text.SetTextFormat("<color={0}><noparse>{1}</noparse></color>", color.ToHex(), msg.ContentsLinkified);
			}
			else
			{
				this.m_text.SetTextFormat("<color={0}>{1}</color>", color.ToHex(), msg.ContentsLinkified);
			}
			this.m_hideTime = new float?(Time.time + GlobalSettings.Values.Chat.OverheadChatTime);
		}

		// Token: 0x06003A82 RID: 14978 RVA: 0x00178188 File Offset: 0x00176388
		protected override void Update()
		{
			if (this.m_hideTime != null && Time.time > this.m_hideTime.Value)
			{
				this.m_hideTime = null;
				this.TargetAlpha = 0f;
				this.m_returnTime = new float?(Time.time + 0.2f);
			}
			if (this.m_canvasGroup.alpha != this.TargetAlpha)
			{
				this.m_canvasGroup.alpha = Mathf.MoveTowards(this.m_canvasGroup.alpha, this.TargetAlpha, Time.deltaTime * this.m_alphaLerpSpeed);
			}
			base.Update();
		}

		// Token: 0x06003A83 RID: 14979 RVA: 0x00178228 File Offset: 0x00176428
		public override void ResetPooledObject()
		{
			if (this.m_parent)
			{
				this.m_parent.RemoveBubble(this);
			}
			base.ResetPooledObject();
			if (this.m_text)
			{
				this.m_text.text = null;
			}
			this.TargetAlpha = 0f;
			this.m_canvasGroup.alpha = 0f;
		}

		// Token: 0x06003A84 RID: 14980 RVA: 0x00067A43 File Offset: 0x00065C43
		public void EarlyRemove()
		{
			this.m_parent = null;
			this.ReturnToPool();
		}

		// Token: 0x040038FC RID: 14588
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x040038FD RID: 14589
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x040038FE RID: 14590
		[SerializeField]
		private float m_alphaLerpSpeed = 1f;

		// Token: 0x04003900 RID: 14592
		private float? m_hideTime;

		// Token: 0x04003901 RID: 14593
		private WorldSpaceChatBubbleController m_parent;

		// Token: 0x04003902 RID: 14594
		private OverheadNameplateMode m_mode;
	}
}
