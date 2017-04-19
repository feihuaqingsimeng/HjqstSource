netmanager={}

local function Start()
	
end

function netmanager.GetProtocolData()
	return LuaInterface.ToLuaProtol.data
end

function netmanager.RegisterProtocol(id,decoder)
  if id == nil then
    print('RegisterProtocol[id]'..id..'is nil')
  end
	if netmanager[id] then print('repeat add:'..id) return end
	netmanager[id]=decoder
end

function netmanager.SendProtocol(id,pb)
	if pb then LuaInterface.ToLuaProtol.data=pb:SerializeToString()
	else LuaInterface.ToLuaProtol.data=pb end
	LuaInterface.ToLuaPb.SetFromLua(id)
    print('lua send protocol to server, protocol id:'..id)
end

function LuaNetDecoder(id)
	print('lua 收到服务器协议 server id:'..id)
	if netmanager[id] then netmanager[id]() 
	else print('not exist id:'..id)	end
	--Observers.Facade.Instance.SendNotification(id);
end

Start()