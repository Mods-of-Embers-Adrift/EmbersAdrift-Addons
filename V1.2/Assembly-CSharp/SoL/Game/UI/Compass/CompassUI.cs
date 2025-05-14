using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.UI.Compass
{
	// Token: 0x020009A0 RID: 2464
	public class CompassUI : MonoBehaviour
	{
		// Token: 0x17001052 RID: 4178
		// (get) Token: 0x060049C3 RID: 18883 RVA: 0x000718E2 File Offset: 0x0006FAE2
		private float m_minOffset
		{
			get
			{
				return this.m_maxOffset * -1f;
			}
		}

		// Token: 0x17001053 RID: 4179
		// (get) Token: 0x060049C4 RID: 18884 RVA: 0x001B0850 File Offset: 0x001AEA50
		private HDCamera CachedHdCamera
		{
			get
			{
				if (!this.m_cachedMainCamera || this.m_cachedHdCamera == null)
				{
					this.m_cachedMainCamera = ClientGameManager.MainCamera;
					if (this.m_cachedMainCamera)
					{
						this.m_cachedHdCamera = HDCamera.GetOrCreate(this.m_cachedMainCamera, 0);
					}
				}
				return this.m_cachedHdCamera;
			}
		}

		// Token: 0x17001054 RID: 4180
		// (get) Token: 0x060049C5 RID: 18885 RVA: 0x000718F0 File Offset: 0x0006FAF0
		internal float Spacing
		{
			get
			{
				return this.m_spacing;
			}
		}

		// Token: 0x17001055 RID: 4181
		// (get) Token: 0x060049C6 RID: 18886 RVA: 0x000718F8 File Offset: 0x0006FAF8
		internal float SpacingScaleFactor
		{
			get
			{
				return this.m_spacingScaleFactor;
			}
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x001B08A4 File Offset: 0x001AEAA4
		private void Awake()
		{
			this.m_thisRectTransform = (base.gameObject.transform as RectTransform);
			Options.GameOptions.ShowUiCompass.Changed += this.ShowUiCompassOnChanged;
			Options.GameOptions.ShowZoneNameOnCompass.Changed += this.ShowZoneNameOnCompassOnChanged;
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x00071900 File Offset: 0x0006FB00
		private void Start()
		{
			this.InitIndicators();
			this.ShowUiCompassOnChanged();
			this.ShowZoneNameOnCompassOnChanged();
		}

		// Token: 0x060049C9 RID: 18889 RVA: 0x00071914 File Offset: 0x0006FB14
		private void OnDestroy()
		{
			Options.GameOptions.ShowUiCompass.Changed -= this.ShowUiCompassOnChanged;
			Options.GameOptions.ShowZoneNameOnCompass.Changed -= this.ShowZoneNameOnCompassOnChanged;
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x001B08F4 File Offset: 0x001AEAF4
		private void Update()
		{
			if (this.m_window && !this.m_window.Visible)
			{
				return;
			}
			float num;
			if (this.m_debug)
			{
				num = this.m_debugAngle;
			}
			else
			{
				if (!LocalPlayer.GameEntity || !ClientGameManager.MainCamera)
				{
					return;
				}
				num = ClientGameManager.MainCamera.gameObject.transform.eulerAngles.y;
			}
			this.UpdateAnimParams();
			this.UpdateAngleVariant();
			float num2 = num + this.m_variantValue;
			if (num2 >= 360f)
			{
				num2 -= 360f;
			}
			else if (num2 < 0f)
			{
				num2 += 360f;
			}
			this.UpdateIndicators(num2);
		}

		// Token: 0x060049CB RID: 18891 RVA: 0x001B09A8 File Offset: 0x001AEBA8
		private void ShowZoneNameOnCompassOnChanged()
		{
			Options.Option_Boolean showZoneNameOnCompass = Options.GameOptions.ShowZoneNameOnCompass;
			if (this.m_zoneLabelRect && this.m_zoneLabelRect.gameObject.activeSelf != showZoneNameOnCompass)
			{
				this.m_zoneLabelRect.gameObject.SetActive(showZoneNameOnCompass);
			}
			float num = (showZoneNameOnCompass && this.m_zoneLabelRect) ? (this.m_zoneLabelRect.sizeDelta.y * this.m_zoneLabelRect.localScale.y) : 0f;
			float num2 = this.m_compassPanelRect ? (this.m_compassPanelRect.sizeDelta.y * this.m_compassPanelRect.localScale.y) : 0f;
			if (this.m_thisRectTransform)
			{
				Vector2 sizeDelta = this.m_thisRectTransform.sizeDelta;
				sizeDelta.y = num + num2;
				this.m_thisRectTransform.sizeDelta = sizeDelta;
			}
			if (this.m_window)
			{
				this.m_window.ClampToScreen(true);
			}
		}

		// Token: 0x060049CC RID: 18892 RVA: 0x00071942 File Offset: 0x0006FB42
		private void InitIndicators()
		{
			this.UpdateIndicators(0f);
		}

		// Token: 0x060049CD RID: 18893 RVA: 0x001B0AB4 File Offset: 0x001AECB4
		private void ShowUiCompassOnChanged()
		{
			if (!this.m_window)
			{
				return;
			}
			if (Options.GameOptions.ShowUiCompass.Value)
			{
				if (!this.m_window.Visible)
				{
					this.m_window.Show(true);
					return;
				}
			}
			else if (this.m_window.Visible)
			{
				this.m_window.Hide(true);
			}
		}

		// Token: 0x060049CE RID: 18894 RVA: 0x001B0B10 File Offset: 0x001AED10
		public void UpdateAnimParams()
		{
			this.m_noiseSpeed = this.m_baseNoiseSpeed;
			this.m_maxOffset = this.m_baseMaxOffset;
			this.m_smoothTime = this.m_baseSmoothTime;
			if (this.CachedHdCamera != null && this.CachedHdCamera.volumeStack != null)
			{
				CompassAnimVolumeComponent component = this.CachedHdCamera.volumeStack.GetComponent<CompassAnimVolumeComponent>();
				if (component)
				{
					if (component.NoiseSpeed.overrideState)
					{
						this.m_noiseSpeed = component.NoiseSpeed.value;
					}
					if (component.MaxOffset.overrideState)
					{
						this.m_maxOffset = component.MaxOffset.value;
					}
					if (component.SmoothTime.overrideState)
					{
						this.m_smoothTime = component.SmoothTime.value;
					}
				}
			}
		}

		// Token: 0x060049CF RID: 18895 RVA: 0x001B0BCC File Offset: 0x001AEDCC
		private void UpdateAngleVariant()
		{
			this.m_perlinScroll += Time.deltaTime * this.m_noiseSpeed;
			float t = Mathf.PerlinNoise1D(this.m_perlinScroll);
			float target = Mathf.Lerp(this.m_minOffset, this.m_maxOffset, t);
			this.m_variantValue = Mathf.SmoothDamp(this.m_variantValue, target, ref this.m_variantVelocity, this.m_smoothTime);
		}

		// Token: 0x060049D0 RID: 18896 RVA: 0x001B0C30 File Offset: 0x001AEE30
		private void UpdateIndicators(float angle)
		{
			if (this.m_fixedIndicators != null)
			{
				for (int i = 0; i < this.m_fixedIndicators.Length; i++)
				{
					if (this.m_fixedIndicators[i])
					{
						this.m_fixedIndicators[i].UpdateIndicator(this, angle);
					}
				}
			}
		}

		// Token: 0x040044B5 RID: 17589
		private const string kAnim = "Anim";

		// Token: 0x040044B6 RID: 17590
		[SerializeField]
		private DraggableUIWindow m_window;

		// Token: 0x040044B7 RID: 17591
		[SerializeField]
		private FixedDirectionIndicator[] m_fixedIndicators;

		// Token: 0x040044B8 RID: 17592
		[SerializeField]
		private float m_spacing = 120f;

		// Token: 0x040044B9 RID: 17593
		[Tooltip("Fraction of Spacing in which outside of which we allow the indicator to scale down.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_spacingScaleFactor = 0.2f;

		// Token: 0x040044BA RID: 17594
		[SerializeField]
		private float m_baseNoiseSpeed = 0.2f;

		// Token: 0x040044BB RID: 17595
		[SerializeField]
		private float m_baseMaxOffset = 10f;

		// Token: 0x040044BC RID: 17596
		[SerializeField]
		private float m_baseSmoothTime = 0.0001f;

		// Token: 0x040044BD RID: 17597
		private RectTransform m_thisRectTransform;

		// Token: 0x040044BE RID: 17598
		[SerializeField]
		private RectTransform m_zoneLabelRect;

		// Token: 0x040044BF RID: 17599
		[SerializeField]
		private RectTransform m_compassPanelRect;

		// Token: 0x040044C0 RID: 17600
		[NonSerialized]
		private float m_variantValue;

		// Token: 0x040044C1 RID: 17601
		[NonSerialized]
		private float m_variantVelocity;

		// Token: 0x040044C2 RID: 17602
		[NonSerialized]
		private float m_perlinScroll;

		// Token: 0x040044C3 RID: 17603
		[NonSerialized]
		private float m_noiseSpeed;

		// Token: 0x040044C4 RID: 17604
		[NonSerialized]
		private float m_maxOffset;

		// Token: 0x040044C5 RID: 17605
		[NonSerialized]
		private float m_smoothTime;

		// Token: 0x040044C6 RID: 17606
		[NonSerialized]
		private Camera m_cachedMainCamera;

		// Token: 0x040044C7 RID: 17607
		[NonSerialized]
		private HDCamera m_cachedHdCamera;

		// Token: 0x040044C8 RID: 17608
		[SerializeField]
		private bool m_debug;

		// Token: 0x040044C9 RID: 17609
		[SerializeField]
		private float m_debugAngle;
	}
}
