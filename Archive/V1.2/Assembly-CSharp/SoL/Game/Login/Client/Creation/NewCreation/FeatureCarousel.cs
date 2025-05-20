using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B67 RID: 2919
	public class FeatureCarousel : MonoBehaviour
	{
		// Token: 0x170014E4 RID: 5348
		// (get) Token: 0x060059B4 RID: 22964 RVA: 0x0007C15A File Offset: 0x0007A35A
		public CharacterSex Sex
		{
			get
			{
				return this.m_recipeList.Sex;
			}
		}

		// Token: 0x170014E5 RID: 5349
		// (get) Token: 0x060059B5 RID: 22965 RVA: 0x0007C167 File Offset: 0x0007A367
		public string SlotName
		{
			get
			{
				return this.m_recipeList.SlotName;
			}
		}

		// Token: 0x060059B6 RID: 22966 RVA: 0x001EAAEC File Offset: 0x001E8CEC
		private void Awake()
		{
			this.m_leftButton.onClick.AddListener(new UnityAction(this.LeftButtonClicked));
			this.m_rightButton.onClick.AddListener(new UnityAction(this.RightButtonClicked));
			if (this.m_randomizeStartingFeature)
			{
				this.RandomizeFeature();
				return;
			}
			this.RefreshLabel();
		}

		// Token: 0x060059B7 RID: 22967 RVA: 0x0007C174 File Offset: 0x0007A374
		private void OnDestroy()
		{
			this.m_leftButton.onClick.RemoveListener(new UnityAction(this.LeftButtonClicked));
			this.m_rightButton.onClick.RemoveListener(new UnityAction(this.RightButtonClicked));
		}

		// Token: 0x060059B8 RID: 22968 RVA: 0x0007C1AE File Offset: 0x0007A3AE
		public void Init(NewCharacterManager manager)
		{
			this.m_manager = manager;
		}

		// Token: 0x060059B9 RID: 22969 RVA: 0x0007C1B7 File Offset: 0x0007A3B7
		public void RandomizeFeature()
		{
			this.m_index = UnityEngine.Random.Range(0, this.m_recipeList.FeatureRecipes.Length);
			this.RefreshLabel();
		}

		// Token: 0x060059BA RID: 22970 RVA: 0x0007C1D8 File Offset: 0x0007A3D8
		public void ResetFeature()
		{
			this.m_index = 0;
			this.RefreshLabel();
		}

		// Token: 0x060059BB RID: 22971 RVA: 0x0007C1E7 File Offset: 0x0007A3E7
		private void RightButtonClicked()
		{
			this.m_index++;
			if (this.m_index >= this.m_recipeList.FeatureRecipes.Length)
			{
				this.m_index = 0;
			}
			this.RefreshLabel();
			this.m_manager.FeatureCarouselChanged(this);
		}

		// Token: 0x060059BC RID: 22972 RVA: 0x0007C225 File Offset: 0x0007A425
		private void LeftButtonClicked()
		{
			this.m_index--;
			if (this.m_index < 0)
			{
				this.m_index = this.m_recipeList.FeatureRecipes.Length - 1;
			}
			this.RefreshLabel();
			this.m_manager.FeatureCarouselChanged(this);
		}

		// Token: 0x060059BD RID: 22973 RVA: 0x0007C265 File Offset: 0x0007A465
		public FeatureRecipe GetCurrentFeatureRecipe()
		{
			if (this.m_recipeList == null || this.m_index >= this.m_recipeList.FeatureRecipes.Length)
			{
				return null;
			}
			return this.m_recipeList.FeatureRecipes[this.m_index];
		}

		// Token: 0x060059BE RID: 22974 RVA: 0x001EAB48 File Offset: 0x001E8D48
		private void RefreshLabel()
		{
			FeatureRecipe currentFeatureRecipe = this.GetCurrentFeatureRecipe();
			string text = (currentFeatureRecipe == null) ? this.m_emptyLabel : currentFeatureRecipe.DisplayName;
			this.m_label.SetText(text);
		}

		// Token: 0x060059BF RID: 22975 RVA: 0x0007C29E File Offset: 0x0007A49E
		private IEnumerable GetRecipeLists()
		{
			return SolOdinUtilities.GetDropdownItems<FeatureRecipeList>();
		}

		// Token: 0x060059C0 RID: 22976 RVA: 0x001EAB80 File Offset: 0x001E8D80
		public void SelectRecipeIfPresent(List<UniqueId> features)
		{
			if (features != null && features.Count > 0)
			{
				for (int i = 0; i < this.m_recipeList.FeatureRecipes.Length; i++)
				{
					if (!(this.m_recipeList.FeatureRecipes[i] == null) && features.Contains(this.m_recipeList.FeatureRecipes[i].Id))
					{
						this.m_index = i;
						this.RefreshLabel();
						return;
					}
				}
			}
			this.m_index = 0;
			this.RefreshLabel();
		}

		// Token: 0x04004EEA RID: 20202
		[SerializeField]
		private bool m_randomizeStartingFeature;

		// Token: 0x04004EEB RID: 20203
		[SerializeField]
		private FeatureRecipeList m_recipeList;

		// Token: 0x04004EEC RID: 20204
		[SerializeField]
		private SolButton m_leftButton;

		// Token: 0x04004EED RID: 20205
		[SerializeField]
		private SolButton m_rightButton;

		// Token: 0x04004EEE RID: 20206
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004EEF RID: 20207
		[SerializeField]
		private string m_emptyLabel = "None";

		// Token: 0x04004EF0 RID: 20208
		private int m_index;

		// Token: 0x04004EF1 RID: 20209
		private NewCharacterManager m_manager;
	}
}
