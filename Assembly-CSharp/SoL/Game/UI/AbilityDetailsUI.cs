using System;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200085D RID: 2141
	public class AbilityDetailsUI : ContainerSlotUI
	{
		// Token: 0x06003DD0 RID: 15824 RVA: 0x00069DC0 File Offset: 0x00067FC0
		private void Awake()
		{
			if (ClientGameManager.UIManager != null)
			{
				this.m_containerUI = ClientGameManager.UIManager.AbilityPanel;
			}
		}

		// Token: 0x06003DD1 RID: 15825 RVA: 0x00069DDF File Offset: 0x00067FDF
		private void OnEnable()
		{
			base.InitHighlight();
		}

		// Token: 0x06003DD2 RID: 15826 RVA: 0x00183948 File Offset: 0x00181B48
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			if (instance.Index == -1)
			{
				base.InstanceAdded(instance);
			}
			this.m_instance = instance;
			this.m_image.overrideSprite = instance.Archetype.Icon;
			this.m_name.text = instance.Archetype.DisplayName;
		}

		// Token: 0x06003DD3 RID: 15827 RVA: 0x001820F0 File Offset: 0x001802F0
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

		// Token: 0x04003C4F RID: 15439
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04003C50 RID: 15440
		[SerializeField]
		private Image m_image;
	}
}
