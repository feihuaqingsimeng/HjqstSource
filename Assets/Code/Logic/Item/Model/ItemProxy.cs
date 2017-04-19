using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Game.Model;

namespace Logic.Item.Model
{
    public class ItemProxy : SingletonMono<ItemProxy>
    {

        private Dictionary<int, ItemInfo> _itemsDictionary = new Dictionary<int, ItemInfo>();

        public delegate void OnItemInfoUpdateDelegate(int instanceID);
        public OnItemInfoUpdateDelegate onItemInfoUpdateDelegate;

        public delegate void OnItemInfoListUpdateDelegate();
        public OnItemInfoListUpdateDelegate onItemInfoListUpdateDelegate;

        void Awake()
        {
            instance = this;
        }

        public Dictionary<int, ItemInfo> GetAllItemInfoDictioary()
        {
            return _itemsDictionary;
        }

        public List<ItemInfo> GetAllItemInfoList()
        {
            return new List<ItemInfo>(_itemsDictionary.Values);
        }

        public int GetAllItemCount()
        {
            return _itemsDictionary.Count;
        }

        public ItemInfo GetItemInfoByInstanceID(int instanceID)
        {
            ItemInfo itemInfo = null;
            if (_itemsDictionary.ContainsKey(instanceID))
            {
                itemInfo = _itemsDictionary[instanceID];
            }
            return itemInfo;
        }
        public ItemInfo GetItemInfoByItemID(int itemID)
        {
            //ItemInfo equipmentInfo = null;
            List<ItemInfo> items = _itemsDictionary.GetValues();
            ItemInfo info;
            for (int i = 0, count = items.Count; i < count; i++)
            {
                info = items[i];
                if (info.itemData.id == itemID)
                {
                    return info;
                }
            }
            return null;
        }

        public int GetItemCountByItemID(int itemId)
        {
            ItemInfo info = GetItemInfoByItemID(itemId);
            int count = 0;
            if (info != null)
            {
                count = info.count;
            }
            return count;
        }
        public bool AddItemInfo(ItemInfo itemInfo)
        {
            if (_itemsDictionary.ContainsKey(itemInfo.instanceId))
            {
                return false;
            }
            _itemsDictionary.Add(itemInfo.instanceId, itemInfo);
            return true;
        }

        public bool RemoveItemInfo(int instanceID)
        {
            return _itemsDictionary.Remove(instanceID);
        }

        public bool UpdateItemInfo(ItemProtoData data)
        {
            GetItemInfoByInstanceID(data.id).Update(data);
            if (onItemInfoUpdateDelegate != null)
            {
                onItemInfoUpdateDelegate(data.id);
            }
            return true;
        }

        public void OnItemInfoListUpdate()
        {
            if (onItemInfoListUpdateDelegate != null)
            {
                onItemInfoListUpdateDelegate();
            }
        }
        public int GetItemCellNum()
        {
            return 32;
        }
        //排序
        public static int CompareItemInfo(ItemInfo aInfo, ItemInfo bInfo)
        {
            int compare = CompareItemInfoBySort(aInfo, bInfo);
            if (compare != 0)
                return compare;
            return CompareItemInfoByID(aInfo, bInfo);
        }
        private static int CompareItemInfoBySort(ItemInfo aInfo, ItemInfo bInfo)
        {
            return aInfo.itemData.sort - bInfo.itemData.sort;
        }
        private static int CompareItemInfoByID(ItemInfo aInfo, ItemInfo bInfo)
        {
            return aInfo.itemData.id - bInfo.itemData.id;
        }
    }
}

