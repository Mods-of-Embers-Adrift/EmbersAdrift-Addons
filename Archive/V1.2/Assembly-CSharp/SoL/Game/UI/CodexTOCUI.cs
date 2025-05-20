using System;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x02000862 RID: 2146
	public class CodexTOCUI : MonoBehaviour
	{
		// Token: 0x06003E05 RID: 15877 RVA: 0x001842EC File Offset: 0x001824EC
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			ITooltip component = this.m_target.GetComponent<ITooltip>();
			ITooltipParameter tooltipParameter = (component != null) ? component.GetTooltipParameter() : null;
			if (tooltipParameter is ObjectTextTooltipParameter)
			{
				ObjectTextTooltipParameter objectTextTooltipParameter = (ObjectTextTooltipParameter)tooltipParameter;
				this.m_text.text = objectTextTooltipParameter.Text + "..........................................................";
			}
		}

		// Token: 0x06003E06 RID: 15878 RVA: 0x00069ED1 File Offset: 0x000680D1
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06003E07 RID: 15879 RVA: 0x00069EEF File Offset: 0x000680EF
		private void ButtonClicked()
		{
			this.m_target.isOn = true;
		}

		// Token: 0x04003C64 RID: 15460
		private const string kEllipses = "..........................................................";

		// Token: 0x04003C65 RID: 15461
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003C66 RID: 15462
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003C67 RID: 15463
		[SerializeField]
		private SolToggle m_target;
	}
}
