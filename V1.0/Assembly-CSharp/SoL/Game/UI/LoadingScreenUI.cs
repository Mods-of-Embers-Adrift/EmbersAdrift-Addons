using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008A7 RID: 2215
	public class LoadingScreenUI : MonoBehaviour
	{
		// Token: 0x17000ED7 RID: 3799
		// (get) Token: 0x06004098 RID: 16536 RVA: 0x0006BB06 File Offset: 0x00069D06
		// (set) Token: 0x06004099 RID: 16537 RVA: 0x0006BB0E File Offset: 0x00069D0E
		public bool IgnoreEvents { get; set; }

		// Token: 0x0600409A RID: 16538 RVA: 0x0018CB60 File Offset: 0x0018AD60
		private void Awake()
		{
			UIManager.LoadingScreenUI = this;
			this.m_window.gameObject.SetActive(true);
			SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.LoadingStartupScene += this.SceneCompositionManagerOnLoadingStartupScene;
			LocalPlayer.FadeLoadingScreenShowUi += this.FadeLoadingScreenShowUi;
			if (this.m_loadingBackground)
			{
				this.m_aspectRatioFitter = this.m_loadingBackground.gameObject.GetComponent<AspectRatioFitter>();
			}
		}

		// Token: 0x0600409B RID: 16539 RVA: 0x0018CBDC File Offset: 0x0018ADDC
		private void Start()
		{
			string text = "";
			for (int i = 0; i < 3; i++)
			{
				text += ".";
			}
			this.m_ellipsis.text = text;
			this.m_ellipsis.maxVisibleCharacters = 0;
			this.m_loadingPercent.text = string.Empty;
		}

		// Token: 0x0600409C RID: 16540 RVA: 0x0018CC30 File Offset: 0x0018AE30
		private void Update()
		{
			if (this.m_ellipsis.enabled && Time.time >= this.m_timeOfNextEllipsis)
			{
				int maxVisibleCharacters = (this.m_ellipsis.maxVisibleCharacters + 1) % 4;
				this.m_ellipsis.maxVisibleCharacters = maxVisibleCharacters;
				this.m_timeOfNextEllipsis = Time.time + 1f;
			}
		}

		// Token: 0x0600409D RID: 16541 RVA: 0x0006BB17 File Offset: 0x00069D17
		private void OnDestroy()
		{
			SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.LoadingStartupScene -= this.SceneCompositionManagerOnLoadingStartupScene;
			LocalPlayer.FadeLoadingScreenShowUi -= this.FadeLoadingScreenShowUi;
		}

		// Token: 0x0600409E RID: 16542 RVA: 0x0006BB4C File Offset: 0x00069D4C
		private void FadeLoadingScreenShowUi()
		{
			this.SetLoadingPercent(1f);
			this.HideLoadingScreen();
		}

		// Token: 0x0600409F RID: 16543 RVA: 0x0006BB5F File Offset: 0x00069D5F
		public void HideLoadingScreen()
		{
			this.ToggleEllipsis(false);
			this.m_ellipsis.maxVisibleCharacters = 0;
			this.m_window.Hide(false);
		}

		// Token: 0x060040A0 RID: 16544 RVA: 0x0006BB80 File Offset: 0x00069D80
		private void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.ShowInternal();
		}

		// Token: 0x060040A1 RID: 16545 RVA: 0x0006BB80 File Offset: 0x00069D80
		private void SceneCompositionManagerOnLoadingStartupScene()
		{
			this.ShowInternal();
		}

		// Token: 0x060040A2 RID: 16546 RVA: 0x0006BB88 File Offset: 0x00069D88
		private void ShowInternal()
		{
			if (!this.IgnoreEvents)
			{
				this.m_timeOfNextEllipsis = Time.time + 1f;
				this.m_window.Show(false);
			}
		}

		// Token: 0x060040A3 RID: 16547 RVA: 0x0006BBAF File Offset: 0x00069DAF
		public void SetLoadingPercent(float percent)
		{
			this.m_loadingPercent.SetTextFormat("{0}%", Mathf.FloorToInt(percent * 100f));
			if (this.m_loadingBar)
			{
				this.m_loadingBar.fillAmount = percent;
			}
		}

		// Token: 0x060040A4 RID: 16548 RVA: 0x0006BBE6 File Offset: 0x00069DE6
		public void SetLoadingStatus(string msg)
		{
			this.m_loadingStatus.ZStringSetText(msg);
		}

		// Token: 0x060040A5 RID: 16549 RVA: 0x0006BBF4 File Offset: 0x00069DF4
		public void SetDestination(string msg)
		{
			this.m_destination.ZStringSetText(msg);
		}

		// Token: 0x060040A6 RID: 16550 RVA: 0x0018CC84 File Offset: 0x0018AE84
		public void SetLoadingImage(Sprite img)
		{
			if (this.m_loadingBackground)
			{
				this.m_loadingBackground.overrideSprite = img;
				if (this.m_aspectRatioFitter)
				{
					this.m_aspectRatioFitter.aspectRatio = ((img == null) ? this.m_loadingBackground.sprite.GetAspectRatio() : img.GetAspectRatio());
				}
			}
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x0006BC02 File Offset: 0x00069E02
		public void SetLoadingTip(string tip)
		{
			if (this.m_loadingTip)
			{
				this.m_loadingTip.SetTextFormat("<i>{0}</i>", tip);
			}
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x0006BC22 File Offset: 0x00069E22
		public void ToggleEllipsis(bool shown)
		{
			this.m_ellipsis.enabled = shown;
		}

		// Token: 0x04003E48 RID: 15944
		[SerializeField]
		private TextMeshProUGUI m_destination;

		// Token: 0x04003E49 RID: 15945
		[SerializeField]
		private TextMeshProUGUI m_loadingPercent;

		// Token: 0x04003E4A RID: 15946
		[SerializeField]
		private TextMeshProUGUI m_loadingStatus;

		// Token: 0x04003E4B RID: 15947
		[SerializeField]
		private TextMeshProUGUI m_loadingTip;

		// Token: 0x04003E4C RID: 15948
		[SerializeField]
		private TextMeshProUGUI m_ellipsis;

		// Token: 0x04003E4D RID: 15949
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04003E4E RID: 15950
		[SerializeField]
		private Image m_loadingBar;

		// Token: 0x04003E4F RID: 15951
		[SerializeField]
		private Image m_loadingBackground;

		// Token: 0x04003E50 RID: 15952
		private const int kNDots = 3;

		// Token: 0x04003E51 RID: 15953
		private const float kRate = 1f;

		// Token: 0x04003E52 RID: 15954
		private float m_timeOfNextEllipsis;

		// Token: 0x04003E53 RID: 15955
		private AspectRatioFitter m_aspectRatioFitter;
	}
}
