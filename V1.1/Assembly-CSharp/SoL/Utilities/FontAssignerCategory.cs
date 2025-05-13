using System;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000288 RID: 648
	[Serializable]
	public class FontAssignerCategory
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0004FF66 File Offset: 0x0004E166
		public FontAssigner.FontType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x060013F4 RID: 5108 RVA: 0x0004FF6E File Offset: 0x0004E16E
		public TMP_FontAsset Font
		{
			get
			{
				return this.m_font;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0004FF76 File Offset: 0x0004E176
		public Material MaterialPreset
		{
			get
			{
				return this.m_materialPreset;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0004FF7E File Offset: 0x0004E17E
		private bool m_showMaterialPresets
		{
			get
			{
				return this.m_font != null;
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0004FF8C File Offset: 0x0004E18C
		private bool m_hasMaterialPresets
		{
			get
			{
				return this.m_font != null && this.m_materialPresets != null && this.m_materialPresets.Length != 0;
			}
		}

		// Token: 0x04001C2D RID: 7213
		[SerializeField]
		private FontAssigner.FontType m_type;

		// Token: 0x04001C2E RID: 7214
		[SerializeField]
		private TMP_FontAsset m_font;

		// Token: 0x04001C2F RID: 7215
		[HideInInspector]
		[SerializeField]
		private Material[] m_materialPresets;

		// Token: 0x04001C30 RID: 7216
		[SerializeField]
		private Material m_materialPreset;
	}
}
