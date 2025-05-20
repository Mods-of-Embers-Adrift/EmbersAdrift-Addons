using System;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000090 RID: 144
	[Serializable]
	public class IvyParameterFloat : IvyParameter
	{
		// Token: 0x060005BB RID: 1467 RVA: 0x00046F93 File Offset: 0x00045193
		public IvyParameterFloat(float value)
		{
			this.value = value;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00046FA2 File Offset: 0x000451A2
		public override void UpdateValue(float value)
		{
			this.value = value;
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x00046FAB File Offset: 0x000451AB
		public static implicit operator float(IvyParameterFloat floatParameter)
		{
			return floatParameter.value;
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00046FB3 File Offset: 0x000451B3
		public static implicit operator IvyParameterFloat(float floatValue)
		{
			return new IvyParameterFloat(floatValue);
		}
	}
}
