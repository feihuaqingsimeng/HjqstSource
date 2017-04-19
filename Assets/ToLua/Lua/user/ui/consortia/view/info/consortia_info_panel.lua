local t = {}
local PREFAB_PATH = 'ui/consortia/info/consortia_info_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')

local consortia_info = require('ui/consortia/model/consortia_info')
local consortia_data = gamemanager.GetData('consortia_data')
local common_errors_tip_view = require('ui/tips/view/common_error_tips_view')
local common_auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

function t.Open (parent)
  local gameObject = GameObject.Instantiate(ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(parent,false)
  
  t.scrollItems = {}
  
  t.BindDelegate ()
  t.InitComponent()
  
  
  
  t.Refresh()
end
function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.UnbindDelegate()
end
function t.BindDelegate ()
  consortia_model.onUpdateRefreshDelegate:AddListener(t.Refresh)
end
function t.UnbindDelegate()
  consortia_model.onUpdateRefreshDelegate:RemoveListener(t.Refresh)

end

function t.InitComponent()
  local info1 = t.transform:Find('info1')
  local info2 = t.transform:Find('info2')
  t.textLeaderName = info2:Find('text_leader'):GetComponent(typeof(Text))
  t.textId= info2:Find('text_id'):GetComponent(typeof(Text))
  t.textMemberCount= info2:Find('text_member'):GetComponent(typeof(Text))
  t.textNotice = t.transform:Find('consortia_notice_frame/text_consortia_notice'):GetComponent(typeof(Text))
  t.textExp = info1:Find('text_exp'):GetComponent(typeof(Text))
  t.textConsortiaName = info1:Find('text_consortia_name'):GetComponent(typeof(Text))
  t.textLevel = info1:Find('text_lv'):GetComponent(typeof(Text))
  t.slideExp = info1:Find('slider_current_exp'):GetComponent(typeof(Slider))
  t.imgConsortiaMark = info1:Find('img_consortia_mark'):GetComponent(typeof(Image))
  
  t.btnSignIn = t.transform:Find('btn_sign_in'):GetComponent(typeof(Button))
  t.btnSignIn.onClick:AddListener(t.ClickSignInHandler)
  t.textSignIn = t.transform:Find('btn_sign_in/text'):GetComponent(typeof(Text))
  local btnExit = t.transform:Find('btn_exit'):GetComponent(typeof(Button))
  btnExit.onClick:AddListener(t.ClickExitHandler)
  t.textExit = t.transform:Find('btn_exit/text'):GetComponent(typeof(Text))
  t.btnEdit = t.transform:Find('consortia_notice_frame/btn_edit'):GetComponent(typeof(Button))
  t.btnEdit.onClick:AddListener(t.ClickEditHandler)
  t.inputFieldNotice = t.transform:Find('consortia_notice_frame/InputField_notice'):GetComponent(typeof(InputField))
  t.inputFieldNotice.onEndEdit:AddListener(t.endNoticeEditHandler)
  t.inputFieldNotice.gameObject:SetActive(false)
  
  t.scrollContent = t.transform:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
end

function t.Refresh()
  t.RefreshConsortiaInfo()
  t.InitScrollContent()
  
end
function t.InitScrollContent()
  local count = #consortia_model.consortiaInfo.logList
  t.scrollContent:Init(count,false,0)
  t.scrollContent.normalizedPosition = Vector2(0, 1)
end

function t.RefreshConsortiaInfo()
  t.imgConsortiaMark.sprite = ui_util.GetConsortiaMarkIconSprite(consortia_model.consortiaInfo.headNo)
  t.textLeaderName.text = consortia_model.consortiaInfo.creatorName
  t.textId.text = consortia_model.consortiaInfo.id
  t.textMemberCount.text = string.format('%d/%d',consortia_model.consortiaInfo.curNum,consortia_model.consortiaInfo.maxNum)
  t.textNotice.text = consortia_model.consortiaInfo.notice
  t.textConsortiaName.text = consortia_model.consortiaInfo.name
  t.textLevel.text = string.format(LocalizationController.instance:Get('ui.consortia_view.consortia_lv'),consortia_model.consortiaInfo.lv)
  
  --[[
  t.slideExp.value = consortia_model.consortiaInfo:ExpPercent()
  if consortia_model.consortiaInfo:IsMaxLevel() then
    t.textExp.text = ''
  else
    t.textExp.text = string.format('%d/%d',consortia_model.consortiaInfo.exp,consortia_data.GetDataById(consortia_model.consortiaInfo.lv+1).exp)
  end
  ]]--
  
  local currentConsortiaInfo = consortia_model.consortiaInfo
  if not currentConsortiaInfo:IsMaxLevel() then  
    t.slideExp.value = currentConsortiaInfo:ExpPercent()
    t.textExp.text = string.format('%d/%d', currentConsortiaInfo.exp,consortia_data.GetDataById(currentConsortiaInfo.lv+1).exp)
  else
    t.slideExp.value = 1
    t.textExp.text = tostring(currentConsortiaInfo:CurrenExp())
  end
  
  if consortia_model.consortiaInfo.roleGuild.isSign then
    t.textSignIn.text = string.format(LocalizationController.instance:Get('ui.consortia_view.consortia_signed'))
  else
    t.textSignIn.text = string.format(LocalizationController.instance:Get('ui.consortia_view.sign_in'))
  end
  if consortia_model.IsConsortiaCreator() then
    t.btnEdit.gameObject:SetActive(true)
    t.textExit.text = LocalizationController.instance:Get('ui.consortia_view.btnDismiss')
  else
    t.btnEdit.gameObject:SetActive(false)
    t.textExit.text = LocalizationController.instance:Get('ui.consortia_view.btnExit')
  end
  
  
end
-------------------click event----------------------------
function t.ResetItemHandler(go,index)
  local item = t.scrollItems[go]
  if not item then
    item = {}
    item.tran = go:GetComponent(typeof(Transform))
    item.textTitle = item.tran:Find('text_title'):GetComponent(typeof(Text))
    item.textContent = item.tran:Find('text_content'):GetComponent(typeof(Text))
    t.scrollItems[go] = item
  end
  local logInfo = consortia_model.consortiaInfo.logList[index+1]
  item.textTitle.text = logInfo:GetTimeString()
  item.textContent.text = logInfo:GetContent()
  item.tran.sizeDelta = Vector2(item.tran.sizeDelta.x,math.abs(item.textContent.transform.localPosition.y)+item.textContent.preferredHeight)
end

function t.OnClickCloseButtonHandler ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
--签到
function t.ClickSignInHandler()
  consortia_controller.GuildSignReq()
end
--退出公会
function t.ClickExitHandler()
  if consortia_model.IsConsortiaCreator() then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.sureDismiss'),function()
        consortia_controller.GuildDismissReq()
      end)
    
  else
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.sureExit'),function()
        consortia_controller.GuildExitReq()
      end)
    
  end
end
--编辑公告
function t.ClickEditHandler()
  t.inputFieldNotice.gameObject:SetActive(true)
  t.textNotice.gameObject:SetActive(false)
  t.inputFieldNotice.text = t.textNotice.text
  t.inputFieldNotice:ActivateInputField()
end

function t.endNoticeEditHandler(str)
  str = string.sub(str,0,150)
  consortia_controller.GuildNoticeReq(str)
  t.inputFieldNotice.gameObject:SetActive(false)
  t.textNotice.gameObject:SetActive(true)
end
--------------------update by protocol-----------


return t
