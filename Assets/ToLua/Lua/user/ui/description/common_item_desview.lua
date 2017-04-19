local tips={}
local PREFAB_PATH = "ui/description/common_item_description_view_lua"

local _defaultContentLineHeight=25
local _defaultBorderX=10
local textTitle
local iconRoot
local textDes
local textFrom
local textUse
local rootPanel
local bottomRoot
local itemInfo
local worldPos
local sizeDelta 
local originSizeDelta

local function Start( ... )
end

function tips.Open(info,pos,sizeDelta)
  local t = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,0).transform
  textTitle=t:FindChild('core/frame/text_title'):GetComponent(typeof(Text))
  iconRoot=t:FindChild('core/frame/icon_root')
  textDes=t:FindChild('core/frame/text_des'):GetComponent(typeof(Text))
  textFrom=t:FindChild('core/frame/bottom_root/text_from'):GetComponent(typeof(Text))
  textUse=t:FindChild('core/frame/bottom_root/text_use'):GetComponent(typeof(Text))
  rootPanel=t:FindChild('core/frame'):GetComponent(typeof(RectTransform))
  bottomRoot=t:FindChild('core/frame/bottom_root'):GetComponent(typeof(RectTransform))
  t:FindChild('core/btn_full_close'):GetComponent(typeof(Button)).onClick:AddListener(tips.Close)
  t:FindChild('core/frame'):GetComponent(typeof(Button)).onClick:AddListener(tips.Close)
  originSizeDelta = rootPanel.sizeDelta
  _defaultContentLineHeight = textUse.preferredHeight

  tips.transform=t
  tips.SetData(info,pos,sizeDelta)
end

function tips.SetData(info,pos,sizedelta)
  itemInfo = info
  worldPos = pos
  sizeDelta = sizedelta
  tips.co=coroutine.start(tips.RefreshCoroutine)
end

function tips.RefreshCoroutine()
  rootPanel.gameObject:SetActive(false)
  coroutine.step()

  textTitle.text = LocalizationController.instance:Get(itemInfo.itemData.name)
  textDes.text = LocalizationController.instance:Get(itemInfo.itemData.des)

  textFrom.text = LuaCsTransfer.GetDropDataDes(BaseResType.Item,itemInfo.itemData.id)

  local icon = require('ui/common_icon/common_item_icon').New(iconRoot)
  icon:SetItemInfo(require('ui/item/model/item_info').New(0,itemInfo.itemData.id,0))

  --maybe not necessary
  --icon.GetComponent<ItemDesButton>().enabled = false

  tips.RefreshSpecialUse()
  local useDeltaHeight = 0
  if not textUse.text or textUse.text == '' then
    useDeltaHeight=0-_defaultContentLineHeight
  else
    useDeltaHeight=textUse.preferredHeight-_defaultContentLineHeight
  end
  local fromDeltaHeight = 0
  if not textFrom.text or textFrom.text == '' then
    fromDeltaHeight=0-_defaultContentLineHeight
  else
    fromDeltaHeight=textFrom.preferredHeight-_defaultContentLineHeight
  end

  local deltaHeight = useDeltaHeight + fromDeltaHeight
  textFrom.transform.localPosition =textFrom.transform.localPosition+ Vector3(0, useDeltaHeight)
  rootPanel.sizeDelta = Vector2(originSizeDelta.x,originSizeDelta.y+deltaHeight)

  local screenHalfHeight = UIMgr.instance.designResolution.y/2
  local localPosition = tips.transform:InverseTransformPoint(worldPos)
  local x = 0
  local y = localPosition.y
  if localPosition.x>0 then
    x = localPosition.x-sizeDelta.x/2-rootPanel.sizeDelta.x/2-_defaultBorderX
  else
    x = localPosition.x+sizeDelta.x/2+rootPanel.sizeDelta.x/2+_defaultBorderX
  end

  if localPosition.y<rootPanel.sizeDelta.y/2-screenHalfHeight then
    y = rootPanel.sizeDelta.y/2-screenHalfHeight
  end

  if localPosition.y>screenHalfHeight-rootPanel.sizeDelta.y/2 then
    y = screenHalfHeight - rootPanel.sizeDelta.y/2
  end
  localPosition = Vector3(x,y)
  rootPanel.localPosition = localPosition
  rootPanel.gameObject:SetActive(true)
end

function tips.RefreshSpecialUse()
  local heroTypeList = itemInfo.itemData.hero_type
  local count = #heroTypeList
  if count == 0 then
    textUse.gameObject:SetActive(false)
  else
    local typeName = ''
    for i = 1,count do
      if i == 1 then
        typeName = ui_util.GetRoleTypeName(heroTypeList[i])
      else 
        typeName = string.format("%s、%s",typeName,ui_util.GetRoleTypeName(heroTypeList[i]))
      end
    end
    textUse.text = string.format(LocalizationController.instance:Get("ui.common_des_view.use_lua"),typeName)
  end
  textUse.gameObject:SetActive(true)
end

function tips.Close()
  coroutine.stop(tips.co)
  UIMgr.instance:Close(PREFAB_PATH)
end

Start()
return tips