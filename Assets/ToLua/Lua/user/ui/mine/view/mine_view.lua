local mine_view = {}
local mine_item_view = require('ui/mine/view/mine_item_view')

local PREFAB_PATH = 'ui/mine_battle/mine_battle_view'
local mineController = gamemanager.GetCtrl('mine_controller')
local mineModel = gamemanager.GetModel('mine_model')
local mineData = gamemanager.GetData('mine_data')
local vipData = gamemanager.GetData('vip_data')
local vipModel = gamemanager.GetModel('vip_model')
mine_view.m_aryMineItemGrid = {}
mine_view.m_tempMineItem = nil
mine_view.m_labCaptureNum = nil
mine_view.m_labRobNum = nil
mine_view.m_labState= nil
mine_view.m_labTime= nil
mine_view.m_labGoldNum = nil
mine_view.m_btnLook = nil
mine_view.m_aryMineItemObj = {}

function mine_view.Open ()
  print("打开主界面")
  uimanager.RegisterView(PREFAB_PATH,mine_view)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  mine_view.transform = gameObject:GetComponent(typeof(Transform)) 
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(mine_view.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.mine_view.mine_view_title'), mine_view.ClickCloseButtonHandler, false, true, true, false, false, false, false)
  mine_view.BindDelegate()
  mine_view.InitComp()
  mineController.GetMineMapReq()
end
function mine_view.ClickCloseButtonHandler()  
  --mine_view.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function mine_view.Close()
  print('Close mine_view')
  coroutine.stop(mine_view.UpdateCoroutine)
  mine_view.UnbindDelegate()
  mine_view.m_aryMineItemObj = {}
  mine_view.m_aryMineItemGrid = {}
end

function mine_view.InitComp()
  mine_view.m_tempMineItem = mine_view.transform:Find('core/mineitem')
  mine_view.m_labCaptureNum = mine_view.transform:Find('core/bottom/text_capture/text_num'):GetComponent(typeof(Text))
  mine_view.m_labRobNum = mine_view.transform:Find('core/bottom/text_rob/text_num'):GetComponent(typeof(Text))
  mine_view.m_labState = mine_view.transform:Find('core/bottom/text_state/text_num'):GetComponent(typeof(Text))
  mine_view.m_labTime = mine_view.transform:Find('core/bottom/text_state/text_time'):GetComponent(typeof(Text))
  
  mine_view.m_labGoldNum = mine_view.transform:Find('core/bottom/text_gold/text_num'):GetComponent(typeof(Text))
  mine_view.m_btnLook = mine_view.transform:Find('core/bottom/btn_look'):GetComponent(typeof(Button))
  for index =1,4 do
    local grid = mine_view.transform:Find(string.format("core/grid%d",index)):GetComponent(typeof(GridLayoutGroup))
    table.insert(mine_view.m_aryMineItemGrid,grid)
  end
  mine_view.m_btnLook.onClick:AddListener(mine_view.OnClickLook)
end

function mine_view.BindDelegate()
  mineModel.OnMapUpdateDelegate:AddListener(mine_view.RefreshMapUI)
  mineModel.OnSelfMineCloseDelegate:AddListener(mine_view.SetOwnInfo)
end

function mine_view.UnbindDelegate()
  mineModel.OnMapUpdateDelegate:RemoveListener(mine_view.RefreshMapUI)
  mineModel.OnSelfMineCloseDelegate:RemoveListener(mine_view.SetOwnInfo)
end

function mine_view.RefreshMapUI()
  mine_view.RefreshAllMine()
  mine_view.SetOwnInfo()
end

function mine_view.RefreshAllMine()
  local length = #mineModel.m_aryMineItems
  for k,v in pairs(mineModel.m_aryMineItems) do
    --1:(1-5) 2:(6-10) 3:(11-15) 4(16-20) 公式為（x-1）/5+1
    local parentIndex = math.floor((k-1)/5)+1
    if(mine_view.m_aryMineItemObj[v.mineNo] ~= nil) then
        mine_view.m_aryMineItemObj[v.mineNo]:SetMineInfo(v)
    else
      local mineObj = GameObject.Instantiate(mine_view.m_tempMineItem.gameObject)
      mineObj.transform:SetParent(mine_view.m_aryMineItemGrid[parentIndex].transform,false) 
      local mineItem = mine_item_view.NewByGameObject(mineObj)
      mine_view.m_aryMineItemObj[v.mineNo] = mineItem
      mineItem.onClick:AddListener(mine_view.OnClickMineItem)
      mineItem:SetMineInfo(v)
    end
  end
  mine_view.m_tempMineItem.gameObject:SetActive(false)
end
function mine_view.SetOwnInfo()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  mine_view.m_labCaptureNum.text =tostring(currentVIPData.plunder_occ-mineModel.m_selfMineInfo.occTime)
  mine_view.m_labRobNum.text = tostring(currentVIPData.plunder_num-mineModel.m_selfMineInfo.plunderTime)
  if mineModel.m_selfMineInfo.endTime ==-1 then
    mine_view.m_labState.text = LocalizationController.instance:Get(string.format("ui.mine_view.state_2"))
    mine_view.m_labTime.gameObject:SetActive(false)   
  elseif mineModel.m_selfMineInfo.endTime ==0 then
    mine_view.m_labState.text = LocalizationController.instance:Get(string.format("ui.mine_view.state_3"))
     mine_view.m_labTime.gameObject:SetActive(false)
  else
    mine_view.m_labState.text = LocalizationController.instance:Get(string.format("ui.mine_view.state_1"))
    mine_view.m_labTime.gameObject:SetActive(true)
    print('start mine_view')
    coroutine.stop(mine_view.UpdateCoroutine)
    mine_view.UpdateCoroutine = coroutine.start(mine_view.UpdateView) 
  end
  mine_view.m_labGoldNum.text = tostring(mineModel.m_selfMineInfo.award)
  
end
function mine_view.OnClickMineItem(itemInfo)
  mineModel.m_curMineInfo = itemInfo
  print("mineModel.m_selfMineInfo",mineModel.m_selfMineInfo)
  if mineModel.m_curMineInfo.mineNo == mineModel.m_selfMineInfo.ownMineNo then
    mineController.OpenSelfMineView()
  else
    mineController.OpenOtherMineView()
  end   
end
function mine_view.OnClickLook()
  mineModel.m_curMineInfo = mineModel.GetCurMineInfo(mineModel.m_selfMineInfo)
  if mineModel.m_curMineInfo == nil then
    gamemanager.GetCtrl('mine_controller').GetOwnInfoReq()
    do return end
  end
  mineController.OpenSelfMineView()
end
function mine_view.RefreshTime() 
  local time =mineModel.m_selfMineInfo.endTime - TimeController.instance.ServerTimeTicksSecond
  if time ~=0 then
     mine_view.m_labTime.text = "("..TimeUtil.FormatSecondToHour(time)..")"
  else
    mine_view.m_labTime.text =  LocalizationController.instance:Get("ui.mine_view.state_2")
  end
end
function mine_view.UpdateView()
  while(true) do
    mine_view.RefreshTime()
    coroutine.wait(1)
  end
end
return mine_view