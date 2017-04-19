local t = {}
local PREFAB_PATH = 'ui/hero_relationship/hero_relationship_view'
local name = PREFAB_PATH

local hero_relationship_data = gamemanager.GetData('hero_relationship_data')
local relationship_item = require('ui/illustration/view/relationship/relationship_item')

function t.Open(heroDataId)
  if uimanager.GetView(name) then
    return
  end
  uimanager.RegisterView(name,t)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.shipDataList = hero_relationship_data.GetDataBydataid(heroDataId)
  t.scrollItem = {}
  
  t.InitComponent()
  t.Refresh()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.InitComponent()
  local btnClose = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.ClickCloseBtnHandler)
  t.scrollContent = t.transform:Find('core/container/scrollview/viewport/content'):GetComponent(typeof(ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.goNoShipTip = t.transform:Find('core/container/text_noship_tip').gameObject
  t.transform:FindChild('core/text_title').gameObject:SetActive(false)
  t.transform:FindChild('core/text_title2').gameObject:SetActive(true)
end
function t.Refresh()
  local count = 0
  if t.shipDataList then
    count = #t.shipDataList
  end
  t.scrollContent:Init(count,false,0)
  t.goNoShipTip:SetActive(count == 0)
end
------------------------------------click event--------------------------------

function t.ClickCloseBtnHandler()
  uimanager.CloseView(name)
end

function t.OnResetItemHandler(go,index)
  local item = t.scrollItem[go]
  if item == nil then
    item = relationship_item.BindTransform(go.transform)
    t.scrollItem[go] = item
  end
  local shipData = t.shipDataList[index+1]
  item:SetRelationShipData(shipData)
end
return t