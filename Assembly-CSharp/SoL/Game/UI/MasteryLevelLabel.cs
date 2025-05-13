using System;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200086A RID: 2154
	public class MasteryLevelLabel : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003E4B RID: 15947 RVA: 0x0006A21A File Offset: 0x0006841A
		private void Start()
		{
			if (LocalPlayer.NetworkEntity.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.NetworkEntity.OnStartLocalClient += this.OnStartLocalClient;
		}

		// Token: 0x06003E4C RID: 15948 RVA: 0x00184E24 File Offset: 0x00183024
		private void OnDestroy()
		{
			LocalClientSkillsController.MasteryLevelChangedEvent -= this.MasteryLevelChanged;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
			}
		}

		// Token: 0x06003E4D RID: 15949 RVA: 0x0006A245 File Offset: 0x00068445
		private void OnStartLocalClient()
		{
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.OnStartLocalClient;
			this.Init();
		}

		// Token: 0x06003E4E RID: 15950 RVA: 0x0006A263 File Offset: 0x00068463
		private void Init()
		{
			LocalClientSkillsController.MasteryLevelChangedEvent += this.MasteryLevelChanged;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
			this.RefreshMasteryLevelLabel(true);
		}

		// Token: 0x06003E4F RID: 15951 RVA: 0x0006A29D File Offset: 0x0006849D
		private void MasteryLevelChanged(ArchetypeInstance obj)
		{
			this.RefreshMasteryLevelLabel(false);
		}

		// Token: 0x06003E50 RID: 15952 RVA: 0x0006A29D File Offset: 0x0006849D
		private void MasteriesOnContentsChanged()
		{
			this.RefreshMasteryLevelLabel(false);
		}

		// Token: 0x06003E51 RID: 15953 RVA: 0x00184E8C File Offset: 0x0018308C
		private void RefreshMasteryLevelLabel(bool initial = false)
		{
			int num = 0;
			int num2 = 0;
			foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
			{
				MasteryArchetype masteryArchetype;
				if (archetypeInstance.Archetype.TryGetAsType(out masteryArchetype) && this.m_masterySphere == masteryArchetype.Type.GetMasterySphere())
				{
					num2 += archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity);
					num++;
				}
			}
			this.m_label.text = num2.ToString();
			int maximumForMasteryLevel = MasteryArchetype.GetMaximumForMasteryLevel(this.m_masterySphere, num2);
			bool flag = num < maximumForMasteryLevel;
			bool flag2 = num >= this.m_previousCount && maximumForMasteryLevel > this.m_previousMaxCount;
			if (!initial && flag2 && flag)
			{
				string text = string.Concat(new string[]
				{
					"You can now obtain an additional ",
					this.m_masterySphere.ToString(),
					" sphere mastery (",
					maximumForMasteryLevel.ToString(),
					" total)."
				});
				CenterScreenAnnouncementOptions opts = new CenterScreenAnnouncementOptions
				{
					Title = "Hint!",
					Text = text,
					TimeShown = 8f,
					MessageType = MessageType.Skills
				};
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(opts);
			}
			this.m_previousCount = num;
			this.m_previousMaxCount = maximumForMasteryLevel;
		}

		// Token: 0x06003E52 RID: 15954 RVA: 0x00184FFC File Offset: 0x001831FC
		private ITooltipParameter GetTooltipParameter()
		{
			BaseTooltip.Sb.Clear();
			BaseTooltip.Sb.AppendLine(string.Concat(new string[]
			{
				"You are capable of learning ",
				this.m_previousMaxCount.ToString(),
				" ",
				this.m_masterySphere.ToString(),
				" sphere masteries."
			}));
			if (this.m_previousCount < this.m_masterySphere.GetMaximumForSphere())
			{
				int requiredMasteryTotalToLearn = MasteryArchetype.GetRequiredMasteryTotalToLearn(this.m_previousMaxCount);
				BaseTooltip.Sb.AppendLine(string.Concat(new string[]
				{
					"<size=75%>At a total ",
					this.m_masterySphere.ToString(),
					" sphere mastery level of ",
					requiredMasteryTotalToLearn.ToString(),
					" you can learn an additional ",
					this.m_masterySphere.ToString(),
					" mastery</size>"
				}));
			}
			else
			{
				BaseTooltip.Sb.AppendLine("<size=75%>You can learn no additional " + this.m_masterySphere.ToString() + " sphere masteries.</size>");
			}
			return new ObjectTextTooltipParameter(this, BaseTooltip.Sb.ToString(), false);
		}

		// Token: 0x17000E60 RID: 3680
		// (get) Token: 0x06003E53 RID: 15955 RVA: 0x0006A2A6 File Offset: 0x000684A6
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E61 RID: 3681
		// (get) Token: 0x06003E54 RID: 15956 RVA: 0x0006A2B4 File Offset: 0x000684B4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E62 RID: 3682
		// (get) Token: 0x06003E55 RID: 15957 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003E57 RID: 15959 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C8A RID: 15498
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003C8B RID: 15499
		[SerializeField]
		private MasterySphere m_masterySphere;

		// Token: 0x04003C8C RID: 15500
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C8D RID: 15501
		private int m_previousMaxCount;

		// Token: 0x04003C8E RID: 15502
		private int m_previousCount;
	}
}
