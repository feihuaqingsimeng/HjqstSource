#if UNITY_EDITOR
//#define log
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Character;
using Logic.Character.Controller;
using Logic.Skill.Model;
using Logic.Enums;
using Common.Util;
using Logic.Skill;
namespace Logic.Net.Controller
{
    public class VirtualServerController : SingletonMono<VirtualServerController>
    {
        private const float INTERVAL = 0.3f;
        private float lastFixedUpdateTime = 0f, lastUpdateTime, lastSkillTime = 0;
        private bool trigger = true;
        public bool autoFight = false;
#if UNITY_EDITOR
        public bool fightEidtor = false;
#endif
        void Awake()
        {
            instance = this;
            runnig = false;
        }

        public bool runnig
        {
            set
            {
                if (this.enabled == value) return;
                this.enabled = value;
                //if (value)
                //{
                //    if (Fight.Controller.FightController.instance.fightType == FightType.PVP)
                //        StartCoroutine("PlaySkillCalcCoroutine");
                //}
                //else
                //    StopCoroutine("PlaySkillCalcCoroutine");
            }
        }

        #region fight logic

        void Update()
        {
            if (VirtualServer.instance.canFight)
            {
                if (Common.GameTime.Controller.TimeController.instance.playerPause)
                    return;
                if (Time.realtimeSinceStartup - lastUpdateTime >= INTERVAL)
                {
                    #region and get hit status and skill status
                    List<HeroEntity> heros = PlayerController.instance.heros;
                    HeroEntity hero = null;
                    for (int i = 0, count = heros.Count; i < count; i++)
                    {
                        hero = heros[i];
                        if (hero.status == Status.GetHit)
                        {
                            if (!VirtualServerControllerUtil.ExistInDic(hero.characterInfo.instanceID, VirtualServer.instance.beLcokedDic))
                            {
                                VirtualServer.instance.FinishedMechanicsed(hero.characterInfo.instanceID, true);
                            }
                        }
                        if (hero.status == Status.Skill)
                        {
                            if (!VirtualServer.instance.skillingDic.ContainsValue(hero.characterInfo.instanceID))
                                hero.ResetStatus();
                        }
                    }
                    hero = null;

                    List<EnemyEntity> enemies = EnemyController.instance.enemies;
                    EnemyEntity enemy = null;
                    for (int i = 0, count = enemies.Count; i < count; i++)
                    {
                        enemy = enemies[i];
                        //if (enemy.HP <= 0 || enemy.isDead)
                        //{
                        //    enemy.isDead = true;
                        //    VirtualServer.instance.VerifyDead(enemy, false);
                        //}
                        if (enemy.status == Status.GetHit)
                        {
                            if (!VirtualServerControllerUtil.ExistInDic(enemy.characterInfo.instanceID, VirtualServer.instance.beLcokedDic))
                            {
                                VirtualServer.instance.FinishedMechanicsed(enemy.characterInfo.instanceID, false);
                            }
                        }
                        if (enemy.status == Status.Skill)
                        {
                            if (!VirtualServer.instance.skillingDic.ContainsValue(enemy.characterInfo.instanceID))
                                enemy.ResetStatus();
                        }
                    }
                    enemy = null;
                    #endregion

                    #region 技能超时验证
                    string[] skillWaitFinishKeys = VirtualServer.instance.skillWaitFinishDic.GetKeyArray();
                    for (int i = 0, count = skillWaitFinishKeys.Length; i < count; i++)
                    {
                        string key = skillWaitFinishKeys[i];
                        float finishTime = VirtualServer.instance.skillWaitFinishDic[key];
                        if (Time.time > finishTime + VirtualServer.FORCE_CLEAR_TIME)
                        {
                            VirtualServer.instance.beLcokedDic.TryDelete(key);
                            VirtualServer.instance.skillingDic.TryDelete(key);
                            VirtualServer.instance.skillWaitFinishDic.TryDelete(key);
                            KeyValuePair<uint, uint> hit = default(KeyValuePair<uint, uint>);
                            string hitKey = string.Empty;
                            if (VirtualServer.instance.hitSkillQueue.Count > 0)
                            {
                                hit = VirtualServer.instance.hitSkillQueue.Peek();
                                hitKey = StringUtil.ConcatNumber(hit.Key, hit.Value);
                            }
                            if (hitKey == key)//普攻
                            {
                                VirtualServer.instance.hitSkillQueue.Dequeue();
                                VirtualServer.instance.hitSkillQueue.Enqueue(hit);
                            }
                            else
                            {
                                KeyValuePair<uint, uint> kvp = key.SplitToKeyValuePair<uint, uint>();
                                HeroEntity heroEntity = PlayerController.instance[kvp.Key];
                                if (heroEntity)
                                {
                                    //heroEntity.ResetCD(heroEntity.GetSkillInfoById(kvp.Value));
                                    VirtualServer.instance.ForceFinishSkill(kvp.Key, kvp.Value, true);
                                }
                                else
                                {
                                    EnemyEntity enemyEntity = EnemyController.instance[kvp.Key];
                                    if (enemyEntity)
                                    {
                                        //enemyEntity.ResetCD(enemyEntity.GetSkillInfoById(kvp.Value));
                                        VirtualServer.instance.ForceFinishSkill(kvp.Key, kvp.Value, false);
                                    }
                                }
                            }
                        }
                    }
                    skillWaitFinishKeys = null;
                    #endregion

                    #region  浮空连击
                    if (Fight.Controller.FightController.instance.isWaitingCombo && !VirtualServer.instance.hangup)
                    {
                        CharacterEntity character = Fight.Controller.FightController.instance.comboer;
                        if (character != null)
                        {
                            #region part of player
                            if (character.isPlayer)
                            {
                                switch (Fight.Controller.FightController.instance.fightStatus)
                                {
                                    case FightStatus.FloatWaiting:
                                        //order float skill
                                        {
                                            if (autoFight)
                                            {
                                                for (int i = 0, count = heros.Count; i < count; i++)
                                                {
                                                    hero = heros[i];
                                                    if (hero == character) continue;
                                                    if (PlayerController.instance.CanSkill1Float(hero))
                                                    {
                                                        hero.skillItemBoxView.skillItemView1.SkillClickHandler();
                                                    }
                                                    if (PlayerController.instance.CanSkill2Float(hero))
                                                    {
                                                        hero.skillItemBoxView.skillItemView2.SkillClickHandler();
                                                    }
                                                }
                                            }
                                            int dicCount = VirtualServer.instance.skillWaitingDic.Count;
                                            if (dicCount > 0)
                                            {
                                                Dictionary<string, uint>.Enumerator e = VirtualServer.instance.skillWaitingDic.GetEnumerator();
                                                Dictionary<uint, uint> floatSkillDic = new Dictionary<uint, uint>();
                                                while (e.MoveNext())
                                                {
                                                    KeyValuePair<string, uint> kvp = e.Current;
                                                    KeyValuePair<uint, uint> key = kvp.Key.SplitToKeyValuePair<uint, uint>();
                                                    HeroEntity heroEntity = PlayerController.instance[key.Key];
                                                    if (heroEntity)
                                                    {
                                                        uint skillId = key.Value;
                                                        SkillInfo skillInfo = heroEntity.GetSkillInfoById(skillId);
                                                        if (SkillUtil.AttackableFloat(skillInfo))
                                                        {
                                                            floatSkillDic.Add(key.Key, key.Value);
                                                        }
                                                    }
                                                }
                                                e.Dispose();
                                                if (floatSkillDic.Count > 0)
                                                {
                                                    Dictionary<uint, uint>.Enumerator floatE = floatSkillDic.GetEnumerator();
                                                    while (floatE.MoveNext())
                                                    {
                                                        KeyValuePair<uint, uint> kvp = floatE.Current;
                                                        VirtualServer.instance.OrderComboSkill(kvp.Key, kvp.Value, true);
                                                        string key = StringUtil.ConcatNumber(kvp.Key, kvp.Value);
                                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                                        Debugger.Log("skillId:{0} of character:{1} turn to float skill.", kvp.Value, kvp.Key);
                                                    }
                                                    floatE.Dispose();
                                                    floatSkillDic.Clear();
                                                }
                                            }
                                        }
                                        break;
                                    case FightStatus.TumbleWaiting:
                                        //order tumble skill
                                        {
                                            if (autoFight)
                                            {
                                                for (int i = 0, count = heros.Count; i < count; i++)
                                                {
                                                    hero = heros[i];
                                                    if (hero == character) continue;
                                                    if (PlayerController.instance.CanSkill1Tumble(hero))
                                                    {
                                                        hero.skillItemBoxView.skillItemView1.SkillClickHandler();
                                                    }
                                                    if (PlayerController.instance.CanSkill2Tumble(hero))
                                                    {
                                                        hero.skillItemBoxView.skillItemView2.SkillClickHandler();
                                                    }
                                                }
                                            }
                                            int dicCount = VirtualServer.instance.skillWaitingDic.Count;
                                            if (dicCount > 0)
                                            {
                                                Dictionary<string, uint>.Enumerator e = VirtualServer.instance.skillWaitingDic.GetEnumerator();
                                                Dictionary<uint, uint> tumbleSkillDic = new Dictionary<uint, uint>();
                                                while (e.MoveNext())
                                                {
                                                    KeyValuePair<string, uint> kvp = e.Current;
                                                    KeyValuePair<uint, uint> key = kvp.Key.SplitToKeyValuePair<uint, uint>();
                                                    HeroEntity heroEntity = PlayerController.instance[key.Key];
                                                    if (heroEntity)
                                                    {
                                                        uint skillId = key.Value;
                                                        SkillInfo skillInfo = heroEntity.GetSkillInfoById(skillId);
                                                        if (SkillUtil.AttackableTumble(skillInfo))
                                                        {
                                                            tumbleSkillDic.Add(key.Key, key.Value);
                                                        }
                                                    }
                                                }
                                                e.Dispose();
                                                if (tumbleSkillDic.Count > 0)
                                                {
                                                    Dictionary<uint, uint>.Enumerator tumbleE = tumbleSkillDic.GetEnumerator();
                                                    while (tumbleE.MoveNext())
                                                    {
                                                        KeyValuePair<uint, uint> kvp = tumbleE.Current;
                                                        VirtualServer.instance.OrderComboSkill(kvp.Key, kvp.Value, true);
                                                        string key = StringUtil.ConcatNumber(kvp.Key, kvp.Value);
                                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                                        Debugger.Log("skillId:{0} of character:{1} turn to tumble skill.", kvp.Value, kvp.Key);
                                                    }
                                                    tumbleE.Dispose();
                                                    tumbleSkillDic.Clear();
                                                }
                                            }
                                        }
                                        break;
                                }
                                hero = null;
                            }
                            #endregion
                            #region part of enemy
                            else
                            {
                                switch (Fight.Controller.FightController.instance.fightStatus)
                                {
                                    case FightStatus.FloatWaiting:
                                        //order float skill
                                        {
                                            if (autoFight)
                                            {
                                                for (int i = 0, count = enemies.Count; i < count; i++)
                                                {
                                                    enemy = enemies[i];
                                                    if (enemy == character) continue;
                                                    if (EnemyController.instance.CanSkill1Float(enemy))
                                                    {
                                                        VirtualServer.instance.OrderComboSkill(enemy.characterInfo.instanceID, enemy.characterInfo.skillId1, false);
                                                    }
                                                    if (EnemyController.instance.CanSkill2Float(enemy))
                                                    {
                                                        VirtualServer.instance.OrderComboSkill(enemy.characterInfo.instanceID, enemy.characterInfo.skillId2, false);
                                                    }
                                                }
                                            }
                                            int dicCount = VirtualServer.instance.skillWaitingDic.Count;
                                            if (dicCount > 0)
                                            {
                                                Dictionary<string, uint>.Enumerator e = VirtualServer.instance.skillWaitingDic.GetEnumerator();
                                                Dictionary<uint, uint> floatSkillDic = new Dictionary<uint, uint>();
                                                while (e.MoveNext())
                                                {
                                                    KeyValuePair<string, uint> kvp = e.Current;
                                                    KeyValuePair<uint, uint> key = kvp.Key.SplitToKeyValuePair<uint, uint>();
                                                    EnemyEntity enemyEntity = EnemyController.instance[key.Key];
                                                    if (enemyEntity)
                                                    {
                                                        uint skillId = key.Value;
                                                        SkillInfo skillInfo = enemyEntity.GetSkillInfoById(skillId);
                                                        if (SkillUtil.AttackableFloat(skillInfo))
                                                        {
                                                            floatSkillDic.Add(key.Key, key.Value);
                                                        }
                                                    }
                                                }
                                                e.Dispose();
                                                if (floatSkillDic.Count > 0)
                                                {
                                                    Dictionary<uint, uint>.Enumerator floatE = floatSkillDic.GetEnumerator();
                                                    while (floatE.MoveNext())
                                                    {
                                                        KeyValuePair<uint, uint> kvp = floatE.Current;
                                                        VirtualServer.instance.OrderComboSkill(kvp.Key, kvp.Value, false);
                                                        string key = StringUtil.ConcatNumber(kvp.Key, kvp.Value);
                                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                                        Debugger.Log("skillId:{0} of character:{1} turn to float skill.", kvp.Value, kvp.Key);
                                                    }
                                                    floatE.Dispose();
                                                    floatSkillDic.Clear();
                                                }
                                            }
                                        }
                                        break;
                                    case FightStatus.TumbleWaiting:
                                        //order tumble skill
                                        {
                                            if (autoFight)
                                            {
                                                for (int i = 0, count = enemies.Count; i < count; i++)
                                                {
                                                    enemy = enemies[i];
                                                    if (enemy == character) continue;
                                                    if (EnemyController.instance.CanSkill1Tumble(enemy))
                                                    {
                                                        VirtualServer.instance.OrderComboSkill(enemy.characterInfo.instanceID, enemy.characterInfo.skillId1, false);
                                                    }
                                                    if (EnemyController.instance.CanSkill2Tumble(enemy))
                                                    {
                                                        VirtualServer.instance.OrderComboSkill(enemy.characterInfo.instanceID, enemy.characterInfo.skillId2, false);
                                                    }
                                                }
                                            }
                                            int dicCount = VirtualServer.instance.skillWaitingDic.Count;
                                            if (dicCount > 0)
                                            {
                                                Dictionary<string, uint>.Enumerator e = VirtualServer.instance.skillWaitingDic.GetEnumerator();
                                                Dictionary<uint, uint> tumbleSkillDic = new Dictionary<uint, uint>();
                                                while (e.MoveNext())
                                                {
                                                    KeyValuePair<string, uint> kvp = e.Current;
                                                    KeyValuePair<uint, uint> key = kvp.Key.SplitToKeyValuePair<uint, uint>();
                                                    EnemyEntity enemyEntity = EnemyController.instance[key.Key];
                                                    if (enemyEntity)
                                                    {
                                                        uint skillId = key.Value;
                                                        SkillInfo skillInfo = enemyEntity.GetSkillInfoById(skillId);
                                                        if (SkillUtil.AttackableTumble(skillInfo))
                                                        {
                                                            tumbleSkillDic.Add(key.Key, key.Value);
                                                        }
                                                    }
                                                }
                                                e.Dispose();
                                                if (tumbleSkillDic.Count > 0)
                                                {
                                                    Dictionary<uint, uint>.Enumerator tumbleE = tumbleSkillDic.GetEnumerator();
                                                    while (tumbleE.MoveNext())
                                                    {
                                                        KeyValuePair<uint, uint> kvp = tumbleE.Current;
                                                        VirtualServer.instance.OrderComboSkill(kvp.Key, kvp.Value, false);
                                                        string key = StringUtil.ConcatNumber(kvp.Key, kvp.Value);
                                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                                        Debugger.Log("skillId:{0} of character:{1} turn to tumble skill.", kvp.Value, kvp.Key);
                                                    }
                                                    tumbleE.Dispose();
                                                    tumbleSkillDic.Clear();
                                                }
                                            }
                                        }
                                        break;
                                }
                                enemy = null;
                            }
                            #endregion
                        }
                    }
                    #endregion
                    lastUpdateTime = Time.realtimeSinceStartup;
                }
            }
        }

        void FixedUpdate()
        {
            if (VirtualServer.instance.canFight)
            {
                if (Common.GameTime.Controller.TimeController.instance.playerPause)
                    return;
                if (Time.time - lastFixedUpdateTime >= INTERVAL)
                {
                    #region verify dead
                    switch (Fight.Controller.FightController.instance.fightType)
                    {
                        case FightType.PVE:
                        case FightType.Arena:
                        case FightType.DailyPVE:
                        case FightType.Expedition:
                        case FightType.WorldTree:
                        case FightType.WorldBoss:
                        case FightType.FirstFight:
                        case FightType.PVP:
                        case FightType.FriendFight:
                        case FightType.MineFight:
#if UNITY_EDITOR
                        case FightType.Imitate:
#endif
                            //try
                            {
                                List<HeroEntity> heros = PlayerController.instance.heros;
                                HeroEntity hero = null;
                                for (int i = 0; i < heros.Count; i++)
                                {
                                    hero = heros[i];
                                    if (hero.HP <= 0 || hero.isDead)
                                    {
                                        hero.isDead = true;
                                        VirtualServer.instance.VerifyDead(hero, true);
                                    }
                                }
                                hero = null;

                                List<EnemyEntity> enemies = EnemyController.instance.enemies;
                                EnemyEntity enemy = null;
                                for (int i = 0; i < enemies.Count; i++)
                                {
                                    enemy = enemies[i];
                                    if (enemy.HP <= 0 || enemy.isDead)
                                    {
                                        enemy.isDead = true;
                                        VirtualServer.instance.VerifyDead(enemy, false);
                                    }
                                }
                                enemy = null;
                            }
                            break;
                        case FightType.SkillDisplay:
                        case FightType.ConsortiaFight:
                            break;
                    }
                    //catch (System.Exception e)
                    //{
                    //    Debugger.LogError("thread exception:{0}", e.StackTrace);
                    //}
                    #endregion
                    //if (Fight.Controller.FightController.instance.fightType != FightType.PVP)
                    PlaySkillCalc();
                    lastFixedUpdateTime = Time.time;
                }
            }
        }

        private IEnumerator PlaySkillCalcCoroutine()
        {
            while (true)
            {
                PlaySkillCalc();
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void PlaySkillCalc()
        {
            #region fight ai
            if (!VirtualServer.instance.interlude)
            {
                #region checked
                if (PlayerController.instance.heros.Count == 0 || EnemyController.instance.enemies.Count == 0) return;
                #endregion

                #region aeon skill
                if (VirtualServer.instance.aeonSkillDic.Count > 0)
                {
                    KeyValuePair<uint, uint> kvp = VirtualServer.instance.aeonSkillDic.First();
                    //Debugger.LogError(kvp.Key + "   " + kvp.Value);
                    if (VirtualServer.instance.skillingDic.Count == 0)
                    {
                        if (!VirtualServer.instance.aeonSkillingDic.ContainsKey(kvp.Key))
                        {
                            VirtualServer.instance.aeonSkillDic.TryDelete(kvp.Key);
                            VirtualServer.instance.aeonSkillingDic.TryAdd(kvp.Key, kvp.Value);
                            PlaySkill(kvp.Value, kvp.Key);
                            string key = StringUtil.ConcatNumber<uint>(kvp.Value, kvp.Key);
                            if (!VirtualServer.instance.skillWaitFinishDic.ContainsKey(key))
                            {
                                AnimationData animationData = AnimationData.GetAnimationDataById(kvp.Key);
                                SkillData skillData = SkillData.GetSkillDataById(kvp.Key);
                                float finishTime = Time.time + animationData.length - skillData.pauseTime;
                                VirtualServer.instance.skillWaitFinishDic.Add(key, finishTime);
                            }
                        }
                    }
                    //lastFixedUpdateTime = Time.time;
                    return;
                }
                if (VirtualServer.instance.aeonSkillingDic.Count > 0)
                {
                    //lastFixedUpdateTime = Time.time;
                    return;
                }
                #endregion

                #region delay skill
                if (VirtualServer.instance.bootSkillWaitingDic.Count > 0)
                {
                    //List<string> bootSkillKeys = VirtualServer.instance.bootSkillWaitingDic.GetKeys();
                    //for (int i = 0, count = bootSkillKeys.Count; i < count; i++)
                    Dictionary<string, SkillStruct>.Enumerator e = VirtualServer.instance.bootSkillWaitingDic.GetEnumerator();
                    //foreach (var kvp in VirtualServer.instance.bootSkillWaitingDic)
                    while (e.MoveNext())
                    {
                        KeyValuePair<string, SkillStruct> kvp = e.Current;
                        SkillStruct bootSkillStruct = kvp.Value;
                        if (Time.time >= bootSkillStruct.time)
                        {
                            if (bootSkillStruct.isPlayer)
                            {
                                HeroEntity heroEntity = PlayerController.instance[bootSkillStruct.id];
                                if (heroEntity && heroEntity.CanPlaySkill(bootSkillStruct.skillId))
                                {
                                    PlaySkill(bootSkillStruct.id, bootSkillStruct.skillId);
                                    //lastFixedUpdateTime = Time.time;
                                    return;
                                }
                            }
                            else
                            {
                                EnemyEntity enemyEntity = EnemyController.instance[bootSkillStruct.id];
                                if (enemyEntity && enemyEntity.CanPlaySkill(bootSkillStruct.skillId))
                                {
                                    PlaySkill(bootSkillStruct.id, bootSkillStruct.skillId, false);
                                    //lastFixedUpdateTime = Time.time;
                                    return;
                                }
                            }
                        }
                    }
                    e.Dispose();
                }
                #endregion

                if (VirtualServer.instance.hangup)
                {
                    if (VirtualServer.instance.skillingDic.Count == 0 && VirtualServer.instance.skillWaitingDic.Count == 0)
                        VirtualServer.instance.FightHangup();
                }
                else
                {
#if UNITY_EDITOR
                    if (!fightEidtor)
                    {
#endif
                        #region hit skill(include enemy skill) ai
                        switch (Fight.Controller.FightController.instance.fightType)
                        {
                            case FightType.PVE:
                            case FightType.Arena:
                            case FightType.DailyPVE:
                            case FightType.Expedition:
                            case FightType.WorldTree:
                            case FightType.WorldBoss:
                            case FightType.FirstFight:
                            case FightType.PVP:
                            case FightType.FriendFight:
                            case FightType.MineFight:
#if UNITY_EDITOR
                            case FightType.Imitate:
#endif
                                if (trigger)
                                {
                                    #region auto fight
                                    if (autoFight)
                                    {
                                        Logic.UI.SkillBar.Controller.SkillBarController.instance.AutoReleaseSkill();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region enemies order skill auto
                                    if (VirtualServer.instance.enemyAutoOrder)
                                    {
                                        List<SkillInfo> skillInfos = EnemyController.instance.skillInfos;
                                        for (int i = 0, count = skillInfos.Count; i < count; i++)
                                        {
                                            SkillInfo skillInfo = skillInfos[i];
                                            if (skillInfo == null)
                                                continue;
                                            EnemyEntity enemy = EnemyController.instance[skillInfo.characterInstanceId];
                                            if (!enemy) continue;
                                            if (enemy.characterInfo.skillInfo1 == skillInfo)
                                            {
                                                if (enemy.canOrderSkill && enemy.canOrderSkill1)
                                                {
                                                    string key = StringUtil.ConcatNumber(skillInfo.characterInstanceId, enemy.characterInfo.skillId1);
                                                    if (!VirtualServer.instance.skillWaitingDic.ContainsKey(key) && !VirtualServer.instance.skillingDic.ContainsKey(key))
                                                    {
                                                        VirtualServer.instance.skillWaitingDic.TryAdd(key, skillInfo.characterInstanceId);
                                                        lock (VirtualServer.instance.skillWaitingQueue)
                                                            VirtualServer.instance.skillWaitingQueue.Enqueue(key);
                                                        enemy.canOrderTime = float.MaxValue;
                                                        enemy.canOrderSkill1 = false;
                                                    }
                                                }
                                            }
                                            else if (enemy.characterInfo.skillInfo2 == skillInfo)
                                            {
                                                if (enemy.canOrderSkill && enemy.canOrderSkill2)
                                                {
                                                    string key = StringUtil.ConcatNumber(skillInfo.characterInstanceId, enemy.characterInfo.skillId2);
                                                    if (!VirtualServer.instance.skillWaitingDic.ContainsKey(key) && !VirtualServer.instance.skillingDic.ContainsKey(key))
                                                    {
                                                        VirtualServer.instance.skillWaitingDic.TryAdd(key, skillInfo.characterInstanceId);
                                                        lock (VirtualServer.instance.skillWaitingQueue)
                                                            VirtualServer.instance.skillWaitingQueue.Enqueue(key);
                                                        enemy.canOrderTime = float.MaxValue;
                                                        enemy.canOrderSkill2 = false;
                                                    }
                                                }
                                            }
                                        }
                                        /*List<EnemyEntity> enemies = EnemyController.instance.enemies;
                                        EnemyEntity enemy = null;
                                        for (int i = 0, count = enemies.Count; i < count; i++)
                                        {
                                            enemy = enemies[i];
                                            uint id = enemy.characterInfo.instanceID;
                                            if (!enemy) continue;
                                            if (enemy.canOrderSkill1)
                                            {
                                                string key = StringUtil.ConcatNumber(id, enemy.characterInfo.skillId1);
                                                if (!VirtualServer.instance.skillWaitingDic.ContainsKey(key) && !VirtualServer.instance.skillingDic.ContainsKey(key))
                                                {
                                                    VirtualServer.instance.skillWaitingDic.TryAdd(key, id);
                                                    lock (VirtualServer.instance.skillWaitingQueue)
                                                        VirtualServer.instance.skillWaitingQueue.Enqueue(key);
                                                    enemy.canOrderSkill1 = false;
                                                }
                                            }
                                            if (enemy.canOrderSkill2)
                                            {
                                                string key = StringUtil.ConcatNumber(id, enemy.characterInfo.skillId2);
                                                if (!VirtualServer.instance.skillWaitingDic.ContainsKey(key) && !VirtualServer.instance.skillingDic.ContainsKey(key))
                                                {
                                                    VirtualServer.instance.skillWaitingDic.TryAdd(key, id);
                                                    lock (VirtualServer.instance.skillWaitingQueue)
                                                        VirtualServer.instance.skillWaitingQueue.Enqueue(key);
                                                    enemy.canOrderSkill2 = false;
                                                }
                                            }
                                        }
                                        enemy = null;*/
                                    }
                                    #endregion
                                }
                                trigger = !trigger;
                                break;
                            case FightType.SkillDisplay:
                            case FightType.ConsortiaFight:
                                break;
                        }
#if UNITY_EDITOR
                    }
#endif
                        #endregion
                }
                if (VirtualServer.instance.skillingDic.Count > 0) return;
                #region play skill
                int dicCount = VirtualServer.instance.skillWaitingDic.Count;
                if (dicCount > 0)
                {
                    string key = VirtualServerControllerUtil.GetFirstSkillKey();
                    if (string.IsNullOrEmpty(key)) return;
                    KeyValuePair<uint, uint> kvp = key.SplitToKeyValuePair<uint, uint>();
                    //if (!VirtualServer.instance.calcTimeDic.ContainsKey(kvp.Value))
                    //    VirtualServer.instance.calcTimeDic.Add(kvp.Value, Common.Util.TimeUtil.GetTimeStamp());
                    uint skillId = kvp.Value;
                    switch (Fight.Controller.FightController.instance.fightType)
                    {
                        case FightType.PVE:
                        case FightType.Arena:
                        case FightType.DailyPVE:
                        case FightType.Expedition:
                        case FightType.WorldTree:
                        case FightType.WorldBoss:
                        case FightType.FirstFight:
                        case FightType.SkillDisplay:
                        case FightType.PVP:
                        case FightType.FriendFight:
                        case FightType.MineFight:
#if UNITY_EDITOR
                        case FightType.Imitate:
#endif
                            #region calc skill normal
                            HeroEntity heroEntity = PlayerController.instance[kvp.Key];
                            if (heroEntity)
                            {
                                if (heroEntity.controled || heroEntity.Silence)//被控制直接取消预约
                                {
                                    VirtualServer.instance.ResetSkillOrder(heroEntity, skillId, true);
                                    //VirtualServer.instance.calcTimeDic.TryDelete(kvp.Value);
                                    //Debugger.Log("skill {0} has been cancled!", skillId);
                                }
                                else
                                {
                                    if (heroEntity)
                                    {
                                        if (heroEntity.ExistSkill(skillId))
                                        {
                                            if (heroEntity.CanPlaySkill(skillId))
                                                PlaySkill(kvp.Key, skillId, true);
                                        }
                                        else
                                        {
                                            //预约错误，原因还不知道
                                            VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                            //VirtualServer.instance.calcTimeDic.TryDelete(kvp.Value);
                                            Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, heroEntity.characterInfo.baseId, skillId);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                EnemyEntity enemyEntity = EnemyController.instance[kvp.Key];
                                if (!enemyEntity)
                                {
                                    Debugger.LogError("can not find characterId:{0} skillId{1}", kvp.Key, kvp.Value);
                                    VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                    //VirtualServer.instance.calcTimeDic.TryDelete(kvp.Value);
                                    return;
                                }
                                if (enemyEntity.controled || enemyEntity.Silence)//被控制直接取消预约
                                {
                                    VirtualServer.instance.ResetSkillOrder(enemyEntity, skillId, false);
                                    //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                    //Debugger.Log("skill {0} has been cancled!", skillId);
                                }
                                else
                                {
                                    if (enemyEntity.ExistSkill(skillId))
                                    {
                                        if (enemyEntity.CanPlaySkill(skillId))
                                            PlaySkill(kvp.Key, skillId, false);
                                    }
                                    else
                                    {
                                        //预约错误，原因还不知道
                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                        //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                        Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, enemyEntity.characterInfo.baseId, skillId);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case FightType.ConsortiaFight:
                            #region do not calc skill
                            CharacterEntity character = CharacterUtil.FindTarget(kvp.Key);
                            if (character)
                            {
                                if (character.isPlayer)
                                {
                                    if (character.ExistSkill(skillId))
                                    {
                                        //if (character.CanPlaySkill(skillId))
                                        PlaySkill(kvp.Key, skillId, true);
                                    }
                                    else
                                    {
                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                        Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, character.characterInfo.baseId, skillId);
                                    }
                                }
                                else
                                {
                                    if (character.ExistSkill(skillId))
                                    {
                                        //if (character.CanPlaySkill(skillId))
                                        PlaySkill(kvp.Key, skillId, false);
                                    }
                                    else
                                    {
                                        VirtualServer.instance.skillWaitingDic.TryDelete(key);
                                        Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, character.characterInfo.baseId, skillId);
                                    }
                                }
                            }
                            else
                            {
                                Debugger.LogError("can not find characterId:{0} skillId{1}", kvp.Key, kvp.Value);
                                VirtualServer.instance.skillWaitingDic.TryDelete(key);
                            }
                            #endregion
                            break;
                    }
                }
                #endregion
                #region hit skill
                else if (!VirtualServer.instance.hangup)
                {
#if UNITY_EDITOR
                    if (!fightEidtor)
                    {
#endif
                        switch (Fight.Controller.FightController.instance.fightType)
                        {
                            case FightType.PVE:
                            case FightType.Arena:
                            case FightType.DailyPVE:
                            case FightType.Expedition:
                            case FightType.WorldTree:
                            case FightType.WorldBoss:
                            case FightType.PVP:
                            case FightType.FriendFight:
                            case FightType.MineFight:
#if UNITY_EDITOR
                            case FightType.Imitate:
#endif
                                lock (VirtualServer.instance.hitSkillQueue)
                                {
                                    if (VirtualServer.instance.hitSkillQueue.Count > 0)
                                    {
                                        KeyValuePair<uint, uint> kvp = VirtualServer.instance.hitSkillQueue.Peek();
                                        //if (!VirtualServer.instance.calcTimeDic.ContainsKey(kvp.Value))
                                        //    VirtualServer.instance.calcTimeDic.Add(kvp.Value, Common.Util.TimeUtil.GetTimeStamp());
                                        HeroEntity heroEntity = PlayerController.instance[kvp.Key];
                                        if (heroEntity)
                                        {
                                            uint skillId = kvp.Value;
                                            if (heroEntity.controled)//被控制直接取消预约
                                            {
                                                VirtualServer.instance.hitSkillQueue.Dequeue();
                                                VirtualServer.instance.hitSkillQueue.Enqueue(kvp);
                                                //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                                Debugger.Log("skill {0} has been cancled!", skillId);
                                            }
                                            else
                                            {
                                                if (heroEntity)
                                                {
                                                    if (heroEntity.ExistSkill(skillId))
                                                    {
                                                        if (heroEntity.CanPlaySkill(skillId))
                                                            PlaySkill(kvp.Key, skillId, true);
                                                    }
                                                    else
                                                    {
                                                        //预约错误
                                                        VirtualServer.instance.hitSkillQueue.Dequeue();
                                                        //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                                        Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, heroEntity.characterInfo.baseId, skillId);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            EnemyEntity enemyEntity = EnemyController.instance[kvp.Key];
                                            if (!enemyEntity)
                                            {
                                                VirtualServer.instance.hitSkillQueue.Dequeue();
                                                VirtualServer.instance.hitSkillQueue.Enqueue(kvp);
                                                //Debugger.LogError("can not find characterId:{0} skillId{1}", kvp.Key, kvp.Value);
                                                return;
                                            }
                                            uint skillId = kvp.Value;
                                            if (enemyEntity.controled)//被控制直接取消预约
                                            {
                                                VirtualServer.instance.hitSkillQueue.Dequeue();
                                                VirtualServer.instance.hitSkillQueue.Enqueue(kvp);
                                                //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                                Debugger.Log("skill {0} has been cancled!", skillId);
                                            }
                                            else
                                            {
                                                if (enemyEntity.ExistSkill(skillId))
                                                {
                                                    if (enemyEntity.CanPlaySkill(skillId))
                                                        PlaySkill(kvp.Key, skillId, false);
                                                }
                                                else
                                                {
                                                    //预约错误
                                                    VirtualServer.instance.hitSkillQueue.Dequeue();
                                                    //VirtualServer.instance.calcTimeDic.TryDelete(skillId);
                                                    Debugger.LogError("userId:{0},baseId:{1} has not {2} skill", kvp.Key, enemyEntity.characterInfo.baseId, skillId);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case FightType.FirstFight:
                            case FightType.SkillDisplay:
                            case FightType.ConsortiaFight:
                                break;
                        }
#if UNITY_EDITOR
                    }
#endif
                }
                #endregion
            }
            #endregion
        }

        private void PlayBootSkill(uint characterId, SkillData skillData, bool isPlayer = true)
        {
            float time = Time.time + skillData.bootTime;
            SkillStruct delaySkillStruct = new SkillStruct(characterId, skillData.skillId, isPlayer, time);
            string key = StringUtil.ConcatNumber<uint>(characterId, skillData.skillId);
            VirtualServer.instance.skillWaitingDic.TryDelete(key);
            VirtualServer.instance.bootSkillWaitingDic.TryAdd(key, delaySkillStruct);
            VirtualServer.instance.PlayBootSkill(characterId, skillData.skillId, isPlayer);
        }

        private void PlaySkill(uint characterId, uint skillId, bool isPlayer = true)
        {
            if (VirtualServer.instance.skillingDic.Count > 0) return;//一个技能完成才能进行下一技能
            //if (VirtualServerControllerUtil.ExistInDic(characterId, VirtualServer.instance.beLcokedDic))//如果被锁定，不允许攻击
            //    return;
            //if (HasCloseupSkill()) return;//有特写镜头技能必须等技能释放完成

            //if (VirtualServer.instance.skillingDic.ContainsKey(key)) return;//已经释放技能不允许释放
            string key = StringUtil.ConcatNumber<uint>(characterId, skillId);
            SkillData skillData = SkillData.GetSkillDataById(skillId);
            if (skillData.bootTime > 0)
            {
                if (!VirtualServer.instance.bootSkillWaitingDic.ContainsKey(key))
                {
                    PlayBootSkill(characterId, skillData, isPlayer);
                    return;
                }
            }
            //if (VirtualServer.instance.beLcokedDic.Count > 0)
            //{
            //    //完成受击后才可进行下一技能
            //    if (VirtualServer.instance.beLcokedDic.First().Value.Count > 0)
            //        return;
            //    if (VirtualServer.instance.beLcokedDic.Count > 1)
            //        return;
            //}
            List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList = GetTargets(characterId, skillData, isPlayer);
            if (timelineList == null)//找不到characerId
            {
                VirtualServer.instance.skillWaitingDic.TryDelete(key);
                VirtualServer.instance.bootSkillWaitingDic.TryDelete(key);
                VirtualServer.instance.aeonSkillingDic.TryDelete(skillId);
                return;
            }
            List<KeyValuePair<uint, uint>> targetTids = new List<KeyValuePair<uint, uint>>();
            #region filter target
            bool canPlay = true;
            for (int i = 0, count = timelineList.Count; i < count; i++)
            {
                Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[i];
                List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                for (int j = 0, jCount = mechanicsKeys.Count; j < jCount; j++)
                {
                    uint mechanicsId = mechanicsKeys[j];
                    List<KeyValuePair<uint, uint>> tids = mechanicsDic[mechanicsId];
                    List<KeyValuePair<uint, uint>> tidsCopy = new List<KeyValuePair<uint, uint>>(tids);
                    MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                    #region check character status
                    if (mechanicsData.targetType == TargetType.Ally && isPlayer)//我方buff
                    {
                        for (int k = 0, kCount = tids.Count; k < kCount; k++)
                        {
                            KeyValuePair<uint, uint> kvp = tids[k];
                            CharacterEntity characterEntity = PlayerController.instance[kvp.Key];
                            if (characterEntity == null)
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            targetTids.AddRange(tidsCopy, true);
                        }
                    }
                    else if (mechanicsData.targetType == TargetType.Enemy && isPlayer)//我方对敌方debuff&damage
                    {
                        for (int k = 0, kCount = tids.Count; k < kCount; k++)
                        {
                            KeyValuePair<uint, uint> kvp = tids[k];
                            EnemyEntity enemy = EnemyController.instance[kvp.Key];
                            if (enemy == null)
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            if (VirtualServer.instance.skillingDic.ContainsValue(kvp.Key))
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            targetTids.AddRange(tidsCopy, true);
                            switch (mechanicsData.rangeType)
                            {
                                case RangeType.Cross:
                                case RangeType.FirstColum:
                                case RangeType.SecondColum:
                                case RangeType.ThirdColum:
                                case RangeType.FirstRow:
                                case RangeType.SecondRow:
                                case RangeType.ThirdRow:
                                case RangeType.ExceptMidpoint:
                                case RangeType.Midpoint:
                                case RangeType.LeadingDiagonal:
                                case RangeType.SecondaryDiagonal:
                                case RangeType.AllAbsolutely:
                                case RangeType.BehindTowColum:
                                    break;
                                default:
                                    if (mechanicsData.needTarget)
                                        canPlay &= enemy.canAttack;
                                    break;
                            }
                        }
                    }
                    else if (mechanicsData.targetType == TargetType.Ally && !isPlayer)//敌方buff
                    {
                        for (int k = 0, kCount = tids.Count; k < kCount; k++)
                        {
                            KeyValuePair<uint, uint> kvp = tids[k];
                            EnemyEntity enemy = EnemyController.instance[kvp.Key];
                            if (enemy == null)
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            targetTids.AddRange(tidsCopy, true);
                        }
                    }
                    else if (mechanicsData.targetType == TargetType.Enemy && !isPlayer)//敌方对我方debuff&damage
                    {
                        for (int k = 0, kCount = tids.Count; k < kCount; k++)
                        {
                            KeyValuePair<uint, uint> kvp = tids[k];
                            CharacterEntity characterEntity = PlayerController.instance[kvp.Key];
                            if (characterEntity == null)
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            if (VirtualServer.instance.skillingDic.ContainsValue(kvp.Key))
                            {
                                tidsCopy.Remove(kvp);
                                continue;
                            }
                            targetTids.AddRange(tidsCopy, true);
                            switch (mechanicsData.rangeType)
                            {
                                case RangeType.Cross:
                                case RangeType.FirstColum:
                                case RangeType.SecondColum:
                                case RangeType.ThirdColum:
                                case RangeType.FirstRow:
                                case RangeType.SecondRow:
                                case RangeType.ThirdRow:
                                case RangeType.ExceptMidpoint:
                                case RangeType.Midpoint:
                                case RangeType.LeadingDiagonal:
                                case RangeType.SecondaryDiagonal:
                                case RangeType.AllAbsolutely:
                                case RangeType.BehindTowColum:
                                    break;
                                default:
                                    if (mechanicsData.needTarget)
                                        canPlay &= characterEntity.canAttack;
                                    break;
                            }
                        }
                    }
                    mechanicsDic[mechanicsId] = tidsCopy;
                    #endregion
                    //if (mechanicsData.needTarget && tidsCopy.Count == 0)
                    //    canPlay &= false;
                    switch (mechanicsData.rangeType)
                    {
                        case RangeType.Cross:
                        case RangeType.FirstColum:
                        case RangeType.SecondColum:
                        case RangeType.ThirdColum:
                        case RangeType.FirstRow:
                        case RangeType.SecondRow:
                        case RangeType.ThirdRow:
                        case RangeType.ExceptMidpoint:
                        case RangeType.Midpoint:
                        case RangeType.LeadingDiagonal:
                        case RangeType.SecondaryDiagonal:
                        case RangeType.AllAbsolutely:
                            break;
                        default:
                            if (mechanicsData.mechanicsType != MechanicsType.Reborn)//重生没有目标
                                if (tidsCopy.Count == 0)
                                    canPlay &= false;
                            break;
                    }
                    if (!canPlay)
                        break;
                }
                if (!canPlay)
                    break;
            }
            #endregion
            if (canPlay)
            {
                //lastSkillTime = Time.time;
                VirtualServer.instance.skillWaitingDic.TryDelete(key);
                VirtualServer.instance.bootSkillWaitingDic.TryDelete(key);
                VirtualServer.instance.skillingDic.TryAdd(key, characterId);
                VirtualServer.instance.beLcokedDic.TryAdd(key, targetTids);

                if (isPlayer)
                    VirtualServer.instance.PlayPlayerSkill(characterId, timelineList, skillId);
                else
                    VirtualServer.instance.PlayEnemySkill(characterId, timelineList, skillId);
                //if (VirtualServer.instance.orderTimeDic.ContainsKey(skillId))
                //    Debugger.Log("cost time from order {0}", (Common.Util.TimeUtil.GetTimeStamp() - VirtualServer.instance.orderTimeDic[skillId]).ToString());
                //if (VirtualServer.instance.calcTimeDic.ContainsKey(skillId))
                //{
                //    Debugger.Log("cost time from first calc {0}", (Common.Util.TimeUtil.GetTimeStamp() - VirtualServer.instance.calcTimeDic[skillId]).ToString());
                //    VirtualServer.instance.calcTimeDic.Remove(skillId);
                //}
                //#if UNITY_EDITOR
                //                Debugger.Log("calc skill {0} cost time:{1}", skillId, (Common.Util.TimeUtil.GetTimeStamp() - _time).ToString());
                //#endif
                //if (Fight.Controller.FightController.instance.fightType == FightType.PVP)
                //    VirtualServer.instance.CalcPVPSkillDelay(skillId);
                AnimationData animationData = AnimationData.GetAnimationDataById(skillId);
                float finishTime = Time.time + animationData.length - skillData.pauseTime;
                VirtualServer.instance.skillWaitFinishDic.TryAdd(key, finishTime);
            }
        }

        //private bool HasCloseupSkill()
        //{
        //    bool result = false;
        //    string[] keys = VirtualServer.instance.skillingDic.GetKeyArray();
        //    for (int i = 0, count = keys.Length; i < count; i++)
        //    {
        //        KeyValuePair<uint, uint> kvp = keys[i].SplitToKeyValuePair<uint, uint>();
        //        AnimationData animationData = AnimationData.GetAnimationDataById(kvp.Value);
        //        if (animationData.closeup)
        //        {
        //            result = true;
        //            break;
        //        }
        //    }
        //    return result;
        //}
        #endregion

        public List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> GetTargets(uint id, SkillData skillData, bool isPlayer = true)
        {
            CharacterEntity character = null;
            if (isPlayer)
                character = PlayerController.instance[id];
            else
                character = EnemyController.instance[id];
            if (!character) return null;
            List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> tids = new List<Dictionary<uint, List<KeyValuePair<uint, uint>>>>();
            //List<float> timelinKeys = skillData.Timeline.GetKeys();
            MechanicsData mechanicsData = null;
            List<uint> mechanicsIdList = null;
            float timeline = 0f;
            //for (int i = 0, count = timelinKeys.Count; i < count; i++)
            Dictionary<float, List<uint>>.Enumerator e = skillData.timeline.GetEnumerator();
            //foreach (var kvp in skillData.timeline)
            while (e.MoveNext())
            {
                KeyValuePair<float, List<uint>> kvp = e.Current;
                //timeline = timelinKeys[i];
                timeline = kvp.Key;
                mechanicsIdList = skillData.timeline[timeline];
                Dictionary<uint, List<KeyValuePair<uint, uint>>> timeLineTids = new Dictionary<uint, List<KeyValuePair<uint, uint>>>();
                for (int j = 0, jCount = mechanicsIdList.Count; j < jCount; j++)
                {
                    mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsIdList[j]);
                    List<KeyValuePair<uint, uint>> mechanicsTids = null;
                    if (!FilterSameRangeAndTargetTids(tids, mechanicsData, out mechanicsTids))
                    {
                        mechanicsTids = GetTargets(character, mechanicsData, isPlayer);
                    }
                    timeLineTids.Add(mechanicsData.mechanicsId, mechanicsTids);
                }
                tids.Add(timeLineTids);
            }
            e.Dispose();
            return tids;
        }

        private bool FilterSameRangeAndTargetTids(List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> tids, MechanicsData mechanicsData, out List<KeyValuePair<uint, uint>> mechanicsTids)
        {
            bool result = false;
            mechanicsTids = null;
            for (int i = 0, count = tids.Count; i < count; i++)
            {
                Dictionary<uint, List<KeyValuePair<uint, uint>>> timeLineTids = tids[i];
                Dictionary<uint, List<KeyValuePair<uint, uint>>>.Enumerator e = timeLineTids.GetEnumerator();
                while (e.MoveNext())
                {
                    MechanicsData m = MechanicsData.GetMechanicsDataById(e.Current.Key);
                    if (m.rangeType == mechanicsData.rangeType && m.targetType == mechanicsData.targetType)
                    {
                        mechanicsTids = new List<KeyValuePair<uint, uint>>(e.Current.Value);
                        result = true;
                        break;
                    }
                }
                e.Dispose();
                if (result)
                    break;
            }
            return result;
        }

#if UNITY_EDITOR
        #region get target window
        public List<KeyValuePair<uint, uint>> GetTargets(uint positionId, TargetType targetType, RangeType rangeType, bool isPlayer)
        {
            return GetTargets(positionId, new MechanicsData()
            {
                targetType = targetType,
                rangeType = rangeType
            }, isPlayer);
        }

        private List<KeyValuePair<uint, uint>> GetTargets(uint positionId, MechanicsData mechanicsData, bool isPlayer)
        {
            List<KeyValuePair<uint, uint>> mechanicsTids = new List<KeyValuePair<uint, uint>>();
            if (mechanicsData.mechanicsType == MechanicsType.Reborn)//重生没有目标
                return mechanicsTids;
            List<CharacterEntity> targets = GetTargetList(mechanicsData.targetType, isPlayer);
            #region 目标范围
            switch (mechanicsData.rangeType)
            {
                case RangeType.CurrentSingle:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                    }
                    break;
                case RangeType.CurrentRow:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetTargetRowIds(target.positionId, targets);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.CurrentColumn:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetTargetColumIds(target.positionId, targets);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.All:
                case RangeType.AllAbsolutely:
                    {
                        List<uint> targetIds = GetTargetAll(targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.CurrentAndBehindFirst:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(target.characterInfo.instanceID, target.characterInfo.instanceID));
                            CharacterEntity behindOne = GetTargetBehindFirst(target, targets);
                            if (behindOne)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(behindOne.characterInfo.instanceID, behindOne.characterInfo.instanceID));
                        }
                    }
                    break;
                case RangeType.CurrentAndNearCross:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(target.characterInfo.instanceID, target.characterInfo.instanceID));
                            CharacterEntity upFirst = GetTargetUpFirst(target, targets);
                            if (upFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(upFirst.characterInfo.instanceID, upFirst.characterInfo.instanceID));
                            CharacterEntity downFirst = GetTargetDownFirst(target, targets);
                            if (downFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(downFirst.characterInfo.instanceID, downFirst.characterInfo.instanceID));
                            CharacterEntity forwardFirst = GetTargetForwardFirst(target, targets);
                            if (forwardFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(forwardFirst.characterInfo.instanceID, forwardFirst.characterInfo.instanceID));
                            CharacterEntity behindFirst = GetTargetBehindFirst(target, targets);
                            if (behindFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(behindFirst.characterInfo.instanceID, behindFirst.characterInfo.instanceID));
                        }
                    }
                    break;
                case RangeType.CurrentBehindFirst:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity behindFirst = GetTargetBehindFirst(target, targets);
                            if (behindFirst)
                                mechanicsTargetId = behindFirst.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentBehindSecond:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity behindSecond = GetTargetBehindSecond(target, targets);
                            if (behindSecond)
                                mechanicsTargetId = behindSecond.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentIntervalOne:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity upOrDown = GetTargetIntervalOne(target, targets);
                            if (upOrDown)
                                mechanicsTargetId = upOrDown.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentAndRandomTwo:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetRandomTargets(3, GetTargetAll(targets), target.characterInfo.instanceID);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.RandomN:
                    {
                        List<uint> targetIds = GetRandomTargetN(GetTargetAll(targets));
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Cross:
                    {
                        List<uint> targetIds = GetTargetCross(targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.FirstColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_1, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ThirdColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.FirstRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_1, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_2, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ThirdRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_3, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ExceptMidpoint:
                    {
                        List<uint> targetIds = GetTargetExceptMidpoint(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Midpoint:
                    {
                        List<uint> targetIds = GetTargetMidpoint(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.LeadingDiagonal:
                    {
                        List<uint> targetIds = GetTargetLeadingDiagonal(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondaryDiagonal:
                    {
                        List<uint> targetIds = GetTargetSecondaryDiagonal(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Weakness:
                    {
                        List<CharacterEntity> list = GetWeakness(targets, mechanicsData.targetType);
                        if (list.Count > 0)
                        {
                            uint targetId = list.First().characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                    }
                    break;
                case RangeType.LowestHP:
                    {
                        List<CharacterEntity> list = GetLowestHP(targets, mechanicsData.targetType);
                        if (list.Count > 0)
                        {
                            uint targetId = list.First().characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                    }
                    break;
                case RangeType.RandomSingle:
                    {
                        List<uint> targetIds = GetRandomTargetN(GetTargetAll(targets), 1);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.CurrentAndBehindTowColum:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            List<uint> targetIds = new List<uint>();

                            if (target.positionData.columnNum < 2)
                            {
                                targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets));
                            }
                            else if (target.positionData.columnNum < 3)
                            {
                                targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets));
                            }
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint Id = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(Id, Id));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.BehindTowColum:
                    {
                        List<uint> targetIds = new List<uint>();
                        targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets));
                        targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets));
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint Id = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(Id, Id));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
            }
            #endregion
            targets.Clear();
            targets = null;
            return mechanicsTids;
        }
        #endregion
#endif

        private List<KeyValuePair<uint, uint>> GetTargets(CharacterEntity character, MechanicsData mechanicsData, bool isPlayer)
        {
            uint positionId = character.positionId;
            List<KeyValuePair<uint, uint>> mechanicsTids = new List<KeyValuePair<uint, uint>>();
            if (mechanicsData.mechanicsType == MechanicsType.Reborn)//重生没有目标
                return mechanicsTids;
            List<CharacterEntity> targets = GetTargetList(mechanicsData.targetType, isPlayer);
            #region 目标范围
            switch (mechanicsData.rangeType)
            {
                case RangeType.CurrentSingle:
                    {
                        int t = 0;

                        Dictionary<int, int>.Enumerator e = character.characterInfo.passiveIdDic.GetEnumerator();
                        //foreach (var kvp in character.characterInfo.passiveIdDic)
                        while (e.MoveNext())
                        {
                            KeyValuePair<int, int> kvp = e.Current;
                            LuaInterface.LuaFunction func = Logic.Fight.Controller.PassiveSkillController.instance.GetPassiveSkillLuaFunction(Logic.Fight.Controller.PassiveSkillController.FIND_TARGET, kvp.Key);
                            if (func != null)
                            {
                                object[] rs = func.Call(kvp.Value);
                                int.TryParse(rs[0].ToString(), out t);
                            }
                        }
                        e.Dispose();
                        bool weak = t == 1;
                        if (!weak)
                        {
                            float weaknessRate = character.GetBuffsValue(BuffType.Weakness, 0f);
                            weak = RandomUtil.GetRandom(weaknessRate);
                        }
                        if (weak)
                        {
                            List<CharacterEntity> list = GetWeakness(targets, mechanicsData.targetType);
                            if (list.Count > 0)
                            {
                                uint targetId = list.First().characterInfo.instanceID;
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                        }
                        else
                        {
                            CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                            if (target)
                            {
                                uint targetId = target.characterInfo.instanceID;
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                        }
                    }
                    break;
                case RangeType.CurrentRow:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetTargetRowIds(target.positionId, targets);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.CurrentColumn:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetTargetColumIds(target.positionId, targets);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.All:
                case RangeType.AllAbsolutely:
                    {
                        List<uint> targetIds = GetTargetAll(targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.CurrentAndBehindFirst:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(target.characterInfo.instanceID, target.characterInfo.instanceID));
                            CharacterEntity behindOne = GetTargetBehindFirst(target, targets);
                            if (behindOne)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(behindOne.characterInfo.instanceID, behindOne.characterInfo.instanceID));
                        }
                    }
                    break;
                case RangeType.CurrentAndNearCross:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(target.characterInfo.instanceID, target.characterInfo.instanceID));
                            CharacterEntity upFirst = GetTargetUpFirst(target, targets);
                            if (upFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(upFirst.characterInfo.instanceID, upFirst.characterInfo.instanceID));
                            CharacterEntity downFirst = GetTargetDownFirst(target, targets);
                            if (downFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(downFirst.characterInfo.instanceID, downFirst.characterInfo.instanceID));
                            CharacterEntity forwardFirst = GetTargetForwardFirst(target, targets);
                            if (forwardFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(forwardFirst.characterInfo.instanceID, forwardFirst.characterInfo.instanceID));
                            CharacterEntity behindFirst = GetTargetBehindFirst(target, targets);
                            if (behindFirst)
                                mechanicsTids.Add(new KeyValuePair<uint, uint>(behindFirst.characterInfo.instanceID, behindFirst.characterInfo.instanceID));
                        }
                    }
                    break;
                case RangeType.CurrentBehindFirst:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity behindFirst = GetTargetBehindFirst(target, targets);
                            if (behindFirst)
                                mechanicsTargetId = behindFirst.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentBehindSecond:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity behindSecond = GetTargetBehindSecond(target, targets);
                            if (behindSecond)
                                mechanicsTargetId = behindSecond.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentIntervalOne:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            uint mechanicsTargetId = 0;
                            CharacterEntity upOrDown = GetTargetIntervalOne(target, targets);
                            if (upOrDown)
                                mechanicsTargetId = upOrDown.characterInfo.instanceID;
                            KeyValuePair<uint, uint> kvp = new KeyValuePair<uint, uint>(targetId, mechanicsTargetId);
                            mechanicsTids.Add(kvp);
                        }
                    }
                    break;
                case RangeType.CurrentAndRandomTwo:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            List<uint> targetIds = GetRandomTargets(3, GetTargetAll(targets), target.characterInfo.instanceID);
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint targetId = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.RandomN:
                    {
                        List<uint> targetIds = GetRandomTargetN(GetTargetAll(targets));
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Cross:
                    {
                        List<uint> targetIds = GetTargetCross(targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.FirstColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_1, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ThirdColum:
                    {
                        List<uint> targetIds = GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.FirstRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_1, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_2, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ThirdRow:
                    {
                        List<uint> targetIds = GetTargetRowIds((uint)FormationPosition.Player_Position_3, targets);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.ExceptMidpoint:
                    {
                        List<uint> targetIds = GetTargetExceptMidpoint(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Midpoint:
                    {
                        List<uint> targetIds = GetTargetMidpoint(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.LeadingDiagonal:
                    {
                        List<uint> targetIds = GetTargetLeadingDiagonal(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.SecondaryDiagonal:
                    {
                        List<uint> targetIds = GetTargetSecondaryDiagonal(targets, mechanicsData.targetType, isPlayer);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.Weakness:
                    {
                        List<CharacterEntity> list = GetWeakness(targets, mechanicsData.targetType);
                        if (list.Count > 0)
                        {
                            uint targetId = list.First().characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                    }
                    break;
                case RangeType.LowestHP:
                    {
                        List<CharacterEntity> list = GetLowestHP(targets, mechanicsData.targetType);
                        if (list.Count > 0)
                        {
                            uint targetId = list.First().characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                    }
                    break;
                case RangeType.RandomSingle:
                    {
                        List<uint> targetIds = GetRandomTargetN(GetTargetAll(targets), 1);
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint targetId = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
                case RangeType.CurrentAndBehindTowColum:
                    {
                        CharacterEntity target = GetTarget1_9(positionId, targets, mechanicsData, isPlayer);
                        if (target)
                        {
                            uint targetId = target.characterInfo.instanceID;
                            mechanicsTids.Add(new KeyValuePair<uint, uint>(targetId, targetId));
                            List<uint> targetIds = new List<uint>();

                            if (target.positionData.columnNum < 2)
                            {
                                targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets));
                            }
                            else if (target.positionData.columnNum < 3)
                            {
                                targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets));
                            }
                            List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                            for (int i = 0, count = targetIds.Count; i < count; i++)
                            {
                                uint Id = targetIds[i];
                                list.Add(new KeyValuePair<uint, uint>(Id, Id));
                            }
                            mechanicsTids.AddRange(list);
                        }
                    }
                    break;
                case RangeType.BehindTowColum:
                    {
                        List<uint> targetIds = new List<uint>();
                        targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_4, targets));
                        targetIds.AddRange(GetTargetColumIds((uint)FormationPosition.Player_Position_7, targets));
                        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                        for (int i = 0, count = targetIds.Count; i < count; i++)
                        {
                            uint Id = targetIds[i];
                            list.Add(new KeyValuePair<uint, uint>(Id, Id));
                        }
                        mechanicsTids.AddRange(list);
                    }
                    break;
            }
            #endregion
            targets.Clear();
            targets = null;
            return mechanicsTids;
        }

        #region find targets
        private List<CharacterEntity> GetTargetList(TargetType targetType, bool isPlayer)
        {
            List<CharacterEntity> result = null;
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                    {
                        List<HeroEntity> values = PlayerController.instance.heros;
                        result = new List<CharacterEntity>();
                        for (int i = 0, count = values.Count; i < count; i++)
                        {
                            result.Add(values[i]);
                        }
                    }
                    else
                    {
                        List<EnemyEntity> values = EnemyController.instance.enemies;
                        result = new List<CharacterEntity>();
                        for (int i = 0, count = values.Count; i < count; i++)
                        {
                            result.Add(values[i]);
                        }
                    }
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                    {
                        List<EnemyEntity> values = EnemyController.instance.enemies;
                        result = new List<CharacterEntity>();
                        for (int i = 0, count = values.Count; i < count; i++)
                        {
                            result.Add(values[i]);
                        }
                    }
                    else
                    {
                        List<HeroEntity> values = PlayerController.instance.heros;
                        result = new List<CharacterEntity>();
                        for (int i = 0, count = values.Count; i < count; i++)
                        {
                            result.Add(values[i]);
                        }
                    }
                    break;
            }
            result.Sort(VirtualServerControllerUtil.SortedByPosition);
            return result;
        }

        private CharacterEntity GetTargetByPositionId(List<CharacterEntity> list, uint positionId)
        {
            for (int i = 0, count = list.Count; i < count; i++)
            {
                CharacterEntity c = list[i];
                if (c.positionId == positionId)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTarget1_9(uint positionId, List<CharacterEntity> list, MechanicsData mechanicsData, bool isPlayer)
        {
            switch (mechanicsData.targetType)
            {
                case TargetType.Ally:
                    for (int i = 0, count = list.Count; i < count; i++)
                    {
                        CharacterEntity character = list[i];
                        if (character.positionId == positionId)
                            return character;
                    }
                    break;
                case TargetType.Enemy:
                    uint row = 0;
                    if (isPlayer)
                        row = positionId % 3;
                    else
                        row = (positionId % 100) % 3;
                    if (row == 0)
                        row = 3;
                    switch (row)
                    {
                        case 1:
                            {
                                for (uint i = 1; i <= 9; i += 3)
                                {
                                    //uint pid = isPlayer ? i + 100 : i;
                                    List<CharacterEntity> columns = GetTargetColumEntities(i, list);
                                    List<CharacterEntity> row1 = GetTargetRowEntities(1, columns);
                                    if (row1.Count > 0)
                                        return row1.First();
                                    List<CharacterEntity> row2 = GetTargetRowEntities(2, columns);
                                    if (row2.Count > 0)
                                        return row2.First();
                                    List<CharacterEntity> row3 = GetTargetRowEntities(3, columns);
                                    if (row3.Count > 0)
                                        return row3.First();
                                    columns.Clear();
                                    columns = null;
                                }
                            }
                            break;
                        case 2:
                            {
                                for (uint i = 1; i <= 9; i += 3)
                                {
                                    //uint pid = isPlayer ? i + 100 : i;
                                    List<CharacterEntity> columns = GetTargetColumEntities(i, list);
                                    List<CharacterEntity> row2 = GetTargetRowEntities(2, columns);
                                    if (row2.Count > 0)
                                        return row2.First();
                                    List<CharacterEntity> row1 = GetTargetRowEntities(1, columns);
                                    if (row1.Count > 0)
                                        return row1.First();
                                    List<CharacterEntity> row3 = GetTargetRowEntities(3, columns);
                                    if (row3.Count > 0)
                                        return row3.First();
                                    columns.Clear();
                                    columns = null;
                                }
                            }
                            break;
                        case 3:
                            {
                                for (uint i = 1; i <= 9; i += 3)
                                {
                                    //uint pid = isPlayer ? i + 100 : i;
                                    List<CharacterEntity> columns = GetTargetColumEntities(i, list);
                                    List<CharacterEntity> row3 = GetTargetRowEntities(3, columns);
                                    if (row3.Count > 0)
                                        return row3.First();
                                    List<CharacterEntity> row2 = GetTargetRowEntities(2, columns);
                                    if (row2.Count > 0)
                                        return row2.First();
                                    List<CharacterEntity> row1 = GetTargetRowEntities(1, columns);
                                    if (row1.Count > 0)
                                        return row1.First();
                                    columns.Clear();
                                    columns = null;
                                }
                            }
                            break;
                    }
                    break;
            }
            return null;
        }

        private List<uint> GetRandomTargetN(List<uint> list, int max = -1)
        {
            return VirtualServerControllerUtil.GetRandomTargetN(list, max);
            //int count = Random.Range(1, list.Count + 1);
            //if (count == list.Count)
            //    return list;
            //List<uint> tids = new List<uint>();
            //int random = 0;
            //while (true)
            //{
            //    random = Random.Range(0, list.Count);
            //    uint id = list[random];
            //    if (tids.Contains(id))
            //        continue;
            //    tids.Add(id);
            //    if (tids.Count == count)
            //        break;
            //}
            //return tids;
        }

        private List<uint> GetRandomTargets(int count, List<uint> list, uint ignoreId)
        {
            List<uint> tids = new List<uint>();
            if (list.Count <= count)
            {
                list.Sort(SortType.Asc);
                tids.AddRange(list);
            }
            else
            {
                int random = 0;
                tids.Add(ignoreId);
                while (true)
                {
                    random = Random.Range(0, list.Count);
                    uint id = list[random];
                    if (tids.Contains(id))
                        continue;
                    tids.Add(id);
                    if (tids.Count == count)
                        break;
                }
            }
            return tids;
        }

        private List<uint> GetTargetAll(List<CharacterEntity> list)
        {
            List<uint> result = new List<uint>();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                result.Add(list[i].characterInfo.instanceID);
            }
            return result;
        }

        private List<uint> GetTargetRowIds(uint positionId, List<CharacterEntity> list)
        {
            List<uint> result = new List<uint>();
            List<CharacterEntity> temps = GetTargetRowEntities(positionId, list);
            for (int i = 0, length = temps.Count; i < length; i++)
            {
                result.Add(temps[i].characterInfo.instanceID);
            }
            return result;
        }

        private List<CharacterEntity> GetTargetRowEntities(uint positionId, List<CharacterEntity> list)
        {
            List<CharacterEntity> result = new List<CharacterEntity>();
            Position.Model.PositionData positionData = Position.Model.PositionData.GetPostionDataById(positionId);
            for (int i = 0, count = list.Count; i < count; i++)
            {
                CharacterEntity c = list[i];
                if (c.positionData.rowNum == positionData.rowNum)
                    result.Add(c);
            }
            return result;
        }

        private List<uint> GetTargetColumIds(uint positionId, List<CharacterEntity> list)
        {
            List<uint> result = new List<uint>();
            List<CharacterEntity> temps = GetTargetColumEntities(positionId, list);
            for (int i = 0, length = temps.Count; i < length; i++)
            {
                result.Add(temps[i].characterInfo.instanceID);
            }
            return result;
        }

        private List<CharacterEntity> GetTargetColumEntities(uint positionId, List<CharacterEntity> list)
        {
            List<CharacterEntity> result = new List<CharacterEntity>();
            Position.Model.PositionData positionData = Position.Model.PositionData.GetPostionDataById(positionId);
            for (int i = 0, count = list.Count; i < count; i++)
            {
                CharacterEntity c = list[i];
                if (c.positionData.columnNum % 3 == positionData.columnNum % 3)
                    result.Add(c);
            }
            return result;
        }

        private CharacterEntity GetTargetForwardFirst(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> currentRow = GetTargetRowEntities(target.positionId, list);
            for (int i = 0, count = currentRow.Count; i < count; i++)
            {
                CharacterEntity c = currentRow[i];
                if (target.positionId - c.positionId == 3)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTargetBehindFirst(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> currentRow = GetTargetRowEntities(target.positionId, list);
            for (int i = 0, count = currentRow.Count; i < count; i++)
            {
                CharacterEntity c = currentRow[i];
                if (c.positionId - target.positionId == 3)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTargetBehindSecond(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> currentRow = GetTargetRowEntities(target.positionId, list);
            for (int i = 0, count = currentRow.Count; i < count; i++)
            {
                CharacterEntity c = currentRow[i];
                if (c.positionId - target.positionId == 6)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTargetIntervalOne(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> targets = GetTargetColumEntities(target.positionId, list);
            for (int i = 0, count = targets.Count; i < count; i++)
            {
                CharacterEntity c = targets[i];
                if (c.positionId - target.positionId == 2 || target.positionId - c.positionId == 2)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTargetUpFirst(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> currentColum = GetTargetColumEntities(target.positionId, list);
            for (int i = 0, count = currentColum.Count; i < count; i++)
            {
                CharacterEntity c = currentColum[i];
                if (c == target)
                    continue;
                if (target.positionId - c.positionId == 1)
                    return c;
            }
            return null;
        }

        private CharacterEntity GetTargetDownFirst(CharacterEntity target, List<CharacterEntity> list)
        {
            if (target == null) return null;
            List<CharacterEntity> currentColum = GetTargetColumEntities(target.positionId, list);
            for (int i = 0, count = currentColum.Count; i < count; i++)
            {
                CharacterEntity c = currentColum[i];
                if (c == target)
                    continue;
                if (c.positionId - target.positionId == 1)
                    return c;
            }
            return null;
        }

        private List<uint> GetTargetCross(List<CharacterEntity> list)
        {
            List<uint> result = new List<uint>();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                CharacterEntity c = list[i];
                uint targetPid = c.positionId % 100;
                if (targetPid == (uint)FormationPosition.Player_Position_2 || targetPid == (uint)FormationPosition.Player_Position_4 ||
                    targetPid == (uint)FormationPosition.Player_Position_5 || targetPid == (uint)FormationPosition.Player_Position_6 ||
                    targetPid == (uint)FormationPosition.Player_Position_8)
                    result.Add(c.characterInfo.instanceID);
            }
            return result;
        }

        private List<uint> GetTargetExceptMidpoint(List<CharacterEntity> list, TargetType targetType, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            result.AddRange(GetTargetAll(list));
            List<uint> midPointTarget = GetTargetMidpoint(list, targetType, isPlayer);
            for (int i = 0, count = midPointTarget.Count; i < count; i++)
            {
                result.Remove(midPointTarget[i]);
            }
            return result;
        }

        private List<uint> GetTargetMidpoint(List<CharacterEntity> list, TargetType targetType, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            CharacterEntity target = null;
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                        target = GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5);
                    else
                        target = GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5);
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                        target = GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5);
                    else
                        target = GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5);
                    break;
            }
            if (target)
                result.Add(target.characterInfo.instanceID);
            return result;
        }

        private List<uint> GetTargetLeadingDiagonal(List<CharacterEntity> list, TargetType targetType, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            List<CharacterEntity> targets = new List<CharacterEntity>();
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_1));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_9));
                    }
                    else
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_1));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_9));
                    }
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_1));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_9));
                    }
                    else
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_1));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_9));
                    }
                    break;
            }
            for (int i = 0, count = targets.Count; i < count; i++)
            {
                CharacterEntity target = targets[i];
                if (target)
                    result.Add(target.characterInfo.instanceID);
            }
            return result;
        }

        private List<uint> GetTargetSecondaryDiagonal(List<CharacterEntity> list, TargetType targetType, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            List<CharacterEntity> targets = new List<CharacterEntity>();
            switch (targetType)
            {
                case TargetType.Ally:
                    if (isPlayer)
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_3));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_7));
                    }
                    else
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_3));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_7));
                    }
                    break;
                case TargetType.Enemy:
                    if (isPlayer)
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_3));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Enemy_Position_7));
                    }
                    else
                    {
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_3));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_5));
                        targets.Add(GetTargetByPositionId(list, (uint)FormationPosition.Player_Position_7));
                    }
                    break;
            }
            for (int i = 0, count = targets.Count; i < count; i++)
            {
                CharacterEntity target = targets[i];
                if (target)
                    result.Add(target.characterInfo.instanceID);
            }
            return result;
        }

        private List<CharacterEntity> GetWeakness(List<CharacterEntity> list, TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.Ally:
                    list.Sort(VirtualServerControllerUtil.SortedByHPAndDefense);
                    break;
                case TargetType.Enemy:
                    list.Sort(VirtualServerControllerUtil.SortedByHPAndDefense);
                    break;
            }
            return list;
        }

        private List<CharacterEntity> GetLowestHP(List<CharacterEntity> list, TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.Ally:
                    list.Sort(VirtualServerControllerUtil.SortedByHPPercent);
                    break;
                case TargetType.Enemy:
                    list.Sort(VirtualServerControllerUtil.SortedByHPPercent);
                    break;
            }
            return list;
        }
        #endregion
        /*
        //#if UNITY_EDITOR
        #region fight logic data
        //#if log

        private bool _showLog = false;

        //void OnEnable()
        //{
        //    int showLog = PlayerPrefs.GetInt("showLog", 0);
        //    _showLog = showLog == 1;
        //}

        void OnGUI()
        {
            if (GUI.Button(new Rect(300, 0, 60, 20), "show"))
            {
                _showLog = !_showLog;
            }
            //if (GUI.Button(new Rect(750, 0, 80, 30), "pause"))
            //{
            //    bool pause = Common.GameTime.Controller.TimeController.instance.playerPause;
            //    Common.GameTime.Controller.TimeController.instance.playerPause = !pause;
            //}
            if (_showLog)
            {
                int j = 0, heigh = 20;
                GUI.Label(new Rect(600, 0, 200, heigh), "英雄列表:");
                j++;
                foreach (var kvp in PlayerController.instance.heroDic)
                {
                    GUI.Label(new Rect(600, heigh * j, 400, heigh), "ID:" + kvp.Key.ToString() + " " + kvp.Value.characterName + " " + kvp.Value.status.ToString());
                    j++;
                }
                GUI.Label(new Rect(600, heigh * j, 200, heigh), "敌方列表:");
                j++;
                foreach (var kvp in EnemyController.instance.enemyDic)
                {
                    GUI.Label(new Rect(600, heigh * j, 400, heigh), "ID:" + kvp.Key.ToString() + " " + kvp.Value.characterName + " " + kvp.Value.status.ToString());
                    j++;
                }
                GUI.Label(new Rect(600, heigh * j, 200, heigh), "等待完成技能列表:");
                j++;
                GUI.Label(new Rect(600, heigh * j, 200, heigh), "当前时间:" + Time.time.ToString());
                j++;
                foreach (var kvp in VirtualServer.instance.skillWaitFinishDic)
                {
                    GUI.Label(new Rect(600, heigh * j, 400, heigh), "ID:" + kvp.Key.ToString() + " finishTime:" + kvp.Value.ToString());
                    j++;
                }
                int i = 1;
                GUI.Label(new Rect(400, 0, 200, heigh), "预约技能列表:");
                i++;
                foreach (var kvp in VirtualServer.instance.skillWaitingDic)
                {
                    GUI.Label(new Rect(400, heigh * i, 200, heigh), "技能ID:" + kvp.Key.ToString());
                    i++;
                    //GUI.Label(new Rect(400, heigh * i, 200, heigh), "玩家ID:" + kvp.Value.ToString());
                    //i++;
                }
                GUI.Label(new Rect(400, heigh * i, 200, 50), "正在释放技能列表:");
                i++;
                foreach (var kvp in VirtualServer.instance.skillingDic)
                {
                    GUI.Label(new Rect(400, heigh * i, 200, heigh), "技能ID:" + kvp.Key.ToString());
                    i++;
                    //GUI.Label(new Rect(400, heigh * i, 200, heigh), "玩家ID:" + kvp.Value.ToString());
                    //i++;
                }
                GUI.Label(new Rect(400, heigh * i, 200, heigh), "技能释放锁定角色列表:");
                i++;
                foreach (var kvp in VirtualServer.instance.beLcokedDic)
                {
                    GUI.Label(new Rect(400, heigh * i, 200, heigh), "技能ID:" + kvp.Key.ToString());
                    i++;
                    foreach (var id in kvp.Value)
                    {
                        GUI.Label(new Rect(400, heigh * i, 200, heigh), "玩家ID:" + id.Key.ToString());
                        i++;
                    }
                }
            }
        }
        //#endif
        #endregion
        //#endif
         */
    }
}