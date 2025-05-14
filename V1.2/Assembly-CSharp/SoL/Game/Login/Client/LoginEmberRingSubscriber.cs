using System;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B30 RID: 2864
	public class LoginEmberRingSubscriber : MonoBehaviour
	{
		// Token: 0x060057F5 RID: 22517 RVA: 0x0007AA6C File Offset: 0x00078C6C
		private void Awake()
		{
			if (this.m_selection)
			{
				this.m_selection.OnStageEnter += this.SelectionOnOnStageEnter;
			}
		}

		// Token: 0x060057F6 RID: 22518 RVA: 0x001E4DB0 File Offset: 0x001E2FB0
		private void Start()
		{
			if (this.m_subscriberParticleSystem)
			{
				Color subscriberColor = GlobalSettings.Values.Subscribers.SubscriberColor;
				ParticleSystem[] componentsInChildren = this.m_subscriberParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					ParticleSystem.MainModule main = componentsInChildren[i].main;
					subscriberColor.a = main.startColor.color.a;
					main.startColor = subscriberColor;
				}
			}
		}

		// Token: 0x060057F7 RID: 22519 RVA: 0x0007AA92 File Offset: 0x00078C92
		private void OnDestroy()
		{
			if (this.m_selection)
			{
				this.m_selection.OnStageEnter -= this.SelectionOnOnStageEnter;
			}
		}

		// Token: 0x060057F8 RID: 22520 RVA: 0x0007AAB8 File Offset: 0x00078CB8
		private void SelectionOnOnStageEnter()
		{
			if (this.m_subscriberParticleSystem)
			{
				this.m_subscriberParticleSystem.gameObject.SetActive(SessionData.User != null && SessionData.User.IsSubscriber());
			}
		}

		// Token: 0x04004D8F RID: 19855
		[SerializeField]
		private LoginStageCharacterSelection m_selection;

		// Token: 0x04004D90 RID: 19856
		[SerializeField]
		private ParticleSystem m_subscriberParticleSystem;
	}
}
