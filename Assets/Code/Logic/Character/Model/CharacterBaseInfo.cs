using Logic.Enums;
using Logic.Fight.Controller;
using Logic.Hero.Model;
using Logic.Player.Model;
using Logic.Role.Model;
using Logic.Skill.Model;
using LuaInterface;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.Character.Model
{
    public class CharacterBaseInfo
    {
        public uint instanceID;//id
        public uint baseId;//固定ID
        public uint hitId;//普攻技能
        public uint skillId1;//技能1
        public uint skillId2;//技能2
        public Dictionary<int, int> passiveIdDic;//被动技能
        public uint passiveId2;//被动技能2
        public uint aeonId;//使魔ID
        public uint aeonSkill;//使魔技能
        public uint aeonSkillLevel;//使魔技能等级
        public uint aeonEffectId;//使魔效果ID 
        public float hitCD;
        public float skill1CD;
        public float skill2CD;
        public int speed;
        public int HP = 0;//血量
        public uint maxHP = 0;//最多血量
        public float angerValue;//怒气值
        public float maxAngerValue;//最多怒气值
        public Vector3 pos;//位置
        public Vector3 eulerAngles;//转角
        public Vector3 scale;//缩放
        public uint positionId;//场景中位置索引
        public SkillInfo hitSkillInfo;
        public SkillInfo skillInfo1;
        public SkillInfo skillInfo2;
        public SkillInfo aeonSkillInfo;
        private bool _floatable;
        public bool floatable
        {
            private set { _floatable = value; }
            get
            {
                return _floatable;
            }
        }
        public RoleType roleType;
        public RoleInfo roleInfo;
        public uint dlevel = 0;//当前星级-最小星级

        public CharacterBaseInfo UpdateCharacterBaseInfo(HeroData heroData)
        {
            baseId = (uint)heroData.id;
            //passiveId1 = heroData.passiveId1;
            //passiveId2 = heroData.passiveId2;
            if (heroData.hitId != 0)
            {
                if (heroData.hitId != hitId)
                {
                    hitSkillInfo = new SkillInfo(heroData.hitId);
                    hitSkillInfo.characterInstanceId = instanceID;
                }
            }
            else
                hitSkillInfo = null;
            if (heroData.skillId1 != 0)
            {
                if (heroData.skillId1 != skillId1)
                {
                    skillInfo1 = new SkillInfo(heroData.skillId1);
                    skillInfo1.characterInstanceId = instanceID;
                }
            }
            else
                skillInfo1 = null;
            if (heroData.skillId2 != 0)
            {
                if (heroData.skillId2 != skillId2)
                {
                    skillInfo2 = new SkillInfo(heroData.skillId2);
                    skillInfo2.characterInstanceId = instanceID;
                }
            }
            else
                skillInfo2 = null;
            hitId = heroData.hitId;
            skillId1 = heroData.skillId1;
            skillId2 = heroData.skillId2;
            return this;
        }

        public CharacterBaseInfo UpdateCharacterBaseInfo(uint skillId, uint newSkillId)
        {
            if (skillId == hitId)
            {
                hitId = newSkillId;
                hitSkillInfo = new SkillInfo(newSkillId);
                hitSkillInfo.characterInstanceId = instanceID;
            }
            else if (skillId == skillId1)
            {
                skillId1 = newSkillId;
                skillInfo1 = new SkillInfo(newSkillId);
                skillInfo1.characterInstanceId = instanceID;
            }
            else if (skillId == skillId2)
            {
                skillId2 = newSkillId;
                skillInfo2 = new SkillInfo(newSkillId);
                skillInfo2.characterInstanceId = instanceID;
            }
            return this;
        }

        public static CharacterBaseInfo CreateCharacterBaseInfo(HeroInfo heroInfo)
        {
            CharacterBaseInfo characterInfo = new CharacterBaseInfo()
            {
                baseId = (uint)heroInfo.heroData.id,
                hitId = heroInfo.heroData.hitId,
                skillId1 = heroInfo.heroData.skillId1,
                skillId2 = heroInfo.heroData.skillId2,
                //passiveId1 = heroInfo.heroData.passiveId1,
                passiveIdDic = new Dictionary<int, int>(),
                passiveId2 = heroInfo.heroData.passiveId2,
                instanceID = heroInfo.instanceID,
                HP = (int)heroInfo.heroData.HP,
                maxHP = heroInfo.heroData.HP,
                floatable = heroInfo.heroData.floatable,
                speed = (int)heroInfo.heroData.speed,
                roleType = heroInfo.heroData.roleType,
                roleInfo = heroInfo
            };
            if (characterInfo.hitId != 0)
            {
                characterInfo.hitSkillInfo = new SkillInfo(characterInfo.hitId);
                characterInfo.hitSkillInfo.characterInstanceId = heroInfo.instanceID;
            }
            if (characterInfo.skillId1 != 0)
            {
                characterInfo.skillInfo1 = new SkillInfo(characterInfo.skillId1);
                characterInfo.skillInfo1.characterInstanceId = heroInfo.instanceID;
            }
            if (characterInfo.skillId2 != 0)
            {
                characterInfo.skillInfo2 = new SkillInfo(characterInfo.skillId2);
                characterInfo.skillInfo2.characterInstanceId = heroInfo.instanceID;
            }
            if (characterInfo.roleInfo.advanceLevel > characterInfo.roleInfo.heroData.starMin)
                characterInfo.dlevel = (uint)characterInfo.roleInfo.advanceLevel - characterInfo.roleInfo.heroData.starMin;
            if (heroInfo.heroData.passiveId1 > 0)
            {
                characterInfo.passiveIdDic.Add((int)heroInfo.heroData.passiveId1, 1);
                string luaFile = "user/skill/passive_skill_" + heroInfo.heroData.passiveId1;
                //if (Lua.Model.LuaData.ExistLuaFile(luaFile))
                LuaScriptMgr.Instance.DoFile(luaFile);
            }
            LuaInterface.LuaFunction initFunc = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.INIT_CHARACTER_DATAS, (int)characterInfo.baseId);
            if (initFunc != null)
                initFunc.Call(characterInfo.instanceID);
            return characterInfo;
        }

        public static CharacterBaseInfo CreateCharacterBaseInfo(PlayerInfo playerInfo)
        {
            CharacterBaseInfo characterInfo = new CharacterBaseInfo()
            {
                baseId = playerInfo.playerData.Id,
                hitId = playerInfo.heroData.hitId,
                skillId1 = playerInfo.heroData.skillId1,
                skillId2 = playerInfo.heroData.skillId2,
                passiveIdDic = new Dictionary<int, int>(),
                //passiveId1 = (uint)playerInfo.passiveSkillId,
                //passiveId1Level = (uint)playerInfo.passiveSkillLevel,
                passiveId2 = playerInfo.heroData.passiveId2,
                aeonEffectId = (uint)playerInfo.summonEffectId,
                aeonSkillLevel = (uint)playerInfo.summonSkillLevel,
                maxAngerValue = playerInfo.playerData.maxAngry,
                instanceID = playerInfo.instanceID,
                HP = (int)playerInfo.heroData.HP,
                maxHP = (uint)playerInfo.heroData.HP,
                angerValue = 0,
                speed = (int)playerInfo.heroData.speed,
                floatable = true,
                roleType = playerInfo.heroData.roleType,
                roleInfo = playerInfo
            };
            if (playerInfo.passiveSkillId > 0)
                characterInfo.passiveIdDic.Add(playerInfo.passiveSkillId, playerInfo.passiveSkillLevel);
            characterInfo.passiveIdDic.AddRange(playerInfo.normalPassiveSkilIdDic);
            if (characterInfo.hitId != 0)
            {
                characterInfo.hitSkillInfo = new SkillInfo(characterInfo.hitId);
                characterInfo.hitSkillInfo.characterInstanceId = playerInfo.instanceID;
            }
            if (characterInfo.skillId1 != 0)
            {
                characterInfo.skillInfo1 = new SkillInfo(characterInfo.skillId1);
                characterInfo.skillInfo1.characterInstanceId = playerInfo.instanceID;
            }
            if (characterInfo.skillId2 != 0)
            {
                characterInfo.skillInfo2 = new SkillInfo(characterInfo.skillId2);
                characterInfo.skillInfo2.characterInstanceId = playerInfo.instanceID;
            }
            if (characterInfo.roleInfo.advanceLevel > characterInfo.roleInfo.heroData.starMin)
                characterInfo.dlevel = (uint)characterInfo.roleInfo.advanceLevel - characterInfo.roleInfo.heroData.starMin;
            string luaFile = "user/skill/passive_skill_" + characterInfo.baseId;
            //if (Lua.Model.LuaData.ExistLuaFile(luaFile))
            LuaScriptMgr.Instance.DoFile(luaFile);
            LuaInterface.LuaFunction initFunc = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.INIT_CHARACTER_DATAS, (int)characterInfo.baseId);
            if (initFunc != null)
                initFunc.Call(characterInfo.instanceID);
            if (characterInfo.aeonEffectId > 0)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.AEON_ID_AND_SKILL_ID, (int)characterInfo.baseId);
                if (func != null)
                {
                    object[] rs = func.Call(characterInfo.aeonEffectId);
                    if (rs.Length > 0)
                    {
                        uint eaonId = 0;
                        uint.TryParse(rs[0].ToString(), out eaonId);
                        uint eaonSkillId = 0;
                        uint.TryParse(rs[1].ToString(), out eaonSkillId);
                        characterInfo.aeonId = eaonId;
                        characterInfo.aeonSkill = eaonSkillId;

                        if (characterInfo.aeonSkill != 0)
                            characterInfo.aeonSkillInfo = new SkillInfo(characterInfo.aeonSkill);
                    }
                }
            }
            if (characterInfo.aeonId == 0)
            {
                characterInfo.aeonId = (uint)playerInfo.playerData.summonID;
                characterInfo.aeonSkill = (uint)playerInfo.playerData.summonSkillId;
                if (characterInfo.aeonSkill != 0)
                {
                    characterInfo.aeonSkillInfo = new SkillInfo(characterInfo.aeonSkill);
                    characterInfo.aeonSkillInfo.characterInstanceId = playerInfo.instanceID;
                }
            }
            return characterInfo;
        }
    }
}
