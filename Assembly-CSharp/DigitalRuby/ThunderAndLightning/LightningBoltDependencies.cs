using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000B2 RID: 178
	public class LightningBoltDependencies
	{
		// Token: 0x040007DA RID: 2010
		public GameObject Parent;

		// Token: 0x040007DB RID: 2011
		public Material LightningMaterialMesh;

		// Token: 0x040007DC RID: 2012
		public Material LightningMaterialMeshNoGlow;

		// Token: 0x040007DD RID: 2013
		public ParticleSystem OriginParticleSystem;

		// Token: 0x040007DE RID: 2014
		public ParticleSystem DestParticleSystem;

		// Token: 0x040007DF RID: 2015
		public Vector3 CameraPos;

		// Token: 0x040007E0 RID: 2016
		public bool CameraIsOrthographic;

		// Token: 0x040007E1 RID: 2017
		public CameraMode CameraMode;

		// Token: 0x040007E2 RID: 2018
		public bool UseWorldSpace;

		// Token: 0x040007E3 RID: 2019
		public float LevelOfDetailDistance;

		// Token: 0x040007E4 RID: 2020
		public string SortLayerName;

		// Token: 0x040007E5 RID: 2021
		public int SortOrderInLayer;

		// Token: 0x040007E6 RID: 2022
		public ICollection<LightningBoltParameters> Parameters;

		// Token: 0x040007E7 RID: 2023
		public LightningThreadState ThreadState;

		// Token: 0x040007E8 RID: 2024
		public Func<IEnumerator, Coroutine> StartCoroutine;

		// Token: 0x040007E9 RID: 2025
		public Action<Light> LightAdded;

		// Token: 0x040007EA RID: 2026
		public Action<Light> LightRemoved;

		// Token: 0x040007EB RID: 2027
		public Action<LightningBolt> AddActiveBolt;

		// Token: 0x040007EC RID: 2028
		public Action<LightningBoltDependencies> ReturnToCache;

		// Token: 0x040007ED RID: 2029
		public Action<LightningBoltParameters, Vector3, Vector3> LightningBoltStarted;

		// Token: 0x040007EE RID: 2030
		public Action<LightningBoltParameters, Vector3, Vector3> LightningBoltEnded;
	}
}
