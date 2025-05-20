using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000707 RID: 1799
	public static class SkyDomeUtilities
	{
		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x0600362F RID: 13871 RVA: 0x000651D4 File Offset: 0x000633D4
		// (set) Token: 0x06003630 RID: 13872 RVA: 0x000651DB File Offset: 0x000633DB
		public static int Latitude { get; private set; }

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x06003631 RID: 13873 RVA: 0x000651E3 File Offset: 0x000633E3
		// (set) Token: 0x06003632 RID: 13874 RVA: 0x000651EA File Offset: 0x000633EA
		public static int Longitude { get; private set; }

		// Token: 0x06003633 RID: 13875 RVA: 0x001689F4 File Offset: 0x00166BF4
		public static void CalculateSunPositionEnv(float internalHour, float d, float ecl, float latitude, float longitude, out double outAzimuth, out double outAltitude)
		{
			float num = 282.9404f + 4.70935E-05f * d;
			float num2 = 0.016709f - 1.151E-09f * d;
			float num3 = 356.047f + 0.98560023f * d;
			float num4 = num3 + num2 * 57.29578f * Mathf.Sin(0.017453292f * num3) * (1f + num2 * Mathf.Cos(0.017453292f * num3));
			float num5 = Mathf.Cos(0.017453292f * num4) - num2;
			float num6 = Mathf.Sin(0.017453292f * num4) * Mathf.Sqrt(1f - num2 * num2);
			float num7 = 57.29578f * Mathf.Atan2(num6, num5);
			float num8 = Mathf.Sqrt(num5 * num5 + num6 * num6);
			float num9 = num7 + num;
			float num10 = num8 * Mathf.Cos(0.017453292f * num9);
			float num11 = num8 * Mathf.Sin(0.017453292f * num9);
			float num12 = num10;
			float num13 = num11 * Mathf.Cos(0.017453292f * ecl);
			float f = Mathf.Atan2(num11 * Mathf.Sin(0.017453292f * ecl), Mathf.Sqrt(num12 * num12 + num13 * num13));
			float num14 = Mathf.Sin(f);
			float num15 = Mathf.Cos(f);
			SkyDomeUtilities.LST = num9 + 180f + internalHour * 15f + longitude;
			float num16 = SkyDomeUtilities.LST - 57.29578f * Mathf.Atan2(num13, num12);
			float f2 = 0.017453292f * num16;
			float num17 = Mathf.Sin(f2);
			float num18 = Mathf.Cos(f2) * num15;
			float num19 = num17 * num15;
			float num20 = num14;
			float f3 = 0.017453292f * latitude;
			float num21 = Mathf.Sin(f3);
			float num22 = Mathf.Cos(f3);
			float num23 = num18 * num21 - num20 * num22;
			float num24 = num19;
			float y = num18 * num22 + num20 * num21;
			float num25 = Mathf.Atan2(num24, num23) + 3.1415927f;
			float num26 = Mathf.Atan2(y, Mathf.Sqrt(num23 * num23 + num24 * num24));
			outAltitude = (double)num26 * 57.29577951308232;
			outAzimuth = (double)num25 * 57.29577951308232;
		}

		// Token: 0x06003634 RID: 13876 RVA: 0x00168BD8 File Offset: 0x00166DD8
		public static void CalculateMoonPositionEnv(float d, float ecl, float latitude, out double outAzimuth, out double outAltitude)
		{
			float num = 125.1228f - 0.05295381f * d;
			float num2 = 5.1454f;
			float num3 = 318.0634f + 0.16435732f * d;
			float num4 = 60.2666f;
			float num5 = 0.0549f;
			float num6 = 115.3654f + 13.064993f * d;
			float num7 = 0.017453292f * num6;
			float f = num7 + num5 * Mathf.Sin(num7) * (1f + num5 * Mathf.Cos(num7));
			float num8 = num4 * (Mathf.Cos(f) - num5);
			float num9 = num4 * (Mathf.Sqrt(1f - num5 * num5) * Mathf.Sin(f));
			float num10 = 57.29578f * Mathf.Atan2(num9, num8);
			float num11 = Mathf.Sqrt(num8 * num8 + num9 * num9);
			float f2 = 0.017453292f * num;
			float num12 = Mathf.Sin(f2);
			float num13 = Mathf.Cos(f2);
			float f3 = 0.017453292f * (num10 + num3);
			float num14 = Mathf.Sin(f3);
			float num15 = Mathf.Cos(f3);
			float f4 = 0.017453292f * num2;
			float num16 = Mathf.Cos(f4);
			float num17 = num11 * (num13 * num15 - num12 * num14 * num16);
			float num18 = num11 * (num12 * num15 + num13 * num14 * num16);
			float num19 = num11 * (num14 * Mathf.Sin(f4));
			float num20 = Mathf.Cos(0.017453292f * ecl);
			float num21 = Mathf.Sin(0.017453292f * ecl);
			float num22 = num17;
			float num23 = num18 * num20 - num19 * num21;
			float y = num18 * num21 + num19 * num20;
			float num24 = Mathf.Atan2(num23, num22);
			float f5 = Mathf.Atan2(y, Mathf.Sqrt(num22 * num22 + num23 * num23));
			float f6 = 0.017453292f * SkyDomeUtilities.LST - num24;
			float num25 = Mathf.Cos(f6) * Mathf.Cos(f5);
			float num26 = Mathf.Sin(f6) * Mathf.Cos(f5);
			float num27 = Mathf.Sin(f5);
			float f7 = 0.017453292f * latitude;
			float num28 = Mathf.Sin(f7);
			float num29 = Mathf.Cos(f7);
			float num30 = num25 * num28 - num27 * num29;
			float num31 = num26;
			float y2 = num25 * num29 + num27 * num28;
			float num32 = Mathf.Atan2(num31, num30) + 3.1415927f;
			float num33 = Mathf.Atan2(y2, Mathf.Sqrt(num30 * num30 + num31 * num31));
			outAzimuth = (double)num32 * 57.29577951308232;
			outAltitude = (double)num33 * 57.29577951308232;
		}

		// Token: 0x06003635 RID: 13877 RVA: 0x00168E0C File Offset: 0x0016700C
		public static double DaysSinceEpoch(DateTime dateTime)
		{
			return dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalDays;
		}

		// Token: 0x06003636 RID: 13878 RVA: 0x00168E38 File Offset: 0x00167038
		public static void SetLatLong()
		{
			int latitude = 33;
			int longitude = 0;
			if (ZoneSettings.Instance && ZoneSettings.Instance.Profile)
			{
				latitude = ZoneSettings.Instance.Profile.Latitude;
				longitude = ZoneSettings.Instance.Profile.Longitude;
			}
			else if (ZoneSettings.SettingsProfile)
			{
				latitude = ZoneSettings.SettingsProfile.Latitude;
				longitude = ZoneSettings.SettingsProfile.Longitude;
			}
			SkyDomeUtilities.Latitude = latitude;
			SkyDomeUtilities.Longitude = longitude;
		}

		// Token: 0x06003637 RID: 13879 RVA: 0x00168EB8 File Offset: 0x001670B8
		public static void GetSkyValues(DateTime dateTime, out float internalHour, out float d, out float ecl)
		{
			internalHour = (float)dateTime.Hour + (float)dateTime.Minute * 0.0166667f + (float)dateTime.Second * 0.000277778f;
			d = (float)(367 * dateTime.Year - 7 * (dateTime.Year + (dateTime.Month / 12 + 9) / 12) / 4 + 275 * dateTime.Month / 9 + dateTime.Day - 730530);
			d += internalHour / 24f;
			ecl = 23.4393f - 3.563E-07f * d;
		}

		// Token: 0x06003638 RID: 13880 RVA: 0x00168F58 File Offset: 0x00167158
		private static MinMaxFloatRange GetMinMaxSunAltitude(DateTime dateTime)
		{
			if (dateTime.Year != SkyDomeUtilities.m_cachedYear)
			{
				SkyDomeUtilities.SetLatLong();
				SkyDomeUtilities.m_cachedYear = dateTime.Year;
				DateTime dateTime2 = new DateTime(dateTime.Year, 1, 1);
				for (int i = 0; i < 367; i++)
				{
					DateTime dateTime3 = dateTime2.AddDays((double)i);
					float internalHour;
					float d;
					float ecl;
					SkyDomeUtilities.GetSkyValues(dateTime3, out internalHour, out d, out ecl);
					double num;
					double num2;
					SkyDomeUtilities.CalculateSunPositionEnv(internalHour, d, ecl, (float)SkyDomeUtilities.Latitude, (float)SkyDomeUtilities.Longitude, out num, out num2);
					SkyDomeUtilities.GetSkyValues(dateTime3.AddHours(12.0), out internalHour, out d, out ecl);
					double num3;
					SkyDomeUtilities.CalculateSunPositionEnv(internalHour, d, ecl, (float)SkyDomeUtilities.Latitude, (float)SkyDomeUtilities.Longitude, out num, out num3);
					SkyDomeUtilities.MinMaxAltitudes[i] = new MinMaxFloatRange((float)num2, (float)num3);
				}
			}
			return SkyDomeUtilities.MinMaxAltitudes[dateTime.DayOfYear];
		}

		// Token: 0x06003639 RID: 13881 RVA: 0x00169038 File Offset: 0x00167238
		public static float GetDayNightCycleFraction()
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				return 0.5f;
			}
			int frameCount = Time.frameCount;
			if (frameCount == SkyDomeUtilities.m_lastDayNightCycleFractionFrame)
			{
				return SkyDomeUtilities.m_dayNightCycleFraction;
			}
			SkyDomeUtilities.m_lastDayNightCycleFractionFrame = frameCount;
			float sunAltitude = SkyDomeManager.SkyDomeController.SunAltitude;
			DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
			MinMaxFloatRange minMaxSunAltitude = SkyDomeUtilities.GetMinMaxSunAltitude(correctedGameDateTime);
			float dayNightCycleFraction;
			if (sunAltitude < 0f)
			{
				float t = Mathf.Clamp01(sunAltitude / minMaxSunAltitude.Min);
				if (correctedGameDateTime.Hour > 12)
				{
					dayNightCycleFraction = Mathf.Lerp(0.75f, 1f, t);
				}
				else
				{
					dayNightCycleFraction = Mathf.Lerp(0.25f, 0f, t);
				}
			}
			else
			{
				float t2 = Mathf.Clamp01(sunAltitude / minMaxSunAltitude.Max);
				if (correctedGameDateTime.Hour >= 12)
				{
					dayNightCycleFraction = Mathf.Lerp(0.75f, 0.5f, t2);
				}
				else
				{
					dayNightCycleFraction = Mathf.Lerp(0.25f, 0.5f, t2);
				}
			}
			SkyDomeUtilities.m_dayNightCycleFraction = dayNightCycleFraction;
			return SkyDomeUtilities.m_dayNightCycleFraction;
		}

		// Token: 0x0600363A RID: 13882 RVA: 0x00169128 File Offset: 0x00167328
		public static int GetTimeZoneHourDelta()
		{
			if (SkyDomeUtilities.Longitude != 0)
			{
				float num = (float)SkyDomeUtilities.Longitude / 15f;
				int num2 = Mathf.FloorToInt(Mathf.Abs(num));
				if (num2 > 0 && num < 0f)
				{
					num2 *= -1;
				}
				return num2;
			}
			return 0;
		}

		// Token: 0x0600363B RID: 13883 RVA: 0x00169168 File Offset: 0x00167368
		public static SkyDomeUtilities.SunriseSunset[] GetNextSunriseSunset()
		{
			DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
			if (SkyDomeUtilities.m_upcomingSunriseSunsets == null)
			{
				SkyDomeUtilities.m_upcomingSunriseSunsets = new SkyDomeUtilities.SunriseSunset[2];
				SkyDomeUtilities.FillSunriseSunsetCache(correctedGameDateTime);
			}
			else if (correctedGameDateTime > SkyDomeUtilities.m_upcomingSunriseSunsets[0].Time || SkyDomeUtilities.m_cachedDay != correctedGameDateTime.DayOfYear || SkyDomeUtilities.m_cachedLongitude != SkyDomeUtilities.Longitude)
			{
				SkyDomeUtilities.FillSunriseSunsetCache(correctedGameDateTime);
			}
			return SkyDomeUtilities.m_upcomingSunriseSunsets;
		}

		// Token: 0x0600363C RID: 13884 RVA: 0x001691D4 File Offset: 0x001673D4
		private static void FillSunriseSunsetCache(DateTime currentDateTime)
		{
			SkyDomeUtilities.SetLatLong();
			SkyDomeUtilities.m_cachedDay = currentDateTime.DayOfYear;
			SkyDomeUtilities.m_cachedLongitude = SkyDomeUtilities.Longitude;
			float num = float.MaxValue;
			DateTime dateTime = DateTime.MinValue;
			float num2 = float.MinValue;
			DateTime dateTime2 = DateTime.MinValue;
			for (int i = 0; i < 1440; i++)
			{
				DateTime dateTime3 = currentDateTime.AddMinutes((double)i);
				float internalHour;
				float d;
				float ecl;
				SkyDomeUtilities.GetSkyValues(dateTime3, out internalHour, out d, out ecl);
				double num3;
				double num4;
				SkyDomeUtilities.CalculateSunPositionEnv(internalHour, d, ecl, (float)SkyDomeUtilities.Latitude, (float)SkyDomeUtilities.Longitude, out num3, out num4);
				if (dateTime3.Hour <= 12)
				{
					if (num4 > 0.0 && num4 < (double)num)
					{
						num = (float)num4;
						dateTime = dateTime3;
					}
				}
				else if (num4 < 0.0 && num4 > (double)num2)
				{
					num2 = (float)num4;
					dateTime2 = dateTime3;
				}
			}
			if (dateTime < dateTime2)
			{
				SkyDomeUtilities.m_upcomingSunriseSunsets[0] = new SkyDomeUtilities.SunriseSunset(true, dateTime);
				SkyDomeUtilities.m_upcomingSunriseSunsets[1] = new SkyDomeUtilities.SunriseSunset(false, dateTime2);
				return;
			}
			SkyDomeUtilities.m_upcomingSunriseSunsets[0] = new SkyDomeUtilities.SunriseSunset(false, dateTime2);
			SkyDomeUtilities.m_upcomingSunriseSunsets[1] = new SkyDomeUtilities.SunriseSunset(true, dateTime);
		}

		// Token: 0x0400341A RID: 13338
		public const float kLongitudeSegments = 24f;

		// Token: 0x0400341B RID: 13339
		public const float kLongitudeHourStep = 15f;

		// Token: 0x0400341C RID: 13340
		public const int kDefaultLatitude = 33;

		// Token: 0x0400341D RID: 13341
		public const int kDefaultLongitude = 0;

		// Token: 0x0400341E RID: 13342
		public const int kDefaultParentPlanetAltitude = 23;

		// Token: 0x0400341F RID: 13343
		public const string kCurveTooltip = "0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight";

		// Token: 0x04003420 RID: 13344
		public const int kInitialLastFrame = -2147483648;

		// Token: 0x04003423 RID: 13347
		private const float kDefaultDayNightCycleFraction = 0.5f;

		// Token: 0x04003424 RID: 13348
		private const int kDaysInYear = 367;

		// Token: 0x04003425 RID: 13349
		private static readonly MinMaxFloatRange[] MinMaxAltitudes = new MinMaxFloatRange[367];

		// Token: 0x04003426 RID: 13350
		private static int m_cachedYear = -1;

		// Token: 0x04003427 RID: 13351
		private static int m_lastDayNightCycleFractionFrame = int.MinValue;

		// Token: 0x04003428 RID: 13352
		private static float m_dayNightCycleFraction = 0.5f;

		// Token: 0x04003429 RID: 13353
		private static int m_cachedDay = -1;

		// Token: 0x0400342A RID: 13354
		private static int m_cachedLongitude = 0;

		// Token: 0x0400342B RID: 13355
		private static SkyDomeUtilities.SunriseSunset[] m_upcomingSunriseSunsets = null;

		// Token: 0x0400342C RID: 13356
		private const double Deg2Rad = 0.017453292519943295;

		// Token: 0x0400342D RID: 13357
		private const double Rad2Deg = 57.29577951308232;

		// Token: 0x0400342E RID: 13358
		private static float LST;

		// Token: 0x02000708 RID: 1800
		public struct SunriseSunset
		{
			// Token: 0x0600363E RID: 13886 RVA: 0x0006522F File Offset: 0x0006342F
			public SunriseSunset(bool isSunrise, DateTime time)
			{
				this.Time = time;
				this.IsSunrise = isSunrise;
			}

			// Token: 0x0400342F RID: 13359
			public readonly DateTime Time;

			// Token: 0x04003430 RID: 13360
			public readonly bool IsSunrise;
		}
	}
}
