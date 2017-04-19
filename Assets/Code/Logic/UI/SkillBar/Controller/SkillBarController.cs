using Logic.UI.SkillBar.View;
using Logic.Enums;
using System.Collections.Generic;
using Logic.Character.Controller;
using Logic.Skill.Model;
using Logic.Character;
namespace Logic.UI.SkillBar.Controller
{
    public class SkillBarController : SingletonMono<SkillBarController>
    {
        private List<SkillItemView> _sorts = new List<SkillItemView>();
        void Awake()
        {
            instance = this;
        }

        private void AddSkillItem(SkillItemView skillItemView)
        {
            RemoveDisableSkillItem();
            int sort = _sorts.Count + 1;
            skillItemView.Sort = sort;
            _sorts.Add(skillItemView);
        }

        private void RemoveSkillItem(uint skillId)
        {
            RemoveDisableSkillItem();
            for (int i = 0, count = _sorts.Count; i < count; i++)
            {
                SkillItemView skillItemView = _sorts[i];
                if (skillItemView)
                {
                    if (skillItemView.skillID == skillId)
                    {
                        skillItemView.Sort = 0;
                        _sorts.Remove(skillItemView);
                        break;
                    }
                }
            }
            for (int i = 0, count = _sorts.Count; i < count; i++)
            {
                SkillItemView skillItemView = _sorts[i];
                if (skillItemView)
                    skillItemView.Sort = i + 1;
            }
        }

        public void RemoveDisableSkillItem()
        {
            List<SkillItemView> list = new List<SkillItemView>();
            for (int i = 0, count = _sorts.Count; i < count; i++)
            {
                SkillItemView skillItemView = _sorts[i];
                if (skillItemView)
                {
                    if (!skillItemView.Enable)
                    {
                        skillItemView.Sort = 0;
                        list.Add(skillItemView);
                    }
                }
            }
            for (int i = 0, count = list.Count; i < count; i++)
            {
                _sorts.Remove(list[i]);
            }
            for (int i = 0, count = _sorts.Count; i < count; i++)
            {
                SkillItemView skillItemView = _sorts[i];
                if (skillItemView)
                    skillItemView.Sort = i + 1;
            }
        }

        private void RemoveAllSkillItem()
        {
            for (int i = 0, count = _sorts.Count; i < count; i++)
            {
                _sorts[i].Sort = 0;
            }
            _sorts.Clear();
        }

        public void OrderAeonSkill(uint id, uint skillId)
        {
            Fight.Controller.FightController.instance.OrderAeonSkill(id, skillId);
        }

        public void OrderPlayerSkill(uint id, SkillItemView skillItemView)
        {
            if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal)
            {
                AddSkillItem(skillItemView);
            }
            Fight.Controller.FightController.instance.OrderPlayerSkill(id, skillItemView.skillID, false);
        }

        public void FinishSkill(uint userId, uint skillId)
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                skillBarView.FinishSkill(userId, skillId);
                RemoveSkillItem(skillId);
            }
        }

        public void SetAngry(float angry)
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                skillBarView.SetAngry(angry);
            }
        }

        public void ResetSkillOrder(uint id, uint skillId)
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                skillBarView.ResetSkillOrder(id, skillId);
                RemoveSkillItem(skillId);
            }
        }

        public void ResetSkillOrder()
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                skillBarView.ResetSkillOrder();
                RemoveAllSkillItem();
            }
        }

        public void Show(bool show)
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                if (show)
                {
                    Common.Util.TransformUtil.SwitchLayer(skillBarView.transform, (int)LayerType.UI);
                }
                else
                {
                    Common.Util.TransformUtil.SwitchLayer(skillBarView.transform, (int)LayerType.None);
                }
            }
        }

        public bool OrderConsortiaSkill(uint id, uint skillId)
        {
            HeroEntity hero = PlayerController.instance[id];
            if (hero)
            {
                if (hero.characterInfo.skillId1 == skillId)
                {
                    hero.skillItemBoxView.skillItemView1.SkillClickHandler();
                    return true;
                }
                else if (hero.characterInfo.skillId2 == skillId)
                {
                    hero.skillItemBoxView.skillItemView2.SkillClickHandler();
                    return true;
                }
            }
            return false;
        }

        public void AutoReleaseSkill()
        {
            SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<SkillBar.View.SkillBarView>(SkillBar.View.SkillBarView.PREFAB_PATH);
            if (skillBarView)
            {
                {
                    for (int i = 0, count = skillBarView.skillItemBoxViewList.Count; i < count; i++)
                    {
                        SkillItemBoxView skillItemBoxView = skillBarView.skillItemBoxViewList[i];
                        if (!skillItemBoxView.isEnable) continue;
                        if (skillItemBoxView.character.isDead) continue;
                        if (skillItemBoxView.isCanPlayAeon && !skillItemBoxView.character.controled && !skillItemBoxView.character.Silence)
                        {
                            skillItemBoxView.AeonBtnClickHandler();
                            break;
                        }
                        //if (skillItemBoxView.skillItemView1.canOrder && skillItemBoxView.character.canOrderSkill1)
                        //{
                        //    skillItemBoxView.skillItemView1.SkillClickHandler();
                        //    break;
                        //}
                        //if (skillItemBoxView.skillItemView2.canOrder && skillItemBoxView.character.canOrderSkill2)
                        //{
                        //    skillItemBoxView.skillItemView2.SkillClickHandler();
                        //    break;
                        //}
                    }
                }
            }

            List<SkillInfo> skillInfos = PlayerController.instance.skillInfos;
            for (int i = 0, count = skillInfos.Count; i < count; i++)
            {
                SkillInfo skillInfo = skillInfos[i];
                if (skillInfo == null)
                    continue;
                HeroEntity hero = PlayerController.instance[skillInfo.characterInstanceId];
                if (!hero) continue;
                if (hero.characterInfo.skillInfo1 == skillInfo)
                {
                    if (hero.canOrderSkill1 && hero.skillItemBoxView.skillItemView1.canOrder)
                    {
                        hero.skillItemBoxView.skillItemView1.SkillClickHandler();
                        break;
                    }
                }
                else if (hero.characterInfo.skillInfo2 == skillInfo)
                {
                    if (hero.canOrderSkill2 && hero.skillItemBoxView.skillItemView2.canOrder)
                    {
                        hero.skillItemBoxView.skillItemView2.SkillClickHandler();
                        break;
                    }
                }
            }
        }
    }
}