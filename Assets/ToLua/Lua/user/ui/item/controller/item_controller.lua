local t = {}
local name = 'item_controller'

require 'item_pb'

local function Start()
  netmanager.RegisterProtocol(MSG.GetAllItemResp,t.GetAllItemResp)
  netmanager.RegisterProtocol(MSG.ItemUpdateResp,t.ItemUpdateResp)
  netmanager.RegisterProtocol(MSG.ExpPotionResp,t.ExpPotionResp)
  netmanager.RegisterProtocol(MSG.OpenGiftBagResp,t.OpenGiftBagResp)
  gamemanager.RegisterCtrl(name,t)
end

--------------------req-----------------
--经验药水
function t.ExpPotionReq(heroid, potionNo, num)
  local req = item_pb.ExpPotionReq()
  req.heroId = heroid
  req.potionNo = potionNo
  req.num = num
  netmanager.SendProtocol(MSG.ExpPotionReq, req)
end

-------------------resp----------------------------------
function t.GetAllItemResp()
  
  local resp = item_pb.GetAllItemResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local item_info = require('ui/item/model/item_info')
  local item_model = gamemanager.GetModel('item_model')
  
  for k,v in ipairs(resp.items) do
     item_model.AddItemInfo(item_info.NewByItemProtoData(v), false)
  end
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler')
end

function t.ItemUpdateResp()
  local resp = item_pb.ItemUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local item_info = require('ui/item/model/item_info')
  local item_model = gamemanager.GetModel('item_model')
  --add
  for k,v in ipairs(resp.addItems) do 
    item_model.AddItemInfo(item_info.NewByItemProtoData(v), true)
    --print('add item..............',v.id)
  end
  --remove
  local tempInfo = nil
  for k,v in ipairs(resp.delItems) do
    tempInfo = item_model.GetItemInfoByInstanceID(v)
    item_model.RemoveItemInfo(v)
    TalkingDataController.instance:TDGAItemOnUse(tempInfo.itemData.id,tempInfo:Count(),BaseResType.Item)
    --print('remove item..............',v)
  end
  --update
  local tempCount = 0
  for k,v in ipairs(resp.updateItems) do
    local info = item_model.GetItemInfoByInstanceID(v.id)
    tempCount = info:Count()
    info:Update(v)
    --print('update item...................',info.instanceId,info.count)
    if tempCount > info:Count() then
      TalkingDataController.instance:TDGAItemOnUse(info.itemData.id,tempCount-info:Count(),BaseResType.Item)
    end
  end
  item_model.updateItemInfoListByProtocol()
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_ITEM_UPDATE_RESP_handler')
  gamemanager.GetModel('hero_model').CheckHasAdvanceBreakthroughHeroByRedPoint()
end

function t.ExpPotionResp()
  local resp = item_pb.ExpPotionResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local item_model = gamemanager.GetModel('item_model')
  item_model.updateExpPotionByProtocol(resp.potionNo)
end

function t.OpenGiftBagReq(itemid)
  local req = item_pb.OpenGiftBagReq()
  req.giftBagNo = itemid
  netmanager.SendProtocol(MSG.OpenGiftBagReq, req)
end

function t.OpenGiftBagResp()
  local resp = item_pb.OpenGiftBagResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local dataList = {}
  for k,v in ipairs(resp.dropItems) do
    dataList[k] = require('ui/game/model/game_res_data').New(v.itemType,v.itemNo,v.itemNum,v.heroStar)
  end
  require('ui/tips/view/common_reward_tips_view').Create(dataList)
end

Start()

return t