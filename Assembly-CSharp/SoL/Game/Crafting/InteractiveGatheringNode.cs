using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.Messages;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CDB RID: 3291
	public class InteractiveGatheringNode : InteractiveRemoteContainerSingleLooter, IGatheringNode, ILootRollSource
	{
		// Token: 0x170017CE RID: 6094
		// (get) Token: 0x06006395 RID: 25493 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170017CF RID: 6095
		// (get) Token: 0x06006396 RID: 25494 RVA: 0x000830E5 File Offset: 0x000812E5
		// (set) Token: 0x06006397 RID: 25495 RVA: 0x000830ED File Offset: 0x000812ED
		public ISpawnController SpawnController { get; set; }

		// Token: 0x170017D0 RID: 6096
		// (get) Token: 0x06006398 RID: 25496 RVA: 0x000830F6 File Offset: 0x000812F6
		// (set) Token: 0x06006399 RID: 25497 RVA: 0x000830FE File Offset: 0x000812FE
		public ResourceSpawnProfileV2 SpawnProfile { get; set; }

		// Token: 0x170017D1 RID: 6097
		// (get) Token: 0x0600639A RID: 25498 RVA: 0x00083107 File Offset: 0x00081307
		// (set) Token: 0x0600639B RID: 25499 RVA: 0x0008310F File Offset: 0x0008130F
		public LootProfile LootProfile { get; set; }

		// Token: 0x170017D2 RID: 6098
		// (get) Token: 0x0600639C RID: 25500 RVA: 0x00083118 File Offset: 0x00081318
		// (set) Token: 0x0600639D RID: 25501 RVA: 0x00083120 File Offset: 0x00081320
		public SpawnTier SpawnTier { get; set; }

		// Token: 0x170017D3 RID: 6099
		// (get) Token: 0x0600639E RID: 25502 RVA: 0x00083129 File Offset: 0x00081329
		// (set) Token: 0x0600639F RID: 25503 RVA: 0x00083131 File Offset: 0x00081331
		public LootTableSampleCount ResourceTable { get; set; }

		// Token: 0x170017D4 RID: 6100
		// (get) Token: 0x060063A0 RID: 25504 RVA: 0x0008313A File Offset: 0x0008133A
		// (set) Token: 0x060063A1 RID: 25505 RVA: 0x00083142 File Offset: 0x00081342
		public MinMaxIntRange? Currency { get; set; }

		// Token: 0x170017D5 RID: 6101
		// (get) Token: 0x060063A2 RID: 25506 RVA: 0x0008314B File Offset: 0x0008134B
		// (set) Token: 0x060063A3 RID: 25507 RVA: 0x00083153 File Offset: 0x00081353
		public bool LogLoot { get; set; }

		// Token: 0x170017D6 RID: 6102
		// (get) Token: 0x060063A4 RID: 25508 RVA: 0x0008315C File Offset: 0x0008135C
		public SynchronizedString LooterString
		{
			get
			{
				return this.m_currentLooter;
			}
		}

		// Token: 0x170017D7 RID: 6103
		// (get) Token: 0x060063A5 RID: 25509 RVA: 0x0007D8C8 File Offset: 0x0007BAC8
		public SynchronizedEnum<InteractiveFlags> SyncInteractiveFlags
		{
			get
			{
				return this.m_interactiveFlags;
			}
		}

		// Token: 0x170017D8 RID: 6104
		// (get) Token: 0x060063A6 RID: 25510 RVA: 0x0007D9F4 File Offset: 0x0007BBF4
		CraftingToolType IGatheringNode.RequiredTool
		{
			get
			{
				return base.GatheringParams.RequiredTool;
			}
		}

		// Token: 0x170017D9 RID: 6105
		// (get) Token: 0x060063A7 RID: 25511 RVA: 0x00062B3E File Offset: 0x00060D3E
		GameEntity IGatheringNode.GameEntity
		{
			get
			{
				return base.GameEntity;
			}
		}

		// Token: 0x170017DA RID: 6106
		// (get) Token: 0x060063A8 RID: 25512 RVA: 0x00083164 File Offset: 0x00081364
		int IGatheringNode.ResourceLevel
		{
			get
			{
				if (base.GatheringParams.Level > 0)
				{
					return base.GatheringParams.Level;
				}
				return 1;
			}
		}

		// Token: 0x170017DB RID: 6107
		// (get) Token: 0x060063A9 RID: 25513 RVA: 0x0007D9E6 File Offset: 0x0007BBE6
		float IGatheringNode.GatherTime
		{
			get
			{
				return (float)base.GatheringParams.GatherTime;
			}
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x001F03C4 File Offset: 0x001EE5C4
		MasteryArchetype IGatheringNode.GetGatheringMastery()
		{
			return base.GatheringParams.GetGatheringMastery();
		}

		// Token: 0x170017DC RID: 6108
		// (get) Token: 0x060063AB RID: 25515 RVA: 0x0007DA01 File Offset: 0x0007BC01
		UniqueId? IGatheringNode.RequiredItemId
		{
			get
			{
				return base.GatheringParams.RequiredItemId;
			}
		}

		// Token: 0x170017DD RID: 6109
		// (get) Token: 0x060063AC RID: 25516 RVA: 0x0007DA0E File Offset: 0x0007BC0E
		bool IGatheringNode.RemoveRequiredItemOnUse
		{
			get
			{
				return base.GatheringParams.RemoveRequiredItemOnUse;
			}
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x00083181 File Offset: 0x00081381
		bool IGatheringNode.CanInteract(GameEntity entity, out ArchetypeInstance requiredItemInstance)
		{
			return this.CanInteractGatheringNode(entity, out requiredItemInstance);
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x0008318B File Offset: 0x0008138B
		void IGatheringNode.BeginInteraction(GameEntity entity)
		{
			this.BeginInteraction(entity);
		}

		// Token: 0x170017DE RID: 6110
		// (get) Token: 0x060063AF RID: 25519 RVA: 0x0007DA1B File Offset: 0x0007BC1B
		InteractiveFlags IGatheringNode.InteractiveFlags
		{
			get
			{
				return this.m_interactiveFlags.Value;
			}
		}

		// Token: 0x170017DF RID: 6111
		// (get) Token: 0x060063B0 RID: 25520 RVA: 0x00083194 File Offset: 0x00081394
		// (set) Token: 0x060063B1 RID: 25521 RVA: 0x000831A1 File Offset: 0x000813A1
		public string Description
		{
			get
			{
				return this.m_description.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_description.Value = value;
				}
			}
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x00207604 File Offset: 0x00205804
		protected override void Awake()
		{
			base.Awake();
			if (this.m_surveyVisuals != null)
			{
				this.m_surveyVisuals.SetActive(false);
			}
			if (GameManager.IsServer && InteractiveGatheringNode.m_despawnWait == null)
			{
				InteractiveGatheringNode.m_despawnWait = new WaitForSeconds(10f);
				InteractiveGatheringNode.m_endInteractionsWait = new WaitForSeconds(5f);
			}
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x000831B6 File Offset: 0x000813B6
		protected override void Start()
		{
			base.Start();
			this.m_interactiveFlags.Value |= InteractiveFlags.Interactive;
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void GenerateRecord(GameEntity interactionSource)
		{
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x00207660 File Offset: 0x00205860
		public override bool CanInteract(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			return base.CanInteract(entity) && base.GatheringParams.EntityCanInteract(entity, this, out archetypeInstance);
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x0020768C File Offset: 0x0020588C
		private bool CanInteractGatheringNode(GameEntity entity, out ArchetypeInstance requiredItemInstance)
		{
			requiredItemInstance = null;
			return base.CanInteract(entity) && base.GatheringParams.EntityCanInteract(entity, this, out requiredItemInstance);
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x002076B8 File Offset: 0x002058B8
		public override bool ClientInteraction()
		{
			bool result = false;
			if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.RecordGenerated))
			{
				result = base.ClientInteraction();
			}
			else if (!LocalPlayer.GameEntity.SkillsController.PendingIsActive && this.CanInteract(LocalPlayer.GameEntity))
			{
				ArchetypeInstance abilityInstance = base.GatheringParams.GetAbilityInstance(LocalPlayer.GameEntity);
				if (abilityInstance != null)
				{
					LocalPlayer.GameEntity.CollectionController.GatheringNode = this;
					LocalPlayer.GameEntity.SkillsController.BeginExecution(abilityInstance);
					this.StopSurvey();
					result = true;
				}
				else
				{
					string content = (base.GatheringParams.RequiredTool == CraftingToolType.None) ? "Unknown Error" : ("You must know " + base.GatheringParams.GetGatheringMasteryDisplayName() + " to harvest!");
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
			}
			return result;
		}

		// Token: 0x060063B8 RID: 25528 RVA: 0x000831D1 File Offset: 0x000813D1
		public override void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			base.EndInteraction(interactionSource, clientIsEnding);
			if (GameManager.IsServer && this.m_record != null && this.m_destroyCo == null)
			{
				this.m_destroyCo = this.DestroyCo();
				base.StartCoroutine(this.m_destroyCo);
			}
		}

		// Token: 0x060063B9 RID: 25529 RVA: 0x0008320B File Offset: 0x0008140B
		private IEnumerator DestroyCo()
		{
			float t = 0f;
			while (t < 60f)
			{
				if (this.m_record.IsEmpty())
				{
					this.m_interactiveFlags.Value &= ~InteractiveFlags.Interactive;
					yield return InteractiveGatheringNode.m_despawnWait;
					IInteractive interactive = base.GameEntity.Interactive;
					if (interactive != null)
					{
						interactive.EndAllInteractions();
					}
					yield return InteractiveGatheringNode.m_endInteractionsWait;
					ISpawnController spawnController = this.SpawnController;
					if (spawnController != null)
					{
						spawnController.NotifyOfDeath(base.GameEntity);
					}
					UnityEngine.Object.Destroy(base.GameEntity.gameObject);
					yield break;
				}
				t += Time.deltaTime;
				yield return null;
			}
			IInteractive interactive2 = base.GameEntity.Interactive;
			if (interactive2 != null)
			{
				interactive2.EndAllInteractions();
			}
			yield return InteractiveGatheringNode.m_endInteractionsWait;
			ISpawnController spawnController2 = this.SpawnController;
			if (spawnController2 != null)
			{
				spawnController2.NotifyOfDeath(base.GameEntity);
			}
			UnityEngine.Object.Destroy(base.GameEntity.gameObject);
			yield break;
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x0008321A File Offset: 0x0008141A
		public void ActivateSurvey(float duration)
		{
			if (this.m_surveyVisuals != null)
			{
				if (this.m_surveyCo != null)
				{
					base.StopCoroutine(this.m_surveyCo);
				}
				this.m_surveyCo = this.SurveyCo(duration);
				base.StartCoroutine(this.m_surveyCo);
			}
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x00083258 File Offset: 0x00081458
		private IEnumerator SurveyCo(float duration)
		{
			this.m_surveyVisuals.SetActive(true);
			yield return new WaitForSeconds(duration);
			this.m_surveyVisuals.SetActive(false);
			this.m_surveyCo = null;
			yield break;
		}

		// Token: 0x060063BC RID: 25532 RVA: 0x0008326E File Offset: 0x0008146E
		private void StopSurvey()
		{
			if (this.m_surveyCo != null)
			{
				base.StopCoroutine(this.m_surveyCo);
				this.m_surveyVisuals.SetActive(false);
			}
		}

		// Token: 0x170017E0 RID: 6112
		// (get) Token: 0x060063BD RID: 25533 RVA: 0x0020778C File Offset: 0x0020598C
		protected override string TooltipDescription
		{
			get
			{
				return ZString.Format<string, string>("{0}\n{1}", this.Description, base.GatheringParams.GetTooltipDescription(this));
			}
		}

		// Token: 0x060063BE RID: 25534 RVA: 0x002077B8 File Offset: 0x002059B8
		private CursorType GetCursorType(bool isActive)
		{
			if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Destroy))
			{
				return CursorType.MainCursor;
			}
			if (!this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.RecordGenerated))
			{
				if (isActive)
				{
					ArchetypeInstance archetypeInstance;
					isActive = base.GatheringParams.EntityCanInteract(LocalPlayer.GameEntity, this, out archetypeInstance);
				}
				return base.GatheringParams.GetCursor(isActive);
			}
			if (!isActive)
			{
				return CursorType.MerchantCursorInactive;
			}
			return CursorType.MerchantCursor;
		}

		// Token: 0x170017E1 RID: 6113
		// (get) Token: 0x060063BF RID: 25535 RVA: 0x00083290 File Offset: 0x00081490
		protected override CursorType ActiveCursorType
		{
			get
			{
				return this.GetCursorType(true);
			}
		}

		// Token: 0x170017E2 RID: 6114
		// (get) Token: 0x060063C0 RID: 25536 RVA: 0x00083299 File Offset: 0x00081499
		protected override CursorType InactiveCursorType
		{
			get
			{
				return this.GetCursorType(false);
			}
		}

		// Token: 0x170017E3 RID: 6115
		// (get) Token: 0x060063C1 RID: 25537 RVA: 0x0007DA28 File Offset: 0x0007BC28
		bool ILootRollSource.LootRollIsPending
		{
			get
			{
				return this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.LootRollPending);
			}
		}

		// Token: 0x060063C2 RID: 25538 RVA: 0x000832A2 File Offset: 0x000814A2
		void ILootRollSource.SetLootRollCount(int count)
		{
			if (count > 0)
			{
				this.m_lootRollCount = new int?(count);
				this.m_interactiveFlags.Value |= InteractiveFlags.LootRollPending;
			}
		}

		// Token: 0x060063C3 RID: 25539 RVA: 0x00207820 File Offset: 0x00205A20
		void ILootRollSource.LootRollCompleted()
		{
			if (this.m_lootRollCount != null)
			{
				this.m_lootRollCount--;
				if (this.m_lootRollCount.Value <= 0)
				{
					this.m_lootRollCount = null;
					this.m_interactiveFlags.Value &= ~InteractiveFlags.LootRollPending;
				}
			}
		}

		// Token: 0x060063C4 RID: 25540 RVA: 0x0007DA62 File Offset: 0x0007BC62
		void ILootRollSource.RemoveFromRecord(ArchetypeInstance instance)
		{
			base.RemoveFromRecordIfPresent(instance);
		}

		// Token: 0x170017E4 RID: 6116
		// (get) Token: 0x060063C5 RID: 25541 RVA: 0x000832C8 File Offset: 0x000814C8
		bool ILootRollSource.IsRaid
		{
			get
			{
				return this.m_isRaid;
			}
		}

		// Token: 0x170017E5 RID: 6117
		// (get) Token: 0x060063C6 RID: 25542 RVA: 0x00045BCA File Offset: 0x00043DCA
		ChallengeRating ILootRollSource.ChallengeRating
		{
			get
			{
				return ChallengeRating.CR0;
			}
		}

		// Token: 0x060063C7 RID: 25543 RVA: 0x0020789C File Offset: 0x00205A9C
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_description);
			this.m_description.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x040056A7 RID: 22183
		[SerializeField]
		private GameObject m_surveyVisuals;

		// Token: 0x040056A8 RID: 22184
		private readonly SynchronizedString m_description = new SynchronizedString();

		// Token: 0x040056A9 RID: 22185
		private const float kTimeToWaitAfterFirstLoot = 60f;

		// Token: 0x040056AA RID: 22186
		private static WaitForSeconds m_despawnWait;

		// Token: 0x040056AB RID: 22187
		private static WaitForSeconds m_endInteractionsWait;

		// Token: 0x040056AC RID: 22188
		private IEnumerator m_destroyCo;

		// Token: 0x040056AD RID: 22189
		private IEnumerator m_surveyCo;

		// Token: 0x040056AE RID: 22190
		private bool m_isRaid;

		// Token: 0x040056AF RID: 22191
		private int? m_lootRollCount;
	}
}
