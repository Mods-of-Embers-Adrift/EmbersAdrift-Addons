using System;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Audio;

namespace SoL.Game.UI
{
	// Token: 0x020008C9 RID: 2249
	[Serializable]
	public class VolumeSlider : FloatSlider
	{
		// Token: 0x17000EFF RID: 3839
		// (get) Token: 0x060041C2 RID: 16834 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showSliderConfig
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060041C3 RID: 16835 RVA: 0x0006C6F4 File Offset: 0x0006A8F4
		public void VolumeSliderInit()
		{
			base.Init(this.GetOption());
		}

		// Token: 0x060041C4 RID: 16836 RVA: 0x001905B4 File Offset: 0x0018E7B4
		protected override bool InitInternal()
		{
			AudioMixerGroup mixerGroup = this.GetMixerGroup();
			if (this.m_obj == null || this.m_obj.Slider == null || mixerGroup == null || this.m_option == null)
			{
				return false;
			}
			this.m_mixerGroupName = mixerGroup.name + "Volume";
			float value;
			if (!GlobalSettings.Values.Audio.Mixer.GetFloat(this.m_mixerGroupName, out value))
			{
				Debug.LogWarning("Unable to get mixer value for " + this.m_mixerGroupName);
				return false;
			}
			this.m_maxMixerValue = value.ConvertFromDecibel();
			this.m_option.Value = Mathf.Clamp(this.m_option.Value, 0f, 1f);
			if (!GlobalSettings.Values.Audio.Mixer.SetFloat(this.m_mixerGroupName, this.m_option.Value.ConvertToDecibel()))
			{
				Debug.LogWarning("Unable to set mixer value for " + this.m_mixerGroupName);
				return false;
			}
			this.m_obj.Slider.wholeNumbers = false;
			this.m_obj.Slider.minValue = 0f;
			this.m_obj.Slider.maxValue = 1f;
			this.m_obj.Slider.value = this.m_option.Value;
			this.SetMixerValue(this.m_option.Value);
			return true;
		}

		// Token: 0x060041C5 RID: 16837 RVA: 0x0006C702 File Offset: 0x0006A902
		protected override void OnSliderChanged(float sliderValue)
		{
			this.SetMixerValue(sliderValue);
			base.OnSliderChanged(sliderValue);
		}

		// Token: 0x060041C6 RID: 16838 RVA: 0x00190724 File Offset: 0x0018E924
		private void SetMixerValue(float optionValue)
		{
			float value = Mathf.Lerp(0f, this.m_maxMixerValue, optionValue);
			GlobalSettings.Values.Audio.Mixer.SetFloat(this.m_mixerGroupName, value.ConvertToDecibel());
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x00190764 File Offset: 0x0018E964
		private Options.Option_Float GetOption()
		{
			switch (this.m_type)
			{
			case VolumeSlider.VolumeSliderType.Master:
				return Options.AudioOptions.MasterVolume;
			case VolumeSlider.VolumeSliderType.Effects:
				return Options.AudioOptions.EffectsVolume;
			case VolumeSlider.VolumeSliderType.Music:
				return Options.AudioOptions.MusicVolume;
			case VolumeSlider.VolumeSliderType.Aura:
				return Options.AudioOptions.AuraVolume;
			case VolumeSlider.VolumeSliderType.Environment:
				return Options.AudioOptions.EnvironmentVolume;
			case VolumeSlider.VolumeSliderType.UI:
				return Options.AudioOptions.UIVolume;
			case VolumeSlider.VolumeSliderType.Footsteps:
				return Options.AudioOptions.FootstepsVolume;
			default:
				throw new ArgumentException("m_type");
			}
		}

		// Token: 0x060041C8 RID: 16840 RVA: 0x001907D4 File Offset: 0x0018E9D4
		private AudioMixerGroup GetMixerGroup()
		{
			switch (this.m_type)
			{
			case VolumeSlider.VolumeSliderType.Master:
				return GlobalSettings.Values.Audio.MasterMixerGroup;
			case VolumeSlider.VolumeSliderType.Effects:
				return GlobalSettings.Values.Audio.SfxParentMixerGroup;
			case VolumeSlider.VolumeSliderType.Music:
				return GlobalSettings.Values.Audio.MusicMixerGroup;
			case VolumeSlider.VolumeSliderType.Aura:
				return GlobalSettings.Values.Audio.AuraMixerGroup;
			case VolumeSlider.VolumeSliderType.Environment:
				return GlobalSettings.Values.Audio.AmbientMixerGroup;
			case VolumeSlider.VolumeSliderType.UI:
				return GlobalSettings.Values.Audio.UIMixerGroup;
			case VolumeSlider.VolumeSliderType.Footsteps:
				return GlobalSettings.Values.Audio.FootstepMixerGroup;
			default:
				throw new ArgumentException("m_type");
			}
		}

		// Token: 0x04003EFF RID: 16127
		[SerializeField]
		private VolumeSlider.VolumeSliderType m_type;

		// Token: 0x04003F00 RID: 16128
		private string m_mixerGroupName;

		// Token: 0x04003F01 RID: 16129
		private float m_maxMixerValue;

		// Token: 0x020008CA RID: 2250
		private enum VolumeSliderType
		{
			// Token: 0x04003F03 RID: 16131
			None,
			// Token: 0x04003F04 RID: 16132
			Master,
			// Token: 0x04003F05 RID: 16133
			Effects,
			// Token: 0x04003F06 RID: 16134
			Music,
			// Token: 0x04003F07 RID: 16135
			Aura,
			// Token: 0x04003F08 RID: 16136
			Environment,
			// Token: 0x04003F09 RID: 16137
			UI,
			// Token: 0x04003F0A RID: 16138
			Footsteps
		}
	}
}
