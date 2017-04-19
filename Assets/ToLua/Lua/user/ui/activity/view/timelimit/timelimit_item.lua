local t={}
local name='activityfirstchargeitem'
local textNum
local icon

local function Start( ... )
	
end

function t.OnClick( ... )
	-- body
		for i,v in ipairs(t.info) do
			print(v)
		end
end

local function Init()
	if textNum == nil then
		textNum=t.item:FindChild('Text'):GetComponent(typeof(Text))
	end
	if icon == nil then
		icon=t.item:FindChild('Image'):GetComponent(typeof(Image))
		sprite=Common.ResMgr.ResMgr.instance:
		LoadSprite(LuaInterface.LuaCsTransfer.GetIcon(tonumber(t.info[1]),tonumber(t.info[2])))
		if sprite then 
			icon.sprite=sprite
		end
	end
	btn=t.item:GetComponent(typeof(Button))
	btn.onClick:RemoveAllListeners()
	btn.onClick:AddListener(t.OnClick)

	textNum.text='+'..t.info[3]
end

function t.Setup(item,info)
	-- body
	t.item=item
	t.info=info
	Init()
end

Start()
return t