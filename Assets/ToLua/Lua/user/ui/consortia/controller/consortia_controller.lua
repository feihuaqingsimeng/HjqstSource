local t = {}
local name = 'consortia_controller'

require 'guild_pb'
require 'common_pb'
local common_auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local common_errors_tip_view = require('ui/tips/view/common_error_tips_view')

--公会自动通过更新
t.GuildAutoPassDelegate = void_delegate.New()

local function Start ()
  
  netmanager.RegisterProtocol(MSG.GuildResp, t.GuildResp)
  netmanager.RegisterProtocol(MSG.GuildListResp, t.GuildListResp)
  netmanager.RegisterProtocol(MSG.GuildMemberListResp, t.GuildMemberListResp)
  netmanager.RegisterProtocol(MSG.GuildReqListResp, t.GuildReqListResp)
  netmanager.RegisterProtocol(MSG.GuildCreateResp, t.GuildCreateResp)
  netmanager.RegisterProtocol(MSG.GuildDismissResp, t.GuildDismissResp)
  netmanager.RegisterProtocol(MSG.GuildAddResp, t.GuildAddResp)
  netmanager.RegisterProtocol(MSG.GuildAnswerResp, t.GuildAnswerResp)
  netmanager.RegisterProtocol(MSG.GuildKickResp, t.GuildKickResp)
  netmanager.RegisterProtocol(MSG.GuildExitResp, t.GuildExitResp)
  netmanager.RegisterProtocol(MSG.GuildNoticeResp, t.GuildNoticeResp)
  netmanager.RegisterProtocol(MSG.GuildSignResp, t.GuildSignResp)
  netmanager.RegisterProtocol(MSG.GuildPresentResp, t.GuildPresentResp)
  netmanager.RegisterProtocol(MSG.GuildSynResp, t.GuildSynResp)
  netmanager.RegisterProtocol(MSG.KickSynResp, t.KickSynResp)
  netmanager.RegisterProtocol(MSG.GuildShopInfoResp, t.GuildShopInfoResp)
  netmanager.RegisterProtocol(MSG.GuildShopBuyResp, t.GuildShopBuyResp)
  netmanager.RegisterProtocol(MSG.GuildShopSynInfoResp, t.GuildShopSynInfoResp)
  netmanager.RegisterProtocol(MSG.GuildAutoPassResp, t.GuildAutoPassResp)
  netmanager.RegisterProtocol(MSG.GuildAutoPassInfoResp, t.GuildAutoPassInfoResp)
  
  gamemanager.RegisterCtrl(name, t)
end

--------------------------protocol---------------------
t.isNotOpenGuildView = nil -- 返回消息后是否打开公会界面
--请求公会信息
function t.GuildReq(isNotOpenGuildView)
 -- if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Consortia,true) then
    netmanager.SendProtocol(MSG.GuildReq,nil)
    t.isNotOpenGuildView = isNotOpenGuildView
 -- end
  
end
--创建公会
function t.GuildCreateReq(name, headNo)
  local req = guild_pb.GuildCreateReq()
  req.name = name
  req.headNo = headNo
  netmanager.SendProtocol(MSG.GuildCreateReq,req)
end
--响应公会信息
t.defaultOpenConsortiaToggleIndex = 1
function t.GuildResp()
  local resp = guild_pb.GuildResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  print('响应公会信息')
  local consortia_model = gamemanager.GetModel('consortia_model')
  
  consortia_model.SetConsortiaInfo(resp.guildInfo)
  consortia_model.UpdateCreateSuccessByProtocol()
  if not t.isNotOpenGuildView then 
    t.OpenConsortiaInfoView(t.defaultOpenConsortiaToggleIndex)
    t.isNotOpenGuildView = false
  end
end
--响应公会列表
function t.GuildListResp()
  print('响应公会列表')
  local resp = guild_pb.GuildListResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('consortia_model').SetConsortiaList(resp.guildList,resp.alreadyApplys)
  if not t.isNotOpenGuildView then 
    t.OpenConsortiaListView ()
    t.isNotOpenGuildView = false
  end
  
end
--请求成员列表
function t.GuildMemberListReq()
  netmanager.SendProtocol(MSG.GuildMemberListReq,nil)
end
--响应成员列表
function t.GuildMemberListResp()
  print('响应成员列表')
  local resp = guild_pb.GuildMemberListResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local consortia_model = gamemanager.GetModel('consortia_model')
  consortia_model.SetMemberInfoList(resp.memberList)
  consortia_model.onUpdateRefreshOneParamByProtocol(1)
end
--请求申请列表
function t.GuildReqListReq()
  netmanager.SendProtocol(MSG.GuildReqListReq,nil)
end
--响应申请列表
function t.GuildReqListResp()
   print('响应申请列表')
  local resp = guild_pb.GuildMemberListResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local consortia_model = gamemanager.GetModel('consortia_model')
  consortia_model.SetGuildApplyList(resp.memberList)
  consortia_model.onUpdateRefreshOneParamByProtocol(2)
end
--响应创建公会
function t.GuildCreateResp()
    local cost = gamemanager.GetData('global_data').guild_creat_cost.count
    TalkingDataController.instance:TDGAItemOnPurchaseByCount("建公会",1,cost)
end
--请求解散公会
function t.GuildDismissReq()
  netmanager.SendProtocol(MSG.GuildDismissReq,nil)
end
--响应解散公会
function t.GuildDismissResp()
  print('公会解散了')
  common_errors_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.dismissSuc'))
  gamemanager.GetModel('consortia_model').onUpdateExitSuccessByProtocol()
  t.GuildReq()
end
--请求申请加入公会
function t.GuildAddReq(guildId)
  local req = common_pb.IntProto()
  req.value = guildId
  netmanager.SendProtocol(MSG.GuildAddReq,req)
end
--响应申请加入公会
function t.GuildAddResp()
  print('响应申请加入公会')
  local resp = common_pb.IntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local consortia_model = gamemanager.GetModel('consortia_model')
  consortia_model.alreadyApplyIdList[resp.value] = resp.value
  common_auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.applySend'))
  consortia_model.onUpdateRefreshByProtocol()
end
--请求批准加入公会 status 0 拒绝 1批准
function t.GuildAnswerReq(id,status)
  local req = common_pb.DoubleIntProto()
  req.value1 = id
  req.value2 = status
  netmanager.SendProtocol(MSG.GuildAnswerReq,req)
end
--响应批准加入公会
function t.GuildAnswerResp()
  print('响应批准加入公会')
  local resp = common_pb.DoubleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  if resp.value2 == 1 then
    t.GuildMemberListReq()
  end
  t.GuildReqListReq()
  
end
--请求踢出公会
function t.GuildKickReq(id)
  local req = common_pb.IntProto()
  req.value = id
  netmanager.SendProtocol(MSG.GuildKickReq,req)
end
--响应踢出公会
function t.GuildKickResp()
  print('响应踢出公会')
  common_auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.create_consortia_view.kickSuc'))
  t.GuildMemberListReq()
end
--请求退出公会
function t.GuildExitReq()
  netmanager.SendProtocol(MSG.GuildExitReq,nil)
end
--响应退出公会
function t.GuildExitResp()
  print('退出公会了')
  common_errors_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.exitSuc'))
  gamemanager.GetModel('consortia_model').onUpdateExitSuccessByProtocol()
  t.GuildReq()
end
--请求修改公告
t.guildNoticeNotify = ''
function t.GuildNoticeReq(str)
  local req = common_pb.StringProto()
  req.value = str
  netmanager.SendProtocol(MSG.GuildNoticeReq,req)
  t.guildNoticeNotify = str
end
--响应修改公告
function t.GuildNoticeResp()
  print('响应公告改了')
  local consortia_model = gamemanager.GetModel('consortia_model')
  consortia_model.consortiaInfo.notice = t.guildNoticeNotify
  consortia_model.onUpdateRefreshByProtocol()
end
--请求签到
function t.GuildSignReq()
  netmanager.SendProtocol(MSG.GuildSignReq,nil)
end
--响应签到
function t.GuildSignResp()
  print('响应签到了')
  common_errors_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.signSuc'))
  gamemanager.GetModel('consortia_model').onUpdateRefreshByProtocol()
end
--请求捐赠presentType:捐赠的类型
function t.GuildPresentReq(presentType)
  local req = common_pb.IntProto()
  req.value = presentType
  netmanager.SendProtocol(MSG.GuildPresentReq,req)
end
--请求商店信息
function t.GuildShopInfoReq()
  netmanager.SendProtocol(MSG.GuildShopInfoReq)
end
--请求商店购买
function t.GuildShopBuyReq(typeid)
  local req = common_pb.IntProto()
  req.value = typeid
  netmanager.SendProtocol(MSG.GuildShopBuyReq,req)
end

--响应捐赠
function t.GuildPresentResp()
  print('响应捐赠')
  gamemanager.GetModel('consortia_model').GuildPresentResp()
end
--同步公会信息
function t.GuildSynResp()
  local resp = guild_pb.GuildResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  print('同步公会信息',resp.guildInfo.no)
  local consortia_model = gamemanager.GetModel('consortia_model')
  consortia_model.SetConsortiaInfo(resp.guildInfo)
end
--相应被踢出公会
function t.KickSynResp()
  gamemanager.GetModel('consortia_model').onUpdateKickedOutByProtocol()
end

--响应公会商店
function t.GuildShopInfoResp()
  local resp = guild_pb.GuildShopInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())

  gamemanager.GetModel('consortia_model').GuildShopInfoResp(resp)
end

--响应公会商店购买
function t.GuildShopBuyResp()
  gamemanager.GetModel('consortia_model').GuildShopBuyResp()
end

--响应公会商店同步
function t.GuildShopSynInfoResp()
  local resp = guild_pb.GuildShopInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('consortia_model').GuildShopSynInfoResp(resp)
end
--请求设置公会自动通过
function t.GuildAutoPassReq(isOpen,limitPower,limitLevel)
  local req = guild_pb.GuildAutoPassReq()
  local open = 0
  if isOpen then
    open = 1
  end
  req.autoAgree = open
  req.autoAgreeLv = limitLevel
  req.autoAgreeCombat = limitPower
  netmanager.SendProtocol(MSG.GuildAutoPassReq,req)
end
--响应设置公会自动通过
function t.GuildAutoPassResp()
  
end
--请求公会自动通过信息
function t.GuildAutoPassInfoReq()
  netmanager.SendProtocol(MSG.GuildAutoPassInfoReq,nil)
end
--响应公会自动通过信息
function t.GuildAutoPassInfoResp()
  local resp = common_pb.TripleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  local isOpen = false
  if resp.value1 == 1 then
    isOpen = true
  end
  gamemanager.GetModel('consortia_model').consortiaInfo:SetAutoPassCondition(isOpen,resp.value2,resp.value3)
  print('GuildAutoPassInfoResp',isOpen,resp.value2,resp.value3)
  t.GuildAutoPassDelegate:Invoke()
end

-------------------------- open view -----------------------------------
function t.OpenConsortiaView (defaultToggleIndex)
  if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Consortia,true) then
    t.GuildReq()
    if defaultToggleIndex == nil then
      t.defaultOpenConsortiaToggleIndex = 1
    else
      t.defaultOpenConsortiaToggleIndex = defaultToggleIndex
    end
  end
 
  --[[local consortia_info = require('ui/consortia/model/consortia_info')
  gamemanager.GetModel('consortia_model').consortiaInfo = consortia_info.New(1)
  t.OpenConsortiaInfoView ()
  for i = 1, 20 do
    gamemanager.GetModel('consortia_model').consortiaInfoList[i] = consortia_info.New(i)
  end
  
  t.OpenConsortiaListView()]]
end
--没入会主界面
function t.OpenConsortiaListView()
  local view = dofile('ui/consortia/view/create/consortia_list_view')
  view.Open()
end
--创建公会
function t.OpenCreateConsortiaView ()
  local view = dofile('ui/consortia/view/create/create_consortia_view')
  view.Open()
end

--入会后的主界面
function t.OpenConsortiaInfoView(defaultToggleIndex)
  local view = dofile('ui/consortia/view/consortia_view')
  view.Open(defaultToggleIndex)
end
---公会战排行界面
function t.OpenConsortiaBattleRankView()
  local view = dofile('ui/consortia/view/battle/consortia_battle_rank_view')
  view.Open()
end
--公会战9路详情界面(--callback 点击上阵回调 。参数：点击index)
-- open view --
function t.OpenConsortiaBattleRoadDetailView(battleRoadInfo,callback)
  local view = dofile('ui/consortia/view/battle/consortia_battle_road_detail_view')
  view.Open(battleRoadInfo,callback)
  
end
--打开公会成员/设置界面
function t.OpenMemberSettingView()
  local view = dofile('ui/consortia/view/member/join_setting_view')
  view.Open()
end

Start ()
return t