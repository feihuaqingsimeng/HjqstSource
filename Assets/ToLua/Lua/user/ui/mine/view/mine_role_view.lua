local t={}
t.__index = t
t.m_imgIcon =nil
t.m_btnLook= nil
t.m_labLv= nil
t.m_labName = nil
t.m_labTime = nil
t.m_labFight = nil
local mineModel = gamemanager.GetModel('mine_model')
local mineData = gamemanager.GetData('mine_data')
local m_curMineData = nil
t.roleInfo =nil

function t.NewByGameObject(gameObject,time)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.roleInfo = nil
  o.allTime  =nil
  o.onClick = void_delegate.New()
  
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.m_imgIcon = self.transform:Find('icon_bg/img_icon'):GetComponent(typeof(Image))
  self.m_labLv  = self.transform:Find('text_lv'):GetComponent(typeof(Text))
  self.m_labName = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.m_labTime = self.transform:Find('text_time/text_num'):GetComponent(typeof(Text))
  self.m_btnLook = self.transform:Find('btn_look'):GetComponent(typeof(Button))
  self.m_labFight = self.transform:Find('img_battle/text_num'):GetComponent(typeof(Text))
  m_curMineData = mineModel.GetMineData(mineModel.m_curMineInfo.mineNo) 
  self:ResetListener ()
end
function t:ResetListener ()
  self.m_btnLook.onClick:RemoveAllListeners()
  self.m_btnLook.onClick:AddListener(
    function()
      self.onClick:InvokeOneParam(self.roleInfo)
    end)
end

function t:SetRoleInfo(info)
  self.roleInfo = info
  --print("self.roleInfo.endTime  :",self.roleInfo.endTime )
  self.m_imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(self.roleInfo.headNo))
  self.m_labLv.text = self.roleInfo.roleLv
  self.m_labName.text = self.roleInfo.roleName
  self.m_labFight.text = self.roleInfo.fightingPower
  self.UpdateCoroutine = coroutine.start(function() self:UpdateView() end)
end
function t:RefreshTime()
 -- print("self.roleInfo.endTime11  :",self.roleInfo.endTime )
  local time =m_curMineData.time-(self.roleInfo.endTime - TimeController.instance.ServerTimeTicksSecond)
  if time <=0 then
    gamemanager.GetCtrl('mine_controller').GetMineInfoReq(mineModel.m_curMineInfo.mineNo)
  end
  self.m_labTime.text = TimeUtil.FormatSecondToHour(time)
end
function t:UpdateView()
  while(true) do
    self:RefreshTime()
    coroutine.wait(1)
  end
end
function t:Close()
    coroutine.stop(self.UpdateCoroutine)
end
return t