using System;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Penalties
{
	// Token: 0x0200095C RID: 2396
	public class PenaltiesUI : DraggableUIWindow
	{
		// Token: 0x040042E3 RID: 17123
		[SerializeField]
		private SolButton m_newPenaltyButton;

		// Token: 0x040042E4 RID: 17124
		[SerializeField]
		private TMP_InputField m_nameInput;

		// Token: 0x040042E5 RID: 17125
		[SerializeField]
		private SolButton m_refreshButton;

		// Token: 0x040042E6 RID: 17126
		[SerializeField]
		private PenaltiesList m_list;

		// Token: 0x040042E7 RID: 17127
		[SerializeField]
		private TextMeshProUGUI m_user;

		// Token: 0x040042E8 RID: 17128
		[SerializeField]
		private TextMeshProUGUI m_characters;

		// Token: 0x040042E9 RID: 17129
		[SerializeField]
		private Image m_newPenaltyPanel;

		// Token: 0x040042EA RID: 17130
		[SerializeField]
		private TMP_InputField m_new_nameInput;

		// Token: 0x040042EB RID: 17131
		[SerializeField]
		private TMP_Dropdown m_typeDropdown;

		// Token: 0x040042EC RID: 17132
		[SerializeField]
		private TMP_InputField m_new_descInput;

		// Token: 0x040042ED RID: 17133
		[SerializeField]
		private TMP_InputField m_new_daysInput;

		// Token: 0x040042EE RID: 17134
		[SerializeField]
		private TMP_InputField m_new_hoursInput;

		// Token: 0x040042EF RID: 17135
		[SerializeField]
		private TMP_InputField m_new_minsInput;

		// Token: 0x040042F0 RID: 17136
		[SerializeField]
		private SolButton m_new_cancelButton;

		// Token: 0x040042F1 RID: 17137
		[SerializeField]
		private SolButton m_new_okButton;

		// Token: 0x040042F2 RID: 17138
		[SerializeField]
		private Image m_noPenaltiesBlocker;
	}
}
