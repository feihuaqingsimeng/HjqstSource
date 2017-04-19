local t = {}
local PREFAB_PATH = 'ui/equipments/up_star/up_star_maker_view'

  local item_data = gamemanager.GetData('item_data')
  local item_model = gamemanager.GetModel('item_model')

function t.Open(selectIndex,callback)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.selectIndex = selectIndex
  t.callback = callback
  t.InitComponent()
  t.Refresh()
  t.BindDelegate()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  if t.callback ~= nil then
    t.callback(t.selectIndex)
  end
end
function t.BindDelegate()
  gamemanager.GetModel('equip_model').OnStarGemComposeSuccessDelegate:AddListener(t.UpdateStarGemComposeSuccessByProtocol)
  item_model.updateItemInfoListDelegate:AddListener(t.Refresh)
end

function t.UnbindDelegate()
  gamemanager.GetModel('equip_model').OnStarGemComposeSuccessDelegate:RemoveListener(t.UpdateStarGemComposeSuccessByProtocol)
  item_model.updateItemInfoListDelegate:RemoveListener(t.Refresh)
end

function t.InitComponent()
  t.textTip = t.transform:Find('core/frame/img_tip/text_tip'):GetComponent(typeof(Text))
  t.materialItem = {}
  local item_root = t.transform:Find('core/frame/item_root')
  local childCount = item_root.childCount
  local common_item_icon = require('ui/common_icon/common_item_icon')
  for i = 1,childCount do
    local child = item_root:GetChild(i-1)
    t.materialItem[i] = {}
    t.materialItem[i].root = child
    t.materialItem[i].textName = child:Find('name'):GetComponent(typeof(Text))
    t.materialItem[i].itemIcon = common_item_icon.New(child)
    t.materialItem[i].itemIcon.onClick:AddListener(t.ClickMaterialItemHandler)
  end
  
  t.textAttr = t.transform:Find('core/frame/img_attr/text_attr'):GetComponent(typeof(Text))
  
  t.btnClose = t.transform:Find('core/frame/btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.ClickCloseHandler)
  t.btnMaker = t.transform:Find('core/frame/bottom_btn/btn_maker'):GetComponent(typeof(Button))
  t.btnMaker.onClick:AddListener(t.ClickMakerHandler)
  t.btnUse = t.transform:Find('core/frame/bottom_btn/btn_use'):GetComponent(typeof(Button))
  t.btnUse.onClick:AddListener(t.ClickUseHandler)
end

function t.Refresh()
  local gen_star = gamemanager.GetData('global_data').gen_star

  --item
  for k,v in pairs(t.materialItem) do
    local itemData = item_data.GetDataById(gen_star[k].item.id)
    t.materialItem[k].textName.text = LocalizationController.instance:Get(itemData.name)
    t.materialItem[k].itemIcon:SetGameResData(gen_star[k].item)
    local count = item_model.GetItemCountByItemID(gen_star[k].item.id)
    if count == 0 then
      t.materialItem[k].itemIcon:SetCount(tostring(count),Color(1,0,0))
    else
      t.materialItem[k].itemIcon:SetCount(tostring(count),Color(0,1,0))
    end
    if t.selectIndex == k then
      t.materialItem[k].itemIcon:SetSelect(true)
    else
      t.materialItem[k].itemIcon:SetSelect(false)
    end
  end
  --attr tip
  local des = LocalizationController.instance:Get('ui.equipment_training_view.star.attrAdd')
  local gen = gen_star[t.selectIndex]
  t.textAttr.text = ui_util.FormatToGreenText(string.format(des,gen.min,gen.max))
  --tip
  local gem_synthesis_data = gamemanager.GetData('gem_synthesis_data')
  local nextData = gem_synthesis_data.GetDataById(gem_synthesis_data.GetDataById(gen.item.id).NextId)
  if nextData == nil then
    t.textTip.text = LocalizationController.instance:Get('ui.equipment_training_view.star.hightestTip')
    t.btnMaker.gameObject:SetActive(false)
  else
    t.textTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.star.makeTip'),nextData.Material[1].count)
    t.btnMaker.gameObject:SetActive(true)
  end
end

---------------------click event-----------------------
function t.ClickMaterialItemHandler(itemIcon)
  
  for k,v in pairs(t.materialItem) do
    if itemIcon == v.itemIcon then
      t.selectIndex = k
      break
    end
  end
  t.Refresh()
  
  local itemGameRes = gamemanager.GetData('global_data').gen_star[t.selectIndex].item
  local count = item_model.GetItemCountByItemID(itemGameRes.id)
  if count == 0 then
    LuaCsTransfer.OpenGoodsJumpPath(itemGameRes.type,itemGameRes.id,itemGameRes.star)
  end
  
end

function t.ClickCloseHandler()
  t.Close()
end
function t.ClickMakerHandler()
  local gem_synthesis_data = gamemanager.GetData('gem_synthesis_data')
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
  
  local gen = gamemanager.GetData('global_data').gen_star[t.selectIndex]
  local nextData = gem_synthesis_data.GetDataById(gem_synthesis_data.GetDataById(gen.item.id).NextId)
  if nextData ~= nil then
    local count = nextData.Material[1].count
    local own = item_model.GetItemCountByItemID(gen.item.id)
    if own < count then
      common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.notEnoughMat'))
      return
    end
    gamemanager.GetCtrl('equip_controller').StarGemComposeReq(gen.item.id)
  end
  
end
function t.ClickUseHandler()
  local gen_star = gamemanager.GetData('global_data').gen_star
  local own = item_model.GetItemCountByItemID(gen_star[t.selectIndex].item.id)
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
  if  own == 0 then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.notUse'))
    return 
  end
   
  t.Close()
end

-----------------update by protocol------------------
function t.UpdateStarGemComposeSuccessByProtocol()
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
   common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.combineSuc'))
end

return t
