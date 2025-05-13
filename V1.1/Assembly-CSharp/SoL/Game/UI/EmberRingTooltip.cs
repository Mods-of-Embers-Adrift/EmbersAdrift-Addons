using System;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200087A RID: 2170
	public class EmberRingTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003F23 RID: 16163 RVA: 0x0018791C File Offset: 0x00185B1C
		private string GetTooltipForCampfire()
		{
			if (this.m_applicator.Lightable == null && (this.m_applicator.Interactive == null || this.m_applicator.Interactive.Data.Value.Item == null))
			{
				return "Campfire";
			}
			if (this.m_applicator.Lightable != null && this.m_applicator.Lightable.CampfireData.Value.IsLit)
			{
				double totalSeconds = (this.m_applicator.Lightable.ExpirationTimestamp - DateTime.UtcNow).TotalSeconds;
				return ZString.Format<int, string, string>("Campfire +{0} lit by {1}\nTime Remaining: {2}", this.m_applicator.Lightable.CampfireData.Value.MasteryLevel, this.m_applicator.Lightable.CampfireData.Value.LighterName, totalSeconds.GetFormattedTime(true));
			}
			return "Unlit campfire";
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x00187A1C File Offset: 0x00185C1C
		private string GetTooltipForEmberRing()
		{
			DiscoveryProfile discoveryProfile = (this.m_discoveryForwarder && this.m_discoveryForwarder.Trigger && this.m_discoveryForwarder.Trigger.Profile) ? this.m_discoveryForwarder.Trigger.Profile : null;
			string result = "";
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append("Ember Ring");
				if (this.m_applicator.Interactive != null)
				{
					if (this.m_applicator.Interactive.Data != null && this.m_applicator.Interactive.Data.Value.Item != null)
					{
						EmberRingEnhancementData value = this.m_applicator.Interactive.Data.Value;
						string formattedTime = (value.ExpirationTime - GameTimeReplicator.GetServerCorrectedDateTime(DateTime.UtcNow)).TotalSeconds.GetFormattedTime(true);
						utf16ValueStringBuilder.AppendFormat<int, string, string, string>("\n  Enhanced +{0} by <i>{1}</i>\n  {2} Remaining: {3}", value.Item.Level, value.SourceName, "<sprite=\"SolIcons\" name=\"Circle\" tint=1>", formattedTime);
					}
					string arg = "Inactive";
					Color color = UIManager.RequirementsNotMetColor;
					if (this.m_applicator.Interactive.SubscriberPresent.Value)
					{
						arg = "Active";
						color = UIManager.RequirementsMetColor;
					}
					utf16ValueStringBuilder.AppendFormat<string, string, string>("\n  <color={0}>Subscriber Bonus:</color> <color={1}>{2}</color>", UIManager.SubscriberColor.ToHex(), color.ToHex(), arg);
				}
				if (discoveryProfile)
				{
					LeyLinkProfile leyLinkProfile = discoveryProfile as LeyLinkProfile;
					if (leyLinkProfile != null)
					{
						if (string.IsNullOrEmpty(leyLinkProfile.DisplayName))
						{
							utf16ValueStringBuilder.AppendFormat<string>("\n{0}", "Ley Link");
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string, string>("\n{0} {1}", leyLinkProfile.DisplayName, "Ley Link");
						}
						utf16ValueStringBuilder.AppendFormat<string>("\n  Required Level: {0}", leyLinkProfile.GetRequiredLevelTooltipText());
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x00187C28 File Offset: 0x00185E28
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_applicator)
			{
				return null;
			}
			string txt = (this.m_applicator.Type == CampfireEffectApplicator.CampfireType.Campfire) ? this.GetTooltipForCampfire() : this.GetTooltipForEmberRing();
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000E98 RID: 3736
		// (get) Token: 0x06003F26 RID: 16166 RVA: 0x0006ABC1 File Offset: 0x00068DC1
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E99 RID: 3737
		// (get) Token: 0x06003F27 RID: 16167 RVA: 0x0006ABCF File Offset: 0x00068DCF
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E9A RID: 3738
		// (get) Token: 0x06003F28 RID: 16168 RVA: 0x0006ABD7 File Offset: 0x00068DD7
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D13 RID: 15635
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04003D14 RID: 15636
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003D15 RID: 15637
		[SerializeField]
		private DiscoveryForwarder m_discoveryForwarder;

		// Token: 0x04003D16 RID: 15638
		[SerializeField]
		private CampfireEffectApplicator m_applicator;
	}
}
