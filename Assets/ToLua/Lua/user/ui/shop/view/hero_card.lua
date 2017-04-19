local t = {}
t.__index = t

local commonHeroIcon = require('ui/common_icon/common_hero_icon')

function t.NewByGameObject(gameObject, heroInfo)
  local o = {}
  setmetatable(o, t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o:InitComponent()
  o:SetHeroInfo(heroInfo)
  
  return o
end

function t:InitComponent ()
  self.isTurnOverStarted = false
  self.isTurnOverFinished = false
  self.OnShowFinishedDelegate = void_delegate.New()
  self.turnOverbutton = self.transform:Find('btn_turn_over'):GetComponent(typeof(Button))
  self.turnOverbutton.onClick:AddListener(function ()
      self:Click()
      end)
  
  self.frameImage = self.transform:GetComponent(typeof(Image))
  self.frameImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_head_box_big_back')
  self.heroIconTransform = self.transform:Find('common_hero_big_icon')
  self.heroIconTransform.gameObject:SetActive(false)
  self.heroIcon = commonHeroIcon.NewByGameObject(self.heroIconTransform)
  self.heroIconFrameImage = self.heroIconTransform:GetComponent(typeof(Image))
  self.normalEffect = self.transform:Find('ui_fanka_shilian')
  self.normalEffect.gameObject:SetActive(false)
  self.specialEffect = self.transform:Find('ui_fanka_shilian_good')
  self.specialEffect.gameObject:SetActive(false)
  self.whiteFrameAnimation = self.transform:Find('img_white')
end

function t:SetHeroInfo (heroInfo)
  self.heroInfo = heroInfo
  self.heroIcon:SetRoleInfo(heroInfo, false)
  

  if heroInfo.heroData.starMax >= 6 then
    self.frameImage.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_head_box_big_back2')
  else
    self.frameImage.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_head_box_big_back')
  end
end

function t:TurnOver ()
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 0, 0.1)
  ltDescr:setOnComplete(Action(function ()
      self:TurnOverComplete()
    end))
  
  if self.heroInfo.heroData.starMax >= 6 then
    self.specialEffect.gameObject:SetActive(true)
  else
    self.normalEffect.gameObject:SetActive(true)
  end
end

function t:TurnOverComplete ()
  self.heroIconTransform .gameObject:SetActive(true)
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 1, 0.1)
  ltDescr:setOnComplete(Action(function ()
    self:OnShowFinished()
  end))
end


function t:OnShowFinished ()
  self.isTurnOverFinished = true
  self.OnShowFinishedDelegate:Invoke()
end

function t:TurnOverAfter (delay)
  if not self.isTurnOverStarted then
    LeanTween.delayedCall(delay, Action(function ()
        self:TurnOver ()
      end))
    self.isTurnOverStarted = true
  end
end

-- [[ UI event handlers ]] --
function t:Click ()
  self:TurnOverAfter(0)
  
end
-- [[ UI event handlers ]] --
return t