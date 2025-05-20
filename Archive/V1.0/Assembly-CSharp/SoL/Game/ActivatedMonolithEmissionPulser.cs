using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200054D RID: 1357
	public class ActivatedMonolithEmissionPulser : MonoBehaviour
	{
		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06002941 RID: 10561 RVA: 0x0005C7EF File Offset: 0x0005A9EF
		// (set) Token: 0x06002942 RID: 10562 RVA: 0x0005C7F7 File Offset: 0x0005A9F7
		internal bool EnablePulsing
		{
			get
			{
				return this.m_enabled;
			}
			set
			{
				this.m_enabled = value;
			}
		}

		// Token: 0x06002943 RID: 10563 RVA: 0x00141038 File Offset: 0x0013F238
		private void Start()
		{
			if (GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			if (this.m_toPulse == null || this.m_toToggle == null)
			{
				base.enabled = false;
				return;
			}
			this.m_setColorOnDestroy = this.m_toPulse.IsValid;
			this.m_toPulse.SetIntensity(this.m_currentIntensity);
			this.m_toToggle.SetIntensity(0f);
			this.m_decreasing = true;
		}

		// Token: 0x06002944 RID: 10564 RVA: 0x0005C800 File Offset: 0x0005AA00
		private void OnDestroy()
		{
			if (this.m_setColorOnDestroy && !GameManager.IsServer)
			{
				ActivatedMonolithEmissionPulser.MaterialEmissionSetting toPulse = this.m_toPulse;
				if (toPulse != null)
				{
					toPulse.OnDestroy();
				}
				ActivatedMonolithEmissionPulser.MaterialEmissionSetting toToggle = this.m_toToggle;
				if (toToggle == null)
				{
					return;
				}
				toToggle.OnDestroy();
			}
		}

		// Token: 0x06002945 RID: 10565 RVA: 0x001410A8 File Offset: 0x0013F2A8
		private void Update()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			float num = 0f;
			if (this.EnablePulsing)
			{
				num = (this.m_decreasing ? this.m_range.x : this.m_range.y);
				if (this.m_currentIntensity == num)
				{
					this.m_decreasing = !this.m_decreasing;
					num = (this.m_decreasing ? this.m_range.x : this.m_range.y);
				}
				ActivatedMonolithEmissionPulser.MaterialEmissionSetting toToggle = this.m_toToggle;
				if (toToggle != null)
				{
					toToggle.SetIntensity(1f);
				}
			}
			else
			{
				this.m_decreasing = false;
				ActivatedMonolithEmissionPulser.MaterialEmissionSetting toToggle2 = this.m_toToggle;
				if (toToggle2 != null)
				{
					toToggle2.SetIntensity(0f);
				}
			}
			if (this.m_currentIntensity != num)
			{
				this.m_currentIntensity = Mathf.MoveTowards(this.m_currentIntensity, num, Time.deltaTime * this.m_speed);
				ActivatedMonolithEmissionPulser.MaterialEmissionSetting toPulse = this.m_toPulse;
				if (toPulse == null)
				{
					return;
				}
				toPulse.SetIntensity(this.m_currentIntensity);
			}
		}

		// Token: 0x04002A39 RID: 10809
		private const string kEmissionColor = "_EmissiveColor";

		// Token: 0x04002A3A RID: 10810
		private static readonly int kEmissionColorId = Shader.PropertyToID("_EmissiveColor");

		// Token: 0x04002A3B RID: 10811
		[SerializeField]
		private bool m_enabled;

		// Token: 0x04002A3C RID: 10812
		[SerializeField]
		private float m_speed = 5f;

		// Token: 0x04002A3D RID: 10813
		[SerializeField]
		private Vector2 m_range = Vector2.one;

		// Token: 0x04002A3E RID: 10814
		[SerializeField]
		private ActivatedMonolithEmissionPulser.MaterialEmissionSetting m_toPulse;

		// Token: 0x04002A3F RID: 10815
		[SerializeField]
		private ActivatedMonolithEmissionPulser.MaterialEmissionSetting m_toToggle;

		// Token: 0x04002A40 RID: 10816
		private bool m_setColorOnDestroy;

		// Token: 0x04002A41 RID: 10817
		private float m_currentIntensity;

		// Token: 0x04002A42 RID: 10818
		private bool m_decreasing;

		// Token: 0x04002A43 RID: 10819
		private Color m_initialColorForToggle;

		// Token: 0x0200054E RID: 1358
		[Serializable]
		private class MaterialEmissionSetting
		{
			// Token: 0x17000878 RID: 2168
			// (get) Token: 0x06002948 RID: 10568 RVA: 0x0005C861 File Offset: 0x0005AA61
			public bool IsValid
			{
				get
				{
					return this.m_material != null;
				}
			}

			// Token: 0x06002949 RID: 10569 RVA: 0x0005C86F File Offset: 0x0005AA6F
			public void AssignColor()
			{
				if (this.m_material)
				{
					this.m_baseEmissionColor = this.m_material.GetColor(ActivatedMonolithEmissionPulser.kEmissionColorId);
				}
			}

			// Token: 0x0600294A RID: 10570 RVA: 0x0005C894 File Offset: 0x0005AA94
			public void OnDestroy()
			{
				if (this.m_material)
				{
					this.m_material.SetColor(ActivatedMonolithEmissionPulser.kEmissionColorId, this.m_baseEmissionColor);
				}
			}

			// Token: 0x0600294B RID: 10571 RVA: 0x0005C8B9 File Offset: 0x0005AAB9
			public void SetIntensity(float intensity)
			{
				if (this.m_material)
				{
					this.m_material.SetColor(ActivatedMonolithEmissionPulser.kEmissionColorId, this.m_baseEmissionColor * intensity);
				}
			}

			// Token: 0x04002A44 RID: 10820
			[SerializeField]
			private Material m_material;

			// Token: 0x04002A45 RID: 10821
			[SerializeField]
			private Color m_baseEmissionColor;
		}
	}
}
