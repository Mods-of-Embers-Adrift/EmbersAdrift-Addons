using System;
using SoL.Game.Loot;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Replication;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA3 RID: 2979
	public abstract class InteractiveRemoteContainerSingleLooter : InteractiveRemoteContainer, ITooltip, IInteractiveBase
	{
		// Token: 0x170015BB RID: 5563
		// (get) Token: 0x06005C32 RID: 23602 RVA: 0x0007DDD8 File Offset: 0x0007BFD8
		// (set) Token: 0x06005C33 RID: 23603 RVA: 0x001F0E2C File Offset: 0x001EF02C
		public GameEntity Looter
		{
			get
			{
				return this.m_looter;
			}
			protected set
			{
				this.m_looter = value;
				if (this.m_looter == null)
				{
					if (this.CanInteractInternal())
					{
						this.m_interactiveFlags.Value |= InteractiveFlags.Interactive;
					}
					else
					{
						this.m_interactiveFlags.Value &= ~InteractiveFlags.Interactive;
					}
					this.m_currentLooter.Value = "";
					this.m_isBeingLooted = false;
					return;
				}
				this.m_interactiveFlags.Value &= ~InteractiveFlags.Interactive;
				this.m_currentLooter.Value = this.m_looter.CharacterData.Name.Value;
				this.m_isBeingLooted = true;
			}
		}

		// Token: 0x170015BC RID: 5564
		// (get) Token: 0x06005C34 RID: 23604 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ValidateOnAwake
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170015BD RID: 5565
		// (get) Token: 0x06005C35 RID: 23605 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool NullifyListOnDestroy
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170015BE RID: 5566
		// (get) Token: 0x06005C36 RID: 23606 RVA: 0x0007DDE0 File Offset: 0x0007BFE0
		// (set) Token: 0x06005C37 RID: 23607 RVA: 0x0007DDED File Offset: 0x0007BFED
		public GatheringParameters GatheringParams
		{
			get
			{
				return this.m_gatheringParams;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_gatheringParams.Value = value;
				}
			}
		}

		// Token: 0x06005C38 RID: 23608 RVA: 0x0007DE02 File Offset: 0x0007C002
		protected virtual bool CanInteractInternal()
		{
			return this.m_record == null || (this.m_record.Instances != null && this.m_record.Instances.Count > 0);
		}

		// Token: 0x06005C39 RID: 23609
		protected abstract void GenerateRecord(GameEntity interactionSource);

		// Token: 0x06005C3A RID: 23610 RVA: 0x0007DE30 File Offset: 0x0007C030
		protected virtual void Start()
		{
			if (!GameManager.IsServer)
			{
				this.m_interactiveFlags.Changed += this.InteractiveFlagsOnChanged;
			}
		}

		// Token: 0x06005C3B RID: 23611 RVA: 0x0007DE51 File Offset: 0x0007C051
		protected virtual void Update()
		{
			if (GameManager.IsServer && this.m_isBeingLooted && this.Looter == null)
			{
				this.Looter = null;
			}
		}

		// Token: 0x06005C3C RID: 23612 RVA: 0x001F0ED8 File Offset: 0x001EF0D8
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.m_interactiveFlags.Changed -= this.InteractiveFlagsOnChanged;
			}
			if (GameManager.IsServer && this.Looter != null)
			{
				this.EndInteraction(this.Looter, false);
			}
			this.m_looter = null;
			if (this.m_record != null && NullifyMemoryLeakSettings.CleanRemoteContainer)
			{
				this.m_record.CleanupReferences();
				this.m_record = null;
			}
			base.OnDestroy();
		}

		// Token: 0x06005C3D RID: 23613 RVA: 0x0007DE77 File Offset: 0x0007C077
		public override bool ClientInteraction()
		{
			return (GameManager.IsServer || !(ClientGameManager.InputManager.MovementInput != Vector2.zero)) && base.ClientInteraction();
		}

		// Token: 0x06005C3E RID: 23614 RVA: 0x0007DE9E File Offset: 0x0007C09E
		public override bool CanInteract(GameEntity entity)
		{
			return !this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Destroy) && this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Interactive) && base.CanInteract(entity);
		}

		// Token: 0x06005C3F RID: 23615 RVA: 0x001F0F54 File Offset: 0x001EF154
		public override void BeginInteraction(GameEntity interactionSource)
		{
			base.BeginInteraction(interactionSource);
			if (!GameManager.IsServer)
			{
				return;
			}
			InteractiveFlags value = this.m_interactiveFlags.Value;
			this.GenerateRecord(interactionSource);
			if (!value.HasBitFlag(InteractiveFlags.RecordGenerated) && this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.RecordGenerated))
			{
				base.StartValidatingInteractors();
			}
			if (this.m_record != null && base.OpenRemoteContainer(interactionSource, this.m_record))
			{
				this.Looter = interactionSource;
			}
		}

		// Token: 0x06005C40 RID: 23616 RVA: 0x001F0FC4 File Offset: 0x001EF1C4
		public override void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			base.EndInteraction(interactionSource, clientIsEnding);
			base.CloseRemoteContainer(interactionSource, ContainerType.Loot.ToString(), clientIsEnding);
			if (GameManager.IsServer)
			{
				this.Looter = null;
				if (this.m_record != null && this.m_record.IsEmpty())
				{
					this.OnRecordEmpty();
				}
			}
		}

		// Token: 0x06005C41 RID: 23617 RVA: 0x0007DECF File Offset: 0x0007C0CF
		protected virtual void OnRecordEmpty()
		{
			this.m_interactiveFlags.Value |= InteractiveFlags.Destroy;
		}

		// Token: 0x06005C42 RID: 23618 RVA: 0x0007DEE4 File Offset: 0x0007C0E4
		public override void EndAllInteractions()
		{
			base.EndAllInteractions();
			if (this.Looter != null)
			{
				this.EndInteraction(this.Looter, false);
			}
			this.Looter = null;
			this.OnRecordEmpty();
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x0007DF14 File Offset: 0x0007C114
		protected virtual void InteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			if (!GameManager.IsServer && obj.HasBitFlag(InteractiveFlags.Destroy))
			{
				VisualDestructionManager.SinkEntity(base.GameEntity, 0f);
			}
		}

		// Token: 0x06005C44 RID: 23620 RVA: 0x0004475B File Offset: 0x0004295B
		protected void RemoveFromRecordIfPresent(ArchetypeInstance instance)
		{
		}

		// Token: 0x170015BF RID: 5567
		// (get) Token: 0x06005C45 RID: 23621 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hideTooltip
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170015C0 RID: 5568
		// (get) Token: 0x06005C46 RID: 23622 RVA: 0x0007DF36 File Offset: 0x0007C136
		protected virtual string TooltipDescription { get; }

		// Token: 0x06005C47 RID: 23623 RVA: 0x001F101C File Offset: 0x001EF21C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_hideTooltip)
			{
				return null;
			}
			BaseTooltip.Sb.Clear();
			BaseTooltip.Sb.AppendLine(this.TooltipDescription);
			bool flag = false;
			if (!string.IsNullOrEmpty(this.m_currentLooter.Value))
			{
				flag = (this.m_currentLooter.Value == LocalPlayer.GameEntity.CharacterData.Name.Value);
				if (flag)
				{
					BaseTooltip.Sb.AppendLine("You are currently looting this");
				}
				else
				{
					BaseTooltip.Sb.AppendLine("Currently being looted by " + this.m_currentLooter.Value);
				}
			}
			PlayerFlags value = LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value;
			if (value.BlockRemoteContainerInteractions(base.PreventInteractionsWhileMissingBag, false))
			{
				if (base.PreventInteractionsWhileMissingBag && value.HasBitFlag(PlayerFlags.MissingBag))
				{
					BaseTooltip.Sb.AppendLine("You do not have your bag!");
				}
				if (value.HasBitFlag(PlayerFlags.InTrade))
				{
					BaseTooltip.Sb.AppendLine("You are currently part of a trade!");
				}
				if (value.HasBitFlag(PlayerFlags.RemoteContainer) && !flag)
				{
					BaseTooltip.Sb.AppendLine("You already have a remote container open!");
				}
			}
			this.AddContextualTooltipText();
			return new ObjectTextTooltipParameter(this, BaseTooltip.Sb.ToString(), false);
		}

		// Token: 0x06005C48 RID: 23624 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AddContextualTooltipText()
		{
		}

		// Token: 0x170015C1 RID: 5569
		// (get) Token: 0x06005C49 RID: 23625 RVA: 0x0007DF3E File Offset: 0x0007C13E
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015C2 RID: 5570
		// (get) Token: 0x06005C4A RID: 23626 RVA: 0x0007DF4C File Offset: 0x0007C14C
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015C3 RID: 5571
		// (get) Token: 0x06005C4B RID: 23627 RVA: 0x0007CE7E File Offset: 0x0007B07E
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x170015C4 RID: 5572
		// (get) Token: 0x06005C4C RID: 23628 RVA: 0x0007CE82 File Offset: 0x0007B082
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MerchantCursorInactive;
			}
		}

		// Token: 0x06005C4D RID: 23629 RVA: 0x0004475B File Offset: 0x0004295B
		protected void LogLootResults(string lootType, string spawnProfileName, string objectName, int level)
		{
		}

		// Token: 0x06005C4E RID: 23630 RVA: 0x001F1158 File Offset: 0x001EF358
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_currentLooter);
			this.m_currentLooter.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_gatheringParams);
			this.m_gatheringParams.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_interactiveFlags);
			this.m_interactiveFlags.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x06005C50 RID: 23632 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400500C RID: 20492
		private GameEntity m_looter;

		// Token: 0x0400500D RID: 20493
		protected readonly SynchronizedEnum<InteractiveFlags> m_interactiveFlags = new SynchronizedEnum<InteractiveFlags>();

		// Token: 0x0400500E RID: 20494
		protected readonly SynchronizedString m_currentLooter = new SynchronizedString();

		// Token: 0x0400500F RID: 20495
		private readonly SynchronizedStruct<GatheringParameters> m_gatheringParams = new SynchronizedStruct<GatheringParameters>();

		// Token: 0x04005010 RID: 20496
		protected ContainerRecord m_record;

		// Token: 0x04005011 RID: 20497
		private bool m_isBeingLooted;

		// Token: 0x04005012 RID: 20498
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
