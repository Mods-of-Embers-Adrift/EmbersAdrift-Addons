using System;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A3A RID: 2618
	public static class EmberStoneExtensions
	{
		// Token: 0x17001216 RID: 4630
		// (get) Token: 0x060050F6 RID: 20726 RVA: 0x00076203 File Offset: 0x00074403
		private static EmberStoneFillLevels[] FillLevels
		{
			get
			{
				if (EmberStoneExtensions.m_fillLevels == null)
				{
					EmberStoneExtensions.m_fillLevels = (EmberStoneFillLevels[])Enum.GetValues(typeof(EmberStoneFillLevels));
				}
				return EmberStoneExtensions.m_fillLevels;
			}
		}

		// Token: 0x060050F7 RID: 20727 RVA: 0x001CE07C File Offset: 0x001CC27C
		public static EmberStoneFillLevels GetFillLevel(float fraction)
		{
			if (fraction >= 1f)
			{
				return EmberStoneFillLevels.Full;
			}
			if (fraction <= 0f)
			{
				return EmberStoneFillLevels.Empty;
			}
			int num = Mathf.FloorToInt(fraction * 100f);
			for (int i = 0; i < EmberStoneExtensions.FillLevels.Length - 1; i++)
			{
				if (num > (int)EmberStoneExtensions.FillLevels[i] && num <= (int)EmberStoneExtensions.FillLevels[i + 1])
				{
					return EmberStoneExtensions.FillLevels[i + 1];
				}
			}
			return EmberStoneFillLevels.Empty;
		}

		// Token: 0x060050F8 RID: 20728 RVA: 0x0007622A File Offset: 0x0007442A
		public static EmberStoneFillLevels GetFillLevel(this EmberStone stoneItem, int count)
		{
			return EmberStoneExtensions.GetFillLevel((float)count / (float)stoneItem.MaxCapacity);
		}

		// Token: 0x060050F9 RID: 20729 RVA: 0x001CE0E0 File Offset: 0x001CC2E0
		public static Color GetEmissiveColor(this EmberStoneFillLevels fillLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
			Color color = Color.white;
			if (fillLevel == EmberStoneFillLevels.Empty)
			{
				color = Color.black;
			}
			else
			{
				color *= (float)fillLevel / 100f;
				color *= GlobalSettings.Values.Ashen.GetGlowIntensity(alchemyPowerLevel);
			}
			return color;
		}

		// Token: 0x0400487F RID: 18559
		public const int kSlotIndex = 512;

		// Token: 0x04004880 RID: 18560
		private static EmberStoneFillLevels[] m_fillLevels;
	}
}
