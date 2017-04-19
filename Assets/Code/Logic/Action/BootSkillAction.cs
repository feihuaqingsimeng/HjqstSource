using UnityEngine;
using System.Collections;
using Common.Animators;
using Logic.Skill.Model;
namespace Logic.Action
{
    public class BootSkillAction : AIAction
    {
        public SkillInfo skillInfo;
        public override void Execute()
        {
            if (!character || skillInfo == null) return;
            AnimatorUtil.Play(character.anim, AnimatorUtil.BOOT_ID, 0, 0f);
        }
    }
}
