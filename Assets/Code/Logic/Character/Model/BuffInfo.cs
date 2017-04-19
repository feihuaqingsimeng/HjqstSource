using Logic.Enums;
using Logic.Skill.Model;
using LuaInterface;
using Common.GameTime.Controller;
namespace Logic.Character.Model
{
    public class BuffInfo
    {
        public MechanicsData mechanics;
        public CharacterEntity character;
        public CharacterEntity target;
        public SkillInfo skillInfo;
        public BuffType buffType;
        public BuffAddType buffAddType;
        public int judgeType;
        public int count;//起作用次数
        public uint targetSkillId;
        public bool forever;
        public bool disperseable;
        public bool kindness;
#if UNITY_EDITOR
        [NoToLua]
        public SkillLevelBuffAddType skillLevelBuffAddType;
#endif
        public float time;
        public float originalValue;
        public float value;
        public int nextTime;
        private int _interval;
        public int interval
        {
            set
            {
                _interval = value;
                nextTime = value;
            }
            get
            {
                return _interval;
            }
        }

        public BuffInfo(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, BuffType buffType, SkillLevelBuffAddType skillLevelBuffAddType, BuffAddType buffAddType, float time, float value, uint level, int judgeType)
        {
            this.mechanics = mechanics;
            this.character = character;
            this.target = target;
            this.skillInfo = skillInfo;
            this.buffType = buffType;
            this.buffAddType = buffAddType;
            this.judgeType = judgeType;
#if UNITY_EDITOR
            this.skillLevelBuffAddType = skillLevelBuffAddType;
#endif
            switch (skillLevelBuffAddType)
            {
                case SkillLevelBuffAddType.Time:
                    {
                        float result = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(time, character.characterInfo.dlevel);
                        result = BuffUtil.GetBuffValue(result, level);
                        this.time = TimeController.instance.fightSkillTime + result;
                        this.value = value;
                        break;
                    }
                case SkillLevelBuffAddType.Value:
                    {
                        float result = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(value, character.characterInfo.dlevel);
                        result = BuffUtil.GetBuffValue(result, level);
                        this.value = result;
                        this.time = TimeController.instance.fightSkillTime + time;
                        break;
                    }
                default:
                    this.time = TimeController.instance.fightSkillTime + time;
                    this.value = value;
                    break;
            }
            kindness = BuffUtil.Judge(this.buffType, this.value);
            originalValue = this.value;
            interval = 1;
        }

        public BuffInfo(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanics, BuffType buffType, SkillLevelBuffAddType skillLevelBuffAddType, BuffAddType buffAddType, int count, uint targetSkillId, float value, uint level, int judgeType)
        {
            this.mechanics = mechanics;
            this.character = character;
            this.target = target;
            this.skillInfo = skillInfo;
            this.buffType = buffType;
            this.buffAddType = buffAddType;
            this.judgeType = judgeType;
#if UNITY_EDITOR
            this.skillLevelBuffAddType = skillLevelBuffAddType;
#endif
            this.targetSkillId = targetSkillId;
            this.count = count;
            switch (skillLevelBuffAddType)
            {
                case SkillLevelBuffAddType.Time:

                    break;
                case SkillLevelBuffAddType.Value:
                    {
                        float result = Skill.SkillUtil.GetMechanicsValueByAdvanceLevel(value, character.characterInfo.dlevel);
                        result = BuffUtil.GetBuffValue(result, level);
                        this.value = result;
                    }
                    break;
            }
            kindness = BuffUtil.Judge(this.buffType, this.value);
            originalValue = this.value;
            interval = 1;
        }

        public BuffInfo(CharacterEntity target, MechanicsData mechanics, BuffType buffType, float time, float intervalTime, int count, float value)
        {
            this.mechanics = mechanics;
            this.target = target;
            this.buffType = buffType;
            this.time = TimeController.instance.fightSkillTime + time;
            this.value = value;
            this.count = count;
            kindness = BuffUtil.Judge(this.buffType, this.value);
            interval = (int)intervalTime;
        }

        public bool outOfTime
        {
            get
            {
                return TimeController.instance.fightSkillTime >= this.time;
            }
        }

        public void Set2OutOfTime()
        {
            this.time = TimeController.instance.fightSkillTime;
        }
    }
}