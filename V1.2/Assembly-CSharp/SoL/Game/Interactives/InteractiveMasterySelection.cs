using System;
using System.Collections;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B97 RID: 2967
	public class InteractiveMasterySelection : BaseInteractive, ITooltip, IInteractiveBase
	{
		// Token: 0x06005B5F RID: 23391 RVA: 0x0007D5DE File Offset: 0x0007B7DE
		protected override void Awake()
		{
			base.Awake();
			if (!GameManager.IsServer)
			{
				this.UpdateLabel();
			}
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001EE870 File Offset: 0x001ECA70
		private void UpdateLabel()
		{
			if (this.m_label != null && this.m_bundle != null && this.m_bundle.Mastery != null)
			{
				this.m_label.text = this.m_bundle.Mastery.DisplayName;
				return;
			}
			if (this.m_label != null)
			{
				this.m_label.text = "";
			}
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001EE8E8 File Offset: 0x001ECAE8
		protected override bool DoubleClickPreCondition(GameEntity entity)
		{
			if (!base.DoubleClickPreCondition(entity))
			{
				return false;
			}
			string msg = "";
			if (this.m_bundle == null || this.m_bundle.Mastery == null || !this.m_bundle.Mastery.CanBeLearnedBy(entity, out msg))
			{
				this.SendErrorMessage(entity, msg);
				return false;
			}
			return true;
		}

		// Token: 0x06005B62 RID: 23394 RVA: 0x0007D5F3 File Offset: 0x0007B7F3
		private void SendErrorMessage(GameEntity interactionSource, string msg)
		{
			if (GameManager.IsServer)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.SendChatNotification(msg);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, msg);
		}

		// Token: 0x06005B63 RID: 23395 RVA: 0x001EE948 File Offset: 0x001ECB48
		public override void BeginInteraction(GameEntity interactionSource)
		{
			base.BeginInteraction(interactionSource);
			if (this.m_bundle != null && this.m_bundle.Mastery != null)
			{
				IMerchantInventory bundle = this.m_bundle;
				if (bundle != null)
				{
					ArchetypeInstance archetypeInstance;
					bundle.AddToPlayer(interactionSource, ItemAddContext.Merchant, 1U, ItemFlags.None, false, out archetypeInstance);
					ArchetypeInstance archetypeInstance2;
					if (this.m_startingMasteryLevel > 1f && interactionSource.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_bundle.Mastery.Id, out archetypeInstance2))
					{
						archetypeInstance2.MasteryData.BaseLevel = this.m_startingMasteryLevel;
						interactionSource.NetworkEntity.PlayerRpcHandler.Server_MasteryLevelChanged(this.m_bundle.Mastery.Id, archetypeInstance2.MasteryData.BaseLevel);
					}
				}
			}
		}

		// Token: 0x06005B64 RID: 23396 RVA: 0x0007D61B File Offset: 0x0007B81B
		private IEnumerable GetMasteryBundles()
		{
			return SolOdinUtilities.GetDropdownItems<MasteryAbilityBundle>();
		}

		// Token: 0x06005B65 RID: 23397 RVA: 0x001EEA08 File Offset: 0x001ECC08
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.CanInteract(LocalPlayer.GameEntity))
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = null,
				Archetype = this.m_bundle
			};
		}

		// Token: 0x17001569 RID: 5481
		// (get) Token: 0x06005B66 RID: 23398 RVA: 0x0007D622 File Offset: 0x0007B822
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700156A RID: 5482
		// (get) Token: 0x06005B67 RID: 23399 RVA: 0x0007D630 File Offset: 0x0007B830
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005B69 RID: 23401 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004FCB RID: 20427
		[SerializeField]
		private MasteryAbilityBundle m_bundle;

		// Token: 0x04004FCC RID: 20428
		[SerializeField]
		private TextMeshPro m_label;

		// Token: 0x04004FCD RID: 20429
		[SerializeField]
		private float m_startingMasteryLevel = 1f;

		// Token: 0x04004FCE RID: 20430
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
