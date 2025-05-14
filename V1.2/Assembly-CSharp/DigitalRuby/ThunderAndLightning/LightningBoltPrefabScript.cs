using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C1 RID: 193
	public class LightningBoltPrefabScript : LightningBoltPrefabScriptBase
	{
		// Token: 0x06000712 RID: 1810 RVA: 0x000ABDE4 File Offset: 0x000A9FE4
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			parameters.Start = ((this.Source == null) ? parameters.Start : this.Source.transform.position);
			parameters.End = ((this.Destination == null) ? parameters.End : this.Destination.transform.position);
			parameters.StartVariance = this.StartVariance;
			parameters.EndVariance = this.EndVariance;
			base.CreateLightningBolt(parameters);
		}

		// Token: 0x0400086F RID: 2159
		[Header("Start/end")]
		[Tooltip("The source game object, can be null")]
		public GameObject Source;

		// Token: 0x04000870 RID: 2160
		[Tooltip("The destination game object, can be null")]
		public GameObject Destination;

		// Token: 0x04000871 RID: 2161
		[Tooltip("X, Y and Z for variance from the start point. Use positive values.")]
		public Vector3 StartVariance;

		// Token: 0x04000872 RID: 2162
		[Tooltip("X, Y and Z for variance from the end point. Use positive values.")]
		public Vector3 EndVariance;
	}
}
