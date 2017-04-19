local t = {}
local PREFAB_PATH = 'ui/ranking/ranking_view'

local UI_RANKING_1 = "ui_paihang_1";
local UI_RANKING_2 = "ui_paihang_2";
local UI_RANKING_3 = "ui_paihang_3";

local function_open_model = gamemanager.GetModel('function_open_model')

 function t.Open(rankType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)  
  t.BindDelegate()
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.InitComponent() 
  t.curToggleIndex = 0
  t.isFirstEnter = true
  t.toggles[rankType]:GetComponent(typeof(Toggle)).isOn = true
  t.ClickToggleHandler(t.toggles[rankType].gameObject)
  
end

function t.Close() 
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  t.ui_ranking_1 = nil
  t.ui_ranking_2 = nil
  t.ui_ranking_3 = nil
end

function t.BindDelegate()
  gamemanager.GetModel('ranking_model').UpdateRankingDelegate:AddListener(t.UpdateRankingByProtocol)
end

function t.UnbindDelegate()
  gamemanager.GetModel('ranking_model').UpdateRankingDelegate:RemoveListener(t.UpdateRankingByProtocol)
end

function t.InitComponent()
  t.core = t.transform:Find('core')
  t.canvas = t.transform:GetComponent(typeof(UnityEngine.Canvas))
  t.scrollContent = t.transform:Find('core/ranking_list/ranking_list_scroll_view/view_port/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.contentItemPrefab:SetActive(false)
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.btn_back =  t.transform:Find('core/btn_back'):GetComponent(typeof(Button))
  t.btn_back.onClick:AddListener(t.Close)
  
  t.ui_ranking_1 = particle_util.CreateParticle('effects/prefabs/'..UI_RANKING_1,t.canvas.sortingLayerName,t.canvas.sortingOrder)
  t.ui_ranking_1:SetActive(false)
  t.ui_ranking_1.transform.localPosition = Vector3(0,0,0)
  t.ui_ranking_1.transform:SetParent(t.core,false)
  
  t.ui_ranking_2 = particle_util.CreateParticle('effects/prefabs/'..UI_RANKING_2,t.canvas.sortingLayerName,t.canvas.sortingOrder)
  t.ui_ranking_2:SetActive(false)
  t.ui_ranking_2.transform.localPosition = Vector3(0,0,0)
  t.ui_ranking_2.transform:SetParent(t.core,false)
  
  t.ui_ranking_3 = particle_util.CreateParticle('effects/prefabs/'..UI_RANKING_3,t.canvas.sortingLayerName,t.canvas.sortingOrder)
  t.ui_ranking_3:SetActive(false)
  t.ui_ranking_3.transform.localPosition = Vector3(0,0,0)
  t.ui_ranking_3.transform:SetParent(t.core,false)
  
  local ranking_type_list = t.transform:Find('core/ranking_type_list')
  t.toggles = {}
  t.toggles[1] = ranking_type_list:Find('toggle_capacity')
  t.toggles[2] = ranking_type_list:Find('toggle_gold')
  t.toggles[3] = ranking_type_list:Find('toggle_honour')
  t.toggles[4] = ranking_type_list:Find('toggle_arena')
  t.toggles[5] = ranking_type_list:Find('toggle_world_tree')
  --t.toggles[6] = ranking_type_list:Find('toggle_consortia')
  t.txt_role_info = t.transform:Find('core/ranking_list/txt_role_info'):GetComponent(typeof(Text))
  t.txt_role_info_tips = t.transform:Find('core/ranking_list/txt_role_info_tips'):GetComponent(typeof(Text))
  t.img_tips = t.transform:Find('core/ranking_list/img_tips'):GetComponent(typeof(Image))
  t.img_capacity_mine = t.transform:Find('core/ranking_list/img_capacity_mine'):GetComponent(typeof(Image))
  t.txt_capacity_mine = t.transform:Find('core/ranking_list/txt_capacity_mine'):GetComponent(typeof(Text))
  for k,v in ipairs(t.toggles) do
    local trigger = v.transform:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    trigger.onClick:AddListener(t.ClickToggleHandler)
  end
end

function t.RegenerateFreeRankingItems (playAnimation,list)
  t.ui_ranking_1:SetActive(false)
  t.ui_ranking_1.transform:SetParent(t.core,false)
  t.ui_ranking_2:SetActive(false)
  t.ui_ranking_2.transform:SetParent(t.core,false)
  t.ui_ranking_3:SetActive(false)
  t.ui_ranking_3.transform:SetParent(t.core,false)
  
  t.rankingItemList = list
  table.sort(t.rankingItemList,t.sort)
  t.scrollContent:Init(#t.rankingItemList, playAnimation, 0)
end

function t.sort(x,y)
  if x.rankNo < y.rankNo then
    return true
  end
  return false
end

function t.OnResetItemHandler(gameObject,index)
  local img_ranking = gameObject.transform:Find('img_ranking'):GetComponent(typeof(Image))
  local img_ranking_bg = gameObject.transform:Find('img_ranking_bg'):GetComponent(typeof(Image))
  local txt_ranking = gameObject.transform:Find('txt_ranking'):GetComponent(typeof(Text))
  local img_head_icon = gameObject.transform:Find('img_head_icon'):GetComponent(typeof(Image))
  local img_capacity = gameObject.transform:Find('img_capacity'):GetComponent(typeof(Image))
  local txt_name = gameObject.transform:Find('txt_name'):GetComponent(typeof(Text))
  --local txt_info = gameObject.transform:Find('txt_info'):GetComponent(typeof(Text))
  local txt_capacity = gameObject.transform:Find('txt_capacity'):GetComponent(typeof(Text))
  local txt_vipLv = gameObject.transform:Find('txt_vip'):GetComponent(typeof(Text))
  local rankingItem = t.rankingItemList[index + 1]
  --btn  查看玩家信息
  local btnCheck = img_head_icon.transform:GetComponent(typeof(Button))
  btnCheck.onClick:RemoveAllListeners()
  btnCheck.onClick:AddListener(function()
     gamemanager.GetCtrl('friend_controller').RoleInfoLookUpReq( rankingItem.id)
    
    end)
if t.curToggleIndex == 1 then
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.capacity'),rankingItem.combat)
    txt_capacity.text = tostring(rankingItem.combat)
    img_capacity.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_dps')
  elseif t.curToggleIndex == 2 then
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.gold'),rankingItem.gold)  
    txt_capacity.text = tostring(rankingItem.gold)       
    img_capacity.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_coin_small')
  elseif t.curToggleIndex == 3 then
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.honour'),rankingItem.honor) 
    txt_capacity.text = tostring(rankingItem.honor)   
    img_capacity.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_honor_small')
  elseif t.curToggleIndex == 4 then
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.arena'),rankingItem.combat)
    txt_capacity.text = tostring(rankingItem.combat)
    img_capacity.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_dps')
  elseif t.curToggleIndex == 5 then 
    local layerOfWorldTree = LuaInterface.LuaCsTransfer.GetLayerOfWorldTree(rankingItem.worldTreeDungeon)
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.world_tree'),layerOfWorldTree)
    txt_capacity.text = tostring(layerOfWorldTree)
    img_capacity.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_worldtree')
  --elseif t.curToggleIndex == 6 then 
    --txt_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.consortia'),rankingItem.combat)
  end
  if(rankingItem.rankNo == 1) then
    img_ranking.gameObject:SetActive(true)
    img_ranking_bg.gameObject:SetActive(false)
    txt_ranking.gameObject:SetActive(false)
    img_ranking.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_1st')
    t.ui_ranking_1.transform:SetParent(img_ranking.transform,false)
    t.ui_ranking_1:SetActive(true)
  elseif(rankingItem.rankNo == 2) then
    img_ranking.gameObject:SetActive(true)
    img_ranking_bg.gameObject:SetActive(false)
    txt_ranking.gameObject:SetActive(false)
    img_ranking.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_2st')
    t.ui_ranking_2.transform:SetParent(img_ranking.transform,false)
    t.ui_ranking_2:SetActive(true)
  elseif(rankingItem.rankNo ==3 ) then
    img_ranking.gameObject:SetActive(true)
    txt_ranking.gameObject:SetActive(false)
    img_ranking_bg.gameObject:SetActive(false)
    img_ranking.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_3st')
    t.ui_ranking_3.transform:SetParent(img_ranking.transform,false)
    t.ui_ranking_3:SetActive(true)
  else
    img_ranking.gameObject:SetActive(false)
    img_ranking_bg.gameObject:SetActive(true)
    txt_ranking.gameObject:SetActive(true)
    txt_ranking.text =tostring(rankingItem.rankNo)
  end
  --print (rankingItem.headNo)
  img_head_icon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(rankingItem.headNo))
  txt_name.text = string.format(LocalizationController.instance:Get('ui.ranking_view.txt_name'),rankingItem.roleName,rankingItem.lv)
  txt_vipLv.text = rankingItem.vipLv
  --txt_capacity.text = tostring(rankingItem.combat)
end

function t.Refresh(rankingInfo)
  t.RegenerateFreeRankingItems(t.isFirstEnter,rankingInfo.rankingList)
  t.isFirstEnter = false
  local myvalue = rankingInfo.combat
  if t.curToggleIndex == 1 then
    if(rankingInfo.currRankNo == 0) then
      t.txt_role_info.text = LocalizationController.instance:Get('ui.ranking_view.lost')
    else
      t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_capacity'),rankingInfo.currRankNo)
    end
    t.img_capacity_mine.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_dps')
  elseif t.curToggleIndex == 2 then
    if(rankingInfo.currRankNo == 0) then
      t.txt_role_info.text = LocalizationController.instance:Get('ui.ranking_view.lost')
    else
      t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_gold'),rankingInfo.currRankNo)    
    end
    t.img_capacity_mine.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_coin_small')
  elseif t.curToggleIndex == 3 then
    if(rankingInfo.currRankNo == 0) then
      t.txt_role_info.text = LocalizationController.instance:Get('ui.ranking_view.lost')
    else
      t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_honour'),rankingInfo.currRankNo)   
    end
    t.img_capacity_mine.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_honor_small')
  elseif t.curToggleIndex == 4 then
    if(rankingInfo.currRankNo == 0) then
      t.txt_role_info.text = LocalizationController.instance:Get('ui.ranking_view.lost')
    else
      t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_arena'),rankingInfo.currRankNo)
    end
    t.img_capacity_mine.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_dps')
  elseif t.curToggleIndex == 5 then 
    if(rankingInfo.currRankNo == 0) then
      t.txt_role_info.text = LocalizationController.instance:Get('ui.ranking_view.lost')
    else
      t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_world_tree'),rankingInfo.currRankNo)
    end
    myvalue = LuaInterface.LuaCsTransfer.GetLayerOfWorldTree(rankingInfo.combat)
    t.img_capacity_mine.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_worldtree')
  --elseif t.curToggleIndex == 6 then 
    --t.txt_role_info.text = string.format(LocalizationController.instance:Get('ui.ranking_view.role_consortia'),rankingInfo.currRankNo)
    --t.txt_role_info_tips.text = tostring(rankinkInfo.lastRankNo)
  end
  t.txt_capacity_mine.text = tostring(myvalue)
    
  if(rankingInfo.lastRankNo == 0) then
     t.img_tips.gameObject:SetActive(false)
     t.txt_role_info_tips.gameObject:SetActive(false)
  else    
    local ranking_up = rankingInfo.lastRankNo - rankingInfo.currRankNo
    if(ranking_up > 0) then
      t.img_tips.gameObject:SetActive(true)
      t.txt_role_info_tips.gameObject:SetActive(true)
      t.txt_role_info_tips.text = tostring(ranking_up)
    else
      t.img_tips.gameObject:SetActive(false)
      t.txt_role_info_tips.gameObject:SetActive(false)
    end
  end
end
--------------------click event-----------------
function t.OnClickBackBtnHandler ()
  t.Close()
end

function t.ClickToggleHandler(gameObject)
  local index = 0
  for k,v in ipairs(t.toggles) do
    if v.gameObject == gameObject then
      index = k
      break
    end
  end
  if t.curToggleIndex == index then
    return
  end
  if index == 1 then
  
  elseif index == 2 then
  
  elseif index == 3 then
    
  elseif index == 4 then   
    if not function_open_model.IsFunctionOpen(FunctionOpenType.FightCenter_Arena,true) then
      t.toggles[index]:GetComponent(typeof(Toggle)).isOn = false
      t.toggles[t.curToggleIndex]:GetComponent(typeof(Toggle)).isOn = true  
      return end
  elseif index == 5 then 
    if not function_open_model.IsFunctionOpen(FunctionOpenType.FightCenter_WorldTree,true) then
      t.toggles[t.curToggleIndex]:GetComponent(typeof(Toggle)).isOn = true  
      t.toggles[index]:GetComponent(typeof(Toggle)).isOn = false
      return end
  --elseif index == 6 then 
  end
    
  t.curToggleIndex = index
  gamemanager.GetCtrl('ranking_controller').RankListReq(index)
end

------------------------update by protocol----------------- 

function t.UpdateRankingByProtocol(ranking_info)
  t.Refresh(ranking_info)
end

return t