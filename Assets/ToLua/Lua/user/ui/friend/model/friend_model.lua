local t = {}
local name = 'friend_model'

local friend_info = require('ui/friend/model/friend_info')


--FriendFuncType,Dictionary<id,friendInfo>
t.friendDictionary = Dictionary.New('number','Dictionary<number,friendInfo>')

local function Start()
  gamemanager.RegisterModel(name,t)
end

function t.Clear()
  t.friendDictionary:Clear()
end
function t.ClearFriendByType(friendFuncType)
  local dic = t.friendDictionary:Get(friendFuncType)
  if dic then
    dic:Clear()
  end
end

function t.AddFriendFromCSharp(friendFuncType,friendInfoCSharp)
  local dic = t.friendDictionary:Get(friendFuncType)
  if not dic then
    dic = Dictionary.New('number','friendInfo')
    t.friendDictionary:Add(friendFuncType ,dic) 
  end
  if not dic:Get(friendInfoCSharp.id) then
    dic:Add(friendInfoCSharp.id,friend_info.NewByCSharpFriendInfo(friendInfoCSharp))
    
  end
end
function t.AddFriend(friendFuncType,friendInfo)
  local dic = t.friendDictionary:Get(friendFuncType)
  if not dic then
    dic = Dictionary.New('number','friendInfo')
    t.friendDictionary:Add(friendFuncType,dic )
  end
  if not dic:Get(friendInfo.id) then
    dic:Add(friendInfo.id,friendInfo)
  end
end

function t.RemoveFriend(friendFuncType,id)
  if t.friendDictionary:Get(friendFuncType) then
    t.friendDictionary:Get(friendFuncType):Remove(id)
  end
end

function t.GetFriendByType(friendFuncType,id)
  local friend = nil
  if t.friendDictionary:Get(friendFuncType) then
    
    friend = t.friendDictionary:Get(friendFuncType):Get(id)
  end
  return friend
end
function t.GetFriend(id)
  for k,v in pairs(t.friendDictionary:GetDatas()) do
    local info = v:Get(id)
    if info then
      return info
    end
  end
  return nil
end
---------------update by protocol---------------


Start()
return t
