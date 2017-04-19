local t={}
local name='exploreitem2'
local textNum
local icon

local function Start( ... )
	t.__index=t
end

function t.New(parent)
	local r={}
	setmetatable(r,t)
	r.item=require('ui/common_icon/common_item_icon').New(parent)
	r.item.onClick:AddListener(t.OnClick)
	return r
end

function t.OnClick(item)
	--print('onclick')
	uimanager.ShowTipsOfItem(item.itemInfo,item.transform.position,Vector2(100,100))
end

-- local function Init()
-- 	if textNum== nil then
-- 		textNum=t.item:Find('Text'):GetComponent(typeof(Text))
-- 		icon=t.item:FindChild('Image'):GetComponent(typeof(Image))
-- 		sprite=Common.ResMgr.ResMgr.instance:
-- 		LoadSprite(LuaInterface.LuaCsTransfer.GetIcon(tonumber(t.info[1]),tonumber(t.info[2])))
-- 		if sprite then 
-- 			icon.sprite=sprite
-- 		end
-- 	end
	
-- 	btn=t.item:GetComponent(typeof(Button))
-- 	btn.onClick:RemoveAllListeners()
-- 	btn.onClick:AddListener(t.OnClick)
-- 	textNum.text='+'..t.info[3]
-- 	t.item.gameObject:SetActive(true)
-- end

-- function t.SetTrans(item)
-- 	t.item=item	
-- end

-- function t.SetInfo(info)
-- 	-- body
-- 	t.info=info
-- 	Init()
-- end

function t:Setup(data)
	self.data=data
	local rd=require('ui/game/model/game_res_data')
	self.item:SetGameResData(rd.NewByString(data))
	self.item:SetActive(true)
end

function t:Show(show)
	self.item:SetActive(show)
end

function t:Close()
	self.item.onClick:RemoveAllListener()
end

Start()
return t