using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Enums;
using Logic.ConsumeTip.Model;
using Common.Localization;

namespace Logic.UI.AccountInfo.View
{
	public class CostTipButton : MonoBehaviour 
	{

		public Text textTitle;
		public Toggle[] toggles;


		private ConsumeTipData _consumeTipData;
		public void SetType(ConsumeTipType type)
		{
			_consumeTipData = ConsumeTipData.GetConsumeTipDataByType(type);
			Refresh();
		}

		private void Refresh()
		{
			textTitle.text = Localization.Get(_consumeTipData.des);
			//0开 1关
			bool enable = ConsumeTipProxy.instance.GetConsumeTipEnable(_consumeTipData.consumeTipType);
			toggles[0].isOn = enable;
			toggles[1].isOn = !enable;
		}

		public void ClickToggleHandler(Toggle toggle)
		{
			if(toggle.isOn)
			{
				int index = toggles.IndexOf(toggle);
				ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipData.consumeTipType,index == 0);
			}
		}
	}
}

