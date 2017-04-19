using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.UI.SignIn.Model;
using Logic.UI.SignIn.Controller;
using Common.Localization;
using Logic.VIP.Model;
using Logic.Game.Model;
using Common.GameTime.Controller;
using LuaInterface;

namespace Logic.UI.SignIn.View
{
	public class SignInView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/sign_in/signin_view";
		public static SignInView Open()
		{
			SignInView view = UIMgr.instance.Open<SignInView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		public ScrollContentExpand scrollContent;
		public GameObject signBtnGO;
		public Text signCountText;
		public Text signBtnText;

		void Start () 
		{
			string mark = string.Format("openSignDay{0}",GameProxy.instance.AccountId);
			int day = PlayerPrefs.GetInt(mark);
			int nowDay = TimeController.instance.ServerTime.DayOfYear;
			PlayerPrefs.SetInt(mark,nowDay);
			PlayerPrefs.Save();
			BindDelegate();
			Init();

		}
		void OnDestroy()
		{
			UnbindDelegate();
		}
		private void BindDelegate()
		{
			SignInProxy.instance.RefreshSignDelegate += RefreshSignByProtocol;
		}
		private void UnbindDelegate()
		{
			SignInProxy.instance.RefreshSignDelegate -= RefreshSignByProtocol;
		}
		private void Init()
		{
			scrollContent.Init(SignInProxy.instance.SignInList.Count);
			Refresh();
		}
		private void Refresh()
		{
			UIUtil.SetGray(signBtnGO,SignInProxy.instance.isSignInToday);
			signCountText.text = string.Format(Localization.Get("ui.signInView.signCount"),SignInProxy.instance.lastSignInId);
			signBtnText.text = SignInProxy.instance.isSignInToday ? Localization.Get("ui.signInView.signAlready") : Localization.Get("ui.signInView.sign") ;
			Outline outline = signBtnText.GetComponent<Outline>();
			if(outline != null)
				outline.enabled = !SignInProxy.instance.isSignInToday;
		}

		public void OnClickSignBtnHandler()
		{
			if(SignInProxy.instance.isSignInToday)
			{
				return;
			}
			SignInController.instance.CLIENT2LOBBY_SignInReq();
		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void OnResetItemHandler(GameObject go, int index)
		{
			SignInButton btn = go.GetComponent<SignInButton>();
			SignInInfo info = SignInProxy.instance.SignInList[index];
			btn.Set(info);
		}
		private void RefreshSignByProtocol(int signId)
		{
			scrollContent.RefreshAllContentItems();
			Refresh();
			List<SignInInfo> signList = SignInProxy.instance.SignInList;
			SignInInfo info = null;
			for(int i = 0,count = signList.Count;i<count;i++)
			{
				info = signList[i];
				if(info.id == signId)
				{
					List<GameResData> dataList = new List<GameResData>();
					dataList.AddRange(info.signData.awardItemList);
					if(VIPProxy.instance.VIPLevel >= info.signData.vip_lv)
					{
						for(int j = 1,count2 = info.signData.vip_multiple ;j < count2 ;j++)
						{
							dataList.AddRange(info.signData.awardItemList);
						}
					}
					//Logic.UI.Pvp.View.PvpGainRewardView.Open(dataList,true);
					//Logic.UI.Tips.View.CommonRewardAutoDestroyTipsView.Open(dataList,true);
					LuaCsTransfer.OpenRewardTipsView(UIUtil.CombineGameResList(dataList));
					break;
				}
			}


        }
	}
}

