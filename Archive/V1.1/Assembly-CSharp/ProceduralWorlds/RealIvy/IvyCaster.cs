using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000086 RID: 134
	public class IvyCaster : MonoBehaviour
	{
		// Token: 0x06000560 RID: 1376 RVA: 0x0009F8E8 File Offset: 0x0009DAE8
		public void CastIvyByPresetName(string presetName, Vector3 position, Quaternion rotation)
		{
			IvyPreset presetByName = this.GetPresetByName(presetName);
			this.CastIvy(presetByName, position, rotation);
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0009F908 File Offset: 0x0009DB08
		public void CastIvy(IvyPreset ivyPreset, Vector3 position, Quaternion rotation)
		{
			IvyController ivyController = this.GetFreeIvy();
			if (ivyController == null)
			{
				IvyController ivyController2 = UnityEngine.Object.Instantiate<IvyController>(this.prefabIvyController);
				ivyController2.transform.parent = base.transform;
				ivyController = ivyController2;
				this.ivys.Add(ivyController2);
			}
			ivyController.transform.position = position;
			ivyController.transform.rotation = rotation;
			ivyController.transform.Rotate(Vector3.right, -90f);
			ivyController.ivyParameters = ivyPreset.ivyParameters;
			ivyController.gameObject.SetActive(true);
			ivyController.StartGrowth();
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0009F99C File Offset: 0x0009DB9C
		public void CastRandomIvy(Vector3 position, Quaternion rotation)
		{
			int num = UnityEngine.Random.Range(0, this.ivyPresets.Length);
			IvyPreset ivyPreset = this.ivyPresets[num];
			this.CastIvy(ivyPreset, position, rotation);
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0009F9CC File Offset: 0x0009DBCC
		private IvyController GetFreeIvy()
		{
			IvyController result = null;
			for (int i = 0; i < this.ivys.Count; i++)
			{
				if (!this.ivys[i].gameObject.activeSelf)
				{
					result = this.ivys[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0009FA1C File Offset: 0x0009DC1C
		private IvyPreset GetPresetByName(string presetName)
		{
			IvyPreset result = null;
			for (int i = 0; i < this.ivyPresets.Length; i++)
			{
				if (this.ivyPresets[i].name == presetName)
				{
					result = this.ivyPresets[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x040005FA RID: 1530
		public IvyPreset[] ivyPresets;

		// Token: 0x040005FB RID: 1531
		public List<IvyController> ivys;

		// Token: 0x040005FC RID: 1532
		public IvyController prefabIvyController;
	}
}
