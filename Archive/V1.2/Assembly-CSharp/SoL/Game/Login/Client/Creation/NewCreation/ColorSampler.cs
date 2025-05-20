using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B63 RID: 2915
	[CreateAssetMenu(menuName = "SoL/Creation/Color Sampler")]
	public class ColorSampler : ScriptableObject
	{
		// Token: 0x170014E0 RID: 5344
		// (get) Token: 0x060059A2 RID: 22946 RVA: 0x0007C0A7 File Offset: 0x0007A2A7
		private bool m_showSamples
		{
			get
			{
				return this.m_texture != null;
			}
		}

		// Token: 0x170014E1 RID: 5345
		// (get) Token: 0x060059A3 RID: 22947 RVA: 0x0007C0B5 File Offset: 0x0007A2B5
		public int ColorCount
		{
			get
			{
				return this.m_colorData.Length;
			}
		}

		// Token: 0x060059A4 RID: 22948 RVA: 0x0007C0BF File Offset: 0x0007A2BF
		public IEnumerable<Color> GetColors()
		{
			int num;
			for (int i = 0; i < this.m_colorData.Length; i = num + 1)
			{
				yield return this.m_colorData[i].Color;
				num = i;
			}
			yield break;
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x0007C0CF File Offset: 0x0007A2CF
		public Color GetColorByIndex(int index)
		{
			if (index < this.ColorCount)
			{
				return this.m_colorData[index].Color;
			}
			return Color.white;
		}

		// Token: 0x060059A6 RID: 22950 RVA: 0x0007C0ED File Offset: 0x0007A2ED
		public string GetNameByIndex(int index)
		{
			if (index < this.ColorCount)
			{
				return this.m_colorData[index].Name;
			}
			return string.Empty;
		}

		// Token: 0x060059A7 RID: 22951 RVA: 0x001EA8C0 File Offset: 0x001E8AC0
		public Color GetRandomColor(System.Random random = null)
		{
			int index = (random != null) ? random.Next(0, this.ColorCount) : UnityEngine.Random.Range(0, this.ColorCount);
			return this.GetColorByIndex(index);
		}

		// Token: 0x060059A8 RID: 22952 RVA: 0x001EA8F4 File Offset: 0x001E8AF4
		private void Sample()
		{
			if (this.m_texture == null)
			{
				return;
			}
			float width = (float)this.m_texture.width;
			int height = this.m_texture.height;
			float num = width / (float)this.m_sampleWidth;
			float num2 = (float)height / (float)this.m_sampleHeight;
			float num3 = num * 0.5f;
			float num4 = num2 * 0.5f;
			List<ColorSampler.ColorData> list = new List<ColorSampler.ColorData>();
			for (int i = 0; i < this.m_sampleWidth; i++)
			{
				for (int j = this.m_sampleHeight - 1; j >= 0; j--)
				{
					int x = Mathf.FloorToInt((float)i * num + num3);
					int y = Mathf.FloorToInt((float)j * num2 + num4);
					Color pixel = this.m_texture.GetPixel(x, y);
					Debug.Log(string.Concat(new string[]
					{
						"[",
						x.ToString(),
						", ",
						y.ToString(),
						"] = ",
						pixel.ToString()
					}));
					list.Add(new ColorSampler.ColorData
					{
						Color = pixel
					});
				}
			}
			this.m_colorData = list.ToArray();
		}

		// Token: 0x060059A9 RID: 22953 RVA: 0x0004475B File Offset: 0x0004295B
		private void CopyColorList()
		{
		}

		// Token: 0x04004EDA RID: 20186
		[SerializeField]
		private Texture2D m_texture;

		// Token: 0x04004EDB RID: 20187
		[SerializeField]
		private int m_sampleWidth = 10;

		// Token: 0x04004EDC RID: 20188
		[SerializeField]
		private int m_sampleHeight = 10;

		// Token: 0x04004EDD RID: 20189
		[SerializeField]
		private ColorSampler.ColorData[] m_colorData;

		// Token: 0x02000B64 RID: 2916
		private enum StartCorner
		{
			// Token: 0x04004EDF RID: 20191
			UpperLeft,
			// Token: 0x04004EE0 RID: 20192
			LowerLeft,
			// Token: 0x04004EE1 RID: 20193
			UpperRight,
			// Token: 0x04004EE2 RID: 20194
			LowerRight
		}

		// Token: 0x02000B65 RID: 2917
		[Serializable]
		private class ColorData
		{
			// Token: 0x04004EE3 RID: 20195
			public Color Color;

			// Token: 0x04004EE4 RID: 20196
			public string Name;
		}
	}
}
