using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.Friend.Model;
using System.Collections.Generic;
using Logic.Enums;
using LuaInterface;
using Logic.Fight.Controller;
using Logic.UI.FightResult.Model;

namespace Logic.UI.Friend.Controller
{
    public class FriendController : SingletonMono<FriendController>
    {

        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendListResp).ToString(), LOBBY2CLIENT_FriendListResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendAddResp).ToString(), LOBBY2CLIENT_FriendAddResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendAddAnswerResp).ToString(), LOBBY2CLIENT_FriendAddAnswerResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendDelResp).ToString(), LOBBY2CLIENT_FriendDelResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendPresentVimResp).ToString(), LOBBY2CLIENT_FriendPresentVimResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendGetVimResp).ToString(), LOBBY2CLIENT_FriendGetVimResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendGiftBagResp).ToString(), LOBBY2CLIENT_FriendGiftBagResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendLookUpTeamResp).ToString(), LOBBY2CLIENT_FriendLookUpTeamResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendMsgListResp).ToString(), LOBBY2CLIENT_FriendMsgListResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendRecommendListResp).ToString(), LOBBY2CLIENT_FriendRecommendListResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.FriendFightResp).ToString(), LOBBY2CLIENT_FRIEND_FIGHT_RESP_HANDLER);
        }
        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendListResp).ToString(), LOBBY2CLIENT_FriendListResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendAddResp).ToString(), LOBBY2CLIENT_FriendAddResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendAddAnswerResp).ToString(), LOBBY2CLIENT_FriendAddAnswerResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendDelResp).ToString(), LOBBY2CLIENT_FriendDelResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendPresentVimResp).ToString(), LOBBY2CLIENT_FriendPresentVimResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendGetVimResp).ToString(), LOBBY2CLIENT_FriendGetVimResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendGiftBagResp).ToString(), LOBBY2CLIENT_FriendGiftBagResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendMsgListResp).ToString(), LOBBY2CLIENT_FriendMsgListResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendRecommendListResp).ToString(), LOBBY2CLIENT_FriendRecommendListResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.FriendFightResp).ToString(), LOBBY2CLIENT_FRIEND_FIGHT_RESP_HANDLER);
            }
        }


        private int GetSucFuncId(int subFuncId)
        {
            return (int)FunctionOpenType.MainView_Friend + subFuncId;
        }

        #region client
        ///请求好友列表(C->S)
        public void CLIENT2LOBBY_FriendListReq_REQ()
        {
            FriendListReq req = new FriendListReq();
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求添加好友(C->S)
        private int _friendAddResposeId = 0;
        public void CLIENT2LOBBY_FriendAddReq_REQ(string roleName, int id = 0)
        {
            _friendAddResposeId = id;
            FriendAddReq req = new FriendAddReq();
            req.roleName = roleName;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求添加好友应答(C->S)
        public void CLIENT2LOBBY_FriendAddAnswerReq_REQ(bool isOneKeyGet, int id = 0, bool isAgree = true)
        {
            FriendAddAnswerReq req = new FriendAddAnswerReq();
            req.isClick = isOneKeyGet;
            req.id = id;
            req.isAgree = isAgree;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///响应删除好友(S->C)
        public void CLIENT2LOBBY_FriendDelReq_REQ(int id)
        {
            FriendDelReq req = new FriendDelReq();
            req.id = id;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求赠送行动力(C->S)(-1：全部赠送 )
        public void CLIENT2LOBBY_FriendPresentVimReq_REQ(int id)
        {
            FriendPresentVimReq req = new FriendPresentVimReq();
            req.id = id;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求领取行动力(C->S)subfuncId:当前所在界面功能id(一键领取时需要传1,2,3) id:好友角色id,单个领取时
        public void CLIENT2LOBBY_FriendGetVimReq_REQ(bool isOneKeyGet, int subfuncId, int id)
        {
            FriendGetVimReq req = new FriendGetVimReq();
            req.isClick = isOneKeyGet;
            req.subFuncId = GetSucFuncId(subfuncId);
            req.id = id;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求领取好友礼包(C->S)
        public void CLIENT2LOBBY_FriendGiftBagReq_REQ()
        {
            FriendGiftBagReq req = new FriendGiftBagReq();
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求查看好友阵型(C->S)subfuncId:当前所在界面功能id(一键领取时需要传1,2,3)
        public void CLIENT2LOBBY_FriendLookUpTeamReq_REQ(int id, int subfuncId)
        {
            FriendLookUpTeamReq req = new FriendLookUpTeamReq();
            req.id = id;
            req.subFuncId = GetSucFuncId(subfuncId);
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请请求好友消息列表(C->S)
        public void CLIENT2LOBBY_FriendMsgListReq_REQ()
        {
            FriendMsgListReq req = new FriendMsgListReq();
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        ///请求推荐好友列表(C->S)
        public void CLIENT2LOBBY_FriendRecommendListReq_REQ()
        {
            FriendRecommendListReq req = new FriendRecommendListReq();
            req.isRefresh = true;
            Protocol.ProtocolProxy.instance.SendProtocol(req);
        }

        public void CLIENT2LOBBY_FriendFightReq(int friendId) 
        {
            IntProto ip = new IntProto();
            ip.value = friendId;
            Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.FriendFightReq,ip);
        }
        #endregion

        #region server
        ///响应好友列表(S->C)
        private bool LOBBY2CLIENT_FriendListResp_handler(Observers.Interfaces.INotification note)
        {
            FriendListResp resp = note.Body as FriendListResp;
            FriendProxy.instance.maxMyFriendCount = resp.friendListNumUpLimit;
            FriendProxy.instance.alreadyGetPveActionCount = resp.getFriendVimTimes;
            FriendProxy.instance.getPveActionTotalTimes = resp.totalGetVimTimes;
            FriendProxy.instance.AddAllFriendList(1, resp.friendList);
            FriendProxy.instance.InitCompleteRefreshByProtocol(1);

            if (resp.friendList.Count > 0)
            {
                for (int i = 0; i < resp.friendList.Count; i++)
                {
                    var d = resp.friendList[i];
                    var f = string.Format("{0}:{1}:{2}:{3}:{4}", d.id, d.roleName, d.lv, d.headNo, d.lastLoginTime == -1 ? 1 : 0);
                    LuaTable friendTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "chat_model")[0];
                    friendTable.GetLuaFunction("AddFriendToList").Call(f);
                }
            }

            return true;
        }

        ///响应添加好友(S->C)

        private bool LOBBY2CLIENT_FriendAddResp_handler(Observers.Interfaces.INotification note)
        {

            //	CLIENT2LOBBY_FriendRecommendListReq_REQ();
            FriendAddResp resp = note.Body as FriendAddResp;
            if (resp.newFriend == null)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Common.Localization.Localization.Get("ui.friendView.friendAddSend"));
                FriendProxy.instance.RemoveFriend(2, _friendAddResposeId);
                FriendProxy.instance.RefreshOneByProtocol(_friendAddResposeId);
            }
            else
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Common.Localization.Localization.Get("ui.friendView.friendAddSuccess"));
                FriendProxy.instance.AddFriend(1, resp.newFriend);
                FriendProxy.instance.RemoveFriend(2, resp.newFriend.id);
                FriendProxy.instance.RemoveFriend(3, resp.newFriend.id);
                FriendProxy.instance.RefreshOneByProtocol(resp.newFriend.id);
            }
            LuaTable friendTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "friend_controller")[0];
            friendTable.GetLuaFunction("FriendAddRespByCSharp").Call(resp.newFriend == null ? 0 : 1);
            //			FriendProxy.instance.AddFriend(1,resp.newFriend);
            //			FriendProxy.instance.RemoveFriend(2,resp.newFriend.id);
            //			FriendProxy.instance.RemoveFriend(3,resp.newFriend.id);
            //			FriendProxy.instance.RefreshOneByProtocol(resp.newFriend.id);
            //			if(_isAssignAddFriend)
            //			{
            //				_isAssignAddFriend = false;
            //				Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Common.Localization.Localization.Get( "ui.friendView.friendAddSuccess"));
            //			}
            return true;
        }
        ///响应添加好友应答(S->C)
        private bool LOBBY2CLIENT_FriendAddAnswerResp_handler(Observers.Interfaces.INotification note)
        {
            FriendAddAnswerResp resp = note.Body as FriendAddAnswerResp;

            FriendInfo info;

            for (int i = 0, count = resp.newFriendId.Count; i < count; i++)
            {
                int id = resp.newFriendId[i];
                if (!FriendProxy.instance.requestFriendListDic.ContainsKey(id))
                {
                    Debugger.LogError("requestFriendListDic has not found server answerid:" + id);
                    continue;
                }

                info = FriendProxy.instance.requestFriendListDic[id];
                info.isBothAuth = true;
                if (resp.isAgree)
                {
                    FriendProxy.instance.AddFriend(1, info);
                }
                FriendProxy.instance.RemoveFriend(2, id);
                FriendProxy.instance.RemoveFriend(3, id);
            }

            for (int i = 0; i < resp.newFriendList.Count; i++)
            {
                var d = resp.newFriendList[i];
                var f = string.Format("{0}:{1}:{2}:{3}:{4}", d.id, d.roleName, d.lv, d.headNo, d.lastLoginTime == -1 ? 1 : 0);
                LuaTable friendTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "chat_model")[0];
                friendTable.GetLuaFunction("AddFriendToList").Call(f);
            }

            if (resp.isClick)
            {
//                FriendProxy.instance.requestFriendListDic.Clear();
//                FriendProxy.instance.RefreshAllByProtocol();
				CLIENT2LOBBY_FriendMsgListReq_REQ();
            }
            else
            {
                int id = resp.isAgree ? resp.newFriendId[0] : resp.rejectId;
                FriendProxy.instance.RemoveFriend(3, id);
                FriendProxy.instance.RefreshOneByProtocol(id);
            }

            return true;
        }
        ///响应删除好友(S->C)
        private bool LOBBY2CLIENT_FriendDelResp_handler(Observers.Interfaces.INotification note)
        {
            FriendDelResp resp = note.Body as FriendDelResp;
            FriendProxy.instance.RemoveFriend(1, resp.id);
            FriendProxy.instance.RefreshOneByProtocol(resp.id);

            LuaTable friendTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "chat_model")[0];
            friendTable.GetLuaFunction("RemoveFriendToList").Call(resp.id);
            return true;
        }
        ///响应赠送行动力(S->C)
        private bool LOBBY2CLIENT_FriendPresentVimResp_handler(Observers.Interfaces.INotification note)
        {
            FriendPresentVimResp resp = note.Body as FriendPresentVimResp;
			for(int i = 0,count = resp.id.Count;i<count;i++)
			{
				FriendInfo info = FriendProxy.instance.myFriendListDic.GetValue(resp.id[i]);
				if (info != null)
				{
					info.isDonate = true;
					LuaTable friendInfo = (LuaTable)FriendProxy.instance.FriendModelLua.GetLuaFunction("GetFriendByType").Call(1,info.id)[0];
					friendInfo.GetLuaFunction("SetDonate").Call(friendInfo,true);
				}
				else
				{
					Debugger.LogError("赠送体力返回消息找不到好友 ，id:" + resp.id);
				}
			}
			FriendProxy.instance.RefreshAllByProtocol();
            return true;
        }
        ///响应领取行动力(S->C)
        private bool LOBBY2CLIENT_FriendGetVimResp_handler(Observers.Interfaces.INotification note)
        {
            FriendGetVimResp resp = note.Body as FriendGetVimResp;

            Dictionary<int, FriendInfo> friendDic = FriendProxy.instance.FriendDic[resp.subFuncId - (int)FunctionOpenType.MainView_Friend];
            for (int i = 0, count = resp.friendIdList.Count; i < count; i++)
            {
                int id = resp.friendIdList[i];
                if (friendDic.ContainsKey(id))
                    friendDic[id].isGetPveAction = true;
            }
            FriendProxy.instance.GetFriendPveActionRefreshByProtocol(resp.friendIdList);
            return true;
        }
        ///响应领取好友礼包(S->C)
        private bool LOBBY2CLIENT_FriendGiftBagResp_handler(Observers.Interfaces.INotification note)
        {
            FriendGiftBagResp resp = note.Body as FriendGiftBagResp;
            FriendProxy.instance.getPveActionTotalTimes = resp.remainTimes;
            FriendProxy.instance.GetFriendGiftBagRefreshByProtocol();
            return true;
        }
        ///响应查看好友阵型(S->C)
        private bool LOBBY2CLIENT_FriendLookUpTeamResp_handler(Observers.Interfaces.INotification note)
        {
//            FriendLookUpTeamResp resp = note.Body as FriendLookUpTeamResp;
//            FriendProxy.instance.UpdateFriendTeam(resp.id, resp.team);
//            FriendProxy.instance.LookupFriendTeamByProtocol(resp.id);
            return true;
        }
        ///响应好友请求列表(S->C)
        private bool LOBBY2CLIENT_FriendMsgListResp_handler(Observers.Interfaces.INotification note)
        {
            FriendMsgListResp resp = note.Body as FriendMsgListResp;
            FriendProxy.instance.AddAllFriendList(3, resp.friendMsgList);
            FriendProxy.instance.InitCompleteRefreshByProtocol(3);
            return true;
        }
        ///响应推荐好友列表(S->C)
        private bool LOBBY2CLIENT_FriendRecommendListResp_handler(Observers.Interfaces.INotification note)
        {
            FriendRecommendListResp resp = note.Body as FriendRecommendListResp;
            FriendProxy.instance.AddAllFriendList(2, resp.friendList);
            //			if (_friendAddResposeId != 0) 
            //			{
            //				FriendProxy.instance.RefreshAllByProtocol();
            //				_friendAddResposeId = 0;
            //			}else
            //			{
            FriendProxy.instance.InitCompleteRefreshByProtocol(2);
            //			}

            return true;
        }
        //好友挑战
        private bool LOBBY2CLIENT_FRIEND_FIGHT_RESP_HANDLER(Observers.Interfaces.INotification note)
        {
            FriendFightResp resp = note.Body as FriendFightResp;
            Logic.Fight.Model.FightProxy.instance.SetData(resp.selfTeamData, resp.opponentTeamData);

            FightController.instance.fightType = FightType.FriendFight;
            FightController.instance.PreReadyFight();
            return true;
        }
        #endregion
    }
}

