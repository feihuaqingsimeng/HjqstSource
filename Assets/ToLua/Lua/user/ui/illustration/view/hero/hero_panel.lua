local t = {}
t.__index = t

local PREFAB_PATH = 'ui/illustrated_handbook/hero/hero_panel'

local illustration_model = gamemanager.GetModel('illustration_model')
local illustration_content_view = require('ui/illustration/view/illustration_content_view')

function t.Open(parent,useSaveState)
  
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject.transform
  t.transform:SetParent(parent,false)
  
  t.InitComponent()
  t.useSaveState = useSaveState
  t.scrollItems = {}
  
  return t
  
end

function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.transform = nil
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.onResetItemHandler)
  t.scrollBar = t.transform:Find('Scroll View/scrollbar'):GetComponent(typeof(Scrollbar))
  t.goHeroPrefab = t.transform:Find('Scroll View/illustratedHeroButton').gameObject
  t.goHeroPrefab:SetActive(false)
end

function t.Refresh(needAction)
  local count = illustration_model.currentSelectList.Count
  if t.useSaveState then
    t.scrollContent:Init(count,false,0)
    t.scrollContent:ScrollToPosition(illustration_model.scrollPercent)
  else
    t.scrollContent:Init(count,needAction,0.2)
  end
  t.useSaveState = false
end

function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end

function t.GetScrollBarValue()
  return t.scrollBar.value
end

--------------------click event-------------------------------------

function t.onResetItemHandler(go,index)
  local item = t.scrollItems[go]
  if not item then
    item = illustration_content_view.BindTransform(go.transform,t.goHeroPrefab)
    t.scrollItems[go] = item
  end
  local scrollItemData = illustration_model.currentSelectList:Get(index)
  if scrollItemData.collectNum == nil then
    item:SetData(scrollItemData.titleStr,scrollItemData.illustrationInfoList,IllustrationType.hero)
  else
    item:SetCollectAttrData(scrollItemData.collectNum,scrollItemData.roleAttrDictionay)
  end
  
end


return t