local t = {}
t.__index = t

local cardBackQualitySpritePathDic = {}
cardBackQualitySpritePathDic[Quality.White] = 'sprite/main_ui/image_card_green'
cardBackQualitySpritePathDic[Quality.Green] = 'sprite/main_ui/image_card_green'
cardBackQualitySpritePathDic[Quality.Blue] = 'sprite/main_ui/image_card_blue'
cardBackQualitySpritePathDic[Quality.Purple] = 'sprite/main_ui/image_card_purple'
cardBackQualitySpritePathDic[Quality.Orange] = 'sprite/main_ui/image_card_orange'
cardBackQualitySpritePathDic[Quality.Red] = 'sprite/main_ui/image_card_red'

local qualityFrameSpriteDic = {}
qualityFrameSpriteDic[Quality.White] = 'sprite/main_ui/image_card_2_green'
qualityFrameSpriteDic[Quality.Green] = 'sprite/main_ui/image_card_2_green'
qualityFrameSpriteDic[Quality.Blue] = 'sprite/main_ui/image_card_2_blue'
qualityFrameSpriteDic[Quality.Purple] = 'sprite/main_ui/image_card_2_purple'
qualityFrameSpriteDic[Quality.Orange] = 'sprite/main_ui/image_card_2_orange'
qualityFrameSpriteDic[Quality.Red] = 'sprite/main_ui/image_card_2_red'

function t.NewByGameObject(gameObject, commonCardInfo)
  local o = {}
  setmetatable(o, t)
  
  o.transform = gameObject.transform
  o:InitComponent()
  o:SetCommonCardInfo(commonCardInfo)
  
  return o
end

function t:InitComponent ()
  self.clickable = true
  self.isTurnOverStarted = false
  self.isTurnOverFinished = false
  self.OnTurnOverStartDelegate = void_delegate.New()
  self.OnTurnOverCompleteDelegate = void_delegate.New()
  self.OnShowCompleteDelegate = void_delegate.New()
  self.turnOverbutton = self.transform:Find('btn_turn_over'):GetComponent(typeof(Button))
  self.turnOverbutton.onClick:AddListener(function ()
      self:Click()
      end)
  
  self.cardBackTransform = self.transform:Find('img_card_back')
  self.cardBackImage = self.cardBackTransform:GetComponent(typeof(Image))
  
  self.cardFrameTransform = self.transform:Find('img_card_frame')
  self.cardFrameTransform.gameObject:SetActive(false)
  
  self.qualityFrameImage = self.transform:Find('img_card_frame/image_quality_frame'):GetComponent(typeof(Image))
  self.iconImage = self.transform:Find('img_card_frame/image_icon'):GetComponent(typeof(Image))
  self.pieceIconImage = self.transform:Find('img_card_frame/img_piece_icon'):GetComponent(typeof(Image))
  self.pieceIconImage.gameObject:SetActive(false)
  self.roleTypeIconImage = self.transform:Find('img_card_frame/img_role_type_icon'):GetComponent(typeof(Image))
  self.roleTypeIconImage.gameObject:SetActive(false)
  self.starsRootTransform = self.transform:Find('img_card_frame/stars_root')
  self.starsRootTransform.gameObject:SetActive(false)
  self.starImages = {}
  for i = 1, 6 do
    self.starImages[i] = self.starsRootTransform:Find('star_'..i):GetComponent(typeof(Image))
  end
  self.textCount = self.transform:Find('img_card_frame/text_count'):GetComponent(typeof(Text))
  self.textCount.gameObject:SetActive(false)
  self.nameText = self.transform:Find('img_card_frame/text_name'):GetComponent(typeof(Text))
  self.desButtonTransform = self.transform:Find('img_card_frame/btn_des')
end

function t:SetCommonCardInfo (commonCardInfo)
  self.commonCardInfo = commonCardInfo
  local quality = Quality.White
  if self.commonCardInfo.baseResType == BaseResType.Hero then
    self:RefreshAsHeroCard()
    quality = self.commonCardInfo.heroData.quality
  elseif self.commonCardInfo.baseResType == BaseResType.Equipment then
    self:RefreshAsEquipmentCard()
    quality = self.commonCardInfo.data.quality
  elseif self.commonCardInfo.baseResType == BaseResType.Item then
    self:RefreshAsItemCard()
    quality = self.commonCardInfo.itemData.quality
  end
  --self.cardBackImage.sprite = ResMgr.instance:LoadSprite(cardBackQualitySpritePathDic[quality])
  self.cardBackImage.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/image_card_red')
end

function t:RefreshAsHeroCard ()
  local quality = self.commonCardInfo.heroData.quality
  self.qualityFrameImage.sprite = ResMgr.instance:LoadSprite(qualityFrameSpriteDic[quality])
  self.iconImage.sprite = ResMgr.instance:LoadSprite(self.commonCardInfo:HeadIcon())
  self.roleTypeIconImage.sprite = ui_util.GetRoleTypeSmallIconSprite(self.commonCardInfo.heroData.roleType)
  self.roleTypeIconImage.gameObject:SetActive(true)
  for k, v in ipairs(self.starImages) do
    v.gameObject:SetActive(k <= self.commonCardInfo.advanceLevel)
  end
  self.starsRootTransform.gameObject:SetActive(true)
  local heroName = LocalizationController.instance:Get(self.commonCardInfo.heroData.name)
  self.nameText.text = ui_util.FormatStringWithinQualityColor(quality, heroName)
  
  LuaInterface.LuaCsTransfer.GetRoleDesButton(self.desButtonTransform.gameObject, self.commonCardInfo.heroData.id,self.commonCardInfo.advanceLevel, true, false)
end

function t:RefreshAsEquipmentCard ()
  local quality = self.commonCardInfo.data.quality
  self.qualityFrameImage.sprite = ResMgr.instance:LoadSprite(qualityFrameSpriteDic[quality])
  self.iconImage.sprite = ResMgr.instance:LoadSprite(self.commonCardInfo.data:IconPath())
  self.roleTypeIconImage.sprite = ui_util.GetRoleTypeSmallIconSprite(self.commonCardInfo.data.equipmentRoleType)
  self.roleTypeIconImage.gameObject:SetActive(true)
  
  local equipmentName = LocalizationController.instance:Get(self.commonCardInfo.data.name)
  self.nameText.text = ui_util.FormatStringWithinQualityColor(quality, equipmentName)
  
  local equip_des_button = require('ui/description/equip_des_button')
  local desButton = equip_des_button.New(self.transform, self.commonCardInfo.data.id)
  desButton:IsLongPress(false)
end

function t:RefreshAsItemCard ()
  local quality = self.commonCardInfo.itemData.quality
  self.qualityFrameImage.sprite = ResMgr.instance:LoadSprite(qualityFrameSpriteDic[quality])
  local itemName = LocalizationController.instance:Get(self.commonCardInfo.itemData.name)
  self.nameText.text = ui_util.FormatStringWithinQualityColor(quality, itemName)
  
  if self.commonCardInfo.itemData.type == ItemType.HeroPiece then
    local heroPieceData = gamemanager.GetData('piece_data').GetDataById(self.commonCardInfo.itemData.id)
    local hero_info = require('ui/hero/model/hero_info')
    local heroInfo = hero_info:NewByGameResData(heroPieceData:GetHeroGameResData())
    self.iconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(heroInfo:HeadIcon())
    self.pieceIconImage.gameObject:SetActive(true)
    self.roleTypeIconImage.sprite = heroInfo:RoleTypeIconSprite()
    self.roleTypeIconImage.gameObject:SetActive(true)
  elseif self.commonCardInfo.itemData.type == ItemType.EquipPiece then
    local pieceData = gamemanager.GetData('piece_data').GetDataById(self.commonCardInfo.itemData.id)
    local equipData = gamemanager.GetData('equip_data').GetDataById(pieceData:GetEquipGameResData().id)
    self.iconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(equipData:IconPath())
    self.pieceIconImage.gameObject:SetActive(true)
  else
    self.iconImage.sprite = ResMgr.instance:LoadSprite(self.commonCardInfo.itemData:IconPath())
  end

  self.textCount.text = self.commonCardInfo:Count()
  self.textCount.gameObject:SetActive(true)

  LuaInterface.LuaCsTransfer.GetItemDesButton(self.desButtonTransform.gameObject, self.commonCardInfo.itemData.id, false)
end

function t:TurnOver ()
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 0, 0.2)
  ltDescr:setOnComplete(Action(function ()
      self:TurnOverComplete()
    end))
  self.OnTurnOverStartDelegate:InvokeOneParam(self.commonCardInfo)
end

function t:TurnOverComplete ()
  self.cardBackTransform.gameObject:SetActive(false)
  self.cardFrameTransform.gameObject:SetActive(true)
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 1, 0.2)
  ltDescr:setOnComplete(Action(function ()
    self:ShowComplete ()
  end))
  self.OnTurnOverCompleteDelegate:InvokeOneParam(self.commonCardInfo)
end

function t:ShowComplete ()
  self.isTurnOverFinished = true
  self.OnShowCompleteDelegate:Invoke()
end

function t:TurnOverAfter (delay)
  if not self.isTurnOverStarted then
    LeanTween.delayedCall(delay, Action(function ()
      self:TurnOver ()
    end))
    self.isTurnOverStarted = true
  end
end

--[[ UI EVENT HANDLERS ]]--
function t:Click ()
  if not self.clickable then
    return
  end
  self:TurnOverAfter(0)
  self.turnOverbutton.gameObject:SetActive(false)
end
--[[ UI EVENT HANDLERS ]]--
return t