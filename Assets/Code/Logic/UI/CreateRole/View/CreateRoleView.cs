using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Localization;
using Common.Util;
using Logic.Avatar.Model;
using Logic.Player;
using Logic.Player.Model;
using Logic.Character;
using Logic.UI.CreateRole.Model;
using Logic.UI.CreateRole.Controller;
using Logic.Enums;
using LuaInterface;

namespace Logic.UI.CreateRole.View
{
    public class CreateRoleView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/create_role/create_role_view";

        private CharacterEntity _characterEntity;

        #region UI components
        public Transform heroModelRoot;
        public InputField playerNameInputField;

        public Text hairCutSettingText;
        public Text hairColorSettingText;
        public Text faceSettingText;

        public Text previousStepText;
        public Text createRoleText;
        public Text pleaseCustomizeYourRoleText;

        public List<Button> hairCutButtonList;
        public List<Button> hairColorButtonList;
        public List<Image> hairColorIconList;
//        public List<Button> faceButtonList;
		public List<Button> skinButtonList;
        public List<Image> skinIconList;

        public Transform selectedHairCutIndicator;
        public Transform selectedHairColorIndicator;
        public Transform selectedFaceIndicator;
        public Text skinTipsText;
        public List<Text> hairCutTipsText;
        public List<Text> hairColorTipsText;
        #endregion

        void Awake()
        {
            BindDelegate();
            Init();
            ClickRandomNameHandler();
        }

        void OnDestroy()
        {
            DespawnCharacter();
            UnbindDelegates();
        }

        private void BindDelegate()
        {
            CreateRoleProxy.instance.onHairCutChangedHandler += OnHairCutChangedHandler;
            CreateRoleProxy.instance.onHairColorChangedHandler += OnHairColorChangedHandler;
            CreateRoleProxy.instance.onFaceChangedHandler += OnFaceChangedHandler;
            CreateRoleProxy.instance.onSkinChangedHandler += OnSkinChangedHandler;
        }

        private void UnbindDelegates()
        {
            CreateRoleProxy.instance.onHairCutChangedHandler -= OnHairCutChangedHandler;
            CreateRoleProxy.instance.onHairColorChangedHandler -= OnHairColorChangedHandler;
            CreateRoleProxy.instance.onFaceChangedHandler -= OnFaceChangedHandler;
            CreateRoleProxy.instance.onSkinChangedHandler -= OnSkinChangedHandler;
        }

        private void Init()
        {
            hairCutSettingText.text = Localization.Get("ui.create_role_view.hair_cut_setting");
            hairColorSettingText.text = Localization.Get("ui.create_role_view.hair_color_setting");
            faceSettingText.text = Localization.Get("ui.create_role_view.face_setting");

            previousStepText.text = Localization.Get("ui.create_role_view.previous_step");
            createRoleText.text = Localization.Get("ui.create_role_view.create_role");
            pleaseCustomizeYourRoleText.text = Localization.Get("ui.create_role_view.please_customize_you_role");

            PlayerData selectedPlayerData = PlayerData.GetPlayerData(CreateRoleProxy.instance.selectedPlayerID);
            AvatarData avatarData = AvatarData.GetAvatarData(selectedPlayerData.avatarID);

            int faceIconCount = skinIconList.Count;
            for (int faceIconIndex = 0; faceIconIndex < faceIconCount; faceIconIndex++)
            {
                skinIconList[faceIconIndex].SetSprite(ResMgr.instance.Load<Sprite>(avatarData.faceIconPaths[faceIconIndex]));
                skinIconList[faceIconIndex].SetNativeSize();
            }

            for (int i = 0, count = hairCutTipsText.Count; i < count; i++)
            {
                hairCutTipsText[i].text = Localization.Get("ui.create_role_view.hairCutTips_" + (i + 1));
            }

            for (int i = 0, count = hairColorTipsText.Count; i < count; i++)
            {
                hairColorTipsText[i].text = Localization.Get("ui.create_role_view.hairColorTips_" + (i + 1));
            }

            Debugger.Log("Create Role");
            _characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(selectedPlayerData);
            if (_characterEntity == null) return;
            TransformUtil.SwitchLayer(_characterEntity.transform, (int)LayerType.UI);
            _characterEntity.transform.SetParent(heroModelRoot, false);
            _characterEntity.transform.localPosition = Vector3.zero;
            _characterEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _characterEntity.transform.localScale = Vector3.one;

            Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = _characterEntity.gameObject.AddComponent<Logic.Model.View.ModelRotateAndAnim>();
            modelRotateAndAnim.canClick = true;
            modelRotateAndAnim.stateNameHash = Common.Animators.AnimatorUtil.VICTORY_02_ID;
            modelRotateAndAnim.ClickBehavior();
            CapsuleCollider capsuleCollider = _characterEntity.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2.5f;
            capsuleCollider.radius = 0.5f;
            capsuleCollider.center = new Vector3(0f, 1.2f, 0f);
            _characterEntity.transform.tag = "Character";

            CreateRoleController.instance.SelectHairCut(0);
            CreateRoleController.instance.SelectHairColor(0);
            CreateRoleController.instance.SelectFace(0);
            CreateRoleController.instance.SelectSkin(0);

        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        private void ChangeHairColor(int index)
        {
            Debugger.Log("ChangeHairColor:" + index);
            PlayerData selectedPlayerData = PlayerData.GetPlayerData(CreateRoleProxy.instance.selectedPlayerID);
            AvatarData avatarData = AvatarData.GetAvatarData(selectedPlayerData.avatarID);
            for (int i = 0, count = hairColorIconList.Count; i < count; i++)
            {
                int pathIndex = index * 4 + i;
                hairColorIconList[i].sprite = ResMgr.instance.Load<Sprite>(avatarData.hairColorIconPaths[pathIndex]);
            }
        }

        #region proxy event handlers
        private void OnHairCutChangedHandler(uint selectedHairCutIndex, uint hairColorRealIndex)
        {
            Debugger.Log(string.Format("********** Select Hair Cut Index: {0} **********", selectedHairCutIndex));
            if (_characterEntity != null)
            {
                PlayerEntityUtil.ChangeHair(CreateRoleProxy.instance.selectedPlayerID, selectedHairCutIndex, _characterEntity);
                StartCoroutine(ChangeHairCoroutine(selectedHairCutIndex, hairColorRealIndex));
            }
            Button selectedHairCutButton = hairCutButtonList[(int)selectedHairCutIndex];
            selectedHairCutIndicator.position = selectedHairCutButton.transform.position;
            ChangeHairColor((int)selectedHairCutIndex);
        }

        private void OnHairColorChangedHandler(uint hairCutIndex, uint hairColorRealIndex)
        {
            Debugger.Log(string.Format("********** Select Hair Color Index: {0} **********", hairColorRealIndex));
            if (_characterEntity != null)
            {
                StartCoroutine(ChangeHairCoroutine(hairCutIndex, hairColorRealIndex));
            }
        }

        //必须等一帧，不然先换了颜色，后换模型，导致颜色贴图找不到
        private IEnumerator ChangeHairCoroutine(uint hairCutIndex, uint hairColorRealIndex)
        {
            yield return null;
            PlayerEntityUtil.ChangeHairColor(CreateRoleProxy.instance.selectedPlayerID, hairCutIndex, hairColorRealIndex, _characterEntity);
        }

        private void OnFaceChangedHandler(uint selectedFaceIndex)
        {
//            Debugger.Log(string.Format("********** Select Face Index: {0} **********", selectedFaceIndex));
//            if (_characterEntity != null)
//            {
//                PlayerEntityUtil.ChangeFace(CreateRoleProxy.instance.selectedPlayerID, selectedFaceIndex, _characterEntity);
//            }
//            Button selectedFaceButton = faceButtonList[(int)selectedFaceIndex];
//            selectedFaceIndicator.position = selectedFaceButton.transform.position;
        }

        private void OnSkinChangedHandler(int selectedSkinIndex)
        {
            Debugger.Log(string.Format("********** Select Skin Index: {0} **********", selectedSkinIndex));
            if (_characterEntity != null)
            {
                int index = selectedSkinIndex;
                index++;
                skinTipsText.text = Localization.Get("ui.create_role_view.bodyTips_" + index);
                PlayerEntityUtil.ChangeSkin(CreateRoleProxy.instance.selectedPlayerID, selectedSkinIndex, _characterEntity);
            }
			Button selectedSkinButton = skinButtonList[selectedSkinIndex];
			selectedFaceIndicator.position = selectedSkinButton.transform.position;
        }
        #endregion

        #region UI event handlers
        public void ClickRandomNameHandler()
        {
            playerNameInputField.text = Logic.UI.RandomName.Model.RandomNameProxy.instance.RandomName(Enums.GenderType.Female);//gender
        }

        public void ClickHairCutButtonHandler(Button hairCutButton)
        {
            if (hairCutButtonList.Contains(hairCutButton))
            {
                int index = hairCutButtonList.IndexOf(hairCutButton);
                CreateRoleController.instance.SelectHairCut((uint)index);
            }
        }

        public void ClickHairColorButtonHandler(Button hairColorButton)
        {
            if (hairColorButtonList.Contains(hairColorButton))
            {
                int index = hairColorButtonList.IndexOf(hairColorButton);
                Button selectedHairColorButton = hairColorButtonList[index];
                selectedHairColorIndicator.localPosition = selectedHairColorButton.transform.localPosition + Vector3.up * 5;
                CreateRoleController.instance.SelectHairColor((uint)index);
            }
        }

//        public void ClickFaceButtonHandler(Button faceButton)
//        {
//            if (faceButtonList.Contains(faceButton))
//            {
//                int index = faceButtonList.IndexOf(faceButton);
//                CreateRoleController.instance.SelectFace((uint)index);
//            }
//        }

		public void ClickSkinButtonHandler (Button skinButton)
		{
			if (skinButtonList.Contains(skinButton))
			{
				CreateRoleProxy.instance.selectedSkinIndex = skinButtonList.IndexOf(skinButton);
				CreateRoleController.instance.SelectSkin(CreateRoleProxy.instance.selectedSkinIndex);
			}
		}

        public void ClickBackButtonHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
            UIMgr.instance.Open(SelectRoleView.PREFAB_PATH);
        }

        public void ClickRightButtonHandler()
        {
            CreateRoleController.instance.SelectSkin(++CreateRoleProxy.instance.selectedSkinIndex);
        }

        public void ClickLeftButtonHandler()
        {
            CreateRoleController.instance.SelectSkin(--CreateRoleProxy.instance.selectedSkinIndex);
        }

        public void ClickCreateRoleButtonHandler()
        {
            string playerName = playerNameInputField.text;

            int playerNameByteLength = 0;
            char[] playerNameCharArray = playerName.ToCharArray();
            for (int i = 0; i < playerNameCharArray.Length; i++)
            {
                if ((int)playerNameCharArray[i] > 127)
                {
                    playerNameByteLength += 2;     // Chinses
                }
                else
                {
                    playerNameByteLength += 1;     // English and Number etc.
                }
            }

            if (playerNameByteLength > 14)
            {
                Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.create_role_view.player_name_too_long"));
                return;
            }

            bool hasBlockWord = Common.Util.BlackListWordUtil.HasBlockWords(playerName);
            if (hasBlockWord)
            {
                Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.create_role_view.hasBlockWords"));
                return;
            }
            uint roleID = CreateRoleProxy.instance.selectedPlayerID;
            //uint hairCutIndex = CreateRoleProxy.instance.selectedHairCutIndex;
            //uint hairColorIndex = CreateRoleProxy.instance.selectedHairColorIndex;
            uint faceIndex = CreateRoleProxy.instance.selectedFaceIndex;
            int skinId = CreateRoleProxy.instance.selectedSkinIndex;
            CreateRoleController.instance.CreateRole(playerName, roleID, CreateRoleProxy.instance.selectedHairCutIndex, CreateRoleProxy.instance.HairColorRealIndex, faceIndex, skinId);
//            Logic.UI.Tutorial.View.ConfirmPlayTutorialView.Open(CreateRole, CreateRole);
        }

        public void CreateRole()
        {
            string playerName = playerNameInputField.text;
            uint roleID = CreateRoleProxy.instance.selectedPlayerID;
            uint faceIndex = CreateRoleProxy.instance.selectedFaceIndex;
            int skinId = CreateRoleProxy.instance.selectedSkinIndex;
            CreateRoleController.instance.CreateRole(playerName, roleID, CreateRoleProxy.instance.selectedHairCutIndex, CreateRoleProxy.instance.HairColorRealIndex, faceIndex, skinId);
        }
        #endregion
    }
}