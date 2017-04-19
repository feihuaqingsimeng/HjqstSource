using Common.Util;
using System.Collections.Generic;
using UnityEngine;


namespace Logic.Game.Model
{
    public class AccountExpData
    {
        private static Dictionary<int, AccountExpData> _accountExpDataDictionary;

        public static Dictionary<int, AccountExpData> GetAccountExpData()
        {

            if (_accountExpDataDictionary == null)
            {
                _accountExpDataDictionary = CSVUtil.Parse<int, AccountExpData>("config/csv/exp_account", "lv");
            }
            return _accountExpDataDictionary;
        }

        public static Dictionary<int, AccountExpData> AccountExpDataDictionary
        {
            get
            {
                if (_accountExpDataDictionary == null)
                {
                    GetAccountExpData();
                }
                return _accountExpDataDictionary;
            }
        }

        public static AccountExpData GetAccountExpDataByLv(int accountLevel)
        {
            AccountExpData accountExpData = null;
            AccountExpDataDictionary.TryGetValue(accountLevel, out accountExpData);
            return accountExpData;
        }

        public static AccountExpData GetAccountExpDataByExp(int totalExp)
        {
            List<AccountExpData> dataList = GetAccountExpDataList();
            for (int i = 0, count = dataList.Count; i < count; i++)
            {
                if (totalExp < dataList[i].expTotal)
                {
                    return dataList[i];
                }
            }
            return null;
        }
        public static int GetMaxLevel()
        {
            List<AccountExpData> dataList = GetAccountExpDataList();
            return dataList[dataList.Count - 1].lv;
        }
        public static List<AccountExpData> GetAccountExpDataList()
        {
            return new List<AccountExpData>(AccountExpDataDictionary.Values);
        }

        [CSVElement("lv")]
        public int lv;

        [CSVElement("exp")]
        public int exp;

        [CSVElement("exp_total")]
        public int expTotal;

        [CSVElement("pve_action")]
        public int pveAction;
    }
}
