﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Logic_AdTracking_Controller_AdTrackingControllerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Logic.AdTracking.Controller.AdTrackingController), typeof(SingletonPersistent<Logic.AdTracking.Controller.AdTrackingController>));
		L.RegFunction("InitAdTracking", InitAdTracking);
		L.RegFunction("AdTracking_OnRegister", AdTracking_OnRegister);
		L.RegFunction("AdTracking_OnLogin", AdTracking_OnLogin);
		L.RegFunction("AdTracking_OnPay", AdTracking_OnPay);
		L.RegFunction("New", _CreateLogic_AdTracking_Controller_AdTrackingController);
		L.RegFunction("__tostring", Lua_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLogic_AdTracking_Controller_AdTrackingController(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Logic.AdTracking.Controller.AdTrackingController obj = new Logic.AdTracking.Controller.AdTrackingController();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Logic.AdTracking.Controller.AdTrackingController.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitAdTracking(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic.AdTracking.Controller.AdTrackingController obj = (Logic.AdTracking.Controller.AdTrackingController)ToLua.CheckObject(L, 1, typeof(Logic.AdTracking.Controller.AdTrackingController));
			obj.InitAdTracking();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdTracking_OnRegister(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic.AdTracking.Controller.AdTrackingController obj = (Logic.AdTracking.Controller.AdTrackingController)ToLua.CheckObject(L, 1, typeof(Logic.AdTracking.Controller.AdTrackingController));
			string arg0 = ToLua.CheckString(L, 2);
			obj.AdTracking_OnRegister(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdTracking_OnLogin(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic.AdTracking.Controller.AdTrackingController obj = (Logic.AdTracking.Controller.AdTrackingController)ToLua.CheckObject(L, 1, typeof(Logic.AdTracking.Controller.AdTrackingController));
			string arg0 = ToLua.CheckString(L, 2);
			obj.AdTracking_OnLogin(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdTracking_OnPay(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 6);
			Logic.AdTracking.Controller.AdTrackingController obj = (Logic.AdTracking.Controller.AdTrackingController)ToLua.CheckObject(L, 1, typeof(Logic.AdTracking.Controller.AdTrackingController));
			string arg0 = ToLua.CheckString(L, 2);
			string arg1 = ToLua.CheckString(L, 3);
			int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
			string arg3 = ToLua.CheckString(L, 5);
			string arg4 = ToLua.CheckString(L, 6);
			obj.AdTracking_OnPay(arg0, arg1, arg2, arg3, arg4);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Lua_ToString(IntPtr L)
	{
		object obj = ToLua.ToObject(L, 1);

		if (obj != null)
		{
			LuaDLL.lua_pushstring(L, obj.ToString());
		}
		else
		{
			LuaDLL.lua_pushnil(L);
		}

		return 1;
	}
}
