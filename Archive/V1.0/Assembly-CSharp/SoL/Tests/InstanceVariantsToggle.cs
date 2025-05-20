using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DAA RID: 3498
	public class InstanceVariantsToggle : MonoBehaviour
	{
		// Token: 0x060068C8 RID: 26824 RVA: 0x0008657F File Offset: 0x0008477F
		private void On()
		{
			this.ToggleAll(true);
		}

		// Token: 0x060068C9 RID: 26825 RVA: 0x00086588 File Offset: 0x00084788
		private void Off()
		{
			this.ToggleAll(false);
		}

		// Token: 0x060068CA RID: 26826 RVA: 0x00215A08 File Offset: 0x00213C08
		private void ToggleAll(bool isOn)
		{
			SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
			for (int i = 0; i < array.Length; i++)
			{
				this.ToggleInstancedForRenderer(array[i], isOn);
			}
			Debug.Log(string.Format("Toggled {0} renderers", array.Length));
		}

		// Token: 0x060068CB RID: 26827 RVA: 0x00215A4C File Offset: 0x00213C4C
		private void ToggleInstancedForRenderer(SkinnedMeshRenderer renderer, bool isOn)
		{
			if (renderer == null || !renderer.gameObject.name.Equals(this.m_searchTerm))
			{
				return;
			}
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				renderer.sharedMaterials[i].enableInstancing = isOn;
			}
		}

		// Token: 0x04005B33 RID: 23347
		[SerializeField]
		private string m_searchTerm;
	}
}
