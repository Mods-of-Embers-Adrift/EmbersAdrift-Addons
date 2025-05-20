using System;
using SoL.Game.SkyDome;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D2C RID: 3372
	[CreateAssetMenu(menuName = "SoL/Profiles/Ambient Audio Zone")]
	public class AmbientAudioZoneProfile : ScriptableObject
	{
		// Token: 0x06006576 RID: 25974 RVA: 0x0020CEBC File Offset: 0x0020B0BC
		private string GetMainClipName()
		{
			AmbientAudioZoneProfile.ClipTypes clipTypes = this.m_clipTypes;
			if (clipTypes != AmbientAudioZoneProfile.ClipTypes.Single && clipTypes == AmbientAudioZoneProfile.ClipTypes.DayNight)
			{
				return "Day Clip";
			}
			return "Clip";
		}

		// Token: 0x17001857 RID: 6231
		// (get) Token: 0x06006577 RID: 25975 RVA: 0x00084513 File Offset: 0x00082713
		public float FadeTime
		{
			get
			{
				return this.m_fadeTime;
			}
		}

		// Token: 0x17001858 RID: 6232
		// (get) Token: 0x06006578 RID: 25976 RVA: 0x0008451B File Offset: 0x0008271B
		public float Volume
		{
			get
			{
				return this.m_volume;
			}
		}

		// Token: 0x17001859 RID: 6233
		// (get) Token: 0x06006579 RID: 25977 RVA: 0x00084523 File Offset: 0x00082723
		public float SpatialBlend
		{
			get
			{
				return this.m_spatialBlend;
			}
		}

		// Token: 0x1700185A RID: 6234
		// (get) Token: 0x0600657A RID: 25978 RVA: 0x0008452B File Offset: 0x0008272B
		public Vector3 MovementAmplitude
		{
			get
			{
				return this.m_movementAmplitude;
			}
		}

		// Token: 0x1700185B RID: 6235
		// (get) Token: 0x0600657B RID: 25979 RVA: 0x00084533 File Offset: 0x00082733
		public Vector3 MovementFrequency
		{
			get
			{
				return this.m_movementFrequency;
			}
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x0020CEE4 File Offset: 0x0020B0E4
		public AudioClip GetAudioClip()
		{
			AmbientAudioZoneProfile.ClipTypes clipTypes = this.m_clipTypes;
			if (clipTypes == AmbientAudioZoneProfile.ClipTypes.Single)
			{
				return this.m_clip;
			}
			if (clipTypes != AmbientAudioZoneProfile.ClipTypes.DayNight)
			{
				throw new ArgumentException("m_clipTypes");
			}
			if (!this.m_nightClip || SkyDomeManager.SkyDomeController == null || SkyDomeManager.SkyDomeController.IsDay)
			{
				return this.m_clip;
			}
			return this.m_nightClip;
		}

		// Token: 0x04005835 RID: 22581
		[SerializeField]
		private AmbientAudioZoneProfile.ClipTypes m_clipTypes;

		// Token: 0x04005836 RID: 22582
		[SerializeField]
		private AudioClip m_clip;

		// Token: 0x04005837 RID: 22583
		[SerializeField]
		private AudioClip m_nightClip;

		// Token: 0x04005838 RID: 22584
		[SerializeField]
		private float m_fadeTime = 5f;

		// Token: 0x04005839 RID: 22585
		[SerializeField]
		private float m_volume = 0.5f;

		// Token: 0x0400583A RID: 22586
		[SerializeField]
		private float m_spatialBlend = 0.8f;

		// Token: 0x0400583B RID: 22587
		[SerializeField]
		private Vector3 m_movementAmplitude = new Vector3(10f, 1f, 10f);

		// Token: 0x0400583C RID: 22588
		[SerializeField]
		private Vector3 m_movementFrequency = new Vector3(0.1f, 0.1f, 0.1f);

		// Token: 0x02000D2D RID: 3373
		private enum ClipTypes
		{
			// Token: 0x0400583E RID: 22590
			Single,
			// Token: 0x0400583F RID: 22591
			DayNight
		}
	}
}
