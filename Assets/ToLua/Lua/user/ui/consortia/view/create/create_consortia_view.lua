local t = {}
local PREFAB_PATH = 'ui/consortia/create/create_consortia_view'

local consortia_info = dofile('ui/consortia/model/consortia_info')
local consortia_item = dofile('ui/consortia/view/create/consortia_item')
local global_data = gamemanager.GetData('global_data')
local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
local common_auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local game_model = gamemanager.GetModel('game_model')

function t.Open ()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
 
  t.markItemList = {}
  t.selectMarkIndex = 1
 
  t.BindDelegate ()
  t.InitComponent()
  t.Refresh()
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
function t.BindDelegate ()
  game_model.onUpdateBaseResourceDelegate:AddListener(t.RefreshCost)
  consortia_model.onUpdateCreateSuccessDelegate:AddListener(t.UpdateConsortiaCreateSuccessByProtocol)
end
function t.UnbindDelegate()
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.RefreshCost)
  consortia_model.onUpdateCreateSuccessDelegate:RemoveListener(t.UpdateConsortiaCreateSuccessByProtocol)
end
function t.InitComponent()
  local frame = t.transform:Find('core/img_frame')
  t.inputFieldName = frame:Find('InputField_name'):GetComponent(typeof(InputField))
  t.goMark = frame:Find('Scroll View/mark_prefab').gameObject
  t.goMark:SetActive(false)
  t.tranMarkRoot = frame:Find('Scroll View/Viewport/Content')
  t.imgCost = frame:Find('cost/Image'):GetComponent(typeof(Image))
  t.textCost = frame:Find('cost/text_cost'):GetComponent(typeof(Text))
  local btnCreate = frame:Find('btn_create'):GetComponent(typeof(Button))
  btnCreate.onClick:AddListener(t.ClickCreateHandler)
  local btnClose = frame:Find('btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.Close)
end

function t.Refresh()
  for k,v in pairs(global_data.guild_mark) do
    local go = GameObject.Instantiate(t.goMark)
    go:SetActive(true)
    local tran = go:GetComponent(typeof(Transform))
    tran:SetParent(t.tranMarkRoot,false)
    t.markItemList[k] = {}
    t.markItemList[k].imgMark = tran:Find('img_mark'):GetComponent(typeof(Image))
    t.markItemList[k].imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(k)

    t.markItemList[k].goSelect = tran:Find('img_select').gameObject
    t.markItemList[k].goSelect:SetActive(k == t.selectMarkIndex)
    t.markItemList[k].imgMark:GetComponent(typeof(Button)).onClick:AddListener(function()
        t.ClickMarkHandler(k)
      end)
  end
  t.RefreshCost()

end
function t.RefreshCost()
    t.imgCost.sprite = ResMgr.instance:LoadSprite(ui_util.GetBaseResIconPath(global_data.guild_creat_cost.type))
  local own = gamemanager.GetModel('game_model').GetBaseResourceValue(global_data.guild_creat_cost.type)
  if own < global_data.guild_creat_cost.count then
    t.textCost.text = ui_util.FormatToRedText(global_data.guild_creat_cost.count)
  else
    t.textCost.text = ui_util.FormatToGreenText(global_data.guild_creat_cost.count)
  end
  
end
--------------------click event---------------------
function t.ClickCreateHandler()
  
  
  local str = t.inputFieldName.text
  local len = string.len(str)
  if len == 0 then
    common_error_tip_view.Open(LocalizationController.instance:Get('ui.create_consortia_view.needName'))
    return
  end
  if len > 21 then
    common_error_tip_view.Open(LocalizationController.instance:Get('ui.create_consortia_view.countLimit'))
    return
  end
  if Common.Util.BlackListWordUtil.HasBlockWords(str) then
    common_error_tip_view.Open(LocalizationController.instance:Get('ui.create_role_view.hasBlockWords'))
    return
  end
  if not game_model.CheckBaseResEnoughByType( global_data.guild_creat_cost.type, global_data.guild_creat_cost.count) then
    return
  end
  consortia_controller.GuildCreateReq(t.inputFieldName.text,t.selectMarkIndex)
end
function t.ClickMarkHandler(index)
  t.markItemList[t.selectMarkIndex].goSelect:SetActive(false)
  t.markItemList[index].goSelect:SetActive(true)
  t.selectMarkIndex = index
end
----------------------------update by protocol-------------------
function t.UpdateConsortiaCreateSuccessByProtocol()
  common_auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.createSuc'))
  t.Close()
end

return t