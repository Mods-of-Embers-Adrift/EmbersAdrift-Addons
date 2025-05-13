using System;
using SoL.Game.Audio;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A32 RID: 2610
	public abstract class BaseArchetype : Identifiable, IArchetype
	{
		// Token: 0x170011F3 RID: 4595
		// (get) Token: 0x060050B2 RID: 20658 RVA: 0x00075FC2 File Offset: 0x000741C2
		public bool ExcludeFromExternalDatabase
		{
			get
			{
				return this.m_excludeFromInternalDatabase;
			}
		}

		// Token: 0x170011F4 RID: 4596
		// (get) Token: 0x060050B3 RID: 20659 RVA: 0x00075FCA File Offset: 0x000741CA
		public bool NpcOnly
		{
			get
			{
				return this.m_npcOnly;
			}
		}

		// Token: 0x170011F5 RID: 4597
		// (get) Token: 0x060050B4 RID: 20660 RVA: 0x00075FD2 File Offset: 0x000741D2
		public virtual string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		// Token: 0x170011F6 RID: 4598
		// (get) Token: 0x060050B5 RID: 20661 RVA: 0x00075FDA File Offset: 0x000741DA
		public virtual string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x170011F7 RID: 4599
		// (get) Token: 0x060050B6 RID: 20662 RVA: 0x00075FE2 File Offset: 0x000741E2
		public string SubHeaderText
		{
			get
			{
				return this.m_subHeaderText;
			}
		}

		// Token: 0x060050B7 RID: 20663 RVA: 0x000759FE File Offset: 0x00073BFE
		public virtual string GetModifiedDisplayName(ArchetypeInstance instance)
		{
			return this.DisplayName;
		}

		// Token: 0x060050B8 RID: 20664 RVA: 0x00075FD2 File Offset: 0x000741D2
		public virtual string GetModifiedDisplayName(BaseArchetype archetype)
		{
			return this.m_displayName;
		}

		// Token: 0x060050B9 RID: 20665 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual GameObject GetInstanceUIPrefabReference()
		{
			return null;
		}

		// Token: 0x060050BA RID: 20666 RVA: 0x00075FEA File Offset: 0x000741EA
		public virtual bool MatchesTextFilter(string filter)
		{
			return this.DisplayName.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x170011F8 RID: 4600
		// (get) Token: 0x060050BB RID: 20667 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual AudioClipCollection DragDropAudio
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170011F9 RID: 4601
		// (get) Token: 0x060050BC RID: 20668 RVA: 0x000522F9 File Offset: 0x000504F9
		public virtual Color IconTint
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x170011FA RID: 4602
		// (get) Token: 0x060050BD RID: 20669 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool LootRoll
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011FB RID: 4603
		// (get) Token: 0x060050BE RID: 20670 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual ArchetypeIconType IconShape
		{
			get
			{
				return ArchetypeIconType.Square;
			}
		}

		// Token: 0x170011FC RID: 4604
		// (get) Token: 0x060050BF RID: 20671 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ChangeFrameColor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011FD RID: 4605
		// (get) Token: 0x060050C0 RID: 20672 RVA: 0x00075FF9 File Offset: 0x000741F9
		public virtual Color FrameColor
		{
			get
			{
				return new Color(0.4627451f, 0.4745098f, 0.4862745f);
			}
		}

		// Token: 0x170011FE RID: 4606
		// (get) Token: 0x060050C1 RID: 20673 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool CanPlaceInGathering
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011FF RID: 4607
		// (get) Token: 0x060050C2 RID: 20674 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001200 RID: 4608
		// (get) Token: 0x060050C3 RID: 20675 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HasDynamicValues
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001201 RID: 4609
		// (get) Token: 0x060050C4 RID: 20676 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsReagent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001202 RID: 4610
		// (get) Token: 0x060050C5 RID: 20677 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsAugmentable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001203 RID: 4611
		// (get) Token: 0x060050C6 RID: 20678 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsWarlordSong
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001204 RID: 4612
		// (get) Token: 0x060050C7 RID: 20679 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ExcludeFromInspection
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x0007600F File Offset: 0x0007420F
		public virtual Color GetInstanceColor(ArchetypeInstance instance)
		{
			return this.IconTint;
		}

		// Token: 0x17001205 RID: 4613
		// (get) Token: 0x060050C9 RID: 20681 RVA: 0x00076017 File Offset: 0x00074217
		public virtual Sprite Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x060050CA RID: 20682 RVA: 0x0007601F File Offset: 0x0007421F
		public virtual ArchetypeInstance CreateNewInstance()
		{
			return ArchetypeInstanceExtensions.CreateNewInstance(this);
		}

		// Token: 0x17001206 RID: 4614
		// (get) Token: 0x060050CB RID: 20683 RVA: 0x00076027 File Offset: 0x00074227
		public virtual ArchetypeCategory Category { get; }

		// Token: 0x17001207 RID: 4615
		// (get) Token: 0x060050CC RID: 20684 RVA: 0x0007602F File Offset: 0x0007422F
		public virtual ItemCategory ItemCategory { get; }

		// Token: 0x060050CD RID: 20685 RVA: 0x001CDCF8 File Offset: 0x001CBEF8
		public bool TryGetItemCategoryColor(ItemCategory.ColorFlags colorType, out Color color)
		{
			color = Color.white;
			if (this.ItemCategory && this.ItemCategory.ColorFlag.HasBitFlag(colorType))
			{
				color = this.ItemCategory.Color;
				return true;
			}
			return false;
		}

		// Token: 0x060050CE RID: 20686 RVA: 0x00076037 File Offset: 0x00074237
		public bool TryGetItemCategoryDescription(out string description)
		{
			description = (this.ItemCategory ? this.ItemCategory.Description : string.Empty);
			return !string.IsNullOrEmpty(description);
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnInstanceCreated(ArchetypeInstance instance)
		{
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void FillAbilityTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, AlchemyPowerLevel alchemyPowerLevel)
		{
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x00076064 File Offset: 0x00074264
		public virtual BaseArchetype BuildDynamic(ArchetypeInstance instance)
		{
			throw new InvalidOperationException("Unable to build dynamic archetype: ya can't have HasDynamicValues return true and not override this method, ya dingbat!");
		}

		// Token: 0x0400485C RID: 18524
		private const string kDisplayGroup = "Display";

		// Token: 0x0400485D RID: 18525
		[TextArea]
		[SerializeField]
		private string m_internalNotes;

		// Token: 0x0400485E RID: 18526
		[SerializeField]
		private bool m_excludeFromInternalDatabase;

		// Token: 0x0400485F RID: 18527
		[SerializeField]
		private bool m_npcOnly;

		// Token: 0x04004860 RID: 18528
		[SerializeField]
		private string m_displayName;

		// Token: 0x04004861 RID: 18529
		[TextArea]
		[SerializeField]
		private string m_description;

		// Token: 0x04004862 RID: 18530
		[Tooltip("Adds line to left sub header - keep it short!")]
		[SerializeField]
		private string m_subHeaderText = string.Empty;

		// Token: 0x04004863 RID: 18531
		[SpritePreview]
		[SerializeField]
		private Sprite m_icon;
	}
}
