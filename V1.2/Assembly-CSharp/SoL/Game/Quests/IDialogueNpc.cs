using System;

namespace SoL.Game.Quests
{
	// Token: 0x02000782 RID: 1922
	public interface IDialogueNpc
	{
		// Token: 0x060038C8 RID: 14536
		bool CanConverseWith(GameEntity entity);

		// Token: 0x060038C9 RID: 14537
		bool InitiateDialogue();
	}
}
