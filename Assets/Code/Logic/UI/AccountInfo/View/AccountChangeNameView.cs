using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Game.Model;
using Logic.Enums;
using Common.Localization;
using Logic.UI.Login.Controller;

namespace Logic.UI.AccountInfo.View
{
	public class AccountChangeNameView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/account_info/account_change_name_view";
		public static AccountChangeNameView Open()
		{
			AccountChangeNameView view = UIMgr.instance.Open<AccountChangeNameView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		public InputField inputFieldName;
		public Text textCost;
		public Image imageCost;

		void Start()
		{
			int cost =  GlobalData.GetGlobalData().renameCostResData.count;
			textCost.text = cost > GameProxy.instance.BaseResourceDictionary.GetValue(GlobalData.GetGlobalData().renameCostResData.type) ? UIUtil.FormatToRedText(cost.ToString()) : cost.ToString();
			imageCost.SetSprite(Common.ResMgr.ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(GlobalData.GetGlobalData().renameCostResData.type)));
			GameProxy.instance.onAccountInfoUpdateDelegate += NameChangeSuccessHandler;
		}
		void OnDestroy()
		{
			GameProxy.instance.onAccountInfoUpdateDelegate -= NameChangeSuccessHandler;
		}
		public void ClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
			AccountInfoView.Open();
		}
		public void ClickSureBtnHandler()
		{
			string name = inputFieldName.text;
			if(string.IsNullOrEmpty(name))
			{
				return;
			}

			int playerNameByteLength = 0;
			char[] playerNameCharArray = name.ToCharArray();
			for (int i = 0; i < playerNameCharArray.Length; i++)
			{
				if ((int)playerNameCharArray[i] > 127)
				{
					playerNameByteLength += 2;     // Chinses
				}
				else
				{
					playerNameByteLength += 1;     // English and Number etc.
				}
			}
			
			if (playerNameByteLength > 14)
			{
				Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.create_role_view.player_name_too_long"));
				return;
			}
			
			bool hasBlockWord = Common.Util.BlackListWordUtil.HasBlockWords(name);
			if (hasBlockWord)
			{
				Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.create_role_view.hasBlockWords"));
				return;
			}

			int own = GameProxy.instance.BaseResourceDictionary.GetValue(GlobalData.GetGlobalData().renameCostResData.type);
			if(own < GlobalData.GetGlobalData().renameCostResData.count)
			{
				Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.common_tips.not_enough_diamond"));
				return;
			}
			LoginController.instance.CLIENT2LOBBY_ROLE_NAME_REQ(name);
		}
		public void NameChangeSuccessHandler()
		{
			ClickCloseBtnHandler();
		}
	}
}

