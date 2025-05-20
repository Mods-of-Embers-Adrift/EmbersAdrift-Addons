using System;
using System.Collections;
using SoL.Game.Settings;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008EA RID: 2282
	public class UnconsciousPanelUI : UIWindow
	{
		// Token: 0x060042E1 RID: 17121 RVA: 0x0006D1A6 File Offset: 0x0006B3A6
		protected override void Awake()
		{
			base.Awake();
			this.m_giveUpButton.onClick.AddListener(new UnityAction(this.GiveUpClicked));
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x0006D1CA File Offset: 0x0006B3CA
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_giveUpButton.onClick.RemoveListener(new UnityAction(this.GiveUpClicked));
		}

		// Token: 0x060042E3 RID: 17123 RVA: 0x0006D1EE File Offset: 0x0006B3EE
		public void Init()
		{
			LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
		}

		// Token: 0x060042E4 RID: 17124 RVA: 0x0006D210 File Offset: 0x0006B410
		public void Unset()
		{
			LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
			this.ResetUI();
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x001936D8 File Offset: 0x001918D8
		private void ResetUI()
		{
			if (this.m_infoText != null)
			{
				this.m_infoText.text = null;
			}
			if (this.m_countdownText != null)
			{
				this.m_countdownText.text = null;
			}
			if (this.m_giveUpButton != null)
			{
				this.m_giveUpButton.interactable = false;
			}
			this.Hide(true);
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x0006D238 File Offset: 0x0006B438
		private void GiveUpClicked()
		{
			this.m_giveUpButton.interactable = false;
			this.m_gaveUp = true;
			LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.Client_GiveUp();
		}

		// Token: 0x060042E7 RID: 17127 RVA: 0x0019373C File Offset: 0x0019193C
		private void HealthStateOnChanged(HealthState obj)
		{
			this.m_healthState = obj;
			this.m_timeOfLastChange = DateTime.UtcNow;
			IEnumerator enumerator = null;
			switch (obj)
			{
			case HealthState.Unconscious:
				this.m_infoText.text = "You have been knocked unconscious!\nWait to be saved, or give up and pass out...";
				this.m_giveUpButton.interactable = true;
				this.m_giveUpButton.gameObject.SetActive(true);
				this.ResetBackgroundColor();
				enumerator = this.FadeCo(0.75f, GlobalSettings.Values.Player.GiveUpTime);
				this.Show(false);
				break;
			case HealthState.WakingUp:
				this.m_infoText.text = "You are waking up...";
				this.m_giveUpButton.interactable = false;
				this.m_giveUpButton.gameObject.SetActive(false);
				enumerator = this.FadeCo(0.25f, GlobalSettings.Values.Player.WakeUpTime);
				break;
			case HealthState.Dead:
				this.m_giveUpButton.interactable = false;
				this.m_giveUpButton.gameObject.SetActive(false);
				this.m_infoText.text = (this.m_gaveUp ? "Respawning..." : "Too late; respawning...");
				this.m_countdownText.text = null;
				enumerator = this.FadeCo(0.75f, GlobalSettings.Values.Player.DeadDelayTime);
				this.m_gaveUp = false;
				break;
			default:
				this.Hide(false);
				this.m_infoText.text = null;
				this.m_countdownText.text = null;
				this.m_giveUpButton.interactable = false;
				break;
			}
			if (enumerator != null)
			{
				if (this.m_fadeCo != null)
				{
					base.StopCoroutine(this.m_fadeCo);
				}
				this.m_fadeCo = enumerator;
				base.StartCoroutine(this.m_fadeCo);
			}
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x001938E0 File Offset: 0x00191AE0
		private void Update()
		{
			switch (this.m_healthState)
			{
			case HealthState.Unconscious:
				this.m_countdownText.text = this.GetCountdownDisplayTime(GlobalSettings.Values.Player.GiveUpTime);
				return;
			case HealthState.WakingUp:
				this.m_countdownText.text = this.GetCountdownDisplayTime(GlobalSettings.Values.Player.WakeUpTime);
				return;
			case HealthState.Dead:
				this.m_countdownText.text = this.GetCountdownDisplayTime(GlobalSettings.Values.Player.DeadDelayTime);
				return;
			default:
				return;
			}
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x0019396C File Offset: 0x00191B6C
		private string GetCountdownDisplayTime(float totalTime)
		{
			float num = (float)(DateTime.UtcNow - this.m_timeOfLastChange).TotalSeconds;
			return Mathf.FloorToInt(Mathf.Clamp(totalTime - num, 0f, float.MaxValue)).ToString() + "s";
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x001939BC File Offset: 0x00191BBC
		private void ResetBackgroundColor()
		{
			Color black = Color.black;
			black.a = 0.25f;
			this.m_backgroundImage.color = black;
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x0006D261 File Offset: 0x0006B461
		private void CrossFadeBackground(float targetAlpha, float time)
		{
			this.m_backgroundImage.CrossFadeAlpha(targetAlpha, time, true);
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x0006D271 File Offset: 0x0006B471
		private IEnumerator FadeCo(float targetAlpha, float time)
		{
			Color startColor = this.m_backgroundImage.color;
			Color targetColor = startColor;
			targetColor.a = targetAlpha;
			float t = 0f;
			while (t <= time)
			{
				float t2 = t / time;
				Color color = Color.Lerp(startColor, targetColor, t2);
				this.m_backgroundImage.color = color;
				t += Time.deltaTime;
				yield return null;
			}
			this.m_backgroundImage.color = targetColor;
			yield break;
		}

		// Token: 0x04003F9F RID: 16287
		private const float kDefaultBackgroundAlpha = 0.25f;

		// Token: 0x04003FA0 RID: 16288
		private const float kMaxBackgroundAlpha = 0.75f;

		// Token: 0x04003FA1 RID: 16289
		private const string kWakeUpText = "You are waking up...";

		// Token: 0x04003FA2 RID: 16290
		private const string kDeathText = "You have been knocked unconscious!\nWait to be saved, or give up and pass out...";

		// Token: 0x04003FA3 RID: 16291
		[SerializeField]
		private TextMeshProUGUI m_infoText;

		// Token: 0x04003FA4 RID: 16292
		[SerializeField]
		private TextMeshProUGUI m_countdownText;

		// Token: 0x04003FA5 RID: 16293
		[SerializeField]
		private SolButton m_giveUpButton;

		// Token: 0x04003FA6 RID: 16294
		[SerializeField]
		private Image m_backgroundImage;

		// Token: 0x04003FA7 RID: 16295
		private HealthState m_healthState;

		// Token: 0x04003FA8 RID: 16296
		private DateTime m_timeOfLastChange = DateTime.MinValue;

		// Token: 0x04003FA9 RID: 16297
		private bool m_gaveUp;

		// Token: 0x04003FAA RID: 16298
		private IEnumerator m_fadeCo;
	}
}
