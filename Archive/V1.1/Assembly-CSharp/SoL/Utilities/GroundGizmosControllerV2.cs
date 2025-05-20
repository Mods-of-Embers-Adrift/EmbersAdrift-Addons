using System;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x0200028D RID: 653
	public class GroundGizmosControllerV2 : MonoBehaviour
	{
		// Token: 0x06001405 RID: 5125 RVA: 0x00050055 File Offset: 0x0004E255
		private void Awake()
		{
			if (!this.m_projector)
			{
				base.enabled = false;
				return;
			}
			this.m_projector.enabled = false;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x000F8D58 File Offset: 0x000F6F58
		private void Update()
		{
			if (!LocalPlayer.GameEntity || ClientGameManager.InputManager == null || !ClientGameManager.InputManager.HoldingAlt || ArchetypeInstanceUI.HoveredInstance == null)
			{
				this.m_instance = null;
				this.ToggleProjector(false);
				return;
			}
			ArchetypeInstance instance = this.m_instance;
			this.m_instance = ArchetypeInstanceUI.HoveredInstance;
			bool flag = LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			if (this.m_instance == null)
			{
				this.ToggleProjector(false);
				return;
			}
			if (instance != this.m_instance || flag != this.m_mainHandSecondaryActive)
			{
				TargetingParams targetingParams = null;
				ICombatEffectSource combatEffectSource;
				if (this.m_instance.Archetype.TryGetAsType(out combatEffectSource))
				{
					int associatedLevelInteger = this.m_instance.GetAssociatedLevelInteger(LocalPlayer.GameEntity);
					targetingParams = combatEffectSource.GetTargetingParams((float)associatedLevelInteger, AlchemyPowerLevel.None);
				}
				WeaponItem weaponItem;
				if (targetingParams != null && targetingParams.TargetType != EffectTargetType.Self)
				{
					if (targetingParams.TargetType.IsAOE())
					{
						this.m_distanceReqs = new MinMaxFloatRange?(new MinMaxFloatRange(0f, targetingParams.GetAoeRadius(LocalPlayer.GameEntity, null)));
						this.m_angleReqs = new float?(targetingParams.TargetType.CheckAngle() ? targetingParams.GetAoeAngle(LocalPlayer.GameEntity, null) : 360f);
					}
					else
					{
						this.m_distanceReqs = new MinMaxFloatRange?(targetingParams.GetTargetDistance(LocalPlayer.GameEntity, null));
						this.m_angleReqs = new float?(targetingParams.GetTargetAngle(LocalPlayer.GameEntity, null));
					}
				}
				else if (this.m_instance.Archetype.TryGetAsType(out weaponItem) && weaponItem.ShowGroundGizmos)
				{
					this.m_distanceReqs = new MinMaxFloatRange?(weaponItem.GetWeaponDistance());
					this.m_angleReqs = new float?(weaponItem.GetWeaponAngle());
				}
				else
				{
					this.m_distanceReqs = null;
					this.m_angleReqs = null;
				}
				if (this.m_distanceReqs == null || this.m_angleReqs == null)
				{
					this.ToggleProjector(false);
					return;
				}
				this.m_projector.size = new Vector3(this.m_distanceReqs.Value.Max * 2f, this.m_distanceReqs.Value.Max * 2f, this.m_projector.size.z);
				float value = (this.m_distanceReqs.Value.Min > 0f) ? (this.m_distanceReqs.Value.Min / this.m_distanceReqs.Value.Max) : 0f;
				this.m_projector.material.SetFloat(GroundGizmosControllerV2.kInnerRadiusID, value);
				this.m_projector.material.SetFloat(GroundGizmosControllerV2.kAngleID, this.m_angleReqs.Value);
				this.ToggleProjector(true);
			}
			this.m_mainHandSecondaryActive = flag;
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x00050078 File Offset: 0x0004E278
		private void ToggleProjector(bool isEnabled)
		{
			if (this.m_projector)
			{
				this.m_projector.enabled = isEnabled;
			}
		}

		// Token: 0x04001C49 RID: 7241
		[SerializeField]
		private DecalProjector m_projector;

		// Token: 0x04001C4A RID: 7242
		private ArchetypeInstance m_instance;

		// Token: 0x04001C4B RID: 7243
		private MinMaxFloatRange? m_distanceReqs;

		// Token: 0x04001C4C RID: 7244
		private float? m_angleReqs;

		// Token: 0x04001C4D RID: 7245
		private bool m_mainHandSecondaryActive;

		// Token: 0x04001C4E RID: 7246
		private static readonly int kInnerRadiusID = Shader.PropertyToID("_InnerRadius");

		// Token: 0x04001C4F RID: 7247
		private static readonly int kAngleID = Shader.PropertyToID("_Angle");
	}
}
