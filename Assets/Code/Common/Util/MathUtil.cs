using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.Util
{
    public static class MathUtil
    {
        /// <summary>
        /// 求一个列表的最小公倍数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static long LeastCommonMultiple(List<int> list)
        {
            long result = list.First();
            if (result <= 0) return result;
            long x, y, gcd;
            for (int i = 0, count = list.Count; (i + 1) < count; i++)
            {
                x = result;
                y = list[i + 1];
                gcd = GetGDC(x, y);
                result = x / gcd * y / gcd * gcd;
            }
            return result;
        }

        /// <summary>
        /// 获取最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long GetGDC(long a, long b)
        {
            long temp;
            if (a < b)
            {
                temp = a;
                a = b;
                b = temp;
            }
            if (a % b == 0)
                return b;
            else
                return GetGDC(b, a % b);

        }
    }
}