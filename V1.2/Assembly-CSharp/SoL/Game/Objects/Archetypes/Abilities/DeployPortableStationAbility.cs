using System;
using Cysharp.Text;
using ENet;
using SoL.Game.Animation;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF0 RID: 2800
	public class DeployPortableStationAbility : DynamicAbility
	{
		// Token: 0x06005679 RID: 22137 RVA: 0x00079A8C File Offset: 0x00077C8C
		public override string GetInstanceId()
		{
			return base.Id;
		}

		// Token: 0x17001426 RID: 5158
		// (get) Token: 0x0600567A RID: 22138 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool CreateInstanceUI
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600567B RID: 22139 RVA: 0x001E1128 File Offset: 0x001DF328
		protected override bool PreExecution(ExecutionCache executionCache, bool initial)
		{
			if (!base.PreExecution(executionCache, initial))
			{
				return false;
			}
			if (!executionCache.SourceEntity.Subscriber)
			{
				executionCache.Message = "Not a subscriber!";
				return false;
			}
			ZoneId zoneId;
			if (LocalZoneManager.ZoneRecord != null && ZoneIdExtensions.ZoneIdDict.TryGetValue(LocalZoneManager.ZoneRecord.ZoneId, out zoneId) && zoneId.PreventPortableCraftingStation())
			{
				executionCache.Message = "You cannot deploy here!";
				return false;
			}
			if (executionCache.SourceEntity.SkillsController.GetElapsedSinceLastConsumable(ConsumableCategory.CraftingStation) < (float)base.Cooldown)
			{
				executionCache.Message = "Cooldown not met!";
				return false;
			}
			IRefinementStation nearbyRefinementStation = InteractiveExtensions.GetNearbyRefinementStation(executionCache.SourceEntity.gameObject.transform.position, 30f);
			if (nearbyRefinementStation != null)
			{
				executionCache.Message = ZString.Format<string>("Another {0} is nearby!", nearbyRefinementStation.DisplayName);
				return false;
			}
			if (!GameManager.IsServer)
			{
				executionCache.SetTargetNetworkEntity(executionCache.SourceEntity.NetworkEntity);
			}
			if (executionCache.TargetNetworkEntity == null)
			{
				executionCache.Message = "Invalid target!";
				return false;
			}
			return true;
		}

		// Token: 0x0600567C RID: 22140 RVA: 0x001E122C File Offset: 0x001DF42C
		protected override void PostExecution(ExecutionCache executionCache)
		{
			executionCache.SourceEntity.SkillsController.MarkConsumableUsed(ConsumableCategory.CraftingStation);
			if (GameManager.IsServer)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_deployablePrefab);
				NetworkEntity component = gameObject.GetComponent<NetworkEntity>();
				component.ServerInit(default(Peer), true, false);
				gameObject.GetComponent<InteractiveRefinementStationPortable>().Initialize(executionCache.SourceEntity);
				ServerGameManager.ServerNetworkEntityManager.SpawnNetworkEntityForRemoteClients(component);
				return;
			}
			DeployPortableCraftingStationUI.LastPortableDeployment = new DateTime?(DateTime.UtcNow);
		}

		// Token: 0x0600567D RID: 22141 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		protected override bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			mastery = null;
			return false;
		}

		// Token: 0x0600567E RID: 22142 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool MeetsRequirementsForUI(GameEntity entity)
		{
			return true;
		}

		// Token: 0x0600567F RID: 22143 RVA: 0x00079A99 File Offset: 0x00077C99
		protected override bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = this.m_animation;
			return this.m_animation != null;
		}

		// Token: 0x04004C7B RID: 19579
		private const ConsumableCategory kConsumableCategory = ConsumableCategory.CraftingStation;

		// Token: 0x04004C7C RID: 19580
		private const float kPortableCraftingStationRange = 30f;

		// Token: 0x04004C7D RID: 19581
		[SerializeField]
		private GameObject m_deployablePrefab;

		// Token: 0x04004C7E RID: 19582
		[SerializeField]
		private AbilityAnimation m_animation;
	}
}
