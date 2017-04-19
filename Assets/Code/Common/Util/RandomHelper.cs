using UnityEngine;
using System.Collections;
namespace Common.Util
{
    /// <summary>
    /// 随机操作类
    /// </summary>
    public class RandomHelper
    {
        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        /// <summary>
        /// 字符串随机
        /// </summary>
        /// <param name="Length">要随机的位数</param>
        /// <returns></returns>
        public string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[Random.Range(0, 62)]);
            }
            return newRandom.ToString();
        }
        private static char[] constant1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        /// <summary>
        /// 数字随机
        /// </summary>
        /// <param name="Length">要随机的位数</param>
        /// <returns></returns>
        public string GenerateNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant1[Random.Range(0, 10)]);
            }
            return newRandom.ToString();
        }
        /// <summary>
        /// 字符串数组随机
        /// </summary>
        /// <param name="chars">数组</param>
        /// <param name="Length">随机的位数</param>
        /// <returns></returns>
        public string GetStrRandomSurname(string[] chars, int Length)
        {
            int count = chars.Length;
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(count);

            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(chars[Random.Range(0, count)]);
            }
            return newRandom.ToString();
        }
        /// <summary>
        /// 字符串*截取
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public string[] getStringToList(string str)
        {
            return str.Split('*');
        }
    }
}