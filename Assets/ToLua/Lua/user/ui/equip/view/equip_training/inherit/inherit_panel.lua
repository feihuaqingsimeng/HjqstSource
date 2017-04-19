local t = {}
local PREFAB_PATH = 'ui/equipments/inherit/inherit_panel'

local equip_model = gamemanager.GetModel('equip_model')
local equip_controller = gamemanager.GetCtrl('equip_controller')
local game_model = gamemanager.GetModel('game_model')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

--clickInheritEquipItemCallback param: equipInfo，isInheritLevel,isInheritStar

function t.Open (rightParent ,equipInfo,InheritEquipItemCallback)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(rightParent, false)
  
  t.BindDelegate()
  t.InitComponent()
  
  t.equipInfo = equipInfo
  t.equipInfoList = ArrayList.New('equipInfo')
  t.scrollItem = Dictionary.New('GameObject','common_equip_icon')
  t.selectEquipInfo = nil
  t.clickInheritEquipItemCallback = InheritEquipItemCallback
  t.sortType = 0
  
  t.RefreshAll()
end

function t.Close()
  if t.transform ~= nil then
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
    t.UnbindDelegate()
  end
end

function t.BindDelegate()
  equip_model.OnEquipInheritSuccessDelegate:AddListener(t.InheritSuccessByProtocol)
  equip_model.OnEquipmentInfoUpdateDelegate:AddListener(t.RefreshAll)
end

function t.UnbindDelegate()
  equip_model.OnEquipInheritSuccessDelegate:RemoveListener(t.InheritSuccessByProtocol)
  equip_model.OnEquipmentInfoUpdateDelegate:RemoveListener(t.RefreshAll)
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  t.btnInherit = t.transform:Find('btn_inherit'):GetComponent(typeof(Button))
  t.btnInherit.onClick:AddListener(t.ClickInheritHandler)
  t.textCost = t.transform:Find('text_cost'):GetComponent(typeof(Text))
  t.toggleStar = t.transform:Find('bottom/Toggle_star'):GetComponent(typeof(Toggle))
  t.toggleStar.onValueChanged:AddListener(t.ClickToggleHandler)
  t.toggleStrengthen = t.transform:Find('bottom/Toggle_strengthen'):GetComponent(typeof(Toggle))
  t.toggleStrengthen.onValueChanged:AddListener(t.ClickToggleHandler)
  t.goNoneTip = t.transform:Find('text_none_tip').gameObject
  
  t.dropDownSort = t.transform:Find('title/Dropdown_select'):GetComponent(typeof(Dropdown))
  t.dropDownSort.onValueChanged:AddListener(t.ClickChangeSortBtnHandler)
  t.dropDownSort.options:Clear()
  t.dropDownSort.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.hero_strengthen_view.sort_ascending")))
  t.dropDownSort.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.hero_strengthen_view.sort_descending")))
end
function t.RefreshAll()
  t.InitScrollContent()
  t.Refresh(false)
end
function t.InitScrollContent()
  local equipInfoList = equip_model.GetFreeEquipmentInfoList ()
  t.equipInfoList:Clear()
  for k,v in pairs(equipInfoList) do
    if v ~= t.equipInfo and (v.strengthenLevel > 0 or v.star > 0 ) then
      t.equipInfoList:Add(v)
    else
      if t.selectEquipInfo ~= nil and t.selectEquipInfo.id == t.equipInfo.id then
        t.selectEquipInfo = nil
      end
    end
  end
  if t.sortType == 0 then
    t.equipInfoList:Sort(equip_model.CompareEquipmentByQualityAsc)
  else
    t.equipInfoList:Sort(equip_model.CompareEquipmentByQualityDsc)
  end
  
  
  t.scrollContent:Init(t.equipInfoList.Count,false,0)
  
  t.goNoneTip:SetActive(t.equipInfoList.Count == 0)
end
function t.Refresh(needRefreshScroll)
  if needRefreshScroll then
    t.RefreshScrollContent()
  end
  t.RefreshMoney()
end
function t.RefreshMoney()
  local cost = 0
  t.textCost.text = cost
  t.textCost.gameObject:SetActive(cost > 0)
end

function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end

function t.SetEquipInfo(equipInfo)
  t.equipInfo = equipInfo
  t.RefreshAll()
end

-----------------------click event---------------------
-- dropdown
function t.ClickChangeSortBtnHandler(index)
  t.sortType = index
  t.RefreshAll()
end

--toggle
function t.ClickToggleHandler(isChange)
  if t.clickInheritEquipItemCallback then
    t.clickInheritEquipItemCallback(t.selectEquipInfo,t.toggleStrengthen.isOn,t.toggleStar.isOn)
  end
end

function t.ResetItemHandler(go,index)
  local item = t.scrollItem:Get(go)
  if item == nil then
    item = common_equip_icon.NewByGameObject(go)
    item.onClick:AddListener(t.ClickEquipItemHandler)
    t.scrollItem:Add(go,item)
  end
  local equipInfo = t.equipInfoList:Get(index + 1)
  item:SetEquipInfo(equipInfo)
  item:SetSelect( t.selectEquipInfo == equipInfo )
end
--点击列表装备
function t.ClickEquipItemHandler(equipIcon)
  t.selectEquipInfo = equipIcon.equipInfo
  t.Refresh(true)
  if t.clickInheritEquipItemCallback then
    t.clickInheritEquipItemCallback(t.selectEquipInfo,t.toggleStrengthen.isOn,t.toggleStar.isOn)
  end
end

--继承
function t.ClickInheritHandler()
  local hasStar = t.toggleStar.isOn
  local hasStrengthen = t.toggleStrengthen.isOn
  if t.selectEquipInfo == nil then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.inherit.choiceInheritEquip'))
    return
  end
  
  if not hasStar and not hasStrengthen then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.inherit.choiceInheritStarOrLevel'))
    return
  end
  if hasStar and hasStrengthen then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.inherit.recoverStarAndLevel'),t.ConfirmInheritHandler)
    return
  end
  if hasStar then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.inherit.recoverStar'),t.ConfirmInheritHandler)
    return
  end
  if hasStrengthen then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.inherit.recoverLevel'),t.ConfirmInheritHandler)
    return
  end
end

function t.ConfirmInheritHandler()
  local hasStar = t.toggleStar.isOn
  local hasStrengthen = t.toggleStrengthen.isOn
  local own = game_model.GetBaseResourceValue(BaseResType.Diamond)
  print(t.equipInfo.id,t.selectEquipInfo.id,hasStar,hasStrengthen)
  equip_controller.EquipInheritReq(t.equipInfo.id,t.selectEquipInfo.id,hasStar,hasStrengthen)
  
end

----------------------------update by protocol---------------------
function t.InheritSuccessByProtocol()
  auto_destroy_tip_view.Open('继承成功')
  t.RefreshAll()
end
return t