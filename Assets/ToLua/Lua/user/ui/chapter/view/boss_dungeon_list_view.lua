local t = {}
local PREFAB_PATH = 'ui/boss_dungeon_list/boss_dungeon_list_view'

local dungeon_model = gamemanager.GetModel('dungeon_model')
local boss_dungeon_item = require('ui/chapter/view/boss_dungeon_item')

function t.Open ()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  uimanager.RegisterView(PREFAB_PATH, t)
  
  t.BindDelegate ()
  t.InitComponent()
end

function t.InitComponent ()
  t.titleText = t.transform:Find('core/frame/text_boss_dungeon_list_title'):GetComponent(typeof(Text))
  
  local costItemInfo = gamemanager.GetModel('item_model').GetItemInfoWithoutNilByItemId(10138)
  t.costItemIconImage = t.transform:Find('core/frame/img_cost_item_icon'):GetComponent(typeof(Image))
  LuaInterface.LuaCsTransfer.GetItemDesButton(t.costItemIconImage.gameObject, costItemInfo.itemData.id, true)
  t.costItemCountText = t.transform:Find('core/frame/text_cost_item_count'):GetComponent(typeof(Text))
  t.RefreshCostItemInfo ()
  
  t.closeButton = t.transform:Find('core/frame/btn_close'):GetComponent(typeof(Button))
  t.closeButton.onClick:AddListener(t.ClickCloseButtonHandler)
  
  t.bossDungeonItems = {}
  t.bossDungeonInfoList = dungeon_model.GetDungeonInfoListByDungeonType(DungeonType.BossSpecies)
  t.scrollContent = t.transform:Find('core/frame/scroll_view/viewport/content'):GetComponent(typeof(ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.scrollContent:Init(table.count(t.bossDungeonInfoList), false, 0)
end

function t.BindDelegate ()
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:AddListener(t.OnItemInfoListUpdateDelegate)
end

function t.UnbindDelegate ()
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:RemoveListener(t.OnItemInfoListUpdateDelegate)
end

function t.RefreshCostItemInfo ()
  local costItemInfo = gamemanager.GetModel('item_model').GetItemInfoWithoutNilByItemId(10138)
  t.costItemIconImage.sprite = ResMgr.instance:LoadSprite(costItemInfo.itemData:IconPath())
  t.costItemCountText.text = costItemInfo.count
end

function t.OnResetItemHandler (gameObject, index)
  local bossDungeonItem = t.bossDungeonItems[gameObject]
  if bossDungeonItem == nil then
    bossDungeonItem = boss_dungeon_item.NewByGameObject(gameObject, t.bossDungeonInfoList[index + 1])
    t.bossDungeonItems[gameObject] = bossDungeonItem
  end
  bossDungeonItem:SetDungeonInfo(t.bossDungeonInfoList[index + 1])
end

function t.Close ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.ClickCloseButtonHandler ()
  t.Close()
end

function t.OnItemInfoListUpdateDelegate ()
  t.RefreshCostItemInfo()
  t.scrollContent:RefreshAllContentItems()
end

return t
