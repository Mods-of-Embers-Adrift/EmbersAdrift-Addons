using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.UI.Skills;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200084D RID: 2125
	public class ActionBarSlotUI : ContainerSlotUI, IBindingLabel
	{
		// Token: 0x17000E28 RID: 3624
		// (get) Token: 0x06003D42 RID: 15682 RVA: 0x000697DF File Offset: 0x000679DF
		internal Image OverlayFrame
		{
			get
			{
				return this.m_overlayFrame;
			}
		}

		// Token: 0x06003D43 RID: 15683 RVA: 0x000697E7 File Offset: 0x000679E7
		private void Awake()
		{
			AlchemySelectionUI.AlchemyLevelChanged += this.RefreshAlchemyHighlight;
		}

		// Token: 0x06003D44 RID: 15684 RVA: 0x000697FA File Offset: 0x000679FA
		private void OnDestroy()
		{
			BindingLabels.DeregisterBinding(this);
			AlchemySelectionUI.AlchemyLevelChanged -= this.RefreshAlchemyHighlight;
		}

		// Token: 0x17000E29 RID: 3625
		// (get) Token: 0x06003D45 RID: 15685 RVA: 0x0004479C File Offset: 0x0004299C
		BindingType IBindingLabel.Type
		{
			get
			{
				return BindingType.Ability;
			}
		}

		// Token: 0x17000E2A RID: 3626
		// (get) Token: 0x06003D46 RID: 15686 RVA: 0x00069813 File Offset: 0x00067A13
		int IBindingLabel.Index
		{
			get
			{
				return this.m_actionBarIndex;
			}
		}

		// Token: 0x17000E2B RID: 3627
		// (get) Token: 0x06003D47 RID: 15687 RVA: 0x0006981B File Offset: 0x00067A1B
		TextMeshProUGUI IBindingLabel.Label
		{
			get
			{
				return this.m_label;
			}
		}

		// Token: 0x17000E2C RID: 3628
		// (get) Token: 0x06003D48 RID: 15688 RVA: 0x00049FFA File Offset: 0x000481FA
		string IBindingLabel.FormattedString
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003D49 RID: 15689 RVA: 0x00069823 File Offset: 0x00067A23
		public override void Initialize(IContainerUI containerUI, int index)
		{
			base.Initialize(containerUI, index);
			this.m_actionBarIndex = index;
			BindingLabels.RegisterBinding(this, false);
		}

		// Token: 0x06003D4A RID: 15690 RVA: 0x0018227C File Offset: 0x0018047C
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			base.InstanceAdded(instance);
			IExecutable executable;
			this.m_executable = ((this.m_instance != null && this.m_instance.Archetype && this.m_instance.Archetype.TryGetAsType(out executable)) ? executable : null);
			if (this.m_instance != null && this.m_instance.AbilityData != null)
			{
				this.m_instance.AbilityData.AlchemyCooldownStatusChanged += this.RefreshAlchemyHighlight;
			}
			this.RefreshAlchemyHighlight();
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x00182300 File Offset: 0x00180500
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			if (this.m_instance != null && this.m_instance.AbilityData != null)
			{
				this.m_instance.AbilityData.AlchemyCooldownStatusChanged -= this.RefreshAlchemyHighlight;
			}
			base.InstanceRemoved(instance);
			this.m_executable = null;
			this.RefreshAlchemyHighlight();
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x00182354 File Offset: 0x00180554
		private void RefreshAlchemyHighlight()
		{
			if (this.m_alchemyHighlights == null)
			{
				return;
			}
			bool enabled = false;
			Color color = Color.white;
			if (AlchemySelectionUI.LevelFlags != AlchemyPowerLevelFlags.None && this.m_executable != null && this.m_instance != null && this.m_instance.AbilityData != null && LocalPlayer.GameEntity)
			{
				float associatedLevel = this.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity);
				AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None;
				if (AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.II) && !this.m_instance.AbilityData.Cooldown_AlchemyII.Active && AlchemyExtensions.AlchemyPowerLevelAvailable(LocalPlayer.GameEntity, this.m_instance, AlchemyPowerLevel.II) && this.m_executable.HasValidAlchemyOverride(associatedLevel, AlchemyPowerLevel.II))
				{
					alchemyPowerLevel = AlchemyPowerLevel.II;
				}
				else if (AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.I) && !this.m_instance.AbilityData.Cooldown_AlchemyI.Active && AlchemyExtensions.AlchemyPowerLevelAvailable(LocalPlayer.GameEntity, this.m_instance, AlchemyPowerLevel.I) && this.m_executable.HasValidAlchemyOverride(associatedLevel, AlchemyPowerLevel.I))
				{
					alchemyPowerLevel = AlchemyPowerLevel.I;
				}
				if (alchemyPowerLevel != AlchemyPowerLevel.None)
				{
					enabled = true;
					color = GlobalSettings.Values.Ashen.GetUIHighlightColor(alchemyPowerLevel);
				}
				if (this.m_pulser)
				{
					this.m_pulser.PowerLevel = alchemyPowerLevel;
				}
			}
			for (int i = 0; i < this.m_alchemyHighlights.Length; i++)
			{
				if (this.m_alchemyHighlights[i])
				{
					this.m_alchemyHighlights[i].color = color;
					this.m_alchemyHighlights[i].enabled = enabled;
				}
			}
		}

		// Token: 0x04003C0C RID: 15372
		[FormerlySerializedAs("m_numberText")]
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003C0D RID: 15373
		[SerializeField]
		protected Image m_overlayFrame;

		// Token: 0x04003C0E RID: 15374
		[SerializeField]
		private AlchemyPulser m_pulser;

		// Token: 0x04003C0F RID: 15375
		[SerializeField]
		private Image[] m_alchemyHighlights;

		// Token: 0x04003C10 RID: 15376
		private int m_actionBarIndex = -1;

		// Token: 0x04003C11 RID: 15377
		private IExecutable m_executable;
	}
}
