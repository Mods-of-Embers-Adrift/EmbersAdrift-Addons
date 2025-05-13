using System;
using System.Collections.Generic;
using System.Linq;
using SoL.GameCamera;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x02000654 RID: 1620
	internal class PlayerTabTargetGroup
	{
		// Token: 0x06003277 RID: 12919 RVA: 0x0015FD94 File Offset: 0x0015DF94
		public PlayerTabTargetGroup(PlayerTargetController controller, PlayerTarget playerTarget)
		{
			this.m_entity = controller.GameEntity;
			this.m_playerTarget = playerTarget;
			this.m_targetType = playerTarget.TargetType;
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x0015FDE8 File Offset: 0x0015DFE8
		internal ITargetable GetTabTarget(bool reverseOrder)
		{
			bool considerFrustum = CameraManager.ActiveType != ActiveCameraTypes.FirstPerson;
			DateTime utcNow = DateTime.UtcNow;
			this.m_potentialTargets.Clear();
			if ((utcNow - this.m_lastTabTarget).TotalSeconds > 2.0)
			{
				this.m_tabTargets.Clear();
			}
			this.m_lastTabTarget = utcNow;
			for (int i = 0; i < this.m_tabTargets.Count; i++)
			{
				if (!this.m_tabTargets[i].Target || this.m_tabTargets[i].Target.Vitals.GetCurrentHealthState() != HealthState.Alive)
				{
					this.m_tabTargets.RemoveAt(i);
					i--;
				}
				else if (!this.HasLOS(this.m_tabTargets[i].Target, considerFrustum))
				{
					this.m_tabTargets.RemoveAt(i);
					i--;
				}
				else if (this.GetRelativePos(this.m_tabTargets[i].Target).z < 0f)
				{
					this.m_tabTargets.RemoveAt(i);
					i--;
				}
				else
				{
					float sqrMagnitude = (this.m_entity.gameObject.transform.position - this.m_tabTargets[i].Target.gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude > this.m_entity.Vitals.MaxTargetDistanceSqr)
					{
						this.m_tabTargets.RemoveAt(i);
						i--;
					}
					else if (this.GetAngleTo(this.m_tabTargets[i].Target) > 45f)
					{
						this.m_tabTargets.RemoveAt(i);
						i--;
					}
					else
					{
						PlayerTabTargetGroup.TabTarget tabTarget = this.m_tabTargets[i];
						tabTarget.SqrDistance = sqrMagnitude;
						this.m_tabTargets[i] = tabTarget;
						this.m_potentialTargets.Add(tabTarget.Target);
					}
				}
			}
			Collider[] colliders = Hits.Colliders100;
			int num = Physics.OverlapSphereNonAlloc(this.m_entity.gameObject.transform.position, this.m_entity.Vitals.MaxTargetDistance, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int j = 0; j < num; j++)
			{
				GameEntity gameEntity;
				if (colliders[j].enabled && DetectionCollider.TryGetEntityForCollider(colliders[j], out gameEntity) && gameEntity && gameEntity.Vitals && gameEntity.CharacterData && !(gameEntity == LocalPlayer.GameEntity))
				{
					if (this.m_targetType == TargetType.Offensive && gameEntity.Type == GameEntityType.Player)
					{
						if (!gameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp) || gameEntity == LocalPlayer.GameEntity || !LocalPlayer.IsPvp)
						{
							goto IL_402;
						}
						if (ClientGroupManager.IsMyGroup(gameEntity.CharacterData.GroupId))
						{
							goto IL_402;
						}
					}
					else if (gameEntity.CharacterData.Faction.GetPlayerTargetType() != this.m_targetType)
					{
						goto IL_402;
					}
					if (gameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive && !this.m_potentialTargets.Contains(gameEntity) && this.HasLOS(gameEntity, considerFrustum) && this.GetRelativePos(gameEntity).z >= 0f && this.GetAngleTo(gameEntity) <= 45f)
					{
						PlayerTabTargetGroup.TabTarget item = new PlayerTabTargetGroup.TabTarget
						{
							Target = gameEntity,
							SqrDistance = (this.m_entity.gameObject.transform.position - gameEntity.gameObject.transform.position).sqrMagnitude,
							LastSelection = DateTime.MinValue
						};
						this.m_tabTargets.Add(item);
						this.m_potentialTargets.Add(gameEntity);
					}
				}
				IL_402:;
			}
			ITargetable result = null;
			if (this.m_tabTargets.Count == 1 && !Options.GameOptions.DeselectSingleTargetOnTab.Value)
			{
				PlayerTabTargetGroup.TabTarget tabTarget2 = this.m_tabTargets[0];
				tabTarget2.LastSelection = utcNow;
				this.m_tabTargets[0] = tabTarget2;
				result = tabTarget2.Target.Targetable;
			}
			else if (this.m_tabTargets.Count > 0)
			{
				List<PlayerTabTargetGroup.TabTarget> tabTargets;
				if (!reverseOrder)
				{
					tabTargets = (from a in this.m_tabTargets
					orderby a.LastSelection, a.SqrDistance
					select a).ToList<PlayerTabTargetGroup.TabTarget>();
				}
				else
				{
					tabTargets = (from a in this.m_tabTargets
					orderby a.LastSelection descending, a.SqrDistance
					select a).ToList<PlayerTabTargetGroup.TabTarget>();
				}
				this.m_tabTargets = tabTargets;
				for (int k = 0; k < this.m_tabTargets.Count; k++)
				{
					if (this.m_tabTargets[k].Target.Targetable != this.m_playerTarget.Target)
					{
						PlayerTabTargetGroup.TabTarget tabTarget3 = this.m_tabTargets[k];
						tabTarget3.LastSelection = utcNow;
						this.m_tabTargets[k] = tabTarget3;
						result = tabTarget3.Target.Targetable;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x00160394 File Offset: 0x0015E594
		private bool HasLOS(GameEntity target, bool considerFrustum)
		{
			Camera mainCamera = ClientGameManager.MainCamera;
			if (!mainCamera)
			{
				return false;
			}
			if (considerFrustum)
			{
				Vector3 vector = mainCamera.WorldToViewportPoint(target.gameObject.transform.position);
				if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f || vector.z < 0f)
				{
					return false;
				}
			}
			return LineOfSight.CameraHasLineOfSight(mainCamera, target);
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x00062C67 File Offset: 0x00060E67
		private Vector3 GetRelativePos(GameEntity entity)
		{
			return ClientGameManager.MainCamera.gameObject.transform.InverseTransformPoint(entity.gameObject.transform.position);
		}

		// Token: 0x0600327B RID: 12923 RVA: 0x00062C8D File Offset: 0x00060E8D
		private float GetAngleTo(GameEntity entity)
		{
			return ClientGameManager.MainCamera.gameObject.AngleTo(entity.gameObject, true);
		}

		// Token: 0x040030D9 RID: 12505
		private const float kTabTargetExpiration = 2f;

		// Token: 0x040030DA RID: 12506
		internal const float kTabTargetAngle = 45f;

		// Token: 0x040030DB RID: 12507
		private readonly GameEntity m_entity;

		// Token: 0x040030DC RID: 12508
		private readonly PlayerTarget m_playerTarget;

		// Token: 0x040030DD RID: 12509
		private readonly TargetType m_targetType;

		// Token: 0x040030DE RID: 12510
		private readonly HashSet<GameEntity> m_potentialTargets = new HashSet<GameEntity>();

		// Token: 0x040030DF RID: 12511
		private List<PlayerTabTargetGroup.TabTarget> m_tabTargets = new List<PlayerTabTargetGroup.TabTarget>();

		// Token: 0x040030E0 RID: 12512
		private DateTime m_lastTabTarget = DateTime.MinValue;

		// Token: 0x02000655 RID: 1621
		private struct TabTarget
		{
			// Token: 0x040030E1 RID: 12513
			public GameEntity Target;

			// Token: 0x040030E2 RID: 12514
			public float SqrDistance;

			// Token: 0x040030E3 RID: 12515
			public DateTime LastSelection;
		}
	}
}
