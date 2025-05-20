using System;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B55 RID: 2901
	public class CharacterCustomizationSelector : Feature
	{
		// Token: 0x06005947 RID: 22855 RVA: 0x0007BC3D File Offset: 0x00079E3D
		public override void Initialize(CreationDirector director, CreationDirector.CharacterToCreate toCreate, CreationDirector.FeatureSetting settings)
		{
			base.Initialize(director, toCreate, settings);
			this.Reset();
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x0007BC4E File Offset: 0x00079E4E
		protected override void Subscribe()
		{
			base.Subscribe();
			this.m_left.onClick.AddListener(new UnityAction(this.OnLeftClicked));
			this.m_right.onClick.AddListener(new UnityAction(this.OnRightClicked));
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x0007BC8E File Offset: 0x00079E8E
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			this.m_left.onClick.RemoveAllListeners();
			this.m_right.onClick.RemoveAllListeners();
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x0007BCB6 File Offset: 0x00079EB6
		public override void OnLockStateChanged(ToggleController.ToggleState obj)
		{
			base.OnLockStateChanged(obj);
			this.m_left.interactable = !base.m_locked;
			this.m_right.interactable = !base.m_locked;
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x001E98A4 File Offset: 0x001E7AA4
		private void OnLeftClicked()
		{
			CharacterCustomization cust = null;
			CreationDirector.CustomizationCategory category = this.GetCategory();
			if (category != null)
			{
				cust = category.IterateBackward();
			}
			this.LoadRecipe(cust);
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x001E98CC File Offset: 0x001E7ACC
		private void OnRightClicked()
		{
			CharacterCustomization cust = null;
			CreationDirector.CustomizationCategory category = this.GetCategory();
			if (category != null)
			{
				cust = category.IterateForward();
			}
			this.LoadRecipe(cust);
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x001E98F4 File Offset: 0x001E7AF4
		private void LoadRecipe(CharacterCustomization cust)
		{
			if (cust == null)
			{
				this.m_character.DCA.ClearSlot(this.m_settings.Slot.ToString());
				this.m_selectionLabel.text = "None";
			}
			else
			{
				this.m_character.DCA.SetSlot(cust.Recipe);
				this.m_selectionLabel.text = cust.DisplayName;
			}
			this.m_character.DCA.Refresh(true, true, true);
		}

		// Token: 0x0600594E RID: 22862 RVA: 0x001E9980 File Offset: 0x001E7B80
		public override void Reset()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Reset();
			CharacterCustomization cust = null;
			CreationDirector.CustomizationCategory category = this.GetCategory();
			if (category != null)
			{
				category.Index = 0;
				cust = category.Customizations[0];
			}
			this.LoadRecipe(cust);
		}

		// Token: 0x0600594F RID: 22863 RVA: 0x001E99C4 File Offset: 0x001E7BC4
		public override void Randomize()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Randomize();
			CharacterCustomization cust = null;
			CreationDirector.CustomizationCategory category = this.GetCategory();
			if (category != null)
			{
				category.Index = UnityEngine.Random.Range(0, category.Customizations.Count);
				cust = category.Customizations[category.Index];
			}
			this.LoadRecipe(cust);
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x001E9A1C File Offset: 0x001E7C1C
		private CreationDirector.CustomizationCategory GetCategory()
		{
			CreationDirector.CustomizationCategory result = null;
			this.m_character.Categories.TryGetValue(this.m_settings.Slot, out result);
			return result;
		}

		// Token: 0x04004E9C RID: 20124
		[SerializeField]
		private TextMeshProUGUI m_selectionLabel;

		// Token: 0x04004E9D RID: 20125
		[SerializeField]
		private Button m_left;

		// Token: 0x04004E9E RID: 20126
		[SerializeField]
		private Button m_right;
	}
}
