using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Net.Controller;
using Logic.Character;
namespace Logic.Net.Editor
{
    [ExecuteInEditMode]
    public class GetTargetsWindow : UnityEditor.EditorWindow
    {
        [MenuItem("CEngine/战斗目标获取检测工具", false, 100)]
        public static void OpenGetTargetsWindow()
        {
            EditorWindow.GetWindow<GetTargetsWindow>();
        }
        TargetType targetType = TargetType.None;
        RangeType rangeType = RangeType.None;
        int positionId;
        bool isPlayer = true;
        List<KeyValuePair<uint, uint>> targets = null;
        private string[] targetArray = new string[] { string.Empty, "我方", "敌方" };
        private string[] rangeTypes = new string[]{string.Empty,"当前目标单体","当前目标所在横行","当前目标所在的列","全体","当前所在目标以及后面1格（共2格）",
            "当前目标以及相邻的十字型","当前目标后面第1格（共1格）","//当前目标后面第2格（共1格）","当前目标横向间隔1格位置（上下2个点，最多打1人)",
            "以当前目标为起点的随机闪电链，3人","全体目标中随机N个","绝对位置，正中十字","绝对位置，第一列","绝对位置，第二列","绝对位置，第三列","绝对位置，第一行",
            "绝对位置，第二行","绝对位置，第三行","绝对位置，除中间1格的其它8格","绝对位置，正中间1格","绝对位置，上对角线(主对角线)","绝对位置，下对角线(次对角线)",
            "绝对位置，全体","弱点攻击","最低血量","随机敌方单体","当前目标及身后两列","固定最好两列","随机中异常的友单位"};
        private string[] positionIds = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        void OnGUI()
        {
            if (Application.isPlaying)
            {
                if (VirtualServerController.instance)
                    VirtualServerController.instance.fightEidtor = true;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("战斗目标获取检测工具");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("选中当前方为我方，未选中当前方为敌方");
            isPlayer = EditorGUILayout.Toggle("我方：", isPlayer, GUILayout.Width(500));
            positionId = EditorGUILayout.Popup("当前位置：", positionId, positionIds, GUILayout.Width(500));
            targetType = (TargetType)EditorGUILayout.Popup("目标类型", (int)targetType, targetArray, GUILayout.Width(500));
            //EditorGUILayout.LabelField("1.CurrentSingle：当前目标单体 2.CurrentRow：当前目标所在横行 3.CurrentColumn：当前目标所在的列");
            //EditorGUILayout.LabelField("4.All：全体 5.CurrentAndBehindFirst：当前所在目标以及后面1格（共2格）6.CurrentAndNearCross：当前目标以及相邻的十字型");
            //EditorGUILayout.LabelField("7.CurrentBehindFirst：当前目标后面第1格（共1格）8.CurrentBehindSecond：当前目标后面第2格（共1格）9.CurrentUpOrDown：当前目标横向间隔1格位置（上下2个点，最多打1人)");
            //EditorGUILayout.LabelField("10.CurrentAndRandomTwo：以当前目标为起点的随机闪电链，3人 11.RandomN：全体目标中随机N个 12.Cross：绝对位置，正中十字");
            //EditorGUILayout.LabelField("13.FirstColum：绝对位置，第一列 14.SecondColum：绝对位置，第二列 15.ThirdColum：绝对位置，第三列");
            //EditorGUILayout.LabelField("16.FirstRow：绝对位置，第一行 17.SecondRow：绝对位置，第二行 18.ThirdRow：绝对位置，第三行");
            //EditorGUILayout.LabelField("19.ExceptMidpoint：绝对位置，除中间1格的其它8格 20.Midpoint：绝对位置，正中间1格");
            //EditorGUILayout.LabelField("21.LeadingDiagonal：绝对位置，上对角线(主对角线) 22.SecondaryDiagonal：绝对位置，下对角线(次对角线)");
            rangeType = (RangeType)EditorGUILayout.Popup("范围类型", (int)rangeType, rangeTypes, GUILayout.Width(500));
            if (Application.isPlaying)
            {
                if (GUILayout.Button("查找", GUILayout.Width(500)))
                {
                    if (Fight.Controller.FightController.instance.fightStatus != FightStatus.GameOver)
                    {
                        if (targetType == TargetType.None || rangeType == RangeType.None) return;
                        if (targets != null)
                            targets.Clear();
                        targets = null;
                        int pid = positionId + (isPlayer ? 1 : 101);
                        targets = VirtualServerController.instance.GetTargets((uint)pid, targetType, rangeType, isPlayer);
                    }
                }
                EditorGUILayout.LabelField("当前方为：" + (isPlayer ? "我方" : "敌方"));
                EditorGUILayout.LabelField("目标为：" + targetType.ToString());
                EditorGUILayout.LabelField("范围为：" + rangeType.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("目标列表：");
                EditorGUILayout.Space();
                if (targets != null)
                {
                    for (int i = 0, count = targets.Count; i < count; i++)
                    {
                        CharacterEntity target1 = CharacterUtil.FindTarget(targetType, isPlayer, targets[i].Key);
                        CharacterEntity target2 = CharacterUtil.FindTarget(targetType, isPlayer, targets[i].Value);
                        if (target1 is PlayerEntity)
                            EditorGUILayout.LabelField("技能目标：主角：" + target1.name.Replace("(Clone)", string.Empty) + "，ID:" + target1.characterInfo.instanceID + "，位置为左边第" + target1.positionId + "位");
                        else if (target1 is HeroEntity)
                            EditorGUILayout.LabelField("技能目标：伙伴：" + target1.name.Replace("(Clone)", string.Empty) + "，ID:" + target1.characterInfo.instanceID + "，位置为左边第" + target1.positionId + "位");
                        else if (target1 is EnemyEntity)
                            EditorGUILayout.LabelField("技能目标：敌方：" + target1.name.Replace("(Clone)", string.Empty) + "，ID:" + target1.characterInfo.instanceID + "，位置为右边第" + (target1.positionId % 100) + "位");
                        else if (target1 is EnemyPlayerEntity)
                            EditorGUILayout.LabelField("技能目标：敌方：" + target1.name.Replace("(Clone)", string.Empty) + "，ID:" + target1.characterInfo.instanceID + "，位置为右边第" + (target1.positionId % 100) + "位");
                        if (target2 is PlayerEntity)
                            EditorGUILayout.LabelField("效果目标：主角：" + target2.name.Replace("(Clone)", string.Empty) + "，ID:" + target2.characterInfo.instanceID + "，位置为左边第" + target2.positionId + "位");
                        else if (target2 is HeroEntity)
                            EditorGUILayout.LabelField("效果目标：伙伴：" + target2.name.Replace("(Clone)", string.Empty) + "，ID:" + target2.characterInfo.instanceID + "，位置为左边第" + target2.positionId + "位");
                        else if (target2 is EnemyEntity)
                            EditorGUILayout.LabelField("效果目标：敌方：" + target2.name.Replace("(Clone)", string.Empty) + "，ID:" + target2.characterInfo.instanceID + "，位置为右边第" + (target2.positionId % 100) + "位");
                        else if (target2 is EnemyPlayerEntity)
                            EditorGUILayout.LabelField("效果目标：敌方：" + target2.name.Replace("(Clone)", string.Empty) + "，ID:" + target2.characterInfo.instanceID + "，位置为右边第" + (target2.positionId % 100) + "位");
                        EditorGUILayout.Space();
                    }
                }
            }
            else
            {
                if (targets != null)
                    targets.Clear();
            }
        }

        void OnLostFocus()
        {
            if (!Application.isPlaying)
                targets.Clear();
        }

        void OnDestroy()
        {
            if (VirtualServerController.instance)
                VirtualServerController.instance.fightEidtor = false;
        }
    }
}