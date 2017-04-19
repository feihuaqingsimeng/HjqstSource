using UnityEngine;
using System.Collections;
using Logic.Enums;
using System.Collections.Generic;

namespace Logic.Skill.Model
{
    public class SkillDesInfo
    {
        public MechanicsType mechanicsType;
        public TargetType target;
        public MechanicsValueType mechanicsValueType;
        public float mechanicsValue1;
        public float mechanicsValue2;
        public bool isGrowup;

        public SkillDesInfo(MechanicsType mechanicsType, TargetType target, MechanicsValueType mechanicsValueType, float mechanicsValue, bool isGrowup) 
        {
            this.mechanicsType = mechanicsType;
            this.target = target;
            this.mechanicsValueType = mechanicsValueType;
            this.mechanicsValue1 = mechanicsValue;
            this.isGrowup = isGrowup;
        }
    }
}
