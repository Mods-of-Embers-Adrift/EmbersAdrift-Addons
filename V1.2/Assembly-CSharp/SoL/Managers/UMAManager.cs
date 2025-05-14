using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Animation;
using SoL.Game.Login.Client.Creation;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Managers
{
	// Token: 0x02000523 RID: 1315
	public static class UMAManager
	{
		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06002762 RID: 10082 RVA: 0x0005B96D File Offset: 0x00059B6D
		public static UMADNARestrictions DnaRestrictions
		{
			get
			{
				if (UMAManager.m_restrictions == null)
				{
					UMAManager.m_restrictions = Resources.Load<UMADNARestrictions>("DNASliderRestrictions");
				}
				return UMAManager.m_restrictions;
			}
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x0005B990 File Offset: 0x00059B90
		public static Dictionary<CharacterCustomizationType, List<CharacterCustomization>> GetCustomizationDict(this CharacterSex sex)
		{
			if (sex == CharacterSex.Male)
			{
				return UMAManager.MaleCustomizations;
			}
			if (sex != CharacterSex.Female)
			{
				throw new ArgumentException("sex");
			}
			return UMAManager.FemaleCustomizations;
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06002764 RID: 10084 RVA: 0x0005B9B2 File Offset: 0x00059BB2
		public static CharacterCustomizationCollection Customizations
		{
			get
			{
				if (UMAManager.m_customizations == null)
				{
					UMAManager.InitCustomizations();
				}
				return UMAManager.m_customizations;
			}
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x06002765 RID: 10085 RVA: 0x0005B9CB File Offset: 0x00059BCB
		public static Dictionary<CharacterCustomizationType, List<CharacterCustomization>> MaleCustomizations
		{
			get
			{
				if (UMAManager.m_maleCustomizations == null)
				{
					UMAManager.InitCustomizations();
				}
				return UMAManager.m_maleCustomizations;
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06002766 RID: 10086 RVA: 0x0005B9DE File Offset: 0x00059BDE
		public static Dictionary<CharacterCustomizationType, List<CharacterCustomization>> FemaleCustomizations
		{
			get
			{
				if (UMAManager.m_femaleCustomizations == null)
				{
					UMAManager.InitCustomizations();
				}
				return UMAManager.m_femaleCustomizations;
			}
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x001375B4 File Offset: 0x001357B4
		private static void InitCustomizations()
		{
			if (UMAManager.m_initialized)
			{
				return;
			}
			UMAManager.m_customizations = Resources.Load<CharacterCustomizationCollection>("CharacterCustomizations");
			UMAManager.m_maleCustomizations = new Dictionary<CharacterCustomizationType, List<CharacterCustomization>>(default(CharacterCustomizationTypeComparer));
			UMAManager.m_femaleCustomizations = new Dictionary<CharacterCustomizationType, List<CharacterCustomization>>(default(CharacterCustomizationTypeComparer));
			foreach (CharacterCustomization characterCustomization in UMAManager.m_customizations.GetCustomizations())
			{
				if (characterCustomization.Available)
				{
					Dictionary<CharacterCustomizationType, List<CharacterCustomization>> dictionary = (characterCustomization.Sex == CharacterSex.Male) ? UMAManager.m_maleCustomizations : UMAManager.m_femaleCustomizations;
					if (dictionary.ContainsKey(characterCustomization.Type))
					{
						dictionary[characterCustomization.Type].Add(characterCustomization);
					}
					else
					{
						dictionary.Add(characterCustomization.Type, new List<CharacterCustomization>
						{
							characterCustomization
						});
					}
				}
			}
			UMAManager.m_initialized = true;
		}

		// Token: 0x06002768 RID: 10088 RVA: 0x001376A4 File Offset: 0x001358A4
		public static void BuildBaseDca(GameEntity entity, DynamicCharacterAvatar dca, CharacterVisuals visuals, List<ArchetypeInstance> itemInstances = null, UnityAction<UMAData> callback = null)
		{
			if (!dca)
			{
				throw new ArgumentNullException("dca");
			}
			if (visuals == null)
			{
				throw new ArgumentNullException("visuals");
			}
			Vector3 position = dca.gameObject.transform.position;
			string name = dca.activeRace.name;
			string text = (visuals.BuildType != CharacterBuildType.None) ? visuals.Sex.GetUmaRace(visuals.BuildType) : visuals.Sex.GetUmaRace();
			if (name != text)
			{
				dca.ChangeRace(text, DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, false);
			}
			dca.ValidateRace();
			dca.ClearSlots();
			if (visuals.Dna != null && visuals.Dna.Count > 0)
			{
				UMAPredefinedDNA umapredefinedDNA = new UMAPredefinedDNA();
				foreach (KeyValuePair<string, float> keyValuePair in visuals.Dna)
				{
					umapredefinedDNA.AddDNA(keyValuePair.Key, keyValuePair.Value);
				}
				dca.predefinedDNA = umapredefinedDNA;
			}
			foreach (KeyValuePair<CharacterColorType, string> keyValuePair2 in visuals.SharedColors)
			{
				keyValuePair2.Key.SetDcaSharedColor(dca, keyValuePair2.Value);
			}
			for (int i = 0; i < visuals.CustomizedSlots.Count; i++)
			{
				FeatureRecipe featureRecipe;
				if (InternalGameDatabase.Archetypes.TryGetAsType<FeatureRecipe>(visuals.CustomizedSlots[i], out featureRecipe) && featureRecipe.WardrobeRecipe != null)
				{
					dca.SetSlot(featureRecipe.WardrobeRecipe);
				}
			}
			if (itemInstances != null)
			{
				ArchetypeInstance archetypeInstance = null;
				EquipableItem equipableItem = null;
				for (int j = 0; j < itemInstances.Count; j++)
				{
					EquipableItem equipableItem2;
					if (itemInstances[j].Index == 65536 && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(itemInstances[j].ArchetypeId, out equipableItem2))
					{
						archetypeInstance = itemInstances[j];
						equipableItem = equipableItem2;
						break;
					}
				}
				for (int k = 0; k < itemInstances.Count; k++)
				{
					EquipableItem equipableItem3;
					if (itemInstances[k].Index != 65536 && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(itemInstances[k].ArchetypeId, out equipableItem3) && (!equipableItem || equipableItem.Type != equipableItem3.Type))
					{
						equipableItem3.OnEquipVisuals(visuals.Sex, dca, itemInstances[k].Index, itemInstances[k].ItemData.VisualIndex, itemInstances[k].ItemData.ColorIndex, false);
					}
				}
				if (equipableItem && archetypeInstance != null && archetypeInstance.ItemData != null)
				{
					foreach (EquipmentSlot index in equipableItem.Type.GetCachedCompatibleSlots())
					{
						equipableItem.OnEquipVisuals(visuals.Sex, dca, (int)index, archetypeInstance.ItemData.VisualIndex, archetypeInstance.ItemData.ColorIndex, false);
					}
				}
			}
			dca.LoadDefaultWardrobe();
			if (callback != null)
			{
				dca.CharacterCreated.AddListener(callback);
			}
			dca.gameObject.transform.position = position;
			dca.BuildCharacterEnabled = true;
			dca.hide = false;
			if (entity && entity.Type == GameEntityType.Player)
			{
				IAnimationController animancerController = entity.AnimancerController;
				if (animancerController == null)
				{
					return;
				}
				animancerController.AssignSex(visuals.Sex);
			}
		}

		// Token: 0x06002769 RID: 10089 RVA: 0x00137A30 File Offset: 0x00135C30
		public static void BuildNpcFromProfile(GameEntity entity, DynamicCharacterAvatar dca, NpcProfile profile, System.Random seed)
		{
			UMAManager.BuildNpc(entity, dca, seed, null, null, profile);
		}

		// Token: 0x0600276A RID: 10090 RVA: 0x00137A5C File Offset: 0x00135C5C
		public static void BuildNpcFromPopulationVisualsProfile(GameEntity entity, DynamicCharacterAvatar dca, NpcPopulationVisualsProfile profile, UniqueId ensembleId, System.Random seed)
		{
			UMAManager.BuildNpc(entity, dca, seed, new UniqueId?(ensembleId), null, profile);
		}

		// Token: 0x0600276B RID: 10091 RVA: 0x0005B9F1 File Offset: 0x00059BF1
		public static void BuildRandomNpc(GameEntity entity, DynamicCharacterAvatar dca, System.Random seed, UniqueId ensembleId, CharacterSex? sexOverride = null)
		{
			UMAManager.BuildNpc(entity, dca, seed, new UniqueId?(ensembleId), sexOverride, null);
		}

		// Token: 0x0600276C RID: 10092 RVA: 0x00137A84 File Offset: 0x00135C84
		private static void BuildNpc(GameEntity entity, DynamicCharacterAvatar dca, System.Random seed, UniqueId? ensembleIdOverride = null, CharacterSex? sexOverride = null, INpcVisualProfile visualProfile = null)
		{
			if (dca == null)
			{
				throw new ArgumentException("dca");
			}
			CharacterSex sex;
			CharacterSex characterSex;
			if (sexOverride != null && sexOverride.Value != CharacterSex.None)
			{
				sex = sexOverride.Value;
			}
			else if (visualProfile != null && visualProfile.TryGetSex(out characterSex))
			{
				sex = characterSex;
			}
			else
			{
				sex = ((seed.NextDouble() < 0.5) ? CharacterSex.Female : CharacterSex.Male);
			}
			CharacterBuildType buildType = (visualProfile != null) ? visualProfile.GetBuildType(seed) : CharacterBuildTypeExtensions.GetRandomBuildType(seed);
			string name = dca.activeRace.name;
			string umaRace = sex.GetUmaRace(buildType);
			if (name != umaRace)
			{
				dca.ChangeRace(umaRace, DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, false);
			}
			dca.ValidateRace();
			dca.ClearSlots();
			if (visualProfile != null)
			{
				visualProfile.SetDna(dca, seed, entity);
			}
			else
			{
				UMAManager.RandomizeDnaNew(dca, seed, entity);
			}
			if (dca.predefinedDNA != null && dca.predefinedDNA.PreloadValues != null && entity && entity.AnimatorReplicator != null)
			{
				float sizeSmall = 0f;
				float sizeLarge = 0f;
				for (int i = 0; i < dca.predefinedDNA.PreloadValues.Count; i++)
				{
					if (dca.predefinedDNA.PreloadValues[i].Name == "sizeSmall")
					{
						sizeSmall = dca.predefinedDNA.PreloadValues[i].Value;
					}
					else if (dca.predefinedDNA.PreloadValues[i].Name == "sizeLarge")
					{
						sizeLarge = dca.predefinedDNA.PreloadValues[i].Value;
					}
				}
				entity.AnimatorReplicator.SetHumanoidSpeedBasedOnSizeValue(sizeSmall, sizeLarge);
			}
			if (visualProfile != null)
			{
				visualProfile.SetColors(dca, seed);
			}
			else
			{
				UMAManager.RandomizeColorsNew(dca, seed);
			}
			if (visualProfile != null)
			{
				visualProfile.SetCustomizations(dca, seed, sex);
			}
			else
			{
				UMAManager.RandomizeCustomizationNew(dca, seed, sex);
			}
			if (entity && entity.CharacterData != null)
			{
				entity.CharacterData.Sex = sex;
				MaleFemaleSpriteCollection maleFemaleSpriteCollection;
				if (InternalGameDatabase.Archetypes.TryGetAsType<MaleFemaleSpriteCollection>(entity.CharacterData.PortraitId.Value, out maleFemaleSpriteCollection))
				{
					entity.CharacterData.PortraitIndex = seed.Next(0, maleFemaleSpriteCollection.GetCount(sex));
				}
			}
			dca.LoadDefaultWardrobe();
			UniqueId id = UniqueId.Empty;
			if (ensembleIdOverride != null && !ensembleIdOverride.Value.IsEmpty)
			{
				id = ensembleIdOverride.Value;
			}
			else if (visualProfile != null && visualProfile.Ensemble != null)
			{
				id = visualProfile.Ensemble.Id;
			}
			WardrobeRecipePairEnsemble wardrobeRecipePairEnsemble;
			if (!id.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<WardrobeRecipePairEnsemble>(id, out wardrobeRecipePairEnsemble))
			{
				wardrobeRecipePairEnsemble.AddEnsembleToDca(dca, sex, seed);
			}
			else if (visualProfile != null)
			{
				visualProfile.LoadEquipmentData(dca);
			}
			dca.BuildCharacterEnabled = true;
			dca.hide = false;
			if (entity && entity.Type == GameEntityType.Npc)
			{
				IAnimationController animancerController = entity.AnimancerController;
				if (animancerController == null)
				{
					return;
				}
				animancerController.AssignSex(sex);
			}
		}

		// Token: 0x0600276D RID: 10093 RVA: 0x00137D78 File Offset: 0x00135F78
		public static void RandomizeCustomizationNew(DynamicCharacterAvatar dca, System.Random seed, CharacterSex sex)
		{
			FeatureRecipeList[] recipeListForSex = GlobalSettings.Values.Uma.GetRecipeListForSex(sex);
			if (recipeListForSex == null)
			{
				return;
			}
			for (int i = 0; i < recipeListForSex.Length; i++)
			{
				if (!(recipeListForSex[i] == null) && recipeListForSex[i].FeatureRecipes != null)
				{
					int num = seed.Next(recipeListForSex[i].FeatureRecipes.Length);
					FeatureRecipe featureRecipe = recipeListForSex[i].FeatureRecipes[num];
					if (!(featureRecipe == null) && !(featureRecipe.WardrobeRecipe == null))
					{
						dca.SetSlot(featureRecipe.WardrobeRecipe);
					}
				}
			}
		}

		// Token: 0x0600276E RID: 10094 RVA: 0x00137E00 File Offset: 0x00136000
		public static void RandomizeDnaNew(DynamicCharacterAvatar dca, System.Random seed, GameEntity entity)
		{
			float size = (float)seed.NextDouble();
			float tone = (float)seed.NextDouble();
			UMAPredefinedDNA predefinedDna = UMAManager.GetPredefinedDna(entity, size, tone);
			dca.predefinedDNA = predefinedDna;
		}

		// Token: 0x0600276F RID: 10095 RVA: 0x00137E30 File Offset: 0x00136030
		public static UMAPredefinedDNA GetPredefinedDna(GameEntity entity, float size, float tone)
		{
			UMAPredefinedDNA umapredefinedDNA = new UMAPredefinedDNA();
			float value;
			float value2;
			UMAManager.GetLeftRightDna(size, out value, out value2);
			umapredefinedDNA.AddDNA("sizeSmall", value);
			umapredefinedDNA.AddDNA("sizeLarge", value2);
			if (entity)
			{
				entity.NameplateHeightOffset = new Vector3?(UMAManager.GetNameplateHeightOffset(size));
			}
			float value3;
			float value4;
			UMAManager.GetLeftRightDna(tone, out value3, out value4);
			umapredefinedDNA.AddDNA("toneFlabby", value3);
			umapredefinedDNA.AddDNA("toneMuscular", value4);
			return umapredefinedDNA;
		}

		// Token: 0x06002770 RID: 10096 RVA: 0x00137EA0 File Offset: 0x001360A0
		public static void GetLeftRightDna(float value, out float leftValue, out float rightValue)
		{
			leftValue = ((value >= 0.5f) ? 0f : Mathf.Lerp(1f, 0f, value * 2f));
			rightValue = ((value <= 0.5f) ? 0f : Mathf.Lerp(0f, 1f, (value - 0.5f) * 2f));
		}

		// Token: 0x06002771 RID: 10097 RVA: 0x0005BA04 File Offset: 0x00059C04
		public static float GetValueForLeftRight(float left, float right)
		{
			if (left > 0f)
			{
				return Mathf.Lerp(0.5f, 0f, left);
			}
			if (right > 0f)
			{
				return Mathf.Lerp(0.5f, 1f, right);
			}
			return 0.5f;
		}

		// Token: 0x06002772 RID: 10098 RVA: 0x0005BA3D File Offset: 0x00059C3D
		public static void RandomizeColorsNew(DynamicCharacterAvatar dca, System.Random seed)
		{
			UMAManager.SetRandomColorNew(dca, seed, CharacterColorType.Skin);
			UMAManager.SetRandomColorNew(dca, seed, CharacterColorType.Hair);
			UMAManager.SetRandomColorNew(dca, seed, CharacterColorType.Eyes);
			UMAManager.SetRandomColorNew(dca, seed, CharacterColorType.Favorite);
		}

		// Token: 0x06002773 RID: 10099 RVA: 0x00137F04 File Offset: 0x00136104
		public static void SetRandomColorNew(DynamicCharacterAvatar dca, System.Random seed, CharacterColorType type)
		{
			if (InternalGameDatabase.GlobalSettings == null || !dca || seed == null)
			{
				return;
			}
			ColorSampler colorSampler;
			switch (type)
			{
			case CharacterColorType.Skin:
				colorSampler = GlobalSettings.Values.Uma.SkinColorSampler;
				break;
			case CharacterColorType.Hair:
				colorSampler = GlobalSettings.Values.Uma.HairColorSampler;
				break;
			case CharacterColorType.Eyes:
				colorSampler = GlobalSettings.Values.Uma.EyeColorSampler;
				break;
			case CharacterColorType.Favorite:
				colorSampler = GlobalSettings.Values.Uma.FavoriteColorSampler;
				break;
			default:
				return;
			}
			if (colorSampler == null)
			{
				return;
			}
			Color randomColor = colorSampler.GetRandomColor(seed);
			type.SetDcaSharedColor(dca, randomColor);
		}

		// Token: 0x06002774 RID: 10100 RVA: 0x0005BA5F File Offset: 0x00059C5F
		private static void RandomizeColors(DynamicCharacterAvatar dca, System.Random seed)
		{
			UMAManager.SetRandomColor(dca, seed, CharacterColorType.Skin);
			UMAManager.SetRandomColor(dca, seed, CharacterColorType.Hair);
			UMAManager.SetRandomColor(dca, seed, CharacterColorType.Eyes);
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x00137FA8 File Offset: 0x001361A8
		private static void SetRandomColor(DynamicCharacterAvatar dca, System.Random seed, CharacterColorType type)
		{
			if (InternalGameDatabase.GlobalSettings == null || !dca || seed == null)
			{
				return;
			}
			ColorCollection colorCollection;
			switch (type)
			{
			case CharacterColorType.Skin:
				colorCollection = GlobalSettings.Values.Uma.SkinColors;
				break;
			case CharacterColorType.Hair:
				colorCollection = GlobalSettings.Values.Uma.HairColors;
				break;
			case CharacterColorType.Eyes:
				colorCollection = GlobalSettings.Values.Uma.EyeColors;
				break;
			default:
				return;
			}
			if (colorCollection == null)
			{
				return;
			}
			int num = seed.Next(0, colorCollection.Colors.Length);
			Color color = colorCollection.Colors[num];
			dca.SetSharedColorForDca(type, color);
		}

		// Token: 0x06002776 RID: 10102 RVA: 0x00138048 File Offset: 0x00136248
		private static void RandomizeDna(DynamicCharacterAvatar dca, System.Random seed, SpawnTier tier)
		{
			float a = 0.4f;
			float b = 0.6f;
			float a2 = 0.4f;
			float b2 = 0.6f;
			if (tier <= SpawnTier.Normal)
			{
				if (tier != SpawnTier.Weak)
				{
					if (tier == SpawnTier.Normal)
					{
						a = 0.4f;
						b = 0.6f;
						a2 = 0.4f;
						b2 = 0.5f;
					}
				}
				else
				{
					a = 0.3f;
					b = 0.7f;
					a2 = 0.3f;
					b2 = 0.4f;
				}
			}
			else if (tier != SpawnTier.Strong)
			{
				if (tier != SpawnTier.Champion)
				{
					if (tier == SpawnTier.Elite)
					{
						a = 0.5f;
						b = 0.5f;
						a2 = 0.7f;
						b2 = 0.8f;
					}
				}
				else
				{
					a = 0.45f;
					b = 0.55f;
					a2 = 0.6f;
					b2 = 0.7f;
				}
			}
			else
			{
				a = 0.42f;
				b = 0.58f;
				a2 = 0.5f;
				b2 = 0.6f;
			}
			UMAPredefinedDNA umapredefinedDNA = new UMAPredefinedDNA();
			foreach (KeyValuePair<string, UMADnaType> keyValuePair in UMADnaTypeExtensions.DnaTypeDict)
			{
				float num = (float)seed.NextDouble();
				UMADnaType value = keyValuePair.Value;
				if (value != UMADnaType.weight)
				{
					if (value != UMADnaType.muscle)
					{
						DNASliderRestriction dnasliderRestriction;
						float value2 = UMAManager.DnaRestrictions.TryGetRestriction(keyValuePair.Value, out dnasliderRestriction) ? dnasliderRestriction.GetNormalizedValue(num) : num;
						umapredefinedDNA.AddDNA(keyValuePair.Key, value2);
					}
					else
					{
						for (int i = 0; i < DnaSlider.kUmaMuscleTypes.Length; i++)
						{
							num = (float)seed.NextDouble();
							float value2 = Mathf.Lerp(a2, b2, num);
							umapredefinedDNA.AddDNA(DnaSlider.kUmaWeightTypes[i], value2);
						}
					}
				}
				else
				{
					for (int j = 0; j < DnaSlider.kUmaWeightTypes.Length; j++)
					{
						num = (float)seed.NextDouble();
						float value2 = Mathf.Lerp(a, b, num);
						umapredefinedDNA.AddDNA(DnaSlider.kUmaWeightTypes[j], value2);
					}
				}
			}
			dca.predefinedDNA = umapredefinedDNA;
		}

		// Token: 0x06002777 RID: 10103 RVA: 0x0013823C File Offset: 0x0013643C
		private static void RandomizeCustomization(DynamicCharacterAvatar dca, System.Random seed, CharacterSex sex)
		{
			foreach (KeyValuePair<CharacterCustomizationType, List<CharacterCustomization>> keyValuePair in sex.GetCustomizationDict())
			{
				int index = seed.Next(0, keyValuePair.Value.Count);
				CharacterCustomization characterCustomization = keyValuePair.Value[index];
				if (characterCustomization != null && characterCustomization.Recipe != null)
				{
					dca.SetSlot(characterCustomization.Recipe);
				}
			}
		}

		// Token: 0x06002778 RID: 10104 RVA: 0x001382D0 File Offset: 0x001364D0
		public static Vector3 GetNameplateHeightOffset(Dictionary<string, float> umaDna)
		{
			if (umaDna != null)
			{
				float left = 0f;
				float right = 0f;
				umaDna.TryGetValue("sizeSmall", out left);
				umaDna.TryGetValue("sizeLarge", out right);
				return UMAManager.GetNameplateHeightOffset(UMAManager.GetValueForLeftRight(left, right));
			}
			return WorldSpaceOverheadController.kDefaultHeightOffset;
		}

		// Token: 0x06002779 RID: 10105 RVA: 0x0013831C File Offset: 0x0013651C
		public static Vector3 GetNameplateHeightOffset(float sizeValue)
		{
			float d = Mathf.Lerp(GlobalSettings.Values.Uma.NameplateHeight.Min, GlobalSettings.Values.Uma.NameplateHeight.Max, sizeValue);
			return Vector3.up * d;
		}

		// Token: 0x0400292B RID: 10539
		private const bool kLoadCustomizations = true;

		// Token: 0x0400292C RID: 10540
		private static UMADNARestrictions m_restrictions;

		// Token: 0x0400292D RID: 10541
		private static CharacterCustomizationCollection m_customizations;

		// Token: 0x0400292E RID: 10542
		private static Dictionary<CharacterCustomizationType, List<CharacterCustomization>> m_maleCustomizations;

		// Token: 0x0400292F RID: 10543
		private static Dictionary<CharacterCustomizationType, List<CharacterCustomization>> m_femaleCustomizations;

		// Token: 0x04002930 RID: 10544
		private static bool m_initialized;
	}
}
