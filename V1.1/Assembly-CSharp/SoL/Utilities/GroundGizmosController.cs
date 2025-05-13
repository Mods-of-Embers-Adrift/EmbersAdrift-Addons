using System;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Utilities
{
	// Token: 0x0200028C RID: 652
	public class GroundGizmosController : MonoBehaviour
	{
		// Token: 0x060013FD RID: 5117 RVA: 0x0004FFDE File Offset: 0x0004E1DE
		private void Awake()
		{
			this.DisableImages();
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x000F8AA0 File Offset: 0x000F6CA0
		private void Update()
		{
			if (!LocalPlayer.GameEntity || ClientGameManager.InputManager == null || !ClientGameManager.InputManager.HoldingAlt || ArchetypeInstanceUI.HoveredInstance == null)
			{
				this.m_instance = null;
				this.DisableImages();
				return;
			}
			ArchetypeInstance instance = this.m_instance;
			this.m_instance = ArchetypeInstanceUI.HoveredInstance;
			if (this.m_instance == null)
			{
				this.DisableImages();
				return;
			}
			if (instance != this.m_instance)
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
				this.UpdateGizmoImages();
			}
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x000F8C40 File Offset: 0x000F6E40
		private void UpdateGizmoImages()
		{
			if (this.m_distanceReqs == null || this.m_angleReqs == null)
			{
				this.DisableImages();
				return;
			}
			float fillAmount = Mathf.Clamp(this.m_angleReqs.Value / 360f, 0f, 360f);
			this.UpdateImage(this.m_primary, this.m_distanceReqs.Value.Max, fillAmount);
			if (this.m_distanceReqs != null && this.m_distanceReqs.Value.Min > 0f)
			{
				this.UpdateImage(this.m_secondary, this.m_distanceReqs.Value.Min, fillAmount);
				return;
			}
			this.DisableImage(this.m_secondary);
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0004FFE6 File Offset: 0x0004E1E6
		private void UpdateImage(Image img, float scale, float fillAmount)
		{
			if (!img)
			{
				return;
			}
			img.rectTransform.localScale = Vector3.one * (scale * 2f);
			img.fillAmount = fillAmount;
			this.RotateImageForward(img);
			img.enabled = true;
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x000F8D04 File Offset: 0x000F6F04
		private void RotateImageForward(Image img)
		{
			if (!img)
			{
				return;
			}
			float num = img.fillAmount * 360f;
			float z = 180f + num * 0.5f;
			img.rectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, z));
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x00050022 File Offset: 0x0004E222
		private void DisableImages()
		{
			this.DisableImage(this.m_primary);
			this.DisableImage(this.m_secondary);
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x0005003C File Offset: 0x0004E23C
		private void DisableImage(Image img)
		{
			if (img && img.enabled)
			{
				img.enabled = false;
			}
		}

		// Token: 0x04001C44 RID: 7236
		[SerializeField]
		private Image m_primary;

		// Token: 0x04001C45 RID: 7237
		[SerializeField]
		private Image m_secondary;

		// Token: 0x04001C46 RID: 7238
		private ArchetypeInstance m_instance;

		// Token: 0x04001C47 RID: 7239
		private MinMaxFloatRange? m_distanceReqs;

		// Token: 0x04001C48 RID: 7240
		private float? m_angleReqs;
	}
}
