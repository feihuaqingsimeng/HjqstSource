using UnityEngine;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
namespace Logic.Character.Model
{
    public class MaterialData
    {
        private static Dictionary<string, MaterialData> _materialDataDic;

        public static Dictionary<string, MaterialData> GetMaterialDatas()
        {
            if (_materialDataDic == null)
            {
                _materialDataDic = CSVUtil.Parse<string, MaterialData>("config/csv/material", "modelName");
            }
            return _materialDataDic;
        }

        public static MaterialData GetMaterialDataByModelName(string modelName)
        {
            if (_materialDataDic == null)
                GetMaterialDatas();
            if (_materialDataDic.ContainsKey(modelName))
                return _materialDataDic[modelName];
            //Debugger.LogError("can't find material modelName:" + modelName);
            return null;
        }

        #region field
        [CSVElement("modelName")]
        public string modelName;

        [CSVElement("rim_pow")]
        public float rimPow;
        #endregion
    }
}