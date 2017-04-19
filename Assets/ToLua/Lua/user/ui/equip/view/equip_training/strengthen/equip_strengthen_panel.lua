local t = {}
local PREFAB_PATH = 'ui/equipments/strengthen/strengthen_panel'

local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local game_model = gamemanager.GetModel('game_model')
local equip_strengthen_data = gamemanager.GetData('equip_strengthen_data')

function t.Open (RightParent, equipInfo,trainingView)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(RightParent, false)
  t.trainingView = trainingView
  t.InitComponent()
  t.SetEquipInfo(equipInfo)
  
  t.BindDelegate()
end

function t.Close()
  if t.transform ~= nil then
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
    t.trainingView = nil
    t.UnbindDelegate()
  end
end

function t.BindDelegate()
  gamemanager.GetModel('equip_model').OnEquipmentStrengthenSuccessDelegate:AddListener(t.UpdateTrainingByProtocol)
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:AddListener(t.Refresh)
  game_model.onUpdateBaseResourceDelegate:AddListener(t.RefreshMoney)
end

function t.UnbindDelegate()
  gamemanager.GetModel('equip_model').OnEquipmentStrengthenSuccessDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:RemoveListener(t.Refresh)
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.RefreshMoney)
end

function t.InitComponent()
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.goRoot = t.transform:Find('root').gameObject
  t.btnStrengthen = t.transform:Find('root/btn_strengthen'):GetComponent(typeof(Button))
  t.btnStrengthen.onClick:AddListener(t.ClickStrengthenHandler)
  t.btnStrengthenMore = t.transform:Find('root/btn_strengthen_more'):GetComponent(typeof(Button))
  t.btnStrengthenMore.onClick:AddListener(t.ClickStrengthenMoreHandler)
  t.textStrengthenBtnMore = t.btnStrengthenMore.transform:Find('text_strengthen'):GetComponent(typeof(Text))
  t.textGold = t.transform:Find('root/text_money'):GetComponent(typeof(Text))
  t.textGoldMore = t.transform:Find('root/text_money_more'):GetComponent(typeof(Text))
  t.tranBefore = t.transform:Find('root/change_attr_root/before')
  t.tranAfter = t.transform:Find('root/change_attr_root/after')
  t.beforeStrengthenTable = {}
  t.beforeStrengthenTable.textLevel = t.tranBefore:Find('text_lv'):GetComponent(typeof(Text))
  t.beforeStrengthenTable.textPower = t.tranBefore:Find('text_power'):GetComponent(typeof(Text))
  t.beforeStrengthenTable.textName = t.tranBefore:Find('text_name'):GetComponent(typeof(Text))
  t.beforeStrengthenTable.textValue = t.tranBefore:Find('text_value'):GetComponent(typeof(Text))
  
  t.afterStrengthenTable = {}
  t.afterStrengthenTable.textLevel = t.tranAfter:Find('text_lv'):GetComponent(typeof(Text))
  t.afterStrengthenTable.textPower = t.tranAfter:Find('text_power'):GetComponent(typeof(Text))
  t.afterStrengthenTable.textName = t.tranAfter:Find('text_name'):GetComponent(typeof(Text))
  t.afterStrengthenTable.textValue = t.tranAfter:Find('text_value'):GetComponent(typeof(Text))
  
  t.tranIconRoot = t.transform:Find('root/icon_root/img_curIcon')
  t.textEquipName = t.transform:Find('root/icon_root/text_name'):GetComponent(typeof(Text))
  t.textEquipTip = t.transform:Find('root/icon_root/text_tip'):GetComponent(typeof(Text))
  
  t.goNoneTip = t.transform:Find('none_tip').gameObject
  t.textNoneTip = t.transform:Find('none_tip/text_tip'):GetComponent(typeof(Text))
  
  --effect
  t.goEffectZhuangbeiQiangHua = t.transform:Find('ui_zhuangbeiqianghua').gameObject
  particle_util.ChangeParticleSortingOrderByCanvas(t.goEffectZhuangbeiQiangHua,t.canvas)
  t.goEffectZhuangbeiQiangHua:SetActive(false)
end

function t.SetEquipInfo(equipInfo)
  t.curEquipIcon = nil
  t.equipInfo = equipInfo
  t.isClickStrengthen = false
  t.Refresh()
end

function t.Refresh()
  --max
  local isMax = t.equipInfo:IsMaxStrengthenLevel()
  if isMax then
    t.goNoneTip:SetActive(true)
    t.goRoot:SetActive(false)
    t.textNoneTip.text = LocalizationController.instance:Get('ui.equipment_training_view.maxLevelUp')
    local nextAccount = game_model.accountLevel + 1
    if game_model:IsAccountMaxLevel() then
      nextAccount = game_model.accountLevel
      t.textEquipTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.strengthen.strengthenNeedLevelTip'),nextAccount)
      return
    else
      t.textEquipTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.strengthen.strengthenNeedLevelTip'),nextAccount)
    end
  else
    t.textEquipTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.strengthen.strengthenTip'),t.equipInfo:GetMaxStrengthenLevel())
  end
  
  if t.curEquipIcon == nil then
    t.curEquipIcon = common_equip_icon.New(t.tranIconRoot)
  end
  t.curEquipIcon:SetEquipInfo(t.equipInfo)
  t.textEquipName.text = LocalizationController.instance:Get(t.equipInfo.data.name)
  
  --cur attr
  t.beforeStrengthenTable.textLevel.text = string.format('%d/%d',t.equipInfo.strengthenLevel,t.equipInfo:GetMaxStrengthenLevel())
  t.beforeStrengthenTable.textPower.text = t.equipInfo:Power()
  local totalBaseAttr = t.equipInfo:GetTotalBaseAttr()
  t.beforeStrengthenTable.textName.text = totalBaseAttr:GetName()
  t.beforeStrengthenTable.textValue.text = totalBaseAttr:GetValueString()
  --next attr
  local nextEquipInfo = t.equipInfo:NewBySelf()
  nextEquipInfo.strengthenLevel = nextEquipInfo.strengthenLevel + 1
  t.afterStrengthenTable.textLevel.text = string.format('%d/%d',nextEquipInfo.strengthenLevel,nextEquipInfo:GetMaxStrengthenLevel())
  t.afterStrengthenTable.textPower.text = nextEquipInfo:Power()
  totalBaseAttr = nextEquipInfo:GetTotalBaseAttr()
  t.afterStrengthenTable.textName.text = totalBaseAttr:GetName()
  t.afterStrengthenTable.textValue.text = totalBaseAttr:GetValueString()
  --show
  t.goRoot:SetActive(true)
  t.goNoneTip:SetActive(false)
  --money
  t.RefreshMoney()
end
function t.RefreshMoney()
  local own = game_model.GetBaseResourceValue(BaseResType.Gold)
  local need = t.GetNeedMoney(1)
  t.textGold.text = ui_util.GetEnoughColorText(need,own,need)
  ui_util.SetGrayChildren(t.btnStrengthen.transform,own < need ,true)
    local isGray = own < need or t.equipInfo:IsMaxStrengthenLevel()
  ui_util.SetGrayChildren(t.btnStrengthen.transform,isGray,true)
  t.btnStrengthen.enabled = not isGray
  
  local count = 10
  if t.equipInfo.strengthenLevel + count > t.equipInfo:GetMaxStrengthenLevel() then
    count = t.equipInfo:GetMaxStrengthenLevel() - t.equipInfo.strengthenLevel
    if count <= 1 then
      count = 10
    end
  end
  need = t.GetNeedMoney(count)
  t.textGoldMore.text =  ui_util.GetEnoughColorText(need,own,need)
  t.textStrengthenBtnMore.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.strengthen.btnStrengthenMore'),LocalizationController.instance:Get('common.num.'..count))
  isGray = own < need or t.equipInfo.strengthenLevel + count > t.equipInfo:GetMaxStrengthenLevel()
  ui_util.SetGrayChildren(t.btnStrengthenMore.transform, isGray,true)
  t.btnStrengthenMore.enabled = not isGray
end

function t.GetNeedMoney(count)
  return gamemanager.GetModel('equip_model').GetStrengthenMoney(t.equipInfo.data.strengthen_type,t.equipInfo.strengthenLevel,count)
end

-----------------click event-----------------

--点击强化
function t.ClickStrengthenHandler()
  t.StrengthenHandler(1)
end
--十次强化
function t.ClickStrengthenMoreHandler()
  t.StrengthenHandler(10)
end
--处理强化啦
function t.StrengthenHandler(count)
  local error_tip_view = require('ui/tips/view/common_error_tips_view')
  
  --max
  local isMax = t.equipInfo:IsMaxStrengthenLevel()
  if isMax then
    error_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.maxLevelUp"))
    return
  end
  local maxLevel = t.equipInfo:GetMaxStrengthenLevel()
  if t.equipInfo.strengthenLevel + count > maxLevel then
    count = maxLevel - t.equipInfo.strengthenLevel
  end
  -- material enough
  local own = game_model.GetBaseResourceValue(BaseResType.Gold)
  local need = t.GetNeedMoney(count)
  if own < need then
    error_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.strengthen.notEnoughGold"))
    return
  end
   if (t.isClickStrengthen) then
     return
    end
    t.isClickStrengthen = true
  --砸锤特效
  coroutine.start(function()
      AudioController.instance:PlayAudio( "equipAggr",false, 0)
      t.goEffectZhuangbeiQiangHua:SetActive(true)
      
      coroutine.wait(1.05)
      if t.transform == nil then
        return
      end
      t.goEffectZhuangbeiQiangHua:SetActive(false)
      t.trainingView.equip_attr_panel:ShowShuXingEffect()
      t.isClickStrengthen = false
      gamemanager.GetCtrl('equip_controller').EquipAggrReq(t.equipInfo.id,count)
    end)
  
end

-------------------------update by protocol--------------------
function t.UpdateTrainingByProtocol()
  --auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.strengthen.success'))
  t.Refresh()
end

return t