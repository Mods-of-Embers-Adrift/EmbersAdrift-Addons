using System;

namespace SoL.Game.NPCs.Senses
{
	// Token: 0x02000831 RID: 2097
	public static class SensorTypeExtensions
	{
		// Token: 0x06003CC1 RID: 15553 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this SensorTypeFlags a, SensorTypeFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x0006928E File Offset: 0x0006748E
		public static SensorType GetSensorTypeForFlag(this SensorTypeFlags flags)
		{
			switch (flags)
			{
			case SensorTypeFlags.VisualImmediate:
				return SensorType.VisualImmediate;
			case SensorTypeFlags.VisualPeripheral:
				return SensorType.VisualPeripheral;
			case SensorTypeFlags.VisualImmediate | SensorTypeFlags.VisualPeripheral:
				break;
			case SensorTypeFlags.Auditory:
				return SensorType.Auditory;
			default:
				if (flags == SensorTypeFlags.Olfactory)
				{
					return SensorType.Olfactory;
				}
				break;
			}
			return SensorType.None;
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x00061025 File Offset: 0x0005F225
		public static SensorTypeFlags GetFlagForType(this SensorType sensorType)
		{
			switch (sensorType)
			{
			case SensorType.VisualImmediate:
				return SensorTypeFlags.VisualImmediate;
			case SensorType.VisualPeripheral:
				return SensorTypeFlags.VisualPeripheral;
			case SensorType.Auditory:
				return SensorTypeFlags.Auditory;
			case SensorType.Olfactory:
				return SensorTypeFlags.Olfactory;
			default:
				return SensorTypeFlags.None;
			}
		}
	}
}
