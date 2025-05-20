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
	// Token: 0x02000B8A RID: 2954
	public class InteractiveEssenceConverterNpc : InteractiveEssenceConverter, IDialogueNpc
	{
		// Token: 0x06005B14 RID: 23316 RVA: 0x0007D372 File Offset: 0x0007B572
		public bool CanConverseWith(GameEntity entity)
		{
			return base.IsWithinDistance(entity) && base.GameEntity.CharacterData.Faction.GetPlayerTargetType() == TargetType.Defensive;
		}

		// Token: 0x06005B15 RID: 23317 RVA: 0x001EE234 File Offset: 0x001EC434
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

		// Token: 0x06005B16 RID: 23318 RVA: 0x001EE2D4 File Offset: 0x001EC4D4
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

		// Token: 0x06005B17 RID: 23319 RVA: 0x0007D398 File Offset: 0x0007B598
		public override bool ClientInteraction()
		{
			return (this.m_hasDialogue && this.InitiateDialogue()) || base.ClientInteraction();
		}

		// Token: 0x06005B18 RID: 23320 RVA: 0x0007D3B2 File Offset: 0x0007B5B2
		public void OpenEssenceConverter()
		{
			base.ClientInteraction();
			DialogueManager.TerminateDialogue();
		}

		// Token: 0x06005B19 RID: 23321 RVA: 0x001EE340 File Offset: 0x001EC540
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

		// Token: 0x1700154E RID: 5454
		// (get) Token: 0x06005B1A RID: 23322 RVA: 0x0007D3C0 File Offset: 0x0007B5C0
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

		// Token: 0x1700154F RID: 5455
		// (get) Token: 0x06005B1B RID: 23323 RVA: 0x0007D3D3 File Offset: 0x0007B5D3
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

		// Token: 0x04004FAD RID: 20397
		private bool m_hasDialogue;
	}
}
