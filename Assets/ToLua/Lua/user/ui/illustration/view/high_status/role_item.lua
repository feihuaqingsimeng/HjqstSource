local t = {}
t.__index = t


t.spriteStarNormal = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star')
t.spriteStarDefault = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star2_big_disable')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  o.transform = transform
  
  o:InitComponent()
  return o
end
function t:Close()
  self:DespawnCharacter()
end
function t:InitComponent()
  self.textTitle = self.transform:Find('head/text_title'):GetComponent(typeof(Text))
  self.imgStars = {}
  for i = 1,6 do
    self.imgStars[i] = self.transform:Find('head/stars_root/star'..i):GetComponent(typeof(Image))
  end
  self.btnCheck = self.transform:Find('head/btn_check'):GetComponent(typeof(Button))
  self.btnCheck.onClick:AddListener(function()
      self:ClickCheckBtnHandler()
    end)
  self.tranModelRoot = self.transform:Find('model_root')
  self.goArrow = self.transform:Find('head/img_arrow').gameObject
end

function t:SetRoleInfo(roleInfo,id)
  self.roleInfo = roleInfo
  self.id = id
  --LuaInterface.LuaCsTransfer.GetRoleDesButton(self.btnCheck.gameObject, self.roleInfo.heroData.id,self.roleInfo.advanceLevel,true,false)
  self:Refresh()
end

function t:SetCheckCallback(callback)
  self.checkClickCallback = callback
end

function t:Refresh()
  self.textTitle.text = string.format( LocalizationController.instance:Get('ui.illustration_view.highstatus.starTitle'),self.roleInfo.advanceLevel-1,self.roleInfo.advanceLevel)
  for k,v in pairs(self.imgStars) do
    if k <= self.roleInfo.advanceLevel then
      v.sprite = t.spriteStarNormal
    else
      v.sprite = t.spriteStarDefault
    end
  end
 
  LeanTween.delayedCall(0.1,Action(function()
        self:RefreshModel()
      end))
  
end
function t:ShowArrow(show)
  self.goArrow:SetActive(show)
end

function t:RefreshModel()
  --model
  self:DespawnCharacter()
  if self.roleInfo.heroData:IsPlayer() then
    self.characterEntity = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(self.roleInfo, self.tranModelRoot, true, true)
  else
    self.characterEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(self.roleInfo,self.tranModelRoot, true, true)
  end
end

function t:DespawnCharacter()
  if self.characterEntity then
    Logic.Pool.Controller.PoolController.instance:Despawn(self.characterEntity.name, self.characterEntity)
    self.characterEntity = nil
  end
end
-----------------------click event--------------------------
function t:ClickCheckBtnHandler()
  if self.checkClickCallback then
    self.checkClickCallback(self.roleInfo,self.id)
  end
end
return t