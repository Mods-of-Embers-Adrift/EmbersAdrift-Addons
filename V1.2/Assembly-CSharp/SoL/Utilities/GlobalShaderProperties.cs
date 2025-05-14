using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002CE RID: 718
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class GlobalShaderProperties : MonoBehaviour
	{
		// Token: 0x060014D9 RID: 5337 RVA: 0x000FBB48 File Offset: 0x000F9D48
		private void Update()
		{
			for (int i = 0; i < this.m_properties.Length; i++)
			{
				this.m_properties[i].Update();
			}
		}

		// Token: 0x04001D2A RID: 7466
		[SerializeField]
		private GlobalShaderProperties.ShaderProperty[] m_properties;

		// Token: 0x020002CF RID: 719
		private enum ShaderPropertyType
		{
			// Token: 0x04001D2C RID: 7468
			None,
			// Token: 0x04001D2D RID: 7469
			Texture,
			// Token: 0x04001D2E RID: 7470
			Color,
			// Token: 0x04001D2F RID: 7471
			Vector,
			// Token: 0x04001D30 RID: 7472
			Float,
			// Token: 0x04001D31 RID: 7473
			Int
		}

		// Token: 0x020002D0 RID: 720
		[Serializable]
		private class ShaderProperty
		{
			// Token: 0x17000512 RID: 1298
			// (get) Token: 0x060014DB RID: 5339 RVA: 0x000508A5 File Offset: 0x0004EAA5
			public string PropertyName
			{
				get
				{
					return this.m_propertyName;
				}
			}

			// Token: 0x17000513 RID: 1299
			// (get) Token: 0x060014DC RID: 5340 RVA: 0x000508AD File Offset: 0x0004EAAD
			private string GetColorPalette
			{
				get
				{
					return this.m_palette.GetColorPaletteName();
				}
			}

			// Token: 0x17000514 RID: 1300
			// (get) Token: 0x060014DD RID: 5341 RVA: 0x000508BA File Offset: 0x0004EABA
			private static int EmptyShaderProperty
			{
				get
				{
					return Shader.PropertyToID("UNKNOWN");
				}
			}

			// Token: 0x17000515 RID: 1301
			// (get) Token: 0x060014DE RID: 5342 RVA: 0x000FBB78 File Offset: 0x000F9D78
			private int PropertyId
			{
				get
				{
					if (this.m_propertyId == null)
					{
						int num = string.IsNullOrEmpty(this.m_propertyName) ? GlobalShaderProperties.ShaderProperty.EmptyShaderProperty : Shader.PropertyToID(this.m_propertyName);
						if (!Application.isPlaying)
						{
							return num;
						}
						this.m_propertyId = new int?(num);
					}
					return this.m_propertyId.Value;
				}
			}

			// Token: 0x060014DF RID: 5343 RVA: 0x000FBBD4 File Offset: 0x000F9DD4
			public void Update()
			{
				if (!this.m_enabled || this.PropertyId == GlobalShaderProperties.ShaderProperty.EmptyShaderProperty)
				{
					return;
				}
				switch (this.m_type)
				{
				case GlobalShaderProperties.ShaderPropertyType.Texture:
					if (this.m_texture)
					{
						Shader.SetGlobalTexture(this.PropertyId, this.m_texture);
						return;
					}
					break;
				case GlobalShaderProperties.ShaderPropertyType.Color:
					Shader.SetGlobalColor(this.PropertyId, this.m_color);
					return;
				case GlobalShaderProperties.ShaderPropertyType.Vector:
					Shader.SetGlobalVector(this.PropertyId, this.m_vector);
					return;
				case GlobalShaderProperties.ShaderPropertyType.Float:
					Shader.SetGlobalFloat(this.PropertyId, this.m_float);
					return;
				case GlobalShaderProperties.ShaderPropertyType.Int:
					Shader.SetGlobalInt(this.PropertyId, this.m_int);
					break;
				default:
					return;
				}
			}

			// Token: 0x04001D32 RID: 7474
			[SerializeField]
			private bool m_enabled;

			// Token: 0x04001D33 RID: 7475
			[SerializeField]
			private GlobalShaderProperties.ShaderPropertyType m_type;

			// Token: 0x04001D34 RID: 7476
			[SerializeField]
			private string m_propertyName;

			// Token: 0x04001D35 RID: 7477
			[SerializeField]
			private Texture m_texture;

			// Token: 0x04001D36 RID: 7478
			[SerializeField]
			private ColorPaletteTypes m_palette;

			// Token: 0x04001D37 RID: 7479
			[SerializeField]
			private Color m_color = Color.white;

			// Token: 0x04001D38 RID: 7480
			[SerializeField]
			private Vector4 m_vector = Vector4.zero;

			// Token: 0x04001D39 RID: 7481
			[SerializeField]
			private float m_float;

			// Token: 0x04001D3A RID: 7482
			[SerializeField]
			private int m_int;

			// Token: 0x04001D3B RID: 7483
			private int? m_propertyId;
		}
	}
}
