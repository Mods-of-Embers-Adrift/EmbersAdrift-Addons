using System;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A0 RID: 1952
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/EmberEssenceObjective")]
	public class EmberEssenceObjective : QuestObjective
	{
		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x06003991 RID: 14737 RVA: 0x00066F91 File Offset: 0x00065191
		public int RequiredEssence
		{
			get
			{
				return this.m_requiredEssence;
			}
		}

		// Token: 0x06003992 RID: 14738 RVA: 0x00066F99 File Offset: 0x00065199
		public override bool Validate(GameEntity entity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			if (!this.HasEnoughEssence(entity))
			{
				message = "Not enough ember essence!";
				return false;
			}
			message = string.Empty;
			return true;
		}

		// Token: 0x06003993 RID: 14739 RVA: 0x001737CC File Offset: 0x001719CC
		public override void OnEntityInitializedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityInitializedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_previousAmount = Math.Min(LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount(), this.m_requiredEssence);
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(questOrTaskId, out quest))
			{
				this.m_muted = quest.IsMuted(sourceEntity);
				if (!quest.IsMuted(sourceEntity))
				{
					LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.OnEmberStoneChanged;
				}
			}
		}

		// Token: 0x06003994 RID: 14740 RVA: 0x00066FC7 File Offset: 0x000651C7
		public override void OnEntityDestroyedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityDestroyedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.OnEmberStoneChanged;
		}

		// Token: 0x06003995 RID: 14741 RVA: 0x0017384C File Offset: 0x00171A4C
		public override void OnActivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnActivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest))
			{
				this.m_muted = quest.IsMuted(sourceEntity);
			}
			int emberEssenceCount = LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount();
			if (emberEssenceCount > 0)
			{
				this.m_previousAmount = 0;
				this.AnnounceCountChange();
			}
			else
			{
				this.m_previousAmount = Math.Min(emberEssenceCount, this.m_requiredEssence);
			}
			LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.OnEmberStoneChanged;
		}

		// Token: 0x06003996 RID: 14742 RVA: 0x00066FF4 File Offset: 0x000651F4
		public override void OnDeactivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnDeactivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.OnEmberStoneChanged;
		}

		// Token: 0x06003997 RID: 14743 RVA: 0x00067021 File Offset: 0x00065221
		public override void OnMuteChanged(GameEntity sourceEntity, bool mute)
		{
			base.OnMuteChanged(sourceEntity, mute);
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_muted = mute;
		}

		// Token: 0x06003998 RID: 14744 RVA: 0x0006703A File Offset: 0x0006523A
		public bool HasEnoughEssence(GameEntity entity)
		{
			return entity.CollectionController.GetEmberEssenceCount() >= this.m_requiredEssence;
		}

		// Token: 0x06003999 RID: 14745 RVA: 0x00067052 File Offset: 0x00065252
		private void OnEmberStoneChanged()
		{
			this.AnnounceCountChange();
		}

		// Token: 0x0600399A RID: 14746 RVA: 0x001738DC File Offset: 0x00171ADC
		private void AnnounceCountChange()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			int emberEssenceCount = LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount();
			if (Math.Min(emberEssenceCount, this.m_requiredEssence) != this.m_previousAmount)
			{
				this.m_previousAmount = Math.Min(emberEssenceCount, this.m_requiredEssence);
				if (!this.m_muted)
				{
					ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
					{
						Title = string.Format("{0} {1}/{2}", base.Description, Math.Min(emberEssenceCount, this.m_requiredEssence), this.m_requiredEssence),
						Text = null,
						TimeShown = 5f,
						ShowDelay = 0f,
						SourceId = new UniqueId?(base.Id)
					});
				}
			}
		}

		// Token: 0x04003843 RID: 14403
		[Min(0f)]
		[SerializeField]
		private int m_requiredEssence;

		// Token: 0x04003844 RID: 14404
		[SerializeField]
		private bool m_removeEssence;

		// Token: 0x04003845 RID: 14405
		private bool m_muted;

		// Token: 0x04003846 RID: 14406
		private int m_previousAmount;
	}
}
