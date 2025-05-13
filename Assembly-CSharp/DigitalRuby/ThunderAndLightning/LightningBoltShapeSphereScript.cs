using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C5 RID: 197
	public class LightningBoltShapeSphereScript : LightningBoltPrefabScriptBase
	{
		// Token: 0x0600074F RID: 1871 RVA: 0x000ACABC File Offset: 0x000AACBC
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			Vector3 start = UnityEngine.Random.insideUnitSphere * this.InnerRadius;
			Vector3 end = UnityEngine.Random.onUnitSphere * this.Radius;
			parameters.Start = start;
			parameters.End = end;
			base.CreateLightningBolt(parameters);
		}

		// Token: 0x040008B2 RID: 2226
		[Header("Lightning Sphere Properties")]
		[Tooltip("Radius inside the sphere where lightning can emit from")]
		public float InnerRadius = 0.1f;

		// Token: 0x040008B3 RID: 2227
		[Tooltip("Radius of the sphere")]
		public float Radius = 4f;
	}
}
