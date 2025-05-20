using System;

namespace SoL.Game.Animation
{
	// Token: 0x02000D71 RID: 3441
	public class AnimationEventHandlerHandheldForward : AnimationEventHandler
	{
		// Token: 0x060067AC RID: 26540 RVA: 0x00085A83 File Offset: 0x00083C83
		protected override void AttachWeapons()
		{
			base.AttachWeapons();
			if (base.GameEntity && base.GameEntity.HandheldMountController)
			{
				base.GameEntity.HandheldMountController.AttachWeapons();
			}
		}

		// Token: 0x060067AD RID: 26541 RVA: 0x00085ABA File Offset: 0x00083CBA
		protected override void DetachWeapons()
		{
			base.DetachWeapons();
			if (base.GameEntity && base.GameEntity.HandheldMountController)
			{
				base.GameEntity.HandheldMountController.DetachWeapons();
			}
		}

		// Token: 0x060067AE RID: 26542 RVA: 0x00085AF1 File Offset: 0x00083CF1
		protected override void AttachLight()
		{
			base.AttachLight();
			if (base.GameEntity && base.GameEntity.HandheldMountController)
			{
				base.GameEntity.HandheldMountController.AttachLight();
			}
		}

		// Token: 0x060067AF RID: 26543 RVA: 0x00085B28 File Offset: 0x00083D28
		protected override void DetachLight()
		{
			base.DetachLight();
			if (base.GameEntity && base.GameEntity.HandheldMountController)
			{
				base.GameEntity.HandheldMountController.DetachLight();
			}
		}
	}
}
