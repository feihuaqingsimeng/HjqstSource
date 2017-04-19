using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Enums;

namespace Logic.Formation.Model
{
    public class FormationData
    {
        private static Dictionary<int, FormationData> _formationDataDictionary;
        public static Dictionary<int, FormationData> FormationDataDictionary
        {
            get
            {
                if (_formationDataDictionary == null)
                {
                    _formationDataDictionary = CSVUtil.Parse<int, FormationData>("config/csv/formation", "id");
                }
                return _formationDataDictionary;
            }
        }

        public static Dictionary<int, FormationData> GetFormationDatas() 
        {
            if (_formationDataDictionary == null)
            {
                _formationDataDictionary = CSVUtil.Parse<int, FormationData>("config/csv/formation", "id");
            }
            return _formationDataDictionary;
        }

        public static List<FormationData> GetAllFormationDataList()
        {
            return FormationDataDictionary.GetValues();
        }

        public static FormationData GetFormationData(int id)
        {
            FormationData formationData = null;
            FormationDataDictionary.TryGetValue(id, out formationData);
            return formationData;
        }

        public List<FormationPosition> GetAllEnabledPos()
        {
            List<FormationPosition> posList = new List<FormationPosition>();
            for (int i = 0, count = pos.Length; i < count; i++)
            {
                if (pos[i])
                {
                    posList.Add((FormationPosition)(i + 1));
                }
            }
            return posList;
        }

        public bool GetPosEnable(int index)
        {
            if (index < 0 || index >= pos.Length)
                return false;
            return pos[index];
        }
        public bool GetPosEnalbe(FormationPosition position)
        {
            int index = (int)position - 1;
            return GetPosEnable(index);
        }

        [CSVElement("id")]
        public int id;

        [CSVElement("name")]
        public string name;

        [CSVElement("des")]
        public string description;

        public bool[] pos = new bool[9];

        [CSVElement("p1")]
        public int p1
        {
            set
            {
                pos[0] = value != 0;
            }
        }

        [CSVElement("p2")]
        public int p2
        {
            set
            {
                pos[1] = value != 0;
            }
        }

        [CSVElement("p3")]
        public int p3
        {
            set
            {
                pos[2] = value != 0;
            }
        }

        [CSVElement("p4")]
        public int p4
        {
            set
            {
                pos[3] = value != 0;
            }
        }

        [CSVElement("p5")]
        public int p5
        {
            set
            {
                pos[4] = value != 0;
            }
        }

        [CSVElement("p6")]
        public int p6
        {
            set
            {
                pos[5] = value != 0;
            }
        }

        [CSVElement("p7")]
        public int p7
        {
            set
            {
                pos[6] = value != 0;
            }
        }

        [CSVElement("p8")]
        public int p8
        {
            set
            {
                pos[7] = value != 0;
            }
        }

        [CSVElement("p9")]
        public int p9
        {
            set
            {
                pos[8] = value != 0;
            }
        }

        [CSVElement("max_lv")]
        public int maxLevel;

        public GameResData upgradeCostGameResData;
        [CSVElement("upgrade_cost")]
        public string upgradeCostStr
        {
            set
            {
                upgradeCostGameResData = new GameResData(value);
            }
        }
        [CSVElement("upgrade_formula_a")]
        public float upgradeA;

        [CSVElement("upgrade_formula_b")]
        public float upgradeB;

        [CSVElement("upgrade_formula_c")]
        public float upgradeC;

        [CSVElement("upgrade_base_cost_a")]
        public float upgrade_base_a;

        [CSVElement("upgrade_base_cost_b")]
        public float upgrade_base_b;

        public int[] formationConditionIds;
        [CSVElement("formation_condition")]
        public string formationConditionString
        {
            set
            {
                formationConditionIds = value.ToArray<int>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public GameResData costGameResData;
        [CSVElement("unlock_cost")]
        public string cost
        {
            set
            {
                costGameResData = new GameResData(value);
            }
        }
    }
}
