using System;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200084B RID: 2123
	[Obsolete]
	public class AbilityPanelSlotUI : ContainerSlotUI
	{
		// Token: 0x06003D36 RID: 15670 RVA: 0x0006974A File Offset: 0x0006794A
		private void Awake()
		{
			this.m_containerUI = ClientGameManager.UIManager.AbilityPanel;
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x0006975C File Offset: 0x0006795C
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			if (instance.Index == -1)
			{
				base.InstanceAdded(instance);
			}
			this.m_image.overrideSprite = instance.Archetype.Icon;
			this.m_name.text = instance.Archetype.DisplayName;
		}

		// Token: 0x06003D38 RID: 15672 RVA: 0x0018214C File Offset: 0x0018034C
		protected override ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance
			};
		}

		// Token: 0x04003C09 RID: 15369
		[SerializeField]
		private Image m_image;

		// Token: 0x04003C0A RID: 15370
		[SerializeField]
		private TextMeshProUGUI m_name;
	}
}
