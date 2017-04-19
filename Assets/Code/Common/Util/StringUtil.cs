using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Util
{
    public static class StringUtil
    {
        #region convert相关
        /// <summary>
        /// 把总秒数格式化为hh:mm:ss格式
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static string ConvertSec2HMS(long sec)
        {
            StringBuilder sb = new StringBuilder();
            long s = sec % 60;
            long m = sec % 3600 / 60;
            long h = sec % 86400 / 3600;
            if (s < 0)
                s = 0;
            if (m < 0)
                m = 0;
            if (h < 0)
                h = 0;

            sb.Append(h.ToString("D2"));
            sb.Append(":");
            sb.Append(m.ToString("D2"));
            sb.Append(":");
            sb.Append(s.ToString("D2"));
            return sb.ToString();
        }
        public static string ConvertBigQuantity(long num)
        {
            if (num >= 100000000) // 亿
            {
                return string.Format(Common.Localization.Localization.Get("common.hundredMillion"), num / 100000000.0f);
            }

            else if (num >= 10000) // 万
            {
				return string.Format(Common.Localization.Localization.Get("common.tenThousand"), num / 10000.0f);
            }
            return num.ToString();
        }
        public static string ConcatNumber<T>(T number1, T number2, string concatStr = "_") where T : struct
        {
            return string.Concat(number1.ToString(), concatStr, number2.ToString());
        }

        //private static bool _withColor = false;
        //public static string ConvertHeroLeagueDesc(string desc, bool withColor)
        //{
        //    _withColor = withColor;
        //    MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertHeroLeagueDesc2);
        //    Regex regex = new Regex("{[0-9]{1,}}");
        //    return regex.Replace(desc, matchEvaluator);
        //}

        //private static string ConvertHeroLeagueDesc2(Match m)
        //{
        //    string rawStr = m.Value;
        //    int heroId = 0;
        //    int.TryParse(rawStr.Substring(1, rawStr.Length - 2), out heroId);
        //    HeroData hd = HeroDataSet.GetHeroData(heroId);
        //    if (hd == null)
        //        return rawStr;
        //    if (!_withColor)
        //        return hd.name;
        //    StringBuilder result = new StringBuilder();
        //    result.Append("[");
        //    result.Append(UIConsts.GetQualityColorStr(hd.quality));
        //    result.Append("]");
        //    result.Append(hd.name);
        //    result.Append("[-]");
        //    return result.ToString();
        //}

        #endregion

        public static string GetTheFirst(string str, int count)
        {
            if (str.Length <= count)
                return str;
            return str.Substring(0, count);
        }

        public static string GetAfter(string str, int count)
        {
            int length = str.Length;
            return str.Substring(length - count);
        }

        #region is相关，判断一个字符串是否为某个类型
        /// <summary>
        /// 是否为邮件地址
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            if (email.Length > 50)
                return false;
            return Regex.IsMatch(
      email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)" +
      @"|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 是否为手机号码
        /// </summary>
        /// <param name="cellPhoneNum"></param>
        /// <returns></returns>
        public static bool IsCellPhoneNum(string cellPhoneNum)
        {
            return Regex.IsMatch(cellPhoneNum, @"^\d{11}$");
        }

        /// <summary>
        /// 是否为密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool IsPassword(string pwd)
        {
            if (pwd.Length < 6 || pwd.Length > 20)
                return false;
            return true;
        }

        /// <summary>
        /// 是否包含任意一个中文字符
        /// \u4e00-\u9fa5 可以匹配所有的中文
        /// [\u4e00-\u9fa5]为匹配任意一个中文字符
        /// [\u4e00-\u9fa5]+为匹配任意一个或多个中文字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsChinese(string str)
        {
            bool result = false;
            if (Regex.IsMatch(str, @"[\u4e00-\u9fa5]+"))
                result = true;
            return result;
        }

        public static bool ContainsWhiteSpace(string str)
        {
            char[] chars = str.ToCharArray();
            for (int i = 0, length = chars.Length; i < length; i++)
            {
                if (char.IsWhiteSpace(chars[i]))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
