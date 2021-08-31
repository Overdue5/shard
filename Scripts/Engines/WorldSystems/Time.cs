using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Engines
{
	public class UtilityWorldTime
	{
		#region Config

#if DEBUG
		private static TimeSpan m_DayTime = TimeSpan.FromMinutes(5);
#else
		private static TimeSpan m_DayTime = TimeSpan.FromHours(5);
#endif


		private static int m_DaysInMonth = 28;

		private static int m_DaysInWeek = 7;

		private static int m_MonthsInYear = 12;

		private static int m_HoursInDay = 24;

		private static int m_ReferencePoint = 382;

		private static int m_MinutesInHour = 60;


		private static int m_DaysInYear = DaysInMonth * MonthsInYear;

		private static int m_MinutesInDay = HoursInDay * MinutesInHour;
		
		private static int m_MinutesInMonth = DaysInMonth * MinutesInDay;

		private static int m_MinutesInYear = MonthsInYear * MinutesInMonth;

		public static int DaysInMonth { get { return m_DaysInMonth; } }

		public static int DaysInWeek { get { return m_DaysInWeek; } }

		public static int DaysInYear { get { return m_DaysInYear; } }

		public static TimeSpan DayTime { get { return m_DayTime; } }

		public static int HoursInDay { get { return m_HoursInDay; } }

		public static int MinutesInDay { get { return m_MinutesInDay; } }

		public static int MinutesInHour { get { return m_MinutesInHour; } }

		public static int MinutesInMonth { get { return m_MinutesInMonth; } }

		public static int MinutesInYear { get { return m_MinutesInYear; } }

		public static int MonthsInYear { get { return m_MonthsInYear; } }

		public static TimeSpan MonthTime
		{
			get { return TimeSpan.FromSeconds(DayTime.TotalSeconds * DaysInMonth); }
		}

		public static int ReferencePoint { get { return m_ReferencePoint; } }

#endregion Config

#region Consts

		private static int m_SecInDay = (int)DayTime.TotalSeconds;

		private static int m_SecInHour = SecInDay / HoursInDay;

		private static float m_SecInMinute = (float)SecInHour / (float)MinutesInHour;

		private static int m_SecInMonth = SecInDay * DaysInMonth;

		private static int m_SecInYear = SecInMonth * MonthsInYear;

		private static DateTime m_StartPoint = new DateTime(2020, 1, 1, 0, 0, 0);

		/// <summary>
		/// The number of real seconds in one game day 
		/// </summary>
		public static int SecInDay { get { return m_SecInDay; } }

		/// <summary>
		/// The number of real seconds in one game hour 
		/// </summary>
		public static int SecInHour { get { return m_SecInHour; } }

		/// <summary>
		/// The number of real seconds in one game minute
		/// </summary>
		public static float SecInMinute { get { return m_SecInMinute; } }

		/// <summary>
		/// The number of real seconds in one game month
		/// </summary>
		public static int SecInMonth { get { return m_SecInMonth; } }

		/// <summary>
		/// The number of real seconds in one game year 
		/// </summary>
		public static int SecInYear { get { return m_SecInYear; } }

		/// <summary>
		/// Time reference point  (2020.01.01 00:00:00)
		/// </summary>
		public static DateTime StartPoint { get { return m_StartPoint; } }

#endregion Consts

#region Public Fields

		public static readonly int BackgroundMaxItemID = 0x445B;

		public static readonly int BackgroundMinItemID = 0x4330;

#endregion Public Fields

#region Private Fields

		private static TimeSpan AutoDayUpdateCheck = TimeSpan.FromSeconds(SecInMinute);

		private static WorldDateTime lastdate;

		private static int lastframe = 999;

		private static List<Serial> m_AutoChangeLightLevelList = new List<Serial>(10);

		private static int m_BackgroundItemID;

		private static int m_NowLightLevel;

#endregion Private Fields

#region Public Properties

		public static TimeSpan NextDayStartsIn
		{
			get
			{
				WorldDateTime date = new WorldDateTime(DateTime.UtcNow);
				var total = (double)(DayTime.TotalSeconds - date.Hours * SecInHour - date.Minutes * SecInMinute);
				return TimeSpan.FromSeconds(total);
			}
		}

		public static TimeSpan NextMinuteStartsIn
		{
			get
			{
				var datetime = DateTime.UtcNow;
				var gametime = new WorldDateTime(datetime); //Bad Code(
				var realtime = new WorldDateTime(gametime.GetTotalMinutes()).DateTime;
				return realtime.AddSeconds(UtilityWorldTime.SecInMinute).Subtract(datetime);
			}
		}

		public static TimeSpan NextMonthStartsIn
		{
			get
			{
				var date = new WorldDateTime(DateTime.UtcNow);
				return TimeSpan.FromSeconds((UtilityWorldTime.DaysInMonth - date.Day) * SecInDay) + NextDayStartsIn;
			}
		}

		public static TimeSpan NextSeasonStartsIn
		{
			get
			{
				var date = new WorldDateTime(DateTime.UtcNow);
				var month = (int)date.Month;
				if (month % 2 == 0)
				{
					return NextMonthStartsIn;
				}

				return TimeSpan.FromSeconds(SecInMonth) + NextMonthStartsIn;
			}
		}

		public static TimeSpan NextYearStartsIn
		{
			get
			{
				var date = new WorldDateTime(DateTime.UtcNow);
				return TimeSpan.FromSeconds(SecInMonth * (UtilityWorldTime.MonthsInYear - (int)date.Month)) +
				       NextMonthStartsIn;
			}
		}

		public static int NowLightLevel
		{
			get { return m_NowLightLevel; }
		}

		#endregion Public Properties

		#region Public Methods

		[Usage("SeasonChange SeasonNum")]
		[Description("Change season ony for one client, for tests somethings.")]
		private static void SeasonChange_OnCommand(CommandEventArgs e)
		{
			try
			{
				NetState ns = e.Mobile.NetState;
				ns.Send(SeasonChange.Instantiate((SeasonName)e.GetInt32(0), true));
			}
			catch (Exception exception)
			{
				Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, exception.Message);
				throw;
			}
		}

		[Usage("DateTime")]
		[Description("Сообщает текущую дату и время.")]
		public static void DateTime_OnCommand(CommandEventArgs e)
		{
			if (e.Mobile is PlayerMobile)
			{
				WorldDateTime date = new WorldDateTime(DateTime.UtcNow);
				string msg = $"{date.Hours:D2}\t{date.Minutes:D2}\t" +
				              $"{date.DayOfWeek}\t{date.Day}\t{date.Month}\t{date.Year}";
				e.Mobile.SendAsciiMessage(msg);
			}
		}

		public static Daylight GetDayLight(WorldDateTime date)
		{
			int factor = date.Hours / 4;
			switch (factor)
			{
				case 0: return Daylight.Midnight;
				case 1: return Daylight.Night;
				case 2: return Daylight.Morning;
				case 3: return Daylight.Midday;
				case 4: return Daylight.Day;
				case 5: return Daylight.Evening;
				default: return Daylight.None;
			}
		}

		public static List<Month> GetMonthBySeason(SeasonName season)
		{
			return GetMonthBySeasons(new List<SeasonName> {season});
		}

		public static List<Month> GetMonthBySeasons(List<SeasonName> seasons)
		{
			var result = new List<Month> { };
			foreach (Month month in (Month[])Enum.GetValues(typeof(Month)))
			{
				if (seasons.Contains(GetSeason(month)))
					result.Add(month);
			}

			return result;
		}

		public static SeasonName GetNextSeason(WorldDateTime date)
		{
			return GetNextSeason(GetSeason(date));
		}

		public static SeasonName GetNextSeason(SeasonName season)
		{
			switch (season)
			{
				case SeasonName.Autumn:
					return SeasonName.Winter;

				case SeasonName.Winter:
					return SeasonName.Spring;

				case SeasonName.Spring:
					return SeasonName.Summer;

				case SeasonName.Summer:
					return SeasonName.Autumn;

				default:
					return SeasonName.None;
			}
		}
		
		public static SeasonName GetPreviousSeason(SeasonName season)
		{
			switch (season)
			{
				case SeasonName.Winter:
					return SeasonName.Autumn;

				case SeasonName.Spring:
					return SeasonName.Winter;

				case SeasonName.Summer:
					return SeasonName.Spring;

				case SeasonName.Autumn:
					return SeasonName.Summer;

				default:
					return SeasonName.None;
			}
		}

		public static SeasonName GetPreviousSeason(WorldDateTime date)
		{
			return GetPreviousSeason(GetSeason(date));
		}

		public static SeasonName GetSeason(WorldDateTime date)
		{
			return GetSeason(date.Month);
		}

		public static SeasonName GetSeason(Month month)
		{
			switch (month)
			{
				case Month.March:
				case Month.April:
				case Month.May:
					return SeasonName.Spring;

				case Month.June:
				case Month.July:
				case Month.August:
					return SeasonName.Summer;

				case Month.September:
				case Month.October:
				case Month.November:
					return SeasonName.Autumn;

				case Month.December:
				case Month.January:
				case Month.February:
					return SeasonName.Winter;

				case Month.None:
				default:
					return SeasonName.None;
			}
		}

		public static void Initialize()
		{
			var nowstime = WorldDateTime.Now;
			CommandSystem.Register("DateTime", AccessLevel.Owner, new CommandEventHandler(DateTime_OnCommand));
			CommandSystem.Register("Season", AccessLevel.Owner, new CommandEventHandler(SeasonChange_OnCommand));

			Region.GlobalLight = m_NowLightLevel = GetLightLevel(nowstime);
			Region.PreventLightUpdate = (m) => { return m_AutoChangeLightLevelList.Contains(m.Serial); };

			EventSink.Login += new LoginEventHandler(OnLogin);

			new AutoDayUpdateTimer().Start();
		}

		public static bool IsAutumn(Month month)
		{
			return month == Month.September || month == Month.October || month == Month.November;
		}

		public static bool IsSpring(Month month)
		{
			return month == Month.March || month == Month.May || month == Month.April;
		}

		public static bool IsSummer(Month month)
		{
			return month == Month.June || month == Month.July || month == Month.August;
		}

		public static bool IsWinter(Month month)
		{
			return month == Month.December || month == Month.January || month == Month.February;
		}

		public static void SendNowDate(Mobile mobile, bool sendSunRisingMsg)
		{
			if (mobile == null)
				return;
			//ToDo Send Info for players
		}

		public static double TotalYears(TimeSpan timeSpan)
		{
			double totalSeconds = (int)timeSpan.TotalSeconds;
			return totalSeconds / SecInYear;
		}

		public static Day GetDayOfWeek(WorldDateTime date)
		{
			return (Day)((date.GetTotalDays() % UtilityWorldTime.DaysInWeek) + 1);
		}

#endregion Public Methods

#region Private Methods

		private static int GetLightLevel(WorldDateTime date)
		{
			int lightlevel = 0;

			int eclipseAtTime = 4 * MinutesInHour;
			int sunriseBeginsAtTime = 8 * MinutesInHour;
			int sunriseEndsAtTime = 12 * MinutesInHour;
			int solsticeAtTime = 16 * MinutesInHour;
			int sunsetBeginsAtTime = 20 * MinutesInHour;
			int sunsetEndsAtTime = 0 * MinutesInHour; // 24

			int sunriseBeginsLight = 24;
			int sunriseEndsLight = 12;
			int deepDayLight = 4;
			int sunsetBeginsLight = 12; // 14
			int sunsetEndsLight = 26;
			int deepNightLight = 28;

			int startTime, stopsTime, startLight, stopsLight;

			switch (date.Daylight)
			{
				case Daylight.Midnight:
					startTime = sunsetEndsAtTime;
					stopsTime = eclipseAtTime;
					startLight = sunsetEndsLight;
					stopsLight = deepNightLight;
					break;

				case Daylight.Night:
					startTime = eclipseAtTime;
					stopsTime = sunriseBeginsAtTime;
					startLight = deepNightLight;
					stopsLight = sunriseBeginsLight;
					break;

				case Daylight.Morning:
					startTime = sunriseBeginsAtTime;
					stopsTime = sunriseEndsAtTime;
					startLight = sunriseBeginsLight;
					stopsLight = sunriseEndsLight;
					break;

				case Daylight.Midday:
					startTime = sunriseEndsAtTime;
					stopsTime = solsticeAtTime;
					startLight = sunriseEndsLight;
					stopsLight = deepDayLight;
					break;

				case Daylight.Day:
					startTime = solsticeAtTime;
					stopsTime = sunsetBeginsAtTime;
					startLight = deepDayLight;
					stopsLight = sunsetBeginsLight;
					break;

				case Daylight.Evening:
					startTime = sunsetBeginsAtTime;
					stopsTime = sunsetEndsAtTime;
					startLight = sunsetBeginsLight;
					stopsLight = sunsetEndsLight;
					break;

				default:
					return lightlevel;
			}

			int deltaLight = stopsLight - startLight;

			int dateTime = MinutesInHour * date.Hours + date.Minutes;
			if (dateTime >= startTime)
				dateTime -= startTime;
			else
				dateTime = MinutesInHour * HoursInDay - dateTime + startTime;

			int deltaTime;
			if (stopsTime >= startTime)
				deltaTime = stopsTime - startTime;
			else
				deltaTime = MinutesInHour * HoursInDay - startTime + stopsTime;
			if (deltaTime < 1)
				deltaTime = 1;

			lightlevel = startLight + (int)((double)deltaLight * (double)dateTime / (double)deltaTime);

			if (startLight <= stopsLight)
				lightlevel = Math.Min(Math.Max(startLight, lightlevel), stopsLight);
			else
				lightlevel = Math.Min(Math.Max(stopsLight, lightlevel), startLight);

			lightlevel = Math.Min(Math.Max(1, lightlevel), 28);

			return lightlevel;
		}

		private static void OnLogin(LoginEventArgs args)
		{
			Mobile mobile = args.Mobile;
			if (mobile == null || mobile.NetState == null)
				return;

			SeasonName season = mobile.Region != null ? mobile.Region.Season : mobile.Map.Season;
			if (season != SeasonName.None)
			{
				mobile.NetState.Send(SeasonChange.Instantiate(season, false));
			}

			if (mobile.AccessLevel == AccessLevel.Player || !m_AutoChangeLightLevelList.Contains(mobile.Serial))
			{
				int lightlevel = (mobile.Region != null ? mobile.Region.Light : mobile.Map.Light);
				GlobalLightLevel m_GlobalLightLevel =
					GlobalLightLevel.Instantiate(lightlevel < 0 ? NowLightLevel : lightlevel);
				mobile.NetState.Send(m_GlobalLightLevel);
			}

			MusicName music = (mobile.Region != null ? mobile.Region.Music : MusicName.Invalid);
			if (music != MusicName.Invalid)
			{
				mobile.NetState.Send(PlayMusic.GetInstance(music));
			}

			SendNowDate(mobile, false);
		}

#endregion Private Methods

#region Private Classes

		private class AutoDayUpdateTimer : Timer
		{
#region Private Fields

			private static WorldDateTime m_LastDate;

			private static bool m_SendDate;

#endregion Private Fields

#region Public Constructors

			public AutoDayUpdateTimer() : base(NextMinuteStartsIn, AutoDayUpdateCheck)
			{
				Priority = TimerPriority.TenMS;
				m_LastDate = WorldDateTime.Now;
				m_SendDate = (m_LastDate.Daylight == Daylight.Morning && m_LastDate.Hours <= 10)
						  || (m_LastDate.Daylight == Daylight.Evening && m_LastDate.Hours <= 22)
						   ? true
						   : false;
			}

#endregion Public Constructors

#region Protected Methods

			protected override void OnTick()
			{
				WorldDateTime date = WorldDateTime.Now;

				bool sendlight = false, sendbackground = false, sendmessage = false;

				GlobalLightLevel packetLight = null;
				int lightlevel = GetLightLevel(date);
				if (m_NowLightLevel != lightlevel)
				{
					m_NowLightLevel = lightlevel;
					sendlight = true;
					packetLight = GlobalLightLevel.Instantiate(m_NowLightLevel);
					Region.GlobalLight = lightlevel;
				}

				if (!sendlight && !sendmessage)
				{
					m_LastDate = date;
					return;
				}

				var season = date.Season;
				foreach (NetState state in NetState.Instances)
				{
					var mobile = state.Mobile;
					if (mobile == null)
						continue;
					//if (mobile.AccessLevel > AccessLevel.Seer)
					//{
					//	continue;
					//}

					if ((mobile.Region != null ? mobile.Region.Light : mobile.Map.Light) < 0)
					{
						if (sendlight && (mobile.AccessLevel == AccessLevel.Player || !m_AutoChangeLightLevelList.Contains(mobile.Serial)))
							state.Send(packetLight);
					}

					state.Send(SeasonChange.Instantiate(season, true));

					if (sendmessage)
					{
						if (m_SendDate)
						{
							if ((date.Daylight == Daylight.Morning && date.Hours >= 10) || (date.Daylight == Daylight.Evening && date.Hours >= 22))
							{
								SendNowDate(mobile, true);
								m_SendDate = false;
							}
						}
						else if (date.Daylight != m_LastDate.Daylight && (date.Daylight == Daylight.Morning || date.Daylight == Daylight.Evening))
							m_SendDate = true;
					}
				}

				m_LastDate = date;
			}

#endregion Protected Methods
		}

#endregion Private Classes
	}

	public class WorldDateTime
	{
#region Private Fields

		private DateTime m_DateTime;

		private int m_Day = -1;

		private Daylight m_Daylight = Daylight.None;

		private Day m_DayOfWeek = Engines.Day.None;

		private int m_Hours = -1;

		private int m_Minutes = -1;

		private Month m_Month = Month.None;

		private SeasonName m_Season = SeasonName.None;

		private int m_Year = -1;

#endregion Private Fields

#region Public Constructors

		public WorldDateTime(int totalminutes)
		{
			int excess = totalminutes;
			m_Year = excess / UtilityWorldTime.MinutesInYear;
			excess %= UtilityWorldTime.MinutesInYear;
			m_Month = (Month)(excess / UtilityWorldTime.MinutesInMonth + 1);
			excess %= UtilityWorldTime.MinutesInMonth;
			m_Day = excess / UtilityWorldTime.MinutesInDay + 1;
			m_DayOfWeek = UtilityWorldTime.GetDayOfWeek(this);
			excess %= UtilityWorldTime.MinutesInDay;
			m_Hours = excess / UtilityWorldTime.MinutesInHour;
			excess %= UtilityWorldTime.MinutesInHour;
			m_Minutes = excess;
			m_DateTime = UtilityWorldTime.StartPoint.AddTicks(TimeSpan.TicksPerSecond * (((long)totalminutes / 2) * (long)(2 * UtilityWorldTime.SecInMinute)));
			if ((totalminutes & 0x01) == 0x01)
				m_DateTime.AddSeconds(UtilityWorldTime.SecInMinute);
			Validate();
		}

		public WorldDateTime(DateTime dateTime)
		{
			m_DateTime = dateTime;
			TimeSpan span = dateTime - UtilityWorldTime.StartPoint;
			int totalSeconds = (int)span.TotalSeconds;

			int excess = totalSeconds;
			m_Year = totalSeconds / UtilityWorldTime.SecInYear + UtilityWorldTime.ReferencePoint;
			excess %= UtilityWorldTime.SecInYear;
			m_Month = (Month)(excess / UtilityWorldTime.SecInMonth + 1);
			excess %= UtilityWorldTime.SecInMonth;
			m_Day = excess / UtilityWorldTime.SecInDay + 1;
			m_DayOfWeek = UtilityWorldTime.GetDayOfWeek(this);
			excess %= UtilityWorldTime.SecInDay;
			m_Hours = excess / UtilityWorldTime.SecInHour;
			excess %= UtilityWorldTime.SecInHour;
			m_Minutes = (int)((float)excess / UtilityWorldTime.SecInMinute);
			Validate();
		}

#endregion Public Constructors

#region Public Properties

		public static WorldDateTime Now
		{
			get { return new WorldDateTime(DateTime.UtcNow); }
		}

		public DateTime DateTime { get { return m_DateTime; } }

		public int Day
		{
			get { return m_Day; }
			set
			{
				if (m_Day == value) return;
				m_Day = Utility.LimitMinMax(1, value, UtilityWorldTime.DaysInMonth);
				m_DayOfWeek = UtilityWorldTime.GetDayOfWeek(this);
			}
		}

		public Daylight Daylight { get { if (m_Daylight == Daylight.None) m_Daylight = UtilityWorldTime.GetDayLight(this); return m_Daylight; } }

		public Day DayOfWeek { get { return m_DayOfWeek; } }

		public int DayOfYear { get { return ((int)m_Month - 1) * UtilityWorldTime.DaysInMonth + Day; } }

		public int Hours
		{
			get { return m_Hours; }
			set
			{
				m_Hours = Utility.LimitMinMax(0, value, UtilityWorldTime.HoursInDay - 1);
				m_Daylight = Daylight.None;
			}
		}

		public int Minutes
		{
			get { return m_Minutes; }
			set
			{
				m_Minutes = Utility.LimitMinMax(0, value, UtilityWorldTime.MinutesInHour - 1);
				m_Daylight = Daylight.None;
			}
		}

		public Month Month
		{
			get { return m_Month; }
			set
			{
				if (m_Month == value) return;
				m_Month = value;
				m_DayOfWeek = UtilityWorldTime.GetDayOfWeek(this);
				m_Season = SeasonName.None;
			}
		}

		public SeasonName Season { get { if (m_Season == SeasonName.None) m_Season = UtilityWorldTime.GetSeason(this); return m_Season; } }

		public int Year
		{
			get { return m_Year; }
			set { m_Year = Utility.LimitMin(0, value); }
		}

#endregion Public Properties

#region Public Methods

		public int GetTotalMinutes()
		{
			return m_Year * UtilityWorldTime.MinutesInYear + ((int)m_Month - 1) * UtilityWorldTime.MinutesInMonth + (m_Day - 1) * UtilityWorldTime.MinutesInDay + m_Hours * UtilityWorldTime.MinutesInHour + m_Minutes;
		}

		public int GetTotalDays()
		{
			return m_Year * UtilityWorldTime.DaysInYear + ((int)m_Month - 1) * UtilityWorldTime.DaysInMonth + (m_Day - 1) * UtilityWorldTime.MinutesInDay;
		}

#endregion Public Methods

#region Private Methods

		private void Validate()
		{
			while (m_Minutes >= UtilityWorldTime.MinutesInHour)
			{
				m_Minutes -= UtilityWorldTime.MinutesInHour;
				++m_Hours;
			}
			while (m_Hours >= UtilityWorldTime.HoursInDay)
			{
				m_Hours -= UtilityWorldTime.HoursInDay;
				++m_Day;
			}
			while (m_Day >= UtilityWorldTime.DaysInMonth)
			{
				m_Day -= UtilityWorldTime.DaysInMonth;
				var month = (int)m_Month + 1;
				if (month > 8)
				{
					month = 1;
					++m_Year;
				}
				m_Month = (Month)month;
			}
			m_DayOfWeek = UtilityWorldTime.GetDayOfWeek(this);
			m_Daylight = Daylight.None;
			m_Season = SeasonName.None;
		}

#endregion Private Methods
	}

	public class WorldTimeSpan
	{
		private int m_Days;

		private int m_Hours;

		private int m_Minutes;

		private int m_Months;

		private TimeSpan m_TimeSpan;

		private double m_TotalDays;

		private double m_TotalHours;

		private double m_TotalMinutes;

		private double m_TotalMonths;

		private double m_TotalYears;

		private int m_Years;

		public int Days
		{ get { return m_Days; } }

		public int Hours
		{ get { return m_Hours; } }

		public int Minutes
		{ get { return m_Minutes; } }

		public int Months
		{ get { return m_Months; } }

		public TimeSpan TimeSpan
		{ get { return m_TimeSpan; } }

		public double TotalDays
		{ get { return m_TotalDays; } }

		public double TotalHours
		{ get { return m_TotalHours; } }

		public double TotalMinutes
		{ get { return m_TotalMinutes; } }

		public double TotalMonths
		{ get { return m_TotalMonths; } }

		public double TotalYears
		{ get { return m_TotalYears; } }

		public int Years
		{ get { return m_Years; } }


#region Public Constructors

		public WorldTimeSpan(TimeSpan span) : this((int)span.TotalSeconds)
		{
		}

		public WorldTimeSpan(int years, int months, int days, int hours, int minutes)
			: this(years * UtilityWorldTime.SecInYear + months * UtilityWorldTime.SecInMonth + days * UtilityWorldTime.SecInDay + hours * UtilityWorldTime.SecInHour + (int)(minutes * UtilityWorldTime.SecInMinute))
		{
		}

#endregion Public Constructors

#region Private Constructors

		private WorldTimeSpan(int seconds)
		{
			m_TimeSpan = TimeSpan.FromSeconds((double)seconds);

			m_TotalYears = (double)seconds / (double)UtilityWorldTime.SecInYear;
			m_TotalMonths = (double)seconds / (double)UtilityWorldTime.SecInMonth;
			m_TotalDays = (double)seconds / (double)UtilityWorldTime.SecInDay;
			m_TotalHours = (double)seconds / (double)UtilityWorldTime.SecInHour;
			m_TotalMinutes = (double)seconds / (double)UtilityWorldTime.SecInMinute;

			m_Years = seconds / UtilityWorldTime.SecInYear;
			m_Months = (seconds %= UtilityWorldTime.SecInYear) / UtilityWorldTime.SecInMonth;
			m_Days = (seconds %= UtilityWorldTime.SecInMonth) / UtilityWorldTime.SecInDay;
			m_Hours = (seconds %= UtilityWorldTime.SecInDay) / UtilityWorldTime.SecInHour;
			m_Minutes = (int)((seconds %= UtilityWorldTime.SecInHour) / UtilityWorldTime.SecInMinute);
		}

#endregion Private Constructors

#region Public Properties

		public static WorldTimeSpan Zero
		{
			get { return new WorldTimeSpan(0); }
		}

#endregion Public Properties

#region Public Methods

		public static WorldTimeSpan FromDays(double days)
		{
			return new WorldTimeSpan((int)(days * (double)UtilityWorldTime.SecInDay));
		}

		public static WorldTimeSpan FromDays(double minDays, double maxDays)
		{
			return WorldTimeSpan.FromDays(Utility.RandomMinMax(minDays, maxDays));
		}

		public static WorldTimeSpan FromHours(double hours)
		{
			return new WorldTimeSpan((int)(hours * (double)UtilityWorldTime.SecInHour));
		}

		public static WorldTimeSpan FromHours(double minHours, double maxHours)
		{
			return WorldTimeSpan.FromHours(Utility.RandomMinMax(minHours, maxHours));
		}

		public static WorldTimeSpan FromMinutes(double minutes)
		{
			return new WorldTimeSpan((int)(minutes * (double)UtilityWorldTime.SecInMinute));
		}

		public static WorldTimeSpan FromMinutes(double minMinutes, double maxMinutes)
		{
			return WorldTimeSpan.FromMinutes(Utility.RandomMinMax(minMinutes, maxMinutes));
		}

		public static WorldTimeSpan FromMonths(double months)
		{
			return new WorldTimeSpan((int)(months * (double)UtilityWorldTime.SecInMonth));
		}

		public static WorldTimeSpan FromMonths(double minMonths, double maxMonths)
		{
			return WorldTimeSpan.FromMonths(Utility.RandomMinMax(minMonths, maxMonths));
		}

		public static WorldTimeSpan FromYears(double years)
		{
			return new WorldTimeSpan((int)(years * (double)UtilityWorldTime.SecInYear));
		}

		public static WorldTimeSpan FromYears(double minYears, double maxYears)
		{
			return WorldTimeSpan.FromYears(Utility.RandomMinMax(minYears, maxYears));
		}

#endregion Public Methods
	}

	internal static class Extension
	{
#region Public Methods

		public static List<Month> ToMonth(this SeasonName season)
		{
			return UtilityWorldTime.GetMonthBySeason(season);
		}

#endregion Public Methods
	}

#region Enums

	public enum Day
	{
		None = 0,

		Monday = 1,

		Tuesday = 2,

		Wednesday = 3,

		Thursday = 4,

		Friday = 5,

		Saturday = 6,

		Sunday = 7,
	};

	public enum Daylight
	{
		None = 0,

		/// <summary>
		/// [00:00 - 04:00]
		/// </summary>
		Midnight = 1,

		/// <summary>
		/// [04:00 - 08:00]
		/// </summary>
		Night = 2,

		/// <summary>
		/// [08:00 - 12:00]
		/// </summary>
		Morning = 3,

		/// <summary>
		/// [12:00 - 16:00]
		/// </summary>
		Midday = 4,

		/// <summary>
		/// [16:00 - 20:00]
		/// </summary>
		Day = 5,

		/// <summary>
		/// [20:00 - 24:00]
		/// </summary>
		Evening = 6
	}

	public enum Month
	{
		None = 0,

		January = 1,

		February = 2,

		March = 3,

		April = 4,

		May = 5,

		June = 6,

		July = 7,

		August = 8,

		September = 9,

		October = 10,

		November = 11,

		December = 12
	};

	//public enum Season
	//{
		
	//	Spring = 0x00,

	//	Summer = 0x01,

	//	Autumn = 0x02,

	//	Winter = 0x03,

	//	Desolation = 0x04,

	//	None = 0xFF
	//}

#endregion Enums
}

