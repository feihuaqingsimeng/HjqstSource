using System.Collections.Generic;
using Logic.Character;
using Logic.Skill.Model;
using Logic.Enums;
using Logic.Net.Controller;
using UnityEngine;
using Common.Util;
namespace Logic.Net
{
    public static class VirtualServerControllerUtil
    {
        public static int SortedByPosition(CharacterEntity x, CharacterEntity y)
        {
            if (x.positionId < y.positionId)
                return -1;
            if (x.positionId > y.positionId)
                return 1;
            return 0;
        }

        public static bool ExistInDic(uint characterId, Dictionary<string, List<KeyValuePair<uint, uint>>> beLcokedDic)
        {
            bool result = false;
            List<string> lockedKeys = beLcokedDic.GetKeys();
            List<KeyValuePair<uint, uint>> lockedList = null;
            for (int j = 0, count = lockedKeys.Count; j < count; j++)
            {
                string key = lockedKeys[j];
                KeyValuePair<uint, uint> kvp = key.SplitToKeyValuePair<uint, uint>();
                SkillData sd = SkillData.GetSkillDataById(kvp.Value);
                MechanicsData md = MechanicsData.GetMechanicsDataById(sd.timeline.First().Value.First());//第一个作用效果
                if (md.targetType == TargetType.Ally)
                    continue;
                lockedList = beLcokedDic[key];
                for (int i = 0, iCount = lockedList.Count; i < iCount; i++)
                {
                    if (lockedList[i].Key == characterId)
                        result |= true;
                }
                lockedList = null;
            }
            lockedKeys.Clear();
            lockedKeys = null;
            return result;
        }

        public static void RemoveFromDic(uint characterId, Dictionary<string, List<KeyValuePair<uint, uint>>> beLcokedDic)
        {
            List<string> lockedKeys = beLcokedDic.GetKeys();
            List<KeyValuePair<uint, uint>> lockedList = null;
            for (int j = 0, count = lockedKeys.Count; j < count; j++)
            {
                string key = lockedKeys[j];
                KeyValuePair<uint, uint> kvp = key.SplitToKeyValuePair<uint, uint>();
                SkillData sd = SkillData.GetSkillDataById(kvp.Value);
                MechanicsData md = MechanicsData.GetMechanicsDataById(sd.timeline.First().Value.First());//第一个作用效果
                if (md.targetType == TargetType.Ally)
                    continue;
                lockedList = beLcokedDic[key];
                for (int i = 0, iCount = lockedList.Count; i < iCount; i++)
                {
                    if (lockedList[i].Key == characterId)
                    {
                        lockedList.RemoveAt(i);
                        break;
                    }
                }
                lockedList = null;
            }
            lockedKeys.Clear();
            lockedKeys = null;
        }

        public static string GetFirstSkillKey()
        {
            lock (VirtualServer.instance.skillWaitingQueue)
            {
                if (VirtualServer.instance.skillWaitingQueue.Count > 0)
                {
                    string key = VirtualServer.instance.skillWaitingQueue.Peek();
                    if (VirtualServer.instance.skillWaitingDic.ContainsKey(key))
                    {
                        return key;
                    }
                    VirtualServer.instance.skillWaitingQueue.Dequeue();
                }
            }
            return string.Empty;
        }

        public static int SortedByHPAndDefense(CharacterEntity x, CharacterEntity y)
        {
            if (x.HP * (x.physicsDefense + x.magicDefense) < y.HP * (y.physicsDefense + y.magicDefense))
                return -1;
            if (x.HP * (x.physicsDefense + x.magicDefense) > y.HP * (y.physicsDefense + y.magicDefense))
                return 1;
            return 0;
        }

        public static int SortedByHPPercent(CharacterEntity x, CharacterEntity y)
        {
            if (((float)x.HP / x.maxHP) < ((float)y.HP / y.maxHP))
                return -1;
            if (((float)x.HP / x.maxHP) > ((float)y.HP / y.maxHP))
                return 1;
            return 0;
        }

        public static List<uint> GetRandomTargetN(List<uint> list, int max)
        {
            List<uint> tids = null;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.ConsortiaFight:
                    tids = GetRandomTargetNPVP(list);
                    break;
                default:
                    tids = GetRandomTargetNPVE(list, max);
                    break;
            }
            return tids;
        }

        private static List<uint> GetRandomTargetNPVE(List<uint> list, int max)
        {
            int count = 0;
            if (max > 0)
                count = max;
            else
                count = Random.Range(1, list.Count + 1);
            if (count == list.Count)
                return list;
            List<uint> tids = new List<uint>();
            int random = 0;
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
            return tids;
        }

        private static List<uint> GetRandomTargetNPVP(List<uint> list)
        {
            List<uint> tids = new List<uint>();
            int size = list.Count;
            int seed = Fight.Model.FightProxy.instance.randomSeed;
            int randomCount = RandomUtil.GetRandomBySeedNoEnd(seed, 1, size + 1);
            for (int i = 0; i < randomCount; i++)
            {
                int index = RandomUtil.GetRandomBySeedNoEnd(i, 0, size);
                tids.Add(list[index]);
            }
            return tids;
        }
    }
}