using System;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B59 RID: 2905
	public class DnaSlider : Feature
	{
		// Token: 0x0600596B RID: 22891 RVA: 0x0007BDC7 File Offset: 0x00079FC7
		private static string[] GetDNAToAlter(UMADnaType val)
		{
			if (val == UMADnaType.weight)
			{
				return DnaSlider.kUmaWeightTypes;
			}
			if (val != UMADnaType.muscle)
			{
				return new string[]
				{
					val.ToString()
				};
			}
			return DnaSlider.kUmaMuscleTypes;
		}

		// Token: 0x0600596C RID: 22892 RVA: 0x001E9EF8 File Offset: 0x001E80F8
		public override void Initialize(CreationDirector director, CreationDirector.CharacterToCreate toCreate, CreationDirector.FeatureSetting settings)
		{
			if (settings.Sliders.Length != 1)
			{
				throw new ArgumentException("Too many sliders!  Did you want a cluster slider?");
			}
			base.Initialize(director, toCreate, settings);
			this.m_label.text = settings.Sliders[0].ToString();
			CreationDirector.CharacterToCreate characterToCreate = (toCreate == null) ? this.m_director.CurrentCharacter : toCreate;
			UMADnaType umadnaType = this.m_settings.Sliders[0];
			string[] dnatoAlter = DnaSlider.GetDNAToAlter(umadnaType);
			float value = characterToCreate.DNASetters[dnatoAlter[0]].Value;
			float[] array = new float[dnatoAlter.Length];
			for (int i = 0; i < dnatoAlter.Length; i++)
			{
				array[i] = value - characterToCreate.DNASetters[dnatoAlter[i]].Value;
			}
			this.m_dnaSettings = new DnaSlider.DnaSettings
			{
				Restriction = UMAManager.DnaRestrictions.GetRestriction(umadnaType),
				Dna = dnatoAlter,
				DefaultValue = value,
				Deltas = array
			};
			this.m_ignoreChanges = true;
			this.m_slider.value = this.m_dnaSettings.DefaultValue;
			this.m_ignoreChanges = false;
		}

		// Token: 0x0600596D RID: 22893 RVA: 0x0007BDF5 File Offset: 0x00079FF5
		protected override void Subscribe()
		{
			base.Subscribe();
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
		}

		// Token: 0x0600596E RID: 22894 RVA: 0x0007BE19 File Offset: 0x0007A019
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			this.m_slider.onValueChanged.RemoveAllListeners();
		}

		// Token: 0x0600596F RID: 22895 RVA: 0x0007BE31 File Offset: 0x0007A031
		public override void OnLockStateChanged(ToggleController.ToggleState obj)
		{
			base.OnLockStateChanged(obj);
			this.m_slider.interactable = !base.m_locked;
		}

		// Token: 0x06005970 RID: 22896 RVA: 0x001EA018 File Offset: 0x001E8218
		private void OnSliderChanged(float value)
		{
			if (this.m_ignoreChanges)
			{
				return;
			}
			float num = value;
			if (this.m_dnaSettings.Restriction != null)
			{
				num = this.m_dnaSettings.Restriction.GetNormalizedValue(num);
			}
			for (int i = 0; i < this.m_dnaSettings.Dna.Length; i++)
			{
				float num2 = num - this.m_dnaSettings.Deltas[i];
				if (this.m_character == null)
				{
					this.m_director.SetSharedSlider(this.m_dnaSettings.Dna[i], num2);
				}
				else
				{
					this.m_character.DNASetters[this.m_dnaSettings.Dna[i]].Set(num2);
				}
			}
			if (this.m_character != null)
			{
				this.m_character.DCA.Refresh(true, true, true);
				return;
			}
			this.m_director.RefreshAll();
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x0007BE4E File Offset: 0x0007A04E
		public override void Reset()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Reset();
			this.m_slider.value = this.m_dnaSettings.DefaultValue;
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x0007BE75 File Offset: 0x0007A075
		public override void Randomize()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Randomize();
			this.m_slider.value = UnityEngine.Random.Range(0f, 1f);
		}

		// Token: 0x04004EAD RID: 20141
		public static string[] kUmaWeightTypes = new string[]
		{
			"upperWeight",
			"lowerWeight",
			"belly",
			"waist"
		};

		// Token: 0x04004EAE RID: 20142
		public static string[] kUmaMuscleTypes = new string[]
		{
			"upperMuscle",
			"lowerMuscle",
			"armWidth",
			"forearmWidth"
		};

		// Token: 0x04004EAF RID: 20143
		[SerializeField]
		private Slider m_slider;

		// Token: 0x04004EB0 RID: 20144
		private bool m_ignoreChanges;

		// Token: 0x04004EB1 RID: 20145
		private DnaSlider.DnaSettings m_dnaSettings;

		// Token: 0x02000B5A RID: 2906
		private struct DnaSettings
		{
			// Token: 0x04004EB2 RID: 20146
			public DNASliderRestriction Restriction;

			// Token: 0x04004EB3 RID: 20147
			public string[] Dna;

			// Token: 0x04004EB4 RID: 20148
			public float DefaultValue;

			// Token: 0x04004EB5 RID: 20149
			public float[] Deltas;
		}
	}
}
