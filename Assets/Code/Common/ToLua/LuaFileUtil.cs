using UnityEngine;
using System.Collections;
using System.IO;
using Common.ResMgr;
using System.Collections.Generic;
namespace LuaInterface
{
    public static class LuaFileUtil
    {
        static List<string> paths = new List<string> { "lua/", "lua/protobuf/", "lua/user/", "lua/user/protocol/", "lua/user/config/" };

        public static void AddLuaPaths()
        {
            LuaTable lt = LuaScriptMgr.Instance.GetLuaTable("gamedataTable.paths");
            string[] strs = lt.ToArray<string>();
            AddSearchPath(strs);
            string aesKey = LuaScriptMgr.Instance.GetTableValue<string>("gamedataTable", "aesKey");
            if (string.IsNullOrEmpty(aesKey))
                Logic.Game.GameConfig.instance.encrypt = false;
            else
            {
                Logic.Game.GameConfig.instance.encrypt = true;
                Logic.Game.GameConfig.instance.aesEncryptKey = aesKey;
            }
        }

        public static byte[] ReadFile(string fileName)
        {
            string localPath = string.Empty;
            fileName = fileName.Replace(".lua", ".txt");
            for (int i = 0, length = paths.Count; i < length; i++)
            {
                string path = paths[i] + fileName;
                if (ResUtil.ExistsInLocal(path, out localPath))
                {
                    byte[] bytes = File.ReadAllBytes(localPath);
#if UNITY_EDITOR && !UNITY_ANDROID
                    if (Logic.Game.GameConfig.instance.loadLuaRemote)
                        bytes = Common.Util.EncryptUtil.MinusExcursionBytes(bytes, Logic.Game.GameConfig.excursion);
#endif
#if UNITY_IOS && !UNITY_EDITOR
                        bytes = Common.Util.EncryptUtil.MinusExcursionBytes(bytes, Logic.Game.GameConfig.excursion);
#endif
                    return bytes;
                }
            }
            Debugger.LogError("can not find {0},load lua txt error!", fileName);
            return null;
        }

        public static void AddSearchPath()
        {
            AddSearchPath(paths.ToArray());
        }

        private static void AddSearchPath(string[] paths)
        {
            for (int i = 0, count = paths.Length; i < count; i++)
            {
#if UNITY_EDITOR
                if (Logic.Game.GameConfig.instance.loadLuaRemote)
                    LuaScriptMgr.Instance.lua.AddSearchPath(ResUtil.GetLocalPath(paths[i]));
                else
                    LuaScriptMgr.Instance.lua.AddSearchPath(string.Format("{0}/ToLua/{1}", Application.dataPath, paths[i]));
#else
                LuaScriptMgr.Instance.lua.AddSearchPath(ResUtil.GetLocalPath(paths[i]));
#endif
            }
        }
    }
}