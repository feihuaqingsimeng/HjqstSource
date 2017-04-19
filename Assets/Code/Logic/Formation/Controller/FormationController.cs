using UnityEngine;
using Logic.Protocol.Model;
using Logic.Formation.Model;
using Logic.UI.TrainFormation.Model;
using Logic.Enums;
using Logic.UI.ManageHeroes.Model;
using Logic.UI.RedPoint.Model;
using LuaInterface;
using System.Collections.Generic;

namespace Logic.Formation.Controller
{
    public class FormationController : SingletonMono<FormationController>
    {
        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            //			Observers.Facade.Instance.RegisterObserver(((int)MSG.TeamInfoResp).ToString(),LOBBY2CLIENT_TeamInfoResp_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.LineupUpgradeResp).ToString(), LOBBY2CLIENT_LineupUpgradeResp_handler);
            //			Observers.Facade.Instance.RegisterObserver(((int)MSG.TeamAddResp).ToString(),LOBBY2CLIENT_TeamAddResp_handler);
           /// Observers.Facade.Instance.RegisterObserver(((int)MSG.LineupPointBuyResp).ToString(), LOBBY2CLIENT_LineupBringUpPointResp_handler);
           // Observers.Facade.Instance.RegisterObserver(((int)MSG.LineupPointSynResp).ToString(), LOBBY2CLIENT_LineupBringUpPointSynResp_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.LineupAddResp).ToString(), LOBBY2CLIENT_LineupAddResp_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.LineupAttrActiveResp).ToString(), LOBBY2CLIENT_LineupAttrActiveResp_handler);

            //Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_TeamInfoResp_handler", LOBBY2CLIENT_TeamInfoResp_handler);
			Observers.Facade.Instance.RegisterObserver("InitAllTeamAndFormationFromLua", InitAllTeamAndFormationFromLua);

        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                //				Observers.Facade.Instance.RemoveObserver(((int)MSG.TeamInfoResp).ToString(),LOBBY2CLIENT_TeamInfoResp_handler);
               // Observers.Facade.Instance.RemoveObserver(((int)MSG.LineupUpgradeResp).ToString(), LOBBY2CLIENT_LineupUpgradeResp_handler);
                //				Observers.Facade.Instance.RemoveObserver(((int)MSG.TeamAddResp).ToString(),LOBBY2CLIENT_TeamAddResp_handler);
               // Observers.Facade.Instance.RemoveObserver(((int)MSG.LineupPointBuyResp).ToString(), LOBBY2CLIENT_LineupBringUpPointResp_handler);
               // Observers.Facade.Instance.RemoveObserver(((int)MSG.LineupPointSynResp).ToString(), LOBBY2CLIENT_LineupBringUpPointSynResp_handler);
               // Observers.Facade.Instance.RemoveObserver(((int)MSG.LineupAddResp).ToString(), LOBBY2CLIENT_LineupAddResp_handler);
               // Observers.Facade.Instance.RemoveObserver(((int)MSG.LineupAttrActiveResp).ToString(), LOBBY2CLIENT_LineupAttrActiveResp_handler);

               // Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_TeamInfoResp_handler", LOBBY2CLIENT_TeamInfoResp_handler);
               // Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_TeamAddResp_handler", LOBBY2CLIENT_TeamAddResp_handler);
				Observers.Facade.Instance.RemoveObserver("InitAllTeamAndFormationFromLua", InitAllTeamAndFormationFromLua);
            }
        }
		private LuaTable _formationModelTable;
		private LuaTable FormationModelTable
		{
			get{
				if (_formationModelTable == null)
					_formationModelTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0];
				return _formationModelTable;
			}
		}
//
//        #region  client to server
//        ///请求队伍信息(C->S)
//        public void CLIENT2LOBBY_TeamInfo_REQ()
//        {
//            TeamInfoReq req = new TeamInfoReq();
//            Protocol.ProtocolProxy.instance.SendProtocol(req);
//        }
//        ///请求阵型升级(C->S)
//        public void CLIENT2LOBBY_LineupUpgrade_REQ(int id)
//        {
//            LineupUpgradeReq req = new LineupUpgradeReq();
//            req.no = id;
//            Protocol.ProtocolProxy.instance.SendProtocol(req);
//        }
//        ///请求队伍变动(C->S)
//        public void CLIENT2LOBBY_TeamChange_REQ(FormationTeamType type)
//        {
//
////            TeamChangeReq req = new TeamChangeReq();
////
////            FormationTeamInfo info = FormationProxy.instance.GetFormationTeam(type);
////            req.team = new TeamProtoData();
////            req.team.teamNo = (int)type;
////            req.team.lineupNo = info.formationId;
////            PosProtoData posData;
////            foreach (var pos in info.teamPosDictionary)
////            {
////                posData = new PosProtoData();
////                posData.posIndex = (int)pos.Key;
////                posData.heroId = (int)pos.Value;
////                req.team.posList.Add(posData);
////            }
////            Protocol.ProtocolProxy.instance.SendProtocol(req);
////			LuaTable formationCtrl =  (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","hero_model")[0];
////			formationCtrl.GetLuaFunction("CheckHasAdvanceBreakthroughHeroByRedPoint").Call();
//
//			LuaTable formationControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "formation_controller")[0];
//			formationControllerLuaTable.GetLuaFunction("TeamChangeReq").Call((int)type);
//			LuaTable heroModelLuaTable =  (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","hero_model")[0];
//			heroModelLuaTable.GetLuaFunction("CheckHasAdvanceBreakthroughHeroByRedPoint").Call();
//        }
//        ///请求购买培养点(C->S)
//        public void CLIENT2LOBBY_LineupPointBuy_REQ()
//        {
//            LineupPointBuyReq req = new LineupPointBuyReq();
//            Protocol.ProtocolProxy.instance.SendProtocol(req);
//        }
//        ///请求培养点同步(C->S)
//        public void CLIENT2LOBBY_LineupPointSyn_REQ()
//        {
//            LineupPointSynReq req = new LineupPointSynReq();
//            Protocol.ProtocolProxy.instance.SendProtocol(req);
//        }
//        #endregion
        public void AllFormationInitInLua()
        {
            LuaTable formationModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0];
            FormationProxy.instance.AddAllUnlockFormation((LuaTable)formationModel["formationInfoTable"]);
            FormationProxy.instance.AddAllTeamData((LuaTable)formationModel["formationTeamTable"]);
            ManageHeroesProxy.instance.ClearFormationTeamDic();
        }
//
//        public void LOBBY2CLIENT_LineupAttrActiveReq(int formationId)
//        {
//            IntProto req = new IntProto();
//            req.value = formationId;
//            Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.LineupAttrActiveReq, req);
//        }
//        #region sever
//        ///响应队伍信息(S->C)
//        private bool LOBBY2CLIENT_TeamInfoResp_handler(Observers.Interfaces.INotification note)
//        {
//            //TeamInfoResp resp = note.Body as TeamInfoResp;
//
//            AllFormationInitInLua();
//            //			TrainFormationProxy.instance.trainPointPurcasedTimes = resp.bringUpPointPurcasedTimes;
//            return true;
//        }
//        ///响应阵型升级(S->C)
//        private bool LOBBY2CLIENT_LineupUpgradeResp_handler(Observers.Interfaces.INotification note)
//        {
//            LineupUpgradeResp resp = note.Body as LineupUpgradeResp;
//            FormationInfo info = FormationProxy.instance.GetFormationInfo(resp.no);
//            info.level = resp.lv;
//			FormationModelTable.GetLuaFunction("SetFormationInfo").Call(info.id,info.level,(int)info.formationState);
//            TrainFormationProxy.instance.UpdateUpgradeFormationByProtocol();
//            return true;
//        }
//        ///响应新增阵型(S->C)
//        private bool LOBBY2CLIENT_LineupAddResp_handler(Observers.Interfaces.INotification note)
//        {
//            LineupAddResp resp = note.Body as LineupAddResp;
//            LineupProtoData data;
//
//			for (int i = 0, count = resp.newLineupList.Count; i < count; i++)
//            {
//                data = resp.newLineupList[i];
//                FormationInfo info = FormationProxy.instance.GetFormationInfo(data.no);
//                info.Update(data);
//				FormationModelTable.GetLuaFunction("SetFormationInfo").Call(info.id,info.level,(int)info.formationState);
//				//Debugger.LogError("[LineupAddResp]id:{0},level:{1},state:{2}",info.id,info.level,info.formationState);
//                TrainFormationProxy.instance.AddNewFormationTip(data.no);
//            }
//            RedPointProxy.instance.Refresh();
//
//            return true;
//        }
//        ///响应新增队伍(S->C)
//        private bool LOBBY2CLIENT_TeamAddResp_handler(Observers.Interfaces.INotification note)
//        {
//            //TeamAddResp resp = note.Body as TeamAddResp;
//
//            AllFormationInitInLua();
//            return true;
//        }
//        ///响应购买培养点(S->C)
//        private bool LOBBY2CLIENT_LineupBringUpPointResp_handler(Observers.Interfaces.INotification note)
//        {
//            LineupPointBuyResp resp = note.Body as LineupPointBuyResp;
//            TrainFormationProxy.instance.trainPointPurcasedTimes = resp.bringUpPointPurcasedTimes;
//
//            TrainFormationProxy.instance.UpdateBuyTrainPointByProtocol();
//            return true;
//        }
//        ///响应培养点同步(S->C)
//        private bool LOBBY2CLIENT_LineupBringUpPointSynResp_handler(Observers.Interfaces.INotification note)
//        {
//            LineupPointSynResp resp = note.Body as LineupPointSynResp;
//            TrainFormationProxy.instance.UpdateTrainPointByProtocol(resp.point, resp.recoverUpperLimit, resp.nextRecoverTime);
//            return true;
//        }
//
//        private bool LOBBY2CLIENT_LineupAttrActiveResp_handler(Observers.Interfaces.INotification note)
//        {
//            IntProto resp = note.Body as IntProto;
//            int formationId = resp.value;
//            FormationProxy.instance.SetAdditionFormationAttrActive(formationId, true);
//            TrainFormationProxy.instance.UpdateAdditionFormationAttrActiveByProtocol();
//            LuaScriptMgr.Instance.GetLuaTable("gamemanager.GetModel");
//            LuaTable formationModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0];
//            formationModelLuaTable.GetLuaFunction("SetAdditionFormationAttrActive").Call(formationId,true);
//            return true;
//        }
//        #endregion
		private bool InitAllTeamAndFormationFromLua(Observers.Interfaces.INotification note)
		{
			AllFormationInitInLua();
			return true;
		}
    }
}