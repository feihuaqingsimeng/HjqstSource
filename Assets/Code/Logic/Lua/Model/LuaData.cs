using UnityEngine;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
namespace Logic.Lua.Model
{
    public class LuaData
    {

        private static Dictionary<string, LuaData> _luaDataDic;

        public static Dictionary<string, LuaData> GetLuaDatas()
        {
            if (_luaDataDic == null)
            {
                string localPath = "config/csv/lua";
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath + ".csv"))
                    _luaDataDic = CSVUtil.Parse<string, LuaData>(localPath, "name");
                else
                    _luaDataDic = CSVUtil.Parse<string, LuaData>("config/lua", "name");
            }
            return _luaDataDic;
        }

        public static LuaData GetGetLuaDataByName(string name)
        {
            if (_luaDataDic == null)
                GetLuaDatas();
            if (_luaDataDic.ContainsKey(name))
                return _luaDataDic[name];
            return null;
        }

        public static bool ExistLuaFile(string name)
        {
            if (_luaDataDic == null)
                GetLuaDatas();
            return _luaDataDic.ContainsKey(name);
        }

        [CSVElement("name")]
        public string name;

        [CSVElement("path")]
        public string path;
    }
}
