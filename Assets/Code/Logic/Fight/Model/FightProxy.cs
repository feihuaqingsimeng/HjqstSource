using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using System.Collections.Generic;
using Logic.Player.Model;
using Logic.Dungeon.Model;
using Logic.Team.Model;
using Logic.Hero.Model;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Formation.Model;
using Logic.Fight.Controller;
using LuaInterface;

namespace Logic.Fight.Model
{
    public class FightProxy : SingletonMono<FightProxy>
    {
        void Awake()
        {
            instance = this;
        }

        public FightPlayerInfo fightPlayerInfo;
        public List<FightHeroInfo> fightHeroInfoList;
        public FormationInfo ourFormation;
        public FightPlayerInfo enemyFightPlayerInfo;
        public List<FightHeroInfo> enemyFightHeroInfoList;
        public FormationInfo enemyFormation;
        public List<DropItem> dropItems;
        public List<ConsortiaFightData> consortiaFightDatas;
        public int fightResultStar = 3;
        public int randomSeed;
        public int fightId;
        public bool isHome;
        public bool consortiaOver;
        public bool consortiaResult;
        public FightOverType fightOverType;
        private Dictionary<BuffType, Dictionary<bool, string>> _buffIcons = new Dictionary<BuffType, Dictionary<bool, string>>();
        //private Dictionary<BuffType, string> _immnueBuffIcon = new Dictionary<BuffType, string>();

        public void Clear()
        {
            fightPlayerInfo = null;
            if (fightHeroInfoList != null)
                fightHeroInfoList.Clear();
            fightHeroInfoList = null;
            enemyFightPlayerInfo = null;
            if (enemyFightHeroInfoList != null)
                enemyFightHeroInfoList.Clear();
            enemyFightHeroInfoList = null;
            if (consortiaFightDatas != null)
                consortiaFightDatas.Clear();
            consortiaFightDatas = null;
            dropItems = null;
            ourFormation = null;
            enemyFormation = null;
            fightResultStar = 3;
            randomSeed = 0;
            fightId = 0;
            isHome = false;
            _buffIcons.Clear();
            //_immnueBuffIcon.Clear();
        }

        public void InitBuffIncos()
        {
            _buffIcons.Clear();
            LuaTable luaTable = LuaScriptMgr.Instance.GetLuaTable("fightdataTable.buffIcons");
            object[] rs = luaTable.ToArray();
            for (int i = 0, length = rs.Length; i < length; i++)
            {
                LuaTable lt = rs[i] as LuaTable;
                BuffType buffType = (BuffType)lt[1];
                bool kindness = lt[2].ToString() == "1";
                string path = lt[3].ToString();
                //Debugger.Log(buffType.ToString() + "   kindness:" + kindness + "   " + path);
                if (!_buffIcons.ContainsKey(buffType))
                    _buffIcons.Add(buffType, new Dictionary<bool, string>());
                Dictionary<bool, string> dic = _buffIcons[buffType];
                dic.Add(kindness, path);
            }
            //_immnueBuffIcon.Clear();
            //LuaTable immuneLuaTable = LuaScriptMgr.Instance.GetLuaTable("fightdataTable.immuneBuffIcons");
            //object[] immune_rs = immuneLuaTable.ToArray();
            //for (int i = 0, length = immune_rs.Length; i < length; i++)
            //{
            //    LuaTable lt = immune_rs[i] as LuaTable;
            //    BuffType buffType = (BuffType)lt[1];
            //    string path = lt[2].ToString();
            //    Debugger.Log(buffType.ToString() + "   " + path);
            //    if (!_immnueBuffIcon.ContainsKey(buffType))
            //        _immnueBuffIcon.Add(buffType, path);
            //}
        }

        public string GetIconPath(BuffType buffType, bool kindness)
        {
            if (_buffIcons.ContainsKey(buffType))
            {
                Dictionary<bool, string> dic = _buffIcons[buffType];
                if (dic.ContainsKey(kindness))
                    return dic[kindness];
            }
            return string.Empty;
        }

        //public string GetImmnueIconPath(BuffType buffType)
        //{
        //    if (_immnueBuffIcon.ContainsKey(buffType))
        //    {
        //        return _immnueBuffIcon[buffType];
        //    }
        //    return string.Empty;
        //}

        public void SetData(TeamFightProtoData myProtoData, List<DropItem> dropItems = null)
        {
            if (myProtoData.player != null)
                fightPlayerInfo = new FightPlayerInfo(GameProxy.instance.PlayerInfo, myProtoData.player);
            fightHeroInfoList = new List<FightHeroInfo>();
            HeroFightProtoData data;
            for (int i = 0, count = myProtoData.heros.Count; i < count; i++)
            {
                data = myProtoData.heros[i];
                fightHeroInfoList.Add(new FightHeroInfo(HeroProxy.instance.GetHeroInfo((uint)data.id), data));
            }
            ourFormation = FormationProxy.instance.GetFormationInfo(myProtoData.lineup.no);
            if (dropItems != null)
                this.dropItems = new List<DropItem>(dropItems);
            Logic.UI.FightResult.Model.FightResultProxy.instance.SaveLastTeamData();
        }

        public void SetData(TeamFightProtoData myProtoData, TeamFightProtoData enemyProtoData, List<DropItem> dropItems = null)
        {
            //my
            if (myProtoData.player != null)
            {
				PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;//new PlayerInfo((uint)myProtoData.player.id, (uint)myProtoData.player.modelId, (uint)myProtoData.player.hairCutId, (uint)myProtoData.player.hairColorId, (uint)myProtoData.player.faceId, myProtoData.player.skinId, string.Empty);
                fightPlayerInfo = new FightPlayerInfo(playerInfo, myProtoData.player);
            }
            fightHeroInfoList = new List<FightHeroInfo>();
            HeroFightProtoData data;
            int count = myProtoData.heros.Count;
            for (int i = 0; i < count; i++)
            {
                data = myProtoData.heros[i];
                HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo((uint)data.id);
                fightHeroInfoList.Add(new FightHeroInfo(heroInfo, data));
            }
            ourFormation = FormationProxy.instance.GetFormationInfo(myProtoData.lineup.no);
            //enemy
            PlayerFightProtoData opponentPlayerData = enemyProtoData.player;
            PlayerInfo enemyPlayer = opponentPlayerData == null ? null : new PlayerInfo((uint)opponentPlayerData.id, (uint)opponentPlayerData.modelId, (uint)opponentPlayerData.hairCutId, (uint)opponentPlayerData.hairColorId, (uint)opponentPlayerData.faceId, opponentPlayerData.skinId, string.Empty);
            if (enemyPlayer != null)
            {
                enemyPlayer.UpdateSkillTalentByProtocol(opponentPlayerData.talnets, opponentPlayerData.selectedTalnet);
                enemyFightPlayerInfo = new FightPlayerInfo(enemyPlayer, opponentPlayerData);
            }

            enemyFightHeroInfoList = new List<FightHeroInfo>();
            count = enemyProtoData.heros.Count;
            for (int i = 0; i < count; i++)
            {
                data = enemyProtoData.heros[i];
                HeroInfo enemyHero = new HeroInfo((uint)data.id, data.modelId, 1, 0, data.star, 1);
                enemyFightHeroInfoList.Add(new FightHeroInfo(enemyHero, data));
            }
            enemyFormation = new FormationInfo(enemyProtoData.lineup.no, enemyProtoData.lineup.lv);
            enemyFormation.isActiveAdditionAttr = enemyProtoData.lineup.attrIsActive;
            //drop
            if (dropItems != null)
                this.dropItems = new List<DropItem>(dropItems);
            Logic.UI.FightResult.Model.FightResultProxy.instance.SaveLastTeamData();
        }

        public void SetFightPlayerInfo(FightPlayerInfo fightPlayerInfo)
        {
            this.fightPlayerInfo = fightPlayerInfo;
        }

        public void SetFightHeroInfoList(List<FightHeroInfo> fightHeroInfoList)
        {
            this.fightHeroInfoList = fightHeroInfoList;
        }

        public void SetEnemyFightHeroInfoList(List<FightHeroInfo> enemyFightHeroInfoList)
        {
            this.enemyFightHeroInfoList = enemyFightHeroInfoList;
        }

        public FightHeroInfo GetFightHeroInfoById(uint id)
        {
            if (fightHeroInfoList == null) return null;
            for (int i = 0, count = fightHeroInfoList.Count; i < count; i++)
            {
                FightHeroInfo fightHeroInfo = fightHeroInfoList[i];
                if (fightHeroInfo.heroInfo.instanceID == id)
                    return fightHeroInfo;
            }
            return null;
        }

        public FightHeroInfo GetFightEnemyHeroInfoById(uint id)
        {
            if (enemyFightHeroInfoList == null) return null;
            for (int i = 0, count = enemyFightHeroInfoList.Count; i < count; i++)
            {
                FightHeroInfo fightHeroInfo = enemyFightHeroInfoList[i];
                if (fightHeroInfo.heroInfo.instanceID == id)
                    return fightHeroInfo;
            }
            return null;
        }

        public void AddConsortiaFightData(ConsortiaFightData consortiaFightData)
        {
            if (consortiaFightDatas == null)
                consortiaFightDatas = new List<ConsortiaFightData>();
            consortiaFightDatas.Add(consortiaFightData);
        }

        public int GetConsortiaJudgeType(int id, int skillId, int targetId)
        {
            int judgeType = 0;
            if (consortiaFightDatas == null) return judgeType;
            int index = 0, count = consortiaFightDatas.Count;
            bool flag = false;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    if (consortiaFightData.judgeDic.ContainsKey(targetId))
                    {
                        judgeType = consortiaFightData.judgeDic[targetId];
                        flag = true;
                    }
                    break;
                }
            }
            if (!flag)
                Debugger.LogError("can not find character {0} skillId {1} tartgetId {2}", id, skillId, targetId);
            if (judgeType == 0)
                Debugger.Log("JudgeType-------miss-----------character {0} skillId {1} tartgetId {2}  judgeType {3}", id, skillId, targetId, judgeType);
            else
                Debugger.Log("JudgeType------------------character {0} skillId {1} tartgetId {2}  judgeType {3}", id, skillId, targetId, judgeType);
            return judgeType;
        }

        public KeyValuePair<int, int> GetConsortiaMechanicsValue(int id, int skillId, int mechanicsIndex, int targetId)
        {
            KeyValuePair<int, int> mechanicsValue = default(KeyValuePair<int, int>);
            if (consortiaFightDatas == null) return mechanicsValue;
            int i = 0, count = consortiaFightDatas.Count;
            for (; i < count; i++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[i];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    if (mechanicsIndex >= consortiaFightData.mechanicses.Count)
                        mechanicsIndex = consortiaFightData.mechanicses.Count - 1;
                    if (mechanicsIndex < 0)
                        return mechanicsValue;
                    List<Gethit> getHits = consortiaFightData.mechanicses[mechanicsIndex].gethits;
                    for (int j = 0, jCount = getHits.Count; j < jCount; j++)
                    {
                        Gethit getHit = getHits[j];
                        if (getHit.heroId == targetId)
                        {
                            mechanicsValue = new KeyValuePair<int, int>(getHit.hurt, getHit.remainHp);
                            Debugger.Log("MechanicsValue------------------character {0} skillId {1} tartgetId{2}  mechanicsValue {3} mechanicsIndex {4}", id, skillId, targetId, mechanicsValue, mechanicsIndex);
                            return mechanicsValue;
                        }
                    }
                }
            }
            return mechanicsValue;
        }

        public List<Buff> GetConsortiaBuffList(int id, int skillId)
        {
            if (consortiaFightDatas == null) return null;
            int index = 0, count = consortiaFightDatas.Count;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    return consortiaFightData.buffList;
                }
            }
            return null;
        }

        public List<Buff> GetConsortiaDelBuffList(int id, int skillId)
        {
            if (consortiaFightDatas == null) return null;
            int index = 0, count = consortiaFightDatas.Count;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    return consortiaFightData.delBuffList;
                }
            }
            return null;
        }

        public List<int> GetConsortiaDeadHeroList(int id, int skillId)
        {
            if (consortiaFightDatas == null) return null;
            int index = 0, count = consortiaFightDatas.Count;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    return consortiaFightData.deadHeroList;
                }
            }
            return null;
        }

        public ConsortiaFightData GetConsortiaFightData(int id, int skillId)
        {
            if (consortiaFightDatas == null) return default(ConsortiaFightData);
            int index = 0, count = consortiaFightDatas.Count;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                {
                    return consortiaFightData;
                }
            }
            return default(ConsortiaFightData);
        }

        public void RemoveConsortiaFightData(int id, int skillId)
        {
            if (consortiaFightDatas == null) return;
            int index = 0, count = consortiaFightDatas.Count;
            for (; index < count; index++)
            {
                ConsortiaFightData consortiaFightData = consortiaFightDatas[index];
                if (consortiaFightData.id == id && consortiaFightData.skillId == skillId)
                    break;
            }
            if (index < count)
                consortiaFightDatas.RemoveAt(index);
        }

        #region add
        private DungeonData _currentDungeonData;
        private int _currentTeamIndex = 0;

        public DungeonData CurrentDungeonData
        {
            get
            {
                return _currentDungeonData;
            }
        }
        public int CurrentTeamIndex
        {
            get
            {
                return _currentTeamIndex;
            }
        }

        public TeamData GetCurrentTeamData()
        {
            TeamData teamData = null;
            if (_currentDungeonData != null
                && _currentTeamIndex < _currentDungeonData.teamIDs.Count)
            {
                uint currentTeamDataID = _currentDungeonData.teamIDs[_currentTeamIndex];
                teamData = TeamData.GetTeamDataByID(currentTeamDataID);
            }
            return teamData;
        }

        public bool hasBoss
        {
            get
            {
#if UNITY_EDITOR
                if (Fight.Controller.FightController.instance.fightType == FightType.Imitate)
                    return Fight.Controller.FightController.instance.hasBossImitate;
#endif
                return GetCurrentTeamData().bossList.Count > 0;
            }
        }

        public bool IsBoss(int teamPosition)
        {
            TeamData currentTeamData = GetCurrentTeamData();
            if (currentTeamData != null && currentTeamData.IsBoss(teamPosition))
                return true;
            return false;
        }

        public void Next()
        {
            _currentTeamIndex++;
        }

        public bool HasNextEnemies()
        {
            int index = _currentTeamIndex;
            index++;
            if (index >= CurrentTeamCount)
                return false;
            return true;
        }

        public int CurrentTeamCount
        {
            get
            {
                int result = 1;
                switch (Fight.Controller.FightController.instance.fightType)
                {
                    case Enums.FightType.Arena:
                        result = 1;
                        break;
                    case Enums.FightType.PVE:
                    case FightType.DailyPVE:
                    case FightType.WorldTree:
                        result = _currentDungeonData.teamIDs.Count;
                        break;
                    case FightType.WorldBoss:
                        result = 1;
                        break;
                    case FightType.FirstFight:
                        result = 1;
                        break;
                }
                return result;
            }
        }

        public bool hasLastTeam
        {
            get
            {
                return _currentTeamIndex < CurrentTeamCount;
            }
        }

        public Dictionary<FormationPosition, HeroInfo> GetMockTeamHeroInfoDictionary(TeamData teamData)
        {
            Dictionary<FormationPosition, HeroInfo> teamHeroInfoDictionary = null;
            if (teamData != null)
            {
                teamHeroInfoDictionary = new Dictionary<FormationPosition, HeroInfo>();
                List<FormationPosition> keys = new List<FormationPosition>(teamData.teamDictionary.Keys);
                int keysCount = keys.Count;
                for (int i = 0; i < keysCount; i++)
                {
                    HeroInfo heroInfo = new HeroInfo((uint)(teamData.teamID + i), teamData.teamDictionary[keys[i]].id, 1, 1, teamData.heroStarDictionary[keys[i]]);
                    teamHeroInfoDictionary.Add(keys[i], heroInfo);
                }
            }
            return teamHeroInfoDictionary;
        }

        public void ResetCurrentDungeonData(DungeonData dungeonData)
        {
            _currentDungeonData = dungeonData;
            _currentTeamIndex = 0;
        }

        public void ResetArena()
        {
            _currentTeamIndex = 0;
        }

        public void ResetExpedition()
        {
            _currentTeamIndex = 0;
        }
        #endregion add
    }
}
