using System;
using Cysharp.Text;
using SoL.Game.Culling;
using TMPro;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D7 RID: 2519
	public class NameplateDebug : MonoBehaviour
	{
		// Token: 0x06004CAC RID: 19628 RVA: 0x00073DBF File Offset: 0x00071FBF
		internal void Init(NameplateControllerUI controller)
		{
			this.m_controller = controller;
			if (controller.Type == NameplateControllerUI.NameplateType.Offensive)
			{
				base.gameObject.SetActive(true);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004CAD RID: 19629 RVA: 0x001BD7AC File Offset: 0x001BB9AC
		private void Update()
		{
			if (!LocalPlayer.NameplateDebug || !this.m_controller || this.m_controller.Targetable == null || !this.m_controller.Targetable.Entity || !this.m_controller.Targetable.Entity.CulledEntity)
			{
				if (this.m_canvasGroup.alpha > 0f)
				{
					this.m_canvasGroup.alpha = 0f;
				}
				return;
			}
			if (this.m_canvasGroup.alpha <= 0f)
			{
				this.m_canvasGroup.alpha = 1f;
			}
			this.UpdateCullingData();
		}

		// Token: 0x06004CAE RID: 19630 RVA: 0x001BD858 File Offset: 0x001BBA58
		private void UpdateCullingData()
		{
			CulledEntity culledEntity = this.m_controller.Targetable.Entity.CulledEntity;
			this.m_label.SetTextFormat("CULLING\n -Initialized: {0}\n -Index: {1}\n -Band: {2}\n -Flags: {3}", culledEntity.Initialized, (culledEntity.Index != null) ? culledEntity.Index.Value : -1, culledEntity.CurrentBand, culledEntity.CullingFlags);
		}

		// Token: 0x04004689 RID: 18057
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x0400468A RID: 18058
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400468B RID: 18059
		private NameplateControllerUI m_controller;
	}
}
