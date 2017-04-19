local t = {}
local PREFAB_PATH = 'ui/consortia/create/consortia_list_view'

local consortia_info = require('ui/consortia/model/consortia_info')
local consortia_item = require('ui/consortia/view/create/consortia_item')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')
local common_auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open ()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
    
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.create_consortia_view.title'),t.OnClickCloseButtonHandler,true,true,true,false,false,false,false)
  
  t.InitComponent()
  
  t.consortiaInfoList = {}
  
  t.scrollItem = {}
  t.selectConsortiaInfo = nil
  
  t.InitScrollContent(consortia_model.consortiaInfoList)
  t.RefreshSelectConsortiaInfo()
  t.BindDelegate()
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
function t.BindDelegate ()
  consortia_model.onUpdateRefreshDelegate:AddListener(t.UpdateApplyGuildByProtocol)
  consortia_model.onUpdateCreateSuccessDelegate:AddListener(t.UpdateConsortiaCreateSuccessByProtocol)
end
function t.UnbindDelegate()
  consortia_model.onUpdateRefreshDelegate:RemoveListener(t.UpdateApplyGuildByProtocol)
  consortia_model.onUpdateCreateSuccessDelegate:RemoveListener(t.UpdateConsortiaCreateSuccessByProtocol)
end
function t.InitComponent()
   t.scrollContent = t.transform:Find('core/img_left/scroll_view/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.OnResetItemHandler)
  
  local left_part = t.transform:Find('core/img_left')
  local right_part = t.transform:Find('core/img_right_part')
  t.inputFieldSearchConsortia = left_part:Find('search_input_field'):GetComponent(typeof(InputField))
  t.inputFieldSearchConsortia.onEndEdit:AddListener(t.EndSearchEditHandler)
  local btnSearch = left_part:Find('btn_search'):GetComponent(typeof(Button))
  btnSearch.onClick:AddListener(t.ClickSearchHandler)
  
  t.btnApply = right_part:Find('btn_apply'):GetComponent(typeof(Button))
  t.btnApply.onClick:AddListener(t.ClickApplyHandler)
  t.btnCreate = right_part:Find('btn_create_consortia'):GetComponent(typeof(Button))
  t.btnCreate.onClick:AddListener(t.ClickCreateHandle)
  
  t.imgConsortiaMark = right_part:Find('img_frame/img_mark'):GetComponent(typeof(Image))
  t.textConsortiaCreator = right_part:Find('img_frame/create_bg/text_creator'):GetComponent(typeof(Text))
  t.textConsortiaId = right_part:Find('img_frame/id_bg/text_id'):GetComponent(typeof(Text))
  t.textConsortiaMenberCount = right_part:Find('img_frame/member_bg/text_member'):GetComponent(typeof(Text))
  t.textConsortiaName = right_part:Find('img_frame/text_name'):GetComponent(typeof(Text))
  t.textConsortiaNotice = right_part:Find('img_frame/notice_bg/text_notice'):GetComponent(typeof(Text))
  t.textConsortiaLv = right_part:Find('img_frame/text_lv'):GetComponent(typeof(Text))
  t.textApply = right_part:Find('btn_apply/text'):GetComponent(typeof(Text))
end

function t.InitScrollContent(infoList)
  t.consortiaInfoList = infoList
  
  local count = #infoList
  if count == 0 then
    t.selectConsortiaInfo = nil
    
  else
    t.selectConsortiaInfo = t.consortiaInfoList[1]

  end
  
  t.scrollContent:Init(count,false,0.25)
end
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
function t.Refresh()
  t.RefreshSelectConsortiaInfo()
end

function t.RefreshSelectConsortiaInfo()
  if t.selectConsortiaInfo then
    t.imgConsortiaMark.gameObject:SetActive(true)
    t.imgConsortiaMark.sprite = ui_util.GetConsortiaMarkIconSprite(t.selectConsortiaInfo.headNo)
    t.textConsortiaCreator.text = t.selectConsortiaInfo.creatorName
    t.textConsortiaId.text = t.selectConsortiaInfo.id
    t.textConsortiaMenberCount.text = string.format('%d/%d',t.selectConsortiaInfo.curNum,t.selectConsortiaInfo.maxNum)
    t.textConsortiaName.text = t.selectConsortiaInfo.name
    t.textConsortiaNotice.text = t.selectConsortiaInfo.notice
    t.textConsortiaLv.text = string.format(LocalizationController.instance:Get('ui.consortia_view.consortia_lv'),t.selectConsortiaInfo.lv)
    if consortia_model.IsApplyConsortia(t.selectConsortiaInfo.id) then
      t.textApply.text = LocalizationController.instance:Get('ui.create_consortia_view.apply_joined')
    else
      t.textApply.text = LocalizationController.instance:Get('ui.create_consortia_view.apply_join')
    end
   t.btnApply.gameObject:SetActive(true)
  else
    local noneStr = LocalizationController.instance:Get('ui.create_consortia_view.none')
    t.textConsortiaCreator.text = noneStr
    t.imgConsortiaMark.gameObject:SetActive(false)
    t.textConsortiaId.text = 0
    t.textConsortiaMenberCount.text = 0
    t.textConsortiaName.text = noneStr
    t.textConsortiaNotice.text = noneStr
    t.btnApply.gameObject:SetActive(false)
    
  end
  
end
------------------------- ui event handlers --------------------------
function t.OnResetItemHandler (go, index)
  if not t.scrollItem[go] then
    t.scrollItem[go] = consortia_item.BindGameObject(go,t.ClickItemHandler)
  end
  local info = t.consortiaInfoList[index+1]
  t.scrollItem[go]:SetConsortiaInfo (info)
  t.scrollItem[go]:SetSelect(info == t.selectConsortiaInfo )
end
function t.ClickItemHandler(consortiaItem)
  t.selectConsortiaInfo = consortiaItem.consortiaInfo
  t.RefreshSelectConsortiaInfo()
  t.RefreshScrollContent()
end

--搜索公会
function t.ClickSearchHandler()
  
  local str = t.inputFieldSearchConsortia.text
  local infoList = {}
  if str ~= '' then
    local index = 1
    for k,v in pairs(consortia_model.consortiaInfoList) do
      if str == v.name then
        infoList[index] = v
        index = index + 1
      
      else
        local id = tonumber(str)
        if id == v.id then
          infoList[index] = v
          index = index + 1
        end
      end
    end
    t.InitScrollContent(infoList)
    t.RefreshSelectConsortiaInfo()
  end
  
end
function t.EndSearchEditHandler(str)
  
  str = string.gsub(str,' ','')
  if str == '' then
    t.InitScrollContent(consortia_model.consortiaInfoList)
    t.RefreshSelectConsortiaInfo()
  end

end

--申请公会
function t.ClickApplyHandler()
  if not t.selectConsortiaInfo then
    return
  end
 
  consortia_controller.GuildAddReq(t.selectConsortiaInfo.id)
end
--创建公会
function t.ClickCreateHandle()
  consortia_controller.OpenCreateConsortiaView()
end
function t.OnClickCloseButtonHandler ()
  t.Close()
end
--------------------update by protocol--------------------
function t.UpdateConsortiaCreateSuccessByProtocol()
  
  t.Close()
end
function t.UpdateApplyGuildByProtocol(id)
  
  t.RefreshSelectConsortiaInfo()
end
return t