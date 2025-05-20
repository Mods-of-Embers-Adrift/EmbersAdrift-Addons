using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x0200059A RID: 1434
	public class LeyLinkIndicator : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06002CB7 RID: 11447 RVA: 0x0005F018 File Offset: 0x0005D218
		private void Start()
		{
			DiscoveryProgression.DiscoveryFound += this.DiscoveryProgressionOnDiscoveryFound;
			LocalPlayer.LocalPlayerInitialized += this.RefreshIcon;
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x0005F03C File Offset: 0x0005D23C
		private void OnDestroy()
		{
			DiscoveryProgression.DiscoveryFound -= this.DiscoveryProgressionOnDiscoveryFound;
			LocalPlayer.LocalPlayerInitialized -= this.RefreshIcon;
		}

		// Token: 0x06002CB9 RID: 11449 RVA: 0x0014AE70 File Offset: 0x00149070
		internal void RefreshMember(GroupMember member)
		{
			this.m_member = member;
			this.m_profile = null;
			if (this.m_member != null && this.m_member.ZoneId > 0 && this.m_member.EmberRingIndex > 0)
			{
				this.m_profile = GlobalSettings.Values.Ashen.GetLeyLinkProfileByIndex(this.m_member.EmberRingIndex);
			}
			this.RefreshIcon();
		}

		// Token: 0x06002CBA RID: 11450 RVA: 0x0014AED8 File Offset: 0x001490D8
		private void RefreshIcon()
		{
			this.m_hasDiscovered = false;
			this.m_meetsLevelRequirements = false;
			if (this.m_profile && LocalPlayer.GameEntity)
			{
				List<UniqueId> list;
				this.m_hasDiscovered = (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Discoveries != null && LocalPlayer.GameEntity.CollectionController.Record.Discoveries.TryGetValue((ZoneId)this.m_member.ZoneId, out list) && list.Contains(this.m_profile.Id));
				this.m_meetsLevelRequirements = (LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.AdventuringLevel >= this.m_profile.LevelRequirement);
			}
			if (this.m_icon)
			{
				this.m_icon.enabled = (this.m_member != null && !this.m_member.IsSelf && this.m_member.InDifferentZone && this.m_profile);
				Color color = this.m_icon.color;
				color.a = ((this.m_hasDiscovered && this.m_meetsLevelRequirements) ? 1f : 0.5f);
				this.m_icon.color = color;
			}
		}

		// Token: 0x06002CBB RID: 11451 RVA: 0x0005F060 File Offset: 0x0005D260
		private void DiscoveryProgressionOnDiscoveryFound(DiscoveryProfile obj)
		{
			this.RefreshIcon();
		}

		// Token: 0x06002CBC RID: 11452 RVA: 0x0014B044 File Offset: 0x00149244
		private ITooltipParameter GetParameter()
		{
			if (this.m_member == null || !this.m_icon || !this.m_icon.enabled || !this.m_profile)
			{
				return null;
			}
			Color color = this.m_hasDiscovered ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			string arg = this.m_hasDiscovered ? "Discovered" : "Undiscovered";
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (string.IsNullOrEmpty(this.m_profile.DisplayName))
				{
					utf16ValueStringBuilder.Append("Ley Link");
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string, string>("{0} {1}", this.m_profile.DisplayName, "Ley Link");
				}
				utf16ValueStringBuilder.AppendFormat<string>("\n  Required Level: {0}", this.m_profile.GetRequiredLevelTooltipText());
				utf16ValueStringBuilder.AppendFormat<string, string>("\n  <color={0}>{1}</color>", color.ToHex(), arg);
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06002CBD RID: 11453 RVA: 0x0005F068 File Offset: 0x0005D268
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x06002CBE RID: 11454 RVA: 0x0005F076 File Offset: 0x0005D276
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x06002CBF RID: 11455 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002C6E RID: 11374
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04002C6F RID: 11375
		[SerializeField]
		private Image m_icon;

		// Token: 0x04002C70 RID: 11376
		private GroupMember m_member;

		// Token: 0x04002C71 RID: 11377
		private LeyLinkProfile m_profile;

		// Token: 0x04002C72 RID: 11378
		private bool m_hasDiscovered;

		// Token: 0x04002C73 RID: 11379
		private bool m_meetsLevelRequirements;
	}
}
