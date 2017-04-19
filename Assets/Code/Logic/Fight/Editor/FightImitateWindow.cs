using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Net.Controller;
using Logic.Fight.Controller;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Player.Model;
using System.Collections.Generic;
using Logic.Judge.Controller;
using Logic.Character.Controller;
namespace Logic.Fight.Editor
{
    public class FightImitateWindow : UnityEditor.EditorWindow
    {
        [MenuItem("CEngine/战斗模拟器 %q", false, 100)]
        public static void OpenFightEditorWindow()
        {
            EditorWindow.GetWindow<FightImitateWindow>();
            FightController.imitate = true;
            LoadHeroAndPlayer();
            Init();
        }

        int heroIndex = 0, playerIndex = 0;
        static List<HeroData> heros;
        static List<PlayerData> players;
        static string[] heroNames, playerNames;
        //float timeScale = 1f;
        int instanceId = 0, enemeyInstanceId = 1000;
        static bool isPlayerPlayer = true, isPlayerHero = true;
        FormationPosition playerPosition = FormationPosition.Player_Position_1, heroPosition = FormationPosition.Player_Position_1;
        List<bool> foldouts = new List<bool>();
        static bool isDoubleSpeed = false;
        bool foldout = true, foldout1 = true;
        bool isBoss = false;
        Vector2 scrollPos1 = Vector2.zero;
        int enemyWave = 1;
        int fightCount
        {
            get
            {
                if (!Application.isPlaying)
                    return 1;
                return FightController.instance.fightCount;
            }
            set
            {
                if (Application.isPlaying)
                    FightController.instance.fightCount = value;
            }
        }

        static int fightFinishCount
        {
            get
            {
                if (Application.isPlaying)
                    return FightController.instance.fightResults.Count;
                return 0;
            }
        }

        static void LoadHeroAndPlayer()
        {
            heros = HeroData.GetHeroDatas().GetValues();
            players = PlayerData.GetPlayerDatas().GetValues();
            heroNames = new string[heros.Count];
            playerNames = new string[players.Count];
            for (int i = 0, length = heros.Count; i < length; i++)
            {
                heroNames[i] = heros[i].defaultModelName;
            }
            for (int i = 0, length = players.Count; i < length; i++)
            {
                playerNames[i] = players[i].model;
            }
        }
        void OnLostFocus()
        {
            if (!Application.isPlaying)
                this.Close();
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
                this.Close();
            FightController.imitate = true;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("战斗模拟器");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清除", GUILayout.Width(200)))
            {
                Init();
            }
            if (GUILayout.Button("暂停战斗", GUILayout.Width(200)))
            {
                if (VirtualServerController.instance)
                {
                    VirtualServerController.instance.fightEidtor = true;
                    Fight.Controller.FightController.instance.autoFight = false;
                }
            }
            if (GUILayout.Button("恢复战斗", GUILayout.Width(200)))
            {
                if (VirtualServerController.instance)
                {
                    VirtualServerController.instance.fightEidtor = false;
                }
            }
            fightCount = EditorGUILayout.IntField("战斗次数：", fightCount, GUILayout.Width(200));
            if (GUILayout.Button("开始战斗", GUILayout.Width(200)))
            {
                if (FightController.instance.heroInfoDic.Count == 0 && FightController.instance.playerInfoDic.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示", "请选择我方至少一个英雄", "确定");
                    //Debugger.LogError("请选择我方至少一个英雄!");
                    return;
                }
                if (FightController.instance.enemeyHeroInfoDic.Count == 0 && FightController.instance.enemeyPlayerInfoDic.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示", "请选择敌方至少一个英雄", "确定");
                    //Debugger.LogError("请选择敌方至少一个英雄!");
                    return;
                }
                if (FightController.instance.fightStatus == FightStatus.GameOver)
                {
                    FightController.instance.fightType = FightType.Imitate;
                    FightController.instance.isDoubleSpeed = isDoubleSpeed;
                    FightController.instance.enemyWave = enemyWave;
                    FightController.instance.ClaerImitateStatisticData();
                    JudgeController.instance.ClaerImitateStatisticData();
                    FightController.instance.PreReadyFight();
                    Fight.Controller.FightController.instance.autoFight = true;
                }
            }
            isDoubleSpeed = EditorGUILayout.Toggle("是否两倍速：", isDoubleSpeed, GUILayout.Width(200));
            enemyWave = EditorGUILayout.IntField("怪物波数：", enemyWave, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("添加主角(我方或敌方)：", GUILayout.Width(200));
            playerIndex = EditorGUILayout.Popup("主角列表：", playerIndex, playerNames, GUILayout.Width(300));
            isPlayerPlayer = EditorGUILayout.Toggle("我方：", isPlayerPlayer, GUILayout.Width(200));
            playerPosition = (FormationPosition)EditorGUILayout.EnumPopup("角色站位：", playerPosition, GUILayout.Width(300));
            if (isPlayerPlayer)
            {
                if ((int)playerPosition > (int)FormationPosition.Player_Position_9)
                    playerPosition = FormationPosition.Player_Position_1;
                if (playerPosition == FormationPosition.Invalid_Position)
                    playerPosition = FormationPosition.Player_Position_1;
            }
            else
            {
                if ((int)playerPosition < (int)FormationPosition.Enemy_Position_1)
                    playerPosition = FormationPosition.Enemy_Position_1;
            }
            if (GUILayout.Button("添加", GUILayout.Width(100)))
            {
                PlayerData playerData = players[playerIndex];
                if (isPlayerPlayer)
                {
                    if (FightController.instance.playerInfoDic.Count > 0)
                    {
                        EditorUtility.DisplayDialog("提示", "我方只能上场一个主角", "确定");
                        //Debugger.LogError(string.Format("我方只能上场一个主角!"));
                        return;
                    }
                    instanceId++;
                    PlayerInfo playerInfo = new PlayerInfo((uint)instanceId, playerData.Id, 0, 0, 0, 0, string.Empty);
                    playerInfo.level = playerInfo.MaxLevel;
                    if (FightController.instance.playerInfoDic.ContainsKey(playerPosition) || FightController.instance.heroInfoDic.ContainsKey(playerPosition))
                        EditorUtility.DisplayDialog("提示", string.Format("位置：{0},已被使用,请重新选择!", playerPosition.ToString()), "确定");
                    //Debugger.LogError(string.Format("位置：{0},已被使用,请重新选择!", playerPosition.ToString()));
                    else
                    {
                        FightController.instance.playerInfoDic.Add(playerPosition, playerInfo);
                        int temp = (int)(playerPosition);
                        temp++;
                        playerPosition = (FormationPosition)temp;
                        heroPosition = playerPosition;
                    }
                }
                else
                {
                    if (FightController.instance.enemeyPlayerInfoDic.Count > 0)
                    {
                        EditorUtility.DisplayDialog("提示", "敌方只能上场一个主角", "确定");
                        //Debugger.LogError(string.Format("敌方只能上场一个主角!"));
                        return;
                    }
                    enemeyInstanceId++;
                    PlayerInfo playerInfo = new PlayerInfo((uint)enemeyInstanceId, playerData.Id, 0, 0, 0, 0, string.Empty);
                    playerInfo.level = playerInfo.MaxLevel;
                    if (FightController.instance.enemeyPlayerInfoDic.ContainsKey(playerPosition) || FightController.instance.enemeyHeroInfoDic.ContainsKey(playerPosition))
                        EditorUtility.DisplayDialog("提示", string.Format("位置：{0},已被使用,请重新选择!", playerPosition.ToString()), "确定");
                    //Debugger.LogError(string.Format("位置：{0},已被使用,请重新选择!", playerPosition.ToString()));
                    else
                    {
                        FightController.instance.enemeyPlayerInfoDic.Add(playerPosition, playerInfo);
                        int temp = (int)(playerPosition);
                        temp++;
                        playerPosition = (FormationPosition)temp;
                        heroPosition = playerPosition;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("添加英雄(我方或敌方)", GUILayout.Width(200));
            heroIndex = EditorGUILayout.Popup("英雄列表", heroIndex, heroNames, GUILayout.Width(300));
            isPlayerHero = EditorGUILayout.Toggle("我方：", isPlayerHero, GUILayout.Width(200));
            heroPosition = (FormationPosition)EditorGUILayout.EnumPopup("角色站位：", heroPosition, GUILayout.Width(300));
            if (!isPlayerHero)
            {
                isBoss = EditorGUILayout.Toggle("是Boss：", isBoss, GUILayout.Width(200));
            }
            if (isPlayerHero)
            {
                if ((int)heroPosition > (int)FormationPosition.Player_Position_9)
                    heroPosition = FormationPosition.Player_Position_1;
                if (heroPosition == FormationPosition.Invalid_Position)
                    heroPosition = FormationPosition.Player_Position_1;

            }
            else
            {
                if ((int)heroPosition < (int)FormationPosition.Enemy_Position_1)
                    heroPosition = FormationPosition.Enemy_Position_1;
            }
            if (GUILayout.Button("添加", GUILayout.Width(100)))
            {
                HeroData heroData = heros[heroIndex];
                if (isPlayerHero)
                {
                    instanceId++;
                    HeroInfo heroInfo = new HeroInfo((uint)instanceId, heroData.id, 0, 20, 6, 80);
                    foreach (var kvp in FightController.instance.heroInfoDic)
                    {
                        if (kvp.Value.heroData.id == heroData.id)
                        {
                            EditorUtility.DisplayDialog("提示", string.Format("{0}已经上场,请重新选择！", heroData.defaultModelName), "确定");
                            //Debugger.LogError(string.Format("{0}已经上场,请重新选择！", heroData.defaultModelName));
                            return;
                        }
                    }
                    if (FightController.instance.heroInfoDic.ContainsKey(heroPosition) || FightController.instance.playerInfoDic.ContainsKey(heroPosition))
                        EditorUtility.DisplayDialog("提示", string.Format("位置：{0},已被使用,请重新选择!", heroPosition.ToString()), "确定");
                    //Debugger.LogError(string.Format("位置：{0},已被使用,请重新选择!", heroPosition.ToString()));
                    else
                    {
                        FightController.instance.heroInfoDic.Add(heroPosition, heroInfo);
                        int temp = (int)(heroPosition);
                        temp++;
                        heroPosition = (FormationPosition)temp;
                        playerPosition = heroPosition;
                        heroIndex++;
                    }
                }
                else
                {
                    enemeyInstanceId++;
                    HeroInfo heroInfo = new HeroInfo((uint)enemeyInstanceId, heroData.id, 0, 20, 6, 80);
                    if (FightController.instance.enemeyHeroInfoDic.ContainsKey(heroPosition) || FightController.instance.enemeyPlayerInfoDic.ContainsKey(heroPosition))
                        EditorUtility.DisplayDialog("提示", string.Format("位置：{0},已被使用,请重新选择!", heroPosition.ToString()), "确定");
                    //Debugger.LogError(string.Format("位置：{0},已被使用,请重新选择!", heroPosition.ToString()));
                    else
                    {
                        FightController.instance.enemeyHeroInfoDic.Add(heroPosition, heroInfo);
                        if (isBoss)
                        {
                            if (!FightController.instance.enemeyBosses.Contains((uint)enemeyInstanceId))
                                FightController.instance.enemeyBosses.Add((uint)enemeyInstanceId);
                        }
                        int temp = (int)(heroPosition);
                        temp++;
                        heroPosition = (FormationPosition)temp;
                        playerPosition = heroPosition;
                        heroIndex++;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前我方列表：");
            EditorGUILayout.EndHorizontal();
            List<FormationPosition> positions = FightController.instance.playerInfoDic.GetKeys();
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                FormationPosition position = positions[i];
                PlayerInfo playerInfo = FightController.instance.playerInfoDic[position];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("主角：" + playerInfo.playerData.model, GUILayout.Width(300));
                EditorGUILayout.LabelField("位置：" + position.ToString(), GUILayout.Width(300));
                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    FightController.instance.playerInfoDic.Remove(position);
                    playerPosition = position;
                }
                EditorGUILayout.EndHorizontal();
            }
            positions = FightController.instance.heroInfoDic.GetKeys();
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                FormationPosition position = positions[i];
                HeroInfo heroInfo = FightController.instance.heroInfoDic[position];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("英雄：" + heroInfo.heroData.defaultModelName, GUILayout.Width(300));
                EditorGUILayout.LabelField("位置：" + position.ToString(), GUILayout.Width(300));
                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    FightController.instance.heroInfoDic.Remove(position);
                    heroPosition = position;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前敌方列表：");
            EditorGUILayout.EndHorizontal();
            positions = FightController.instance.enemeyPlayerInfoDic.GetKeys();
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                FormationPosition position = positions[i];
                PlayerInfo playerInfo = FightController.instance.enemeyPlayerInfoDic[position];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("主角：" + playerInfo.playerData.model, GUILayout.Width(300));
                EditorGUILayout.LabelField("位置：" + position.ToString(), GUILayout.Width(300));
                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    FightController.instance.enemeyPlayerInfoDic.Remove(position);
                    playerPosition = position;
                }
                EditorGUILayout.EndHorizontal();
            }
            positions = FightController.instance.enemeyHeroInfoDic.GetKeys();
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                FormationPosition position = positions[i];
                HeroInfo heroInfo = FightController.instance.enemeyHeroInfoDic[position];
                EditorGUILayout.BeginHorizontal();
                if (FightController.instance.enemeyBosses.Contains(heroInfo.instanceID))
                    EditorGUILayout.LabelField("英雄：" + heroInfo.heroData.defaultModelName + "(Boss)", GUILayout.Width(300));
                else
                    EditorGUILayout.LabelField("英雄：" + heroInfo.heroData.defaultModelName, GUILayout.Width(300));
                EditorGUILayout.LabelField("位置：" + position.ToString(), GUILayout.Width(300));

                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    FightController.instance.enemeyHeroInfoDic.Remove(position);
                    if (FightController.instance.enemeyBosses.Contains(heroInfo.instanceID))
                        FightController.instance.enemeyBosses.Remove(heroInfo.instanceID);
                    heroPosition = position;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("已经完成{0}场战斗", fightFinishCount));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1, false, false);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("战斗数据统计：");
            EditorGUILayout.EndHorizontal();
            #region average data
            if (fightFinishCount > 0)
            {
                foldout1 = EditorGUILayout.Foldout(foldout1, "展开平均统计列表");
                if (foldout1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    foreach (var kvp in FightController.instance.playerInfoDic)
                    {
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中普攻平均回合数：" + GetHitAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中技能平均回合数：" + GetSkillAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中普攻伤害平均数：" + GetHitAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中技能伤害平均数：" + GetSkillAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                    }
                    foreach (var kvp in FightController.instance.heroInfoDic)
                    {
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中普攻平均回合数：" + GetHitAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中技能平均回合数：" + GetSkillAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中普攻伤害平均数：" + GetHitAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中技能伤害平均数：" + GetSkillAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    foreach (var kvp in FightController.instance.enemeyPlayerInfoDic)
                    {
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中普攻平均回合数：" + GetHitAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中技能平均回合数：" + GetSkillAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中普攻伤害平均数：" + GetHitAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在" + fightFinishCount + "场战斗中技能伤害平均数：" + GetSkillAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                    }
                    foreach (var kvp in FightController.instance.enemeyHeroInfoDic)
                    {
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中普攻平均回合数：" + GetHitAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中技能平均回合数：" + GetSkillAverageCounts(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中普攻伤害平均数：" + GetHitAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在" + fightFinishCount + "场战斗中技能伤害平均数：" + GetSkillAverageDamages(kvp.Value.instanceID).ToString(), GUILayout.Width(500));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("我方阵容：");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均普攻回合数：" + GetHitAverageCounts().ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均技能总回合数：" + GetSkillAverageCounts().ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均普攻总伤害：" + GetHitAverageDamages().ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均技能总伤害：" + GetSkillAverageDamages().ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "战斗中普攻伤害占总伤害的百分比：" + GetHitDamageRateOfTotalDamage().ToString() + "%", GUILayout.Width(800));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("敌方阵容：");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均普攻总回合数：" + GetHitAverageCounts(false).ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均技能总回合数：" + GetSkillAverageCounts(false).ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均普攻总伤害：" + GetHitAverageDamages(false).ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中平均技能总伤害：" + GetSkillAverageDamages(false).ToString(), GUILayout.Width(800));
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中普攻伤害占总伤害的百分比：" + GetHitDamageRateOfTotalDamage(false).ToString() + "%", GUILayout.Width(800));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("在" + fightFinishCount + "场战斗中我方的胜率：" + GetWinRate().ToString() + "%", GUILayout.Width(800));
                    EditorGUILayout.EndHorizontal();
                }
            }
            #endregion
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("详细数据：");
            EditorGUILayout.EndHorizontal();
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            for (int i = 0, count = fightFinishCount; i < count; i++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("第{0}场战斗数据：", i + 1));
                EditorGUILayout.LabelField(string.Format("战斗耗时 {0} 秒", FightController.instance.fightCostTimes[i].ToString()));
                EditorGUILayout.EndHorizontal();
                if (foldouts.Count <= i)
                    foldouts.Add(true);
                foldout = foldouts[i];
                foldout = EditorGUILayout.Foldout(foldout, "展开");
                foldouts[i] = foldout;
                if (foldout)
                {
                    #region detail data
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    foreach (var kvp in FightController.instance.playerInfoDic)
                    {
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在本场战斗中普攻回合数：" + GetHitCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在本场战斗中技能回合数：" + GetSkillCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在本场战斗中普攻总伤害：" + GetHitDamages(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方主角：" + kvp.Value.playerData.model + "在本场战斗中技能总伤害：" + GetSkillDamages(i, kvp.Value.instanceID).ToString());
                    }
                    foreach (var kvp in FightController.instance.heroInfoDic)
                    {
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中普攻回合数：" + GetHitCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中技能回合数：" + GetSkillCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中普攻总伤害：" + GetHitDamages(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("我方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中技能总伤害：" + GetSkillDamages(i, kvp.Value.instanceID).ToString());
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    foreach (var kvp in FightController.instance.enemeyPlayerInfoDic)
                    {
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在本场战斗中普攻回合数：" + GetHitCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在本场战斗中技能回合数：" + GetSkillCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在本场战斗中普攻总伤害：" + GetHitDamages(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方主角：" + kvp.Value.playerData.model + "在本场战斗中技能总伤害：" + GetSkillDamages(i, kvp.Value.instanceID).ToString());
                    }
                    foreach (var kvp in FightController.instance.enemeyHeroInfoDic)
                    {
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中普攻回合数：" + GetHitCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中技能回合数：" + GetSkillCounts(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中普攻总伤害：" + GetHitDamages(i, kvp.Value.instanceID).ToString());
                        EditorGUILayout.LabelField("敌方英雄：" + kvp.Value.heroData.defaultModelName + "在本场战斗中技能总伤害：" + GetSkillDamages(i, kvp.Value.instanceID).ToString());
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("我方阵容：");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("在本场战斗中普攻总回合数：" + GetAllHitCounts(i).ToString());
                    EditorGUILayout.LabelField("在本场战斗中技能总回合数：" + GetAllSkillCounts(i).ToString());
                    EditorGUILayout.LabelField("在本场战斗中普攻总伤害：" + GetAllHitDamages(i).ToString());
                    EditorGUILayout.LabelField("在本场战斗中技能总伤害：", GetAllSkillDamages(i).ToString());
                    EditorGUILayout.LabelField("在本场战斗中普攻伤害占总伤害的百分比：" + GetHitDamageRateOfTotalDamage(i).ToString() + "%");
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("敌方阵容：");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("在本场战斗中普攻总回合数：" + GetAllHitCounts(i, false).ToString());
                    EditorGUILayout.LabelField("在本场战斗中技能总回合数：" + GetAllSkillCounts(i, false).ToString());
                    EditorGUILayout.LabelField("在本场战斗中普攻总伤害：" + GetAllHitDamages(i, false).ToString());
                    EditorGUILayout.LabelField("在本场战斗中技能总伤害：" + GetAllSkillDamages(i, false).ToString());
                    EditorGUILayout.LabelField("在本场战斗中普攻伤害占总伤害的百分比：" + GetHitDamageRateOfTotalDamage(i, false).ToString() + "%");
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    #endregion
                }
            }
            //EditorGUILayout.EndScrollView();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        #region data statistics
        private static uint GetHitCounts(int fightIndex, uint instanceId)
        {
            uint result = 0;
            Dictionary<string, uint> hitCountDic = FightController.instance.hitCounts[fightIndex];
            foreach (var kvp in hitCountDic)
            {
                if (kvp.Key.StartsWith(instanceId + "_"))
                {
                    result = kvp.Value;
                    break;
                }
            }
            return result;
        }

        private static uint GetSkillCounts(int fightIndex, uint instanceId)
        {
            uint result = 0;
            Dictionary<string, uint> skillCountDic = FightController.instance.skillCounts[fightIndex];
            foreach (var kvp in skillCountDic)
            {
                if (kvp.Key.StartsWith(instanceId + "_"))
                {
                    result = kvp.Value;
                    break;
                }
            }
            return result;
        }

        private static uint GetHitDamages(int fightIndex, uint instanceId)
        {
            uint result = 0;
            Dictionary<string, uint> hitDamageDic = JudgeController.instance.hitDamages[fightIndex];
            foreach (var kvp in hitDamageDic)
            {
                if (kvp.Key.StartsWith(instanceId + "_"))
                {
                    result = kvp.Value;
                    break;
                }
            }
            return result;
        }

        private static uint GetSkillDamages(int fightIndex, uint instanceId)
        {
            uint result = 0;
            Dictionary<string, uint> skillDamageDic = JudgeController.instance.skillDamages[fightIndex];
            foreach (var kvp in skillDamageDic)
            {
                if (kvp.Key.StartsWith(instanceId + "_"))
                {
                    result = kvp.Value;
                    break;
                }
            }
            return result;
        }

        private static uint GetHitAverageCounts(uint instanceId)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = 0;
            int i = 0;
            foreach (var hitCountDic in FightController.instance.hitCounts)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in hitCountDic)
                    {
                        if (kvp.Key.StartsWith(instanceId + "_"))
                        {
                            result += kvp.Value;
                            break;
                        }
                    }
                }
                i++;
            }
            return (uint)(result / fightFinishCount);
        }

        private static uint GetSkillAverageCounts(uint instanceId)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = 0;
            int i = 0;
            foreach (var skillCountDic in FightController.instance.skillCounts)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in skillCountDic)
                    {
                        if (kvp.Key.StartsWith(instanceId + "_"))
                        {
                            result += kvp.Value;
                            break;
                        }
                    }
                }
                i++;
            }
            return (uint)(result / fightFinishCount);
        }

        private static uint GetHitAverageDamages(uint instanceId)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = 0;
            int i = 0;
            foreach (var hitDamageDic in JudgeController.instance.hitDamages)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in hitDamageDic)
                    {
                        if (kvp.Key.StartsWith(instanceId + "_"))
                        {
                            result += kvp.Value;
                            break;
                        }
                    }
                }
                i++;
            }
            return (uint)(result / fightFinishCount);
        }

        private static uint GetSkillAverageDamages(uint instanceId)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = 0;
            int i = 0;
            foreach (var skillDamageDic in JudgeController.instance.skillDamages)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in skillDamageDic)
                    {
                        if (kvp.Key.StartsWith(instanceId + "_"))
                        {
                            result += kvp.Value;
                            break;
                        }
                    }
                }
                i++;
            }
            return (uint)(result / fightFinishCount);
        }

        private static uint GetHitAverageCounts(bool isPlayer = true)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = GetAllHitCounts(isPlayer);
            return (uint)(result / fightFinishCount);
        }

        private static uint GetSkillAverageCounts(bool isPlayer = true)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = GetAllSkillCounts(isPlayer);
            return (uint)(result / fightFinishCount);
        }

        private static uint GetHitAverageDamages(bool isPlayer = true)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = GetAllHitDamages(isPlayer);
            return (uint)(result / fightFinishCount);
        }

        private static uint GetSkillAverageDamages(bool isPlayer = true)
        {
            if (fightFinishCount <= 0) return 0;
            uint result = GetAllSkillDamages(isPlayer);
            return (uint)(result / fightFinishCount);
        }

        private static uint GetAllHitCounts(int fightIndex, bool isPlayer = true)
        {
            uint result = 0;
            Dictionary<string, uint> hitCountDic = FightController.instance.hitCounts[fightIndex];
            foreach (var kvp in hitCountDic)
            {
                string[] keys = kvp.Key.Split('_');
                int instanceId = int.Parse(keys[0]);
                if (isPlayer)
                {
                    if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                        result += kvp.Value;
                }
                else
                {
                    if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                        result += kvp.Value;
                }
            }
            return result;
        }

        private static uint GetAllSkillCounts(int fightIndex, bool isPlayer = true)
        {
            uint result = 0;
            Dictionary<string, uint> skillCountDic = FightController.instance.skillCounts[fightIndex];
            foreach (var kvp in skillCountDic)
            {
                string[] keys = kvp.Key.Split('_');
                int instanceId = int.Parse(keys[0]);
                if (isPlayer)
                {
                    if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                        result += kvp.Value;
                }
                else
                {
                    if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                        result += kvp.Value;
                }
            }
            return result;
        }

        private static uint GetAllHitDamages(int fightIndex, bool isPlayer = true)
        {
            uint result = 0;
            Dictionary<string, uint> hitDamageDic = JudgeController.instance.hitDamages[fightIndex];
            foreach (var kvp in hitDamageDic)
            {
                string[] keys = kvp.Key.Split('_');
                int instanceId = int.Parse(keys[0]);
                if (isPlayer)
                {
                    if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                        result += kvp.Value;
                }
                else
                {
                    if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                        result += kvp.Value;
                }
            }
            return result;
        }

        private static uint GetAllSkillDamages(int fightIndex, bool isPlayer = true)
        {
            uint result = 0;
            Dictionary<string, uint> skillDamageDic = JudgeController.instance.skillDamages[fightIndex];
            foreach (var kvp in skillDamageDic)
            {
                string[] keys = kvp.Key.Split('_');
                int instanceId = int.Parse(keys[0]);
                if (isPlayer)
                {
                    if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                        result += kvp.Value;
                }
                else
                {
                    if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                        result += kvp.Value;
                }
            }
            return result;
        }

        private static uint GetAllHitCounts(bool isPlayer = true)
        {
            uint result = 0;
            int i = 0;
            foreach (var hitCountDic in FightController.instance.hitCounts)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in hitCountDic)
                    {
                        string[] keys = kvp.Key.Split('_');
                        int instanceId = int.Parse(keys[0]);
                        if (isPlayer)
                        {
                            if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                        else
                        {
                            if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                    }
                }
                i++;
            }
            return result;
        }

        private static uint GetAllSkillCounts(bool isPlayer = true)
        {
            uint result = 0;
            int i = 0;
            foreach (var skillCountDic in FightController.instance.skillCounts)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in skillCountDic)
                    {
                        string[] keys = kvp.Key.Split('_');
                        int instanceId = int.Parse(keys[0]);
                        if (isPlayer)
                        {
                            if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                        else
                        {
                            if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                    }
                }
                i++;
            }
            return result;
        }

        private static uint GetAllHitDamages(bool isPlayer = true)
        {
            uint result = 0;
            int i = 0;
            foreach (var hitDamageDic in JudgeController.instance.hitDamages)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in hitDamageDic)
                    {
                        string[] keys = kvp.Key.Split('_');
                        int instanceId = int.Parse(keys[0]);
                        if (isPlayer)
                        {
                            if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                        else
                        {
                            if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                    }
                }
                i++;
            }
            return result;
        }

        private static uint GetAllSkillDamages(bool isPlayer = true)
        {
            uint result = 0;
            int i = 0;
            foreach (var skillDamageDic in JudgeController.instance.skillDamages)
            {
                if (i < fightFinishCount)
                {
                    foreach (var kvp in skillDamageDic)
                    {
                        string[] keys = kvp.Key.Split('_');
                        int instanceId = int.Parse(keys[0]);
                        if (isPlayer)
                        {
                            if (ExisitInDic(FightController.instance.playerInfoDic, instanceId) || ExisitInDic(FightController.instance.heroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                        else
                        {
                            if (ExisitInDic(FightController.instance.enemeyPlayerInfoDic, instanceId) || ExisitInDic(FightController.instance.enemeyHeroInfoDic, instanceId))
                                result += kvp.Value;
                        }
                    }
                }
                i++;
            }
            return result;
        }

        private static float GetHitDamageRateOfTotalDamage(int fightIndex, bool isPlayer = true)
        {
            float result = 0f;
            uint hitDamages = GetAllHitDamages(fightIndex, isPlayer);
            uint skillDamages = GetAllSkillDamages(fightIndex, isPlayer);
            if (hitDamages + skillDamages == 0)
                return 1f;
            result = (float)hitDamages / (float)(hitDamages + skillDamages);
            return result * 100f;
        }

        private static float GetHitDamageRateOfTotalDamage(bool isPlayer = true)
        {
            float result = 0f;
            uint hitDamages = GetAllHitDamages(isPlayer);
            uint skillDamages = GetAllSkillDamages(isPlayer);
            if (hitDamages + skillDamages == 0)
                return 100f;
            result = (float)hitDamages / (float)(hitDamages + skillDamages);
            return result * 100;
        }

        private static float GetWinRate()
        {
            if (fightFinishCount == 0)
                return 0f;
            float result = 0f;
            int winCount = 0;
            foreach (var r in FightController.instance.fightResults)
            {
                if (r)
                    winCount++;
            }
            result = winCount / fightFinishCount;
            return result * 100;
        }

        private static bool ExisitInDic(Dictionary<FormationPosition, HeroInfo> dic, int instanceId)
        {
            foreach (var kvp in dic)
            {
                if (kvp.Value.instanceID == instanceId)
                    return true;
            }
            return false;
        }

        private static bool ExisitInDic(Dictionary<FormationPosition, PlayerInfo> dic, int instanceId)
        {
            foreach (var kvp in dic)
            {
                if (kvp.Value.instanceID == instanceId)
                    return true;
            }
            return false;
        }
        #endregion

        private static void Init()
        {
            FightController.instance.InitImitateData();
            JudgeController.instance.InitImitateData();
        }

        void OnDestroy()
        {
            FightController.imitate = false;
        }
    }
}