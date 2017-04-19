﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Logic_Pool_Controller_PoolControllerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Logic.Pool.Controller.PoolController), typeof(SingletonMono<Logic.Pool.Controller.PoolController>));
		L.RegFunction("CreatePool", CreatePool);
		L.RegFunction("CreateCharacterPool", CreateCharacterPool);
		L.RegFunction("Despawn", Despawn);
		L.RegFunction("GetPool", GetPool);
		L.RegFunction("ContainsPool", ContainsPool);
		L.RegFunction("ClearTemporaryPools", ClearTemporaryPools);
		L.RegFunction("ExsitInPool", ExsitInPool);
		L.RegFunction("SetPoolForever", SetPoolForever);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", Lua_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreatePool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			string arg0 = ToLua.CheckString(L, 2);
			string arg1 = ToLua.CheckString(L, 3);
			bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
			PathologicalGames.SpawnPool o = obj.CreatePool(arg0, arg1, arg2);
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateCharacterPool(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4 && TypeChecker.CheckTypes(L, typeof(Logic.Pool.Controller.PoolController), typeof(string), typeof(string), typeof(bool)))
			{
				Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.ToObject(L, 1);
				string arg0 = ToLua.ToString(L, 2);
				string arg1 = ToLua.ToString(L, 3);
				bool arg2 = LuaDLL.lua_toboolean(L, 4);
				PathologicalGames.SpawnPool o = obj.CreateCharacterPool(arg0, arg1, arg2);
				ToLua.Push(L, o);
				return 1;
			}
			else if (count == 5 && TypeChecker.CheckTypes(L, typeof(Logic.Pool.Controller.PoolController), typeof(string), typeof(string), typeof(bool), typeof(System.Action<PathologicalGames.SpawnPool>)))
			{
				Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.ToObject(L, 1);
				string arg0 = ToLua.ToString(L, 2);
				string arg1 = ToLua.ToString(L, 3);
				bool arg2 = LuaDLL.lua_toboolean(L, 4);
				System.Action<PathologicalGames.SpawnPool> arg3 = null;
				LuaTypes funcType5 = LuaDLL.lua_type(L, 5);

				if (funcType5 != LuaTypes.LUA_TFUNCTION)
				{
					 arg3 = (System.Action<PathologicalGames.SpawnPool>)ToLua.ToObject(L, 5);
				}
				else
				{
					LuaFunction func = ToLua.ToLuaFunction(L, 5);
					arg3 = DelegateFactory.CreateDelegate(typeof(System.Action<PathologicalGames.SpawnPool>), func) as System.Action<PathologicalGames.SpawnPool>;
				}

				obj.CreateCharacterPool(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Logic.Pool.Controller.PoolController.CreateCharacterPool");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Despawn(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3 && TypeChecker.CheckTypes(L, typeof(Logic.Pool.Controller.PoolController), typeof(string), typeof(Logic.Character.CharacterEntity)))
			{
				Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.ToObject(L, 1);
				string arg0 = ToLua.ToString(L, 2);
				Logic.Character.CharacterEntity arg1 = (Logic.Character.CharacterEntity)ToLua.ToObject(L, 3);
				obj.Despawn(arg0, arg1);
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes(L, typeof(Logic.Pool.Controller.PoolController), typeof(string), typeof(UnityEngine.Transform)))
			{
				Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.ToObject(L, 1);
				string arg0 = ToLua.ToString(L, 2);
				UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.ToObject(L, 3);
				obj.Despawn(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Logic.Pool.Controller.PoolController.Despawn");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			string arg0 = ToLua.CheckString(L, 2);
			PathologicalGames.SpawnPool o = obj.GetPool(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ContainsPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			string arg0 = ToLua.CheckString(L, 2);
			bool o = obj.ContainsPool(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearTemporaryPools(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			obj.ClearTemporaryPools();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ExsitInPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			string arg0 = ToLua.CheckString(L, 2);
			bool o = obj.ExsitInPool(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPoolForever(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			Logic.Pool.Controller.PoolController obj = (Logic.Pool.Controller.PoolController)ToLua.CheckObject(L, 1, typeof(Logic.Pool.Controller.PoolController));
			string arg0 = ToLua.CheckString(L, 2);
			bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
			obj.SetPoolForever(arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
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
