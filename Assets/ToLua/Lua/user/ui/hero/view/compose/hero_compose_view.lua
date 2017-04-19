local t = {}
local PREFAB_PATH = 'ui/hero_compose/hero_compose_view'

local hero_controller = gamemanager.GetCtrl('hero_controller')
local game_model = gamemanager.GetModel('game_model')
local hero_model = gamemanager.GetModel('hero_model')
local equip_model = gamemanager.GetModel('equip_model')
local item_model = gamemanager.GetModel('item_model')

local hero_compose_data = gamemanager.GetData('hero_compose_data')
local item_data = gamemanager.GetData('item_data')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local common_item_icon = require('ui/common_icon/common_item_icon')
local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
local game_res_data = require('ui/game/model/game_res_data')
local common_reward_auto_destroy_view = require('ui/tips/view/reward_auto_destroy_tip_view')

function t.Open()
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.hero_combine_view.text_title"),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.scrollItems = {}
  t.selectComposeGameResData = nil
  t.composeHeroIcon = nil
  t.isMaterialEnough = false
  t.selectMaterialGRDList = {}
  t.InitComponent()
  t.InitScrollContent()
  t.Refresh()
  t.BindDelegate()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate()
  item_model.updateItemInfoListDelegate:AddListener(t.RefreshMaterial)
  hero_model.OnHeroComposeSuccessDelegate:AddListener(t.UpdateHeroComposeSuccessByProtocol)
  game_model.onUpdateBaseResourceDelegate:AddListener(t.RefreshCost)
end
function t.UnbindDelegate()
  item_model.updateItemInfoListDelegate:RemoveListener(t.RefreshMaterial)
  hero_model.OnHeroComposeSuccessDelegate:RemoveListener(t.UpdateHeroComposeSuccessByProtocol)
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.RefreshCost)
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('core/img_big_frame/scrollrect/content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  local left_part = t.transform:Find('core/left_part')
  t.tranCurIcon = left_part:Find('left_top/img_combine_bg')
  t.tranMaterialRoot = {}
  for i = 1,6 do
    local mat = {}
    mat.root = left_part:Find('left_top/img_hero_material_'..i)
    mat.textCount = mat.root:Find('text_count'):GetComponent(typeof(Text))
    t.tranMaterialRoot[i] = mat
  end
  t.imgBaseResIcon = left_part:Find('bottom/img_coin'):GetComponent(typeof(Image))
  t.textBaseRes = left_part:Find('bottom/img_coin/text_coin'):GetComponent(typeof(Text))
  t.btnCompose = left_part:Find('bottom/btn_combine'):GetComponent(typeof(Button))
  t.btnCompose.onClick:AddListener(t.ClickComposeBtnHandler)
end
function t.Refresh()
  t.RefreshCurrentComposeIcon()
  t.RefreshMaterial()
  t.RefreshCost()
end

function t.InitScrollContent()
  t.heroGameResDataList = {}
  t.heroGameResDataListCount = 0
  local index = 1
  for k,v in pairs(hero_compose_data.data) do
    t.heroGameResDataList[index] = v.hero
    index = index + 1
  end
  t.heroGameResDataListCount = index - 1
  t.selectComposeGameResData = t.heroGameResDataList[1]
  t.scrollContent:Init(t.heroGameResDataListCount,true,0)
end
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
function t.RefreshCurrentComposeIcon()
  
  if t.composeHeroIcon == nil then
    t.composeHeroIcon = common_hero_icon.CreateBig(t.tranCurIcon)
  end
  t.composeHeroIcon:SetGameResData(t.selectComposeGameResData,false)
  t.composeHeroIcon:AddRoleDesButton(false)
end
function t.RefreshMaterial()
  local composeData = hero_compose_data.GetDataByHeroId(t.selectComposeGameResData.id)
  local materials = composeData.need_material
  local mat
  t.isMaterialEnough = true
  for k,v in pairs(t.tranMaterialRoot) do
    if v.materialItem then
      GameObject.Destroy(v.materialItem.transform.gameObject)
      v.materialItem = nil
    end
    mat = materials[k]
    v.textCount.gameObject:SetActive(mat ~= nil)
    if mat  then
      local own = 0
      if mat.type == BaseResType.Hero then
        v.materialItem = common_hero_icon.New(v.root)
        local heroInfo = t.GetHeroMaterialList(mat.id)[1]
        if heroInfo then
          v.materialItem:SetRoleInfo(heroInfo,false)
          t.selectMaterialGRDList[k] = game_res_data.New(BaseResType.Hero,heroInfo.instanceID,0,0)
          own = 1
        else
          v.materialItem:SetGameResData(mat,false)
          own = 0
        end
        v.materialItem:AddRoleDesButton()
      elseif mat.type == BaseResType.Equipment then
        v.materialItem = common_equip_icon.New(v.root)
        local equipInfo = t.GetEquipMaterialList(mat.id)[1]
        if equipInfo then
          v.materialItem:SetEquipInfo(equipInfo)
          t.selectMaterialGRDList[k] = game_res_data.New(BaseResType.Equipment,equipInfo.id,0,0)
          own = 1
        else
          v.materialItem:SetGameResData(mat)
          own = 0
        end
        v.materialItem:AddEquipDesButton()
      elseif mat.type == BaseResType.Item then
        local itemInfo = item_model.GetItemInfoByItemID(mat.id)
        own = 0
        if itemInfo then
          t.selectMaterialGRDList[k] = mat
          own = itemInfo:Count() 
        end
        v.materialItem = common_item_icon.New(v.root)
        v.materialItem:SetGameResData(mat)
        v.materialItem:ShowCount(false)
        
      else
        t.selectMaterialGRDList[k] = mat
        own = t.GetBaseResourceValue(mat.type)
        v.materialItem = common_item_icon.New(v.root)
        v.materialItem:SetGameResData(mat)
        v.materialItem:ShowCount(false)
      end
      v.materialItem.onClick:AddListener(function(icon)
          t.ClickMaterialHandler(k)
        end)
      v.textCount.transform:SetAsLastSibling()
      if own >= mat.count then
        v.textCount.text = ui_util.FormatToGreenText(string.format('%d/%d',own,mat.count))
      else
        v.textCount.text = ui_util.FormatToRedText(string.format('%d/%d',own,mat.count))
        t.isMaterialEnough = false
      end
      
    end
    
  end
end

function t.RefreshCost()
  -- refresh money
  local composeData = hero_compose_data.GetDataByHeroId(t.selectComposeGameResData.id)
  t.imgBaseResIcon.sprite = ResMgr.instance:LoadSprite( ui_util.GetBaseResIconPath(composeData.need_cost.type))
  t.imgBaseResIcon:SetNativeSize()
  local ownBaseRes = game_model.GetBaseResourceValue(composeData.need_cost.type)
  if ownBaseRes >= composeData.need_cost.count then
    t.textBaseRes.text = ui_util.FormatToGreenText(composeData.need_cost.count)
  else
    t.textBaseRes.text = ui_util.FormatToRedText(composeData.need_cost.count)
  end
end
function t.GetEquipMaterialList(equipId)
  local equipList = equip_model.GetEquipmentInfoListByEquipId(equipId)
  table.sort(equipList, equip_model.CompareEquipmentByQualityAsc)
  return equipList
end
function t.GetHeroMaterialList(heroid)
  local tempList = hero_model.GetHeroInfoListByHeroDataID(heroid)
  table.sort(tempList,hero_model.CompareHeroByQualityAsc)
  return tempList
end
---------------------click event-------------
function t.OnClickBackBtnHandler()
  t.Close()
end
function t.ClickMaterialHandler(index)
  local composeData = hero_compose_data.GetDataByHeroId(t.selectComposeGameResData.id)
  local gameResData = composeData.need_material[index]
  if gameResData then
    LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(gameResData.type, gameResData.id,gameResData.star)
  end
  
end

function t.ResetItemHandler(go,index)
  local res = t.heroGameResDataList[index+1]
  if t.scrollItems[go] == nil then
    t.scrollItems[go] = common_hero_icon.NewByGameObject(go)
    t.scrollItems[go].onClick:AddListener(t.ClickItemHandler)
  end
  t.scrollItems[go]:SetGameResData(res,false)
  t.scrollItems[go]:AddRoleDesButton()
  if res.id == t.selectComposeGameResData.id then
    t.scrollItems[go]:SetSelect(true)
  else
    t.scrollItems[go]:SetSelect(false)
  end

end
function t.ClickItemHandler(heroIcon)
  if heroIcon.gameResData.id == t.selectComposeGameResData.id then
    return 
  end
  t.selectComposeGameResData = heroIcon.gameResData
  t.RefreshScrollContent()
  t.Refresh()
end
function t.ClickComposeBtnHandler()
  if not t.isMaterialEnough then
    common_error_tip_view.Open(LocalizationController.instance:Get('ui.hero_combine_view.text_not_enough_material'))
    return
  end
  local composeData = hero_compose_data.GetDataByHeroId(t.selectComposeGameResData.id)
  local ownBaseRes = game_model.GetBaseResourceValue(composeData.need_cost.type)
  if ownBaseRes < composeData.need_cost.count then
    local str = LocalizationController.instance:Get('ui.hero_combine_view.text_not_enough_baseRes')
    local name = LocalizationController.instance:Get(item_data.GetBasicResItemByType(composeData.need_cost.type).name)
    common_error_tip_view.Open(string.format(str,name))
    return
  end
 
  hero_controller.HeroComposeReq(composeData.id,t.selectMaterialGRDList)
end
----------------------update by protocol---------------------
function t.UpdateHeroComposeSuccessByProtocol(newHeroId)
  print('合成成功啦：id'..newHeroId)
  local heroInfo = hero_model.GetHeroInfo(newHeroId)
  common_reward_auto_destroy_view.Open(game_res_data.New(BaseResType.Hero,heroInfo.heroData.id,1,heroInfo.advanceLevel))
  
  t.Refresh()
end

return t