using Common.Util;
using System.Collections;
using System.Collections.Generic;

namespace Logic.Dungeon.Model
{
    public class SpecialBossData
    {
        private static Dictionary<uint, SpecialBossData> _specialBossDataDictionary;
        public static Dictionary<uint, SpecialBossData> GetSpecialBossDatas()
        {
            if (_specialBossDataDictionary == null)
            {
                _specialBossDataDictionary = CSVUtil.Parse<uint, SpecialBossData>("config/csv/special_boss", "accountlv");
            }
            return _specialBossDataDictionary;
        }

        public static SpecialBossData GetSpecialBossDataByLevel(uint level)
        {
            GetSpecialBossDatas();
            SpecialBossData specialBossData = null;
            if (_specialBossDataDictionary.ContainsKey(level))
            {
                specialBossData = _specialBossDataDictionary[level];
            }
            else
            {
                Debugger.LogError("can not find SpecialBossData data ,level:" + level);
            }
            return specialBossData;
        }

        public static float GetBossPowerByLevel(uint level)
        {
            SpecialBossData specialBossData = GetSpecialBossDataByLevel(level);
            if (specialBossData != null)
                return specialBossData.bossPower;
            return 1f;
        }

        [CSVElement("accountlv")]
        public uint accountlv;

        [CSVElement("bossspower")]
        public float bossPower;
    }
}
