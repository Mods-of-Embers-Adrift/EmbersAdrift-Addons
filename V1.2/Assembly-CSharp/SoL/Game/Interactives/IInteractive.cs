using System;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B8F RID: 2959
	public interface IInteractive : IInteractiveBase
	{
		// Token: 0x1700155A RID: 5466
		// (get) Token: 0x06005B37 RID: 23351
		bool RequiresLos { get; }

		// Token: 0x06005B38 RID: 23352
		bool ClientInteraction();

		// Token: 0x06005B39 RID: 23353
		bool CanInteract(GameEntity entity);

		// Token: 0x06005B3A RID: 23354
		void BeginInteraction(GameEntity interactionSource);

		// Token: 0x06005B3B RID: 23355
		void EndInteraction(GameEntity interactionSource, bool clientIsEnding);

		// Token: 0x06005B3C RID: 23356
		void EndAllInteractions();
	}
}
