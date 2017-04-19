local t = {}
local name = 'vip_controller'
require 'vip_pb'

local vipModel = gamemanager.GetModel('vip_model')

t.lastDrawBenefitsVIPLevel = -1

local function Start ()
  netmanager.RegisterProtocol(MSG.VipInfoResp, t.VipInfoResp)
  netmanager.RegisterProtocol(MSG.VipGiftBagResp, t.VipGiftBagResp)
  
  gamemanager.RegisterCtrl(name,t)
end

function t.VipInfoReq ()
  local req = vip_pb.VIPInfoReq()
  netmanager.SendProtocol(MSG.VIPInfoReq, req)
end

function t.VipInfoResp ()
  print('========== VipInfoResp ==========')
  local resp = vip_pb.VipInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  vipModel.UpdateVIPInfo(resp.dailyGiftIsGet, resp.totalRecharge, resp.getGiftBagLvList)
end

function t.VipGiftBagReq (vipLevel)
  local req = vip_pb.VipGiftBagReq()
  req.vipLv = vipLevel
  t.lastDrawBenefitsVIPLevel = vipLevel
  netmanager.SendProtocol(MSG.VipGiftBagReq, req)
end

function t.VipGiftBagResp ()
  print('========== VipInfoResp ==========')
  local resp = vip_pb.VipGiftBagResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  vipModel.DrawVIPBenefitsSuccess(t.lastDrawBenefitsVIPLevel)
end

function t.OpenVIPView ()
  local vip_view = require('ui/vip/view/vip_view')
  vip_view.Open()
end

Start ()
return t