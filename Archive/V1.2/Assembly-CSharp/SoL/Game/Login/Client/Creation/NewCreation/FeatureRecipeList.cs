using System;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B69 RID: 2921
	[CreateAssetMenu(menuName = "SoL/Creation/Feature Recipe List")]
	public class FeatureRecipeList : ScriptableObject
	{
		// Token: 0x170014E8 RID: 5352
		// (get) Token: 0x060059C6 RID: 22982 RVA: 0x0007C2E8 File Offset: 0x0007A4E8
		public CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
		}

		// Token: 0x170014E9 RID: 5353
		// (get) Token: 0x060059C7 RID: 22983 RVA: 0x0007C2F0 File Offset: 0x0007A4F0
		public string SlotName
		{
			get
			{
				return this.m_slotName;
			}
		}

		// Token: 0x170014EA RID: 5354
		// (get) Token: 0x060059C8 RID: 22984 RVA: 0x0007C2F8 File Offset: 0x0007A4F8
		public FeatureRecipe[] FeatureRecipes
		{
			get
			{
				return this.m_features;
			}
		}

		// Token: 0x04004EF3 RID: 20211
		[SerializeField]
		private CharacterSex m_sex;

		// Token: 0x04004EF4 RID: 20212
		[SerializeField]
		private string m_slotName;

		// Token: 0x04004EF5 RID: 20213
		[SerializeField]
		private FeatureRecipe[] m_features;
	}
}
