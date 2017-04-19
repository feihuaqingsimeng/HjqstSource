local t = {}

local PREFAB_PATH = 'ui/common/common_equipment_icon'

t.__index = t

t.spriteStarNormal = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star')
t.spriteStarDefault = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star2_big_disable')


--callback contains one obj
local equip_des_button = require('ui/description/equip_des_button')

function t.New(transformParent)
  
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  
  local o = t.NewByGameObject(gameObject)
  o.transform:SetParent(transformParent,false)
  o.isSelect = false
  
  return o
end

function t.NewByGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.equipInfo = nil  
  o.onClick = void_delegate.New()
  
  o:InitComponent()
  
  return o
end

function t:InitComponent()
  self.roleQualityFrame = self.transform:GetComponent(typeof(Image))
  self.imgEquipIcon = self.transform:Find('img_icon'):GetComponent(typeof(Image))
  self.imgRoleType = self.transform:Find('img_role_type_icon'):GetComponent(typeof(Image))
  self.textLevel = self.transform:Find('text_level'):GetComponent(typeof(Text))
  self.textStrengthenLevel = self.transform:Find('text_strengthen_level'):GetComponent(typeof(Text))
  
  self.goNewMarkTransform = self.transform:Find('img_new_mark')
  if self.goNewMarkTransform ~= nil then
    self.goNewMarkTransform.gameObject:SetActive(false)
  end
  
  self.goSelect = self.transform:Find('img_select').gameObject
  self.tranStarRoot = self.transform:Find('star_root')
  
  self.btn = self.transform:GetComponent(typeof(Button))
  self:ResetListener()
end

function t:SetEquipInfo(equipInfo)
  self.equipInfo = equipInfo
  self:Refresh()
  ------------need add equip des button here--------------
end

function t:SetGameResData(gameResData)
  local equip_info = require('ui/equip/model/equip_info')
  self:SetEquipInfo(equip_info.New(0,gameResData.id))
end

function t:Refresh()
  self.roleQualityFrame.sprite = ui_util.GetRoleQualityFrameSprite(self.equipInfo.data.quality)
  self.imgEquipIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.equipInfo:EquipIcon())
  self.imgRoleType.sprite = ui_util.GetRoleTypeSmallIconSprite(self.equipInfo.data.equipmentRoleType)
  self.textLevel.text = ''
  if self.equipInfo.strengthenLevel > 0 then
    self.textStrengthenLevel.text = string.format('+%d',self.equipInfo.strengthenLevel)
  else
    self.textStrengthenLevel.text = ''
  end
  self.textStrengthenLevel.color = ui_util.GetEquipQualityColor(self.equipInfo.data)
  self:SetStar(self.equipInfo.star,self.equipInfo.star)
  
end
--为了处理操蛋的装备继承显示问题
function t:SetStar(starMin,starMax)
  local tran = nil
  for i = 1,6 do
    tran = self.tranStarRoot:GetChild(i-1)
    tran.gameObject:SetActive(i <= starMax)
    if i <= starMin then
      tran:GetComponent(typeof(Image)).sprite = self.spriteStarNormal
    else
      tran:GetComponent(typeof(Image)).sprite = self.spriteStarDefault
    end
  end
end

function t:SetActive(active)
  self.transform.gameObject:SetActive(active)
end
function t:ShowStrengthenLevel(show)
    self.textStrengthenLevel.gameObject:SetActive(show)
end

function t:AddEquipDesButton(isLongPress)
  if isLongPress == nil then
    isLongPress = true
  end
  if self.desButton == nil then
    self.desButton = equip_des_button.New(self.transform,self.equipInfo.data.id)
  end
  self.desButton:SetId(self.equipInfo.data.id)
  self.desButton:IsLongPress(isLongPress)
end

function t:ButtonEnable(enable)
  local btn = self.transform:GetComponent(typeof(Button))
  btn.enabled = enable
end

function t:SetSelect(isSelect)
  self.isSelect = isSelect
  self.goSelect:SetActive(isSelect)
end
function t:ShowSelectMark(isSelect)
  self.goSelect:SetActive(isSelect)
end
function t:SetLevelColor(color)
  self.textLevel.color = color
end

function t:ShowNewMark(isNew)
  if self.goNewMarkTransform ~= nil then
    self.goNewMarkTransform.gameObject:SetActive(isNew)
  end
end

function t:ResetListener ()
  self.btn.onClick:RemoveAllListeners()
  self.btn.onClick:AddListener(function()
    print('instanceId:',self.equipInfo.id,'modelId:',self.equipInfo.data.id)
    self.onClick:InvokeOneParam(self)
    end)
end

return t