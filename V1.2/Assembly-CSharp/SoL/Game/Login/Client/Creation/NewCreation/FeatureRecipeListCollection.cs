using System;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B6A RID: 2922
	[CreateAssetMenu(menuName = "SoL/Creation/Feature Recipe List Collection")]
	public class FeatureRecipeListCollection : ScriptableObject
	{
		// Token: 0x060059CA RID: 22986 RVA: 0x0007C300 File Offset: 0x0007A500
		public FeatureRecipeList[] GetFeatureList(CharacterSex sex)
		{
			if (sex != CharacterSex.Male)
			{
				return this.m_femaleFeatures;
			}
			return this.m_maleFeatures;
		}

		// Token: 0x04004EF6 RID: 20214
		[SerializeField]
		private FeatureRecipeList[] m_maleFeatures;

		// Token: 0x04004EF7 RID: 20215
		[SerializeField]
		private FeatureRecipeList[] m_femaleFeatures;
	}
}
