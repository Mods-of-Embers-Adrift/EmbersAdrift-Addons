using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000570 RID: 1392
	public class EmberStoneColorLerper : GameEntityComponent
	{
		// Token: 0x06002AE8 RID: 10984 RVA: 0x0005DBB6 File Offset: 0x0005BDB6
		private void Awake()
		{
			if (this.m_controller)
			{
				this.m_controller.EmberStoneColorExternallyControlled = true;
			}
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x0005DBD1 File Offset: 0x0005BDD1
		private void Start()
		{
			if (base.GameEntity && base.GameEntity.SkillsController)
			{
				base.GameEntity.SkillsController.PendingExecutionChanged += this.SkillsControllerOnPendingExecutionChanged;
			}
		}

		// Token: 0x06002AEA RID: 10986 RVA: 0x0005DC0E File Offset: 0x0005BE0E
		private void OnDestroy()
		{
			if (base.GameEntity && base.GameEntity.SkillsController)
			{
				base.GameEntity.SkillsController.PendingExecutionChanged -= this.SkillsControllerOnPendingExecutionChanged;
			}
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x00144F78 File Offset: 0x00143178
		private void Update()
		{
			if (this.m_controller && this.m_controller.EmberStoneItem && this.m_controller.EmberStoneItem.RendererToAlter && this.m_controller.EmberStoneItem.RendererToAlter.material)
			{
				if (this.m_emissiveProperty == null)
				{
					this.m_emissiveProperty = new EmissiveColorMaterialProperty();
					this.m_emissiveProperty.Init(this.m_controller.EmberStoneItem.RendererToAlter.material);
				}
				else if (this.m_emissiveProperty.Material != this.m_controller.EmberStoneItem.RendererToAlter.material)
				{
					this.m_emissiveProperty.Init(this.m_controller.EmberStoneItem.RendererToAlter.material);
				}
				Color currentValue = this.m_emissiveProperty.CurrentValue;
				Color color = (this.m_alchemyColor != null) ? this.m_alchemyColor.Value : this.m_controller.EmberStoneFillColor;
				if (currentValue != color)
				{
					Color value = Color.Lerp(currentValue, color, Time.deltaTime * GlobalSettings.Values.Ashen.GlowAnimationSpeed);
					this.m_emissiveProperty.SetValue(value);
				}
			}
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x001450C4 File Offset: 0x001432C4
		private void SkillsControllerOnPendingExecutionChanged(SkillsController.PendingExecution obj)
		{
			if (obj == null || obj.AlchemyPowerLevel == AlchemyPowerLevel.None || !base.GameEntity || !base.GameEntity.SkillsController)
			{
				if (this.m_alchemyColor != null)
				{
					this.m_alchemyColor = null;
				}
				return;
			}
			SkillsController.PendingExecution.PendingStatus status = obj.Status;
			if (status - SkillsController.PendingExecution.PendingStatus.CompleteReceived <= 3)
			{
				this.m_alchemyColor = null;
				return;
			}
			this.m_alchemyColor = new Color?(base.GameEntity.CharacterData.EmberStoneFillLevel.GetEmissiveColor(obj.AlchemyPowerLevel));
		}

		// Token: 0x04002B29 RID: 11049
		internal const float kDefaultIntensity = 2f;

		// Token: 0x04002B2A RID: 11050
		[SerializeField]
		private HandheldMountController m_controller;

		// Token: 0x04002B2B RID: 11051
		private EmissiveColorMaterialProperty m_emissiveProperty;

		// Token: 0x04002B2C RID: 11052
		private Color? m_alchemyColor;
	}
}
