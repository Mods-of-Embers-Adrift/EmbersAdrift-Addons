using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002CA RID: 714
	public class SetLayerOnAwake : MonoBehaviour
	{
		// Token: 0x060014D3 RID: 5331 RVA: 0x0005087F File Offset: 0x0004EA7F
		private void Awake()
		{
			this.ExecuteLayerSet();
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x000FB944 File Offset: 0x000F9B44
		private void ExecuteLayerSet()
		{
			int layerIndex = this.m_layer.LayerIndex;
			if (this.m_layerSource == SetLayerOnAwake.LayerSource.GlobalResource && GlobalSettings.Values != null && GlobalSettings.Values.Rendering != null && GlobalSettings.Values.Rendering.GlobalResourceNodeLayer != null)
			{
				layerIndex = GlobalSettings.Values.Rendering.GlobalResourceNodeLayer.LayerIndex;
			}
			SetLayerOnAwake.SetMethod method = this.m_method;
			if (method != SetLayerOnAwake.SetMethod.Local)
			{
				if (method != SetLayerOnAwake.SetMethod.Targeted)
				{
					return;
				}
				if (this.m_objects != null)
				{
					for (int i = 0; i < this.m_objects.Length; i++)
					{
						if (this.m_objects[i] != null)
						{
							this.m_objects[i].layer = layerIndex;
						}
					}
				}
			}
			else
			{
				if (!this.m_excludeSelf)
				{
					base.gameObject.layer = layerIndex;
				}
				if (this.m_includeChildren)
				{
					Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].gameObject.layer = layerIndex;
					}
					return;
				}
			}
		}

		// Token: 0x04001D13 RID: 7443
		[SerializeField]
		private SetLayerOnAwake.LayerSource m_layerSource;

		// Token: 0x04001D14 RID: 7444
		[SerializeField]
		private SingleUnityLayer m_layer;

		// Token: 0x04001D15 RID: 7445
		[SerializeField]
		private SetLayerOnAwake.SetMethod m_method;

		// Token: 0x04001D16 RID: 7446
		[SerializeField]
		private bool m_excludeSelf;

		// Token: 0x04001D17 RID: 7447
		[SerializeField]
		private bool m_includeChildren;

		// Token: 0x04001D18 RID: 7448
		[SerializeField]
		private GameObject[] m_objects;

		// Token: 0x020002CB RID: 715
		private enum LayerSource
		{
			// Token: 0x04001D1A RID: 7450
			Local,
			// Token: 0x04001D1B RID: 7451
			GlobalResource
		}

		// Token: 0x020002CC RID: 716
		private enum SetMethod
		{
			// Token: 0x04001D1D RID: 7453
			Local,
			// Token: 0x04001D1E RID: 7454
			Targeted
		}
	}
}
