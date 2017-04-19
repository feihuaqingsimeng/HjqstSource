local t = {}
local PREFAB_PATH = 'ui/common/common_skill_icon'

t.__index = t


function t.New(transform)
  
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  local o = t.BindTransform(gameObject:GetComponent(typeof(Transform)))
  
  return o
end
function t.BindTransform(tran)
  local o = {}
  setmetatable(o,t)
  
  o.skillData = nil
  local skill_des_button = require('ui/description/skill_des_button')
  o.skillDesButton = skill_des_button.BindTransform(tran)
  o.transform = tran
  o:InitComponent()
  return o
end
function t:InitComponent()
  self.imgSkillIcon = self.transform:GetComponent(typeof(Image))
  local tranType = self.transform:Find('img_type')
  if tranType ~= nil then
    self.imgTypeIcon = tranType:GetComponent(typeof(Image))
  end
  local tranTypeLeft = self.transform:Find('img_type_left')
  if tranTypeLeft ~= nil then
    self.imgTypeIconLeft = tranTypeLeft:GetComponent(typeof(Image))
  end
end

function t:Refresh()
  
  if self.isPassive then
      self.transform.parent.gameObject:SetActive(self.skillData ~= nil)
  else
      self.transform.gameObject:SetActive(self.skillData ~= nil)
  end
  
  if self.skillData == nil then
    if self.imgTypeIcon ~= nil then
      self.imgTypeIcon.gameObject:SetActive(false)
    end
    --self.imgSkillIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/skill/icon_skill_none')
  else
    self.imgSkillIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.skillData:SkillIcon())
    if self.imgTypeIcon ~= nil then
      local sp = ResMgr.instance:LoadSprite(self.skillData:DesTypeIcon())
      if sp ~= nil then
        self.imgTypeIcon.sprite = sp
      end
      self.imgTypeIcon.gameObject:SetActive(sp ~= nil)
    end
    if self.imgTypeIconLeft ~= nil then
      local sp = ResMgr.instance:LoadSprite(self.skillData:DesTypeIcon2())
      if sp ~= nil then
        self.imgTypeIconLeft.sprite = sp
      end
      self.imgTypeIconLeft.gameObject:SetActive(sp ~= nil)
    end
  end
end

--技能id，英雄星级，英雄最小星级，是否是被动
function t:SetSkillData(dataId,star,starMinLevel,isPassive)
  self.id=  dataId
  self.isPassive = isPassive
  self.skillData = gamemanager.GetData('skill_data').GetDataById(dataId)
  self:Refresh()
  self.skillDesButton:SetSkillId(dataId,star,starMinLevel)
end
function t:LockSkill()
  self.skillData = nil
  self:Refresh()
  self.skillDesButton:SetSkillId(0,0,0)
end
return t