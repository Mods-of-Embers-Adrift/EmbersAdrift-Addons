using System;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A3 RID: 1443
	public class RoleIndicator : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06002D38 RID: 11576 RVA: 0x0005F68F File Offset: 0x0005D88F
		private void Awake()
		{
			if (this.m_image)
			{
				this.m_defaultImageColor = this.m_image.color;
			}
		}

		// Token: 0x06002D39 RID: 11577 RVA: 0x0014D2E8 File Offset: 0x0014B4E8
		public void RefreshIndicator(NameplateControllerUI controller)
		{
			if (controller.Targetable == null || !controller.Targetable.Entity || !controller.Targetable.Entity.CharacterData || controller.Targetable.Entity.CharacterData.BaseRoleId.IsEmpty)
			{
				this.DisableIndicator();
				return;
			}
			this.m_baseRole = null;
			this.m_specializedRole = null;
			CharacterData characterData = controller.Targetable.Entity.CharacterData;
			if (!characterData.BaseRoleId.IsEmpty)
			{
				InternalGameDatabase.Archetypes.TryGetAsType<BaseRole>(characterData.BaseRoleId, out this.m_baseRole);
			}
			if (!characterData.SpecializedRoleId.IsEmpty)
			{
				InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(characterData.SpecializedRoleId, out this.m_specializedRole);
			}
			if (this.m_specializedRole)
			{
				this.m_image.sprite = this.m_specializedRole.Icon;
				this.m_image.color = this.m_specializedRole.IconTint;
			}
			else
			{
				if (!this.m_baseRole)
				{
					this.DisableIndicator();
					return;
				}
				this.m_image.sprite = this.m_baseRole.Icon;
				this.m_image.color = this.m_baseRole.IconTint;
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06002D3A RID: 11578 RVA: 0x0005F6AF File Offset: 0x0005D8AF
		public void DisableIndicator()
		{
			base.gameObject.SetActive(false);
			this.m_image.sprite = null;
			this.m_image.color = this.m_defaultImageColor;
		}

		// Token: 0x06002D3B RID: 11579 RVA: 0x0014D444 File Offset: 0x0014B644
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_baseRole == null && this.m_specializedRole == null)
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, (this.m_specializedRole != null) ? this.m_specializedRole.DisplayName : this.m_baseRole.DisplayName, false);
		}

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06002D3C RID: 11580 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x06002D3D RID: 11581 RVA: 0x0005F6DA File Offset: 0x0005D8DA
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06002D3E RID: 11582 RVA: 0x0005F6E8 File Offset: 0x0005D8E8
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06002D40 RID: 11584 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002CCE RID: 11470
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002CCF RID: 11471
		[SerializeField]
		private Image m_image;

		// Token: 0x04002CD0 RID: 11472
		private BaseRole m_baseRole;

		// Token: 0x04002CD1 RID: 11473
		private SpecializedRole m_specializedRole;

		// Token: 0x04002CD2 RID: 11474
		private Color m_defaultImageColor;
	}
}
