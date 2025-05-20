using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002D3 RID: 723
	[Serializable]
	public class ColorMaterialProperty : MaterialProperty<Color>
	{
		// Token: 0x060014EE RID: 5358 RVA: 0x000509B4 File Offset: 0x0004EBB4
		protected override void GetDefaultValue()
		{
			if (this.m_material)
			{
				this.m_defaultValue = this.m_material.GetColor(this.PropertyKey);
			}
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x000509DA File Offset: 0x0004EBDA
		protected override void SetValueInternal()
		{
			if (this.m_material)
			{
				this.m_material.SetColor(this.PropertyKey, this.m_currentValue);
			}
		}
	}
}
