local t= {}
local PREFAB_PATH = 'ui/compose/equip_compose_view'
local composeController = gamemanager.GetCtrl('compose_controller')
local composeModel = gamemanager.GetModel('compose_model')
local equipModel = gamemanager.GetModel('equip_model')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local common_item_icon = require('ui/common_icon/common_item_icon')
local equipComposeData = gamemanager.GetData('equip_compose_data')
local confirm_error_tip_view = require('ui/tips/view/common_error_tips_view')
local gameModel = gamemanager.GetModel('game_model')
local item_model = gamemanager.GetModel('item_model')

t.m_dropChoseColor=nil
t.m_labGoldSuccess =nil
t.m_labMoneySuccess = nil
t.m_labGold =nil
t.m_labMoney =nil
t.m_btnGoldCompose =nil
t.m_btnMoneyCompose =nil
t.m_labCompose =nil
t.m_labHignCompose = nil
t.m_labPush = nil
t.m_labChoseQuality = nil
t.m_btnPush = nil
t.m_labEquipTitle =nil
t.m_scrollEquipContent = nil
t.m_objSuccessPanel =nil
t.m_objSuccessEquip = nil
t.m_btnSuccessClose =nil
t.m_labSuccessEquipName =nil
t.m_labSuccessEquipFight =nil
t.m_labSuccessEquipAtt =nil
t.m_labSuccessEquipAttNum =nil
t.m_imgComposeVagueEquip =nil

t.m_objFailPanel =nil
t.m_objFailItem = nil
t.m_btnSuccessClose =nil
t.m_objComposeEffect =nil

t.m_eComposeMode = nil
t.m_nLastChoseIndex = 0
t.m_curSuccessTargetObj =nil
t.m_aryEquipParent= {}
t.m_aryAllEquipInfo = {}
t.m_aryCurSelectEquipInfo = {}
t.m_aryTargetSelectEquip = {}
t.m_aryTargetSelectEquip = {}
t.m_aryEquipScrollItem = {}
t.m_lastEquipInfo = nil --上次点击的装备，用于判断是否点击相同的装备
t.m_curComposeEquipData = nil

t.ComposeType=
{
  Gold =0,
  Money =1
}
t.Quality=
{
  White =1,
  Green =2,
  Blue =3,
  Purple =4,
  Orange= 5
}
t.Result=
{
  Fail =0,
  Success =1
}
t.ComposeMode=
{
  Single =0,
  All =1
}

function t.Open ()
  print("合成主界面")
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform)) 
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_view_title'), t.ClickCloseButtonHandler, false, true, true, false, false, false, false)
  t.BindDelegate()
  t.m_nLastChoseIndex =0
  t.m_aryEquipParent= {}
  t.InitComp()
  t.UpdateEquipList()
end
function t.ClickCloseButtonHandler()  
  t.Close()
end
function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.InitComp()
  t.m_dropChoseColor = t.transform:Find("core/chosequality"):GetComponent(typeof(Dropdown))
  t.m_labGoldSuccess = t.transform:Find("core/gold/bg1/text_num"):GetComponent(typeof(Text))
  t.m_labMoneySuccess = t.transform:Find("core/money/bg1/text_num"):GetComponent(typeof(Text))
  t.m_labGold = t.transform:Find("core/gold/text_num"):GetComponent(typeof(Text))
  t.m_labMoney = t.transform:Find("core/money/text_num"):GetComponent(typeof(Text))
  t.m_labCompose = t.transform:Find("core/gold/text"):GetComponent(typeof(Text))
  t.m_labHignCompose = t.transform:Find("core/money/text"):GetComponent(typeof(Text))
  t.m_labChoseQuality = t.transform:Find("core/chose_text"):GetComponent(typeof(Text))
  t.m_labPush = t.transform:Find("core/btn_push/text"):GetComponent(typeof(Text))
  t.m_labEquipTitle = t.transform:Find("core/img_title/text_title"):GetComponent(typeof(Text))
  t.m_btnPush = t.transform:Find("core/btn_push"):GetComponent(typeof(Button))
  t.m_btnGoldCompose = t.transform:Find("core/gold/btn_compose"):GetComponent(typeof(Button))
  t.m_btnMoneyCompose = t.transform:Find("core/money/btn_compose"):GetComponent(typeof(Button))
  t.m_scrollEquipContent = t.transform:Find("core/equipscorllow/Viewport/Content"):GetComponent(typeof(ScrollContent))
  t.m_objSuccessPanel = t.transform:Find("core/compse_success").gameObject
  t.m_objSuccessEquip = t.transform:Find("core/compse_success/equip_info/common_equipment_icon").gameObject
  t.m_btnSuccessClose =  t.transform:Find("core/compse_success/btn_close"):GetComponent(typeof(Button))
  t.m_labSuccessEquipName = t.transform:Find("core/compse_success/equip_info/text_name"):GetComponent(typeof(Text))
  t.m_labSuccessEquipFight = t.transform:Find("core/compse_success/equip_info/img_fight/text_num"):GetComponent(typeof(Text))
  t.m_labSuccessEquipAtt = t.transform:Find("core/compse_success/equip_info/text_att"):GetComponent(typeof(Text))
  t.m_labSuccessEquipAttNum = t.transform:Find("core/compse_success/equip_info/text_att/text_num"):GetComponent(typeof(Text))
  t.m_objFailPanel = t.transform:Find("core/compose_fail").gameObject
  t.m_objFailItem = t.transform:Find("core/compose_fail/item").gameObject
  t.m_btnFailClose =t.transform:Find("core/compose_fail/btn_close"):GetComponent(typeof(Button))
  t.m_imgComposeVagueEquip = t.transform:Find("core/equip/img_equip6/img_target"):GetComponent(typeof(Image))
  t.m_objComposeEffect = t.transform:Find("core/ui_hecheng").gameObject
  

  t.m_objComposeEffect:SetActive(false)
  t.m_objSuccessPanel:SetActive(false)
  t.m_objFailPanel:SetActive(false)
  t.m_imgComposeVagueEquip.gameObject:SetActive(true)
  t.m_eComposeMode = t.ComposeMode.Single
  t.m_scrollEquipContent.onResetItem:AddListener(t.OnResetItemHandler)
  for index=1,3 do
    t.m_dropChoseColor.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get(string.format("ui.equip_compose_view.equip_compose_view_color_%s",index))))  
  end
  for index =1,6 do
    local equipParent = t.transform:Find(string.format("core/equip/img_equip%s",index)):GetComponent(typeof(Image))
    table.insert(t.m_aryEquipParent,equipParent)
  end
  t.m_dropChoseColor.onValueChanged:AddListener(t.OnScrollColorChanged)
  t.m_curComposeEquipData = equipComposeData.GetDataByQuality(t.Quality.Green)
  t.SetStaticText()
  t.SetComposeView() 
  t.m_btnPush.onClick:AddListener(t.OnClickPush)
  t.m_btnGoldCompose.onClick:AddListener(t.OnClickCompose)
  t.m_btnMoneyCompose.onClick:AddListener(t.OnClickHighCompose)
  t.m_btnSuccessClose.onClick:AddListener(t.OnClickCloseSuccess)
  t.m_btnFailClose.onClick:AddListener(t.OnClickCloseFail)
end

function t.BindDelegate()
   composeModel.OnSuccessUpdateDelegate:AddListener(t.UpdateComposeResult)
   gameModel.onUpdateBaseResourceDelegate:AddListener(t.SetComposeView)
end

function t.UnbindDelegate()
  composeModel.OnSuccessUpdateDelegate:RemoveListener(t.UpdateComposeResult)
  gameModel.onUpdateBaseResourceDelegate:RemoveListener(t.SetComposeView)
end

function t.UpdateComposeResult(result,id)
  t.m_objComposeEffect:SetActive(false)
  t.m_objComposeEffect:SetActive(true)
  t.SetResultVagueEquipActive(false)
 
  
  t.UpdateCoroutine = coroutine.start(t.UpdateView,result,id)
end

function t.UpdateView(result,id)
  t.UpdateEquipList()
  t.ClearTargetEquip() 
  if result == t.Result.Fail then
    --[[local itemModel = gamemanager.GetModel('item_model')
    local itemInfo = itemModel.GetItemInfoByInstanceID(id)
    --]]
    --策划说碎片没配临时失败送装备，正式用上面的屏蔽代码
    local equipInfo = equipModel.GetEquipmentInfoByInstanceID(id)
    coroutine.wait(0.2)
    t.SetSuccessEquipInfo(equipInfo)
    coroutine.wait(1)
    t.SetFailPanelInfo(equipInfo)
  elseif result == t.Result.Success then
    local equipInfo = equipModel.GetEquipmentInfoByInstanceID(id)
    coroutine.wait(0.2)
    t.SetFailItemInfo(equipInfo)
    coroutine.wait(1)
    t.SetSuccessPanelInfo(equipInfo)
  end
end

function t.UpdateEquipList()
  t.m_aryCurSelectEquipInfo ={}
  t.m_aryAllEquipInfo = composeModel.GetDicToList(equipModel.GetFreeEquipmentInfoList())
  table.sort(t.m_aryAllEquipInfo,composeModel.EquipCompare)
  local length = composeModel.GetDicCount(t.m_aryAllEquipInfo)
  t.m_scrollEquipContent:Init(length,false,0)
end


function t.OnClickPush()
  local curQuality = t.Quality.Green
  t.ClearTargetEquip()
  t.ClearResultEquip()
  for quality=t.Quality.Green,t.Quality.Orange do
    local num = composeModel.GetQualityCount(quality,t.m_aryAllEquipInfo)
    if num >=5 then
      curQuality = quality
      if curQuality >=t.Quality.Orange or curQuality <=t.Quality.White then
        confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_error_5'))
        do return end
      end
      --t.NewEquipList(quality)
      break
    end
  end
  local num =1
  for index =1,#t.m_aryAllEquipInfo do
     if t.m_aryAllEquipInfo[index].data.quality ==curQuality then
        if num <=5 then
          t.m_aryAllEquipInfo[index].isSelect =true 
          table.insert(t.m_aryCurSelectEquipInfo,t.m_aryAllEquipInfo[index])
          t.m_curComposeEquipData =equipComposeData.GetDataByQuality(t.m_aryAllEquipInfo[index].data.quality)
          t.m_lastEquipInfo = t.m_aryAllEquipInfo[index]
          num =num +1
        else
          t.m_aryAllEquipInfo[index].isSelect =false         
        end
     end
  end
  t.SetResultVagueEquipActive(true)
  t.SetResultQuality(t.m_curComposeEquipData.quality+1) 
  t.SetChoseText(t.m_curComposeEquipData.quality-2)
  for index =1,#t.m_aryCurSelectEquipInfo do
     t.SetTargetComposeEquip(t.m_aryCurSelectEquipInfo[index],t.m_aryCurSelectEquipInfo[index].isSelect)
  end
  t.m_eComposeMode = t.ComposeMode.All
 
  t.SetComposeView()  
  t.m_scrollEquipContent:RefreshAllContentItems()
end
function t.ClearTargetEquip()
  t.m_aryCurSelectEquipInfo = {}
  for index=1, 5 do
     local equip=t.m_aryEquipParent[index].transform:Find("common_equip_icon")
     if equip ~=nil then
        GameObject.DestroyImmediate(equip.gameObject)
     end   
  end
end
function t.SetChoseText(index)
  t.m_dropChoseColor.captionText.text =  LocalizationController.instance:Get(string.format("ui.equip_compose_view.equip_compose_view_color_%s",index+1))
  if t.m_nLastChoseIndex ~= index then
    t.m_nLastChoseIndex = index
  end
  t.m_dropChoseColor.value =  index 
end
function t.OnClickCompose()
  t.SendComposeCS(t.ComposeType.Gold)
end
function t.OnClickHighCompose()
  t.SendComposeCS(t.ComposeType.Money)
end
function t.SendComposeCS(costType)
   if #t.m_aryCurSelectEquipInfo <5 then
     confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_error_3')) 
     do return end
  end
  local equipTab= {}
  for k,v in pairs(t.m_aryCurSelectEquipInfo) do
     table.insert(equipTab,v.id)
  end  
  composeController.EquipComposeReq(t.m_curComposeEquipData.quality,equipTab,costType)
end


function t.SetStaticText()
  t.m_labCompose.text =  LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_compose")  
  t.m_labHignCompose.text =  LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_high_compose")
  t.m_dropChoseColor.captionText.text =  LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_color_1")
  t.m_labPush.text =  LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_push")
  t.m_labChoseQuality.text = LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_chose_normal")
  t.OnScrollColorChanged (0)
end
function t.SetResultQuality(quality) 
  t.m_aryEquipParent[6].sprite = ui_util.GetRoleQualityFrameSprite (quality)
end
function t.OnScrollColorChanged (index)
  if t.m_nLastChoseIndex ~= index then --此时的状态如果满足则是点击选择按钮变得，并不是点击装备或一件选择导致的
    t.ClearTargetEquip()
    t.ClearResultEquip()
    t.m_nLastChoseIndex = index   
    t.m_curComposeEquipData = equipComposeData.GetDataById(index+1) 
    t.NewEquipList(t.m_curComposeEquipData.quality)
    t.SetComposeView() 
    t.SetResultVagueEquipActive(true)
    t.SetResultQuality(t.Quality.White) 
  end  
end
function t.ClearResultEquip()
  if t.m_curSuccessTargetObj ~= nil then
    GameObject.Destroy(t.m_curSuccessTargetObj)
    t.m_curSuccessTargetObj =nil
  end 
end
function t.SetComposeView()  
  t.m_labGoldSuccess.text =string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_success"),t.m_curComposeEquipData.gold_chance)
  t.m_labMoneySuccess.text= string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_success"),t.m_curComposeEquipData.diamond_chance)
  local ownGold = gameModel.GetBaseResourceValue(BaseResType.Gold)
  local ownMoney= gameModel.GetBaseResourceValue(BaseResType.Diamond)
  if ownGold<t.m_curComposeEquipData.need_gold_cost.count then   
    t.m_labGold.text =ui_util.FormatToRedText( string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_gold"),t.m_curComposeEquipData.need_gold_cost.count))
  else
    t.m_labGold.text =ui_util.FormatToGreenText(string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_gold"),t.m_curComposeEquipData.need_gold_cost.count))
  end
  if ownMoney<t.m_curComposeEquipData.need_diamond_cost.count then
  t.m_labMoney.text =ui_util.FormatToRedText( string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_money"),t.m_curComposeEquipData.need_diamond_cost.count))
  else
  t.m_labMoney.text =ui_util.FormatToGreenText( string.format(LocalizationController.instance:Get("ui.equip_compose_view.equip_compose_view_money"),t.m_curComposeEquipData.need_diamond_cost.count))
  end
end

function t.SetResultVagueEquipActive(isActive)
  print("isActive:",isActive)
  t.m_imgComposeVagueEquip.gameObject:SetActive(isActive)
end
function t.OnResetItemHandler(go,index)
  if t.m_aryEquipScrollItem[go] == nil then
      t.m_aryEquipScrollItem[go]= common_equip_icon.NewByGameObject(go)
      t.m_aryEquipScrollItem[go].onClick:AddListener(t.OnClickEquip)
  end
  
  local equipItem = t.m_aryEquipScrollItem[go]
  local equipInfo = t.m_aryAllEquipInfo[index+1] 
  equipItem:SetEquipInfo(equipInfo)
  equipItem:AddEquipDesButton(true)
  local isHave =false
  for index =1, #t.m_aryCurSelectEquipInfo do
    local selectInfo = t.m_aryCurSelectEquipInfo[index]
      if equipInfo.id == selectInfo.id then
          isHave = true
          break
      end
  end
  equipItem:SetSelect(isHave)
end
function t.OnClickEquip(go) 
  t.ClearResultEquip()
  local confirm_error_tip_view = require('ui/tips/view/common_error_tips_view')
  if go.isSelect then
    print("[OnClickEquip]#t.m_aryCurSelectEquipInfo:",#t.m_aryCurSelectEquipInfo)
    for k,v in pairs(t.m_aryCurSelectEquipInfo) do
        if go.equipInfo.id == v.id then
          print("go.equipInfo.id:",go.equipInfo.id)
          table.remove(t.m_aryCurSelectEquipInfo,k)
          break
        end
    end
    print("[OnClickEquip]#t.m_aryCurSelectEquipInfo:",#t.m_aryCurSelectEquipInfo)
    if #t.m_aryCurSelectEquipInfo ==0 then
      --t.SetResultVagueEquipActive(true)
      t.SetResultQuality(t.Quality.White) 
    else
     -- t.SetResultVagueEquipActive(false)
    end
  else
    if #t.m_aryCurSelectEquipInfo >=5 then  
        confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_error_1'))
        do return end
    end
    if #t.m_aryCurSelectEquipInfo ~=0 and t.m_lastEquipInfo ~=nil and go.equipInfo.data.quality ~= t.m_lastEquipInfo.data.quality then
        confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_error_2'))
        do return end
    end
    if go.equipInfo.data.quality >=t.Quality.Orange or go.equipInfo.data.quality <=t.Quality.White then
        confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.equip_compose_view.equip_compose_error_4'))
        do return end
    end
    t.m_curComposeEquipData = equipComposeData.GetDataByQuality(go.equipInfo.data.quality)   
    t.SetResultVagueEquipActive(true)
    t.SetResultQuality(t.m_curComposeEquipData.quality+1) 
    t.m_lastEquipInfo = go.equipInfo
    table.insert(t.m_aryCurSelectEquipInfo,go.equipInfo)
  end
  
  go.isSelect =  not go.isSelect
  go:SetSelect(go.isSelect)
  t.SetTargetComposeEquip(go.equipInfo,go.isSelect)
  t.SetComposeView()  
  t.m_eComposeMode = t.ComposeMode.Single
  t.SetChoseText(t.m_curComposeEquipData.quality-2)
  t.m_scrollEquipContent:RefreshAllContentItems()
end
function t.NewEquipList(quality)
  t.m_aryAllEquipInfo = composeModel.GetNewSortList(quality,t.m_aryAllEquipInfo)
  t.m_scrollEquipContent:RefreshAllContentItems()
end
function t.SetTargetComposeEquip(equipInfo,isSelect)
  if isSelect then
    for index=1, 5 do
        local equip=t.m_aryEquipParent[index].transform:Find("common_equip_icon")
        if equip ==nil then
          equip = common_equip_icon.New(t.m_aryEquipParent[index].transform)   
          equip:SetEquipInfo(equipInfo)
          equip:AddEquipDesButton(true)
          equip.onClick:AddListener(t.OnClickEquip)
          equip:SetSelect(true)
          equip:ShowSelectMark(false)
          equip.transform.name = "common_equip_icon"
          table.insert(t.m_aryTargetSelectEquip,equip)
          break
        end   
    end
  else
    for index= #t.m_aryTargetSelectEquip,1,-1 do
        if t.m_aryTargetSelectEquip[index].equipInfo.id == equipInfo.id then
          local equip = table.remove(t.m_aryTargetSelectEquip,index)
          GameObject.Destroy(equip.transform.gameObject)
          break
        end   
    end    
  end
end

function t.OnClickCloseSuccess()
   t.m_objSuccessPanel:SetActive(false)
end
function t.OnClickCloseFail()
  t.m_objFailPanel:SetActive(false)
end

function t.SetSuccessEquipInfo(equipInfo)
  local equipResultCompose = common_equip_icon.New(t.m_aryEquipParent[6].transform)
  equipResultCompose:SetEquipInfo(equipInfo)
  equipResultCompose:AddEquipDesButton(true)
  t.m_curSuccessTargetObj = equipResultCompose.transform.gameObject
end
function t.SetSuccessPanelInfo(equipInfo)
  t.m_objSuccessPanel:SetActive(true)
  local successEquip =common_equip_icon.NewByGameObject(t.m_objSuccessEquip)
  successEquip:SetEquipInfo(equipInfo)
  t.m_labSuccessEquipName.text = LocalizationController.instance:Get(equipInfo.data.name)
  t.m_labSuccessEquipName.color =  ui_util.GetEquipQualityColor (equipInfo.data)
  t.m_labSuccessEquipFight.text = tostring(equipInfo:Power())
  local equipAtt = equipInfo:GetTotalBaseAttr()
  t.m_labSuccessEquipAttNum.text = equipAtt:GetValueString()
  t.m_labSuccessEquipAtt.text =equipAtt:GetName()
end

function t.SetFailItemInfo(itemInfo)
  --[[local item = common_item_icon.New(t.m_aryEquipParent[6].transform)
    item:SetItemInfo(itemInfo) 
    --]]
  --策划说碎片没配临时失败送装备，正式用上面的屏蔽代码
  local equipTargetCompose = common_equip_icon.New(t.m_aryEquipParent[6].transform)
  equipTargetCompose:SetEquipInfo(itemInfo)
  equipTargetCompose:AddEquipDesButton(true)
  t.m_curSuccessTargetObj = equipTargetCompose.transform.gameObject   
end
function t.SetFailPanelInfo(itemInfo)
  t.m_objFailPanel:SetActive(true) 
  local failEquip = common_equip_icon.New(t.m_objFailItem.transform)
  failEquip:SetEquipInfo(itemInfo)
end
return t