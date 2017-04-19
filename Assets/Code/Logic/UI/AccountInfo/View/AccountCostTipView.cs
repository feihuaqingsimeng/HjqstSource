using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.ConsumeTip.Model;

namespace Logic.UI.AccountInfo.View
{
	public class AccountCostTipView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/account_info/account_cost_tip_view";
		public static AccountCostTipView Open()
		{
			AccountCostTipView view = UIMgr.instance.Open<AccountCostTipView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
            return view;
        }
        
		public ScrollContentExpand scrollContent;

		List<ConsumeTipData> _consumeTipList ; 

		void Start()
		{
			Refresh();
		}
		private void Refresh()
		{
			_consumeTipList = ConsumeTipData.ConsumeTipDataDictionary.GetValues();
			scrollContent.Init(_consumeTipList.Count);
		}

		public void ClickSureBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
			AccountInfoView.Open();
		}
		public void OnResetItemHandler(GameObject go,int index)
		{
			CostTipButton btn = go.GetComponent<CostTipButton>();
			btn.SetType(_consumeTipList[index].consumeTipType);
		}
    }
}

