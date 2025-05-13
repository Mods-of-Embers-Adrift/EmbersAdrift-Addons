using System;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200088E RID: 2190
	public class InCombatUI : MonoBehaviour
	{
		// Token: 0x06003FC8 RID: 16328 RVA: 0x00189B38 File Offset: 0x00187D38
		private void Update()
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Vitals)
			{
				return;
			}
			float num = (float)(DateTime.UtcNow - LocalPlayer.GameEntity.Vitals.LastCombatTimestamp).TotalSeconds;
			float num2 = 1f - num / GlobalSettings.Values.Combat.CombatRecoveryTime;
			float num3 = (Mathf.Abs(this.m_lastFrameFill - num2) > 0.4f) ? num2 : Mathf.MoveTowards(this.m_image.fillAmount, num2, this.m_speed * Time.deltaTime);
			this.m_image.fillAmount = num3;
			this.m_lastFrameFill = num3;
		}

		// Token: 0x04003D69 RID: 15721
		[SerializeField]
		private float m_speed = 10f;

		// Token: 0x04003D6A RID: 15722
		[SerializeField]
		private Image m_image;

		// Token: 0x04003D6B RID: 15723
		private float m_lastFrameFill = 1f;
	}
}
