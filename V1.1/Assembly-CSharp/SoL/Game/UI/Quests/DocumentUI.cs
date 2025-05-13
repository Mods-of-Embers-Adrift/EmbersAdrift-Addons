using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using Ink;
using Ink.Runtime;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000940 RID: 2368
	public class DocumentUI : DraggableUIWindow
	{
		// Token: 0x17000F9E RID: 3998
		// (get) Token: 0x060045EC RID: 17900 RVA: 0x0006F0F6 File Offset: 0x0006D2F6
		// (set) Token: 0x060045ED RID: 17901 RVA: 0x0006F0FE File Offset: 0x0006D2FE
		public IInteractive CurrentInteractive { get; set; }

		// Token: 0x060045EE RID: 17902 RVA: 0x001A1CD4 File Offset: 0x0019FED4
		protected override void Start()
		{
			base.Start();
			if (this.m_initialDetailContentY == null)
			{
				this.m_initialDetailContentY = new float?(this.m_detailContentRect.localPosition.y);
			}
			if (this.m_initialDetailContentHeight == null)
			{
				this.m_initialDetailContentHeight = new float?(this.m_detailContentRect.rect.height);
			}
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x0006F107 File Offset: 0x0006D307
		protected void Update()
		{
			if (base.Visible && LocalPlayer.GameEntity && !this.CurrentInteractive.CanInteract(LocalPlayer.GameEntity))
			{
				this.Hide(false);
			}
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x001A1D3C File Offset: 0x0019FF3C
		public void SetInkFile(TextAsset inkFile)
		{
			if (this.m_story != null)
			{
				this.m_story.onError -= DocumentUI.OnError;
			}
			this.m_story = new Story(inkFile.text);
			this.m_story.onError += DocumentUI.OnError;
			this.RefreshVisuals();
		}

		// Token: 0x060045F1 RID: 17905 RVA: 0x001A1D9C File Offset: 0x0019FF9C
		private void RefreshVisuals()
		{
			if (this.m_story != null)
			{
				Container container = this.m_story.KnotContainerWithName("DOCUMENT");
				bool? flag;
				if (container == null)
				{
					flag = null;
				}
				else
				{
					Dictionary<string, Ink.Runtime.Object> namedOnlyContent = container.namedOnlyContent;
					flag = ((namedOnlyContent != null) ? new bool?(namedOnlyContent.ContainsKey("TITLE")) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					this.m_header.gameObject.SetActive(true);
					this.m_detailContentRect.SetLocalPositionAndRotation(new Vector3(30f, this.m_initialDetailContentY.Value, 0f), Quaternion.identity);
					this.m_story.ChoosePathString("DOCUMENT.TITLE", true, Array.Empty<object>());
					if (!this.m_story.canContinue)
					{
						Debug.LogError("Invalid document ink file!");
						return;
					}
					this.m_title.ZStringSetText(this.m_story.Continue());
				}
				else
				{
					this.m_header.gameObject.SetActive(false);
					this.m_detailContentRect.SetLocalPositionAndRotation(new Vector3(30f, 40f, 0f), Quaternion.identity);
				}
				this.m_story.ChoosePathString("DOCUMENT.BODY", true, Array.Empty<object>());
				StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
				while (this.m_story.canContinue)
				{
					fromPool.Append(this.m_story.Continue());
				}
				this.m_body.ZStringSetText(fromPool.ToString_ReturnToPool());
				this.m_detailContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Math.Max(this.m_initialDetailContentHeight.Value, this.m_body.preferredHeight + 40f));
			}
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x0006688C File Offset: 0x00064A8C
		private static void OnError(string message, ErrorType type)
		{
			Debug.Log("Ink runtime error: " + message);
		}

		// Token: 0x04004229 RID: 16937
		[SerializeField]
		private Image m_header;

		// Token: 0x0400422A RID: 16938
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x0400422B RID: 16939
		[SerializeField]
		private TextMeshProUGUI m_body;

		// Token: 0x0400422C RID: 16940
		[SerializeField]
		private RectTransform m_detailContentRect;

		// Token: 0x0400422D RID: 16941
		private Story m_story;

		// Token: 0x0400422E RID: 16942
		private float? m_initialDetailContentY;

		// Token: 0x0400422F RID: 16943
		private float? m_initialDetailContentHeight;
	}
}
