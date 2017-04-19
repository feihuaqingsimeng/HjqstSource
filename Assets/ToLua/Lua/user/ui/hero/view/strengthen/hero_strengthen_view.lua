local t = {}
local PREFAB_PATH = 'ui/hero_strengthen/hero_strengthen_view'

local game_model = gamemanager.GetModel('game_model')
local player_model = gamemanager.GetModel('player_model')
local hero_model = gamemanager.GetModel('hero_model')
local item_model = gamemanager.GetModel('item_model')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local common_error_tip = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tip_view  = require('ui/tips/view/auto_destroy_tip_view')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

local role_model = gamemanager.GetModel('role_model')
local hero_strengthen_provide_data = gamemanager.GetData('hero_strengthen_provide_data')
local hero_strengthen_need_data = gamemanager.GetData('hero_strengthen_need_data')
local player_controller = gamemanager.GetCtrl('player_controller')
local hero_controller = gamemanager.GetCtrl('hero_controller')
local global_data = gamemanager.GetData('global_data')
local aggr_extra_exp_data = gamemanager.GetData('aggr_extra_exp_data')

function t.Open(heroInstanceId)
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.sortType = 0
  t.isClickStrengthenBtn = false
  t.selectedMaterialHeroInfos = {}
  t.isReachMaxLevel = false
  t.strengthenAddLevel = 0
  t.heroInfoList = {}
  t.spendMoney = 0
  t.isPlayer = game_model.IsPlayer(heroInstanceId)
  if t.isPlayer then
    t.roleInfo = game_model.playerInfo
  else
    t.roleInfo = hero_model.GetHeroInfo(heroInstanceId)
  end
  --print('强化英雄：instanceid :',heroInstanceId,'modelId:',t.roleInfo.heroData.id,'strengthen:',t.roleInfo.strengthenLevel,'strengthenExp:',t.roleInfo.strengthenExp,'expPercent:',t.roleInfo:StrengthenExpPercent())
  t.scrollItems = {}
  
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.hero_strengthen_view.hero_strengthen_title"),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.InitComponent()
  --dropDown
  t.dropDownSort.options:Clear()
  t.dropDownSort.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.hero_strengthen_view.sort_ascending")))
  t.dropDownSort.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.hero_strengthen_view.sort_descending")))
  t.dropDownSort.value = 0
  
  t.Refresh()
  t.BindDelegate()
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  coroutine.stop(t.successCoroutine)
end

function t.BindDelegate()
  player_model.OnPlayerAggrSuccessDelegate:AddListener(t.OnStrengthenSuccessHandler)
  hero_model.onHeroStrengthenSuccessDelegate:AddListener(t.OnStrengthenSuccessHandler)
  game_model.onUpdateBaseResourceDelegate:AddListener(t.RefreshAttribute)
  item_model.updateItemInfoListDelegate:AddListener(t.RefreshAttribute)
end
function t.UnbindDelegate()
  player_model.OnPlayerAggrSuccessDelegate:RemoveListener(t.OnStrengthenSuccessHandler)
  hero_model.onHeroStrengthenSuccessDelegate:RemoveListener(t.OnStrengthenSuccessHandler)
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.RefreshAttribute)
  item_model.updateItemInfoListDelegate:RemoveListener(t.RefreshAttribute)
end

function t.InitComponent()
  
  t.rootCanvas = t.transform:GetComponent(typeof(UnityEngine.Canvas))
  
  local left_part = t.transform:Find('core/left_part')
  
  t.tranCurIcon = left_part:Find('img_main_avatar_frame')
  t.tranMaterialRoot = {}
  t.tranMaterialRoot[1] = left_part:Find('btn_selected_material_hero_0')
  t.tranMaterialRoot[2] = left_part:Find('btn_selected_material_hero_1')
  t.tranMaterialRoot[3] = left_part:Find('btn_selected_material_hero_2')
  t.tranMaterialRoot[4] = left_part:Find('btn_selected_material_hero_3')
  local attr_panel_root = left_part:Find('img_attributes_frame')
  t.sliderAddExp = attr_panel_root:Find('slider_add_exp'):GetComponent(typeof(Slider))
  t.sliderCurExp = attr_panel_root:Find('slider_current_exp'):GetComponent(typeof(Slider))
  t.textAddExp = attr_panel_root:Find('text_add_exp'):GetComponent(typeof(Text))
  t.textStrengthenLevelDes = attr_panel_root:Find('text_strengthen_level_des'):GetComponent(typeof(Text))
  t.textStrengthenMaxLevelTip = attr_panel_root:Find('text_strengthen_max_tip'):GetComponent(typeof(Text))
  t.tranAttrRoot = attr_panel_root:Find('attribute_root')
  t.goAttrPrefab = attr_panel_root:Find('attribute_tepmlate').gameObject
  t.goAttrPrefab:SetActive(false)
  t.textMoney = attr_panel_root:Find('text_coin'):GetComponent(typeof(Text))
  t.btnStrengthen = attr_panel_root:Find('btn_strengthen'):GetComponent(typeof(Button))
  t.btnStrengthen.onClick:AddListener(t.ClickStrengthenHandler)
  t.btnStrengthenAutoPutIn = attr_panel_root:Find('btn_strengthenAutoPut'):GetComponent(typeof(Button))
  t.btnStrengthenAutoPutIn.onClick:AddListener(t.ClickStrengthenAutoPutInHandler)
  t.dropDownSort = t.transform:Find('core/img_big_frame/Dropdown_sort'):GetComponent(typeof(Dropdown))
  t.dropDownSort.onValueChanged:AddListener(t.ClickChangeSortBtnHandler)
  t.scrollContent = t.transform:Find('core/img_big_frame/scrollrect/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.goNoneTip = t.transform:Find('core/img_big_frame/text_no_available_strengthen_material').gameObject
  
  --effect 
  t.goEffectZhuangbeiqianghua2 = t.transform:Find('core/ui_chui_root/ui_zhuangbeiqianghua2').gameObject
  t.goEffectZhuangbeiqianghua2:SetActive(false)
end
function t.Refresh()
  t.RegenerateHeroButtons(true)
  t.RefreshAttribute()
  t.RefreshStrengthenHeroIcon()
  t.RefreshMaterial()
end

function t.RegenerateHeroButtons(needInit)
  if needInit then
    --local infoList  = hero_model.GetNotInAnyTeamHeroInfoList()
    local infoList  = hero_model.GetAllHeroInfoList()
    --去除自己 and leve >1 ,strengthenLevel > 0
    t.heroInfoList = {}
    local index = 1
    for k,v in pairs(infoList) do
      if v.instanceID ~= t.roleInfo.instanceID then
        t.heroInfoList[index] = v
        index = index + 1
        
      end
    end
  end
  
  if t.sortType == 0 then--品质升序
    table.sort(t.heroInfoList,hero_model.CompareHeroAsMaterialByQualityAsc)
  else--品质降序
    table.sort(t.heroInfoList,hero_model.CompareHeroAsMaterialByQualityDesc)
  end
  local count = table.count(t.heroInfoList)
  
  t.goNoneTip:SetActive(count == 0)
  if needInit then
    t.scrollContent:Init(count,false,0)
  else
    t.RefreshScrollContent()
  end
  
  
end

function t.RefreshMaterial()
  for i = 1,4 do
    ui_util.ClearChildren( t.tranMaterialRoot[i],true)
    local heroInfo = t.selectedMaterialHeroInfos[i]
    if heroInfo ~= nil then
      local icon = common_hero_icon.New( t.tranMaterialRoot[i])
      icon:SetRoleInfo(heroInfo,false)
      icon.onClick:AddListener(t.ClickMaterialHandler)
    end
  end
end
function t.RefreshScrollContent()
  
 t.scrollContent:RefreshAllContentItems()
end

function t.RefreshStrengthenHeroIcon()
  if t.strengthenHeroIcon == nil then
    t.strengthenHeroIcon = common_hero_icon.CreateBig(t.tranCurIcon)
  end
  t.strengthenHeroIcon:SetRoleInfo(t.roleInfo,t.isPlayer)
  if t.isPlayer then
    t.strengthenHeroIcon:UsePetIcon()
  end
  
end
function t.RefreshAttribute()
  local addExp = 0
  local selectMaterialCount = 0
  local extraStrengthenExp = 0 -- 英雄强化经验将继承下来
  for k,v in pairs(t.selectedMaterialHeroInfos) do
    local data = hero_strengthen_provide_data.GetDataByStarAndQuality(v.advanceLevel,v.heroData.quality)
    if data ~= nil then
      addExp = addExp + data.exp_provide+aggr_extra_exp_data.GetExtraExp(v.heroData.id,v.advanceLevel)
    end
    selectMaterialCount = selectMaterialCount + 1
    extraStrengthenExp = extraStrengthenExp + hero_strengthen_need_data.GetTotalExp(v.strengthenLevel)+v.strengthenExp
  end
  --按比例继承经验
  extraStrengthenExp = math.floor(extraStrengthenExp * global_data.aggr_inherit_per)
  --level
  local curData = hero_strengthen_need_data.GetDataById(t.roleInfo.strengthenLevel)
  local nextData = curData
  if addExp ~= 0 then
    local curTotalExp = hero_strengthen_need_data.GetTotalExp(t.roleInfo.strengthenLevel)+t.roleInfo.strengthenExp+addExp
    nextData = hero_strengthen_need_data.GetDataByExp(curTotalExp)
  end
  
  t.strengthenAddLevel = 0
  t.isReachMaxLevel = true
  if curData ~= nil then
    if nextData == nil then
      t.strengthenAddLevel = hero_strengthen_need_data.LastNeedData().aggr_lv - t.roleInfo.strengthenLevel + 1
    else
      t.strengthenAddLevel = nextData.aggr_lv - t.roleInfo.strengthenLevel
      t.isReachMaxLevel = false
    end
  end
  local accountLimitData = hero_strengthen_need_data.GetLimitDataByAccountLevel(game_model.accountLevel)
  
  local totalLv = t.roleInfo.strengthenLevel + t.strengthenAddLevel
  --账号等级限制
  
  if totalLv >= accountLimitData.aggr_lv then
    totalLv = accountLimitData.aggr_lv
    t.strengthenAddLevel = totalLv-t.roleInfo.strengthenLevel
    t.isReachMaxLevel = true
  end
  --print('账号等级限制lv',accountLimitData.aggr_lv,'curLv',t.roleInfo.strengthenLevel,'totalLv',totalLv,'addExp',addExp,'reachMax',t.isReachMaxLevel)
  --先算金币 money ,addexp,slidervalue,textAddValue
  local moneyTotal,addExpPercent,sliderValue,textAddValue = t.CalcMoneyAndExp(t.roleInfo.strengthenLevel,totalLv,addExp,selectMaterialCount)
  t.spendMoney = moneyTotal
  local own = game_model.GetBaseResourceValue(BaseResType.Gold)
  if moneyTotal > own then
    t.textMoney.text = ui_util.FormatToRedText(tostring(moneyTotal))
  else
    t.textMoney.text = tostring(moneyTotal)
  end
  --------------强化过的英雄可增加额外经验-----------------------
  --
  if extraStrengthenExp ~= 0 then
    addExp = addExp + extraStrengthenExp
    local curTotalExp = hero_strengthen_need_data.GetTotalExp(t.roleInfo.strengthenLevel)+t.roleInfo.strengthenExp+addExp
    nextData = hero_strengthen_need_data.GetDataByExp(curTotalExp)
    if curData ~= nil then
      if nextData == nil then
        t.strengthenAddLevel = hero_strengthen_need_data.LastNeedData().aggr_lv - t.roleInfo.strengthenLevel + 1
      else
        t.strengthenAddLevel = nextData.aggr_lv - t.roleInfo.strengthenLevel
        t.isReachMaxLevel = false
      end
    end
    totalLv = t.roleInfo.strengthenLevel + t.strengthenAddLevel
      --账号等级限制
    if totalLv >= accountLimitData.aggr_lv then
      totalLv = accountLimitData.aggr_lv
      t.strengthenAddLevel = totalLv-t.roleInfo.strengthenLevel
      t.isReachMaxLevel = true
    end
    print('额外增加强化经验:',extraStrengthenExp)
    
    moneyTotal,addExpPercent,sliderValue,textAddValue = t.CalcMoneyAndExp(t.roleInfo.strengthenLevel,totalLv,addExp,selectMaterialCount)
  end
    print('账号等级限制lv',accountLimitData.aggr_lv,'curLv',t.roleInfo.strengthenLevel,'totalLv',totalLv,'addExp',addExp,'reachMax',t.isReachMaxLevel)
  --各种经验显示
  t.sliderCurExp.value = sliderValue
  t.sliderAddExp.value = addExpPercent
  t.textAddExp.text = string.format(LocalizationController.instance:Get('ui.hero_strengthen_view.strengthen_exp'),math.floor(textAddValue*100))
  --------------------------END------------------------------------
  --强化加几提示
  local stlv = role_model.GetStrengthenAddShowValue(totalLv)
  local strengthenColorName = role_model.GetStrengthenLevelColorName(totalLv)
  local limitStrengthenColorName = role_model.GetStrengthenLevelColorName(accountLimitData.aggr_lv)
  local limitStlv = role_model.GetStrengthenAddShowValue(accountLimitData.aggr_lv)
  local limitStage = hero_strengthen_need_data.GetDataById(accountLimitData.aggr_lv-1).color
  --if stlv == 0 then
    --t.textStrengthenLevelDes.text = string.format(LocalizationController.instance:Get("ui.hero_strengthen_view.strengthen_level"))
  --else
    t.textStrengthenLevelDes.text = string.format(LocalizationController.instance:Get("ui.hero_strengthen_view.strengthen_level2"),stlv)
  --end
  --账号等级限制强化提示
  if limitStlv == 0 then
    t.textStrengthenMaxLevelTip.text = string.format('%s%s',LocalizationController.instance:Get("ui.hero_strengthen_view.strengthen_maxLvTip"),role_model.FormatStrengthenLevelColor(limitStage))
  else
    t.textStrengthenMaxLevelTip.text = string.format('%s%s',LocalizationController.instance:Get("ui.hero_strengthen_view.strengthen_maxLvTip"),role_model.FormatStrengthenLevelColor(limitStage,'+'..limitStlv))
  end
  local strengthenNeedData = hero_strengthen_need_data.GetDataById(totalLv-1)
  if strengthenNeedData ~= nil then
    t.textStrengthenLevelDes.color = role_model.GetRoleNameColor(strengthenNeedData.color)
  else
    t.textStrengthenLevelDes.color = role_model.GetRoleNameColor(0)
  end
  
  
  --属性
  t.RefreshMainAttribute(t.strengthenAddLevel)
end

function t.RefreshMainAttribute()
  ui_util.ClearChildren(t.tranAttrRoot,false)
 -- t.goAttrPrefab
  local mainAttriList = role_model.CalcRoleMainAttributesList(t.roleInfo)

  local isMaxLevel = hero_strengthen_need_data.MaxLevel() == t.roleInfo.strengthenLevel
  local newRoleInfo = t.roleInfo:Clone()
  newRoleInfo.strengthenLevel = newRoleInfo.strengthenLevel + t.strengthenAddLevel
  local nextMainAttriList = role_model.CalcRoleMainAttributesList(newRoleInfo)
  
  for k,v in pairs(mainAttriList) do
    local go = GameObject.Instantiate(t.goAttrPrefab)
    go:SetActive(true)
    local tran = go:GetComponent(typeof(Transform))
    tran:SetParent(t.tranAttrRoot,false)
    local textName = tran:Find('text_title'):GetComponent(typeof(Text))
    local textValue = tran:Find('text_value'):GetComponent(typeof(Text))
    local textAdd = tran:Find('text_add'):GetComponent(typeof(Text))
    textName.text = v:GetName()
    textValue.text = v:GetValueString()
    if isMaxLevel then
      textAdd.text = ''
    else
      local add = math.floor(nextMainAttriList[k].value-v.value)
      if add <= 0 then
        textAdd.text = ""
      else
        textAdd.text = string.format('(+%d)',add)
      end
    end
  end
end

function t.CalcMoneyAndExp( curlevel, nextLevel, expTotal,selectMaterialCount)
  local expTempTotal = expTotal
  local moneyTotal = 0
  local partMoney = 0
  local nextLevelPercent = 0
  
  for i = curlevel ,nextLevel do
    local needData = hero_strengthen_need_data.GetDataById(i)
    if needData == nil then
      needData = hero_strengthen_need_data.LastNeedData()
    end
    if i == nextLevel then
      partMoney = math.min( expTempTotal ,needData.exp_need) * needData.gold_need
      moneyTotal = moneyTotal + partMoney
      nextLevelPercent = expTempTotal/needData.exp_need
      if nextLevelPercent > 1 then
        nextLevelPercent = 1
      end
    elseif i == curlevel then
      local exp = needData.exp_need - t.roleInfo.strengthenExp
      partMoney = exp * needData.gold_need
      moneyTotal = moneyTotal + partMoney
      expTempTotal = expTempTotal - exp
    else
      partMoney = needData.exp_need * needData.gold_need
      moneyTotal = moneyTotal + partMoney
      expTempTotal = expTempTotal - needData.exp_need
    end
     --print('CalcMoneyAndExp','partMoney:'..partMoney,'moneyTotal:'..moneyTotal,'nextLevelPercent:'..nextLevelPercent,'expTempTotal:'..expTempTotal)
  end
  --money
  if expTotal ~= 0 then
    moneyTotal = moneyTotal
  else
    moneyTotal = 0
  end
  
  --slide exp
  local needData = hero_strengthen_need_data.GetDataById(t.roleInfo.strengthenLevel)
  local percent = 0
  local addPercent = 0
  local sliderValue = 0
  local textAddValue = 0
  if needData ~= nil then
    percent = t.roleInfo:StrengthenExpPercent()
    addPercent = percent + expTotal/needData.exp_need
  end
  if expTotal == 0 and percent == 1 then
    sliderValue = percent
  elseif addPercent >= 1 then
    addPercent = nextLevelPercent
    sliderValue = 0
  else
    
    sliderValue = percent
  end
    --exp
  if expTotal == 0 then
    textAddValue = percent
  else
    textAddValue = addPercent
    
  end
  --print('[return]','money:'..moneyTotal,'percent:'..percent,'addPercent:'..addPercent,'nextLevelPercent:'..nextLevelPercent,'slidervalue:'..sliderValue,'textaddValue:'..textAddValue)
  return moneyTotal,addPercent,sliderValue,textAddValue
end

--设置选择的材料
function t.SetSelectMaterialInfo(heroInfo)
  for i = 1,4  do
    if t.selectedMaterialHeroInfos[i] == nil then
      t.selectedMaterialHeroInfos[i] = heroInfo
      return
    end
  end
end

function t.SetSelectMaterialInfoByIndex(index,heroInfo)
  t.selectedMaterialHeroInfos[index] = heroInfo
end
function t.SetSelectMaterialInfoById(oldInstanceId,newInfo)
  for k,v in pairs(t.selectedMaterialHeroInfos) do
    if v ~= nil and v.instanceID == oldInstanceId then
      t.selectedMaterialHeroInfos[k] = newInfo
    end
  end
end


--------------------click event---------------
function t.OnClickBackBtnHandler()
  t.Close()
end
--点击强化
function t.ClickStrengthenHandler()
  if t.isClickStrengthenBtn then
    print('已经点击强化啦~o~,成功动画还没跑完耶')
    return 
  end
  local isMaxLevel = hero_strengthen_need_data.MaxLevel() == t.roleInfo.strengthenLevel
  if isMaxLevel then
    common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.arrival_max_level'))
    return
  end
  --material
  local materialHeroIDList = {}
  for k,v in pairs(t.selectedMaterialHeroInfos) do
    materialHeroIDList[k] = v.instanceID
  end
  if table.count(materialHeroIDList) == 0 then
    common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.material_not_enough'))
    return
  end
  --money
  if not game_model.CheckBaseResEnoughByType(BaseResType.Gold,t.spendMoney) then
    return
  end
  
  if t.CheckMaterialHighStar(materialHeroIDList) then
    return
  end
  if t.CheckMaterialWearEquip(materialHeroIDList) then
    
    return
  end
   t.ConfirmStrengthenHandler(materialHeroIDList)
  
end
--高星级确认
function t.CheckMaterialHighStar(materialHeroIDList)
  local heroInfo = nil
  local hasHighStar = false
  for k,v in pairs(materialHeroIDList) do
    heroInfo = hero_model.GetHeroInfo(v)
    if heroInfo.advanceLevel >= 4  then
      hasHighStar = true
      break
    end
  end

  if hasHighStar then
    
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.confirm_high_star_strengthen_tip'),function()
        if not t.CheckMaterialWearEquip(materialHeroIDList) then
          t.ConfirmStrengthenHandler(materialHeroIDList)
        end
    end)
  end
  return hasHighStar
end
--检测材料是否穿装备 ，返回：true有,false没
function t.CheckMaterialWearEquip(materialHeroIDList)
  local isWear = false
  local heroInfo = nil
  for k,v in pairs(materialHeroIDList) do
    heroInfo = hero_model.GetHeroInfo(v)
    if heroInfo:IsWearEquipment() then
      isWear = true
      break
    end
  end
  if isWear then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.confirm_wear_equip_strengthen_tip'),function()
        t.ConfirmStrengthenHandler(materialHeroIDList)
    end)
  end
  return isWear
end
function t.ConfirmStrengthenHandler(materialHeroIDList)
  if t.isPlayer then
    player_controller.PlayerAggrReq (t.roleInfo.instanceID, materialHeroIDList)
  else
    hero_controller.HeroAggrReq(t.roleInfo.instanceID,materialHeroIDList)
  end
  t.isClickStrengthenBtn = true
end

--点击一键放入
function t.ClickStrengthenAutoPutInHandler()
  if t.isClickStrengthenBtn then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.pleaseHoldOnTip'))
    return 
  end
  --local count = table.count(t.heroInfoList)
  local count = 0
  for k, v in ipairs(t.heroInfoList) do
    if v:CanBeUsedAsMaterial () then
      count = count + 1
    end
  end
  if count == 0 then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.no_available_strengthen_material'))
    return
  end
  if table.count(t.selectedMaterialHeroInfos) == 4 then
    return
  end
  
  if t.isReachMaxLevel then
      if hero_strengthen_need_data.MaxLevel() > t.roleInfo.strengthenLevel+t.strengthenAddLevel then
        local str = LocalizationController.instance:Get("ui.hero_strengthen_view.maxExpNeedLevel")
        local limitData = hero_strengthen_need_data.GetLimitDataByAccountLevel(game_model.accountLevel)
        auto_destroy_tip_view.Open(string.format(str,hero_strengthen_need_data.GetAccountLevelByStrengthenLevel(limitData.aggr_lv+1)))
      else
        auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.hero_strengthen_view.maxExp"))
      end
      
      return
    end
  
  local slotCount = 4
  if count < 4 then
    slotCount = count
  end
  for i = 1,slotCount do
    t.SetSelectMaterialInfoByIndex(i,t.heroInfoList[i])
  end
  t.RefreshAttribute()
  t.RefreshMaterial()
  t.RefreshScrollContent()
end

function t.OnResetItemHandler(go,index)
  if t.scrollItems[go] == nil then
    t.scrollItems[go] = common_hero_icon.NewByGameObject(go)
    t.scrollItems[go].onClick:AddListener(t.ClickHeroItemHandler)
  end
  local heroInfo = t.heroInfoList[index+1]
  t.scrollItems[go]:SetRoleInfo(heroInfo,false)
  local isSelect = false
  for k,v in pairs(t.selectedMaterialHeroInfos) do
    if v == heroInfo then
      isSelect = true
      break
    end
  end
  t.scrollItems[go]:SetSelect(isSelect)
  

  local cantEatMaskTransform = t.scrollItems[go].transform:Find('img_cant_eat_mask')
  if not heroInfo:CanBeUsedAsMaterial() then
    if cantEatMaskTransform == nil then
      cantEatMaskTransform = GameObject('img_cant_eat_mask').transform
      cantEatMaskTransform:SetParent(t.scrollItems[go].transform, false)
      cantEatMaskTransform.localPosition = Vector3(0, 0, 0)
      local cantEatMaskImage = cantEatMaskTransform.gameObject:AddComponent(typeof(Image))
      cantEatMaskImage.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/ui_items_02')
      cantEatMaskImage.color = Color.New(0, 0, 0, 0.7)
      cantEatMaskImage:SetNativeSize()
    end
    t.scrollItems[go].transform:Find('img_cant_eat_mask').gameObject:SetActive(true)
  else
    if cantEatMaskTransform ~= nil then
      t.scrollItems[go].transform:Find('img_cant_eat_mask').gameObject:SetActive(false)
    end
  end
end
function t.ClickMaterialHandler(heroIcon)
  t.SetSelectMaterialInfoById(heroIcon.roleInfo.instanceID,nil)
  t.RefreshAttribute()
  t.RefreshMaterial()
  t.RefreshScrollContent()
end
function t.ClickHeroItemHandler(heroIcon)
  if t.isClickStrengthenBtn then
    return
  end
  
  if not heroIcon.roleInfo:CanBeUsedAsMaterial() then
    local formation_model = gamemanager.GetModel('formation_model')
    local hero_model = gamemanager.GetModel('hero_model')
    if heroIcon.roleInfo.isLocked then
      common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.hero_locked_tip'))
    elseif formation_model.IsHeroInAnyTeam(heroIcon.roleInfo.instanceID) then
      common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.hero_in_formation_tip'))
    elseif hero_model.IsHeroInRelations(heroIcon.roleInfo.instanceID) then
      common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.hero_in_relations_tip'))
    elseif hero_model.IsHeroInExploring(heroIcon.roleInfo.instanceID) then
      common_error_tip.Open(LocalizationController.instance:Get('ui.hero_strengthen_view.hero_in_exploring_tip'))
    end
    return
  end
  
  if heroIcon.isSelect then
    heroIcon:SetSelect(false)
    
    t.SetSelectMaterialInfoById(heroIcon.roleInfo.instanceID,nil)
  else 
    if table.count(t.selectedMaterialHeroInfos) == 4 then
      return
    end
    if t.isReachMaxLevel then
      if hero_strengthen_need_data.MaxLevel() > t.roleInfo.strengthenLevel+t.strengthenAddLevel then
        local str = LocalizationController.instance:Get("ui.hero_strengthen_view.maxExpNeedLevel")
        local limitData = hero_strengthen_need_data.GetLimitDataByAccountLevel(game_model.accountLevel)
        auto_destroy_tip_view.Open(string.format(str,hero_strengthen_need_data.GetAccountLevelByStrengthenLevel(limitData.aggr_lv+1)))
      else
        auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.hero_strengthen_view.maxExp"))
      end
      
      return
    end
    
    t.SetSelectMaterialInfo(heroIcon.roleInfo)
    heroIcon:SetSelect(true)
  end
  t.RefreshAttribute()
  t.RefreshMaterial()
end
function t.ClickChangeSortBtnHandler(index)
  t.sortType = index
  t.RegenerateHeroButtons(false)
end
--------------------update by protocol--------------------
function t.OnStrengthenSuccessHandler(isCrit)
  print('强化成功啦，isCrit',isCrit)
  Observers.Facade.Instance:SendNotification(string.format('%s::%s', PREFAB_PATH, 'OnStrengthenSuccess'))
  t.successCoroutine = coroutine.start(t.StrengthenSuccessRefreshCoroutine)
end
function t.StrengthenSuccessRefreshCoroutine()
  t.RegenerateHeroButtons(true)
  coroutine.wait(0.01)
  
  local materialSlots = {}
  for k,v in pairs(t.selectedMaterialHeroInfos) do
    materialSlots[k] = v
  end
  --砸锤特效
  AudioController.instance:PlayAudio( "heroAggr",false, 0)
  t.goEffectZhuangbeiqianghua2:SetActive(false)
  t.goEffectZhuangbeiqianghua2:SetActive(true)
  coroutine.wait(0.4)
  --强化材料特效
  local meterialEffectTime = 0.5
  local index = 0
  local effect = nil
  for k,v in pairs(materialSlots) do
    effect = particle_util.CreateParticle('effects/prefabs/qianghua',t.rootCanvas.sortingLayerName,t.rootCanvas.sortingOrder)
    effect.transform:SetParent(t.tranMaterialRoot[k].parent,false)
    effect.transform.localPosition = t.tranMaterialRoot[k].localPosition
    GameObject.Destroy(effect,1)
  end
  coroutine.wait(meterialEffectTime)
  --clear 材料
  t.selectedMaterialHeroInfos = {}
  t.RefreshMaterial()
  t.RefreshStrengthenHeroIcon()
  --聚集粒子特效到头像上
  local particleMoveEffectTime = 0.6
  local moveLocation = t.tranCurIcon.localPosition
  local randomX = 0
  local randomY = 0
  for k,v in pairs(materialSlots) do
    local randomNum = UnityEngine.Random.Range(1,3)
    for i = 0,randomNum-1 do
      effect = particle_util.CreateParticle('effects/prefabs/dandao',t.rootCanvas.sortingLayerName,t.rootCanvas.sortingOrder)
      effect.transform:SetParent(t.tranMaterialRoot[k].parent,false)
      effect.transform.localPosition = t.tranMaterialRoot[k].localPosition
      randomX = UnityEngine.Random.Range(-150,150) + effect.transform.localPosition.x;
      randomY =  UnityEngine.Random.Range(-150,150) + effect.transform.localPosition.y;
      local flyLocation = Vector3(randomX,randomY,0)
      local v2 = flyLocation
      local v3 = Vector3((moveLocation.x+v2.x)/2,(moveLocation.y+v2.y)/2)
      local v = {}
      v[1] = effect.transform.localPosition
      v[2] = v3 
      v[3] = v2
      v[4] = moveLocation
      LeanTween.moveLocal(effect,v,particleMoveEffectTime):setDelay(0.03*i):setEase(LeanTweenType.easeInSine)
      GameObject.Destroy(effect,particleMoveEffectTime+0.1)
    end
  end
  coroutine.wait(particleMoveEffectTime)
  --升级特效
  if t.strengthenAddLevel ~= 0 then
     effect = particle_util.CreateParticle('effects/prefabs/shengji',t.rootCanvas.sortingLayerName,t.rootCanvas.sortingOrder)
  else
     effect = particle_util.CreateParticle('effects/prefabs/kapaixishou',t.rootCanvas.sortingLayerName,t.rootCanvas.sortingOrder)
  end
  effect.transform:SetParent(t.tranCurIcon.parent,false)
  effect.transform.localPosition = t.tranCurIcon.localPosition
  GameObject.Destroy(effect,1)
  --经验条特效
  effect = particle_util.CreateParticle('effects/prefabs/qianghua_jindutiao',t.rootCanvas.sortingLayerName,t.rootCanvas.sortingOrder)
  effect.transform:SetParent(t.sliderCurExp.transform.parent,false)
  effect.transform.localPosition = t.sliderCurExp.transform.localPosition
  GameObject.Destroy(effect,1)
  
  t.RefreshAttribute()
  t.isClickStrengthenBtn = false
end

return t