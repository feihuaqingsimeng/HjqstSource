using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;
using Common.ResMgr;
using Logic.Game.Model;
using Logic.Game;
using Common.Localization;
using Logic.Dungeon.Model;
using Logic.Enums;
using Common.GameTime.Controller;
using System.Text;
using Logic.Audio.Controller;
using Logic.UI.IllustratedHandbook.Model;
using Logic.UI.IllustratedHandbook.Controller;
using Logic.Protocol.Model;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.FunctionOpen.Model;
using LuaInterface;

namespace Logic.UI.AccountInfo.View
{
    public class AccountInfoView : MonoBehaviour
    {

        public const string PREFAB_PATH = "ui/account_info/account_info_view";
        public static AccountInfoView Open()
        {
            AccountInfoView view = UIMgr.instance.Open<AccountInfoView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
            return view;
        }
        public Image imageHead;
        public Text textName;
        public Text textAccountId;
        public Text textAccountLv;
        public Text textAccountExp;
        public Text textDungeonProgress;

        public Text textSystemTime;
        public Text[] textHeroCollection;
        public Slider sliderAccountExp;
        public Slider sliderDungeonProgress;
        public Slider[] sliderHeroCollection;

        public Toggle toggleSoundBg;//背景音乐
        public Toggle toggleSound;//音效
        public Toggle toggleMsgPush;
        public Toggle toggleEffectPlayable;

        public Image imageSoundBg;
        public Image imageSound;
        public Image imagePushMsg;
        public Image imageEffectPlayable;

        private Sprite _spSoundBgOn;
        private Sprite _spSoundBgOff;
        private Sprite _spSoundOn;
        private Sprite _spSoundOff;
        private Sprite _spPushMsgOn;
        private Sprite _spPushMsgOff;
        private Sprite _spEffectPlayableOn;
        private Sprite _spEffectPlayableOff;


        void Start()
        {
            _spSoundBgOn = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_volume_open");
            _spSoundBgOff = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_volume_close");
            _spSoundOn = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_sound_open");
            _spSoundOff = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_sound_close");
            _spPushMsgOn = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_push_open");
            _spPushMsgOff = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_push_close");
            _spEffectPlayableOn = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_effect_open");
            _spEffectPlayableOff = ResMgr.instance.Load<Sprite>("sprite/main_ui/btn_option_effect_close");

            Refresh();
            InitToggle();
//            if (FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_illustrate) )
//            {
//                IllustrationController.instance.CLIENT2LOBBY_Illustration_REQ();
//            }
//            else
//            {
                RefreshHeroCollection();
//            }
            BindDelegate();
        }
        void OnDestroy()
        {
            UnbindDelegate();
        }
        private void BindDelegate()
        {
//            Observers.Facade.Instance.RegisterObserver(((int)MSG.IllustrationResp).ToString(), LOBBY2CLIENT_IllustrationResp_handler);
			IllustratedHandbookProxy.instance.UpdateIllustrationDelegate += LOBBY2CLIENT_IllustrationResp_handler;
        }
        private void UnbindDelegate()
        {
			IllustratedHandbookProxy.instance.UpdateIllustrationDelegate -= LOBBY2CLIENT_IllustrationResp_handler;
        }
        private void InitToggle()
        {
            toggleSoundBg.isOn = AudioController.instance.isOpenAudioBg;
            toggleSound.isOn = AudioController.instance.isOpenAudio;
            toggleMsgPush.isOn = GameSetting.instance.pushMessage;
            toggleEffectPlayable.isOn = GameSetting.instance.effectPlayable;
            RefreshSoundSprite();
        }
        private void Refresh()
        {
			imageHead.SetSprite(ResMgr.instance.Load<Sprite>(GameProxy.instance.AccountHeadIcon));
            //			textName.text = GameProxy.instance.PlayerInfo.name;
            textName.text = GameProxy.instance.AccountName;
            textAccountId.text = GameProxy.instance.AccountId.ToString();
            textAccountLv.text = GameProxy.instance.AccountLevel.ToString();
            float percent = AccountUtil.GetAccountExpPercentToNextLevel(GameProxy.instance.AccountLevel);
            textAccountExp.text = string.Format("{0}%", (int)(percent * 100));
            sliderAccountExp.value = percent;
            RefreshDungeon();

        }
        private void RefreshDungeon()
        {
            Dictionary<int, DungeonInfo> dungeonDic = DungeonProxy.instance.DungeonInfoDictionary;
            DungeonType type;
            int totalCount = 0;
            int threeStarCount = 0;
            foreach (var data in dungeonDic)
            {
                type = data.Value.dungeonData.dungeonType;
                if (type == DungeonType.Easy || type == DungeonType.Normal || type == DungeonType.Hard)
                {
                    totalCount++;
                    if (!data.Value.isLock)
                    {
                        threeStarCount++;
                    }
                }
            }
            float percent = (threeStarCount + 0.0f) / totalCount;
            textDungeonProgress.text = string.Format("{0}%", (int)(percent * 100));
            sliderDungeonProgress.value = percent;
        }
        private void RefreshHeroCollection()
        {
            List<RoleInfo> roleInfoList = IllustratedHandbookProxy.instance.GetIllustrationRoleList();

            int[] quality = new int[] { 2, 3, 4, 5 };
            int[] qualityHeroCount = new int[] { 0, 0, 0, 0 };
            int[] qualityHeroTotalCount = new int[] { 0, 0, 0, 0 };
            RoleInfo info;
            for (int i = 0, count = roleInfoList.Count; i < count; i++)
            {
                info = roleInfoList[i];
                for (int j = 0; j < 4; j++)
                {
					if ((int)info.heroData.roleQuality == quality[j])
                        qualityHeroCount[j]++;
                }
            }
			Dictionary<int, IllustratedData> heroDataDic = IllustratedData.IllustratedDataDictionary;
            HeroData heroData;
            foreach (var data in heroDataDic)
            {
				if(data.Value.atlas_type == 1) //英雄
				{
					for (int k = 0; k < 4; k++)
					{
						heroData = HeroData.GetHeroDataByID( data.Value.resData.id);
						if ((int)heroData.roleQuality  == quality[k])
						{
							qualityHeroTotalCount[k]++;
						}
					}
				}
                
            }
            for (int i = 0; i < textHeroCollection.Length; i++)
            {
                float percent = (qualityHeroCount[i] + 0.0f) / qualityHeroTotalCount[i];
				if(percent > 1)
					percent = 1;
                textHeroCollection[i].text = string.Format("{0}%", (int)(percent * 100));
                sliderHeroCollection[i].value = percent;
            }
        }
        private void RefreshSoundSprite()
        {
			imageSoundBg.SetSprite(toggleSoundBg.isOn ? _spSoundBgOn : _spSoundBgOff);
			imageSound.SetSprite(toggleSound.isOn ? _spSoundOn : _spSoundOff);
			imagePushMsg.SetSprite(toggleMsgPush.isOn ? _spPushMsgOn : _spPushMsgOff);
			imageEffectPlayable.SetSprite(toggleEffectPlayable.isOn ? _spEffectPlayableOn : _spEffectPlayableOff);
        }

        private int _frame = 30;
        private int _curFrame = 0;
        void Update()
        {
            if (_curFrame <= 0)
            {
                textSystemTime.text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", TimeController.instance.ServerTime);
                _curFrame = _frame;
            }
            _curFrame--;
        }

		public void ClickQuitGameButtonHandler ()
		{
		    if (Application.isEditor)
		    {
		        PlatformResultProxy.instance.PlatformLogoutSuccess("");
		    }
		    else
			PlatformProxy.instance.showSdkLogout();
		}

        public void ClickNameChangeBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
            AccountChangeNameView.Open();
        }
        public void ClickHeadChangeBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
            AccountChangeHeadIconView.Open();
        }
        public void ClickBindAccountBtnHandler()
        {

        }
        public void ClickSpendTipBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
            AccountCostTipView.Open();

        }
        // 0 音效 1背景音乐 2推送
        public void ClickToggleHandler(ToggleContent toggleContent)
        {
            switch (toggleContent.id)
            {
                case 0:
                    AudioController.instance.isOpenAudio = toggleSound.isOn;
                    break;
                case 1:
                    AudioController.instance.isOpenAudioBg = toggleSoundBg.isOn;
                    break;
                case 2:
                    GameSetting.instance.pushMessage = toggleMsgPush.isOn;
                    break;
                case 3:
                    GameSetting.instance.effectPlayable = toggleEffectPlayable.isOn;
                    break;
            }
            RefreshSoundSprite();
            AudioController.instance.SavePlayerPref();
        }
        public void ClickQQBtnHandler()
        {

        }
        public void ClickOfficialWebBtnHandler()
        {
            Application.OpenURL("www.hj.yukusoft.com");
        }
        public void ClickCloseBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        public void ClickGMBtnHandler()
        {
            Logic.UI.Chat.View.GMView.Open();
        }
		public void ClickCDkeyHandler()
		{
			LuaTable gameModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","game_model")[0];
			gameModel.GetLuaFunction("OpenCdkeyGiftView").Call();
		}
        //响应图鉴信息(C->S)
        private void LOBBY2CLIENT_IllustrationResp_handler()
        {
            RefreshHeroCollection();
            
        }
    }
}
