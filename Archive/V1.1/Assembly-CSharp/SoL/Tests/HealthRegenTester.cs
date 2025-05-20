using System;
using System.Collections;
using System.Diagnostics;
using Cysharp.Text;
using SoL.Game;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Tests
{
	// Token: 0x02000DA5 RID: 3493
	public class HealthRegenTester : MonoBehaviour
	{
		// Token: 0x060068AF RID: 26799 RVA: 0x002155B4 File Offset: 0x002137B4
		private void Awake()
		{
			if (this.m_executeButton)
			{
				this.m_executeButton.onClick.AddListener(new UnityAction(this.Execute));
			}
			this.RefreshLabel((float)this.m_maxHealth, 0f, 0f, 0f);
		}

		// Token: 0x060068B0 RID: 26800 RVA: 0x000863CC File Offset: 0x000845CC
		private void OnDestroy()
		{
			if (this.m_executeButton)
			{
				this.m_executeButton.onClick.RemoveListener(new UnityAction(this.Execute));
			}
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x000863F7 File Offset: 0x000845F7
		private void Execute()
		{
			if (this.m_currentCo != null)
			{
				base.StopCoroutine(this.m_currentCo);
			}
			this.m_currentCo = this.HealthCo();
			base.StartCoroutine(this.m_currentCo);
		}

		// Token: 0x060068B2 RID: 26802 RVA: 0x00086426 File Offset: 0x00084626
		private IEnumerator HealthCo()
		{
			UnityEngine.Debug.Log("Starting!");
			Stopwatch sw = new Stopwatch();
			sw.Start();
			float totalTimeElapsed = 0f;
			float timeFullyRested = 0f;
			float combatExitTime = this.m_combatExitTime;
			float health = (float)this.m_startingHealth;
			while (health < (float)this.m_maxHealth)
			{
				float num = Time.deltaTime * this.m_timeMultiplier;
				float num2 = (float)this.m_regenStat * 0.01f;
				float num3 = this.m_stance.GetStanceProfile().GetHealthRegenRate(1f);
				if (this.m_stance.ApplyFullyRestedBonus())
				{
					num3 += this.GetFullyRestedBonus(timeFullyRested, num2);
				}
				if (this.m_regenStatAppliesToBase)
				{
					num3 = num3.PercentModification(num2);
				}
				totalTimeElapsed += num;
				combatExitTime -= num;
				if (combatExitTime <= 0f)
				{
					timeFullyRested += num;
				}
				health += num3 * num;
				this.RefreshLabel(health, num3, totalTimeElapsed, timeFullyRested);
				yield return null;
			}
			sw.Stop();
			UnityEngine.Debug.Log(string.Format("{0}s", sw.Elapsed.TotalSeconds));
			yield break;
		}

		// Token: 0x060068B3 RID: 26803 RVA: 0x00086435 File Offset: 0x00084635
		private void RefreshLabel(float health, float rate, float totalElapsed, float bonusElapsed)
		{
			if (this.m_label)
			{
				this.m_label.SetTextFormat("Health:\t{0}\nFrac:\t{1}%\nRate:\t{2}/s\nTotal:\t{3}s\nBonus:\t{4}s", health, Mathf.FloorToInt(health / (float)this.m_maxHealth * 100f), rate, totalElapsed, bonusElapsed);
			}
		}

		// Token: 0x060068B4 RID: 26804 RVA: 0x00215608 File Offset: 0x00213808
		private float GetFullyRestedBonus(float timeFullyRested, float healthRegenBonusPercent)
		{
			if (timeFullyRested <= 0f || this.m_fullyRestedBonusValue <= 0f)
			{
				return 0f;
			}
			if (healthRegenBonusPercent > 0f && this.m_healthRegenBonusCountCurve != null)
			{
				timeFullyRested += this.m_healthRegenBonusCountCurve.Evaluate(healthRegenBonusPercent) * (float)this.m_fullyRestedBonusTime;
			}
			int num = Mathf.FloorToInt(timeFullyRested / (float)this.m_fullyRestedBonusTime);
			int num2 = num * this.m_fullyRestedBonusTime;
			float t = (timeFullyRested - (float)num2) / (float)this.m_fullyRestedBonusTime;
			return Mathf.Clamp((float)num * this.m_fullyRestedBonusValue + Mathf.Lerp(0f, this.m_fullyRestedBonusValue, t), 0f, this.m_fullyRestedBonusValueMax);
		}

		// Token: 0x04005B06 RID: 23302
		[Range(1f, 10f)]
		[SerializeField]
		private float m_timeMultiplier = 1f;

		// Token: 0x04005B07 RID: 23303
		[SerializeField]
		private float m_combatExitTime;

		// Token: 0x04005B08 RID: 23304
		[Range(1f, 400f)]
		[SerializeField]
		private int m_startingHealth = 1;

		// Token: 0x04005B09 RID: 23305
		[Range(1f, 400f)]
		[SerializeField]
		private int m_maxHealth = 300;

		// Token: 0x04005B0A RID: 23306
		[SerializeField]
		private bool m_regenStatAppliesToBase;

		// Token: 0x04005B0B RID: 23307
		[Range(0f, 400f)]
		[SerializeField]
		private int m_regenStat;

		// Token: 0x04005B0C RID: 23308
		[SerializeField]
		private Stance m_stance = Stance.Sit;

		// Token: 0x04005B0D RID: 23309
		private const string kFullyRestedGroup = "Fully Rested";

		// Token: 0x04005B0E RID: 23310
		[Tooltip("Time elapsed after leaving combat to apply bonus.Each pass results in another bonus applied. i.e. bonusTime=10s, 20s elapsed would return 2x bonusValue")]
		[SerializeField]
		private int m_fullyRestedBonusTime = 10;

		// Token: 0x04005B0F RID: 23311
		[Tooltip("Amount of regen bonus to apply per fullyRestedBonusTime elapsed.")]
		[SerializeField]
		private float m_fullyRestedBonusValue = 1f;

		// Token: 0x04005B10 RID: 23312
		[Tooltip("Maximum bonus that can be applied.")]
		[SerializeField]
		private float m_fullyRestedBonusValueMax = 5f;

		// Token: 0x04005B11 RID: 23313
		[Tooltip("X-Axis is regen stat in percent (+150% = 1.5)\nY-Axis is N-Bonuses you start with.")]
		[SerializeField]
		private AnimationCurve m_healthRegenBonusCountCurve;

		// Token: 0x04005B12 RID: 23314
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04005B13 RID: 23315
		[SerializeField]
		private Button m_executeButton;

		// Token: 0x04005B14 RID: 23316
		private IEnumerator m_currentCo;
	}
}
