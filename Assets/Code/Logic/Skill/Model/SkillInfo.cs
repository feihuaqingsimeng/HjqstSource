using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Logic.Skill.Model
{
    public class SkillInfo
    {
        public Vector3 rotateAngles;//技能人物转向
        public uint currentLevel;//当前技能阶数
        public float effectDelay;
        public int mechanicsIndex = 0;
        public SkillData skillData = null;
        public AnimationData animationData = null;
        public uint characterInstanceId;
        public SkillInfo(uint skillId)
        {
            if (skillId != 0)
            {
                skillData = SkillData.GetSkillDataById(skillId);
                animationData = AnimationData.GetAnimationDataById(skillId);
            }
        }

        public static List<SkillInfo> GetSkillListByIds(uint[] skillIds)
        {
            List<SkillInfo> result = new List<SkillInfo>();
            for (int i = 0, count = skillIds.Length; i < count; i++)
            {
                result.Add(new SkillInfo(skillIds[i]));
            }
            return result;
        }
    }
}