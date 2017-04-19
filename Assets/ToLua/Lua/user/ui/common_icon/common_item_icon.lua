local t = {}
local PREFAB_PATH = 'ui/common/common_item_icon_lua'
t.__index = t



function t.New(parent)
 
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  
  local o = t.NewByGameObject(gameObject)
  o.transform:SetParent(parent,false)
  
  return o
end

function t.NewByGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  
  --callback contains one obj
  o.onClick = void_delegate.New()
  o.isSelect = false
  o:InitComponent()
  return o
end
function t:InitComponent()
  self.itemQualityFrame = self.transform:GetComponent(typeof(Image))
  self.imgIcon = self.transform:Find('img_icon'):GetComponent(typeof(Image))
  self.textCount = self.transform:Find('text_count'):GetComponent(typeof(Text))
  
  local tranImgPieceMark = self.transform:Find('img_piece_mark')
  if tranImgPieceMark ~= nil then
    self.pieceMarkImage = tranImgPieceMark:GetComponent(typeof(Image))
  end
  
  self.tranStarsRoot = self.transform:Find('stars_root')
  self.starImages = {}
  for i = 1, 6 do
    local starImage = self.tranStarsRoot:Find('img_star_'..i):GetComponent(typeof(Image))
    table.insert(self.starImages, starImage)
  end
  
  local tranImgRoleType = self.transform:Find('img_role_type')
  if tranImgRoleType ~= nil then
    self.roleTypeIconImage = tranImgRoleType:GetComponent(typeof(Image))
  end
  
  self.goNewMarkTransform = self.transform:Find('img_new_mark')
  if self.goNewMarkTransform ~= nil then
    self.goNewMarkTransform.gameObject:SetActive(false)
  end
  self.goSelect = self.transform:Find('img_select').gameObject
  self.goSelect:SetActive(false)
  
  self.btn = self.transform:GetComponent(typeof(Button))
  self:ResetListener ()
end
function t:SetActive(active)
  self.transform.gameObject:SetActive(active)
end
function t:SetItemInfo(itemInfo)
  self.itemInfo = itemInfo
  
  self:Refresh()
  self:AddItemDesButton(true)
end

function t:SetGameResData(gameResData)
  local itemInfo = require('ui/item/model/item_info')

  if (gameResData.type ~= BaseResType.Hero and gameResData.type ~= BaseResType.Equipment and gameResData.type ~= BaseResType.Item ) then
      local data = gamemanager.GetData('item_data').GetBasicResItemByType(gameResData.type)
      if data then
        itemInfo = itemInfo.New(0,data.id,gameResData.count)
      end
  else
        itemInfo = itemInfo.New(0,gameResData.id,gameResData.count)
  end
  self:SetItemInfo(itemInfo)
end

function t:SetItemDataId(itemDataId)
   local itemInfo = require('ui/item/model/item_info')
  itemInfo = itemInfo.New(0,itemDataId,0)
  self:SetItemInfo(itemInfo)
end
function t:AddItemDesButton(isLongPress)
  if isLongPress == nil then
    isLongPress = true
  end
  LuaInterface.LuaCsTransfer.GetItemDesButton(self.transform.gameObject, self.itemInfo.itemData.id,isLongPress)
end
function t:Refresh()
  self.itemQualityFrame.sprite = ui_util.GetItemQualityFrameSprite(self.itemInfo.itemData.quality)
  if self.itemInfo.itemData.type == ItemType.HeroPiece then
    local heroPieceData = gamemanager.GetData('piece_data').GetDataById(self.itemInfo.itemData.id)
    local hero_info = require('ui/hero/model/hero_info')
    local heroInfo = hero_info:NewByGameResData(heroPieceData:GetHeroGameResData())
    self.imgIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(heroInfo:HeadIcon())
    
    if self.pieceMarkImage ~= nil then
      self.pieceMarkImage.gameObject:SetActive(true)
    end
    if self.tranStarsRoot ~= nil then
      local spriteStarNormal = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star')
      local spriteStarDefault = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star2_big_disable')

      for k, v in ipairs(self.starImages) do
        if k <= heroInfo.advanceLevel then
          v.sprite = spriteStarNormal
        else
          v.sprite = spriteStarDefault
        end
        v.gameObject:SetActive(k <= heroInfo:MaxAdvanceLevel())
      end
      --self.tranStarsRoot.gameObject:SetActive(self.itemInfo.itemData.star > 0)
      self.tranStarsRoot.gameObject:SetActive(false)
    end
    if self.roleTypeIconImage ~= nil then
      self.roleTypeIconImage.sprite = heroInfo:RoleTypeIconSprite()
      self.roleTypeIconImage.gameObject:SetActive(true)
    end
  elseif self.itemInfo.itemData.type == ItemType.EquipPiece then
    local pieceData = gamemanager.GetData('piece_data').GetDataById(self.itemInfo.itemData.id)
    local equipData = gamemanager.GetData('equip_data').GetDataById(pieceData:GetEquipGameResData().id)
    self.imgIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(equipData:IconPath())
    
    if self.tranStarsRoot ~= nil then
      self.tranStarsRoot.gameObject:SetActive(false)
    end
    if self.pieceMarkImage ~= nil then
      self.pieceMarkImage.gameObject:SetActive(true)
    end
    if self.roleTypeIconImage ~= nil then
      self.roleTypeIconImage.sprite = equipData:RoleTypeIconSprite()
      self.roleTypeIconImage.gameObject:SetActive(true)
    end
  else
    self.imgIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.itemInfo:ItemIcon())
    
    if self.pieceMarkImage ~= nil then
      self.pieceMarkImage.gameObject:SetActive(false)
    end
    
    if self.tranStarsRoot ~= nil then
      self.tranStarsRoot.gameObject:SetActive(false)
    end
    
    if self.roleTypeIconImage ~= nil then
      self.roleTypeIconImage.gameObject:SetActive(false)
    end
  end
  
  local count = self.itemInfo:Count()
  --if count < 1 then count = 1 end
  self.textCount.text = tostring(count)
   if count >= 1 then
     self.textCount.gameObject:SetActive(true)
   else
     self.textCount.gameObject:SetActive(false)
   end
end
function t:SetSelect(isSelect)
  self.goSelect:SetActive(isSelect)
  self.isSelect = isSelect
end

function t:ShowCount(isShow)
  self.textCount.gameObject:SetActive(isShow)
end
function t:ShowNewMark(isNew)
  if self.goNewMarkTransform ~= nil then
    self.goNewMarkTransform.gameObject:SetActive(isNew)
  end
end
function t:SetCount(countString,color)
  self:ShowCount(true)
  self.textCount.text = countString
  if color ~= nil then
    self.textCount.color = color
  end
end

function t:ResetListener ()
  self.btn.onClick:RemoveAllListeners()
  self.btn.onClick:AddListener(
    function()
       --print('instanceId:',self.itemInfo.instanceId,'modelId:',self.itemInfo.itemData.id)
      self.onClick:InvokeOneParam(self)
    end)
end

return t