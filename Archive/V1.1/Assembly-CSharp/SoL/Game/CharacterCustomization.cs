using System;
using System.Collections;
using SoL.Game.Objects;
using SoL.Networking.Database;
using SoL.Utilities;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x0200055F RID: 1375
	[CreateAssetMenu(menuName = "SoL/UMA/Character Customization", order = 5)]
	public class CharacterCustomization : Identifiable
	{
		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x060029AA RID: 10666 RVA: 0x0005CCDF File Offset: 0x0005AEDF
		public CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x060029AB RID: 10667 RVA: 0x0005CCE7 File Offset: 0x0005AEE7
		public CharacterCustomizationType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x060029AC RID: 10668 RVA: 0x0005CCEF File Offset: 0x0005AEEF
		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x060029AD RID: 10669 RVA: 0x0005CCF7 File Offset: 0x0005AEF7
		public bool Available
		{
			get
			{
				return this.m_available;
			}
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x060029AE RID: 10670 RVA: 0x0005CCFF File Offset: 0x0005AEFF
		public UMAWardrobeRecipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x060029AF RID: 10671 RVA: 0x0005CD07 File Offset: 0x0005AF07
		public Image Image
		{
			get
			{
				return this.m_image;
			}
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x0005CD0F File Offset: 0x0005AF0F
		public IEnumerable GetRecipes()
		{
			return SolOdinUtilities.GetDropdownItems<UMAWardrobeRecipe>();
		}

		// Token: 0x04002AA1 RID: 10913
		[SerializeField]
		private bool m_available = true;

		// Token: 0x04002AA2 RID: 10914
		[SerializeField]
		private CharacterSex m_sex;

		// Token: 0x04002AA3 RID: 10915
		[SerializeField]
		private CharacterCustomizationType m_type;

		// Token: 0x04002AA4 RID: 10916
		[SerializeField]
		private string m_displayName;

		// Token: 0x04002AA5 RID: 10917
		[Header("Data")]
		[SerializeField]
		private UMAWardrobeRecipe m_recipe;

		// Token: 0x04002AA6 RID: 10918
		[SerializeField]
		private Image m_image;
	}
}
