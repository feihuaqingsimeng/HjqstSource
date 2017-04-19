local t = {}
local name = "game_controller"
require 'login_pb'
require 'pack_pb'
require 'pve_pb'

local function Start ()
  netmanager.RegisterProtocol(MSG.PackResp, t.PackResp)
  netmanager.RegisterProtocol(MSG.BuyPackCellResp, t.BuyPackCellResp)
  
  netmanager.RegisterProtocol(MSG.RoleLvAndExpResp, t.RoleLvAndExpResp)
  netmanager.RegisterProtocol(MSG.BaseResourceSyn, t.BaseResourceSyn)



  gamemanager.RegisterCtrl(name,t)

end

-------------------------- pack -----------------------
--背包数
function t.PackResp ()
  local packResp = pack_pb.PackResp()
  packResp:ParseFromString(netmanager.GetProtocolData())
  local game_model = gamemanager.GetModel('game_model')
  game_model.UpdateHeroCellNum(packResp.heroCellNum, packResp.heroCellBuyNum)
  game_model.UpdateEquipcellNum(packResp.equipCellNum, packResp.equipCellBuyNum)
  local s = string.format('%d,%d,%d,%d',packResp.heroCellNum, packResp.heroCellBuyNum,packResp.equipCellNum, packResp.equipCellBuyNum)
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_PACK_RESP_handler',s)
end

function t.BuyPackCellReq (packType)
  local buyPackCellReq = pack_pb.BuyPackCellReq()
  buyPackCellReq.packType = packType
  netmanager.SendProtocol(MSG.BuyPackCellReq, buyPackCellReq)
end
--背包购买次数
function t.BuyPackCellResp ()
  local buyPackCellResp = pack_pb.BuyPackCellResp()
  buyPackCellResp:ParseFromString(netmanager.GetProtocolData())
  local game_model = gamemanager.GetModel('game_model')
  if buyPackCellResp.packType == 1 then       --1代表英雄背包
    game_model.UpdateHeroCellNum(buyPackCellResp.cellNum, game_model.heroCellBuyNum + 1)
  elseif buyPackCellResp.packType == 2 then   --2代表装备背包
    game_model.UpdateEquipcellNum(buyPackCellResp.cellNum, game_model.equipCellBuyNum + 1)
  end
  local s = string.format('%d,%d',buyPackCellResp.packType,buyPackCellResp.cellNum)
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_BUY_PACK_CELL_RESP',s)
end
-------------------------- pack -----------------------

--------------------------resp------------------------
function t.RoleLvAndExpResp()
  local resp = login_pb.RoleLvAndExpResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local game_model = gamemanager.GetModel('game_model')
  game_model.UpdateAccountLevelAndExp(resp.lv,resp.exp)
end

function t.BaseResourceSyn()
  local resp = login_pb.BaseResourceSyn()
  resp:ParseFromString(netmanager.GetProtocolData())
  local game_model = gamemanager.GetModel('game_model')
  game_model.OnBaseResourcesUpdate(resp.resourceInfos)
  
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_BASE_RESOURCE_SYN_handler')
end
--请求 pve 行动力
function t.SynPveActionReq()
  local req = pve_pb.SynPveActionReq()
  netmanager.SendProtocol(MSG.SynPveActionReq, req)
end
------------------------------open view---------------------
function t.OpenMainView()
  local view = dofile('ui/main_view/view/main_view')
  view.Open()
end


Start()
return t
