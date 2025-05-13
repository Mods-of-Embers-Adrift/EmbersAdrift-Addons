using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000349 RID: 841
	[ExecuteInEditMode]
	public class BackgroundBlockerSizeFitter : MonoBehaviour
	{
		// Token: 0x060016F8 RID: 5880 RVA: 0x00052158 File Offset: 0x00050358
		private void Awake()
		{
			this.m_rect.pivot = Vector2.one * 0.5f;
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x00052174 File Offset: 0x00050374
		private void Update()
		{
			this.m_rect.sizeDelta = new Vector2((float)Screen.width * 2f, (float)Screen.height * 2f);
			this.m_rect.anchoredPosition = Vector2.zero;
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x000521AE File Offset: 0x000503AE
		public void SetColor(Color color)
		{
			if (this.m_img)
			{
				this.m_img.color = color;
			}
		}

		// Token: 0x04001EC0 RID: 7872
		[SerializeField]
		private RectTransform m_rect;

		// Token: 0x04001EC1 RID: 7873
		[SerializeField]
		private Image m_img;
	}
}
