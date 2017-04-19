using Logic.Dungeon.Model;
using Logic.Effect.Model;
using Logic.Enums;
using Logic.Fight.Controller;
using Logic.Fight.Model;
using Logic.Hero.Model;
using Logic.Skill.Model;
using Logic.Team.Model;
using System.Collections;
using System.Collections.Generic;
namespace Logic.Fight
{
    public static class FightUtil
    {
        public static List<string> GetEffects(int heroId, bool isSkill)
        {
            List<string> result = new List<string>();
            List<uint> effectIds = new List<uint>();
            HeroData heroData = HeroData.GetHeroDataByID(heroId);
            if (heroData != null)
            {
                List<uint> skillIds = new List<uint>();
                List<uint> mechanicsIds = new List<uint>();
                if (!isSkill)
                {
                    if (heroData.hitId > 0)
                        skillIds.Add(heroData.hitId);
                    if (heroData.passiveId1 > 0)
                    {
                        for (int i = 0; i < 5; i++)//预留5个被动效果
                        {
                            int passiveMechanicsId = heroId * 100 + 55 + i;
                            if (MechanicsData.ExistMechanicsId((uint)passiveMechanicsId))
                                mechanicsIds.Add((uint)passiveMechanicsId);
                        }
                    }
                }
                else
                {
                    if (heroData.skillId1 > 0)
                        skillIds.Add(heroData.skillId1);
                    if (heroData.skillId2 > 0)
                        skillIds.Add(heroData.skillId2);
                }
                for (int i = 0, count = skillIds.Count; i < count; i++)
                {
                    SkillData skillData = SkillData.GetSkillDataById(skillIds[i]);
                    if (skillData != null)
                    {
                        foreach (var kvp in skillData.timeline)
                        {
                            mechanicsIds.AddRange(kvp.Value);
                        }
                        if (skillData.effectIds.Length > 0)
                            effectIds.AddRange(skillData.effectIds, true);
                        if (skillData.aoeEffects.Length > 0)
                            effectIds.AddRange(skillData.aoeEffects, true);
                        if (skillData.aoeFlyEffects.Length > 0)
                            effectIds.AddRange(skillData.aoeFlyEffects, true);
                        if (skillData.flyEffectIds.Count > 0)
                            effectIds.AddRange(skillData.flyEffectIds, true);
                    }
                }
                for (int i = 0, count = mechanicsIds.Count; i < count; i++)
                {
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsIds[i]);
                    if (mechanicsData != null)
                    {
                        if (mechanicsData.effectIds.Length > 0)
                            effectIds.AddRange(mechanicsData.effectIds, true);
                    }
                }
            }
            //Debugger.Log("{0} effects count:{1}", heroId, effectIds.Count);
            for (int i = 0, count = effectIds.Count; i < count; i++)
            {
                //Debugger.Log("effect id:{0}", effectIds[i]);
                EffectData effectData = EffectData.GetEffectDataById(effectIds[i]);
                if (effectData != null)
                {
                    switch (effectData.effectType)
                    {
                        case Logic.Enums.EffectType.Root:
                        case Logic.Enums.EffectType.Trace:
                        case Logic.Enums.EffectType.TargetArea:
                        case Logic.Enums.EffectType.LockPart:
                        case Logic.Enums.EffectType.LockTarget:
                        case Logic.Enums.EffectType.CurveTrace:
                        case Logic.Enums.EffectType.MoveTargetPos:
                        case Logic.Enums.EffectType.FullScreen:
                            string path = string.Format("effects/prefabs/{0}", effectData.effectName);
                            if (!result.Contains(path))
                                result.Add(path);
                            break;
                    }
                }
            }
            return result;
        }


        public static List<string> GetEffectsInFight(bool isSkill)
        {
            List<string> result = new List<string>();
            switch (FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.DailyPVE:
                case FightType.WorldTree:
                    {
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            result.AddRange(GetEffects(fightHeroInfos[i].heroInfo.heroData.id, isSkill));
                        }
                        if (FightProxy.instance.fightPlayerInfo != null)
                        {
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.heroData.id, isSkill));
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.summonEffectId, isSkill));
                        }
                        DungeonData dungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
                        for (int i = 0, length = dungeonData.teamDataList.Count; i < length; i++)
                        {
                            TeamData teamData = dungeonData.teamDataList[i];
                            List<HeroData> heros = teamData.teamDictionary.GetValues();
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                result.AddRange(GetEffects(heros[j].id, isSkill));
                            }
                        }
                    }
                    break;
                case FightType.WorldBoss:
                    {
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            result.AddRange(GetEffects(fightHeroInfos[i].heroInfo.heroData.id, isSkill));
                        }
                        if (FightProxy.instance.fightPlayerInfo != null)
                        {
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.heroData.id, isSkill));
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.summonEffectId, isSkill));
                        }
                        HeroData worldBossHeroData = HeroData.GetHeroDataByID(WorldBoss.Model.WorldBossProxy.instance.BossID);
                        result.AddRange(GetEffects(worldBossHeroData.id, isSkill));
                    }
                    break;
                case FightType.Arena:
                case FightType.FirstFight:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.ConsortiaFight:
                case FightType.Expedition:
                case FightType.SkillDisplay:
                case FightType.MineFight:
                    {
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            result.AddRange(GetEffects(fightHeroInfos[i].heroInfo.heroData.id, isSkill));
                        }
                        if (FightProxy.instance.fightPlayerInfo != null)
                        {
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.heroData.id, isSkill));
                            result.AddRange(GetEffects(FightProxy.instance.fightPlayerInfo.playerInfo.summonEffectId, isSkill));
                        }

                        List<FightHeroInfo> fightEnemyHeroInfos = FightProxy.instance.enemyFightHeroInfoList;
                        for (int i = 0, count = fightEnemyHeroInfos.Count; i < count; i++)
                        {
                            result.AddRange(GetEffects(fightEnemyHeroInfos[i].heroInfo.heroData.id, isSkill));
                        }
                        if (FightProxy.instance.enemyFightPlayerInfo != null)
                        {
                            result.AddRange(GetEffects(FightProxy.instance.enemyFightPlayerInfo.playerInfo.heroData.id, isSkill));
                            //result.AddRange(GetEffects(FightProxy.instance.enemyFightPlayerInfo.playerInfo.summonId));
                        }
                    }
                    break;
            }
            return result;
        }
    }
}