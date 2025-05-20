using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008AF RID: 2223
	public class MasteryProgressBarBubble : MonoBehaviour, IContextMenu, IInteractiveBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000EE9 RID: 3817
		// (get) Token: 0x06004123 RID: 16675 RVA: 0x0006C0C5 File Offset: 0x0006A2C5
		public Image Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x06004124 RID: 16676 RVA: 0x0006C0CD File Offset: 0x0006A2CD
		private void Awake()
		{
			this.m_highlight.Toggle(false);
		}

		// Token: 0x06004125 RID: 16677 RVA: 0x0006C0DB File Offset: 0x0006A2DB
		public void Init(MasteryProgressBar controller)
		{
			this.m_controller = controller;
		}

		// Token: 0x06004126 RID: 16678 RVA: 0x0006C0E4 File Offset: 0x0006A2E4
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_highlight.Toggle(true);
		}

		// Token: 0x06004127 RID: 16679 RVA: 0x0006C0CD File Offset: 0x0006A2CD
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_highlight.Toggle(false);
		}

		// Token: 0x17000EEA RID: 3818
		// (get) Token: 0x06004128 RID: 16680 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004129 RID: 16681 RVA: 0x0018E850 File Offset: 0x0018CA50
		public string FillActionsGetTitle()
		{
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("None", true, delegate()
			{
				this.SelectMastery(null);
			}, null, null);
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Masteries, out containerInstance))
			{
				using (IEnumerator<ArchetypeInstance> enumerator = containerInstance.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ArchetypeInstance masteryInstance = enumerator.Current;
						string displayName = masteryInstance.Archetype.DisplayName;
						SpecializedRole specializedRole;
						if (masteryInstance.MasteryData != null && masteryInstance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(masteryInstance.MasteryData.Specialization.Value, out specializedRole))
						{
							displayName = specializedRole.DisplayName;
						}
						ContextMenuUI.AddContextAction(displayName, true, delegate()
						{
							this.SelectMastery(masteryInstance);
						}, null, null);
					}
				}
			}
			return "Track Experience For";
		}

		// Token: 0x0600412A RID: 16682 RVA: 0x0006C0F2 File Offset: 0x0006A2F2
		private void SelectMastery(ArchetypeInstance masteryInstance)
		{
			this.m_controller.SelectMastery(masteryInstance);
		}

		// Token: 0x0600412C RID: 16684 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E9D RID: 16029
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003E9E RID: 16030
		[SerializeField]
		private ToggleController m_highlight;

		// Token: 0x04003E9F RID: 16031
		private MasteryProgressBar m_controller;
	}
}
