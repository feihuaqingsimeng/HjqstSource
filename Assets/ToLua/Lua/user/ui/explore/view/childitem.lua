local t={}
local name='childitem'
local ctrl
local item
local container = {}

local function Start( ... )
	t.__index=t
end

function t.New(go)
	local r={}
	setmetatable(r,t)
	r:Init(go)
	return r
end

function t:Init(go)
	self.go=go
end

function t:ClickSelect()
	print(self.heroitem.roleInfo.instanceID)
end

function t:DoSelect()
	if self.invalid then return end

	if self.selected then
		self:UnSelect()
	else
		self:Select()
	end
end

function t:Invalid( ... )
	self.invalid=true
	self.selectgo:SetActive(false)
	self.invalidgo:SetActive(true)
end

function t:Explored( ... )
	self:Invalid()
end
function t:Exploring( ... )
	self:Invalid()
end

function t:Select( ... )
	self.selected=true
	self.invalid=false
	self.selectgo:SetActive(true)
	self.invalidgo:SetActive(false)
end

function t:UnSelect( ... )
	self.selected=false
	self.invalid=false
	self.selectgo:SetActive(false)
	self.invalidgo:SetActive(false)
end

function t:Setup(info)
	if not self.heroitem then
		hero=require('ui/common_icon/common_hero_icon')
		self.heroitem=hero.New(self.go.transform)
		--self.heroitem.onClick:AddListener(self.ClickSelect)
		self.onClick=self.heroitem.transform:GetComponent(typeof(Button)).onClick
		self.heroitem.transform:SetAsFirstSibling()
		self.selectgo=self.go.transform:FindChild('select').gameObject
		self.invalidgo=self.go.transform:FindChild('invalid').gameObject
	end
	self.onClick:RemoveAllListeners()
	self.onClick:AddListener(function ()
		--print('---'..info.instanceID)
		self.DoSelect(self)
		self.heroitem.onClick:InvokeOneParam(self)
	end)
	self.heroitem:SetRoleInfo(info,false)
end

function t:Close()
	-- for i=1,#using do 
	-- 	it=using[i]
	-- 	it.gameObject:SetActive(false)
	-- 	using[i]=nil
	-- 	table.insert(notuse,it)
	-- end
	t.opened=false
	--self.transform.gameObject:SetActive(false)
	LuaInterface.LuaCsTransfer.UIMgrOpen('ui/main/main_view',0,0)
end

Start()
return t