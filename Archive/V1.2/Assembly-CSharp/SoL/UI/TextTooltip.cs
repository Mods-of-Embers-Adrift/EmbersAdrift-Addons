using System;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000382 RID: 898
	public class TextTooltip : BaseTooltip
	{
		// Token: 0x060018B9 RID: 6329 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void SetData()
		{
		}

		// Token: 0x04001FC7 RID: 8135
		[SerializeField]
		private TextMeshProUGUI m_text;
	}
}
