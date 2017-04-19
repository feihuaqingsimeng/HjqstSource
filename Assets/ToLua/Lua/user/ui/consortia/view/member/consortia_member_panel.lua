local t = {}
local PREFAB_PATH = 'ui/consortia/member/consortia_member_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')
local member_item = dofile('ui/consortia/view/member/member_item')
local common_errors_tip_view = require('ui/tips/view/common_error_tips_view')

function t.Open (parent)
  local gameObject = GameObject.Instantiate(ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(parent,false)
  
  t.scrollItems = {}
  t.selectToggleIndex = 1
  t.memberInfoList = {}
  t.applyList = {}
  
  t.BindDelegate ()
  t.InitComponent()
  
  t.goToggles[1]:GetComponent(typeof(Toggle)).isOn = true
  t.goToggles[2]:SetActive(consortia_model.IsConsortiaCreator())
  
  
  consortia_controller.GuildMemberListReq()
end
function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.UnbindDelegate()
end
function t.BindDelegate ()
  consortia_model.onUpdateRefreshOneParamDelegate:AddListener(t.onUpdateRefreshOneParamByProtocol)
end
function t.UnbindDelegate()
  consortia_model.onUpdateRefreshOneParamDelegate:RemoveListener(t.onUpdateRefreshOneParamByProtocol)
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  
  t.goToggles = {}
  t.goToggles[1] = t.transform:Find('toggle_root/Toggle_member').gameObject
  t.goToggles[2] = t.transform:Find('toggle_root/Toggle_apply_list').gameObject
  for k,v in ipairs(t.goToggles) do
    v:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
  end
  t.tranJob = t.transform:Find('title/text_job')
  t.tranContribute = t.transform:Find('title/text_contribute')
  t.tranStatus = t.transform:Find('title/text_status')
  t.textOnlineCount = t.transform:Find('title/text_online_count'):GetComponent(typeof(Text))
  t.statusOriginPos = t.tranStatus.localPosition
  t.btnSetting = t.transform:Find('title/btn_setting'):GetComponent(typeof(Button))
  t.btnSetting.onClick:AddListener(t.ClickMemberSettingHandler)
end
function t.Refresh()
  t.InitScrollContent()
  t.RefreshTitle()
end
function t.RefreshTitle()
  if t.selectToggleIndex == 1 then
    t.tranJob.gameObject:SetActive(true)
    t.tranContribute.gameObject:SetActive(true)
    t.tranStatus.localPosition = t.statusOriginPos
    local count = 0
    for k,v in pairs(t.memberInfoList) do
      if v.lastLoginTime == -1 then
        count = count + 1
      end
    end
    t.textOnlineCount.gameObject:SetActive(true)
    t.textOnlineCount.text = string.format('%d/%d',count, consortia_model.consortiaInfo.maxNum)
  else
    t.tranJob.gameObject:SetActive(false)
    t.tranContribute.gameObject:SetActive(false)
    t.tranStatus.localPosition = t.tranJob.localPosition
    t.textOnlineCount.gameObject:SetActive(false)
  end
  t.btnSetting.gameObject:SetActive(t.selectToggleIndex == 2 and consortia_model.IsConsortiaCreator())
end
function t.InitScrollContent()
  if t.selectToggleIndex == 1 then
    t.scrollContent:Init(#t.memberInfoList,false,0)
  else
    t.scrollContent:Init(#t.applyList,false,0)
  end
  
end
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
---------------------------click event ---------------------
function t.ResetItemHandler(go,index)
  local item = t.scrollItems[go]
  if not item then
    item = member_item.BindGameObject(go)
    t.scrollItems[go] = item
  end
  local info 
  if t.selectToggleIndex == 1 then
    info = t.memberInfoList[index+1]
  else
    info = t.applyList[index+1]
  end
  item:SetMemberInfo(info,t.selectToggleIndex)
  
end
function t.ClickToggleHandler(go)
  local index = 1
  for k,v in ipairs(t.goToggles) do
    if v == go then
      index = k
      break
    end
  end
  if t.selectToggleIndex == index then
    return
  end
  t.selectToggleIndex = index
  if index == 2 then
    consortia_controller.GuildReqListReq()
    if consortia_model.hasGuildApply then
      consortia_model.hasGuildApply = false
      gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Consortia_apply)
    end
  else
    t.Refresh()
  end
end
--加入公会设置
function t.ClickMemberSettingHandler()
  consortia_controller.OpenMemberSettingView()
end
------------------------update by protocol--------------------
--1 响应成员列表  2 响应申请列表
function t.onUpdateRefreshOneParamByProtocol(param)
  if param == 1 then
    t.memberInfoList = consortia_model.memberInfoList
    if t.selectToggleIndex == 1 then
      t.Refresh()
    end
  else
    t.applyList = consortia_model.guildApplyList
    if t.selectToggleIndex == 2 then
      t.Refresh()
    end
  end
end

return  t