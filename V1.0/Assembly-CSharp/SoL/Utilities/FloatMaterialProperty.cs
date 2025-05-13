using System;

namespace SoL.Utilities
{
	// Token: 0x020002D2 RID: 722
	[Serializable]
	public class FloatMaterialProperty : MaterialProperty<float>
	{
		// Token: 0x060014EB RID: 5355 RVA: 0x00050960 File Offset: 0x0004EB60
		protected override void GetDefaultValue()
		{
			if (this.m_material)
			{
				this.m_defaultValue = this.m_material.GetFloat(this.PropertyKey);
			}
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00050986 File Offset: 0x0004EB86
		protected override void SetValueInternal()
		{
			if (this.m_material)
			{
				this.m_material.SetFloat(this.PropertyKey, this.m_currentValue);
			}
		}
	}
}
