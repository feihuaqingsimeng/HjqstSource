using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.RandomGift.Model;
using Logic.Item.Model;
using Common.Localization;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.UI.RandomGift.Controller;

namespace Logic.UI.RandomGift.View
{
	public class RandomGiftView : MonoBehaviour 
	{
		

		#region ui component
		public Text textName;
		#endregion

		void Start () {
			BindDelegate();
			Refresh();
		}
		void OnDestroy()
		{
			UnbindDelegate();
		}
		private void BindDelegate()
		{
			RandomGiftProxy.instance.GetRandomGiftRefreshDelegate += GetRandomGiftRewardByProtocol;
			RandomGiftProxy.instance.RefreshDelegate += Refresh;
		}
		private void UnbindDelegate()
		{
			RandomGiftProxy.instance.GetRandomGiftRefreshDelegate -= GetRandomGiftRewardByProtocol;
			RandomGiftProxy.instance.RefreshDelegate -= Refresh;
			
		}
		private void Refresh()
		{

			if(RandomGiftProxy.instance.RandomGiftDictionary.Count == 0)
			{
				gameObject.SetActive(false);
			}else
			{
				ItemInfo info = RandomGiftProxy.instance.RandomGiftDictionary.First().Value;
				gameObject.SetActive(true);
				textName.text = Localization.Get( info.itemData.name);
            }
		}
		public void OnClickRandomGiftBtnHandler()
		{
			if(RandomGiftProxy.instance.RandomGiftDictionary.Count == 0)
				return;
			ItemInfo info = RandomGiftProxy.instance.RandomGiftDictionary.First().Value;
			RandomGiftController.instance.CLIENT2LOBBY_OPEN_RANDOM_GIFT_REQ(info.itemData.id);
			Debugger.Log("打开礼包id:"+info.itemData.id);
		}
		#region refresh by server
		private void GetRandomGiftRewardByProtocol(List<GameResData> dataList)
		{
			Refresh();
			if(dataList.Count != 0)
				Logic.UI.Tips.View.CommonRewardAutoDestroyTipsView.Open(dataList);
			else
				Debugger.LogError("礼包内奖励数量为0");
		}
		#endregion
	}
}

