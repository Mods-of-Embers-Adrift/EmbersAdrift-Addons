using System;
using System.Collections;
using System.Collections.Generic;
using Drawing;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Player;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Game.Spawning.Behavior;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Spawning
{
	// Token: 0x020006BE RID: 1726
	public class SpawnController : MonoBehaviourGizmos, ISpawnController
	{
		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x06003485 RID: 13445 RVA: 0x00063F9E File Offset: 0x0006219E
		protected bool m_showTargetPopulationInfoBox
		{
			get
			{
				return this.m_spawnProfileData != null && this.m_spawnProfileData.IsFixedDistribution;
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06003486 RID: 13446 RVA: 0x00063FB5 File Offset: 0x000621B5
		protected virtual bool m_showTargetPopulation
		{
			get
			{
				return this.m_spawnProfileData == null || !this.m_spawnProfileData.IsFixedDistribution;
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06003487 RID: 13447 RVA: 0x00164B20 File Offset: 0x00162D20
		protected virtual Vector3? m_currentPosition
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06003488 RID: 13448 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool PerformOccupancyCheck
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06003489 RID: 13449 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual int SpawnIndex
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x0600348A RID: 13450 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool DespawnOnDeath
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x0600348B RID: 13451 RVA: 0x00063FCF File Offset: 0x000621CF
		public bool CallForHelpRequiresLos
		{
			get
			{
				return this.m_callForHelpRequiresLos;
			}
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x0600348C RID: 13452 RVA: 0x00063FD7 File Offset: 0x000621D7
		public bool ForceIndoorProfiles
		{
			get
			{
				return this.m_forceIndoorProfiles;
			}
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x0016517C File Offset: 0x0016337C
		private string GetTimingInfoDescription()
		{
			string str = string.Empty;
			if (this.m_customInitialSpawnTime)
			{
				str = (Mathf.Approximately(this.m_initialSpawnTime.Min, this.m_initialSpawnTime.Max) ? this.m_initialSpawnTime.Min.GetFormattedTime(true) : (this.m_initialSpawnTime.Min.GetFormattedTime(true) + " to " + this.m_initialSpawnTime.Max.GetFormattedTime(true)));
			}
			else if (this.m_instantSpawn)
			{
				str = 15f.GetFormattedTime(true);
			}
			else
			{
				str = this.m_monitorFrequency.GetFormattedTime(true);
			}
			MinMaxFloatRange respawnTimer = this.RespawnTimer;
			string str2 = Mathf.Approximately(respawnTimer.Min, respawnTimer.Max) ? respawnTimer.Min.GetFormattedTime(true) : (respawnTimer.Min.GetFormattedTime(true) + " to " + respawnTimer.Max.GetFormattedTime(true));
			return "Initial spawn:\t" + str + "\nRespawn:\t\t" + str2;
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x0016527C File Offset: 0x0016347C
		private string GetPopulationDescription()
		{
			string result = string.Empty;
			if (this.m_spawnProfileData == null || !this.m_spawnProfileData.IsFixedDistribution)
			{
				return result;
			}
			int fixedDistributionTargetPopulation = this.m_spawnProfileData.GetFixedDistributionTargetPopulation(true);
			if (this.m_spawnProfileData.DayNightCondition == DayNightSpawnCondition.SeparateDayNight)
			{
				int fixedDistributionTargetPopulation2 = this.m_spawnProfileData.GetFixedDistributionTargetPopulation(false);
				result = "Target Population is controlled by a FIXED DISTRIBUTION!\nDAY: " + fixedDistributionTargetPopulation.ToString() + ", NIGHT: " + fixedDistributionTargetPopulation2.ToString();
			}
			else
			{
				result = "Target Population is controlled by a FIXED DISTRIBUTION!\nTargetPopulation: " + fixedDistributionTargetPopulation.ToString();
			}
			return result;
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x0600348F RID: 13455 RVA: 0x00063FDF File Offset: 0x000621DF
		private bool m_showLocalRespawnTimer
		{
			get
			{
				return this.m_respawnTimerProfile == null;
			}
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06003490 RID: 13456 RVA: 0x00063FED File Offset: 0x000621ED
		private MinMaxFloatRange RespawnTimer
		{
			get
			{
				if (!(this.m_respawnTimerProfile != null))
				{
					return this.m_respawnTimer;
				}
				return this.m_respawnTimerProfile.Range;
			}
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x00063AF7 File Offset: 0x00061CF7
		private IEnumerable GetBehaviorProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<BehaviorProfile>();
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x0006400F File Offset: 0x0006220F
		public bool TryGetBehaviorProfile(out BehaviorProfile profile)
		{
			profile = (this.m_overrideBehaviorProfile ? this.m_behaviorProfile : null);
			return profile != null;
		}

		// Token: 0x06003493 RID: 13459 RVA: 0x0006402C File Offset: 0x0006222C
		public bool TryGetLevel(out int level)
		{
			level = (this.m_overrideLevelRange ? this.m_levelRange.RandomWithinRange() : 0);
			return this.m_overrideLevelRange;
		}

		// Token: 0x06003494 RID: 13460 RVA: 0x0006404C File Offset: 0x0006224C
		public int GetLevel()
		{
			return this.m_levelRange.RandomWithinRange();
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x00064059 File Offset: 0x00062259
		public bool OverrideInteractionFlags(out NpcInteractionFlags flags)
		{
			flags = (this.m_overrideInteractionFlags ? this.m_interactionFlags : NpcInteractionFlags.None);
			return this.m_overrideInteractionFlags;
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06003496 RID: 13462 RVA: 0x00064074 File Offset: 0x00062274
		public int TargetPopulation
		{
			get
			{
				return this.m_targetPopulation;
			}
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06003497 RID: 13463 RVA: 0x0006407C File Offset: 0x0006227C
		public BehaviorSubTreeCollection BehaviorOverrides
		{
			get
			{
				return this.m_behaviorOverrides;
			}
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06003498 RID: 13464 RVA: 0x00064084 File Offset: 0x00062284
		public DialogueSource OverrideDialogue
		{
			get
			{
				return this.m_overrideDialogue;
			}
		}

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x06003499 RID: 13465 RVA: 0x00165304 File Offset: 0x00163504
		public float? LeashDistance
		{
			get
			{
				if (!this.m_overrideLeashDistance)
				{
					return null;
				}
				return new float?(this.m_leashDistance);
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x0600349A RID: 13466 RVA: 0x00165330 File Offset: 0x00163530
		public float? ResetDistance
		{
			get
			{
				if (!this.m_overrideResetDistance)
				{
					return null;
				}
				return new float?(this.m_resetDistance);
			}
		}

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x0600349B RID: 13467 RVA: 0x0006408C File Offset: 0x0006228C
		internal SpawnController.DecalColorType DecalColor
		{
			get
			{
				return this.m_decalColor;
			}
		}

		// Token: 0x0600349C RID: 13468 RVA: 0x0016535C File Offset: 0x0016355C
		protected virtual void Awake()
		{
			if (this.m_isTemplate || !GameManager.IsServer)
			{
				base.enabled = false;
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (SpawnController.m_hits == null)
			{
				SpawnController.m_hits = new RaycastHit[50];
				SpawnController.m_layerMask = GlobalSettings.Values.Npcs.TerrainLayerMask.value;
			}
			this.m_overrides = base.GetComponent<SpawnControllerOverrides>();
		}

		// Token: 0x0600349D RID: 13469 RVA: 0x001653C4 File Offset: 0x001635C4
		protected virtual void Start()
		{
			if (this.m_isTemplate || !GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			if (SpawnController.m_instantWait == null)
			{
				SpawnController.m_instantWait = new WaitForSeconds(15f);
			}
			this.m_wait = new WaitForSeconds(this.m_monitorFrequency);
			this.m_spawnMonitorCo = this.SpawnMonitorCo();
			base.StartCoroutine(this.m_spawnMonitorCo);
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x00064094 File Offset: 0x00062294
		protected virtual void OnDestroy()
		{
			if (this.m_spawnMonitorCo != null)
			{
				base.StopCoroutine(this.m_spawnMonitorCo);
				this.m_spawnMonitorCo = null;
			}
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool UnregisterSpawnLocationEarly(GameEntity entity)
		{
			return false;
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x000640B1 File Offset: 0x000622B1
		protected virtual int GetTargetPopulation()
		{
			if (this.m_spawnProfileData == null || !this.m_spawnProfileData.IsFixedDistribution)
			{
				return this.m_targetPopulation;
			}
			return this.m_spawnProfileData.GetFixedDistributionTargetPopulation(SkyDomeManager.IsDay());
		}

		// Token: 0x060034A1 RID: 13473 RVA: 0x000640DF File Offset: 0x000622DF
		protected virtual WaitForSeconds GetStartupWait()
		{
			if (this.m_customInitialSpawnTime)
			{
				return new WaitForSeconds(this.m_initialSpawnTime.RandomWithinRange());
			}
			if (!this.m_instantSpawn)
			{
				return this.m_wait;
			}
			return SpawnController.m_instantWait;
		}

		// Token: 0x060034A2 RID: 13474 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SpawnMonitorCoStartup()
		{
		}

		// Token: 0x060034A3 RID: 13475 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void BeforeSpawnMonitorCycle()
		{
		}

		// Token: 0x060034A4 RID: 13476 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void MidSpawnMonitorCycle()
		{
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AfterSpawnMonitorCycle()
		{
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void NotifyOfDeath(GameEntity entity)
		{
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x00165428 File Offset: 0x00163628
		protected void MonitorActiveSpawns()
		{
			DateTime utcNow = DateTime.UtcNow;
			for (int i = this.m_activeSpawns.Count - 1; i >= 0; i--)
			{
				bool flag = false;
				if (!this.m_activeSpawns[i].Entity)
				{
					this.m_activeSpawns[i].Despawn();
					flag = true;
				}
				else if (this.m_activeSpawns[i].Entity.RemoveFromActiveSpawns)
				{
					this.m_activeSpawns[i].UnregisterSpawnLocation();
					flag = true;
				}
				else if (this.UnregisterSpawnLocationEarly(this.m_activeSpawns[i].Entity))
				{
					this.m_activeSpawns[i].UnregisterSpawnLocation();
					flag = true;
				}
				if (flag)
				{
					this.m_spawnProfileData.RemoveActiveSpawn(this.m_activeSpawns[i]);
					this.m_respawnTimes.Add(utcNow.AddSeconds((double)UnityEngine.Random.Range(this.RespawnTimer.Min, this.RespawnTimer.Max)));
					this.m_activeSpawns.RemoveAt(i);
				}
			}
		}

		// Token: 0x060034A8 RID: 13480 RVA: 0x0006410E File Offset: 0x0006230E
		private IEnumerator SpawnMonitorCo()
		{
			while (NetworkManager.MyHost == null)
			{
				yield return null;
			}
			this.SpawnMonitorCoStartup();
			yield return this.GetStartupWait();
			for (;;)
			{
				this.BeforeSpawnMonitorCycle();
				DateTime utcNow = DateTime.UtcNow;
				for (int i = this.m_respawnTimes.Count - 1; i >= 0; i--)
				{
					if (utcNow >= this.m_respawnTimes[i])
					{
						this.m_respawnTimes.RemoveAt(i);
					}
				}
				this.MonitorActiveSpawns();
				this.MidSpawnMonitorCycle();
				bool isDay = SkyDomeManager.IsDay();
				bool flag = false;
				bool flag2 = false;
				switch (this.m_spawnProfileData.DayNightCondition)
				{
				case DayNightSpawnCondition.Unified:
					flag = true;
					flag2 = false;
					break;
				case DayNightSpawnCondition.SeparateDayNight:
					flag = true;
					flag2 = true;
					break;
				case DayNightSpawnCondition.DayOnly:
					flag = isDay;
					flag2 = true;
					break;
				case DayNightSpawnCondition.NightOnly:
					flag = !isDay;
					flag2 = true;
					break;
				}
				if (flag2)
				{
					int num = 0;
					for (int j = this.m_activeSpawns.Count - 1; j >= 0; j--)
					{
						if (this.m_activeSpawns[j].IsDay != isDay && (this.m_bypassDespawnObserverCheck || this.m_activeSpawns[j].Entity.NetworkEntity.NObservers <= 0 || !SpawnController.HasPlayersNearby(this.m_activeSpawns[j].Entity)))
						{
							this.m_spawnProfileData.RemoveActiveSpawn(this.m_activeSpawns[j]);
							this.m_respawnTimes.Add(utcNow.AddSeconds(5.0));
							this.m_activeSpawns[j].Despawn();
							this.m_activeSpawns.RemoveAt(j);
							num++;
							if (num >= 2)
							{
								break;
							}
						}
					}
				}
				if (flag)
				{
					int failedSelections = 0;
					while (this.m_activeSpawns.Count + this.m_respawnTimes.Count < this.GetTargetPopulation())
					{
						SpawnProfile spawnProfile = null;
						if (this.m_spawnType == SpawnController.SpawnType.FixedPositionPerProfile)
						{
							isDay = SkyDomeManager.IsDay();
							spawnProfile = this.m_spawnProfileData.GetSpawnProfile(isDay);
						}
						ISpawnLocation spawnLocation = this.SelectSpawnPoint(spawnProfile);
						if (spawnLocation != null)
						{
							if (this.m_spawnType != SpawnController.SpawnType.FixedPositionPerProfile)
							{
								isDay = SkyDomeManager.IsDay();
								spawnProfile = this.m_spawnProfileData.GetSpawnProfile(isDay);
							}
							if (spawnProfile != null)
							{
								Vector3 position = spawnLocation.GetPosition();
								Quaternion rotation = spawnLocation.GetRotation();
								if (this.m_logSpawns)
								{
									spawnProfile.SpawnMessage(position, rotation);
								}
								GameEntity gameEntity = spawnProfile.DynamicSpawn(this, position, rotation, null);
								if (gameEntity)
								{
									gameEntity.NetworkEntity.SpawnController = this;
									this.m_activeSpawns.Add(new SpawnController.SpawnTracker(gameEntity, spawnLocation, isDay, spawnProfile, this.SpawnIndex));
								}
							}
						}
						else
						{
							failedSelections++;
							if (failedSelections >= 10)
							{
								break;
							}
						}
						yield return null;
					}
				}
				this.AfterSpawnMonitorCycle();
				yield return this.m_wait;
			}
			yield break;
		}

		// Token: 0x060034A9 RID: 13481 RVA: 0x00165548 File Offset: 0x00163748
		protected static bool HasPlayersNearby(GameEntity entity)
		{
			if (entity == null || !entity.gameObject)
			{
				return false;
			}
			Collider[] colliders = Hits.Colliders25;
			int num = Physics.OverlapSphereNonAlloc(entity.gameObject.transform.position, 50f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				if (DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity) && gameEntity && gameEntity.Type == GameEntityType.Player && !gameEntity.IsNoTarget)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060034AA RID: 13482 RVA: 0x00049FFA File Offset: 0x000481FA
		protected ISpawnLocation SelectSpawnPoint(SpawnProfile spawnProfile = null)
		{
			return null;
		}

		// Token: 0x060034AB RID: 13483 RVA: 0x001655D4 File Offset: 0x001637D4
		private Vector3 SelectRandomPosition()
		{
			Vector3 vector = base.gameObject.transform.localScale * 0.5f;
			Vector3 point;
			switch (this.m_areaShape)
			{
			case SpawnController.AreaShape.Cube:
			{
				Vector3 b = new Vector3(UnityEngine.Random.Range(-vector.x, vector.x), UnityEngine.Random.Range(-vector.y, vector.y), UnityEngine.Random.Range(-vector.z, vector.z));
				point = base.gameObject.transform.position + b;
				break;
			}
			case SpawnController.AreaShape.Sphere:
			{
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				Vector3 b = new Vector3(insideUnitSphere.x * vector.x, insideUnitSphere.y * vector.y, insideUnitSphere.z * vector.z);
				point = base.gameObject.transform.position + b;
				break;
			}
			case SpawnController.AreaShape.Circle:
			{
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				Vector3 b = new Vector3(insideUnitCircle.x * vector.x, vector.y, insideUnitCircle.y * vector.z);
				point = base.gameObject.transform.position + b;
				break;
			}
			default:
				throw new ArgumentException("Unknown area shape " + this.m_areaShape.ToString() + "!");
			}
			return point.RotateAroundPivot(base.gameObject.transform.position, new Vector3(0f, base.gameObject.transform.eulerAngles.y, 0f));
		}

		// Token: 0x060034AC RID: 13484 RVA: 0x00165770 File Offset: 0x00163970
		private Vector3? SelectRandomPerlinPosition()
		{
			int i;
			for (i = 0; i < 10; i++)
			{
				Vector3 vector = this.SelectRandomPosition();
				float num = Mathf.PerlinNoise(vector.x, vector.z);
				if (UnityEngine.Random.Range(0f, 1f) <= num)
				{
					return new Vector3?(vector);
				}
			}
			Debug.LogWarning(string.Concat(new string[]
			{
				"Iterated ",
				i.ToString(),
				" times while trying to SelectRandomPerlinPosition for ",
				base.gameObject.name,
				" at ",
				base.gameObject.transform.position.ToString()
			}));
			return null;
		}

		// Token: 0x060034AD RID: 13485 RVA: 0x00165828 File Offset: 0x00163A28
		private ISpawnLocation SelectFixedPosition(GameObjectProbabilityCollection collection)
		{
			if (collection == null)
			{
				return null;
			}
			FixedSpawnMarkerEntry entry = collection.GetEntry(null, false);
			if (entry == null || entry.Obj == null || entry.Obj.gameObject == null)
			{
				return null;
			}
			if (this.PerformOccupancyCheck && entry.Obj.Occupied)
			{
				return null;
			}
			return entry.Obj;
		}

		// Token: 0x060034AE RID: 13486 RVA: 0x00165888 File Offset: 0x00163A88
		private bool PassesRulesCheck(NavMeshHit hit)
		{
			if (this.m_rules == null || this.m_rules.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < this.m_rules.Length; i++)
			{
				if (!this.m_rules[i].IsValid(hit))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060034AF RID: 13487 RVA: 0x0006411D File Offset: 0x0006231D
		internal void UpdateSpawnParameters(ISpawnController controllerInterface)
		{
			this.m_levelRange = controllerInterface.LevelRange;
		}

		// Token: 0x060034B0 RID: 13488 RVA: 0x0006412B File Offset: 0x0006232B
		internal void ReplaceSpawnProfiles(SpawnProfileData other)
		{
			SpawnProfileData spawnProfileData = this.m_spawnProfileData;
			if (spawnProfileData == null)
			{
				return;
			}
			spawnProfileData.ReplaceData(other);
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x0004475B File Offset: 0x0004295B
		internal virtual void ReplaceTargetPopulationThresholds(TargetPopulationThreshold[] thresholds)
		{
		}

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x060034B2 RID: 13490 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.MatchAttackerLevel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x060034B3 RID: 13491 RVA: 0x0006413E File Offset: 0x0006233E
		bool ISpawnController.LogSpawns
		{
			get
			{
				return this.m_logSpawns;
			}
		}

		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x060034B4 RID: 13492 RVA: 0x00064146 File Offset: 0x00062346
		MinMaxIntRange ISpawnController.LevelRange
		{
			get
			{
				return this.m_levelRange;
			}
		}

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x060034B5 RID: 13493 RVA: 0x0006414E File Offset: 0x0006234E
		Vector3? ISpawnController.CurrentPosition
		{
			get
			{
				return this.m_currentPosition;
			}
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x00064156 File Offset: 0x00062356
		void ISpawnController.NotifyOfDeath(GameEntity entity)
		{
			this.NotifyOfDeath(entity);
		}

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x060034B7 RID: 13495 RVA: 0x0006415F File Offset: 0x0006235F
		int ISpawnController.XpAdjustment
		{
			get
			{
				return this.m_xpAdjustment;
			}
		}

		// Token: 0x060034B8 RID: 13496 RVA: 0x00064167 File Offset: 0x00062367
		bool ISpawnController.TryGetOverrideData(SpawnProfile spawnProfile, out SpawnControllerOverrideData data)
		{
			data = null;
			return spawnProfile && this.m_overrides && this.m_overrides.TryGetOverrideData(spawnProfile, out data);
		}

		// Token: 0x060034B9 RID: 13497 RVA: 0x001658D0 File Offset: 0x00163AD0
		private void LogBadSpawnLocation(ISpawnLocation location)
		{
			if (this.m_spawnType != SpawnController.SpawnType.FixedPosition || location == null)
			{
				return;
			}
			if (SpawnController.m_invalidSpawnLocations == null)
			{
				SpawnController.m_invalidSpawnLocations = new HashSet<ISpawnLocation>(100);
			}
			if (!SpawnController.m_invalidSpawnLocations.Contains(location))
			{
				SpawnController.m_invalidSpawnLocations.Add(location);
				string debugString = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, location.GetPosition(), location.GetRotation()).DebugString;
				SpawnController.InvalidLocationArray[0] = true;
				SpawnController.InvalidLocationArray[1] = debugString;
				Debug.LogWarning("Invalid FixedPosition Spawn Location at " + debugString);
				SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "{@Server} Invalid FixedPosition Spawn Location at {@DebugPosition}", SpawnController.InvalidLocationArray);
			}
		}

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x060034BA RID: 13498 RVA: 0x00064190 File Offset: 0x00062390
		private bool m_showCopySingle
		{
			get
			{
				return this.m_copyFrom != null;
			}
		}

		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x060034BB RID: 13499 RVA: 0x0006419E File Offset: 0x0006239E
		private bool m_showCopyAllFixedPoints
		{
			get
			{
				return this.m_copyPointsFrom != null && this.m_copyPointsFrom.Length != 0;
			}
		}

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x060034BC RID: 13500 RVA: 0x000641B4 File Offset: 0x000623B4
		private bool m_showCopyAllFromParents
		{
			get
			{
				return this.m_copyFromParents != null && this.m_copyFromParents.Length != 0;
			}
		}

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x060034BD RID: 13501 RVA: 0x000641CA File Offset: 0x000623CA
		private bool m_showFillCopyFromArray
		{
			get
			{
				return this.m_copyFromTagFilter > SpawnController.SpawnControllerTags.All;
			}
		}

		// Token: 0x04003286 RID: 12934
		private const float kInstantDelay = 15f;

		// Token: 0x04003287 RID: 12935
		private const int kMaxIterationsPerSpawn = 10;

		// Token: 0x04003288 RID: 12936
		private const float kNavSampleDistance = 2f;

		// Token: 0x04003289 RID: 12937
		private const int kNavMeshMask = -1;

		// Token: 0x0400328A RID: 12938
		private const int kMaxDespawnPerCycle = 2;

		// Token: 0x0400328B RID: 12939
		protected const string kParameterGroupName = "Running Parameters";

		// Token: 0x0400328C RID: 12940
		private const string kSpawnDataGroupName = "Spawn Config";

		// Token: 0x0400328D RID: 12941
		private const string kNpcGroupName = "NPCs";

		// Token: 0x0400328E RID: 12942
		private const string kNpcLeashGroupName = "NPCs/Leash";

		// Token: 0x0400328F RID: 12943
		private const string kNpcResetGroupName = "NPCs/Reset";

		// Token: 0x04003290 RID: 12944
		private const string kDecalGroup = "Decal";

		// Token: 0x04003291 RID: 12945
		private SpawnControllerOverrides m_overrides;

		// Token: 0x04003292 RID: 12946
		[Tooltip("Log Spawns to the log output, and one day elastic.")]
		[SerializeField]
		private bool m_logSpawns;

		// Token: 0x04003293 RID: 12947
		[SerializeField]
		private DummyClass m_dummyTimingInfo;

		// Token: 0x04003294 RID: 12948
		[SerializeField]
		private bool m_instantSpawn;

		// Token: 0x04003295 RID: 12949
		[SerializeField]
		protected bool m_customInitialSpawnTime;

		// Token: 0x04003296 RID: 12950
		[SerializeField]
		protected MinMaxFloatRange m_initialSpawnTime = new MinMaxFloatRange(30f, 30f);

		// Token: 0x04003297 RID: 12951
		[SerializeField]
		private bool m_requireTerrainBelow;

		// Token: 0x04003298 RID: 12952
		[SerializeField]
		private bool m_bypassDespawnObserverCheck;

		// Token: 0x04003299 RID: 12953
		[SerializeField]
		private bool m_bypassLoggingBadFixedPositions;

		// Token: 0x0400329A RID: 12954
		[SerializeField]
		protected int m_targetPopulation = 1;

		// Token: 0x0400329B RID: 12955
		[SerializeField]
		private DummyClass m_dummyPopulationInfoBox;

		// Token: 0x0400329C RID: 12956
		[SerializeField]
		protected float m_monitorFrequency = 5f;

		// Token: 0x0400329D RID: 12957
		[SerializeField]
		private MinMaxFloatRangeScriptable m_respawnTimerProfile;

		// Token: 0x0400329E RID: 12958
		[SerializeField]
		private MinMaxFloatRange m_respawnTimer = new MinMaxFloatRange(120f, 120f);

		// Token: 0x0400329F RID: 12959
		[SerializeField]
		private SpawnController.SpawnType m_spawnType;

		// Token: 0x040032A0 RID: 12960
		[SerializeField]
		private SpawnController.AreaShape m_areaShape = SpawnController.AreaShape.Sphere;

		// Token: 0x040032A1 RID: 12961
		[SerializeField]
		private GameObjectProbabilityCollection m_markers;

		// Token: 0x040032A2 RID: 12962
		[SerializeField]
		private SpawnController.ProfileSpecificMarkers[] m_profileSpecificMarkers;

		// Token: 0x040032A3 RID: 12963
		[SerializeField]
		private SpawnRule[] m_rules;

		// Token: 0x040032A4 RID: 12964
		[SerializeField]
		private SpawnProfileData m_spawnProfileData;

		// Token: 0x040032A5 RID: 12965
		[MinMaxIntRangeSlider(1, 54, 50f)]
		[SerializeField]
		private MinMaxIntRange m_levelRange = new MinMaxIntRange(1, 1);

		// Token: 0x040032A6 RID: 12966
		[SerializeField]
		private bool m_callForHelpRequiresLos;

		// Token: 0x040032A7 RID: 12967
		[Tooltip("Force the NPC to use the indoor Call For Help Profile AND their indoor Sensor Profile.")]
		[SerializeField]
		private bool m_forceIndoorProfiles;

		// Token: 0x040032A8 RID: 12968
		[Tooltip("% XP Adjustment. Spawn Profile values stack with Spawn Controller values.  25=+25%")]
		[SerializeField]
		private int m_xpAdjustment;

		// Token: 0x040032A9 RID: 12969
		[SerializeField]
		private bool m_overrideLeashDistance;

		// Token: 0x040032AA RID: 12970
		[SerializeField]
		private float m_leashDistance = 20f;

		// Token: 0x040032AB RID: 12971
		[SerializeField]
		private bool m_overrideResetDistance;

		// Token: 0x040032AC RID: 12972
		[SerializeField]
		private float m_resetDistance = 40f;

		// Token: 0x040032AD RID: 12973
		[SerializeField]
		private BehaviorProfile m_behaviorProfile;

		// Token: 0x040032AE RID: 12974
		[SerializeField]
		private BehaviorSubTreeCollectionWithOverride m_behaviorOverrides;

		// Token: 0x040032AF RID: 12975
		[SerializeField]
		private DialogueSource m_overrideDialogue;

		// Token: 0x040032B0 RID: 12976
		[SerializeField]
		private bool m_overrideLevelRange;

		// Token: 0x040032B1 RID: 12977
		[SerializeField]
		private bool m_overrideBehaviorProfile;

		// Token: 0x040032B2 RID: 12978
		private const string kInteractionGroup = "NPCs/Interaction";

		// Token: 0x040032B3 RID: 12979
		[SerializeField]
		private bool m_overrideInteractionFlags;

		// Token: 0x040032B4 RID: 12980
		[SerializeField]
		private NpcInteractionFlags m_interactionFlags;

		// Token: 0x040032B5 RID: 12981
		private const int kDownwardHitCount = 50;

		// Token: 0x040032B6 RID: 12982
		private const float kRaycastDistance = 1024f;

		// Token: 0x040032B7 RID: 12983
		private static int m_layerMask = 0;

		// Token: 0x040032B8 RID: 12984
		private static RaycastHit[] m_hits = null;

		// Token: 0x040032B9 RID: 12985
		public const int kInitialListSize = 10;

		// Token: 0x040032BA RID: 12986
		protected readonly List<DateTime> m_respawnTimes = new List<DateTime>(10);

		// Token: 0x040032BB RID: 12987
		internal readonly List<SpawnController.SpawnTracker> m_activeSpawns = new List<SpawnController.SpawnTracker>(10);

		// Token: 0x040032BC RID: 12988
		private WaitForSeconds m_wait;

		// Token: 0x040032BD RID: 12989
		private IEnumerator m_spawnMonitorCo;

		// Token: 0x040032BE RID: 12990
		protected static WaitForSeconds m_instantWait = null;

		// Token: 0x040032BF RID: 12991
		[SerializeField]
		private SpawnController.DecalColorType m_decalColor;

		// Token: 0x040032C0 RID: 12992
		private const float kNearbyRadius = 50f;

		// Token: 0x040032C1 RID: 12993
		private const string kInvalidLocationTemplate = "{@Server} Invalid FixedPosition Spawn Location at {@DebugPosition}";

		// Token: 0x040032C2 RID: 12994
		private static readonly object[] InvalidLocationArray = new object[2];

		// Token: 0x040032C3 RID: 12995
		private static HashSet<ISpawnLocation> m_invalidSpawnLocations = null;

		// Token: 0x040032C4 RID: 12996
		[SerializeField]
		private bool m_disableGizmos;

		// Token: 0x040032C5 RID: 12997
		[Tooltip("When set to true script is disabled on awake")]
		[SerializeField]
		private bool m_isTemplate;

		// Token: 0x040032C6 RID: 12998
		[SerializeField]
		private SpawnController.SpawnControllerTags m_templateTags;

		// Token: 0x040032C7 RID: 12999
		[SerializeField]
		private int m_templateDistance = 50;

		// Token: 0x040032C8 RID: 13000
		[SerializeField]
		private Transform m_templateCenter;

		// Token: 0x040032C9 RID: 13001
		[SerializeField]
		private Vector3 m_repositionLocalPos = Vector3.zero;

		// Token: 0x040032CA RID: 13002
		[SerializeField]
		private Quaternion m_repositionLocalRotation = Quaternion.identity;

		// Token: 0x040032CB RID: 13003
		[SerializeField]
		private SpawnController m_copyFrom;

		// Token: 0x040032CC RID: 13004
		[SerializeField]
		private SpawnController.SpawnControllerTags m_copyFromTagFilter;

		// Token: 0x040032CD RID: 13005
		[SerializeField]
		private SpawnController[] m_copyPointsFrom;

		// Token: 0x040032CE RID: 13006
		[SerializeField]
		private GameObject[] m_copyFromParents;

		// Token: 0x040032CF RID: 13007
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x040032D0 RID: 13008
		private const int kBadIndex = -1;

		// Token: 0x040032D1 RID: 13009
		private const string kCopyGroup = "COPY";

		// Token: 0x040032D2 RID: 13010
		private const string kCopySingle = "COPY/Single Spawn Controller";

		// Token: 0x040032D3 RID: 13011
		private const string kCopySingleBtnGroup = "COPY/Single Spawn Controller/Buttons";

		// Token: 0x040032D4 RID: 13012
		private const string kCopyMultiple = "COPY/Multiple Spawn Points";

		// Token: 0x040032D5 RID: 13013
		private const string kCopyMultipleBtnGroup = "COPY/Multiple Spawn Points/Buttons";

		// Token: 0x040032D6 RID: 13014
		private const string kEditorGroup = "Editor";

		// Token: 0x020006BF RID: 1727
		internal enum AreaShape
		{
			// Token: 0x040032D8 RID: 13016
			Cube,
			// Token: 0x040032D9 RID: 13017
			Sphere,
			// Token: 0x040032DA RID: 13018
			Circle
		}

		// Token: 0x020006C0 RID: 1728
		internal enum SpawnType
		{
			// Token: 0x040032DC RID: 13020
			Random,
			// Token: 0x040032DD RID: 13021
			RandomPerlin,
			// Token: 0x040032DE RID: 13022
			FixedPosition,
			// Token: 0x040032DF RID: 13023
			FixedPositionPerProfile
		}

		// Token: 0x020006C1 RID: 1729
		internal enum DecalColorType
		{
			// Token: 0x040032E1 RID: 13025
			Color0,
			// Token: 0x040032E2 RID: 13026
			Color1,
			// Token: 0x040032E3 RID: 13027
			Color2,
			// Token: 0x040032E4 RID: 13028
			Color3,
			// Token: 0x040032E5 RID: 13029
			Color4,
			// Token: 0x040032E6 RID: 13030
			Color5,
			// Token: 0x040032E7 RID: 13031
			Color6,
			// Token: 0x040032E8 RID: 13032
			Color7,
			// Token: 0x040032E9 RID: 13033
			Color8,
			// Token: 0x040032EA RID: 13034
			Color9
		}

		// Token: 0x020006C2 RID: 1730
		internal readonly struct SpawnTracker
		{
			// Token: 0x060034C0 RID: 13504 RVA: 0x000641FA File Offset: 0x000623FA
			public SpawnTracker(GameEntity entity, ISpawnLocation location, bool isDay, SpawnProfile profile, int index)
			{
				this.Entity = entity;
				this.SpawnLocation = location;
				this.SpawnLocation.Occupied = true;
				this.IsDay = isDay;
				this.Profile = profile;
				this.Index = index;
			}

			// Token: 0x060034C1 RID: 13505 RVA: 0x00165A24 File Offset: 0x00163C24
			public void Despawn()
			{
				this.UnregisterSpawnLocation();
				if (this.Entity != null)
				{
					try
					{
						UnityEngine.Object.Destroy(this.Entity.gameObject);
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
				}
			}

			// Token: 0x060034C2 RID: 13506 RVA: 0x0006422D File Offset: 0x0006242D
			public void UnregisterSpawnLocation()
			{
				this.SpawnLocation.Occupied = false;
			}

			// Token: 0x040032EB RID: 13035
			public readonly ISpawnLocation SpawnLocation;

			// Token: 0x040032EC RID: 13036
			public readonly GameEntity Entity;

			// Token: 0x040032ED RID: 13037
			public readonly bool IsDay;

			// Token: 0x040032EE RID: 13038
			public readonly SpawnProfile Profile;

			// Token: 0x040032EF RID: 13039
			public readonly int Index;
		}

		// Token: 0x020006C3 RID: 1731
		[Serializable]
		private class ProfileSpecificMarkers
		{
			// Token: 0x17000B90 RID: 2960
			// (get) Token: 0x060034C3 RID: 13507 RVA: 0x0006423B File Offset: 0x0006243B
			public SpawnProfile Profile
			{
				get
				{
					return this.m_profile;
				}
			}

			// Token: 0x17000B91 RID: 2961
			// (get) Token: 0x060034C4 RID: 13508 RVA: 0x00064243 File Offset: 0x00062443
			public GameObjectProbabilityCollection Markers
			{
				get
				{
					return this.m_markers;
				}
			}

			// Token: 0x040032F0 RID: 13040
			[SerializeField]
			private SpawnProfile m_profile;

			// Token: 0x040032F1 RID: 13041
			[SerializeField]
			private GameObjectProbabilityCollection m_markers;
		}

		// Token: 0x020006C4 RID: 1732
		[Flags]
		private enum SpawnControllerTags
		{
			// Token: 0x040032F3 RID: 13043
			All = 0,
			// Token: 0x040032F4 RID: 13044
			NpcRare = 1,
			// Token: 0x040032F5 RID: 13045
			NpcIdle = 2,
			// Token: 0x040032F6 RID: 13046
			NpcWander = 4,
			// Token: 0x040032F7 RID: 13047
			ResourceRare = 1024,
			// Token: 0x040032F8 RID: 13048
			ResourceGeneric = 2048,
			// Token: 0x040032F9 RID: 13049
			ResourceProspecting = 4096,
			// Token: 0x040032FA RID: 13050
			ResourceForesting = 8192,
			// Token: 0x040032FB RID: 13051
			ChestRare = 1048576,
			// Token: 0x040032FC RID: 13052
			Chest = 2097152,
			// Token: 0x040032FD RID: 13053
			NestRare = 4194304,
			// Token: 0x040032FE RID: 13054
			Nest = 8388608
		}
	}
}
