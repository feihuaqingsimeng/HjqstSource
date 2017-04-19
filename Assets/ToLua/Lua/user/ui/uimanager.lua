Text = UnityEngine.UI.Text
Image = UnityEngine.UI.Image
Slider = UnityEngine.UI.Slider
Button = UnityEngine.UI.Button
Outline=UnityEngine.UI.Outline
Toggle = UnityEngine.UI.Toggle
Transform = UnityEngine.Transform
Dropdown = UnityEngine.UI.Dropdown
GameObject = UnityEngine.GameObject
ScrollRect=UnityEngine.UI.ScrollRect
RectTransform = UnityEngine.RectTransform
GridLayoutGroup = UnityEngine.UI.GridLayoutGroup
LayoutElement = UnityEngine.UI.LayoutElement
PlayerPrefs = UnityEngine.PlayerPrefs
InputField = UnityEngine.UI.InputField
Canvas = UnityEngine.Canvas
Scrollbar = UnityEngine.UI.Scrollbar
Animator = UnityEngine.Animator
AudioClip = UnityEngine.AudioClip
AudioSource = UnityEngine.AudioSource
--c#
UIMgr = Logic.UI.UIMgr
ResMgr = Common.ResMgr.ResMgr
AudioController = Logic.Audio.Controller.AudioController
LuaCsTransfer = LuaInterface.LuaCsTransfer
AnimatorUtil = Common.Animators.AnimatorUtil
CharacterEntity = Logic.Character.CharacterEntity
PoolController = Logic.Pool.Controller.PoolController
EventTriggerDelegate = Common.UI.Components.EventTriggerDelegate
LocalizationController = Common.Localization.LocalizationController
ToggleContent = Common.UI.Components.ToggleContent
TimeController = Common.GameTime.Controller.TimeController
TimeUtil = Common.Util.TimeUtil
ScrollContent = Common.UI.Components.ScrollContent
ScrollContentExpand = Common.UI.Components.ScrollContentExpand
CommonMoveByAnimation = Logic.UI.CommonAnimations.CommonMoveByAnimation
CommonFadeInAnimation = Logic.UI.CommonAnimations.CommonFadeInAnimation
CommonFadeToAnimation = Logic.UI.CommonAnimations.CommonFadeToAnimation
GoodsJumpButton = Logic.UI.GoodsJump.View.GoodsJumpButton
NumberIncreaseAction = Logic.UI.FightResult.View.NumberIncreaseAction
TalkingDataController = Logic.TalkingData.Controller.TalkingDataController

local ASSETOBJ
local ASSETRLT

uimanager={}
uimanager.views = {}

local function Start()
	 -- local ltrace = dofile('ltrace')
	 -- ltrace.getfulltrace(2)
	-- local ts={'23','sdfh',{'shihe','sjeeee'}}
	-- local out={}
	-- table.concatdeep(ts,';',out)
	-- print(table.concat(out))
end

local function mInstantiate()
	ASSETRLT=Object.Instantiate(ASSETOBJ)
end

function Instantiate(src)
	ASSETOBJ=src
	local co=coroutine.create(mInstantiate)
	local flag,msg = coroutine.resume(co)
	if not flag then print(msg) end
	return ASSETRLT
end

--默认带两个按钮
function uimanager.ShowTips(tipsString,callback)
	require('ui/tips/view/confirm_tip_view').Open(tipsString,callback)
end

--一个按钮
function uimanager.ShowTipsOnly(tipsString)
	require('ui/tips/view/common_error_tips_view').Open(tipsString)
end

--奖励列表
function uimanager.ShowTipsAward(list)
	require('ui/tips/view/common_reward_tips_view').CreateOriginal(list)
end

function uimanager.ShowTipsOfItem(info,position,size)
	require('ui/description/common_item_desview').Open(info,position,size)
end
function uimanager.ShowTipsOfEquip(info,position)
	require('ui/description/common_equip_desview').OpenByInfo(info,position)
end
--info:四段式字符串
function uimanager.ShowTipsOfItemOriginal(info,position,size)
	require('ui/description/common_item_desview').Open(require('ui/item/model/item_info').New(0,tonumber(info[2]),tonumber(info[3])),
		position,size)
end

function uimanager.GetView(name)
	if uimanager.views[name] then
		return uimanager.views[name]
	else
		--print('can not get view: '..name)
		return nil
	end
end

function uimanager.RegisterView(name,view)
	--print('register:'..name)
	if not uimanager.views[name] then
		uimanager.views[name]=view
	else
		print('repeat register:'..name) 
	end
end

function uimanager.CloseView(name)
  local view = uimanager.GetView(name)
  if(view) then
    uimanager.views[name] = nil
    if(view.Close ~= nil) then
      view.Close()
    else
      Debugger.LogError('can not find close function from '..name)
    end
  end
end

function uimanager.DealWithTime(from,to)
	local f1=string.sub(from,1,10)
	local t1=string.sub(to,1,10)
	if f1 == t1 then
		return from,to
	else
		if string.sub(t1,1,4)=='2050' then 
			return LuaInterface.LuaCsTransfer.LocalizationGet('event_public_forever')	
		else
			return f1,t1
		end
	end
end

Start()
