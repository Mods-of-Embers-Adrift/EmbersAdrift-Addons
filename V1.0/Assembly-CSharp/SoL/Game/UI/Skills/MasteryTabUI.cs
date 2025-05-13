using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092E RID: 2350
	public class MasteryTabUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000F7F RID: 3967
		// (get) Token: 0x06004519 RID: 17689 RVA: 0x0006EA1C File Offset: 0x0006CC1C
		public bool LowerTab
		{
			get
			{
				return this.m_lowerTab;
			}
		}

		// Token: 0x17000F80 RID: 3968
		// (get) Token: 0x0600451A RID: 17690 RVA: 0x0006EA24 File Offset: 0x0006CC24
		// (set) Token: 0x0600451B RID: 17691 RVA: 0x0006EA31 File Offset: 0x0006CC31
		public bool ToggleIsOn
		{
			get
			{
				return this.m_toggle.isOn;
			}
			set
			{
				this.m_toggle.isOn = value;
			}
		}

		// Token: 0x17000F81 RID: 3969
		// (get) Token: 0x0600451C RID: 17692 RVA: 0x0006EA3F File Offset: 0x0006CC3F
		// (set) Token: 0x0600451D RID: 17693 RVA: 0x0019E924 File Offset: 0x0019CB24
		public ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
			set
			{
				this.m_instance = value;
				if (this.m_instance != null)
				{
					this.m_image.overrideSprite = this.m_instance.Archetype.Icon;
					this.m_frame.color = this.m_instance.Archetype.FrameColor;
					base.gameObject.SetActive(true);
					return;
				}
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600451E RID: 17694 RVA: 0x0019E990 File Offset: 0x0019CB90
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Instance != null)
			{
				return new ArchetypeTooltipParameter
				{
					Instance = this.Instance
				};
			}
			return null;
		}

		// Token: 0x17000F82 RID: 3970
		// (get) Token: 0x0600451F RID: 17695 RVA: 0x0006EA47 File Offset: 0x0006CC47
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F83 RID: 3971
		// (get) Token: 0x06004520 RID: 17696 RVA: 0x0006EA55 File Offset: 0x0006CC55
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F84 RID: 3972
		// (get) Token: 0x06004521 RID: 17697 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004523 RID: 17699 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004193 RID: 16787
		[SerializeField]
		private Image m_image;

		// Token: 0x04004194 RID: 16788
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004195 RID: 16789
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004196 RID: 16790
		[SerializeField]
		private bool m_lowerTab;

		// Token: 0x04004197 RID: 16791
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004198 RID: 16792
		private ArchetypeInstance m_instance;
	}
}
