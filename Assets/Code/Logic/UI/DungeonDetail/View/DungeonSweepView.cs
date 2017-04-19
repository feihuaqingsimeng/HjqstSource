using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Game.Model;
using UnityEngine.UI;
using Common.UI.Components;
using Common.Localization;
using Common.Util;
using Logic.Player.Model;
using Logic.Enums;
using Logic.UI.DungeonDetail.Model;
using Logic.Dungeon.Controller;
using Logic.Item.Model;
using Logic.UI.Tips.View;
using Logic.Dungeon.Model;


namespace Logic.UI.DungeonDetail.View
{
    public class DungeonSweepView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/dungeon_detail/dungeon_sweep_view";

        private Dictionary<int, List<GameResData>> _rewardDic;
        private int _oldAccountLv;
        private int _oldAccountExp;
        private bool _isClickCurrentSweepBtn;
        public static DungeonSweepView Open(Dictionary<int, List<GameResData>> data, int oldAccountLv, int oldAccountExp)
        {
            DungeonSweepView view = UIMgr.instance.Open<DungeonSweepView>(PREFAB_PATH,EUISortingLayer.Tips);
            view.SetData(data, oldAccountLv, oldAccountExp);
            return view;
        }

        #region ui component
        public Text textTitle;
        public Transform scrollContentRoot;
        public GameObject sweepCompletePrefab;
        public ScrollRect scrollRect;
        public SweepButton sweepButtonPrefab;
        public GameObject shakePanel;
        public Button sweepBtnOne;
        public Button sweepBtnTen;
		public Text sweepTenText;
        public float sweepShowInterval = 0.7f;
        public float sweepShowDelay = 0.3f;
        public bool refresh;
        private Scrollbar _scrollBar;
        private bool _isShowComplete;
        #endregion
        void Awake()
        {
            _scrollBar = scrollRect.verticalScrollbar;
			BindDelegate();
        }
//        void Update()
//        {
//            if (refresh)
//            {
//                refresh = false;
//                SetData(_rewardDic, _oldAccountLv, _oldAccountExp);
//            }
//        }
        private void SetData(Dictionary<int, List<GameResData>> rewards, int oldAccountLv, int oldAccountExp)
        {
            _rewardDic = rewards;
            if (!_isClickCurrentSweepBtn)
            {
                _oldAccountLv = oldAccountLv;
                _oldAccountExp = oldAccountExp;
            }
            StartCoroutine(ShowOneByOneActionCoroutine());

			RefreshSweepText();
        }

		void OnDestroy()
		{
			UnBindDelegate();
		}
		private void BindDelegate()
		{
			DungeonProxy.instance.onDungeonInfosUpdateDelegate += RefreshSweepText;
		}
		private void UnBindDelegate()
		{
			DungeonProxy.instance.onDungeonInfosUpdateDelegate -= RefreshSweepText;
		}
		private void RefreshSweepText()
		{
			int count = DungeonDetailProxy.instance.GetSweepTimes();
			count = count > 1 ? count : 10;
			sweepTenText.text = Localization.Get("ui.dungeon_sweep_view.sweep_"+count);
		}
        private IEnumerator ShowOneByOneActionCoroutine()
        {
            TransformUtil.ClearChildren(scrollContentRoot, true);
            sweepBtnOne.interactable = false;
            sweepBtnTen.interactable = false;
            scrollRect.vertical = false;
            sweepButtonPrefab.gameObject.SetActive(false);
            _scrollBar.interactable = false;
            _scrollBar.value = 1;

            List<int> keys = _rewardDic.GetKeys();
            int count = keys.Count;
            AccountExpData oldExpData = AccountExpData.GetAccountExpDataByLv(_oldAccountLv);

            int addExp = 0;
            for (int i = 0; i < count; i++)
            {

                int key = keys[i];
                SweepButton sweepBtn = Instantiate<SweepButton>(sweepButtonPrefab);
                sweepBtn.gameObject.SetActive(true);
                sweepBtn.transform.SetParent(scrollContentRoot, false);
                List<GameResData> rewardList = _rewardDic[key];
                //exp
                addExp += GetBaseResNum(rewardList, BaseResType.Account_Exp);
                int totalexp = addExp + _oldAccountExp + (oldExpData.expTotal - oldExpData.exp);
                AccountExpData accountExpData = AccountExpData.GetAccountExpDataByExp(totalexp);
                int accountLv = 0;
                int accountExp = 0;
                if (accountExpData == null)
                {
                    accountLv = GameProxy.instance.AccountMaxLevel;
                    accountExp = 0;
                }
                else
                {
                    accountLv = accountExpData.lv;
                    accountExp = totalexp - (accountExpData.expTotal - accountExpData.exp);
                }
                float percent = (addExp * 100.0f) / oldExpData.exp;
                sweepBtn.SetSweepData(Localization.Get(string.Format("ui.dungeon_sweep_view.sweep_{0}", key)), rewardList, accountLv, accountExp, percent);
                yield return null;
                yield return null;
                yield return null;
                StartCoroutine(ScrollToBottomCoroutine(sweepShowInterval));
				if (count == 1)
				{
					yield return new WaitForSeconds(sweepShowDelay);
				}else
				{
					yield return new WaitForSeconds(sweepShowInterval + sweepShowDelay);
				}
                

            }
            yield return null;

            //animation
            GameObject completeAni = Instantiate<GameObject>(sweepCompletePrefab);
			completeAni.SetActive(true);
            completeAni.transform.SetParent(sweepCompletePrefab.transform.parent, false);
            completeAni.transform.GetChild(0).gameObject.SetActive(true);


            _scrollBar.interactable = true;
            scrollRect.vertical = true;
            yield return new WaitForSeconds(0.25f);
            //shake
            LeanTween.moveLocalY(shakePanel, shakePanel.transform.localPosition.y - 6, 0.03f).setLoopPingPong(1);
            yield return new WaitForSeconds(1f);
            sweepBtnOne.interactable = true;
            sweepBtnTen.interactable = true;
            completeAni.AddComponent<Logic.UI.CommonAnimations.CommonFadeToAnimation>().init(1, 0, 0.2f, 0);
            yield return new WaitForSeconds(0.2f);
            GameObject.DestroyImmediate(completeAni);

        }
        private IEnumerator ScrollToBottomCoroutine(float t)
        {
            float value = _scrollBar.value;
            //float time = t;
            while (true)
            {
                _scrollBar.value = _scrollBar.value - value * Time.deltaTime / t;
                if (_scrollBar.value == 0)
                    break;
                yield return null;
            }
        }
        private int GetBaseResNum(List<GameResData> dataList, BaseResType type)
        {
            int num = 0;
            GameResData data;
            for (int i = 0, count = dataList.Count; i < count; i++)
            {
                data = dataList[i];
                if (data.type == type)
                {
                    num += data.count;
                }
            }
            return num;
        }

        #region ui event handler
        public void OnClickCloseBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);

        }
        public void OnClickSweepOneBtnHandler()
        {
            if (DungeonDetailProxy.instance.CheckSweep(false))
            {
                _oldAccountLv = GameProxy.instance.AccountLevel;
                _oldAccountExp = GameProxy.instance.AccountExp;
                _isClickCurrentSweepBtn = true;
                DungeonController.instance.CLIENT2LOBBY_PVE_MOP_UP_REQ(DungeonDetailProxy.instance.DungeonInfo.id);
			}else
			{
				OnClickCloseBtnHandler();
			}

        }
        public void OnClickSweepTenBtnHandler()
        {
			if (DungeonDetailProxy.instance.CheckSweep(true))
            {
                _oldAccountLv = GameProxy.instance.AccountLevel;
                _oldAccountExp = GameProxy.instance.AccountExp;
                _isClickCurrentSweepBtn = true;
				DungeonController.instance.CLIENT2LOBBY_PveTenMopUp_REQ(DungeonDetailProxy.instance.DungeonInfo.id,DungeonDetailProxy.instance.GetSweepTimes());
			}else
			{
				OnClickCloseBtnHandler();
			}

        }
        #endregion
    }

}
