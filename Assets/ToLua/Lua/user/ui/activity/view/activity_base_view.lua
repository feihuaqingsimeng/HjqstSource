activity_base_view = {}
local name='activity_base_view'
activity_base_view.__index = activity_base_view

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')

local activity_main_view = nil
activity_base_view.transform = nil
activity_base_view.parent = nil
activity_base_view.type = -1 

function activity_base_view.activity_view(view)
  local o = view or {}
  setmetatable(o, activity_base_view)  
  return o
end

function activity_base_view:Start(btype)
    local activityView = activity_model:GetActivityConfigByType(btype)
    if nil == activityView then do return end end
    
    local transform = Instantiate(ResMgr.instance:Load(activityView["prefabPath"])).transform
    activity_main_view = uimanager.GetView('activity_main_view')
		transform:SetParent(activity_main_view.coretransform)
		transform:SetSiblingIndex(0)
		transform.localPosition = Vector3.zero
		transform.localScale = Vector3.one
		self.transform = transform
    self.type = btype    
end

function activity_base_view:Open()
  self.transform.gameObject:SetActive(true)  
end

function activity_base_view:OnTimerTick()
end

function activity_base_view:Close()
   self.transform.gameObject:SetActive(false)   
end

function activity_base_view:Clear()
  self.transform = nil
end

function activity_base_view:Init()
  self:InitBarTitle()
end

function activity_base_view:InitBarTitle()
  local eventData = event_data.GetDataByType(self.type)
  if nil == eventData then do return end end
  activity_main_view:SetCommomTopBarTitle(LocalizationController.instance:Get(eventData.event_name))  
end
---获得itemList 结构如下
--local o = {}
 --   o.type = activityData.type
 --   o.subType = activityData.subType
 --   o.conditionValue = activityData.conditionValue
 --   o.id = k
 --   o.state = v
function activity_base_view:GetListData()
  local activityData = activity_model:GetActivityByType(self.type)--获得activity_info
  if nil == activityData then do return end end
  
  local blist = {}
  for k, v in pairs(activityData.scheduleStus.data) do
    local o = {}
    o.type = activityData.type
    o.subType = activityData.subType
    o.conditionValue = activityData.conditionValue
    o.id = k
    o.state = v
    table.insert(blist, o)
  end 
  
  local tevent_data = event_data.GetTypeDatasByType(self.type)
  if nil == tevent_data then do return end end
  
  for i = 1, #tevent_data do
    local isAlredyContain = false
    for j = 1, #blist do if blist[j].id == tonumber(tevent_data[i].small_id) then isAlredyContain = true end end
    
    if not isAlredyContain then
      local o = {}
      o.type = activityData.type
      o.subType = tonumber(tevent_data[i].event_small_type)
      o.conditionValue = activityData.conditionValue
      o.id = tonumber(tevent_data[i].small_id)
      o.state = 3
      table.insert(blist, o)
    end
  end 
  table.sort(blist, function(A, B) return self:SortActivitys(A, B) end)
  return blist
end

function activity_base_view:SortActivitys(activityDataA, activityDataB)
  if nil == activityDataA or nil == activityDataB then return false end
  
  local A, B = 0, 0
  local dataA = event_data.GetDataBySmallId(activityDataA.type, activityDataA.id)
  if nil == dataA then do return false end end
  
  local dataB = event_data.GetDataBySmallId(activityDataB.type, activityDataB.id)
  if nil == dataB then do return false end end  
  
  if activityDataA.state == 2 then  A = A + 10000000 end
  if activityDataB.state == 2 then  B = B + 10000000 end  
  
  A = A + dataA.event_param  
  B = B + dataB.event_param 
  
  return A < B
end

function activity_base_view:Refresh()  
end

function activity_base_view:ActivityUpdate()
end

function activity_base_view:OnBtnGetAwards()
end

function activity_base_view:OnBtnNavagation()
end

function activity_base_view:OnBtnPast()
end

function activity_base_view:OnBtnNone()
end

return activity_base_view