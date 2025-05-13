using System;
using System.Collections;
using SoL.Game.SkyDome;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007EF RID: 2031
	public class PlayerDummySettings : MonoBehaviour
	{
		// Token: 0x06003B10 RID: 15120 RVA: 0x0017A5F0 File Offset: 0x001787F0
		private void Start()
		{
			SceneCompositionManager.InvokeSceneCompositionLoaded();
			if (SkyDomeManager.SkyDomeController == null)
			{
				SkyDomeManager.SkydomeControllerChanged += this.InitSkyDome;
				this.m_subscribed = true;
			}
			else
			{
				this.InitSkyDome();
			}
			this.m_animator = base.gameObject.GetComponent<Animator>();
			if (this.m_animator)
			{
				this.m_speedKey = Animator.StringToHash("Speed");
			}
		}

		// Token: 0x06003B11 RID: 15121 RVA: 0x00067FD5 File Offset: 0x000661D5
		private void OnDestroy()
		{
			this.UnSubscribeToSkydomeControllerChanged();
		}

		// Token: 0x06003B12 RID: 15122 RVA: 0x00067FDD File Offset: 0x000661DD
		private void UnSubscribeToSkydomeControllerChanged()
		{
			if (this.m_subscribed)
			{
				SkyDomeManager.SkydomeControllerChanged -= this.InitSkyDome;
				this.m_subscribed = false;
			}
		}

		// Token: 0x06003B13 RID: 15123 RVA: 0x00067FFF File Offset: 0x000661FF
		private void InitSkyDome()
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				return;
			}
			this.UnSubscribeToSkydomeControllerChanged();
			base.StartCoroutine("InitSkyDomeCo");
		}

		// Token: 0x06003B14 RID: 15124 RVA: 0x0006801B File Offset: 0x0006621B
		private IEnumerator InitSkyDomeCo()
		{
			yield return new WaitForSeconds(1f);
			SkyDomeManager.SkyDomeController.ProgressTime = !this.m_fixedTime;
			DateTime time = GameDateTime.UtcNow.DateTime;
			if (this.m_fixedTime)
			{
				time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
				time = time.AddHours((double)this.m_time);
			}
			SkyDomeManager.SkyDomeController.SetTime(time);
			yield break;
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x0006802A File Offset: 0x0006622A
		private void Update()
		{
			if (this.m_animator)
			{
				this.m_animator.SetFloat(this.m_speedKey, this.m_speed);
			}
		}

		// Token: 0x04003985 RID: 14725
		[SerializeField]
		private float m_speed = 1f;

		// Token: 0x04003986 RID: 14726
		[SerializeField]
		private bool m_fixedTime;

		// Token: 0x04003987 RID: 14727
		[Range(0f, 24f)]
		[SerializeField]
		private float m_time = 14f;

		// Token: 0x04003988 RID: 14728
		private Animator m_animator;

		// Token: 0x04003989 RID: 14729
		private int m_speedKey;

		// Token: 0x0400398A RID: 14730
		private bool m_subscribed;
	}
}
