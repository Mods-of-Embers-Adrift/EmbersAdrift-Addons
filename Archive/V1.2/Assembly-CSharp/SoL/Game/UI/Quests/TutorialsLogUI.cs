using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Notifications;
using SoL.Game.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000958 RID: 2392
	public class TutorialsLogUI : MonoBehaviour
	{
		// Token: 0x17000FC3 RID: 4035
		// (get) Token: 0x060046E4 RID: 18148 RVA: 0x0006FBF9 File Offset: 0x0006DDF9
		public BaseNotification SelectedTutorial
		{
			get
			{
				return this.m_tutorialCategoriesList.SelectedItem;
			}
		}

		// Token: 0x060046E5 RID: 18149 RVA: 0x001A554C File Offset: 0x001A374C
		protected void Start()
		{
			if (this.m_initialDetailContentHeight == null)
			{
				this.m_initialDetailContentHeight = new float?(this.m_detailContentRect.rect.height);
			}
			this.m_tutorialCategoriesList.PlayerPrefsKey = this.PlayerPrefsKey + "_Category";
			this.m_tutorialCategoriesList.SelectionChanged += this.OnSelectionChanged;
			SolInput.Mapper.MappingsChanged += this.OnMappingChanged;
		}

		// Token: 0x060046E6 RID: 18150 RVA: 0x001A55CC File Offset: 0x001A37CC
		protected void OnDestroy()
		{
			this.m_tutorialCategoriesList.SelectionChanged -= this.OnSelectionChanged;
			SolInput.Mapper.MappingsChanged -= this.OnMappingChanged;
			this.m_tutorialCategoriesList.FullyInitialized -= this.UpdateList;
		}

		// Token: 0x060046E7 RID: 18151 RVA: 0x0006FC06 File Offset: 0x0006DE06
		public void Show()
		{
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x060046E8 RID: 18152 RVA: 0x0006FC14 File Offset: 0x0006DE14
		public void SelectTutorial(BaseNotification tutorial)
		{
			if (this.m_tutorialCategoriesList.IsInitialized)
			{
				this.m_tutorialCategoriesList.Select(tutorial);
				return;
			}
			this.m_selectOnInit = tutorial;
		}

		// Token: 0x060046E9 RID: 18153 RVA: 0x0006FC37 File Offset: 0x0006DE37
		private void OnSelectionChanged(Category<BaseNotification> category, BaseNotification notification)
		{
			this.RefreshVisuals();
		}

		// Token: 0x060046EA RID: 18154 RVA: 0x0006FC37 File Offset: 0x0006DE37
		private void OnMappingChanged()
		{
			this.RefreshVisuals();
		}

		// Token: 0x060046EB RID: 18155 RVA: 0x001A5620 File Offset: 0x001A3820
		public void RefreshVisuals()
		{
			if (this.SelectedTutorial == null)
			{
				this.m_noSelection.gameObject.SetActive(true);
				this.m_detailPane.SetActive(false);
				return;
			}
			this.m_noSelection.gameObject.SetActive(false);
			this.m_detailPane.SetActive(true);
			this.m_title.ZStringSetText(this.SelectedTutorial.Title);
			this.m_description.SetTextWithReplacements(this.SelectedTutorial.Description);
			this.m_description.ForceMeshUpdate(false, false);
			this.m_detailContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Math.Max(this.m_initialDetailContentHeight.Value, this.m_description.preferredHeight + 40f));
		}

		// Token: 0x060046EC RID: 18156 RVA: 0x0006FC3F File Offset: 0x0006DE3F
		private void UpdateListWhenReady()
		{
			if (this.m_tutorialCategoriesList.IsFullyInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_tutorialCategoriesList.FullyInitialized += this.UpdateList;
		}

		// Token: 0x060046ED RID: 18157 RVA: 0x001A56DC File Offset: 0x001A38DC
		private void UpdateList()
		{
			List<Category<BaseNotification>> fromPool = StaticListPool<Category<BaseNotification>>.GetFromPool();
			foreach (BaseNotification baseNotification in InternalGameDatabase.Notifications.GetAllItems())
			{
				if (baseNotification.Presentation.HasBitFlag(NotificationPresentationFlags.TutorialPopup))
				{
					Category<BaseNotification> category = default(Category<BaseNotification>);
					string text = baseNotification.Category.ToString();
					foreach (Category<BaseNotification> category2 in fromPool)
					{
						if (category2.Name == text)
						{
							category = category2;
						}
					}
					if (category.Name == null)
					{
						category = new Category<BaseNotification>
						{
							Name = text,
							Data = StaticListPool<BaseNotification>.GetFromPool()
						};
						fromPool.Add(category);
					}
					category.Data.Add(baseNotification);
				}
			}
			this.m_tutorialCategoriesList.UpdateCategories(fromPool);
			this.m_tutorialCategoriesList.ReindexItems(this.SelectedTutorial);
			foreach (Category<BaseNotification> category3 in fromPool)
			{
				StaticListPool<BaseNotification>.ReturnToPool(category3.Data);
			}
			StaticListPool<Category<BaseNotification>>.ReturnToPool(fromPool);
			if (this.m_selectOnInit != null)
			{
				this.m_tutorialCategoriesList.Select(this.m_selectOnInit);
				this.m_selectOnInit = null;
			}
		}

		// Token: 0x040042CA RID: 17098
		[SerializeField]
		private TutorialCategoriesList m_tutorialCategoriesList;

		// Token: 0x040042CB RID: 17099
		[SerializeField]
		private TextMeshProUGUI m_noSelection;

		// Token: 0x040042CC RID: 17100
		[SerializeField]
		private GameObject m_detailPane;

		// Token: 0x040042CD RID: 17101
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x040042CE RID: 17102
		[SerializeField]
		private TextMeshProUGUI m_description;

		// Token: 0x040042CF RID: 17103
		[SerializeField]
		private RectTransform m_detailContentRect;

		// Token: 0x040042D0 RID: 17104
		private float? m_initialDetailContentHeight;

		// Token: 0x040042D1 RID: 17105
		private BaseNotification m_selectOnInit;

		// Token: 0x040042D2 RID: 17106
		public string PlayerPrefsKey = string.Empty;
	}
}
