local t = {}
local name = 'consortia_model'

local consortia_info = require('ui/consortia/model/consortia_info')
local member_info = require('ui/consortia/model/member_info')

--自己的公会
t.consortiaInfo = nil
---公会列表
t.consortiaInfoList = {}
--已经申请的公会列表
t.alreadyApplyIdList = {}
--成员列表
t.memberInfoList = {}
--申请列表
t.guildApplyList = {}
--红点提示 是否有申请
t.hasGuildApply = false


t.onUpdateRefreshDelegate = void_delegate.New() -- 更新界面
t.onUpdateRefreshOneParamDelegate = void_delegate.New() -- 更新界面(一个参数)
t.onUpdateCreateSuccessDelegate = void_delegate.New()--创建公会
t.onUpdateExitSuccessDelegate = void_delegate.New()--退出公会
t.onUpdateKickedOutDelegate = void_delegate.New()--被踢出公会
t.onUpdateGuildSyncDelegate = void_delegate.New()--响应公会更新
t.onUpdateGuildPresentRespDelegate = void_delegate.New()--响应公会捐献
t.onUpdateGuildShopInfoDelegate = void_delegate.New()--响应公会商店
t.onUpdateGuildShopBuyRespDelegate = void_delegate.New()--响应公会商店购买
t.onUpdateGuildShopSynInfoRespDelegate = void_delegate.New()--公会商店同步

local function Start ()
  gamemanager.RegisterModel(name, t)
end

--公会是俺的否
function t.IsConsortiaCreator()
  if t.consortiaInfo then
    return gamemanager.GetModel('game_model').accountName == t.consortiaInfo.creatorName
  end
  return false
end
--是否已申请该公会
function t.IsApplyConsortia(consortiaId)
  return t.alreadyApplyIdList[consortiaId] ~= nil
end
--设置本尊的公会
function t.SetConsortiaInfo(guildInfoProto)
  if t.consortiaInfo == nil then
    t.consortiaInfo = consortia_info.NewByGuildInfoProto(guildInfoProto)
  else
    t.consortiaInfo:SetByGuildInfoProto(guildInfoProto)
    t.onUpdateGuildSyncDelegate:Invoke()
  end
end
--设置公会列表
function t.SetConsortiaList(guildInfoProtoList,alreadyApplyList)
  t.consortiaInfo = nil
  t.consortiaInfoList = {}
  for k,v in ipairs(guildInfoProtoList) do
    t.consortiaInfoList[k] = consortia_info.NewByGuildInfoProto(v)
  end
  table.sort(t.consortiaInfoList,function(a,b)
      return a.lv > b.lv
    end)
  
  t.alreadyApplyIdList = {}
  for k,v in ipairs(alreadyApplyList) do
    t.alreadyApplyIdList[v] = v
  end
end
--设置成员列表
function t.SetMemberInfoList(guildMemberProtoList)
  t.memberInfoList = {}
  for k,v in ipairs(guildMemberProtoList) do
    t.memberInfoList[k] = member_info.New(v)
  end
end
--设置申请列表
function t.SetGuildApplyList(guildMemberProtoList)
  t.guildApplyList = {}
  for k,v in ipairs(guildMemberProtoList) do
    t.guildApplyList[k] = member_info.New(v)
  end
end
---------------------------update by protocol-------------------
function t.onUpdateRefreshByProtocol()
  t.onUpdateRefreshDelegate:Invoke()
end
function t.onUpdateRefreshOneParamByProtocol(param)
  t.onUpdateRefreshOneParamDelegate:InvokeOneParam(param)
end
function t.UpdateCreateSuccessByProtocol()
  t.onUpdateCreateSuccessDelegate:Invoke()
end

function t.onUpdateExitSuccessByProtocol()
  t.onUpdateExitSuccessDelegate:Invoke()
end
function t.onUpdateKickedOutByProtocol()
  t.onUpdateKickedOutDelegate:Invoke()
end
function t.GuildPresentResp()
  t.onUpdateGuildPresentRespDelegate:Invoke()
end

function t.GuildShopInfoResp(rsp)
  t.hasBuy=rsp.hasBuy
  t.guildShopInfo={}
  for i=1,#rsp.guildShopInfo do
    table.insert(t.guildShopInfo,rsp.guildShopInfo[i])
  end

  table.sort(t.guildShopInfo,function (a,b)
    return a.type<b.type
  end)

  t.onUpdateGuildShopInfoDelegate:Invoke()
end

function t.GuildShopBuyResp()
  t.onUpdateGuildShopBuyRespDelegate:Invoke()
end

function t.mGuildShopBuyResp(delta)
  t.consortiaInfo.roleGuild.contribute=t.consortiaInfo.roleGuild.contribute-delta
end

function t.GuildShopSynInfoResp(resp)
  t.GuildShopInfoResp(resp)
  t.onUpdateGuildShopSynInfoRespDelegate:Invoke()
end

Start ()
return t