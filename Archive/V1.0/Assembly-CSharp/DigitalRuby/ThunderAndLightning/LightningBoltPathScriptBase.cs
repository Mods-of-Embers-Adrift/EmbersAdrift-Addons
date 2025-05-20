using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000BE RID: 190
	public abstract class LightningBoltPathScriptBase : LightningBoltPrefabScriptBase
	{
		// Token: 0x060006FD RID: 1789 RVA: 0x000AB6F0 File Offset: 0x000A98F0
		protected List<GameObject> GetCurrentPathObjects()
		{
			this.currentPathObjects.Clear();
			if (this.LightningPath != null)
			{
				foreach (GameObject gameObject in this.LightningPath)
				{
					if (gameObject != null && gameObject.activeInHierarchy)
					{
						this.currentPathObjects.Add(gameObject);
					}
				}
			}
			return this.currentPathObjects;
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00047BF0 File Offset: 0x00045DF0
		protected override LightningBoltParameters OnCreateParameters()
		{
			LightningBoltParameters lightningBoltParameters = base.OnCreateParameters();
			lightningBoltParameters.Generator = LightningGenerator.GeneratorInstance;
			return lightningBoltParameters;
		}

		// Token: 0x04000846 RID: 2118
		[Header("Lightning Path Properties")]
		[Tooltip("The game objects to follow for the lightning path")]
		public List<GameObject> LightningPath;

		// Token: 0x04000847 RID: 2119
		private readonly List<GameObject> currentPathObjects = new List<GameObject>();
	}
}
