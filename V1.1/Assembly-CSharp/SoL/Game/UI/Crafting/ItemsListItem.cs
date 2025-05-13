using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200099A RID: 2458
	public class ItemsListItem : MonoBehaviour
	{
		// Token: 0x06004987 RID: 18823 RVA: 0x00071664 File Offset: 0x0006F864
		private void Start()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.OnClick));
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x00071682 File Offset: 0x0006F882
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.OnClick));
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x001AFE64 File Offset: 0x001AE064
		public void Init(ItemsList parent, List<ArchetypeInstance> instances, SpecialItemListItemType? special = null)
		{
			this.m_parent = parent;
			this.m_typeCode = ((instances != null && instances.Count > 0) ? instances[0].CombinedTypeCode : 0);
			int amountRequired = -1;
			if (this.m_parent != null && instances != null && instances.Count > 0)
			{
				foreach (ComponentMaterial componentMaterial in this.m_parent.Component.AcceptableMaterials)
				{
					if (instances[0].ArchetypeId == componentMaterial.Archetype.Id)
					{
						amountRequired = componentMaterial.AmountRequired;
						break;
					}
				}
			}
			if (special == null)
			{
				this.m_itemButton.SetItem(instances, amountRequired, false);
				return;
			}
			this.m_itemButton.SetSpecial(special.Value);
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x000716A0 File Offset: 0x0006F8A0
		private void OnClick()
		{
			if (this.m_parent != null)
			{
				this.m_parent.PickItem(this.m_typeCode);
			}
		}

		// Token: 0x04004491 RID: 17553
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04004492 RID: 17554
		[SerializeField]
		private ItemButton m_itemButton;

		// Token: 0x04004493 RID: 17555
		private ItemsList m_parent;

		// Token: 0x04004494 RID: 17556
		private int m_typeCode;
	}
}
