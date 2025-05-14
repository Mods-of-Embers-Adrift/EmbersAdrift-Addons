using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008B1 RID: 2225
	public class MasterySelectionBubble : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000EEB RID: 3819
		// (get) Token: 0x06004130 RID: 16688 RVA: 0x0006C11C File Offset: 0x0006A31C
		// (set) Token: 0x06004131 RID: 16689 RVA: 0x0006C124 File Offset: 0x0006A324
		public MasterySelectionBubblesUI BubbleController { get; set; }

		// Token: 0x06004132 RID: 16690 RVA: 0x0004475B File Offset: 0x0004295B
		private void Awake()
		{
		}

		// Token: 0x06004133 RID: 16691 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnDestroy()
		{
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x0006C12D File Offset: 0x0006A32D
		public void ToggleLock(bool locked)
		{
			this.m_button.interactable = !locked;
		}

		// Token: 0x06004135 RID: 16693 RVA: 0x0006C13E File Offset: 0x0006A33E
		public void Init(ArchetypeInstance instance, CombatMasteryArchetype mastery, bool locked, bool selected)
		{
			this.m_instance = instance;
			this.m_mastery = mastery;
			this.m_image.overrideSprite = mastery.Icon;
			this.m_toggleController.State = (selected ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
			this.ToggleLock(locked);
		}

		// Token: 0x06004136 RID: 16694 RVA: 0x0018E968 File Offset: 0x0018CB68
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_mastery == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance
			};
		}

		// Token: 0x17000EEC RID: 3820
		// (get) Token: 0x06004137 RID: 16695 RVA: 0x0006C179 File Offset: 0x0006A379
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EED RID: 3821
		// (get) Token: 0x06004138 RID: 16696 RVA: 0x0006C187 File Offset: 0x0006A387
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EEE RID: 3822
		// (get) Token: 0x06004139 RID: 16697 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600413B RID: 16699 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003EA2 RID: 16034
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003EA3 RID: 16035
		[SerializeField]
		private Image m_image;

		// Token: 0x04003EA4 RID: 16036
		[SerializeField]
		private ToggleController m_toggleController;

		// Token: 0x04003EA6 RID: 16038
		private ArchetypeInstance m_instance;

		// Token: 0x04003EA7 RID: 16039
		private CombatMasteryArchetype m_mastery;

		// Token: 0x04003EA8 RID: 16040
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
