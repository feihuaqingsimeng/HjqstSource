using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Util
{
    public static class RandomUtil
    {
        public static bool GetRandom(float probability)
        {
            if (probability == 1) return true;
            return Random.Range(0f, 1f) <= probability;
        }

        public static bool GetRandom(float probability, bool zeroEqOne)
        {
            if (zeroEqOne)
                probability = probability == 0 ? 1f : probability;
            return GetRandom(probability);
        }

        public static float GetRandom()
        {
            return Random.Range(0, 1f);
        }

        /** server 
    	 * 判断概率是否触发	 
	     * @param seed	种子
	     * @param prob	概率
	     * @param ratio	概率比
	     * @return
         * **/
        public static bool GetRandom(int seed, float prob, int ratio)
        {
            int randomNum = GetRandomBySeedNoEnd(seed, 0, ratio);
            if (randomNum < prob * ratio)
            {
                return true;
            }
            return false;
        }

        public static int GetRandomBySeedNoEnd(int seed, int start, int end)
        {
            if (start == 0 && end == 0)
            {
                return 0;
            }

            seed = (499999 * seed + 19961) % 2147483647;
            if (start == 0)
            {
                return seed % end;
            }

            return start + seed % (end - start);
        }
    }
}