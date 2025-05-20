using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Objects;
using SoL.Game.UMA;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200074D RID: 1869
	[Serializable]
	public class UmaSettings
	{
		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x060037B7 RID: 14263 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x060037B8 RID: 14264 RVA: 0x0004FA02 File Offset: 0x0004DC02
		private IEnumerable GetColorSampler()
		{
			return SolOdinUtilities.GetDropdownItems<ColorSampler>();
		}

		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x060037B9 RID: 14265 RVA: 0x00066170 File Offset: 0x00064370
		public ColorSampler SkinColorSampler
		{
			get
			{
				return this.m_skinColorSampler;
			}
		}

		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x060037BA RID: 14266 RVA: 0x00066178 File Offset: 0x00064378
		public float SkinColorCorrection
		{
			get
			{
				return this.m_skinColorCorrection;
			}
		}

		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x060037BB RID: 14267 RVA: 0x00066180 File Offset: 0x00064380
		public ColorSampler EyeColorSampler
		{
			get
			{
				return this.m_eyeColorSampler;
			}
		}

		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x060037BC RID: 14268 RVA: 0x00066188 File Offset: 0x00064388
		public float EyeColorCorrection
		{
			get
			{
				return this.m_eyeColorCorrection;
			}
		}

		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x060037BD RID: 14269 RVA: 0x00066190 File Offset: 0x00064390
		public ColorSampler HairColorSampler
		{
			get
			{
				return this.m_hairColorSampler;
			}
		}

		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x060037BE RID: 14270 RVA: 0x00066198 File Offset: 0x00064398
		public float HairColorCorrection
		{
			get
			{
				return this.m_hairColorCorrection;
			}
		}

		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x060037BF RID: 14271 RVA: 0x000661A0 File Offset: 0x000643A0
		public ColorSampler FavoriteColorSampler
		{
			get
			{
				return this.m_favoriteColorSampler;
			}
		}

		// Token: 0x17000CAC RID: 3244
		// (get) Token: 0x060037C0 RID: 14272 RVA: 0x000661A8 File Offset: 0x000643A8
		public float FavoriteColorCorrection
		{
			get
			{
				return this.m_favoriteColorCorrection;
			}
		}

		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x060037C1 RID: 14273 RVA: 0x000661B0 File Offset: 0x000643B0
		public MinMaxFloatRange NameplateHeight
		{
			get
			{
				return this.m_nameplateHeight;
			}
		}

		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x060037C2 RID: 14274 RVA: 0x000661B8 File Offset: 0x000643B8
		public bool CorrectMountPoints
		{
			get
			{
				return this.m_correctMountPoints;
			}
		}

		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x060037C3 RID: 14275 RVA: 0x000661C0 File Offset: 0x000643C0
		public float LookBodyWeight
		{
			get
			{
				return this.m_lookBodyWeight;
			}
		}

		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x060037C4 RID: 14276 RVA: 0x000661C8 File Offset: 0x000643C8
		public float LookHeadWeight
		{
			get
			{
				return this.m_lookHeadWeight;
			}
		}

		// Token: 0x17000CB1 RID: 3249
		// (get) Token: 0x060037C5 RID: 14277 RVA: 0x000661D0 File Offset: 0x000643D0
		public float LookEyeWeight
		{
			get
			{
				return this.m_lookEyeWeight;
			}
		}

		// Token: 0x17000CB2 RID: 3250
		// (get) Token: 0x060037C6 RID: 14278 RVA: 0x000661D8 File Offset: 0x000643D8
		public float MaxFootRotationAngle
		{
			get
			{
				return this.m_maxFootRotationAngle;
			}
		}

		// Token: 0x060037C7 RID: 14279 RVA: 0x000661E0 File Offset: 0x000643E0
		public FeatureRecipeList[] GetRecipeListForSex(CharacterSex sex)
		{
			if (!(this.m_featureRecipeListCollection == null))
			{
				return this.m_featureRecipeListCollection.GetFeatureList(sex);
			}
			return null;
		}

		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x060037C8 RID: 14280 RVA: 0x0016C0C8 File Offset: 0x0016A2C8
		public Dictionary<MaterialColorType, ColorSampler> MaterialColorTypeSamplerDict
		{
			get
			{
				if (this.m_materialColorTypeSamplerDict == null)
				{
					this.m_materialColorTypeSamplerDict = new Dictionary<MaterialColorType, ColorSampler>(default(MaterialColorTypeComparer));
					for (int i = 0; i < this.m_materialColorTypeSamplers.Length; i++)
					{
						if (this.m_materialColorTypeSamplers[i].MaterialColorType != MaterialColorType.None && !this.m_materialColorTypeSamplerDict.ContainsKey(this.m_materialColorTypeSamplers[i].MaterialColorType))
						{
							this.m_materialColorTypeSamplerDict.Add(this.m_materialColorTypeSamplers[i].MaterialColorType, this.m_materialColorTypeSamplers[i].Sampler);
						}
					}
				}
				return this.m_materialColorTypeSamplerDict;
			}
		}

		// Token: 0x060037C9 RID: 14281 RVA: 0x0016C160 File Offset: 0x0016A360
		public WardrobeRecipePair GetTurnipHead(System.Random seed)
		{
			if (seed.NextDouble() < (double)this.m_turnipChance)
			{
				int num = seed.Next(this.m_turnipHeads.Length);
				return this.m_turnipHeads[num];
			}
			return null;
		}

		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x060037CA RID: 14282 RVA: 0x000661FE File Offset: 0x000643FE
		public float DamageTargetToSourceOffset
		{
			get
			{
				return this.m_damageTargetToSourceOffset;
			}
		}

		// Token: 0x04003699 RID: 13977
		private const string kLookIk = "LookIK";

		// Token: 0x0400369A RID: 13978
		public bool UsePackedRecipeForDNA = true;

		// Token: 0x0400369B RID: 13979
		public ColorCollection SkinColors;

		// Token: 0x0400369C RID: 13980
		public ColorCollection HairColors;

		// Token: 0x0400369D RID: 13981
		public ColorCollection EyeColors;

		// Token: 0x0400369E RID: 13982
		[SerializeField]
		private ColorSampler m_skinColorSampler;

		// Token: 0x0400369F RID: 13983
		[Range(1f, 2f)]
		[SerializeField]
		private float m_skinColorCorrection = 1.1f;

		// Token: 0x040036A0 RID: 13984
		[SerializeField]
		private ColorSampler m_eyeColorSampler;

		// Token: 0x040036A1 RID: 13985
		[Range(1f, 2f)]
		[SerializeField]
		private float m_eyeColorCorrection = 1.1f;

		// Token: 0x040036A2 RID: 13986
		[SerializeField]
		private ColorSampler m_hairColorSampler;

		// Token: 0x040036A3 RID: 13987
		[Range(1f, 2f)]
		[SerializeField]
		private float m_hairColorCorrection = 1.1f;

		// Token: 0x040036A4 RID: 13988
		[SerializeField]
		private ColorSampler m_favoriteColorSampler;

		// Token: 0x040036A5 RID: 13989
		[Range(1f, 2f)]
		[SerializeField]
		private float m_favoriteColorCorrection = 1.1f;

		// Token: 0x040036A6 RID: 13990
		[SerializeField]
		private FeatureRecipeListCollection m_featureRecipeListCollection;

		// Token: 0x040036A7 RID: 13991
		[SerializeField]
		private MinMaxFloatRange m_nameplateHeight = new MinMaxFloatRange(1.8f, 2.2f);

		// Token: 0x040036A8 RID: 13992
		[SerializeField]
		private bool m_correctMountPoints;

		// Token: 0x040036A9 RID: 13993
		[SerializeField]
		private UmaSettings.MaterialColorTypeSampler[] m_materialColorTypeSamplers;

		// Token: 0x040036AA RID: 13994
		[Range(0f, 1f)]
		[SerializeField]
		private float m_lookBodyWeight = 0.05f;

		// Token: 0x040036AB RID: 13995
		[Range(0f, 1f)]
		[SerializeField]
		private float m_lookHeadWeight = 0.75f;

		// Token: 0x040036AC RID: 13996
		[Range(0f, 1f)]
		[SerializeField]
		private float m_lookEyeWeight = 0.5f;

		// Token: 0x040036AD RID: 13997
		[Range(0f, 90f)]
		[SerializeField]
		private float m_maxFootRotationAngle = 20f;

		// Token: 0x040036AE RID: 13998
		private Dictionary<MaterialColorType, ColorSampler> m_materialColorTypeSamplerDict;

		// Token: 0x040036AF RID: 13999
		[Range(0f, 1f)]
		[SerializeField]
		private float m_turnipChance = 0.5f;

		// Token: 0x040036B0 RID: 14000
		[SerializeField]
		private WardrobeRecipePair[] m_turnipHeads;

		// Token: 0x040036B1 RID: 14001
		[SerializeField]
		private float m_damageTargetToSourceOffset;

		// Token: 0x0200074E RID: 1870
		[Serializable]
		private class MaterialColorTypeSampler
		{
			// Token: 0x17000CB5 RID: 3253
			// (get) Token: 0x060037CC RID: 14284 RVA: 0x00066206 File Offset: 0x00064406
			public MaterialColorType MaterialColorType
			{
				get
				{
					return this.m_materialColorType;
				}
			}

			// Token: 0x17000CB6 RID: 3254
			// (get) Token: 0x060037CD RID: 14285 RVA: 0x0006620E File Offset: 0x0006440E
			public ColorSampler Sampler
			{
				get
				{
					return this.m_colorSampler;
				}
			}

			// Token: 0x060037CE RID: 14286 RVA: 0x0004FA02 File Offset: 0x0004DC02
			private IEnumerable GetColorSampler()
			{
				return SolOdinUtilities.GetDropdownItems<ColorSampler>();
			}

			// Token: 0x040036B2 RID: 14002
			[SerializeField]
			private MaterialColorType m_materialColorType;

			// Token: 0x040036B3 RID: 14003
			[SerializeField]
			private ColorSampler m_colorSampler;
		}
	}
}
