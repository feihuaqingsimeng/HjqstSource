using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Common.GameTime.Controller;


namespace Common.Util
{
    public class TimeUtil
    {
        public static DateTime zeroDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
        /// <summary>
        /// 从1970开始增加秒数，格式化成DateTime
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FormatTime(int seconds)
        {
            DateTime zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(zero.AddSeconds(seconds));

            return dateTime;
        }

        public static DateTime FormatTime(string time)
        {
            int year = System.DateTime.Now.Year;
            int month = System.DateTime.Now.Month;
            int day = System.DateTime.Now.Day;
            string[] strs = time.ToArray(CSVUtil.SYMBOL_COLON);
            int hour = 0;
            int minute = 0;
            int second = 0;
            if (strs.Length >= 3)
            {
                int.TryParse(strs[0], out hour);
                int.TryParse(strs[1], out minute);
                int.TryParse(strs[2], out second);
            }
            System.DateTime newDate = new System.DateTime(year, month, day, hour, minute, second);
            return newDate;
        }

        public static string FormatTimeToString(int seconds, string format = "")
        {
            DateTime zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(zero.AddSeconds(seconds));
            string result = dateTime.ToString(format);
            return result;
        }

        public static string FormatSecondToMinute(int seconds)
        {
            StringBuilder sb = new StringBuilder();
            int minute = seconds / 60;
            int second = seconds % 60;
            if (minute < 10)
                sb.Append("0");
            sb.Append(minute);
            sb.Append(":");
            if (second < 10)
                sb.Append("0");
            sb.Append(second);
            return sb.ToString();
        }

        public static string FormatSecondToHour(int seconds)
        {
            StringBuilder sb = new StringBuilder();
            int hour = seconds / 3600;
            int minute = (seconds - hour * 3600) / 60;
            int second = seconds % 60;
            if (hour < 10)
                sb.Append("0");
            sb.Append(hour);
            sb.Append(":");
            if (minute < 10)
                sb.Append("0");
            sb.Append(minute);
            sb.Append(":");
            if (second < 10)
                sb.Append("0");
            sb.Append(second);
            return sb.ToString();
        }

        /// <summary>
        /// 返回13位时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long result = Convert.ToInt64(ts.TotalMilliseconds);
            return result;
        }

        /// <summary>
        /// 返回10位时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampBefore10()
        {
            long result = GetTimeStamp();
            result /= 1000;
            return result;
        }

        public static long GetTimeStamp(DateTime endTime)
        {
            TimeSpan ts = endTime - zeroDateTime;
            long result = Convert.ToInt64(ts.TotalMilliseconds);
            return result;
        }

        public static int GetDiffTime(DateTime startTime, DateTime endTime)
        {
            return (int)(endTime - startTime).TotalSeconds;
        }

        public static string FormatLastLoginTimeString(long lastLoginTime)
        {
            if (lastLoginTime == -1)
            {
                return Localization.Localization.Get("ui.friendView.friendOnline");
            }

            long time = TimeController.instance.ServerTimeTicksSecond - lastLoginTime / 1000;
            string timeStr = "";

            if (time < 60)
            {
                timeStr = string.Format(Localization.Localization.Get("ui.friendView.loginMinute"), 1);
            }
            else if (time < 3600)
            {
                timeStr = string.Format(Localization.Localization.Get("ui.friendView.loginMinute"), time / 60);
            }
            else if (time < 86400)
            {
                timeStr = string.Format(Localization.Localization.Get("ui.friendView.loginHour"), time / 3600);
            }
            else
            {
                timeStr = string.Format(Localization.Localization.Get("ui.friendView.loginDay"), time / 86400);

            }
            return timeStr;
        }
    }
}