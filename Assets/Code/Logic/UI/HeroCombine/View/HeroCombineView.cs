using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Localization;
using Common.Util;
using Logic.UI.CommonAnimations;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.UI.HeroCombine.Model;
using Logic.Enums;
using Logic.Hero.Model.Combine;
using Logic.Game.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.Tips.View;

namespace Logic.UI.HeroCombine.View
{
    public class HeroCombineView : MonoBehaviour {

	    public const string PREFAB_PATH = "ui/hero_combine/hero_combine_view";

	    #region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text text_select_material_title;
	    public Text text_star;
	    public Text text_star_rate;
	    public Text text_star_rate_num;
		public Text text_not_choice;
	    
	    public Text text_choice_same_star;
	    public Text text_btn_combine;
	    public Text text_coin;
	    public Transform  heroButtonsRootTransform;
        public HeroButton heroButtonPrefab;
	    public Transform[] combineMaterialRootTran = new Transform[4];
    
        public Transform combineNewHeroRootTrans;
        public RectTransform combineCircleBGTran;
		public Dropdown dropDownSort;
	    #endregion



        private Vector3[] _combineMaterialRootTranPos ;
        private List<HeroInfo> _heroTableInfoList;
		private Button _btn_combine;
		//private Logic.UI.CommonPopupView.CommonPopup _sortTitlePopup;

		private int _selectHeroStar = -1;
		private bool _isStartCombineAnim = false;
		private GameObject _newHeroGO;

        void Awake() {
            init();


        }
        private void init() {
            initText();
			_btn_combine = text_btn_combine.transform.parent.GetComponent<Button>();
			//_sortTitlePopup = text_sort_title.transform.parent.GetComponent<Logic.UI.CommonPopupView.CommonPopup>();

			string[] choice = new string[6];
			for(int i = 0;i<choice.Length;i++){
				choice[i] = GetSortStarTitleString(i);
			}
			//_sortTitlePopup.Init(0,choice,ClickHeroTableSort);

			int length = combineMaterialRootTran.Length;
            
            _combineMaterialRootTranPos = new Vector3[length];
            for (int i = 0; i < length; i++) {
                _combineMaterialRootTranPos[i] = combineMaterialRootTran[i].localPosition;
            }
			List<Dropdown.OptionData> options = dropDownSort.options;
			for(int i = 0,count = options.Count;i<count;i++){
				//options[i].text = 
			}
			HeroCombineProxy.instance.onUpdateCombineByProtocolDelegate = RefreshCombineByProtocol;
			HeroCombineProxy.instance.ClearCombineMaterialDic();
            initHeroTable();
			RefreshNewHeroTips ();
        }
		void OnDestroy(){

			HeroCombineProxy.instance.onUpdateCombineByProtocolDelegate = null;
		}

		/// <summary>
		/// Gets the sort star title string. 显示一星，显示二星等
		/// </summary>

		private string GetSortStarTitleString(int star){
			if (star >= 1 && star <= 6) {
				return string.Format (Localization.Get ("ui.hero_combine_view.text_sort_title_2"), GetStartString(star));
			}
			return Localization.Get ("ui.hero_combine_view.text_sort_title_1");
		}
		private string GetStartString(int star){
			if (star >= 1 && star <= 6) {
				return Localization.Get ("ui.hero_combine_view.xing_"+star);
			}else{
				return Localization.Get ("ui.hero_combine_view.xing_6");
			}

		}

		private void initText(){
			string title = Localization.Get("ui.hero_combine_view.text_title");
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(title, ClickBackHandler, true, true, true, false);

			text_select_material_title.text = Localization.Get("ui.hero_combine_view.text_select_material_title");

			text_star.text = Localization.Get("ui.hero_combine_view.text_star");
			text_star_rate.text = Localization.Get("ui.hero_combine_view.text_star_rate");
			
			
			text_choice_same_star.text = Localization.Get("ui.hero_combine_view.text_choice_same_star");
			text_btn_combine.text = Localization.Get("ui.hero_combine_view.text_btn_combine");
		}
        private void initHeroTable() {
            heroButtonPrefab.gameObject.SetActive(false);
			_heroTableInfoList = HeroCombineProxy.instance.GetEnableCombineHeros();
			UpdateHeroTable (0);
        }

		private void UpdateHeroTable(int star = -1){
//			if (_selectHeroStar == star)
//				return;
//			_selectHeroStar = star;

			//check enable combine hero

			List<HeroInfo> starHeroTempList ;
			int count = _heroTableInfoList.Count;
			if (star >= 1 && star <= 6) {
				starHeroTempList = new List<HeroInfo> ();
				HeroInfo info;
				for (int i = 0; i<count; i++) {
					info = _heroTableInfoList[i];
					if(info.advanceLevel == star){
						starHeroTempList.Add(info);
					}
				}
			} else {
				starHeroTempList = _heroTableInfoList;
			}


			//TransformUtil.ClearChildren (heroButtonsRootTransform, false);
			//add child
			SortedDictionary<CombinePosition,uint> materials =  HeroCombineProxy.instance.CombineMaterialDictionary;
			count = starHeroTempList.Count;
			int childCount = heroButtonsRootTransform.childCount;
			GameObject go;
			Transform cloneTran;
			HeroButton heroButton;
			for (int i = 0; i < count; i++) {
				if(i<childCount){
					go = heroButtonsRootTransform.GetChild(i).gameObject;
				}else{
					go = CreateGameObject(heroButtonPrefab.gameObject,heroButtonsRootTransform);
				
				}
				go.SetActive(true);

				heroButton = go.GetComponent<HeroButton>();
				heroButton.HeroButtonSelectDelegate = HeroSelect;
				heroButton.SetHeroInfo(starHeroTempList[i]);

				//refresh
				if(HeroCombineProxy.instance.IsHeroInCombineMaterial(starHeroTempList[i].instanceID)){
					heroButton.SetSelect(true);
				}else{
					heroButton.SetSelect(false);
				}
			}
			for (int i = count; i<childCount; i++) {
				heroButtonsRootTransform.GetChild(i).gameObject.SetActive(false);
			}

		
		}
		private void StartHeroCombineAnimation(){

			_isStartCombineAnim = true;

			Vector3 zAxis = new Vector3( 0,0,-1);

			LeanTween.rotateAroundLocal(combineCircleBGTran.gameObject,zAxis,1080, 4).setEase(LeanTweenType.easeInOutQuint);

			for (int i = 0,count = combineMaterialRootTran.Length; i<count; i++) {

				GameObject go = combineMaterialRootTran[i].gameObject;
				CombineRotateAnimation animation = go.GetComponent<CombineRotateAnimation>();

				animation.init();

				if(i == count-1)
					StartCoroutine(AnimationCompleteCoroutine(animation.rotateTime+animation.rotateDelay));
			}


		}
        private IEnumerator AnimationCompleteCoroutine(float time) {

			yield return new WaitForSeconds(time);

            Debugger.Log("animation complete");
			//show new hero icon

			_newHeroGO = CreateGameObject(heroButtonPrefab.gameObject, combineNewHeroRootTrans);
			HeroInfo info = HeroProxy.instance.GetHeroInfo((uint)HeroCombineProxy.instance.NewHeroIntanceId);
			if(info == null){
				Debugger.LogError(string.Format("combine HeroInfo is not find id:{0}",HeroCombineProxy.instance.NewHeroIntanceId));
			}	
			HeroButton btn = _newHeroGO.GetComponent<HeroButton>();
			btn.SetHeroInfo(info);
			btn.HideLevel();

			
			//delay time to reset all
			StartCoroutine(ResetToOriginCoroutine(2));

        }
		private IEnumerator ResetToOriginCoroutine(float delay){
			yield return new WaitForSeconds(delay);
			_isStartCombineAnim = false;
			_heroTableInfoList = HeroCombineProxy.instance.GetEnableCombineHeros();
			ResetMaterialToOrigin ();
			UpdateHeroTable ();

			_ignoreClickHeroTableSort = true;
			dropDownSort.value = 0 ;

		}
        private void ResetMaterialToOrigin() {
           
			float scaleTime = 0.2f;
			float fadeTime = 0.1f;

			for (int i = 0, count = combineMaterialRootTran.Length; i < count; i++) {
				
				TransformUtil.ClearChildren(combineMaterialRootTran[i], true);
				combineMaterialRootTran[i].GetComponent<CombineRotateAnimation>().ResetPosition();
//				combineMaterialRootTran[i].localPosition = _combineMaterialRootTranPos[i];
//
//				CommonFadeToAnimation ani = CommonFadeToAnimation.Get(combineMaterialRootTran[i].gameObject);
//				ani.init(0, 1, fadeTime, 0);
//				
			}
			LeanTween.scaleX( combineNewHeroRootTrans.gameObject,0,scaleTime);
			LeanTween.scaleX(combineNewHeroRootTrans.gameObject,1,scaleTime).setDelay(scaleTime);
			Destroy(_newHeroGO,scaleTime);
			
			HeroCombineProxy.instance.ClearCombineMaterialDic();
			
			RefreshNewHeroTips ();

        }
		/// <summary>
		/// select combine hero .
		/// </summary>
		/// <returns><c>true</c>, if num < max can select, <c>false</c> can't select.</returns>

		public bool HeroSelect(HeroButton heroButton,bool select){
			if (heroButton == null)
				return false;
			if (CanClickEnable())
				return false;
			Debugger.Log("instanceid:"+heroButton.HeroInfo.instanceID);

			if(select){
				return HeroCombineProxy.instance.AddHeroToCombineMaterial(heroButton.HeroInfo.instanceID);
			}else{
				return HeroCombineProxy.instance.RemoveHeroFromCombineMaterial(heroButton.HeroInfo.instanceID);
			}


		}
		private void RefreshNewHeroTips(){
			int _selectHeroNum = HeroCombineProxy.instance.CombineMaterialDictionary.Count;
			if (_selectHeroNum == 0) {
				text_star.gameObject.SetActive (false);
				text_not_choice.gameObject.SetActive (true);
				text_coin.text = "0";
			} else {
				text_not_choice.gameObject.SetActive(false);
				text_star.gameObject.SetActive(true);
				text_star.text = string.Format( Localization.Get("ui.hero_combine_view.text_star"),GetStartString(_selectHeroStar+1));

				HeroCombineNeedData data = HeroCombineNeedData.GetHeroCombineNeedDataByStar(_selectHeroStar);
				text_star_rate_num.text = string.Format("{0}%",data.GetHeroCombineRate(_selectHeroNum));

				HeroCombineNeedData needData = HeroCombineNeedData.GetHeroCombineNeedDataByStar(_selectHeroStar);
				text_coin.text = needData.gold.ToString() ;
				text_choice_same_star.text = string.Format(Localization.Get("ui.hero_combine_view.text_choice_same_star"),GetStartString(_selectHeroStar));
			}

		}
		public void RefreshUIHandle(){
			RefreshUIMaterialHandle();
			RefreshUIHeroTableHandle();
			RefreshNewHeroTips();
		}
		private void RefreshUIMaterialHandle(){
			HeroCombineProxy HeroCombineProxyInstance = HeroCombineProxy.instance;
			SortedDictionary<CombinePosition,uint> materials = HeroCombineProxyInstance.CombineMaterialDictionary;
			CombinePosition pos;
			HeroInfo info;
			for(int i = 0,count = combineMaterialRootTran.Length;i<count;i++){
				pos = (CombinePosition)(i+1);
				info = HeroCombineProxyInstance.GetHeroAt(pos);
				if(info == null){
					TransformUtil.ClearChildren(combineMaterialRootTran[i],true);
				}else{
					HeroButton b = combineMaterialRootTran[i].GetComponentInChildren<HeroButton>();
					if(b != null){
						b.SetHeroInfo(info);
						b.HideLevel();
					}else{
						GameObject go = CreateGameObject(heroButtonPrefab.gameObject,combineMaterialRootTran[i]);
						go.transform.localScale = new Vector3(0.68f,0.68f,1);
						b = go.GetComponent<HeroButton>();
						b.SetHeroInfo(info);
						b.HideLevel();
					}
					b.SetSelect(true);
					b.HideMark();
				}
			}
		}
		private void RefreshUIHeroTableHandle(){
			SortedDictionary<CombinePosition,uint> materials =  HeroCombineProxy.instance.CombineMaterialDictionary;
			_selectHeroStar = -1;
			if(materials.Count >= 1){
				KeyValuePair<CombinePosition ,uint> keyValue = materials.First();
				uint instanceID =  keyValue.Value;
				HeroInfo info = HeroCombineProxy.instance.GetHeroInfo(instanceID);
				if(info!= null){
					UpdateHeroTable(info.advanceLevel);
					_selectHeroStar = info.advanceLevel;
				}
			}else{
				UpdateHeroTable();
			}
			
		}
		/// <summary>
		/// 合成动画开始后，按钮不可点击
		/// </summary>

		private bool CanClickEnable(){
			return _isStartCombineAnim;
		}
		#region ui event handle

//		public void ClickMaterialHandler(Transform material){
//			int pos = combineMaterialRootTran.IndexOf(material)+1;
//			uint heroId = HeroCombineProxy.instance.GetHeroInstanceIDAt((CombinePosition)pos);
//			if(heroId != 0 )
//				HeroCombineProxy.instance.RemoveHeroFromCombineMaterial(heroId);
//		}

		public void ClickHeroCombine(){
			if (CanClickEnable()) {
				return;
			}
			
			List<HeroInfo> materialList = HeroCombineProxy.instance.GetMaterialHeroInfoList();
			List<int> materialIds = new List<int>();
			int selectHeroNum = materialList.Count;
			if (selectHeroNum < 2) {
				ShowSingleTips("请放入两个以上材料");
				
				return;
			}
			int money = text_coin.text.ToInt32();
			int star = 0;
			int maxLevel = 0;
			int maxStrengthenLevel = 0;
			for(int i = 0,count = materialList.Count;i<count;i++){
				HeroInfo info = materialList[i];
				star = info.advanceLevel;
				if(info.level>maxLevel)
					maxLevel = info.level;
				if(info.strengthenLevel>maxStrengthenLevel)
					maxStrengthenLevel = info.strengthenLevel;
				materialIds.Add((int)info.instanceID);
			}
			HeroCombineNeedData needData = HeroCombineNeedData.GetHeroCombineNeedDataByStar(star);
			
			if(maxLevel< needData.level){
				ShowSingleTips("使用的材料等级不足");
				return;
			}
			int gold = 0;
			GameProxy.instance.BaseResourceDictionary.TryGetValue( BaseResType.Gold,out gold);
			if(gold < money){
				ShowSingleTips("金币不足");
				return;
			}
			if(maxStrengthenLevel>1){
				Debugger.Log("有英雄强化等级>1是否继续？");
			}
			//Debugger.Log("穿的装备有4星以上，是否继续？");


			Logic.Hero.Controller.HeroController.instance.CLIENT2LOBBY_HERO_COMPOSE_REQ(materialIds);
		}
		//合成协议过来后刷新
		public void RefreshCombineByProtocol(bool isSuccess,int newHeroid,bool isSpecial){
			StartHeroCombineAnimation();
		}
		

		/// <summary>
		/// Clicks the hero table sort. index 对应star
		/// </summary>
		private bool _ignoreClickHeroTableSort;
		public void ClickHeroTableSort(int index){
			if(_ignoreClickHeroTableSort)
			{
				_ignoreClickHeroTableSort = false;
				return;
			}
			if(!CanClickEnable()){
				HeroCombineProxy.instance.ClearCombineMaterialDic();
				RefreshUIMaterialHandle();
				RefreshNewHeroTips ();
			}
			int sortIndex = dropDownSort.value;
			if (sortIndex == 0) {//all star
				UpdateHeroTable ();
			} else {
				UpdateHeroTable(sortIndex);
			}
			
		}
		public void ClickBackHandler(){
			if(CanClickEnable())
				return;
			UIMgr.instance.Close(PREFAB_PATH);
		}
		
		public void ClickHeroButtonHandle(HeroButton hb)
		{

			HeroSelect(hb,!hb.IsSelected);

			int count = HeroCombineProxy.instance.CombineMaterialDictionary.Count;
			if(count == 0)
			{
				if(dropDownSort.value != 0)
				{
					_ignoreClickHeroTableSort = true;
					dropDownSort.value = 0 ;
				}

			}else{
				if(dropDownSort.value != hb.HeroInfo.advanceLevel)
				{
					_ignoreClickHeroTableSort = true;
					dropDownSort.value = hb.HeroInfo.advanceLevel;
				}

			}
			RefreshUIHandle();
		}
		#endregion

		private GameObject CreateGameObject(GameObject go,Transform parent){
			GameObject clone = Instantiate<GameObject>(go);
			clone.SetActive(true);
			Transform tran = clone.transform;
			tran.SetParent(parent,false);
			tran.localPosition = Vector3.zero;
			return clone;
		}
		private void ShowSingleTips(string message){
			CommonErrorTipsView.Open(message);
		}
    }
}

