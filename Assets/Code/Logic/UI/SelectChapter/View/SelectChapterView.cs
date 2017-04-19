using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.Enums;
using Logic.Dungeon.Model;
using Logic.Chapter.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.DailyDungeon.View;
using Logic.Player.Model;
using Logic.Character;
using Logic.Game.Model;
using Common.Animators;
using Logic.Audio.Controller;
using LuaInterface;
using Common.ResMgr;
using Logic.UI.SelectChapter.View;

namespace Logic.UI.SelectChapter.View
{
    public class SelectChapterView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/chapter/select_chapter_view";

        private Dictionary<int, DungeonButton> _easyDungeonButtonDic = new Dictionary<int, DungeonButton>();
        private Dictionary<int, DungeonButton> _normalDungeonButtonDic = new Dictionary<int, DungeonButton>();
        private Dictionary<int, DungeonButton> _hardDungeonButtonDic = new Dictionary<int, DungeonButton>();
        private DungeonType _currentSelectDungeonType;
		private ChapterData _currentSelectChapterData;

        private Vector2 _chapterScrollViewportSize = Vector2.zero;
        private CharacterEntity _characterEntity;

		private List<ChapterView> _chapterViewList = new List<ChapterView>();
		private Dictionary<int, ChapterView> _chapterViewsDictionary = new Dictionary<int, ChapterView>();

        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public SelectChapterScrollRect selectChapterScrollRect;
        public Transform chapterBGsRoot;
        public Transform dungeonButtonsRoot;
        public ChapterView chapterViewPrefab;
        public DungeonButton dungeonButtonPrefab;

		public Color selectedDungeonTypeTextColor;

		public Button previousChapterButton;
		public Button nextChapterButton;

		public Button easyButton;
		public Button normalButton;
		public Button hardButton;

        public RectTransform roleModelRootRectTransform;
        public Transform systemNoticeRoot;
		
		public Text chapterStarCountText;
		public Slider chapterStarCollectSlider;

		public GameObject chest1GameObject;
		public GameObject chest2GameObject;
		public GameObject chest3GameObject;

		public Image chest1Image;
		public Image chest2Image;
		public Image chest3Image;

		public GameObject chest1CanDrawFXGameObject;
		public GameObject chest2CanDrawFXGameObject;
		public GameObject chest3CanDrawFXGameObject;

		public Text chest1NameText;
		public Text chest2NameText;
		public Text chest3NameText;

		public GameObject easyButtonParticleEffectRootGameObject;
		public GameObject normalButtonParticleEffectRootGameObject;
		public GameObject hardButtonParticleEffectRootGameObject;

		public GameObject easyParticleRootGameObject;
		public GameObject normalParticleRootGameObject;
		public GameObject hardParticleRootGameObject;
        #endregion UI components

        void Awake()
        {
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsCommonStyle(string.Empty, ClickCloseHandler, true, true, true, false);
            _commonTopBarView.transform.localPosition = new Vector3(_commonTopBarView.transform.localPosition.x, _commonTopBarView.transform.localPosition.y, -1000);

            Logic.UI.Chat.View.SystemNoticeView.Create(systemNoticeRoot);

            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
            _characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, roleModelRootRectTransform, false, false);
            AnimatorUtil.SetBool(_characterEntity.anim, AnimatorUtil.RUN, true);

            chapterViewPrefab.gameObject.SetActive(true);
            List<ChapterInfo> chapterInfoList = ChapterProxy.instance.GetAllChapterInfos();
            int chapterInfoCount = chapterInfoList.Count;
            for (int i = 0; i < chapterInfoCount; i++)
            {
                ChapterInfo chapterInfo = chapterInfoList[i];
                ChapterView chapterView = GameObject.Instantiate<ChapterView>(chapterViewPrefab);
//                chapterView.SetChapterInfoAndType(chapterInfo);
                chapterView.transform.SetParent(chapterBGsRoot, false);
                chapterView.transform.localScale = Vector3.one;
                chapterView.transform.localPosition = chapterInfo.chapterData.chapterPosition;
				_chapterViewList.Add(chapterView);
				_chapterViewsDictionary.Add(chapterInfo.chapterData.Id, chapterView);

                int dungeonsCount = chapterInfo.chapterData.easyDungeonIDList.Count;
                for (int dungeonIndex = 0; dungeonIndex < dungeonsCount; dungeonIndex++)
                {
                    int easyDungeonID = chapterInfo.chapterData.easyDungeonIDList[dungeonIndex];
                    int normalDungeonID = chapterInfo.chapterData.normalDungeonIDList[dungeonIndex];
                    int hardDungeonID = chapterInfo.chapterData.hardDungeonIDList[dungeonIndex];

                    DungeonButton dungeonButton = GameObject.Instantiate<DungeonButton>(dungeonButtonPrefab);
                    dungeonButton.transform.SetParent(dungeonButtonsRoot, false);
                    dungeonButton.transform.localPosition = chapterInfo.chapterData.chapterPosition + chapterInfo.chapterData.positions[dungeonIndex];
                    dungeonButton.SetDungeonInfo(DungeonProxy.instance.GetDungeonInfo(chapterInfo.chapterData.easyDungeonIDList[dungeonIndex]));
                    dungeonButton.name = string.Format("dungeon_button_{0}", dungeonButton.DungeonInfo.dungeonData.dungeonID);
                    dungeonButton.gameObject.SetActive(true);
                    _easyDungeonButtonDic.Add(easyDungeonID, dungeonButton);
                    _normalDungeonButtonDic.Add(normalDungeonID, dungeonButton);
                    _hardDungeonButtonDic.Add(hardDungeonID, dungeonButton);
                }
                dungeonButtonPrefab.gameObject.SetActive(false);
            }
            chapterViewPrefab.gameObject.SetActive(false);
            selectChapterScrollRect.content.sizeDelta = RectTransformUtility.CalculateRelativeRectTransformBounds(chapterBGsRoot).size;
			selectChapterScrollRect.onEndDragDelegate += OnMapEndDragHandler;
			selectChapterScrollRect.onDragLeftDelegate += OnMapDragLeftHandler;
			selectChapterScrollRect.onDragRightDelegate += OnMapDragRightHandler;
            _chapterScrollViewportSize = selectChapterScrollRect.viewport.rect.size;
            AudioController.instance.PlayBGMusic(Logic.Audio.Controller.AudioController.SELECTDUNGEON);
			BindDelegate();
        }

        void Start()
        {
            Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
        }
		
		void OnDestroy()
		{
			selectChapterScrollRect.onEndDragDelegate -= OnMapEndDragHandler;
			LeanTween.cancel(gameObject);
			DespawnCharacter();
			UnbindDelegate();
		}

		void BindDelegate ()
		{
			DungeonProxy.instance.onDungeonInfosUpdateDelegate += OnDungeonInfosUpdateHandler;
			ChapterProxy.instance.onDungeonStarBoxInfoUpdateDelegate += OnDungeonStarBoxInfoUpdateHandler;
		}

		void UnbindDelegate ()
		{
			DungeonProxy.instance.onDungeonInfosUpdateDelegate -= OnDungeonInfosUpdateHandler;
			ChapterProxy.instance.onDungeonStarBoxInfoUpdateDelegate -= OnDungeonStarBoxInfoUpdateHandler;
		}

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        public void SetSelectDungeon(int dungeonID)
        {
            DungeonType dungeonType = DungeonData.GetDungeonDataByID(dungeonID).dungeonType;
            if (_currentSelectDungeonType != dungeonType)
            {
                ResetDifficulty(dungeonType);
            }
            DungeonButton dungeonButton = null;
            if (_easyDungeonButtonDic.ContainsKey(dungeonID))
            {
                _easyDungeonButtonDic.TryGetValue(dungeonID, out dungeonButton);
            }
            else if (_normalDungeonButtonDic.ContainsKey(dungeonID))
            {
                _normalDungeonButtonDic.TryGetValue(dungeonID, out dungeonButton);
            }
            else if (_hardDungeonButtonDic.ContainsKey(dungeonID))
            {
                _hardDungeonButtonDic.TryGetValue(dungeonID, out dungeonButton);
            }

			ChapterData chapterData = ChapterData.GetChapterDataContainsDungeon(dungeonID);
			MoveTo(chapterData);
        }

		private void MoveTo (ChapterData chapterData, bool withTweenAnimation = false)
		{
			selectChapterScrollRect.StopMovement();
			_currentSelectChapterData = chapterData;

			CanvasScaler parentCanvasScaler = GetComponentInParent<CanvasScaler>();
			float diffx = 0;
			Vector2 destPos = Vector2.zero;

			if (parentCanvasScaler.matchWidthOrHeight <= 0)
			{
				diffx = (960f - ChapterView.CHAPTER_BG_WIDTH) * 0.5f;
				destPos = -_currentSelectChapterData.chapterPosition + new Vector2(diffx ,0);
			}
			else if (parentCanvasScaler.matchWidthOrHeight >= 1)
			{
				diffx = (((float)Screen.width / Screen.height) * 640 - ChapterView.CHAPTER_BG_WIDTH) * 0.5f;
				destPos = -_currentSelectChapterData.chapterPosition + new Vector2(diffx ,0);
			}

			if (withTweenAnimation)
			{
				LTDescr ltDescr = LeanTween.value(gameObject, selectChapterScrollRect.content.anchoredPosition, destPos, 0.25f);
				ltDescr.setOnUpdateVector2(OnUpdateMapPosition);
			}
			else
			{
				selectChapterScrollRect.content.anchoredPosition = destPos;
			}

			ChapterView chapterView = _chapterViewsDictionary.GetValue(chapterData.Id);
			chapterView.Show();
			_commonTopBarView.titleText.text = string.Format(Localization.Get("ui.select_chapter_view.title_template"), UIUtil.GetChineseNumberString(chapterData.Id), Localization.Get(chapterData.name));
			RefreshStarChests();
			RefreshPreviousAndNextChapterButtons();
		}

		private void OnUpdateMapPosition (Vector2 anchoredPosition)
		{
			selectChapterScrollRect.content.anchoredPosition = anchoredPosition;
		}

        private void ResetDifficulty(DungeonType dungeonType)
        {
            _currentSelectDungeonType = dungeonType;
			DungeonProxy.instance.DungeonModelLuaTable.GetLuaFunction("SetPveSelectDungeonType").Call((int)dungeonType);
            if (_currentSelectDungeonType == DungeonType.Easy)
            {
                List<int> easyDungeonIDs = _easyDungeonButtonDic.GetKeys();
                int easyDungeonIDCount = easyDungeonIDs.Count;
                for (int i = 0; i < easyDungeonIDCount; i++)
                {
                    int dungeonID = easyDungeonIDs[i];
                    _easyDungeonButtonDic[dungeonID].SetDungeonInfo(DungeonProxy.instance.GetDungeonInfo(dungeonID));
                }
                ResetRoleModel(_easyDungeonButtonDic[DungeonProxy.instance.LastUnlockEasyDungeonID].GetComponent<RectTransform>().anchoredPosition);
            }
            else if (_currentSelectDungeonType == DungeonType.Normal)
            {
                List<int> normalDungeonIDs = _normalDungeonButtonDic.GetKeys();
                int normalDungeonIDCount = normalDungeonIDs.Count;
                for (int i = 0; i < normalDungeonIDCount; i++)
                {
                    int dungeonID = normalDungeonIDs[i];
                    _normalDungeonButtonDic[dungeonID].SetDungeonInfo(DungeonProxy.instance.GetDungeonInfo(dungeonID));
                }
                ResetRoleModel(_normalDungeonButtonDic[DungeonProxy.instance.LastUnlockNormalDungeonID].GetComponent<RectTransform>().anchoredPosition);
            }
            else if (_currentSelectDungeonType == DungeonType.Hard)
            {
                List<int> hardDungeonIDs = _hardDungeonButtonDic.GetKeys();
                int hardDungeonIDCount = hardDungeonIDs.Count;
                for (int i = 0; i < hardDungeonIDCount; i++)
                {
                    int dungeonID = hardDungeonIDs[i];
                    _hardDungeonButtonDic[dungeonID].SetDungeonInfo(DungeonProxy.instance.GetDungeonInfo(dungeonID));
                }
                ResetRoleModel(_hardDungeonButtonDic[DungeonProxy.instance.LastUnlockHardDungeonID].GetComponent<RectTransform>().anchoredPosition);
            }

			easyButton.transform.localScale = new Vector3(0.9f, 0.9f, 1);
			UIUtil.SetGrayExcludeUITextOutline(easyButton.gameObject, true);
			normalButton.transform.localScale = new Vector3(0.9f, 0.9f, 1);
			UIUtil.SetGrayExcludeUITextOutline(normalButton.gameObject, true);
			hardButton.transform.localScale = new Vector3(0.9f, 0.9f, 1);
			UIUtil.SetGrayExcludeUITextOutline(hardButton.gameObject, true);
			if (_currentSelectDungeonType == DungeonType.Easy)
			{
				easyButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
				UIUtil.SetGrayExcludeUITextOutline(easyButton.gameObject, false);
			}
			else if (_currentSelectDungeonType == DungeonType.Normal)
			{
				normalButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
				UIUtil.SetGrayExcludeUITextOutline(normalButton.gameObject, false);
			}
			else if (_currentSelectDungeonType == DungeonType.Hard)
			{
				hardButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
				UIUtil.SetGrayExcludeUITextOutline(hardButton.gameObject, false);
			}

			List<ChapterInfo> chapterInfoList = ChapterProxy.instance.GetAllChapterInfos();
			ChapterInfo chapterInfo = null;
			ChapterView chapterView = null;
			for (int i = 0, count = _chapterViewList.Count; i < count; i++)
			{
				chapterInfo = chapterInfoList[i];
				chapterView = _chapterViewList[i];
				if (!chapterInfo.IsLock(_currentSelectDungeonType))
				{
					chapterView.SetChapterInfoAndType(chapterInfo);
					chapterView.gameObject.SetActive(true);
				}
				else
				{
					chapterView.gameObject.SetActive(false);
				}

			}
			selectChapterScrollRect.content.sizeDelta = RectTransformUtility.CalculateRelativeRectTransformBounds(chapterBGsRoot).size;

			int lastUnlockedDungeonID = Logic.Dungeon.Model.DungeonProxy.instance.GetLastUnlockDungeonID(dungeonType);
			this.SetSelectDungeon(lastUnlockedDungeonID);
			ChapterProxy.instance.SetLastSelect(_currentSelectDungeonType, 0);
			DungeonProxy.instance.LastSelectPVEDungeonType = dungeonType;

			easyButtonParticleEffectRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Easy);
			normalButtonParticleEffectRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Normal);
			hardButtonParticleEffectRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Hard);

			easyParticleRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Easy);
			normalParticleRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Normal);
			hardParticleRootGameObject.SetActive(_currentSelectDungeonType == DungeonType.Hard);
        }

        private void ResetRoleModel(Vector2 position)
        {
            roleModelRootRectTransform.anchoredPosition = position;
        }

		private void RefreshPreviousAndNextChapterButtons ()
		{
			int currentSelectChapterLastDungeonID = _currentSelectChapterData.GetChapterDungeonIDListOfDungeonType(_currentSelectDungeonType).Last();
			DungeonInfo currentSelectChapterLastDungeonInfo = DungeonProxy.instance.GetDungeonInfo(currentSelectChapterLastDungeonID);

			bool shouldShowPreviousChapterButton = _currentSelectChapterData.Id > 1;
			bool shouldShowNextChapterButton = _currentSelectChapterData.NextChapterData != null && currentSelectChapterLastDungeonInfo.star > 0;
			previousChapterButton.gameObject.SetActive(shouldShowPreviousChapterButton);
			nextChapterButton.gameObject.SetActive(shouldShowNextChapterButton);
		}

		private void RefreshChapterStarCollectSlider()
		{
			int playerGainChapterStarCount = DungeonProxy.instance.GetPlayerGainStarCountOfChapterOfDungeonType(_currentSelectDungeonType, _currentSelectChapterData.Id);
			int chapterTotalStarCount = DungeonProxy.instance.GetTotalStarCountOfChapterOfDungeonType(_currentSelectDungeonType, _currentSelectChapterData.Id);
			chapterStarCollectSlider.value = (float)playerGainChapterStarCount / chapterTotalStarCount;
		}

		private void RefreshStarChests ()
		{
			int playerGainChapterStarCount = DungeonProxy.instance.GetPlayerGainStarCountOfChapterOfDungeonType(_currentSelectDungeonType, _currentSelectChapterData.Id);
			int chapterTotalStarCount = DungeonProxy.instance.GetTotalStarCountOfChapterOfDungeonType(_currentSelectDungeonType, _currentSelectChapterData.Id);
			chapterStarCountText.text = string.Format(Localization.Get("common.value/max"), playerGainChapterStarCount, chapterTotalStarCount);
			DungeonStarData chest1DungeonStarData = DungeonStarData.GetDungeonStarData(_currentSelectDungeonType, _currentSelectChapterData.Id, 1);
			DungeonStarData chest2DungeonStarData = DungeonStarData.GetDungeonStarData(_currentSelectDungeonType, _currentSelectChapterData.Id, 2); 
			DungeonStarData chest3DungeonStarData = DungeonStarData.GetDungeonStarData(_currentSelectDungeonType, _currentSelectChapterData.Id, 3); 

			chest1GameObject.SetActive(chest1DungeonStarData != null);
			chest2GameObject.SetActive(chest2DungeonStarData != null);
			chest3GameObject.SetActive(chest3DungeonStarData != null);

			chest1CanDrawFXGameObject.SetActive(false);
			chest2CanDrawFXGameObject.SetActive(false);
			chest3CanDrawFXGameObject.SetActive(false);

			LuaTable chapterModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "chapter_model")[0];
			if (chest1DungeonStarData != null)
			{
				chest1NameText.text = chest1DungeonStarData.starNumber.ToString();
				bool hasReceived = chapterModelLuaTable.GetLuaFunction("DungeonStarBoxHasReceived").Call(chest1DungeonStarData.id)[0].ToString().ToBoolean();
				if (hasReceived)
				{
					chest1Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox2_2");
				}
				else
				{
					chest1Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox2_1");
				}
				chest1Image.SetNativeSize();
				chest1CanDrawFXGameObject.SetActive(playerGainChapterStarCount >= chest1DungeonStarData.starNumber && !hasReceived);
			}
			if (chest2DungeonStarData != null)
			{
				chest2NameText.text = chest2DungeonStarData.starNumber.ToString();
				bool hasReceived = chapterModelLuaTable.GetLuaFunction("DungeonStarBoxHasReceived").Call(chest2DungeonStarData.id)[0].ToString().ToBoolean();
				if (hasReceived)
				{
					chest2Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox3_2");
				}
				else
				{
					chest2Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox3_1");
				}
				chest2Image.SetNativeSize();
				chest2CanDrawFXGameObject.SetActive(playerGainChapterStarCount >= chest2DungeonStarData.starNumber && !hasReceived);
			}
			if (chest3DungeonStarData != null)
			{
				chest3NameText.text = chest3DungeonStarData.starNumber.ToString();
				bool hasReceived = chapterModelLuaTable.GetLuaFunction("DungeonStarBoxHasReceived").Call(chest3DungeonStarData.id)[0].ToString().ToBoolean();
				if (hasReceived)
				{
					chest3Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox4_2");
				}
				else
				{
					chest3Image.sprite = ResMgr.instance.LoadSprite("sprite/main_ui/jdtbox4_1");
				}
				chest3Image.SetNativeSize();
				chest3CanDrawFXGameObject.SetActive(playerGainChapterStarCount >= chest3DungeonStarData.starNumber && !hasReceived);
			}

			RefreshChapterStarCollectSlider();
		}

		private void MoveToPreviousChapter ()
		{
			if (_currentSelectChapterData.PreviousChapterData != null)
			{
				MoveTo(_currentSelectChapterData.PreviousChapterData, false);
			}
		}

		private void MoveToNextChapter ()
		{
			int currentSelectChapterLastDungeonID = _currentSelectChapterData.GetChapterDungeonIDListOfDungeonType(_currentSelectDungeonType).Last();
			DungeonInfo currentSelectChapterLastDungeonInfo = DungeonProxy.instance.GetDungeonInfo(currentSelectChapterLastDungeonID);
			bool canMoveToNextChapter = _currentSelectChapterData.NextChapterData != null && currentSelectChapterLastDungeonInfo.star > 0;
			if (canMoveToNextChapter)
			{
				MoveTo(_currentSelectChapterData.NextChapterData, false);
			}
			else
			{
				MoveTo(_currentSelectChapterData, false);
			}
		}

        #region ui event handlers
		void OnMapEndDragHandler ()
		{
			if (Mathf.Abs(selectChapterScrollRect.content.anchoredPosition.x) < _currentSelectChapterData.chapterPosition.x - 200)
			{
				MoveToPreviousChapter();
			}
			else if (Mathf.Abs(selectChapterScrollRect.content.anchoredPosition.x) > _currentSelectChapterData.chapterPosition.x + 200)
			{
				MoveToNextChapter();
			}
			else
			{
				MoveTo(_currentSelectChapterData, false);
			}
		}

		void OnMapDragLeftHandler ()
		{
			MoveToNextChapter();
		}

		void OnMapDragRightHandler ()
		{
			MoveToPreviousChapter();
		}

        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
            Logic.Audio.Controller.AudioController.instance.PlayBGMusic(Logic.Audio.Controller.AudioController.MAINSCENE);

        }

        public void ClickEasyHandler()
        {
            ResetDifficulty(DungeonType.Easy);
        }

        public void ClickNormalHandler()
        {
            if (!Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.PVE_Normal, true))
            {
                return;
            }
            ResetDifficulty(DungeonType.Normal);
        }

        public void ClickHardHandler()
        {
            if (!Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.PVE_Hard, true))
            {
                return;
            }
            ResetDifficulty(DungeonType.Hard);
        }

		public void ClickBossDungeonList ()
		{
			LuaTable chapterControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "chapter_controller")[0];
			chapterControllerLuaTable.GetLuaFunction("OpenBossDungeonListView").Call((int)_currentSelectDungeonType);
		}

		public void ClickPreviousChapterButtonHandler ()
		{
			if (_currentSelectChapterData.PreviousChapterData != null)
			{
				MoveTo(_currentSelectChapterData.PreviousChapterData, false);
			}
		}

		public string GetDifficultyNameByDungeonType (DungeonType dungeonType)
		{
			string difficultyName = "";
			if (dungeonType == DungeonType.Easy)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.easy_type");
			}
			else if (dungeonType == DungeonType.Normal)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.normal_type");
			}
			else if (dungeonType == DungeonType.Hard)
			{
				difficultyName = Localization.Get("ui.dungeon_detail_view.hard_type");
			}
			return difficultyName;
		}

		public void ClickNextChapterButtonHandler ()
		{
			if (_currentSelectChapterData.NextChapterData != null)
			{
				int nextChapterFirstDungeonID = _currentSelectChapterData.NextChapterData.GetChapterDungeonIDListOfDungeonType(_currentSelectDungeonType).First();
				DungeonInfo nextChapterFirstDungeonInfo = DungeonProxy.instance.GetDungeonInfo(nextChapterFirstDungeonID);
				if (nextChapterFirstDungeonInfo.isLock)
				{
					int accountLevel = Game.Model.GameProxy.instance.AccountLevel;
					string tips = "";
					if (nextChapterFirstDungeonInfo.dungeonData.unlockLevel > 0)
					{
						if (accountLevel < nextChapterFirstDungeonInfo.dungeonData.unlockLevel)
						{
							tips = string.Format(Localization.Get("ui.select_chapter_view.chapter_unlock_level_not_enough"), nextChapterFirstDungeonInfo.dungeonData.unlockLevel);
							Tips.View.CommonAutoDestroyTipsView.Open(tips);
							return;
						}
					}
					if (nextChapterFirstDungeonInfo.dungeonData.unlockDungeonIDPre1 > 0)
					{
						DungeonInfo preDungeonInfo = DungeonProxy.instance.GetDungeonInfo(nextChapterFirstDungeonInfo.dungeonData.unlockDungeonIDPre1);
						string preDungeonName = GetDifficultyNameByDungeonType(preDungeonInfo.dungeonData.dungeonType) + preDungeonInfo.dungeonData.GetOrderName();
						if (preDungeonInfo.star <= 0)
						{
							tips = string.Format(Localization.Get("ui.select_chapter_view.chapter_unlock_need_pass_dungeon"), preDungeonName);
							return;
						}
					}
					if (nextChapterFirstDungeonInfo.dungeonData.unlockDungeonIDPre2 > 0)
					{
						DungeonInfo preDungeonInfo = DungeonProxy.instance.GetDungeonInfo(nextChapterFirstDungeonInfo.dungeonData.unlockDungeonIDPre2);
						string preDungeonName = GetDifficultyNameByDungeonType(preDungeonInfo.dungeonData.dungeonType) + preDungeonInfo.dungeonData.GetOrderName();
						if (preDungeonInfo.star <= 0)
						{
							tips = string.Format(Localization.Get("ui.select_chapter_view.chapter_unlock_need_pass_dungeon"), preDungeonName);
							return;
						}
					}
					if (nextChapterFirstDungeonInfo.dungeonData.unlockStarCount > 0)
					{
						int ownStarCount = DungeonProxy.instance.GetTotalStarCountOfDungeonType(nextChapterFirstDungeonInfo.dungeonData.unlockStarDungeonType);
						if (ownStarCount < nextChapterFirstDungeonInfo.dungeonData.unlockStarCount)
						{
							tips = string.Format(Localization.Get("ui.select_chapter_view.chapter_unlock_level_star_count_not_enough"), nextChapterFirstDungeonInfo.dungeonData.unlockStarCount);
							return;
						}
					}
				}
				else
				{
					MoveTo(_currentSelectChapterData.NextChapterData, false);
				}
			}
		}

		public void ClickChest (int chestPosition)
		{
			DungeonStarData dungeonStarData = DungeonStarData.GetDungeonStarData(_currentSelectDungeonType, _currentSelectChapterData.Id, chestPosition);

			LuaTable chapterControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "chapter_controller")[0];
			chapterControllerLuaTable.GetLuaFunction("OpenDungeonStarBoxDetailView").Call(dungeonStarData.id);
		}
        #endregion ui event handlers

		#region proxy callback
		void OnDungeonStarBoxInfoUpdateHandler ()
		{
			RefreshStarChests();
		}

		void OnDungeonInfosUpdateHandler ()
		{
			ResetDifficulty(_currentSelectDungeonType);
		}
		#endregion proxy callback
    }
}