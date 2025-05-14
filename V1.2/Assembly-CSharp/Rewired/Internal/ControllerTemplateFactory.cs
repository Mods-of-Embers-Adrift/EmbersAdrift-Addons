using System;

namespace Rewired.Internal
{
	// Token: 0x0200006E RID: 110
	public static class ControllerTemplateFactory
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x00046109 File Offset: 0x00044309
		public static Type[] templateTypes
		{
			get
			{
				return ControllerTemplateFactory._defaultTemplateTypes;
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x00046110 File Offset: 0x00044310
		public static Type[] templateInterfaceTypes
		{
			get
			{
				return ControllerTemplateFactory._defaultTemplateInterfaceTypes;
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0009C578 File Offset: 0x0009A778
		public static IControllerTemplate Create(Guid typeGuid, object payload)
		{
			if (typeGuid == GamepadTemplate.typeGuid)
			{
				return new GamepadTemplate(payload);
			}
			if (typeGuid == RacingWheelTemplate.typeGuid)
			{
				return new RacingWheelTemplate(payload);
			}
			if (typeGuid == HOTASTemplate.typeGuid)
			{
				return new HOTASTemplate(payload);
			}
			if (typeGuid == FlightYokeTemplate.typeGuid)
			{
				return new FlightYokeTemplate(payload);
			}
			if (typeGuid == FlightPedalsTemplate.typeGuid)
			{
				return new FlightPedalsTemplate(payload);
			}
			if (typeGuid == SixDofControllerTemplate.typeGuid)
			{
				return new SixDofControllerTemplate(payload);
			}
			return null;
		}

		// Token: 0x04000572 RID: 1394
		private static readonly Type[] _defaultTemplateTypes = new Type[]
		{
			typeof(GamepadTemplate),
			typeof(RacingWheelTemplate),
			typeof(HOTASTemplate),
			typeof(FlightYokeTemplate),
			typeof(FlightPedalsTemplate),
			typeof(SixDofControllerTemplate)
		};

		// Token: 0x04000573 RID: 1395
		private static readonly Type[] _defaultTemplateInterfaceTypes = new Type[]
		{
			typeof(IGamepadTemplate),
			typeof(IRacingWheelTemplate),
			typeof(IHOTASTemplate),
			typeof(IFlightYokeTemplate),
			typeof(IFlightPedalsTemplate),
			typeof(ISixDofControllerTemplate)
		};
	}
}
