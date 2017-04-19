local t = {}
t.__index=t

local textWorld
local textGuild
local textPrivate

local function Start()
    textWorld=LocalizationController.instance:Get('ui.chat_view.toggle0')
    textGuild=LocalizationController.instance:Get('ui.chat_view.toggle3')
    textPrivate=LocalizationController.instance:Get('ui.chat_view.toggle1')
end

function t.New(transform,type)
    local r={}
    setmetatable(r,t)
    r.transform=transform
    r.onClick=void_delegate.New()
    r:InitComponent(type)
    return r
end

function t:InitComponent(type)
    local tog=self.transform:GetComponent(typeof(Toggle))
    tog.onValueChanged:AddListener(function ()
        if tog.isOn then
            self.onClick:InvokeTwoParam(self,type)
        end
    
    end)
    self.tog=tog
    self.onLabel=self.transform:FindChild('onLabel'):GetComponent(typeof(Text))
    self.note=self.transform:FindChild('note').gameObject

    if type == ChatType.World then
        self.onLabel.text=textWorld
    elseif type == ChatType.Guild then
        self.onLabel.text=textGuild
    elseif type == ChatType.Private then
        self.onLabel.text=textPrivate
    end
end

function t:SetSelect(isOn)
    self.tog.isOn=isOn
end

function t:HasNews(new)
    self.note:SetActive(new)
end

Start()
return t
