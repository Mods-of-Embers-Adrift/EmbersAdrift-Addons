using System;
using System.Collections.Generic;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B62 RID: 2914
	public class ColorPalette : MonoBehaviour
	{
		// Token: 0x170014DE RID: 5342
		// (get) Token: 0x06005998 RID: 22936 RVA: 0x0007C085 File Offset: 0x0007A285
		public CharacterColorType CharacterColorType
		{
			get
			{
				return this.m_colorType;
			}
		}

		// Token: 0x170014DF RID: 5343
		// (get) Token: 0x06005999 RID: 22937 RVA: 0x0007C08D File Offset: 0x0007A28D
		public SelectableColor SelectedColor
		{
			get
			{
				return this.m_selectedColor;
			}
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x001EA658 File Offset: 0x001E8858
		private void Awake()
		{
			this.m_selectableColors = new List<SelectableColor>();
			SelectableColor[] componentsInChildren = this.m_parent.GetComponentsInChildren<SelectableColor>();
			int num = 0;
			foreach (Color color in this.m_colorType.GetColorSampler().GetColors())
			{
				SelectableColor selectableColor;
				if (num < componentsInChildren.Length)
				{
					selectableColor = componentsInChildren[num];
				}
				else
				{
					selectableColor = UnityEngine.Object.Instantiate<GameObject>(this.m_colorSelectorPrefab, this.m_parent.transform).GetComponent<SelectableColor>();
				}
				if (selectableColor != null)
				{
					selectableColor.Init(this, color);
					this.m_selectableColors.Add(selectableColor);
					num++;
				}
			}
			if (num < componentsInChildren.Length)
			{
				for (int i = num; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.SetActive(false);
				}
			}
			if (this.m_randomizeStartColor)
			{
				this.RandomizeColor();
				return;
			}
			this.SelectIndex(0);
		}

		// Token: 0x0600599B RID: 22939 RVA: 0x001EA750 File Offset: 0x001E8950
		public void ColorSelected(SelectableColor selectableColor)
		{
			int index = this.m_selectableColors.IndexOf(selectableColor);
			this.SelectIndex(index);
			this.m_manager.ColorSelected(this.m_colorType, this.m_selectedColor.SwatchColor);
		}

		// Token: 0x0600599C RID: 22940 RVA: 0x001EA790 File Offset: 0x001E8990
		public void SelectColor(string colorHex)
		{
			Color rhs;
			if (ColorUtility.TryParseHtmlString(colorHex, out rhs))
			{
				for (int i = 0; i < this.m_selectableColors.Count; i++)
				{
					if (this.m_selectableColors[i].SwatchColor == rhs)
					{
						this.SelectIndex(i);
						return;
					}
				}
			}
			this.SelectIndex(0);
		}

		// Token: 0x0600599D RID: 22941 RVA: 0x0007C095 File Offset: 0x0007A295
		public void Init(NewCharacterManager manager)
		{
			this.m_manager = manager;
		}

		// Token: 0x0600599E RID: 22942 RVA: 0x001EA7E8 File Offset: 0x001E89E8
		public void RandomizeColor()
		{
			int index = UnityEngine.Random.Range(0, this.m_selectableColors.Count);
			this.SelectIndex(index);
		}

		// Token: 0x0600599F RID: 22943 RVA: 0x0007C09E File Offset: 0x0007A29E
		public void ResetColor()
		{
			this.SelectIndex(0);
		}

		// Token: 0x060059A0 RID: 22944 RVA: 0x001EA810 File Offset: 0x001E8A10
		private void SelectIndex(int index)
		{
			for (int i = 0; i < this.m_selectableColors.Count; i++)
			{
				bool flag = i == index;
				this.m_selectableColors[i].SetIsSelected(flag);
				if (flag)
				{
					this.m_selectedColor = this.m_selectableColors[i];
				}
			}
		}

		// Token: 0x04004ED3 RID: 20179
		[SerializeField]
		private bool m_randomizeStartColor;

		// Token: 0x04004ED4 RID: 20180
		[SerializeField]
		private CharacterColorType m_colorType;

		// Token: 0x04004ED5 RID: 20181
		[SerializeField]
		private GameObject m_colorSelectorPrefab;

		// Token: 0x04004ED6 RID: 20182
		[SerializeField]
		private GameObject m_parent;

		// Token: 0x04004ED7 RID: 20183
		private List<SelectableColor> m_selectableColors;

		// Token: 0x04004ED8 RID: 20184
		private NewCharacterManager m_manager;

		// Token: 0x04004ED9 RID: 20185
		private SelectableColor m_selectedColor;
	}
}
