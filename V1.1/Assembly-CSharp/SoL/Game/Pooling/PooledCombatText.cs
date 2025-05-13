using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Pooling
{
	// Token: 0x020007D7 RID: 2007
	public class PooledCombatText : PooledObject
	{
		// Token: 0x06003A86 RID: 14982 RVA: 0x0017822C File Offset: 0x0017642C
		public void Init(string text, Transform parent, Vector3 pos, Quaternion rot, Color color, Sprite icon)
		{
			if (this.m_uiText)
			{
				this.m_uiText.text = text;
				this.m_uiText.color = color;
				if (icon)
				{
					this.m_uiText.horizontalAlignment = HorizontalAlignmentOptions.Right;
					this.m_icon.overrideSprite = icon;
					this.m_iconPanel.alpha = 1f;
				}
				else
				{
					this.m_uiText.horizontalAlignment = HorizontalAlignmentOptions.Center;
					this.m_iconPanel.alpha = 0f;
					this.m_icon.overrideSprite = null;
				}
			}
			else if (this.m_text)
			{
				this.m_text.text = text;
				this.m_text.color = color;
			}
			this.m_timestamp = Time.time;
			pos += this.m_initialOffset;
			pos += new Vector3(UnityEngine.Random.Range(-this.m_randomOffset.x, this.m_randomOffset.x), UnityEngine.Random.Range(this.m_randomOffset.y, this.m_randomOffset.y * 2f), UnityEngine.Random.Range(-this.m_randomOffset.z, this.m_randomOffset.z));
			if (Options.GameOptions.ShowOverheadNameplates)
			{
				if (this.m_uiText)
				{
					pos.y += 0.4f;
				}
				else
				{
					pos.y += 0.25f;
				}
			}
			base.Initialize(parent, pos, rot);
		}

		// Token: 0x06003A87 RID: 14983 RVA: 0x001783A8 File Offset: 0x001765A8
		public override void ResetPooledObject()
		{
			base.ResetPooledObject();
			if (this.m_text != null)
			{
				this.m_text.text = null;
			}
			if (base.gameObject != null)
			{
				base.gameObject.transform.localScale = Vector3.one;
			}
			this.m_timestamp = 0f;
		}

		// Token: 0x06003A88 RID: 14984 RVA: 0x00067A65 File Offset: 0x00065C65
		public bool UpdateExternal()
		{
			return Time.time - this.m_timestamp >= 2.5f;
		}

		// Token: 0x04003903 RID: 14595
		[SerializeField]
		private TextMeshProUGUI m_uiText;

		// Token: 0x04003904 RID: 14596
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003905 RID: 14597
		[SerializeField]
		private CanvasGroup m_iconPanel;

		// Token: 0x04003906 RID: 14598
		[SerializeField]
		private TextMeshPro m_text;

		// Token: 0x04003907 RID: 14599
		[SerializeField]
		private Vector3 m_initialOffset = Vector3.zero;

		// Token: 0x04003908 RID: 14600
		[SerializeField]
		private Vector3 m_randomOffset = Vector3.zero;

		// Token: 0x04003909 RID: 14601
		private float m_timestamp;
	}
}
