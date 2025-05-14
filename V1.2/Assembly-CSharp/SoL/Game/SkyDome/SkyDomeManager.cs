using System;
using System.Collections.Generic;
using System.Globalization;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.VegetationSystem;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using TheVisualEngine;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000706 RID: 1798
	public static class SkyDomeManager
	{
		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06003601 RID: 13825 RVA: 0x00167E74 File Offset: 0x00166074
		// (remove) Token: 0x06003602 RID: 13826 RVA: 0x00167EA8 File Offset: 0x001660A8
		public static event Action SkydomeControllerChanged;

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06003603 RID: 13827 RVA: 0x00167EDC File Offset: 0x001660DC
		// (remove) Token: 0x06003604 RID: 13828 RVA: 0x00167F10 File Offset: 0x00166110
		public static event Action VSPManagerAssigned;

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06003605 RID: 13829 RVA: 0x00065060 File Offset: 0x00063260
		// (set) Token: 0x06003606 RID: 13830 RVA: 0x00065067 File Offset: 0x00063267
		public static bool InEmberHighlightZone { get; set; } = false;

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x06003607 RID: 13831 RVA: 0x0006506F File Offset: 0x0006326F
		// (set) Token: 0x06003608 RID: 13832 RVA: 0x00065076 File Offset: 0x00063276
		public static IExposureController ExposureController { get; set; }

		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06003609 RID: 13833 RVA: 0x0006507E File Offset: 0x0006327E
		// (set) Token: 0x0600360A RID: 13834 RVA: 0x00167F44 File Offset: 0x00166144
		public static GameTimeReplicator GameTimeReplicator
		{
			get
			{
				return SkyDomeManager.m_gameTimeReplicator;
			}
			set
			{
				if (SkyDomeManager.m_gameTimeReplicator == value)
				{
					return;
				}
				if (SkyDomeManager.m_gameTimeReplicator != null)
				{
					SkyDomeManager.m_gameTimeReplicator.TimeOverride.Changed -= SkyDomeManager.TimeOverrideOnChanged;
				}
				SkyDomeManager.m_gameTimeReplicator = value;
				if (SkyDomeManager.m_gameTimeReplicator != null)
				{
					SkyDomeManager.m_gameTimeReplicator.TimeOverride.Changed += SkyDomeManager.TimeOverrideOnChanged;
				}
				SkyDomeManager.RefreshGameTime();
			}
		}

		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x0600360B RID: 13835 RVA: 0x00065085 File Offset: 0x00063285
		// (set) Token: 0x0600360C RID: 13836 RVA: 0x00167FBC File Offset: 0x001661BC
		public static ISkyDomeController SkyDomeController
		{
			get
			{
				return SkyDomeManager.m_skyDomeController;
			}
			set
			{
				if (SkyDomeManager.m_skyDomeController == value)
				{
					return;
				}
				SkyDomeManager.SubscribeToZoneChange();
				if (SkyDomeManager.m_skyDomeController != null)
				{
					SkyDomeManager.m_skyDomeController.DayNightChanged -= SkyDomeManager.OnDayNightChanged;
				}
				SkyDomeManager.m_skyDomeController = value;
				if (SkyDomeManager.m_skyDomeController != null)
				{
					SkyDomeManager.m_skyDomeController.DayNightChanged += SkyDomeManager.OnDayNightChanged;
				}
				SkyDomeManager.InitInternal();
				Action skydomeControllerChanged = SkyDomeManager.SkydomeControllerChanged;
				if (skydomeControllerChanged == null)
				{
					return;
				}
				skydomeControllerChanged();
			}
		}

		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x0600360D RID: 13837 RVA: 0x0006508C File Offset: 0x0006328C
		// (set) Token: 0x0600360E RID: 13838 RVA: 0x00065093 File Offset: 0x00063293
		public static VegetationStudioManager VSPManager
		{
			get
			{
				return SkyDomeManager.m_vspManager;
			}
			set
			{
				if (SkyDomeManager.m_vspManager == value)
				{
					return;
				}
				SkyDomeManager.SubscribeToZoneChange();
				SkyDomeManager.m_vspManager = value;
				SkyDomeManager.InitVegetationStudioPro();
				Action vspmanagerAssigned = SkyDomeManager.VSPManagerAssigned;
				if (vspmanagerAssigned == null)
				{
					return;
				}
				vspmanagerAssigned();
			}
		}

		// Token: 0x0600360F RID: 13839 RVA: 0x0016802C File Offset: 0x0016622C
		private static void InitInternal()
		{
			if (SkyDomeManager.m_skyDomeInitialized)
			{
				return;
			}
			if (SkyDomeManager.m_pending != null && SkyDomeManager.m_pending.Count > 0)
			{
				foreach (IDayNightToggle fx in SkyDomeManager.m_pending)
				{
					SkyDomeManager.RegisterFXInternal(fx);
				}
				SkyDomeManager.m_pending.Clear();
			}
			SkyDomeManager.m_skyDomeInitialized = true;
			SkyDomeManager.InitVegetationStudioPro();
			SkyDomeManager.RefreshActiveLight();
			if (SkyDomeManager.SkyDomeController != null && SkyDomeManager.SkyDomeController is EnviroController)
			{
				SkyDomeManager.RefreshGameTime();
			}
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x001680CC File Offset: 0x001662CC
		private static void InitVegetationStudioPro()
		{
			if (SkyDomeManager.m_vegetationStudioInitialized || SkyDomeManager.SkyDomeController == null || SkyDomeManager.VSPManager == null)
			{
				return;
			}
			WindZone wind = SkyDomeManager.SkyDomeController.Wind;
			if (wind)
			{
				List<VegetationSystemPro> vegetationSystemList = SkyDomeManager.VSPManager.VegetationSystemList;
				for (int i = 0; i < vegetationSystemList.Count; i++)
				{
					if (vegetationSystemList[i].SelectedWindZone != null && vegetationSystemList[i].SelectedWindZone != wind)
					{
						vegetationSystemList[i].SelectedWindZone.gameObject.SetActive(false);
					}
					vegetationSystemList[i].SelectedWindZone = wind;
					vegetationSystemList[i].UpdateWindSettings();
				}
			}
			SkyDomeManager.m_vegetationStudioInitialized = true;
		}

		// Token: 0x06003611 RID: 13841 RVA: 0x000650C2 File Offset: 0x000632C2
		public static void RegisterFX(IDayNightToggle fx)
		{
			if (SkyDomeManager.SkyDomeController != null && SkyDomeManager.m_skyDomeInitialized)
			{
				SkyDomeManager.RegisterFXInternal(fx);
				return;
			}
			SkyDomeManager.m_pending.Add(fx);
		}

		// Token: 0x06003612 RID: 13842 RVA: 0x000650E5 File Offset: 0x000632E5
		public static void UnregisterFX(IDayNightToggle fx)
		{
			if (SkyDomeManager.SkyDomeController != null && SkyDomeManager.m_skyDomeInitialized)
			{
				SkyDomeManager.UnregisterFXInternal(fx);
				return;
			}
			HashSet<IDayNightToggle> pending = SkyDomeManager.m_pending;
			if (pending == null)
			{
				return;
			}
			pending.Remove(fx);
		}

		// Token: 0x06003613 RID: 13843 RVA: 0x0006510D File Offset: 0x0006330D
		private static void RegisterFXInternal(IDayNightToggle fx)
		{
			if (fx.DayNightEnableCondition != DayNightEnableCondition.Always && !SkyDomeManager.m_dayNightToggles.Contains(fx))
			{
				SkyDomeManager.m_dayNightToggles.Add(fx);
				SkyDomeManager.ToggleFx(fx, SkyDomeManager.IsDay());
			}
		}

		// Token: 0x06003614 RID: 13844 RVA: 0x0006513A File Offset: 0x0006333A
		private static void UnregisterFXInternal(IDayNightToggle fx)
		{
			SkyDomeManager.m_dayNightToggles.Remove(fx);
		}

		// Token: 0x06003615 RID: 13845 RVA: 0x00168184 File Offset: 0x00166384
		private static void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			SkyDomeManager.GameTimeReplicator = null;
			SkyDomeManager.SkyDomeController = null;
			SkyDomeManager.m_skyDomeInitialized = false;
			HashSet<IDayNightToggle> pending = SkyDomeManager.m_pending;
			if (pending != null)
			{
				pending.Clear();
			}
			List<IDayNightToggle> dayNightToggles = SkyDomeManager.m_dayNightToggles;
			if (dayNightToggles != null)
			{
				dayNightToggles.Clear();
			}
			SkyDomeManager.InEmberHighlightZone = false;
			SkyDomeManager.VSPManager = null;
			SkyDomeManager.m_vegetationStudioInitialized = false;
		}

		// Token: 0x06003616 RID: 13846 RVA: 0x00065148 File Offset: 0x00063348
		private static void TimeOverrideOnChanged(float? obj)
		{
			SkyDomeManager.RefreshGameTime();
		}

		// Token: 0x06003617 RID: 13847 RVA: 0x001681D8 File Offset: 0x001663D8
		public static void RefreshGameTimeIfNeeded()
		{
			if (SkyDomeManager.SkyDomeController != null && SkyDomeManager.SkyDomeController.ProgressTime)
			{
				DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
				DateTime time = SkyDomeManager.SkyDomeController.GetTime();
				if (Mathf.Abs((float)(time - correctedGameDateTime).TotalMinutes) > 10f)
				{
					SkyDomeManager.SkyDomeController.SetTime(correctedGameDateTime);
					Debug.Log("Correcting GameTime!  was: " + time.ToString() + ", should be: " + correctedGameDateTime.ToString());
				}
			}
		}

		// Token: 0x06003618 RID: 13848 RVA: 0x00168254 File Offset: 0x00166454
		public static void RefreshGameTime()
		{
			DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
			ISkyDomeController skyDomeController = SkyDomeManager.SkyDomeController;
			if (skyDomeController == null)
			{
				return;
			}
			skyDomeController.SetTime(correctedGameDateTime);
		}

		// Token: 0x06003619 RID: 13849 RVA: 0x00168278 File Offset: 0x00166478
		private static void OnDayNightChanged()
		{
			SkyDomeManager.RefreshIsDay();
			for (int i = 0; i < SkyDomeManager.m_dayNightToggles.Count; i++)
			{
				SkyDomeManager.ToggleFx(SkyDomeManager.m_dayNightToggles[i], SkyDomeManager.m_isDay);
			}
		}

		// Token: 0x0600361A RID: 13850 RVA: 0x001682B4 File Offset: 0x001664B4
		public static DateTime GetCorrectedGameDateTime()
		{
			int frameCount = Time.frameCount;
			if (frameCount == SkyDomeManager.m_lastGameTimeFrame)
			{
				return SkyDomeManager.m_gameTime;
			}
			SkyDomeManager.m_lastGameTimeFrame = frameCount;
			if (SkyDomeManager.SkyDomeController != null && !SkyDomeManager.SkyDomeController.ProgressTime)
			{
				SkyDomeManager.m_gameTime = SkyDomeManager.SkyDomeController.GetTime();
				return SkyDomeManager.m_gameTime;
			}
			DateTime gameTime = GameDateTime.UtcNow.DateTime;
			if (SkyDomeManager.GameTimeReplicator && SkyDomeManager.GameTimeReplicator.TimeOfLastTimeOverrideChange.Value != null && SkyDomeManager.GameTimeReplicator.TimeOverride.Value != null)
			{
				DateTime d = new DateTime(SkyDomeManager.GameTimeReplicator.TimeOfLastTimeOverrideChange.Value.Value);
				GameDateTime gameDateTime = new GameDateTime(d.Ticks);
				float num = SkyDomeManager.GameTimeReplicator.TimeOverride.Value.Value - (float)gameDateTime.Hour;
				DateTime dateTime = gameDateTime.DateTime.AddHours((double)num);
				double value = (GameTimeReplicator.GetServerCorrectedDateTimeUtc() - d).TotalSeconds / 60.0 / 113.0;
				gameTime = dateTime.AddDays(value);
			}
			SkyDomeManager.m_gameTime = gameTime;
			return SkyDomeManager.m_gameTime;
		}

		// Token: 0x0600361B RID: 13851 RVA: 0x0006514F File Offset: 0x0006334F
		public static string GetSeason()
		{
			return SkyDomeManager.GetSeason(SkyDomeManager.GetCorrectedGameDateTime());
		}

		// Token: 0x0600361C RID: 13852 RVA: 0x00168400 File Offset: 0x00166600
		public static string GetSeason(DateTime dateTime)
		{
			SkyDomeManager.SpringEquinox = SkyDomeManager.SpringEquinox.ChangeYear(dateTime.Year);
			SkyDomeManager.SummerSolstice = SkyDomeManager.SummerSolstice.ChangeYear(dateTime.Year);
			SkyDomeManager.FallEquinox = SkyDomeManager.FallEquinox.ChangeYear(dateTime.Year);
			SkyDomeManager.WinterSolstice = SkyDomeManager.WinterSolstice.ChangeYear(dateTime.Year);
			if (dateTime < SkyDomeManager.SpringEquinox)
			{
				return "<sprite=\"SeasonIcons\" name=\"Winter\" tint=1> Winter";
			}
			if (dateTime < SkyDomeManager.SummerSolstice)
			{
				return "<sprite=\"SeasonIcons\" name=\"Spring\" tint=1> Spring";
			}
			if (dateTime < SkyDomeManager.FallEquinox)
			{
				return "<sprite=\"SeasonIcons\" name=\"Summer\" tint=1> Summer";
			}
			if (dateTime < SkyDomeManager.WinterSolstice)
			{
				return "<sprite=\"SeasonIcons\" name=\"Fall\" tint=1> Fall";
			}
			return "<sprite=\"SeasonIcons\" name=\"Winter\" tint=1> Winter";
		}

		// Token: 0x0600361D RID: 13853 RVA: 0x001684B8 File Offset: 0x001666B8
		private static DateTime GetTimeZoneAdjustedGameTime(out string timeZoneAdjustmentLabel)
		{
			timeZoneAdjustmentLabel = string.Empty;
			DateTime result = SkyDomeManager.GetCorrectedGameDateTime();
			if (SkyDomeUtilities.Longitude != 0)
			{
				float num = (float)SkyDomeUtilities.Longitude / 15f;
				int num2 = Mathf.FloorToInt(Mathf.Abs(num));
				if (num2 > 0)
				{
					timeZoneAdjustmentLabel = ZString.Format<string, int>("{0}{1}", (num < 0f) ? "-" : "+", num2);
					if (num < 0f)
					{
						num2 *= -1;
					}
					result = result.AddHours((double)num2);
				}
			}
			return result;
		}

		// Token: 0x0600361E RID: 13854 RVA: 0x0006515B File Offset: 0x0006335B
		public static string GetSunMoonIconInline()
		{
			if (!SkyDomeManager.IsDay())
			{
				return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			}
			return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
		}

		// Token: 0x0600361F RID: 13855 RVA: 0x0006516F File Offset: 0x0006336F
		public static string GetSunMoonUnicode()
		{
			if (!SkyDomeManager.IsDay())
			{
				return "";
			}
			return "";
		}

		// Token: 0x06003620 RID: 13856 RVA: 0x00168530 File Offset: 0x00166730
		public static string GetGameTimeChatCommand()
		{
			string gameDisplaySeason = SkyDomeManager.GetGameDisplaySeason(true);
			string gameDisplayDate = SkyDomeManager.GetGameDisplayDate(true);
			string gameDisplayTime = SkyDomeManager.GetGameDisplayTime(true);
			return ZString.Format<string, string, string, string>("{0} {1} on {2} ({3})", SkyDomeManager.GetSunMoonIconInline(), gameDisplayTime, gameDisplayDate, gameDisplaySeason);
		}

		// Token: 0x06003621 RID: 13857 RVA: 0x00168564 File Offset: 0x00166764
		public static string GetGameDisplayTime(bool adjustForTimeZone = true)
		{
			string empty = string.Empty;
			string arg = (adjustForTimeZone ? SkyDomeManager.GetTimeZoneAdjustedGameTime(out empty) : SkyDomeManager.GetCorrectedGameDateTime()).ToString("h:mm tt", CultureInfo.InvariantCulture);
			return ZString.Format<string, string, string>("{0} ({1}{2})", arg, "<link=\"text:Newhaven Standard Time\">NST</link>", empty);
		}

		// Token: 0x06003622 RID: 13858 RVA: 0x001685AC File Offset: 0x001667AC
		public static string GetGameDisplayDate(bool adjustForTimeZone = true)
		{
			string text;
			return (adjustForTimeZone ? SkyDomeManager.GetTimeZoneAdjustedGameTime(out text) : SkyDomeManager.GetCorrectedGameDateTime()).ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x06003623 RID: 13859 RVA: 0x001685DC File Offset: 0x001667DC
		public static string GetGameDisplaySeason(bool adjustForTimeZone = true)
		{
			string text;
			return SkyDomeManager.GetSeason(adjustForTimeZone ? SkyDomeManager.GetTimeZoneAdjustedGameTime(out text) : SkyDomeManager.GetCorrectedGameDateTime());
		}

		// Token: 0x06003624 RID: 13860 RVA: 0x00065183 File Offset: 0x00063383
		public static string GetLocalDisplayTime()
		{
			return SkyDomeManager.GetDisplayTimeInternal(DateTime.Now);
		}

		// Token: 0x06003625 RID: 13861 RVA: 0x0006518F File Offset: 0x0006338F
		public static string GetUtcDisplayTime()
		{
			return SkyDomeManager.GetDisplayTimeInternal(DateTime.UtcNow);
		}

		// Token: 0x06003626 RID: 13862 RVA: 0x00168600 File Offset: 0x00166800
		public static string GetServerDisplayTime()
		{
			string result;
			try
			{
				result = SkyDomeManager.GetDisplayTimeInternal(TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06003627 RID: 13863 RVA: 0x00168644 File Offset: 0x00166844
		private static string GetDisplayTimeInternal(DateTime inputTime)
		{
			string arg = inputTime.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
			string arg2 = inputTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
			return ZString.Format<string, string>("{0} on {1}", arg2, arg);
		}

		// Token: 0x06003628 RID: 13864 RVA: 0x00168684 File Offset: 0x00166884
		public static void RefreshActiveLight()
		{
			if (SkyDomeManager.SkyDomeController == null || SkyDomeManager.SkyDomeController.Light == null)
			{
				return;
			}
			VegetationStudioManager.SetSunDirectionalLight(SkyDomeManager.SkyDomeController.Light);
			if (TVEManager.Instance != null)
			{
				TVEManager.Instance.mainLight = SkyDomeManager.SkyDomeController.Light;
			}
		}

		// Token: 0x06003629 RID: 13865 RVA: 0x0006519B File Offset: 0x0006339B
		private static void SubscribeToZoneChange()
		{
			if (SkyDomeManager.m_subscribedToZoneChange)
			{
				return;
			}
			SceneCompositionManager.ZoneLoadStarted += SkyDomeManager.SceneCompositionManagerOnZoneLoadStarted;
			SkyDomeManager.m_subscribedToZoneChange = true;
		}

		// Token: 0x0600362A RID: 13866 RVA: 0x000651BC File Offset: 0x000633BC
		public static bool IsDay()
		{
			if (Time.frameCount > SkyDomeManager.m_lastIsDayFrame)
			{
				SkyDomeManager.RefreshIsDay();
			}
			return SkyDomeManager.m_isDay;
		}

		// Token: 0x0600362B RID: 13867 RVA: 0x001686DC File Offset: 0x001668DC
		private static void RefreshIsDay()
		{
			if (SkyDomeManager.SkyDomeController != null)
			{
				SkyDomeManager.m_isDay = SkyDomeManager.SkyDomeController.IsDay;
			}
			else
			{
				DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
				SkyDomeManager.m_isDay = (correctedGameDateTime.Hour >= 8 && correctedGameDateTime.Hour <= 20);
			}
			SkyDomeManager.m_lastIsDayFrame = Time.frameCount;
		}

		// Token: 0x0600362C RID: 13868 RVA: 0x00168734 File Offset: 0x00166934
		private static void ToggleFx(IDayNightToggle fx, bool isDay)
		{
			DayNightEnableCondition dayNightEnableCondition = fx.DayNightEnableCondition;
			if (dayNightEnableCondition == DayNightEnableCondition.Day)
			{
				fx.Toggle(isDay);
				return;
			}
			if (dayNightEnableCondition != DayNightEnableCondition.Night)
			{
				return;
			}
			fx.Toggle(!isDay);
		}

		// Token: 0x0600362D RID: 13869 RVA: 0x00168764 File Offset: 0x00166964
		internal static string GetFullTimeDisplay()
		{
			string result = string.Empty;
			string text = ZString.Format<int>("<indent={0}px>", GlobalSettings.Values.UI.TimeWindowIndent);
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendFormat<string, string, string>("Time:{0}{1} {2}</indent>\n", text, SkyDomeManager.GetSunMoonIconInline(), SkyDomeManager.GetGameDisplayTime(true));
				utf16ValueStringBuilder.AppendFormat<string, string>("Date:{0}{1}</indent>\n", text, SkyDomeManager.GetGameDisplayDate(true));
				utf16ValueStringBuilder.AppendFormat<string, string>("Season:{0}{1}</indent>\n", text, SkyDomeManager.GetGameDisplaySeason(true));
				utf16ValueStringBuilder.Append("<line-height=50%>\n</line-height>");
				DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
				SkyDomeUtilities.SunriseSunset[] nextSunriseSunset = SkyDomeUtilities.GetNextSunriseSunset();
				int timeZoneHourDelta = SkyDomeUtilities.GetTimeZoneHourDelta();
				for (int i = 0; i < nextSunriseSunset.Length; i++)
				{
					string arg = nextSunriseSunset[i].IsSunrise ? "Sunrise" : "Sunset";
					DateTime dateTime = nextSunriseSunset[i].Time.AddHours((double)timeZoneHourDelta);
					TimeSpan timeSpan = TimeSpan.FromSeconds((nextSunriseSunset[i].Time - correctedGameDateTime).TotalSeconds / 12.743362426757812);
					string arg2 = string.Empty;
					if (timeSpan.TotalMinutes < 1.0)
					{
						arg2 = ZString.Format<int>("{0}s", timeSpan.Seconds);
					}
					else if (timeSpan.TotalHours < 1.0)
					{
						arg2 = ZString.Format<int>("{0}m", timeSpan.Minutes);
					}
					else
					{
						arg2 = ZString.Format<int, int>("{0}hr {1}m", timeSpan.Hours, timeSpan.Minutes);
					}
					utf16ValueStringBuilder.AppendFormat<string, string, string, string>("{0}:{1}{2} ({3})</indent>", arg, text, dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture), arg2);
					if (i != nextSunriseSunset.Length - 1)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x040033FF RID: 13311
		private const string kWinter = "<sprite=\"SeasonIcons\" name=\"Winter\" tint=1> Winter";

		// Token: 0x04003400 RID: 13312
		private const string kSpring = "<sprite=\"SeasonIcons\" name=\"Spring\" tint=1> Spring";

		// Token: 0x04003401 RID: 13313
		private const string kSummer = "<sprite=\"SeasonIcons\" name=\"Summer\" tint=1> Summer";

		// Token: 0x04003402 RID: 13314
		private const string kFall = "<sprite=\"SeasonIcons\" name=\"Fall\" tint=1> Fall";

		// Token: 0x04003403 RID: 13315
		private static DateTime SpringEquinox = new DateTime(2022, 3, 20, 12, 0, 0);

		// Token: 0x04003404 RID: 13316
		private static DateTime SummerSolstice = new DateTime(2022, 6, 21, 12, 0, 0);

		// Token: 0x04003405 RID: 13317
		private static DateTime FallEquinox = new DateTime(2022, 9, 23, 12, 0, 0);

		// Token: 0x04003406 RID: 13318
		private static DateTime WinterSolstice = new DateTime(2022, 12, 21, 12, 0, 0);

		// Token: 0x04003407 RID: 13319
		private static bool m_subscribedToZoneChange = false;

		// Token: 0x04003408 RID: 13320
		private static bool m_skyDomeInitialized = false;

		// Token: 0x04003409 RID: 13321
		private static bool m_vegetationStudioInitialized = false;

		// Token: 0x0400340A RID: 13322
		private static bool m_isDay = false;

		// Token: 0x0400340B RID: 13323
		private static int m_lastIsDayFrame = int.MinValue;

		// Token: 0x0400340C RID: 13324
		private static DateTime m_gameTime;

		// Token: 0x0400340D RID: 13325
		private static int m_lastGameTimeFrame = int.MinValue;

		// Token: 0x0400340E RID: 13326
		private static readonly HashSet<IDayNightToggle> m_pending = new HashSet<IDayNightToggle>();

		// Token: 0x0400340F RID: 13327
		private static readonly List<IDayNightToggle> m_dayNightToggles = new List<IDayNightToggle>();

		// Token: 0x04003414 RID: 13332
		private static GameTimeReplicator m_gameTimeReplicator = null;

		// Token: 0x04003415 RID: 13333
		private static ISkyDomeController m_skyDomeController = null;

		// Token: 0x04003416 RID: 13334
		private static VegetationStudioManager m_vspManager = null;

		// Token: 0x04003417 RID: 13335
		public const string kNST = "NST";

		// Token: 0x04003418 RID: 13336
		public const string kNSTWithTooltip = "<link=\"text:Newhaven Standard Time\">NST</link>";

		// Token: 0x04003419 RID: 13337
		private const string kServerTimeZone = "Central Standard Time";
	}
}
