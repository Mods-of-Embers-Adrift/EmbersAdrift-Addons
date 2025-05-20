using System;
using Cysharp.Text;
using SoL.Game;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.UI
{
	// Token: 0x02000375 RID: 885
	public class TeleportButtonPanel : MonoBehaviour
	{
		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06001840 RID: 6208 RVA: 0x00103B20 File Offset: 0x00101D20
		// (remove) Token: 0x06001841 RID: 6209 RVA: 0x00103B58 File Offset: 0x00101D58
		public event Action TravelClicked;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06001842 RID: 6210 RVA: 0x00103B90 File Offset: 0x00101D90
		// (remove) Token: 0x06001843 RID: 6211 RVA: 0x00103BC8 File Offset: 0x00101DC8
		public event Action EssenceClicked;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06001844 RID: 6212 RVA: 0x00103C00 File Offset: 0x00101E00
		// (remove) Token: 0x06001845 RID: 6213 RVA: 0x00103C38 File Offset: 0x00101E38
		public event Action CancelClicked;

		// Token: 0x06001846 RID: 6214 RVA: 0x00103C70 File Offset: 0x00101E70
		private void Awake()
		{
			if (this.m_useTravelButton)
			{
				this.m_useTravelButton.onClick.AddListener(new UnityAction(this.UseTravelButtonClicked));
			}
			if (this.m_useEssenceButton)
			{
				this.m_useEssenceButton.onClick.AddListener(new UnityAction(this.UseEssenceButtonClicked));
			}
			if (this.m_cancelButton)
			{
				this.m_cancelButton.onClick.AddListener(new UnityAction(this.OnCancelClicked));
			}
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x00103CF8 File Offset: 0x00101EF8
		private void OnDestroy()
		{
			if (this.m_useTravelButton)
			{
				this.m_useTravelButton.onClick.RemoveListener(new UnityAction(this.UseTravelButtonClicked));
			}
			if (this.m_useEssenceButton)
			{
				this.m_useEssenceButton.onClick.RemoveListener(new UnityAction(this.UseEssenceButtonClicked));
			}
			if (this.m_cancelButton)
			{
				this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.OnCancelClicked));
			}
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000530B1 File Offset: 0x000512B1
		private void OnCancelClicked()
		{
			Action cancelClicked = this.CancelClicked;
			if (cancelClicked == null)
			{
				return;
			}
			cancelClicked();
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000530C3 File Offset: 0x000512C3
		private void UseTravelButtonClicked()
		{
			Action travelClicked = this.TravelClicked;
			if (travelClicked == null)
			{
				return;
			}
			travelClicked();
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x000530D5 File Offset: 0x000512D5
		private void UseEssenceButtonClicked()
		{
			Action essenceClicked = this.EssenceClicked;
			if (essenceClicked == null)
			{
				return;
			}
			essenceClicked();
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x00103D80 File Offset: 0x00101F80
		public void InitButtons(int cost)
		{
			if (!this.m_useTravelButton || !this.m_useEssenceButton)
			{
				this.ToggleInteractivity(false);
				return;
			}
			int num = 0;
			int num2 = 0;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				ValueTuple<int, int> emberAndTravelEssenceCounts = LocalPlayer.GameEntity.CollectionController.GetEmberAndTravelEssenceCounts();
				num = emberAndTravelEssenceCounts.Item1;
				num2 = emberAndTravelEssenceCounts.Item2;
			}
			if (cost > num + num2)
			{
				this.m_useTravelButton.interactable = false;
				this.m_useTravelButton.SetText("Insufficient Travel + Ember Essence");
				this.m_useEssenceButton.interactable = false;
				this.m_useEssenceButton.SetText("Insufficient Ember Essence");
				return;
			}
			this.m_useTravelButton.interactable = (num2 > 0);
			string text = "No Travel Essence";
			if (num2 > 0)
			{
				text = ((num2 >= cost) ? ZString.Format<int>("{0} Travel Essence", cost) : ZString.Format<int, int>("{0} Travel + {1} Ember Essence", num2, cost - num2));
			}
			this.m_useTravelButton.SetText(text);
			this.m_useEssenceButton.interactable = (num >= cost);
			this.m_useEssenceButton.SetText(ZString.Format<int>("{0} Ember Essence", cost));
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x000530E7 File Offset: 0x000512E7
		public void ToggleInteractivity(bool isInteractive)
		{
			if (this.m_useTravelButton)
			{
				this.m_useTravelButton.interactable = isInteractive;
			}
			if (this.m_useEssenceButton)
			{
				this.m_useEssenceButton.interactable = isInteractive;
			}
		}

		// Token: 0x04001F90 RID: 8080
		[SerializeField]
		private SolButton m_useTravelButton;

		// Token: 0x04001F91 RID: 8081
		[SerializeField]
		private SolButton m_useEssenceButton;

		// Token: 0x04001F92 RID: 8082
		[SerializeField]
		private SolButton m_cancelButton;
	}
}
