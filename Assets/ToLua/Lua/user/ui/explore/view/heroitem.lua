local t={}
local name='heroitem'

local function Start( ... )
	t.__index=t
end

function t.New( ... )
	local r={}
	setmetatable(r,t)
	return r
end

function t:Init(transform)
	self.transform=transform
	hero=require('ui/common_icon/common_hero_icon')
	self.heroitem=hero.New(transform)
	self.onClick=self.heroitem.transform:GetComponent(typeof(Button)).onClick
end

function t:Setup(item,id)
	info=gamemanager.GetModel('hero_model').GetHeroInfo(id)
	self.item=item
	self.onClick:RemoveAllListeners()
	self.onClick:AddListener(function ()
		self.heroitem.onClick:InvokeOneParam(item)
	end)
	self.heroitem:SetRoleInfo(info,false)
	if self.transform then
		self.transform.gameObject:SetActive(true)
	end
end

function t:Close( ... )
	if self.transform then
		self.transform.gameObject:SetActive(false)
	end
end

Start()
return t