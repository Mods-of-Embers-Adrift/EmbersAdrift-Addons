using System;
using System.Collections;
using SoL.Utilities;
using UMA;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005E1 RID: 1505
	[CreateAssetMenu(menuName = "SoL/UMA/SlotOverlay Shader Assignment", order = 5)]
	public class SlotOverlayShaderAssignment : ScriptableObject
	{
		// Token: 0x06002FCC RID: 12236 RVA: 0x00060F66 File Offset: 0x0005F166
		private IEnumerable GetMaterials()
		{
			return SolOdinUtilities.GetDropdownItems<UMAMaterial>();
		}

		// Token: 0x04002EB4 RID: 11956
		[SerializeField]
		private SlotDataAsset[] m_slots;

		// Token: 0x04002EB5 RID: 11957
		[SerializeField]
		private OverlayDataAsset[] m_overlays;

		// Token: 0x04002EB6 RID: 11958
		[SerializeField]
		private UMAMaterial m_material;
	}
}
