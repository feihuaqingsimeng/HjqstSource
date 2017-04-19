﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_AudioRolloffModeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(UnityEngine.AudioRolloffMode));
		L.RegVar("Logarithmic", get_Logarithmic, null);
		L.RegVar("Linear", get_Linear, null);
		L.RegVar("Custom", get_Custom, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Logarithmic(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.AudioRolloffMode.Logarithmic);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Linear(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.AudioRolloffMode.Linear);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Custom(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.AudioRolloffMode.Custom);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		UnityEngine.AudioRolloffMode o = (UnityEngine.AudioRolloffMode)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}
