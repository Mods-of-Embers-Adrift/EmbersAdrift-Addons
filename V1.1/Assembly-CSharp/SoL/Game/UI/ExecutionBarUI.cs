using System;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000881 RID: 2177
	public class ExecutionBarUI : MonoBehaviour
	{
		// Token: 0x06003F76 RID: 16246 RVA: 0x00188E78 File Offset: 0x00187078
		private void Update()
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.SkillsController || LocalPlayer.GameEntity.SkillsController.Pending == null || !LocalPlayer.GameEntity.SkillsController.Pending.Active || LocalPlayer.GameEntity.SkillsController.Pending.Executable == null)
			{
				this.HideIfShown();
				return;
			}
			SkillsController.PendingExecution pending = LocalPlayer.GameEntity.SkillsController.Pending;
			float num = (pending.ExecutionTime - pending.ExecutionTimeRemaining) / pending.ExecutionTime;
			this.m_label.text = pending.Executable.DisplayName;
			this.m_image.fillAmount = 1f - num;
			this.m_percentLabel.SetFormattedTime(pending.ExecutionTimeRemaining, true);
			this.m_icon.overrideSprite = ((pending.Instance != null && pending.Instance.Archetype) ? pending.Instance.Archetype.Icon : null);
			if (!this.m_window.Visible)
			{
				this.m_window.Show(false);
			}
		}

		// Token: 0x06003F77 RID: 16247 RVA: 0x0006AEAC File Offset: 0x000690AC
		private void HideIfShown()
		{
			if (this.m_window.Visible)
			{
				this.m_window.Hide(false);
			}
		}

		// Token: 0x04003D43 RID: 15683
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04003D44 RID: 15684
		[SerializeField]
		private Image m_image;

		// Token: 0x04003D45 RID: 15685
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003D46 RID: 15686
		[SerializeField]
		private TextMeshProUGUI m_percentLabel;

		// Token: 0x04003D47 RID: 15687
		[SerializeField]
		private Image m_icon;
	}
}
