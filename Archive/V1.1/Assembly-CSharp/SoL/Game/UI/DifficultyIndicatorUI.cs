using System;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.Targeting;
using SoL.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200084A RID: 2122
	public class DifficultyIndicatorUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000E22 RID: 3618
		// (get) Token: 0x06003D27 RID: 15655 RVA: 0x00069683 File Offset: 0x00067883
		internal DifficultyRating GelDifficulty
		{
			get
			{
				return this.m_gelDifficulty;
			}
		}

		// Token: 0x17000E23 RID: 3619
		// (get) Token: 0x06003D28 RID: 15656 RVA: 0x0006968B File Offset: 0x0006788B
		internal ChallengeRating ChallengeRating
		{
			get
			{
				return this.m_challengeRating;
			}
		}

		// Token: 0x17000E24 RID: 3620
		// (get) Token: 0x06003D29 RID: 15657 RVA: 0x00069693 File Offset: 0x00067893
		internal bool GelIconActive
		{
			get
			{
				return this.m_gelIcon && this.m_gelIcon.isActiveAndEnabled && base.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x06003D2A RID: 15658 RVA: 0x00181C80 File Offset: 0x0017FE80
		public void RefreshIndicator(NameplateControllerUI controller)
		{
			ChallengeRating challengeRating = ChallengeRating.CR0;
			DifficultyRating difficulty = DifficultyRating.None;
			DifficultyRating gelDifficulty = DifficultyRating.None;
			this.m_targetType = TargetType.Defensive;
			if (controller && controller.Targetable != null)
			{
				challengeRating = controller.Targetable.GetChallengeRating();
				difficulty = controller.Targetable.GetDifficultyRating(false);
				gelDifficulty = controller.Targetable.GetDifficultyRating(true);
				this.m_targetType = controller.Targetable.Faction.GetPlayerTargetType();
			}
			this.m_challengeRating = challengeRating;
			this.m_difficulty = difficulty;
			this.m_gelDifficulty = gelDifficulty;
			this.RefreshIndicatorColor();
			this.RefreshChallengeRatingIcon(controller);
			this.RefreshEventIcon(controller);
		}

		// Token: 0x06003D2B RID: 15659 RVA: 0x00181D10 File Offset: 0x0017FF10
		private void RefreshChallengeRatingIcon(NameplateControllerUI controller)
		{
			switch (this.m_challengeRating)
			{
			case ChallengeRating.CR1:
				this.m_cr0.SetActive(false);
				this.m_cr1.SetActive(true);
				this.m_cr2.SetActive(false);
				this.m_cr3.SetActive(false);
				this.m_cr4.SetActive(false);
				this.m_crB.SetActive(false);
				return;
			case ChallengeRating.CR2:
				this.m_cr0.SetActive(false);
				this.m_cr1.SetActive(false);
				this.m_cr2.SetActive(true);
				this.m_cr3.SetActive(false);
				this.m_cr4.SetActive(false);
				this.m_crB.SetActive(false);
				return;
			case ChallengeRating.CR3:
				this.m_cr0.SetActive(false);
				this.m_cr1.SetActive(false);
				this.m_cr2.SetActive(false);
				this.m_cr3.SetActive(true);
				this.m_cr4.SetActive(false);
				this.m_crB.SetActive(false);
				return;
			case ChallengeRating.CR4:
				this.m_cr0.SetActive(false);
				this.m_cr1.SetActive(false);
				this.m_cr2.SetActive(false);
				this.m_cr3.SetActive(false);
				this.m_cr4.SetActive(true);
				this.m_crB.SetActive(false);
				return;
			case ChallengeRating.CRB:
				this.m_cr0.SetActive(false);
				this.m_cr1.SetActive(false);
				this.m_cr2.SetActive(false);
				this.m_cr3.SetActive(false);
				this.m_cr4.SetActive(false);
				this.m_crB.SetActive(true);
				return;
			}
			this.m_cr0.SetActive(this.m_targetType == TargetType.Offensive);
			this.m_cr1.SetActive(false);
			this.m_cr2.SetActive(false);
			this.m_cr3.SetActive(false);
			this.m_cr4.SetActive(false);
			this.m_crB.SetActive(false);
		}

		// Token: 0x06003D2C RID: 15660 RVA: 0x00181F10 File Offset: 0x00180110
		private void RefreshIndicatorColor()
		{
			Color color = Color.white;
			if (this.m_difficulty == DifficultyRating.None)
			{
				base.gameObject.SetActive(false);
				return;
			}
			color = GlobalSettings.Values.Npcs.GetDifficultyRatingColor(this.m_difficulty);
			for (int i = 0; i < this.m_images.Length; i++)
			{
				if (this.m_images[i])
				{
					this.m_images[i].color = color;
				}
			}
			this.RefreshGelIndicator();
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x06003D2D RID: 15661 RVA: 0x00181FA0 File Offset: 0x001801A0
		private void RefreshGelIndicator()
		{
			if (!this.m_gelIcon)
			{
				return;
			}
			if (!Options.GameOptions.ShowGelIndicators.Value || this.m_targetType != TargetType.Offensive)
			{
				this.m_gelIcon.enabled = false;
				return;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.AdventuringLevel < LocalPlayer.GameEntity.CharacterData.GroupedLevel && this.m_gelDifficulty != DifficultyRating.None)
			{
				this.m_gelIcon.color = GlobalSettings.Values.Npcs.GetDifficultyRatingColor(this.m_gelDifficulty);
				this.m_gelIcon.enabled = true;
				return;
			}
			this.m_gelIcon.enabled = false;
		}

		// Token: 0x06003D2E RID: 15662 RVA: 0x00182060 File Offset: 0x00180260
		private void RefreshEventIcon(NameplateControllerUI controller)
		{
			if (this.m_eventIcon)
			{
				bool enabled = controller && controller.Targetable != null && controller.Targetable.Entity && controller.Targetable.Entity.Type == GameEntityType.Npc && controller.Targetable.Entity.CharacterData && controller.Targetable.Entity.CharacterData.NpcInitData.BypassLevelDeltaCombatAdjustments;
				this.m_eventIcon.enabled = enabled;
			}
		}

		// Token: 0x06003D2F RID: 15663 RVA: 0x000696BC File Offset: 0x000678BC
		public void Deactivate()
		{
			base.gameObject.SetActive(false);
			if (this.m_gelIcon)
			{
				this.m_gelIcon.enabled = false;
			}
			if (this.m_eventIcon)
			{
				this.m_eventIcon.enabled = false;
			}
		}

		// Token: 0x06003D30 RID: 15664 RVA: 0x000696FC File Offset: 0x000678FC
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, GlobalSettings.Values.Npcs.GetDifficultyChallengeText(this.m_difficulty, this.m_challengeRating), false);
		}

		// Token: 0x17000E25 RID: 3621
		// (get) Token: 0x06003D31 RID: 15665 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E26 RID: 3622
		// (get) Token: 0x06003D32 RID: 15666 RVA: 0x00069725 File Offset: 0x00067925
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E27 RID: 3623
		// (get) Token: 0x06003D33 RID: 15667 RVA: 0x00069733 File Offset: 0x00067933
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003D35 RID: 15669 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003BFB RID: 15355
		[SerializeField]
		private Image[] m_images;

		// Token: 0x04003BFC RID: 15356
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003BFD RID: 15357
		[SerializeField]
		private GameObject m_cr0;

		// Token: 0x04003BFE RID: 15358
		[SerializeField]
		private GameObject m_cr1;

		// Token: 0x04003BFF RID: 15359
		[SerializeField]
		private GameObject m_cr2;

		// Token: 0x04003C00 RID: 15360
		[SerializeField]
		private GameObject m_cr3;

		// Token: 0x04003C01 RID: 15361
		[SerializeField]
		private GameObject m_cr4;

		// Token: 0x04003C02 RID: 15362
		[FormerlySerializedAs("m_crBoss")]
		[SerializeField]
		private GameObject m_crB;

		// Token: 0x04003C03 RID: 15363
		private DifficultyRating m_difficulty;

		// Token: 0x04003C04 RID: 15364
		private ChallengeRating m_challengeRating;

		// Token: 0x04003C05 RID: 15365
		private TargetType m_targetType = TargetType.Defensive;

		// Token: 0x04003C06 RID: 15366
		[SerializeField]
		private Image m_gelIcon;

		// Token: 0x04003C07 RID: 15367
		private DifficultyRating m_gelDifficulty;

		// Token: 0x04003C08 RID: 15368
		[SerializeField]
		private Image m_eventIcon;
	}
}
