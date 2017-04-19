using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.Fight.Model;
using Logic.Net.Controller;
using System.Collections.Generic;
using Logic.Skill.Model;
using Logic.Enums;
namespace Logic.Fight.Controller
{
    public struct ConsortiaFightData
    {
        public int id;
        public int skillId;
        public Dictionary<int, int> judgeDic;
        public List<Mechanics> mechanicses;
        public List<Buff> buffList;
        public List<Buff> delBuffList;
        public List<int> deadHeroList;
    }

    public class ConsortiaFightController : SingletonMono<ConsortiaFightController>
    {
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RealTimeFightDataResp).ToString(), LOBBY2CLIENT_REALTIME_FIGHT_DATA_RESP_HANDLER);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.FightCmdSynResp).ToString(), LOBBY2CLIENT_FIGHT_CMD_SYN_RESP_HANDLER);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FightOverResp).ToString(), LOBBY2CLIENT_FIGHT_OVER_RESP_HANDLER);
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RealTimeFightDataResp).ToString(), LOBBY2CLIENT_REALTIME_FIGHT_DATA_RESP_HANDLER);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.FightCmdSynResp).ToString(), LOBBY2CLIENT_FIGHT_CMD_SYN_RESP_HANDLER);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FightOverResp).ToString(), LOBBY2CLIENT_FIGHT_OVER_RESP_HANDLER);
            }
        }

        #region request to server
        //匹配战斗
        public void CLIENT2LOBBY_FIGHT_MATCH_REQ()
        {
            Protocol.ProtocolProxy.instance.SendProtocolByte((int)MSG.FightMatchReq, null);
        }

        //战斗开始
        public void CLIENT2LOBBY_FIGHT_START_REQ(int fightId)
        {
            FightStartReq fightStartReq = new FightStartReq();
            fightStartReq.fightId = fightId;
            Protocol.ProtocolProxy.instance.SendProtocol(fightStartReq);
        }

        //请求释放技能
        public void CLIENT2LOBBY_SKILL_REQ(int fightId, int heroId, int skillId)
        {
            SkillReq skillReq = new SkillReq();
            skillReq.fightId = fightId;
            skillReq.heroId = heroId;
            skillReq.skillId = skillId;
            Protocol.ProtocolProxy.instance.SendProtocol(skillReq);
        }

        public void CLIENT2LOBBY_FIGHT_CMD_SYN_REVISE_REQ()
        {
            Protocol.ProtocolProxy.instance.SendProtocolByte((int)MSG.FightCmdSynReviseReq, null);
        }
        #endregion

        #region server call back

        private bool LOBBY2CLIENT_REALTIME_FIGHT_DATA_RESP_HANDLER(Observers.Interfaces.INotification note)
        {
            RealTimeFightDataResp resp = note.Body as RealTimeFightDataResp;
            FightProxy.instance.fightId = resp.fightId;
            FightProxy.instance.randomSeed = resp.randomSeed;
            FightProxy.instance.SetData(resp.selfTeamData, resp.opponentTeamData);
            //FightController.instance.fightType = Enums.FightType.PVP;
            FightController.instance.fightType = Enums.FightType.ConsortiaFight;
            FightProxy.instance.consortiaOver = false;
            FightProxy.instance.isHome = resp.isHome;
            DataMessageHandler.DataMessage_ClearConsortiaSkills();
            Debugger.Log("start consortia fight --------------------------------------------------------------");
            _count = 0;
            for (int i = 0, count = resp.cmdList.Count; i < count; i++)
            {
                FightCmdSynResp fightCmdSynResp = resp.cmdList[i];
                Dictionary<int, int> judgeDic = new Dictionary<int, int>();
                Debugger.Log("character {0} skillId {1}", fightCmdSynResp.heroId, fightCmdSynResp.skillId);
                for (int j = 0, jCount = fightCmdSynResp.effectJudgeTypes.Count; j < jCount; j++)
                {
                    DoubleIntProto dip = fightCmdSynResp.effectJudgeTypes[j];
                    Debugger.Log("character {0} target {1} judgeType {2}", fightCmdSynResp.heroId, dip.value1, dip.value2);
                    judgeDic.Add(dip.value1, dip.value2);
                }
                _count++;
                ConsortiaFightData consortiaFightData = new ConsortiaFightData();
                consortiaFightData.id = fightCmdSynResp.heroId;
                consortiaFightData.skillId = fightCmdSynResp.skillId;
                consortiaFightData.judgeDic = judgeDic;
                consortiaFightData.mechanicses = fightCmdSynResp.mechanicsList;
                consortiaFightData.buffList = fightCmdSynResp.newBuffList;
                consortiaFightData.delBuffList = fightCmdSynResp.delBuffList;
                consortiaFightData.deadHeroList = fightCmdSynResp.diedHeroList;
                FightProxy.instance.AddConsortiaFightData(consortiaFightData);
                foreach (var item in consortiaFightData.mechanicses)
                {
                    foreach (var g in item.gethits)
                    {
                        Debugger.Log("damaged----------------character {0} hurted {1} remainHp  {2}", g.heroId, g.hurt, g.remainHp);
                    }
                }
                foreach (var did in consortiaFightData.deadHeroList)
                {
                    Debugger.Log("dead----------------character {0} skillId {1} deadid  {2}", fightCmdSynResp.heroId, fightCmdSynResp.skillId, did);
                }
                DataMessageHandler.DataMessage_OrderConsortiaSkill((uint)fightCmdSynResp.heroId, (uint)fightCmdSynResp.skillId, fightCmdSynResp.releaseTime, false);
            }

            Debugger.LogError("server skill count:{0}", _count);
            FightController.instance.PreReadyFight();
            return true;
        }

        int _count = 0;

        //private bool LOBBY2CLIENT_FIGHT_CMD_SYN_RESP_HANDLER(Observers.Interfaces.INotification note)
        //{
        //    FightCmdSynResp resp = note.Body as FightCmdSynResp;
        //    Dictionary<int, int> judgeDic = new Dictionary<int, int>();
        //    Debugger.Log("character {0} skillId {1}", resp.heroId, resp.skillId);
        //    for (int i = 0, count = resp.effectJudgeTypes.Count; i < count; i++)
        //    {
        //        DoubleIntProto dip = resp.effectJudgeTypes[i];
        //        Debugger.Log("character {0} target {1} judgeType {2}", resp.heroId, dip.value1, dip.value2);
        //        judgeDic.Add(dip.value1, dip.value2);
        //    }
        //    _count++;
        //    Debugger.LogError("server skill count:{0}", _count);
        //    //List<Dictionary<int, int>> mechanicsList = new List<Dictionary<int, int>>();
        //    //for (int i = 0, count = resp.mechanicsList.Count; i < count; i++)
        //    //{
        //    //    Mechanics mechanics = resp.mechanicsList[i];
        //    //    Dictionary<int, int> targets = new Dictionary<int, int>();
        //    //    for (int j = 0, jCount = mechanics.gethits.Count; j < jCount; j++)
        //    //    {
        //    //        Gethit gh = mechanics.gethits[j];
        //    //        targets.Add(gh.heroId, gh.hurt);
        //    //    }
        //    //    mechanicsList.Add(targets);
        //    //}

        //    ConsortiaFightData consortiaFightData = new ConsortiaFightData();
        //    consortiaFightData.id = resp.heroId;
        //    consortiaFightData.skillId = resp.skillId;
        //    consortiaFightData.judgeDic = judgeDic;
        //    consortiaFightData.mechanicses = resp.mechanicsList;
        //    consortiaFightData.buffList = resp.newBuffList;
        //    consortiaFightData.delBuffList = resp.delBuffList;
        //    FightProxy.instance.AddConsortiaFightData(consortiaFightData);
        //    foreach (var item in consortiaFightData.mechanicses)
        //    {
        //        foreach (var g in item.gethits)
        //        {
        //            Debugger.Log("damaged----------------character {0} judgeType {1} hurted {2} remainHp  {3}", g.heroId, g.judgeType, g.hurt, g.remainHp);
        //        }
        //    }
        //    DataMessageHandler.DataMessage_OrderConsortiaSkill((uint)resp.heroId, (uint)resp.skillId, resp.releaseTime, false);
        //    return true;
        //}

        private bool LOBBY2CLIENT_FIGHT_OVER_RESP_HANDLER(Observers.Interfaces.INotification note)
        {
            FightOverResp resp = note.Body as FightOverResp;
            FightProxy.instance.consortiaOver = true;
            FightProxy.instance.consortiaResult = resp.isWin;
            Debugger.Log("pvp result {0}", resp.isWin);
            return true;
        }
        #endregion
    }
}