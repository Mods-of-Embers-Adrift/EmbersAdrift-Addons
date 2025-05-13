using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Networking.Database;
using UMA;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005DD RID: 1501
	[CreateAssetMenu(menuName = "SoL/UMA/Character Customization Collection", order = 5)]
	public class CharacterCustomizationCollection : IdentifiableScriptableObjectCollection<CharacterCustomization>
	{
		// Token: 0x06002FA8 RID: 12200 RVA: 0x00157988 File Offset: 0x00155B88
		public CharacterCustomization GetCustomizationByTextRecipe(UMATextRecipe textRecipe)
		{
			this.Initialize();
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (this.m_items[i].Available && this.m_items[i].Recipe.recipeString == textRecipe.recipeString)
				{
					return this.m_items[i];
				}
			}
			return null;
		}

		// Token: 0x06002FA9 RID: 12201 RVA: 0x00060E2A File Offset: 0x0005F02A
		protected override bool ShouldAddToCollection(CharacterCustomization item)
		{
			return base.ShouldAddToCollection(item) && item.Available && item.Sex > CharacterSex.None;
		}

		// Token: 0x06002FAA RID: 12202 RVA: 0x00060E48 File Offset: 0x0005F048
		public IEnumerable<CharacterCustomization> GetCustomizations()
		{
			this.Initialize();
			int num;
			for (int i = 0; i < this.m_items.Length; i = num + 1)
			{
				if (this.m_items[i].Available)
				{
					yield return this.m_items[i];
				}
				num = i;
			}
			yield break;
		}
	}
}
