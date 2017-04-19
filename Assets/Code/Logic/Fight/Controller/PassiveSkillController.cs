using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;
using System.Text;
namespace Logic.Fight.Controller
{
    public class PassiveSkillController : SingletonMono<PassiveSkillController>
    {
        #region const filed
        public const string INIT_CHARACTER_DATAS = "InitCharacterDatas_";
        public const string GET_IMMUNE_BUFFS = "GetImmuneBuffs";
        public const string GET_RANDOM_BUFFS = "GetRandomBuffs";
        public const string GET_DISPERSE_FRIENDS_BUFFS = "GetDisperseFriendsBuffs";
        public const string GET_DISPERSE_ENEMIES_BUFFS = "GetDisperseEnemiesBuffs";
        public const string GET_EXCPETION_BUFFS = "GetExceptionBuffs";

        public const string GET_FOREVER_BUFF_ICON = "GetForeverBuffIcon";
        public const string GET_IMMUNE_BUFF_ICON = "GetImmuneBuffIcon";

        public const string PLAY_SKILL = "playSkill_";
        public const string ADD_BUFF = "addBuff_";
        public const string ADDED_BUFF = "addedBuff_";
        public const string ADD_HALO_BUFF = "addHaloBuff_";
        public const string PHYSICS_ATTACK = "physicsAttack_";
        public const string MAGIC_ATTACK = "magicAttack_";
        public const string PHYSICS_DEFENSE = "physicsDefense_";
        public const string MAGIC_DEFENSE = "magicDefense_";
        public const string IGNORE_DEFENSE = "ignoreDefense_";
        public const string ATTACK_BUFF = "attackBuff_";
        public const string ATTACKED_BUFF = "attackedBuff_";
        public const string TREAT_BUFF = "treatBuff_";
        public const string ATTACK_FINISH_BUFF = "attackFinishBuff_";
        public const string ATTACKED_FINISH_BUFF = "attackedFinishBuff_";
        public const string DODGE = "dodge_";
        public const string CRIT = "crit_";
        public const string BLOCK = "block_";
        public const string CRIT_HURT_DEC = "critHurtDec_";
        public const string KILLER_BUFF = "killerBuff_";
        public const string FRIEND_DEAD = "friendDead_";
        public const string DAMAGE = "damage_";
        public const string DEAD = "dead_";
        public const string ANGRY = "angry_";
        public const string JUDGE_MECHANICS = "judgeMechanics_";
        public const string DAMAGE_2_FLOAT = "damage2Float_";
        public const string FIND_TARGET = "findTarget_";
        public const string AEON_ID_AND_SKILL_ID = "getAeonIdAndSkillId_";
        #endregion
        private StringBuilder _sb;
        void Awake()
        {
            instance = this;
            _sb = new StringBuilder();
        }

        public LuaFunction GetPassiveSkillLuaFunction(string luaFuncName)
        {
            LuaFunction func = null;
            switch (FightController.instance.fightType)
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
                    func = LuaScriptMgr.Instance.GetLuaFunction(luaFuncName);
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    break;
            }
            return func;
        }

        public LuaFunction GetPassiveSkillLuaFunction(string luaFuncName, int id)
        {
            LuaFunction func = null;
            switch (FightController.instance.fightType)
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
                    if (_sb.Length > 0)
                        _sb.Remove(0, _sb.Length);
                    _sb.Append(luaFuncName);
                    _sb.Append(id);
                    func = LuaScriptMgr.Instance.GetLuaFunction(_sb.ToString());
                    break;
                case Logic.Enums.FightType.ConsortiaFight:
                    break;
            }
            return func;
        }
    }
}