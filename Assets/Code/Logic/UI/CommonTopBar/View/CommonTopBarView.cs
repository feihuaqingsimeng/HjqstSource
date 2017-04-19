using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Game.Model;
using Logic.UI.Shop.View;
using Logic.FunctionOpen.Model;
using Logic.UI.WorldTree.Model;
using Logic.UI.WorldTree.Controller;
using Common.Util;
using Logic.UI.CommonItem.View;
using Logic.Item.Model;
using LuaInterface;

namespace Logic.UI.CommonTopBar.View
{
    public class CommonTopBarView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/common/common_top_bar";

        public delegate void OnClickBackButtonDelegate();
        public OnClickBackButtonDelegate onClickBackButtonDelegate;

        private Dictionary<BaseResType, Text> _baseResourceTextDictionary;

        #region UI components
        public Image backgroundImage;
        public Button backButton;
        public Text titleText;

        public Text roleNameText;
        public GameObject worldTreeFruitItemGameObject;
        public GameObject pvpActionItemGameObject;
        public GameObject pveActionItemGameObject;
        public GameObject goldItemGameObject;
        public GameObject diamondItemGameObject;
        public GameObject honorItemGameObject;
        public GameObject expeditionGameObject;

        public Button btnChat;

        public Text pveActionPointText;
        public Text goldText;
        public Text diamondText;
        public Text honorText;
        public Text expeditionText;
        public Text worldTreeFruitText;

        #endregion UI components

        void Start()
        {
            _baseResourceTextDictionary = new Dictionary<BaseResType, Text>();
            _baseResourceTextDictionary.Add(BaseResType.Gold, goldText);
            _baseResourceTextDictionary.Add(BaseResType.Diamond, diamondText);
            _baseResourceTextDictionary.Add(BaseResType.Honor, honorText);
            _baseResourceTextDictionary.Add(BaseResType.ExpeditionPoint, expeditionText);


            GameProxy.instance.onPveActionInfoUpdateDelegate += OnPveActionUpdateHandler;
            GameProxy.instance.onBaseResourcesUpdateDelegate += OnBaseResourcesUpdateHandler;
            WorldTreeProxy.instance.onWorldTreeFruitUpdateDelegate += OnWorldTreeFruitUpdateHandler;
            WorldTreeProxy.instance.onWorldTreeFruitNextRecoverCountDownDelegate += OnWorldTreeFruitNextRecoverCountDownHandler;

            RefreshPveActionInfo();
            RefreshBaseResources();
            if (worldTreeFruitItemGameObject.activeSelf)
            {
                RefreshWorldTreeFruitInfo();
            }
        }

        void OnDestroy()
        {
            GameProxy.instance.onPveActionInfoUpdateDelegate -= OnPveActionUpdateHandler;
            GameProxy.instance.onBaseResourcesUpdateDelegate -= OnBaseResourcesUpdateHandler;
            WorldTreeProxy.instance.onWorldTreeFruitUpdateDelegate -= OnWorldTreeFruitUpdateHandler;
            WorldTreeProxy.instance.onWorldTreeFruitNextRecoverCountDownDelegate -= OnWorldTreeFruitNextRecoverCountDownHandler;
        }

        public void SetAsMainViewStyle()
        {
            backgroundImage.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            titleText.gameObject.SetActive(false);
            roleNameText.gameObject.SetActive(false);
            pvpActionItemGameObject.SetActive(false);
            expeditionGameObject.SetActive(false);
            worldTreeFruitItemGameObject.SetActive(false);
        }

        public void SetAsRoleViewStyle(string roleName, OnClickBackButtonDelegate onClickBackButtonDelegate)
        {
            titleText.gameObject.SetActive(false);
            roleNameText.text = roleName;
            this.onClickBackButtonDelegate += onClickBackButtonDelegate;
            pvpActionItemGameObject.SetActive(false);
            pveActionItemGameObject.SetActive(false);
            goldItemGameObject.SetActive(false);
            diamondItemGameObject.SetActive(false);
            honorItemGameObject.SetActive(false);
            expeditionGameObject.SetActive(false);
            worldTreeFruitItemGameObject.SetActive(false);
        }

        public void SetAsCommonStyle(string title, OnClickBackButtonDelegate onClickBackButtonDelegate, bool showPveActionItem, bool showGoldItem, bool showDiamondItem, bool showHonorItem, bool showPVPActionItem = false, bool showExpeditionItem = false, bool showWorldTreeFruitItem = false)
        {
            titleText.text = title;
            roleNameText.gameObject.SetActive(false);
            this.onClickBackButtonDelegate += onClickBackButtonDelegate;
            pveActionItemGameObject.SetActive(showPveActionItem);
            goldItemGameObject.SetActive(showGoldItem);
            diamondItemGameObject.SetActive(showDiamondItem);
            honorItemGameObject.SetActive(showHonorItem);
            pvpActionItemGameObject.SetActive(showPVPActionItem);
            expeditionGameObject.SetActive(showExpeditionItem);
            worldTreeFruitItemGameObject.SetActive(showWorldTreeFruitItem);
        }

        public static CommonTopBarView CreateNewAndAttachTo(Transform parent)
        {
            GameObject commonTopBarViewPrefab = ResMgr.instance.Load<GameObject>(PREFAB_PATH);
            CommonTopBarView commonTopBarView = GameObject.Instantiate(commonTopBarViewPrefab).GetComponent<CommonTopBarView>();
            commonTopBarView.transform.SetParent(parent, false);
			commonTopBarView.transform.localPosition = new Vector3(commonTopBarView.transform.localPosition.x, commonTopBarView.transform.localPosition.y, 0);
            return commonTopBarView;
        }

        private void RefreshPveActionInfo()
        {
            pveActionPointText.text = string.Format(Localization.Get("common.value/max"), GameProxy.instance.PveAction, GameProxy.instance.PveActionMax);
        }

        public void ShowBackButton(bool show)
        {
            backButton.gameObject.SetActive(show);
        }
        private void RefreshBaseResources()
        {
            Dictionary<BaseResType, int> baseResourceDictionary = GameProxy.instance.BaseResourceDictionary;
            List<BaseResType> keys = new List<BaseResType>(_baseResourceTextDictionary.Keys);
            int keyCount = keys.Count;
            for (int i = 0; i < keyCount; i++)
            {
                BaseResType baseResType = keys[i];
                if (baseResourceDictionary.ContainsKey(baseResType))
                {
                    int value = baseResourceDictionary[baseResType];


                    _baseResourceTextDictionary[baseResType].text = StringUtil.ConvertBigQuantity(value);
                }
                else
                {
                    _baseResourceTextDictionary[baseResType].text = "0";
                }
            }
        }

        private void RefreshWorldTreeFruitInfo()
        {
            worldTreeFruitText.text = WorldTree.Model.WorldTreeProxy.instance.WorldTreeFruit.ToString();
            //			if (WorldTree.Model.WorldTreeProxy.instance.WorldTreeFruit <= 0)
            //			{
            //				LTDescr ltDescr = LeanTween.delayedCall(gameObject, 1, OnUpdateWorldTreeFruitNextRecoverTime);
            //				ltDescr.setId(1);
            //				ltDescr.setIgnoreTimeScale(true);
            //				ltDescr.setRepeat(-1);
            //			}
        }

        //		private void OnUpdateWorldTreeFruitNextRecoverTime ()
        //		{
        //			int countDownTimeInSeconds = Mathf.Max(0, (int)Common.GameTime.Controller.TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldTree.Model.WorldTreeProxy.instance.WorldTreeNextRecoverTime));
        //			string countDownTimeString = Common.Util.TimeUtil.FormatSecondToMinute(countDownTimeInSeconds);
        //			worldTreeFruitText.text = countDownTimeString;
        //			if (countDownTimeInSeconds <= 0)
        //			{
        //				LeanTween.removeTween(1);
        //				WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_FRUIT_SYN_REQ();
        //			}
        //		}

        private void OnPveActionUpdateHandler()
        {
            RefreshPveActionInfo();
        }

        private void OnBaseResourcesUpdateHandler()
        {
            RefreshBaseResources();
        }

        void OnWorldTreeFruitUpdateHandler()
        {
            RefreshWorldTreeFruitInfo();
        }

        void OnWorldTreeFruitNextRecoverCountDownHandler(int countDownTime)
        {
            string countDownTimeString = Common.Util.TimeUtil.FormatSecondToMinute(countDownTime);
            worldTreeFruitText.text = countDownTimeString;
        }

        #region UI event handlers
        public void ClickBackButtonHandler()
        {
            if (onClickBackButtonDelegate != null)
            {
                onClickBackButtonDelegate();
            }
        }

        public void ClickAddWorldTreeFruitButtonHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
        }

        public void ClickAddPveActionButtonHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
        }
		public void ClickAddPvpActionButtonHandler()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
		}
        public void ClickAddGoldButtonHandler()
        {
            //FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Gold,5);
			LuaTable goldenTouchCtrl = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","golden_touch_controller")[0];
			goldenTouchCtrl.GetLuaFunction("OpenGoldenTouchView").Call();
        }

        public void ClickAddDiamondButtonHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
        }

        public void ClickChatButtonHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Chat, null, false, true);
        }
        #endregion UI event handlers
    }
}