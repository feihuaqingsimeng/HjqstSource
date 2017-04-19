using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using Logic.Character;
using Logic.Effect.Controller;
using System.Collections.Generic;

namespace Logic.Effect.Model
{
    public class EffectInfo
    {
        public Vector3 pos;
        public Vector3 endPos;
        public Vector3 rotateAngles = Vector3.zero;//技能人物转向
        public CharacterEntity character;
        public CharacterEntity target;
        public Transform lockTrans;
        public SkillInfo skillInfo;
        public MechanicsData mechanicsData;
        public Triple<float,float, float> mechanicsValue;
        public float delay;//技能延迟
        public EffectData effectData;
        public float time;//移动时间（计算结果）
        public bool isLastTarget;
        public float length;//only for black screen and full screen
        public EffectInfo(uint effectId)
        {
            if (effectId != 0)
                effectData = EffectData.GetEffectDataById(effectId);
        }
    }
}
