using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C4 RID: 196
	public class LightningBoltShapeConeScript : LightningBoltPrefabScriptBase
	{
		// Token: 0x0600074D RID: 1869 RVA: 0x000ACA0C File Offset: 0x000AAC0C
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * this.InnerRadius;
			Vector3 start = base.transform.rotation * new Vector3(vector.x, vector.y, 0f);
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * this.OuterRadius;
			Vector3 end = base.transform.rotation * new Vector3(vector2.x, vector2.y, 0f) + base.transform.forward * this.Length;
			parameters.Start = start;
			parameters.End = end;
			base.CreateLightningBolt(parameters);
		}

		// Token: 0x040008AF RID: 2223
		[Header("Lightning Cone Properties")]
		[Tooltip("Radius at base of cone where lightning can emit from")]
		public float InnerRadius = 0.1f;

		// Token: 0x040008B0 RID: 2224
		[Tooltip("Radius at outer part of the cone where lightning emits to")]
		public float OuterRadius = 4f;

		// Token: 0x040008B1 RID: 2225
		[Tooltip("The length of the cone from the center of the inner and outer circle")]
		public float Length = 4f;
	}
}
