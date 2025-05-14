using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C7B RID: 3195
	public static class EffectDescriptorExtensions
	{
		// Token: 0x06006176 RID: 24950 RVA: 0x00081B46 File Offset: 0x0007FD46
		public static bool ValidTargetForKinematicEffectType(this KinematicEffectTypes kinematicEffectType, EffectTargetType targetType)
		{
			return targetType - EffectTargetType.Offensive <= 2 || (targetType - EffectTargetType.Defensive <= 2 && kinematicEffectType == KinematicEffectTypes.MoveForwardTo);
		}

		// Token: 0x06006177 RID: 24951 RVA: 0x00081B5E File Offset: 0x0007FD5E
		public static StatusEffectSubType GetResistChannel(this KinematicEffectTypes channel)
		{
			if (channel - KinematicEffectTypes.PullTargetTo <= 1)
			{
				return StatusEffectSubType.StatusEffectResist_Kinematic;
			}
			throw new ArgumentException("channel");
		}
	}
}
