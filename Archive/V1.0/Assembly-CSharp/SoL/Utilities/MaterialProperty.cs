using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002D1 RID: 721
	[Serializable]
	public abstract class MaterialProperty<T>
	{
		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x060014E1 RID: 5345 RVA: 0x000508E4 File Offset: 0x0004EAE4
		public Material Material
		{
			get
			{
				return this.m_material;
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x000508EC File Offset: 0x0004EAEC
		protected virtual int PropertyKey
		{
			get
			{
				return this.m_propertyKey;
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x060014E3 RID: 5347 RVA: 0x000508F4 File Offset: 0x0004EAF4
		public T CurrentValue
		{
			get
			{
				return this.m_currentValue;
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x060014E4 RID: 5348 RVA: 0x000508FC File Offset: 0x0004EAFC
		public bool HasMaterial
		{
			get
			{
				return this.m_material;
			}
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00050909 File Offset: 0x0004EB09
		public void Init(Material material)
		{
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			this.m_material = material;
			this.m_propertyKey = Shader.PropertyToID(this.m_propertyName);
			this.GetDefaultValue();
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0005093D File Offset: 0x0004EB3D
		public void ResetDefaultValue()
		{
			this.m_currentValue = this.m_defaultValue;
			this.SetValueInternal();
		}

		// Token: 0x060014E7 RID: 5351
		protected abstract void GetDefaultValue();

		// Token: 0x060014E8 RID: 5352
		protected abstract void SetValueInternal();

		// Token: 0x060014E9 RID: 5353 RVA: 0x00050951 File Offset: 0x0004EB51
		public void SetValue(T value)
		{
			this.m_currentValue = value;
			this.SetValueInternal();
		}

		// Token: 0x04001D3C RID: 7484
		[SerializeField]
		private string m_propertyName;

		// Token: 0x04001D3D RID: 7485
		[NonSerialized]
		protected Material m_material;

		// Token: 0x04001D3E RID: 7486
		[NonSerialized]
		protected int m_propertyKey;

		// Token: 0x04001D3F RID: 7487
		[NonSerialized]
		protected T m_defaultValue;

		// Token: 0x04001D40 RID: 7488
		[NonSerialized]
		protected T m_currentValue;
	}
}
