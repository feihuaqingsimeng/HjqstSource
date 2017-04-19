using UnityEngine;
using Logic.Skill.Model;
using Logic.Player.Model;
using LuaInterface;
using Common.UI.Components;
namespace Logic.UI.Description.View
{
    [RequireComponent(typeof(EventTriggerDelegate))]
    public class SkillDesButton : MonoBehaviour
    {
        private LuaTable _luaTalbe;
        public static SkillDesButton Get(GameObject go)
        {
            SkillDesButton btn = go.GetComponent<SkillDesButton>();
            if (btn == null)
                btn = go.AddComponent<SkillDesButton>();
            btn.GetLuaSkillDesButton();
            return btn;
        }

        private CommonSkillDesView _skillView;

        private SkillInfo _skillInfo;
        private PlayerSkillTalentInfo _talentSkillInfo;
        private float _showDelay;
        private int _curStar;
        private int _starMin;

        public void GetLuaSkillDesButton()
        {
            if (_luaTalbe == null)
            {
                _luaTalbe = LuaScriptMgr.Instance.DoFile("ui/description/skill_des_button", true)[0] as LuaTable;
                _luaTalbe = _luaTalbe.GetLuaFunction("BindTransform").Call(transform)[0] as LuaTable;
            }
        }

        public void SetSkillInfo(SkillInfo data, int curStar, int starMin, float showDelay = 0)
        {
            _skillInfo = data;
            _curStar = curStar;
            _starMin = starMin;
            _showDelay = showDelay;
            GetLuaSkillDesButton();
            _luaTalbe.GetLuaFunction("SetSkillId").Call(_luaTalbe, data.skillData.skillId, _curStar, _starMin, showDelay);
			_luaTalbe.GetLuaFunction("IsLongPress").Call(_luaTalbe,true);
        }
        public void SetSkillInfo(uint skillId, int curStar, int starMin, float showDelay = 0)
        {
            SkillInfo info = new SkillInfo(skillId);
            SetSkillInfo(info, curStar, starMin, showDelay);

        }
        //public void SetTalenSkillInfo(PlayerSkillTalentInfo talentSkill, float showDelay = 0)
        //{
        //    _talentSkillInfo = talentSkill;
        //    _showDelay = showDelay;
        //}
        void Awake()
        {

        }

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //    if (_skillInfo == null && _talentSkillInfo == null)
        //        return;
        //    StartCoroutine(showSkillTipsCoroutine());

        //}
        //public void OnPointerUp(PointerEventData eventData)
        //{
        //    StopAllCoroutines();
        //    if (_skillView != null)
        //        _skillView.Close();
        //    _skillView = null;
        //}
        //private IEnumerator showSkillTipsCoroutine()
        //{
        //    yield return new WaitForSeconds(_showDelay);
        //    RectTransform rectTran = transform as RectTransform;
        //    Vector2 sizeDelta = new Vector2(65, 65);
        //    if (rectTran != null)
        //        sizeDelta = (transform as RectTransform).sizeDelta;
        //    if (_skillInfo != null)
        //        _skillView = CommonSkillDesView.Open(_skillInfo,_curStar, _starMin,transform.position, sizeDelta);
        //    else if (_talentSkillInfo != null)
        //        _skillView = CommonSkillDesView.Open(_talentSkillInfo, transform.position, sizeDelta);
        //}
    }
}

