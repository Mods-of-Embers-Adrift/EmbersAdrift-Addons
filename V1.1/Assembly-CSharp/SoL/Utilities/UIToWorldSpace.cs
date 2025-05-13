using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F9 RID: 761
	public class UIToWorldSpace : MonoBehaviour
	{
		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001582 RID: 5506 RVA: 0x0005121F File Offset: 0x0004F41F
		// (set) Token: 0x06001583 RID: 5507 RVA: 0x00051227 File Offset: 0x0004F427
		public GameObject Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
			}
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00051230 File Offset: 0x0004F430
		private void Awake()
		{
			this.m_rect = (base.transform as RectTransform);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x000FCAF8 File Offset: 0x000FACF8
		private void Update()
		{
			if (this.m_parent && ClientGameManager.MainCamera)
			{
				base.gameObject.transform.position = ClientGameManager.MainCamera.WorldToScreenPoint(this.m_parent.gameObject.transform.position) - this.m_positionOffset;
				if (this.m_fixedVerticalRectPos && this.m_rect)
				{
					Vector2 anchoredPosition = this.m_rect.anchoredPosition;
					anchoredPosition.y = this.m_fixedVerticalValue;
					this.m_rect.anchoredPosition = anchoredPosition;
				}
			}
		}

		// Token: 0x04001D98 RID: 7576
		[SerializeField]
		private GameObject m_parent;

		// Token: 0x04001D99 RID: 7577
		[SerializeField]
		private Vector3 m_positionOffset = Vector3.zero;

		// Token: 0x04001D9A RID: 7578
		[SerializeField]
		private bool m_fixedVerticalRectPos;

		// Token: 0x04001D9B RID: 7579
		[SerializeField]
		private float m_fixedVerticalValue;

		// Token: 0x04001D9C RID: 7580
		private RectTransform m_rect;
	}
}
