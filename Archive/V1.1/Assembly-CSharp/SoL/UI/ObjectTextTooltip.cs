using System;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200037E RID: 894
	public class ObjectTextTooltip : BaseTooltip
	{
		// Token: 0x060018A0 RID: 6304 RVA: 0x0010512C File Offset: 0x0010332C
		protected override void SetData()
		{
			BaseTooltip.GetTooltipParameter parameterGetter = this.m_parameterGetter;
			ITooltipParameter tooltipParameter = (parameterGetter != null) ? parameterGetter() : null;
			if (tooltipParameter == null || !(tooltipParameter is ObjectTextTooltipParameter))
			{
				return;
			}
			ObjectTextTooltipParameter objectTextTooltipParameter = (ObjectTextTooltipParameter)tooltipParameter;
			this.m_text.text = objectTextTooltipParameter.Text;
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x00105170 File Offset: 0x00103370
		protected override void LateUpdateInternal()
		{
			TextMeshProUGUI samplerText = this.m_samplerText;
			samplerText.ZStringSetText(this.m_text.text);
			samplerText.ForceMeshUpdate(true, false);
			float x = Mathf.Clamp(samplerText.GetPreferredValues().x + 32f, 32f, 300f);
			Vector2 sizeDelta = this.m_parentRect.sizeDelta;
			sizeDelta.x = x;
			this.m_parentRect.sizeDelta = sizeDelta;
			base.LateUpdateInternal();
		}

		// Token: 0x04001FBA RID: 8122
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04001FBB RID: 8123
		[SerializeField]
		private TextMeshProUGUI m_samplerText;

		// Token: 0x04001FBC RID: 8124
		[SerializeField]
		private RectTransform m_parentRect;

		// Token: 0x04001FBD RID: 8125
		private const float kBuffer = 32f;

		// Token: 0x04001FBE RID: 8126
		private const float kMaxSize = 300f;
	}
}
