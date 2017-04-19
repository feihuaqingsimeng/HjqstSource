local t = {}
local name='first_charge_view'
local PREFAB_PATH = 'ui/activity/first_charge_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')

t.transform = nil
t.coretransform = nil

t._BtnClose = nil
t._ItemRoot1 = nil
t._ItemRoot2 = nil
t._BtnAward = nil
t._BtnAwardText = nil
t._RoleRankImage = nil
t._TranModelRoot = nil
t._TextDes = nil

t.type = 0
t.activityId = 0
t.HeroInfo = nil
t.characterEntity = nil
t.tempRewardInfo = {}
t.tranHeroStarsTable = {}

local function Start( ... )
	uimanager.RegisterView(name,t)
end

function t:Open() 
  local asset = LuaInterface.LuaCsTransfer.UIMgrOpen(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)	
	self.transform = asset:GetComponent(typeof(Transform))
  
	self.coretransform = self.transform:FindChild('core')
  self._BtnClose = self.coretransform:FindChild('right/btn_close'):GetComponent(typeof(Button))
  self._BtnClose.onClick:AddListener(function()	self:OnBtnClose() end)
  
  self._ItemRoot1 = self.coretransform:FindChild('right/item_root1')
  self._ItemRoot2 = self.coretransform:FindChild('right/item_root2')
  
  --t._RoleRankImage = self.coretransform:Find('left/img_role_rank_bg/img_role_rank'):GetComponent(typeof(Image))
  --t._TranModelRoot = self.coretransform:Find('left/role_model_anchor/role_model_root')
  
  --local heroStarRoot = ui_util.FindComp(self.coretransform, 'left/stars_root', Transform)
  --for i = 1,6 do t.tranHeroStarsTable[i] = ui_util.FindComp(heroStarRoot, 'img_star_'..i, Transform) end
  
  --t._TextDes = self.coretransform:Find('left/img_des_bg/Scroll View/Viewport/Content/text_des'):GetComponent(typeof(Text))
  self._BtnAward = self.coretransform:FindChild('right/btn_award'):GetComponent(typeof(Button))
  self._BtnAwardText = self.coretransform:FindChild('right/btn_award/Text'):GetComponent(typeof(Text))
  
  self:Init()
end

function t:RecordCurActivity(type, id)
  self.tempRewardInfo.type = type
  self.tempRewardInfo.id = id
end

function t:Init()
  self.type = activity_model:GetFirstChargeType()  
  local eventData = event_data.GetDataByType(self.type)
  if nil == eventData then do return end end
  
  self.activityId = tonumber(eventData.small_id)
  
  self:InitAward()
  self:InitModel()
  self:InitButton()
  self.BindDelegate()
end

function t.BindDelegate()
  activity_ctrl.OnDlegateActivityListResp:AddListener(t.OnMsgActivityList)
  activity_ctrl.OnDlegateActivityUpdateResp:AddListener(t.OnMsgActivityUpdate)
  activity_ctrl.OnDlegateActivityRewardResp:AddListener(t.OnMsgActivityReward) 
end

function t.UnBindDelegate()
  activity_ctrl.OnDlegateActivityListResp:RemoveListener(t.OnMsgActivityList)
  activity_ctrl.OnDlegateActivityUpdateResp:RemoveListener(t.OnMsgActivityUpdate)
  activity_ctrl.OnDlegateActivityRewardResp:RemoveListener(t.OnMsgActivityReward)
end

function t:InitAward()
  local eventData = event_data.GetDataByType(self.type)
  if nil == eventData then do return end end
  
  ui_util.ClearChildren(self._ItemRoot1, true)
  ui_util.ClearChildren(self._ItemRoot2, true)
  
  local awards = string.split(eventData.event_award, ';')
  for i = 1, #awards do    
    local bdata = game_res_data.NewByString(awards[i])
    if i < 4 then common_reward_icon.New(self._ItemRoot1, bdata) end
    if i == 4 then common_reward_icon.New(self._ItemRoot2, bdata) end
  end
end

function t:InitModel()  
  local bData = nil
  
  local eventData = event_data.GetDataByType(self.type)
  if nil == eventData then do return end end
  
  local awards = string.split(eventData.event_award, ';')    
  for i = 1, #awards do    
    local resData = game_res_data.NewByString(awards[i])      
    if i == 4 and resData.type == BaseResType.Hero then bData = resData end
  end
  if nil == bData then do return end end
  
  local heroInfo = require('ui/hero/model/hero_info')    
  t.HeroInfo = heroInfo:New(0, bData.id, 0, 0, bData.star, 1)
  
  t.RefreshRoleRank ()
  LeanTween.delayedCall(0.05, Action(t.RefreshModelDelay))
  
  self:InitDes()
end

function t:InitButton()
  local activityData = activity_model:GetActivityByType(self.type)
  if nil == activityData then do return end end
  
  local state = activityData:GetStateFromId(activityData, self.activityId)
  if state == 0 then 
    	self._BtnAward.onClick:RemoveAllListeners()
      self._BtnAward.onClick:AddListener(function()	self:OnBtnOverTime() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_public_OverTime2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)      
  elseif state == 1 then 
      self._BtnAward.onClick:RemoveAllListeners()
      self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1003_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  elseif state == 2 then 
      self._BtnAward.onClick:RemoveAllListeners()
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1003_btn3')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  elseif state == 3 then 
      self._BtnAward.onClick:RemoveAllListeners()
      self._BtnAward.onClick:AddListener(function()	self:OnBtnNavagation() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1003_btn1')
      ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  end
end

function t:InitDes()
  --self._TextDes.text = LocalizationController.instance:Get(self.HeroInfo.heroData.description)
end

function t.RefreshRoleRank()
  --t._RoleRankImage.sprite = ResMgr.instance:LoadSprite(t.HeroInfo.heroData:GetRankImagePath())
end

function t.RefreshModelDelay()
  --[[t.DespawnCharacter()
  ui_util.ClearChildren(t.tranModelRoot,true)
  t.characterEntity = CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(t.HeroInfo, t._TranModelRoot, true, true)   
  t._TranModelRoot.localRotation = Quaternion.Euler(t.HeroInfo.heroData.home_rotation)  
  AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
  for k,v in pairs(t.tranHeroStarsTable) do  v.gameObject:SetActive(k <= t.HeroInfo.advanceLevel) end]]
end

function t.DespawnCharacter()
  if t.characterEntity then
    PoolController.instance:Despawn(t.characterEntity.name, t.characterEntity)
    t.characterEntity = nil
  end
end

function t.OnMsgActivityList(list)
  t:InitButton()
end

function t.OnMsgActivityUpdate()
  t:InitButton()
end

function t.OnMsgActivityReward()
  local itemData = event_data.GetDataBySmallId(t.tempRewardInfo.type, t.tempRewardInfo.id)
  if nil == itemData then do return end end
  
  if nil == itemData.event_award or itemData.event_award == "" then do return end end 
  local list = string.split(itemData.event_award, ";")
  
  uimanager.ShowTipsAward(list)
  t.tempRewardInfo = {}
end

function t:OnBtnOverTime()
  auto_destroy_tips_view.Open(LocalizationController.instance:Get('event_public_OverTime1'))
end

function t:OnBtnGetAward()
  local activityData = activity_model:GetActivityByType(self.type)
  if nil == activityData then do return end end  
  activity_ctrl.ReqActivityReward(self.type, self.activityId, activityData.subType)
end

function t:OnBtnNavagation()
  gamemanager.GetCtrl('shop_controller').OpenShopView(3)
end

function t:OnBtnClose()
  self:UnBindDelegate()
  t.DespawnCharacter()
  UIMgr.instance:Close(PREFAB_PATH)
  uimanager.CloseView(name)
end

function t.Close() end
Start()
return t