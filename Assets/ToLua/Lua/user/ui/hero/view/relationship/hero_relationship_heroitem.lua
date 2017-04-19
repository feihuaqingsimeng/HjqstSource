local t = {}
t.__index=t

function t.Init(pv)
	t.pv=pv
end

function t.New(transform)
	local r={}
	setmetatable(r,t)
	local go = Instantiate(transform)
	r:InitComponent(go.transform)
	return r
end

function t:InitComponent(transform)
	self.transform=transform
	self.mask=transform:FindChild('mask')
	self.maskText=transform:FindChild('mask/mask_Text'):GetComponent(typeof(Text))
	self.hero=require('ui/common_icon/common_hero_icon').New(transform)
	self.hero.onClick:AddListener(function ()
		t.pv.ClickHero(self)
	end)
	self.desBtn=self.hero.transform.gameObject
end

function t:Reset(parent)
	self.transform.gameObject:SetActive(true)
	self.transform:SetParent(parent)
	self.transform.localScale=Vector3.one
	local pos=self.transform.localPosition
	self.transform.localPosition=Vector3(pos.x,pos.y,0)
end

function t:Setup(data)
	self.data=data
	self.hero:SetGameResData(require('ui/game/model/game_res_data').New(0,data[1],0,data[2]),false)
	self.hero:AddRoleDesButton()
	LuaCsTransfer.SetRoleDesBtnType(self.desBtn,1)

	self.hero:ShowLv(false)
	self.hero.transform:SetAsFirstSibling()
	-- self.transform.gameObject:SetActive(true)
end

function t:SetupInfo(info)
	self.hero:SetRoleInfo(info)
	self.hero:ShowLv(false)
	self.hero.transform:SetAsFirstSibling()
end

function t:SetExist()
	self.mask.gameObject:SetActive(false)
end 

function t:SetStarEnough()
	self.normal=true
	self.using=false
	self.notexist=false
	self.mask.gameObject:SetActive(false)
	LuaCsTransfer.SetRoleDesButton(self.desBtn,true)
end

function t:SetStarNotEnough()
	-- self.lackstar=true
	-- self.mask.gameObject:SetActive(true)
	-- self.maskText.text='星数不够'

	self.notexist=true
	self.normal=false
	self.using=false
	self.mask.gameObject:SetActive(true)
	self.maskText.text=LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.nothave')
	LuaCsTransfer.SetRoleDesButton(self.desBtn,false)
end

function t:SetUsing()
	self.using=true
	self.normal=false
	self.notexist=false
	self.mask.gameObject:SetActive(true)
	self.maskText.text=LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.using')
	LuaCsTransfer.SetRoleDesButton(self.desBtn,false)
end

function t:SetNotExist()
	self.notexist=true
	self.using=false
	self.normal=false
	self.mask.gameObject:SetActive(true)
	self.maskText.text=LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.nothave')
	LuaCsTransfer.SetRoleDesButton(self.desBtn,false)
end

function t:Close( ... )
	self.transform.gameObject:SetActive(false)
end

return t