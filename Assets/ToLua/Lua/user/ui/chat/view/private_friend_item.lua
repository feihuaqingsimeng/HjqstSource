local t = {}
t.__index=t

local pv

local function Start()
  textMaster=LocalizationController.instance:Get('ui.consortia_view.chat.master')
  textNotMaster=LocalizationController.instance:Get('ui.consortia_view.chat.notmaster')
end

function t.Init(pview)
	pv=pview
end

function t.New(go)
    local r={}
    setmetatable(r,t)
    r.go=go
    r.onClick=void_delegate.New()
    r:InitComponent()
    return r
end

function t:InitComponent()
    local transform=self.go.transform
    transform:GetComponent(typeof(Button)).onClick:AddListener(function ()
    	self.onClick:InvokeOneParam(self)
    end)
	self.note=transform:FindChild('note').gameObject
    self.lvText=transform:FindChild('lvText'):GetComponent(typeof(Text))
    self.icon=transform:FindChild('icon'):GetComponent(typeof(Image))
    self.nameText=transform:FindChild('nameText'):GetComponent(typeof(Text))
    self.online=transform:FindChild('online'):GetComponent(typeof(Text))
    self.online.text='离线'
end

function t:Setup(data,new)
	data.new=new
	self.data=data
	self.lvText.text='Lv '..data.lv
	self.nameText.text=data.name
	self.icon.sprite=ResMgr.instance:LoadSprite(data.icon)
	if data.select then
		self:SetSelect()
	end
	self:SetOnline(data.online)
	self:SetNews(data.new)
end

function t:Close( ... )
	-- print('content_item close')
	if self.data.select then
		pv.select.gameObject:SetActive(false)
	end
end

function t:SetSelect()
	self.data.select=true
	pv.select:SetParent(self.go.transform)
	pv.select.localPosition=Vector3(-86,4,0)
	pv.select.localScale=Vector3.one

	self:SetNews(false)
	pv.select.gameObject:SetActive(true)
end

function t:SetOnline(online)
	self.online.enabled=not online
	-- print('SetOnline ',online)
end

function t:SetNews(new)
	gamemanager.GetModel('chat_model').SetNew(self.data.name,new)
	self.note:SetActive(new)
end

Start()
return t
