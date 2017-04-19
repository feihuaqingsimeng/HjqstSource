using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Logic.Character;
using Logic.Character.Controller;
namespace Logic.Net.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Net.Controller.VirtualServer))]
    public class VirtualServerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Net.Controller.VirtualServer virtualServer = target as Logic.Net.Controller.VirtualServer;
            EditorGUILayout.LabelField("当前屏数:", (Logic.Fight.Model.FightProxy.instance.CurrentTeamIndex + 1).ToString());
            EditorGUILayout.LabelField("能否战斗:", virtualServer.canFight.ToString());
            EditorGUILayout.LabelField("玩家装备:", virtualServer.playerReady.ToString());
            EditorGUILayout.LabelField("怪物准备:", virtualServer.enemyReady.ToString());

            EditorGUILayout.LabelField("--------------------------------------------------");
            EditorGUILayout.LabelField(virtualServer.skillWaitingOrders.Count.ToString() + "个等待预约技能列表:");
            foreach (var kvp in virtualServer.skillWaitingOrders)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString() + "  " + "玩家ID:" + kvp.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("--------------------------------------------------");
            EditorGUILayout.LabelField("引导技能列表:");
            EditorGUILayout.LabelField("当前时间:" + Time.time.ToString());
            foreach (var kvp in virtualServer.bootSkillWaitingDic)
            {
                EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString());
                EditorGUILayout.LabelField("isPlayer:" + kvp.Value.isPlayer.ToString());
                EditorGUILayout.LabelField("技能释放时间:" + kvp.Value.time.ToString());
            }
            EditorGUILayout.LabelField(string.Format("普攻技能列表:{0}", virtualServer.hitSkillQueue.Count.ToString()));
            foreach (var kvp in virtualServer.hitSkillQueue)
            {
                EditorGUILayout.BeginHorizontal();

                CharacterEntity c = PlayerController.instance[kvp.Key];
                if (!c)
                    c = EnemyController.instance[kvp.Key];
                if (c)
                    EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString() + "  " + "玩家ID:" + kvp.Value.ToString() + "  speed:" + c.speed);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("--------------------------------------------------");
            EditorGUILayout.LabelField("预约技能列表:");
            foreach (var kvp in virtualServer.skillWaitingDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString() + "  " + "玩家ID:" + kvp.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("--------------------------------------------------");
            EditorGUILayout.LabelField("正在释放技能列表:");
            foreach (var kvp in virtualServer.skillingDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString() + " " + "玩家ID:" + kvp.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("--------------------------------------------------");
            EditorGUILayout.LabelField("技能释放锁定角色列表:");
            foreach (var kvp in virtualServer.beLcokedDic)
            {
                EditorGUILayout.LabelField("技能ID:" + kvp.Key.ToString());
                EditorGUILayout.BeginHorizontal();
                foreach (var id in kvp.Value)
                {
                    EditorGUILayout.LabelField("玩家ID:" + id.Key.ToString());
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}