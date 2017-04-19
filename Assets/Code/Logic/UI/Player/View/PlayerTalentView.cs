using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.UI.CommonReward.View;
using Logic.UI.Hero.View;
using Logic.Role;
using Logic.Hero.Model;
using Logic.Enums;
using Logic.Character;
using Logic.Player;
using Logic.Player.Model;
using Common.ResMgr;
using Common.Localization;
using Logic.Skill.Model;
using Logic.Item.Model;
using Logic.Player.Controller;
using Logic.UI.Tips.View;
using Logic.Pet.Model;

namespace Logic.UI.Player.View
{
    public class PlayerTalentView : MonoBehaviour
    {

        public const string PREFAB_PATH = "ui/player/player_talent_view";
        public static PlayerTalentView Open()
        {
            PlayerTalentView view = UIMgr.instance.Open<PlayerTalentView>(PREFAB_PATH);
            return view;
        }
        public Transform core;
        public Transform PassiveChoiceOneRoot;
        public Transform SummonChoiceOneRoot;
        public Transform PassiveRoot;
        public Transform materialRoot;
        public Transform attrRoot;
        public RoleAttributeView attrPrefab;
        public SkillTalentButton skillButtonPrefab;
        public GameObject materialPrefab;
        public GameObject materialRootGO;
        public Text materialTipText;
        public Text activeButtonText;
        public Text activeNeedExpText;
        public GameObject activeNeedExpGO;
        public Transform selectTransform;
        public Text passiveChoiceOneText;
        public Text summonChoiceOneText;


        public Text playerNameText;
        public Text playerLvText;
        public Text playerStrengthenLvText;
        public Transform playerModelRoot;
        public Image playerTypeImage;
        public Image activePassiveImage;
        public Image activeSummonImage;

        private PlayerSkillTalentInfo _currentClickSkillInfo;
        private int _activePassiveId;
        private int _activeSummonId;
        private PetEntity _petEntiy;
		private Canvas _canvas;
        void Start()
        {
            BindDelegate();
            CommonTopBar.View.CommonTopBarView barView = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(core);
            barView.SetAsCommonStyle(Localization.Get("ui.player_cultivate_view.title"), ClickCloseBtnHander, false, false, false, false);

            skillButtonPrefab.gameObject.SetActive(false);
            materialPrefab.SetActive(false);
            attrPrefab.gameObject.SetActive(false);
            selectTransform.gameObject.SetActive(false);
			_canvas = transform.GetComponent<Canvas>();
            Refresh();
            RefreshPlayerModel();

        }
        private IEnumerator InitSelectTipCoroutine(SkillTalentButton btn)
        {
            yield return null;
            if (_currentClickSkillInfo == null && btn != null)
            {
                RefreshSelectTip(btn.info, btn.transform.position);
            }
        }
        void OnDestroy()
        {
            DespawnCharacter();
            UnbindDelegate();
        }
        private void BindDelegate()
        {
            PlayerTalentProxy.instance.UpdateTalentDelegate += UpdateTalentByProtocol;
            PlayerTalentProxy.instance.UpdateAllTalentDelegate += UpdateAllTalentByProtocol;
        }
        private void UnbindDelegate()
        {
            PlayerTalentProxy.instance.UpdateTalentDelegate -= UpdateTalentByProtocol;
            PlayerTalentProxy.instance.UpdateAllTalentDelegate -= UpdateAllTalentByProtocol;
        }
        private void Refresh()
        {
            RefreshPassiveChoiceOneSkill();
            RefreshSummonChoiceOneSkill();
            RefreshPassiveSkill();
            RefreshMaterial();
            RefreshPlayerInfo();
        }

        private void RefreshPassiveChoiceOneSkill()
        {
            TransformUtil.ClearChildren(PassiveChoiceOneRoot, true);
            List<PlayerSkillTalentInfo> skillList = PlayerTalentProxy.instance.GetCurPlayerSkillList(PlayerSkillTalentType.PassiveThreeChoiceOne);
            if (skillList == null)
                return;
            PlayerSkillTalentInfo info;
            for (int i = 0, count = skillList.Count; i < count; i++)
            {
                info = skillList[i];
                SkillTalentButton btn = Instantiate<SkillTalentButton>(skillButtonPrefab);
                btn.gameObject.SetActive(true);
                btn.transform.SetParent(PassiveChoiceOneRoot, false);
                btn.name = i.ToString();
                btn.SetInfo(info);
                if (info.IsCarry)
                    _activePassiveId = info.id;
                if (i == 0)
                    StartCoroutine(InitSelectTipCoroutine(btn));
            }
            passiveChoiceOneText.text = string.Format(Localization.Get("ui.player_cultivate_view.choice"), _activePassiveId == 0 ? 0 : 1);
        }
        private void RefreshSummonChoiceOneSkill()
        {
            TransformUtil.ClearChildren(SummonChoiceOneRoot, true);
            List<PlayerSkillTalentInfo> skillList = PlayerTalentProxy.instance.GetCurPlayerSkillList(PlayerSkillTalentType.SummonThreeChoiceOne);
            if (skillList == null)
                return;
            PlayerSkillTalentInfo info;
            for (int i = 0, count = skillList.Count; i < count; i++)
            {
                info = skillList[i];
                SkillTalentButton btn = Instantiate<SkillTalentButton>(skillButtonPrefab);
                btn.gameObject.SetActive(true);
                btn.transform.SetParent(SummonChoiceOneRoot, false);
                btn.name = i.ToString();
                btn.SetInfo(info);
                if (info.IsCarry)
                    _activeSummonId = info.id;
            }
            summonChoiceOneText.text = string.Format(Localization.Get("ui.player_cultivate_view.choice"), _activeSummonId == 0 ? 0 : 1);

        }
        private void RefreshPassiveSkill()
        {
            TransformUtil.ClearChildren(PassiveRoot, true);
            List<PlayerSkillTalentInfo> skillList = PlayerTalentProxy.instance.GetCurPlayerSkillList(PlayerSkillTalentType.PassiveNormal);
            if (skillList == null)
                return;
            PlayerSkillTalentInfo info;
            for (int i = 0, count = skillList.Count; i < count; i++)
            {
                info = skillList[i];
                SkillTalentButton btn = Instantiate<SkillTalentButton>(skillButtonPrefab);
                btn.gameObject.SetActive(true);
                btn.transform.SetParent(PassiveRoot, false);
                btn.name = i.ToString();
                btn.SetInfo(info);
            }
        }
        private void RefreshMaterial()
        {
            TransformUtil.ClearChildren(materialRoot, true);
            materialRootGO.SetActive(false);
            materialTipText.gameObject.SetActive(false);
            if (_currentClickSkillInfo == null)
                return;
            if (_currentClickSkillInfo.IsMaxLevel)
            {
                materialTipText.gameObject.SetActive(true);
                materialTipText.text = Localization.Get("ui.player_cultivate_view.maxLevel");
                return;
            }
            List<GameResData> costList = _currentClickSkillInfo.UpgradeCost;
            for (int i = 0, count = costList.Count; i < count; i++)
            {
                GameResData res = costList[i];
                GameObject go = Instantiate<GameObject>(materialPrefab);
                RectTransform rt = go.transform as RectTransform;
                go.name = i.ToString();
                go.SetActive(true);
                rt.SetParent(materialRoot, false);

                CommonRewardIcon icon = CommonRewardIcon.Create(rt);
                float scale = rt.sizeDelta.x / (icon.transform as RectTransform).sizeDelta.x;
                icon.transform.localScale = new Vector3(scale, scale, scale);
                icon.SetGameResData(res);
                icon.HideCount();

                Text countText = rt.Find("text_count").GetComponent<Text>();
                int own = 0;
                if (res.type == BaseResType.Item)
                {
                    own = ItemProxy.instance.GetItemCountByItemID(res.id);
                }
                else
                {
                    own = GameProxy.instance.BaseResourceDictionary.GetValue(res.type);
                }
                string countString = string.Format("{0}/{1}", own, res.count);
                countText.text = res.count > own ? UIUtil.FormatToRedText(countString) : UIUtil.FormatToGreenText(countString);
            }
            int level = _currentClickSkillInfo.level;
            string expNeedString = string.Format("{0}/{1}", _currentClickSkillInfo.exp, _currentClickSkillInfo.talentData.exp_need);
            activeNeedExpText.text = _currentClickSkillInfo.isMaxExp ? UIUtil.FormatToGreenText(expNeedString) : UIUtil.FormatToRedText(expNeedString);
            if (level == 0)
            {
                activeButtonText.text = Localization.Get("ui.player_cultivate_view.activate");
                activeNeedExpGO.SetActive(true);
            }
            else
            {
                activeButtonText.text = Localization.Get("ui.player_cultivate_view.upgrade");
                activeNeedExpGO.SetActive(false);
            }
            PlayerSkillTalentInfo preInfo = PlayerTalentProxy.instance.GetSkillTalentInfo(_currentClickSkillInfo.talentData.pre_id);

            if (preInfo != null)
            {
                if (preInfo.level == 0)
                {
                    materialRootGO.SetActive(false);
                    materialTipText.gameObject.SetActive(true);
                    materialTipText.text = string.Format(Localization.Get("ui.player_cultivate_view.needActivePre"), Localization.Get(preInfo.name));
                }
                else
                {
                    materialRootGO.SetActive(true);
                    materialTipText.gameObject.SetActive(false);
                }
            }
            else
            {
                materialRootGO.SetActive(true);
                materialTipText.gameObject.SetActive(false);
            }
        }
        private void RefreshPlayerInfo()
        {
            PlayerInfo player = GameProxy.instance.PlayerInfo;
            playerNameText.text = Localization.Get(player.heroData.name);
            playerLvText.text = string.Format("LV{0}", player.level);
            playerStrengthenLvText.text = string.Format("+{0}", player.strengthenLevel);
			playerTypeImage.SetSprite( UIUtil.GetRoleTypeBigIconSprite(player.heroData.roleType));

			activePassiveImage.SetSprite(_activePassiveId != 0 ? PlayerTalentProxy.instance.GetSkillTalentInfo(_activePassiveId).skillIcon : ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
            activeSummonImage.SetSprite( _activeSummonId != 0 ? PlayerTalentProxy.instance.GetSkillTalentInfo(_activeSummonId).skillIcon : ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
            RefreshAttr();
        }
        private void RefreshPlayerModel()
        {
            StartCoroutine(RefreshPlayerModelCoroutine());
        }
        private IEnumerator RefreshPlayerModelCoroutine()
        {
            DespawnCharacter();
            TransformUtil.ClearChildren(playerModelRoot, true);
            yield return new WaitForSeconds(0.3f);
            //			PlayerEntity.CreatePlayerEntityAsUIElement(GameProxy.instance.PlayerInfo,playerModelRoot,true,true);
            PetData petData = PetData.GetPetDataByID(GameProxy.instance.PlayerInfo.playerData.pet_id);
            _petEntiy = PlayerEntity.CreatePetEntiy(petData.modelName);
            _petEntiy.transform.SetParent(playerModelRoot, false);
            _petEntiy.transform.localScale = petData.scale;
            _petEntiy.transform.localEulerAngles = petData.homeRotation;
            TransformUtil.SwitchLayer(_petEntiy.transform, (int)LayerType.UI);
        }

        private void DespawnCharacter()
        {
            if (_petEntiy)
                Pool.Controller.PoolController.instance.Despawn(_petEntiy.name, _petEntiy);
            _petEntiy = null;
        }

        private void RefreshAttr()
        {
            TransformUtil.ClearChildren(attrRoot, true);
            List<RoleAttributeType> typeList = new List<RoleAttributeType>();
            typeList.Add(RoleAttributeType.HP);
            typeList.Add(RoleAttributeType.MagicAtk);
            typeList.Add(RoleAttributeType.NormalAtk);
            typeList.Add(RoleAttributeType.Normal_Def);
            typeList.Add(RoleAttributeType.Speed);
            List<RoleAttribute> attrList = PlayerUtil.CalcPlayerAttributesList(GameProxy.instance.PlayerInfo, typeList);

            for (int i = 0, count = attrList.Count; i < count; i++)
            {
                RoleAttributeView view = Instantiate<RoleAttributeView>(attrPrefab);
                view.gameObject.SetActive(true);
                view.transform.SetParent(attrRoot, false);
                view.Set(attrList[i]);
            }
        }
        private void RefreshSelectTip(PlayerSkillTalentInfo info, Vector3 worldpos)
        {
            _currentClickSkillInfo = info;
            selectTransform.position = worldpos;
            selectTransform.gameObject.SetActive(true);
            RefreshMaterial();
        }
        public void ClickSkillButton(SkillTalentButton btn)
        {
            RefreshSelectTip(btn.info, btn.transform.position);
			Debugger.Log("ClickSkillButton:{0}",btn.info.id);
            if (btn.info.talentData.groupType == PlayerSkillTalentType.PassiveThreeChoiceOne && btn.info.level > 0)
            {
                _activePassiveId = btn.info.id;
                PlayerController.instance.CLIENT2LOBBY_TALENT_CHOOSE_REQ(_activePassiveId, _activeSummonId);
            }
            else if (btn.info.talentData.groupType == PlayerSkillTalentType.SummonThreeChoiceOne && btn.info.level > 0)
            {
                _activeSummonId = btn.info.id;
                PlayerController.instance.CLIENT2LOBBY_TALENT_CHOOSE_REQ(_activePassiveId, _activeSummonId);
            }

        }
        public void ClickActiveButtonHander()
        {
            if (_currentClickSkillInfo == null)
                return;
            if (_currentClickSkillInfo.level == 0 && !_currentClickSkillInfo.isMaxExp)
            {
                CommonErrorTipsView.Open(Localization.Get("ui.player_cultivate_view.expNotMax"));
                return;
            }
            List<GameResData> costList = _currentClickSkillInfo.UpgradeCost;
            for (int i = 0, count = costList.Count; i < count; i++)
            {
                GameResData res = costList[i];
                int own = 0;
                if (res.type == BaseResType.Item)
                {
                    own = ItemProxy.instance.GetItemCountByItemID(res.id);
                }
                else
                {
                    own = GameProxy.instance.BaseResourceDictionary.GetValue(res.type);
                }
                if (res.count > own)
                {
                    ItemData tempItemData = ItemData.GetBasicResItemByType(res.type);
                    Logic.UI.Tips.View.CommonErrorTipsView.Open(string.Format(Localization.Get("ui.player_cultivate_view.materialNotEnough"), Localization.Get(tempItemData.name)));
                    return;
                }
            }

            if (_currentClickSkillInfo.level == 0)
            {
                PlayerController.instance.CLIENT2LOBBY_TALENT_ACTIVATE_REQ(_currentClickSkillInfo.id);
				GameObject effect =  ParticleUtil.CreateParticle("effects/prefabs/ui_shimo",_canvas);
				if (effect != null )
				{
					effect.transform.SetParent(selectTransform.parent,false);
					effect.transform.localPosition = selectTransform.localPosition;
					GameObject.Destroy(effect,1);
				}
            }
            else
            {
                PlayerController.instance.CLIENT2LOBBY_TALENT_UPGRADE_REQ(_currentClickSkillInfo.id);
            }
        }
        public void ClickCloseBtnHander()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }

        private void UpdateTalentByProtocol(int id)
        {
            PlayerSkillTalentInfo info = PlayerTalentProxy.instance.GetSkillTalentInfo(id);
            if (info.talentData.groupType == PlayerSkillTalentType.PassiveThreeChoiceOne)
            {
                RefreshPassiveChoiceOneSkill();

            }
            else if (info.talentData.groupType == PlayerSkillTalentType.SummonThreeChoiceOne)
            {
                RefreshSummonChoiceOneSkill();
            }
            else
            {
                RefreshPassiveSkill();
            }
            RefreshPlayerInfo();
            RefreshMaterial();
        }
        private void UpdateAllTalentByProtocol()
        {
            Refresh();
        }
    }
}

