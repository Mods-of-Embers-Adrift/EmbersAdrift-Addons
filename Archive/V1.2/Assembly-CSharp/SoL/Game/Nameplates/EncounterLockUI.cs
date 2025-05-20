using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D3 RID: 2515
	public class EncounterLockUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004C8E RID: 19598 RVA: 0x00073C99 File Offset: 0x00071E99
		private void Awake()
		{
			this.m_defaultColor = this.m_baseImage.color;
			this.DisableImages();
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x001BD0DC File Offset: 0x001BB2DC
		public void RefreshIndicator(NameplateControllerUI controller)
		{
			if (controller == null || !controller.AllowLockIndicator() || controller.Targetable == null || controller.Targetable.Entity == null || controller.Targetable.Entity.Interactive == null)
			{
				this.DisableImages();
				return;
			}
			InteractiveNpc interactiveNpc = controller.Targetable.Entity.Interactive as InteractiveNpc;
			if (interactiveNpc == null)
			{
				this.DisableImages();
				return;
			}
			bool isTagged = interactiveNpc.IsTagged;
			bool isInteractive = interactiveNpc.IsInteractive;
			if (interactiveNpc.NpcContributed)
			{
				this.SetLocked();
				return;
			}
			if (isTagged)
			{
				if (interactiveNpc.IsValidTagger(LocalPlayer.GameEntity))
				{
					this.SetUnlocked();
					return;
				}
				this.SetLocked();
				return;
			}
			else
			{
				if (isInteractive)
				{
					this.SetUnlocked();
					return;
				}
				this.DisableImages();
				return;
			}
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x00073CB2 File Offset: 0x00071EB2
		private void DisableImages()
		{
			this.m_parent.SetActive(false);
			this.m_available = false;
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x00073CC7 File Offset: 0x00071EC7
		private void SetLocked()
		{
			this.m_baseImage.enabled = true;
			this.m_baseImage.color = this.m_unavailableColor;
			this.m_slashImage.enabled = true;
			this.m_parent.SetActive(true);
			this.m_available = false;
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x00073D05 File Offset: 0x00071F05
		private void SetUnlocked()
		{
			this.m_baseImage.enabled = true;
			this.m_baseImage.color = this.m_defaultColor;
			this.m_slashImage.enabled = false;
			this.m_parent.SetActive(true);
			this.m_available = true;
		}

		// Token: 0x06004C93 RID: 19603 RVA: 0x001BD1A0 File Offset: 0x001BB3A0
		private ITooltipParameter GetTooltipParameter()
		{
			string txt = this.m_available ? "Valid Permissions" : "Invalid Permissions";
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x170010EA RID: 4330
		// (get) Token: 0x06004C94 RID: 19604 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170010EB RID: 4331
		// (get) Token: 0x06004C95 RID: 19605 RVA: 0x00073D43 File Offset: 0x00071F43
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010EC RID: 4332
		// (get) Token: 0x06004C96 RID: 19606 RVA: 0x00073D51 File Offset: 0x00071F51
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004C98 RID: 19608 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400466F RID: 18031
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004670 RID: 18032
		[SerializeField]
		private GameObject m_parent;

		// Token: 0x04004671 RID: 18033
		[SerializeField]
		private Color m_unavailableColor;

		// Token: 0x04004672 RID: 18034
		[SerializeField]
		private Image m_baseImage;

		// Token: 0x04004673 RID: 18035
		[SerializeField]
		private Image m_slashImage;

		// Token: 0x04004674 RID: 18036
		private Color m_defaultColor;

		// Token: 0x04004675 RID: 18037
		private bool m_available;
	}
}
