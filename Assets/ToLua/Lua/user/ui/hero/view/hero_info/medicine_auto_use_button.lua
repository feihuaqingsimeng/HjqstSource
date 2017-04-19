local t = {}
t.__index = t

local item_data = gamemanager.GetData('item_data')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  
  o.transform = transform
  --one param ,itemid  ,need return bool
  o.onMedicineStartCallback = nil
  --one param :addExp,need return bool
  o.onAutoAddExpCallback = nil
  --two param: itemid,costCount
  o.onMedicineEndCallback = nil
  
  o.useTotalNum = 0
  o.isPressUp = 0
  
  o:InitComponent()
  
  
  return o
end

function t:InitComponent()
  self.imgItem = self.transform:Find('img_icon'):GetComponent(typeof(Image))
  self.textCount = self.transform:Find('text_count'):GetComponent(typeof(Text))
  self.eventTriggerDelegate = self.transform:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
  self.eventTriggerDelegate.onDown:AddListener(function (go)
      self:ClickDownHandler()
    end)
  self.eventTriggerDelegate.onUp:AddListener(function (go)
      self:ClickUpHandler()
    end)
  self.eventTriggerDelegate.onExit:AddListener(function (go)
      self:OnExitHandler()
    end)
  
end
function t:Refresh()
  self.imgItem.sprite =  Common.ResMgr.ResMgr.instance:LoadSprite(self.itemData:IconPath())
  self:RefreshCount()
end
function t:RefreshCount()
  if self.count == 0 then
    self.textCount.text = ui_util.FormatToRedText(self.count)
  else
    self.textCount.text = ui_util.FormatToGreenText(self.count)
  end
  
end
function t:SetItemData( itemId,count)
  self.itemData =  item_data.GetDataById(itemId)
  self.count = count
  self:Refresh()
end

function t:MedicineUseCoroutine()
  if self.onAutoAddExpCallback == nil then
    print('onAutoAddExpCallback 没有赋值哟')
    return
  end
  
  local index = 0
  local addExps = gamemanager.GetData('global_data').expSolutions
  local addExp = 0
  if self.itemData.id == ITEM_ID.bigExpMedicine then
    addExp = addExps[1]
  elseif self.itemData.id == ITEM_ID.midExpMedicine then
    addExp = addExps[2]
  else
    addExp = addExps[3]
  end
  --循环加经验啦
 while true do
    if self.isPressUp then
      coroutine.stop(self.medicineUseCoroutine)
      break
    end
    self.useTotalNum = self.useTotalNum + 1
    self.count = self.count - 1
    self:RefreshCount()
    local isAdd = self.onAutoAddExpCallback(addExp)
    if isAdd == false then
      break
    end
    if self.count == 0 then
      break
    end 
    if index == 0 then
      break
      --coroutine.wait(0.6)
    else
      --coroutine.wait(0.2)
    end
   -- index = index + 1
  end
end
--------------------click event------------------
function t:ClickDownHandler()
  self.useTotalNum = 0
  if self.onMedicineStartCallback ~= nil then
    local canStart = self.onMedicineStartCallback(self.itemData.id)
    if canStart then
      self.isPressUp = false
      self.useTotalNum = 0
      self.medicineUseCoroutine = coroutine.start(function()
          self:MedicineUseCoroutine()
        end)
    end
  end
end

function t:ClickUpHandler()
  self.isPressUp = true
  if self.onMedicineEndCallback ~= nil then
    self.onMedicineEndCallback(self.itemData.id,self.useTotalNum)
    self.useTotalNum = 0
  end
end

function t:OnExitHandler()
  self.isPressUp = true
end

return t