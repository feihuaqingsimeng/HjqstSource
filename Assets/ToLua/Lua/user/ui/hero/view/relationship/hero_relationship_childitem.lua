local t = {}
t.__index=t

local function Start()

end

function t.Init(pv)
	t.pv=pv
end

function t.New(go)
	local r={}	
	setmetatable(r,t)
	r:InitComponent(go)
	return r
end

function t:InitComponent(go)
	self.go=go
	local transform=go.transform
	
	self.btn=transform:FindChild('Button')	
	self.onClick=self.btn:GetComponent(typeof(Button)).onClick

	self.hero=require('ui/common_icon/common_hero_icon').New(transform:FindChild('hero'))
end

function t:ClickFree()
	--print(self)
	local req={}
	req.id=self.data.id
	req.heroId=self.data.instanceID
	req.activeFlag=0
	req.friends={}
	t.pv.HeroRelationShipReq(req,self)
end

function t:ClickHero()
	print(self)
end

function t:Setup(index,data)
	if data.modelId ==0 then
		print('error','子面板英雄 modelId,instanceID ',data.modelId,data.instanceID)
		return
	end
	self.index=index
	self.data=data
	self.hero.onClick:RemoveAllListener()
	self.hero.onClick:AddListener(function ()
		self:ClickHero()
	end)

	self.onClick:RemoveAllListeners()
	self.onClick:AddListener(function()
		self:ClickFree()
	end)

	self.hero:SetGameResData(require('ui/game/model/game_res_data').New(0,data.modelId,0,data.stars),false)
	self.hero:ShowLv(false)
end


function t:Close( ... )
	self.go:SetActive(false)
end

return t