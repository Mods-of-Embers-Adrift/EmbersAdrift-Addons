using System;
using System.Collections.Generic;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x0200004C RID: 76
	public class CaveAmbientMixer : AmbienceMixer<CaveAmbientPreset>
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00045262 File Offset: 0x00043462
		// (set) Token: 0x0600016F RID: 367 RVA: 0x0004526A File Offset: 0x0004346A
		public SoundElement Atmosphere1
		{
			get
			{
				return this._atmosphere1;
			}
			private set
			{
				this._atmosphere1 = this.Atmosphere1;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00045278 File Offset: 0x00043478
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00045280 File Offset: 0x00043480
		public SoundElement Atmosphere2
		{
			get
			{
				return this._atmosphere2;
			}
			private set
			{
				this._atmosphere2 = this.Atmosphere2;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000172 RID: 370 RVA: 0x0004528E File Offset: 0x0004348E
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00045296 File Offset: 0x00043496
		public SoundElement Atmosphere3
		{
			get
			{
				return this._atmosphere3;
			}
			private set
			{
				this._atmosphere3 = this.Atmosphere3;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000174 RID: 372 RVA: 0x000452A4 File Offset: 0x000434A4
		// (set) Token: 0x06000175 RID: 373 RVA: 0x000452AC File Offset: 0x000434AC
		public SoundElement Sediment
		{
			get
			{
				return this._sediment;
			}
			private set
			{
				this._sediment = this.Sediment;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000176 RID: 374 RVA: 0x000452BA File Offset: 0x000434BA
		// (set) Token: 0x06000177 RID: 375 RVA: 0x000452C2 File Offset: 0x000434C2
		public SoundElement WaterDrops
		{
			get
			{
				return this._waterDrops;
			}
			private set
			{
				this._waterDrops = this.WaterDrops;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000178 RID: 376 RVA: 0x000452D0 File Offset: 0x000434D0
		// (set) Token: 0x06000179 RID: 377 RVA: 0x000452D8 File Offset: 0x000434D8
		public SoundElement WaterStream
		{
			get
			{
				return this._waterStream;
			}
			private set
			{
				this._waterStream = this.WaterStream;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600017A RID: 378 RVA: 0x000452E6 File Offset: 0x000434E6
		// (set) Token: 0x0600017B RID: 379 RVA: 0x000452EE File Offset: 0x000434EE
		public SoundElement Sewer
		{
			get
			{
				return this._sewer;
			}
			private set
			{
				this._sewer = this.Sewer;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600017C RID: 380 RVA: 0x000452FC File Offset: 0x000434FC
		// (set) Token: 0x0600017D RID: 381 RVA: 0x00045304 File Offset: 0x00043504
		public SoundElement Fire
		{
			get
			{
				return this._fire;
			}
			private set
			{
				this._fire = this.Fire;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00045312 File Offset: 0x00043512
		// (set) Token: 0x0600017F RID: 383 RVA: 0x0004531A File Offset: 0x0004351A
		public SoundElement Critters
		{
			get
			{
				return this._critters;
			}
			private set
			{
				this._critters = this.Critters;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00099FDC File Offset: 0x000981DC
		protected override List<SoundElement> elements
		{
			get
			{
				if (this._elements == null)
				{
					this._elements = new List<SoundElement>
					{
						this._atmosphere1,
						this._atmosphere2,
						this._atmosphere3,
						this._sediment,
						this._waterDrops,
						this._waterStream,
						this._sewer,
						this._fire,
						this._critters
					};
				}
				return this._elements;
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0009A070 File Offset: 0x00098270
		public override void ApplyPreset(CaveAmbientPreset selectedPreset)
		{
			this._atmosphere1.SetIntensity(selectedPreset.atmosphere1Intensity);
			this._atmosphere1.SetVolumeMultiplier(selectedPreset.atmosphere1VolumeMultiplier);
			this._atmosphere2.SetIntensity(selectedPreset.atmosphere2Intensity);
			this._atmosphere2.SetVolumeMultiplier(selectedPreset.atmosphere2VolumeMultiplier);
			this._atmosphere3.SetIntensity(selectedPreset.atmosphere3Intensity);
			this._atmosphere3.SetVolumeMultiplier(selectedPreset.atmosphere3VolumeMultiplier);
			this._sediment.SetIntensity(selectedPreset.sedimentIntensity);
			this._sediment.SetVolumeMultiplier(selectedPreset.sedimentVolumeMultiplier);
			this._waterDrops.SetIntensity(selectedPreset.waterDropsIntensity);
			this._waterDrops.SetVolumeMultiplier(selectedPreset.waterDropsVolumeMultiplier);
			this._waterStream.SetIntensity(selectedPreset.waterDropsIntensity);
			this._waterStream.SetVolumeMultiplier(selectedPreset.waterDropsVolumeMultiplier);
			this._sewer.SetIntensity(selectedPreset.sewerIntensity);
			this._sewer.SetVolumeMultiplier(selectedPreset.sewerVolumeMultiplier);
			this._fire.SetIntensity(selectedPreset.fireIntensity);
			this._fire.SetVolumeMultiplier(selectedPreset.fireVolumeMultiplier);
			this._critters.SetIntensity(selectedPreset.crittersIntensity);
			this._critters.SetVolumeMultiplier(selectedPreset.crittersVolumeMultiplier);
		}

		// Token: 0x04000362 RID: 866
		[SerializeField]
		private SoundElement _atmosphere1;

		// Token: 0x04000363 RID: 867
		[SerializeField]
		private SoundElement _atmosphere2;

		// Token: 0x04000364 RID: 868
		[SerializeField]
		private SoundElement _atmosphere3;

		// Token: 0x04000365 RID: 869
		[SerializeField]
		private SoundElement _sediment;

		// Token: 0x04000366 RID: 870
		[SerializeField]
		private SoundElement _waterDrops;

		// Token: 0x04000367 RID: 871
		[SerializeField]
		private SoundElement _waterStream;

		// Token: 0x04000368 RID: 872
		[SerializeField]
		private SoundElement _sewer;

		// Token: 0x04000369 RID: 873
		[SerializeField]
		private SoundElement _fire;

		// Token: 0x0400036A RID: 874
		[SerializeField]
		private SoundElement _critters;

		// Token: 0x0400036B RID: 875
		private List<SoundElement> _elements;
	}
}
