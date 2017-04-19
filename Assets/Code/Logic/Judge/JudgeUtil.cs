using UnityEngine;
using System.Collections;
using Logic.Character;
using Common.Util;
namespace Logic.Judge
{
    public static class JudgeUtil
    {
        public const int RATIO = 1000;//概率比
        public static int GetJudgeResult(CharacterEntity character, CharacterEntity target1, CharacterEntity target2, int skillId)
        {
            int judgeType = 0;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case Logic.Enums.FightType.PVE:
                case Logic.Enums.FightType.Arena:
                case Logic.Enums.FightType.DailyPVE:
                case Logic.Enums.FightType.Expedition:
                case Logic.Enums.FightType.WorldTree:
                case Logic.Enums.FightType.WorldBoss:
                case Logic.Enums.FightType.FirstFight:
                case Logic.Enums.FightType.SkillDisplay:
                case Logic.Enums.FightType.PVP:
                case Logic.Enums.FightType.FriendFight:
                case Logic.Enums.FightType.MineFight:
#if UNITY_EDITOR
                case Logic.Enums.FightType.Imitate:
#endif
                    #region calc
                    //判断是否命中→是否暴击→是否产生格挡→普通命中
                    float hitProbability = 0;
                    if (Fight.Controller.FightController.instance.isComboing)
                        hitProbability = 1;
                    else
                        hitProbability = character.hit - target1.dodge;//命中几率=攻击方命中-受击方闪避
                    if (hitProbability > 0)
                    {
                        if (RandomUtil.GetRandom(hitProbability))
                        {
                            judgeType = 1 << 0;
                            //Debugger.Log(judgeType);
                            float critProbability = character.crit - target2.antiCrit;//暴击几率=攻击方暴击-受击方抗暴击
                            if (RandomUtil.GetRandom(critProbability))
                            {
                                judgeType |= 1 << 1;
                                //Debugger.Log(judgeType);
                            }
                            else
                            {
                                float blockProbability = target2.block - character.antiBlock;//格挡几率=受击方格挡-攻击方破击
                                if (RandomUtil.GetRandom(blockProbability))
                                {
                                    judgeType |= 1 << 2;
                                    //Debugger.Log(judgeType);
                                }
                            }
                        }
                    }
                    #endregion
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    judgeType = Fight.Model.FightProxy.instance.GetConsortiaJudgeType((int)character.characterInfo.instanceID, skillId, (int)target2.characterInfo.instanceID);
                    break;
            }
            return judgeType;
        }

        public static bool GetRandom(float probability)
        {
            bool result = false;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case Logic.Enums.FightType.PVE:
                case Logic.Enums.FightType.Arena:
                case Logic.Enums.FightType.DailyPVE:
                case Logic.Enums.FightType.Expedition:
                case Logic.Enums.FightType.WorldTree:
                case Logic.Enums.FightType.WorldBoss:
                case Logic.Enums.FightType.FirstFight:
                case Logic.Enums.FightType.SkillDisplay:
                case Logic.Enums.FightType.PVP:
                case Logic.Enums.FightType.FriendFight:
                case Logic.Enums.FightType.MineFight:
#if UNITY_EDITOR
                case Logic.Enums.FightType.Imitate:
#endif
                    result = RandomUtil.GetRandom(probability);
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    result = RandomUtil.GetRandom(Fight.Model.FightProxy.instance.randomSeed, probability, RATIO);
                    break;
            }
            return result;
        }

        public static bool GetRandom(float probability, bool zeroEqOne)
        {
            bool result = false;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case Logic.Enums.FightType.PVE:
                case Logic.Enums.FightType.Arena:
                case Logic.Enums.FightType.DailyPVE:
                case Logic.Enums.FightType.Expedition:
                case Logic.Enums.FightType.WorldTree:
                case Logic.Enums.FightType.WorldBoss:
                case Logic.Enums.FightType.FirstFight:
                case Logic.Enums.FightType.SkillDisplay:
                case Logic.Enums.FightType.PVP:
                case Logic.Enums.FightType.FriendFight:
                case Logic.Enums.FightType.MineFight:
#if UNITY_EDITOR
                case Logic.Enums.FightType.Imitate:
#endif
                    result = RandomUtil.GetRandom(probability, zeroEqOne);
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    result = RandomUtil.GetRandom(Fight.Model.FightProxy.instance.randomSeed, probability, RATIO);//没有处理概率为0则为1
                    break;
            }
            return result;
        }

        public static float GetRandom()
        {
            float result = 0f;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case Logic.Enums.FightType.PVE:
                case Logic.Enums.FightType.Arena:
                case Logic.Enums.FightType.DailyPVE:
                case Logic.Enums.FightType.Expedition:
                case Logic.Enums.FightType.WorldTree:
                case Logic.Enums.FightType.WorldBoss:
                case Logic.Enums.FightType.FirstFight:
                case Logic.Enums.FightType.SkillDisplay:
                case Logic.Enums.FightType.PVP:
                case Logic.Enums.FightType.FriendFight:
                case Logic.Enums.FightType.MineFight:
#if UNITY_EDITOR
                case Logic.Enums.FightType.Imitate:
#endif
                    result = RandomUtil.GetRandom();
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    result = RandomUtil.GetRandom();//没有处理概率为0则为1
                    break;
            }
            return result;
        }
    }
}