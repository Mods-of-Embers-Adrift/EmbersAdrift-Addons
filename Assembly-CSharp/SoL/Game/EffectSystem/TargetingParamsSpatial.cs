using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C28 RID: 3112
	[Serializable]
	public class TargetingParamsSpatial : TargetingParams
	{
		// Token: 0x17001708 RID: 5896
		// (get) Token: 0x0600600B RID: 24587 RVA: 0x0008092B File Offset: 0x0007EB2B
		private bool m_showAoeParameterSource
		{
			get
			{
				return this.m_targetType.IsAOE();
			}
		}

		// Token: 0x17001709 RID: 5897
		// (get) Token: 0x0600600C RID: 24588 RVA: 0x00080A39 File Offset: 0x0007EC39
		protected override bool m_showTargetDistanceAngle
		{
			get
			{
				return base.m_showTargetDistanceAngle && this.m_spatialSource == SpatialSource.Custom;
			}
		}

		// Token: 0x1700170A RID: 5898
		// (get) Token: 0x0600600D RID: 24589 RVA: 0x00080A4E File Offset: 0x0007EC4E
		protected override bool m_showAoeRadius
		{
			get
			{
				return base.m_showAoeRadius && this.m_aoeParamSource == SpatialSource.Custom;
			}
		}

		// Token: 0x1700170B RID: 5899
		// (get) Token: 0x0600600E RID: 24590 RVA: 0x00080A63 File Offset: 0x0007EC63
		protected override bool m_showAoeAngle
		{
			get
			{
				return base.m_showAoeAngle && this.m_aoeParamSource == SpatialSource.Custom;
			}
		}

		// Token: 0x0600600F RID: 24591 RVA: 0x001FBCDC File Offset: 0x001F9EDC
		private WeaponItem GetWeaponItemForSpatialSource(SpatialSource source, GameEntity entity, IHandHeldItems handHeldItems)
		{
			WeaponItem result = null;
			ArchetypeInstance archetypeInstance2;
			if (source != SpatialSource.MainHandWeapon)
			{
				if (source == SpatialSource.OffHandWeapon)
				{
					if (handHeldItems != null && handHeldItems.OffHand.WeaponItem)
					{
						result = handHeldItems.OffHand.WeaponItem;
					}
					else if (entity)
					{
						ArchetypeInstance archetypeInstance;
						entity.TryGetHandheldItem_OffHandAsType(out archetypeInstance, out result);
					}
				}
			}
			else if (handHeldItems != null && handHeldItems.MainHand.WeaponItem)
			{
				result = handHeldItems.MainHand.WeaponItem;
			}
			else if (entity && entity.TryGetHandheldItem_MainHandAsType(out archetypeInstance2, out result))
			{
			}
			return result;
		}

		// Token: 0x06006010 RID: 24592 RVA: 0x001FBD68 File Offset: 0x001F9F68
		public override MinMaxFloatRange GetTargetDistance(GameEntity entity, IHandHeldItems handHeldItems)
		{
			WeaponItem weaponItemForSpatialSource = this.GetWeaponItemForSpatialSource(this.m_spatialSource, entity, handHeldItems);
			if (!weaponItemForSpatialSource)
			{
				return base.GetTargetDistance(entity, handHeldItems);
			}
			return weaponItemForSpatialSource.GetWeaponDistance();
		}

		// Token: 0x06006011 RID: 24593 RVA: 0x001FBD9C File Offset: 0x001F9F9C
		public override float GetTargetAngle(GameEntity entity, IHandHeldItems handHeldItems)
		{
			WeaponItem weaponItemForSpatialSource = this.GetWeaponItemForSpatialSource(this.m_spatialSource, entity, handHeldItems);
			if (!weaponItemForSpatialSource)
			{
				return base.GetTargetAngle(entity, handHeldItems);
			}
			return weaponItemForSpatialSource.GetWeaponAngle();
		}

		// Token: 0x06006012 RID: 24594 RVA: 0x001FBDD0 File Offset: 0x001F9FD0
		public override float GetAoeRadius(GameEntity entity, IHandHeldItems handHeldItems)
		{
			WeaponItem weaponItemForSpatialSource = this.GetWeaponItemForSpatialSource(this.m_aoeParamSource, entity, handHeldItems);
			if (!weaponItemForSpatialSource)
			{
				return base.GetAoeRadius(entity, handHeldItems);
			}
			return weaponItemForSpatialSource.GetWeaponAoeRadius();
		}

		// Token: 0x06006013 RID: 24595 RVA: 0x001FBE04 File Offset: 0x001FA004
		public override float GetAoeAngle(GameEntity entity, IHandHeldItems handHeldItems)
		{
			WeaponItem weaponItemForSpatialSource = this.GetWeaponItemForSpatialSource(this.m_aoeParamSource, entity, handHeldItems);
			if (!weaponItemForSpatialSource)
			{
				return base.GetAoeAngle(entity, handHeldItems);
			}
			return weaponItemForSpatialSource.GetWeaponAoeAngle();
		}

		// Token: 0x040052CD RID: 21197
		[SerializeField]
		private SpatialSource m_spatialSource;

		// Token: 0x040052CE RID: 21198
		[SerializeField]
		private SpatialSource m_aoeParamSource;
	}
}
