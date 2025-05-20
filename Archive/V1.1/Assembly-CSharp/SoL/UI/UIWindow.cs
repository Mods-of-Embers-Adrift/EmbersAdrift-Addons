using System;
using System.Collections;
using SoL.Game.Audio;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000395 RID: 917
	public class UIWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06001913 RID: 6419 RVA: 0x00105E84 File Offset: 0x00104084
		// (remove) Token: 0x06001914 RID: 6420 RVA: 0x00105EBC File Offset: 0x001040BC
		public event Action LockButtonPressedEvent;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001915 RID: 6421 RVA: 0x00105EF4 File Offset: 0x001040F4
		// (remove) Token: 0x06001916 RID: 6422 RVA: 0x00105F2C File Offset: 0x0010412C
		public event Action WindowClosed;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x06001917 RID: 6423 RVA: 0x00105F64 File Offset: 0x00104164
		// (remove) Token: 0x06001918 RID: 6424 RVA: 0x00105F9C File Offset: 0x0010419C
		public event Action ShowCalled;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06001919 RID: 6425 RVA: 0x00105FD4 File Offset: 0x001041D4
		// (remove) Token: 0x0600191A RID: 6426 RVA: 0x0010600C File Offset: 0x0010420C
		public event Action HideCalled;

		// Token: 0x0600191B RID: 6427 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioClipCollection()
		{
			return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00053978 File Offset: 0x00051B78
		public void SetAudioClipCollection(bool open, AudioClipCollection collection)
		{
			if (open)
			{
				this.m_openClips = collection;
				return;
			}
			this.m_closeClips = collection;
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x0600191D RID: 6429 RVA: 0x0005398C File Offset: 0x00051B8C
		private bool m_useAnimator
		{
			get
			{
				return this.m_animator != null && this.m_transitionSettings.UseAnimator;
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x0600191E RID: 6430 RVA: 0x000539A9 File Offset: 0x00051BA9
		// (set) Token: 0x0600191F RID: 6431 RVA: 0x000539B1 File Offset: 0x00051BB1
		public Action ShowCallback { get; set; }

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06001920 RID: 6432 RVA: 0x00106044 File Offset: 0x00104244
		// (remove) Token: 0x06001921 RID: 6433 RVA: 0x0010607C File Offset: 0x0010427C
		public event Action PostShowCallback;

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06001922 RID: 6434 RVA: 0x000539BA File Offset: 0x00051BBA
		// (set) Token: 0x06001923 RID: 6435 RVA: 0x000539C2 File Offset: 0x00051BC2
		public Action HideCallback { get; set; }

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06001924 RID: 6436 RVA: 0x001060B4 File Offset: 0x001042B4
		// (remove) Token: 0x06001925 RID: 6437 RVA: 0x001060EC File Offset: 0x001042EC
		public event Action PostHideCallback;

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001926 RID: 6438 RVA: 0x000539CB File Offset: 0x00051BCB
		public bool CloseWithEscape
		{
			get
			{
				return this.m_closeOnEscape && this.m_lockButton.State == ToggleController.ToggleState.OFF && this.Visible;
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x000539EB File Offset: 0x00051BEB
		public bool Visible
		{
			get
			{
				return this.m_state == UIWindow.UIWindowState.Shown || this.m_state == UIWindow.UIWindowState.FadingIn;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06001928 RID: 6440 RVA: 0x00053A01 File Offset: 0x00051C01
		public bool CursorInside
		{
			get
			{
				return this.m_cursorInside;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x00053A09 File Offset: 0x00051C09
		public bool Locked
		{
			get
			{
				return this.m_lockButton != null && this.m_lockButton.State == ToggleController.ToggleState.ON;
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x0600192A RID: 6442 RVA: 0x00053A23 File Offset: 0x00051C23
		public virtual bool CanModify
		{
			get
			{
				return this.m_lockButton.State == ToggleController.ToggleState.OFF && this.m_visible;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x0600192B RID: 6443 RVA: 0x00106124 File Offset: 0x00104324
		public RectTransform RectTransform
		{
			get
			{
				if (!this.m_isQuitting && !this.m_rectTransform && base.gameObject && base.gameObject.transform)
				{
					this.m_rectTransform = (base.gameObject.transform as RectTransform);
				}
				return this.m_rectTransform;
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x0600192C RID: 6444 RVA: 0x00106184 File Offset: 0x00104384
		public CanvasGroup CanvasGroup
		{
			get
			{
				if (!this.m_cachedCanvasGroup && !this.m_canvasGroup && base.gameObject)
				{
					this.m_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
					if (!this.m_canvasGroup)
					{
						this.m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
					}
					this.m_cachedCanvasGroup = true;
				}
				return this.m_canvasGroup;
			}
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x001061F0 File Offset: 0x001043F0
		protected virtual void Awake()
		{
			this.m_animator = base.gameObject.GetComponent<Animator>();
			if (this.m_animator != null)
			{
				this.m_animator.enabled = false;
			}
			this.m_opacitySlider.Init(this, null);
			this.m_resetButton.Init(this, new Action(this.ResetButtonPressed));
			this.m_closeButton.Init(this, new Action(this.CloseButtonPressed));
			this.m_lockButton.Init(this, new Action(this.LockButtonPressed));
			this.m_windowOptionsButton.Init(this, new Action(this.OptionsButtonPressed));
			if (this.m_visibleOnAwake)
			{
				this.Show(true);
				return;
			}
			this.Hide(true);
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x00053A3B File Offset: 0x00051C3B
		protected virtual void Start()
		{
			UIManager.RegisterUIWindow(this);
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x00053A43 File Offset: 0x00051C43
		protected virtual void OnDestroy()
		{
			this.m_closeButton.Destroy();
			this.m_opacitySlider.Destroy();
			this.m_lockButton.Destroy();
			UIManager.UnregisterUIWindow(this);
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x00053A6C File Offset: 0x00051C6C
		private void OnApplicationQuit()
		{
			this.m_isQuitting = true;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x00053A75 File Offset: 0x00051C75
		protected void SetLockState(bool isLocked)
		{
			UIWindow.WindowToggleController lockButton = this.m_lockButton;
			if (lockButton == null)
			{
				return;
			}
			lockButton.SetState(isLocked ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x00053A8E File Offset: 0x00051C8E
		private void UIAnimatorHideShowEvent(UIWindow.UIWindowState state)
		{
			switch (state)
			{
			case UIWindow.UIWindowState.Hidden:
				this.PostTransition(false, false);
				return;
			case UIWindow.UIWindowState.Shown:
				this.PostTransition(true, false);
				return;
			case UIWindow.UIWindowState.FadingIn:
				this.PreTransition(true, false);
				return;
			case UIWindow.UIWindowState.FadingOut:
				this.PreTransition(false, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x00053ACA File Offset: 0x00051CCA
		public void PreventFromClosing()
		{
			this.m_closeOnEscape = false;
			if (this.m_closeButton.Button != null)
			{
				this.m_closeButton.Button.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ResolutionChanged()
		{
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnPointerEnter(PointerEventData eventData)
		{
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnPointerExit(PointerEventData eventData)
		{
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x00053AFC File Offset: 0x00051CFC
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (this.m_bringToFrontOnClick)
			{
				base.gameObject.transform.SetAsLastSibling();
				UIManager.UIWindowToFront(this);
			}
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x00053B1C File Offset: 0x00051D1C
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_cursorInside = true;
			this.OnPointerEnter(eventData);
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00053B2C File Offset: 0x00051D2C
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_cursorInside = false;
			this.OnPointerExit(eventData);
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x00053B3C File Offset: 0x00051D3C
		private void OptionsButtonPressed()
		{
			if (this.m_windowOptions.Visible)
			{
				this.m_windowOptions.Hide(false);
				return;
			}
			this.m_windowOptions.Show(false);
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LockButtonPressed()
		{
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00053B64 File Offset: 0x00051D64
		public virtual void CloseButtonPressed()
		{
			this.Hide(false);
			Action windowClosed = this.WindowClosed;
			if (windowClosed == null)
			{
				return;
			}
			windowClosed();
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00053B7D File Offset: 0x00051D7D
		protected virtual void ResetButtonPressed()
		{
			this.m_opacitySlider.Reset();
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x00053B8A File Offset: 0x00051D8A
		public void ToggleWindow()
		{
			if (this.Visible)
			{
				this.Hide(false);
				return;
			}
			this.Show(false);
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x001062B0 File Offset: 0x001044B0
		public virtual void Show(bool skipTransition = false)
		{
			skipTransition = (skipTransition || !base.gameObject.activeInHierarchy);
			this.TransitionInternal(true, skipTransition);
			Action showCallback = this.ShowCallback;
			if (showCallback != null)
			{
				showCallback();
			}
			Action showCalled = this.ShowCalled;
			if (showCalled != null)
			{
				showCalled();
			}
			if (this.m_bringToFrontOnShow)
			{
				base.gameObject.transform.SetAsLastSibling();
				UIManager.UIWindowToFront(this);
			}
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x0010631C File Offset: 0x0010451C
		public virtual void Hide(bool skipTransition = false)
		{
			skipTransition = (UIManager.UIResetting || skipTransition || !base.gameObject || !base.gameObject.activeInHierarchy);
			this.TransitionInternal(false, skipTransition);
			Action hideCallback = this.HideCallback;
			if (hideCallback != null)
			{
				hideCallback();
			}
			Action hideCalled = this.HideCalled;
			if (hideCalled == null)
			{
				return;
			}
			hideCalled();
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x0010637C File Offset: 0x0010457C
		private void TransitionInternal(bool show, bool skipTransition)
		{
			if (this.m_transitionCo != null)
			{
				base.StopCoroutine(this.m_transitionCo);
			}
			if (skipTransition)
			{
				this.PreTransition(show, true);
				this.PostTransition(show, true);
				return;
			}
			if (this.m_useAnimator)
			{
				if (!this.m_animator.enabled)
				{
					this.m_animator.enabled = true;
				}
				this.m_animator.Play(show ? "IN" : "OUT");
				return;
			}
			this.m_transitionCo = this.TransitionCo(show, show ? this.m_transitionSettings.ShowTime : this.m_transitionSettings.HideTime);
			base.StartCoroutine(this.m_transitionCo);
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x00053BA3 File Offset: 0x00051DA3
		private IEnumerator TransitionCo(bool showing, float time)
		{
			this.PreTransition(showing, false);
			UIWindowTransitionSettings.TransitionCurves curves = showing ? this.m_transitionSettings.Settings.InCurves : this.m_transitionSettings.Settings.OutCurves;
			float startAlpha = this.CanvasGroup.alpha;
			float targetAlpha = showing ? 1f : 0f;
			float elapsed = 0f;
			while (elapsed < time)
			{
				float num = elapsed / time;
				this.CanvasGroup.alpha = (this.m_transitionSettings.LinearlyInterpolateAlpha ? Mathf.Lerp(startAlpha, targetAlpha, num) : curves.Alpha.Evaluate(num));
				if (this.m_transitionSettings.Settings.Scale)
				{
					float num2 = curves.Scale.Evaluate(num);
					this.RectTransform.localScale = new Vector3(num2, num2, num2);
				}
				elapsed += Time.deltaTime;
				yield return null;
			}
			this.m_transitionCo = null;
			this.PostTransition(showing, false);
			yield break;
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x00106424 File Offset: 0x00104624
		private void PreTransition(bool showing, bool transitionSkipped)
		{
			this.m_state = (showing ? UIWindow.UIWindowState.FadingIn : UIWindow.UIWindowState.FadingOut);
			if (showing && this.m_canvas != null)
			{
				this.m_canvas.enabled = true;
			}
			if (showing && this.m_transitionSettings.Settings.Scale)
			{
				this.RectTransform.localScale = this.m_transitionSettings.Settings.ScaleInStart;
			}
			UIWindow.WindowButton closeButton = this.m_closeButton;
			if (closeButton != null)
			{
				closeButton.SetInteractable(showing);
			}
			if (this.CanvasGroup != null)
			{
				this.CanvasGroup.blocksRaycasts = showing;
			}
			if (!transitionSkipped)
			{
				AudioClipCollection collection = showing ? this.m_openClips : this.m_closeClips;
				ClientGameManager.UIManager.PlayRandomClip(collection, null);
			}
			if (showing && this.m_backGroundBlocker)
			{
				this.m_backGroundBlocker.enabled = true;
			}
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x001064FC File Offset: 0x001046FC
		private void PostTransition(bool showing, bool transitionSkipped)
		{
			if (this.m_isQuitting)
			{
				return;
			}
			this.m_state = (showing ? UIWindow.UIWindowState.Shown : UIWindow.UIWindowState.Hidden);
			this.m_visible = showing;
			if (this.CanvasGroup != null)
			{
				this.CanvasGroup.alpha = (showing ? this.m_transitionSettings.Settings.InCurves.Alpha.Evaluate(1f) : this.m_transitionSettings.Settings.OutCurves.Alpha.Evaluate(1f));
				this.CanvasGroup.blocksRaycasts = showing;
			}
			if (this.m_transitionSettings != null && this.m_transitionSettings.Settings != null && this.m_transitionSettings.Settings.Scale && this.RectTransform)
			{
				this.RectTransform.localScale = (showing ? this.m_transitionSettings.Settings.DefaultScale : this.m_transitionSettings.Settings.ScaleOutComplete);
			}
			if (this.m_canvas != null && !showing)
			{
				this.m_canvas.enabled = false;
			}
			if (this.m_useAnimator && this.m_animator)
			{
				this.m_animator.enabled = false;
			}
			if (!showing && this.m_backGroundBlocker)
			{
				this.m_backGroundBlocker.enabled = false;
			}
			if (showing)
			{
				Action postShowCallback = this.PostShowCallback;
				if (postShowCallback == null)
				{
					return;
				}
				postShowCallback();
				return;
			}
			else
			{
				Action postHideCallback = this.PostHideCallback;
				if (postHideCallback == null)
				{
					return;
				}
				postHideCallback();
				return;
			}
		}

		// Token: 0x0400201F RID: 8223
		[SerializeField]
		protected UIWindow.TransitionSettings m_transitionSettings;

		// Token: 0x04002020 RID: 8224
		[SerializeField]
		protected UIWindow.WindowButton m_closeButton;

		// Token: 0x04002021 RID: 8225
		[SerializeField]
		private UIWindow.WindowButton m_resetButton;

		// Token: 0x04002022 RID: 8226
		[SerializeField]
		private UIWindow.WindowToggleController m_lockButton;

		// Token: 0x04002023 RID: 8227
		[SerializeField]
		private UIWindow.OpacitySlider m_opacitySlider;

		// Token: 0x04002024 RID: 8228
		[SerializeField]
		private UIWindow.WindowButton m_windowOptionsButton;

		// Token: 0x04002025 RID: 8229
		[SerializeField]
		private UIWindowOptions m_windowOptions;

		// Token: 0x04002026 RID: 8230
		[SerializeField]
		private Canvas m_canvas;

		// Token: 0x04002027 RID: 8231
		[SerializeField]
		protected BackgroundBlockerSizeFitter m_backGroundBlocker;

		// Token: 0x04002028 RID: 8232
		[SerializeField]
		private bool m_closeOnEscape;

		// Token: 0x04002029 RID: 8233
		[SerializeField]
		private bool m_visibleOnAwake;

		// Token: 0x0400202A RID: 8234
		[SerializeField]
		private bool m_bringToFrontOnClick;

		// Token: 0x0400202B RID: 8235
		[SerializeField]
		private bool m_bringToFrontOnShow;

		// Token: 0x0400202C RID: 8236
		[SerializeField]
		private AudioClipCollection m_openClips;

		// Token: 0x0400202D RID: 8237
		[SerializeField]
		private AudioClipCollection m_closeClips;

		// Token: 0x0400202E RID: 8238
		protected UIWindow.UIWindowState m_state;

		// Token: 0x0400202F RID: 8239
		private bool m_visible;

		// Token: 0x04002030 RID: 8240
		private bool m_cursorInside;

		// Token: 0x04002031 RID: 8241
		private bool m_isQuitting;

		// Token: 0x04002032 RID: 8242
		private IEnumerator m_transitionCo;

		// Token: 0x04002033 RID: 8243
		private Animator m_animator;

		// Token: 0x04002038 RID: 8248
		private RectTransform m_rectTransform;

		// Token: 0x04002039 RID: 8249
		private bool m_cachedCanvasGroup;

		// Token: 0x0400203A RID: 8250
		private CanvasGroup m_canvasGroup;

		// Token: 0x02000396 RID: 918
		protected enum UIWindowState
		{
			// Token: 0x0400203C RID: 8252
			Hidden,
			// Token: 0x0400203D RID: 8253
			Shown,
			// Token: 0x0400203E RID: 8254
			FadingIn,
			// Token: 0x0400203F RID: 8255
			FadingOut
		}

		// Token: 0x02000397 RID: 919
		[Serializable]
		protected class TransitionSettings
		{
			// Token: 0x06001946 RID: 6470 RVA: 0x00053BC0 File Offset: 0x00051DC0
			private static UIWindowTransitionSettings GetDefaultUIWindowTransitionSettings()
			{
				if (UIWindow.TransitionSettings.m_defaultTransitionSettings == null)
				{
					UIWindow.TransitionSettings.m_defaultTransitionSettings = Resources.Load<UIWindowTransitionSettings>("DefaultUIWindowTransitionSettings");
				}
				return UIWindow.TransitionSettings.m_defaultTransitionSettings;
			}

			// Token: 0x17000607 RID: 1543
			// (get) Token: 0x06001947 RID: 6471 RVA: 0x00053BE3 File Offset: 0x00051DE3
			public UIWindowTransitionSettings Settings
			{
				get
				{
					if (this.m_settings == null)
					{
						this.m_settings = ((this.m_override == null) ? UIWindow.TransitionSettings.GetDefaultUIWindowTransitionSettings() : this.m_override);
					}
					return this.m_settings;
				}
			}

			// Token: 0x17000608 RID: 1544
			// (get) Token: 0x06001948 RID: 6472 RVA: 0x00053C1A File Offset: 0x00051E1A
			public float ShowTime
			{
				get
				{
					if (!this.m_customShowTime)
					{
						return 0.3f;
					}
					return this.m_showTime;
				}
			}

			// Token: 0x17000609 RID: 1545
			// (get) Token: 0x06001949 RID: 6473 RVA: 0x00053C30 File Offset: 0x00051E30
			public float HideTime
			{
				get
				{
					if (!this.m_customHideTime)
					{
						return 0.3f;
					}
					return this.m_hideTime;
				}
			}

			// Token: 0x1700060A RID: 1546
			// (get) Token: 0x0600194A RID: 6474 RVA: 0x00053C46 File Offset: 0x00051E46
			public bool UseAnimator
			{
				get
				{
					return this.m_source == UIWindow.TransitionSettings.UIWindowTransitionSource.Animator;
				}
			}

			// Token: 0x1700060B RID: 1547
			// (get) Token: 0x0600194B RID: 6475 RVA: 0x00053C51 File Offset: 0x00051E51
			public bool LinearlyInterpolateAlpha
			{
				get
				{
					return this.m_linearlyInterpolateAlpha;
				}
			}

			// Token: 0x04002040 RID: 8256
			private const float kGlobalShowTime = 0.3f;

			// Token: 0x04002041 RID: 8257
			private const float kGlobalHideTime = 0.3f;

			// Token: 0x04002042 RID: 8258
			private static UIWindowTransitionSettings m_defaultTransitionSettings;

			// Token: 0x04002043 RID: 8259
			private UIWindowTransitionSettings m_settings;

			// Token: 0x04002044 RID: 8260
			[SerializeField]
			private UIWindow.TransitionSettings.UIWindowTransitionSource m_source;

			// Token: 0x04002045 RID: 8261
			[SerializeField]
			private UIWindowTransitionSettings m_override;

			// Token: 0x04002046 RID: 8262
			[SerializeField]
			private bool m_customShowTime;

			// Token: 0x04002047 RID: 8263
			[SerializeField]
			private float m_showTime = 0.3f;

			// Token: 0x04002048 RID: 8264
			[SerializeField]
			private bool m_customHideTime;

			// Token: 0x04002049 RID: 8265
			[SerializeField]
			private float m_hideTime = 0.3f;

			// Token: 0x0400204A RID: 8266
			[SerializeField]
			private bool m_linearlyInterpolateAlpha;

			// Token: 0x02000398 RID: 920
			public enum UIWindowTransitionSource
			{
				// Token: 0x0400204C RID: 8268
				Coroutine,
				// Token: 0x0400204D RID: 8269
				Animator
			}
		}

		// Token: 0x02000399 RID: 921
		[Serializable]
		protected abstract class WindowFeature<T> where T : UnityEngine.Object
		{
			// Token: 0x1700060C RID: 1548
			// (get) Token: 0x0600194D RID: 6477 RVA: 0x00053C77 File Offset: 0x00051E77
			protected virtual bool Inactive
			{
				get
				{
					return this.m_object == null;
				}
			}

			// Token: 0x0600194E RID: 6478 RVA: 0x00053C8A File Offset: 0x00051E8A
			public void Init(UIWindow window, Action callback)
			{
				if (this.Inactive)
				{
					return;
				}
				this.m_window = window;
				this.m_callback = callback;
				this.InitInternal();
			}

			// Token: 0x0600194F RID: 6479 RVA: 0x0004475B File Offset: 0x0004295B
			protected virtual void InitInternal()
			{
			}

			// Token: 0x06001950 RID: 6480 RVA: 0x0004475B File Offset: 0x0004295B
			public virtual void Destroy()
			{
			}

			// Token: 0x0400204E RID: 8270
			[SerializeField]
			protected T m_object;

			// Token: 0x0400204F RID: 8271
			protected UIWindow m_window;

			// Token: 0x04002050 RID: 8272
			protected Action m_callback;
		}

		// Token: 0x0200039A RID: 922
		[Serializable]
		protected class WindowButton : UIWindow.WindowFeature<Button>
		{
			// Token: 0x1700060D RID: 1549
			// (get) Token: 0x06001952 RID: 6482 RVA: 0x00053CA9 File Offset: 0x00051EA9
			public Button Button
			{
				get
				{
					return this.m_object;
				}
			}

			// Token: 0x06001953 RID: 6483 RVA: 0x00053CB1 File Offset: 0x00051EB1
			protected override void InitInternal()
			{
				this.m_object.onClick.AddListener(new UnityAction(this.ButtonClicked));
			}

			// Token: 0x06001954 RID: 6484 RVA: 0x00053CCF File Offset: 0x00051ECF
			public override void Destroy()
			{
				if (this.Inactive)
				{
					return;
				}
				base.Destroy();
				this.m_object.onClick.RemoveAllListeners();
			}

			// Token: 0x06001955 RID: 6485 RVA: 0x00053CF0 File Offset: 0x00051EF0
			public void SetInteractable(bool interactable)
			{
				if (this.Inactive)
				{
					return;
				}
				this.m_object.interactable = interactable;
			}

			// Token: 0x06001956 RID: 6486 RVA: 0x00053D07 File Offset: 0x00051F07
			public void SetActive(bool active)
			{
				if (this.Inactive)
				{
					return;
				}
				this.m_object.gameObject.SetActive(active);
			}

			// Token: 0x06001957 RID: 6487 RVA: 0x00053D23 File Offset: 0x00051F23
			private void ButtonClicked()
			{
				Action callback = this.m_callback;
				if (callback == null)
				{
					return;
				}
				callback();
			}
		}

		// Token: 0x0200039B RID: 923
		[Serializable]
		protected class WindowToggleController : UIWindow.WindowFeature<ToggleController>
		{
			// Token: 0x1700060E RID: 1550
			// (get) Token: 0x06001959 RID: 6489 RVA: 0x00053D3D File Offset: 0x00051F3D
			public ToggleController.ToggleState State
			{
				get
				{
					if (!(this.m_object == null))
					{
						return this.m_object.State;
					}
					return ToggleController.ToggleState.OFF;
				}
			}

			// Token: 0x0600195A RID: 6490 RVA: 0x00053D5A File Offset: 0x00051F5A
			protected override void InitInternal()
			{
				this.m_object.ToggleChanged += this.ToggleStateChanged;
			}

			// Token: 0x0600195B RID: 6491 RVA: 0x00053D73 File Offset: 0x00051F73
			public override void Destroy()
			{
				if (this.Inactive)
				{
					return;
				}
				base.Destroy();
				this.m_object.ToggleChanged -= this.ToggleStateChanged;
			}

			// Token: 0x0600195C RID: 6492 RVA: 0x00053D9B File Offset: 0x00051F9B
			private void ToggleStateChanged(ToggleController.ToggleState state)
			{
				Action callback = this.m_callback;
				if (callback == null)
				{
					return;
				}
				callback();
			}

			// Token: 0x0600195D RID: 6493 RVA: 0x00053DAD File Offset: 0x00051FAD
			public void SetState(ToggleController.ToggleState state)
			{
				if (this.m_object)
				{
					this.m_object.State = state;
				}
			}
		}

		// Token: 0x0200039C RID: 924
		[Serializable]
		private class OpacitySlider : UIWindow.WindowFeature<Slider>
		{
			// Token: 0x1700060F RID: 1551
			// (get) Token: 0x0600195F RID: 6495 RVA: 0x00053DD0 File Offset: 0x00051FD0
			protected override bool Inactive
			{
				get
				{
					return base.Inactive || this.m_image == null;
				}
			}

			// Token: 0x06001960 RID: 6496 RVA: 0x00106678 File Offset: 0x00104878
			protected override void InitInternal()
			{
				this.m_object.minValue = 0f;
				this.m_object.maxValue = 1f;
				this.m_object.wholeNumbers = false;
				this.m_originalOpacity = this.m_image.color.a;
				this.m_object.value = this.m_originalOpacity;
				this.m_object.onValueChanged.AddListener(new UnityAction<float>(this.SliderChanged));
			}

			// Token: 0x06001961 RID: 6497 RVA: 0x00053DE8 File Offset: 0x00051FE8
			public override void Destroy()
			{
				if (this.Inactive)
				{
					return;
				}
				base.Destroy();
				this.m_object.onValueChanged.RemoveAllListeners();
			}

			// Token: 0x06001962 RID: 6498 RVA: 0x00053E09 File Offset: 0x00052009
			private void SliderChanged(float value)
			{
				this.SetOpacity(value, false);
			}

			// Token: 0x06001963 RID: 6499 RVA: 0x001066F4 File Offset: 0x001048F4
			private void SetOpacity(float value, bool setSlider)
			{
				Color color = this.m_image.color;
				color.a = value;
				this.m_image.color = color;
				if (setSlider)
				{
					this.m_object.value = value;
				}
			}

			// Token: 0x06001964 RID: 6500 RVA: 0x00053E13 File Offset: 0x00052013
			public void SetInteractable(bool interactable)
			{
				if (this.Inactive)
				{
					return;
				}
				this.m_object.interactable = interactable;
			}

			// Token: 0x06001965 RID: 6501 RVA: 0x00053E2A File Offset: 0x0005202A
			public void Reset()
			{
				this.SetOpacity(this.m_originalOpacity, true);
			}

			// Token: 0x04002051 RID: 8273
			[SerializeField]
			private Image m_image;

			// Token: 0x04002052 RID: 8274
			private float m_originalOpacity;
		}
	}
}
