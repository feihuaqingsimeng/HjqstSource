local t = {}
local PREFAB_PATH = 'ui/equipments/equipment_training_view'
local name = PREFAB_PATH

local equip_model = gamemanager.GetModel('equip_model')
local hero_model = gamemanager.GetModel('hero_model')
local game_model = gamemanager.GetModel('game_model')

local equip_recast_panel = dofile('ui/equip/view/equip_training/recast/equip_recast_panel')
local up_star_panel = dofile('ui/equip/view/equip_training/up_star/up_star_panel')
local equip_strengthen_panel = dofile('ui/equip/view/equip_training/strengthen/equip_strengthen_panel')
local equip_gem_insert_panel = dofile('ui/equip/view/equip_training/gem_insert/equip_gem_insert_panel')
local inherit_panel = dofile('ui/equip/view/equip_training/inherit/inherit_panel')

local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

local OpenType = {}
OpenType.role = 1
OpenType.equipBag =2

--英雄装备循环
function t.OpenByRoleInfo(heroInstanceId,selectEquipInstanceId,toggleIndex)
  
  uimanager.RegisterView(name,t)
  
  t.common_top_bar_title = LocalizationController.instance:Get('ui.equipment_training_view.heroTitle')
  local roleInfo = nil
  if game_model.IsPlayer(heroInstanceId) then
    roleInfo = game_model.playerInfo
  else
    roleInfo = hero_model.GetHeroInfo(heroInstanceId)
  end
  t.openType = OpenType.role
  t.equipInfoList = {}
  if roleInfo.weaponID > 0 then
    table.insert(t.equipInfoList,equip_model.GetEquipmentInfoByInstanceID(roleInfo.weaponID))
  end
  if roleInfo.armorID > 0 then
    table.insert(t.equipInfoList,equip_model.GetEquipmentInfoByInstanceID(roleInfo.armorID))
  end
  if roleInfo.accessoryID > 0 then
    table.insert(t.equipInfoList,equip_model.GetEquipmentInfoByInstanceID(roleInfo.accessoryID))
  end
   t.Open(selectEquipInstanceId,toggleIndex)
end
--背包装备循环
function t.OpenByEquipBag(selectEquipInstanceId,toggleIndex)

  uimanager.RegisterView(name,t)
  
  t.common_top_bar_title = LocalizationController.instance:Get('ui.equipment_training_view.bagTitle')
  t.openType = OpenType.equipBag
  t.InitBagEquipList()
  t.Open(selectEquipInstanceId,toggleIndex)
end

--please do not call this method ,this is used to inside,please do not call ,pleasedonotcallpleasedonotcallpleasedonotcallpleasedonotcall
function t.Open(equipInstanceId,toggleIndex)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(t.common_top_bar_title,t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.InitComponent()
  
  t.equipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(equipInstanceId)
  t.curToggleIndex = 0
  t.selectEquipInfoIndex = 0
  for k,v in pairs(t.equipInfoList) do
    if v.id == equipInstanceId then
      t.selectEquipInfoIndex = k
      break
    end
  end
  print('t.selectEquipInfoIndex',t.selectEquipInfoIndex)
  
  t.functionOpenTypeTable = {}
  t.functionOpenTypeTable[1] = FunctionOpenType.EquipTraining
  t.functionOpenTypeTable[2] = FunctionOpenType.EquipRecast
  t.functionOpenTypeTable[3] = FunctionOpenType.EquipUpStar
  t.functionOpenTypeTable[4] = FunctionOpenType.EquipGemInsert
  t.functionOpenTypeTable[5] = FunctionOpenType.EquipInherit
  
  local equip_attr_panel = dofile('ui/equip/view/equip_attr_panel')
 
  t.equip_attr_panel = equip_attr_panel.OpenByGameObject(t.transform:Find('core/left/attr_root/equip_attr_panel').gameObject)
  t.equip_attr_panel:SetEquipInfo(t.equipInfo)
  
  if gamemanager.GetModel('function_open_model').IsFunctionOpen(t.functionOpenTypeTable[toggleIndex],true) then
    t.SetToggleOn(toggleIndex)
  else
    t.SetToggleOn(1)
  end
  
  t.BindDelegate()
end

function t.Close()
  
  if t.functionPanel ~= nil then
    t.functionPanel.Close()
    t.functionPanel = nil
  end
  t.equip_attr_panel:Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate()
  equip_model.UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipGemInsertSuccessDelegate:AddListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipEnchantSuccessDelegate:AddListener(t.OnEquipEnchantSuccessHandler)
  equip_model.OnEquipmentInfoUpdateDelegate:AddListener(t.UpdateEquipmentInfoListByProtocol)
  equip_model.OnEquipInheritSuccessDelegate:AddListener(t.UpdateEquipInheritSucByProtocol)
end

function t.UnbindDelegate()
  equip_model.UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipEnchantSuccessDelegate:RemoveListener(t.OnEquipEnchantSuccessHandler)
  equip_model.OnEquipGemInsertSuccessDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipmentInfoUpdateDelegate:RemoveListener(t.UpdateEquipmentInfoListByProtocol)
  equip_model.OnEquipInheritSuccessDelegate:RemoveListener(t.UpdateEquipInheritSucByProtocol)
end


function t.InitComponent()
  local left = t.transform:Find('core/left')
  t.tranIconRoot = left:Find('img_cur_equip_root')
  
  t.btn_right = left:Find('attr_root/equip_attr_panel/btn_right'):GetComponent(typeof(Button))
  t.btn_right.onClick:AddListener(t.ClickRightChoiceHandler)
  t.btn_left = left:Find('attr_root/equip_attr_panel/btn_left'):GetComponent(typeof(Button))
  t.btn_left.onClick:AddListener(t.ClickLeftChoiceHandler)
  
  local other_attr_root = left:Find('attr_root')
  t.tranRandomAttr = other_attr_root:Find('random_attr_root')
  t.tranDiamondAttr = other_attr_root:Find('diamond_attr_root')
  t.tranEnchantAttr = other_attr_root:Find('enchant_attr_root')
  
  local right = t.transform:Find('core/right')
  t.toggles = {}
  t.toggles[1] = right:Find('toggle_group/toggle_strengthen')
  t.toggles[2] = right:Find('toggle_group/toggle_recast')
  t.toggles[3] = right:Find('toggle_group/toggle_upStar')
  t.toggles[4] = right:Find('toggle_group/toggle_diamond_insert')
  t.toggles[5] = right:Find('toggle_group/toggle_inherit')

  for k,v in ipairs(t.toggles) do
    local trigger = v.transform:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    trigger.onClick:AddListener(t.ClickToggleHandler)
  end
  
  t.functionParent = right:Find('training_function_root')
  
end

function t.InitBagEquipList()
  t.equipInfoList = equip_model.GetFreeEquipmentInfoList()
end

function t.Refresh()
  t.equip_attr_panel:SetEquipInfo(t.equipInfo)
  if t.functionPanel ~= nil then
    t.functionPanel.SetEquipInfo(t.equipInfo)
  end  
end
--判断是否重铸了，但没电二次确认
function t.CheckRecastOperate()
  if gamemanager.GetModel('equip_model').isRecast then
    local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
    confirm_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.recast_not_save"),t.ConfirmRecastOperateHandler)
    return true
  end
  return false
end
--------------------click event-----------------

function t.ConfirmRecastOperateHandler()
  gamemanager.GetCtrl('equip_controller').EquipRecastAffirmReq(t.equipInfo.id,false)
end

function t.ClickToggleHandler(gameObject)
  local index = 0
  for k,v in ipairs(t.toggles) do
    if v.gameObject == gameObject then
      index = k
      break
    end
  end
  if t.curToggleIndex == index then
    return
  end
  --check
  if t.CheckRecastOperate() then
    t.toggles[t.curToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
    t.toggles[index]:GetComponent(typeof(Toggle)).isOn = false
    return
  end
  --open function 
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(t.functionOpenTypeTable[index],true) then
    t.toggles[t.curToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
    t.toggles[index]:GetComponent(typeof(Toggle)).isOn = false
    return 
  end
  ----------------------
  t.curToggleIndex = index
  
  if t.functionPanel ~= nil then
    t.functionPanel.Close()
  end
  
  t.equip_attr_panel:HideAllTip()
  t.equip_attr_panel:ClearInheritEquipInfo()
  
  if t.curToggleIndex == 1 then--强化
    t.functionPanel = equip_strengthen_panel
    t.functionPanel.Open(t.functionParent, t.equipInfo,t)
    t.equip_attr_panel:ShowMainTip(true,nil)
  elseif t.curToggleIndex == 2 then--重铸
    t.functionPanel = equip_recast_panel
    t.functionPanel.Open(t.functionParent, t.equipInfo,t)
    t.equip_attr_panel:ShowRandomTip(true,1,nil)
    t.equip_attr_panel:ShowRandomTip(true,2,nil)
  elseif t.curToggleIndex == 3 then --升星
    t.functionPanel = up_star_panel
    t.functionPanel.Open( t.functionParent, t.equipInfo,t)
    t.equip_attr_panel:ShowMainTip(true,nil)
  elseif t.curToggleIndex == 4 then --镶嵌
    t.functionPanel = equip_gem_insert_panel
    t.functionPanel.Open(t.functionParent, t.equipInfo,t)
    --t.equip_attr_panel:ShowGemTip(true,1,nil)
    --t.equip_attr_panel:ShowGemTip(true,2,nil)
  elseif t.curToggleIndex == 5 then --继承
    t.functionPanel = inherit_panel
    t.functionPanel.Open(t.functionParent, t.equipInfo,
      function(equipInfo,isInheritLevel,isInheritStar) 
        t.equip_attr_panel:SetInheritEquipInfo(equipInfo,isInheritLevel,isInheritStar)
        end)
  end
end
function t.SetToggleOn(index)
  if index > #t.toggles or index <= 0 then
    return
  end
  t.toggles[index]:GetComponent(typeof(Toggle)).isOn = true
  t.ClickToggleHandler(t.toggles[index].gameObject)
end

function t.OnClickBackBtnHandler()
  if t.CheckRecastOperate()  then
    return
  end
  uimanager.CloseView(name)
  
end
--右
function t.ClickRightChoiceHandler()
  if t.CheckRecastOperate() then
    return
  end
  t.selectEquipInfoIndex  = t.selectEquipInfoIndex + 1
  local maxCount = #t.equipInfoList
  if t.selectEquipInfoIndex > maxCount then
    t.selectEquipInfoIndex = 1
  end
  t.equipInfo = t.equipInfoList[t.selectEquipInfoIndex]
  t.Refresh()
end
--左
function t.ClickLeftChoiceHandler()
  if t.CheckRecastOperate() then
    return
  end
  t.selectEquipInfoIndex  = t.selectEquipInfoIndex - 1
  local maxCount = #t.equipInfoList
  if t.selectEquipInfoIndex <= 0 then
    t.selectEquipInfoIndex = maxCount
  end
  t.equipInfo = t.equipInfoList[t.selectEquipInfoIndex]
  t.Refresh()
end
------------------------update by protocol----------------- 
--列表改变
function t.UpdateEquipmentInfoListByProtocol()
  if t.openType == OpenType.equipBag then -- 装备背包列表更新了
    t.InitBagEquipList()
  end
end
function t.UpdateTrainingByProtocol()
  t.Refresh()
end


function t.OnEquipEnchantSuccessHandler ()
  t.equip_attr_panel:Refresh()
  --t.equip_attr_panel:ShowShuXingEffect()
end
function t.UpdateEquipInheritSucByProtocol()
  t.equip_attr_panel:ClearInheritEquipInfo()
   t.Refresh()
  t.equip_attr_panel:ShowEquipIconEffect()
end

return t

