using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UMA
{
	// Token: 0x0200061F RID: 1567
	public class DualDnaSlider : MonoBehaviour
	{
		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x0600319C RID: 12700 RVA: 0x00062373 File Offset: 0x00060573
		// (set) Token: 0x0600319D RID: 12701 RVA: 0x00062380 File Offset: 0x00060580
		public float Value
		{
			get
			{
				return this.m_slider.value;
			}
			set
			{
				this.m_slider.value = value;
			}
		}

		// Token: 0x0600319E RID: 12702 RVA: 0x0006238E File Offset: 0x0006058E
		private void Awake()
		{
			this.m_slider.value = 0.5f;
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.SliderChanged));
		}

		// Token: 0x0600319F RID: 12703 RVA: 0x000623BC File Offset: 0x000605BC
		private void OnDestroy()
		{
			this.m_slider.onValueChanged.RemoveListener(new UnityAction<float>(this.SliderChanged));
		}

		// Token: 0x060031A0 RID: 12704 RVA: 0x000623DA File Offset: 0x000605DA
		private void Update()
		{
			if (this.m_activeRace != this.m_dca.activeRace.name)
			{
				this.SetDna();
				this.SliderChanged(this.Value);
			}
		}

		// Token: 0x060031A1 RID: 12705 RVA: 0x0006240B File Offset: 0x0006060B
		private void SetDna()
		{
			this.m_dna = this.m_dca.GetDNA(null);
			this.m_activeRace = this.m_dca.activeRace.name;
		}

		// Token: 0x060031A2 RID: 12706 RVA: 0x0015D410 File Offset: 0x0015B610
		private void SliderChanged(float value)
		{
			if (this.m_dna == null)
			{
				this.SetDna();
			}
			if (this.m_dna == null)
			{
				return;
			}
			DnaSetter dnaSetter;
			if (this.m_dna.TryGetValue(this.m_dnaLeft, out dnaSetter))
			{
				if (value >= 0.5f)
				{
					dnaSetter.Set(0f);
				}
				else
				{
					dnaSetter.Set(Mathf.Lerp(1f, 0f, value * 2f));
				}
			}
			DnaSetter dnaSetter2;
			if (this.m_dna.TryGetValue(this.m_dnaRight, out dnaSetter2))
			{
				if (value <= 0.5f)
				{
					dnaSetter2.Set(0f);
				}
				else
				{
					dnaSetter2.Set(Mathf.Lerp(0f, 1f, (value - 0.5f) * 2f));
				}
			}
			this.m_dca.Refresh(true, true, true);
		}

		// Token: 0x04003006 RID: 12294
		private const float kDefaultValue = 0.5f;

		// Token: 0x04003007 RID: 12295
		[SerializeField]
		private DynamicCharacterAvatar m_dca;

		// Token: 0x04003008 RID: 12296
		[SerializeField]
		private Slider m_slider;

		// Token: 0x04003009 RID: 12297
		[SerializeField]
		private string m_dnaLeft;

		// Token: 0x0400300A RID: 12298
		[SerializeField]
		private string m_dnaRight;

		// Token: 0x0400300B RID: 12299
		private Dictionary<string, DnaSetter> m_dna;

		// Token: 0x0400300C RID: 12300
		private string m_activeRace;
	}
}
