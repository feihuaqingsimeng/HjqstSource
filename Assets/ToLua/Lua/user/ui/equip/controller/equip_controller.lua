local t={}
local name='equip_controller'

require 'equip_pb'
require 'common_pb'

t.equipDecomposeSucDelegate = void_delegate.New()

local function Start( ... )
  --dofile('ui/equip/protocol/equip_protocol')
  netmanager.RegisterProtocol(MSG.GetAllEquipResp, t.GetAllEquipmentResp)
  netmanager.RegisterProtocol(MSG.EquipUpdateResp, t.EquipUpdateResp)
  netmanager.RegisterProtocol(MSG.EquipWearOffResp, t.EquipWearOffResp)
  netmanager.RegisterProtocol(MSG.EquipAggrResp, t.EquipAggrResp)
  netmanager.RegisterProtocol(MSG.EquipSellResp, t.EquipSellResp)
  netmanager.RegisterProtocol(MSG.EquipUpgradeResp, t.EquipUpgradeResp)
  netmanager.RegisterProtocol(MSG.EquipRecastResp, t.EquipRecastResp)
  netmanager.RegisterProtocol(MSG.EquipRecastAffirmResp, t.EquipRecastAffirmResp)
  netmanager.RegisterProtocol(MSG.EquipInlayGemResp, t.EquipInlayGemResp)
  netmanager.RegisterProtocol(MSG.InlayGemComposeResp, t.InlayGemComposeResp)
  netmanager.RegisterProtocol(MSG.InlayGemSlotUnlockResp, t.InlayGemSlotUnlockResp)
  netmanager.RegisterProtocol(MSG.EnchantingScrollComposeResp,t.EnchantingScrollComposeResp)
  netmanager.RegisterProtocol(MSG.EquipEnchantResp, t.EquipEnchantResp)
  netmanager.RegisterProtocol(MSG.StarGemComposeResp, t.StarGemComposeResp)
  netmanager.RegisterProtocol(MSG.EquipStarResp, t.EquipStarResp)
  netmanager.RegisterProtocol(MSG.EquipPieceComposeResp, t.EquipPieceComposeResp)
  netmanager.RegisterProtocol(MSG.EquipInheritResp, t.EquipInheritResp)
  netmanager.RegisterProtocol(MSG.EquipDeComposeResp, t.EquipDeComposeResp)

	gamemanager.RegisterCtrl(name,t)
	print('equip start!')
 
end

function t.GetEmptyTable ()
  emptyTable = {}
  return emptyTable
end

--------------------- req--------------------------------

function t.GetAllEquipmentsReq ()
  --if not equip_pb then require 'equip_pb' end
  local getAllEquipReq = equip_pb.GetAllEquipReq()
  netmanager.SendProtocol(MSG.GetAllEquipmentsReq, getAllEquipReq)
end

function t.EquipWearOffReq (instanceID, wearOffType, shouldDestroy, heroInstanceID)
 -- if not equip_pb then require 'equip_pb' end
  local equipWearOffReq = equip_pb.EquipWearOffReq()
  equipWearOffReq.equipId = instanceID
  equipWearOffReq.wearOrOff = wearOffType
  equipWearOffReq.isDestroy = shouldDestroy
  equipWearOffReq.heroId = heroInstanceID
  netmanager.SendProtocol(MSG.EquipWearOffReq, equipWearOffReq)
end

function t.EquipAggrReq (equipInstanceId, count)
  --if not equip_pb then require 'equip_pb' end
  local req = common_pb.DoubleIntProto()
  req.value1 = equipInstanceId
  req.value2 = count
  netmanager.SendProtocol(MSG.EquipAggrReq, req)
end

function t.EquipSellReq (equipId)
  local intproto = common_pb.IntProto()
  intproto.value = equipId
  netmanager.SendProtocol(MSG.EquipSellReq,intproto)
end
--strengthen
function t.EquipUpgradeReq(equipid)
  local intproto = common_pb.IntProto()
  intproto.value = equipid
  netmanager.SendProtocol(MSG.EquipUpgradeReq,intproto)
end
--重铸
function t.EquipRecastReq(equipid)
  local intproto = common_pb.IntProto()
  intproto.value = equipid
  netmanager.SendProtocol(MSG.EquipRecastReq,intproto)
end
--重铸确认
function t.EquipRecastAffirmReq(equipid,isUsed)
  local req = equip_pb.EquipRecastAffirmReq()
  req.equipId = equipid
  req.isUsed = isUsed
  netmanager.SendProtocol(MSG.EquipRecastAffirmReq,req)
end
--请求镶嵌宝石(第一个int:装备id，第二个int:槽的位置索引从0开始，第三个int:宝石编号(卸下就为0))
function t.EquipInlayGemReq(equipId,slotIndex,gemId)
  local req = common_pb.TripleIntProto()
  req.value1 = equipId
  req.value2 = slotIndex
  req.value3 = gemId
  netmanager.SendProtocol(MSG.EquipInlayGemReq,req)
  t.gemInsertSlot = slotIndex + 1 --客户端下标从1开始
end
--请求镶嵌宝石合成(合成的宝石编号,装备id,槽的位置索引从0开始)
function t.InlayGemComposeReq(gemid,equipId,slotIndex)
  local req = common_pb.TripleIntProto()
  req.value1 = gemid
  req.value2 = equipId  
  req.value3 = slotIndex

  netmanager.SendProtocol(MSG.InlayGemComposeReq,req)
end
--请求解锁宝石槽 装备id，第二个int:槽的位置索引,从0开始)
function t.InlayGemSlotUnlockReq(equipId,slot)
  local req = common_pb.DoubleIntProto()
  req.value1 = equipId
  req.value2 = slot
  netmanager.SendProtocol(MSG.InlayGemSlotUnlockReq,req)
end
--请求升星宝石合成
function t.StarGemComposeReq(gemId)
  local req = common_pb.IntProto()
  req.value = gemId
  netmanager.SendProtocol(MSG.StarGemComposeReq,req)
end
--请求升星或降星(第一个int:装备id，第二个int:升星or降星 1：升星，第三个int:升星宝石编号)
function t.EquipStarReq(equipid,starState,gemid)
  local req = common_pb.TripleIntProto()
  req.value1 = equipid
  req.value2 = starState
  req.value3 = gemid
  netmanager.SendProtocol(MSG.EquipStarReq,req)
end
--装备分解
function t.EquipDeComposeReq(decomposeId,instanceId,count)
  local req = common_pb.TripleIntProto()
  req.value1 = decomposeId
  req.value2 = instanceId
  req.value3 = count
  netmanager.SendProtocol(MSG.EquipDeComposeReq,req)
end

--装备分解
function t.EquipDeComposeReqByEquipInfo(equipInfo)
  if equipInfo == nil then
    return
  end
  local id = equipInfo.id
  local count = 1
  local decomposeData = gamemanager.GetData('equip_decompose_data').GetDataByTypeAndItemId(BaseResType.Equipment,equipInfo.data.id)
  t.EquipDeComposeReq (decomposeData.id,id,count)
end
--装备分解
function t.EquipDeComposeReqByEquipPiece(itemInfo)
  if itemInfo == nil then
    return
  end
  local id = itemInfo.itemData.id
  local count = itemInfo.count
  local decomposeData = gamemanager.GetData('equip_decompose_data').GetDataByTypeAndItemId(BaseResType.Item,id)
  t.EquipDeComposeReq (decomposeData.id,id,count)
end

--相应装备分解
function t.EquipDeComposeResp()
  t.equipDecomposeSucDelegate:Invoke()
  require('ui/tips/view/auto_destroy_tip_view').Open(LocalizationController.instance:Get('ui.pack_view.decomposeSuc'))
end
--------------------- resp--------------------------------

function t.GetAllEquipmentResp ()
  --if not equip_pb then require 'equip_pb' end
  local getAllEquipResp = equip_pb.GetAllEquipResp()
  getAllEquipResp:ParseFromString(netmanager.GetProtocolData())
  
  local length = #getAllEquipResp.equips
  if length == 0 then
    print('=====================there is no equip')
  else
   local equip_info = require ('ui/equip/model/equip_info') 
    for i = 1, length do
      local equipInfo = equip_info:NewByEquip(getAllEquipResp.equips[i])
      gamemanager.GetModel('equip_model').AddEquipmentInfo(equipInfo, false)
    end
  end
end

function t.EquipUpdateResp ()
  --if not equip_pb then require 'equip_pb' end 
  local equipUpdateResp = equip_pb.EquipUpdateResp()
  equipUpdateResp:ParseFromString(netmanager.GetProtocolData())
  
  local addEquips = equipUpdateResp.addEquips
  local delEquips = equipUpdateResp.delEquips
  local updateEquips = equipUpdateResp.updateEquips
  
  gamemanager.GetModel("equip_model").UpdateEquipmentInfoList(addEquips, delEquips, updateEquips)
end

function t.EquipWearOffResp ()
  --if not equip_pb then require 'equip_pb' end
  local equipWearOffResp = equip_pb.EquipWearOffResp();
  equipWearOffResp:ParseFromString(netmanager.GetProtocolData())
  
  gamemanager.GetModel("equip_model").OnEquipmentInfoListUpdate()
end

function t.EquipAggrResp ()

  gamemanager.GetModel('equip_model').OnEquipmentStrengthenSuccess ()
end

function t.EquipSellResp ()
 -- if not equip_pb then require 'equip_pb' end
  local equipSellResp = common_pb.IntProto();
  equipSellResp:ParseFromString(netmanager.GetProtocolData())
  
  gamemanager.GetModel("equip_model").RemoveEquipmentInfo (equipSellResp.value)
  gamemanager.GetModel("equip_model").OnEquipmentInfoListUpdate()
end
--强化、进阶
function t.EquipUpgradeResp()
   local equip_model = gamemanager.GetModel("equip_model")
   equip_model.UpdateTrainingByProtocol()
end
--重铸
function t.EquipRecastResp()
  local equipRecastResp = equip_pb.EquipRecastResp();
  equipRecastResp:ParseFromString(netmanager.GetProtocolData())
  
  local equip_model = gamemanager.GetModel("equip_model")
  equip_model.SetEquipRecastAttrList(equipRecastResp.randomAttrs)
  equip_model.isRecast = true
  equip_model.UpdateTrainingByProtocol()
end
--重铸确认
function t.EquipRecastAffirmResp()
  local equip_model = gamemanager.GetModel("equip_model")
  equip_model.SetEquipRecastAttrList(nil)
  equip_model.isRecast = false
  equip_model.UpdateTrainingByProtocol()
end
--响应镶嵌宝石
function t.EquipInlayGemResp()
  local equip_model = gamemanager.GetModel("equip_model")
  print('响应镶嵌宝石slot:',t.gemInsertSlot)
  equip_model.OnEquipGemInsertSuccessByProtocol(t.gemInsertSlot)
end
--响应镶嵌宝石合成
function t.InlayGemComposeResp()
  local equip_model = gamemanager.GetModel("equip_model")
  print('响应镶嵌宝石合成')
  equip_model.UpdateTrainingByProtocol()
end
--响应解锁宝石槽
function t.InlayGemSlotUnlockResp()
  local equip_model = gamemanager.GetModel("equip_model")
  print('响应解锁宝石槽')
  equip_model.UpdateTrainingByProtocol()
end

--[[制作附魔卷轴]]--
function t.EnchantingScrollComposeReq (scrollItemId)
  local intproto = common_pb.IntProto()
  intproto.value = scrollItemId
  netmanager.SendProtocol(MSG.EnchantingScrollComposeReq, intproto)
end

function t.EnchantingScrollComposeResp ()
  print('t.EnchantingScrollComposeResp')
  
  local enchantingScrollComposeResp = common_pb.IntProto()
  enchantingScrollComposeResp:ParseFromString(netmanager.GetProtocolData())
  
  local equip_model = gamemanager.GetModel("equip_model").OnEnchantingScrollComposeSuccess()
end
--[[制作附魔卷轴]]--

--[[装备附魔]]--
function t.EquipEnchantReq(equipId, scrollItemId)
  local doubleProto = common_pb.DoubleIntProto()
  doubleProto.value1 = equipId
  doubleProto.value2 = scrollItemId
  netmanager.SendProtocol(MSG.EquipEnchantReq, doubleProto)
end

function t.EquipEnchantResp()
  print('t.EquipEnchantResp')
  
  local equip_model = gamemanager.GetModel("equip_model").OnEquipEnchantSuccess()
end
--[[装备附魔]]--
--响应升星宝石合成
function t.StarGemComposeResp()
  local equip_model = gamemanager.GetModel("equip_model")
  print('响应升星宝石合成')
  equip_model.UpdateStarGemComposeSuccessByProtocol()
end
--响应升星或降星
function t.EquipStarResp()
  local equip_model = gamemanager.GetModel("equip_model")
  print('响应升星或降星')
  equip_model.UpdateTrainingByProtocol()
end

-- [[ 装备碎片合成 ]] --
function t.EquipPieceComposeReq (equipPieceID)
  local intProto = common_pb.IntProto()
  intProto.value = equipPieceID
  netmanager.SendProtocol(MSG.EquipPieceComposeReq, intProto)
end

function t.EquipPieceComposeResp ()
  local intProto = common_pb.IntProto()
  intProto:ParseFromString(netmanager.GetProtocolData())
  
  local equipPieceData = gamemanager.GetData('piece_data').GetDataById(intProto.value)
  local equipGameResData = equipPieceData:GetEquipGameResData ()
  
  local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
  local equipData = gamemanager.GetData('equip_data').GetDataById(equipGameResData.id)
  local ui_util = require 'util/ui_util'
  local equipNameWithColor =  ui_util.FormatStringWithinQualityColor (equipData.quality, LocalizationController.instance:Get(equipData.name))
  auto_destroy_tip_view.Open(string.format(LocalizationController.instance:Get('common.tips.hero_piece_compose_success'), equipNameWithColor))
end
-- [[ 装备碎片合成 ]] --

--请求继承(type: 继承类型（0:全部继承1：星级2：等级）)

function t.EquipInheritReq(equipId,materialEquipId,hasStar,hasStrengthen)
  local inheritType = -1
  if hasStar and hasStrengthen then
    inheritType = 0
  elseif hasStar then
    inheritType = 1
  elseif hasStrengthen then
    inheritType = 2
  end
  print('inheritType',inheritType)
  local req = equip_pb.EquipInheritReq()
  req.equipId = equipId
  req.materialEquipId = materialEquipId
  req.type = inheritType
  netmanager.SendProtocol(MSG.EquipInheritReq, req)
end
--相应继承
function t.EquipInheritResp()
  print('响应继承')
  local equip_model = gamemanager.GetModel("equip_model")
  equip_model.OnEquipInheritSuccessDelegate:Invoke()
end

function t.ExpandEquipmentCell()
  local game_model = gamemanager.GetModel('game_model')
  local global_data = gamemanager.GetData('global_data')
  if game_model.equipCellNum >= global_data.equip_package_max_num then
    local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.common_tips.equipment_bag_reach_max'))
    return
  end
  
  local cost = (game_model.equipCellBuyNum + 1) * global_data.equip_package_buy_a + global_data.equip_package_buy_b
  local common_expand_bag_tip_view = require('ui/tips/view/common_expand_bag_tip_view')
  common_expand_bag_tip_view.Open2(2,cost)
end

Start()
return t
