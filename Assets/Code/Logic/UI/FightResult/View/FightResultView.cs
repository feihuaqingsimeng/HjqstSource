using UnityEngine;
using System.Collections;
using Logic.Net.Controller;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Fight.Model;
using Common.Util;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Dungeon.Model;
using Logic.UI.CommonAnimations;
using Logic.UI.CommonReward.View;
using Logic.Game.Model;
using Logic.FunctionOpen.Model;
using Common.Localization;
using Logic.UI.FightResult.Model;
using Logic.Hero.Model;
using Logic.Fight.Controller;
using Logic.UI.DungeonDetail.Model;
using Logic.VIP.Model;
using LuaInterface;
using Logic.UI.WorldTree.Model;
using Logic.UI.Tips.View;
using Logic.UI.WorldTree.Controller;
using Logic.Audio.Controller;

namespace Logic.UI.FightResult.View
{
    public class FightResultView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/fight_result/fight_result_view";
        public static FightResultView Open(bool isWin, FightType fightType)
        {
            FightResultView view = UIMgr.instance.Open<FightResultView>(PREFAB_PATH);
            view.Show(isWin, fightType);
            return view;
        }

        #region unity objct
        public GameObject winRoot;
        public GameObject lostRoot;
        public Text text_profession_name;
        public Text text_profession_level;
        public Text text_pass_time;
        public Text text_max_hit;
        public Text text_total_exp;
        public Text text_total_money;
		public GameObject goFailedTimeOut;

        public Image[] startImages;
        public GameObject Prefab_HeroLevelUpView;
        public Transform TranHeroRoot;
        public GameObject btn_root;
        public GameObject panelDrop;
        public Transform tranDropRoot;
        public RectTransform image_light;
        public TreasureBoxRotateShake treasureBox;

        public Button btnWinNext;
		public Button btnWinBack;
		public Button btnWinAgain;

        public Button btnFailedAgain;
        public Button btnFailedBack;
		public Button btnFailedHero;
		public Button btnFailedFormation;
		public Button btnFailedEquip;
		public Button btnFailedShop;

		//extra
		public Transform[] tranExtraRoot;
        #endregion

		
        private int _dropAccountExp;
		private int _dropHeroExp;
        private List<DropItem> _dropHeroAndEquipList = new List<DropItem>();
        private FightType _fightType;
		public FightType fightType
		{
			get{return _fightType;}
		}
        private bool _isWin;
		public bool isWin{
			get{ return _isWin;}
		}
		public int starNum
		{
			get{return _isWin ? FightProxy.instance.fightResultStar : 0;}
		}
		public bool hasReward
		{
			get{return _dropHeroAndEquipList.Count > 0;}
		}
		private FightResultBasePanel resultBasePanel;

		public System.Action onBoxOpenCallback;


        void Start()
        {
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
        }
		void OnDestroy()
		{
			if(resultBasePanel!=null)
				resultBasePanel.OnDestroy();
			resultBasePanel = null;
			AudioController.instance.StopAudio(AudioController.fireworks_audio);
		}
        public void Show(bool isWin, FightType type)
        {
			Debugger.Log(string.Format("iswin:{0},fightType:{1}",isWin,type));
            _isWin = isWin;
            _fightType = type;
            winRoot.SetActive(_isWin);
            lostRoot.SetActive(!_isWin);
            if (_isWin)
            {
                Prefab_HeroLevelUpView.SetActive(false);
                treasureBox.gameObject.SetActive(false);
                btn_root.SetActive(false);
                panelDrop.SetActive(false);
				int star = starNum;
                float heroDelay = (star + 1) * 0.3f;
                FightResultProxy.instance.CalcDropItem(type);
                _dropHeroAndEquipList = FightResultProxy.instance.DropHeroAndEquipList;
                ShowStars(star);
                ShowHeroLevelUp(heroDelay);
                ShowTotalExp(heroDelay);

                text_pass_time.text = TimeUtil.FormatSecondToMinute(FightController.instance.fightCostTime);
                text_max_hit.text = FightController.instance.comboCount.ToString();
                text_profession_name.text = Localization.Get(GameProxy.instance.AccountName);
				text_profession_level.text = string.Format(Localization.Get("common.role_icon.role_lv"),GameProxy.instance.AccountLevel);
				AudioController.instance.PlayAudioRepeat(AudioController.fireworks_audio);
			}else
			{
				goFailedTimeOut.SetActive(FightProxy.instance.fightOverType == FightOverType.Timeout);
			}
			//extra hide
			for(int i = 0,count = tranExtraRoot.Length;i<count;i++)
			{
				int childCount = tranExtraRoot[i].childCount;
				for(int j = 0;j<childCount;j++)
				{
					tranExtraRoot[i].GetChild(j).gameObject.SetActive(false);
				}
			}
			resultBasePanel = FightResultBasePanel.Create(type,this);
			if(resultBasePanel!=null)
			{
				resultBasePanel.Init();
				//Invoke("AutoGoMainView",30);
			}
				

        }
		//30秒不操作，自动返回主界面
		public void AutoGoMainView()
		{
			resultBasePanel.OnAutoGoMainViewHandler();
		}
        private void ShowHeroLevelUp(float delay)
        {
            FightResultProxy fightResultProxy = FightResultProxy.instance;
            TransformUtil.ClearChildren(TranHeroRoot, true);
            GameObject go;
            Transform tran;
            HeroLevelUpView heroView;
            int noDieNoMaxLvHeroCount = 0;
			bool isMaxLevel = false;
			int accountExp = fightResultProxy.dropBaseResDictionary.GetValue(BaseResType.Account_Exp);
			int heroExp = fightResultProxy.dropBaseResDictionary.GetValue(BaseResType.Hero_Exp);
			if (GameProxy.instance.IsMaxAccountLevel)
			{
				accountExp = 0;
			}
            //player
			if (fightResultProxy.FightBeforeTeamPlayer != null)
			{
				go = Instantiate<GameObject>(Prefab_HeroLevelUpView);
				go.SetActive(true);
				tran = go.transform;
				tran.SetParent(TranHeroRoot, false);
				heroView = tran.GetComponent<HeroLevelUpView>();
				isMaxLevel = fightResultProxy.FightBeforeTeamPlayer.IsMaxLevel;
				int playerExp = heroExp;
				if (isMaxLevel)
				{
					playerExp = 0;
				}

				if (fightResultProxy.IsHeroDie(Logic.Game.Model.GameProxy.instance.PlayerInfo.instanceID))
				{
					heroView.SetMainHeroData(fightResultProxy.FightBeforeTeamPlayer,0, 0,0);
				}
				else
				{
					
					heroView.SetMainHeroData(fightResultProxy.FightBeforeTeamPlayer,playerExp, accountExp, delay);
					if (playerExp != 0)
						noDieNoMaxLvHeroCount++;
				}
			}

            //hero
            List<HeroInfo> oldHeroList = fightResultProxy.FightBeforeTeamHeroDictionary.GetValues();
            int count = oldHeroList.Count;
            for (int i = 0; i < count; i++)
            {
                go = Instantiate<GameObject>(Prefab_HeroLevelUpView);
                go.SetActive(true);
                tran = go.transform;
                tran.SetParent(TranHeroRoot, false);
                heroView = tran.GetComponent<HeroLevelUpView>();
                HeroInfo oldHeroInfo = oldHeroList[i];
                isMaxLevel = oldHeroInfo.IsMaxLevel;
                if (fightResultProxy.IsHeroDie(oldHeroInfo.instanceID) || isMaxLevel)
                {
					heroView.SetHeroData(oldHeroInfo,0,0);
                }
                else
                {
					heroView.SetHeroData(oldHeroInfo,heroExp, delay);
                    noDieNoMaxLvHeroCount++;
                }

            }
			_dropAccountExp = accountExp;
			_dropHeroExp = heroExp * noDieNoMaxLvHeroCount;
        }


        public void ShowStars(int star)
        {
            Image starImage;

            for (int i = 0, count = startImages.Length; i < count; i++)
            {
                starImage = startImages[i];

                if (i >= star)
                {
                    starImage.gameObject.SetActive(false);
                }
                else
                {
                    starImage.GetComponent<StarScaleAction>().Init();
                }
            }
        }

        private void ShowTotalExp(float delay = 0)
        {
            text_total_exp.transform.parent.gameObject.SetActive(true);
            text_total_exp.transform.GetComponent<NumberIncreaseAction>().Init(0, _dropAccountExp, delay);
        }

        public IEnumerator ShowDropBoxCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_dropHeroAndEquipList.Count == 0)
            {
                ShowBtnRoot(0);
            }
            else
            {
                treasureBox.gameObject.SetActive(true);
                treasureBox.init();
				Invoke("OpenBox", 4);
            }

        }


		private void ShowBtnRoot(float delay,GameResData heroResData = null)
        {
            //btn 
			StartCoroutine(DoShowBtnRoot(delay,heroResData));
        }
        private IEnumerator DoShowBtnRoot(float t,GameResData heroResData)
        {
            yield return new WaitForSeconds(t);
            btn_root.SetActive(true);
            CommonFadeInAnimation fadeIn = btn_root.AddComponent<CommonFadeInAnimation>();
            fadeIn.set(0.25f, 0);
//			if (heroResData != null)
//			{
//				bool hasShowHeroDisplayView = ShowHeroDisplayView(heroResData);
//				if (!hasShowHeroDisplayView)
//				{
//					Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewStay"));
//				}
//			}
//			else
//			{
//				Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewStay"));
//			}

			if (heroResData != null && ShowHeroDisplayView(heroResData))
			{

			}
			else
			{
				Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewStay"));
			}
        }
		private void OpenBox()
		{
			if (!treasureBox.gameObject.activeSelf)
				return;
			AudioController.instance.PlayAudio(AudioController.open_box_audio,false);
			DoShowDropItem();
			if(onBoxOpenCallback != null)
				onBoxOpenCallback();
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", FightResultView.PREFAB_PATH, "OnBoxOpen"));
		}

        private void DoShowDropItem()
        {
            if (!treasureBox.gameObject.activeSelf)
                return;

            panelDrop.SetActive(true);
            treasureBox.gameObject.SetActive(false);

            TransformUtil.ClearChildren(tranDropRoot, true);
            int count = _dropHeroAndEquipList.Count;


            int half = count / 2;
            float scaleShakeTime = 0.2f;
            float scaleShakeDelayTime = 0.1f;
            float allDelay = 0.5f;
			float btnDelayShowTime = count * scaleShakeTime + 0.5f + allDelay;


            //drop
			GameResData heroGRD = null;
            for (int i = 0; i < count; i++)
            {
                DropItem dropItem = _dropHeroAndEquipList[i];
                CommonRewardIcon rewardIcon;
                rewardIcon = CommonRewardIcon.Create(tranDropRoot);
				GameResData resData = new GameResData((BaseResType)dropItem.itemType, dropItem.itemNo, dropItem.itemNum, dropItem.heroStar);

				rewardIcon.SetGameResData(resData);
				rewardIcon.SetDesButtonType(ShowDescriptionType.click);
				rewardIcon.ShowRewardCount(true);
				Transform tran = rewardIcon.transform;
                GameObject go = rewardIcon.gameObject;
                if (count % 2 == 0)
                {
                    tran.localPosition = new Vector3((i - half) * 120 + 60, 0, 0);
                }
                else
                {
                    tran.localPosition = new Vector3((i - half) * 120, 0, 0);
                }
                go.AddComponent<ScaleShake>().init(0, 1, scaleShakeTime, i * scaleShakeDelayTime + allDelay, ScaleShake.ScaleShakeType.ScleShake_After);
				if(resData.type == BaseResType.Hero)
					heroGRD = resData;
			}

			ShowBtnRoot(btnDelayShowTime,heroGRD);

            //bg light

            Vector3 to = image_light.localScale;
            image_light.localScale = Vector3.zero;
            LeanTween.scale(image_light, to, scaleShakeTime).setDelay(allDelay);

        }
		//英雄模型展示
		private bool ShowHeroDisplayView(GameResData resData)
		{
			return Logic.Hero.Controller.HeroController.HeroControllerLuaTable.GetLuaFunction("ShowNewHero").Call(resData.id, resData.star, null, false, false)[0].ToString().ToBoolean();
		}
        //退出动画
		private bool _isShowQuitAction = false;
		public void CloseDropItemAction(FightResultQuitType type,System.Action< FightResultQuitType> callback)
		{
			if(_isShowQuitAction)
				return;
			_isShowQuitAction = true;
			float time = 0.15f;
			LeanTween.scale(image_light.gameObject, Vector3.zero, time);
			
			int count = tranDropRoot.childCount;
			float shakeTime = 0.2f;
			if(count == 0)
			{
				callback(type);
			}else
			{
				for (int i = 0; i < count; i++)
				{
					Transform tran = tranDropRoot.GetChild(i);
					ScaleShake shake = tran.GetComponent<ScaleShake>();
					shake.init(1, shakeTime, time, 0.1f, ScaleShake.ScaleShakeType.ScaleShake_Before);
					CommonFadeToAnimation fadeTo = tran.gameObject.AddComponent<CommonFadeToAnimation>();
					fadeTo.init(1, 0, time, time);
					if (i == 0)
					{
						fadeTo.onComplete = ()=>{
							_isShowQuitAction = false;
							callback(type);
						};
					}
				}
			}
		}
	
        #region 事件

        public void onClickTreasureBoxHnadler()
        {
			OpenBox();
        }

        #endregion
    }
}