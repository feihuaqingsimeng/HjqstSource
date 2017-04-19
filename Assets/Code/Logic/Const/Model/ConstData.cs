using UnityEngine;
using System.Collections;
using Common.Util;
namespace Logic.Const.Model
{
    public class ConstData
    {
        private static ConstData _constData;

        public static ConstData GetConstData()
        {
            if (_constData == null)
            {
                _constData = CSVUtil.ParseClass<ConstData>("config/csv/const");
            }
            return _constData;
        }

        #region field
        [CSVElement("skill_hurt_a")]
        public float skillHurtA;
        
        [CSVElement("float_hurt_add")]
        public float floatHurtAdd;
        
        [CSVElement("caron_hurt_add")]
        public float comboHurtAdd;

        [CSVElement("caron_hurt_max")]
        public float comboHurtMax;

        [CSVElement("caron_hurt_time")]
        public float comboHurtDelay;

        [CSVElement("ai_skill_round")]
        public uint aiSkillRound;
        #endregion
    }
}