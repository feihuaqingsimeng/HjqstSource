using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.Enums;
using Logic.Game;
using Logic.Game.Model;
using Logic.Task;
using Logic.Task.Model;
using Logic.Item.Model;
using Logic.Player.Model;
using Logic.Player.Controller;
using Logic.UI.CommonItem.View;
using Logic.UI.Tips.View;
using Logic.UI.GeneralMeterial.View;

namespace Logic.UI.ChangeProfession.View
{
	public class ActivateProfessionView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/change_profession/activate_profession_view";
		public static ActivateProfessionView Open(PlayerData playerData)
		{
			ActivateProfessionView view = UIMgr.instance.Open<ActivateProfessionView>(ActivateProfessionView.PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetPlayerData(playerData);
			return view;
		}
		private PlayerData _playerData;
		private GameResData _costItemGameResData;

		#region UI components
		public Text transferTitleText;
		public Transform transferConditionsRoot;
		public Text transferConditionTextPrefab;
		public Text costResourcesTitleText;
		public Transform transferCostResourcesRoot;
		public CommonItemIcon commonItemIconPrefab;
		public Text transferText;
		public Text cancelText;
		public GeneralMeterialGroup generalMeterialGroup;
		#endregion UI components

		public void SetPlayerData (PlayerData playerData)
		{
			_playerData = playerData;
			transferTitleText.text = Localization.Get("ui.activate_profession_view.activate_profession_title");
			costResourcesTitleText.text = Localization.Get("ui.activate_profession_view.cost_resources_title");
			transferText.text = Localization.Get("ui.activate_profession_view.transfer");
			cancelText.text = Localization.Get("ui.activate_profession_view.cancel");

			int transferTaskContidionCount = _playerData.transferTaskConditionIDList.Count;
			for (int i = 0; i < transferTaskContidionCount; i++)
			{
				TaskConditionData taskConditionData = TaskConditionData.GetTaskConditionData(_playerData.transferTaskConditionIDList[i]);
				Text transferConditionText = GameObject.Instantiate<Text>(transferConditionTextPrefab);
				transferConditionText.text = TaskUtil.GetTaskConditionDescriptionWithColor(taskConditionData);
				transferConditionText.transform.SetParent(transferConditionsRoot, false);
				transferConditionText.gameObject.SetActive(true);
			}
			transferConditionTextPrefab.gameObject.SetActive(false);

			List<GameResData> transferCostResourcesList = _playerData.transferCostList;
			int transferCostResourceCount = transferCostResourcesList.Count;
			commonItemIconPrefab.gameObject.SetActive(true);
			for (int i = 0; i < transferCostResourceCount; i++)
			{
				GameResData gameResData = transferCostResourcesList[i];

				if(gameResData.type == BaseResType.Item)
				{
					int universalId = ItemData.GetItemDataByID(gameResData.id).universal_id;
					if(universalId != 0)
					{
						CommonItemIcon itemOrigin = CreateItemIcon(gameResData,true);
						CommonItemIcon itemGeneral = CreateItemIcon(new GameResData(BaseResType.Item,universalId,gameResData.count,gameResData.star),true);
						Toggle originToggle = itemOrigin.GetComponentInChildren<Toggle>();
						generalMeterialGroup.AddToggle((uint)gameResData.id,originToggle);
						generalMeterialGroup.AddToggle((uint)gameResData.id,itemGeneral.GetComponentInChildren<Toggle>());
						generalMeterialGroup.OnClickToggle(originToggle);
					}else
					{
						CreateItemIcon(gameResData,false);
					}
				}else
				{
					CreateItemIcon(gameResData,false);
				}

			}
			commonItemIconPrefab.gameObject.SetActive(false);
		}
		private CommonItemIcon CreateItemIcon(GameResData data,bool showToggle)
		{
			_costItemGameResData = data;
			CommonItemIcon commonItemIcon = GameObject.Instantiate<CommonItemIcon>(commonItemIconPrefab);
			commonItemIcon.SetAsRequireResource(data);
			commonItemIcon.transform.SetParent(transferCostResourcesRoot, false);
			commonItemIcon.gameObject.SetActive(true);
			commonItemIcon.SetEnableItemDesButton(false);
			commonItemIcon.onClickHandler = ClickCostItemHandler;

//			Toggle toggle = commonItemIcon.GetComponentInChildren<Toggle>();
//			toggle.gameObject.SetActive(showToggle);
			return commonItemIcon;
		}

		public void ClickCostItemHandler (CommonItemIcon commonItemIcon)
		{
			GoodsJump.View.GoodsJumpPathView.Open(_costItemGameResData);
		}

		#region UI event handlers
		public void ClickTransferHandler ()
		{
			int transferTaskConditioCount = _playerData.transferTaskConditionIDList.Count;
			for (int i = 0; i < transferTaskConditioCount; i++)
			{
				TaskConditionData taskConditionData = TaskConditionData.GetTaskConditionData(_playerData.transferTaskConditionIDList[i]);
				if (!TaskUtil.IsTaskConditionFinished(taskConditionData))
				{
					CommonErrorTipsView.Open(Localization.Get("ui.common_tips.activate_profession_task_unfinished"));
					return;
				}
			}
			//通用材料检测
			Dictionary<uint, List<Toggle>> groupDic = generalMeterialGroup.toggleGroup;
			List<int> generalMList = new List<int>();
			foreach(var value in groupDic)
			{
				if(!value.Value[0].isOn)
				{
                    generalMList.Add((int)value.Key);
                }
            }
			//资源是否够
			List<GameResData> transferCostResourcesList = _playerData.transferCostList;
			int transferCostResourceCount = transferCostResourcesList.Count;
			for (int i = 0; i < transferCostResourceCount; i++)
			{
				GameResData gameResData = transferCostResourcesList[i];
				if (GameResUtil.IsBaseRes(gameResData.type))
				{
					if (GameProxy.instance.BaseResourceDictionary[gameResData.type] < gameResData.count)
					{
						CommonErrorTipsView.Open(string.Format(Localization.Get("ui.common_tips.not_enough_resource"), GameResUtil.GetBaseResName(gameResData.type)));
						return;
					}
				}
				else if (gameResData.type == BaseResType.Item)
				{
					ItemData itemData = ItemData.GetItemDataByID(gameResData.id);
					if(generalMList.Contains(gameResData.id))//选择了通用材料
					{
						itemData = ItemData.GetItemDataByID(itemData.universal_id);
					}

					ItemInfo itemInfo = ItemProxy.instance.GetItemInfoByItemID(itemData.id);
					if (itemInfo == null || itemInfo.count < gameResData.count)
					{
//						CommonErrorTipsView.Open(string.Format(Localization.Get("ui.common_tips.not_enough_resource"), Localization.Get(itemData.name)));
						CommonAutoDestroyTipsView.Open(Localization.Get("ui.activate_profession_view.item_not_enough"));
						return;
					}
				}
			}

			PlayerController.instance.CLIENT2LOBBY_PLAYER_TRANSFER_REQ((int)_playerData.Id,generalMList);
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickCancelHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion UI event handlers
	}
}
