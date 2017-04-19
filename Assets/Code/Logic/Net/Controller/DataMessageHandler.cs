using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Audio.Controller;
using Logic.Character;
using Logic.Character.Controller;
using Logic.Fight.Controller;
namespace Logic.Net.Controller
{
    public class DataMessageHandler
    {

        #region client to server
        public static void DataMessage_ReadyFight()
        {
            VirtualServer.instance.ReadyFight();
        }

        public static void DataMessage_ReadySceneSuccess()
        {
            VirtualServer.instance.ReadySceneSuccess();
        }

        public static void DataMessage_ForceFightFinished(bool result, Logic.Enums.FightOverType fightOverType)
        {
            VirtualServer.instance.ForceFightFinished(result, fightOverType);
        }

        public static void DataMessage_PlayAeonSkill(uint id, uint skillID)
        {
            VirtualServer.instance.PlayAeonSkill(id, skillID);
        }

        public static void DataMessage_OrderSkill(uint id, uint skillID, bool forceFirst)
        {
            VirtualServer.instance.OrderSkill(id, skillID, forceFirst);
        }

        public static void DataMessage_ClearConsortiaSkills()
        {
            VirtualServer.instance.ClearConsortiaSkills();
        }

        public static void DataMessage_OrderConsortiaSkill(uint id, uint skillID, long releaseTime, bool forceFirst)
        {
            VirtualServer.instance.OrderConsortiaSkill(id, skillID, releaseTime, forceFirst);
        }
        //初始化角色完成
        public static void DataMessage_InitOK(CharacterType type)
        {
            VirtualServer.instance.InitOK(type);
        }

        public static void DataMessage_FinishSkill(uint skillID, uint characterId, bool isPlayer)
        {
            VirtualServer.instance.FinishSkill(skillID, characterId, isPlayer);
        }

        public static void DataMessage_FinishRun(CharacterType type)
        {
            VirtualServer.instance.FinishRun(type);
        }

        public static void DataMessage_FinishedSkillMechanics(uint skillId, uint characterId, bool isPlayer)
        {
            VirtualServer.instance.FinishedSkillMechanics(skillId, characterId, isPlayer);
        }

        public static void DataMessage_TeamAllDead(CharacterType characterType)
        {
            VirtualServer.instance.TeamAllDead(characterType);
        }

        public static void DataMessage_BreakSkill(uint characterId)
        {
            VirtualServer.instance.BreakSkill(characterId);
        }

        public static void DataMessage_BreakBootSkill(uint characterId)
        {
            VirtualServer.instance.BreakBootSkill(characterId);
        }

        public static void DataMessage_RemoveCharacterFromHitSkillQueue(uint characterId)
        {
            VirtualServer.instance.RemoveCharacterFromHitSkillQueue(characterId);
        }

        public static void DataMessage_FightHangupOrder()
        {
            VirtualServer.instance.FightHangupOrder();
        }

        public static void DataMessage_FightRegainOrder()
        {
            VirtualServer.instance.FightRegainOrder();
        }

        public static void DataMessage_ResetSkillOrder(CharacterEntity character, uint skillId, bool isPlayer)
        {
            VirtualServer.instance.ResetSkillOrder(character, skillId, true);
        }

        public static bool DataMessage_CanCancelSkillOrder(CharacterEntity character, uint skillId)
        {
            return VirtualServer.instance.CanCancelSkillOrder(character, skillId);
        }
        #endregion

        #region server to client
        public static void DataMessage_ReadyScene()
        {
            Fight.Controller.FightController.instance.ReadyScene();
        }

        public static void DataMessage_StartFight()
        {
            Fight.Controller.FightController.instance.StartFight();
        }

        public static void DataMessage_PlayFightStartEffect()
        {
            Fight.Controller.FightController.instance.PlayFightStartEffect();
        }

        public static void DataMessage_PlayNextFightEffect()
        {
            Fight.Controller.FightController.instance.PlayNextFightEffect();
        }

        public static void DataMessage_PlayBossAppearEffect()
        {
            Fight.Controller.FightController.instance.PlayBossAppearEffect();
        }

        public static void DataMessage_FightResult(bool isWin, Logic.Enums.FightOverType fightOverType)
        {
            Fight.Controller.FightController.instance.FinishFight(isWin, fightOverType);
        }

        public static void DataMessage_Run(CharacterType type)
        {
            if (type == CharacterType.Player)
                PlayerController.instance.Run_Scene();
            else
                EnemyController.instance.Run_Scene();
        }

        public static void DataMessage_Dead(uint id, bool isPlayer)
        {
            FightController.instance.Dead(id, isPlayer);
        }

        public static void DataMessage_RunWithoutMove()
        {
            PlayerController.instance.RunWithoutMove_Scene();
            Map.Controller.MapController.instance.MoveForward();
        }

        public static void DataMessage_EndRun(CharacterType type)
        {
            if (type == CharacterType.Player)
                PlayerController.instance.EndRun_Scene();
            //else
            //    EnemyController.instance.EndRun_Scene();
        }

        public static void DataMessage_ResetSkillOrder(uint id, uint skillId)
        {
            Logic.UI.SkillBar.Controller.SkillBarController.instance.ResetSkillOrder(id, skillId);
        }

        public static void DataMessage_ResetSkillOrder()
        {
            Logic.UI.SkillBar.Controller.SkillBarController.instance.ResetSkillOrder();
        }

        public static bool DataMessage_OrderConsortiaSkill(uint id, uint skillId)
        {
            return Logic.UI.SkillBar.Controller.SkillBarController.instance.OrderConsortiaSkill(id, skillId);
        }

        public static void DataMessage_InitPlayers()
        {
            PlayerController.instance.CreateHeros();
        }

        public static void DataMessage_InitEnemys()
        {
            EnemyController.instance.CreateEnemys();
        }

        public static void OrderComboSkill(uint id, uint skillId, bool isPlayer)
        {
            FightController.instance.OrderComboSkill(id, skillId, isPlayer);
        }

        public static void DataMessage_PlayPlayerSkill(uint id, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillId)
        {
            FightController.instance.PlayPlayerSkill(id, timelineList, skillId);
        }

        public static void DataMessage_PlayEnemySkill(uint id, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillId)
        {
            FightController.instance.PlayEnemySkill(id, timelineList, skillId);
        }

        public static void DataMessage_FinishedMechanicsed(uint characterId, bool isPlayer)
        {
            FightController.instance.FinishedMechanicsed(characterId, isPlayer);
        }

        public static void DataMessage_ForceFinishedSkill(uint id, uint skillId, bool isPlayer)
        {
            FightController.instance.ForceFinishedSkill(id, skillId, isPlayer);
        }

        public static void DataMessage_PlayBootSkill(uint id, uint skillId, bool isPlayer)
        {
            if (isPlayer)
                FightController.instance.PlayPlayerBootSkill(id, skillId);
            else
                FightController.instance.PlayEnemyBootSkill(id, skillId);
        }

        public static void DataMessage_TickCD(bool tick)
        {
            FightController.instance.tickCD = tick;
        }

        public static void DataMessage_LastTickTime(float time)
        {
            FightController.instance.lastTickTime = time;
        }

        public static void DataMessage_FightHangup()
        {
            FightController.instance.FightHangup();
        }
        #endregion
    }
}