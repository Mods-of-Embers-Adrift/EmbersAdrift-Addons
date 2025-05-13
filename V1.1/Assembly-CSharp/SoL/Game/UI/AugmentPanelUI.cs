using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000853 RID: 2131
	public class AugmentPanelUI : MonoBehaviour
	{
		// Token: 0x06003D88 RID: 15752 RVA: 0x00069AC0 File Offset: 0x00067CC0
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x00182DAC File Offset: 0x00180FAC
		public void Subscribe(ArchetypeInstance instance)
		{
			this.Unsubscribe();
			if (instance != null && instance.ItemData != null && instance.Archetype != null && instance.Archetype.IsAugmentable)
			{
				this.m_instance = instance;
				instance.ItemData.AugmentChanged += this.OnAugmentChanged;
				this.OnAugmentChanged();
				this.m_subscribed = true;
			}
			this.ToggleObject();
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x00182E18 File Offset: 0x00181018
		public void Unsubscribe()
		{
			if (this.m_subscribed && this.m_instance != null && this.m_instance.ItemData != null)
			{
				this.m_instance.ItemData.AugmentChanged -= this.OnAugmentChanged;
			}
			this.m_instance = null;
			this.m_subscribed = false;
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x00182E6C File Offset: 0x0018106C
		private void OnAugmentChanged()
		{
			float fillAmount = 0f;
			if (this.m_instance != null && this.m_instance.ItemData != null && this.m_instance.ItemData.Augment != null && this.m_instance.ItemData.Augment.AugmentItemRef != null)
			{
				float num = (float)this.m_instance.ItemData.Augment.AugmentItemRef.ExpirationAmount;
				float num2 = num * 5f;
				fillAmount = ((float)this.m_instance.ItemData.Augment.StackCount * num - (float)this.m_instance.ItemData.Augment.Count) / num2;
			}
			if (this.m_status)
			{
				this.m_status.fillAmount = fillAmount;
			}
			this.ToggleObject();
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x00182F40 File Offset: 0x00181140
		private void ToggleObject()
		{
			((this.m_objectToDisable == null) ? base.gameObject : this.m_objectToDisable).SetActive(this.m_instance != null && this.m_instance.ItemData != null && this.m_instance.ItemData.Augment != null);
		}

		// Token: 0x04003C29 RID: 15401
		[SerializeField]
		private Image m_status;

		// Token: 0x04003C2A RID: 15402
		[SerializeField]
		private GameObject m_objectToDisable;

		// Token: 0x04003C2B RID: 15403
		private ArchetypeInstance m_instance;

		// Token: 0x04003C2C RID: 15404
		private bool m_subscribed;
	}
}
