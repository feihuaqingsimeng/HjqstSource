using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.Friend.Model;
using Common.Localization;
using Common.Util;
using Common.GameTime.Controller;
using Common.ResMgr;
using Logic.Hero.Model;
using Logic.UI.Friend.Controller;
using Logic.UI.Tips.View;
using LuaInterface;
using Logic.Enums;

namespace Logic.UI.Friend.View
{
    public class FriendButton : MonoBehaviour
    {

        #region ui component
        public Image imageIcon;
        public Text textLevel;
        public Text textName;
        public Text textPower;
		public Text textVip;
        public Image imageRelationship;
        public Button btnCheck;
        public Button btnChallenge;
        public GameObject[] LineGO;
        public Text textLastLogin;
        public GameObject DonateAlreadyGO;
        public GameObject DonateGO;
        public GameObject DeleteGO;
        public GameObject AddGO;
        public GameObject acceptGO;
        public GameObject refuseGO;
        public GameObject getPveActionGO;

        private Sprite _imageBothAuth;
        private Sprite _imageSingleAuth;
        #endregion


        private FriendInfo _friendInfo;
        private int _type;

        void Awake()
        {
            _imageBothAuth = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_friendly_3");
            _imageSingleAuth = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_friendly_2");
        }

        //type : 1、好友列表 2、好友推荐  3、好友请求
        public void SetFriendInfo(FriendInfo info, int type)
        {
            _friendInfo = info;
            _type = type;
            Refresh();
        }
        private void Refresh()
        {
            ShowComponent();
            textLevel.text = string.Format(Localization.Get("ui.friendView.friendLv"), _friendInfo.level);
            textName.text = _friendInfo.name;
            textPower.text = string.Format(Localization.Get("ui.friendView.friendPower"), _friendInfo.power);
            textLastLogin.text = GetLastLoginTimeString();
			textVip.text = _friendInfo.vip.ToString();
			imageIcon.SetSprite(ResMgr.instance.Load<Sprite>(_friendInfo.headIcon));
            imageRelationship.gameObject.SetActive(_type != 2);
            if (_friendInfo.isBothAuth)
            {
				imageRelationship.SetSprite(_imageBothAuth);
            }
            else
            {
				imageRelationship.SetSprite( _imageSingleAuth);
            }

        }
        private string GetLastLoginTimeString()
        {
            if (_friendInfo.lastLoginTime == -1)
            {
                return TimeUtil.FormatLastLoginTimeString(_friendInfo.lastLoginTime);
            }
            else
            {
                return string.Format(Localization.Get("ui.friendView.friendLastLogin"), TimeUtil.FormatLastLoginTimeString(_friendInfo.lastLoginTime));

            }
        }
        private void ShowComponent()
        {
            for (int i = 0; i < LineGO.Length; i++)
            {
                LineGO[i].SetActive(_type == 1);//1显示
            }
            btnCheck.gameObject.SetActive(_type == 1);
            btnChallenge.gameObject.SetActive(_type == 1);
            DonateAlreadyGO.SetActive(_type == 1);//1显示
            DonateGO.SetActive(_type == 1);//1显示
            getPveActionGO.SetActive(_type != 2);//1,3显示
            AddGO.SetActive(_type == 2);//2显示
            acceptGO.SetActive(_type == 3);//3显示
            DeleteGO.SetActive(_type == 1);//1显示
            refuseGO.SetActive(_type == 3);//3显示
            textLastLogin.gameObject.SetActive(_type == 1);//1显示

            if (_type == 1)
            {
                DonateAlreadyGO.SetActive(_friendInfo.isGetPveAction && _friendInfo.isDonate);
                DonateGO.SetActive(_friendInfo.isGetPveAction && !_friendInfo.isDonate);
                getPveActionGO.SetActive(!_friendInfo.isGetPveAction);
            }
            else if (_type == 3)
            {
                getPveActionGO.SetActive(!_friendInfo.isGetPveAction);
            }
        }

        #region ui event handler
        //查看队伍信息
        public void OnClickCheckBtnHandler()
        {
            //FriendDetailView.Open(_friendInfo);
            //FriendController.instance.CLIENT2LOBBY_FriendLookUpTeamReq_REQ(_friendInfo.id, _type);
			LuaTable friendCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","friend_controller")[0];
			friendCtrlLua.GetLuaFunction("RoleInfoLookUpReq").Call(_friendInfo.id,FunctionOpenType.FightCenter_Arena);
        }
        //赠送
        public void OnClickDonateBtnHandler()
        {
            FriendController.instance.CLIENT2LOBBY_FriendPresentVimReq_REQ(_friendInfo.id);
        }
        //领取
        public void OnClickGetPveActionBtnHandler()
        {
            if (FriendProxy.instance.alreadyGetPveActionCount >= FriendProxy.instance.maxGetPveActionCount)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachPveActionLimit"));
                return;
            }
            FriendController.instance.CLIENT2LOBBY_FriendGetVimReq_REQ(false, _type, _friendInfo.id);
        }
        //添加
        public void OnClickAddBtnHandler()
        {
            if (FriendProxy.instance.myFriendListDic.Count >= FriendProxy.instance.maxMyFriendCount)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachMyFriendLimit"));
                return;
            }
            FriendController.instance.CLIENT2LOBBY_FriendAddReq_REQ(_friendInfo.name, _friendInfo.id);
        }
        //接受
        public void OnClickAcceptBtnHandler()
        {
            if (FriendProxy.instance.myFriendListDic.Count >= FriendProxy.instance.maxMyFriendCount)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachMyFriendLimit"));
                return;
            }
            FriendController.instance.CLIENT2LOBBY_FriendAddAnswerReq_REQ(false, _friendInfo.id, true);
        }
        //删除
        public void OnClickDeleteBtnHandler()
        {
            FriendController.instance.CLIENT2LOBBY_FriendDelReq_REQ(_friendInfo.id);
        }
        //拒绝
        public void OnClickRefuseBtnHandler()
        {
            FriendController.instance.CLIENT2LOBBY_FriendAddAnswerReq_REQ(false, _friendInfo.id, false);
        }

        public void OnClickChallengeBtnHandler()
        {
            FriendController.instance.CLIENT2LOBBY_FriendFightReq(_friendInfo.id);
        }
        #endregion
    }
}

