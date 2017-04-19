using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Friend.Model;
using Common.UI.Components;
using Logic.UI.CommonTopBar.View;
using Common.Localization;
using Logic.UI.Friend.Controller;
using Logic.Game.Model;
using Logic.Item.Model;
using Logic.Enums;
using Logic.UI.Tips.View;
using Logic.UI.CommonAnimations;
using LuaInterface;

namespace Logic.UI.Friend.View
{
    public class FriendView : MonoBehaviour
    {

        public const string PREFAB_PATH = "ui/friend/friend_view";
        public static FriendView Open()
        {
            FriendView view = UIMgr.instance.Open<FriendView>(PREFAB_PATH);
            return view;
        }

        #region ui component
        public Transform panel;
        public Text textGetPveActionCount;
        public Text textFriendCount;
        public Text textGetPveActionTimes;
        public Slider sliderGetPveActionTimes;
        public ScrollContentExpand scrollContent;
        public Toggle[] toggles;
        public GameObject rewardGO;
        public GameObject[] bottomBtnRoot;
        #endregion

        private Toggle _currentToggle;
        private List<FriendInfo> _currentFriendList;
        private int _currentToggleId = 1;//1好友列表  2好友推荐 3好友请求
        private bool _isFirstEnter = true;
        void Awake()
        {
            CommonTopBarView bar = CommonTopBarView.CreateNewAndAttachTo(panel);
            bar.SetAsCommonStyle(Localization.Get("ui.friendView.title"), OnCloseBtnHandler, true, true, true, true);
        }
        void Start()
        {
            BindDelegate();
            InitToggles();
            FriendProxy.instance.Clear();
            FriendController.instance.CLIENT2LOBBY_FriendMsgListReq_REQ();
            FriendController.instance.CLIENT2LOBBY_FriendRecommendListReq_REQ();
            FriendController.instance.CLIENT2LOBBY_FriendListReq_REQ();
            //oneKeyRootGO.AddComponent<CommonFadeInAnimation>().set(0.4f,0.8f);

        }

        void OnDestroy()
        {
            UnbindDelegate();
        }

        private void BindDelegate()
        {
            FriendProxy.instance.RefreshAllDelegate += RefreshByProtocol;
            FriendProxy.instance.RefreshOneDelegate += RefreshOneByProtocol;
            FriendProxy.instance.InitCompleteRefreshDelegate += InitCompleteRefreshByProtocol;
            FriendProxy.instance.GetFriendPveActionRefreshDelegate += GetFriendPveActionRefreshByProtocol;
            FriendProxy.instance.GetFriendGiftBagRefreshDelegate += GetFriendGiftBagRefreshByProtocol;
            FriendProxy.instance.LookupFriendTeamDelegate += LookupFriendTeamByProtocol;
        }
        private void UnbindDelegate()
        {
            FriendProxy.instance.RefreshAllDelegate -= RefreshByProtocol;
            FriendProxy.instance.RefreshOneDelegate -= RefreshOneByProtocol;
            FriendProxy.instance.InitCompleteRefreshDelegate -= InitCompleteRefreshByProtocol;
            FriendProxy.instance.GetFriendPveActionRefreshDelegate -= GetFriendPveActionRefreshByProtocol;
            FriendProxy.instance.GetFriendGiftBagRefreshDelegate -= GetFriendGiftBagRefreshByProtocol;
            FriendProxy.instance.LookupFriendTeamDelegate -= LookupFriendTeamByProtocol;
        }
        private void InitToggles()
        {
            for (int i = 0, count = toggles.Length; i < count; i++)
            {
                toggles[i].GetComponent<ToggleContent>().Set(i + 1, Localization.Get("ui.friendView.friend" + i.ToString()));
                toggles[i].isOn = i == 0 ? true : false;
                bottomBtnRoot[i].SetActive(i == 0);
            }

            FriendProxy.instance.NewFriendListComing = false;
            //_currentToggle = toggles[0];
        }
        private void Refresh(bool startAction)
        {
            _currentFriendList = FriendProxy.instance.GetFriendListByType(_currentToggleId);
            RefreshFriend(startAction);
            RefreshAreadyGetPveAction();
        }
        private void RefreshAreadyGetPveAction()
        {
			textGetPveActionCount.text = string.Format("{0}/{1}", FriendProxy.instance.alreadyGetPveActionCount*GlobalData.GetGlobalData().friendDonatePveActionPer, FriendProxy.instance.maxGetPveActionCount);
            textGetPveActionTimes.text = string.Format("{0}/{1}", FriendProxy.instance.getPveActionTotalTimes, FriendProxy.instance.getRewardNeedTimes);
            float percent = FriendProxy.instance.getPveActionTotalTimes / (FriendProxy.instance.getRewardNeedTimes + 0.0f);
            if (percent > 1)
                percent = 1;
            sliderGetPveActionTimes.value = percent;
        }
        private void RefreshFriend(bool startAction)
        {

            scrollContent.Init(_currentFriendList.Count, _isFirstEnter);
            _isFirstEnter = false;
            textFriendCount.text = string.Format(Localization.Get("ui.friendView.friendCount"), FriendProxy.instance.myFriendListDic.Count, FriendProxy.instance.maxMyFriendCount);
        }
        #region ui event handler
        public void OnCloseBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        public void OnClickToggleHandler(Toggle toggle)
        {
            //if(_currentToggle == toggle)
            //return;
            if (toggle.isOn)
            {

                FriendProxy friendProxy = FriendProxy.instance;
                ToggleContent tc = toggle.GetComponent<ToggleContent>();
                _currentToggleId = tc.id;

                if (_currentToggleId == 1 && friendProxy.NewFriendListComing)
                    FriendController.instance.CLIENT2LOBBY_FriendListReq_REQ();
                else if (_currentToggleId == 3 && friendProxy.NewFriendRequestComing)
                    FriendController.instance.CLIENT2LOBBY_FriendMsgListReq_REQ();
                else
                {
                    Refresh(true);
                }
                if (_currentToggleId == 1)
                {
                    FriendProxy.instance.NewFriendListComing = false;
                }
                else if (_currentToggleId == 3)
                {
                    FriendProxy.instance.NewFriendRequestComing = false;
                }
                for (int i = 0; i < bottomBtnRoot.Length; i++)
                {
                    bottomBtnRoot[i].SetActive(i == (_currentToggleId - 1));
                }
                _currentToggle = toggle;
            }
        }
        //一键赠送
        public void OnClickOneKeyToDonate()
        {
            bool canDonate = false;
            foreach (var value in FriendProxy.instance.myFriendListDic)
            {
                if (!value.Value.isDonate)
                {
                    canDonate = true;
                    break;
                }
            }
            if (canDonate)
                FriendController.instance.CLIENT2LOBBY_FriendPresentVimReq_REQ(-1);
            else
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.one_key_donate_already_tip"));
        }

        //一键领取
        public void OnClickOneKeyToGetPveAction()
        {

            Dictionary<int, FriendInfo> friendDic = FriendProxy.instance.FriendDic[_currentToggleId];
            bool hasGet = false;
            foreach (var value in friendDic)
            {
                if (!value.Value.isGetPveAction)
                {
                    hasGet = true;
                    break;
                }
            }
            if (!hasGet)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.noGet"));
                return;
            }
            if (FriendProxy.instance.alreadyGetPveActionCount >= FriendProxy.instance.maxGetPveActionCount)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachPveActionLimit"));
                return;
            }
            FriendController.instance.CLIENT2LOBBY_FriendGetVimReq_REQ(true, _currentToggleId, 0);
        }
        //领取奖励
        public void OnClickRewardBoxBtnHandler()
        {
            if (FriendProxy.instance.getPveActionTotalTimes < FriendProxy.instance.getRewardNeedTimes)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.not_enough_times"));
                return;
            }

            FriendController.instance.CLIENT2LOBBY_FriendGiftBagReq_REQ();
        }
        //指定添加
        public void OnClickAppointAddBtnHandler()
        {
            FriendAddView.Open();
        }
        //换一批
        public void OnClickExchangeAddBtnHandler()
        {
            FriendController.instance.CLIENT2LOBBY_FriendRecommendListReq_REQ();
        }
        //一键接受
        public void OnClickOneKeyToAcceptBtnHandler()
        {
            if (FriendProxy.instance.requestFriendListDic.Count == 0)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.noFriendRequest"));
                return;
            }

            if (FriendProxy.instance.myFriendListDic.Count >= FriendProxy.instance.maxMyFriendCount)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachMyFriendLimit"));
                return;
            }
            FriendController.instance.CLIENT2LOBBY_FriendAddAnswerReq_REQ(true, 0, true);
        }
        //一键拒绝
        public void OnClickOneKeyToRefuseBtnHandler()
        {
            if (FriendProxy.instance.requestFriendListDic.Count == 0)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.noFriendRequest"));
                return;
            }

            FriendController.instance.CLIENT2LOBBY_FriendAddAnswerReq_REQ(true, 0, false);
        }
        public void OnResetItemHandler(GameObject go, int index)
        {
            FriendButton fb = go.GetComponent<FriendButton>();
			if (index < _currentFriendList.Count) 
			{
				fb.SetFriendInfo(_currentFriendList[index], _currentToggleId);
			}else
			{
				Debugger.LogError("index out of range ,index"+ index + ",count:"+_currentFriendList.Count);
			}
            
        }
        #endregion

        #region update by protocol
        private void RefreshByProtocol()
        {
            Refresh(false);
        }
        private void InitCompleteRefreshByProtocol(int funcid)
        {
            if (funcid == _currentToggleId)
                Refresh(true);
        }
        private void RefreshOneByProtocol(int id)
        {
            //			int index = -1;
            //			for(int i = 0,count =_currentFriendList.Count;i<count;i++)
            //			{
            //				if(_currentFriendList[i].id == id)
            //				{
            //					index = i;
            //					break;
            //				}
            //			}
            _currentFriendList = FriendProxy.instance.GetFriendListByType(_currentToggleId);
            scrollContent.Init(_currentFriendList.Count);
            textFriendCount.text = string.Format(Localization.Get("ui.friendView.friendCount"), FriendProxy.instance.myFriendListDic.Count, FriendProxy.instance.maxMyFriendCount);
            RefreshAreadyGetPveAction();
            if (_currentToggleId == 2 && _currentFriendList.Count == 0)
            {
                FriendController.instance.CLIENT2LOBBY_FriendRecommendListReq_REQ();
            }
        }
        private void GetFriendPveActionRefreshByProtocol(int count)
        {
            List<GameResData> dataList = new List<GameResData>();
            dataList.Add(new GameResData(BaseResType.PveAction, 0, count * GlobalData.GetGlobalData().friendDonatePveActionPer, 0));
            Logic.UI.Tips.View.CommonRewardAutoDestroyTipsView.Open(dataList);
            Refresh(false);
        }
        private void GetFriendGiftBagRefreshByProtocol(List<GameResData> dataList)
        {
            RefreshAreadyGetPveAction();
            //Logic.UI.Tips.View.CommonRewardAutoDestroyTipsView.Open(dataList);
			LuaCsTransfer.OpenRewardTipsView(dataList);
        }
        private void LookupFriendTeamByProtocol(int id)
        {
            FriendInfo info = null;
            for (int i = 0, count = _currentFriendList.Count; i < count; i++)
            {
                info = _currentFriendList[i];
                if (info.id == id)
                {
                    FriendDetailView.Open(info);
                    break;
                }
            }
        }
        #endregion
    }

}
