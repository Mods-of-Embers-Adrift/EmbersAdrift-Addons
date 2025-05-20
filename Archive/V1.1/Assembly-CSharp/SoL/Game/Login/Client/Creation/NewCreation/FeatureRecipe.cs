using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B68 RID: 2920
	[CreateAssetMenu(menuName = "SoL/Creation/Feature Recipe")]
	public class FeatureRecipe : BaseArchetype
	{
		// Token: 0x170014E6 RID: 5350
		// (get) Token: 0x060059C2 RID: 22978 RVA: 0x0007C2B8 File Offset: 0x0007A4B8
		public UMAWardrobeRecipe WardrobeRecipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x060059C3 RID: 22979 RVA: 0x0005CD0F File Offset: 0x0005AF0F
		public IEnumerable GetRecipes()
		{
			return SolOdinUtilities.GetDropdownItems<UMAWardrobeRecipe>();
		}

		// Token: 0x170014E7 RID: 5351
		// (get) Token: 0x060059C4 RID: 22980 RVA: 0x0007C2C0 File Offset: 0x0007A4C0
		private bool m_showCopyDisplayName
		{
			get
			{
				return this.m_recipe != null && this.m_recipe.UserField != this.DisplayName;
			}
		}

		// Token: 0x04004EF2 RID: 20210
		[SerializeField]
		private UMAWardrobeRecipe m_recipe;
	}
}
