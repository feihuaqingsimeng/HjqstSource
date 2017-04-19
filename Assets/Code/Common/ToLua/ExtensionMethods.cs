using System;
using System.Text;
using UnityEngine;
using LuaInterface;
public static class ExtensionMethods
{
    public static void AppendLineEx(this StringBuilder sb, string str = "")
    {
        sb.Append(str + "\r\n");
    }

    public static T[] ToArray<T>(this LuaTable luaTable)
    {
        object[] os = luaTable.ToArray();
        T[] ts = new T[luaTable.Length];
		for (int i = 0, count = luaTable.Length; i < count; i++)
        {
            ts[i] = (T)os[i];
        }
        return ts;
    }

    public static LuaTable GetLuaTable(this LuaArrayTable luaArrayTable, int key)
    {
        return luaArrayTable[key] as LuaTable;
    }

    public static LuaTable GetLuaTable(this LuaDictTable luaDictTable, string key)
    {
        return luaDictTable[key] as LuaTable;
    }

}
