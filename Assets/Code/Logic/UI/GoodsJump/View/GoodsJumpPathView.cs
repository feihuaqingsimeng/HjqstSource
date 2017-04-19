using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.Role.Model;
using Logic.Game.Model;
using Logic.UI.CommonReward.View;
using Logic.Enums;
using Logic.Player.Model;
using Common.Localization;
using Logic.UI.GoodsJump.Model;
using Logic.Dungeon.Model;
using LuaInterface;

namespace Logic.UI.GoodsJump.View
{
	public class GoodsJumpPathView : MonoBehaviour 
	{
		[NoToLua]
		public const string PREFAB_PATH = "ui/goods_jump/goods_jumppath_view";
		[NoToLua]
		public static GoodsJumpPathView Open(GameResData data)
		{
			GoodsJumpPathView view = UIMgr.instance.Open<GoodsJumpPathView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetData(data);
			return view;
		}
		public static GoodsJumpPathView Open(int type,int id,int star)
		{
			GameResData data = new GameResData((BaseResType)type,id,1,star);
			return Open(data);
		}
		#region ui component
		[NoToLua]
		public Text goodsNameText;
		[NoToLua]
		public Transform goodsRoot;
		[NoToLua]
		public GameObject noPathGO;
		[NoToLua]
		public GoodsJumpButton[] pathButton;
		#endregion

		[NoToLua]
		public GameResData _gameResData;

		private void SetData(GameResData data)
		{
			_gameResData = data;
			Refresh();
		}
		private void Refresh()
		{
			CommonRewardIcon icon = goodsRoot.GetComponentInChildren<CommonRewardIcon>();
			if(icon == null)
			{
				icon = CommonRewardIcon.Create(goodsRoot);
			}
			icon.SetGameResData(_gameResData,true);
			icon.SetDesButtonEnable(false);
			icon.HideCount();
			if(_gameResData.type == BaseResType.Hero)
			{
				goodsNameText.text = Localization.Get( icon.RoleInfo.heroData.name);
			}else if(_gameResData.type == BaseResType.Equipment)
			{
				goodsNameText.text = Localization.Get( icon.CommonEquipmentIcon.EquipmentInfo.equipmentData.name);
			}else
			{
				goodsNameText.text = Localization.Get( icon.CommonItemIcon.ItemInfo.itemData.name);
			}
			RefreshPath();
		}
		private void RefreshPath()
		{
			List<PathData> pathDataList = DropMessageData.GetPathDatas(_gameResData,pathButton.Length);
			if (pathDataList == null)
			{
				for(int i = 0 ;i<pathButton.Length;i++)
				{
					pathButton[i].gameObject.SetActive(false);
				}
				noPathGO.SetActive(true);
				noPathGO.GetComponent<Text>().text = "无";
			}else if(pathDataList.Count == 0)
			{
				for(int i = 0 ;i<pathButton.Length;i++)
				{
					pathButton[i].gameObject.SetActive(false);
				}
				noPathGO.SetActive(true);
				DropMessageData data = DropMessageData.GetDropMsgDataByResData(_gameResData,true);
				if (data != null)
					noPathGO.GetComponent<Text>().text = Localization.Get( data.des);
			}else
			{
				for(int i = 0;i<pathButton.Length;i++)
				{
					if(i < pathDataList.Count)
						pathButton[i].Set(pathDataList[i].type,pathDataList[i].id);
					
					pathButton[i].gameObject.SetActive(i < pathDataList.Count);
				}
				noPathGO.SetActive(false);
			}

		}
		[NoToLua]
		public void OnCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

