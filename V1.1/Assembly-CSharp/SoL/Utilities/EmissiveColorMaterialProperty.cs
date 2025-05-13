using System;

namespace SoL.Utilities
{
	// Token: 0x020002D4 RID: 724
	public class EmissiveColorMaterialProperty : ColorMaterialProperty
	{
		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x060014F1 RID: 5361 RVA: 0x00050A08 File Offset: 0x0004EC08
		protected override int PropertyKey
		{
			get
			{
				return ShaderExtensions.kEmissiveColorId;
			}
		}
	}
}
