using System;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Game.Targeting;
using SoL.Game.UI.Dialog;
using SoL.Managers;
using SoL.Utilities;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B9C RID: 2972
	public class InteractiveMerchantNpc : InteractiveMerchant, IDialogueNpc
	{
		// Token: 0x17001575 RID: 5493
		// (get) Token: 0x06005B84 RID: 23428 RVA: 0x0007D6BD File Offset: 0x0007B8BD
		// (set) Token: 0x06005B85 RID: 23429 RVA: 0x0007D6C5 File Offset: 0x0007B8C5
		public MerchantBundleCollection MerchantBundleCollection
		{
			get
			{
				return this.m_merchantBundleCollection;
			}
			set
			{
				this.m_merchantBundleCollection = value;
			}
		}

		// Token: 0x17001576 RID: 5494
		// (get) Token: 0x06005B86 RID: 23430 RVA: 0x0007D6CE File Offset: 0x0007B8CE
		// (set) Token: 0x06005B87 RID: 23431 RVA: 0x0007D6D6 File Offset: 0x0007B8D6
		public MerchantBundle[] MerchantBundles
		{
			get
			{
				return this.m_merchantBundles;
			}
			set
			{
				this.m_merchantBundles = value;
			}
		}

		// Token: 0x06005B88 RID: 23432 RVA: 0x0007D372 File Offset: 0x0007B572
		public bool CanConverseWith(GameEntity entity)
		{
			return base.IsWithinDistance(entity) && base.GameEntity.CharacterData.Faction.GetPlayerTargetType() == TargetType.Defensive;
		}

		// Token: 0x06005B89 RID: 23433 RVA: 0x001EEF98 File Offset: 0x001ED198
		protected void Start()
		{
			if (!GameManager.IsServer)
			{
				NpcProfile npcProfile;
				InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
				DialogueSource dialogueSource2;
				DialogueSource dialogueSource = InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource2) ? dialogueSource2 : npcProfile;
				if (dialogueSource != null && dialogueSource.HasDefaultDialogue)
				{
					this.m_hasDialogue = true;
					return;
				}
				if (dialogueSource != null)
				{
					GameManager.QuestManager.QuestsUpdated += this.OnQuestsUpdated;
					this.OnQuestsUpdated();
				}
			}
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x001EF038 File Offset: 0x001ED238
		private void OnQuestsUpdated()
		{
			NpcProfile npcProfile;
			InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
			DialogueSource dialogueSource;
			if ((InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource) ? dialogueSource : npcProfile).HasQuestDialogue)
			{
				this.m_hasDialogue = true;
				return;
			}
			this.m_hasDialogue = false;
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x0007D6DF File Offset: 0x0007B8DF
		public override bool ClientInteraction()
		{
			return (this.m_hasDialogue && this.InitiateDialogue()) || base.ClientInteraction();
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x0007D3B2 File Offset: 0x0007B5B2
		public void OpenMerchant()
		{
			base.ClientInteraction();
			DialogueManager.TerminateDialogue();
		}

		// Token: 0x06005B8D RID: 23437 RVA: 0x001EE340 File Offset: 0x001EC540
		public bool InitiateDialogue()
		{
			if (!GameManager.IsServer)
			{
				NpcProfile npcProfile;
				InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
				DialogueSource dialogueSource2;
				DialogueSource dialogueSource = InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource2) ? dialogueSource2 : npcProfile;
				if (dialogueSource.HasAnyDialogue)
				{
					DialogueManager.InitiateDialogue(dialogueSource, this, null, DialogSourceType.NPC);
					return true;
				}
			}
			return false;
		}

		// Token: 0x17001577 RID: 5495
		// (get) Token: 0x06005B8E RID: 23438 RVA: 0x0007D6F9 File Offset: 0x0007B8F9
		protected override CursorType ActiveCursorType
		{
			get
			{
				if (!this.m_hasDialogue)
				{
					return base.ActiveCursorType;
				}
				return CursorType.TextCursor;
			}
		}

		// Token: 0x17001578 RID: 5496
		// (get) Token: 0x06005B8F RID: 23439 RVA: 0x0007D70C File Offset: 0x0007B90C
		protected override CursorType InactiveCursorType
		{
			get
			{
				if (!this.m_hasDialogue)
				{
					return base.InactiveCursorType;
				}
				return CursorType.TextCursorInactive;
			}
		}

		// Token: 0x04004FE1 RID: 20449
		private bool m_hasDialogue;
	}
}
