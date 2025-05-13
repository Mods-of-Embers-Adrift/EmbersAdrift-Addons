using System;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200008F RID: 143
	[Serializable]
	public class IvyParameterInt : IvyParameter
	{
		// Token: 0x060005B7 RID: 1463 RVA: 0x00046F67 File Offset: 0x00045167
		public IvyParameterInt(int value)
		{
			this.value = (float)value;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x00046F77 File Offset: 0x00045177
		public override void UpdateValue(float value)
		{
			this.value = (float)((int)value);
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00046F82 File Offset: 0x00045182
		public static implicit operator int(IvyParameterInt intParameter)
		{
			return (int)intParameter.value;
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00046F8B File Offset: 0x0004518B
		public static implicit operator IvyParameterInt(int intValue)
		{
			return new IvyParameterInt(intValue);
		}
	}
}
