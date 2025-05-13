using System;
using SoL.Networking.Database;
using UMA;
using UMA.CharacterSystem;
using UMA.PoseTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000340 RID: 832
	public static class UMAExtensions
	{
		// Token: 0x060016CA RID: 5834 RVA: 0x00051F44 File Offset: 0x00050144
		public static void Refresh(this DynamicCharacterAvatar dca, bool dnaDirty = true, bool textureDirty = true, bool meshDirty = true)
		{
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			dca.BuildCharacter(true, false, true);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x00051F63 File Offset: 0x00050163
		public static void LoadFromRecipe(this DynamicCharacterAvatar dca, UMADynamicCharacterAvatarRecipe recipe, UnityAction<UMAData> callback = null)
		{
			UMAExtensions.LoadFromRecipeOrRecipeString(dca, recipe, null, callback);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00051F6E File Offset: 0x0005016E
		public static void LoadFromRecipeString(this DynamicCharacterAvatar dca, string recipe, UnityAction<UMAData> callback = null)
		{
			UMAExtensions.LoadFromRecipeOrRecipeString(dca, null, recipe, callback);
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x00101340 File Offset: 0x000FF540
		private static void LoadFromRecipeOrRecipeString(DynamicCharacterAvatar dca, UMADynamicCharacterAvatarRecipe recipe, string recipeString, UnityAction<UMAData> callback)
		{
			Vector3 position = dca.gameObject.transform.position;
			if (recipe != null)
			{
				dca.LoadFromRecipe(recipe, DynamicCharacterAvatar.LoadOptions.useDefaults);
			}
			else
			{
				if (string.IsNullOrEmpty(recipeString))
				{
					throw new ArgumentException("Both recipe and recipe string are null!");
				}
				dca.SetLoadString(recipeString);
			}
			dca.gameObject.transform.position = position;
			UMAExtensions.FinalizeDca(dca);
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x001013A4 File Offset: 0x000FF5A4
		public static void SetSharedColorForDca(this DynamicCharacterAvatar dca, CharacterColorType type, Color color)
		{
			string name = type.ToString();
			dca.characterColors.SetColor(name, color);
			if (type == CharacterColorType.Skin)
			{
				OverlayColorData color2 = dca.GetColor(name);
				if (color2 != null)
				{
					Color color3 = color2.channelAdditiveMask[color2.channelCount - 1];
					color3.a = 0.05882353f;
					color2.channelAdditiveMask[color2.channelCount - 1] = color3;
				}
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x00051F79 File Offset: 0x00050179
		private static void FinalizeDca(DynamicCharacterAvatar dca)
		{
			dca.BuildCharacterEnabled = true;
			dca.hide = false;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x00101418 File Offset: 0x000FF618
		public static UMAExpressionPlayer AddExpressionPlayer(this DynamicCharacterAvatar dca)
		{
			dca.SetExpressionSet(true);
			UMAExpressionPlayer component = dca.GetComponent<UMAExpressionPlayer>();
			if (component != null)
			{
				component.Initialize();
				component.enableBlinking = true;
				component.enableSaccades = true;
			}
			return component;
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x00101454 File Offset: 0x000FF654
		private static void SetExpression(DynamicCharacterAvatar dca)
		{
			dca.SetExpressionSet(true);
			UMAExpressionPlayer component = dca.gameObject.GetComponent<UMAExpressionPlayer>();
			if (component != null)
			{
				component.Initialize();
				component.enableBlinking = true;
				component.enableSaccades = true;
			}
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x00101494 File Offset: 0x000FF694
		public static void SetShadowMode(this DynamicCharacterAvatar dca, ShadowCastingMode mode)
		{
			if (dca == null || dca.umaData == null || dca.umaData.rendererCount <= 0)
			{
				return;
			}
			SkinnedMeshRenderer renderer = dca.umaData.GetRenderer(0);
			if (renderer != null && renderer.shadowCastingMode != mode)
			{
				renderer.shadowCastingMode = mode;
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x00051F89 File Offset: 0x00050189
		public static void ValidateRace(this DynamicCharacterAvatar dca)
		{
			if (dca.activeRace.racedata == null)
			{
				dca.activeRace.SetRaceData();
			}
		}

		// Token: 0x04001EA5 RID: 7845
		private const float kSkinAlpha = 0.05882353f;
	}
}
