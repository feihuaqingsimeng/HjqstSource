local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_intro_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_info = require('ui/consortia/model/consortia_info')

function t.Open (parent,consortiaView)
  local gameObject = GameObject.Instantiate(ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(parent,false)
  t.consortiaView = consortiaView
  t.topThreeConsortiaList = {}
  for i = 1,3 do
    t.topThreeConsortiaList[i] = consortia_info.New(i)
  end
  t.InitComponent()
  t.BindDelegate ()
  t.Refresh()
end

function t.Close()
  if t.transform then
    GameObject.Destroy(t.transform.gameObject)
    t.UnbindDelegate()
    t.transform = nil
  end
end

function t.BindDelegate ()  
end

function t.UnbindDelegate()
end

function t.InitComponent()
  local top_three_root = t.transform:Find('top_three_root')
  t.tranTopTree = {}
  for i = 1,3 do
    local tran = top_three_root:Find(i..'')
    t.tranTopTree[i] = {}
    t.tranTopTree[i].root = tran:Find('root')
    t.tranTopTree[i].imgMark = tran:Find('root/mark'):GetComponent(typeof(Image))
    t.tranTopTree[i].textLevel = tran:Find('root/text_lv'):GetComponent(typeof(Text))
    t.tranTopTree[i].textName = tran:Find('root/text_name'):GetComponent(typeof(Text))
  end
  t.textDes = t.transform:Find('intro/text_des'):GetComponent(typeof(Text))
  t.btnRank = t.transform:Find('btn_rank'):GetComponent(typeof(Button))
  t.btnRank.onClick:RemoveAllListeners()
  t.btnRank.onClick:AddListener(t.ClickRankHandler)
  t.btnEnter = t.transform:Find('btn_enter'):GetComponent(typeof(Button))
  t.btnEnter.onClick:RemoveAllListeners()
  t.btnEnter.onClick:AddListener(t.ClickEnterHandler)
  t.btnRegister = t.transform:Find('btn_register'):GetComponent(typeof(Button))
  t.btnRegister.onClick:RemoveAllListeners()
  t.btnRegister.onClick:AddListener(t.ClickRegisterHandler)
end

function t.Refresh()
  t.RefreshTopThreeConsortia()
  t.RefreshButtons ()
end

function t.RefreshTopThreeConsortia()
  local isSeasonStart = false
  if isSeasonStart then
    local consortia 
    for k,v in pairs(t.tranTopTree) do
      consortia = t.topThreeConsortiaList[k] 
      if consortia then
        v.root.gameObject:SetActive(true)
        v.imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(consortia.headNo)
        v.imgMark:SetNativeSize()
        v.textLevel.text = string.format('Lv %d',consortia.lv)
        v.textName.text = consortia.name
      else
        v.root.gameObject:SetActive(false)
      end
    end
  else
    for k,v in pairs(t.tranTopTree) do
      consortia = t.topThreeConsortiaList[k] 
      if consortia then
        v.root.gameObject:SetActive(true)
        v.imgMark.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_unknown_2')
        v.imgMark:SetNativeSize()
        v.textLevel.text = ''
        v.textName.text = LocalizationController.instance:Get('ui.consortia_view.battle.no_consortia_name')
      else
        v.root.gameObject:SetActive(false)
      end
    end
  end
end

function t.RefreshButtons ()
  t.btnRegister.gameObject:SetActive(consortia_model.IsConsortiaCreator() and not t.testIsRegistered)
end

-------------------------click event-------------------
--本赛季排名
function t.ClickRankHandler()
  if false then
    local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.battle.no_ranking_because_season_not_start_tips'))
    return
  end
  
  consortia_controller.OpenConsortiaBattleRankView()
end

--进入
function t.ClickEnterHandler()
  if not t.testIsRegistered then
    local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.battle.not_register_tips'))
    return
  end
  
  t.consortiaView.OpenBattleSecondPanel()
end

--报名
function t.ClickRegisterHandler()
  t.testIsRegistered = true
  t.Refresh()
end

return  t