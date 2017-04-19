using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Logic.Hero.Model;

namespace Logic.Team.Model
{
    public class TeamData
    {
        private static Dictionary<uint, TeamData> _teamDataDictionary;

        public static Dictionary<uint, TeamData> GetTeamDatas()
        {
            if (_teamDataDictionary == null)
            {
                _teamDataDictionary = CSVUtil.Parse<uint, TeamData>("config/csv/team", "team_id");
            }
            return _teamDataDictionary;
        }

        public static Dictionary<uint, TeamData> TeamDataDictionary
        {
            get
            {
                if (_teamDataDictionary == null)
                {
                    GetTeamDatas();
                }
                return _teamDataDictionary;
            }
        }

        public static TeamData GetTeamDataByID(uint teamID)
        {
            TeamData teamData = null;
            if (TeamDataDictionary.ContainsKey(teamID))
            {
                teamData = TeamDataDictionary[teamID];
            }
            return teamData;
        }

        public Dictionary<FormationPosition, HeroData> teamDictionary = new Dictionary<FormationPosition, HeroData>();
        public Dictionary<FormationPosition, int> heroStarDictionary = new Dictionary<FormationPosition, int>();
        public Dictionary<FormationPosition, Rate> rateDictionary = new Dictionary<FormationPosition, Rate>();
        public Dictionary<FormationPosition, float> scaleDictionary = new Dictionary<FormationPosition, float>();

        [CSVElement("team_id")]
        public uint teamID;

        [CSVElement("id_lv_1")]
        public string IDLV1
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_1, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_1, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_1, rate);
                }
            }
        }

        [CSVElement("id_lv_2")]
        public string IDLV2
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_2, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_2, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_2, rate);
                }
            }
        }

        [CSVElement("id_lv_3")]
        public string IDLV3
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_3, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_3, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_3, rate);
                }
            }
        }

        [CSVElement("id_lv_4")]
        public string IDLV4
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_4, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_4, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_4, rate);
                }
            }
        }

        [CSVElement("id_lv_5")]
        public string IDLV5
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_5, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_5, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_5, rate);
                }
            }
        }

        [CSVElement("id_lv_6")]
        public string IDLV6
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_6, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_6, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_6, rate);
                }
            }
        }

        [CSVElement("id_lv_7")]
        public string IDLV7
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_7, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_7, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_7, rate);
                }
            }
        }

        [CSVElement("id_lv_8")]
        public string IDLV8
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_8, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_8, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_8, rate);
                }
            }
        }

        [CSVElement("id_lv_9")]
        public string IDLV9
        {
            set
            {
                if (value != string.Empty)
                {
                    float[] values = value.ToArray<float>(CSVUtil.SYMBOL_SEMICOLON);
                    int heroID = (int)values[0];
                    int heroStar = (int)values[1];
                    float HPRate = values[2];
                    float attackRate = values[3];
                    float defenseRate = values[4];
                    Rate rate = new Rate();
                    rate.HPRate = HPRate;
                    rate.attackRate = attackRate;
                    rate.defenseRate = defenseRate;
                    teamDictionary.Add(FormationPosition.Enemy_Position_9, HeroData.GetHeroDataByID(heroID));
                    heroStarDictionary.Add(FormationPosition.Enemy_Position_9, heroStar);
                    rateDictionary.Add(FormationPosition.Enemy_Position_9, rate);
                }
            }
        }

        public List<FormationPosition> bossList;
        [CSVElement("boss")]
        public string bossStr
        {
            set
            {
                bossList = new List<FormationPosition>();
                List<uint> list = value.ToList<uint>(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    bossList.Add((FormationPosition)(list[i] + 100));//敌方+100
                }
                list.Clear();
                list = null;
            }
        }

        public bool IsBoss(int teamPosition)
        {
            return bossList.Contains((FormationPosition)teamPosition);
        }

        [CSVElement("pre_dialog")]
        public int preDialogID;
        public bool HasPredialog
        {
            get
            {
                return preDialogID > 0;
            }
        }

        [CSVElement("end_dialog")]
        public int endDialogID;
        public bool HasEndDialog
        {
            get
            {
                return endDialogID > 0;
            }
        }

        [CSVElement("monster_scale1")]
        public string monsterScale1
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_1, scale);
            }
        }

        [CSVElement("monster_scale2")]
        public string monsterScale2
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_2, scale);
            }
        }

        [CSVElement("monster_scale3")]
        public string monsterScale3
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_3, scale);
            }
        }

        [CSVElement("monster_scale4")]
        public string monsterScale4
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_4, scale);
            }
        }

        [CSVElement("monster_scale5")]
        public string monsterScale5
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_5, scale);
            }
        }

        [CSVElement("monster_scale6")]
        public string monsterScale6
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_6, scale);
            }
        }

        [CSVElement("monster_scale7")]
        public string monsterScale7
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_7, scale);
            }
        }

        [CSVElement("monster_scale8")]
        public string monsterScale8
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_8, scale);
            }
        }

        [CSVElement("monster_scale9")]
        public string monsterScale9
        {
            set
            {
                float scale = 0;
                float.TryParse(value, out scale);
                scaleDictionary.Add(FormationPosition.Enemy_Position_9, scale);
            }
        }

        [CSVElement("CD_reductionOnce")]
        public float cdReduceRate;
    }

    public struct Rate
    {
        public float HPRate;
        public float attackRate;
        public float defenseRate;
    }
}