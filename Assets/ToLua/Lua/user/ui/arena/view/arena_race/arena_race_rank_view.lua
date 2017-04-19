local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_race_rank_view'

local function Start()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.initEvent()
  
  t.myRankTransform = t.transform:Find('core/img_bg/img_my_rank')
  local scrollContent = t.transform:Find('core/img_bg/scrollrect/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  scrollContent:Init(20,true,0)
  
  
  
  local myRankItem = t.transform:Find('core/img_bg/img_my_rank')
  local myRankInfo = gamemanager.GetModel('arena_model').myRaceRankInfo
  t.initRankItemTransform(myRankItem, myRankInfo)
  
  
  
end

--
function t.initRankItemTransform(item,info)
  
  

  if info ~= nil then 
    local textRank = item:Find('text_rank'):GetComponent(typeof(Text))
    local textName = item:Find('text_name'):GetComponent(typeof(Text))
    local textPower = item:Find('text_power_num'):GetComponent(typeof(Text))
    
    textRank.text = info.rank
    textName.text = info.name
    textPower.text = info.power
    
  end
end

function t.initEvent()
  local btnClose = t.transform:Find('core/img_bg/btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.OnClickCloseBtnHandler)
end

--------------------click event---------------
function t.OnClickCloseBtnHandler()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.OnResetItemHandler(gameObject,index)
end


Start()
return t