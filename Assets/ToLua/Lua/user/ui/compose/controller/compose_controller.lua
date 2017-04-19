local t = {}
local name = 'compose_controller'
require 'equip_pb'
require 'common_pb'
local equipView = nil

local function Start()
  netmanager.RegisterProtocol(MSG.EquipComposeResp, t.EquipComposeResp)
  gamemanager.RegisterCtrl(name,t)
  equipView =dofile("ui/compose/view/equip_compose_view")
end

function t.OpenEquipCompose()
  equipView.Open()
end

--Req
local tempComposeDiamondCost = 0
function t.EquipComposeReq(qualityId,equipTab,composeType)
  local req = equip_pb.EquipComposeReq()
  print("qualityId:",qualityId)
  req.qualityId=qualityId
  for k,v in pairs(equipTab) do
    print("id:",v)
    table.insert(req.equip,v)
    --req.equip:Add(v)
  end
  req.currencyType= composeType
  print("req.currencyType:",req.currencyType)
  netmanager.SendProtocol(MSG.EquipComposeReq,req)
  if composeType == 1 then
    tempComposeDiamondCost = gamemanager.GetData('equip_compose_data').GetDataByQuality(qualityId).need_diamond_cost.count
  else
    tempComposeDiamondCost = 0
  end
end

--Resp
function t.EquipComposeResp()
  
  local resp = common_pb.DoubleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  local composeModel = gamemanager.GetModel('compose_model')
  composeModel.UpdateComposeSuccess(resp.value1,resp.value2)
  print("compose return:",resp.value1,resp.value2)
  --talkingData--
  
  if tempComposeDiamondCost ~= 0 then
      local equipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(resp.value2)
      if equipInfo then
        TalkingDataController.instance:TDGAItemOnPurchase('合成',BaseResType.Equipment,equipInfo.data.id,1,tempComposeDiamondCost)
      end
  end
  --end--
end
Start()

return t