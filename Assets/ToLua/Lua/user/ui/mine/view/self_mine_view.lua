local self_mine_view ={}
local  mineModel= gamemanager.GetModel('mine_model')
local mine_role_view = require('ui/mine/view/mine_role_view')
local PREFAB_PATH = 'ui/mine_battle/self_mine_view'
local mineData = gamemanager.GetData('mine_data')
self_mine_view.m_labMineName = nil
self_mine_view.m_imgIcon = nil
self_mine_view.m_labReward = nil
self_mine_view.m_labTime = nil
self_mine_view.m_btnClose = nil
self_mine_view.m_btnGiveUp = nil
self_mine_view.m_btnGetReward= nil
self_mine_view.m_objLogInfo = nil
self_mine_view.m_imgGetReward = nil
self_mine_view.m_scrollLogParent = nil
local  m_curMineData= nil


function self_mine_view.Open()
  local obj = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  self_mine_view.transform = obj:GetComponent(typeof(Transform))
  self_mine_view.InitComp()
  self_mine_view.BindDelegate()
  gamemanager.GetCtrl('mine_controller').GetOwnInfoReq()
end

function self_mine_view.InitComp() 
  self_mine_view.m_labMineName = self_mine_view.transform:Find('core/top/text_title'):GetComponent(typeof(Text))
  self_mine_view.m_imgIcon =self_mine_view .transform:Find('core/top/mine_info/spr_icon'):GetComponent(typeof(Image))
  self_mine_view.m_labReward = self_mine_view.transform:Find('core/top/mine_info/text_gold/text_num'):GetComponent(typeof(Text))
  self_mine_view.m_labTime= self_mine_view .transform:Find('core/top/mine_info/text_time/text_num'):GetComponent(typeof(Text))
  self_mine_view.m_btnClose = self_mine_view.transform:Find('core/top/btn_close'):GetComponent(typeof(Button))
  self_mine_view.m_btnGiveUp = self_mine_view.transform:Find('core/top/mine_info/btn_giveup'):GetComponent(typeof(Button))
  self_mine_view.m_btnGetReward = self_mine_view.transform:Find('core/bottom/btn_get'):GetComponent(typeof(Button))
  self_mine_view.m_imgGetReward = self_mine_view.transform:Find('core/bottom/btn_get'):GetComponent(typeof(Image))
  self_mine_view.m_objLogInfo = self_mine_view.transform:Find('core/bottom/battle_info_scroll/Viewport/mine_battle_info').gameObject
  self_mine_view.m_scrollLogParent = self_mine_view.transform:Find('core/bottom/battle_info_scroll/Viewport/Content'):GetComponent(typeof(ScrollContent))
  self_mine_view.m_scrollLogParent.onResetItem:AddListener(self_mine_view.OnResetItemHandler)
  m_curMineData = mineModel.GetMineData(mineModel.m_curMineInfo.mineNo) 
  self_mine_view.m_btnClose.onClick:AddListener(self_mine_view.Close)
  self_mine_view.m_btnGiveUp.onClick:AddListener(self_mine_view.OnClickGiveUp)
  self_mine_view.m_btnGetReward.onClick:AddListener(self_mine_view.OnClickGetReward)
end

function self_mine_view.BindDelegate()
  mineModel.OnSelfMineCloseDelegate:AddListener(self_mine_view.Close)
  mineModel.OnUpdateSelfMineDelegate:AddListener(self_mine_view.RefreshSelfMine)
  mineModel.OnRedPointUpdateDelegate:AddListener(self_mine_view.UpdateSelfLogInfo)
end

function self_mine_view.UnbindDelegate()
   mineModel.OnSelfMineCloseDelegate:RemoveListener(self_mine_view.Close)
   mineModel.OnUpdateSelfMineDelegate:RemoveListener(self_mine_view.RefreshSelfMine)
   mineModel.OnRedPointUpdateDelegate:RemoveListener(self_mine_view.UpdateSelfLogInfo)
end

function self_mine_view.UpdateSelfLogInfo()
  gamemanager.GetCtrl('mine_controller').GetOwnInfoReq()
end

function self_mine_view.RefreshSelfMine()   
  print("#mineModel.m_selfMineInfo.m_listLog:=",#mineModel.m_selfMineInfo.m_listLog)
  mineModel.UpdateRedPoint(false)
  self_mine_view.m_scrollLogParent:Init(#mineModel.m_selfMineInfo.m_listLog,false,0)
  self_mine_view.SetCurMineInfo()
  self_mine_view.m_scrollLogParent:RefreshAllContentItems()
end
function self_mine_view.SetCurMineInfo()
  self_mine_view.m_labMineName.text = LocalizationController.instance:Get(string.format("ui.mine_view.mine_view_mine_%d",m_curMineData.quality))
  self_mine_view.m_imgIcon.sprite = ResMgr.instance:LoadSprite(string.format('sprite/main_ui/image_ore_%d',m_curMineData.quality))
  self_mine_view.m_labReward.text  = tostring(mineModel.m_selfMineInfo.m_reward)
  if mineModel.m_selfMineInfo.endTime ==-1 then
    self_mine_view.m_labTime.text = LocalizationController.instance:Get("ui.mine_view.state_2")
    self_mine_view.m_btnGetReward.gameObject:SetActive(true)
    self_mine_view.m_btnGiveUp.gameObject:SetActive(false)
  else
    self_mine_view.m_btnGetReward.gameObject:SetActive(false)
    self_mine_view.m_btnGiveUp.gameObject:SetActive(true)
    self_mine_view.UpdateCoroutine = coroutine.start(self_mine_view.UpdateView)
  end 
end
function self_mine_view.RefreshTime() 
  local time =mineModel.m_selfMineInfo.endTime - TimeController.instance.ServerTimeTicksSecond
  if time ~=0 then
    self_mine_view.m_labTime.text = TimeUtil.FormatSecondToHour(time)
    self_mine_view.m_btnGetReward.gameObject:SetActive(false)
    self_mine_view.m_btnGiveUp.gameObject:SetActive(true)
  else
    self_mine_view.m_btnGetReward.gameObject:SetActive(true)
    self_mine_view.m_btnGiveUp.gameObject:SetActive(false)
    self_mine_view.m_labTime.text =  LocalizationController.instance:Get("ui.mine_view.state_2")
  end
end
function self_mine_view.UpdateView()
  while(true) do
    self_mine_view.RefreshTime()
    coroutine.wait(1)
  end
end
function self_mine_view.Close()
    coroutine.stop(self_mine_view.UpdateCoroutine)
    self_mine_view.UnbindDelegate()
    Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function self_mine_view.OnClickGiveUp()
  local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
  confirm_tip_view.Open(LocalizationController.instance:Get('ui.mine_view.give_up'),function() gamemanager.GetCtrl('mine_controller').AbandonMineReq() 
        end)
  
end
function self_mine_view.OnClickGetReward()
  gamemanager.GetCtrl('mine_controller').GetMineAwardReq()
end
function self_mine_view.OnResetItemHandler(go,index)

  local labDes = go.transform:Find('text_contant'):GetComponent(typeof(Text))
  local log=mineModel.m_selfMineInfo.m_listLog[index+1]
  local time = TimeUtil.FormatTimeToString(log.time,"MM-dd HH:mm:ss")    
  local des =''
  local chinese = LocalizationController.instance:Get(string.format("ui.mine_view.log_%s",log.type))
  if log.type ==1 then
     des = string.format(chinese,time)
  elseif log.type ==2 then
    des = string.format(chinese,time,log.params[1])
    self_mine_view.m_btnGetReward.gameObject:SetActive(true)
    self_mine_view.m_btnGiveUp.gameObject:SetActive(false)
  elseif log.type == 3 then
    des = string.format(chinese,time,log.params[1],log.params[2])
  end
  labDes.text =des
end
return self_mine_view