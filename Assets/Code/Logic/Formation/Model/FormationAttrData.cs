using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Enums;

namespace Logic.Formation.Model
{
    public class FormationAttrData
    {
        private static List<FormationAttrData> _formationDataList;
        public static List<FormationAttrData> FormationDataList
        {
            get
            {
                if (_formationDataList == null)
                {
                    _formationDataList = CSVUtil.Parse<FormationAttrData>("config/csv/formation_attr");
                }
                return _formationDataList;
            }
        }

        public static List<FormationAttrData> GetFormationDatas(int id, FormationAttrType formationAttrType = FormationAttrType.Base)
        {
            List<FormationAttrData> dataList = new List<FormationAttrData>();
            FormationAttrData formationAttrData = null;
            for (int i = 0, count = FormationDataList.Count; i < count; i++)
            {
                formationAttrData = FormationDataList[i];
                if (formationAttrData.id == id && formationAttrType == formationAttrData.formationAttrType)
                {
                    dataList.Add(formationAttrData);
                }
            }
            return dataList;
        }

        public static List<FormationAttrData> GetFormationDatas(int id, int level, FormationAttrType formationAttrType = FormationAttrType.Base)
        {
            List<FormationAttrData> dataList = GetFormationDatas(id, formationAttrType);
            List<FormationAttrData> result = new List<FormationAttrData>();
            FormationAttrData formationAttrData = null;
            for (int i = 0, count = dataList.Count; i < count; i++)
            {
                formationAttrData = dataList[i];
                if (formationAttrData.unlock_lv <= level)
                {
                    result.Add(formationAttrData);
                }
            }
            return result;
        }


        [CSVElement("id")]
        public int id;

        public FormationAttrType formationAttrType;

        [CSVElement("type")]
        public int typeInt
        {
            set
            {
                formationAttrType = (FormationAttrType)value;
            }
        }

        [CSVElement("comat_a")]
        public float comat_a;//1级阵型，对队伍战力的系数

        [CSVElement("comat_b")]
        public float comat_b;//每升1级，增加的队伍战力系数

        [CSVElement("time")]
        public float time;

        [CSVElement("counts")]
        public uint count;

        [CSVElement("interval")]
        public int interval;

        public TargetType targetType;
        [CSVElement("target")]
        public int targetTypeInt
        {
            set
            {
                targetType = (TargetType)value;
            }
        }

        public FormationEffectType effectType;
        [CSVElement("effect_type")]
        public int effect_typeid
        {
            set
            {
                effectType = (FormationEffectType)value;
            }
        }
        [CSVElement("effect_base")]
        public float effect_base;

        [CSVElement("effect_upgrade")]
        public float effect_upgrade;

        [CSVElement("unlock_lv")]
        public int unlock_lv;



    }
}
