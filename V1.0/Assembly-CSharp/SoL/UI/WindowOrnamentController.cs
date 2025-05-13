using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x020003A5 RID: 933
	public class WindowOrnamentController : MonoBehaviour
	{
		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001984 RID: 6532 RVA: 0x00054031 File Offset: 0x00052231
		// (set) Token: 0x06001985 RID: 6533 RVA: 0x00054039 File Offset: 0x00052239
		public OrnamentType Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
				this.OnOrnamentTypeChanged();
			}
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x00106BA0 File Offset: 0x00104DA0
		public void SetColor(Color color)
		{
			for (int i = 0; i < this.m_sets.Length; i++)
			{
				for (int j = 0; j < this.m_sets[i].Ornaments.Length; j++)
				{
					if (this.m_sets[i].Ornaments[j] != null)
					{
						this.m_sets[i].Ornaments[j].color = color;
					}
				}
			}
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x00106C08 File Offset: 0x00104E08
		private void OnOrnamentTypeChanged()
		{
			for (int i = 0; i < this.m_sets.Length; i++)
			{
				this.m_sets[i].Parent.SetActive(this.m_sets[i].Type == this.m_type);
			}
		}

		// Token: 0x0400207A RID: 8314
		[SerializeField]
		private OrnamentType m_type;

		// Token: 0x0400207B RID: 8315
		[SerializeField]
		private WindowOrnamentController.OrnamentSet[] m_sets;

		// Token: 0x020003A6 RID: 934
		[Serializable]
		private class OrnamentSet
		{
			// Token: 0x0400207C RID: 8316
			public OrnamentType Type;

			// Token: 0x0400207D RID: 8317
			public GameObject Parent;

			// Token: 0x0400207E RID: 8318
			public Image[] Ornaments;
		}
	}
}
