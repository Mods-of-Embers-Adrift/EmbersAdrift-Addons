using System;
using System.Collections.Generic;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004B1 RID: 1201
	public struct ProximitySensorComparer : IEqualityComparer<ProximitySensor>, IComparer<ProximitySensor>
	{
		// Token: 0x0600217D RID: 8573 RVA: 0x000583EA File Offset: 0x000565EA
		public bool Equals(ProximitySensor x, ProximitySensor y)
		{
			return x == y;
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x00050A5F File Offset: 0x0004EC5F
		public int GetHashCode(ProximitySensor obj)
		{
			return obj.GetHashCode();
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x00123FB8 File Offset: 0x001221B8
		public int Compare(ProximitySensor x, ProximitySensor y)
		{
			return x.SensorBand.CompareTo(y.SensorBand);
		}
	}
}
