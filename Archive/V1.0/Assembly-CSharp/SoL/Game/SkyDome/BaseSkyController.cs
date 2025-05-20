using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006E5 RID: 1765
	public abstract class BaseSkyController : MonoBehaviour, ISkyDomeController
	{
		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x0600354F RID: 13647 RVA: 0x000647AB File Offset: 0x000629AB
		// (set) Token: 0x06003550 RID: 13648 RVA: 0x000647B3 File Offset: 0x000629B3
		protected bool IsDay
		{
			get
			{
				return this.m_isDay;
			}
			set
			{
				if (this.m_isDay == value && this.IsInitialized)
				{
					return;
				}
				this.m_isDay = value;
				this.ValidateLighting();
				Action dayNightChangedEvent = this.DayNightChangedEvent;
				if (dayNightChangedEvent != null)
				{
					dayNightChangedEvent();
				}
				SkyDomeManager.RefreshActiveLight();
			}
		}

		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x06003551 RID: 13649 RVA: 0x000647EA File Offset: 0x000629EA
		// (set) Token: 0x06003552 RID: 13650 RVA: 0x000647F2 File Offset: 0x000629F2
		private protected bool IsInitialized { protected get; private set; }

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x06003553 RID: 13651 RVA: 0x000647FB File Offset: 0x000629FB
		// (set) Token: 0x06003554 RID: 13652 RVA: 0x00064803 File Offset: 0x00062A03
		private protected float SunAltitude { protected get; private set; }

		// Token: 0x06003555 RID: 13653 RVA: 0x000522F9 File Offset: 0x000504F9
		protected virtual Color GetLightColor()
		{
			return Color.white;
		}

		// Token: 0x06003556 RID: 13654 RVA: 0x0006480C File Offset: 0x00062A0C
		protected virtual float GetLightTemperature()
		{
			return 6500f;
		}

		// Token: 0x06003557 RID: 13655 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual Light GetActiveLight()
		{
			return null;
		}

		// Token: 0x06003558 RID: 13656 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateCelestialsInternal(double sunZenithAngle, double sunAzimuthAngle)
		{
		}

		// Token: 0x06003559 RID: 13657 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateIsDayInternal()
		{
		}

		// Token: 0x0600355A RID: 13658 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ValidateLighting()
		{
		}

		// Token: 0x0600355B RID: 13659 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual WindZone GetWindZone()
		{
			return null;
		}

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x0600355C RID: 13660 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool IsIndoors
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600355D RID: 13661 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ResetSkybox()
		{
		}

		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x0600355E RID: 13662 RVA: 0x00064813 File Offset: 0x00062A13
		// (set) Token: 0x0600355F RID: 13663 RVA: 0x0006481B File Offset: 0x00062A1B
		private protected float D { protected get; private set; }

		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x06003560 RID: 13664 RVA: 0x00064824 File Offset: 0x00062A24
		// (set) Token: 0x06003561 RID: 13665 RVA: 0x0006482C File Offset: 0x00062A2C
		private protected float Ecl { protected get; private set; }

		// Token: 0x06003562 RID: 13666 RVA: 0x00064835 File Offset: 0x00062A35
		protected virtual void Initialize()
		{
			if (this.IsInitialized)
			{
				return;
			}
			if (this.m_initializeTime)
			{
				this.SetTime(SkyDomeManager.GetCorrectedGameDateTime(), true);
			}
			this.UpdateCelestials();
			SkyDomeManager.SkyDomeController = this;
			this.IsInitialized = true;
		}

		// Token: 0x06003563 RID: 13667 RVA: 0x00166CD8 File Offset: 0x00164ED8
		protected void UpdateCelestials()
		{
			SkyDomeUtilities.SetLatLong();
			float internalHour;
			float d;
			float ecl;
			SkyDomeUtilities.GetSkyValues(this.m_internalDateTime.DateTime, out internalHour, out d, out ecl);
			this.D = d;
			this.Ecl = ecl;
			double sunAzimuthAngle;
			double num;
			SkyDomeUtilities.CalculateSunPositionEnv(internalHour, d, ecl, (float)SkyDomeUtilities.Latitude, (float)SkyDomeUtilities.Longitude, out sunAzimuthAngle, out num);
			this.SunAltitude = (float)num;
			this.UpdateCelestialsInternal(num, sunAzimuthAngle);
			this.UpdateIsDayInternal();
		}

		// Token: 0x06003564 RID: 13668 RVA: 0x00064867 File Offset: 0x00062A67
		protected virtual void SetTime(DateTime time, bool updateReflections)
		{
			this.m_internalDateTime.DateTime = time;
		}

		// Token: 0x140000AE RID: 174
		// (add) Token: 0x06003565 RID: 13669 RVA: 0x00166D40 File Offset: 0x00164F40
		// (remove) Token: 0x06003566 RID: 13670 RVA: 0x00166D78 File Offset: 0x00164F78
		private event Action DayNightChangedEvent;

		// Token: 0x140000AF RID: 175
		// (add) Token: 0x06003567 RID: 13671 RVA: 0x00064875 File Offset: 0x00062A75
		// (remove) Token: 0x06003568 RID: 13672 RVA: 0x0006487E File Offset: 0x00062A7E
		event Action ISkyDomeController.DayNightChanged
		{
			add
			{
				this.DayNightChangedEvent += value;
			}
			remove
			{
				this.DayNightChangedEvent -= value;
			}
		}

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x06003569 RID: 13673 RVA: 0x00064887 File Offset: 0x00062A87
		bool ISkyDomeController.IsDay
		{
			get
			{
				return this.IsDay;
			}
		}

		// Token: 0x0600356A RID: 13674 RVA: 0x0006488F File Offset: 0x00062A8F
		void ISkyDomeController.SetTime(DateTime time)
		{
			this.SetTime(time, true);
		}

		// Token: 0x0600356B RID: 13675 RVA: 0x00064899 File Offset: 0x00062A99
		DateTime ISkyDomeController.GetTime()
		{
			return this.m_internalDateTime.DateTime;
		}

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x0600356C RID: 13676 RVA: 0x000648A6 File Offset: 0x00062AA6
		Light ISkyDomeController.Light
		{
			get
			{
				return this.GetActiveLight();
			}
		}

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x0600356D RID: 13677 RVA: 0x000648AE File Offset: 0x00062AAE
		WindZone ISkyDomeController.Wind
		{
			get
			{
				return this.GetWindZone();
			}
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x0600356E RID: 13678 RVA: 0x000648B6 File Offset: 0x00062AB6
		// (set) Token: 0x0600356F RID: 13679 RVA: 0x000648BE File Offset: 0x00062ABE
		bool ISkyDomeController.ProgressTime
		{
			get
			{
				return this.m_progressTime;
			}
			set
			{
				this.m_progressTime = value;
			}
		}

		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x06003570 RID: 13680 RVA: 0x000648C7 File Offset: 0x00062AC7
		Color ISkyDomeController.LightColor
		{
			get
			{
				return this.GetLightColor();
			}
		}

		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x06003571 RID: 13681 RVA: 0x000648CF File Offset: 0x00062ACF
		float ISkyDomeController.LightTemperature
		{
			get
			{
				return this.GetLightTemperature();
			}
		}

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x06003572 RID: 13682 RVA: 0x000648D7 File Offset: 0x00062AD7
		float ISkyDomeController.SunAltitude
		{
			get
			{
				return this.SunAltitude;
			}
		}

		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x06003573 RID: 13683 RVA: 0x000648DF File Offset: 0x00062ADF
		bool ISkyDomeController.IsIndoors
		{
			get
			{
				return this.IsIndoors;
			}
		}

		// Token: 0x06003574 RID: 13684 RVA: 0x000648E7 File Offset: 0x00062AE7
		void ISkyDomeController.ResetSkybox()
		{
			this.ResetSkybox();
		}

		// Token: 0x0400336D RID: 13165
		public const float kZenith = 90f;

		// Token: 0x0400336E RID: 13166
		public const float kZenithInverted = 0.011111111f;

		// Token: 0x0400336F RID: 13167
		private bool m_isDay;

		// Token: 0x04003372 RID: 13170
		[SerializeField]
		protected bool m_initializeTime;

		// Token: 0x04003373 RID: 13171
		[SerializeField]
		protected bool m_progressTime;

		// Token: 0x04003374 RID: 13172
		[SerializeField]
		protected BaseSkyController.InternalDateTime m_internalDateTime;

		// Token: 0x020006E6 RID: 1766
		[Serializable]
		protected class InternalDateTime
		{
			// Token: 0x17000BD5 RID: 3029
			// (get) Token: 0x06003576 RID: 13686 RVA: 0x000648EF File Offset: 0x00062AEF
			// (set) Token: 0x06003577 RID: 13687 RVA: 0x000648F7 File Offset: 0x00062AF7
			public DateTime DateTime
			{
				get
				{
					return this.m_dateTime;
				}
				set
				{
					this.m_dateTime = value;
				}
			}

			// Token: 0x06003578 RID: 13688 RVA: 0x00166DB0 File Offset: 0x00164FB0
			public DateTime GetTimeBasedOnValues()
			{
				TimeSpan value = new TimeSpan(this.m_day - 1, this.m_hour, this.m_minute, this.m_second);
				DateTime dateTime = new DateTime(this.m_year, 1, 1);
				return dateTime.Add(value);
			}

			// Token: 0x06003579 RID: 13689 RVA: 0x00166DF8 File Offset: 0x00164FF8
			private string GetDateTimeString()
			{
				return this.GetTimeBasedOnValues().ToString();
			}

			// Token: 0x04003378 RID: 13176
			private const string kTime = "Time";

			// Token: 0x04003379 RID: 13177
			private const string kDate = "Date";

			// Token: 0x0400337A RID: 13178
			private DateTime m_dateTime;

			// Token: 0x0400337B RID: 13179
			[SerializeField]
			private DummyClass m_dummy;

			// Token: 0x0400337C RID: 13180
			[Range(0f, 23f)]
			[SerializeField]
			private int m_hour;

			// Token: 0x0400337D RID: 13181
			[Range(0f, 59f)]
			[SerializeField]
			private int m_minute;

			// Token: 0x0400337E RID: 13182
			[Range(0f, 59f)]
			[SerializeField]
			private int m_second;

			// Token: 0x0400337F RID: 13183
			[Range(1f, 365f)]
			[SerializeField]
			private int m_day = 1;

			// Token: 0x04003380 RID: 13184
			[SerializeField]
			private int m_year = 2000;
		}
	}
}
