using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Enums;
using Logic.Player.Model;
using Logic.Protocol.Model;
using LuaInterface;

namespace Logic.Player.Model
{

    public class PlayerTalentProxy : SingletonMono<PlayerTalentProxy>
    {

        void Awake()
        {
            instance = this;
        }

        public System.Action<int> UpdateTalentDelegate;
        public System.Action UpdateAllTalentDelegate;

        private int _PassiveChoiceOneGroup;
        private int _summonChoiceOneGroup;

        //int: playerid  主角天赋技能
        private Dictionary<int, Dictionary<int, PlayerSkillTalentInfo>> _skillTalentDictionary = new Dictionary<int, Dictionary<int, PlayerSkillTalentInfo>>();
        public Dictionary<int, Dictionary<int, PlayerSkillTalentInfo>> SkillTalentDictionary
        {
            get
            {
                if (_skillTalentDictionary.Count == 0)
                {
                    Dictionary<int, PlayerSkillTalentData> talentDataDic = PlayerSkillTalentData.SkillTalentDataDictionary;
                    PlayerSkillTalentInfo info;
                    foreach (var data in talentDataDic)
                    {
                        if (!_skillTalentDictionary.ContainsKey(data.Value.pet_id))
                        {
                            _skillTalentDictionary.Add(data.Value.pet_id, new Dictionary<int, PlayerSkillTalentInfo>());
                        }
                        Dictionary<int, PlayerSkillTalentInfo> skillInfoList = _skillTalentDictionary[data.Value.pet_id];
                        info = new PlayerSkillTalentInfo(data.Key, 0, 0);
                        skillInfoList.Add(data.Key, info);
                    }
                }
                return _skillTalentDictionary;
            }
        }
		public void UpdateSkillTalentByLuaTable()
		{
			LuaTable playerModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","player_model")[0];
			LuaTable talentInfoTable = (LuaTable)playerModel["playerSkillTalentInfoTable"];
			foreach(DictionaryEntry kvp in talentInfoTable.ToDictTable())
			{
				LuaTable talent = (LuaTable)kvp.Value;
				int playerModelId = kvp.Key.ToString().ToInt32();

				List<TalentProto> talents = new List<TalentProto>();
				List<int> selectList = new List<int>();

				foreach(DictionaryEntry kvp2 in talent.ToDictTable())
				{
					LuaTable talentInfo = (LuaTable)kvp2.Value;
					int no = talentInfo["id"].ToString().ToInt32();
					int lv = talentInfo["level"].ToString().ToInt32();
					int exp = talentInfo["exp"].ToString().ToInt32();
					bool isCarry = talentInfo["isCarry"].ToString().ToBoolean();
					if(isCarry)
					{
						selectList.Add(no);
					}
					TalentProto proto = new TalentProto();
					proto.no = no;
					proto.lv = lv;
					proto.exp = exp;
					talents.Add(proto);
					UpdateSkillTalent(playerModelId,talents,selectList);
				}

			}
		}
        public void UpdateSkillTalent(int playerModelId, List<TalentProto> talents, List<int> selectList)
        {
            if (!SkillTalentDictionary.ContainsKey(playerModelId))
                return;
            Dictionary<int, PlayerSkillTalentInfo> infoDic = SkillTalentDictionary[playerModelId];
            //天赋
            if (talents != null)
            {
                for (int i = 0, count = talents.Count; i < count; i++)
                {
                    TalentProto proto = talents[i];
                    if (infoDic.ContainsKey(proto.no))
                    {
                        infoDic[proto.no].Set(proto.lv, proto.exp);

                    }
                }
            }
            //携带的天赋
            if (selectList != null)
            {
                if (selectList.Count > 0)
                {
                    foreach (var data in infoDic)
                    {
                        data.Value.IsCarry = false;
                    }
                }

                for (int i = 0, count2 = selectList.Count; i < count2; i++)
                {
                    int id = selectList[i];

                    if (infoDic.ContainsKey(id))
                    {
                        infoDic[id].IsCarry = true;
                    }
                }
            }
            //update Player
            PlayerInfo player = PlayerProxy.instance.GetPlayerInfoByModelId(playerModelId);
            if (player != null)
            {
                Dictionary<int, int> activeNormalTalentDic = new Dictionary<int, int>();
                int passiveId = 0;
                int passiveLevel = 0;
                int summonId = 0;
                int summonLevel = 0;
                PlayerSkillTalentInfo info;
                foreach (var data in infoDic)
                {
                    info = data.Value;
                    if (info.level >= 1 && info.talentData.groupType == PlayerSkillTalentType.PassiveNormal)
                        activeNormalTalentDic[info.talentData.effect]= info.level;
                    if (info.talentData.groupType == PlayerSkillTalentType.PassiveThreeChoiceOne && info.IsCarry)
                    {
                        passiveId = info.talentData.effect;
                        passiveLevel = info.level;
                    }
					else if (info.talentData.groupType == PlayerSkillTalentType.SummonThreeChoiceOne&& info.IsCarry)
                    {
                        summonId = info.talentData.effect;
                        summonLevel = info.level;
                    }
                }
                player.UpdateSelectSkillTalent(passiveId, passiveLevel, summonId, summonLevel);
                player.UpdateNormalActiveSkillTalent(activeNormalTalentDic);
            }
        }


        public PlayerSkillTalentInfo GetSkillTalentInfo(int id)
        {
            Dictionary<int, PlayerSkillTalentInfo> infoDic;
            foreach (var data in SkillTalentDictionary)
            {
                infoDic = data.Value;
                if (infoDic.ContainsKey(id))
                {
                    return infoDic[id];
                }

            }
            return null;
        }

        public PlayerSkillTalentInfo GetSkillTalentInfo(int playerModelId, int id)
        {
            if (SkillTalentDictionary.ContainsKey(playerModelId))
            {
                Dictionary<int, PlayerSkillTalentInfo> infoDic = SkillTalentDictionary[playerModelId];
                if (infoDic.ContainsKey(id))
                {
                    return infoDic[id];
                }
            }
            return null;
        }
        public PlayerSkillTalentInfo GetCurPlayerSkillInfoByEffectId(int effectid)
        {
            int playerid = (int)GameProxy.instance.PlayerInfo.playerData.Id;
            if (SkillTalentDictionary.ContainsKey(playerid))
            {
                Dictionary<int, PlayerSkillTalentInfo> infoDic = SkillTalentDictionary[playerid];
                foreach (var data in infoDic)
                {
                    if (data.Value.talentData.effect == effectid)
                    {
                        return data.Value;
                    }
                }
            }
            return null;
        }
        public List<PlayerSkillTalentInfo> GetCurPlayerSkillList(PlayerSkillTalentType type)
        {
            List<PlayerSkillTalentInfo> tempList = new List<PlayerSkillTalentInfo>();
            int playerid = (int)GameProxy.instance.PlayerInfo.playerData.Id;
            if (SkillTalentDictionary.ContainsKey(playerid))
            {
                Dictionary<int, PlayerSkillTalentInfo> infoDic = SkillTalentDictionary[playerid];
                foreach (var data in infoDic)
                {
                    if (data.Value.talentData.groupType == type)
                    {
                        tempList.Add(data.Value);
                    }
                }
            }
            return tempList;
        }


        public void UpdateTalentByProtocol(int id)
        {
            if (UpdateTalentDelegate != null)
                UpdateTalentDelegate(id);
        }
        public void UpdateAllTalentByProtocol()
        {
            if (UpdateTalentDelegate != null)
                UpdateAllTalentDelegate();
        }
    }
}

