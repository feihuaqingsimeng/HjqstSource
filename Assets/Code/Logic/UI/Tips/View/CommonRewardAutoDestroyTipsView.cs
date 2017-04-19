using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using System.Collections;
using Logic.UI.CommonAnimations;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Enums;
using Logic.Hero.Model;
using Common.Localization;
using Logic.Equipment.Model;
using Logic.Item.Model;

namespace Logic.UI.Tips.View
{
	public class CommonRewardAutoDestroyTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/common_reward_auto_destroy_tips_view";
		/// <summary>
		/// Open the specified dataList .needCombine set to true 合并datalist中重复的元素
		/// </summary>
		public static CommonRewardAutoDestroyTipsView Open(List<GameResData> dataList,bool needCombine = false){
			CommonRewardAutoDestroyTipsView tips = UIMgr.instance.Open<CommonRewardAutoDestroyTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
			tips.SetDataList(dataList,needCombine);
			return tips;
		}
		#region component
		public GameObject tipTextPrefab;
		public Transform tipRoot;
		#endregion


		private float _delay = 1f;
		private float _totalDelay = 0;
		private List<GameResData> _dataList = new List<GameResData>();
		private bool _isStartCoroutine;
		private bool _isLast;
		private List<GameObject> _tipList = new List<GameObject>();

		void Start(){

		}

		private void SetDataList(List<GameResData> dataList,bool needCombine)
		{
			if(dataList.Count == 0)
				Debugger.LogError("奖励列表为空");
			_dataList.AddRange(dataList);
			if(needCombine)
				CombineList();
			if(!_isStartCoroutine)
			{
				_isStartCoroutine = true;
				StartCoroutine(RefreshCoroutine());

			}
			//CancelInvoke("Close");
			//float delay = _dataList.Count*_delay+2.0f-_totalDelay;
			//Debugger.Log(delay.ToString());
			//Invoke("Close",delay);
		}
		private void CombineList()
		{
			_dataList = UIUtil.CombineGameResList(_dataList);

		}

		private IEnumerator RefreshCoroutine()
		{

			GameResData data;
			string tips = "error";
			tipTextPrefab.SetActive(false);
			int i = 0;
			while(true)
			{
				if(i>=_dataList.Count)
				{
					_isLast = true;
					yield return null;
					continue;
				}
				data = _dataList[i];

				if(data.type == BaseResType.Hero)
				{
					HeroData heroData = HeroData.GetHeroDataByID(data.id);
					if(heroData!=null)
					{
						tips = string.Format(Localization.Get("ui.reward_view.gain_hero"),data.star,Localization.Get( heroData.name),data.count);
					}else
					{
						Debugger.LogError("奖励的英雄不存在 id:"+data.id);
					}
				}else if(data.type == BaseResType.Equipment)
				{
					EquipmentData equipData = EquipmentData.GetEquipmentDataByID(data.id);
					if(equipData != null)
					{
						tips = string.Format(Localization.Get("ui.reward_view.gain_equip"),Localization.Get(equipData.name),data.count);
					}else
					{
						Debugger.LogError("奖励的装备不存在 id:"+data.id);
					}
				}else if(data.type == BaseResType.Item){
					ItemData itemData = ItemData.GetItemDataByID(data.id);
					if(itemData!= null)
					{
						tips = string.Format(Localization.Get("ui.reward_view.gain_item"),Localization.Get(itemData.name),data.count);
					}else
					{
						Debugger.LogError("奖励的道具不存在 id:"+data.id);
					}
				}else{
					ItemData itemData = ItemData.GetBasicResItemByType(data.type);
					if(itemData!= null)
					{
						tips = string.Format(Localization.Get("ui.reward_view.gain_base_res"),Localization.Get(itemData.name),data.count);
					}else
					{
						Debugger.LogError("奖励的基础资源不存在 id:"+data.id);
					}
				}
				for(int j = 0,count = _tipList.Count;j<count;j++)
				{
					GameObject go = _tipList[j];
					if(go != null)
						CommonMoveByAnimation.Get(go).Init(0.15f,0,new Vector3(0,30,0));
				}
				GameObject tipObj = Instantiate<GameObject>(tipTextPrefab);
				tipObj.SetActive(true);
				tipObj.transform.SetParent(tipRoot,false);
				tipObj.transform.localPosition = Vector3.zero;
				tipObj.GetComponent<Text>().text = tips;
				CommonFadeInAnimation.Get(tipObj).set(0.3f,0);
				//CommonMoveByAnimation.Get(tipObj).Init(2,0,new Vector3(0,100,0));
				StartCoroutine(FadeOutCoroutine(2f,tipObj));

				_totalDelay += _delay;

				_tipList.Add(tipObj);

				yield return new WaitForSeconds(_delay);
				i++;
			}

		}
		private IEnumerator FadeOutCoroutine(float delay,GameObject tipObj)
		{
			yield return new WaitForSeconds(delay);
			float time = 0.3f;
			CommonFadeToAnimation.Get(tipObj).init(1,0,time);

			yield return new WaitForSeconds(time);

			GameObject.DestroyImmediate(tipObj);

			if(tipRoot.childCount == 0)
			{
				Close();
			}
		}
		private void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}