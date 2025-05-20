using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x02000875 RID: 2165
	public class DeployPortableCraftingStationUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000E8B RID: 3723
		// (get) Token: 0x06003EE3 RID: 16099 RVA: 0x0006A8AB File Offset: 0x00068AAB
		// (set) Token: 0x06003EE4 RID: 16100 RVA: 0x0006A8B2 File Offset: 0x00068AB2
		public static DateTime? LastPortableDeployment { get; set; }

		// Token: 0x06003EE5 RID: 16101 RVA: 0x0006A8BA File Offset: 0x00068ABA
		private void Start()
		{
			if (!this.m_deployButton)
			{
				base.enabled = false;
				return;
			}
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x0006A8F0 File Offset: 0x00068AF0
		private void OnDestroy()
		{
			if (this.m_isSubscriber)
			{
				this.m_deployButton.onClick.RemoveListener(new UnityAction(this.DeployPortableCraftingStationClicked));
			}
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x00186700 File Offset: 0x00184900
		private void Update()
		{
			if (this.m_parentWindow && this.m_parentWindow.Visible)
			{
				if (DeployPortableCraftingStationUI.LastPortableDeployment != null && SessionData.User != null && SessionData.User.IsSubscriber())
				{
					double totalSeconds = (DateTime.UtcNow - DeployPortableCraftingStationUI.LastPortableDeployment.Value).TotalSeconds;
					int cooldown = GlobalSettings.Values.Subscribers.DeployPortableCraftingStationAbility.Cooldown;
					if (totalSeconds >= (double)cooldown)
					{
						this.ResetDeployButton();
						return;
					}
					this.m_timeLeft = ((double)cooldown - totalSeconds).GetFormattedTime(false);
					this.m_deployButton.interactable = false;
					this.m_deployButton.SetText(this.m_timeLeft);
					return;
				}
				else if (DeployPortableCraftingStationUI.LastPortableDeployment == null && !this.m_deployButton.interactable)
				{
					this.ResetDeployButton();
				}
			}
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x001867E4 File Offset: 0x001849E4
		private void ResetDeployButton()
		{
			this.m_deployButton.interactable = true;
			this.m_deployButton.SetText("Portable");
			DeployPortableCraftingStationUI.LastPortableDeployment = null;
		}

		// Token: 0x06003EE9 RID: 16105 RVA: 0x0006A916 File Offset: 0x00068B16
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x06003EEA RID: 16106 RVA: 0x0018681C File Offset: 0x00184A1C
		private void Init()
		{
			this.m_isSubscriber = (SessionData.User != null && SessionData.User.IsSubscriber());
			this.m_deployButton.gameObject.SetActive(this.m_isSubscriber);
			if (!this.m_isSubscriber)
			{
				base.enabled = false;
				this.m_deployButton.gameObject.SetActive(false);
				return;
			}
			this.m_deployButton.SetText("Portable");
			this.m_deployButton.gameObject.SetActive(true);
			this.m_deployButton.onClick.AddListener(new UnityAction(this.DeployPortableCraftingStationClicked));
			DateTime timestampOfLastConsumable = LocalPlayer.GameEntity.SkillsController.GetTimestampOfLastConsumable(ConsumableCategory.CraftingStation);
			if (timestampOfLastConsumable > DateTime.MinValue)
			{
				DeployPortableCraftingStationUI.LastPortableDeployment = new DateTime?(timestampOfLastConsumable);
			}
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x001868E4 File Offset: 0x00184AE4
		private void DeployPortableCraftingStationClicked()
		{
			if (SessionData.User != null && SessionData.User.IsSubscriber() && GlobalSettings.Values && GlobalSettings.Values.Crafting != null && GlobalSettings.Values.Subscribers.DeployPortableCraftingStationAbility && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null && LocalPlayer.GameEntity.SkillsController != null)
			{
				ArchetypeInstance instance = GlobalSettings.Values.Subscribers.DeployPortableCraftingStationAbility.DynamicallyLoad(LocalPlayer.GameEntity.CollectionController.Abilities);
				LocalPlayer.GameEntity.SkillsController.BeginExecution(instance);
			}
		}

		// Token: 0x06003EEC RID: 16108 RVA: 0x001869B0 File Offset: 0x00184BB0
		private ITooltipParameter GetTooltipParameter()
		{
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendLine("Deploy a portable crafting station.");
				if (this.m_deployButton.interactable)
				{
					utf16ValueStringBuilder.AppendFormat<string>("{0} Cooldown", GlobalSettings.Values.Subscribers.DeployPortableCraftingStationAbility.Cooldown.GetFormattedTime(false));
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string>("{0} Cooldown remaining.", this.m_timeLeft);
				}
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000E8C RID: 3724
		// (get) Token: 0x06003EED RID: 16109 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E8D RID: 3725
		// (get) Token: 0x06003EEE RID: 16110 RVA: 0x0006A92F File Offset: 0x00068B2F
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E8E RID: 3726
		// (get) Token: 0x06003EEF RID: 16111 RVA: 0x0006A93D File Offset: 0x00068B3D
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003CDE RID: 15582
		private const string kButtonText = "Portable";

		// Token: 0x04003CE0 RID: 15584
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003CE1 RID: 15585
		[SerializeField]
		private UIWindow m_parentWindow;

		// Token: 0x04003CE2 RID: 15586
		[SerializeField]
		private SolButton m_deployButton;

		// Token: 0x04003CE3 RID: 15587
		private bool m_isSubscriber;

		// Token: 0x04003CE4 RID: 15588
		private string m_timeLeft = string.Empty;
	}
}
