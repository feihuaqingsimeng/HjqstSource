local t = {}
t.__index = t

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  
  o.transform = go.transform
  o:InitComponent()
  
  o.formationInfo = nil
  o.formationState = nil
  o.onClick = void_delegate.New()
  
  o.spriteNormalBg = ResMgr.instance:LoadSprite('sprite/main_ui/bg_team_1')
  o.spriteSelectBg = ResMgr.instance:LoadSprite('sprite/main_ui/bg_team_2')
  
  o:SetSelect(false)
  return o
end
function t:InitComponent()
  self.goPanel = self.transform:Find('panel').gameObject
  self.goLevelRoot = self.transform:Find('img_level_bg').gameObject
  self.textLevel = self.transform:Find('img_level_bg/text_level'):GetComponent(typeof(Text))
  self.imgBg = self.transform:Find('panel/bg'):GetComponent(typeof(Image))
  self.imgBg.transform:GetComponent(typeof(Button)).onClick:AddListener(function()
      self.onClick:InvokeOneParam(self)
    end)
  self.goSelect = self.transform:Find('img_select').gameObject
  self.goLock = self.transform:Find('image_lock').gameObject
  self.goPosList = {}
  for i = 1,9 do
    self.goPosList[i] = self.goPanel.transform:Find('pos/image'..i).gameObject
  end
  self.btnClick = self.imgBg.transform:GetComponent(typeof(Button))
  self.redPointView = self.transform:Find('image_red'):GetComponent('RedPointView')
end

function t:SetInfo(formationInfo,formationState)
  self.formationInfo = formationInfo
  self.formationState = formationState
  self:Refresh()
end

function t:Refresh()
  self.textLevel.text = string.format(LocalizationController.instance:Get("ui.train_formation_view.formation_level"),self.formationInfo.level)
  
  for k,v in pairs(self.formationInfo.formationData.pos) do
    self.goPosList[k]:SetActive(v)
  end
  self.redPointView:SetId(self.formationInfo.id)
  self:SetState(self.formationState)
end

--设置阵型的状态
function t:SetState(formationState)
  self.formationState = formationState
  if self.formationState == FormationState.InUse then
    self.imgBg.sprite = self.spriteSelectBg
  else
    self.imgBg.sprite = self.spriteNormalBg
  end
  local isLock = self.formationState == FormationState.Locked
  ui_util.SetGrayChildren(self.goPanel.transform,isLock,true)
  self.goLevelRoot:SetActive(not isLock)
  self.goLock:SetActive(isLock)
end

function t:SetSelect(isSelect)
  self.goSelect:SetActive(isSelect)
  if isSelect then
    gamemanager.GetModel('formation_model').RemoveNewFormationTip(self.formationInfo.id)
  end
end

return t