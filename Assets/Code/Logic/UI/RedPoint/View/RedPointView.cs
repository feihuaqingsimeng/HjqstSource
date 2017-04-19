using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Enums;
using Logic.UI.RedPoint.Model;
using Logic.FunctionOpen.Model;
using Logic.UI.Task.Model;
using Logic.UI.Mail.Model;
using Logic.UI.Friend.Model;
using Logic.UI.TrainFormation.Model;
using LuaInterface;

namespace Logic.UI.RedPoint.View
{
    public class RedPointView : MonoBehaviour
    {
		[NoToLua]
        public RedPointType type;
		//[HideInInspector]
		public int id;
		[NoToLua]
		public Animator animator;
		[NoToLua]
		public RuntimeAnimatorController defaultController;
		[NoToLua]
		public RuntimeAnimatorController activeController;
		public int[] luaType;
        

        void Awake()
        {

            RedPointProxy.instance.OnRefreshDelegate += Refresh;
			RedPointProxy.instance.OnRefreshSpecificDelegate += RefreshSpecific;
        }
        void Start()
        {

            Refresh();
        }
        void OnDestroy()
        {
            RedPointProxy.instance.OnRefreshDelegate -= Refresh;
			RedPointProxy.instance.OnRefreshSpecificDelegate -= RefreshSpecific;
        }
		private LuaTable _luaRedPointModel;
		private LuaTable LuaRedPointModel
		{
			get
			{
				if(_luaRedPointModel == null)
					_luaRedPointModel = (LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","red_point_model")[0];
				return _luaRedPointModel;
			}
		}
		[NoToLua]
        public void Set(RedPointType type, int id = 0)
        {
            this.type = type;
            this.id = id;
            Refresh();
        }
		public void SetId(int id)
		{
			this.id = id;
			Refresh();
		}
		//刷新特定的红点type
		private void RefreshSpecific(int type)
		{
			bool needRefresh = false;
			if ((int)this.type == type)
			{
				Refresh();
			}else
			{
				for(int i = 0,count = luaType.Length;i<count;i++)
				{
					if(luaType[i] == type)
					{
						Refresh();
						break;
					}
				}
			}
		}
        private void Refresh()
        {
            int num = 0;
            switch (type)
            {
                case RedPointType.RedPoint_TaskAll:
                    num = TaskProxy.instance.GetAllCompleteTaskCount();
                    break;
                case RedPointType.RedPoint_TaskAchievement:
                    num = TaskProxy.instance.GetCompleteAchievementTaskCount();
                    break;
                case RedPointType.RedPoint_TaskDaily:
                    num = TaskProxy.instance.GetCompleteDailyTaskCount();
                    break;
                case RedPointType.RedPoint_TaskMain:
                    num = TaskProxy.instance.GetCompleteMainTaskCount();
                    break;
                case RedPointType.RedPoint_Mail:
                    num = MailProxy.instance.GetNotGetRewardMailCount();
                    break;
                case RedPointType.RedPoint_FriendAll:
                    num = FriendProxy.instance.GetNewFriendListCountByRedPoint() + FriendProxy.instance.GetNewFriendRequestCountByRedPoint();
                    break;
                case RedPointType.RedPoint_MyFriend:
                    num = FriendProxy.instance.GetNewFriendListCountByRedPoint();
                    break;
                case RedPointType.RedPoint_FriendRequest:
                    num = FriendProxy.instance.GetNewFriendRequestCountByRedPoint();
                    break;
                
                case RedPointType.RedPoint_Hero_New:
                    num = Logic.Hero.Model.HeroProxy.instance.HasNewHero() ? 1 : 0;
                    break;
                case RedPointType.RedPoint_VIP_Benefit:
                    num = Logic.VIP.Model.VIPProxy.instance.VIPLevel + 1 - Logic.VIP.Model.VIPProxy.instance.HasReceivedGiftVIPLevelList.Count;
                    break;
                case RedPointType.RedPoint_Shop_Has_Free_Item:
                    num = Logic.UI.Shop.Model.ShopProxy.instance.GetCurrentFreeItemsCount();
                    break;
                case RedPointType.RedPoint_Activity:
					if(LuaInterface.LuaScriptMgr.Instance != null)
					{
						var activity = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "activitymodel")[0];
						var hasnew = (bool)activity.GetLuaFunction("HasNew").Call(1)[0];
						num = hasnew ? 1 : 0;
                    }
                    break;
                case RedPointType.RedPoint_Chat:
					if(LuaInterface.LuaScriptMgr.Instance != null)
					{
						var chat = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "chat_model")[0];
						var hasnew1 = (bool)chat.GetLuaFunction("HasNew").Call(1)[0];
						num = hasnew1 ? 1 : 0;
                        transform.parent.GetComponent<Animator>().enabled = hasnew1;
                        if(!hasnew1)
                        {
                            transform.parent.localScale = Vector3.one;
                        }
                    }
                    break;
				case RedPointType.RedPoint_Explore_Has_Unhandle_Task:
					LuaTable exploreModelLuaTable = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "exploremodel")[0];
					bool hasUnhandledTask = exploreModelLuaTable.GetLuaFunction("HasUnhandledTask").Call(null)[0].ToString().ToBoolean();
					num = hasUnhandledTask ? 1 : 0;
					break;
				case RedPointType.RedPoint_Have_Not_Challenge_Any_Daily_Dungeon_Today:
					num = Activity.Model.ActivityProxy.instance.HaveNotChallengeAnyActivityToday() ? 1 : 0;
					break;
            }
			if (luaType.Length > 0)
			{
				bool hasRed = LuaRefreshRedPoint() ;
				if( hasRed) num = 1;
			}
			if(num > 0 ) 
			{
				if(animator != null)
				{
					animator.runtimeAnimatorController = activeController;
				}
				gameObject.SetActive(true);
			}else
			{
				if(animator != null)
				{
					animator.runtimeAnimatorController = defaultController;
				}
				gameObject.SetActive(false);
			}

        }
		private bool LuaRefreshRedPoint()
		{
			if(LuaInterface.LuaScriptMgr.Instance != null)
			{
				return LuaRedPointModel.GetLuaFunction("RefreshCountByCSharp").Call(luaType,id)[0].ToString().ToBoolean();
			}
			return false;
		}
    }
}

