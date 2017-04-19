using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Localization;
using Logic.Enums;
using Logic.ConsumeTip.Model;

namespace Logic.UI.TrainFormation.View
{
	public class ComfirmBuyTrainPointView : MonoBehaviour 
	{

		public const string PREFAB_PATH = "ui/train_formation/comfirm_buy_train_point_view";
		public static ComfirmBuyTrainPointView Open(int costDiamond,int buyCount,int remaindCount,System.Action sureCallback,ConsumeTipType type)
		{
			ComfirmBuyTrainPointView view = UIMgr.instance.Open<ComfirmBuyTrainPointView>(PREFAB_PATH,EUISortingLayer.Tips);
			view.SetTip( costDiamond, buyCount, remaindCount,sureCallback,type);
			return view;
		}

		#region ui component
		public Text costDiamondText;
		public Text buyCountText;
		public Text remaindText;
		public Toggle toggleTip;
		#endregion

		private System.Action _sureCallback;
		private ConsumeTipType _consumeTipType;


		public void SetTip(int costDiamond,int buyCount,int remaindCount,System.Action sureCallback,ConsumeTipType type)
		{
			_sureCallback = sureCallback;
			costDiamondText.text = string.Format(Localization.Get("ui.train_buy_point_view.diamondCost"),costDiamond);
			buyCountText.text = string.Format(Localization.Get("ui.train_buy_point_view.buyNum"),buyCount);
			remaindText.text = string.Format(Localization.Get("ui.train_buy_point_view.remaindTime"),remaindCount);
			_consumeTipType = type;
			if(!ConsumeTipProxy.instance.HasConsumeTipKey(type))
			{
				toggleTip.gameObject.SetActive(false);
			}
		}

		public void ClickSureBtnHandler()
		{
			ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipType,!toggleTip.isOn);

			if(_sureCallback!= null)
				_sureCallback();

			ClickCloseBtnHandler();

		}

		public void ClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

