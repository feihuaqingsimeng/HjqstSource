using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.CommonTopBar.View;
using Logic.Fight.Controller;
using Logic.FunctionOpen.Model;
using Logic.Enums;
using Logic.UI.IllustratedHandbook.Model;
using System.Collections.Generic;
using Logic.Character;
using Logic.Character.Controller;
using Logic.Player.Model;
using Logic.Hero.Model;
using Common.Util;
using Logic.Role.Model;
using Logic.Skill.Model;
using Common.Localization;
using LuaInterface;

namespace Logic.UI.IllustratedHandbook.View
{
    public class IllustrationSkillDisplayView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/illustrated_handbook/illustration_skill_display_view";

        public delegate void SkillDisplayOverHandler();
        public SkillDisplayOverHandler skillDisplayOverHandler;

        public static IllustrationSkillDisplayView Open()
        {
            IllustrationSkillDisplayView view = UIMgr.instance.Open<IllustrationSkillDisplayView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
            return view;
        }

        public Transform core;
        public IllustrationSkillItemButton skillItemBtnPrefab;
        public Transform skillRootTran;
        public Text titleText;
        public Image backBtnImg;
        public Button backBtn;
        [HideInInspector]
        public bool isClickSkill = false;

        private CharacterEntity _character;
        private List<IllustrationSkillItemButton> _skillItemList = new List<IllustrationSkillItemButton>();
        void Start()
        {
            //            CommonTopBarView topBarView = CommonTopBarView.CreateNewAndAttachTo(core);
            //            topBarView.SetAsCommonStyle("技能预览", ClickBackHandler, false, false, false, false);
			LuaTable illustration_model =(LuaTable) LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","illustration_model")[0];
			LuaTable roleInfoLua = (LuaTable)illustration_model["selectRoleInfo"];
			HeroData heroData = HeroData.GetHeroDataByID(((LuaTable)roleInfoLua["heroData"])["id"].ToString().ToInt32());
            RoleInfo info  = null;
			if (heroData.IsPlayer())
				info = new PlayerInfo(roleInfoLua);
			else
				info = new HeroInfo(roleInfoLua);
            titleText.text = Localization.Get(info.heroData.name);
            skillItemBtnPrefab.gameObject.SetActive(false);
            InitSkill();
            BindDelegate();
        }

        private void InitSkill()
        {
            TransformUtil.ClearChildren(skillRootTran, true);
            _skillItemList.Clear();

            List<HeroEntity> heros = new List<HeroEntity>(PlayerController.instance.heros);
            if (heros.Count == 0)
                return;
            HeroEntity character = heros[0];
            _character = character;
            RoleInfo role = null;
            if (character is PlayerEntity)
            {
                role = PlayerController.instance.GetPlayerInfo();
            }
            else if (character is HeroEntity)
            {
                role = PlayerController.instance.GetHeroInfo(character.characterInfo.instanceID);
            }
            else
            {
                Debugger.LogError("these is not this type: " + character.GetType());
            }
            if (character.characterInfo.skillInfo1 != null)
            {
                IllustrationSkillItemButton btn = Instantiate<IllustrationSkillItemButton>(skillItemBtnPrefab);
                btn.transform.SetParent(skillRootTran, false);
                btn.gameObject.SetActive(true);
                btn.SetData((int)character.characterInfo.instanceID, character.characterInfo.skillInfo1);
                _skillItemList.Add(btn);
            }
            if (character.characterInfo.skillInfo2 != null)
            {
                IllustrationSkillItemButton btn = Instantiate<IllustrationSkillItemButton>(skillItemBtnPrefab);
                btn.transform.SetParent(skillRootTran, false);
                btn.gameObject.SetActive(true);
                btn.SetData((int)character.characterInfo.instanceID, character.characterInfo.skillInfo2);
                _skillItemList.Add(btn);
            }
//            if (character.characterInfo.aeonSkillInfo != null)
//            {
//                IllustrationSkillItemButton btn = Instantiate<IllustrationSkillItemButton>(skillItemBtnPrefab);
//                btn.transform.SetParent(skillRootTran, false);
//                btn.gameObject.SetActive(true);
//                btn.SetData((int)character.characterInfo.instanceID, character.characterInfo.aeonSkillInfo);
//                _skillItemList.Add(btn);
//            }
        }

        public void SetBackBtnStatus(bool clickable)
        {
            backBtnImg.SetGray(!clickable);
            backBtn.enabled = clickable;
        }

        void OnDestroy()
        {
            UnbindDelegate();
        }
        private void BindDelegate()
        {
            FightController.instance.finishedSkillHandler += FinishedSkillHandler;
        }
        private void UnbindDelegate()
        {
            FightController.instance.finishedSkillHandler -= FinishedSkillHandler;
        }
        private void FinishedSkillHandler(SkillInfo skill)
        {
            isClickSkill = false;
            SetBackBtnStatus(true);
        }
        #region event
        public void ClickBackHandler()
        {
            if (skillDisplayOverHandler != null)
                skillDisplayOverHandler();
        }

        //		private void loadFinishedCallback()
        //		{
        //			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.IllustrationView, null, true);
        //			IllustrationDetailView.Open(IllustratedHandbookProxy.instance.CheckedDetailRoleInfo);
        //		}
        #endregion
    }
}

