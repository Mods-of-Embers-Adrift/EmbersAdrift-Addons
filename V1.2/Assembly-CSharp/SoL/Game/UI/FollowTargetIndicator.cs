using System;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000886 RID: 2182
	public class FollowTargetIndicator : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003F86 RID: 16262 RVA: 0x0006AF57 File Offset: 0x00069157
		private void Awake()
		{
			LocalPlayer.FollowTargetChanged += this.LocalPlayerOnFollowTargetChanged;
		}

		// Token: 0x06003F87 RID: 16263 RVA: 0x0006AF6A File Offset: 0x0006916A
		private void Start()
		{
			if (this.m_icon)
			{
				this.m_icon.color = GlobalSettings.Values.Subscribers.SubscriberColor;
			}
			this.LocalPlayerOnFollowTargetChanged(LocalPlayer.FollowTarget);
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x0006AF9E File Offset: 0x0006919E
		private void OnDestroy()
		{
			LocalPlayer.FollowTargetChanged -= this.LocalPlayerOnFollowTargetChanged;
		}

		// Token: 0x06003F89 RID: 16265 RVA: 0x00189248 File Offset: 0x00187448
		private void LocalPlayerOnFollowTargetChanged(GameEntity obj)
		{
			if (obj && this.m_controller && this.m_controller.Member != null && this.m_controller.Member.Entity == obj)
			{
				this.m_visuals.SetActive(true);
				return;
			}
			this.m_visuals.SetActive(false);
		}

		// Token: 0x06003F8A RID: 16266 RVA: 0x0006AFB1 File Offset: 0x000691B1
		private ITooltipParameter GetParameter()
		{
			return new ObjectTextTooltipParameter(this, "You are following this player", false);
		}

		// Token: 0x17000EAF RID: 3759
		// (get) Token: 0x06003F8B RID: 16267 RVA: 0x0006AFC4 File Offset: 0x000691C4
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000EB0 RID: 3760
		// (get) Token: 0x06003F8C RID: 16268 RVA: 0x0006AFD2 File Offset: 0x000691D2
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000EB1 RID: 3761
		// (get) Token: 0x06003F8D RID: 16269 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003F8F RID: 16271 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D4E RID: 15694
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003D4F RID: 15695
		[SerializeField]
		private GroupNameplateControllerUI m_controller;

		// Token: 0x04003D50 RID: 15696
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x04003D51 RID: 15697
		[SerializeField]
		private Image m_icon;
	}
}
