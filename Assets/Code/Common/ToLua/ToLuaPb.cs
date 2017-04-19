using UnityEngine;
using System.Collections;
using LuaInterface;
namespace LuaInterface
{
    public static class ToLuaPb
    {
        private static byte[] _data;
        private static int _id;

        [NoToLua]
        public static void Start()
        {
            //var inst = LuaScriptMgr.Instance;
            //LuaCoroutine.Register(inst.lua, Logic.Game.Controller.GameController.instance);
        }

        [NoToLua]
        public static void SetFromSocket(int id, byte[] bytes)
        {
            _id = id;

            if (ToLuaProtol.data == null)
                ToLuaProtol.data = new LuaByteBuffer(bytes);
            else
                ToLuaProtol.data.buffer = bytes;
            LuaScriptMgr.Instance.CallLuaFunction("LuaNetDecoder", _id);
        }

        //[NoToLua]
        //public static void CallFromSocket()
        //{
        //    LuaScriptMgr.Instance.CallLuaFunction("LuaNetDecoder", _id);
        //}

        //public static void Login(string account, string password, string devicesId)
        //{
        //    LuaScriptMgr.Instance.CallLuaFunction("Login", account, password, devicesId);
        //}

        public static void SetFromLua(int id)
        {
            if (ToLuaProtol.data == null) _data = null;
            else _data = ToLuaProtol.data.buffer;

            if (Logic.Protocol.ProtocolProxy.instance == null)
                Debugger.Log("pp is null!");
            else
				Logic.Protocol.ProtocolProxy.instance.SendProtocolByte(id, _data);
        }
    }
}