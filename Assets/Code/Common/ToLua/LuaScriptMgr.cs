using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System;
namespace LuaInterface
{
    public class LuaScriptMgr
    {
        public static LuaScriptMgr Instance;
        public LuaState lua { get; private set; }
        private LuaLooper _loop;
        private List<string> _fileNames = new List<string>();
        private Dictionary<string, LuaBaseRef> _luaVarCache = new Dictionary<string, LuaBaseRef>();

        public LuaScriptMgr()
        {
            Instance = this;
            lua = new LuaState();
            lua.OpenLibs(LuaDLL.luaopen_pb);
            lua.LuaSetTop(0);
        }

        public void Start()
        {
            LuaFileUtil.AddSearchPath();
            LuaBinder.Bind(lua);
            lua.Start();

            _loop = GameObject.Find("root/base").AddComponent<LuaLooper>();
            _loop.luaState = lua;

            DoFile("main");

            LuaFileUtil.AddLuaPaths();
        }

        public object[] DoFile(string fullpath, bool multiExcute = false)
        {
            if (multiExcute)
            {
                if (!_fileNames.Contains(fullpath))
                    _fileNames.Add(fullpath);
                return lua.DoFile(fullpath);
            }
            else
            {
                if (!_fileNames.Contains(fullpath))
                {
                    _fileNames.Add(fullpath);
                    return lua.DoFile(fullpath);
                }
            }
            return null;
        }

        public void LuaGC() 
        {
            LuaScriptMgr.Instance.CallLuaFunction("gamemanager.gc");
        }

        public bool IsFuncExists(string name)
        {
            //just for global func
            return GetLuaFunction(name) != null;
        }

        public LuaFunction GetLuaFunction(string name)
        {
            return GetLuaVar(name) as LuaFunction;
        }

        /// <summary>
        /// have params or return value
        /// </summary>
        public object[] CallLuaFunction(string name, params object[] args)
        {
            var func = GetLuaFunction(name);
            if (func != null)
            {
                if (args == null || args.Length == 0) { return func.Call(0); }
                else { return func.Call(args); }
            }
            return null;
        }

        /// <summary>
        /// no params and no return value
        /// </summary>
        public void CallLuaFunctionVoid(string name)
        {
            var func = GetLuaFunction(name);
            if (func != null)
            {
                func.Call();
            }
        }

        public void Destroy()
        {
            foreach (var item in _luaVarCache)
            {
                if (item.Value != null)
                    item.Value.Dispose();
            }

            UnityEngine.Object.Destroy(_loop);

            _luaVarCache.Clear();
            _luaVarCache = null;

            Instance = null;

            lua.Dispose();
            lua = null;
        }

        public T GetTableValue<T>(string tableName, string key)
        {
            var t = GetLuaTable(tableName);
            if (t != null)
            {
                var o = t[key];
                return (T)o;
            }
            else
            {
                Debugger.Log("table: {0} not exist!", tableName);
            }

            return default(T);
        }

        public LuaTable GetLuaTable(string fullPath)
        {
            var t = GetLuaVar(fullPath);
            return t as LuaTable;
        }

        private LuaBaseRef GetLuaVar(string name)
        {
            if (_luaVarCache.ContainsKey(name))
                return _luaVarCache[name];
            if (lua == null) return null;
            var t = lua[name] as LuaBaseRef;
            _luaVarCache.Add(name, t);
            return t;
        }

        public T[] LuaTableToArray<T>(string tableName)
        {
            LuaTable luaTable = GetLuaTable(tableName);
            if (luaTable == null) return default(T[]);

            var objs = luaTable.ToArray();
            var length = objs.Length;
            T[] rs = new T[length];
            for (int i = 0; i < length; i++)
            {
                rs[i] = (T)objs[i];
            }
            return rs;
        }

        public int[] LuaTableToArrayInt(string tableName)
        {
            LuaTable luaTable = GetLuaTable(tableName);
            if (luaTable == null) return null;
            var objs = luaTable.ToArray();
            var length = objs.Length;
            int[] rs = new int[length];
            for (int i = 0; i < length; i++)
            {
                rs[i] = (int)((double)objs[i]);
            }
            return rs;
        }
    }
}