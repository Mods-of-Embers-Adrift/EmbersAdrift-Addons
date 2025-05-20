using System;
using System.Collections.Generic;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x0200004E RID: 78
	public class VillageAmbienceMixer : AmbienceMixer<VillageAmbiencePreset>
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00045330 File Offset: 0x00043530
		// (set) Token: 0x06000185 RID: 389 RVA: 0x00045338 File Offset: 0x00043538
		public SoundElement Birds
		{
			get
			{
				return this._birds;
			}
			private set
			{
				this._birds = this.Birds;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00045346 File Offset: 0x00043546
		// (set) Token: 0x06000187 RID: 391 RVA: 0x0004534E File Offset: 0x0004354E
		public SoundElement Rain
		{
			get
			{
				return this._rain;
			}
			private set
			{
				this._rain = this.Rain;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0004535C File Offset: 0x0004355C
		// (set) Token: 0x06000189 RID: 393 RVA: 0x00045364 File Offset: 0x00043564
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00045372 File Offset: 0x00043572
		// (set) Token: 0x0600018B RID: 395 RVA: 0x0004537A File Offset: 0x0004357A
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00045388 File Offset: 0x00043588
		// (set) Token: 0x0600018D RID: 397 RVA: 0x00045390 File Offset: 0x00043590
		public SoundElement Crowd
		{
			get
			{
				return this._crowd;
			}
			private set
			{
				this._crowd = this.Crowd;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600018E RID: 398 RVA: 0x0004539E File Offset: 0x0004359E
		// (set) Token: 0x0600018F RID: 399 RVA: 0x000453A6 File Offset: 0x000435A6
		public SoundElement Blacksmith
		{
			get
			{
				return this._blacksmith;
			}
			private set
			{
				this._blacksmith = this.Blacksmith;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000190 RID: 400 RVA: 0x000453B4 File Offset: 0x000435B4
		// (set) Token: 0x06000191 RID: 401 RVA: 0x000453BC File Offset: 0x000435BC
		public SoundElement Lumbermill
		{
			get
			{
				return this._lumbermill;
			}
			private set
			{
				this._lumbermill = this.Lumbermill;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000192 RID: 402 RVA: 0x000453CA File Offset: 0x000435CA
		// (set) Token: 0x06000193 RID: 403 RVA: 0x000453D2 File Offset: 0x000435D2
		public SoundElement HumanActivity
		{
			get
			{
				return this._humanActivity;
			}
			private set
			{
				this._humanActivity = this.HumanActivity;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000194 RID: 404 RVA: 0x000453E0 File Offset: 0x000435E0
		// (set) Token: 0x06000195 RID: 405 RVA: 0x000453E8 File Offset: 0x000435E8
		public SoundElement FarmAnimals
		{
			get
			{
				return this._farmAnimals;
			}
			private set
			{
				this._farmAnimals = this.FarmAnimals;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0009A208 File Offset: 0x00098408
		protected override List<SoundElement> elements
		{
			get
			{
				if (this._elements == null)
				{
					this._elements = new List<SoundElement>
					{
						this._birds,
						this._rain,
						this._waterStream,
						this._fire,
						this._crowd,
						this._blacksmith,
						this._lumbermill,
						this._humanActivity,
						this._farmAnimals
					};
				}
				return this._elements;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0009A29C File Offset: 0x0009849C
		public override void ApplyPreset(VillageAmbiencePreset selectedPreset)
		{
			this._birds.SetIntensity(selectedPreset.birdsIntensity);
			this._birds.SetVolumeMultiplier(selectedPreset.birdsVolumeMultiplier);
			this._rain.SetIntensity(selectedPreset.rainIntensity);
			this._rain.SetVolumeMultiplier(selectedPreset.rainVolumeMultiplier);
			this._waterStream.SetIntensity(selectedPreset.waterStreamIntensity);
			this._waterStream.SetVolumeMultiplier(selectedPreset.waterStreamVolumeMultiplier);
			this._fire.SetIntensity(selectedPreset.fireIntensity);
			this._fire.SetVolumeMultiplier(selectedPreset.fireVolumeMultiplier);
			this._crowd.SetIntensity(selectedPreset.crowdIntensity);
			this._crowd.SetVolumeMultiplier(selectedPreset.crowdVolumeMultiplier);
			this._blacksmith.SetIntensity(selectedPreset.blacksmithIntensity);
			this._blacksmith.SetVolumeMultiplier(selectedPreset.blacksmithVolumeMultiplier);
			this._lumbermill.SetIntensity(selectedPreset.lumbermillIntensity);
			this._lumbermill.SetVolumeMultiplier(selectedPreset.lumbermillVolumeMultiplier);
			this._humanActivity.SetIntensity(selectedPreset.humanActivityIntensity);
			this._humanActivity.SetVolumeMultiplier(selectedPreset.humanActivityVolumeMultiplier);
			this._farmAnimals.SetIntensity(selectedPreset.farmAnimalsIntensity);
			this._farmAnimals.SetVolumeMultiplier(selectedPreset.farmAnimalsVolumeMultiplier);
		}

		// Token: 0x0400037E RID: 894
		[SerializeField]
		private SoundElement _birds;

		// Token: 0x0400037F RID: 895
		[SerializeField]
		private SoundElement _rain;

		// Token: 0x04000380 RID: 896
		[SerializeField]
		private SoundElement _waterStream;

		// Token: 0x04000381 RID: 897
		[SerializeField]
		private SoundElement _fire;

		// Token: 0x04000382 RID: 898
		[SerializeField]
		private SoundElement _crowd;

		// Token: 0x04000383 RID: 899
		[SerializeField]
		private SoundElement _blacksmith;

		// Token: 0x04000384 RID: 900
		[SerializeField]
		private SoundElement _lumbermill;

		// Token: 0x04000385 RID: 901
		[SerializeField]
		private SoundElement _humanActivity;

		// Token: 0x04000386 RID: 902
		[SerializeField]
		private SoundElement _farmAnimals;

		// Token: 0x04000387 RID: 903
		private List<SoundElement> _elements;
	}
}
