local t = {}
local PREFAB_PATH = 'ui/common/common_hero_icon'
local BIG_PREFAB_PATH = 'ui/common/common_hero_big_icon'
local SMALL_PREFAB_PATH = 'ui/common/common_hero_small_icon'

local ui_util = require 'util/ui_util'

t.__index = t

t.spriteStarNormal = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star')
t.spriteStarDefault = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star2_big_disable')



function t.New(transformParent)
  
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  
  local o = t.NewByGameObject(gameObject)
  o.transform:SetParent(transformParent,false)
  
  return o
end

function t.CreateBig(parent)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(BIG_PREFAB_PATH))
    
  local o = t.NewByGameObject(gameObject)
  o.transform:SetParent(parent, false)
  
  return o
end

function t.CreateSmall(parent)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(SMALL_PREFAB_PATH))
    
  local o = t.NewByGameObject(gameObject)
  o.transform:SetParent(parent, false)
  
  return o
end

function t.NewByGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.isSelect = false
  o.roleInfo = nil
  o.gameResData = nil
  o.isPlayer = false
  o.isUsePetHeadIcon = false
  --callback contains one obj
  o.onClick = void_delegate.New()
  
  o:InitComponent()
  
  return o
end

function t:InitComponent()
  self.roleQualityFrame = self.transform:GetComponent(typeof(Image))
  self.imgHead = self.transform:Find('img_head'):GetComponent(typeof(Image))
  self.imgRoleType = self.transform:Find('img_role_type'):GetComponent(typeof(Image))
  self.tranStarRoot = self.transform:Find('stars_root')
  local tname = self.transform:Find('text_name')
  if tname then
     self.textName = tname:GetComponent(typeof(Text))
  end
  self.textLevel = self.transform:Find('text_level'):GetComponent(typeof(Text))
  self.textStrengthenLevel = self.transform:Find('text_strengthen_level'):GetComponent(typeof(Text))
  self.goFormationMark = self.transform:Find('img_in_formation_mark').gameObject
  self.goSelectMark = self.transform:Find('img_selected_tag').gameObject
  
  self.heroBtn = self.transform:GetComponent(typeof(Button))
  self:ResetListener ()
end

function t:SetActive(active)
  self.transform.gameObject:SetActive(active)
end

function t:SetRoleInfo(roleInfo,isPlayer)
  self.roleInfo = roleInfo
  self.isPlayer = self.roleInfo.heroData:IsPlayer()
  self:Refresh()
end

function t:SetGameResData(gameResData,isPlayer)
  
  if isPlayer then
    local playerInfo = require('ui/player/model/player_info')
    self.roleInfo = playerInfo:New(0,gameResData.id,0,0,0,'')
  else
    local heroInfo = require('ui/hero/model/hero_info')
    
    self.roleInfo = heroInfo:New(0,gameResData.id,0,0,gameResData.star,1)
  end
  self.gameResData = gameResData
  self.isPlayer = isPlayer
  self:Refresh()
end
--长按提示
function t:AddRoleDesButton(isLongPress)
  if isLongPress == nil then
    isLongPress = true
  end
  if self.roleInfo.instanceID == 0 then
    LuaInterface.LuaCsTransfer.GetRoleDesButton(self.transform.gameObject, self.roleInfo.heroData.id,self.roleInfo.advanceLevel,true,isLongPress)
  else
    LuaInterface.LuaCsTransfer.GetRoleDesButton(self.transform.gameObject, self.roleInfo.instanceID,self.roleInfo.advanceLevel,false,isLongPress)
  end
end
--查看其他玩家英雄
function t:AddRoleDesButtonByOtherPlayer(isLongPress)
  
  LuaInterface.LuaCsTransfer.GetRoleDesButton(self.transform.gameObject, self.roleInfo,self.roleInfo.heroData:IsPlayer(),isLongPress)

end
function t:SetClickType(ctype)
  LuaInterface.LuaCsTransfer.SetRoleDesBtnType(self.transform.gameObject,1)
end

function t:ShowLv(show)
  self.textLevel.enabled=show
end

function t:UsePetIcon()
  --[[
  self.isUsePetHeadIcon = true
  if self.isPlayer then
    self.imgHead.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.roleInfo:PetHeadIcon())
  end
  ]]--
end

function t:Refresh()
  self.roleQualityFrame.sprite = ui_util.GetRoleQualityFrameSprite(self.roleInfo.heroData.quality)
  --if self.isPlayer and self.isUsePetHeadIcon then
  --if self.isPlayer then
  --  self.imgHead.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.roleInfo:PetHeadIcon())
  --else
    self.imgHead.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.roleInfo:HeadIcon())
  --end
  
  self:RefreshStars()
  
  if self.textLevel ~= nil then
    self.textLevel.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),self.roleInfo.level)
  end
  if self.textStrengthenLevel ~= nil then
    self.textStrengthenLevel.text = gamemanager.GetModel('role_model').GetStrengthenAddShowValueString(self.roleInfo)
  end
  if self.imgRoleType ~= nil then
    self.imgRoleType.sprite = ui_util.GetRoleTypeSmallIconSprite(self.roleInfo.heroData.roleType)
  end
  if self.textName then
    self.textName.text = self.roleInfo.heroData:GetNameWithQualityColor()
  end
  
end

function t:RefreshStars()
  local count = self.tranStarRoot.childCount
  local tranStar
  for i = 1,count do
    tranStar = self.tranStarRoot:GetChild(i-1)
    if i <= self.roleInfo.advanceLevel then
      tranStar:GetComponent(typeof(Image)).sprite = self.spriteStarNormal
    else
      tranStar:GetComponent(typeof(Image)).sprite = self.spriteStarDefault
    end
    --tranStar.gameObject:SetActive(i <= self.roleInfo:MaxAdvanceLevel())
    tranStar.gameObject:SetActive(i <= self.roleInfo.advanceLevel)
  end
end

function t:SetInFormation(isInFormation)
  if self.goFormationMark ~= nil then
    self.goFormationMark:SetActive(isInFormation)
  end
end

function t:ShowLevel(isSelect)
  if self.textLevel ~= nil then
    self.textLevel.gameObject:SetActive(isSelect)
  end
end

function t:SetSelect(isSelect)
  self.isSelect = isSelect
  if self.goSelectMark ~= nil then
    self.goSelectMark:SetActive(isSelect)
  end
end

function t:ResetListener ()
  self.heroBtn.onClick:RemoveAllListeners()
  self.heroBtn.onClick:AddListener(
    function()
      print('instanceId:',self.roleInfo.instanceID,'modelId:',self.roleInfo.heroData.id)
      self.onClick:InvokeOneParam(self)
    end)
end

function t:AddTipsClick()
    self.heroBtn.onClick:AddListener(
    function()
      print('AddTipsClick instanceId:',self.itemInfo.instanceId,'modelId:',self.itemInfo.itemData.id)
      -- uimanager.ShowTipsOfItem(self.itemInfo,self.transform.position,Vector2(100,100))
    end)
end

return t