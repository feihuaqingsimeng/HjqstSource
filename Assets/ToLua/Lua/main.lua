Object=UnityEngine.Object
Resources=UnityEngine.Resources
Action=System.Action
Action_object=System.Action_object

local ASSETOBJ
local ASSETNAME

local function mLoadAsset()
	 ASSETOBJ=Object.Instantiate(Resources.Load(ASSETNAME))
end

function LoadAsset(name)
	ASSETNAME=name
	local co=coroutine.create(mLoadAsset)
	local flag,msg = coroutine.resume(co)
	if not flag then print(msg) end
	return ASSETOBJ
end

require 'luautil'
require 'netmanager'
require 'gamedata'
require 'fightdata'
require 'ui/uimanager'
require 'gamemanager'

gamedataTable.InitGamedata()

--[[
require 'user/Procotol/login_pb'
function Encoder()
	local pb=login_pb.LoginResp()
	pb.roleName='heronamefromlua'

	local s=pb:SerializeToString()
	LuaInterface.ToLuaProtol.data=s

	LuaInterface.ToLuaPb.SetFromLua(999)
end

function Decoder( ... )
	local id=select(1,...)
	local pb=login_pb.LoginResp()
	pb:ParseFromString(LuaInterface.ToLuaProtol.data)

	print('ReceiveName:'..pb.roleName)

	pb.roleName='heronamefromlua'

	local s=pb:SerializeToString()
	LuaInterface.ToLuaProtol.data=s

	print(pb.roleName..pb.account)
	LuaInterface.ToLuaPb.SetFromLua(999)
end

function Login(account,password,devicesId)
	local pb = login_pb.LoginReq()
	pb.account = account
	pb.password = password
	pb.devicesId = devicesId
	local s = pb:SerializeToString()
	LuaInterface.ToLuaProtol.data = s

	LuaInterface.ToLuaPb.SetFromLua(1)
end
--]]

--[[
data=nil
print(package.path)
require 'user/Procotol/activity_pb'
function Encoder()
	local pb=activity_pb.ActivityRsp()
	print('activity:'..type(pb.infos))

	data=pb:SerializeToString()
	-- local s=pb:SerializeToString()
	-- LuaInterface.ToLuaProtol.data=s

	-- LuaInterface.ToLuaPb.SetFromLua(999)
end

function Decoder( ... )
	local id=select(1,...)
	local pb=activity_pb.ActivityRsp()
	pb:ParseFromString(data)

	print('activity:'..pb.infos)
end

Encoder()
Decoder()
--]]

