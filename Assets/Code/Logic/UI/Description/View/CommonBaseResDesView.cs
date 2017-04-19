using UnityEngine;
using System.Collections;
using Logic.Item.Model;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.CommonItem.View;
using System.Collections.Generic;
using Logic.Enums;
using Logic.UI.GoodsJump.Model;
using Logic.Dungeon.Model;
using Logic.Game.Model;
using System.Text;
using Logic.UI.WorldTree.Model;
using Common.GameTime.Controller;
using Common.Util;

namespace Logic.UI.Description.View
{
	public class CommonBaseResDesView : MonoBehaviour {
		
		public const string PREFAB_PATH = "ui/description/common_baseRes_description_view";
		
		public static CommonBaseResDesView Open(BaseResType type, Vector3 pos,Vector2 sizeDelta)
		{
			CommonBaseResDesView view = UIMgr.instance.Open<CommonBaseResDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(type,pos,sizeDelta);
			return view;
		}

		public Text textTitle;
		public Text textDes;
		public RectTransform rootPanel;

		public BaseResType baseType;

		private float _defaultContentLineHeight = 25;
		private int _defaultBorderX = 10;
		private Vector3 _worldPos;
		private Vector2 _sizeDelta ;
		private Vector2 _originSizeDelta;

		void Awake()
		{
			_originSizeDelta = rootPanel.sizeDelta;
			_defaultContentLineHeight = textDes.preferredHeight;
		}

		public void SetData(BaseResType type,Vector3 worldPos,Vector2 sizeDelta)
		{
			baseType = type;
			_worldPos = worldPos;
			_sizeDelta = sizeDelta;
			StartCoroutine(RefreshCoroutine());
		}
		public void SetPivot(Vector2 pivot)
		{
			rootPanel.pivot = pivot;
		}
		public IEnumerator RefreshCoroutine()
		{
			rootPanel.gameObject.SetActive(false);
			yield return null;

			StartCoroutine(UpdateDesCouroutine());
			//size
			float deltaHeight = textDes.preferredHeight-_defaultContentLineHeight;
			rootPanel.sizeDelta = new Vector2(_originSizeDelta.x,_originSizeDelta.y+deltaHeight);
			//pos
			float screenHalfHeight = UIMgr.instance.designResolution.y/2;
			Vector3 localPosition = transform.InverseTransformPoint(_worldPos);
			float x = 0f;
			float y = localPosition.y;
			if(localPosition.x>0)
			{
				x = localPosition.x-_sizeDelta.x/2-rootPanel.sizeDelta.x/2-_defaultBorderX;
			}else{
				x = localPosition.x+_sizeDelta.x/2+rootPanel.sizeDelta.x/2+_defaultBorderX;
			}
			if(localPosition.y<rootPanel.sizeDelta.y/2-screenHalfHeight)
			{
				y = rootPanel.sizeDelta.y/2-screenHalfHeight;
			}
			if(localPosition.y>screenHalfHeight-rootPanel.sizeDelta.y/2)
			{
				y = screenHalfHeight - rootPanel.sizeDelta.y/2;
			}
			localPosition = new Vector3(x,y);
			rootPanel.localPosition = localPosition;
			rootPanel.gameObject.SetActive(true);
		}
		private IEnumerator UpdateDesCouroutine()
		{
			while(true)
			{
				textDes.text = GetDesString();
				yield return new WaitForSeconds(1);
			}
		}
		private string GetDesString()
		{
			string expend = string.Empty;
			string path = string.Empty;
			string use = string.Empty;
			string recoverCoolDown = string.Empty;
			string recover = string.Empty;
			string owner = string.Empty;
			string limit = string.Empty;

			ItemData itemData = ItemData.GetBasicResItemByType(baseType);
			string name = Localization.Get(itemData.name);

            //int oneHundredBillion = 100000000;//1亿
			switch(baseType)
			{

			case BaseResType.PveAction:
			{
				expend = string.Format(Localization.Get("ui.common_des_view.expend_pveAction"),DungeonProxy.instance.DungeonInfoDictionary.First().Value.dungeonData.actionNeed);
				recover = GetRecoverTimeString(GlobalData.GetGlobalData().pveActionRecoverTime);
				if(GameProxy.instance.PveAction < GameProxy.instance.PveActionMax)
				{
					recoverCoolDown = string.Format(Localization.Get("ui.common_des_view.recover_time"),TimeUtil.FormatSecondToMinute((int)GameProxy.instance.PveActionNextRecoverTime));
				}
				owner = string.Format(Localization.Get("ui.common_des_view.owner2"),GameProxy.instance.PveAction,name);
				limit = string.Format(Localization.Get("ui.common_des_view.limit"),GameProxy.instance.PveActionMax,name);

			}

				break;
			case BaseResType.TowerAction:
			{
				expend = string.Format(Localization.Get("ui.common_des_view.expend_TowerAction"),1);
				recover = GetRecoverTimeString(GlobalData.GetGlobalData().worldTreeFruitRecoverTime);
				if(WorldTreeProxy.instance.WorldTreeFruit < WorldTreeProxy.instance.WorldTreeFruitUpperLimit)
				{
					int second = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldTreeProxy.instance.WorldTreeNextRecoverTime);
					recoverCoolDown = string.Format(Localization.Get("ui.common_des_view.recover_time"),TimeUtil.FormatSecondToMinute(second));
				}
				owner = string.Format(Localization.Get("ui.common_des_view.owner2"),WorldTreeProxy.instance.WorldTreeFruit,name);
				limit = string.Format(Localization.Get("ui.common_des_view.limit"),WorldTreeProxy.instance.WorldTreeFruitUpperLimit,name);
			}
				break;
			case BaseResType.PvpAction:
			{
				expend = string.Format(Localization.Get("ui.common_des_view.expend_pvpAction"),1);
				//recover = GetRecoverTimeString(GlobalData.GetGlobalData().challengeTimesRecoverTime);
//				if(GameProxy.instance.PvpAction<GameProxy.instance.PvpActionMax)
//				{
//					recoverCoolDown = string.Format(Localization.Get("ui.common_des_view.recover_time"),TimeUtil.FormatSecondToHour((int)GameProxy.instance.PvpActionNextRecoverTime/1000));
//				}

				owner = string.Format(Localization.Get("ui.common_des_view.owner2"),GameProxy.instance.PvpAction,name);
				limit = string.Format(Localization.Get("ui.common_des_view.limit"),GameProxy.instance.PvpActionMax,name);
			}
				break;
			
			default:
				//limit = string.Format(Localization.Get("ui.common_des_view.limit"),GetNumberLimitString(oneHundredBillion),name);
				owner = string.Format(Localization.Get("ui.common_des_view.owner2"),GetNumberLimitString(GameProxy.instance.BaseResourceDictionary.GetValue(baseType)),name);
				break;
			}
			DropMessageData data = DropMessageData.GetDropMsgDataByResData((int)BaseResType.Item,itemData.id);
			path = data == null ? "" : Localization.Get( data.des);
			return string.Format("{0}{1}{2}{3}{4}{5}{6}",expend,use,recover,recoverCoolDown,owner,limit,path);
		}
		private string GetRecoverTimeString(int second)
		{
			int hour = second/3600;
			int minute = second/60 % 60;
			if(hour == 0)
			{
				return string.Format(Localization.Get("ui.common_des_view.recover_minute"),minute);
			}else if(minute == 0)
			{
				return string.Format(Localization.Get("ui.common_des_view.recover_hour"),hour);
			}else
			{
				return string.Format(Localization.Get("ui.common_des_view.recover_hourMinute"),hour,minute);
			}
		}
		private string GetNumberLimitString(int number)
		{
			StringBuilder s = new StringBuilder();
			while(true)
			{
				int num = number %1000;
				number = number/1000;
				if(number == 0)
				{
					s.Insert(0,num);
					break;
				}
				s.Insert(0,string.Format("{0:d3}",num));
				s.Insert(0,",");
			}
			return s.ToString();
		}
		public void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

