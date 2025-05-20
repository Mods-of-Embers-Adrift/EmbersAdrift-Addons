using System;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000856 RID: 2134
	public class BindingLabel : MonoBehaviour, IBindingLabel
	{
		// Token: 0x06003DA8 RID: 15784 RVA: 0x00069C23 File Offset: 0x00067E23
		private void Start()
		{
			BindingLabels.RegisterBinding(this, false);
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x00069C2C File Offset: 0x00067E2C
		private void OnDestroy()
		{
			BindingLabels.DeregisterBinding(this);
		}

		// Token: 0x17000E43 RID: 3651
		// (get) Token: 0x06003DAA RID: 15786 RVA: 0x00069C34 File Offset: 0x00067E34
		BindingType IBindingLabel.Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000E44 RID: 3652
		// (get) Token: 0x06003DAB RID: 15787 RVA: 0x00069C3C File Offset: 0x00067E3C
		int IBindingLabel.Index
		{
			get
			{
				return this.m_index;
			}
		}

		// Token: 0x17000E45 RID: 3653
		// (get) Token: 0x06003DAC RID: 15788 RVA: 0x00069C44 File Offset: 0x00067E44
		TextMeshProUGUI IBindingLabel.Label
		{
			get
			{
				return this.m_label;
			}
		}

		// Token: 0x17000E46 RID: 3654
		// (get) Token: 0x06003DAD RID: 15789 RVA: 0x00049FFA File Offset: 0x000481FA
		string IBindingLabel.FormattedString
		{
			get
			{
				return null;
			}
		}

		// Token: 0x04003C3B RID: 15419
		[SerializeField]
		private BindingType m_type;

		// Token: 0x04003C3C RID: 15420
		[SerializeField]
		private int m_index = -1;

		// Token: 0x04003C3D RID: 15421
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
