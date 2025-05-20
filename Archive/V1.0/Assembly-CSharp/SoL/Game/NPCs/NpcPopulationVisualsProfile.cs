using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x0200080A RID: 2058
	[CreateAssetMenu(menuName = "SoL/Profiles/NPC Population")]
	public class NpcPopulationVisualsProfile : BaseArchetype, INpcVisualProfile
	{
		// Token: 0x17000DA6 RID: 3494
		// (get) Token: 0x06003B9A RID: 15258 RVA: 0x00068522 File Offset: 0x00066722
		private bool m_hideSkinColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeSkinColor;
			}
		}

		// Token: 0x17000DA7 RID: 3495
		// (get) Token: 0x06003B9B RID: 15259 RVA: 0x00068534 File Offset: 0x00066734
		private bool m_hideHairColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeHairColor;
			}
		}

		// Token: 0x17000DA8 RID: 3496
		// (get) Token: 0x06003B9C RID: 15260 RVA: 0x00068546 File Offset: 0x00066746
		private bool m_hideEyeColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeEyeColor;
			}
		}

		// Token: 0x17000DA9 RID: 3497
		// (get) Token: 0x06003B9D RID: 15261 RVA: 0x00068558 File Offset: 0x00066758
		private bool m_hideFavoriteColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeFavoriteColor;
			}
		}

		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x06003B9E RID: 15262 RVA: 0x00049FFA File Offset: 0x000481FA
		public WardrobeRecipePairEnsemble Ensemble
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x0006856A File Offset: 0x0006676A
		public bool TryGetSex(out CharacterSex sex)
		{
			sex = this.m_sex;
			return this.m_sex > CharacterSex.None;
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x0006857D File Offset: 0x0006677D
		public CharacterBuildType GetBuildType(System.Random seed)
		{
			if (!this.m_randomizeBuildType)
			{
				return this.m_buildType;
			}
			return CharacterBuildTypeExtensions.GetRandomBuildType(this.m_buildTypeFlags, seed);
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x0017C270 File Offset: 0x0017A470
		public void SetDna(DynamicCharacterAvatar dca, System.Random seed, GameEntity entity)
		{
			if (seed == null)
			{
				throw new ArgumentException("seed");
			}
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			float size = this.m_randomizeSize ? this.m_sizeRange.RandomWithinRange(seed) : this.m_size;
			float tone = this.m_randomizeTone ? this.m_toneRange.RandomWithinRange(seed) : this.m_tone;
			dca.predefinedDNA = UMAManager.GetPredefinedDna(entity, size, tone);
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x0017C2E8 File Offset: 0x0017A4E8
		public void SetColors(DynamicCharacterAvatar dca, System.Random seed)
		{
			if (seed == null)
			{
				throw new ArgumentException("seed");
			}
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			if (this.m_randomizeColors)
			{
				UMAManager.RandomizeColorsNew(dca, seed);
				return;
			}
			this.SetIndividualColor(this.m_randomizeSkinColor, this.m_skinColor, dca, seed, CharacterColorType.Skin);
			this.SetIndividualColor(this.m_randomizeHairColor, this.m_hairColor, dca, seed, CharacterColorType.Hair);
			this.SetIndividualColor(this.m_randomizeEyeColor, this.m_eyeColor, dca, seed, CharacterColorType.Eyes);
			this.SetIndividualColor(this.m_randomizeFavoriteColor, this.m_favoriteColor, dca, seed, CharacterColorType.Favorite);
		}

		// Token: 0x06003BA3 RID: 15267 RVA: 0x0006859A File Offset: 0x0006679A
		private void SetIndividualColor(bool randomize, Color color, DynamicCharacterAvatar dca, System.Random seed, CharacterColorType colorType)
		{
			if (randomize)
			{
				UMAManager.SetRandomColorNew(dca, seed, colorType);
				return;
			}
			colorType.SetDcaSharedColor(dca, color);
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x0017C37C File Offset: 0x0017A57C
		public void SetCustomizations(DynamicCharacterAvatar dca, System.Random seed, CharacterSex sex)
		{
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			if (this.m_specifiedFeatures.Length == 0)
			{
				UMAManager.RandomizeCustomizationNew(dca, seed, sex);
				return;
			}
			if (NpcPopulationVisualsProfile.SpecifiedFeatures == null)
			{
				NpcPopulationVisualsProfile.SpecifiedFeatures = new HashSet<string>(10);
			}
			NpcPopulationVisualsProfile.SpecifiedFeatures.Clear();
			for (int i = 0; i < this.m_specifiedFeatures.Length; i++)
			{
				if (this.m_specifiedFeatures[i].Type != NpcPopulationVisualsProfile.FeatureType.None)
				{
					FeatureRecipe randomForSex = this.m_specifiedFeatures[i].GetRandomForSex(sex, seed);
					if (randomForSex != null && randomForSex.WardrobeRecipe != null)
					{
						dca.SetSlot(randomForSex.WardrobeRecipe);
					}
					NpcPopulationVisualsProfile.SpecifiedFeatures.Add(this.m_specifiedFeatures[i].Type.ToString());
				}
			}
			FeatureRecipeList[] recipeListForSex = GlobalSettings.Values.Uma.GetRecipeListForSex(sex);
			if (recipeListForSex != null)
			{
				for (int j = 0; j < recipeListForSex.Length; j++)
				{
					if (recipeListForSex[j] != null && recipeListForSex[j].Sex == sex && recipeListForSex[j].FeatureRecipes != null && recipeListForSex[j].FeatureRecipes.Length != 0 && !NpcPopulationVisualsProfile.SpecifiedFeatures.Contains(recipeListForSex[j].SlotName))
					{
						int num = seed.Next(recipeListForSex[j].FeatureRecipes.Length);
						FeatureRecipe featureRecipe = recipeListForSex[j].FeatureRecipes[num];
						if (featureRecipe != null && featureRecipe.WardrobeRecipe != null)
						{
							dca.SetSlot(featureRecipe.WardrobeRecipe);
						}
					}
				}
			}
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x0004475B File Offset: 0x0004295B
		public void LoadEquipmentData(DynamicCharacterAvatar dca)
		{
		}

		// Token: 0x04003A1A RID: 14874
		private const string kVisuals = "Visuals";

		// Token: 0x04003A1B RID: 14875
		private const string kSeedGroup = "Seed";

		// Token: 0x04003A1C RID: 14876
		private const string kBuildGroup = "Build";

		// Token: 0x04003A1D RID: 14877
		private const string kCustoGroup = "Customizations";

		// Token: 0x04003A1E RID: 14878
		private const string kColorGroup = "Colors";

		// Token: 0x04003A1F RID: 14879
		private const string kImportGroup = "Import";

		// Token: 0x04003A20 RID: 14880
		[SerializeField]
		private CharacterSex m_sex = CharacterSex.Male;

		// Token: 0x04003A21 RID: 14881
		[SerializeField]
		private bool m_randomizeBuildType;

		// Token: 0x04003A22 RID: 14882
		[SerializeField]
		private bool m_randomizeSize;

		// Token: 0x04003A23 RID: 14883
		[SerializeField]
		private bool m_randomizeTone;

		// Token: 0x04003A24 RID: 14884
		[SerializeField]
		private CharacterBuildType m_buildType = CharacterBuildType.Brawny;

		// Token: 0x04003A25 RID: 14885
		[SerializeField]
		private CharacterBuildTypeFlags m_buildTypeFlags = CharacterBuildTypeFlags.All;

		// Token: 0x04003A26 RID: 14886
		[Range(0f, 1f)]
		[SerializeField]
		private float m_size = 0.5f;

		// Token: 0x04003A27 RID: 14887
		[SerializeField]
		private MinMaxFloatRange m_sizeRange = new MinMaxFloatRange(0f, 1f);

		// Token: 0x04003A28 RID: 14888
		[Range(0f, 1f)]
		[SerializeField]
		private float m_tone = 0.5f;

		// Token: 0x04003A29 RID: 14889
		[SerializeField]
		private MinMaxFloatRange m_toneRange = new MinMaxFloatRange(0f, 1f);

		// Token: 0x04003A2A RID: 14890
		private const string kRandomizeLabel = "Randomize";

		// Token: 0x04003A2B RID: 14891
		private const string kSkinColorGroup = "Colors/Skin";

		// Token: 0x04003A2C RID: 14892
		private const string kHairColorGroup = "Colors/Hair";

		// Token: 0x04003A2D RID: 14893
		private const string kEyeColorGroup = "Colors/Eyes";

		// Token: 0x04003A2E RID: 14894
		private const string kFavoriteColorGroup = "Colors/Favorite";

		// Token: 0x04003A2F RID: 14895
		[SerializeField]
		private bool m_randomizeColors;

		// Token: 0x04003A30 RID: 14896
		[SerializeField]
		private bool m_randomizeSkinColor;

		// Token: 0x04003A31 RID: 14897
		[SerializeField]
		private Color m_skinColor = Color.white;

		// Token: 0x04003A32 RID: 14898
		[SerializeField]
		private bool m_randomizeHairColor;

		// Token: 0x04003A33 RID: 14899
		[SerializeField]
		private Color m_hairColor = Color.white;

		// Token: 0x04003A34 RID: 14900
		[SerializeField]
		private bool m_randomizeEyeColor;

		// Token: 0x04003A35 RID: 14901
		[SerializeField]
		private Color m_eyeColor = Color.white;

		// Token: 0x04003A36 RID: 14902
		[SerializeField]
		private bool m_randomizeFavoriteColor;

		// Token: 0x04003A37 RID: 14903
		[SerializeField]
		private Color m_favoriteColor = Color.white;

		// Token: 0x04003A38 RID: 14904
		private static HashSet<string> SpecifiedFeatures;

		// Token: 0x04003A39 RID: 14905
		[SerializeField]
		private NpcPopulationVisualsProfile.FeatureSpecifier[] m_specifiedFeatures;

		// Token: 0x0200080B RID: 2059
		public enum FeatureType
		{
			// Token: 0x04003A3B RID: 14907
			None,
			// Token: 0x04003A3C RID: 14908
			Hair_Top,
			// Token: 0x04003A3D RID: 14909
			Hair_Tail,
			// Token: 0x04003A3E RID: 14910
			Hair_Side,
			// Token: 0x04003A3F RID: 14911
			Hair_Eyebrows,
			// Token: 0x04003A40 RID: 14912
			Hair_Face,
			// Token: 0x04003A41 RID: 14913
			Face_Paint,
			// Token: 0x04003A42 RID: 14914
			Face_Makeup,
			// Token: 0x04003A43 RID: 14915
			Complexion
		}

		// Token: 0x0200080C RID: 2060
		[Serializable]
		public class FeatureSpecifier
		{
			// Token: 0x17000DAB RID: 3499
			// (get) Token: 0x06003BA7 RID: 15271 RVA: 0x000685B3 File Offset: 0x000667B3
			public NpcPopulationVisualsProfile.FeatureType Type
			{
				get
				{
					return this.m_type;
				}
			}

			// Token: 0x06003BA8 RID: 15272 RVA: 0x0017C5A0 File Offset: 0x0017A7A0
			public FeatureRecipe GetRandomForSex(CharacterSex sex, System.Random seed)
			{
				if (sex == CharacterSex.None)
				{
					throw new ArgumentException("Sex cannot be None!");
				}
				FeatureRecipe[] array = (sex == CharacterSex.Male) ? this.m_maleRecipes : this.m_femaleRecipes;
				if (array != null && array.Length != 0)
				{
					return array[seed.Next(0, array.Length)];
				}
				return null;
			}

			// Token: 0x17000DAC RID: 3500
			// (get) Token: 0x06003BA9 RID: 15273 RVA: 0x000685BB File Offset: 0x000667BB
			private IEnumerable GetEnsembles
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<FeatureRecipe>();
				}
			}

			// Token: 0x04003A44 RID: 14916
			[SerializeField]
			private NpcPopulationVisualsProfile.FeatureType m_type;

			// Token: 0x04003A45 RID: 14917
			[SerializeField]
			private FeatureRecipe[] m_maleRecipes;

			// Token: 0x04003A46 RID: 14918
			[SerializeField]
			private FeatureRecipe[] m_femaleRecipes;
		}
	}
}
