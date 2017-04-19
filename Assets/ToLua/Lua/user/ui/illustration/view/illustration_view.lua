local t = {}
local PREFAB_PATH = 'ui/illustrated_handbook/illustrated_view_lua'
local name = PREFAB_PATH

local illustration_model = gamemanager.GetModel('illustration_model')
local illustration_ctrl = gamemanager.GetCtrl('illustration_ctrl')
local illustration_content_view = require('ui/illustration/view/illustration_content_view')
local hero_panel = require('ui/illustration/view/hero/hero_panel')
local equip_panel = require('ui/illustration/view/equip/equip_panel')
local item_panel = require('ui/illustration/view/item/item_panel')

--useSaveState:是否使用上次的页签和scroll的位置
function t.Open(useSaveState)
  if uimanager.GetView(name) then
    return
  end
  uimanager.RegisterView(name,t)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.useSaveState = useSaveState
  t.currentDropdownIndex = 0
  t.selectIllustrationType = nil
  t.isFirstEnter = true
  t.dropdownIdList = MList.New('number')
  t.InitComponent()
  t.BindDelegate()
  
  t.subPanel = nil
  --illustration_model.Clear()
  illustration_model.InitDataDictionary()
  --t.InitToggles()
  
  --illustration_ctrl.IllustrationReq()
  t.ChangeSubPanel(IllustrationType.hero)
end
function t.Close()
  print('illustration Close')
  illustration_model.selectIllustrationType = t.selectIllustrationType
  illustration_model.selectDropdownIndex = t.currentDropdownIndex
  illustration_model.scrollPercent = t.subPanel.GetScrollBarValue()
  t.UnbindDelegate()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  
  
end

function t.BindDelegate()
  illustration_ctrl.InitIllustrationDelegate:AddListener(t.InitIllustrationByProtocol)
  illustration_ctrl.UpdateIllustrationDelegate:AddListener(t.UpdateIllustrationByProtocol)
end
function t.UnbindDelegate()
  illustration_ctrl.InitIllustrationDelegate:RemoveListener(t.InitIllustrationByProtocol)
  illustration_ctrl.UpdateIllustrationDelegate:RemoveListener(t.UpdateIllustrationByProtocol)
end

function t.InitComponent()
  local root = t.transform:Find('core/root')
  t.btnClose = root:Find('top_root/btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.OnClickBackBtnHandler)
  t.textTitle = root:Find('top_root/text_name'):GetComponent(typeof(Text))
  t.imgTopHead = root:Find('top_root/img_head'):GetComponent(typeof(Image))
  t.imgBottomHead = root:Find('bottom_root/img_head'):GetComponent(typeof(Image))
  
  t.textCount = root:Find('bottom_root/text_count'):GetComponent(typeof(Text))
  --dropdown
  t.dropdownSelect = root:Find('bottom_root/Dropdown_select'):GetComponent(typeof(Dropdown))
  t.dropdownSelect.onValueChanged:AddListener(t.OnDropdownSelectChangedHandler)
  
  t.subPanelRoot = root:Find('sub_panel_root')
  t.btnIllustrationTable = {}
  t.btnIllustrationTable[1] = root:Find('bottom_root/btn/btn_hero_illus'):GetComponent(typeof(Button))
  t.btnIllustrationTable[1].onClick:AddListener(t.OnClickHeroIllusBtnHandler)
  t.btnIllustrationTable[2] = root:Find('bottom_root/btn/btn_equip_illus'):GetComponent(typeof(Button))
  t.btnIllustrationTable[2].onClick:AddListener(t.OnClickEquipIllusBtnHandler)
  t.btnIllustrationTable[3] = root:Find('bottom_root/btn/btn_item_illus'):GetComponent(typeof(Button))
  t.btnIllustrationTable[3].onClick:AddListener(t.OnClickItemIllusBtnHandler)
end
function t.Refresh()
  
  if t.subPanel then
    illustration_model.UpdateSelectDropdownDic(t.selectIllustrationType,t.dropdownIdList:Get(t.currentDropdownIndex))
    t.subPanel.Refresh(t.isFirstEnter)
    t.UpdateCountText()
    t.isFirstEnter = false
    t.textTitle.text = LocalizationController.instance:Get('ui.illustration_view.select'..t.selectIllustrationType)
    local str = LocalizationController.instance:Get('ui.illustration_view.selectHead'..t.selectIllustrationType)
    t.imgBottomHead.sprite = ResMgr.instance:LoadSprite(str)
    t.imgTopHead.sprite = ResMgr.instance:LoadSprite(str)
  end
  
end
function t.RefreshScrollContent()
  t.subPanel.RefreshScrollContent()
end
--数量
function t.UpdateCountText()
  local totalCount = 0
  local hasCount = 0
  local roleInfo = nil
  for k,v in pairs(illustration_model.currentSelectList:GetDatas()) do
    local infoList = v.illustrationInfoList
    if infoList then
      totalCount = totalCount + infoList.Count
      for k1,v1 in pairs(infoList:GetDatas()) do
        if illustration_model.IsGotInIllustration(t.selectIllustrationType, v1.id) then
          hasCount = hasCount + 1
        end
      end
    end
    
  end
  t.textCount.text = string.format('%d/%d',hasCount,totalCount)
end
--drop down 初始化
function t.InitDropdownList()
  
  local titleDic = illustration_model.GetBigTitleStringDictionaryByIllustrationType(t.selectIllustrationType)
  local firstKey = 0
  local value = nil
  t.dropdownIdList:Clear()
  t.dropdownSelect.options:Clear()
  for index,key in ipairs(titleDic:GetSortKeysList()) do
    value = titleDic:Get(key)
    t.dropdownIdList:Add(key)
    t.dropdownSelect.options:Add(UnityEngine.UI.Dropdown.OptionData.New(value))
  end
  if t.useSaveState then
    t.currentDropdownIndex = illustration_model.selectDropdownIndex
  else
    t.currentDropdownIndex = 0
  end
  t.dropdownSelect.value = t.currentDropdownIndex
  t.dropdownSelect.captionText.text = titleDic:Get(t.dropdownIdList:Get(t.currentDropdownIndex))
end
function t.ChangeSubPanel(illustrationType)
  if t.selectIllustrationType == illustrationType then
    return
  end
  t.selectIllustrationType = illustrationType
  if t.useSaveState then
    t.selectIllustrationType = illustration_model.selectIllustrationType
  end
  if t.subPanel then
    t.subPanel.Close()
    t.subPanel = nil
  end
  illustration_model.selectIllustrationType = t.selectIllustrationType
  t.InitDropdownList()
  
  if t.selectIllustrationType == IllustrationType.hero then
    t.subPanel = hero_panel.Open(t.subPanelRoot,t.useSaveState)
  elseif t.selectIllustrationType == IllustrationType.equip then
    t.subPanel = equip_panel.Open(t.subPanelRoot,t.useSaveState)
  else 
    t.subPanel = item_panel.Open(t.subPanelRoot,t.useSaveState)
  end
  t.useSaveState = false
  t.Refresh()
  for k,v in pairs(t.btnIllustrationTable) do
    if k == t.selectIllustrationType then
      v.transform.localScale = Vector3(1.1,1.1,1.1)
      v.transform:Find('mark').gameObject:SetActive(false)
    else
      v.transform.localScale = Vector3(0.9,0.9,0.9)
      v.transform:Find('mark').gameObject:SetActive(true)
    end
  end
end
--------------------click event------------------------
function t.OnClickBackBtnHandler()
  uimanager.CloseView(name)
end
function t.OnDropdownSelectChangedHandler(index)
  print(index)
  t.currentDropdownIndex = index
  t.Refresh()
end
--点击英雄图鉴
function t.OnClickHeroIllusBtnHandler()
  t.ChangeSubPanel(IllustrationType.hero)
end
--点击装备图鉴
function t.OnClickEquipIllusBtnHandler()
  t.ChangeSubPanel(IllustrationType.equip)
end
--点击道具图鉴
function t.OnClickItemIllusBtnHandler()
  t.ChangeSubPanel(IllustrationType.item)
end
--------------------------------update by protocol---------------
function t.InitIllustrationByProtocol()
  t.ChangeSubPanel(IllustrationType.hero)
end
function t.UpdateIllustrationByProtocol()
  t.RefreshScrollContent()
  t.UpdateCountText()
end
return t