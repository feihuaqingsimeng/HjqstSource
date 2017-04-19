using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Skill.Model;
using Common.ResMgr;
using Logic.Effect.Controller;
using Logic.UI.Description.View;
using Logic.Audio.Controller;

namespace Logic.UI.IllustratedHandbook.View
{
    public class IllustrationSkillItemButton : MonoBehaviour
    {
        public Image skillIcon;
        public GameObject selectGO;
        public GameObject selectedImage;

        private int _characterId;
        private SkillInfo _skillInfo;

        void Awake()
        {

            SetSelect(false);
        }

        public void SetData(int characterId, SkillInfo info)
        {
            _characterId = characterId;
            _skillInfo = info;
            Init();
        }
        public void SetSelect(bool value)
        {
            selectedImage.SetActive(value);
        }
        public void Init()
        {
			skillIcon.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(_skillInfo.skillData.skillIcon)));
            //SkillDesButton desBtn = SkillDesButton.Get(gameObject);

            //desBtn.SetSkillInfo(_skillInfo, 0,0,0.5f);
        }

        public void ClickSkillHandler()
        {
            IllustrationSkillDisplayView skillDisplayView = UIMgr.instance.Get<IllustrationSkillDisplayView>(IllustrationSkillDisplayView.PREFAB_PATH);
            if (_isClick && !skillDisplayView.isClickSkill)
            {
                skillDisplayView.isClickSkill = true;
                skillDisplayView.SetBackBtnStatus(false);
                Canvas canvas = skillDisplayView.GetComponent<Canvas>();
                EffectController.instance.PlayUIEffect(Logic.Effect.Controller.EffectController.EFFECT_CLICKSKILL, Vector3.zero, Quaternion.identity, Vector3.one, 2f, canvas.sortingOrder, transform);
                Logic.Character.Controller.PlayerController.instance.SetHerosCD2Zero();
                //                if (_skillInfo.skillData.skillType == Enums.SkillType.Aeon)
                //                    //Fight.Controller.FightController.instance.OrderAeonSkill((uint)_characterId, (uint)_skillInfo.skillData.skillId);
                //                else
                Fight.Controller.FightController.instance.OrderPlayerSkill((uint)_characterId, (uint)_skillInfo.skillData.skillId, false);
                StartCoroutine(SelectCoroutine());
                AudioController.instance.PlayAudio(AudioController.SKILL_CLICK, false);
            }
        }
        private IEnumerator SelectCoroutine()
        {
            SetSelect(true);
            yield return new WaitForSeconds(0.3f);
            SetSelect(false);
        }

        #region ui event
        bool _isClick = true;
        public void SkillPointDownHandler()
        {
            StopCoroutine("SkillPointDownCoroutine");
            StartCoroutine("SkillPointDownCoroutine");
        }
        public void SkillPointUpHandler()
        {
            StopCoroutine("SkillPointDownCoroutine");
        }
        private IEnumerator SkillPointDownCoroutine()
        {
            _isClick = true;
            yield return new WaitForSeconds(0.6f);
            _isClick = false;
        }
        #endregion
    }
}

