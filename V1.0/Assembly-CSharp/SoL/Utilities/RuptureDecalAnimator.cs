using System;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002D7 RID: 727
	public class RuptureDecalAnimator : MonoBehaviour
	{
		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x060014FA RID: 5370 RVA: 0x00050A67 File Offset: 0x0004EC67
		// (set) Token: 0x060014FB RID: 5371 RVA: 0x00050A6E File Offset: 0x0004EC6E
		internal static Material SharedMaterial { get; private set; }

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x060014FC RID: 5372 RVA: 0x00050A76 File Offset: 0x0004EC76
		// (set) Token: 0x060014FD RID: 5373 RVA: 0x00050A7D File Offset: 0x0004EC7D
		internal static float Radius { get; private set; }

		// Token: 0x060014FE RID: 5374 RVA: 0x000FBCF0 File Offset: 0x000F9EF0
		private void Setup()
		{
			if (this.m_material)
			{
				FloatMaterialProperty emissionRadius = this.m_emissionRadius;
				if (emissionRadius != null)
				{
					emissionRadius.Init(this.m_material);
				}
				FloatMaterialProperty emissionFadeRadius = this.m_emissionFadeRadius;
				if (emissionFadeRadius != null)
				{
					emissionFadeRadius.Init(this.m_material);
				}
				FloatMaterialProperty emissionRadius2 = this.m_emissionRadius;
				if (emissionRadius2 != null)
				{
					emissionRadius2.SetValue(0f);
				}
				FloatMaterialProperty emissionFadeRadius2 = this.m_emissionFadeRadius;
				if (emissionFadeRadius2 != null)
				{
					emissionFadeRadius2.SetValue(0f);
				}
			}
			this.RefreshResetTime();
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x00050A85 File Offset: 0x0004EC85
		private void Awake()
		{
			if (GameManager.IsServer)
			{
				base.enabled = false;
			}
			RuptureDecalAnimator.SharedMaterial = this.m_material;
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00050AA0 File Offset: 0x0004ECA0
		private void Start()
		{
			this.Setup();
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x000FBD6C File Offset: 0x000F9F6C
		private void Update()
		{
			if (this.m_emissionRadius == null || this.m_emissionFadeRadius == null || !this.m_emissionRadius.HasMaterial || !this.m_emissionFadeRadius.HasMaterial)
			{
				return;
			}
			float num = this.m_emissionRadius.CurrentValue;
			num = Mathf.MoveTowards(num, 1f, Time.deltaTime * this.m_radiusSpeed);
			float num2 = this.m_emissionFadeRadius.CurrentValue;
			num2 = ((num >= this.m_fadeRadialFraction) ? Mathf.MoveTowards(num2, 1f, Time.deltaTime * this.m_fadeRadiusSpeed) : 0f);
			if (this.m_resetTime != null && Time.time >= this.m_resetTime.Value)
			{
				num = 0f;
				num2 = 0f;
				this.m_resetTime = null;
			}
			this.m_emissionRadius.SetValue(num);
			this.m_emissionFadeRadius.SetValue(num2);
			if (this.m_resetTime == null && num2 >= 1f && num >= 1f)
			{
				this.RefreshResetTime();
			}
			RuptureDecalAnimator.Radius = num;
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00050AA8 File Offset: 0x0004ECA8
		private void RefreshResetTime()
		{
			this.m_resetTime = new float?(Time.time + GlobalSettings.Values.Ashen.GetRandomRippleDelay());
		}

		// Token: 0x04001D44 RID: 7492
		[SerializeField]
		private Material m_material;

		// Token: 0x04001D45 RID: 7493
		[SerializeField]
		private FloatMaterialProperty m_emissionRadius;

		// Token: 0x04001D46 RID: 7494
		[SerializeField]
		private FloatMaterialProperty m_emissionFadeRadius;

		// Token: 0x04001D47 RID: 7495
		[SerializeField]
		private float m_radiusSpeed = 0.2f;

		// Token: 0x04001D48 RID: 7496
		[SerializeField]
		private float m_fadeRadialFraction = 0.2f;

		// Token: 0x04001D49 RID: 7497
		[SerializeField]
		private float m_fadeRadiusSpeed = 0.2f;

		// Token: 0x04001D4A RID: 7498
		private float? m_resetTime;
	}
}
