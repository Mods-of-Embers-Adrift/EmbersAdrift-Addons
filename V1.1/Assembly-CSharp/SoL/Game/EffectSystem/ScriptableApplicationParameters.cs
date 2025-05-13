using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C42 RID: 3138
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Scriptable Application")]
	public class ScriptableApplicationParameters : ScriptableObject
	{
		// Token: 0x1700173E RID: 5950
		// (get) Token: 0x060060D2 RID: 24786 RVA: 0x00081392 File Offset: 0x0007F592
		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		// Token: 0x1700173F RID: 5951
		// (get) Token: 0x060060D3 RID: 24787 RVA: 0x0008139A File Offset: 0x0007F59A
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x17001740 RID: 5952
		// (get) Token: 0x060060D4 RID: 24788 RVA: 0x000813A2 File Offset: 0x0007F5A2
		public Sprite Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x0400537D RID: 21373
		[SerializeField]
		private string m_displayName;

		// Token: 0x0400537E RID: 21374
		[SerializeField]
		private Sprite m_icon;

		// Token: 0x0400537F RID: 21375
		[TextArea]
		[SerializeField]
		private string m_description;
	}
}
