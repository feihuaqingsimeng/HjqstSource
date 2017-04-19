local t = {}
local PREFAB_PATH = 'ui/equipments/gem_insert/gem_insert_panel'

local gem_insert_item = dofile('ui/equip/view/equip_training/gem_insert/gem_insert_item')
local item_data = gamemanager.GetData('item_data')
local item_model = gamemanager.GetModel('item_model')
local gem_synthesis_data = gamemanager.GetData('gem_synthesis_data')
local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
local global_data = gamemanager.GetData('global_data')
local equip_model = gamemanager.GetModel('equip_model')


function t.Open (rightParent ,equipInfo,trainingView)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(rightParent, false)
  t.trainingView = trainingView
  t.InitComponent()
  
  --('属性类型type','好多宝石List')
  t.gemInfoDictionary = Dictionary.New('number','ArrayList<ItemInfo>')
  t.currentGemInfoList = ArrayList.New('ArrayList<ItemInfo>')
  --obj,gem_insert_item
  t.scrollItemData = {}
  t.equipInfo = equipInfo

  --选中状态
  t.SelectData = {}
  t.SelectData.selectState = GemSelectState.NoSelect
  t.SelectData.selectGemId = 0 -- 点中未镶嵌宝石id
  t.SelectData.selectSlotIndex = 0--点中槽id
  
  t.InitScrollContent()
  t.Refresh()
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
  equip_model.UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipGemInsertSuccessDelegate:AddListener(t.OnEquipGemInsertSuccessByProtocol)
end

function t.UnbindDelegate()
  equip_model.UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  equip_model.OnEquipGemInsertSuccessDelegate:RemoveListener(t.OnEquipGemInsertSuccessByProtocol)

end

function t.InitComponent()
  local canvas =  t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.scrollContent = t.transform:Find('img_bg/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  t.goNoneTip = t.transform:Find('img_bg/text_none').gameObject
  t.leftRoot = t.transform:Find('left_root')
  --origin
  t.originRootTable = {}
  t.originRootTable.root = t.leftRoot:Find('origin_root')
  t.originRootTable.tranSelect = t.originRootTable.root:Find('img_select')
  t.originRootTable.tranSelect.gameObject:SetActive(false)
  t.originRootTable.gemSlot = {}
  t.originRootTable.gemSlot[1] = {}
  t.originRootTable.gemSlot[2] = {}
  t.originRootTable.gemSlot[1].tranBg= t.originRootTable.root:Find('gem_1')
  t.originRootTable.gemSlot[1].tranBg:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickGemSlotHandler)
  t.originRootTable.gemSlot[1].imgIcon= t.originRootTable.root:Find('gem_1/img_icon'):GetComponent(typeof(Image))
  t.originRootTable.gemSlot[1].imgSlotTypeIcon = t.originRootTable.root:Find('gem_1/gem_1_small'):GetComponent(typeof(Image))
  t.originRootTable.gemSlot[1].goEffectInsert = t.originRootTable.root:Find('gem_1/ui_baoshixiangqian').gameObject
  t.originRootTable.gemSlot[1].goEffectInsert:SetActive(false)
  particle_util.ChangeParticleSortingOrder(t.originRootTable.gemSlot[1].goEffectInsert,canvas.sortingLayerName,canvas.sortingOrder)
  
  t.originRootTable.gemSlot[2].tranBg = t.originRootTable.root:Find('gem_2')
  t.originRootTable.gemSlot[2].tranBg:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickGemSlotHandler)
  t.originRootTable.gemSlot[2].imgIcon= t.originRootTable.root:Find('gem_2/img_icon'):GetComponent(typeof(Image))
  t.originRootTable.gemSlot[2].imgSlotTypeIcon = t.originRootTable.root:Find('gem_2/gem_2_small'):GetComponent(typeof(Image))
  t.originRootTable.gemSlot[2].goEffectInsert = t.originRootTable.root:Find('gem_2/ui_baoshixiangqian').gameObject
  t.originRootTable.gemSlot[2].goEffectInsert:SetActive(false)
  particle_util.ChangeParticleSortingOrder(t.originRootTable.gemSlot[2].goEffectInsert,canvas.sortingLayerName,canvas.sortingOrder)
end

function t.SetEquipInfo(equipInfo)
  t.equipInfo = equipInfo
  t.Refresh()
end

function t.InitScrollContent()
  t.InitGemInfoTable()
  local count = t.currentGemInfoList.Count
  t.scrollContent:Init(count,false,0)
  
  t.goNoneTip:SetActive(count == 0)
end
function t.Refresh()
  
  t.RefreshCurrentInsertGem()
  t.scrollContent:RefreshAllContentItems()
  --t.RefreshGemAttr()
  
  --t.RefreshButton()
end
--宝石分类
function t.InitGemInfoTable()
  local gemInfoTable = gamemanager.GetModel('item_model').GetItemInfoListByItemType(ItemType.Gem)
  t.gemInfoTable = {}
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  local attrData = nil
  t.gemInfoDictionary:Clear()
  for k,v in pairs(gemInfoTable) do
    if v:Count() > 0 then
      attrData = gem_attr_data.GetDataById(v.itemData.id)
      if attrData ~= nil then
        if  t.gemInfoDictionary:Get(attrData.equipAttr.type) == nil then
          t.gemInfoDictionary:Add(attrData.equipAttr.type,ArrayList.New('ItemInfo'))
        end
        local infoList = t.gemInfoDictionary:Get(attrData.equipAttr.type)
        infoList:Add(v)
      end
    end
  end
  --sort
  t.currentGemInfoList:Clear()
  for k,v in pairs(t.gemInfoDictionary:GetDatas()) do
   v:Sort(t.SortGemInfo)
   t.currentGemInfoList:Add(v)
   
  end
end

function t.SortGemInfo(itemInfo1,itemInfo2)
  local attrData1 = gamemanager.GetData('gem_attr_data').GetDataById(itemInfo1.itemData.id)
  local attrData2 = gamemanager.GetData('gem_attr_data').GetDataById(itemInfo2.itemData.id)
  return attrData1.equipAttr.value - attrData2.equipAttr.value > 0
end

function t.RefreshCurrentInsertGem()
  
  --t.originRootTable.tranSelect.gameObject:SetActive(false)
  
  local count = #t.originRootTable.gemSlot
  for i = 1,count do
    local gemSlot = t.originRootTable.gemSlot[i]
    gemSlot.imgSlotTypeIcon.sprite = ui_util.GetGemSlotIconSprite(t.equipInfo.data.gem[i])
    --没有该槽 or 锁住
    if t.equipInfo.gemInsertIds[i] == -1 then
      gemSlot.imgIcon.gameObject:SetActive(true)
      gemSlot.imgIcon.sprite = ResMgr.instance:LoadSprite('sprite/skill/icon_skill_none')
    else 
      --镶嵌了宝石
      if t.equipInfo.gemInsertIds[i] ~= 0 then
        gemSlot.imgIcon.gameObject:SetActive(true)
        gemSlot.imgIcon.sprite = ResMgr.instance:LoadSprite(item_data.GetDataById(t.equipInfo.gemInsertIds[i]):IconPath())
      else
        gemSlot.imgIcon.gameObject:SetActive(false)
      end
    end
    --[[if t.SelectData.selectState == GemSelectState.SelectEquipSlot and i == t.SelectData.selectSlotIndex then
      t.originRootTable.tranSelect.gameObject:SetActive(true)
      t.originRootTable.tranSelect.localPosition = gemSlot.tranBg.localPosition
    end]]
    
  end
end

----------------------click event---------------------
--点击装备上的宝石孔
function t.ClickGemSlotHandler(obj)
  
  for k,v in ipairs(t.originRootTable.gemSlot) do
    if v.tranBg.gameObject == obj then--找到点击对象啦
      if t.equipInfo.gemInsertIds[k] == -1 then -- -_-锁住啦
        t.OpenGemUnlockSlotView(t.equipInfo,k)
      else
        t.SelectData.selectState = GemSelectState.SelectEquipSlot
        t.SelectData.selectSlotIndex = k
        t.SelectData.selectGemId = 0
        --t.originRootTable.tranSelect.gameObject:SetActive(true)
        --t.originRootTable.tranSelect.localPosition = v.tranBg.localPosition
        t.Refresh()
        if t.equipInfo.gemInsertIds[k] > 0 then
          t.OpenGemCheckView()
        else
          t.OpenGemUnlockSlotView(t.equipInfo,k)
        end
      end
      break
    end
  end
end

function t.ResetItemHandler(obj,index)
 
  if t.scrollItemData[obj] == nil then
    t.scrollItemData[obj] = gem_insert_item.New(obj,t.ClickGemItemHandler)
  end
  local item = t.scrollItemData[obj]
  local infoList = t.currentGemInfoList:Get(index+1)
  item:SetInfoList(infoList)
end
--点击未镶嵌宝石
function t.ClickGemItemHandler(commonItemIcon)
  
  t.SelectData.selectState = GemSelectState.SelectUnUseGem
  t.SelectData.selectGemId = commonItemIcon.itemInfo.instanceId
  print('点击未镶嵌宝石 select:',commonItemIcon.itemInfo.instanceId,'gemid:',t.SelectData.selectGemId)
  t.OpenGemCheckView()

end

function t.OpenGemUnlockSlotView(euqipInfo,slotIndex)
  local gem_unlock_slot_view = require('ui/equip/view/equip_training/gem_insert/gem_unlock_slot_view')
  gem_unlock_slot_view.Open(euqipInfo,slotIndex)
end

function t.OpenGemCheckView()
  dofile('ui/equip/view/equip_training/gem_insert/equip_gem_check_view').Open(t.equipInfo,t.SelectData)
end
---------------------update by protocol----------------
function t.UpdateTrainingByProtocol()
  local needReset = false
  if t.SelectData.selectState == GemSelectState.SelectUnUseGem then
   local itemInfo = gamemanager.GetModel('item_model').GetItemInfoByInstanceID(t.SelectData.selectGemId)
   if itemInfo == nil or itemInfo:Count() == 0 then
      needReset = true
    end
  else
    needReset = true
  end
  if needReset then
    t.SelectData.selectState = GemSelectState.NoSelect
    t.SelectData.selectGemId = 0
  end
  t.InitScrollContent()
  t.Refresh()
end
function t.OnEquipGemInsertSuccessByProtocol(slot)
  local index = t.SelectData.selectSlotIndex
  local state = t.SelectData.selectState
  
  t.SelectData.selectState = GemSelectState.NoSelect
  t.SelectData.selectGemId = 0
  
  t.InitScrollContent()
  t.Refresh()
  if state == GemSelectState.SelectUnUseGem then
    coroutine.start(function()
      t.originRootTable.gemSlot[slot].goEffectInsert:SetActive(true)
      coroutine.wait(1)
      if t.transform then
        t.originRootTable.gemSlot[slot].goEffectInsert:SetActive(false) 
      end
    end)
  end
  
end
return t