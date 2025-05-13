using System;
using System.Collections;
using SoL.Game.UMA;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005E4 RID: 1508
	[CreateAssetMenu(menuName = "SoL/UMA/Wardrobe Recipe Pair", order = 5)]
	public class WardrobeRecipePair : ScriptableObject
	{
		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06002FD3 RID: 12243 RVA: 0x00060F6D File Offset: 0x0005F16D
		public MaterialColorType MaterialColorType
		{
			get
			{
				return this.m_materialColorType;
			}
		}

		// Token: 0x06002FD4 RID: 12244 RVA: 0x00060F75 File Offset: 0x0005F175
		private UMAWardrobeRecipe GetWardrobeRecipe(CharacterSex sex)
		{
			if (sex == CharacterSex.Male)
			{
				return this.m_male;
			}
			if (sex != CharacterSex.Female)
			{
				throw new ArgumentException("Unknown sex! " + sex.ToString());
			}
			return this.m_female;
		}

		// Token: 0x06002FD5 RID: 12245 RVA: 0x00158168 File Offset: 0x00156368
		public void OnEquipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, bool refresh = true, Color? overrideColor = null)
		{
			if (!dca)
			{
				throw new ArgumentNullException("dca");
			}
			UMAWardrobeRecipe wardrobeRecipe = this.GetWardrobeRecipe(sex);
			if (!wardrobeRecipe)
			{
				throw new ArgumentNullException("recipe");
			}
			Color? color = (overrideColor != null) ? overrideColor : this.m_colorOverride.GetColorOverride(null);
			if (color != null)
			{
				dca.SetColor(wardrobeRecipe.wardrobeSlot, color.Value, default(Color), 0f, false);
				this.m_customColorSet = true;
			}
			dca.SetSlot(wardrobeRecipe);
			if (refresh)
			{
				dca.Refresh(true, true, true);
			}
		}

		// Token: 0x06002FD6 RID: 12246 RVA: 0x0015820C File Offset: 0x0015640C
		public void OnUnequipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, bool refresh = true)
		{
			if (!dca)
			{
				throw new ArgumentNullException("dca");
			}
			UMAWardrobeRecipe wardrobeRecipe = this.GetWardrobeRecipe(sex);
			if (!wardrobeRecipe)
			{
				throw new ArgumentNullException("recipe");
			}
			if (this.m_customColorSet)
			{
				dca.SetColor(wardrobeRecipe.wardrobeSlot, WardrobeRecipePair.kDefaultColor, default(Color), 0f, false);
			}
			dca.ClearSlot(wardrobeRecipe.wardrobeSlot);
			if (refresh)
			{
				dca.Refresh(true, true, true);
			}
		}

		// Token: 0x06002FD7 RID: 12247 RVA: 0x00158288 File Offset: 0x00156488
		public void ResetColor(CharacterSex sex, DynamicCharacterAvatar dca, bool refresh = true)
		{
			if (!dca)
			{
				throw new ArgumentNullException("dca");
			}
			UMAWardrobeRecipe wardrobeRecipe = this.GetWardrobeRecipe(sex);
			if (!wardrobeRecipe)
			{
				throw new ArgumentNullException("recipe");
			}
			dca.SetColor(wardrobeRecipe.wardrobeSlot, WardrobeRecipePair.kDefaultColor, default(Color), 0f, false);
			if (refresh)
			{
				dca.Refresh(true, true, true);
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06002FD8 RID: 12248 RVA: 0x0005CD0F File Offset: 0x0005AF0F
		private IEnumerable GetWardrobeRecipeValues
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<UMAWardrobeRecipe>();
			}
		}

		// Token: 0x04002EBB RID: 11963
		private const string kColorGroup = "Color";

		// Token: 0x04002EBC RID: 11964
		private static Color kDefaultColor = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04002EBD RID: 11965
		[SerializeField]
		private UMAWardrobeRecipe m_male;

		// Token: 0x04002EBE RID: 11966
		[SerializeField]
		private UMAWardrobeRecipe m_female;

		// Token: 0x04002EBF RID: 11967
		[SerializeField]
		private MaterialColorType m_materialColorType;

		// Token: 0x04002EC0 RID: 11968
		[SerializeField]
		private ColorOverride m_colorOverride;

		// Token: 0x04002EC1 RID: 11969
		[NonSerialized]
		private bool m_customColorSet;
	}
}
